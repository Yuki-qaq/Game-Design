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
    /// 可扩展交互对象画布视图
    /// </summary>
    [CanvasView(typeof(ExtensionalInteractObject))]
    public class ExtensionalInteractObjectCanvasView : MBCanvasView<ExtensionalInteractObject> { }

    /// <summary>
    /// 可扩展交互对象画布视图
    /// </summary>
    /// <typeparam name="T">可扩展交互对象</typeparam>
    public class ExtensionalInteractObjectCanvasView<T> : ExtensionalInteractObjectCanvasView where T : ExtensionalInteractObject
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
