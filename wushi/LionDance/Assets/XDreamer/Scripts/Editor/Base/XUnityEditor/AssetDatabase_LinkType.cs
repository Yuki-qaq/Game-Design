using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 资产数据库关联类型
    /// </summary>
    [LinkType(typeof(AssetDatabase))]
    public class AssetDatabase_LinkType : LinkType<AssetDatabase_LinkType>
    {
        #region assetFolderGUID

        /// <summary>
        /// 资产文件夹GUID 属性信息
        /// </summary>
        public static XPropertyInfo assetFolderGUID_XPropertyInfo { get; } = GetXPropertyInfo(nameof(assetFolderGUID));

        /// <summary>
        /// 资产文件夹GUID
        /// </summary>
        public static string assetFolderGUID
        {
            get => assetFolderGUID_XPropertyInfo?.GetValue(null) as string;
        }

        #endregion

        #region CollectAllChildren

        /// <summary>
        /// 搜集所有子级 属性类型
        /// </summary>
        public static XMethodInfo CollectAllChildren_XPropertyInfo { get; } = GetXMethodInfo(nameof(CollectAllChildren));

        /// <summary>
        /// 搜集所有子级
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string[] CollectAllChildren(string guid, string[] collection)
        {
            return CollectAllChildren_XPropertyInfo?.Invoke(null, new object[] { guid, collection }) as string[];
        }

        #endregion
    }
}
