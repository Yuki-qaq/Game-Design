using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginXBox
{
    /// <summary>
    /// ID区间
    /// </summary>
    public static class IDRange
    {
        /// <summary>
        /// 开始35328
        /// </summary>
        public const int Begin = (int)EExtensionID._0x14;

        /// <summary>
        /// 结束35456-1=35455
        /// </summary>
        public const int End = (int)EExtensionID._0x15 - 1;

        /// <summary>
        /// 片段24
        /// </summary>
        public const int Fragment = 0x18;

        /// <summary>
        /// 通用35328
        /// </summary>
        public const int Common = Begin + Fragment * 0;

        /// <summary>
        /// Mono行为35352
        /// </summary>
        public const int MonoBehaviour = Begin + Fragment * 1;

        /// <summary>
        /// 状态库35376
        /// </summary>
        public const int StateLib = Begin + Fragment * 2;

        /// <summary>
        /// 工具库35400
        /// </summary>
        public const int Tools = Begin + Fragment * 3;

        /// <summary>
        /// 编辑器35424
        /// </summary>
        public const int Editor = Begin + Fragment * 4;
    }

    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    [ScriptEnum(typeof(XBoxManager))]
    public enum EScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region XBox-目录
        /// <summary>
        /// XBox 360
        /// </summary>
        [ScriptName(nameof(XBox), nameof(XBox), EGrammarType.Category)]
        [ScriptDescription("XBox的相关脚本目录；")]
        #endregion
        XBox,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}
