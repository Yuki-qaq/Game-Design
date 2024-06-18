using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCNScripts;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.Windows.ListViews
{
    /// <summary>
    /// 列表视图项
    /// </summary>
    [Name("列表视图项")]
    [XCSJ.Attributes.Icon(EIcon.List)]
    public sealed class ListViewItem : BaseObjectMemberViewModelProvider, IScrollHandler, IViewItem
    {
        /// <summary>
        /// 当分配
        /// </summary>
        internal void OnAlloc()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 当释放
        /// </summary>
        internal void OnFree()
        {
            index = -1;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 列表视图
        /// </summary>
        public ListView listView { get; internal set; }

        /// <summary>
        /// 索引：在列表中的程序索引
        /// </summary>
        public int index { get; internal set; } = -1;

        /// <summary>
        /// 模型：列表视图项模型
        /// </summary>
        public ListViewItemModel model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    if (_model != null)
                    {
                        _model.listViewItem = this;
                    }
                    else
                    {
                        return;
                    }

                    foreach (var item in _linkModelViews)
                    {
                        if (!item._modelView) continue;

                        switch (item._modelSource)
                        {
                            case EModelSource.Model:
                                {
                                    item._modelView.SetModelMainObject(_model, this, "");
                                    break;
                                }
                            case EModelSource.ModelEntity:
                                {
                                    item._modelView.SetModelMainObject(_model.modelEntity, this, "");
                                    break;
                                }
                        }
                    }
                }
            }
        }

        private ListViewItemModel _model;

        /// <summary>
        /// 模型实体
        /// </summary>
        public object modelEntity => _model?.modelEntity;

        /// <summary>
        /// 关联模型视图
        /// </summary>
        [Serializable]
        public class LinkModelView
        {
            /// <summary>
            /// 模型源
            /// </summary>
            [Name("模型源")]
            [EnumPopup]
            public EModelSource _modelSource = EModelSource.Model;

            /// <summary>
            /// 模型视图
            /// </summary>
            [Name("模型视图")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            public BaseModelView _modelView;

            /// <summary>
            /// 构造函数
            /// </summary>
            public LinkModelView() { }

            /// <summary>
            /// 构造函数
            /// </summary>
            public LinkModelView(BaseModelView modelView)
            {
                _modelView = modelView;
            }
        }

        /// <summary>
        /// 关联模型视图列表
        /// </summary>
        [Name("关联模型视图列表")]
        public List<LinkModelView> _linkModelViews = new List<LinkModelView>();

        /// <summary>
        /// 不可交互性效果
        /// </summary>
        [Name("不可交互性效果")]
        public enum EDisableInteractableEffect
        {
            /// <summary>
            /// 设置透明度
            /// </summary>
            [Name("设置透明度")]
            SetTransparent,
        }

        /// <summary>
        /// 不可交互性效果
        /// </summary>
        [Name("不可交互性效果")]
        [EnumPopup]
        public EDisableInteractableEffect _disableInteractableEffect = EDisableInteractableEffect.SetTransparent;

        /// <summary>
        /// 不可交互透明度
        /// </summary>
        [Name("不可交互透明度")]
        [HideInSuperInspector(nameof(_disableInteractableEffect), EValidityCheckType.NotEqual, EDisableInteractableEffect.SetTransparent)]
        public float _alpha = 0.5f;

        /// <summary>
        /// 图形记录器
        /// </summary>
        private GraphicRecorder _graphicRecorder = new GraphicRecorder();

        /// <summary>
        /// 查找并设置父级视图
        /// </summary>
        /// <returns></returns>
        protected override View FindAndSetParentView()
        {
            var parentView = this._parentView;
            if (!parentView || !(parentView is ListView))
            {
                var listView = this.GetComponentInParent<ListView>();
                if (listView)
                {
                    this.parentView = listView;
                }
            }
            return base.FindAndSetParentView();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (subitemTemplate) { }
            _linkModelViews.Add(new LinkModelView() { _modelSource = EModelSource.Model, _modelView = objectMemberView });
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            subitemGenerateRuleOnEnable = _subitemGenerateRule;
            subitemKeyGenerateRuleOnEnable = _subitemKeyGenerateRule;

            if (subitemTemplate) { }

            if (subitemPool != null) { }

            // 容器为0时默认添加自身对象成员视图
            if (_linkModelViews.Count == 0)
            {
                _linkModelViews.Add(new LinkModelView(objectMemberView));
            }

            _graphicRecorder.Record(GetComponentsInChildren<Graphic>());
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            _graphicRecorder.Recover();
            _graphicRecorder.Clear();
        }

        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            if (model != null && model.interactable != lastInteractable)
            {
                lastInteractable = model.interactable;
                UpdateInteractable();
            }
        }

        private bool lastInteractable = false;

        private void UpdateInteractable()
        {
            // 设置透明度
            if (model.interactable)
            {
                _graphicRecorder.Recover();
            }
            else
            {
                _graphicRecorder.SetAlpha(_alpha);
            }
        }

        /// <summary>
        /// 设置模型
        /// </summary>
        /// <param name="index"></param>
        /// <param name="model"></param>
        public void SetModel(int index, ListViewItemModel model)
        {
            this.index = index;
            transform.SetSiblingIndex(index);
            this.model = model;

            if (this.model != null)
            {
                lastInteractable = this.model.interactable;
                UpdateSubitems();
                UpdateInteractable();
                var scriptManager = ScriptManager.instance;
                if (scriptManager)
                {
                    scriptManager.SetSystemVariableValues(this.gameObject.GameObjectToString());
                    scriptManager.TrySetOrAddSetHierarchyVarValue(this.referenceObjectVarString, this);
                    scriptManager.ExecuteFunction(_customFunction);
                }
            }
            else
            {
                UpdateSubitems();
            }
        }

        #region 点击和拖拽

        /// <summary>
        /// 点击项：带参数
        /// </summary>
        /// <param name="inputCmdParam"></param>
        public void ClickItem(string inputCmdParam = "")
        {
            TryInteract(nameof(ListView.Click), CNScriptHelper.ParseExpressionAndSetVarValue(inputCmdParam, referenceObjectVarString, this));
        }

        /// <summary>
        /// 父对象滚动矩形
        /// </summary>
        private ScrollRect _parentScrollRect = null;
        private ScrollRect parentScrollRect => this.XGetComponentInParent(ref _parentScrollRect);

        /// <summary>
        /// 响应滚轮滚动
        /// </summary>
        /// <param name="eventData"></param>
        public void OnScroll(PointerEventData eventData)
        {
            if (parentScrollRect)
            {
                parentScrollRect.OnScroll(eventData);
            }
        }

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="pointerEventData"></param>
        protected override void OnClick(PointerEventData pointerEventData)
        {
            TryInteract(nameof(ListView.Click), null, pointerEventData);
        }

        /// <summary>
        /// 指针按下
        /// </summary>
        /// <param name="pointerEventData"></param>
        public override void OnBeginDrag(PointerEventData pointerEventData)
        {
            CommonFun.BeginOnUI();

            if (parentScrollRect)
            {
                parentScrollRect.OnBeginDrag(pointerEventData);
            }

            TryInteract(nameof(ListView.DragStart), null, pointerEventData);
        }

        /// <summary>
        /// 拖拽进行中
        /// </summary>
        /// <param name="pointerEventData"></param>
        public override void OnDrag(PointerEventData pointerEventData)
        {
            base.OnDrag(pointerEventData);

            if (parentScrollRect && parentScrollRect.enabled)
            {
                parentScrollRect.OnDrag(pointerEventData);

                // 求与垂直的夹角，判断拖动方向，防止水平与垂直方向同时响应导致的拖动时整个界面都会动
                float angle = Vector2.Angle(pointerEventData.delta, Vector2.up);

                // 视图项正在水平运动时禁用垂直运行
                if (angle > 45f && angle < 135f) parentScrollRect.enabled = false;
            }

            TryInteract(nameof(ListView.Draging), null, pointerEventData);
        }

        /// <summary>
        /// 指针弹起
        /// </summary>
        /// <param name="pointerEventData"></param>
        public override void OnEndDrag(PointerEventData pointerEventData)
        {
            CommonFun.EndOnUI();

            if (parentScrollRect)
            {
                parentScrollRect.enabled = true;
                parentScrollRect.OnEndDrag(pointerEventData);
            }

            TryInteract(nameof(ListView.DragEnd), null, pointerEventData);
        }

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="cmdParam"></param>
        /// <param name="pointerEventData"></param>
        /// <returns></returns>
        private bool TryInteract(string cmd, object cmdParam = null, PointerEventData pointerEventData = null)
        {
            return listView && model != null && model.interactable && listView.TryInteract(new ListViewInteractData(pointerEventData, this, listView.GetInCmdName(cmd), cmdParam), out _);
        }

        #endregion

        #region 子项模版

        /// <summary>
        /// 子项模版
        /// </summary>
        [Group("动态列表视图子项设置", textEN = "Dynamic List View Subitem Settings")]
        [Name("子项模版")]
        public ListViewSubitem _subitemTemplate;

        /// <summary>
        /// 子项模板
        /// </summary>
        private ListViewSubitem subitemTemplate => this.XGetComponentInChildren(ref _subitemTemplate);

        private ListViewSubitem CreateNewSubitem()
        {
            if (_subitemTemplate)
            {
                var newGO = _subitemTemplate.gameObject.XCloneAndSetParent(_subitemTemplate.transform.parent);
                newGO.transform.localScale = _subitemTemplate.transform.localScale;
                var newSubitem = newGO.GetComponent<ListViewSubitem>();
                newSubitem.listViewItem = this;
                return newSubitem;
            }
            return default;
        }

        /// <summary>
        /// 列表子项缓存池
        /// </summary>
        private WorkObjectPool<ListViewSubitem> subitemPool
        {
            get
            {
                if (_subitemPool == null)
                {
                    _subitemPool = new WorkObjectPool<ListViewSubitem>();
                    if (_subitemTemplate) _subitemTemplate.gameObject.SetActive(false);

                    _subitemPool.Init(() => CreateNewSubitem(),
                        item =>
                        {
                            if (item)
                            {
                                item.OnAlloc();
                            }
                        },
                        item =>
                        {
                            if (item)
                            {
                                item.OnFree();
                            }
                        },
                        item => item, false);
                }
                return _subitemPool;
            }
        }
        private WorkObjectPool<ListViewSubitem> _subitemPool = null;

        /// <summary>
        /// 子项生成规则
        /// </summary>
        [Name("子项生成规则")]
        [EnumPopup]
        public EListItemGenerateRule _subitemGenerateRule = EListItemGenerateRule.UseObjectPool;

        private EListItemGenerateRule? subitemGenerateRuleOnEnable = null;

        private EListItemGenerateRule subitemGenerateRule => subitemGenerateRuleOnEnable ?? _subitemGenerateRule;

        private ListViewSubitem CreateSubitem(int index)
        {
            ListViewSubitem item = default;
            switch (subitemGenerateRule)
            {
                case EListItemGenerateRule.CreateNew:
                    {
                        item = CreateNewSubitem();
                        break;
                    }
                case EListItemGenerateRule.UseObjectPool:
                    {
                        item = subitemPool.Alloc();
                        break;
                    }
            }
            if (item)
            {
                item.index = index;
            }
            return item;
        }

        private bool RemoveSubitem(ListViewSubitem listViewSubitem)
        {
            switch (subitemGenerateRule)
            {
                case EListItemGenerateRule.CreateNew:
                    {
                        if (listViewSubitem)
                        {
                            listViewSubitem.gameObject.XDestoryObject();
                            return true;
                        }
                        break;
                    }
                case EListItemGenerateRule.UseObjectPool:
                    {
                        subitemPool.Free(listViewSubitem);
                        return true;
                    }
            }
            return false;
        }

        private List<ListViewSubitem> dispalyListViewSubitems = new List<ListViewSubitem>();

        #endregion

        #region 子项关键字模版

        /// <summary>
        /// 当子项关键字生成时项索引条件
        /// </summary>
        [Name("当子项关键字生成时项索引条件")]
        [Tip("当索引值为-1时，标识所有的项都期望根据子项关键字模版做子项关键字生成；否则必须索引值相等匹配时，才做对应子项关键字生成操作；", "When the index value is -1, it indicates that all items are expected to be generated according to the sub key template; Otherwise, only when the index values are equal and matched, can the corresponding sub item keyword generation operation be performed;")]
        public IntPropertyValue _itemConditionIndexOnSubitemKeyGenerate = new IntPropertyValue();

        /// <summary>
        /// 子项关键字模版
        /// </summary>
        [Name("子项关键字模版")]
        public ListViewSubitem _subitemKeyTemplate;

        /// <summary>
        /// 子项关键字模版
        /// </summary>
        private ListViewSubitem subitemKeyTemplate => this.XGetComponentInChildren(ref _subitemKeyTemplate);

        private ListViewSubitem CreateNewSubitemKey()
        {
            if (_subitemKeyTemplate)
            {
                var newGO = _subitemKeyTemplate.gameObject.XCloneAndSetParent(_subitemKeyTemplate.transform.parent);
                newGO.transform.localScale = _subitemKeyTemplate.transform.localScale;
                var newSubitemKey = newGO.GetComponent<ListViewSubitem>();
                newSubitemKey.listViewItem = this;
                return newSubitemKey;
            }
            return default;
        }

        /// <summary>
        /// 子项关键字缓存池
        /// </summary>
        private WorkObjectPool<ListViewSubitem> subitemKeyPool
        {
            get
            {
                if (_subitemKeyPool == null)
                {
                    _subitemKeyPool = new WorkObjectPool<ListViewSubitem>();
                    if (_subitemKeyTemplate) _subitemKeyTemplate.gameObject.SetActive(false);

                    _subitemKeyPool.Init(() => CreateNewSubitemKey(),
                        item =>
                        {
                            if (item)
                            {
                                item.OnAlloc();
                            }
                        },
                        item =>
                        {
                            if (item)
                            {
                                item.OnFree();
                            }
                        },
                        item => item, false);
                }
                return _subitemKeyPool;
            }
        }
        private WorkObjectPool<ListViewSubitem> _subitemKeyPool = null;

        /// <summary>
        /// 子项关键字生成规则
        /// </summary>
        [Name("子项关键字生成规则")]
        [EnumPopup]
        public EListItemGenerateRule _subitemKeyGenerateRule = EListItemGenerateRule.UseObjectPool;

        private EListItemGenerateRule? subitemKeyGenerateRuleOnEnable = null;

        private EListItemGenerateRule subitemKeyGenerateRule => subitemKeyGenerateRuleOnEnable ?? _subitemKeyGenerateRule;

        private ListViewSubitem CreateSubitemKey(int index)
        {
            ListViewSubitem item = default;
            switch (subitemKeyGenerateRule)
            {
                case EListItemGenerateRule.CreateNew:
                    {
                        item = CreateNewSubitemKey();
                        break;
                    }
                case EListItemGenerateRule.UseObjectPool:
                    {
                        item = subitemKeyPool.Alloc();
                        break;
                    }
            }
            if (item)
            {
                item.index = index;
            }
            return item;
        }

        private bool RemoveSubitemKey(ListViewSubitem listViewSubitemKey)
        {
            switch (subitemKeyGenerateRule)
            {
                case EListItemGenerateRule.CreateNew:
                    {
                        if (listViewSubitemKey)
                        {
                            listViewSubitemKey.gameObject.XDestoryObject();
                            return true;
                        }
                        break;
                    }
                case EListItemGenerateRule.UseObjectPool:
                    {
                        subitemKeyPool.Free(listViewSubitemKey);
                        return true;
                    }
            }
            return false;
        }

        private List<ListViewSubitem> dispalyListViewSubitemKeys = new List<ListViewSubitem>();

        #endregion

        /// <summary>
        /// 更新子项
        /// </summary>
        private void UpdateSubitems()
        {
            if (index >= 0 && _model != null)//创建
            {
                if (_subitemKeyTemplate)
                {
                    var conditionIndex= _itemConditionIndexOnSubitemKeyGenerate.GetValue();
                    if (conditionIndex == index || conditionIndex == -1)
                    {
                        int i = 0;
                        foreach (var key in _model.keys)
                        {
                            var newSubitemKey = CreateSubitemKey(i++);
                            newSubitemKey.SetKey(key);
                            dispalyListViewSubitemKeys.Add(newSubitemKey);
                        }
                    }
                }

                if (_subitemTemplate)
                {
                    int i = 0;
                    foreach (var key in _model.keys)
                    {
                        var newSubitem = CreateSubitem(i++);
                        newSubitem.SetKey(key);
                        dispalyListViewSubitems.Add(newSubitem);
                    }
                }
            }
            else//销毁
            {
                foreach (var item in dispalyListViewSubitems)
                {
                    RemoveSubitem(item);
                }
                dispalyListViewSubitems.Clear();


                foreach (var item in dispalyListViewSubitemKeys)
                {
                    RemoveSubitemKey(item);
                }
                dispalyListViewSubitemKeys.Clear();
            }   
        }

        /// <summary>
        /// 设置模型回调函数
        /// </summary>
        [Name("设置模型回调函数")]
        public CustomFunction _customFunction = new CustomFunction();
    }

    /// <summary>
    /// 列表项生成规则
    /// </summary>
    public enum EListItemGenerateRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 创建新对象
        /// </summary>
        [Name("创建新对象")]
        CreateNew,

        /// <summary>
        /// 使用对象池
        /// </summary>
        [Name("使用对象池")]
        UseObjectPool,
    }

    /// <summary>
    /// 模型源
    /// </summary>
    [Name("模型源")]
    public enum EModelSource
    {
        /// <summary>
        /// 模型
        /// </summary>
        [Name("模型")]
        Model,

        /// <summary>
        /// 模型实体
        /// </summary>
        [Name("模型实体")]
        ModelEntity,
    }
}
