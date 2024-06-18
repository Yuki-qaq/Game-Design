using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 资源商店工具
    /// </summary>
    [LinkType("UnityEditor.AssetStoreUtils")]
    public class AssetStoreUtils : LinkType<AssetStoreUtils>
    {
        #region GetLoaderPath

        /// <summary>
        /// 获取加载器路径 方法信息
        /// </summary>
        public static XMethodInfo GetLoaderPath_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetLoaderPath), TypeHelper.DefaultLookup);

        /// <summary>
        /// 获取加载器路径
        /// </summary>
        /// <returns></returns>
        public static string GetLoaderPath()
        {
            return (string)GetLoaderPath_MethodInfo.Invoke(null, Empty<object>.Array);
        }

        #endregion
    }
}
