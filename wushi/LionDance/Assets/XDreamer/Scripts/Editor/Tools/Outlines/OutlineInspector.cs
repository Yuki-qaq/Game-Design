using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginTools.Outlines;
using XCSJ.EditorCommonUtils;

namespace XCSJ.EditorTools.Outlines
{
    /// <summary>
    /// 轮廓线检查器
    /// </summary>
    [Name("轮廓线检查器")]
    [CustomEditor(typeof(Outline))]
    public class OutlineInspector : MBInspector<Outline>
    {

    }

}