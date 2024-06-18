using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Inputs
{
    /// <summary>
    /// 输入键码:输入键码组件是键盘按键的触发器。键盘按下或弹起后，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSCategory.InputDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(InputKeyCode))]
    [Tip("输入键码组件是键盘按键的触发器。键盘按下或弹起后，组件切换为完成态。", "The input key code component is the trigger of keyboard keys. After the keyboard is pressed or popped up, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Keyboard)]
    public class InputKeyCode : Trigger<InputKeyCode>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "输入键码";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(CommonCategory.CommonUse, typeof(SMSManager))]
        [StateComponentMenu(CommonCategory.CommonUseDirectory + Title, typeof(SMSManager))]
        [StateLib(SMSCategory.Input, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.InputDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(InputKeyCode))]
        [Tip("输入键码组件是键盘按键的触发器。键盘按下或弹起后，组件切换为完成态。", "The input key code component is the trigger of keyboard keys. After the keyboard is pressed or popped up, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 按压状态
        /// </summary>
        [Name("按压状态")]
        [EnumPopup]
        public EPressState _pressType = EPressState.Keeping;

        /// <summary>
        /// 按压状态
        /// </summary>
        [Name("按压状态")]
        public enum EPressState
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 按下
            /// </summary>
            [Name("按下")]
            Down,

            /// <summary>
            /// 弹起
            /// </summary>
            [Name("弹起")]
            Up,

            /// <summary>
            /// 按下保持中
            /// </summary>
            [Name("按下保持中")]
            Keeping,
        }

        /// <summary>
        /// 触发规则
        /// </summary>
        [Name("触发规则")]
        [EnumPopup]
        public ETriggerRule triggerRule = ETriggerRule.None;

        /// <summary>
        /// 键码
        /// </summary>
        [Name("键码")]
        [Tip("多个键码同时按下才能触发后续逻辑", "多个键码同时按下才能触发后续逻辑")]
        public List<KeyCode> keyCodes = new List<KeyCode>();

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            if (finished || _pressType == EPressState.None) return;
            if (keyCodes.Count == 0)
            {
                finished = true;
                return;
            }
            foreach (var key in keyCodes)
            {
                switch(_pressType)
                {
                    case EPressState.Keeping:
                        {
                            if (!XInput.GetKey(key)) return;
                            break;
                        }
                    case EPressState.Down:
                        {
                            if (!XInput.GetKeyDown(key)) return;
                            break;
                        }
                    case EPressState.Up:
                        {
                            if (!XInput.GetKeyUp(key)) return;
                            break;
                        }
                }
            }

            switch (triggerRule)
            {
                case ETriggerRule.OnlyEnableWhenOnUGUI:
                    {
                        if (!CommonFun.IsOnUGUI())
                        {
                            return;
                        }
                        break;
                    }
                case ETriggerRule.DisableWhenOnUGUI:
                    {
                        if (CommonFun.IsOnUGUI())
                        {
                            return;
                        }
                        break;
                    }
                case ETriggerRule.OnlyEnableWhenOnGUI:
                    {
                        if (!CommonFun.IsOnGUI())
                        {
                            return;
                        }
                        break;
                    }
                case ETriggerRule.DisableWhenOnIMGUI:
                    {
                        if (CommonFun.IsOnGUI())
                        {
                            return;
                        }
                        break;
                    }
                case ETriggerRule.OnlyEnableWhenOnUnityUI:
                    {
                        if (!CommonFun.IsOnUINow())
                        {
                            return;
                        }
                        break;
                    }
                case ETriggerRule.DisableWhenOnUnityUI:
                    {
                        if (CommonFun.IsOnUINow())
                        {
                            return;
                        }
                        break;
                    }
            }
            finished = true;
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return keyCodes.Count > 0;
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return keyCodes.Count > 0 ? keyCodes[0].ToString() : "";
        }

        /// <summary>
        /// 触发规则
        /// </summary>
        [Name("触发规则")]
        public enum ETriggerRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 仅UGUI上时启用
            /// </summary>
            [Name("仅UGUI上时启用")]
            OnlyEnableWhenOnUGUI,

            /// <summary>
            /// 在UGUI上时禁用
            /// </summary>
            [Name("在UGUI上时禁用")]
            DisableWhenOnUGUI,

            /// <summary>
            /// 在UGUI上时启用
            /// </summary>
            [Name("在UGUI上时启用")]
            OnlyEnableWhenOnGUI,

            /// <summary>
            /// 在IMGUI上时禁用
            /// </summary>
            [Name("在IMGUI上时禁用")]
            DisableWhenOnIMGUI,

            /// <summary>
            /// 在UGUI上时启用
            /// </summary>
            [Name("在UGUI上时启用")]
            OnlyEnableWhenOnUnityUI,

            /// <summary>
            /// 在UnityUI上时禁用
            /// </summary>
            [Name("在UnityUI上时禁用")]
            DisableWhenOnUnityUI,
        }
    }
}
