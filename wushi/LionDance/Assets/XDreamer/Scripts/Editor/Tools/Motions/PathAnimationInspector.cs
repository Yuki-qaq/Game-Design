using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.EditorSMS.States.UGUI;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Motions;

namespace XCSJ.EditorTools.Motions
{
    /// <summary>
    /// 路径动画检查器
    /// </summary>
    [CustomEditor(typeof(PathAnimation), true)]
    public class PathAnimationInspector : PlayableContentInspector<PathAnimation>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(PathAnimation._movePath):
                case nameof(PathAnimation._viewPath):
                case nameof(PathAnimation._moveTransforms):
                    {
                        EditorGUILayout.BeginHorizontal();

                        var batchGameObjectSP = serializedObject.FindProperty(nameof(PathAnimation._batchGameObject));
                        var includeSP = serializedObject.FindProperty(nameof(PathAnimation._include));
                        var chilerenSP = serializedObject.FindProperty(nameof(PathAnimation._chileren));

                        targetObject._batchGameObject = (GameObject)EditorGUILayout.ObjectField(TrLabelByTarget(nameof(targetObject._batchGameObject)), targetObject._batchGameObject, typeof(GameObject), true);
                        includeSP.boolValue = UICommonFun.ButtonToggle(TrLabelByTarget(nameof(PathAnimation._include)), includeSP.boolValue, EditorStyles.miniButtonMid, GUILayout.Width(35));
                        chilerenSP.boolValue = UICommonFun.ButtonToggle(TrLabelByTarget(nameof(PathAnimation._chileren)), chilerenSP.boolValue, EditorStyles.miniButtonRight, GUILayout.Width(35));

                        if (targetObject._batchGameObject)
                        {
                            if (includeSP.boolValue)
                            {
                                AddObjects(serializedProperty, targetObject._batchGameObject);
                            }
                            if (chilerenSP.boolValue)
                            {
                                AddObjects(serializedProperty, CommonFun.GetChildGameObjects(targetObject._batchGameObject).ToArray());
                            }
                            targetObject._batchGameObject = null;
                        }

                        EditorGUI.BeginDisabledGroup(!EditorHandler.IsLockInspectorWindow());
                        if (GUILayout.Button(ButtonClickInspector.selectGameObjectGUIContent, EditorStyles.miniButtonLeft, UICommonOption.Width80, UICommonOption.Height18))
                        {
                            AddObjects(serializedProperty, Selection.gameObjects);
                        }
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.EndHorizontal();

                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private void AddObjects(SerializedProperty objectsSP, params GameObject[] gameObjects)
        {
            if (objectsSP == null || gameObjects == null) return;

            for (int i = 0; i <= gameObjects.Length - 1; ++i)
            {
                var gameObject = gameObjects[i];
                if (!gameObject) continue;

                objectsSP.arraySize++;
                objectsSP.GetArrayElementAtIndex(objectsSP.arraySize - 1).objectReferenceValue = gameObject;
            }
        }
    }
}
