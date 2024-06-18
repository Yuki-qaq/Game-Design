using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.Windows.ListViews
{
    /// <summary>
    /// 列表视图子项
    /// </summary>
    [Name("列表视图子项")]
    [RequireComponent(typeof(RectTransform))]
    public sealed class ListViewSubitem : DraggableView, IViewItem
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
            this.index = -1;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 可拖拽
        /// </summary>
        public override bool canDrag { get => false; set => base.canDrag = value; }

        /// <summary>
        /// 模型视图
        /// </summary>
        [Name("模型视图")]
        [ComponentPopup]
        public BaseModelView _modelView;

        /// <summary>
        /// 模型视图
        /// </summary>
        public BaseModelView modelView => this.XGetComponent(ref _modelView);

        /// <summary>
        /// 列表视图项
        /// </summary>
        private ListViewItem _listViewItem;

        /// <summary>
        /// 列表视图项
        /// </summary>
        public ListViewItem listViewItem
        {
            get => _listViewItem;
            set
            {
                if (_listViewItem && _listViewItem != value)
                {
                    Debug.Log("");
                    return;
                }
                _listViewItem = value;
            }
        }

        /// <summary>
        /// 列表视图项模型
        /// </summary>
        public ListViewItemModel listViewItemModel => listViewItem ? listViewItem.model : default;

        /// <summary>
        /// 模型实体
        /// </summary>
        public object modelEntity => listViewItemModel.modelEntity;

        /// <summary>
        /// 键
        /// </summary>
        public string key { get; private set; } = "";

        /// <summary>
        /// 模型源
        /// </summary>
        [Name("模型源")]
        [EnumPopup]
        public EModelSource _modelSource = EModelSource.Model;

        /// <summary>
        /// 设置键
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(string key)
        {
            this.key = key;
            var modelView = this.modelView;
            var model = _modelSource == EModelSource.Model ? this.listViewItemModel : this.modelEntity;
            if (modelView && modelView is IModelKeyValueHostModifier modifier)
            {
                modifier.SetModelKey(model, key);
            }
        }

        /// <summary>
        /// 索引
        /// </summary>
        public int index { get; internal set; } = -1;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (modelView) { }
        }

        /// <summary>
        /// 查找并设置父级视图
        /// </summary>
        /// <returns></returns>
        protected override View FindAndSetParentView()
        {
            var parentView = this._parentView;
            if (!parentView || !(parentView is ListViewItem))
            {
                var listViewItem = this.GetComponentInParent<ListViewItem>();
                if (listViewItem)
                {
                    this.parentView = listViewItem;
                }
            }
            return base.FindAndSetParentView();
        }
    }
}
