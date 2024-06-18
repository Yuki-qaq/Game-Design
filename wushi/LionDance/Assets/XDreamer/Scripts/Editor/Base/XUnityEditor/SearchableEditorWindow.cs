using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 可搜索的编辑器窗口关联类型
    /// </summary>
    public interface ISearchableEditorWindow_LinkType : IEditorWindow_LinkType
    {

    }

    /// <summary>
    /// 类<see cref="SearchableEditorWindow"/>关联类型泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SearchableEditorWindow_LinkType<T> : EditorWindow_LinkType<T>, ISearchableEditorWindow_LinkType
        where T: SearchableEditorWindow_LinkType<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        protected SearchableEditorWindow_LinkType() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public SearchableEditorWindow_LinkType(SearchableEditorWindow obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public SearchableEditorWindow_LinkType(object obj) : base(obj) { }
    }

    /// <summary>
    /// 类<see cref="SearchableEditorWindow"/>关联类型
    /// </summary>
    [LinkType(typeof(SearchableEditorWindow))]
    public class SearchableEditorWindow_LinkType : SearchableEditorWindow_LinkType<SearchableEditorWindow_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        protected SearchableEditorWindow_LinkType() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public SearchableEditorWindow_LinkType(SearchableEditorWindow obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public SearchableEditorWindow_LinkType(object obj) : base(obj) { }
    }
}
