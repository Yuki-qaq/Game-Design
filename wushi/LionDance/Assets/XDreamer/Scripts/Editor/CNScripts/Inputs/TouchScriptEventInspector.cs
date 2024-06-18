using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCNScripts.Inputs;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorCNScripts.Inputs
{
    /// <summary>
    /// 触摸脚本事件检查器
    /// </summary>
    [Name("触摸脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(TouchScriptEvent))]
    public class TouchScriptEventInspector : BaseScriptEventInspector<TouchScriptEvent, ETouchScriptEventType, TouchScriptEventFunction, TouchScriptEventFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(EditorCNScriptHelper.InputMenu + TouchScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(EditorCNScriptHelper.InputMenu + TouchScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal();
        }
    }
}
