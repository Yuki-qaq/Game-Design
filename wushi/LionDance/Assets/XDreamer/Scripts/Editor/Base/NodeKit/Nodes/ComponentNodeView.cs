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
    #region 组件节点视图

    /// <summary>
    /// 组件节点视图
    /// </summary>
    [NodeView(typeof(Component))]
    [XCSJ.Attributes.Icon(EIcon.Component)]
    [Name("组件", nameof(Component))]
    public class ComponentNodeView : NodeView
    {
        /// <summary>
        /// 节点类型节点视图
        /// </summary>
        public override NodeTypeNodeView nodeTypeNodeView { get; } = new NodeTypeNodeView();

        /// <summary>
        ///  背景风格
        /// </summary>
        protected override GUIStyle backgroundStyle => customStyle.MBNodeStyle;

        /// <summary>
        /// 绘制背景内容
        /// </summary>
        protected override void DrawContentBackground() => this.DrawImportTexture2D();
    }

    #endregion
}
