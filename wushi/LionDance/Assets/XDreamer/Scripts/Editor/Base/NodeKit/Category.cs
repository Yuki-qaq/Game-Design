using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.EditorSMS.States;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.NodeKit;
using XCSJ.PluginSMS.States;
using static XCSJ.PluginCommonUtils.XDreamer;

namespace XCSJ.EditorExtension.Base.NodeKit
{
    #region 分类

    /// <summary>
    /// 分类
    /// </summary>
    public abstract class Category
    {
        #region 初始化

        static GUIContent[] _contents;

        /// <summary>
        /// 内容
        /// </summary>
        public static GUIContent[] contents
        {
            get
            {
                if (_contents == null)
                {
                    Init();
                }
                return _contents;
            }
        }

        /// <summary>
        /// 类型
        /// </summary>
        static List<Type> _types;

        static List<string> _categoryModes;

        static List<string> categoryModes
        {
            get
            {
                if (_categoryModes == null)
                {
                    Init();
                }
                return _categoryModes;
            }
        }

        /// <summary>
        /// 获取索引
        /// </summary>
        /// <param name="categoryMode"></param>
        /// <returns></returns>
        public static int GetIndex(string categoryMode) => categoryModes.IndexOf(categoryMode);

        /// <summary>
        /// 获取分类模式
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetCategoryMode(int index)
        {
            try
            {
                return categoryModes[index];
            }
            catch
            {
                return "";
            }
        }

        static Dictionary<string, Category> InternalCreateInstance(FastCreatorCanvasView fastCreatorCanvasView)
        {
            Dictionary<string, Category> instance = new Dictionary<string, Category>();
            foreach (var type in _types)
            {
                if (TypeHelper.CreateInstance(type) is Category category)
                {
                    category.fastCreatorCanvasView = fastCreatorCanvasView;
                    instance.Add(category.categoryMode, category);
                }
                else
                {
                    Debug.LogErrorFormat("类型[{0}]无法构建有效的分类对象！", type);
                }
            }
            return instance;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="fastCreatorCanvasView"></param>
        /// <returns></returns>
        public static Dictionary<string, Category> CreateInstance(FastCreatorCanvasView fastCreatorCanvasView)
        {
            Init();
            return InternalCreateInstance(fastCreatorCanvasView);
        }

        static void Init()
        {
            if (_types != null) return;

            _types = new List<Type>();
            _types.AddRange(UnityEditor.TypeCache.GetTypesWithAttribute<CategoryModeAttribute>());
            _types.Sort((x, y) => AttributeCache<CategoryModeAttribute>.Get(x).index - AttributeCache<CategoryModeAttribute>.Get(y).index);

            _contents = _types.Cast(t => CommonFun.NameTip(t, ENameTip.EmptyTextWhenHasImage)).ToArray();
            _categoryModes = InternalCreateInstance(default).Keys.ToList();
        }

        /// <summary>
        /// 绘制工具条
        /// </summary>
        /// <param name="categoryMode"></param>
        /// <returns></returns>
        public static string DrawToolbar(string categoryMode)
        {

            return categoryMode;
        }

        #endregion

        /// <summary>
        /// 获取样式
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public GUIStyle GetGUIStyle(bool selected) => selected ? GUI.skin.button : GUIStyle.none;

        /// <summary>
        /// 快创编辑器画布视图
        /// </summary>
        public FastCreatorCanvasView fastCreatorCanvasView { get; private set; }

        /// <summary>
        /// 节点工具编辑器
        /// </summary>
        public NodeKitEditor nodeKitEditor => fastCreatorCanvasView?.fastCreator;

        /// <summary>
        /// 分类宽度
        /// </summary>
        public float categoryWidth { get; internal set; }

        /// <summary>
        /// 分类模式滚动值
        /// </summary>
        public Vector2 categoryModeScrollValue { get; internal set; }

        string _categoryMode;

        /// <summary>
        /// 分类模式
        /// </summary>
        public string categoryMode => _categoryMode ?? (_categoryMode = GetType().FullName);

        /// <summary>
        /// 重加载
        /// </summary>
        public abstract void Reload();

        /// <summary>
        /// 记录轮廓线矩形
        /// </summary>
        protected bool recordOutlineRect = false;

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        public virtual void OnGUI()
        {
            recordOutlineRect = Event.current.type == EventType.Repaint;
        }

        string _searchResult;

        /// <summary>
        /// 搜索结果
        /// </summary>
        public string searchResult => _searchResult ?? (_searchResult = searches.Count.ToString() + "/" + totalCount);

        /// <summary>
        /// 总数量
        /// </summary>
        protected int totalCount = 0;

        /// <summary>
        /// 搜索集
        /// </summary>
        protected HashSet<ITreeNodeGraph> searches = new HashSet<ITreeNodeGraph>();

        /// <summary>
        /// 显示搜索结果列表
        /// </summary>
        protected bool displaySearchResultList = false;

        /// <summary>
        /// 当搜索内容已修改
        /// </summary>
        /// <param name="searchContext"></param>
        public virtual void OnSearchContextChanged(ISearchContext searchContext)
        {
            _searchResult = null;
            totalCount = 0;
            searches.Clear();
            displaySearchResultList = searchContext.needSearch && fastCreatorCanvasView.searchResultDisplayMode == ESearchResultDisplayMode.List;
        }

        /// <summary>
        /// 处理搜索
        /// </summary>
        /// <param name="treeNode"></param>
        /// <param name="searchContext"></param>
        protected void HandleSearch(ITreeNodeGraph treeNode, ISearchContext searchContext)
        {
            treeNode.visible = treeNode.displayName.SearchMatch(searchContext.searchText);
            if (searchContext.invert) treeNode.visible = !treeNode.visible;
            if (treeNode.visible)
            {
                searches.Add(treeNode);
                if (!displaySearchResultList)
                {
                    var parnet = treeNode.parent;
                    while (parnet != null)
                    {
                        searches.Add(parnet);

                        parnet.visible = true;
                        parnet = parnet.parent;
                    }
                }
            }
        }

        /// <summary>
        /// 聚焦树形节点
        /// </summary>
        public virtual void FocusTreeNode() { }

        /// <summary>
        /// 设置聚焦树形节点
        /// </summary>
        public void SetFocusTreeNode(ITreeNodeGraph treeNode, float y)
        {
            fastCreatorCanvasView.categoryModeScrollValue.y = y;
        }

        /// <summary>
        /// 当树形节点已点击
        /// </summary>
        /// <param name="treeNode"></param>
        public virtual void OnTreeNodeClicked(ITreeNodeGraph treeNode) => treeNode?.OnClick();

        /// <summary>
        /// 设置前景画布
        /// </summary>
        /// <param name="canvasModel"></param>
        public void SetForegroundCanvasView(object canvasModel)
        {
            if (displaySearchResultList)
            {
                UICommonFun.DelayCall(() =>
                {
                    nodeKitEditor.foregroundCanvasView = nodeKitEditor.GetOrCreateForegroundCanvasView(canvasModel);
                });
            }
            else
            {
                nodeKitEditor.foregroundCanvasView = nodeKitEditor.GetOrCreateForegroundCanvasView(canvasModel);
            }
        }
    }

    #endregion

    #region 分类模式特性

    /// <summary>
    /// 分类模式特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CategoryModeAttribute : IndexAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="index"></param>
        public CategoryModeAttribute(int index)
        {
            this.index = index;
        }
    }

    #endregion

    #region 状态机

    /// <summary>
    /// 状态机
    /// </summary>
    [Name("状态机")]
    [XCSJ.Attributes.Icon(EIcon.State)]
    [CategoryMode(0)]
    public class StateMachineCategory : Category
    {
        /// <summary>
        /// 当树形节点已点击
        /// </summary>
        /// <param name="treeNode"></param>
        public override void OnTreeNodeClicked(ITreeNodeGraph treeNode)
        {
            if (treeNode is SMTreeNode sMTreeNode)
            {
                OnSMTreeNodeClicked(sMTreeNode);
                return;
            }
            base.OnTreeNodeClicked(treeNode);
        }

        private void OnSMTreeNodeClicked(SMTreeNode treeNode)
        {
            SetForegroundCanvasView(treeNode.orgData);
        }

        private bool CheckVisibleAndValid(SMTreeNode smTreeNode)
        {
            return smTreeNode.visible = smTreeNode.valid = smTreeNode.data && smTreeNode.data is SubStateMachine;
        }

        private List<SMTreeNodeRoot> sceneNodes = new List<SMTreeNodeRoot>();

        /// <summary>
        /// 重新加载
        /// </summary>
        public override void Reload()
        {
            sceneNodes.Clear();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    var rootNode = new SMTreeNodeRoot(scene);
                    sceneNodes.Add(rootNode);
                    UpdateNodes(rootNode);
                }
            }

        }

        private void UpdateNodes(SMTreeNode smTreeNode)
        {
            CheckVisibleAndValid(smTreeNode);
            bool checkParent = true;
            smTreeNode.childNodes.ForEach(n =>
            {
                UpdateNodes(n);
                // 子对象有一个可视，父对象就设置为可视
                if (checkParent && n.visible)
                {
                    checkParent = false;
                    smTreeNode.visible = true;
                }
            });
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        public override void OnGUI()
        {
            base.OnGUI();
            var option = SMTreeOption.weakInstance;
            var displayKeyNodeIcon = option.displayKeyNodeIcon;
            var keyNodeIconDistinct = SMSEditorOption.weakInstance.keyNodeIconDistinct;

            if (displaySearchResultList)
            {
                foreach (var node in searches)
                {
                    EditorGUILayout.BeginHorizontal(UICommonOption.Height24);
                    DrawTreeNode(node, default, displayKeyNodeIcon, keyNodeIconDistinct);
                    EditorGUILayout.EndHorizontal();
                }
                return;
            }

            TreeView.Draw(sceneNodes.ToArray(), null, (iText, node, i) => "", (node, guiContent) =>
            {
                DrawTreeNode(node, guiContent, displayKeyNodeIcon, keyNodeIconDistinct);
            }, 12, 12);
        }

        Dictionary<ITreeNodeGraph, Rect> outlines = new Dictionary<ITreeNodeGraph, Rect>();

        private void DrawTreeNode(ITreeNodeGraph node, GUIContent guiContent, bool displayKeyNodeIcon, bool keyNodeIconDistinct)
        {
            var treeNode = node as SMTreeNode;

            // 轮廓线
            GUILayout.Label("", GUILayout.Width(2));
            var outlineRect = GUILayoutUtility.GetLastRect();
            outlineRect.position += new Vector2(outlineRect.size.x + 4, 0);
            outlineRect.size = new Vector2(categoryWidth - outlineRect.position.x - 4 + categoryModeScrollValue.x, 16);
            if (recordOutlineRect)
            {
                outlines[node] = outlineRect;
            }
            CommonFun.DrawColorGUI(Color.clear, () => GUI.Box(outlineRect, "", NodeEditorStyle.instance.treeIconNodeSelectionBorder));

            // 图标
            if (GUILayout.Button(SMSEditorStyle.instance.GetSMTreeNodeContent(treeNode.orgData), NodeEditorStyle.instance.iconStyle, GUILayout.Width(16), GUILayout.Height(16)))
            {
                OnSMTreeNodeClicked(treeNode);
            }

            // 文字
            if (GUILayout.Button(treeNode.display, GetGUIStyle(IsForeground(treeNode)), GUILayout.ExpandWidth(true), GUILayout.Height(16)))
            {
                OnSMTreeNodeClicked(treeNode);
            }

            var state = treeNode.state;
            // 显示关键节点图标
            if (displayKeyNodeIcon)
            {
                foreach (var keyNode in StateKeyNodeCache.Get(state, keyNodeIconDistinct))
                {
                    if (GUILayout.Button(keyNode.content, NodeEditorStyle.instance.iconStyle, GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        OnSMTreeNodeClicked(treeNode);
                    }
                }
            }
        }

        /// <summary>
        /// 当搜索内容已变更
        /// </summary>
        /// <param name="searchContext"></param>
        public override void OnSearchContextChanged(ISearchContext searchContext)
        {
            base.OnSearchContextChanged(searchContext);
            var uo = nodeKitEditor.foregroundCanvasView.GetUnityObject();
            selected = default;
            foreach (var sn in sceneNodes)
            {
                sn.Foreach(n =>
                {
                    if (n is SMTreeNode treeNode)
                    {
                        if (treeNode.valid || n is SMTreeNodeRoot)
                        {
                            totalCount++;
                        }
                        OnSearchContextChanged(searchContext, treeNode);
                        treeNode.selected = treeNode.data == uo;
                        if (treeNode.selected)
                        {
                            selected = treeNode;
                        }
                    }
                });
            }
        }

        void OnSearchContextChanged(ISearchContext searchContext, SMTreeNode treeNode)
        {
            if (CheckVisibleAndValid(treeNode))
            {
                HandleSearch(treeNode, searchContext);
            }
        }

        /// <summary>
        /// 是前景
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        public bool IsForeground(SMTreeNode treeNode) => fastCreatorCanvasView.foregroundCanvasView?.nodeModel == treeNode.orgData;

        SMTreeNode selected;

        /// <summary>
        /// 聚焦树形节点
        /// </summary>
        public override void FocusTreeNode()
        {
            base.FocusTreeNode();
            if (selected != null && selected.selected)
            {
                if(outlines.TryGetValue(selected,out var r))
                {
                    SetFocusTreeNode(selected, r.y);
                }
                return;
            }
            foreach (var sn in sceneNodes)
            {
                if (sn.Any(n =>
                {
                    if (n is SMTreeNode searchEvent && searchEvent.selected)
                    {
                        if (outlines.TryGetValue(searchEvent, out var r))
                        {
                            SetFocusTreeNode(searchEvent, r.y);
                        }
                        return true;
                    }
                    return false;
                })) break;
            }
        }
    }

    #endregion

    #region 层级

    /// <summary>
    /// 层级
    /// </summary>
    [Name("层级")]
    [XCSJ.Attributes.Icon(EIcon.Layout)]
    [CategoryMode(10)]
    public class HieraychyCategory : Category
    {
        List<GameObjectTreeNode> gameObjectTreeNodes = new List<GameObjectTreeNode>();
        GameObjectTreeNode[] _gameObjectTreeNodeArray;

        GameObjectTreeNode[] gameObjectTreeNodeArray
        {
            get
            {
                if (_gameObjectTreeNodeArray == null)
                {
                    gameObjectTreeNodes.Clear();
                    var scene = SceneManager.GetActiveScene();
                    foreach (var gameObject in scene.GetRootGameObjects())
                    {
                        gameObjectTreeNodes.Add(new GameObjectTreeNode(gameObject, null, nodeKitEditor, this));
                    }
                    _gameObjectTreeNodeArray = gameObjectTreeNodes.ToArray();
                }
                return _gameObjectTreeNodeArray;
            }
        }

        private bool IsForeground(GameObjectTreeNode treeNode) => nodeKitEditor.IsForegroundCanvasView(treeNode.gameObject);

        /// <summary>
        /// 重新加载
        /// </summary>
        public override void Reload()
        {
            _gameObjectTreeNodeArray = null;
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        public override void OnGUI()
        {
            base.OnGUI();

            if (displaySearchResultList)
            {
                foreach (var node in searches)
                {
                    EditorGUILayout.BeginHorizontal(UICommonOption.Height24);
                    DrawTreeNode(node);
                    EditorGUILayout.EndHorizontal();
                }
                return;
            }

            TreeView.Draw(gameObjectTreeNodeArray, null, (iText, node, i) => "", (node, guiContent) =>
            {
                DrawTreeNode(node);
            }, 12, 12);
        }

        void DrawTreeNode(ITreeNodeGraph node)
        {
            var treeNode = node as GameObjectTreeNode;

            // 轮廓线
            GUILayout.Label("", GUILayout.Width(2));
            var outlineRect = GUILayoutUtility.GetLastRect();
            outlineRect.position += new Vector2(outlineRect.size.x + 4, 0);
            outlineRect.size = new Vector2(categoryWidth - outlineRect.position.x - 4 + categoryModeScrollValue.x, 16);
            if (recordOutlineRect) treeNode.outlineRect = outlineRect;
            CommonFun.DrawColorGUI(Color.clear, () => GUI.Box(outlineRect, "", NodeEditorStyle.instance.treeIconNodeSelectionBorder));

            if (treeNode.enable)
            {
                // 图标
                if (GUILayout.Button(treeNode.iconContent, treeNode.iconStyle, GUILayout.Width(16), GUILayout.Height(16)))
                {
                    OnTreeNodeClicked(treeNode);
                }
            }

            // 文字
            if (GUILayout.Button(treeNode.display, GetGUIStyle(treeNode.selected), GUILayout.ExpandWidth(true), GUILayout.Height(16)))
            {
                OnTreeNodeClicked(treeNode);
            }
        }

        GameObjectTreeNode selected;

        /// <summary>
        /// 当搜索内容已修改
        /// </summary>
        /// <param name="searchContext"></param>
        public override void OnSearchContextChanged(ISearchContext searchContext)
        {
            base.OnSearchContextChanged(searchContext);
            var go = nodeKitEditor.foregroundCanvasView.GetGameObject();
            selected = default;
            foreach (var sn in gameObjectTreeNodeArray)
            {
                sn.Foreach(n =>
                {
                    totalCount++;
                    if (n is GameObjectTreeNode searchEvent)
                    {
                        HandleSearch(searchEvent, searchContext);

                        searchEvent.selected = searchEvent.gameObject == go;
                        if (searchEvent.selected)
                        {
                            selected = searchEvent;
                        }
                        searchEvent.OnSearchContextChanged(searchContext);
                    }
                });
            }
        }

        /// <summary>
        /// 聚焦树形节点
        /// </summary>
        public override void FocusTreeNode()
        {
            base.FocusTreeNode();
            if (selected != null && selected.selected)
            {
                SetFocusTreeNode(selected, selected.outlineRect.y);
                return;
            }
            foreach (var sn in gameObjectTreeNodeArray)
            {
                if (sn.Any(n =>
                {
                    if (n is GameObjectTreeNode gameObjectTreeNode && gameObjectTreeNode.selected)
                    {
                        SetFocusTreeNode(gameObjectTreeNode, gameObjectTreeNode.outlineRect.y);
                        return true;
                    }
                    return false;
                })) break;
            }
        }
    }

    #region 游戏对象树形节点

    class GameObjectTreeNode : ITreeNodeGraph, ISearchEvent
    {
        public GameObjectTreeNode(GameObject gameObject, GameObjectTreeNode parent, NodeKitEditor nodeKitEditor, Category category)
        {
            this.gameObject = gameObject;
            this.parentGameObjectTreeNode = parent;
            this.nodeKitEditor = nodeKitEditor;
            this.category = category;

            foreach (Transform child in gameObject.transform)
            {
                gameObjectTreeNodes.Add(new GameObjectTreeNode(child.gameObject, this, nodeKitEditor, category));
            }
            gameObjectTreeNodeArray = gameObjectTreeNodes.ToArray();
            enable = gameObject.GetComponents<ICanvasModel>().Length > 0;
            if (enable)
            {
                iconContent = SMSEditorStyle.instance.SM;
                iconStyle = NodeEditorStyle.instance.iconStyle;
            }
            visible = enable || this.Any(node => node.enable);
        }
        public GUIContent iconContent;
        public XGUIStyle iconStyle;

        public Rect outlineRect;

        public GameObject gameObject;
        GameObjectTreeNode parentGameObjectTreeNode;
        NodeKitEditor nodeKitEditor;
        public Category category;

        List<GameObjectTreeNode> gameObjectTreeNodes = new List<GameObjectTreeNode>();
        GameObjectTreeNode[] gameObjectTreeNodeArray;

        GUIContent content = new GUIContent();

        public GUIContent display
        {
            get
            {
                content.text = gameObject ? gameObject.name : "";
                return content;
            }
        }

        public bool enable { get; set; }

        public bool visible { get; set; } = true;

        public int depth => parentGameObjectTreeNode == null ? 0 : parentGameObjectTreeNode.depth;

        public bool expanded { get; set; } = true;

        public bool selected { get; set; } = false;

        public ITreeNodeGraph parent => parentGameObjectTreeNode;

        public ITreeNodeGraph[] children => gameObjectTreeNodeArray;

        public string displayName => gameObject ? gameObject.name : "";

        public string name { get => gameObject.name; set => gameObject.name = value; }

        ITreeNode ITreeNode.parent => parentGameObjectTreeNode;

        ITreeNode[] ITreeNode.children => children;

        public void OnClick()
        {
            category?.SetForegroundCanvasView(gameObject);
        }

        public void OnSearchContextChanged(ISearchContext searchContext) { }
    }

    #endregion

    #endregion

    #region 插件

    /// <summary>
    /// 插件
    /// </summary>
    [Name("插件")]
    [XCSJ.Attributes.Icon(EIcon.Plugin)]
    [CategoryMode(20)]
    public class PluginCategory : Category
    {
        List<PluginsTreeNode> pluginsTreeNodes = new List<PluginsTreeNode>();
        PluginsTreeNode[] _pluginsTreeNodeArray;
        PluginsTreeNode[] pluginsTreeNodeArray
        {
            get
            {
                if (_pluginsTreeNodeArray == null)
                {
                    pluginsTreeNodes.Clear();
                    foreach (var mi in XDreamer.GetManagerTypeInfosInAppWithSort())
                    {
                        pluginsTreeNodes.Add(new PluginsTreeNode(mi, nodeKitEditor,this));
                    }
                    foreach (var c in ComponentCache.GetComponents<ICanvasModel>())
                    {
                        ManualHelper.GetManualOwnerClass(c, out var ownerType);
                        pluginsTreeNodes.FirstOrDefault(mi => mi.managerTypeInfo.type == ownerType)?.Add(c);
                    }
                    foreach (var node in pluginsTreeNodes)
                    {
                        node.Sort();
                        node.visible = node.Any(n => n.visible || n is ComponentTreeNode);
                    }
                    _pluginsTreeNodeArray = pluginsTreeNodes.ToArray();
                }
                return _pluginsTreeNodeArray;
            }
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        public override void Reload()
        {
            _pluginsTreeNodeArray = null;
        }

        /// <summary>
        /// 当绘制分类插件
        /// </summary>
        public override void OnGUI()
        {
            base.OnGUI();

            if (displaySearchResultList)
            {
                foreach (var node in searches)
                {
                    EditorGUILayout.BeginHorizontal(UICommonOption.Height24);
                    DrawTreeNode(node);
                    EditorGUILayout.EndHorizontal();
                }
                return;
            }

            TreeView.Draw(pluginsTreeNodeArray, null, (iText, node, i) => "", (node, guiContent) =>
            {
                DrawTreeNode(node);
            }, 12, 12);
        }

        void DrawTreeNode(ITreeNodeGraph node)
        {
            var treeNode = node;

            // 轮廓线
            GUILayout.Label("", GUILayout.Width(2));
            var outlineRect = GUILayoutUtility.GetLastRect();
            outlineRect.position += new Vector2(outlineRect.size.x + 4, 0);
            outlineRect.size = new Vector2(categoryWidth - outlineRect.position.x - 4 + categoryModeScrollValue.x, 16);
            if (recordOutlineRect && node is ComponentTreeNode component) component.outlineRect = outlineRect;
            CommonFun.DrawColorGUI(Color.clear, () => GUI.Box(outlineRect, "", NodeEditorStyle.instance.treeIconNodeSelectionBorder));

            if (treeNode.enable)
            {
                // 图标
                if (GUILayout.Button(SMSEditorStyle.instance.SM, NodeEditorStyle.instance.iconStyle, GUILayout.Width(16), GUILayout.Height(16)))
                {
                    OnTreeNodeClicked(treeNode);
                }
            }

            // 文字
            if (GUILayout.Button(treeNode.display, GetGUIStyle(treeNode.selected), GUILayout.ExpandWidth(true), GUILayout.Height(16)))
            {
                OnTreeNodeClicked(treeNode);
            }
        }

        private bool IsForeground(ITreeNodeGraph treeNode)
        {
            if (treeNode is ComponentTreeNode componentTreeNode) return nodeKitEditor.IsForegroundCanvasView(componentTreeNode.component as ICanvasModel);
            if (treeNode is GameObjectTreeNode gameObjectTreeNode) return IsForeground(gameObjectTreeNode);
            if (treeNode is SMTreeNode sMTreeNode) return IsForeground(sMTreeNode);
            return false;
        }

        ComponentTreeNode selected;

        /// <summary>
        /// 当搜索内容已修改
        /// </summary>
        /// <param name="searchContext"></param>
        public override void OnSearchContextChanged(ISearchContext searchContext)
        {
            base.OnSearchContextChanged(searchContext);
            var c = nodeKitEditor.foregroundCanvasView.GetComponent();
            if (!c)
            {
                var go = nodeKitEditor.foregroundCanvasView.GetGameObject();
                if (go)
                {
                    c = go.GetComponent<ICanvasModel>() as Component;
                }
            }
            selected = default;
            foreach (var sn in pluginsTreeNodeArray)
            {
                sn.Foreach(n =>
                {
                    totalCount++;
                    if (n is ComponentTreeNode component)
                    {
                        component.selected = component.component == c;
                        if (component.selected)
                        {
                            selected = component;
                        }
                        component.OnSearchContextChanged(searchContext);
                    }
                    else if (n is ISearchEvent searchEvent)
                    {
                        searchEvent.OnSearchContextChanged(searchContext);
                    }

                    HandleSearch(n, searchContext);
                });
            }
        }

        /// <summary>
        /// 聚焦树形节点
        /// </summary>
        public override void FocusTreeNode()
        {
            base.FocusTreeNode();
            if (selected != null && selected.selected)
            {
                SetFocusTreeNode(selected, selected.outlineRect.y);
                return;
            }
            foreach (var sn in pluginsTreeNodeArray)
            {
                if (sn.Any(n =>
                {
                    if (n is ComponentTreeNode component && component.selected)
                    {
                        SetFocusTreeNode(component, component.outlineRect.y);
                        return true;
                    }
                    return false;
                })) break;
            }
        }
    }

    class PluginsTreeNode : ITreeNodeGraph, ISearchEvent
    {
        public PluginsTreeNode(ManagerTypeInfo managerTypeInfo, NodeKitEditor nodeKitEditor, Category category)
        {
            this.managerTypeInfo = managerTypeInfo;
            this.nodeKitEditor = nodeKitEditor;
            this.category = category;
        }

        public void Add(ICanvasModel canvasModel)
        {
            var type = canvasModel.GetType();
            componentTypeTreeNodes.FirstOrNew(n => n.type == type, () =>
            {
                var node = new ComponentTypeTreeNode(type, this, nodeKitEditor, category);
                componentTypeTreeNodes.Add(node);
                return node;
            })?.Add(canvasModel);
        }

        public void Sort()
        {
            componentTypeTreeNodes.Sort((x, y) => string.Compare(x.displayName, y.displayName));
            foreach (var node in componentTypeTreeNodes)
            {
                node.Sort();
                node.visible = node.Any(n => n.visible || n is ComponentTreeNode);
            }
        }

        List<ComponentTypeTreeNode> componentTypeTreeNodes = new List<ComponentTypeTreeNode>();
        ComponentTypeTreeNode[] _componentTypeTreeNodeArray;
        ComponentTypeTreeNode[] componentTypeTreeNodeArray => _componentTypeTreeNodeArray ?? (_componentTypeTreeNodeArray = componentTypeTreeNodes.ToArray());

        public ManagerTypeInfo managerTypeInfo;
        public NodeKitEditor nodeKitEditor;
        Category category;

        public GUIContent display => managerTypeInfo.type.TrLabel();

        public bool enable { get; set; } = false;
        public bool visible { get; set; } = false;

        public int depth => parent == null ? 0 : parent.depth + 1;

        public bool expanded { get; set; } = true;
        public bool selected { get; set; }

        public ITreeNodeGraph parent => default;

        public ITreeNodeGraph[] children => componentTypeTreeNodeArray;

        public string displayName => managerTypeInfo.type.Tr();

        public string name { get => displayName; set { } }

        ITreeNode ITreeNode.parent => parent;

        ITreeNode[] ITreeNode.children => children;

        public void OnClick()
        {
            //Debug.Log(displayName);
        }

        public void OnSearchContextChanged(ISearchContext searchContext) { }
    }

    class ComponentTypeTreeNode : ITreeNodeGraph, ISearchEvent
    {
        public ComponentTypeTreeNode(Type type, PluginsTreeNode pluginsTreeNode, NodeKitEditor nodeKitEditor, Category category)
        {
            this.type = type;
            this.pluginsTreeNode = pluginsTreeNode;
            this.nodeKitEditor = nodeKitEditor;
            this.category = category;
        }

        public Type type;
        PluginsTreeNode pluginsTreeNode;
        public NodeKitEditor nodeKitEditor;
        Category category;

        public void Add(ICanvasModel canvasModel)
        {
            if (canvasModel is Component component && component)
            {
                componentTreeNodes.Add(new ComponentTreeNode(component, this, nodeKitEditor, category));
            }
        }

        public void Sort()
        {
            componentTreeNodes.Sort((x, y) => string.Compare(x.displayName, y.displayName));
        }

        public List<ComponentTreeNode> componentTreeNodes = new List<ComponentTreeNode>();
        public ComponentTreeNode[] _componentTreeNodeArray;
        public ComponentTreeNode[] componentTreeNodeArray => _componentTreeNodeArray ?? (_componentTreeNodeArray = componentTreeNodes.ToArray());

        public GUIContent display => type.TrLabel();

        public bool enable { get; set; } = false;
        public bool visible { get; set; } = false;

        public int depth => parent == null ? 0 : parent.depth + 1;

        public bool expanded { get; set; } = true;
        public bool selected { get; set; }

        public ITreeNodeGraph parent => pluginsTreeNode;

        public ITreeNodeGraph[] children => componentTreeNodeArray;

        public string displayName => type.Tr();

        public string name { get => displayName; set { } }

        ITreeNode ITreeNode.parent => parent;

        ITreeNode[] ITreeNode.children => children;

        public void OnClick()
        {
            //Debug.Log(displayName);
        }

        public void OnSearchContextChanged(ISearchContext searchContext) { }
    }

    class ComponentTreeNode : ITreeNodeGraph, ISearchEvent
    {
        public ComponentTreeNode(Component component, ComponentTypeTreeNode componentTypeTreeNode, NodeKitEditor nodeKitEditor, Category category)
        {
            this.component = component;
            this.componentTypeTreeNode = componentTypeTreeNode;
            this.nodeKitEditor = nodeKitEditor;
            this.category = category;
        }

        public Component component;
        ComponentTypeTreeNode componentTypeTreeNode;
        public NodeKitEditor nodeKitEditor;
        Category category;

        public Rect outlineRect;

        GUIContent _display = new GUIContent();

        public GUIContent display
        {
            get
            {
                _display.text = component.name;
                return _display;
            }
        }

        public bool enable { get; set; } = true;
        public bool visible { get; set; } = true;

        public int depth => parent == null ? 0 : parent.depth + 1;

        public bool expanded { get; set; } = true;
        public bool selected { get; set; }

        public ITreeNodeGraph parent => componentTypeTreeNode;

        public ITreeNodeGraph[] children => Empty<ITreeNodeGraph>.Array;

        public string displayName => component.name;

        public string name { get => component.name; set => component.name = value; }

        ITreeNode ITreeNode.parent => parent;

        ITreeNode[] ITreeNode.children => children;

        public void OnClick()
        {
            category.SetForegroundCanvasView(component);
        }

        public void OnSearchContextChanged(ISearchContext searchContext) { }
    }

    #endregion
}
