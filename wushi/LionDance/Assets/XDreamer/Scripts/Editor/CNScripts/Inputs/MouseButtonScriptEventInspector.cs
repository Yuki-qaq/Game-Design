using System;
using UnityEditor;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCNScripts.Inputs;
using XCSJ.EditorCommonUtils;
using XCSJ.Attributes;

namespace XCSJ.EditorCNScripts.Inputs
{
    /// <summary>
    /// 鼠标按钮脚本事件检查器
    /// </summary>
    [Name("鼠标按钮脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(MouseButtonScriptEvent))]
    public class MouseButtonScriptEventInspector : BaseScriptEventInspector<MouseButtonScriptEvent, EMouseButtonScriptEventType, MouseButtonScriptEventFunction, MouseButtonScriptEventFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(EditorCNScriptHelper.InputMenu + MouseButtonScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(EditorCNScriptHelper.InputMenu + MouseButtonScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal();
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("过多使用本组件，影响效率！请谨慎使用^_^", "过多使用本组件，影响效率！请谨慎使用^_^"), UICommonOption.labelYellowBG, GUILayout.ExpandWidth(true));
            base.OnInspectorGUI();
        }
    }
}
