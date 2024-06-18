using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.Windows.ListViews
{
    /// <summary>
    /// 基础列表视图模型提供器
    /// </summary>
    [RequireManager(typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    [RequireComponent(typeof(ListView))]
    public abstract class BaseListViewModelProvider : View
    {
        private ListView _listView;

        /// <summary>
        /// 列表视图
        /// </summary>
        public ListView listView
        {
            get
            {
                if (!_listView)
                {
                    _listView = this.XGetOrAddComponent<ListView>();
                }
                return _listView;
            }
        }
    }

    /// <summary>
    /// 列表视图模型提供器
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.List)]
    [Tool(XGUICategory.ListView, nameof(ListViewModelProvider), rootType = typeof(XGUIManager))]
    public abstract class ListViewModelProvider : BaseListViewModelProvider
    {
        /// <summary>
        /// 预制模型：启用时加载
        /// </summary>
        protected virtual IEnumerable<ListViewItemModel> prefabModels => Empty<ListViewItemModel>.Array;

        /// <summary>
        /// 模型列表
        /// </summary>
        public virtual List<ListViewItemModel> models { get; protected set; } = new List<ListViewItemModel>();

        private EModelType listViewModelTypeOnEnable = EModelType.FieldPropertyMethodBinder;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            listViewModelTypeOnEnable = listView._modelType;

            if (listViewModelTypeOnEnable == EModelType.DrivedByOutside)
            {
                listView.AddListViewModelProvider(this);

                models.AddRangeWithDistinct(prefabModels);
                AddModelsOnEnable();
                listView.Refresh();
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (listViewModelTypeOnEnable == EModelType.DrivedByOutside)
            {
                RemoveModelsOnDisable();
                models.Clear();//执行清空操作
                listView.RemoveListViewModelProvider(this);
                listView.Refresh();
            }
        }

        /// <summary>
        /// 启用时添加模型
        /// </summary>
        protected virtual void AddModelsOnEnable() => listView.AddModel(models);

        /// <summary>
        /// 禁用时移除模型
        /// </summary>
        protected virtual void RemoveModelsOnDisable() => listView.RemoveModel(models);

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => interactData is ListViewInteractData;

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="listViewInteractData"></param>
        internal virtual void OnClick(ListViewInteractData listViewInteractData) { }

        /// <summary>
        /// 拖拽开始
        /// </summary>
        /// <param name="listViewInteractData"></param>
        internal virtual void OnDragStart(ListViewInteractData listViewInteractData) { }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="listViewInteractData"></param>
        internal virtual void OnDrag(ListViewInteractData listViewInteractData) { }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        /// <param name="listViewInteractData"></param>
        internal virtual void OnDragEnd(ListViewInteractData listViewInteractData) { }
    }
}
