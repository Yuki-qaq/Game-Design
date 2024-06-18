using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorSMS.NodeKit.Nodes
{
    /// <summary>
    /// 子状态机节点视图
    /// </summary>
    [NodeView(typeof(SubStateMachine))]
    [XCSJ.Attributes.Icon(EIcon.State)]
    public class SubStateMachineNodeView: StateNodeView
    {
        /// <summary>
        /// 节点类型节点视图
        /// </summary>
        public override NodeTypeNodeView nodeTypeNodeView { get; } = new NodeTypeNodeView();

        /// <summary>
        ///  背景风格
        /// </summary>
        protected override GUIStyle backgroundStyle => customStyle.cyanNodeStyle;

        /// <summary>
        /// 小地图颜色
        /// </summary>
        protected override Color miniMapColor { get; } = Color.cyan;

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            inSlotLocalPositionMap = nodeRect.LayoutLocalOutsideRectIn4Of8(customStyle.slotNodeSize);
            outSlotLocalPositionMap = nodeRect.LayoutLocalOutsideRectOut4Of8(customStyle.slotNodeSize);
        }

        /// <summary>
        /// 绘制背景内容
        /// </summary>
        protected override void DrawContentBackground() => this.DrawImportTexture2D();
    }

    /// <summary>
    /// 子状态机父级节点视图
    /// </summary>
    [NodeView(typeof(SubStateMachine), typeof(SubStateMachine), true)]
    [XCSJ.Attributes.Icon(EIcon.State)]
    public class SubStateMachine_ParentNodeView : StateNodeView
    {
        /// <summary>
        /// 画布模型
        /// </summary>
        public SubStateMachine subStateMachine { get; private set; }

        /// <summary>
        /// 节点类型节点视图
        /// </summary>
        public override NodeTypeNodeView nodeTypeNodeView { get; } = new NodeTypeNodeView();

        /// <summary>
        ///  背景风格
        /// </summary>
        protected override GUIStyle backgroundStyle => customStyle.parentNodeStyle;

        /// <summary>
        /// 小地图颜色
        /// </summary>
        protected override Color miniMapColor => customStyle.miniMapParentItemColor;

        /// <summary>
        /// 内容样式
        /// </summary>
        protected override GUIStyle contentStyle => null;

        private Rect _nodeRect = new Rect();

        /// <summary>
        /// 节点矩形
        /// </summary>
        public override Rect nodeRect
        {
            get
            {
                return parent?.nodeModel is SubStateMachine ssm ? ssm.parentRect : _nodeRect;
            }
            set
            {
                if (parent?.nodeModel is SubStateMachine ssm)
                {
                    ssm.parentRect = value;
                }
                else
                {
                    _nodeRect = value;
                }
            }
        }

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            if (base.nodeModel is SubStateMachine cm)
            {
                subStateMachine = cm;
            }
            else
            {
                Debug.LogErrorFormat("[{0}]模型数据必须是[{1}]类型！", typeof(SubStateMachine_ParentNodeView).FullName, typeof(SubStateMachine).FullName);
            }
        }

        /// <summary>
        /// 绘制背景内容
        /// </summary>
        protected override void DrawContentBackground() => this.DrawExportTexture2D();
    }

    /// <summary>
    /// 状态机控制器父级节点视图：在状态机画布内表现状态机控制器的节点视图
    /// </summary>
    [NodeView(typeof(SMController), typeof(StateMachine), true)]
    [XCSJ.Attributes.Icon(EIcon.Component)]
    public class SMController_ParentNodeView : ParentNodeView
    {
        /// <summary>
        /// 绘制背景内容
        /// </summary>
        protected override void DrawContentBackground() => this.DrawExportTexture2D();
    }
}
