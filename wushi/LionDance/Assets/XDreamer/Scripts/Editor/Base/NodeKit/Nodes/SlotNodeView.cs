using NUnit;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorSMS.Utils;

namespace XCSJ.EditorExtension.Base.NodeKit.Nodes
{
    /// <summary>
    /// 插槽节点视图
    /// </summary>
    public class SlotNodeView : ConnectVirtualNodeView
    {
        /// <summary>
        /// 插槽类型：入或出
        /// </summary>
        public ESlotType slotType { get; private set; } = ESlotType.Out;

        /// <summary>
        /// 插槽在节点的方位
        /// </summary>
        public EDirection direction { get; private set; } = EDirection.Right;

        /// <summary>
        /// 连接插槽节点视图
        /// </summary>
        internal SlotNodeView connectSlot { get; set; }

        /// <summary>
        /// 连接虚体节点视图
        /// </summary>
        public override ConnectVirtualNodeView connetVirtualNodeView => connectSlot;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="requireEntityNodeView"></param>
        public SlotNodeView(ESlotType slotType, BaseEntityNodeView requireEntityNodeView)
        {
            this.slotType = slotType;
            Init(requireEntityNodeView);
        }

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            if (nodeRect.size == Vector2.zero)
            {
                nodeRect = new Rect(Vector2.zero, NodeKitHelperExtension.customStyle.slotNodeSize);
            }
        }

        private NodeView nodeView => requireEntityNodeView as NodeView;

        /// <summary>
        /// 当重布局
        /// </summary>
        protected internal override void OnRelayout()
        {
            //base.OnRelayout();

            //if (connectSlot != null && requireEntityNodeView is NodeView nodeView)
            //{
            //    direction = NodeKitHelperExtension.CalculateDirection(requireEntityNodeView.nodeRect, connectSlot.requireEntityNodeView.nodeRect, customStyle.transitionNodeSize);
            //    ResetDirection(direction);
            //}
        }

        /// <summary>
        /// 重置插槽方向
        /// </summary>
        /// <param name="direction"></param>
        public void ResetDirection(EDirection direction)
        {
            this.direction = direction; 
            nodeRect = new Rect(nodeView.nodeRect.position + nodeView.CalculateSlotLocalPosition(slotType, direction), nodeRect.size);
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        internal protected override void OnGUI()
        {
            //base.OnGUI();
        }

        /// <summary>
        /// 连接到
        /// </summary>
        /// <param name="slotNodeView"></param>
        /// <returns></returns>
        internal protected virtual bool ConnectTo(SlotNodeView slotNodeView)
        {
            if (slotNodeView ==null || slotType != ESlotType.Out || slotNodeView.slotType != ESlotType.In) return false;
            if (connectSlot == slotNodeView) return false;

            connectSlot = slotNodeView;
            slotNodeView.connectSlot = this;
            return true;
        }

        /// <summary>
        /// 连接从
        /// </summary>
        /// <param name="slotNodeView"></param>
        /// <returns></returns>
        internal protected virtual bool ConnectFrom(SlotNodeView slotNodeView)
        {
            if (slotNodeView == null || slotType != ESlotType.In || slotNodeView.slotType != ESlotType.Out) return false;
            if (connectSlot == slotNodeView) return false;

            slotNodeView.ConnectTo(this);
            return true;
        }
    }

    /// <summary>
    /// 插槽类型
    /// </summary>
    public enum ESlotType
    {
        /// <summary>
        /// 入
        /// </summary>
        In,

        /// <summary>
        /// 出
        /// </summary>
        Out,
    }
}
