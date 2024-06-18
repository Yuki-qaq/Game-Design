using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCNScripts;
using XCSJ.PluginCNScripts.UGUI;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorCNScripts.UGUI
{
    /// <summary>
    /// UGUI切换脚本事件检查器
    /// </summary>
    [Name("UGUI切换脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(UGUIToggleScriptEvent))]
    public class UGUIToggleScriptEventInspector : BaseScriptEventInspector<UGUIToggleScriptEvent, EUGUIToggleScriptEventType, UGUIToggleScriptEventFunction, UGUIToggleScriptEventFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(EditorCNScriptHelper.UGUIMenu + UGUIToggleScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(EditorCNScriptHelper.UGUIMenu + UGUIToggleScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<Toggle>();
        }
    }
}
