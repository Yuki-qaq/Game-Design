using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXBox.Base;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;

namespace XCSJ.PluginXBox.Tools
{
    /// <summary>
    /// 交互通过XBox
    /// </summary>
    [Name("交互通过XBox")]
    [Tip("通过XBox模拟控制器交互的输入输出", "Input and output of controller interaction through Xbox simulation")]
    [RequireManager(typeof(XBoxManager), typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.InteractInput, nameof(AnalogController))]
    [Tool(XBoxHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [Owner(typeof(XBoxManager))]
    public class InteractByXBox : BaseAnalogProvider, IInteractProvider, IHapticImpulseProvider, IXBox, IActionProvider
    {
        /// <summary>
        /// 动作名
        /// </summary>
        [Name("动作名")]
        [ActionNamePopup]
        public string _actionName;

        /// <summary>
        /// 动作名
        /// </summary>
        public override string actionName { get => _actionName; set => _actionName = value; }

        /// <summary>
        /// 选择按钮
        /// </summary>
        [Name("选择按钮")]
        [EnumPopup]
        public EXBoxAxisAndButton _buttonOfSelect = EXBoxAxisAndButton.LeftBumper;

        /// <summary>
        /// 选择按钮
        /// </summary>
        public EXBoxAxisAndButton buttonOfSelect
        {
            get => _buttonOfSelect;
            set => this.XModifyProperty(ref _buttonOfSelect, value);
        }

        /// <summary>
        /// 选择死区
        /// </summary>
        [Name("选择死区")]
        [LimitRange(0, 1)]
        public Vector2 _selectDeadZone = new Vector2(0.5f, 1);

        /// <summary>
        /// 激活按钮
        /// </summary>
        [Name("激活按钮")]
        [EnumPopup]
        public EXBoxAxisAndButton _buttonOfActivate = EXBoxAxisAndButton.LeftTrigger;

        /// <summary>
        /// 激活按钮
        /// </summary>
        public EXBoxAxisAndButton buttonOfActivate
        {
            get => _buttonOfActivate;
            set => this.XModifyProperty(ref _buttonOfActivate, value);
        }

        /// <summary>
        /// 激活死区
        /// </summary>
        [Name("激活死区")]
        [LimitRange(0, 1)]
        public Vector2 _activateDeadZone = new Vector2(0.5f, 1);

        /// <summary>
        /// UI按钮
        /// </summary>
        [Name("UI按钮")]
        [EnumPopup]
        public EXBoxAxisAndButton _buttonOfUI = EXBoxAxisAndButton.LeftTrigger;

        /// <summary>
        /// UI按钮
        /// </summary>
        public EXBoxAxisAndButton buttonOfUI
        {
            get => _buttonOfUI;
            set => this.XModifyProperty(ref _buttonOfUI, value);
        }

        /// <summary>
        /// UI死区
        /// </summary>
        [Name("UI死区")]
        [LimitRange(0, 1)]
        public Vector2 _uiDeadZone = new Vector2(0.5f, 1);

        /// <summary>
        /// 启用触觉脉冲
        /// </summary>
        [Name("启用触觉脉冲")]
        public bool _enableHapticImpulse = true;

        /// <summary>
        /// 是否按下选择键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfSelect(AnalogController analogController) => _buttonOfSelect.IsPressed(_selectDeadZone);

        /// <summary>
        /// 是否按下激活键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfActivate(AnalogController analogController) => _buttonOfActivate.IsPressed(_activateDeadZone);

        /// <summary>
        /// 是否按下UI键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfUI(AnalogController analogController) => _buttonOfUI.IsPressed(_uiDeadZone);

        /// <summary>
        /// 发送触觉脉冲
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="amplitude"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool SendHapticImpulse(AnalogController analogController, float amplitude, float duration)
        {
            if(_enableHapticImpulse)
            {
#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS
                var current = XBoxHelper.current;
                if (current != null)
                {
                    current.ResetHaptics();
                    current.ResumeHaptics();
                    CommonFun.DelayCall(current.PauseHaptics, duration);
                    return true;
                }
#endif
            }
            return false;
        }

        /// <summary>
        /// 尝试获取动作值
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="actionName"></param>
        /// <param name="value"></param>
        /// <param name="actionConfigType"></param>
        /// <returns></returns>
        public bool TryGetActionValue(AnalogController analogController, string actionName, out bool value, EActionConfigType actionConfigType = EActionConfigType.Button_InteractSelect)
        {
            if (actionName == this.actionName)
            {
                switch (actionConfigType)
                {
                    case EActionConfigType.Button_Interact:
                        {
                            value = _buttonOfActivate.IsPressed(_selectDeadZone)
                                || _buttonOfSelect.IsPressed(_activateDeadZone)
                                || _buttonOfUI.IsPressed(_uiDeadZone);
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = _buttonOfActivate.IsPressed(_selectDeadZone);
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = _buttonOfSelect.IsPressed(_activateDeadZone);
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = _buttonOfUI.IsPressed(_uiDeadZone);
                            return true;
                        }
                }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试获取动作值
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="actionName"></param>
        /// <param name="value"></param>
        /// <param name="actionConfigType"></param>
        /// <returns></returns>
        public bool TryGetActionValue(AnalogController analogController, string actionName, out float value, EActionConfigType actionConfigType = EActionConfigType.Analog_NX)
        {
            if (actionName == this.actionName)
            {
                switch (actionConfigType)
                {
                    case EActionConfigType.Button_Interact:
                        {
                            value = _buttonOfActivate.IsPressed(_selectDeadZone)
                                || _buttonOfSelect.IsPressed(_activateDeadZone)
                                || _buttonOfUI.IsPressed(_uiDeadZone) ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = _buttonOfActivate.GetValue(_selectDeadZone);
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = _buttonOfSelect.GetValue(_activateDeadZone);
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = _buttonOfUI.GetValue(_uiDeadZone);
                            return true;
                        }
                }
            }
            value = default;
            return false;
        }
    }
    
    /// <summary>
    /// XBox功能接口
    /// </summary>
    public interface IXBox
    {

    }
}
