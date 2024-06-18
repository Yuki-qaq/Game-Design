using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 检查器窗口
    /// </summary>
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(InspectorWindow))]
    public class InspectorWindow : EditorWindow_LinkType<InspectorWindow>
    {
    }
}
