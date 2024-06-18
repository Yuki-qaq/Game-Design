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
    /// UGUI滚动视图脚本事件检查器
    /// </summary>
    [Name("UGUI滚动视图脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(UGUIScrollViewScriptEvent))]
    public class UGUIScrollViewScriptEventInspector : BaseScriptEventInspector<UGUIScrollViewScriptEvent, EUGUIScrollViewScriptEventType, UGUIScrollViewScriptEventFunction, UGUIScrollViewScriptEventFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(EditorCNScriptHelper.UGUIMenu + UGUIScrollViewScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(EditorCNScriptHelper.UGUIMenu + UGUIScrollViewScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<ScrollRect>();
        }
    }
}
