using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
using XCSJ.Scripts;

namespace XCSJ.EditorCNScripts.NodeKit.Canvases
{
    /// <summary>
    /// 函数集合画布视图
    /// </summary>
    [CanvasView(typeof(FuncCollection))]
    public class FuncCollectionCanvasView : CanvasView
    {
        /// <summary>
        /// 当绘制检查器
        /// </summary>
        protected internal override void OnGUIInspector()
        {
            //base.OnGUIInspector();
            NodeKitHelperExtension.DrawNodeModelInspector(parent?.nodeModel, inspectorWidth);
        }
    }
}
