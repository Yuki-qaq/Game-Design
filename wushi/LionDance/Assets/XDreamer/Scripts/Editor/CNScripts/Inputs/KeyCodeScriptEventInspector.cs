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
    /// 键码脚本事件检查器
    /// </summary>
    [Name("键码脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(KeyCodeScriptEvent))]
    public class KeyCodeScriptEventInspector : BaseScriptEventInspector<KeyCodeScriptEvent, KeyCode, KeyCodeScriptEventFunction, KeyCodeScriptEventFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(EditorCNScriptHelper.InputMenu + KeyCodeScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(EditorCNScriptHelper.InputMenu + KeyCodeScriptEvent.Title, true)]
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
            EditorGUILayout.LabelField(new GUIContent("特别注意:LeftCommand与LeftWindows对应相同的回调事件！仅回调 LeftCommand 事件；", "在执行时仅回调 LeftCommand  事件"), UICommonOption.labelYellowBG, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(new GUIContent("特别注意:RightCommand与RightApple对应相同的回调事件！仅回调 RightCommand 事件；", "在执行时仅回调 RightCommand  事件"), UICommonOption.labelYellowBG, GUILayout.ExpandWidth(true));
            base.OnInspectorGUI();
        }
    }
}
