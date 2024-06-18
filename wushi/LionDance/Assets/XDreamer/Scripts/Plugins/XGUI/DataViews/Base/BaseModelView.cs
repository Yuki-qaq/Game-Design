using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.ComponentModel;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginXGUI.ViewControllers;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.DataViews.Base
{
    /// <summary>
    /// 基础模型视图：将视图与模型对象的字段、属性或方法进行绑定
    /// </summary>
    public abstract class BaseModelView : BaseViewController, IDropdownPopupAttribute, ITypeBinderGetter, IModelViewEvent, IModelKeyValueHost, IModelKeyValueHostModifier
    {
        #region Unity生命周期方法

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (this.XGetComponent(ref _modelToViewConverter)) { }
            if (this.XGetComponent(ref _viewToModelConverter)) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_modelType == EModelType.ScriptVariable)
            {
                Variable.onValueChanged += OnVariableValueChanged;
            }

            AddModelMainObjectEventListener(modelMainObject);

        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            Variable.onValueChanged -= OnVariableValueChanged;

            validModelToViewConverter = null;
            validViewToModelConverter = null;

            RemoveModelMainObjectEventListener();
        }

        /// <summary>
        /// 启用时尝试将模型数据刷新至视图
        /// </summary>
        protected virtual void Start()
        {
        }

        private float modelToViewDataUpdateTimeCounter = 0;

        private float viewToModelDataUpdateTimeCounter = 0;

        /// <summary>
        /// 更新函数
        /// </summary>
        protected virtual void Update()
        {
            if (CanModelToView())
            {
                if (_modelToViewDataUpdateRule == EDataUpdateRule.Timing)
                {
                    modelToViewDataUpdateTimeCounter += Time.deltaTime;
                    if (modelToViewDataUpdateTimeCounter >= _modelToViewDataUpdateIntervalTime)
                    {
                        modelToViewDataUpdateTimeCounter = 0;
                        ModelToViewIfValueChanged();
                    }
                }
                else if (_modelToViewDataUpdateRule == EDataUpdateRule.EveryFrame)
                {
                    ModelToViewIfValueChanged();
                }
            }

            if (CanViewToModel())
            {
                if (_viewToModelDataUpdateRule == EDataUpdateRule.Timing)
                {
                    viewToModelDataUpdateTimeCounter += Time.deltaTime;
                    if (viewToModelDataUpdateTimeCounter >= _viewToModelDataUpdateIntervalTime)
                    {
                        viewToModelDataUpdateTimeCounter = 0;
                        ViewToModelIfValueChanged();
                    }
                }
                else if (_viewToModelDataUpdateRule == EDataUpdateRule.EveryFrame)
                {
                    ViewToModelIfValueChanged();
                }
            }
        }

        /// <summary>
        /// 当销毁
        /// </summary>
        protected virtual void OnDestroy()
        {
            RemoveModelMainObjectEventListener();
        }

        #endregion

        #region 模型主对象

        private object _modelMainObjectForTriggerEvent;

        private void AddModelMainObjectEventListener(object modelMainObject)
        {
            RemoveModelMainObjectEventListener();
            this._modelMainObjectForTriggerEvent = modelMainObject;
            if (modelMainObject is IModelEventListener listener)
            {
                listener.AddModelEventListener(OnModelMainObjectTriggerEvent);
            }

            UpdateValidModelToViewConverter();

            UpdateValidViewToModelConverter();

            ModelToViewIfCan();
        }

        private void RemoveModelMainObjectEventListener()
        {
            if (this._modelMainObjectForTriggerEvent is IModelEventListener listener)
            {
                listener.RemoveModelEventListener(OnModelMainObjectTriggerEvent);
            }
        }

        /// <summary>
        /// 当模型主对象触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="modelEventArgs"></param>
        protected virtual void OnModelMainObjectTriggerEvent(object sender, ModelEventArgs modelEventArgs)
        {
            if (modelEventArgs is ModelPropertyChangedEventArgs modelPropertyChangedEventArgs && CheckMemberName(modelPropertyChangedEventArgs.name))
            {
                //如果是模型属性已变更事件、那么触发模型到视图；
                ModelToViewIfCanAndTrigger();
            }
        }

        /// <summary>
        /// 检查成员名称
        /// </summary>
        /// <param name="propertyName">属性名：支持属性名、字段名、键名等；</param>
        /// <returns></returns>
        protected bool CheckMemberName(string propertyName)
        {
            switch (_memberNameCheckRuleOnTrigger)
            {
                case EMemberNameCheckRule.Allow: return true;
                case EMemberNameCheckRule.Empty: return string.IsNullOrEmpty(propertyName);
                case EMemberNameCheckRule.Match: break;
                case EMemberNameCheckRule.Default:
                case EMemberNameCheckRule.EmptyOrMatch:
                    {
                        if (string.IsNullOrEmpty(propertyName)) return true;
                        break;
                    }
                default: return false;
            }

            //处理匹配
            switch (_modelType)
            {
                case EModelType.FieldPropertyMethodBinder:
                case EModelType.FieldPropertyMethodBinder_MainObject:
                    {
                        return propertyName.Contains(_fieldPropertyMethodBinder.memberName) || _fieldPropertyMethodBinder._propertyPathList._propertyPaths.Any(m => propertyName.Contains(m._memberName));
                    }
                case EModelType.ModelObjectKeyValue:
                    {
                        return _modelKey.TryGetValue(out var key) && propertyName.Contains(key);
                    }
            }
            return false;
        }

        /// <summary>
        /// 模型主对象
        /// </summary>
        public object modelMainObject
        {
            get
            {
                switch (_modelType)
                {
                    case EModelType.FieldPropertyMethodBinder: return _fieldPropertyMethodBinder.mainObject;
                    case EModelType.ModelObject:
                    case EModelType.ModelObjectKeyValue: return modelObject;
                    case EModelType.FieldPropertyMethodBinder_MainObject: return _fieldPropertyMethodBinder.mainObject;
                    case EModelType.UnityObject: return _unityObject;
                    default: return default;
                }
            }
            private set
            {
                switch (_modelType)
                {
                    case EModelType.ModelObject:
                    case EModelType.ModelObjectKeyValue:
                        {
                            AddModelMainObjectEventListener(value);
                            modelObject = value;
                            break;
                        }
                    case EModelType.FieldPropertyMethodBinder_MainObject:
                        {
                            AddModelMainObjectEventListener(value);
                            _fieldPropertyMethodBinder.mainObject = value;
                            break;
                        }
                    case EModelType.UnityObject:
                        {
                            AddModelMainObjectEventListener(value);
                            _unityObject = value as UnityEngine.Object;
                            break;
                        }
                    default:
                        {
                            AddModelMainObjectEventListener(default);
                            break;
                        }
                }
                ModelToViewIfCan();
            }
        }

        /// <summary>
        /// 属性路径
        /// </summary>
        public string propertyPath { get; private set; } = "";

        /// <summary>
        /// 设置模型主对象
        /// </summary>
        /// <param name="modelMainObject"></param>
        /// <param name="modelMainObjectHost"></param>
        /// <param name="parentPorpertyPath"></param>
        public void SetModelMainObject(object modelMainObject, UnityEngine.Object modelMainObjectHost, string parentPorpertyPath)
        {
            this.modelMainObject = modelMainObject;
            this.modelMainObjectHost = modelMainObjectHost;
            switch (_modelType)
            {
                case EModelType.FieldPropertyMethodBinder:
                case EModelType.FieldPropertyMethodBinder_MainObject:
                    {
                        var key = _fieldPropertyMethodBinder._memberName;
                        if (!string.IsNullOrEmpty(key))
                        {
                            this.propertyPath = string.IsNullOrEmpty(parentPorpertyPath) ? key : (parentPorpertyPath + "." + key);
                        }
                        break;
                    }
                case EModelType.ModelObjectKeyValue:
                    {
                        var key = _modelKey.GetValue();
                        if (!string.IsNullOrEmpty(key))
                        {
                            this.propertyPath = string.IsNullOrEmpty(parentPorpertyPath) ? key : (parentPorpertyPath + "." + key);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 模型父级
        /// </summary>
        public UnityEngine.Object modelMainObjectHost { get; private set; }

        List<IModelViewEvent> modelViewEvents = new List<IModelViewEvent>();

        /// <summary>
        /// 添加监听器
        /// </summary>
        /// <param name="modelViewEvent"></param>
        public void AddListener(IModelViewEvent modelViewEvent)
        {
            modelViewEvents.AddWithDistinct(modelViewEvent);
        }

        /// <summary>
        /// 移除监听器
        /// </summary>
        /// <param name="modelViewEvent"></param>
        public void RemoveListener(IModelViewEvent modelViewEvent)
        {
            modelViewEvents.Remove(modelViewEvent);
        }

        void CallChildModelToView(object modelValue, object viewValue)
        {
            var modelViewEventData = new ModelViewEventData(this, modelValue, viewValue, EModelViewDataLinkMode.ModelToView);

            CallFinished(modelViewEventData);
            foreach (var item in modelViewEvents)
            {
                item.OnChildViewToModel(modelViewEventData);
            }
        }

        /// <summary>
        /// 当子级模型到视图
        /// </summary>
        /// <param name="modelViewEventData"></param>
        public virtual void OnChildModelToView(ModelViewEventData modelViewEventData)
        {
            var data = new ModelViewEventData(modelViewEventData, this);
            CallFinished(data);
            foreach (var item in modelViewEvents)
            {
                item.OnChildModelToView(data);
            }
        }

        /// <summary>
        /// 调用子级视图到模型：即通知父级有子级执行了视图到模型的操作；
        /// </summary>
        /// <param name="modelValue"></param>
        /// <param name="viewValue"></param>
        protected void CallChildViewToModel(object modelValue, object viewValue)
        {
            ModelViewEventData modelViewEventData = new ModelViewEventData(this, modelValue, viewValue, EModelViewDataLinkMode.ViewToModel);

            CallFinished(modelViewEventData);
            foreach (var item in modelViewEvents)
            {
                item.OnChildViewToModel(modelViewEventData);
            }
        }

        /// <summary>
        /// 当子级视图到模型
        /// </summary>
        /// <param name="modelViewEventData"></param>
        public virtual void OnChildViewToModel(ModelViewEventData modelViewEventData)
        {
            var cloneData = new ModelViewEventData(modelViewEventData, this);
            CallFinished(cloneData);
            foreach (var item in modelViewEvents)
            {
                item.OnChildViewToModel(cloneData);
            }
        }

        #endregion

        #region 模型<==>视图

        /// <summary>
        /// 模型到视图数据连接模式
        /// </summary>
        [Name("模型到视图数据连接模式")]
        [EnumPopup]
        public EModelViewDataLinkMode _modelViewDataLinkMode = EModelViewDataLinkMode.Both;

        /// <summary>
        /// 能否将模型数据更新到视图
        /// </summary>
        /// <returns></returns>
        public virtual bool CanModelToView() => ((_modelViewDataLinkMode & EModelViewDataLinkMode.ModelToView) == EModelViewDataLinkMode.ModelToView);

        /// <summary>
        /// 能否将视图数据更新到模型
        /// </summary>
        /// <returns></returns>
        public virtual bool CanViewToModel() => ((_modelViewDataLinkMode & EModelViewDataLinkMode.ViewToModel) == EModelViewDataLinkMode.ViewToModel);

        #endregion

        #region 模型

        /// <summary>
        /// 模型数据类型
        /// </summary>
        public virtual Type modelValueType
        {
            get
            {
                switch (_modelType)
                {
                    case EModelType.FieldPropertyMethodBinder:
                    case EModelType.FieldPropertyMethodBinder_MainObject:
                        {
                            if (_fieldPropertyMethodBinder.TryGetPropertyPathValueType(out var value))
                            {
                                return value;
                            }
                            break;
                        }
                    case EModelType.ScriptVariable: return typeof(string);
                    case EModelType.ModelObject: return modelObject?.GetType();
                    case EModelType.ModelObjectKeyValue: return modelObject?.GetModelKeyValueType(_modelKey.GetValue());
                    case EModelType.UnityObject: return _unityObject ? _unityObject.GetType() : typeof(UnityEngine.Object);
                }
                return null;
            }
        }

        /// <summary>
        /// 模型数据值
        /// </summary>
        public virtual object modelValue
        {
            get
            {
                switch (_modelType)
                {
                    case EModelType.FieldPropertyMethodBinder:
                    case EModelType.FieldPropertyMethodBinder_MainObject:
                        {
                            if (_fieldPropertyMethodBinder.TryGetPropertyPathValue(out var value))
                            {
                                return value;
                            }
                            break;
                        }
                    case EModelType.ScriptVariable:
                        {
                            if (_scriptVariable.TryGetHierarchyVarValue(out var value))
                            {
                                return value;
                            }
                            break;
                        }
                    case EModelType.ModelObject: return modelObject;
                    case EModelType.ModelObjectKeyValue: return modelObject.GetModelKeyValue(_modelKey.GetValue());
                    case EModelType.UnityObject: return _unityObject;
                }
                return null;
            }
            set
            {
                switch (_modelType)
                {
                    case EModelType.FieldPropertyMethodBinder:
                    case EModelType.FieldPropertyMethodBinder_MainObject:
                        {
                            _fieldPropertyMethodBinder.TrySetPropertyPathValue(value);
                            break;
                        }
                    case EModelType.ScriptVariable: _scriptVariable.TrySetOrAddSetHierarchyVarValue(value.ToString()); break;
                    case EModelType.ModelObject: modelObject = value; break;
                    case EModelType.ModelObjectKeyValue: modelObject.SetModelKeyValue(_modelKey.GetValue(), value); break;
                    case EModelType.UnityObject: _unityObject = value as UnityEngine.Object; break;
                }
            }
        }

        /// <summary>
        /// 模型类型
        /// </summary>
        [Group("模型", textEN = "Model")]
        [Name("模型类型")]
        [EnumPopup]
        public EModelType _modelType = EModelType.FieldPropertyMethodBinder;

        #region 字段属性方法绑定器

        /// <summary>
        /// 字段、属性或方法绑定器
        /// </summary>
        [Name("字段属性方法绑定器")]
        [HideInSuperInspector(nameof(_modelType), EValidityCheckType.NotEqual | EValidityCheckType.And, EModelType.FieldPropertyMethodBinder, nameof(_modelType), EValidityCheckType.NotEqual, EModelType.FieldPropertyMethodBinder_MainObject)]
        public FieldPropertyMethodMemberBinder _fieldPropertyMethodBinder = new FieldPropertyMethodMemberBinder();

        /// <summary>
        /// 模型目标
        /// </summary>
        public virtual UnityEngine.Object modelTarget => _fieldPropertyMethodBinder.target;

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="target"></param>
        public virtual void BindModel(UnityEngine.Object target)
        {
            _fieldPropertyMethodBinder.target = target;

            ModelToViewIfCan();
        }

        /// <summary>
        /// 绑定数据成员
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bindType"></param>
        /// <param name="memberName"></param>
        public virtual void BindModel(UnityEngine.Object target, EBindType bindType, string memberName)
        {
            _fieldPropertyMethodBinder._bindType = bindType;
            _fieldPropertyMethodBinder.memberName = memberName;

            BindModel(target);
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        public virtual void UnbindModel()
        {
            _fieldPropertyMethodBinder.target = null;
        }

        #endregion

        #region 中文脚本变量

        /// <summary>
        /// 脚本变量
        /// </summary>
        [Name("脚本变量")]
        [HideInSuperInspector(nameof(_modelType), EValidityCheckType.NotEqual, EModelType.ScriptVariable)]
        [VarString(EVarStringHierarchyKeyMode.Both)]
        public string _scriptVariable;

        #endregion

        #region 模型对象

        /// <summary>
        /// 模型对象
        /// </summary>
        public object modelObject { get; set; }

        #endregion

        #region 模型对象键值

        /// <summary>
        /// 模型关键字
        /// </summary>
        [Name("模型关键字")]
        [HideInSuperInspector(nameof(_modelType), EValidityCheckType.NotEqual, EModelType.ModelObjectKeyValue)]
        public ModelKeyPropertyValue _modelKey = new ModelKeyPropertyValue();

        /// <summary>
        /// 模型键值
        /// </summary>
        IModelKeyValue IModelKeyValueHost.modelKeyValue => modelMainObject as IModelKeyValue;

        #endregion

        #region 字段属性方法绑定器主体对象

        /// <summary>
        /// 字段属性方法绑定器主体对象
        /// </summary>
        public object fieldPropertyMethodBinderMainObject { get => _fieldPropertyMethodBinder.mainObject; set => _fieldPropertyMethodBinder.mainObject = value; }

        #endregion

        #region Unity对象

        /// <summary>
        /// Unity对象
        /// </summary>
        [Name("Unity对象")]
        [HideInSuperInspector(nameof(_modelType), EValidityCheckType.NotEqual, EModelType.UnityObject)]
        [ObjectPopup]
        public UnityEngine.Object _unityObject;

        #endregion

        #endregion

        #region 模型=>视图

        /// <summary>
        /// 模型到视图更新规则
        /// </summary>
        [Name("模型到视图更新规则")]
        [Tip("根据不同的视图刷新规则，将模型数据更新到视图", "Update the model data to the view according to different view refresh rules")]
        [EnumPopup]
        public EDataUpdateRule _modelToViewDataUpdateRule = EDataUpdateRule.Timing;

        /// <summary>
        /// 模型到视图数据更新间隔时间
        /// </summary>
        [Name("模型到视图数据更新间隔时间")]
        [Range(0, 3)]
        [HideInSuperInspector(nameof(_modelToViewDataUpdateRule), EValidityCheckType.NotEqual, EDataUpdateRule.Timing)]
        public float _modelToViewDataUpdateIntervalTime = 0.3f;

        /// <summary>
        /// 模型到视图数据更新触发定时间隔时间
        /// </summary>
        [Name("模型到视图数据更新触发定时间隔时间")]
        [Range(0, 3)]
        [HideInSuperInspector(nameof(_modelToViewDataUpdateRule), EValidityCheckType.NotEqual, EDataUpdateRule.TriggerAndTimed)]
        public float _modelToViewDataUpdateTrgggerTimedIntervalTime = 1f;

        /// <summary>
        /// 成员名称检查规则
        /// </summary>
        public enum EMemberNameCheckRule
        {
            /// <summary>
            /// 默认
            /// </summary>
            [Name("默认")]
            [Tip("默认规则即在空或匹配时认为检查结果成立", "The default rule is to assume that the check result is valid when empty or matched")]
            Default,

            /// <summary>
            /// 允许
            /// </summary>
            [Name("允许")]
            [Tip("不做检查，并认为检查结果总是成立，即允许继续执行后续逻辑；", "Not conducting inspections and believing that the inspection results are always valid, i.e. allowing for continued execution of subsequent logic;")]
            Allow,

            /// <summary>
            /// 拒绝
            /// </summary>
            [Name("拒绝")]
            [Tip("不做检查，并认为检查结果总是不成立，即拒绝继续执行后续逻辑；", "Refusing to conduct inspections and believing that the inspection results are always unfounded, i.e. refusing to continue executing subsequent logic;")]
            Refuse,

            /// <summary>
            /// 空
            /// </summary>
            [Name("无")]
            Empty,

            /// <summary>
            /// 匹配
            /// </summary>
            [Name("匹配")]
            Match,

            /// <summary>
            /// 空或匹配
            /// </summary>
            [Name("空或匹配")]
            EmptyOrMatch,
        }

        /// <summary>
        /// 当触发时成员名称检查规则
        /// </summary>
        [Name("当触发时成员名称检查规则")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_modelToViewDataUpdateRule), EValidityCheckType.NotEqual | EValidityCheckType.And, EDataUpdateRule.Trigger, nameof(_modelToViewDataUpdateRule), EValidityCheckType.NotEqual, EDataUpdateRule.TriggerAndTimed)]
        public EMemberNameCheckRule _memberNameCheckRuleOnTrigger = EMemberNameCheckRule.Default;

        /// <summary>
        /// 变量变换回调
        /// </summary>
        /// <param name="variable"></param>
        private void OnVariableValueChanged(Variable variable)
        {
            if (variable.name == _scriptVariable)
            {
                ModelToViewIfCanAndTrigger();
            }
        }

        /// <summary>
        /// 模型发生改变
        /// </summary>
        /// <returns></returns>
        public virtual bool ModelHasChanged() => true;

        /// <summary>
        /// 将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToView() => OnModelToView();

        /// <summary>
        /// 如果可以则模型到视图：如果模型至视图可连通，则将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToViewIfCan() => CanModelToView() && OnModelToView();

        private float lastTriggerM2VTime = 0;

        private bool inM2VTrigger = false;

        /// <summary>
        /// 如果模型至视图可连通，并且模型到视图数据更新规则为触发器，则将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToViewIfCanAndTrigger()
        {
            switch (_modelToViewDataUpdateRule)
            {
                case EDataUpdateRule.Trigger: return ModelToViewIfCan();
                case EDataUpdateRule.TriggerAndTimed:
                    {
                        if (inM2VTrigger) break;

                        var time = XTime.realtimeSinceSceneLoaded;
                        var it = time - lastTriggerM2VTime;

                        if (it >= _modelToViewDataUpdateTrgggerTimedIntervalTime || it <= 0)
                        {
                            lastTriggerM2VTime = time;
                            return ModelToViewIfCan();
                        }
                        else
                        {
                            inM2VTrigger = true;
                            CommonFun.DelayCallInMainThread(() =>
                            {
                                inM2VTrigger = false;
                                lastTriggerM2VTime = XTime.realtimeSinceSceneLoaded;
                                ModelToViewIfCan();
                            }, _modelToViewDataUpdateTrgggerTimedIntervalTime - it);
                        }
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// 如果模型至视图可连通，并且数据已变化，则将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToViewIfCanAndValueChanged() => ModelHasChanged() && ModelToViewIfCan();

        /// <summary>
        /// 如果模型至视图可连通，模型到视图数据更新规则为触发器，并且模型数据已改变，则将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToViewIfCanAndTriggerAndValueChanged() => ModelHasChanged() && ModelToViewIfCanAndTrigger();

        /// <summary>
        /// 如果模型数据已变化，则将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToViewIfValueChanged() => ModelHasChanged() && OnModelToView();

        /// <summary>
        /// 将模型数据更新至视图
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnModelToView()
        {
            var modelValue = this.modelValue;
            bool result = TryConvertModelToView(modelValue, out var value);
            if (result)
            {
                UpdateModelInfoText();
                SetViewValue(value);

                CallChildModelToView(modelValue, value);
            }
            return result;
        }

        /// <summary>
        /// 设置模型值：可破循环
        /// </summary>
        /// <param name="value"></param>
        public void SetModelValue(object value)
        {
            try
            {
                if (inViewToModel) return;
                inViewToModel = true;

                modelValue = value;
            }
            finally
            {
                inViewToModel = false;
            }
        }

        /// <summary>
        /// 模型数据更新中
        /// </summary>
        public bool inViewToModel { get; private set; } = false;

        #region 数据转换器

        /// <summary>
        /// 模型到视图数据转换器接口类型
        /// </summary>
        public Type modelToViewConverterInterfaceType
        {
            get
            {
                if (modelValueType == null || viewValueType == null
                    || typeof(MethodInfo).IsAssignableFrom(modelValueType) || typeof(MethodInfo).IsAssignableFrom(viewValueType)) return null;
                return typeof(IDataConverter<,>).MakeGenericType(modelValueType, viewValueType);
            }
        }

        /// <summary>
        /// 模型到视图数据转换器
        /// </summary>
        [Name("模型到视图数据转换器")]
        [ComponentPopup]
        public BaseDataConverter _modelToViewConverter;

        private BaseDataConverter validModelToViewConverter;

        /// <summary>
        /// 更新有效的模型到视图数据转换器
        /// </summary>
        /// <returns></returns>
        private bool UpdateValidModelToViewConverter()
        {
            if (TryGetValidModelToViewConverter(out var MVConverter))
            {
                validModelToViewConverter = MVConverter;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 如果需要更新有效的模型到视图数据转换器
        /// </summary>
        /// <returns></returns>
        private bool UpdateValidModelToViewConverterIfNeed() => validModelToViewConverter || UpdateValidModelToViewConverter();

        /// <summary>
        /// 转换模型数据到视图中
        /// </summary>
        /// <param name="modelVale"></param>
        /// <param name="viewValue"></param>
        /// <returns></returns>
        protected bool TryConvertModelToView(object modelVale, out object viewValue)
        {
            if (modelVale != null)
            {
                if (UpdateValidModelToViewConverterIfNeed() && validModelToViewConverter.TryConvertTo(this, modelVale, viewValueType, out viewValue))
                {
                    return true;
                }
                if (Converter.instance.TryConvertTo(modelVale, viewValueType, out viewValue))
                {
                    return true;
                }
            }
            viewValue = default;
            return false;
        }

        /// <summary>
        /// 尝试获取有效模型到视图转换器
        /// </summary>
        /// <param name="modelToViewConverter"></param>
        /// <returns></returns>
        public bool TryGetValidModelToViewConverter(out BaseDataConverter modelToViewConverter)
        {
            if (_modelToViewConverter && modelValueType != null)
            {
                if (DataConverterCache.Get(modelValueType, viewValueType, _modelToViewConverter.GetType()).canI2O)
                {
                    modelToViewConverter = _modelToViewConverter;
                    return true;
                }
                else
                {
                    var tmp = _modelToViewConverter.GetComponent(modelToViewConverterInterfaceType) as BaseDataConverter;
                    if (tmp)
                    {
                        modelToViewConverter = tmp;
                        return true;
                    }
                }
            }
            modelToViewConverter = default;
            return false;
        }

        #endregion

        #endregion

        #region 模型信息

        /// <summary>
        /// 模型信息
        /// </summary>
        [Group("模型信息", textEN = "Model Infomation", defaultIsExpanded = false)]
        [Name("模型信息文本")]
        public Text _modelInfoText;

        /// <summary>
        /// 模型信息
        /// </summary>
        public string modelInfo
        {
            get => _modelInfoText ? _modelInfoText.text : default;
            set
            {
                if (_modelInfoText)
                {
                    _modelInfoText.text = value;
                }
            }
        }

        /// <summary>
        /// 模型信息源
        /// </summary>
        public enum EModelInfoSource
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 模型主对象类型名称
            /// </summary>
            [Name("模型主对象类型名称")]
            ModelMainObjectTypeName,

            /// <summary>
            /// 模型值类型名称
            /// </summary>
            [Name("模型值类型名称")]
            ModelValueTypeName,

            /// <summary>
            /// 字段属性方法绑定器成员信息名称
            /// </summary>
            [Name("字段属性方法绑定器成员信息名称")]
            [Tip("【模型类型】为【字段属性方法绑定器】或【字段属性方法绑定器主体对象】时有效", "Valid when the model type is either Field Attribute Method Binder or Field Attribute Method Binder Body Object")]
            fieldPropertyMethodBinderMemberInfoName,

            /// <summary>
            /// 脚本变量名称
            /// </summary>
            [Name("脚本变量名称")]
            [Tip("【模型类型】为【脚本变量】时有效", "Valid when Model Type is Script Variable")]
            VarName,

            /// <summary>
            /// 模型关键字
            /// </summary>
            [Name("模型关键字")]
            ModelKey,
        }

        /// <summary>
        /// 模型信息源
        /// </summary>
        [Name("模型信息源")]
        [EnumPopup]
        public EModelInfoSource _modelInfoSource = EModelInfoSource.ModelMainObjectTypeName;

        /// <summary>
        /// 设置模型信息
        /// </summary>
        /// <param name="text"></param>
        /// <param name="useConverter"></param>
        public void SetModelInfo(string text, bool useConverter = true)
        {
            if (useConverter
                && _modelInfoText
                && _modelInfoConverter
                && _modelInfoConverter.TryConvertTo(this, text, typeof(string), out var value)
                && value is string newText)
            {
                text = newText;
            }
            modelInfo = text;
        }

        private void UpdateModelInfoText()
        {
            if (!_modelInfoText) return;

            switch (_modelInfoSource)
            {
                case EModelInfoSource.ModelMainObjectTypeName:
                    {
                        if (modelMainObject == null) return;
                        SetModelInfo(CommonFun.Name(modelMainObject.GetType()));
                        break;
                    }
                case EModelInfoSource.ModelValueTypeName:
                    {
                        SetModelInfo(CommonFun.Name(modelValueType));
                        break;
                    }
                case EModelInfoSource.fieldPropertyMethodBinderMemberInfoName:
                    {
                        if (_modelType == EModelType.FieldPropertyMethodBinder || _modelType == EModelType.FieldPropertyMethodBinder_MainObject)
                        {
                            SetModelInfo(CommonFun.Name(_fieldPropertyMethodBinder.memberInfo));
                        }
                        break;
                    }
                case EModelInfoSource.VarName:
                    {
                        if (_modelType == EModelType.ScriptVariable)
                        {
                            SetModelInfo(_scriptVariable);
                        }
                        break;
                    }
                case EModelInfoSource.ModelKey:
                    {
                        if (_modelType == EModelType.ModelObjectKeyValue)
                        {
                            SetModelInfo(_modelKey.GetValue());
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 模型信息转换器
        /// </summary>
        [Name("模型信息转换器")]
        [ComponentPopup]
        public BaseDataConverter _modelInfoConverter;

        #endregion

        #region 视图

        /// <summary>
        /// 视图数据类型
        /// </summary>
        public abstract Type viewValueType { get; }

        /// <summary>
        /// 视图数据值
        /// </summary>
        public abstract object viewValue { get; set; }

        #endregion

        #region 视图=>模型

        /// <summary>
        /// 模型数据更新规则
        /// </summary>
        [Group("视图", textEN = "View", defaultIsExpanded = false)]
        [Name("视图到模型数据更新规则")]
        [Tip("根据不同的模型数据更新规则，将视图数据更新到模型", "Update the view data to the model according to different refresh rules")]
        [EnumPopup]
        public EDataUpdateRule _viewToModelDataUpdateRule = EDataUpdateRule.Trigger;

        /// <summary>
        /// 视图到模型数据更新间隔时间
        /// </summary>
        [Name("视图到模型数据更新间隔时间")]
        [Range(0, 3)]
        [HideInSuperInspector(nameof(_viewToModelDataUpdateRule), EValidityCheckType.NotEqual, EDataUpdateRule.Timing)]
        public float _viewToModelDataUpdateIntervalTime = 0.3f;

        /// <summary>
        /// 视图到模型数据更新触发定时间隔时间
        /// </summary>
        [Name("视图到模型数据更新触发定时间隔时间")]
        [Range(0, 3)]
        [HideInSuperInspector(nameof(_viewToModelDataUpdateRule), EValidityCheckType.NotEqual, EDataUpdateRule.TriggerAndTimed)]
        public float _viewToModelDataUpdateTrgggerTimedIntervalTime = 1f;

        /// <summary>
        /// 视图发生改变
        /// </summary>
        /// <returns></returns>
        public virtual bool ViewHasChanged() => true;

        /// <summary>
        /// 将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        public bool ViewToModel() => OnViewToModel();

        /// <summary>
        /// 将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnViewToModel()
        {
            var viewValue = this.viewValue;
            bool result = TryConvertViewToModel(viewValue, out var value);
            if (result)
            {
                SetModelValue(value);

                CallChildViewToModel(value, viewValue);
            }
            return result;
        }

        /// <summary>
        /// 设置视图值：可破循环
        /// </summary>
        /// <param name="value"></param>
        public void SetViewValue(object value)
        {
            try
            {
                if (inModelToView) return;
                inModelToView = true;

                viewValue = value;
            }
            finally
            {
                inModelToView = false;
            }
        }

        /// <summary>
        /// 视图更新中
        /// </summary>
        public bool inModelToView { get; private set; } = false;

        /// <summary>
        /// 如果视图至模型可连通，将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        public bool ViewToModelIfCan() => CanViewToModel() && OnViewToModel();

        private float lastTriggerV2MTime = 0;

        private bool inV2MTrigger = false;

        /// <summary>
        /// 如果视图至模型可连通，并且视图到模型数据更新规则为触发器，将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        public bool ViewToModelIfCanAndTrigger() {

            switch (_viewToModelDataUpdateRule)
            {
                case EDataUpdateRule.Trigger: return ViewToModelIfCan();
                case EDataUpdateRule.TriggerAndTimed:
                    {
                        if (inV2MTrigger) break;

                        var time = XTime.realtimeSinceSceneLoaded;
                        var it = time - lastTriggerV2MTime;

                        if (it >= _viewToModelDataUpdateTrgggerTimedIntervalTime || it <= 0)
                        {
                            lastTriggerV2MTime = time;
                            return ViewToModelIfCan();
                        }
                        else
                        {
                            inV2MTrigger = true;
                            CommonFun.DelayCallInMainThread(() =>
                            {
                                inV2MTrigger = false;
                                lastTriggerV2MTime = XTime.realtimeSinceSceneLoaded;
                                ViewToModelIfCan();
                            }, _viewToModelDataUpdateTrgggerTimedIntervalTime - it);
                        }
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// 如果视图至模型可连通，并且视图数据已改变，将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        public bool ViewToModelIfCanAndValueChanged() => ViewHasChanged() && ViewToModelIfCan();

        /// <summary>
        /// 如果视图至模型可连通，并且视图到模型数据更新规则为触发器，并且视图数据已改变，将视图数据设置到模型 
        /// </summary>
        /// <returns></returns>
        public bool ViewToModelIfCanAndTriggerAndValueChanged() => ViewHasChanged() && ViewToModelIfCanAndTrigger();

        /// <summary>
        /// 如果视图数据已改变，将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        public bool ViewToModelIfValueChanged() => ViewHasChanged() && OnViewToModel();

        #region 数据转换器

        /// <summary>
        /// 视图到模型数据转换器接口类型
        /// </summary>
        public Type viewToModelConverterInterfaceType
        {
            get
            {
                if (viewValueType == null || modelValueType == null) return null;
                return typeof(IDataConverter<,>).MakeGenericType(viewValueType, modelValueType);
            }
        }

        /// <summary>
        /// 视图到模型数据转换器
        /// </summary>
        [Name("视图到模型数据转换器")]
        [ComponentPopup]
        public BaseDataConverter _viewToModelConverter;

        /// <summary>
        /// 有效的视图到模型数据转换器
        /// </summary>
        private BaseDataConverter validViewToModelConverter;

        /// <summary>
        /// 更新有效的视图到模型数据转换器
        /// </summary>
        /// <returns></returns>
        private bool UpdateValidViewToModelConverter()
        {
            if (TryGetValidViewToModelConverter(out var VMConverter))
            {
                validViewToModelConverter = VMConverter;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 如果需要更新有效的视图到模型数据转换器
        /// </summary>
        /// <returns></returns>
        private bool UpdateValidViewToModelConverterIfNeed() => validViewToModelConverter || UpdateValidViewToModelConverter();

        /// <summary>
        /// 转换视图数据到模型中
        /// </summary>
        /// <param name="viewData"></param>
        /// <param name="modelData"></param>
        /// <returns></returns>
        protected bool TryConvertViewToModel(object viewData, out object modelData)
        {
            if (viewData != null)
            {
                if (UpdateValidViewToModelConverterIfNeed() && validViewToModelConverter.TryConvertTo(this, viewData, modelValueType, out modelData))
                {
                    return true;
                }
                if (Converter.instance.TryConvertTo(viewData, modelValueType, out modelData))
                {
                    return true;
                }
            }
            modelData = default;
            return false;
        }

        /// <summary>
        /// 尝试获取有效视图到模型转换器
        /// </summary>
        /// <param name="viewToModelConverter"></param>
        /// <returns></returns>
        public bool TryGetValidViewToModelConverter(out BaseDataConverter viewToModelConverter)
        {
            if (_viewToModelConverter && modelValueType != null)
            {
                if (DataConverterCache.Get(viewValueType, modelValueType, _viewToModelConverter.GetType()).canI2O)
                {
                    viewToModelConverter = _viewToModelConverter;
                    return true;
                }
                else
                {
                    var tmp = _viewToModelConverter.GetComponent(viewToModelConverterInterfaceType) as BaseDataConverter;
                    if (tmp)
                    {
                        viewToModelConverter = tmp;
                        return true;
                    }
                }
            }
            viewToModelConverter = default;
            return false;
        }

        #endregion

        #endregion

        #region ITypeBinderGetter

        /// <summary>
        /// 获取器所有者
        /// </summary>
        public UnityEngine.Object owner => this;

        /// <summary>
        /// 类型绑定器获取器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITypeBinder> GetTypeBinders() => new ITypeBinder[] { _fieldPropertyMethodBinder };

        #endregion

        #region IDropdownPopupAttribute

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            return _fieldPropertyMethodBinder.TryGetOptions(purpose, propertyPath, out options);
        }

        bool IDropdownPopupAttribute.TryGetOption(string purpose, string propertyPath, object propertyValue, out string option)
        {
            return _fieldPropertyMethodBinder.TryGetOption(purpose, propertyPath, propertyValue, out option);
        }

        bool IDropdownPopupAttribute.TryGetPropertyValue(string purpose, string propertyPath, string option, out object propertyValue)
        {
            return _fieldPropertyMethodBinder.TryGetPropertyValue(purpose, propertyPath, option, out propertyValue);
        }

        #endregion

        #region 模型键值宿主修改器

        object IModelKeyValueHostModifier.model => modelMainObject;

        string IModelKeyValueHostModifier.key => _modelKey.GetValue();

        void IModelKeyValueHostModifier.SetModelKey(object model, string key)
        {
            switch (_modelType)
            {
                case EModelType.ModelObject:
                case EModelType.ModelObjectKeyValue:
                    {
                        _modelKey.SetValue(key);
                        AddModelMainObjectEventListener(model);
                        modelObject = model;
                        break;
                    }
                case EModelType.FieldPropertyMethodBinder_MainObject:
                    {
                        _fieldPropertyMethodBinder.memberName = key;
                        AddModelMainObjectEventListener(model);
                        _fieldPropertyMethodBinder.mainObject = model;
                        break;
                    }
                case EModelType.UnityObject:
                    {
                        AddModelMainObjectEventListener(model);
                        _unityObject = model as UnityEngine.Object;
                        break;
                    }
                default:
                    {
                        AddModelMainObjectEventListener(default);
                        break;
                    }
            }
            ModelToViewIfCan();
        }

        #endregion
    }

    /// <summary>
    /// 模型类型
    /// </summary>
    public enum EModelType
    {
        /// <summary>
        /// 字段属性方法绑定器
        /// </summary>
        [Name("字段属性方法绑定器")]
        FieldPropertyMethodBinder,

        /// <summary>
        /// 脚本变量
        /// </summary>
        [Name("脚本变量")]
        ScriptVariable,

        /// <summary>
        /// 模型对象
        /// </summary>
        [Name("模型对象")]
        ModelObject,

        /// <summary>
        /// 模型对象键值
        /// </summary>
        [Name("模型对象键值")]
        ModelObjectKeyValue,

        /// <summary>
        /// 字段属性方法绑定器主体对象
        /// </summary>
        [Name("字段属性方法绑定器主体对象")]
        FieldPropertyMethodBinder_MainObject,

        /// <summary>
        /// Unity对象
        /// </summary>
        [Name("Unity对象")]
        UnityObject,

        /// <summary>
        /// 外部驱动
        /// </summary>
        [Name("外部驱动")]
        DrivedByOutside,
    }

    /// <summary>
    /// 数据更新规则
    /// </summary>
    public enum EDataUpdateRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 定时
        /// </summary>
        [Name("定时")]
        Timing,

        /// <summary>
        /// 每帧
        /// </summary>
        [Name("每帧")]
        EveryFrame,

        /// <summary>
        /// 触发
        /// </summary>
        [Name("触发")]
        Trigger,

        /// <summary>
        /// 触发并定时：触发且根据定时时间更新；即如果短时间内连续触发，也仅会在定时时间到之后才触发一次；
        /// </summary>
        [Name("触发并定时")]
        [Tip("触发且根据定时时间更新；即如果短时间内连续触发，也仅会在定时时间到之后才触发一次；")]
        TriggerAndTimed,
    }

    /// <summary>
    /// 模型视图数据连接模式
    /// </summary>
    [Name("模型视图数据连接模式")]
    public enum EModelViewDataLinkMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 模型到视图
        /// </summary>
        [Name("模型-->视图")]
        [Tip("模型到视图", "Model to View")]
        ModelToView = 1 << 0,

        /// <summary>
        /// 视图到模型
        /// </summary>
        [Name("模型<--视图")]
        [Tip("视图到模型", "View to Model")]
        ViewToModel = 1 << 1,

        /// <summary>
        /// 同时允许模型到视图和视图到模型
        /// </summary>
        [Name("模型<-->视图")]
        [Tip("同时允许模型到视图和视图到模型", "Allow both model to view and view to model")]
        Both,

    }
}
