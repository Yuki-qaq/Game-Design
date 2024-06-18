using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorSMS.NodeKit.Nodes
{
    /// <summary>
    /// 普通状态节点视图
    /// </summary>
    [NodeView(typeof(NormalState))]
    public class NormalStateNodeView : StateNodeView
    {
        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            inSlotLocalPositionMap = nodeRect.LayoutLocalOutsideRectIn4Of8(customStyle.slotNodeSize);
            outSlotLocalPositionMap = nodeRect.LayoutLocalOutsideRectOut4Of8(customStyle.slotNodeSize);
        }
    }
}