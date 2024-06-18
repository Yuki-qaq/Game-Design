using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 包工具
    /// </summary>
    [LinkType("UnityEditor.PackageUtility")]
    public class PackageUtility : LinkType<PackageUtility>
    {
        #region BuildExportPackageItemsList

        /// <summary>
        /// 编译导出包项列表 方法信息
        /// </summary>
        public static XMethodInfo BuildExportPackageItemsList_MethodInfo { get; } = new XMethodInfo(Type, nameof(BuildExportPackageItemsList), TypeHelper.DefaultLookup);

        /// <summary>
        /// 编译导出包项列表
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="dependencies"></param>
        /// <returns></returns>
        public static ExportPackageItem[] BuildExportPackageItemsList(string[] guids, bool dependencies)
        {
            var source = BuildExportPackageItemsList_MethodInfo.Invoke(null, new object[] { guids, dependencies }) as IEnumerable;
            return source?.Cast(s => new ExportPackageItem(s)).ToArray();
        }

        #endregion

        #region ExportPackage

        /// <summary>
        /// 导出包 方法信息
        /// </summary>
        public static XMethodInfo ExportPackage_MethodInfo { get; } = new XMethodInfo(Type, nameof(ExportPackage), TypeHelper.DefaultLookup);

        /// <summary>
        /// 导出包
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="fileName"></param>
        public static void ExportPackage(string[] guids, string fileName)
        {
            ExportPackage_MethodInfo.Invoke(null, new object[] { guids, fileName });
        }

        #endregion
    }
}
