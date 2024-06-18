using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Windows.ListViews;

namespace XCSJ.PluginXGUI.DataViews.Base
{
    #region 对象成员视图

    /// <summary>
    /// 对象成员视图
    /// </summary>
    [Name("对象成员视图")]
    [XCSJ.Attributes.Icon(EIcon.List)]
    [Tool(XGUICategory.DataView, rootType = typeof(XGUIManager))]
    public class ObjectMemberView : BaseModelView
    {
        /// <summary>
        /// 模型视图列表
        /// </summary>
        [Name("模型视图列表")]
        [ComponentPopup(typeof(BaseModelView))]
        [EndGroup(true)]
        [Array]
        public List<BaseModelView> _modelViews = new List<BaseModelView>();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (var item in _modelViews)
            {
                if (item)
                {
                    item.AddListener(this);
                }
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var item in _modelViews)
            {
                if (item)
                {
                    item.RemoveListener(this);
                }
            }
        }

        /// <summary>
        /// 当模型到视图
        /// </summary>
        /// <returns></returns>
        protected override bool OnModelToView()
        {
            var rs = base.OnModelToView();

            var modelValue = this.modelValue;
            foreach (var item in _modelViews)
            {
                if (item)
                {
                    item.SetModelMainObject(modelValue, this, propertyPath);
                }
            }
            return rs;
        }

        /// <summary>
        /// 视图值类型
        /// </summary>
        public override Type viewValueType => modelValueType;

        /// <summary>
        /// 视图值
        /// </summary>
        public override object viewValue { get => modelValue; set { } }
    }

    #endregion

    #region 基础对象成员视图模型提供者

    /// <summary>
    /// 基础对象成员视图模型提供者
    /// </summary>
    [RequireManager(typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    [Tool(XGUICategory.DataView, nameof(ObjectMemberView))]
    [RequireComponent(typeof(ObjectMemberView))]
    public abstract class BaseObjectMemberViewModelProvider : DraggableView, IModelViewEvent
    {
        /// <summary>
        /// 对象成员视图
        /// </summary>
        [Name("对象成员视图")]
        public ObjectMemberView _objectMemberView;

        /// <summary>
        /// 对象成员视图
        /// </summary>
        public ObjectMemberView objectMemberView
        {
            get
            {
                if (!_objectMemberView)
                {
                    _objectMemberView = this.XGetOrAddComponent<ObjectMemberView>();
                }
                return _objectMemberView;
            }
        }

        /// <summary>
        /// 当子级模型到视图
        /// </summary>
        /// <param name="modelViewEventData"></param>
        public virtual void OnChildModelToView(ModelViewEventData modelViewEventData) { }

        /// <summary>
        /// 当子级视图到模型
        /// </summary>
        /// <param name="modelViewEventData"></param>
        public virtual void OnChildViewToModel(ModelViewEventData modelViewEventData) { }
    }

    #endregion

    #region 模型视图事件数据

    /// <summary>
    /// 模型视图事件数据
    /// </summary>
    public class ModelViewEventData : InteractData<ModelViewEventData>
    {
        /// <summary>
        /// 基础模型视图
        /// </summary>
        public BaseModelView baseModelView => interactor as BaseModelView;

        /// <summary>
        /// 模型值
        /// </summary>
        public object modelValue { get; private set; }

        /// <summary>
        /// 视图值
        /// </summary>
        public object viewValue { get; private set; }

        /// <summary>
        /// 模型视图数据连接模式
        /// </summary>
        public EModelViewDataLinkMode modelViewDataLinkMode { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ModelViewEventData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseModelView"></param>
        /// <param name="modelValue"></param>
        /// <param name="viewValue"></param>
        /// <param name="modelViewDataLinkMode"></param>
        public ModelViewEventData(BaseModelView baseModelView, object modelValue, object viewValue, EModelViewDataLinkMode modelViewDataLinkMode) : base("", baseModelView)
        {
            this.modelValue = modelValue;
            this.viewValue = viewValue;
            this.modelViewDataLinkMode = modelViewDataLinkMode;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="interactor"></param>
        public ModelViewEventData(ModelViewEventData parent, InteractObject interactor) : base(parent.cmdName, parent, interactor) { }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="interactData"></param>
        protected override void CopyTo(ModelViewEventData interactData)
        {
            interactData.modelValue = this.modelValue;
            interactData.viewValue = this.viewValue;
            interactData.modelViewDataLinkMode = modelViewDataLinkMode;
        }
    }

    #endregion

    #region 模型视图事件接口

    /// <summary>
    /// 模型视图事件接口
    /// </summary>
    public interface IModelViewEvent
    {
        /// <summary>
        /// 当子级模型到视图
        /// </summary>
        /// <param name="modelViewEventData"></param>
        void OnChildModelToView(ModelViewEventData modelViewEventData);

        /// <summary>
        /// 当子级视图到模型
        /// </summary>
        /// <param name="modelViewEventData"></param>
        void OnChildViewToModel(ModelViewEventData modelViewEventData);
    } 

    #endregion
}

