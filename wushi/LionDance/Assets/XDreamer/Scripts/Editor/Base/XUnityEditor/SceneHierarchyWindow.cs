using UnityEditor;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 场景层级窗口
    /// </summary>
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(SceneHierarchyWindow))]
    public class SceneHierarchyWindow: SearchableEditorWindow_LinkType<SceneHierarchyWindow>
    {
        /// <summary>
        /// 构造
        /// </summary>
        protected SceneHierarchyWindow() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public SceneHierarchyWindow(SearchableEditorWindow obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public SceneHierarchyWindow(object obj) : base(obj) { }

        #region s_LastInteractedHierarchy

        /// <summary>
        /// 最后交互层级 字段信息
        /// </summary>
        public static XFieldInfo s_LastInteractedHierarchy_FieldInfo { get; } = new XFieldInfo(Type, nameof(s_LastInteractedHierarchy));

        /// <summary>
        /// 最后交互层级
        /// </summary>
        public static SceneHierarchyWindow s_LastInteractedHierarchy
        {
            get
            {
                return new SceneHierarchyWindow(s_LastInteractedHierarchy_FieldInfo?.GetValue(null));
            }
        }

        #endregion

        #region SetExpandedRecursive

        /// <summary>
        /// 设置递归展开 方法信息
        /// </summary>
        public static XMethodInfo SetExpandedRecursive_MethodInfo { get; } = GetXMethodInfo(nameof(SetExpandedRecursive));

        /// <summary>
        /// 设置递归展开
        /// </summary>
        /// <param name="id"></param>
        /// <param name="expand"></param>
        public void SetExpandedRecursive(int id, bool expand)
        {
            SetExpandedRecursive_MethodInfo.Invoke(obj, new object[] { id, expand });
        }

        #endregion
    }
}
