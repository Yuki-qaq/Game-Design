using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.Extension.Base.NodeKit;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.NodeKit;

namespace XCSJ.EditorExtension.Base.NodeKit.Canvases
{
    /// <summary>
    /// 节点工具编辑器画布视图
    /// </summary>
    public abstract class NodeKitEditorCanvasView : BaseNodeKitEditorCanvasView, INavigationCmd
    {
        #region 导航

        /// <summary>
        /// 可撤销型导航
        /// </summary>
        protected Navigation navigationUndo
        {
            get
            {
                if (_navigationUndo == null)
                {
                    _navigationUndo = new Navigation(this);
                }
                return _navigationUndo;
            }
        }

        private Navigation _navigationUndo = null;

        /// <summary>
        /// 跳转到
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public object Goto(string path)
        {
            if (navigationDataCache.TryGetValue(path, out var data) && !data.canvasModel.ObjectIsNull())
            {
                var canvasView = nodeKitEditor.GetOrCreateCanvasView(data.canvasModel, this);
                OnSetForegroundCanvasView(canvasView);
                return canvasView;
            }
            return default;
        }

        /// <summary>
        /// 可跳转
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool CanGo(string path)
        {
            return !lockForegroundCanvasView;
        }

        /// <summary>
        /// 锁定前景画布视图
        /// </summary>
        public bool lockForegroundCanvasView = false;

        private GameObject selectedGameObject;

        class NavigationData
        {
            /// <summary>
            /// 画布模型
            /// </summary>
            public ICanvasModel canvasModel { get; set; }

            /// <summary>
            /// 路径
            /// </summary>
            public string path { get; set; }
        }

        Dictionary<string, NavigationData> navigationDataCache = new Dictionary<string, NavigationData>();

        private string GetOrCreatePath(ICanvasModel canvasModel)
        {
            if (canvasModel.ObjectIsNull()) return "";

            foreach (var item in navigationDataCache)
            {
                if (item.Value.canvasModel == canvasModel)
                {
                    return item.Key;
                }
            }

            var path = GuidHelper.GetNewGuid();
            navigationDataCache.Add(path, new NavigationData() { canvasModel = canvasModel, path = path });
            return path;
        }

        #endregion

        /// <summary>
        /// 自定义样式
        /// </summary>
        protected CustomStyle customStyle => NodeKitHelperExtension.customStyle;

        /// <summary>
        /// 前景画布视图
        /// </summary>
        protected CanvasView _foregroundCanvasView;

        /// <summary>
        /// 前景画布视图
        /// </summary>
        public override BaseCanvasView foregroundCanvasView
        {
            get => _foregroundCanvasView;
            set
            {
                if (_foregroundCanvasView == value) return;

                navigationUndo.Push(GetOrCreatePath(_foregroundCanvasView?.canvasModel), GetOrCreatePath(value?.canvasModel));

                OnSetForegroundCanvasView(value);
            }
        }

        /// <summary>
        /// 当设置前景画布视图
        /// </summary>
        /// <param name="baseCanvasView"></param>
        protected virtual void OnSetForegroundCanvasView(BaseCanvasView baseCanvasView)
        {
            _foregroundCanvasView?.CallOnDisable();
            _foregroundCanvasView = baseCanvasView as CanvasView;
            _foregroundCanvasView?.CallOnEnable();
        }

        /// <summary>
        /// 获取或创建前景画布视图
        /// </summary>
        /// <param name="canvasModel"></param>
        /// <returns></returns>
        public override BaseCanvasView GetOrCreateForegroundCanvasView(ICanvasModel canvasModel)
        {
            if (canvasModel.ObjectIsNull()) return default;

            if (nodeKitEditor.canvasCache.TryGetValue(canvasModel, out var canvasView))
            {
                return canvasView;
            }

            List<ICanvasModel> parents = new List<ICanvasModel>();
            parents.Add(canvasModel);
            BaseCanvasView parentCanvasView;
            var parent = canvasModel.parentCanvasModel;
            while (true)
            {
                if (parent.ObjectIsNull())
                {
                    parentCanvasView = this;
                    break;
                }
                if (nodeKitEditor.canvasCache.TryGetValue(parent, out parentCanvasView))
                {
                    break;
                }
                parents.Insert(0, parent);
                parent = parent.parentCanvasModel;
            };
            foreach (var cm in parents)
            {
                parentCanvasView = nodeKitEditor.GetOrCreateCanvasView(cm, parentCanvasView);
            }
            return parentCanvasView;
        }

        /// <summary>
        /// 当选择集变更
        /// </summary>
        protected override void OnSelectionChange()
        {
            base.OnSelectionChange();

            CreateCanvasViewWithSelectedGameObject();
        }

        private void CreateCanvasViewWithSelectedGameObject()
        {
            if (selectedGameObject != Selection.activeGameObject || foregroundCanvasView == null)
            {
                selectedGameObject = Selection.activeGameObject;
                if (selectedGameObject)
                {
                    foregroundCanvasView = nodeKitEditor.GetOrCreateCanvasView(new GameObjectCanvasModel(selectedGameObject), this);
                    _foregroundCanvasView?.TrySetChildNodeViewAsForegroundCanvasView();
                }
            }
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            CreateCanvasViewWithSelectedGameObject();
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            //base.OnGUI(nodeKitEditor);

            canvasRect = new Rect(0, 0, nodeKitEditor.position.width, nodeKitEditor.position.height);
            toolBarRect = new Rect(canvasRect.x, canvasRect.y, canvasRect.width, toolBarHeight);
            stateBarRect = new Rect(canvasRect.x, canvasRect.yMax - stateBarHeight, canvasRect.width, stateBarHeight);
            middleRect = new Rect(canvasRect.x, canvasRect.y + toolBarRect.height, canvasRect.width, canvasRect.height - toolBarRect.height - stateBarRect.height);

            if (displayCategory)
            {
                categoryRect = new Rect(0, middleRect.y, categoryWidth, middleRect.height);
                categorySeparatorRect = new Rect(categoryRect.xMax, middleRect.y, separatorWidth, middleRect.height);
            }
            else
            {
                categoryRect = new Rect(0, middleRect.y, 0, middleRect.height);
                categorySeparatorRect = new Rect(0, middleRect.y, 0, middleRect.height);
            }

            if (displayInspector)
            {
                inspectorRect = new Rect(middleRect.xMax - inspectorWidth, middleRect.y, inspectorWidth, middleRect.height);
                inspectorSeparatorRect = new Rect(inspectorRect.xMin - separatorWidth, middleRect.y, separatorWidth, middleRect.height);
            }
            else
            {
                inspectorRect = new Rect(middleRect.xMax, middleRect.y, 0, middleRect.height);
                inspectorSeparatorRect = new Rect(inspectorRect.xMin, middleRect.y, 0, middleRect.height);
            }

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true), GUILayout.Height(toolBarHeight));
                OnGUIToolBar();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                OnGUIMiddle();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true), GUILayout.Height(stateBarHeight));
                OnGUIStateBar();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        #region 上中下矩形数据

        /// <summary>
        /// 工具栏高度：顶部
        /// </summary>
        public float toolBarHeight { get; private set; } = 24;

        /// <summary>
        /// 状态栏高度：底部
        /// </summary>
        public float stateBarHeight { get; private set; } = 20;

        Vector2 middleLeftValue = new Vector2(CategoryMinWidth, 0);
        /// <summary>
        /// 分类宽度
        /// </summary>
        public float categoryWidth => middleLeftValue.x;

        Vector2 middleRightValue = new Vector2(InspectorMinWidth, 0);
        /// <summary>
        /// 检查器宽度
        /// </summary>
        public float inspectorWidth => middleRightValue.x;

        /// <summary>
        /// 分隔符宽度
        /// </summary>
        public float separatorWidth { get; private set; } = 3;

        /// <summary>
        /// 工具栏矩形
        /// </summary>
        public Rect toolBarRect { get; private set; }

        /// <summary>
        /// 中部矩形:包括分类、前景画布视图和检查器
        /// </summary>
        public Rect middleRect { get; private set; }

        /// <summary>
        /// 分类矩形
        /// </summary>
        public Rect categoryRect { get; private set; }

        const float CategoryMinWidth = 300;

        /// <summary>
        /// 分类分隔符矩形
        /// </summary>
        public Rect categorySeparatorRect { get; private set; }

        /// <summary>
        /// 前景画布视图矩形
        /// </summary>
        public Rect foregroundCanvasViewRect { get; private set; }

        const float ForegroundCanvasViewMinWidth = 100;

        /// <summary>
        /// 检查器分隔符矩形
        /// </summary>
        public Rect inspectorSeparatorRect { get; private set; }

        /// <summary>
        /// 检查器矩形
        /// </summary>
        public Rect inspectorRect { get; private set; }

        const float InspectorMinWidth = 300;

        /// <summary>
        /// 状态栏矩形
        /// </summary>
        public Rect stateBarRect { get; private set; }

        #endregion

        #region 工具栏（头部）

        /// <summary>
        /// 当绘制工具栏：顶部
        /// </summary>
        void OnGUIToolBar()
        {
            // 分类
            if (GUILayout.Button(customStyle.categoryButtonContent, customStyle.statebarBtn, UICommonOption.Width32))
            {
                displayCategory = !displayCategory;
                DisplayFlag_To_MiddleAreaDisplayState();
            }
            // 后退
            EditorGUI.BeginDisabledGroup(!navigationUndo.CanUndo());
            if (GUILayout.Button(customStyle.backContent, customStyle.statebarBtn, UICommonOption.Width32))
            {
                navigationUndo.Undo();
            }
            EditorGUI.EndDisabledGroup();

            // 前进
            EditorGUI.BeginDisabledGroup(!navigationUndo.CanDo());
            if (GUILayout.Button(customStyle.forwardContent, customStyle.statebarBtn, UICommonOption.Width32))
            {
                navigationUndo.Do();
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(UICommonOption.Reset, customStyle.statebarBtn, UICommonOption.Width32))
            {
                foreach (var item in nodeKitEditor.canvasCache)
                {
                    if (item.Value != foregroundCanvasView)
                    {
                        item.Value.Refresh(false);
                    }
                }
                foregroundCanvasView?.Refresh(true);
            }

            _foregroundCanvasView?.OnGUIToolBar();
            GUILayout.FlexibleSpace();

            // 缩放
            DrawZoomValue();

            // 检查器
            if (GUILayout.Button(customStyle.inspectorButtonContent, customStyle.statebarBtn, UICommonOption.Width32))
            {
                displayInspector = !displayInspector;
                DisplayFlag_To_MiddleAreaDisplayState();
            }
        }

        /// <summary>
        /// 绘制缩放值
        /// </summary>
        void DrawZoomValue()
        {
            if (_foregroundCanvasView == null) return;

            var zoomArea = _foregroundCanvasView.zoomArea;
            if (GUILayout.Button(typeof(CustomStyle).TrLabel(nameof(CustomStyle.resetScale), ENameTip.EmptyTextWhenHasImage), customStyle.statebarBtn, UICommonOption.Width32))
            {
                zoomArea.Reset();
                CommonFun.FocusControl();
            }

            float newZoom;
            EditorGUI.BeginChangeCheck();
            {
                newZoom = GUILayout.HorizontalSlider(zoomArea.zoom, zoomArea.range.x, zoomArea.range.y, UICommonOption.Width80);
                newZoom = EditorGUILayout.FloatField(newZoom, UICommonOption.Width32);
            }
            if (EditorGUI.EndChangeCheck())
            {
                zoomArea.zoom = newZoom;
            }
        }

        #endregion

        #region 中部区域（身体）

        /// <summary>
        /// 显示分类
        /// </summary>
        public bool displayCategory = true;

        /// <summary>
        /// 显示检查器
        /// </summary>
        public bool displayInspector = true;

        private Vector2 categoryScrollValue = new Vector2();

        private Vector2 inspectorScrollValue = Vector2.zero;

        /// <summary>
        /// 网格背景
        /// </summary>
        private GirdBackground gridBackground { get; } = new GirdBackground();

        /// <summary>
        /// 当绘制中部
        /// </summary>
        void OnGUIMiddle()
        {
            foregroundCanvasViewRect = new Rect(categorySeparatorRect.xMax, middleRect.y, inspectorSeparatorRect.xMin - categorySeparatorRect.xMax, middleRect.height);

            // 分类
            if (displayCategory)
            {
                categoryScrollValue = EditorGUILayout.BeginScrollView(categoryScrollValue, GUILayout.Width(categoryWidth), GUILayout.ExpandHeight(true));
                if (_foregroundCanvasView == null) OnGUICategoryDefault();
                else _foregroundCanvasView.CallOnGUICategory(categoryWidth);
                EditorGUILayout.EndScrollView();

                middleLeftValue = UICommonFun.ResizeSeparatorLayout(middleLeftValue, true, true, CategoryMinWidth, Mathf.Max(CategoryMinWidth, canvasRect.width - ForegroundCanvasViewMinWidth - (displayInspector ? middleRightValue.x : 0) - 2 * separatorWidth), XGUIStyleLib.Get(EGUIStyle.Separator), GUILayout.Width(separatorWidth), GUILayout.ExpandHeight(true));
            }

            // 前景画布
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (_foregroundCanvasView!=null)
            {
                _foregroundCanvasView.CallOnGUI(foregroundCanvasViewRect);
            }
            else
            {
                gridBackground.Draw(foregroundCanvasViewRect, 1);
            }
            EditorGUILayout.EndVertical();

            // 检查器
            if (displayInspector)
            {
                middleRightValue = UICommonFun.ResizeSeparatorLayout(middleRightValue, true, false, InspectorMinWidth, Mathf.Max(InspectorMinWidth, canvasRect.width - ForegroundCanvasViewMinWidth - (displayCategory ? middleLeftValue.x : 0) - 2 * separatorWidth), XGUIStyleLib.Get(EGUIStyle.Separator), GUILayout.Width(separatorWidth), GUILayout.ExpandHeight(true));

                inspectorScrollValue = EditorGUILayout.BeginScrollView(inspectorScrollValue, GUILayout.Width(inspectorWidth), GUILayout.ExpandHeight(true));
                _foregroundCanvasView?.CallOnGUIInspector(inspectorWidth);
                EditorGUILayout.EndScrollView();
            }
        }

        #endregion

        #region 分类（中部左侧）

        /// <summary>
        /// 当绘制分类默认
        /// </summary>
        internal protected virtual void OnGUICategoryDefault() => GUILayout.FlexibleSpace();

        #endregion

        #region 状态栏（脚）

        private enum EMiddleAreaDisplayState
        {
            /// <summary>
            /// 分类_节点区_检查器
            /// </summary>
            [Name("分类_节点区_检查器", "Category_NodeArea_Inspector")]
            [XCSJ.Attributes.Icon(EIcon.CategoryNodeAreaInspector)]
            Category_NodeArea_Inspector,

            /// <summary>
            /// 分类_节点区
            /// </summary>
            [Name("分类_节点区", "Category_NodeArea")]
            [XCSJ.Attributes.Icon(EIcon.CategoryNodeArea)]
            Category_NodeArea,

            /// <summary>
            /// 节点区
            /// </summary>
            [Name("节点区", "NodeArea")]
            [XCSJ.Attributes.Icon(EIcon.NodeArea)]
            NodeArea,

            /// <summary>
            /// 节点区_检查器
            /// </summary>
            [Name("节点区_检查器", "NodeArea_Inspector")]
            [XCSJ.Attributes.Icon(EIcon.NodeAreaInspector)]
            NodeArea_Inspector,
        }

        /// <summary>
        /// 中部区域显示状态
        /// </summary>
        private EMiddleAreaDisplayState middleAreaDisplayState = EMiddleAreaDisplayState.Category_NodeArea_Inspector;

        /// <summary>
        /// 当绘制状态栏
        /// </summary>
        void OnGUIStateBar()
        {
            if (_foregroundCanvasView != null)
            {
                _foregroundCanvasView.OnGUIStateBar();
            }
            GUILayout.FlexibleSpace();

            var state = UICommonFun.Toolbar(middleAreaDisplayState, ENameTip.EmptyTextWhenHasImage, customStyle.statebarBtn); 
            if (state!= middleAreaDisplayState)
            {
                middleAreaDisplayState = state;
                MiddleAreaDisplayState_To_DisplayFlag();
            }
        }

        private void MiddleAreaDisplayState_To_DisplayFlag()
        {
            switch (middleAreaDisplayState)
            {
                case EMiddleAreaDisplayState.Category_NodeArea:
                    {
                        displayCategory = true;
                        displayInspector = false;
                        break;
                    }
                case EMiddleAreaDisplayState.Category_NodeArea_Inspector:
                    {
                        displayCategory = true;
                        displayInspector = true;
                        break;
                    }
                case EMiddleAreaDisplayState.NodeArea:
                    {
                        displayCategory = false;
                        displayInspector = false;
                        break;
                    }
                case EMiddleAreaDisplayState.NodeArea_Inspector:
                    {
                        displayCategory = false;
                        displayInspector = true;
                        break;
                    }
            }
        }

        private void DisplayFlag_To_MiddleAreaDisplayState()
        {
            if (displayCategory)
            {
                if (displayInspector)
                {
                    middleAreaDisplayState = EMiddleAreaDisplayState.Category_NodeArea_Inspector;
                }
                else
                {
                    middleAreaDisplayState = EMiddleAreaDisplayState.Category_NodeArea;
                }
            }
            else 
            {
                if (displayInspector)
                {
                    middleAreaDisplayState = EMiddleAreaDisplayState.NodeArea_Inspector;
                }
                else
                {
                    middleAreaDisplayState = EMiddleAreaDisplayState.NodeArea;
                }
            }
        }

        #endregion

    }
}
