using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.EditorSMS.NodeKit.Nodes
{
    /// <summary>
    /// 状态组件节点视图
    /// </summary>
    [NodeView(typeof(StateComponent))]
    public class StateComponentNodeView : ModelNodeView
    {
        ///// <summary>
        ///// 背景样式
        ///// </summary>
        //protected override GUIStyle backgroundStyle => customStyle.yellowNodeStyle;
    }

    /// <summary>
    /// 跳转组件节点视图
    /// </summary>
    [NodeView(typeof(TransitionComponent))]
    public class TransitionComponentNodeView : ModelNodeView
    {
        ///// <summary>
        ///// 背景样式
        ///// </summary>
        //protected override GUIStyle backgroundStyle => customStyle.yellowNodeStyle;
    }
}
