using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginTools.Inputs;

namespace XCSJ.EditorTools.Inputs
{
    /// <summary>
    /// 键码输入检查器
    /// </summary>
    [Name("键码输入检查器")]
    [CustomEditor(typeof(KeyCodeInput))]
    public class KeyCodeInputInspector : InteractorInspector<KeyCodeInput>
    {
    }
}