using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginCameras.Tools.Base;
using UnityEngine.XR;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginPeripheralDevice;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 交互通过输入动作:通过输入动作模拟控制器交互输入输出
    /// </summary>
    [Name("交互通过输入动作")]
    [Tip("通过输入动作模拟控制器交互输入输出", "Simulate the input and output of controller interaction through input action")]
    [Tool(XRITHelper.InteractInput, nameof(AnalogController))]
    [XCSJ.Attributes.Icon(EIcon.Import)]
    public class InteractByInputAction : BaseAnalogProvider, IInteractProvider, IActionProvider
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

#if XDREAMER_INPUT_SYSTEM

        /// <summary>
        /// 选择动作
        /// </summary>
        [Name("选择动作")]
        public InputActionProperty _selectAction = new InputActionProperty();

        /// <summary>
        /// 选择动作
        /// </summary>
        public InputActionProperty selectAction
        {
            get => _selectAction;
            set => SetInputActionProperty(ref _selectAction, value);
        }

        /// <summary>
        /// 激活动作
        /// </summary>
        [Name("激活动作")]
        public InputActionProperty _activateAction = new InputActionProperty();

        /// <summary>
        /// 激活动作
        /// </summary>
        public InputActionProperty activateAction
        {
            get => _activateAction;
            set => SetInputActionProperty(ref _activateAction, value);
        }

        /// <summary>
        /// UI动作
        /// </summary>
        [Name("UI动作")]
        public InputActionProperty _uiAction = new InputActionProperty();

        /// <summary>
        /// UI动作
        /// </summary>
        public InputActionProperty uiAction
        {
            get => _uiAction;
            set => SetInputActionProperty(ref _uiAction, value);
        }

        void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
        {
#if !UNITY_WEBGL
            var isPlaying = Application.isPlaying;
            if (isPlaying)
                property.DisableDirectAction();

            property = value;

            if (isPlaying && isActiveAndEnabled)
                property.EnableDirectAction();
#endif
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

#if !UNITY_WEBGL
            _selectAction.EnableDirectAction();
            _activateAction.EnableDirectAction();
            _uiAction.EnableDirectAction();
#endif
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

#if !UNITY_WEBGL
            _selectAction.DisableDirectAction();
            _activateAction.DisableDirectAction();
            _uiAction.DisableDirectAction();
#endif
        }

        /// <summary>
        /// 是选择按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfSelect(AnalogController analogController) => _selectAction.IsPressed();

        /// <summary>
        /// 是激活按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfActivate(AnalogController analogController) => _activateAction.IsPressed();

        /// <summary>
        /// 是UI按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfUI(AnalogController analogController) => _uiAction.IsPressed();

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
                            value = _selectAction.IsPressed()
                                || _activateAction.IsPressed()
                                || _uiAction.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = _selectAction.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = _activateAction.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = _uiAction.IsPressed();
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
                            value = _selectAction.IsPressed()
                                || _activateAction.IsPressed()
                                || _uiAction.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = _selectAction.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = _activateAction.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = _uiAction.IsPressed() ? 1 : 0;
                            return true;
                        }
                }
            }
            value = default;
            return false;
        }

#else

        /// <summary>
        /// 是激活按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfActivate(AnalogController analogController) => false;

        /// <summary>
        /// 是选择按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfSelect(AnalogController analogController) => false;

        /// <summary>
        /// 是UI按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfUI(AnalogController analogController) => false;

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
            value = default;
            return false;
        }

#endif
    }
}
