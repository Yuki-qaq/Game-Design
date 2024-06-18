using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
using XCSJ.Extension.Base.NodeKit;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Menus;
using XCSJ.PluginCommonUtils.NodeKit;

namespace XCSJ.EditorExtension.Base.NodeKit.Nodes
{
    #region 虚体节点视图

    /// <summary>
    /// 虚体节点视图
    /// </summary>
    public class VirtualNodeView : BaseVirtualNodeView
    {
        /// <summary>
        /// 依赖节点视图
        /// </summary>
        public NodeView requireNodeView { get; private set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public virtual string displayName { get; set; } = "";

        /// <summary>
        /// 节点尺寸
        /// </summary>
        public virtual Vector2 size => nodeRect.size;

        /// <summary>
        /// 自定义样式
        /// </summary>
        protected CustomStyle customStyle => NodeKitHelperExtension.customStyle;

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            requireNodeView = requireEntityNodeView as NodeView;
        }

        /// <summary>
        /// 当重布局
        /// </summary>
        internal protected virtual void OnRelayout() { }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        internal protected virtual void OnGUI()
        {
            GUI.Button(nodeRect, displayName);
        }

        /// <summary>
        /// 布局：在外部矩形方位上布局
        /// </summary>
        /// <param name="rectOutsideDirection"></param>
        public void Layout(ERectOutsideDirection rectOutsideDirection) => nodeRect = new Rect(requireNodeView.nodeRect.LayoutOutsideRect(size, rectOutsideDirection), size);
    }

    #endregion

    #region 连接虚体节点视图

    /// <summary>
    /// 连接虚体节点视图：关联其他虚体节点视图
    /// </summary>
    public class ConnectVirtualNodeView : VirtualNodeView
    {
        /// <summary>
        /// 连接虚体节点视图
        /// </summary>
        public virtual ConnectVirtualNodeView connetVirtualNodeView { get; private set; }
    }

    #endregion
}
