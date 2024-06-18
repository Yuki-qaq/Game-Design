using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;
using static UnityEditor.SearchableEditorWindow;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 场景层级窗口关联类型
    /// </summary>
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(SceneHierarchyWindow))]
    public class SceneHierarchyWindow_LinkType : SearchableEditorWindow_LinkType<SceneHierarchyWindow_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public SceneHierarchyWindow_LinkType(object obj) : base(obj) { }

        #region SetSearchFilter

        /// <summary>
        /// 设置搜索筛选器 方法信息
        /// </summary>
        public static XMethodInfo SetSearchFilter_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetSearchFilter), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 设置搜索筛选器
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <param name="searchMode"></param>
        /// <param name="setAll"></param>
        /// <param name="delayed"></param>
        public void SetSearchFilter(string searchFilter, SearchMode searchMode, bool setAll= false, bool delayed = false) => SetSearchFilter_MethodInfo.Invoke(obj, new object[] { searchFilter, (int)searchMode, setAll, delayed });

        #endregion
    }

    /// <summary>
    /// 场景层级窗口扩展
    /// </summary>
    public static class SceneHierarchyWindow_Extension
    {
        /// <summary>
        /// 设置搜索过滤器
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="searchMode"></param>
        /// <param name="setAll"></param>
        /// <param name="delayed"></param>
        public static void SetSearchFilter(string filter, SearchMode searchMode, bool setAll = false, bool delayed = false)
        {
            SearchableEditorWindow[] windows = (SearchableEditorWindow[])Resources.FindObjectsOfTypeAll(typeof(SearchableEditorWindow));

            foreach (SearchableEditorWindow window in windows)
            {
                if (window.GetType().Name == nameof(SceneHierarchyWindow))
                {
                    new SceneHierarchyWindow_LinkType(window).SetSearchFilter(filter, searchMode, setAll, delayed);
                }
            }
        }
    }
}
