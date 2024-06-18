using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.NodeKit;

namespace XCSJ.EditorExtension.Base.NodeKit.Canvases
{
    /// <summary>
    /// 画布视图
    /// </summary>
    [LanguageFileOutput]
    public class CanvasView : BaseCanvasView, INavigationData, ISearchEvent
    {
        #region 节点数据操作

        /// <summary>
        /// 自定义样式
        /// </summary>
        protected CustomStyle customStyle => NodeKitHelperExtension.customStyle;

        /// <summary>
        /// 父级画布视图
        /// </summary>
        public CanvasView parentCanvasView { get; private set; }

        /// <summary>
        /// 父级节点视图
        /// </summary>
        public NodeView parentNodeView { get; private set; }

        /// <summary>
        /// 子级节点视图列表
        /// </summary>
        public List<NodeView> childrenNodeViews { get; } = new List<NodeView>();

        /// <summary>
        /// 实体节点视图集合
        /// </summary>
        public override IEnumerable<IEntityNodeView> entityNodeViews => nodeViews;

        /// <summary>
        /// 节点视图集合
        /// </summary>
        public IEnumerable<NodeView> nodeViews
        {
            get
            {
                if (parentNodeView != null) yield return parentNodeView;
                foreach (var item in childrenNodeViews) yield return item;
            }
        }

        /// <summary>
        /// 获取节点视图
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public NodeView GetNodeView(Func<NodeView, bool> match)
        {
            if (match == null) return default;
            if (parentNodeView != null && match(parentNodeView)) return parentNodeView;
            return childrenNodeViews.FirstOrDefault(match);
        }

        /// <summary>
        /// 获取节点视图
        /// </summary>
        /// <param name="nodeModel"></param>
        /// <returns></returns>
        public NodeView GetNodeView(INodeModel nodeModel) => GetNodeView(nv => nv.nodeModel == nodeModel);

        private NodeView CreateNodeView(INodeModel nodeModel, bool useForParent)
        {
            if (nodeModel.ObjectIsNull()) return default;

            if (NodeKitHelperExtension.CreateEntityNodeView(nodeModel.GetType(), useForParent, 
                canvasModel.entityModel.GetType(), 
                canvasModel.GetType(),
                GetType()) is NodeView nodeView)
            {
                nodeView.Init(nodeModel, this, nodeKitEditor);
                return nodeView;
            }
            return default;
        }

        private void UpdateNodeViews()
        {
            foreach (var item in childrenNodeViews)
            {
                item.CallOnDisable();
            }
            parentNodeView?.CallOnDisable();

            childrenNodeViews.Clear();
            parentNodeView = CreateNodeView(canvasModel.parentCanvasModel, true);
            parentNodeView?.CallOnEnable();
            foreach (var item in canvasModel.nodeModels)
            {
                if (CreateNodeView(item, false) is NodeView nodeView)
                {
                    childrenNodeViews.Add(nodeView);
                    nodeView.CallOnEnable();
                }
            }
        }

        /// <summary>
        /// 显示节点视图列表
        /// </summary>
        public List<NodeView> displayNodeViews = new List<NodeView>();

        /// <summary>
        /// 拾取节点视图
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual NodeView PickNodeView(Vector2 position)
        {
            for (int i = displayNodeViews.Count - 1; i >= 0; --i)
            {
                var nodeView = displayNodeViews[i];
                if (nodeView.CanPick(position))
                {
                    return nodeView;
                }
            }
            return default;
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="offset"></param>
        protected virtual void Move(Vector2 offset) => displayNodeViews.ForEach(v => v.Move(offset));

        /// <summary>
        /// 需要刷新
        /// </summary>
        public bool needRefresh { get; private set; } = false;

        /// <summary>
        /// 层级路径
        /// </summary>
        public string hierarchyPath => (parentCanvasView?.hierarchyPath ?? "") + "/" + displayName;

        /// <summary>
        /// 延时刷新
        /// </summary>
        /// <param name="isForce"></param>
        public void DelayRefresh(bool isForce = true)
        {
            UICommonFun.DelayCall(() => Refresh(isForce));
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="isForce"></param>
        public override void Refresh(bool isForce)
        {
            base.Refresh(isForce);
            if (isForce) HandleRefresh();
            else needRefresh = true;
        }

        private void HandleRefresh()
        {
            needRefresh = false;

            var selection = nodeSelections.ToList();
            UpdateNodeViews();
            CreateDisplayNodeView();
            foreach (var snv in selection)
            {
                var nnv = GetNodeView(tnv =>
                {
                    if (tnv == snv || tnv.nodeModel == snv.nodeModel || tnv.hierarchyPath == snv.hierarchyPath) return true;
                    return false;
                });
                //Debug.Log(snv.hierarchyPath + "==>" + (nnv?.hierarchyPath ?? "<null>"));
                AddToSelection(nnv, false);
            }
            selection.Clear();
        }

        private void CreateDisplayNodeView()
        {
            displayNodeViews.Clear();
            displayNodeViews.AddRange(nodeViews);
            OnCreateDisplayNodeView();
        }

        /// <summary>
        /// 当创建显示节点视图
        /// </summary>
        protected virtual void OnCreateDisplayNodeView()
        {
            ClearSelection();
            RecreateNavigationItems();
        }

        /// <summary>
        /// 是前景画布视图
        /// </summary>
        public bool isForegroundCanvasView => nodeKitEditor.foregroundCanvasView == this;

        /// <summary>
        /// 尝试设置子级节点视图为前景画布视图
        /// </summary>
        public void TrySetChildNodeViewAsForegroundCanvasView()
        {
            if (childrenNodeViews.Count == 1 && childrenNodeViews[0].TrySetAsForegroundCanvasView(out var canvasView))
            {
                canvasView.TrySetChildNodeViewAsForegroundCanvasView();
            }
        }

        #endregion

        #region 画布连接

        /// <summary>
        /// 能连接
        /// </summary>
        /// <param name="fromNodeView"></param>
        /// <param name="toNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanConnect(NodeView fromNodeView, NodeView toNodeView, ICanvasContext canvasContext)
        {
            if (fromNodeView == null || toNodeView == null) return false;
            if (fromNodeView.CanConnectTo(toNodeView, canvasContext) && toNodeView.CanConnectFrom(fromNodeView, canvasContext))
            {
                if (canvasModel is ICanvasConnect canvasConnect)
                {
                    return canvasConnect.CanConnect(fromNodeView.nodeModel, toNodeView.nodeModel, canvasContext);
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <param name="fromNodeView"></param>
        /// <param name="toNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool TryConnect(NodeView fromNodeView, NodeView toNodeView, ICanvasContext canvasContext)
        {
            if (fromNodeView == null || toNodeView == null) return false;
            if (fromNodeView.TryConnectTo(toNodeView, canvasContext, out _) && toNodeView.TryConnectFrom(fromNodeView, canvasContext, out _))
            {
                if (canvasModel is ICanvasConnect canvasConnect)
                {
                    return canvasConnect.TryConnect(fromNodeView.nodeModel, toNodeView.nodeModel, canvasContext);
                }
            }
            return false;
        }

        /// <summary>
        /// 创建选择集连接
        /// </summary>
        /// <param name="connectionNodeView">连接节点视图</param>
        /// <param name="isOut">入或出</param>
        public virtual bool CreateSelectionConection(NodeView connectionNodeView, bool isOut) => connectionNodeView != null && CreateSelectionConection(connectionNodeView.nodeModel, isOut);

        /// <summary>
        /// 创建选择集连接
        /// </summary>
        /// <param name="connectionNodeModel">连接节点模型</param>
        /// <param name="isOut">入或出</param>
        public virtual bool CreateSelectionConection(INodeModel connectionNodeModel, bool isOut)
        {
            if (connectionNodeModel.ObjectIsNull()) return false;
            var result = false;
            var nodes = new List<NodeView>(nodeSelections);
            if (canvasModel is ICanvasConnect canvasConnect)
            {
                if (isOut)
                {
                    foreach (var item in nodes)
                    {
                        if (canvasConnect.TryConnect(item.nodeModel, connectionNodeModel, CanvasContext.Default)) result = true;
                    }
                }
                else
                {
                    foreach (var item in nodes)
                    {
                        if (canvasConnect.TryConnect(connectionNodeModel, item.nodeModel, CanvasContext.Default)) result = true;
                    }
                }
            }
            return result;
        }

        #endregion

        #region 克隆

        /// <summary>
        /// 可克隆
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="withInOut"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanClone(IEnumerable<NodeView> nodeViews, bool withInOut, ICanvasContext canvasContext)
        {
            if (canvasModel is ICanvasClone canvasClone && nodeViews != null && nodeViews.Any(n => n.CanClone(this, withInOut, canvasContext)))
            {
                return canvasClone.CanClone(nodeViews.Cast(nv => nv.nodeModel), canvasContext);
            }
            return false;
        }

        /// <summary>
        /// 尝试克隆
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="withInOut"></param>
        /// <param name="canvasContext"></param>
        /// <param name="cloneDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryClone(IEnumerable<NodeView> nodeViews, bool withInOut, ICanvasContext canvasContext, out object cloneDescriptor)
        {
            if (nodeViews != null)
            {
                foreach (var n in nodeViews)
                {
                    n.TryClone(this, withInOut, canvasContext, out _);
                }
                if (canvasModel is ICanvasClone canvasClone)
                {
                    return canvasClone.TryClone(nodeViews.Cast(nv => nv.nodeModel), canvasContext, out cloneDescriptor);
                }
            }
            cloneDescriptor = default;
            return false;
        }

        /// <summary>
        /// 克隆选择集
        /// </summary>
        /// <param name="withInOut"></param>
        /// <returns></returns>
        public virtual bool CloneSelection(bool withInOut = false) => TryClone(_nodeSelections.ToArray(), withInOut, CanvasContext.Default, out _);

        /// <summary>
        /// 克隆选择集
        /// </summary>
        /// <returns></returns>
        [CanvasToolBar(nameof(DuplicateSelection), group = 1, index = 4)]
        [Name("克隆")]
        [XCSJ.Attributes.Icon(EIcon.Copy)]
        public virtual bool DuplicateSelection() => CloneSelection();

        /// <summary>
        /// 克隆选择集验证
        /// </summary>
        [CanvasToolBar(nameof(DuplicateSelection), true)]
        public bool DuplicateSelectionValidate() => _nodeSelections.Any(n => n.CanClone(this, false, CanvasContext.Default));

        #endregion

        #region 删除

        /// <summary>
        /// 能删除
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanDelete(IEnumerable<NodeView> nodeViews, ICanvasContext canvasContext)
        {
            if (canvasModel is ICanvasDelete canvasClone && nodeViews != null && nodeViews.Any(n => n.CanDelete(this, canvasContext)))
            {
                return canvasClone.CanDelete(nodeViews.Cast(nv => nv.nodeModel), canvasContext);
            }
            return false;
        }

        /// <summary>
        /// 尝试删除
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="canvasContext"></param>
        /// <param name="deleteDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryDelete(IEnumerable<NodeView> nodeViews, ICanvasContext canvasContext, out object deleteDescriptor)
        {
            if (nodeViews != null)
            {
                foreach (var n in nodeViews)
                {
                    n.TryDelete(this, canvasContext, out _);
                }
                if (canvasModel is ICanvasDelete canvasDelete)
                {
                    return canvasDelete.TryDelete(nodeViews.Cast(nv => nv.nodeModel), canvasContext, out deleteDescriptor);
                }
            }
            deleteDescriptor = default;
            return false;
        }

        /// <summary>
        /// 删除选择集
        /// </summary>
        /// <returns></returns>
        [CanvasToolBar(nameof(DeleteSelection), group = 1, index = 0)]
        [Name("删除")]
        [XCSJ.Attributes.Icon(EIcon.Delete)] 
        public virtual bool DeleteSelection() => TryDelete(_nodeSelections.ToArray(), CanvasContext.Default, out _);

        /// <summary>
        /// 删除选择集验证
        /// </summary>
        [CanvasToolBar(nameof(DeleteSelection), true)]
        public bool DeleteSelectionValidate() => _nodeSelections.Any(n => n.CanDelete(this, CanvasContext.Default));

        #endregion

        #region 复制

        /// <summary>
        /// 可复制
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanCopy(IEnumerable<NodeView> nodeViews, ICanvasContext canvasContext)
        {
            if (nodeViews != null && nodeViews.Any(n => n.CanCopy(this, canvasContext)))
            {
                if (canvasModel is ICanvasClone canvasClone)
                {
                    return canvasClone.CanClone(nodeViews.Cast(nv => nv.nodeModel), canvasContext);
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试复制
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="canvasContext"></param>
        /// <param name="copyDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryCopy(IEnumerable<NodeView> nodeViews, ICanvasContext canvasContext, out object copyDescriptor)
        {
            copyDescriptor = default;
            if (nodeViews != null)
            {
                pasteNodeViews.Clear();
                pasteNodeViews.AddRange(nodeViews);
                pasteType = EPasteType.Copy;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 复制选择集
        /// </summary>
        /// <returns></returns>
        [CanvasToolBar(nameof(CopySelection), group = 1, index = 3)]
        [Name("复制")]
        [XCSJ.Attributes.Icon(EIcon.Copy)]
        public virtual bool CopySelection() => TryCopy(nodeSelections, CanvasContext.Default, out _);

        /// <summary>
        /// 复制选择集验证
        /// </summary>
        [CanvasToolBar(nameof(CopySelection), true)]
        public bool CopySelectionValidate() => _nodeSelections.Any(n => n.CanCopy(this, CanvasContext.Default));

        #endregion

        #region 剪切

        /// <summary>
        /// 可剪切
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanCut(IEnumerable<NodeView> nodeViews, ICanvasContext canvasContext)
        {
            if (nodeViews == null) return false;
            if (canvasModel is ICanvasClone canvasClone && nodeViews.Any(n => n.CanCut(this, canvasContext)))
            {
                if (canvasClone.CanClone(nodeViews.Cast(nv => nv.nodeModel), canvasContext))
                {
                    if (canvasModel is ICanvasDelete canvasDelete && nodeViews.Any(n => n.CanDelete(this, canvasContext)))
                    {
                        if (canvasDelete.CanDelete(nodeViews.Cast(nv => nv.nodeModel), canvasContext))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试剪切
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="canvasContext"></param>
        /// <param name="cutDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryCut(IEnumerable<NodeView> nodeViews, ICanvasContext canvasContext, out object cutDescriptor)
        {
            cutDescriptor = default;
            if (nodeViews != null)
            {
                pasteNodeViews.Clear();
                pasteNodeViews.AddRange(_nodeSelections);
                pasteType = EPasteType.Cut;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 剪切选择集
        /// </summary>
        /// <returns></returns>
        [CanvasToolBar(nameof(CutSelection), group = 1, index = 2)]
        [Name("剪切选择集")]
        [XCSJ.Attributes.Icon(EIcon.Cut)]
        public virtual bool CutSelection() => TryCut(nodeSelections, CanvasContext.Default, out _);

        /// <summary>
        /// 剪切选择集验证
        /// </summary>
        [CanvasToolBar(nameof(CutSelection), true)]
        public bool CutSelectionValidate() => _nodeSelections.Any(n => n.CanCut(this, CanvasContext.Default));

        #endregion

        #region 粘贴

        /// <summary>
        /// 可粘贴
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanPaste(IEnumerable<NodeView> nodeViews, ICanvasContext canvasContext)
        {
            if (nodeViews == null) return false;
            if (nodeViews.Any(n => n.CanPaste(this, canvasContext)))
            {
                if (canvasModel is ICanvasClone canvasClone)
                {
                    return canvasClone.CanClone(nodeViews.Cast(nv => nv.nodeModel), canvasContext);
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试粘贴
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="canvasContext"></param>
        /// <param name="pasteDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryPaste(IEnumerable<NodeView> nodeViews, ICanvasContext canvasContext, out object pasteDescriptor)
        {
            if (nodeViews != null && nodeViews.Any(n => n.TryPaste(this, canvasContext, out _)))
            {
                if (canvasModel is ICanvasClone canvasClone)
                {
                    return canvasClone.TryClone(nodeViews.Cast(nv => nv.nodeModel), canvasContext, out pasteDescriptor);
                }
            }
            pasteDescriptor = default;
            return false;
        }

        /// <summary>
        /// 粘贴类型
        /// </summary>
        public enum EPasteType
        {
            /// <summary>
            /// 无
            /// </summary>
            None,

            /// <summary>
            /// 复制
            /// </summary>
            Copy,

            /// <summary>
            /// 剪切
            /// </summary>
            Cut,
        }

        private static EPasteType pasteType = EPasteType.None;

        private static List<NodeView> pasteNodeViews = new List<NodeView>();

        /// <summary>
        /// 粘贴节点数量
        /// </summary>
        public static int pasteNodeCount => pasteNodeViews.Count;

        /// <summary>
        /// 粘贴
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 粘贴
        /// </summary>
        [CanvasToolBar(nameof(Paste), group = 1, index = 1)]
        [Name("粘贴")]
        [XCSJ.Attributes.Icon(EIcon.Paste)]
        public virtual bool Paste()
        {
            try
            {
                return TryPaste(pasteNodeViews, CanvasContext.Default, out _);
            }
            finally
            {
                if (pasteType == EPasteType.Cut)
                {
                    TryDelete(pasteNodeViews, CanvasContext.Default, out _);
                }
                pasteType = EPasteType.None;
                pasteNodeViews.Clear();
            }
        }

        /// <summary>
        /// 粘贴验证
        /// </summary>
        [CanvasToolBar(nameof(Paste), true)]
        public bool PasteValidate() => pasteNodeViews.Any();

        #endregion

        #region 初始化启用禁用

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            parentCanvasView = parent as CanvasView;
            HandleRefresh();
        }

        /// <summary>
        /// 缩放区域
        /// </summary>
        public ZoomArea zoomArea { get; private set; } = new ZoomArea();

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            zoomArea.onZoomChanged += OnZoom;
            Refresh(true);
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            zoomArea.onZoomChanged -= OnZoom;
        }

        /// <summary>
        /// 当聚焦
        /// </summary>
        protected override void OnFocus()
        {
            base.OnFocus();
            Refresh(true);
        }

        /// <summary>
        /// 当节点事件
        /// </summary>
        /// <param name="nodeEventArgs"></param>
        protected override void OnNodeEvent(NodeEventArgs nodeEventArgs)
        {
            base.OnNodeEvent(nodeEventArgs);
            HandleRefresh();
        }

        /// <summary>
        /// 当撤销重做已执行
        /// </summary>
        protected override void OnUndoRedoPerformed()
        {
            base.OnUndoRedoPerformed();
            DelayRefresh();
        }

        #endregion

        #region 绘制

        private void OnZoom(Vector2 offset)
        {
            gridBackground.panOffset -= offset;
            Move(-offset);
        }

        /// <summary>
        /// 画布矩形
        /// </summary>
        public override Rect canvasRect
        {
            get => base.canvasRect;
            set
            {
                zoomCanvas = base.canvasRect = value;
                zoomCanvas.position = Vector2.zero;
            }
        }

        private Rect zoomCanvas = new Rect();

        /// <summary>
        /// 调用当启用
        /// </summary>
        internal void CallOnEnable() => OnEnable();

        /// <summary>
        /// 调用当禁用
        /// </summary>
        internal void CallOnDisable() => OnDisable();

        /// <summary>
        /// 调用当绘制GUI
        /// </summary>
        /// <param name="canvasRect"></param>
        internal void CallOnGUI(Rect canvasRect)
        {
            this.canvasRect = canvasRect;
            OnGUI();
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            //base.OnGUI();

            OnGUIGridBackground();

            OnGUIOutZoomAreaBefore();

            zoomArea.Draw(canvasRect, 22, () => OnGUIZoomArea());

            OnGUIOutZoomAreaAfter();

            OnMouseEvent();
        }

        /// <summary>
        /// 当绘制缩放区域外部之前
        /// </summary>
        internal protected virtual void OnGUIOutZoomAreaBefore() { }

        /// <summary>
        /// 当绘制节点视图集
        /// </summary>
        protected virtual void OnGUINodeViews()
        {
            foreach (var item in displayNodeViews)
            {
                item?.CallOnGUI();
            }
        }

        /// <summary>
        /// 当绘制缩放区域
        /// </summary>
        internal protected virtual void OnGUIZoomArea()
        {
            OnGUINodeViews();

            OnGUIDragSelectBox();

            OnGUIConnecting();
        }

        /// <summary>
        /// 当绘制缩放区域外部之后
        /// </summary>
        internal protected virtual void OnGUIOutZoomAreaAfter()
        {
            OnGUIMiniMap();
            OnGUICanvasToolBar();
        }

        #endregion

        #region 工具栏

        /// <summary>
        /// 重新创建导航项列表
        /// </summary>
        protected virtual void RecreateNavigationItems()
        {
            navigationItemGUIContent.text = displayName;
            navigationItemGUIContent.tooltip = CommonFun.Name(canvasModel.GetType());

            navigationItems.Clear();
            foreach (var cm in this.GetParentCanvasViews())
            {
                navigationItems.Add(cm);
            }
        }

        /// <summary>
        /// 导航项GUI内容
        /// </summary>
        protected GUIContent navigationItemGUIContent = new GUIContent();

        /// <summary>
        /// 导航项列表
        /// </summary>
        public virtual List<object> navigationItems { get; private set; } = new List<object>();

        /// <summary>
        /// 获取导航项GUI内容
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual GUIContent GetNavigationItemGUIContent(object item)
        {
            if (item is CanvasView canvasView) return canvasView.navigationItemGUIContent;
            if (item is BaseCanvasView baseCanvasView) return CommonFun.TempContent(baseCanvasView.displayName, CommonFun.Name(baseCanvasView.canvasModel.GetType()));

            var scene = SceneManager.GetActiveScene();
            return CommonFun.TempContent(scene.name, scene.path);
        }

        /// <summary>
        /// 当点击导航项
        /// </summary>
        /// <param name="item"></param>
        public virtual void OnClickNavigationItem(object item)
        {
            nodeKitEditor.foregroundCanvasView = item as CanvasView;
        }

        /// <summary>
        /// 当工具栏绘制
        /// </summary>
        internal protected virtual void OnGUIToolBar()
        {
            NavigationBar.Draw(this);
        }

        #endregion

        #region 状态栏

        /// <summary>
        /// 状态栏信息
        /// </summary>
        public virtual string stateBarInfo
        {
            get
            {
                var info = "";
                if (nodeSelections.Count > 0) info += string.Format("Selected Node Count".Tr(this) + "：{0}", nodeSelections.Count);
                switch (pasteType)
                {
                    case EPasteType.Copy:
                        {
                            if (pasteNodeCount > 0) info += string.Format("Copy Node Count".Tr(this) + "：{0}", nodeSelections.Count);
                            break;
                        }
                    case EPasteType.Cut:
                        {
                            if (pasteNodeCount > 0) info += string.Format("Cut Node Count".Tr(this) + "：{0}", nodeSelections.Count);
                            break;
                        }
                }
                return info;
            }
        }

        /// <summary>
        /// 当状态栏绘制
        /// </summary>
        [LanguageTuple("Selected Node Count", "选中节点数")]
        [LanguageTuple("Copy Node Count", "复制节点数")]
        [LanguageTuple("Cut Node Count", "剪切节点数")]
        internal protected virtual void OnGUIStateBar()
        {
            GUILayout.Label(stateBarInfo, GUILayout.ExpandWidth(true));
        }

        #endregion

        #region 分类

        /// <summary>
        /// 分类宽度
        /// </summary>
        public float categoryWidth { get; private set; }

        internal void CallOnGUICategory(float categoryWidth)
        {
            this.categoryWidth = categoryWidth;
            OnGUICategory();
        }

        /// <summary>
        /// 当绘制分类
        /// </summary>
        internal protected virtual void OnGUICategory()
        {
            if (nodeKitEditor.backgroundCanvasView is NodeKitEditorCanvasView nodeKitEditorCanvasView)
            {
                nodeKitEditorCanvasView.OnGUICategoryDefault();
            }
            else
            {
                GUILayout.FlexibleSpace();
            }
        }

        /// <summary>
        /// 当搜索内容已修改
        /// </summary>
        /// <param name="searchContext"></param>
        public virtual void OnSearchContextChanged(ISearchContext searchContext)
        {
            //Debug.Log("OnSearchContextChanged: " + searchContext.searchText + "==>" + searchContext.invert);
            foreach (var nodeView in nodeViews)
            {
                nodeView?.OnSearchContextChanged(searchContext);
            }
        }

        #endregion

        #region 检查器

        /// <summary>
        /// 检查器宽度
        /// </summary>
        public float inspectorWidth { get; private set; }

        /// <summary>
        /// 检查器节点视图：当前绘制的检查器对应的节点视图
        /// </summary>
        public BaseEntityNodeView inspectorNodeView { get; private set; }

        internal void CallOnGUIInspector(float inspectorWidth)
        {
            this.inspectorWidth = inspectorWidth;
            OnGUIInspector();
        }

        /// <summary>
        /// 当绘制检查器
        /// </summary>
        internal protected virtual void OnGUIInspector()
        {
            if (isForegroundCanvasView && _nodeSelections.FirstOrDefault() is NodeView nodeView)
            {
                inspectorNodeView = nodeView;
                nodeView.CallOnGUIInspector(inspectorWidth);
                return;
            }
            inspectorNodeView = this;
            if (nodeModel.DrawNodeModelInspector(inspectorWidth)) return;

            GUILayout.FlexibleSpace();
        }

        #endregion

        #region 网格背景

        /// <summary>
        /// 网格背景
        /// </summary>
        public GirdBackground gridBackground { get; } = new GirdBackground();

        /// <summary>
        /// 当绘制网格背景
        /// </summary>
        internal protected virtual void OnGUIGridBackground() => gridBackground.Draw(canvasRect, zoomArea.zoom);

        #endregion

        #region 选择集

        /// <summary>
        /// 节点选择集
        /// </summary>
        private List<NodeView> _nodeSelections = new List<NodeView>();

        /// <summary>
        /// 绘制拖拽选择矩形
        /// </summary>
        protected bool drawDragSelectRect { get; private set; } = false;

        /// <summary>
        /// 节点选择集
        /// </summary>
        public List<NodeView> nodeSelections => _nodeSelections;

        /// <summary>
        /// 拖拽旋转矩形
        /// </summary>
        protected Rect dragSelectRect { get; private set; } = new Rect();

        /// <summary>
        /// 缩放鼠标按下位置
        /// </summary>
        protected Vector2 zoomMousePositionDown = Vector2.zero;

        /// <summary>
        /// 添加到选择集
        /// </summary>
        /// <param name="nodeView"></param>
        /// <param name="isOverride"></param>
        public void AddToSelection(NodeView nodeView, bool isOverride)
        {
            if (isOverride)
            {
                var tmpNodeValue = nodeView;
                for (int i = _nodeSelections.Count - 1; i >= 0; --i)
                {
                    var selection = _nodeSelections[i];
                    if (selection != tmpNodeValue)
                    {
                        _nodeSelections.RemoveAt(i);
                        selection.OnUnselect();
                    }
                    else
                    {
                        nodeView = null;
                    }
                }
            }
            if (nodeView == null) return;
            if (nodeView.canvasView == this && _nodeSelections.AddWithDistinct(nodeView))
            {
                nodeView.OnSelected();
            }
        }

        /// <summary>
        /// 从选择集移除
        /// </summary>
        /// <param name="nodeView"></param>
        public void RemoveFromSelection(NodeView nodeView)
        {
            if (nodeView == null) return;

            if (_nodeSelections.Remove(nodeView))
            {
                nodeView.OnUnselect();
            }
        }

        /// <summary>
        /// 包含在选择集中
        /// </summary>
        /// <param name="nodeView"></param>
        /// <returns></returns>
        public bool InSelection(NodeView nodeView) => _nodeSelections.Contains(nodeView);

        /// <summary>
        /// 清除选择集
        /// </summary>
        public void ClearSelection()
        {
            foreach (var item in _nodeSelections)
            {
                item.OnUnselect();
            }
            _nodeSelections.Clear();
        }

        private void UpdateDragSelectRect(Vector2 p1, Vector2 p2)
        {
            dragSelectRect = Rect.MinMaxRect(Mathf.Min(p1.x, p2.x), Mathf.Min(p1.y, p2.y), Mathf.Max(p1.x, p2.x), Mathf.Max(p1.y, p2.y));
        }

        private void OnGUIDragSelectBox()
        {
            if (!drawDragSelectRect) return;

            GUI.Box(dragSelectRect, "", customStyle.selectionRectStyle);

            displayNodeViews.ForEach(nv =>
            {
                if (nv.nodeRect.Overlaps(dragSelectRect))
                {
                    AddToSelection(nv, false);
                }
                else
                {
                    RemoveFromSelection(nv);
                }
            });
        }

        #endregion

        #region 小地图

        /// <summary>
        /// 显示小地图
        /// </summary>
        public bool displayMiniMap = true;

        /// <summary>
        /// 获取视图矩形
        /// </summary>
        /// <returns></returns>
        public virtual Rect GetViewRect() => new Rect(Vector2.zero, canvasRect.size / zoomArea.zoom);//rect;

        private Rect GetCanvasRightBottomCornerRect(Vector2 size) => new Rect(canvasRect.xMax - size.x, canvasRect.yMax - size.y, size.x, size.y);

        /// <summary>
        /// 当绘制小地图
        /// </summary>
        internal protected virtual void OnGUIMiniMap()
        {
            if (displayMiniMap)
            {
                var viewRect = GetViewRect();
                Rect wordRect = MathU.GetRectBounds(MathU.GetRectBounds(displayNodeViews.Cast(n => n.nodeRect).ToArray()), viewRect);

                var miniMapOption = MiniMapOption.weakInstance;
                var miniMapRect = GetCanvasRightBottomCornerRect(customStyle.miniMapSize);
                MiniMap.Draw(miniMapRect, wordRect, viewRect, miniMapOption.viewColor, miniMapOption.boundsColor, miniMapOption.backgroundColor, OnGUIMiniMapItems, eventData =>
                {
                    //编辑器拖拽节点的时候，不响应小地图鼠标
                    if (dragDistance.magnitude < 10)
                    {
                        Move(eventData.GetOffsetOfCenter());
                    }
                });
            }

            if (GUI.Button(GetCanvasRightBottomCornerRect(customStyle.miniMapControlButtonSize), customStyle.miniMapLabel, customStyle.iconButtonStyle))
            {
                displayMiniMap = !displayMiniMap;
            }
        }

        /// <summary>
        /// 当绘制小地图项集合
        /// </summary>
        /// <param name="miniMapData"></param>
        internal protected virtual void OnGUIMiniMapItems(MiniMapData miniMapData)
        {
            Vector2 leftTopOffset = -new Vector2(miniMapData.boundsRect.x < 0 ? miniMapData.boundsRect.x : 0, miniMapData.boundsRect.y < 0 ? miniMapData.boundsRect.y : 0);

            if (drawDragSelectRect)
            {
                var selectionRect = dragSelectRect;
                selectionRect.position = (selectionRect.position + leftTopOffset) * miniMapData.scaleValue + miniMapData.miniMapBoundsRect.position;
                selectionRect.size *= miniMapData.scaleValue;

                GUI.Box(selectionRect, GUIContent.none, customStyle.selectionRectStyle);
            }

            foreach (NodeView node in displayNodeViews)
            {
                Vector2 nodePos = (node.nodeRect.position + leftTopOffset) * miniMapData.scaleValue + miniMapData.miniMapBoundsRect.position;
                Vector2 nodeSize = node.nodeRect.size * miniMapData.scaleValue;

                node.OnGUIMiniMap(new Rect(nodePos, nodeSize), miniMapData.scaleValue);
            }

            var mousePosition = localMousePosition;
            mousePosition = (mousePosition + leftTopOffset) * miniMapData.scaleValue + miniMapData.miniMapBoundsRect.position;
            if (miniMapData.miniMapRect.Contains(mousePosition))
            {
                GUI.Box(new Rect(mousePosition, new Vector2(4, 4)), GUIContent.none, customStyle.selectionRectStyle);
            }
        }

        #endregion

        #region 鼠标处理

        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector2 mousePosition { get; set; } = new Vector2(0, 0);

        /// <summary>
        /// 本地鼠标位置：相对于本画布矩形的鼠标位置
        /// </summary>
        public Vector2 localMousePosition => MousePositionToLocal(mousePosition);

        /// <summary>
        /// 缩放鼠标位置：本地鼠标位置经过缩放区域变换后的鼠标位置
        /// </summary>
        public Vector2 zoomMousePosition => MousePositionToZoom(mousePosition);

        /// <summary>
        /// 鼠标位置到本地
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public Vector2 MousePositionToLocal(Vector2 mousePosition) => mousePosition - canvasRect.position;

        /// <summary>
        /// 鼠标位置到缩放
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public Vector2 MousePositionToZoom(Vector2 mousePosition) => zoomArea.ScreenToZoom(MousePositionToLocal(mousePosition));

        /// <summary>
        /// 当鼠标事件：鼠标左键、中键、右键和滚轮事件
        /// </summary>
        internal protected virtual void OnMouseEvent()
        {
            var e = Event.current;
            mousePosition = e.mousePosition;

            switch (e.button)
            {
                case 0:// 左键
                    {
                        switch (e.rawType)
                        {
                            case EventType.MouseDown:
                                {
                                    OnMouseLeftButtonDown(e);

                                    // 只有按下时，clickCount才可能等于2
                                    if (e.clickCount == 2)
                                    {
                                        OnMouseLeftButtonDoubleClick(e);
                                    }
                                    break;
                                }
                            case EventType.MouseUp: OnMouseLeftButtonUp(e); break;
                            case EventType.MouseDrag: OnMouseLeftButtonDrag(e, zoomArea.ScreenToZoom(e.delta)); break;
                        }
                        break;
                    }
                case 1:// 右键
                    {
                        switch (e.rawType)
                        {
                            case EventType.MouseDown: OnMouseRightButtonDown(e); break;
                            case EventType.MouseUp: OnMouseRightButtonUp(e); break;
                            case EventType.MouseDrag: OnMouseRightButtonDrag(e); break;
                        }
                        break;
                    }
                case 2:// 中键
                    {
                        switch (e.rawType)
                        {
                            case EventType.MouseDown: OnMouseMiddleButtonDown(e); break;
                            case EventType.MouseUp: OnMouseMiddleButtonUp(e); break;
                            case EventType.MouseDrag: OnMouseMiddleButtonDrag(e); break;
                        }
                        break;
                    }
            }

            // 鼠标滚轮事件
            if (e.type == EventType.ScrollWheel)
            {
                OnMouseScrollWheel(e);
            }
            OnMouseMove();
            OnPopupNodeViewMenu(e);
        }

        NodeView _hoverNodeView;

        /// <summary>
        /// 悬停节点视图
        /// </summary>
        public NodeView hoverNodeView
        {
            get => _hoverNodeView;
            protected set
            {
                if (_hoverNodeView != value)
                {
                    _hoverNodeView?.OnHoverExit();
                    _hoverNodeView = value;
                    _hoverNodeView?.OnHoverEnter();
                }
            }
        }

        /// <summary>
        /// 当鼠标移动
        /// </summary>
        protected virtual void OnMouseMove() => hoverNodeView = PickHoverNodeView(displayNodeViews, zoomMousePosition);

        /// <summary>
        /// 拾取悬停节点视图
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        protected NodeView PickHoverNodeView(List<NodeView> nodeViews, Vector2 mousePosition)
        {
            foreach (var item in nodeViews)
            {
                if (item.InHoverArea(mousePosition)) return item;
            }
            return default;
        }

        /// <summary>
        /// 鼠标在画布矩形内
        /// </summary>
        /// <returns></returns>
        internal protected virtual bool IsMouseInCanvasRect() => canvasModel.canvasRect.Contains(mousePosition);

        #endregion

        #region 鼠标左键:处理选择、拖拽和连线

        /// <summary>
        /// 鼠标右键按下节点视图
        /// </summary>
        public NodeView mouseLeftDownNodeView { get; protected set; }

        /// <summary>
        /// 当鼠标左键按下
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseLeftButtonDown(Event e)
        {
            zoomMousePositionDown = zoomMousePosition;
            dragDistance = Vector2.zero;
            if (!IsMouseInCanvasRect())
            {
                mouseLeftDownNodeView = null;
                return;
            }

            mouseLeftDownNodeView = PickNodeView(zoomMousePosition);

            if (isConnecting)
            {
                EndConnnection(mouseLeftDownNodeView);
            }
            else
            {
                // 选中节点
                if (mouseLeftDownNodeView != null)
                {
                    if (!_nodeSelections.Contains(mouseLeftDownNodeView))
                    {
                        AddToSelection(mouseLeftDownNodeView, true);
                    }
                    mouseLeftDownNodeView.OnMouseLeftButtonDown(e);
                }
                else
                {
                    ClearSelection();
                    drawDragSelectRect = true;
                    dragSelectRect = new Rect(zoomMousePositionDown, Vector2.zero);
                }
            }
        }

        private Vector2 dragDistance = Vector2.zero;

        /// <summary>
        /// 当鼠标左键拖拽
        /// </summary>
        /// <param name="e"></param>
        /// <param name="offset"></param>
        internal protected virtual void OnMouseLeftButtonDrag(Event e, Vector2 offset)
        {
            dragDistance += offset;
            if (drawDragSelectRect)
            {
                UpdateDragSelectRect(zoomMousePositionDown, zoomMousePosition);
            }
            else
            {
                if (mouseLeftDownNodeView != null && _nodeSelections.Contains(mouseLeftDownNodeView))
                {
                    foreach (var item in _nodeSelections)
                    {
                        item.OnMouseLeftButtonDrag(e, offset);
                    }
                }
            }

            nodeKitEditor.Repaint();
        }

        /// <summary>
        /// 当鼠标左键弹起
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseLeftButtonUp(Event e)
        {
            drawDragSelectRect = false;

            if (!IsMouseInCanvasRect()) return;

            var node = PickNodeView(zoomMousePosition);
            if (node != null)
            {
                // 拖拽距离超过指定值按拖拽处理
                if (dragDistance.magnitude > 10)
                {

                }
                else // 覆盖式选中节点
                {
                    if (node == mouseLeftDownNodeView)
                    {
                        AddToSelection(node, true);
                    }
                }

                node.OnMouseLeftButtonUp(e);
            }
            else
            {
                EndConnnection(null);
            }
        }

        /// <summary>
        /// 当鼠标左键双击
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseLeftButtonDoubleClick(Event e)
        {
            if (!IsMouseInCanvasRect()) return;

            if (PickNodeView(zoomMousePosition) is NodeView nodeView)
            {
                nodeView.OnMouseLeftButtonDoubleClick(e);
            }
            else
            {
                UICommonFun.PingObject(this.GetGameObject());
            }
        }

        #endregion

        #region 鼠标中键：处理画布缩放和拖拽

        /// <summary>
        /// 当鼠标滚轮滚动
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseScrollWheel(Event e)
        {
            if (!IsMouseInCanvasRect()) return;

            zoomArea.Zoom(canvasModel.canvasRect, localMousePosition);
        }

        /// <summary>
        /// 当鼠标中键按下
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseMiddleButtonDown(Event e) { }

        /// <summary>
        /// 当鼠标中键弹起
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseMiddleButtonUp(Event e) { }

        /// <summary>
        /// 当鼠标中键拖拽
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseMiddleButtonDrag(Event e)
        {
            Move(zoomArea.ScreenToZoom(e.delta));

            Repaint();
        }

        #endregion

        #region 鼠标右键：处理弹出菜单

        /// <summary>
        /// 鼠标右键按下节点视图
        /// </summary>
        public NodeView mouseRightDownNodeView { get; protected set; }

        /// <summary>
        /// 当鼠标右键按下
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseRightButtonDown(Event e)
        {
            if (!IsMouseInCanvasRect()) return;

            mouseRightDownNodeView = PickNodeView(zoomMousePosition);
            mouseRightDownNodeView?.OnMouseRightButtonDown(e);
        }

        /// <summary>
        /// 当鼠标右键拖拽
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseRightButtonDrag(Event e)
        {

        }

        /// <summary>
        /// 当鼠标右键弹起
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseRightButtonUp(Event e)
        {
            if (!IsMouseInCanvasRect()) return;

            var nodeView = PickNodeView(zoomMousePosition);
            nodeView?.OnMouseRightButtonUp(e);
            if (mouseRightDownNodeView == null && nodeView == null)
            {
                OnMouseRightButtonAsButton(e);
            }
            else if (mouseRightDownNodeView != null && mouseRightDownNodeView == nodeView)
            {
                // 延时弹起菜单，保证当前节点被选中
                AddToSelection(nodeView, !InSelection(nodeView));
                popupMenuNodeView = nodeView;
            }
            mouseRightDownNodeView = null;

            EndConnnection(null);
        }

        private NodeView popupMenuNodeView;
        private void OnPopupNodeViewMenu(Event e)
        {
            if (popupMenuNodeView == null || e.rawType != EventType.Repaint) return;
            try
            {
                popupMenuNodeView.OnMouseRightButtonAsButton(e);
            }
            finally
            {
                popupMenuNodeView = null;
            }
        }

        /// <summary>
        /// 当鼠标右键作为按钮
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseRightButtonAsButton(Event e) => PopupMenu();

        /// <summary>
        /// 弹出菜单
        /// </summary>
        protected void PopupMenu() => MenuHelper.DrawMenu(GetType().Name, menuInfo => OnPopupMenu(menuInfo));

        /// <summary>
        /// 当弹出菜单
        /// </summary>
        /// <param name="menuInfo"></param>
        [LanguageTuple("Paste", "粘贴")]
        protected virtual void OnPopupMenu(MenuInfo menuInfo)
        {
            if (pasteType != EPasteType.None && pasteNodeCount > 0)
            {
                menuInfo.AddMenuItem("Paste".Tr(this), () => Paste(), null, 10, PluginCommonUtils.Menus.ESeparatorType.TopDown);
            }
        }

        #endregion

        #region 连线

        /// <summary>
        /// 开始连接节点视图:发起连接的节点视图
        /// </summary>
        public NodeView beginConnectionNodeView { get; protected set; }

        /// <summary>
        /// 连接中
        /// </summary>
        public bool isConnecting => connectState != EConnectState.None;

        /// <summary>
        /// 连接状态
        /// </summary>
        public EConnectState connectState { get; private set; } = EConnectState.None;

        /// <summary>
        /// 连接状态
        /// </summary>
        public enum EConnectState
        {
            /// <summary>
            /// 无
            /// </summary>
            None,

            /// <summary>
            /// 入
            /// </summary>
            In,

            /// <summary>
            /// 出
            /// </summary>
            Out,
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="nodeView"></param>
        /// <param name="connectOut"></param>
        internal protected void BeginConnection(NodeView nodeView, bool connectOut = true)
        {
            if (isConnecting) return;
            connectState = connectOut ? EConnectState.Out : EConnectState.In;
            beginConnectionNodeView = nodeView;
            AddToSelection(nodeView, false);

            OnBeginConnection();
        }

        /// <summary>
        /// 当开始连接
        /// </summary>
        internal protected virtual void OnBeginConnection() { }

        /// <summary>
        /// 当结束连接
        /// </summary>
        /// <param name="nodeView"></param>
        internal protected void EndConnnection(NodeView nodeView)
        {
            if (!isConnecting) return;
            try
            {
                CreateSelectionConection(nodeView, connectState == EConnectState.Out);
            }
            finally
            {
                OnEndConnection();
                ClearSelection();
            }
        }

        /// <summary>
        /// 当结束连接
        /// </summary>
        internal protected virtual void OnEndConnection()
        {
            connectState = EConnectState.None;
            beginConnectionNodeView = null;
        }

        /// <summary>
        /// 当绘制正在连接
        /// </summary>
        internal protected virtual void OnGUIConnecting()
        {
            switch (connectState)
            {
                case EConnectState.In:
                    {
                        foreach (var item in nodeSelections)
                        {
                            NodeKitHelperExtension.DrawStraightLine(zoomMousePosition, item.nodeRect.center, true);
                        }
                        break;
                    }
                case EConnectState.Out:
                    {
                        foreach (var item in nodeSelections)
                        {
                            NodeKitHelperExtension.DrawStraightLine(item.nodeRect.center, zoomMousePosition, true);
                        }
                        break;
                    }
            }
        }

        #endregion

        #region 画布工具栏

        /// <summary>
        /// 当绘制画布工具栏
        /// </summary>
        internal protected virtual void OnGUICanvasToolBar()
        {
            if (displayCanvasToolBar) canvasToolBar.Draw(canvasRect);

            if (GUI.Button(GetCanvasRightUpCornerRect(customStyle.miniMapControlButtonSize), customStyle.canvasToolBarLabel, customStyle.iconButtonStyle))
            {
                displayCanvasToolBar = !displayCanvasToolBar;
            }
        }

        private Rect GetCanvasRightUpCornerRect(Vector2 size) => new Rect(canvasRect.xMax - size.x, canvasRect.y, size.x, size.y);

        /// <summary>
        /// 显示画布工具栏
        /// </summary>
        public bool displayCanvasToolBar = true;

        CanvasToolBar _canvasToolBar;

        /// <summary>
        /// 画布工具栏
        /// </summary>
        public CanvasToolBar canvasToolBar => _canvasToolBar ?? (_canvasToolBar = new CanvasToolBar(this));

        /// <summary>
        /// 自动布局
        /// </summary>
        [CanvasToolBar(nameof(AutoLayout))]
        [Name("自动布局")]
        [XCSJ.Attributes.Icon(EIcon.Layout)]
        public virtual void AutoLayout()
        {
            var normalNodeSize = customStyle.normalNodeSize;
            var cellSize = normalNodeSize * 2;
            if (parentNodeView != null)
            {
                parentNodeView.nodeRect = new Rect(normalNodeSize, parentNodeView.nodeRect.size);
            }
            childrenNodeViews.GridLayout(new Vector2(normalNodeSize.x, normalNodeSize.y + cellSize.y), cellSize, (int)(canvasRect.width / cellSize.x));
        }

        /// <summary>
        /// 自动布局出节点视图列表
        /// </summary>
        [CanvasToolBar(nameof(AutoLayoutOutNodeViews))]
        [Name("自动布局出节点视图列表")]
        [XCSJ.Attributes.Icon(EIcon.List)]
        public void AutoLayoutOutNodeViews()
        {
            if (_nodeSelections.FirstOrDefault() is NodeView nodeView)
            {
                MenuHelper.PopupMenu<EDirection>(direction =>
                {
                    nodeView.AutoLayoutOutNodeViews(direction);
                });
            }
        }

        /// <summary>
        /// 自动布局出节点视图列表验证
        /// </summary>
        [CanvasToolBar(nameof(AutoLayoutOutNodeViews), true)]
        public bool AutoLayoutOutNodeViewsValidate() => _nodeSelections.Any();

        #endregion

    }

    /// <summary>
    /// 画布上下文
    /// </summary>
    public class CanvasContext : ICanvasContext
    {
        /// <summary>
        /// 克隆带输入输出
        /// </summary>
        public bool cloneWithInOut { get; private set; } = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CanvasContext() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cloneWithInOut"></param>
        public CanvasContext(bool cloneWithInOut) => this.cloneWithInOut = cloneWithInOut;

        /// <summary>
        /// 缺省画布上下文
        /// </summary>
        public static CanvasContext Default { get; } = new CanvasContext();
    }

    /// <summary>
    /// 画布视图
    /// </summary>
    /// <typeparam name="T">画布模型</typeparam>
    public class CanvasView<T> : CanvasView where T : class, ICanvasModel
    {
        /// <summary>
        /// 画布模型
        /// </summary>
        public new T canvasModel { get; private set; }

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            if (base.canvasModel is T cm)
            {
                canvasModel = cm;
            }
            else
            {
                Debug.LogErrorFormat("画布模型对象不是有效的[{0}]类型对象！", typeof(T));
            }
        }
    }

    /// <summary>
    /// MB画布视图
    /// </summary>
    [CanvasView(typeof(MB))]
    public class MBCanvasView : CanvasView { }

    /// <summary>
    /// MB画布视图
    /// </summary>
    /// <typeparam name="T">MB画布模型</typeparam>
    public class MBCanvasView<T> : CanvasView<T> where T : MB, ICanvasModel
    {
        /// <summary>
        /// 画布模型
        /// </summary>
        public new T canvasModel { get; private set; }

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            if (base.canvasModel is T cm)
            {
                canvasModel = cm;
            }
            else
            {
                Debug.LogErrorFormat("画布模型对象不是有效的[{0}]类型对象！", typeof(T));
            }
        }
    }
}

