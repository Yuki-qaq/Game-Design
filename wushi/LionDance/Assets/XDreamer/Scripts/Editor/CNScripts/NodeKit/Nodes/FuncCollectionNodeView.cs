using XCSJ.Attributes;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.Scripts;

namespace XCSJ.EditorCNScripts.NodeKit.Nodes
{
    /// <summary>
    /// 函数集合节点视图
    /// </summary>
    [NodeView(typeof(FuncCollection))]
    [XCSJ.Attributes.Icon(EIcon.Function)]
    public class FuncCollectionNodeView : NodeView
    {
        /// <summary>
        /// 当绘制检查器
        /// </summary>
        protected internal override void OnGUIInspector()
        {
            //base.OnGUIInspector();
            NodeKitHelperExtension.DrawNodeModelInspector(parent?.nodeModel, inspectorWidth);
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            //base.OnGUI();
            DrawOutline();
            OnGUIVirtualNodeView();
            this.DrawImportNodeView();
        }
    }
}
