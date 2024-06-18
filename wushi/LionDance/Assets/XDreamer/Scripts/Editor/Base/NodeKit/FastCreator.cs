using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
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
    /// <summary>
    /// 快创编辑器
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Window)]
    public sealed class FastCreator : NodeKitEditor
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "快创编辑器";

        /// <summary>
        /// 创建和显示
        /// </summary>
        /// <returns></returns>
        public static FastCreator CreateAndShow()
        {
            var editor = CreateWindow<FastCreator>();
            if (editor)
            {
                editor.Show();
                editor.Focus();
            }
            return editor;
        }

        /// <summary>
        /// 打开
        /// </summary>
        //[MenuItem(XDreamerMenu.NamePath + Title, priority = 2)]
        public static void Open() => CreateAndShow();

        private GUIContent content_ThisScript = new GUIContent("编辑[" + Title + "]脚本");
        private GUIContent content_ForegroundCanvasViewScript = new GUIContent("编辑[前景画布视图]脚本");
        private GUIContent content_ForegroundCanvasModelScript = new GUIContent("编辑[前景画布模型]脚本");
        private GUIContent content_SelectNodeViewScript = new GUIContent("编辑[选中节点视图]脚本");
        private GUIContent content_SelectNodeModelScript = new GUIContent("编辑[选中节点模型]脚本");

        /// <summary>
        /// 添加项到菜单：窗口增加点击的菜单项
        /// </summary>
        /// <param name="menu"></param>
        public override void AddItemsToMenu(GenericMenu menu)
        {
            base.AddItemsToMenu(menu);

#if XDREAMER_EDITION_DEVELOPER

            menu.AddItem(content_ThisScript, false, () =>
            {
                EditorHelper.OpenMonoScript(GetType());
            });
            menu.AddItem(content_ForegroundCanvasViewScript, false, () =>
            {
                if (backgroundCanvasView is NodeKitEditorCanvasView canvasView)
                {
                    EditorHelper.OpenMonoScript(canvasView.foregroundCanvasView?.GetType());
                }
            });
            menu.AddItem(content_ForegroundCanvasModelScript, false, () =>
            {
                if (backgroundCanvasView is NodeKitEditorCanvasView canvasView)
                {
                    EditorHelper.OpenMonoScript(canvasView.foregroundCanvasView?.canvasModel?.GetType());
                }
            });
            menu.AddItem(content_SelectNodeViewScript, false, () =>
            {
                if (backgroundCanvasView is NodeKitEditorCanvasView canvasView && canvasView.foregroundCanvasView is CanvasView foregroundCanvasView)
                {
                    EditorHelper.OpenMonoScript(foregroundCanvasView.nodeSelections.FirstOrDefault()?.GetType());
                }
            });
            menu.AddItem(content_SelectNodeModelScript, false, () =>
            {
                if (backgroundCanvasView is NodeKitEditorCanvasView canvasView && canvasView.foregroundCanvasView is CanvasView foregroundCanvasView)
                {
                    EditorHelper.OpenMonoScript(foregroundCanvasView.nodeSelections.FirstOrDefault()?.nodeModel?.GetType());
                }
            });
#endif
        }

        /// <summary>
        /// 最后激活的
        /// </summary>
        public static FastCreator lastActive => FastCreatorCanvasView.lastActive.nodeKitEditor as FastCreator;

        /// <summary>
        /// 快创编辑器列表
        /// </summary>
        public static List<FastCreator> fastCreators { get; } = new List<FastCreator>();

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            if (fastCreators.Count == 0)
            {
                FastCreatorCanvasView.OnFirstOnEnable();
            }
            fastCreators.Add(this);
            base.OnEnable();
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            fastCreators.RemoveAll(i => !i || i == this);
            if (fastCreators.Count == 0)
            {
                FastCreatorCanvasView.OnLastOnDisable();
            }
        }

        /// <summary>
        /// 快创编辑器画布视图
        /// </summary>
        public FastCreatorCanvasView fastCreatorCanvasView => backgroundCanvasView as FastCreatorCanvasView;

        /// <summary>
        /// 缓存的快创编辑器画布视图
        /// </summary>
        public FastCreatorCanvasView _cachedFastCreatorCanvasView = new FastCreatorCanvasView();
    }

    /// <summary>
    /// 快创编辑器画布视图
    /// </summary>
    [CanvasView(typeof(FastCreator))]
    [Serializable]
    public sealed class FastCreatorCanvasView : NodeKitEditorCanvasView
    {
        /// <summary>
        /// 快创编辑器
        /// </summary>
        public FastCreator fastCreator { get; private set; }

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            fastCreator = nodeKitEditor as FastCreator;
            CopyDataFrom(fastCreator._cachedFastCreatorCanvasView);
            fastCreator._cachedFastCreatorCanvasView = this;
            base.OnInit();
            Reload(false);
        }

        /// <summary>
        /// 当设置前景画布视图
        /// </summary>
        /// <param name="baseCanvasView"></param>
        protected override void OnSetForegroundCanvasView(BaseCanvasView baseCanvasView)
        {
            base.OnSetForegroundCanvasView(baseCanvasView);
            CallOnSearchContextChanged();
        }

        /// <summary>
        /// 最后激活的
        /// </summary>
        public static FastCreatorCanvasView lastActive { get; private set; }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (searchContext.needSearch)
            {
                CallOnSearchContextChanged();
            }
        }

        /// <summary>
        /// 当聚焦
        /// </summary>
        protected override void OnFocus()
        {
            lastActive = this;
            base.OnFocus();
            StartFastCreatorShortcut();
        }

        /// <summary>
        /// 当失去焦点
        /// </summary>
        protected override void OnLostFocus()
        {
            base.OnLostFocus();
            StopFastCreatorShortcut();
        }

        /// <summary>
        /// 当首先启用
        /// </summary>
        internal static void OnFirstOnEnable()
        {
            var instance = ShortcutManager.instance;
            activeProfileIdOnEnable = instance.activeProfileId;
            instance.activeProfileChanged += OnActiveProfileChanged;
        }

        /// <summary>
        /// 当最后禁用
        /// </summary>
        internal static void OnLastOnDisable()
        {
            var instance = ShortcutManager.instance;
            instance.activeProfileId = activeProfileIdOnEnable;
            instance.activeProfileChanged += OnActiveProfileChanged;
        }

        /// <summary>
        /// 当编辑模式下激活场景已变更
        /// </summary>
        /// <param name="oldScene"></param>
        /// <param name="newScene"></param>
        protected override void OnActiveSceneChangedInEditMode(Scene oldScene, Scene newScene)
        {
            base.OnActiveSceneChangedInEditMode(oldScene, newScene);
            Reload();
        }

        /// <summary>
        /// 当新场景已创建
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="setup"></param>
        /// <param name="mode"></param>
        protected override void OnNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            base.OnNewSceneCreated(scene, setup, mode);
            Reload();
        }

        /// <summary>
        /// 当场景已关闭
        /// </summary>
        /// <param name="scene"></param>
        protected override void OnSceneClosed(Scene scene)
        {
            base.OnSceneClosed(scene);
            Reload();
        }

        /// <summary>
        /// 当场景已打开
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        protected override void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            base.OnSceneOpened(scene, mode);
            Reload();
        }

        /// <summary>
        /// 当撤销重做已执行
        /// </summary>
        protected override void OnUndoRedoPerformed()
        {
            base.OnUndoRedoPerformed();
            Reload();
        }

        void Reload(bool refreshForegroundCanvasView = true)
        {
            var categories = this.categories;
            foreach (var c in categories)
            {
                c.Value.Reload();
            }
            if (refreshForegroundCanvasView) foregroundCanvasView?.Refresh(true);
        }

        void CopyDataFrom(FastCreatorCanvasView other)
        {
            this.categoryModeScrollValue = other.categoryModeScrollValue;
            this.categoryMode = other.categoryMode;
            this.searchContext = other.searchContext;
        }

        #region 分类

        /// <summary>
        /// 当绘制分类默认
        /// </summary>
        [LanguageTuple("<Invalid Category>", "<无效分类>")]
        [LanguageTuple("Search Result : ", "搜索结果：")]
        internal protected override void OnGUICategoryDefault()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.BeginChangeCheck();
            searchContext.searchText = UICommonFun.SearchTextField(searchContext.searchText, GetType().ToString(), false, UICommonOption.ToolbarSeachTextField, UICommonOption.ToolbarSeachCancelButton);
            searchContext.invert = UICommonFun.ButtonToggle(typeof(SearchContext).TrLabel(nameof(SearchContext.invert)), searchContext.invert, EditorStyles.toolbarButton, GUILayout.Width(34));
            if (GUILayout.Button(EIcon.Focus.TrLabel(ENameTip.EmptyTextWhenHasImage), UICommonOption.WH24x16))
            {
                currentCategory?.FocusTreeNode();
            }
            if (EditorGUI.EndChangeCheck())
            {
                CallOnSearchContextChanged();
            }
            GUILayout.EndHorizontal();

            var i = Category.GetIndex(categoryMode);
            var ni = GUILayout.Toolbar(i, Category.contents, UICommonOption.Height24);
            if (i != ni)
            {
                categoryMode = Category.GetCategoryMode(ni);
                CallOnSearchContextChanged();
            }

            GUILayout.Space(2);
            categoryModeScrollValue = GUILayout.BeginScrollView(categoryModeScrollValue);
            var category = currentCategory;
            if (category != null)
            {
                category.categoryWidth = categoryWidth;
                category.categoryModeScrollValue = categoryModeScrollValue;
                category.OnGUI();
            }
            GUILayout.EndScrollView();

            if (searchContext.needSearch)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.SelectableLabel("Search Result : ".Tr(this) + (category?.searchResult ?? "<Invalid Category>".Tr(this)));

                var searchResultDisplayModeNew = UICommonFun.Toolbar(searchResultDisplayMode, ENameTip.EmptyTextWhenHasImage, UICommonOption.Width32x2, UICommonOption.Height24);
                if (searchResultDisplayModeNew != searchResultDisplayMode)
                {
                    searchResultDisplayMode = searchResultDisplayModeNew;
                    CallOnSearchContextChanged();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 搜索内容
        /// </summary>
        public SearchContext searchContext = new SearchContext();

        /// <summary>
        /// 搜索结果显示模式
        /// </summary>
        public ESearchResultDisplayMode searchResultDisplayMode = ESearchResultDisplayMode.Hieraychy;
                
        Dictionary<string, Category> _categories;

        Dictionary<string, Category> categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = Category.CreateInstance(this);
                }
                return _categories;
            }
        }

        Category currentCategory
        {
            get
            {
                if (categories.TryGetValue(categoryMode, out var category)) return category;

                //使用第一个作为当前
                var defaultCategory = categories.FirstOrDefault();
                categoryMode = defaultCategory.Key ?? "";
                CallOnSearchContextChanged();
                return defaultCategory.Value;
            }
        }

        void CallOnSearchContextChanged()
        {
            _foregroundCanvasView?.OnSearchContextChanged(searchContext);
            currentCategory?.OnSearchContextChanged(searchContext);
        }

        /// <summary>
        /// 分类模式滚动值
        /// </summary>
        public Vector2 categoryModeScrollValue = new Vector2();

        /// <summary>
        /// 分类模式
        /// </summary>
        public string categoryMode = "";

        #endregion

        #region 快捷键

        void StartFastCreatorShortcut()
        {
            if (string.IsNullOrEmpty(activeProfileIdOnFocus))
            {
                var instance = ShortcutManager.instance;
                activeProfileIdOnFocus = instance.activeProfileId;
                if (activeProfileIdOnFocus != ProfileId)
                {
                    if (!instance.GetAvailableProfileIds().Contains(ProfileId))
                    {
                        instance.CreateProfile(ProfileId);
                        instance.activeProfileId = ProfileId;
                        HandleShortcutConflict();
                    }
                    else
                    {
                        instance.activeProfileId = ProfileId;
                    }
                }
            }
        }

        void StopFastCreatorShortcut()
        {
            if (string.IsNullOrEmpty(activeProfileIdOnFocus)) return;
            var instance = ShortcutManager.instance;
            if (activeProfileIdOnFocus != ProfileId)
            {
                instance.activeProfileId = activeProfileIdOnFocus;
            }
            else if (activeProfileIdOnEnable != ProfileId)
            {
                instance.activeProfileId = activeProfileIdOnEnable;
            }
            else
            {
                instance.activeProfileId = "Default";
            }
            activeProfileIdOnFocus = "";
        }

        static void OnActiveProfileChanged(ActiveProfileChangedEventArgs activeProfileChangedEventArgs)
        {
            if (activeProfileChangedEventArgs.currentActiveProfileId != ProfileId)
            {
                activeProfileIdOnEnable = activeProfileChangedEventArgs.currentActiveProfileId;
            }
        }

        /// <summary>
        /// 处理快捷键冲突
        /// </summary>
        private static void HandleShortcutConflict()
        {
            var instance = ShortcutManager.instance;
            if (instance.activeProfileId != ProfileId) return;

            var infos = MethodHelper.GetStaticMethodsAndAttributes<FastCreatorShortcutAttribute>().ToList(i => instance.GetShortcutBinding(i.attribute.shortcutId));
            foreach (var id in instance.GetAvailableShortcutIds())
            {
                if (!id.StartsWith(ShortcutIdPrefix))
                {
                    var bind = instance.GetShortcutBinding(id);
                    if (infos.Any(i => i.Equals(bind)))
                    {
                        instance.RebindShortcut(id, ShortcutBinding.empty);
                    }
                }
            }
        }

        static string activeProfileIdOnEnable = "";

        string activeProfileIdOnFocus = "";

        const string ProfileId = nameof(XDreamer) + "-" + nameof(FastCreator);

        /// <summary>
        /// 快捷键ID前缀
        /// </summary>
        public const string ShortcutIdPrefix = nameof(XDreamer) + "/" + nameof(FastCreator) + "/";

        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="args"></param>
        [FastCreatorShortcut(nameof(Duplicate), KeyCode.D, ShortcutModifiers.Action)]
        public static void Duplicate(ShortcutArguments args)
        {
            if (args.context is FastCreator fastCreator && fastCreator.foregroundCanvasView is CanvasView canvasView)
            {
                canvasView.DuplicateSelection();
            }
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="args"></param>
        [FastCreatorShortcut(nameof(Copy), KeyCode.C, ShortcutModifiers.Action)]
        public static void Copy(ShortcutArguments args)
        {
            if (args.context is FastCreator fastCreator && fastCreator.foregroundCanvasView is CanvasView canvasView)
            {
                canvasView.CopySelection();
            }
        }

        /// <summary>
        /// 剪切
        /// </summary>
        /// <param name="args"></param>
        [FastCreatorShortcut(nameof(Cut), KeyCode.X, ShortcutModifiers.Action)]
        public static void Cut(ShortcutArguments args)
        {
            if (args.context is FastCreator fastCreator && fastCreator.foregroundCanvasView is CanvasView canvasView)
            {
                canvasView.CutSelection();
            }
        }

        /// <summary>
        /// 粘贴
        /// </summary>
        /// <param name="args"></param>
        [FastCreatorShortcut(nameof(Paste), KeyCode.V, ShortcutModifiers.Action)]
        public static void Paste(ShortcutArguments args)
        {
            if (args.context is FastCreator fastCreator && fastCreator.foregroundCanvasView is CanvasView canvasView)
            {
                canvasView.Paste();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="args"></param>
        [FastCreatorShortcut(nameof(Delete), KeyCode.Delete)]
        public static void Delete(ShortcutArguments args)
        {
            if (args.context is FastCreator fastCreator && fastCreator.foregroundCanvasView is CanvasView canvasView)
            {
                canvasView.DeleteSelection();
            }
        }

        #endregion
    }

    /// <summary>
    /// 快创编辑器快捷键特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class FastCreatorShortcutAttribute : ShortcutAttribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// 快捷键ID
        /// </summary>
        public string shortcutId { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultKeyCode"></param>
        /// <param name="defaultShortcutModifiers"></param>
        public FastCreatorShortcutAttribute(string name, KeyCode defaultKeyCode, ShortcutModifiers defaultShortcutModifiers = ShortcutModifiers.None)
           : base(FastCreatorCanvasView.ShortcutIdPrefix + name, typeof(FastCreator), defaultKeyCode, defaultShortcutModifiers)
        {
            this.name = name;
            shortcutId = FastCreatorCanvasView.ShortcutIdPrefix + name;
        }
    }
}
