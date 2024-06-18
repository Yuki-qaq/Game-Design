using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginRepairman.Tools;

namespace XCSJ.EditorRepairman.Tools
{
    /// <summary>
    /// 零件检查器
    /// </summary>
    [Name("零件检查器")]
    [CustomEditor(typeof(Part), true)]
    public class PartInspector : PartInspector<Part>
    {

    }

    /// <summary>
    /// 零件检查器模板
    /// </summary>
    public class PartInspector<T> : InteractorInspector<T> where T : Part
    {

    }
}