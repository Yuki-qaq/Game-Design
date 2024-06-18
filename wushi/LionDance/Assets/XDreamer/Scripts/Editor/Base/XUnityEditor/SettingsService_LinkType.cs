using UnityEditor;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// <see cref="SettingsService"/>关联类
    /// </summary>
    [LinkType(typeof(SettingsService))]
    public class SettingsService_LinkType:LinkType<SettingsService_LinkType>
    {
        #region FetchSettingsProviders

        /// <summary>
        /// 获取设置提供者 方法信息
        /// </summary>
        public static XMethodInfo FetchSettingsProviders_MethodInfo { get; } = GetXMethodInfo(nameof(FetchSettingsProviders));

        /// <summary>
        /// 获取设置提供者
        /// </summary>
        /// <returns></returns>
        public static SettingsProvider[] FetchSettingsProviders() => FetchSettingsProviders_MethodInfo.Invoke(null, null) as SettingsProvider[];

        #endregion
    }
}
