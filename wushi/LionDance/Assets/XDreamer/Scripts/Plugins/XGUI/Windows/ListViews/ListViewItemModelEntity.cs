using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginXGUI.Windows.ListViews
{
    /// <summary>
    /// 列表视图项实体
    /// 1、当【列表视图项】从列表中移出时，将被具象化为【列表视图项实体】
    /// 2、【列表视图项实体】具备指向Model对象的一个引用，通过该引用可反向知道列表视图，它是代表着列表视图对实体的拥有关系    
    /// </summary>
    [Name("列表视图项实体")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Tool(XGUICategory.DataView, nameof(InteractableVirtual))]
    [RequireManager(typeof(XGUIManager))]
    [DisallowMultipleComponent]
    public abstract class ListViewItemModelEntity : InteractProvider
    {
        /// <summary>
        /// 模型
        /// </summary>
        public abstract ListViewItemModel model { get; }

        /// <summary>
        /// 实例模型:与ListViewItem强相关的model
        /// </summary>
        public ListViewItemModel instanceModel { get; set; }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            var listView = UnityObjectExtension.XGetComponentInGlobal<ListView>();
            this.AddTagWithDistinct(ListView.ListViewNameTag, listView ? listView.name : "");
            this.AddTagWithDistinct(ListView.ListViewItemGroupNameTag, name);
        }
    }
}
