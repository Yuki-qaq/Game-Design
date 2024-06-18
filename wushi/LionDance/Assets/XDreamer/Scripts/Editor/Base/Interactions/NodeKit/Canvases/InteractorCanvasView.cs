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

namespace XCSJ.EditorExtension.Base.Interactions.NodeKit.Canvases
{
    /// <summary>
    /// 交互器画布视图
    /// </summary>
    [CanvasView(typeof(Interactor))]
    public class InteractorCanvasView : ExtensionalInteractObjectCanvasView<Interactor>
    {
        SerializedObject serializedObject;
        SerializedProperty serializedPropertyArray;
        int arraySize = 0;

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            serializedObject = canvasModel.GetCachedSerializedObject();
            serializedPropertyArray = serializedObject.FindProperty(nameof(Interactor._interactInputs));
            arraySize = serializedPropertyArray.arraySize;
        }

        /// <summary>
        /// 当绘制检查器
        /// </summary>
        protected internal override void OnGUIInspector()
        {
            base.OnGUIInspector();
            if (inspectorNodeView == this)
            {
                serializedObject.UpdateIfRequiredOrScript();
                if (arraySize != serializedPropertyArray.arraySize)
                {
                    arraySize = serializedPropertyArray.arraySize;
                    DelayRefresh();
                }
            }
        }
    }

    /// <summary>
    /// 交互器画布视图
    /// </summary>
    /// <typeparam name="T">交互器</typeparam>
    public class InteractorCanvasView<T> : InteractorCanvasView where T : Interactor
    {
        /// <summary>
        /// 画布模型
        /// </summary>
        public new T canvasModel { get; private set; }

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            if (base.canvasModel is T cm)
            {
                canvasModel = cm;
            }
            else
            {
                Debug.LogErrorFormat("画布模型对象不是有效的[{0}]类型对象！", typeof(T));
            }
        }
    }
}
