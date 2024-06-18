using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginHoloLens
{
    /// <summary>
    /// ID区间
    /// </summary>
    public static class IDRange
    {
        /// <summary>
        /// 开始34944
        /// </summary>
        public const int Begin = (int)EExtensionID._0x11;

        /// <summary>
        /// 结束
        /// </summary>
        public const int End = (int)EExtensionID._0xa - 1;

        /// <summary>
        /// 片段24
        /// </summary>
        public const int Fragment = 0x18;//24

        /// <summary>
        /// 通用34944
        /// </summary>
        public const int Common = Begin + Fragment * 0;

        /// <summary>
        /// Mono行为34968
        /// </summary>
        public const int MonoBehaviour = Begin + Fragment * 1;

        /// <summary>
        /// 状态库34992
        /// </summary>
        public const int StateLib = Begin + Fragment * 2;

        /// <summary>
        /// 工具库35016
        /// </summary>
        public const int Tools = Begin + Fragment * 3;

        /// <summary>
        /// 编辑器35,040
        /// </summary>
        public const int Editor = Begin + Fragment * 4;
    }

    /// <summary>
    /// HoloLens脚本ID
    /// </summary>
    [Name("HoloLens脚本ID")]
    [ScriptEnum(typeof(HoloLensManager))]
    public enum EHoloLensScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region HoloLens-目录
        /// <summary>
        /// HoloLens脚本
        /// </summary>
        [ScriptName(nameof(HoloLens), nameof(HoloLens), EGrammarType.Category)]
        #endregion
        HoloLens,

        /// <summary>
        /// 当前最大
        /// </summary>
        MaxCurrent
    }
}
