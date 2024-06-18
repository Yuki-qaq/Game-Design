using System;
using UnityEditor;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCNScripts.Inputs;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCNScripts.Base;
using XCSJ.Attributes;

namespace XCSJ.EditorCNScripts.Base
{
    /// <summary>
    /// MonoBehaviour脚本事件简版检查器
    /// </summary>
    [Name("MonoBehaviour脚本事件简版检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(MonoBehaviourScriptEventLite))]
    public class MonoBehaviourScriptEventLiteInspector : BaseScriptEventInspector<MonoBehaviourScriptEventLite, EMonoBehaviourScriptEventLiteType, MonoBehaviourScriptEventLiteFunction, MonoBehaviourScriptEventLiteFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(XDreamerMenu.ScriptEvent + MonoBehaviourScriptEventLite.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(XDreamerMenu.ScriptEvent + MonoBehaviourScriptEventLite.Title, true)]
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
            base.OnInspectorGUI();
        }
    }
}
