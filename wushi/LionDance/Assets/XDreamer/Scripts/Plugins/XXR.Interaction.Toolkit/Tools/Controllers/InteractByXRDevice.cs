using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCameras;
using XCSJ.PluginCameras.Controllers;
using XCSJ.PluginCameras.Tools.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;
using UnityEngine.XR;
using XCSJ.PluginXRSpaceSolution.Base;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 交互通过输入动作:通过输入动作模拟控制器交互输入输出
    /// </summary>
    [Name("交互通过XR设备")]
    [Tip("通过输入动作模拟控制器交互输入输出", "Simulate the input and output of controller interaction through input action")]
    [Tool(XRITHelper.InteractInput, nameof(AnalogController))]
    [XCSJ.Attributes.Icon(EIcon.Import)]
    public class InteractByXRDevice : BaseAnalogProvider, IInteractProvider, IActionProvider
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
        /// 选择设备类型
        /// </summary>
        [Name("选择设备类型")]
        [EnumPopup]
        public EXRDeviceType _selectDeviceType = EXRDeviceType.Left;

        /// <summary>
        /// 选择按钮
        /// </summary>
        [Name("选择按钮")]
        [EnumPopup]
        public EXRButton _selectButton = EXRButton.Trigger;

        /// <summary>
        /// 选择设备类型
        /// </summary>
        [Name("选择设备类型")]
        [EnumPopup]
        public EXRDeviceType _activateDeviceType = EXRDeviceType.Left;

        /// <summary>
        /// 选择按钮
        /// </summary>
        [Name("选择按钮")]
        [EnumPopup]
        public EXRButton _activateButton = EXRButton.Grip;

        /// <summary>
        /// 选择设备类型
        /// </summary>
        [Name("选择设备类型")]
        [EnumPopup]
        public EXRDeviceType _uiDeviceType = EXRDeviceType.Left;

        /// <summary>
        /// 选择按钮
        /// </summary>
        [Name("选择按钮")]
        [EnumPopup]
        public EXRButton _uiButton = EXRButton.Trigger;

        /// <summary>
        /// 是选择按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfSelect(AnalogController analogController)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            return _selectDeviceType.IsPressed(_selectButton, out var pressed) && pressed;
#else
            return false;
#endif
        }

        /// <summary>
        /// 是激活按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfActivate(AnalogController analogController)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            return _activateDeviceType.IsPressed(_activateButton, out var pressed) && pressed;            
#else
            return false;
#endif
        }

        /// <summary>
        /// 是UI按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfUI(AnalogController analogController)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            return _uiDeviceType.IsPressed(_uiButton, out var pressed) && pressed;           
#else
            return false;
#endif
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
                            value = IsPressedOfSelect(default)
                                || IsPressedOfActivate(default)
                                || IsPressedOfUI(default);
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = IsPressedOfSelect(default);
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = IsPressedOfActivate(default);
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = IsPressedOfUI(default);
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
                            value = IsPressedOfSelect(default)
                                || IsPressedOfActivate(default)
                                || IsPressedOfUI(default) ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = IsPressedOfSelect(default) ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = IsPressedOfActivate(default) ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = IsPressedOfUI(default) ? 1 : 0;
                            return true;
                        }
                }
            }
            value = default;
            return false;
        }
    }
}
