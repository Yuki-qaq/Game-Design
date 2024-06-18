using UnityEngine;
using System.Collections;
using UnityEditor;
using XCSJ.PluginEasyAR;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;

namespace XCSJ.EditorEasyAR
{
    /// <summary>
    /// 脚本图片目标事件检查器
    /// </summary>
    [Name("脚本图片目标事件检查器")]
    [CustomEditor(typeof(ImageTargetScriptEvent))]
    public class ScriptImageTargetEventInspector : BaseScriptEventInspector<ImageTargetScriptEvent, EImageTargetScriptEventType, ImageTargetScriptEventFunction, ImageTargetScriptEventFunctionCollection>
    {
    }
}
