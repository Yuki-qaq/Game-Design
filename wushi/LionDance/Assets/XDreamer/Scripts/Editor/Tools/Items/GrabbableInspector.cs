using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginTools.Items;

namespace XCSJ.EditorTools.Items
{
    /// <summary>
    /// 可抓对象检查器
    /// </summary>
    [Name("可抓对象检查器")]
    [CustomEditor(typeof(Grabbable), true)]
    [CanEditMultipleObjects]
    public class GrabbableInspector : InteractableVirtualInspector<Grabbable>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(Grabbable)));

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CategoryListExtension.DrawVertical(categoryList);
        }
    }
}
