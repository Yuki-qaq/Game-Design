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
    #region 组件父级节点视图

    /// <summary>
    /// 组件父级节点视图：在组件子级画布视图内表现组件的节点视图
    /// </summary>
    [NodeView(typeof(Component), true)]
    public class Component_ParentNodeView : ParentNodeView
    {
        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            nodeTypeNodeView._content = CommonFun.NameTip(nodeModel.GetType(), ENameTip.EmptyTextWhenHasImage);
        }
    }

    #endregion
}
