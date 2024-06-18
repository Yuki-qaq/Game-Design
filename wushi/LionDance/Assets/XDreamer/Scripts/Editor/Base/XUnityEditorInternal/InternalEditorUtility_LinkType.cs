using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.Kernel;

namespace XCSJ.EditorExtension.Base.XUnityEditorInternal
{
    /// <summary>
    /// 内部编辑器工具关联类型
    /// </summary>
    [LinkType(typeof(InternalEditorUtility))]
    public class InternalEditorUtility_LinkType : LinkType<InternalEditorUtility_LinkType>
    {
        #region IsScriptOrAssembly

        /// <summary>
        /// 是脚本或程序集 属性信息
        /// </summary>
        public static XMethodInfo IsScriptOrAssembly_XPropertyInfo { get; } = GetXMethodInfo(nameof(IsScriptOrAssembly));

        /// <summary>
        /// 是脚本或程序集
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsScriptOrAssembly(string filename)
        {
            return (bool)IsScriptOrAssembly_XPropertyInfo?.Invoke(null, new object[] { filename });
        }

        #endregion

        #region GetAllScriptGUIDs

        /// <summary>
        /// 获取所有脚本GUID 属性信息
        /// </summary>
        public static XMethodInfo GetAllScriptGUIDs_XPropertyInfo { get; } = GetXMethodInfo(nameof(GetAllScriptGUIDs));

        /// <summary>
        /// 获取所有脚本GUID
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAllScriptGUIDs()
        {
            return GetAllScriptGUIDs_XPropertyInfo?.Invoke(null, Empty<object>.Array) as IEnumerable<string>;
        }

        #endregion
    }
}
