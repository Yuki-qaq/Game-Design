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
    #region 游戏对象父级节点视图

    /// <summary>
    /// 游戏对象父级节点视图：在组件画布视图内表现游戏对象的节点视图
    /// </summary>
    [NodeView(typeof(GameObjectCanvasModel), true)]
    public class GameObject_ParentNodeView : ParentNodeView
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
