using System;
using System.Linq;
using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// Mono导入器关联类型
    /// </summary>
    [LinkType(typeof(MonoImporter))]
    public class MonoImporter_LinkType : LinkType<MonoImporter_LinkType>
    {
        #region CopyMonoScriptIconToImporters

        /// <summary>
        /// 复制Mono脚本图标到导入器 方法信息
        /// </summary>
        public static XMethodInfo CopyMonoScriptIconToImporters_MethodInfo { get; } = new XMethodInfo(Type, nameof(CopyMonoScriptIconToImporters), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 复制Mono脚本图标到导入器
        /// </summary>
        /// <param name="script"></param>
        public static void CopyMonoScriptIconToImporters(MonoScript script)
        {
            CopyMonoScriptIconToImporters_MethodInfo.Invoke(null, new object[] { script });
        }

        #endregion

        /// <summary>
        /// 获取Mono脚本
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MonoScript GetMonoScript(Type type)
        {
            if (type == null) return null;
            return MonoImporter.GetAllRuntimeMonoScripts().FirstOrDefault(ms => ms.GetClass() == type);
        }
    }
}
