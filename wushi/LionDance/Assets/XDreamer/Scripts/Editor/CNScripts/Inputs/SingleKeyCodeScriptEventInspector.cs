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
    /// 单一按键脚本事件检查器
    /// </summary>
    [Name("单一按键脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(SingleKeyCodeScriptEvent))]
    public class SingleKeyCodeScriptEventInspector : BaseScriptEventInspector<SingleKeyCodeScriptEvent, ESingleKeyCodeScriptEventType, SingleKeyCodeScriptEventFunction, SingleKeyCodeScriptEventFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(EditorCNScriptHelper.InputMenu + SingleKeyCodeScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(EditorCNScriptHelper.InputMenu + SingleKeyCodeScriptEvent.Title, true)]
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
            EditorGUILayout.LabelField(new GUIContent("特别注意:LeftCommand与LeftWindows对应相同的回调事件！仅回调 LeftCommand 事件；", "在执行时仅回调 LeftCommand  事件"), UICommonOption.labelYellowBG, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(new GUIContent("特别注意:RightCommand与RightApple对应相同的回调事件！仅回调 RightCommand 事件；", "在执行时仅回调 RightCommand  事件"), UICommonOption.labelYellowBG, GUILayout.ExpandWidth(true));

            EditorGUILayout.Separator();
            base.OnInspectorGUI();
        }
    }
}
