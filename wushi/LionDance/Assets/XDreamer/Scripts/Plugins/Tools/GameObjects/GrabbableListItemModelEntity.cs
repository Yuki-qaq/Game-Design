using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginXGUI.Windows.ListViews
{
    /// <summary>
    /// 可抓列表项模型实体:具备本组件的对象可放入到【可抓对象列表】中
    /// </summary>
    [Name("可抓列表项模型实体")]
    [Tip("具备本组件的对象可放入到【可抓对象列表】中", "Objects with this component can be placed in the [GrabbableList]")]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    [RequireComponent(typeof(Grabbable))]
    [Tool(XGUICategory.ListView, nameof(Grabbable))]
    public class GrabbableListItemModelEntity : ListViewItemModelEntity
    {
        /// <summary>
        /// 可抓模型
        /// </summary>
        [Name("可抓模型")]
        public SerializableGrabbableModel _grabbableModel = new SerializableGrabbableModel();

        /// <summary>
        /// 模型
        /// </summary>
        public override ListViewItemModel model => _grabbableModel;

        /// <summary>
        /// 开始时加入可抓对象列表：仅加入场景中第一个【列表视图名称】标签匹配的可抓对象列表中（需在各个交互对象启用事件监听之后进行加入列表操作）
        /// </summary>
        [Name("开始时加入可抓对象列表")]
        [Tip("仅加入场景中第一个【列表视图名称】标签匹配的可抓对象列表中", "Only add to the grabable object list that matches the first [List View Name] tag in the scene")]
        public bool _addGrabbableListOnStart = false;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _grabbableModel.title = name;
            _grabbableModel._unityObject = GetComponent<Grabbable>();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            if (_addGrabbableListOnStart)
            {
                var grabbableLists = UnityObjectExtension.GetComponentsInGlobal<GrabbableList>(false);
                foreach (var item in grabbableLists)
                {
                    if (item.listView.IsMatchListViewNameTag(this))
                    {
                        if (item.AddGrabbableListItemModelEntity(this))
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
