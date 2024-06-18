using UnityEngine;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorSMS.NodeKit.Nodes
{
    /// <summary>
    /// 退出状态节点视图
    /// </summary>
    [NodeView(typeof(ExitState))]
    public class ExitStateNodeView : SingleStateNodeView
    {
        /// <summary>
        ///  背景风格
        /// </summary>
        protected override GUIStyle backgroundStyle => customStyle.redNodeStyle;

        /// <summary>
        /// 小地图颜色
        /// </summary>
        protected override Color miniMapColor { get; } = Color.red;

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            inSlotLocalPositionMap = nodeRect.LayoutLocalOutsideRectIn4Of8(customStyle.slotNodeSize);
        }

        /// <summary>
        /// 绘制背景内容
        /// </summary>
        protected override void DrawContentBackground() => this.DrawTexture2D(customStyle.exitTexture);
    }
}
