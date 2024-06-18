using System;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.NodeKit.Nodes
{
    #region 按钮节点视图

    /// <summary>
    /// 按钮节点视图
    /// </summary>
    public class ButtonNodeView : VirtualNodeView
    {
        /// <summary>
        /// 尺寸
        /// </summary>
        public override Vector2 size => customStyle.iconSize;

        /// <summary>
        /// GUI内容
        /// </summary>
        internal protected GUIContent _content = new GUIContent();

        /// <summary>
        /// GUI内容
        /// </summary>
        public virtual GUIContent content
        {
            get
            {
                _content.text = displayName;
                return _content;
            }
        }

        /// <summary>
        /// 显示GUI
        /// </summary>
        public virtual bool displayGUI { get; protected set; } = true;

        /// <summary>
        /// 当点击
        /// </summary>
        public event Action<ButtonNodeView> onClick;

        /// <summary>
        /// 调用点击事件
        /// </summary>
        protected void CallClickEvent() => onClick?.Invoke(this);

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected internal override void OnGUI()
        {
            //base.OnGUI();

            if (displayGUI && GUI.Button(nodeRect, content, GUIStyle.none))
            {
                onClick?.Invoke(this);
            }
        }
    }

    #endregion

    #region 节点类型图标节点视图

    /// <summary>
    /// 节点类型图标节点视图：通过查找实体节点视图上的图标特性在上右方位上进行绘制
    /// </summary>
    public class NodeTypeNodeView : ButtonNodeView
    {
        /// <summary>
        /// GUI内容
        /// </summary>
        public override GUIContent content => base._content;

        /// <summary>
        /// 显示GUI
        /// </summary>
        public override bool displayGUI { get => base.displayGUI && _content.image; protected set => base.displayGUI = value; }

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            _content = CommonFun.NameTip(requireNodeView.GetType(), ENameTip.EmptyTextWhenHasImage);
        }

        /// <summary>
        /// 当重布局
        /// </summary>
        protected internal override void OnRelayout()
        {
            base.OnRelayout();
            Layout(ERectOutsideDirection.UpRight);
        }
    }

    #endregion

    #region 入连接按钮节点视图

    /// <summary>
    /// 入连接按钮节点视图
    /// </summary>
    public class InConnectButtonNodeView : ButtonNodeView
    {
        /// <summary>
        /// 当重布局
        /// </summary>
        protected internal override void OnRelayout()
        {
            base.OnRelayout();
            var size = customStyle.slotNodeSize;
            nodeRect = new Rect(requireNodeView.nodeRect.LayoutOutsideRectIn4Of8(size, EDirection.Left), size);
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected internal override void OnGUI()
        {
            if (!requireNodeView.isHover || !requireNodeView.CanConnectFrom(null, null)) return;
            if (GUI.Button(nodeRect, customStyle.connectIn, customStyle.iconButtonStyle))
            {
                CallClickEvent();
            }
        }
    }

    #endregion

    #region 出连接按钮节点视图

    /// <summary>
    /// 出连接按钮节点视图
    /// </summary>
    public class OutConnectButtonNodeView : ButtonNodeView
    {
        /// <summary>
        /// 当重布局
        /// </summary>
        protected internal override void OnRelayout()
        {
            base.OnRelayout();
            var size = customStyle.slotNodeSize;
            nodeRect = new Rect(requireNodeView.nodeRect.LayoutOutsideRectOut4Of8(size, EDirection.Right), size);
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected internal override void OnGUI()
        {
            if (!requireNodeView.isHover || !requireNodeView.CanConnectTo(null, null)) return;
            if (GUI.Button(nodeRect, customStyle.connectOut, customStyle.iconButtonStyle))
            {
                CallClickEvent();
            }
        }
    }

    #endregion
}
