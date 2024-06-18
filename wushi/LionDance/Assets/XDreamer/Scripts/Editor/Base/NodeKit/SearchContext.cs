using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;

namespace XCSJ.EditorExtension.Base.NodeKit
{
    /// <summary>
    /// 搜索事件
    /// </summary>
    public interface ISearchEvent
    {
        /// <summary>
        /// 当搜索内容已修改
        /// </summary>
        /// <param name="searchContext"></param>
        void OnSearchContextChanged(ISearchContext searchContext);
    }

    /// <summary>
    /// 搜索内容
    /// </summary>
    public interface ISearchContext
    {
        /// <summary>
        /// 需要搜索
        /// </summary>
        bool needSearch { get; }

        /// <summary>
        /// 搜索文本
        /// </summary>
        string searchText { get; }

        /// <summary>
        /// 取反
        /// </summary>
        bool invert { get; }
    }

    /// <summary>
    /// 搜索内容
    /// </summary>
    [Serializable]
    public class SearchContext : ISearchContext
    {
        /// <summary>
        /// 需要搜索
        /// </summary>
        public bool needSearch => !string.IsNullOrEmpty(_searchText) || _invert;

        /// <summary>
        /// 搜索文本
        /// </summary>
        public string _searchText = "";

        /// <summary>
        /// 搜索文本
        /// </summary>
        public string searchText { get => _searchText; set => _searchText = value; }

        /// <summary>
        /// 反选
        /// </summary>
        public bool _invert = false;

        /// <summary>
        /// 反选
        /// </summary>
        [Name("反选")]
        [Tip("勾选时显示不符合搜索关键字的节点对象，不勾选时显示符合搜索关键字的节点对象", "When checked, node objects that do not match the search keywords will be displayed; when unchecked, node objects that match the search keywords will be displayed")]
        public bool invert { get => _invert; set => _invert = value; }
    }

    #region 搜索结果显示模式

    /// <summary>
    /// 搜索结果显示模式
    /// </summary>
    public enum ESearchResultDisplayMode
    {
        /// <summary>
        /// 层级
        /// </summary>
        [Name("层级")]
        [XCSJ.Attributes.Icon(EIcon.Layout)]
        Hieraychy,

        /// <summary>
        /// 列表
        /// </summary>
        [Name("列表")]
        [XCSJ.Attributes.Icon(EIcon.List)]
        List,
    }

    #endregion
}
