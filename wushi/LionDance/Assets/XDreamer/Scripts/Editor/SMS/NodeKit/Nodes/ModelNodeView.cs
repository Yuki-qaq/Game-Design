using System.Collections.Generic;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.EditorSMS.NodeKit.Canvases;
using XCSJ.Interfaces;
using XCSJ.Languages;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorSMS.NodeKit.Nodes
{
    /// <summary>
    /// 模型节点矩形:专用于状态机系统的模型节点视图
    /// </summary>
    public abstract class ModelNodeView : NodeView
    {
        /// <summary>
        /// 子状态机画布视图
        /// </summary>
        public SubStateMachineCanvasView subStateMachineCanvasView { get; private set; }

        /// <summary>
        /// 父级子状态机
        /// </summary>
        public SubStateMachine parentSubStateMachine => subStateMachineCanvasView.canvasModel;

        /// <summary>
        /// 友好字符串
        /// </summary>
        public virtual string friendString => (nodeModel as IToFriendlyString)?.ToFriendlyString();

        /// <summary>
        /// 入插槽相对位置缓存：用于记录当前插槽相对节点视图入位置
        /// </summary>

        protected Dictionary<EDirection, Vector2> inSlotLocalPositionMap = new Dictionary<EDirection, Vector2>();

        /// <summary>
        /// 出插槽相对位置缓存：用于记录当前插槽相对节点视图出位置
        /// </summary>

        protected Dictionary<EDirection, Vector2> outSlotLocalPositionMap = new Dictionary<EDirection, Vector2>();

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            if (canvasView is SubStateMachineCanvasView subStateMachineCanvasView)
            {
                this.subStateMachineCanvasView = subStateMachineCanvasView;
            }
        }

        /// <summary>
        /// 计算插槽矩形
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public override Vector2 CalculateSlotLocalPosition(ESlotType slotType, EDirection direction)
        {
            switch (slotType)
            {
                case ESlotType.In:
                    {
                        if (inSlotLocalPositionMap.TryGetValue(direction, out var position)) return position;
                        break;
                    }
                case ESlotType.Out:
                    {
                        if (outSlotLocalPositionMap.TryGetValue(direction, out var position)) return position;
                        break;
                    }
            }
            return base.CalculateSlotLocalPosition(slotType, direction);
        }

        /// <summary>
        /// 当绘制检查器
        /// </summary>
        protected internal override void OnGUIInspector()
        {
            base.OnGUIInspector();

            //foreach (var item in slotNodeViews)
            //{
            //    EditorGUILayout.LabelField(displayName+":"+item.slotType.ToString()+":"+item.direction+"==>"+item.connectSlot.requireEntityNodeView.displayName);
            //}
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            //var position = nodeRect.position;
            //foreach (var item in inSlotLocalPositionMap)
            //{
            //    NodeKitHelperExtension.OnGUIColor(Color.red, () => GUI.Box(new Rect(position + item.Value, customStyle.slotNodeSize), item.Key.ToString().Substring(0,1)));
            //}
            //foreach (var item in outSlotLocalPositionMap)
            //{
            //    NodeKitHelperExtension.OnGUIColor(Color.green, () => GUI.Box(new Rect(position + item.Value, customStyle.slotNodeSize), item.Key.ToString().Substring(0, 1)));
            //}
            base.OnGUI();
        }
    }
}
