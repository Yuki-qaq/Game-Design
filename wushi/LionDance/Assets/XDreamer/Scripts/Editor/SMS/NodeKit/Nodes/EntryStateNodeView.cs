using UnityEngine;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorSMS.NodeKit.Nodes
{
    /// <summary>
    /// 进入状态节点视图
    /// </summary>
    [NodeView(typeof(EntryState))]
    public class EntryStateNodeView : SingleStateNodeView
    {
        /// <summary>
        ///  背景风格
        /// </summary>
        protected override GUIStyle backgroundStyle => customStyle.greenNodeStyle;

        /// <summary>
        /// 小地图颜色
        /// </summary>
        protected override Color miniMapColor { get; } = Color.green;

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
        protected override void DrawContentBackground() => this.DrawTexture2D(customStyle.entryTexture);
    }
}
