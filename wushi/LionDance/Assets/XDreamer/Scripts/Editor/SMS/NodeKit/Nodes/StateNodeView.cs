using System.Collections.Generic;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.EditorSMS.NodeKit.Canvases;
using XCSJ.EditorSMS.States;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Menus;
using XCSJ.PluginCommonUtils.NodeKit;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorSMS.NodeKit.Nodes
{
    /// <summary>
    /// 状态节点视图
    /// </summary>
    [NodeView(typeof(State))]
    public class StateNodeView : ModelNodeView
    {
        /// <summary>
        /// 状态
        /// </summary>
        public State state { get; private set; }

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            if (nodeModel is State state && state)
            {
                this.state = state;
            }
            else
            {
                Debug.LogErrorFormat("[{0}]模型数据必须是[{1}]类型！", typeof(StateNodeView).FullName, typeof(State).FullName);
            }
        }

        #region 状态编辑操作

        /// <summary>
        /// 可连入
        /// </summary>
        /// <param name="fromNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanConnectFrom(NodeView fromNodeView, ICanvasContext canvasContext)
        {
            return state.allowIn || (fromNodeView is StateNodeView stateNodeView && stateNodeView.state.allowOut);
        }

        /// <summary>
        /// 可连出
        /// </summary>
        /// <param name="toNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanConnectTo(NodeView toNodeView, ICanvasContext canvasContext)
        {
            return state.allowOut || (toNodeView is StateNodeView stateNodeView && stateNodeView.state.allowIn);
        }

        /// <summary>
        /// 可克隆
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="withInOut"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanClone(CanvasView canvasView, bool withInOut, ICanvasContext canvasContext) => canvasView is SubStateMachineCanvasView;

        /// <summary>
        /// 尝试克隆
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="withIO"></param>
        /// <param name="canvasContext"></param>
        /// <param name="cloneDescriptor"></param>
        /// <returns></returns>
        public override bool TryClone(CanvasView canvasView, bool withIO, ICanvasContext canvasContext, out object cloneDescriptor)
        {
            cloneDescriptor = default;
            var cloneState = state.Clone(withIO);
            if (cloneState)
            {
                // 克隆完成后向右下偏移
                var rect = cloneState.nodeRect;
                rect.position += rect.size;
                cloneState.nodeRect = rect;
            }
            return cloneState;
        }

        /// <summary>
        /// 可删除
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanDelete(CanvasView canvasView, ICanvasContext canvasContext) => canvasView is SubStateMachineCanvasView;

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
            return parentSubStateMachine.RemoveState(state, false);
        }

        /// <summary>
        /// 可粘贴
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public override bool CanPaste(CanvasView canvasView, ICanvasContext canvasContext) => canvasView is SubStateMachineCanvasView;

        /// <summary>
        /// 尝试粘贴
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <param name="pasteDescriptor"></param>
        /// <returns></returns>
        public override bool TryPaste(CanvasView canvasView, ICanvasContext canvasContext, out object pasteDescriptor)
        {
            pasteDescriptor = default;
            return state.Clone(canvasView.nodeModel as SubStateMachine);
        }

        #endregion

        #region 绘制

        /// <summary>
        /// 绘制内容
        /// </summary>
        protected override void DrawContent()
        {
            base.DrawContent();

            DrawFriendlyString();
        }

        /// <summary>
        /// 当绘制轮廓线
        /// </summary>
        protected override bool DrawOutline()
        {
            if (base.DrawOutline()) return true;

            // 繁忙
            if (state.busy)
            {
                NodeKitHelperExtension.OnGUIColor(customStyle.busyColor, () => GUI.Box(nodeRect, GUIContent.none, customStyle.selectionBorderStyle));
                return true;
            }

            // 激活
            if (state.active)
            {
                NodeKitHelperExtension.OnGUIColor(customStyle.activeColor, () => GUI.Box(nodeRect, GUIContent.none, customStyle.selectionBorderStyle));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 绘制优化字符串
        /// </summary>
        protected virtual void DrawFriendlyString()
        {
            var info = friendString;
            if (string.IsNullOrEmpty(info)) return;

            GUI.Label(new Rect(nodeRect.x, nodeRect.position.y + titleHeight, nodeRect.width, customStyle.nodeContentHeight), CommonFun.TempContent(info, info), NodeEditorStyle.instance.contentTextStyle);
        }

        /// <summary>
        /// 不在搜索中绘制
        /// </summary>
        /// <param name="nodeEditorInfo"></param>
        protected virtual void DrawNotInSearch(NodeEditorInfo nodeEditorInfo)
        {
            CommonFun.DrawColorGUI(nodeEditorInfo.graphOption.nodeColorWhenNotInSearch, () => GUI.Box(nodeRect, GUIContent.none, NodeEditorStyle.instance.notInSearchNodeStyle));
        }

        /// <summary>
        /// 出节点视图列表
        /// </summary>
        public override IEnumerable<NodeView> outNodeViews
        {
            get
            {
                if (parent is CanvasView canvasView && nodeModel is State state && state)
                {
                    foreach (var outState in state.outStates)
                    {
                        if (canvasView.GetNodeView(outState) is NodeView result && result != this)
                        {
                            yield return result;
                        }
                    }
                }
            }
        }

        #endregion

        #region 弹出菜单

        /// <summary>
        /// 当弹起菜单
        /// </summary>
        /// <param name="menuInfo"></param>
        [LanguageTuple("Create Transition", "创建跳转")]
        [LanguageTuple("Create Reverse Transition", "创建反向跳转")]
        [LanguageTuple("Clone(With Transition)", "克隆(带跳转)")]
        protected override void OnPopupMenu(MenuInfo menuInfo)
        {
            base.OnPopupMenu(menuInfo);

            if (CanConnectTo(null, null))
            {
                menuInfo.AddMenuItem("Create Transition".Tr(this), () => canvasView.BeginConnection(this), null, 100);
                CreateTransitionOut(menuInfo);
                CreateTransitionOutToUpLayer(menuInfo);
            }
            if (CanConnectFrom(null, null))
            {
                menuInfo.AddMenuItem("Create Reverse Transition".Tr(this), () => canvasView.BeginConnection(this, false), null, 110);
                CreateTransitionIn(menuInfo);
            }

            if (CanClone(subStateMachineCanvasView, true, null))
            {
                menuInfo.AddMenuItem("Clone(With Transition)".Tr(this), () => subStateMachineCanvasView.CloneSelection(true), null, 401);
            }
        }

        /// <summary>
        /// 创建跳转到
        /// </summary>
        /// <param name="menuInfo"></param>
        [LanguageTuple("Transition Out/", "跳转到/")]
        private void CreateTransitionOut(MenuInfo menuInfo) => CreateOutMenu(menuInfo, "Transition Out/".Tr(this), 103, 104, true);

        /// <summary>
        /// 创建跳转到上层的出
        /// </summary>
        /// <param name="menuInfo"></param>
        [LanguageTuple("Transition Out/UpLayer/", "跳转到/上层/")]
        private void CreateTransitionOutToUpLayer(MenuInfo menuInfo) => CreateOutMenu(menuInfo, "Transition Out/UpLayer/".Tr(this), 101, 102, true);

        /// <summary>
        /// 创建反向跳转到
        /// </summary>
        /// <param name="menuInfo"></param>
        [LanguageTuple("Transition In/", "反向跳转到/")]
        private void CreateTransitionIn(MenuInfo menuInfo) => CreateOutMenu(menuInfo, "Transition In/".Tr(this), 111, 112, false);

        private void CreateOutMenu(MenuInfo menuInfo, string menuName, int specailStateMenuIndex, int normalStateMenuIndex, bool isOut)
        {
            var specailStates = new List<State>();
            var normalStates = new List<State>();

            SMSEditorHelper.GetSpecailAndNormalStates(state, parentSubStateMachine, specailStates, normalStates, isOut);

            SMSEditorHelper.AddMenus(menuInfo, specailStates, menuName, (s) => subStateMachineCanvasView.CreateSelectionConection(s, isOut), specailStateMenuIndex, () => true, specailStates.Count - 1, ESeparatorType.SubDown);
            SMSEditorHelper.AddMenus(menuInfo, normalStates, menuName, (s) => subStateMachineCanvasView.CreateSelectionConection(s, isOut), normalStateMenuIndex);
        }

        #endregion
    }
}

