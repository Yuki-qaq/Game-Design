using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.EditorSMS.Input;
using XCSJ.EditorSMS.NodeKit.Nodes;
using XCSJ.EditorSMS.StateLibrary;
using XCSJ.EditorSMS.Utils;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Menus;
using XCSJ.PluginCommonUtils.NodeKit;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using static XCSJ.EditorSMS.StateLibrary.StateLib;
using EDirection = XCSJ.EditorExtension.Base.NodeKit.EDirection;

namespace XCSJ.EditorSMS.NodeKit.Canvases
{
    /// <summary>
    /// 子状态机画布视图
    /// </summary>
    [CanvasView(typeof(SubStateMachine))]
    public class SubStateMachineCanvasView : CanvasView<SubStateMachine>, ICategoryViewHeaderInfo
    {
        #region 数据模型

        /// <summary>
        /// 状态节点视图集合
        /// </summary>
        protected List<StateNodeView> stateNodeViews = new List<StateNodeView>();

        /// <summary>
        /// 跳转节点视图集合
        /// </summary>
        protected List<TransitionNodeView> transitionNodeViews = new List<TransitionNodeView>();

        /// <summary>
        /// 状态组节点视图集合
        /// </summary>
        protected List<StateGroupNodeView> stateGroupNodeViews = new List<StateGroupNodeView>();

        /// <summary>
        /// 跳转布局器
        /// </summary>
        private TransitionLayouter transitionLayouter = new TransitionLayouter();

        /// <summary>
        /// 创建选择集连接
        /// </summary>
        /// <param name="connectionNodeModel"></param>
        /// <param name="isOut"></param>
        public override bool CreateSelectionConection(INodeModel connectionNodeModel, bool isOut)
        {
            // 发起连接的是跳转视图，则使用跳转的更改输入输出命令
            if (beginConnectionNodeView is TransitionNodeView)
            {
                if (connectionNodeModel is State state && state)
                {
                    var changeNodes = new List<NodeView>(nodeSelections);
                    foreach (var item in changeNodes)
                    {
                        if (item is TransitionNodeView transitionNodeView)
                        {
                            if (isOut)
                            {
                                transitionNodeView.transition.UpdateOutState(state);
                            }
                            else
                            {
                                transitionNodeView.transition.UpdateInState(state);
                            }
                        }
                        else
                        {
                            if (canvasModel is ICanvasConnect canvasConnect) 
                            {
                                if (isOut)
                                {
                                    canvasConnect.TryConnect(item.nodeModel, connectionNodeModel, null);
                                }
                                else
                                {
                                    canvasConnect.TryConnect(connectionNodeModel, item.nodeModel, null);
                                }
                            }

                        }
                    }
                }
                return true;
            }

            return base.CreateSelectionConection(connectionNodeModel, isOut);
        }

        #endregion

        #region 移动和布局

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="offset"></param>
        protected override void Move(Vector2 offset)
        {
            base.Move(offset);

            Layout();
        }

        /// <summary>
        /// 当鼠标左键拖拽
        /// </summary>
        /// <param name="e"></param>
        /// <param name="offset"></param>
        protected internal override void OnMouseLeftButtonDrag(Event e, Vector2 offset)
        {
            base.OnMouseLeftButtonDrag(e, offset);

            if (!drawDragSelectRect)
            {
                Layout();
            }
        }

        private void Layout()
        {
            transitionLayouter.Update();

            foreach (var nodeView in displayNodeViews)
            {
                nodeView.Relayout();
            }
        }

        #endregion

        #region 画布消息

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            CreateTransitions();
        }

        /// <summary>
        /// 当创建显示节点视图
        /// </summary>
        protected override void OnCreateDisplayNodeView()
        {
            base.OnCreateDisplayNodeView();

            CreateTransitions();

            stateNodeViews.Clear();
            transitionNodeViews.Clear();
            stateGroupNodeViews.Clear();
            foreach (var item in childrenNodeViews)
            {
                if (item is StateGroupNodeView stateGroupNodeView)
                {
                    stateGroupNodeViews.Add(stateGroupNodeView);
                }
                else if (item is StateNodeView stateNode)
                {
                    stateNodeViews.Add(stateNode);
                }
                else if (item is TransitionNodeView transitionNodeView)
                {
                    transitionNodeViews.Add(transitionNodeView);
                }
            }
        }

        private void CreateTransitions()
        {
            if (canvasModel)
            {
                foreach (var t in canvasModel.transitions)
                {
                    if (t)
                    {
                        this.CreateConnection(t.inState, t);
                        this.CreateConnection(t, t.outState);
                    }
                }
                transitionLayouter.Init(canvasModel);
                Layout();
            }
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            StateLib.StateComponentItem.onClick += OnStateLibItemClick;
            StateLib.StateComponentItem.onDragItemEvent += OnDragStateLibEvent;

            State.onStateCreated += OnStateCreate;
            State.onEntry += OnEntry;
            State.onExit += OnExit;
            State.onOutOfBounds += OnOutOfBoundsDelegate;

            Transition.onTransitionCreated += OnTransitionCreate;
            Transition.onWillDeleteTransition += OnWillDeleteTransition;
            Transition.onUpdatedInState += OnUpdatedInState;
            Transition.onUpdatedOutState += OnUpdatedOutState;

            Model.onWillDelete += OnModelWillDelete;
            Model.onDeleted += OnModelDeleted;

        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            StateLib.StateComponentItem.onClick -= OnStateLibItemClick;
            StateLib.StateComponentItem.onDragItemEvent -= OnDragStateLibEvent;

            State.onStateCreated -= OnStateCreate;
            State.onEntry -= OnEntry;
            State.onExit -= OnExit;
            State.onOutOfBounds -= OnOutOfBoundsDelegate;

            Transition.onTransitionCreated -= OnTransitionCreate;
            Transition.onWillDeleteTransition -= OnWillDeleteTransition;
            Transition.onUpdatedInState -= OnUpdatedInState;
            Transition.onUpdatedOutState -= OnUpdatedOutState;

            Model.onWillDelete -= OnModelWillDelete;
            Model.onDeleted -= OnModelDeleted;
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            if (willDeleteModel) return;
            base.OnGUI();
        }

        /// <summary>
        /// 当绘制节点集合:绘制顺序依次为状态组节点、跳转节点、状态节点
        /// </summary>
        protected override void OnGUINodeViews()
        {
            //base.OnGUINodeViews();
            
            foreach (var item in stateGroupNodeViews)
            {
                item.CallOnGUI();
            }
            foreach (var item in transitionNodeViews)
            {
                item.CallOnGUI();
            }
            parentNodeView?.CallOnGUI();
            foreach (var item in stateNodeViews)
            {
                item.CallOnGUI();
            }
        }

        #endregion

        #region 右键弹出菜单

        /// <summary>
        /// 当弹起菜单
        /// </summary>
        /// <param name="menuInfo"></param>
        [LanguageTuple("Create Empty State", "创建空状态")]
        [LanguageTuple("Create Sub State Machine", "创建子状态机")]
        protected override void OnPopupMenu(MenuInfo menuInfo)
        {
            base.OnPopupMenu(menuInfo);

            menuInfo.AddMenuItem("Create Empty State".Tr(this), () => OnCreateState(canvasModel.CreateNormalState()), null, 100);
            menuInfo.AddMenuItem("Create Sub State Machine".Tr(this), () => OnCreateState(canvasModel.CreateSubStateMachine()), null, 101, ESeparatorType.TopDown);

            CreateStateMenu(menuInfo);
        }

        /// <summary>
        /// 当创建状态：更新状态对应节点视图的位置，并将节点视图加入到选择集中
        /// </summary>
        /// <param name="state"></param>
        private void OnCreateState(State state)
        {
            if (!state) return;
            var stateNodeView = stateNodeViews.Find(n => n.nodeModel == (INodeModel)state);
            if (stateNodeView == null) return;
            if (IsMouseInCanvasRect()) stateNodeView.position = zoomMousePosition;
            AddToSelection(stateNodeView, false);
        }

        private class NameMethodInfo
        {
            internal StateComponentMenuAttribute attribute { get; set; }
            internal MethodInfo methodInfo { get; set; }
        }
        private static List<NameMethodInfo> _stateComponentList = null;
        private static List<NameMethodInfo> stateComponentList
        {
            get
            {
                // 反射查找所有带有StateComponentMenuAttribute 属性的静态方法
                if (_stateComponentList == null)
                {
                    _stateComponentList = new List<NameMethodInfo>();
                    var list = new List<MethodHelper.Info<StateComponentMenuAttribute>>(MethodHelper.GetStaticMethodsAndAttributes<StateComponentMenuAttribute>());
                    foreach (var item in list)
                    {
                        foreach (var att in item.attributes)
                        {
                            if (att.Valid()) stateComponentList.Add(new NameMethodInfo() { attribute = att, methodInfo = item.methodInfo });
                        }
                    }
                    stateComponentList.Sort((x, y) =>
                    {
                        if (x.attribute.categoryIndex < y.attribute.categoryIndex) return -1;
                        if (x.attribute.categoryIndex > y.attribute.categoryIndex) return 1;
                        return string.Compare(x.attribute.itemName, y.attribute.itemName);
                    });
                }
                return _stateComponentList;
            }
        }

        private void CreateStateMenu(MenuInfo menuInfo)
        {
            stateComponentList.Foreach(data =>
            {
                if (EditorCommon.IsMethodValid(data.methodInfo, new Type[] { typeof(object) }) && typeof(State).IsAssignableFrom(data.methodInfo.ReturnType))
                {
                    menuInfo.AddMenuItem(data.attribute.itemName, () =>
                    {
                        //CreateOrChangeConnections(NodeSelection.selections.ToList(), CreateState(data.methodInfo));
                        OnCreateState(data.methodInfo?.Invoke(null, new object[] { canvasModel }) as State);
                    });
                }
            });
        }

        #endregion

        #region 数据变更事件

        private void OnStateLibItemClick(StateComponentItem stateComponentItem)
        {
            OnCreateState(stateComponentItem.createStateMethodInfo?.Invoke(null, new object[] { canvasModel }) as State);
        }

        /// <summary>
        /// 状态库拖拽响应函数
        /// </summary>
        /// <param name="dragFlag">开启和停止拖拽</param>
        /// <param name="methodInfo">创建状态的静态方法</param>
        private void OnDragStateLibEvent(bool dragFlag, MethodInfo methodInfo)
        {
            if (!StateLibOption.weakInstance.enableDrag) return;

            ////Log.Debug("OnDragStateLibEvent :" + dragFlag + ",dragStateLib:" + dragStateLib);
            //if (selectedCanvas == null || selectedCanvas is RootStateMachineNodeCanvas) return;

            //if (dragFlag)
            //{
            //    if (dragStateLib == EDragStateLib.None)
            //    {
            //        dragStateLib = EDragStateLib.Entry;
            //    }
            //}
            //else
            //{
            //    if (dragStateLib == EDragStateLib.Entry || dragStateLib == EDragStateLib.Draging)
            //    {
            //        dragStateLib = EDragStateLib.Exit;
            //    }
            //}

            //if (methodInfo != null)
            //{
            //    createStateMethodInfo = methodInfo;
            //    //dragImage = EditorIconHelper.GetIconInLib(createStateMethodInfo);
            //}
        }

        private void OnStateCreate(State state)
        {
            if (canvasModel == state || canvasModel.Contains(state))
            {
                Refresh(true);
            }
        }

        /// <summary>
        /// 不在模拟运行下触发
        /// </summary>
        /// <param name="state"></param>
        /// <param name="stateData"></param>
        private void OnEntry(State state, StateData stateData)
        {
            if (state.parent == null) return;

            switch (state.parent.workMode)
            {
                case EWorkMode.Default:
                    {
                        switch (state.workMode)
                        {
                            case EWorkMode.Default:
                                //AutoLinkStateNode(state);
                                break;
                            case EWorkMode.Simulate:
                                break;
                            case EWorkMode.Play:
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case EWorkMode.Simulate:
                    break;
                case EWorkMode.Play:
                    break;
                default:
                    break;
            }
        }

        private void OnExit(State state, StateData stateData)
        {
            if (state.parent == null) return;

            switch (state.parent.workMode)
            {
                case EWorkMode.Default:
                    {
                        switch (state.workMode)
                        {
                            case EWorkMode.Default:
                                //ReleaseLinkStateNode(state);
                                break;
                            case EWorkMode.Simulate:
                                break;
                            case EWorkMode.Play:
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case EWorkMode.Simulate:
                    break;
                case EWorkMode.Play:
                    break;
                default:
                    break;
            }
        }

        private void OnOutOfBoundsDelegate(State state, IWorkClipPlayer player, EOutOfBoundsMode outOfBoundsMode, double percent, StateData stateData, double lastPercent, IStateWorkClip stateWorkClip)
        {
            switch (outOfBoundsMode)
            {
                case EOutOfBoundsMode.None:
                    break;
                case EOutOfBoundsMode.Left:
                case EOutOfBoundsMode.Right:
                    {
                        //ReleaseLinkStateNode(state);
                    }
                    break;
                case EOutOfBoundsMode.LeftToIn:
                case EOutOfBoundsMode.RightToIn:
                    {
                        //AutoLinkStateNode(state);
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnTransitionCreate(Transition transition)
        {
            if (canvasModel.Contains(transition))
            {
                Refresh(true);
            }
        }

        private void OnWillDeleteTransition(Transition transition, bool deleteObject)
        {
            if (canvasModel.Contains(transition))
            {
                Refresh(true);
            }
            //if (deleteObject) return;
            //UndoHelper.RegisterObjectReferenceInScene(transition);
        }

        private void OnUpdatedInState(Transition transition, State state) 
        {
            if (canvasModel.Contains(transition))
            {
                Refresh(true);
            }
        }

        private void OnUpdatedOutState(Transition transition, State state)
        {
            if (canvasModel.Contains(transition))
            {
                Refresh(true);
            }
        }

        private Model willDeleteModel;

        private void OnModelWillDelete(Model model, bool deleteObject)
        {
            if (canvasModel == model || canvasModel.Contains(model as State) || canvasModel.Contains(model as Transition) || canvasModel.groups.IndexOf(model as StateGroup) >= 0)
            {
                willDeleteModel = model;
            }
            //if (deleteObject) return;

            //if (model && !(model is Transition))
            //{
            //    UndoHelper.RegisterObjectReferenceInScene(model);
            //}
        }

        private void OnModelDeleted(Model model, bool deleteObject)
        {
            if (willDeleteModel == model)
            {
                Refresh(true);
                willDeleteModel = null;
            }
            //if (!deleteObject && model)
            //{
            //    UndoHelper.DestroyModelWithRegisterObjectReferenceInScene(model);
            //}
        }

        #endregion

        #region 画布工具栏


        /// <summary>
        /// 自动布局
        /// </summary>
        [CanvasToolBar(nameof(AutoLayout))]
        public override void AutoLayout()
        {
            //base.AutoLayout();

            //优先布局父级
            var normalNodeSize = customStyle.normalNodeSize;
            var cellSize = normalNodeSize * 2;
            if (parentNodeView != null)
            {
                parentNodeView.nodeRect = new Rect(normalNodeSize, parentNodeView.nodeRect.size);
            }

            var beginPosition = new Vector2(normalNodeSize.x, normalNodeSize.y + cellSize.y);
            HashSet<NodeView> alreayLayoutNodes = new HashSet<NodeView>();

            //布局进入
            var enterState = GetNodeView(canvasModel.entryState);
            enterState.nodeRect = new Rect(beginPosition, enterState.nodeRect.size);
            LayoutOutNodeViews(enterState, alreayLayoutNodes);

            //布局任意
            var anyState = GetNodeView(canvasModel.anyState);
            anyState.nodeRect = new Rect(beginPosition += new Vector2(0, cellSize.y), anyState.nodeRect.size);
            LayoutOutNodeViews(anyState, alreayLayoutNodes);

            //布局游离状态
            stateNodeViews.Where(n => n != parentNodeView && !alreayLayoutNodes.Contains(n)).GridLayout(beginPosition += new Vector2(0, cellSize.y), cellSize, (int)(canvasRect.width / cellSize.x));
        }

        private static void LayoutOutNodeViews(NodeView nodeView, HashSet<NodeView> alreayLayoutNodes)
        {
            alreayLayoutNodes.Add(nodeView);
            var outs = nodeView.outNodeViews.Where(n =>
            {
                if (alreayLayoutNodes.Contains(n)) return false;
                alreayLayoutNodes.Add(n);
                return true;
            }).ToList();
            nodeView.LayoutNodeViews(EDirection.Right, outs);

            foreach (var o in outs)
            {
                LayoutOutNodeViews(o, alreayLayoutNodes);
            }
        }

        /// <summary>
        /// 新建子状态机
        /// </summary>
        /// <returns></returns>
        [CanvasToolBar(nameof(NewSubStateMachine), group = 1, index = 10)]
        [Name("新建子状态机")]
        [XCSJ.Attributes.Icon(EIcon.New)]
        public bool NewSubStateMachine() => canvasModel.CreateSubStateMachine();

        #endregion

        #region 检查器

        EInpectorMode inpectorMode = EInpectorMode.Inspector;

        /// <summary>
        /// 检查器模式
        /// </summary>
        [Name("检查器模式")]
        enum EInpectorMode
        {
            /// <summary>
            /// 检查器
            /// </summary>
            [Name("检查器")]
            [XCSJ.Attributes.Icon(EIcon.Edit)]
            Inspector,

            /// <summary>
            /// 状态库
            /// </summary>
            [Name("状态库")]
            [XCSJ.Attributes.Icon(EIcon.StateLib)]
            StateLib,
        }

        Vector2 inspectorScrollValue = Vector2.zero;

        /// <summary>
        /// 当绘制检查器
        /// </summary>
        protected internal override void OnGUIInspector()
        {
            inpectorMode = UICommonFun.Toolbar(inpectorMode, ENameTip.Image, UICommonOption.Height24);
            EditorGUILayout.Space(2);
            inspectorScrollValue = EditorGUILayout.BeginScrollView(inspectorScrollValue);
            switch (inpectorMode)
            {
                case EInpectorMode.Inspector: base.OnGUIInspector(); break;
                case EInpectorMode.StateLib: stateLibView.DrawWithScrollBar(); break;
            }
            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region 状态库视图

        CategoryView _stateLibView = null;
        CategoryView stateLibView
        {
            get
            {
                if (_stateLibView == null)
                {
                    _stateLibView = new CategoryView();
                    _stateLibView.InitData(StateLib.categoryList, this);
                }
                return _stateLibView;
            }
        }

        private CategoryConfig _categoryConfig = new CategoryConfig(nameof(SubStateMachineCanvasView));

        /// <summary>
        /// 搜索文本
        /// </summary>
        public string searchText { get => _categoryConfig.searchText; set => _categoryConfig.searchText = value; }

        /// <summary>
        /// 布局
        /// </summary>
        public ECategoryLayout categoryLayout { get => _categoryConfig.categoryLayout; set => _categoryConfig.categoryLayout = value; }

        /// <summary>
        /// 仅绘制可点击项
        /// </summary>
        public bool drawCanClickOnly { get => _categoryConfig.drawCanClickOnly; set => _categoryConfig.drawCanClickOnly = value; }

        /// <summary>
        /// 帮助按钮点击
        /// </summary>
        public void OnHelp() => UICommonFun.OpenManual(this);

        #endregion
    }

    #region 跳转布局器

    /// <summary>
    /// 跳转布局器：用于解决相同起点和终点的跳转的分组和布局
    /// </summary>
    public class TransitionLayouter
    {
        private List<Group> groups = new List<Group>();

        /// <summary>
        /// 构建布局组
        /// </summary>
        public void Init(SubStateMachine subStateMachine)
        {
            groups.Clear();
            foreach (var transition in subStateMachine.transitions)
            {
                if (!groups.Exists(g => g.AddTransition(transition)))
                {
                    groups.Add(new Group(transition));
                }
            }
        }

        /// <summary>
        /// 更新布局组
        /// </summary>
        public void Update() => groups.ForEach(g => g.Layout());

        /// <summary>
        /// 跳转节点组
        /// </summary>
        class Group
        {
            /// <summary>
            /// 来
            /// </summary>
            public State from { get; private set; }

            /// <summary>
            /// 去
            /// </summary>
            public State to { get; private set; }

            private List<Transition> fromTo = new List<Transition>();

            private List<Transition> toFrom = new List<Transition>();

            /// <summary>
            /// 空白
            /// </summary>
            private static Vector2 space = new Vector2(10, 10);

            private bool selfConnection => from == to;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="t"></param>
            public Group(Transition t)
            {
                from = t.inState;
                to = t.outState;
                fromTo.Add(t);
            }

            /// <summary>
            /// 添加跳转
            /// </summary>
            /// <param name="transition"></param>
            /// <returns></returns>
            public bool AddTransition(Transition transition)
            {
                if (from == transition.inState && to == transition.outState) return fromTo.AddWithDistinct(transition);
                else if (from == transition.outState && to == transition.inState) return toFrom.AddWithDistinct(transition);
                return false;
            }

            private Rect GetFirstRect() => fromTo.Count > 0 ? fromTo[0].nodeRect : (toFrom.Count > 0 ? toFrom[0].nodeRect : new Rect());

            /// <summary>
            /// 布局
            /// </summary>
            public void Layout()
            {
                // 计算跳转关联节点的方位关系
                var dir = selfConnection ? EDirection.Down : NodeKitHelperExtension.CalculateDirection(from.nodeRect, to.nodeRect, GetFirstRect().size);

                // 划分顺序组
                GetOrderGroup(dir, out List<Transition> firstList, out List<Transition> secondList);

                var firstListCount = firstList.Count;
                var secondListCount = secondList.Count;
                var nodeCount = firstListCount + secondListCount;
                var size = NodeKitHelperExtension.customStyle.transitionNodeSize;

                var fristListIsFromTo = firstList == fromTo;

                // 需要布局
                if (firstListCount > 1 || secondListCount > 1)
                {
                    // 计算偏移量:空白+矩形宽或高
                    Vector2 offset = GetLayoutOffset(dir);
                    Vector2 halfOffset = offset / 2;

                    // 计算初始位置
                    Vector2 centerPosition = GetLayoutCenter(offset, firstListCount, secondListCount, dir, size, fristListIsFromTo) - size / 2;
                    int centerIndex = nodeCount - 1;

                    int i = 0;
                    // 设置第一组位置
                    foreach (var t in firstList)
                    {
                        t.nodeRect = new Rect(centerPosition + (i - centerIndex) * halfOffset, size);
                        i += 2;
                    }

                    // 设置第二组位置
                    foreach (var t in secondList)
                    {
                        t.nodeRect = new Rect(centerPosition + (i - centerIndex) * halfOffset, size);
                        i += 2;
                    }
                }
                else
                {
                    if (firstListCount > 0) firstList[0].nodeRect = CalculateSingle(fristListIsFromTo, dir, size);
                    if (secondListCount > 0) secondList[0].nodeRect = CalculateSingle(!fristListIsFromTo, dir, size);
                }
            }

            private Vector2 GetLayoutCenter(Vector2 offset, int firstCount, int secondCount, EDirection direction, Vector2 layoutedSize, bool fristListIsFromTo)
            {
                if (selfConnection) return from.nodeRect.center + offset * ((firstCount + secondCount) / 2 + 2);
                if (firstCount == 0) return CalculateSingle(!fristListIsFromTo, direction, layoutedSize).center;
                if (secondCount == 0) return CalculateSingle(fristListIsFromTo, direction, layoutedSize).center;
                return (from.nodeRect.LayoutClockwiseSide(direction) + to.nodeRect.LayoutClockwiseSide(direction.Reverse())) / 2;
            }

            private Rect CalculateSingle(bool isFistList, EDirection direction, Vector2 transitionSize)
            {
                if (selfConnection)
                {
                    var rect = from.nodeRect;
                    return new Rect((new Vector2(rect.x + rect.width / 2, rect.yMax + rect.height / 2) - transitionSize / 2), transitionSize);
                }
                return new Rect(isFistList ? GetSlotCenter(from.nodeRect, to.nodeRect, direction) : GetSlotCenter(to.nodeRect, from.nodeRect, direction.Reverse()), transitionSize);
            }

            /// <summary>
            /// 分成两个不同箭头方向的组
            /// </summary>
            /// <param name="dir">方位</param>
            /// <param name="firstGroup">先排组</param>
            /// <param name="nextGroup">后排组</param>
            private void GetOrderGroup(EDirection dir, out List<Transition> firstGroup, out List<Transition> nextGroup)
            {
                switch (dir)
                {
                    // 垂直布局，把从下出的排左边
                    case EDirection.Up:
                    case EDirection.Down:
                        {
                            firstGroup = from.nodeRect.position.y < to.nodeRect.position.y ? toFrom : fromTo;
                            break;
                        }
                    // 水平布局，把从左出的排上头
                    case EDirection.Left:
                    case EDirection.Right:
                        {
                            firstGroup = from.nodeRect.position.x > to.nodeRect.position.x ? toFrom : fromTo;
                            break;
                        }
                    default:
                        {
                            firstGroup = fromTo;
                            break;
                        }
                }
                nextGroup = (firstGroup == fromTo) ? toFrom : fromTo;
            }

            private Vector2 GetLayoutOffset(EDirection dir)
            {
                // 跳转节点或者垂直分布，或者水平分布
                var rect = GetFirstRect();
                if (selfConnection) return new Vector2(0, rect.height + space.y);
                switch (dir)
                {
                    case EDirection.Up:
                    case EDirection.Down: return new Vector2(rect.width + space.x, 0);
                    case EDirection.Left:
                    case EDirection.Right: return new Vector2(0, rect.height + space.y);
                }
                return Vector2.zero;
            }

            private Vector2 GetSlotCenter(Rect fromRect, Rect toRect, EDirection direction)
            {
                var slotSize = NodeKitHelperExtension.customStyle.slotNodeSize;
                return (fromRect.LayoutOutsideRectOut4Of8(slotSize, direction) + toRect.LayoutOutsideRectIn4Of8(slotSize, direction.Reverse())) / 2;
            }
        }
    }

    #endregion
}

