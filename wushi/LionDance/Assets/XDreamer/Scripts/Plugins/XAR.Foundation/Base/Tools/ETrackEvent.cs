using XCSJ.Attributes;

namespace XCSJ.PluginXAR.Foundation.Base.Tools
{
    /// <summary>
    /// 跟踪事件
    /// </summary>
    [Name("跟踪事件")]
    public enum ETrackEvent
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [Tip("不做任何操作")]
        None,

        /// <summary>
        /// 当已添加时
        /// </summary>
        [Name("当已添加时")]
        OnAdded,

        /// <summary>
        /// 当已移除时
        /// </summary>
        [Name("当已移除时")]
        OnRemoved,

        /// <summary>
        /// 当已更新时
        /// </summary>
        [Name("当已更新时")]
        OnUpdated,

        /// <summary>
        /// 当无时
        /// </summary>
        [Name("当无时")]
        [Tip("不考虑上次跟踪状态，本次跟踪状态为无时回调；")]
        OnNone,

        /// <summary>
        /// 当到无时
        /// </summary>
        [Name("当到无时")]
        [Tip("上次跟踪状态不为无，本次跟踪状态为无时回调；")]
        OnToNone,

        /// <summary>
        /// 当总是无时
        /// </summary>
        [Name("当总是无时")]
        [Tip("上次跟踪状态为无，本次跟踪状态为无时回调；")]
        OnNoneAlways,

        /// <summary>
        /// 当无到限定时
        /// </summary>
        [Name("当无到限定时")]
        [Tip("上次跟踪状态为无，本次跟踪状态为限定时回调；")]
        OnNoneToLimited,

        /// <summary>
        /// 当无到跟踪中时
        /// </summary>
        [Name("当无到跟踪中时")]
        [Tip("上次跟踪状态为无，本次跟踪状态为跟踪中时回调；")]
        OnNoneToTracking,

        /// <summary>
        /// 当限定时
        /// </summary>
        [Name("当限定时")]
        [Tip("不考虑上次跟踪状态，本次跟踪状态为限定时回调；")]
        OnLimited,

        /// <summary>
        /// 当到限定时
        /// </summary>
        [Name("当到限定时")]
        [Tip("上次跟踪状态不为限定，本次跟踪状态为限定时回调；")]
        OnToLimited,

        /// <summary>
        /// 当总是限定时
        /// </summary>
        [Name("当总是限定时")]
        [Tip("上次跟踪状态为限定，本次跟踪状态为限定时回调；")]
        OnLimitedAlways,

        /// <summary>
        /// 当限定到无时
        /// </summary>
        [Name("当限定到无时")]
        [Tip("上次跟踪状态为限定，本次跟踪状态为无时回调；")]
        OnLimitedToNone,

        /// <summary>
        /// 当限定到跟踪中时
        /// </summary>
        [Name("当限定到跟踪中时")]
        [Tip("上次跟踪状态为限定，本次跟踪状态为跟踪中时回调；")]
        OnLimitedToTracking,

        /// <summary>
        /// 当跟踪中时
        /// </summary>
        [Name("当跟踪中时")]
        [Tip("不考虑上次跟踪状态，本次跟踪状态为跟踪中时回调；")]
        OnTracking,

        /// <summary>
        /// 当到跟踪中时
        /// </summary>
        [Name("当到跟踪中时")]
        [Tip("上次跟踪状态不为跟踪中，本次跟踪状态为跟踪中时回调；")]
        OnToTracking,

        /// <summary>
        /// 当总是跟踪时
        /// </summary>
        [Name("当总是跟踪时")]
        [Tip("上次跟踪状态为跟踪中，本次跟踪状态为跟踪中时回调；")]
        OnTrackingAlways,

        /// <summary>
        /// 当跟踪中到无时
        /// </summary>
        [Name("当跟踪中到无时")]
        [Tip("上次跟踪状态为跟踪中，本次跟踪状态为无时回调；")]
        OnTrackingToNone,

        /// <summary>
        /// 当跟踪中到限定时
        /// </summary>
        [Name("当跟踪中到限定时")]
        [Tip("上次跟踪状态为跟踪中，本次跟踪状态为限定时回调；")]
        OnTrackingToLimited,
    }
}
