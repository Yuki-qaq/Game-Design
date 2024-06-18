using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.PluginXGUI.Windows.MiniMaps;

namespace XCSJ.EditorXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 导航图项检查器
    /// </summary>
    [Name("导航图项检查器")]
    [CustomEditor(typeof(MiniMapItem))]
    [CanEditMultipleObjects]
    public class MiniMapItemInspector : InteractObjectInspector<MiniMapItem>
    {
    }
}
