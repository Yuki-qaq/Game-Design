using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.XAssets.Libs
{
    /// <summary>
    /// 资源显示导航面板
    /// </summary>
    public class AssetNaviPanel : BasePanel
    {
        private Model model;

        #region 显示缓存
        private int searchCount = 0;
        private int totalCount = 0;
        private int pageIndex = 0;
        private int pageIndexTemp = 0;
        private int pageCount = 0;

        void InitInfo()
        {
            var assetGridPanel = AssetLibWindow.instance.assetGridPanel;
            if (assetGridPanel.currentCategory == null) return;

            searchCount = assetGridPanel.searchAssetPanels.Count;
            totalCount = AssetLibWindow.instance.inEdit ? assetGridPanel.currentCategory.itemTotalCount : assetGridPanel.currentCategory.visibleItemTotalCount;

            pageIndex = assetGridPanel.pageIndex;
            pageIndexTemp = pageIndex;
            pageCount = assetGridPanel.pageCount;
        }

        #endregion 显示缓存

        #region BasePanel

        /// <summary>
        /// 当选择的目录已变更
        /// </summary>
        /// <param name="category"></param>
        /// <param name="lastCategory"></param>
        public override void OnSelectedCategoryChanged(Category category, Category lastCategory)
        {
            base.OnSelectedCategoryChanged(category, lastCategory);
            this.model = category;
            InitInfo();
        }

        /// <summary>
        /// 当编辑模型
        /// </summary>
        /// <param name="model"></param>
        public override void OnEditModel(Model model)
        {
            this.model = model;
            InitInfo();
        }

        /// <summary>
        /// 当取消编辑
        /// </summary>
        /// <param name="model"></param>
        public override void OnCancelEdit(Model model)
        {
            this.model = model;
        }

        /// <summary>
        /// 当搜索项
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="searchType"></param>
        public override void OnSearchItems(string searchText, ESearchType searchType)
        {
            base.OnSearchItems(searchText, searchType);
            InitInfo();
        }

        /// <summary>
        /// 当选择页面
        /// </summary>
        /// <param name="index"></param>
        public override void OnSelectPage(int index)
        {
            base.OnSelectPage(index);
            InitInfo();
        }

        /// <summary>
        /// 当编辑模式修改
        /// </summary>
        /// <param name="edit"></param>
        public override void OnEditModeChange(bool edit)
        {
            base.OnEditModeChange(edit);
            InitInfo();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            InitInfo();
        }

        /// <summary>
        /// 渲染
        /// </summary>
        public override void Render()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(AssetLibWindowStyle.NavBarHeight), GUILayout.ExpandWidth(true));
            if (model != null)
            {
                NavigationBar.Draw<object>(model.navigationItems, OnNavSelect);
                GUILayout.FlexibleSpace();
                DrawPage();
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion BasePanel

        private bool isList = false;

        private void DrawPage()
        {
            if (AssetLibWindow.instance.windowState != AssetLibWindow.EWindowState.Loaded) return;

            
            GUILayout.Label(CommonFun.TempContent(string.Format("资源数：{0}/{1}", searchCount, totalCount), string.Format("搜索所得资源数目：{0}, 总资源数目：{1}", searchCount, totalCount)));

            GUILayout.Space(2);

            if (GUILayout.Button(AssetLibWindow.firstGUIContent, EditorStyles.toolbarButton, UICommonOption.WH24x16)) GotoPage(0);

            if (GUILayout.Button(AssetLibWindow.previousGUIContent, EditorStyles.toolbarButton, UICommonOption.WH24x16))  GotoPage(pageIndex - 1);

            GUILayout.Label("第");
            pageIndexTemp = EditorGUILayout.DelayedIntField(pageIndexTemp + 1, EditorStyles.toolbarTextField, UICommonOption.WH24x16) - 1;
            GUILayout.Label(string.Format( "/{0}页", pageCount));
            if (pageIndexTemp != pageIndex)
            {
                Event.current.Use();
                GotoPage(pageIndexTemp);
            }

            if (GUILayout.Button(AssetLibWindow.nextGUIContent, EditorStyles.toolbarButton, UICommonOption.WH24x16)) GotoPage(pageIndex + 1);

            if (GUILayout.Button(AssetLibWindow.lastGUIContent, EditorStyles.toolbarButton, UICommonOption.WH24x16)) GotoPage(pageCount - 1);
            GUILayout.Space(8);
            if (!(isList = !GUILayout.Toggle(!isList, AssetLibWindow.gridGUIContent, EditorStyles.toolbarButton, UICommonOption.WH24x16)))
            {
                AssetLibWindow.instance.CallChangePlaceType(EPlaceType.Grid);
            }

            if (isList = GUILayout.Toggle(isList, AssetLibWindow.listGUIContent, EditorStyles.toolbarButton, UICommonOption.WH24x16))
            {
                AssetLibWindow.instance.CallChangePlaceType(EPlaceType.List);
            }
            GUILayout.Space(0);
        }

        private void GotoPage(int index)
        {
            CommonFun.FocusControl();
            AssetLibWindow.instance.CallSelectPage(index);
        }

        private void OnNavSelect(object item)
        {
            var model = item as Model;
            if (AssetLibWindow.instance.windowState == AssetLibWindow.EWindowState.Loaded)
            {
                if (model is Category) AssetLibWindow.instance.category = model as Category;
            }
            else
            {
                AssetLibWindow.instance.CallEditModel(model);
            }
        }
    }
}
