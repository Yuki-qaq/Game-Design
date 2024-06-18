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
    #region 父级节点视图

    /// <summary>
    /// 父级节点视图
    /// </summary>
    public class ParentNodeView : NodeView
    {
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
        /// 绘制背景内容
        /// </summary>
        protected override void DrawContentBackground() => this.DrawExportTexture2D();

        private Rect _nodeRect = new Rect();

        /// <summary>
        /// 节点矩形
        /// </summary>
        public override Rect nodeRect
        {
            get
            {
                return parent?.canvasModel is ICanvasModel canvasModel ? canvasModel.parentRect : _nodeRect;
            }
            set
            {
                if (parent?.canvasModel is ICanvasModel canvasModel)
                {
                    canvasModel.parentRect = value;
                }
                else
                {
                    _nodeRect = value;
                }
            }
        }

        /// <summary>
        /// 当绘制检查器
        /// </summary>
        protected internal override void OnGUIInspector()
        {
            if (parent?.parent is CanvasView canvasView)
            {
                canvasView.CallOnGUIInspector(inspectorWidth);
                return;
            }
            base.OnGUIInspector();
        }
    }

    #endregion
}
