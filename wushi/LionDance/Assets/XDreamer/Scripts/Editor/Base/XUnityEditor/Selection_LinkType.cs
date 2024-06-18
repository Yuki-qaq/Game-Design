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
    /// 选择集关联类型
    /// </summary>
    [LinkType(typeof(Selection))]
    public class Selection_LinkType : LinkType<Selection_LinkType>
    {
        #region assetGUIDsDeepSelection

        /// <summary>
        /// 资产GUID深度选择集 属性信息
        /// </summary>
        public static XPropertyInfo assetGUIDsDeepSelection_XPropertyInfo { get; } = GetXPropertyInfo(nameof(assetGUIDsDeepSelection));

        /// <summary>
        /// 资产GUID深度选择集
        /// </summary>
        public static string[] assetGUIDsDeepSelection
        {
            get => assetGUIDsDeepSelection_XPropertyInfo?.GetValue(null) as string[];
        }

        #endregion
    }
}
