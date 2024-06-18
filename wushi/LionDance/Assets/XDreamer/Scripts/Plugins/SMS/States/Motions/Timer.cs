using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 定时器：用于添加具有延时执行功能的状态
    /// </summary>
    [Name(Title, nameof(Timer))]
    [Tip("用于添加具有延时执行功能的状态", "Used to add a state with delayed execution function")]
    [XCSJ.Attributes.Icon(EIcon.Timer)]
    [ComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
    public class Timer : WorkClip<Timer>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "定时器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(Timer))]
        [Tip("用于添加具有延时执行功能的状态", "Used to add a state with delayed execution function")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(SMSCategory.Action, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
        [StateLib(CommonCategory.CommonUse, typeof(SMSManager), categoryIndex = -1)]
        [StateComponentMenu(CommonCategory.CommonUseDirectory + Title, typeof(SMSManager), categoryIndex = -1)]
        public static State CreateTimer(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => timeLength.ToString() + "秒";

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData) { }
    }
}
