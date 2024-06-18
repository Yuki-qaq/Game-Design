using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 编辑器工具关联类型
    /// </summary>
    [LinkType(typeof(EditorUtility))]
    public class EditorUtility_LinkType : LinkType<EditorUtility_LinkType>
    {
        #region ForceReloadInspectors

        /// <summary>
        /// 强制重新加载检查器 方法信息
        /// </summary>
        public static XMethodInfo ForceReloadInspectors_MethodInfo { get; } = new XMethodInfo(Type, nameof(ForceReloadInspectors), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 强制重新加载检查器
        /// </summary>
        public static void ForceReloadInspectors()
        {
            ForceReloadInspectors_MethodInfo.Invoke(null, null);
        }

        #endregion
    }
};