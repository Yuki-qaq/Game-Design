using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.UGUI
{
    /// <summary>
    /// 滚动条控制:滚动条控制组件是滚动条当前值与设定值符合设定条件的触发器。当条件满足时，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSCategory.UGUIDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(ScrollBarController))]
    [Tip("滚动条控制组件是滚动条当前值与设定值符合设定条件的触发器。当条件满足时，组件切换为完成态。", "The scroll bar control component is a trigger that the current value of the scroll bar and the set value meet the setting conditions. When the conditions are met, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.ScrollBar)]
    public class ScrollBarController : Trigger<ScrollBarController>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "滚动条控制";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.UGUI, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.UGUIDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(ScrollBarController))]
        [Tip("滚动条控制组件是滚动条当前值与设定值符合设定条件的触发器。当条件满足时，组件切换为完成态。", "The scroll bar control component is a trigger that the current value of the scroll bar and the set value meet the setting conditions. When the conditions are met, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateScrollBarController(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 滚动条
        /// </summary>
        [Name("滚动条")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(Scrollbar))]
        [Readonly(EEditorMode.Runtime)]
        public Scrollbar scrollbar;

        /// <summary>
        /// 数值比较触发器
        /// </summary>
        [Name("数值比较触发器")]
        public FloatValueTrigger numberValueTrigger = new FloatValueTrigger();

        /// <summary>
        /// 完成一次
        /// </summary>
        [Name("完成一次")]
        [Tip("勾选时：条件成立立即完成；不勾选时：成立条件随着滚动条值不断变化", "When checked: completed immediately after the condition is established; If unchecked: the valid condition changes with the scroll bar value")]
        public bool finishOnce = true;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            if (scrollbar)
            {
                scrollbar.onValueChanged.AddListener(OnScrollBarChange);
            }
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            if (scrollbar)
            {
                scrollbar.onValueChanged.RemoveListener(OnScrollBarChange);
            }
            base.OnExit(data);
        }

        private void OnScrollBarChange(float value)
        {
            if (finishOnce && finished)
            {
                return;
            }
            finished = numberValueTrigger.IsTrigger(value);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => scrollbar;

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => scrollbar ? scrollbar.name : "";
    }
}
