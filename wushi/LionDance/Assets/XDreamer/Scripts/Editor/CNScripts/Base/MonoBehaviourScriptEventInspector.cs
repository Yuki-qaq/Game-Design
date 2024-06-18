using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCNScripts.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorCNScripts.Base
{
    /// <summary>
    /// MonoBehaviour脚本事件检查器
    /// </summary>
    [Name("MonoBehaviour脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(MonoBehaviourScriptEvent))]
    public class MonoBehaviourScriptEventInspector : BaseScriptEventInspector<MonoBehaviourScriptEvent, EMonoBehaviourScriptEventType, MonoBehaviourScriptEventFunction, MonoBehaviourScriptEventFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(XDreamerMenu.ScriptEvent + MonoBehaviourScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(XDreamerMenu.ScriptEvent + MonoBehaviourScriptEvent.Title, true)]
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
            EditorGUILayout.LabelField(new GUIContent("过多使用本组件，影响效率(推荐使用简版)！请谨慎使用^_^", "过多使用本组件，影响效率！请谨慎使用^_^"), UICommonOption.labelYellowBG, GUILayout.ExpandWidth(true));
            base.OnInspectorGUI();
        }
    }
}
