using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// GUI工具关联类型
    /// </summary>
    [LinkType(typeof(GUIUtility))]
    public class GUIUtility_LinkType : GUIUtility_LinkType<GUIUtility_LinkType> { }

    /// <summary>
    /// GUI工具关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GUIUtility_LinkType<T> : LinkType<T> where T : GUIUtility_LinkType<T>
    {
        #region guiIsExiting

        /// <summary>
        /// GUI是退出中 属性信息
        /// </summary>
        public static XPropertyInfo guiIsExiting_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(guiIsExiting), TypeHelper.StaticNotPublicHierarchy);

        /// <summary>
        /// GUI是退出中
        /// </summary>
        public static bool guiIsExiting => (bool)guiIsExiting_PropertyInfo.GetValue(null);

        #endregion

        #region GetDefaultSkin

        /// <summary>
        /// 获取默认皮肤 方法信息
        /// </summary>
        public static XMethodInfo GetDefaultSkin_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetDefaultSkin), TypeHelper.StaticNotPublicHierarchy);

        /// <summary>
        /// 获取默认皮肤
        /// </summary>
        /// <returns></returns>
        public static GUISkin GetDefaultSkin()
        {
            return GetDefaultSkin_MethodInfo.Invoke(null, null) as GUISkin;
        }

        #endregion
    }
}
