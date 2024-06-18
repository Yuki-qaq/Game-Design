using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.EditorSMS.NodeKit.Canvases;
using XCSJ.EditorSMS.States;
using XCSJ.Interfaces;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils.Menus;
using XCSJ.PluginCommonUtils.NodeKit;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorSMS.NodeKit.Nodes
{
    /// <summary>
    /// 跳转节点视图
    /// </summary>
    [NodeView(typeof(Transition))]
    public class TransitionNodeView : ModelNodeView
    {
        /// <summary>
        /// 跳转
        /// </summary>
        public Transition transition { get; private set; }

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            if (base.nodeModel is Transition t)
            {
                transition = t;
            }
            else
            {
                Debug.LogErrorFormat("[{0}]模型数据必须是[{1}]类型！", typeof(TransitionNodeView).FullName, typeof(Transition).FullName);
            }

            inSlotLocalPositionMap = nodeRect.LayoutLocalOutsideRect4Center(customStyle.slotNodeSize);
            outSlotLocalPositionMap = nodeRect.LayoutLocalOutsideRect4Center(customStyle.slotNodeSize);
        }

        /// <summary>
        /// 当初始化尺寸
        /// </summary>
        protected override void OnInitSize() => nodeRect = new Rect(Vector2.zero, customStyle.transitionNodeSize);

        /// <summary>
        /// 绘制标题
        /// </summary>
        protected override void DrawTitle() { }//base.DrawTitle();

        /// <summary>
        /// 绘制内容背景
        /// </summary>
        protected override void DrawContentBackground() { }//base.DrawContentBackground();

        /// <summary>
        /// 绘制内容
        /// </summary>
        protected override void DrawContent() { }//base.DrawContent();

        /// <summary>
        /// 当绘制
        /// </summary>
        protected override void OnGUI()
        {
            base.OnGUI();

            foreach (var slotNodeView in slotNodeViews)
            {
                var fromRect = slotNodeView.nodeRect;
                var toRect = slotNodeView.connectSlot.nodeRect;
                var fromDir = slotNodeView.direction;
                var toDir = slotNodeView.connectSlot.direction;
                switch (slotNodeView.slotType)
                {
                    case ESlotType.In:
                        {
                            NodeKitHelperExtension.DrawConnectionLine(toRect.LayoutSideOnCenter(toDir.Reverse()), fromRect.LayoutSideOnCenter(fromDir.Reverse()), toDir, fromDir, false);
                            break;
                        }
                    case ESlotType.Out:
                        {
                            //var toPoint = connectSlot.drawInArrow ? connectSlot.nodeRect.center : connectSlot.nodeRect.LayoutSideOnCenter(connectSlot.direction.Reverse());
                            NodeKitHelperExtension.DrawConnectionLine(fromRect.LayoutSideOnCenter(fromDir.Reverse()), toRect.center, fromDir, toDir, true);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 当绘制小地图
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scaleValue"></param>
        public override void OnGUIMiniMap(Rect rect, float scaleValue) { }//base.OnGUIMiniMap(rect, scaleValue);

        /// <summary>
        /// 布局
        /// </summary>
        public override void Relayout()
        {
            base.Relayout();

            if (!transition) return;

            // 设置插槽位置
            if (transition.inState == transition.outState)
            {
                foreach (var s in slotNodeViews)
                {
                    if (s.slotType == ESlotType.In)
                    {
                        s.ResetDirection(EDirection.Right);
                    }
                    else
                    {
                        s.ResetDirection(EDirection.Left);
                    }
                    s.connectSlot.ResetDirection(EDirection.Down);
                }
            }
            else
            {
                var dir = NodeKitHelperExtension.CalculateDirection(transition.inState.nodeRect, transition.outState.nodeRect, transition.outState.nodeRect.size);
                foreach (var slotNodeView in slotNodeViews)
                {
                    switch (slotNodeView.slotType)
                    {
                        case ESlotType.In:
                            {
                                slotNodeView.ResetDirection(dir.Reverse());
                                slotNodeView.connectSlot.ResetDirection(dir);
                                break;
                            }
                        case ESlotType.Out:
                            {
                                slotNodeView.ResetDirection(dir);
                                slotNodeView.connectSlot.ResetDirection(dir.Reverse());
                                break;
                            }
                    }
                }
            }

            // 设置连入连出位置
            inConnectButtonNodeView.Layout(ERectOutsideDirection.LeftMiddle);
            outConnectButtonNodeView.Layout(ERectOutsideDirection.RightMiddle);
        }

        /// <summary>
        /// 移动：跳转节点不响应移动
        /// </summary>
        /// <param name="offset"></param>
        public override void Move(Vector2 offset) { }//base.Move(offset); 

        /// <summary>
        /// 可连入
        /// </summary>
        /// <param name="fromNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanConnectFrom(NodeView fromNodeView, ICanvasContext canvasContext) => true;

        /// <summary>
        /// 可连出
        /// </summary>
        /// <param name="fromNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanConnectTo(NodeView fromNodeView, ICanvasContext canvasContext) => true;

        /// <summary>
        /// 可复制
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanCopy(CanvasView canvasView, ICanvasContext canvasContext) => false;

        /// <summary>
        /// 可剪切
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanCut(CanvasView canvasView, ICanvasContext canvasContext) => false;

        /// <summary>
        /// 可删除
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanDelete(CanvasView canvasView, ICanvasContext canvasContext) => canvasView != null && canvasView is SubStateMachineCanvasView;

        /// <summary>
        /// 尝试删除
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <param name="deleteDescriptor"></param>
        /// <returns></returns>
        public override bool TryDelete(CanvasView canvasView, ICanvasContext canvasContext, out object deleteDescriptor)
        {
            deleteDescriptor = default;
            return parentSubStateMachine.RemoveTransition(transition, false);
        }

        /// <summary>
        /// 可克隆
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="withInOut"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanClone(CanvasView canvasView, bool withInOut, ICanvasContext canvasContext)
        {
            return canvasView != null && canvasView is SubStateMachineCanvasView;
        }

        /// <summary>
        /// 尝试克隆
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="withInOut"></param>
        /// <param name="canvasContext"></param>
        /// <param name="cloneDescriptor"></param>
        /// <returns></returns>
        public override bool TryClone(CanvasView canvasView, bool withInOut, ICanvasContext canvasContext, out object cloneDescriptor)
        {
            cloneDescriptor = default;
            return transition.CloneWithInOutState();
        }

        /// <summary>
        /// 当弹出菜单
        /// </summary>
        /// <param name="menuInfo"></param>
        [LanguageTuple("Change In/", "更改输入到/")]
        [LanguageTuple("Change out/", "更改输出到/")]
        [LanguageTuple("Change out/Up layer/", "更改输出到/上层/")]
        protected override void OnPopupMenu(MenuInfo menuInfo)
        {
            base.OnPopupMenu(menuInfo);

            if (parentSubStateMachine)
            {
                // 更改输入到/ 菜单
                CreateChangedTransitionMenus(menuInfo, "Change In/".Tr(this), 101, parentSubStateMachine, false);

                // 更改输出到/ 菜单
                CreateChangedTransitionMenus(menuInfo, "Change out/".Tr(this), 113, parentSubStateMachine, true);

                // 更改输出到/上层/ 菜单
                if (parentSubStateMachine.parent is SubStateMachine ssm && ssm)
                {
                    CreateChangedTransitionMenus(menuInfo, "Change out/Up layer/".Tr(this), 111, ssm, true);
                }
            }
        }

        private void CreateChangedTransitionMenus(MenuInfo menuInfo, string menuName, int menuIndex, SubStateMachine subStateMachine, bool isOut)
        {
            State fromState = transition.inState;
            State toState = transition.outState;

            var specailStates = new List<State>();
            var normalStates = new List<State>();
            SMSEditorHelper.GetSpecailAndNormalStates(fromState, subStateMachine, specailStates, normalStates, true);
            specailStates.Remove(toState);
            normalStates.Remove(toState);

            SMSEditorHelper.AddMenus(menuInfo, specailStates, menuName, (s) =>
            subStateMachineCanvasView.CreateSelectionConection(s, isOut), menuIndex, () => true, specailStates.Count - 1, ESeparatorType.SubDown);
            SMSEditorHelper.AddMenus(menuInfo, normalStates, menuName, (s) =>
            subStateMachineCanvasView.CreateSelectionConection(s, isOut), menuIndex+1);
        }
    }
}

