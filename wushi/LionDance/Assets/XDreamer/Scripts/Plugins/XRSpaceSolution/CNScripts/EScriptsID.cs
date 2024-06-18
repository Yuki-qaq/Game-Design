using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginXRSpaceSolution.CNScripts
{
    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值，38656
        /// </summary>
        public const int Begin = (int)EExtensionID._0x2e;

        /// <summary>
        /// 结束值，38784-1=38783
        /// </summary>
        public const int End = (int)EExtensionID._0x2f - 1;
    }

    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    [ScriptEnum(typeof(XRSpaceSolutionManager))]
    public enum EScriptsID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        /// <summary>
        /// XR空间解决方案
        /// </summary>
        [ScriptName(XRSpaceSolutionHelper.Title, nameof(XRSpaceSolution), EGrammarType.Category)]
        XRSpaceSolution,

        /// <summary>
        /// 刷新XRIS配置
        /// </summary>
        [ScriptName("刷新XRIS配置", nameof(RefreshXRISConfig))]
        [ScriptDescription("刷新XRIS配置；")]
        [ScriptReturn("成功返回 #True；失败返回 #False ；")]
        RefreshXRISConfig,
    }
}
