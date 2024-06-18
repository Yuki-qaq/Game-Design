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
    /// UGUI文本输入框脚本事件检查器
    /// </summary>
    [Name("UGUI文本输入框脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(UGUIInputFieldScriptEvent))]
    public class UGUIInputFieldScriptEventInspector : BaseScriptEventInspector<UGUIInputFieldScriptEvent, EUGUIInputFieldScriptEventType, UGUIInputFieldScriptEventFunction, UGUIInputFieldScriptEventFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(EditorCNScriptHelper.UGUIMenu + UGUIInputFieldScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(EditorCNScriptHelper.UGUIMenu + UGUIInputFieldScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<InputField>();
        }
    }
}
