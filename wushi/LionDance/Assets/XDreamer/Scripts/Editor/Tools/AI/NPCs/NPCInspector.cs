using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginTools.AI.NPCs;

namespace XCSJ.EditorTools.AI.NPCs
{
    /// <summary>
    /// NPC检查器
    /// </summary>
    [Name("NPC检查器")]
    [CustomEditor(typeof(NPC))]
    public class NPCInspector : InteractorInspector<NPC>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(NPCAction)));

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
