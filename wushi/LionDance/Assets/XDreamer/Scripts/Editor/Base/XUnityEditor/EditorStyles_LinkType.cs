using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 编辑器样式
    /// </summary>
    [LinkType(typeof(EditorStyles))]
    public class EditorStyles_LinkType : LinkType<EditorStyles_LinkType>
    {
        #region toggleMixed

        /// <summary>
        /// 切换混合 属性信息
        /// </summary>
        public static XPropertyInfo toggleMixed_XPropertyInfo { get; } = GetXPropertyInfo(nameof(toggleMixed));

        /// <summary>
        /// 切换混合
        /// </summary>
        public static GUIStyle toggleMixed
        {
            get
            {
                var s = toggleMixed_XPropertyInfo?.GetValue(null);
                return s as GUIStyle;
            }
        }

        #endregion
    }
}
