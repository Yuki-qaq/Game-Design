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
    /// UGUI滚动条脚本事件检查器
    /// </summary>
    [Name("UGUI滚动条脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(UGUIScrollbarScriptEvent))]
    public class UGUIScrollbarScriptEventInspector : BaseScriptEventInspector<UGUIScrollbarScriptEvent, EUGUIScrollbarScriptEventType, UGUIScrollbarScriptEventFunction, UGUIScrollbarScriptEventFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(EditorCNScriptHelper.UGUIMenu + UGUIScrollbarScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(EditorCNScriptHelper.UGUIMenu + UGUIScrollbarScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<Scrollbar>();
        }
    }
}
