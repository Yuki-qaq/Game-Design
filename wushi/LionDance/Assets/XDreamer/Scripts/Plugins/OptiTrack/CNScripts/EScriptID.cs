using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.PluginOptiTrack.CNScripts
{
    /// <summary>
    /// 脚本ID枚举
    /// </summary>
    [Name("脚本ID")]
    [ScriptEnum(typeof(OptiTrackManager))]
    public enum EScriptID
    {
        /// <summary>
        /// 开始值
        /// </summary>
        _Begin = IDRange.Begin,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}
