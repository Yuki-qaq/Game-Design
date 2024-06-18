using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils.NodeKit;

namespace XCSJ.EditorExtension.Base.Interactions.NodeKit.Nodes
{
    /// <summary>
    /// 交互输入节点视图
    /// </summary>
    [NodeView(typeof(InteractorInput))]
    [XCSJ.Attributes.Icon(EIcon.Import)]
    public class InteractorInputNodeView : NodeView<InteractorInput>
    {
        Interactor interactor;

        SerializedObject serializedObject;
        SerializedProperty serializedPropertyArray;
        SerializedProperty serializedProperty;

        int serializedPropertyIndex = -1;
        string serializedPropertyIndexString = "";
        int arraySize = 0;

        /// <summary>
        /// 显示名
        /// </summary>
        public override string displayName => serializedPropertyIndexString + base.displayName;

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            //Debug.Log("OnEnable 0: " + this.GetComponent());
            if (canvasView.canvasModel is Interactor unityObject)
            {
                interactor = unityObject;
                serializedObject = interactor.GetCachedSerializedObject();
                serializedPropertyArray = serializedObject.FindProperty(nameof(Interactor._interactInputs));
                if (serializedPropertyArray != null && serializedPropertyArray.isArray)
                {
                    arraySize = serializedPropertyArray.arraySize;
                    if (interactor._interactInputs.Count == serializedPropertyArray.arraySize)
                    {
                        var i = interactor._interactInputs.IndexOf(nodeModel);
                        if (i >= 0)
                        {
                            serializedProperty = serializedPropertyArray.GetArrayElementAtIndex(i);
                            serializedPropertyIndex = i;
                            serializedPropertyIndexString = (i + 1).ToString() + ".";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 当绘制检查器
        /// </summary>
        protected internal override void OnGUIInspector()
        {
            serializedObject.UpdateIfRequiredOrScript();

            var arraySize = serializedPropertyArray.arraySize;
            if (this.arraySize != arraySize)
            {
                this.arraySize = arraySize;
                canvasView.DelayRefresh();
            }

            if (serializedPropertyIndex < arraySize)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedProperty, true);
                var ret = EditorGUI.EndChangeCheck();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(UICommonOption.Copy, UICommonOption.Height24))
                {
                    serializedProperty.DuplicateCommand();
                    ret = true;
                }
                if (GUILayout.Button(UICommonOption.Delete, UICommonOption.Height24))
                {
                    serializedProperty.DeleteCommand();
                    ret = true;
                }
                EditorGUILayout.EndHorizontal();
                if (ret)
                {
                    serializedObject.ApplyModifiedProperties();
                    canvasView.DelayRefresh();
                }

                return;
            }

            base.OnGUIInspector();
        }

        /// <summary>
        /// 尝试克隆
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="withInOut"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanClone(CanvasView canvasView, bool withInOut, ICanvasContext canvasContext)
        {
            return base.CanClone(canvasView, withInOut, canvasContext);
        }

        /// <summary>
        /// 尝试克隆
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="withInOut"></param>
        /// <param name="canvasContext"></param>
        /// <param name="cloneDescriptor"></param>
        /// <returns></returns>
        public override bool TryClone(CanvasView canvasView, bool withInOut, ICanvasContext canvasContext, out object cloneDescriptor)
        {
            return base.TryClone(canvasView, withInOut, canvasContext, out cloneDescriptor);
        }
    }
}
