using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.PluginSMS.CNScripts
{
    /// <summary>
    /// 生命周期事件简版
    /// </summary>
    [Name("生命周期事件简版")]
    public enum ELifecycleEventLite
    {
        /// <summary>
        /// 进入
        /// </summary>
        [Name("进入")]
        [Tip("当事件进入时调用", "Called when an event enters")]
        OnEntry,

        /// <summary>
        /// 更新
        /// </summary>
        [Name("更新")]
        [Tip("当事件更新时调用", "Called when the event is updated")]
        OnUpdate,

        /// <summary>
        /// 退出
        /// </summary>
        [Name("退出")]
        [Tip("当事件退出时调用", "Exit event when called")]
        OnExit,
    }

    /// <summary>
    /// 生命周期事件简版函数
    /// </summary>
    [Serializable]
    [Name("生命周期事件简版函数")]
    public class LifecycleEventLiteFunction : EnumFunction<ELifecycleEventLite> { }

    /// <summary>
    /// 生命周期事件简版函数集合
    /// </summary>
    [Name("生命周期事件简版函数集合")]
    [Serializable]
    public class LifecycleEventLiteFunctionCollection : EnumFunctionCollection<ELifecycleEventLite, LifecycleEventLiteFunction> { }

    /// <summary>
    /// 生命周期事件
    /// </summary>
    [Name("生命周期事件")]
    public enum ELifecycleEvent
    {
        /// <summary>
        /// 预进入
        /// </summary>
        [Name("预进入")]
        [Tip("当事件将进入时调用", "Called when the event is about to enter")]
        OnBeforeEntry,

        /// <summary>
        /// 进入
        /// </summary>
        [Name("进入")]
        [Tip("当事件进入时调用", "Called when an event enters")]
        OnEntry,

        /// <summary>
        /// 已进入
        /// </summary>
        [Name("已进入")]
        [Tip("当事件已进入时调用", "Called when an event has entered")]
        OnAfterEntry,

        /// <summary>
        /// 更新
        /// </summary>
        [Name("更新")]
        [Tip("当事件更新时调用", "Called when the event is updated")]
        OnUpdate,

        /// <summary>
        /// 预退出
        /// </summary>
        [Name("预退出")]
        [Tip("当事件将退出时调用", "Called when the event will exit")]
        OnBeforeExit,

        /// <summary>
        /// 退出
        /// </summary>
        [Name("退出")]
        [Tip("当事件退出时调用", "Exit event when called")]
        OnExit,

        /// <summary>
        /// 已退出
        /// </summary>
        [Name("已退出")]
        [Tip("当事件已退出时调用", "Called when the event has exited")]
        OnAfterExit,
    }

    /// <summary>
    /// 生命周期事件函数
    /// </summary>
    [Name("生命周期事件函数")]
    [Serializable]
    public class LifecycleEventFunction : EnumFunction<ELifecycleEvent> { }

    /// <summary>
    /// 生命周期事件函数集合
    /// </summary>
    [Name("生命周期事件函数集合")]
    [Serializable]
    public class LifecycleEventFunctionCollection : EnumFunctionCollection<ELifecycleEvent, LifecycleEventFunction> { }
}
