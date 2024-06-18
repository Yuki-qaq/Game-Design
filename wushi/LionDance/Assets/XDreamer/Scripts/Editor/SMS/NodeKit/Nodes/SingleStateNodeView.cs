using UnityEngine;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
using XCSJ.PluginCommonUtils.NodeKit;

namespace XCSJ.EditorSMS.NodeKit.Nodes
{
    /// <summary>
    /// 单一状态节点视图
    /// </summary>
    public abstract class SingleStateNodeView : StateNodeView
    {
        /// <summary>
        /// 内容样式
        /// </summary>
        protected override GUIStyle contentStyle => null;

        /// <summary>
        /// 可删除
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanDelete(CanvasView canvasView, ICanvasContext canvasContext) => false;

        /// <summary>
        /// 可克隆
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="withInOut"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanClone(CanvasView canvasView, bool withInOut, ICanvasContext canvasContext) => false;

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanCopy(CanvasView canvasView, ICanvasContext canvasContext) => false;

        /// <summary>
        /// 剪切
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanCut(CanvasView canvasView, ICanvasContext canvasContext) => false;

        /// <summary>
        /// 粘贴
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanPaste(CanvasView canvasView, ICanvasContext canvasContext) => false;
    }
}
