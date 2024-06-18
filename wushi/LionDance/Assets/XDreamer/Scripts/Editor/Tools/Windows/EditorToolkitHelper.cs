using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// 编辑工具包组手
    /// </summary>
    public static class EditorToolkitHelper
    {
        /// <summary>
        /// 游戏对象字段
        /// </summary>
        /// <param name="goContent"></param>
        /// <param name="gameObject"></param>
        /// <param name="useSelectionContent"></param>
        /// <param name="useSelection"></param>
        /// <param name="afterGO"></param>
        /// <param name="afterUseSelection"></param>
        /// <returns></returns>
        public static GameObject GameObjectField(GUIContent goContent, GameObject gameObject, GUIContent useSelectionContent, ref bool useSelection, Action afterGO = null, Action afterUseSelection = null)
        {
            EditorGUILayout.BeginHorizontal();
            var cgo = EditorGUILayout.ObjectField(goContent, gameObject, typeof(GameObject), true) as GameObject;
            afterGO?.Invoke();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            useSelection = EditorGUILayout.Toggle(useSelectionContent, useSelection);
            afterUseSelection?.Invoke();
            EditorGUILayout.EndHorizontal();

            return useSelection ? Selection.activeObject as GameObject : cgo;
        }
    }
}
