using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.Kernel;

namespace XCSJ.EditorExtension.Base.XUnityEditorInternal
{
    /// <summary>
    /// 日志条目
    /// </summary>
    [LinkType(EditorHelper.UnityEditorInternalPrefix + nameof(LogEntries))]
    public class LogEntries : LinkType<LogEntries>
    {
        #region Clear

        /// <summary>
        /// 清理 方法信息
        /// </summary>
        public static XMethodInfo Clear_MethodInfo { get; } = new XMethodInfo(Type, nameof(Clear), BindingFlags.Static | BindingFlags.Public);

        /// <summary>
        /// 清理
        /// </summary>
        public static void Clear()=> Clear_MethodInfo.Invoke(null, null);

        #endregion
    }
}
