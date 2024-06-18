using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.CNScripts;

namespace XCSJ.EditorGenericStandards.CNScripts
{
    /// <summary>
    /// 生成设置场景名称
    /// </summary>
    [CustomScriptParamValueDrawer(nameof(BuildSettingsSceneNames))]
    public class BuildSettingsSceneNames : ScriptParamValueDrawer
    {
        string[] sceneNames = Empty<string>.Array;

        /// <summary>
        /// 构造
        /// </summary>
        public BuildSettingsSceneNames()
        {
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
            OnSceneListChanged();
        }

        /// <summary>
        /// 析构
        /// </summary>
        ~BuildSettingsSceneNames()
        {
            EditorBuildSettings.sceneListChanged -= OnSceneListChanged;
        }

        /// <summary>
        /// 当场景列表变更
        /// </summary>
        void OnSceneListChanged()
        {
            sceneNames = EditorBuildSettings.scenes.Cast(s => Path.GetFileNameWithoutExtension(s.path)).ToArray() ?? Empty<string>.Array;
        }

        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            EditorGUI.indentLevel = 2;
            scriptParamDrawer.paramObject = UICommonFun.Popup(scriptParamDrawer.paramObject as string, sceneNames, true);
        }
    }
}