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
    #region 节点视图

    /// <summary>
    /// 节点视图
    /// </summary>
    [LanguageFileOutput]
    public class NodeView : BaseEntityNodeView, ISearchEvent
    {
        #region 基础属性

        /// <summary>
        /// 自定义样式
        /// </summary>
        protected CustomStyle customStyle => NodeKitHelperExtension.customStyle;

        /// <summary>
        /// 画布视图
        /// </summary>
        public CanvasView canvasView { get; private set; }

        /// <summary>
        /// 位置
        /// </summary>
        public Vector2 position
        {
            get => nodeRect.position;
            set
            {
                var rect = nodeRect;
                rect.position = value;
                nodeRect = rect;
            }
        }

        /// <summary>
        /// 索引：当前对象在画布视图的子级节点视图列表中的索引；
        /// </summary>
        public int index => canvasView.childrenNodeViews.IndexOf(this);

        /// <summary>
        /// 层级路径
        /// </summary>
        public string hierarchyPath => (canvasView?.hierarchyPath ?? "") + "/" + displayName;

        /// <summary>
        /// 尝试设置为前景画布视图
        /// </summary>
        /// <param name="canvasView"></param>
        /// <returns></returns>
        public bool TrySetAsForegroundCanvasView(out CanvasView canvasView)
        {
            if (CanSetAsForegroundCanvasView(out canvasView))
            {
                nodeKitEditor.foregroundCanvasView = canvasView;
                return true;
            }
            canvasView = default;
            return false;
        }

        /// <summary>
        /// 可以设置为前景画布视图
        /// </summary>
        /// <param name="canvasView"></param>
        /// <returns></returns>
        public bool CanSetAsForegroundCanvasView(out CanvasView canvasView)
        {
            if (nodeModel is ICanvasModel canvasModel && canvasModel.nodeModels.Any())
            {
                canvasView = nodeKitEditor.GetOrCreateCanvasView(canvasModel, parent) as CanvasView;
                return canvasView != null;
            }
            canvasView = default;
            return false;
        }

        #endregion

        #region 连入

        /// <summary>
        /// 入连接按钮节点视图
        /// </summary>
        public virtual InConnectButtonNodeView inConnectButtonNodeView { get; } = new InConnectButtonNodeView();

        /// <summary>
        /// 可连入
        /// </summary>
        /// <param name="fromNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanConnectFrom(NodeView fromNodeView, ICanvasContext canvasContext)
        {
            if (fromNodeView == null) return false;
            return nodeModel is INodeConnectFrom nodeConnectFrom? nodeConnectFrom.CanConnectFrom(fromNodeView.nodeModel, canvasContext): false;
        }

        /// <summary>
        /// 尝试连入
        /// </summary>
        /// <param name="fromNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <param name="connectFromDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryConnectFrom(NodeView fromNodeView, ICanvasContext canvasContext, out object connectFromDescriptor)
        {
            if (fromNodeView!=null && nodeModel is INodeConnectFrom nodeConnectFrom)
            {
                nodeConnectFrom.TryConnectFrom(fromNodeView.nodeModel, canvasContext, out connectFromDescriptor);
            }
            connectFromDescriptor = default;
            return false;
        }

        /// <summary>
        /// 可删除连入
        /// </summary>
        /// <param name="fromNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanRemoveConnectFrom(NodeView fromNodeView, ICanvasContext canvasContext)
        {
            if (fromNodeView == null) return false;
            return nodeModel is INodeConnectFrom nodeConnectFrom ? nodeConnectFrom.CanRemoveConnectFrom(fromNodeView.nodeModel, canvasContext) : false;
        }

        /// <summary>
        /// 尝试删除连入
        /// </summary>
        /// <param name="fromNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <param name="connectFromDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryRemoveConnectFrom(NodeView fromNodeView, ICanvasContext canvasContext, out object connectFromDescriptor)
        {
            if (fromNodeView != null && nodeModel is INodeConnectFrom nodeConnectFrom)
            {
                nodeConnectFrom.TryRemoveConnectFrom(fromNodeView.nodeModel, canvasContext, out connectFromDescriptor);
            }
            connectFromDescriptor = default;
            return false;
        }

        #endregion

        #region 连出

        /// <summary>
        /// 出连接按钮节点视图
        /// </summary>
        public virtual OutConnectButtonNodeView outConnectButtonNodeView { get; } = new OutConnectButtonNodeView();

        /// <summary>
        /// 可连出
        /// </summary>
        /// <param name="toNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanConnectTo(NodeView toNodeView, ICanvasContext canvasContext)
        {
            if (toNodeView == null) return false;
            return nodeModel is INodeConnectTo nodeConnectTo ? nodeConnectTo.CanConnectTo(toNodeView.nodeModel, canvasContext) : false;
        }

        /// <summary>
        /// 尝试连出
        /// </summary>
        /// <param name="toNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <param name="connectToDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryConnectTo(NodeView toNodeView, ICanvasContext canvasContext, out object connectToDescriptor)
        {
            if (toNodeView != null && CanConnectTo(toNodeView, canvasContext) && nodeModel is INodeConnectTo nodeConnectTo)
            {
                nodeConnectTo.TryConnectTo(toNodeView.nodeModel, canvasContext, out connectToDescriptor);
            }
            connectToDescriptor = default;
            return false;
        }

        /// <summary>
        /// 能删除连出
        /// </summary>
        /// <param name="toNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanRemoveConnectTo(NodeView toNodeView, ICanvasContext canvasContext)
        {
            if (toNodeView == null) return false;
            return nodeModel is INodeConnectTo nodeConnectTo ? nodeConnectTo.CanRemoveConnectTo(toNodeView.nodeModel, canvasContext) : false;
        }

        /// <summary>
        /// 尝试删除连出
        /// </summary>
        /// <param name="toNodeView"></param>
        /// <param name="canvasContext"></param>
        /// <param name="connectToDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryRemoveConnectTo(NodeView toNodeView, ICanvasContext canvasContext, out object connectToDescriptor)
        {
            if (toNodeView != null && CanRemoveConnectTo(toNodeView, canvasContext) && nodeModel is INodeConnectTo nodeConnectTo)
            {
                nodeConnectTo.TryRemoveConnectTo(toNodeView.nodeModel, canvasContext, out connectToDescriptor);
            }
            connectToDescriptor = default;
            return false;
        }

        #endregion

        #region 克隆

        /// <summary>
        /// 可克隆
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="withInOut"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanClone(CanvasView canvasView, bool withInOut, ICanvasContext canvasContext)
        {
            if (canvasView == null) return false;
            return nodeModel is INodeClone nodeClone ? nodeClone.CanClone(canvasView.canvasModel, canvasContext) : false;
        }

        /// <summary>
        /// 尝试克隆
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="withInOut"></param>
        /// <param name="canvasContext"></param>
        /// <param name="cloneDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryClone(CanvasView canvasView, bool withInOut, ICanvasContext canvasContext, out object cloneDescriptor)
        {
            if (canvasView != null && CanClone(canvasView, withInOut, canvasContext) && nodeModel is INodeClone nodeClone)
            {
                nodeClone.TryClone(canvasView.canvasModel, canvasContext, out cloneDescriptor);
            }
            cloneDescriptor = default;
            return false;
        }

        #endregion

        #region 删除

        /// <summary>
        /// 可删除
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanDelete(CanvasView canvasView, ICanvasContext canvasContext)
        {
            if (canvasView == null) return false;
            return nodeModel is INodeDelete nodeDelete ? nodeDelete.CanDelete(canvasView.canvasModel, canvasContext) : false;
        }

        /// <summary>
        /// 尝试删除
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <param name="deleteDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryDelete(CanvasView canvasView, ICanvasContext canvasContext, out object deleteDescriptor)
        {
            if (canvasView != null && CanDelete(canvasView, canvasContext) && nodeModel is INodeDelete nodeDelete)
            {
                nodeDelete.TryDelete(canvasView.canvasModel, canvasContext, out deleteDescriptor);
            }
            deleteDescriptor = default;
            return false;
        }

        #endregion

        #region 复制

        /// <summary>
        /// 可复制
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanCopy(CanvasView canvasView, ICanvasContext canvasContext)
        {
            if (canvasView == null) return false;
            return CanClone(canvasView, false, canvasContext);
        }

        /// <summary>
        /// 尝试复制
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <param name="copyDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryCopy(CanvasView canvasView, ICanvasContext canvasContext, out object copyDescriptor)
        {
            copyDescriptor = default;
            return canvasView != null;
        }

        #endregion

        #region 剪切

        /// <summary>
        /// 可剪切
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanCut(CanvasView canvasView, ICanvasContext canvasContext)
        {
            if (canvasView == null) return false;
            return CanClone(canvasView, false, canvasContext) && CanDelete(canvasView, canvasContext);
        }

        /// <summary>
        /// 尝试剪切
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <param name="cutDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryCut(CanvasView canvasView, ICanvasContext canvasContext, out object cutDescriptor)
        {
            cutDescriptor = default;
            return canvasView != null;
        }

        #endregion

        #region 粘贴

        /// <summary>
        /// 可粘贴
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <returns></returns>
        public virtual bool CanPaste(CanvasView canvasView, ICanvasContext canvasContext)
        {
            if (canvasView == null) return false;
            return nodeModel is INodeClone nodeClone ? nodeClone.CanClone(canvasView.canvasModel, canvasContext) : false;
        }

        /// <summary>
        /// 尝试粘贴
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="canvasContext"></param>
        /// <param name="pasteDescriptor"></param>
        /// <returns></returns>
        public virtual bool TryPaste(CanvasView canvasView, ICanvasContext canvasContext, out object pasteDescriptor)
        {
            if (canvasView == null)
            {
                pasteDescriptor = default;
                return false;
            }
            return TryClone(canvasView, (canvasContext is CanvasContext context)? context.cloneWithInOut:false, canvasContext, out pasteDescriptor);
        }

        #endregion

        #region 初始化启用禁用

        /// <summary>
        /// 包含：判断点是否在节点矩形内
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector2 point) => nodeRect.Contains(point);

        /// <summary>
        /// 可拾取
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual bool CanPick(Vector2 point) => Contains(point);

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            canvasView = parent as CanvasView;
            OnInitSize();
            foreach (var item in virtualNodeViews)
            {
                item?.Init(this);
            }
            Relayout();
        }

        /// <summary>
        /// 当初始化尺寸
        /// </summary>
        protected virtual void OnInitSize()
        {
            if (nodeRect.size == Vector2.zero)
            {
                nodeRect = new Rect((parent.canvasRect.size - customStyle.normalNodeSize) / 2, customStyle.normalNodeSize);
            }
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="offset"></param>
        public virtual void Move(Vector2 offset)
        {
            position += offset;
            Relayout();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (var item in virtualNodeViews)
            {
                if (item is ButtonNodeView buttonNodeView)
                {
                    buttonNodeView.onClick += OnButtonNodeViewClick;
                }
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var item in virtualNodeViews)
            {
                if (item is ButtonNodeView buttonNodeView)
                {
                    buttonNodeView.onClick -= OnButtonNodeViewClick;
                }
            }
        }

        /// <summary>
        /// 调用启用
        /// </summary>
        internal void CallOnEnable() => OnEnable();

        /// <summary>
        /// 调用禁用
        /// </summary>
        internal void CallOnDisable() => OnDisable();

        #endregion

        #region 悬停

        /// <summary>
        /// 在悬停区域内
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual bool InHoverArea(Vector2 point) => GetHoverRect().Any(rect => rect.Contains(point));

        /// <summary>
        /// 是悬停
        /// </summary>
        public bool isHover { get; protected set; } = false;

        /// <summary>
        /// 获取悬停区
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Rect> GetHoverRect()
        {
            yield return nodeRect;
            if (inConnectButtonNodeView!=null) yield return inConnectButtonNodeView.nodeRect;
            if (outConnectButtonNodeView != null) yield return outConnectButtonNodeView.nodeRect;
        }

        /// <summary>
        /// 当悬停进入
        /// </summary>
        internal protected void OnHoverEnter() => isHover = true;

        /// <summary>
        /// 当悬停退出
        /// </summary>
        internal protected void OnHoverExit() => isHover = false;

        #endregion

        #region 标题内容和图标

        /// <summary>
        /// 名称提示
        /// </summary>
        public virtual string tip => "";

        /// <summary>
        /// 标题风格
        /// </summary>
        protected virtual GUIStyle titleStyle => customStyle.titleStyleWithoutBackground;

        /// <summary>
        /// 标题高度
        /// </summary>
        protected virtual float titleHeight => customStyle.nodeTitleHeight;

        /// <summary>
        /// 标题矩形
        /// </summary>
        public Rect titleRect => new Rect(nodeRect.position.x, nodeRect.position.y, nodeRect.width, titleHeight);

        /// <summary>
        /// 内容矩形
        /// </summary>
        public Rect contentRect => new Rect(nodeRect.position.x, nodeRect.position.y + titleHeight, nodeRect.width, nodeRect.height - titleHeight);

        /// <summary>
        /// 内容风格
        /// </summary>
        protected virtual GUIStyle contentStyle => customStyle.contentStyle;

        /// <summary>
        /// 绘制标题
        /// </summary>
        protected virtual void DrawTitle()
        {
            var nameContent = CommonFun.TempContent(displayName, tip);
            GUI.Label(nameContent.CalculateMatchGUIContentRect(titleRect, titleStyle), nameContent, titleStyle);
        }

        /// <summary>
        /// 绘制内容
        /// </summary>
        protected virtual void DrawContent()
        {
            DrawContentBackground();
        }

        /// <summary>
        /// 绘制内容背景
        /// </summary>
        protected virtual void DrawContentBackground()
        {
            if (contentStyle != null)
            {
                var contentRect = nodeRect;
                contentRect.position += new Vector2(0, titleHeight - 2);
                contentRect.size -= new Vector2(0, titleHeight - 2);

                contentStyle.DrawStyle(contentRect);
            }
        }

        #endregion

        #region 绘制

        /// <summary>
        /// 节点背景风格
        /// </summary>
        protected virtual GUIStyle backgroundStyle => customStyle.titleContentNodeBackgroundStyle;

        /// <summary>
        /// 调用当绘制GUI
        /// </summary>
        internal void CallOnGUI() => OnGUI();

        /// <summary>
        /// 绘制轮廓线
        /// </summary>
        protected virtual bool DrawOutline()
        {
            // 选中
            if (selected)
            {
                NodeKitHelperExtension.OnGUIColor(customStyle.selectedColor, () => GUI.Box(nodeRect, GUIContent.none, customStyle.selectionBorderStyle));
                return true;
            }

            // 无效内容
            if (!nodeModel.isValidContent)
            {
                NodeKitHelperExtension.OnGUIColor(customStyle.contentInvalidColor, () => GUI.Box(nodeRect, GUIContent.none, customStyle.selectionBorderStyle));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 绘制背景：节点背景
        /// </summary>
        protected virtual void DrawBackground() => backgroundStyle.DrawStyle(nodeRect);

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            DrawOutline();
            OnGUIVirtualNodeView();
            DrawBackground();
            DrawTitle();
            DrawContent();
        }

        #endregion

        #region 鼠标左键

        /// <summary>
        /// 当鼠标左键按下
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseLeftButtonDown(Event e) { }

        /// <summary>
        /// 当鼠标左键拖拽
        /// </summary>
        /// <param name="e"></param>
        /// <param name="offset"></param>
        internal protected virtual void OnMouseLeftButtonDrag(Event e, Vector2 offset) => Move(offset);

        /// <summary>
        /// 当鼠标左键弹起
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseLeftButtonUp(Event e) { }

        /// <summary>
        /// 当鼠标左键双击
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseLeftButtonDoubleClick(Event e)
        {
            if (selected)
            {
                TrySetAsForegroundCanvasView(out _);
            }
        }

        #endregion

        #region 鼠标右键

        /// <summary>
        /// 当鼠标右键按下
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseRightButtonDown(Event e) { }

        /// <summary>
        /// 当鼠标右键弹起
        /// </summary>
        /// <param name="e"></param>
        internal protected virtual void OnMouseRightButtonUp(Event e) { }

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
        [LanguageTuple("Copy", "复制")]
        [LanguageTuple("Cut", "剪切")]
        [LanguageTuple("Clone", "克隆")]
        [LanguageTuple("Delete", "删除")]
        protected virtual void OnPopupMenu(MenuInfo menuInfo)
        {
            if (CanCopy(canvasView, CanvasContext.Default))
            {
                menuInfo.AddMenuItem("Copy".Tr(this), () => canvasView.CopySelection(), null, 350, ESeparatorType.TopUp);
            }
            if (CanCut(canvasView, CanvasContext.Default))
            {
                menuInfo.AddMenuItem("Cut".Tr(this), () => canvasView.CutSelection(), null, 351);
            }

            if (CanClone(canvasView, false, CanvasContext.Default))
            {
                menuInfo.AddMenuItem("Clone".Tr(this), () => canvasView.CloneSelection(false), null, 400, ESeparatorType.TopUp);
            }

            if (CanDelete(canvasView, CanvasContext.Default))
            {
                menuInfo.AddMenuItem("Delete".Tr(this), () => canvasView.DeleteSelection(), null, 410);
            }
        }

        #endregion

        #region 选择

        /// <summary>
        /// 已选择
        /// </summary>
        public bool selected { get; private set; }

        /// <summary>
        /// 当已选择
        /// </summary>
        internal protected virtual void OnSelected()
        {
            selected = true;
        }

        /// <summary>
        /// 当取消选择
        /// </summary>
        internal protected virtual void OnUnselect()
        {
            selected = false;
        }

        #endregion

        #region 虚体节点

        /// <summary>
        /// 虚体节点视图集合
        /// </summary>
        public virtual IEnumerable<VirtualNodeView> virtualNodeViews
        {
            get
            {
                if (nodeTypeNodeView != null) yield return nodeTypeNodeView;
                if (inConnectButtonNodeView != null) yield return inConnectButtonNodeView;
                if (outConnectButtonNodeView != null) yield return outConnectButtonNodeView;
                foreach (var item in slotNodeViews) yield return item;
            }
        }

        /// <summary>
        /// 节点类型节点视图
        /// </summary>
        public virtual NodeTypeNodeView nodeTypeNodeView { get; } = null;

        /// <summary>
        /// 重布局
        /// </summary>
        public virtual void Relayout()
        {
            // 更新虚体节点视图布局
            foreach (var item in virtualNodeViews)
            {
                item.OnRelayout();
            }
        }

        /// <summary>
        /// 当绘制虚体节点视图
        /// </summary>
        protected virtual void OnGUIVirtualNodeView()
        {
            foreach (var item in virtualNodeViews)
            {
                item.OnGUI();
            }
        }

        /// <summary>
        /// 插槽节点视图集:当前工作的插槽节点视图集合
        /// </summary>
        protected List<SlotNodeView> slotNodeViews = new List<SlotNodeView>();

        /// <summary>
        /// 计算插槽位置
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public virtual Vector2 CalculateSlotLocalPosition(ESlotType slotType, EDirection direction) => nodeRect.position;

        /// <summary>
        /// 创建出插槽节点视图
        /// </summary>
        /// <returns></returns>
        public SlotNodeView CreateOutSlotNodeView() => CreateSlotNodeView(ESlotType.Out);

        /// <summary>
        /// 创建入插槽节点视图
        /// </summary>
        /// <returns></returns>
        public SlotNodeView CreateInSlotNodeView() => CreateSlotNodeView(ESlotType.In);

        private SlotNodeView CreateSlotNodeView(ESlotType slotType)
        {
            var node = new SlotNodeView(slotType, this);
            OnCreateSlotNodeView(node);
            return node;
        }

        /// <summary>
        /// 当创建插槽节点模型
        /// </summary>
        /// <param name="slotNodeView"></param>
        protected virtual void OnCreateSlotNodeView(SlotNodeView slotNodeView) => slotNodeViews.Add(slotNodeView);

        #endregion

        #region 按钮节点视图点击

        /// <summary>
        /// 当按钮节点视图点击
        /// </summary>
        protected virtual void OnButtonNodeViewClick(ButtonNodeView buttonNodeView)
        {
            if (buttonNodeView == inConnectButtonNodeView)
            {
                OnInConnectButtonNodeViewClick();
            }
            else if (buttonNodeView == outConnectButtonNodeView)
            {
                OnOutConnectButtonNodeViewClick();
            }
        }

        /// <summary>
        /// 当入连接按钮节点视图点击
        /// </summary>
        protected virtual void OnInConnectButtonNodeViewClick() => canvasView.BeginConnection(this, false);

        /// <summary>
        /// 当出连接按钮节点视图
        /// </summary>
        protected virtual void OnOutConnectButtonNodeViewClick() => canvasView.BeginConnection(this);

        #endregion

        #region 分类

        /// <summary>
        /// 当搜索内容已修改
        /// </summary>
        /// <param name="searchContext"></param>
        public virtual void OnSearchContextChanged(ISearchContext searchContext) { }

        #endregion

        #region 检查器

        /// <summary>
        /// 检查器宽度
        /// </summary>
        public float inspectorWidth { get; private set; }

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
            if (nodeModel.DrawNodeModelInspector(inspectorWidth)) return;
            GUILayout.FlexibleSpace();
        }

        #endregion

        #region 小地图

        /// <summary>
        /// 小地图颜色：节点视图在小地图中绘制的颜色
        /// </summary>
        protected virtual Color miniMapColor => customStyle.miniMapItemColor;

        /// <summary>
        /// 当绘制小地图：绘制小地图中的节点
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scaleValue"></param>
        public virtual void OnGUIMiniMap(Rect rect, float scaleValue)
        {
            //if (!canDraw) return;

            if (selected)
            {
                CommonFun.DrawColorGUI(customStyle.selectedColor, () => GUI.Box(rect, GUIContent.none, customStyle.selectionBorderStyle));
            }

            CommonFun.DrawColorGUI(miniMapColor, () => GUI.Box(rect, GUIContent.none, customStyle.miniMapStyle));
        }

        #endregion

        #region 画布工具栏

        /// <summary>
        /// 出节点视图列表
        /// </summary>
        public virtual IEnumerable<NodeView> outNodeViews { get; } = Empty<NodeView>.Array;

        /// <summary>
        /// 自动布局出节点视图列表
        /// </summary>
        /// <param name="direction"></param>
        public virtual void AutoLayoutOutNodeViews(EDirection direction) => this.LayoutNodeViews(direction, outNodeViews);

        #endregion
    }

    #endregion

    #region 节点视图模板

    /// <summary>
    /// 节点视图
    /// </summary>
    /// <typeparam name="T">节点模型</typeparam>
    public class NodeView<T> : NodeView where T : class, INodeModel
    {
        /// <summary>
        /// 节点模型
        /// </summary>
        public new T nodeModel { get; private set; }

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            if (base.nodeModel is T nm)
            {
                nodeModel = nm;
            }
            else
            {
                Debug.LogErrorFormat("节点模型必须为[{0}]类型对象", typeof(T).FullName);
            }
        }
    }

    #endregion
}

