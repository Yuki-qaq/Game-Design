using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginHTCVive
{
    /// <summary>
    /// HTCVive ID区间
    /// </summary>
    public static class HTCViveIDRange
    {
        /// <summary>
        /// 开始35712
        /// </summary>
        public const int Begin = (int)EExtensionID._0x17;

        /// <summary>
        /// 借宿
        /// </summary>
        public const int End = (int)EExtensionID._0xa - 1;

        /// <summary>
        /// 片段24
        /// </summary>
        public const int Fragment = 0x18;//24

        /// <summary>
        /// 通用35712
        /// </summary>
        public const int Common = Begin + Fragment * 0;

        /// <summary>
        /// Mono行为35736
        /// </summary>
        public const int MonoBehaviour = Begin + Fragment * 1;

        /// <summary>
        /// 状态库35760
        /// </summary>
        public const int StateLib = Begin + Fragment * 2;

        /// <summary>
        /// 工具库35784
        /// </summary>
        public const int Tools = Begin + Fragment * 3;

        /// <summary>
        /// 编辑器35808
        /// </summary>
        public const int Editor = Begin + Fragment * 4;
    }

    /// <summary>
    /// HTC Vive脚本ID
    /// </summary>
    [Name("HTC Vive脚本ID")]
    [ScriptEnum(typeof(HTCViveManager))]
    public enum EHTCViveScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = HTCViveIDRange.Begin,

        #region HTC Vive-目录
        /// <summary>
        /// HTC Vive
        /// </summary>
        [ScriptName(nameof(HTCVive), nameof(HTCVive), EGrammarType.Category)]
        [ScriptDescription("HTC Vive的相关脚本目录；")]
        #endregion
        HTCVive,

        #region 激活SteamVR输入动作集
        /// <summary>
        /// 激活SteamVR输入动作集
        /// </summary>
        [ScriptName("激活SteamVR输入动作集", nameof(ActiveSteamVRInputActionSet))]
        [ScriptDescription("SteamVR动作集激活后才能执行对应的动作", "")]
        [ScriptReturn("返回值为空", "Return null")]
        [ScriptParams(1, EParamType.String, "动作集名称:")]
        [ScriptParams(2, EParamType.Bool, "激活:", defaultObject = EBool.Yes)]
        #endregion
        ActiveSteamVRInputActionSet,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}
