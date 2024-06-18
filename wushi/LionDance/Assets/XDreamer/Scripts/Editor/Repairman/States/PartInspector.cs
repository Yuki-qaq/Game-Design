using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorRepairman.Inspectors;
using XCSJ.PluginRepairman.States;

namespace XCSJ.EditorRepairman.States
{
    /// <summary>
    /// 零件检查器
    /// </summary>
    [Name("零件检查器")]
    [CustomEditor(typeof(Part), true)]
    public class PartInspector : ItemInspector
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (stateComponent is Part p && p && p.interactPart) { }
        }
    }
}
