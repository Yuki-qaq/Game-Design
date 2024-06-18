using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.ComponentModel;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.Windows.ListViews
{
    /// <summary>
    /// 列表视图:用于接收数据模型并实例化为UI列表项
    /// </summary>
    [Name("列表视图")]
    [XCSJ.Attributes.Icon(EIcon.List)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    [Tool(XGUICategory.ListView, rootType = typeof(XGUIManager), index = IndexAttribute.DefaultIndex - 1)]
    public sealed class ListView : ObjectMemberView, IPropertyKeyProvider
    {
        #region 列表视图标签

        /// <summary>
        /// 列表视图名称标签
        /// </summary>
        [PropertyKey]
        public const string ListViewNameTag = "列表视图名称";

        /// <summary>
        /// 列表视图项组名称标签
        /// </summary>
        [PropertyKey]
        public const string ListViewItemGroupNameTag = "列表视图项组名称";

        /// <summary>
        /// 列表视图第一个标签值
        /// </summary>
        public string listViewTagFirstValue => _tagProperty.GetFirstValue(_listViewTagNameKeys);

        /// <summary>
        /// 列表视图名称关键字列表
        /// </summary>
        [Name("列表视图名称关键字列表")]
        public List<string> _listViewTagNameKeys = new List<string>();

        /// <summary>
        /// 列表视图项组名称关键字列表
        /// </summary>
        [Name("列表视图项组名称关键字列表")]
        public List<string> _listViewItemGroupTagNameKeys = new List<string>();

        /// <summary>
        /// 属性关键字信息
        /// </summary>
        public List<PropertyKeyInfo> propertyKeyInfos
        {
            get
            {
                var className = CommonFun.Name(typeof(ListView));
                var propertyKeyName = CommonFun.Name(typeof(ListView), nameof(ListView._listViewTagNameKeys));

                var list = new List<PropertyKeyInfo>();
                foreach (var item in _listViewTagNameKeys)
                {
                    list.Add(new PropertyKeyInfo(className, propertyKeyName, item));
                }
                propertyKeyName = CommonFun.Name(typeof(ListView), nameof(ListView._listViewItemGroupTagNameKeys));
                foreach (var item in _listViewItemGroupTagNameKeys)
                {
                    list.Add(new PropertyKeyInfo(className, propertyKeyName, item));
                }
                return list;
            }
        }

        /// <summary>
        /// 匹配列表视图名称标签
        /// </summary>
        /// <param name="tagPropertyHost"></param>
        /// <returns></returns>
        public bool IsMatchListViewNameTag(ITagPropertyHost tagPropertyHost) => this.ExistsSameTagKeyValue(_listViewTagNameKeys, tagPropertyHost);

        #endregion

        #region Unity 消息

        /// <summary>
        /// 重置命令
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _modelToViewDataUpdateRule = EDataUpdateRule.Trigger;

            _listViewTagNameKeys.Add(ListViewNameTag);
            _listViewItemGroupTagNameKeys.Add(ListViewItemGroupNameTag);
            _tagProperty.AddTagWithDistinct(ListViewNameTag, name);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            listItemGenerateRuleOnEnable = _listItemGenerateRule;

            if (template) { }

            if (itemPool != null) { }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

        }

        /// <summary>
        /// 当模型到视图
        /// </summary>
        /// <returns></returns>
        protected override bool OnModelToView()
        {
            var rs = base.OnModelToView();

            Refresh();

            return rs;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            // 移除
            foreach (var model in displayModels)
            {
                RemoveItem(model);
            }
            displayModels.Clear();

            // 获取数据
            if (_modelType == EModelType.DrivedByOutside)
            {
                displayModels.AddRange(drivedByOutsideModels);
            }
            else
            {
                if (modelValue is IList list)
                {
                    listHost = modelMainObject as IListHost;
                    refreshList = list;
                    foreach (var item in refreshList)
                    {
                        displayModels.Add(new ListViewItemModel(item));
                    }
                }
            }

            // 添加
            for (int i = 0; i < displayModels.Count; i++)
            {
                CreateItem(i, displayModels[i]);
            }
        }

        /// <summary>
        /// 列表宿主：刷新列表视图的模型选择对象
        /// </summary>
        public IListHost listHost { get; private set; }

        /// <summary>
        /// 刷新列表：刷新列表视图的列表
        /// </summary>
        public IList refreshList { get; private set; }

        #endregion

        #region 交互处理

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => interactData is ListViewInteractData data;

        /// <summary>
        /// 当交互时
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteract(InteractData interactData)
        {
            var rs = base.OnInteract(interactData);
            if (_modelType != EModelType.DrivedByOutside && modelMainObject is IListHost listHost)
            {
                listHost.OnViewEvent(interactData as ViewInteractData);
            }
            return rs;
        }

        #endregion

        #region 交互命令

        /// <summary>
        /// 点击
        /// </summary>
        [InteractCmd]
        [Name("点击")]
        public void Click() => TryInteract(nameof(Click));

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="listViewInteractData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Click))]
        public EInteractResult Click(ListViewInteractData listViewInteractData)
        {
            if (listViewInteractData == null) return EInteractResult.Fail;

            TrySelectModel(listViewInteractData.listViewItemModel);
            foreach (var item in listViewModelProviders)
            {
                item.OnClick(listViewInteractData);
            }
            return EInteractResult.Success;
        }

        /// <summary>
        /// 选择
        /// </summary>
        [InteractCmd]
        [Name("选择")]
        public void Select() => TryInteract(nameof(Select));

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="listViewInteractData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Select))]
        public EInteractResult Select(ListViewInteractData listViewInteractData)
        {
            if (listViewInteractData == null) return EInteractResult.Fail;

            var model = listViewInteractData.listViewItemModel;
            if (model == null || selectedDisplayModel == model || !displayModels.Contains(model)) return EInteractResult.Fail;

            // 上次记录选择模型设置为不选中
            TryUnselectModel(selectedDisplayModel);

            // 将当前模型设置为选中
            selectedDisplayModel = model;
            selectedDisplayModel.selected = true;
            return EInteractResult.Success;
        }

        /// <summary>
        /// 取消选择
        /// </summary>
        [InteractCmd]
        [Name("取消选择")]
        public void Unselect() => TryInteract(nameof(Unselect));

        /// <summary>
        /// 取消选择
        /// </summary>
        /// <param name="listViewInteractData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Unselect))]
        public EInteractResult Unselect(ListViewInteractData listViewInteractData)
        {
            if (listViewInteractData == null) return EInteractResult.Fail;

            var model = listViewInteractData.listViewItemModel;
            if (model == null) return EInteractResult.Fail;

            model.selected = false;
            if (model == selectedDisplayModel)
            {
                selectedDisplayModel = null;
            }
            return EInteractResult.Success;
        }


        /// <summary>
        /// 拖拽开始
        /// </summary>
        [InteractCmd]
        [Name("拖拽开始")]
        public void DragStart() => TryInteract(nameof(DragStart));

        /// <summary>
        /// 拖拽开始
        /// </summary>
        /// <param name="listViewInteractData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(DragStart))]
        public EInteractResult DragStart(ListViewInteractData listViewInteractData)
        {
            if (listViewInteractData == null) return EInteractResult.Fail;

            foreach (var item in listViewModelProviders)
            {
                item.OnDragStart(listViewInteractData);
            }
            return EInteractResult.Success;
        }

        /// <summary>
        /// 拖拽进行中
        /// </summary>
        [InteractCmd]
        [Name("拖拽进行中")]
        public void Draging() => TryInteract(nameof(Draging));

        /// <summary>
        /// 拖拽进行中
        /// </summary>
        /// <param name="listViewInteractData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Draging))]
        public EInteractResult Draging(ListViewInteractData listViewInteractData)
        {
            if (listViewInteractData == null) return EInteractResult.Fail;

            foreach (var item in listViewModelProviders)
            {
                item.OnDrag(listViewInteractData);
            }
            return EInteractResult.Success;
        }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        [InteractCmd]
        [Name("拖拽结束")]
        public void DragEnd() => TryInteract(nameof(DragEnd));

        /// <summary>
        /// 拖拽结束
        /// </summary>
        /// <param name="listViewInteractData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(DragEnd))]
        public EInteractResult DragEnd(ListViewInteractData listViewInteractData)
        {
            if (listViewInteractData == null) return EInteractResult.Fail;

            foreach (var item in listViewModelProviders)
            {
                item.OnDragEnd(listViewInteractData);
            }
            return EInteractResult.Success;
        }

        #endregion

        #region 列表视图模型提供器

        /// <summary>
        /// 添加列表视图模型提供器
        /// </summary>
        /// <param name="listViewModelProvider"></param>
        public void AddListViewModelProvider(ListViewModelProvider listViewModelProvider) => listViewModelProviders.Add(listViewModelProvider);

        /// <summary>
        /// 移除列表视图模型提供器
        /// </summary>
        /// <param name="listViewModelProvider"></param>
        public void RemoveListViewModelProvider(ListViewModelProvider listViewModelProvider) => listViewModelProviders.Remove(listViewModelProvider);

        #endregion

        #region 模型值为IList接口的模型

        private List<ListViewItemModel> listItemModels => displayModels;

        #endregion

        #region 显示模型

        /// <summary>
        /// 模型列表
        /// </summary>
        public List<ListViewItemModel> displayModels { get; private set; } = new List<ListViewItemModel>();

        /// <summary>
        /// 当前选择表数据模型
        /// </summary>
        public ListViewItemModel selectedDisplayModel { get; private set; } = null;

        /// <summary>
        /// 项数量
        /// </summary>
        public int itemCount => displayModels.Count;

        private List<ListViewModelProvider> listViewModelProviders = new List<ListViewModelProvider>();

        /// <summary>
        /// 选择模型
        /// </summary>
        /// <param name="model"></param>
        public bool TrySelectModel(ListViewItemModel model)
        {
            return model != null && TryInteract(new ListViewInteractData(model, GetInCmdName(nameof(Select)), this, null), out _);
        }

        /// <summary>
        /// 取消选择
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool TryUnselectModel(ListViewItemModel model)
        {
            return model != null && TryInteract(new ListViewInteractData(model, GetInCmdName(nameof(Unselect)), this, null), out _);
        }

        /// <summary>
        /// 包含数据
        /// </summary>
        /// <param name="listViewItemModel"></param>
        /// <returns></returns>
        public bool Contains(ListViewItemModel listViewItemModel) => displayModels.Contains(listViewItemModel);

        #endregion

        #region 外部驱动模型

        private List<ListViewItemModel> drivedByOutsideModels = new List<ListViewItemModel>();

        /// <summary>
        /// 添加模型
        /// </summary>
        /// <param name="models"></param>
        public void AddModel(IEnumerable<ListViewItemModel> models) => models.Foreach(m => AddModel(m));

        /// <summary>
        /// 添加模型
        /// </summary>
        /// <param name="model"></param>
        public void AddModel(ListViewItemModel model) => drivedByOutsideModels.AddWithDistinct(model);

        /// <summary>
        /// 移除模型
        /// </summary>
        /// <param name="models"></param>
        public void RemoveModel(IEnumerable<ListViewItemModel> models) => models.Foreach(m => RemoveModel(m));

        /// <summary>
        /// 移除模型
        /// </summary>
        /// <param name="model"></param>
        public void RemoveModel(ListViewItemModel model) => drivedByOutsideModels.Remove(model);

        #endregion

        #region 视图

        /// <summary>
        /// 列表项模板
        /// </summary>
        [Group("视图设置", textEN = "View Settings")]
        [Name("列表项模板")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public ListViewItem _template;

        /// <summary>
        /// 列表项模板
        /// </summary>
        private ListViewItem template => this.XGetComponentInChildrenOrGlobal(ref _template);

        /// <summary>
        /// 列表项生成规则
        /// </summary>
        [Name("列表项生成规则")]
        [EnumPopup]
        public EListItemGenerateRule _listItemGenerateRule = EListItemGenerateRule.UseObjectPool;

        private EListItemGenerateRule? listItemGenerateRuleOnEnable = null;

        private EListItemGenerateRule rule => listItemGenerateRuleOnEnable.HasValue ? listItemGenerateRuleOnEnable.Value : _listItemGenerateRule;

        /// <summary>
        /// 列表项缓存池
        /// </summary>
        private WorkObjectPool<ListViewItem> itemPool
        {
            get
            {
                if (_itemPool == null)
                {
                    _itemPool = new WorkObjectPool<ListViewItem>();
                    if (_template) _template.gameObject.SetActive(false);

                    _itemPool.Init(() => CreateNewItem(),
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
                                item.SetModel(-1, null);
                                item.OnFree();
                            }
                        },
                        item => item, false);
                }
                return _itemPool;
            }
        }
        private WorkObjectPool<ListViewItem> _itemPool = null;

        private ListViewItem CreateNewItem()
        {
            if (_template)
            {
                var newGO = _template.gameObject.XCloneAndSetParent(_template.transform.parent);
                newGO.transform.localScale = _template.transform.localScale;
                var newItem = newGO.GetComponent<ListViewItem>();
                newItem.listView = this;
                return newItem;
            }
            return default;
        }

        private ListViewItem CreateItem(int index, ListViewItemModel model)
        {
            ListViewItem item = default;
            switch (rule)
            {
                case EListItemGenerateRule.CreateNew:
                    {
                        item = CreateNewItem();
                        break;
                    }
                case EListItemGenerateRule.UseObjectPool:
                    {
                        item = itemPool.Alloc();
                        break;
                    }
            }
            if (item)
            {
                item.SetModel(index, model);
            }
            return item;
        }

        private bool RemoveItem(ListViewItemModel model)
        {
            switch (rule)
            {
                case EListItemGenerateRule.CreateNew:
                    {
                        if (model.listViewItem)
                        {
                            model.listViewItem.gameObject.XDestoryObject();
                            return true;
                        }
                        break;
                    }
                case EListItemGenerateRule.UseObjectPool:
                    {
                        itemPool.Free(model.listViewItem);
                        return true;
                    }
            }
            return false;
        }

        /// <summary>
        /// 项边框选中色：当前值与Unity编辑器选中对象颜色保持一致
        /// </summary>
        [Name("项边框选中色")]
        public Color _selectedColor = new Color(1, 0.4f, 0, 1);

        /// <summary>
        /// 项边框未选中色
        /// </summary>
        [Name("项边框未选中色")]
        public Color _unselectedColor = Color.black;

        /// <summary>
        /// 当子级视图事件:由子级向父级传导视图发生的各种交互事件；
        /// </summary>
        /// <param name="viewInteractData">视图交互数据</param>
        public override void OnChildViewEvent(ViewInteractData viewInteractData)
        {
            onChildInteractEvent?.Invoke(this, viewInteractData);
            base.OnChildViewEvent(viewInteractData);
        }

        /// <summary>
        /// 当子级交互事件
        /// </summary>
        public static event Action<ListView, ViewInteractData> onChildInteractEvent;

        #endregion
    }

    #region 列表视图交互数据

    /// <summary>
    /// 列表视图交互数据
    /// </summary>
    public class ListViewInteractData : ViewInteractData
    {
        /// <summary>
        /// 模型
        /// </summary>
        public override object model => listViewItemModel;

        /// <summary>
        /// 模型实体
        /// </summary>
        public override object modelEntity => listViewItemModel?.modelEntity;

        /// <summary>
        /// 视图
        /// </summary>
        public override IView view => listView;

        /// <summary>
        /// 视图实体
        /// </summary>
        public override IView viewEntity => listViewItem;

        /// <summary>
        /// 列表视图
        /// </summary>
        public ListView listView => interactor as ListView;

        /// <summary>
        /// 列表视图项
        /// </summary>
        public ListViewItem listViewItem { get; private set; }

        /// <summary>
        /// 列表视图项模型
        /// </summary>
        public ListViewItemModel listViewItemModel { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ListViewInteractData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="listViewItemModel"></param>
        /// <param name="cmdName"></param>
        /// <param name="listView"></param>
        /// <param name="cmdParam"></param>
        /// <param name="interactables"></param>
        public ListViewInteractData(ListViewItemModel listViewItemModel, string cmdName, ListView listView, object cmdParam, params InteractObject[] interactables) : base(cmdName, listView, cmdParam, interactables)
        {
            this.listViewItemModel = listViewItemModel;
            this.listViewItem = listViewItemModel.listViewItem;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="listViewItem"></param>
        /// <param name="cmdName"></param>
        /// <param name="cmdParam"></param>
        /// <param name="interactables"></param>
        public ListViewInteractData(PointerEventData pointerEventData, ListViewItem listViewItem, string cmdName, object cmdParam, params InteractObject[] interactables) : base(pointerEventData, cmdName, listViewItem ? listViewItem.listView : default, cmdParam, interactables)
        {
            this.listViewItem = listViewItem;
            this.listViewItemModel = listViewItem.model;
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="interactData"></param>
        public override void CopyTo(InteractData interactData)
        {
            base.CopyTo(interactData);

            if (interactData is ListViewInteractData listViewInteractData)
            {
                listViewInteractData.listViewItem = listViewItem;
                listViewInteractData.listViewItemModel = listViewItemModel;
            }
        }
    }

    #endregion
}
