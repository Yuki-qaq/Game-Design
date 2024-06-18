using UnityEngine;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorSMS.NodeKit.Nodes
{
    /// <summary>
    /// 任意状态节点视图
    /// </summary>
    [NodeView(typeof(AnyState))]
    public class AnyStateNodeView : SingleStateNodeView
    {
        /// <summary>
        ///  背景风格
        /// </summary>
        protected override GUIStyle backgroundStyle => customStyle.blueNodeStyle;

        /// <summary>
        /// 小地图颜色
        /// </summary>
        protected override Color miniMapColor { get; } = Color.blue;

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            outSlotLocalPositionMap = nodeRect.LayoutLocalOutsideRectOut4Of8(customStyle.slotNodeSize);
        }

        /// <summary>
        /// 绘制背景内容
        /// </summary>
        protected override void DrawContentBackground() => this.DrawTexture2D(customStyle.updateTexture);
    }
}
