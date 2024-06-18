namespace XCSJ.EditorExtension.Base.XAssets.Libs
{
    /// <summary>
    /// 基础面板
    /// </summary>
    public class BasePanel
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize(){ }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update(){ }

        /// <summary>
        /// 渲染
        /// </summary>
        public virtual void Render(){ }

        /// <summary>
        /// 当选项已更改
        /// </summary>
        /// <param name="alwo"></param>
        public virtual void OnOptionModified(AssetLibWindowOption alwo) { }

        /// <summary>
        /// 当选择的目录已更改
        /// </summary>
        /// <param name="category"></param>
        /// <param name="lastCategory"></param>
        public virtual void OnSelectedCategoryChanged(Category category, Category lastCategory) { }

        /// <summary>
        /// 当搜索项
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="searchType"></param>
        public virtual void OnSearchItems(string searchText, ESearchType searchType) { }

        /// <summary>
        /// 当编辑模式更改
        /// </summary>
        /// <param name="edit"></param>
        public virtual void OnEditModeChange(bool edit) { }

        /// <summary>
        /// 当编辑模型
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnEditModel(Model model) { }

        /// <summary>
        /// 当可视模型修改
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnVisibleModelChange(Model model) { }

        /// <summary>
        /// 当取消编辑
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnCancelEdit(Model model) { }

        /// <summary>
        /// 当取消编辑目录
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnCancelEditCategory(Model model) { }

        /// <summary>
        /// 当提交编辑
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnSubmitEdit(Model model) { }

        /// <summary>
        /// 当选择页面
        /// </summary>
        /// <param name="index"></param>
        public virtual void OnSelectPage(int index) { }

        /// <summary>
        /// 当修改排版类型
        /// </summary>
        /// <param name="placeType"></param>
        public virtual void OnChangePlaceType(EPlaceType placeType) { }

        /// <summary>
        /// 当选择目录
        /// </summary>
        /// <param name="isNext"></param>
        public virtual void OnSelectCategory(bool isNext) { }

        /// <summary>
        /// 当展开目录
        /// </summary>
        public virtual void OnExpandCategory() { }

        /// <summary>
        /// 当保存全部
        /// </summary>
        public virtual void OnSaveAll() { }
    }
}
