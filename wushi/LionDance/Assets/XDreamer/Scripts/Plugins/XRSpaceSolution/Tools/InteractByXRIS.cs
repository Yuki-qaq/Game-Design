using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Maths;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCameras;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginXBox.Base;
using XCSJ.PluginXBox.Tools;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginART.Tools;
using XCSJ.PluginART;
using XCSJ.PluginART.Base;
using XCSJ.PluginPeripheralDevice;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#endif

namespace XCSJ.PluginXRSpaceSolution.Tools
{
    /// <summary>
    /// 交互通过XRIS
    /// </summary>
    [Name("交互通过XRIS")]
    [Tip("通过XRIS模拟控制器交互的输入输出", "Analog input and output of controller interaction through XRIS")]
    [RequireManager(typeof(XRSpaceSolutionManager), typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.InteractInput, nameof(AnalogController))]
    //[Tool(XRSpaceSolutionHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [RequireComponent(typeof(AnalogController))]
    public class InteractByXRIS : BaseAnalogProvider, IInteractProvider, IActionProvider
    {
        /// <summary>
        /// 交互处理
        /// </summary>
        [Name("交互处理")]
        [OnlyMemberElements]
        public InteractHandler _interactHandler = new InteractHandler();

        /// <summary>
        /// 动作名
        /// </summary>
        public override string actionName { get => _interactHandler.actionName; set => _interactHandler.actionName = value; }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            XDreamerEvents.onXRAnswerReceived += OnXRAnswerReceived;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            XDreamerEvents.onXRAnswerReceived -= OnXRAnswerReceived;
        }

        private void OnXRAnswerReceived(XRAnswer answer)
        {
            if (!_interactHandler.Handle(answer)) return;

            HandleUnityXRDevice();
            HandleUnityInputAction();
            HandleXBox();
            HandleART();
        }

        private void HandleXBox()
        {
            var interact = GetComponents<InteractByXBox>().FirstOrDefault(c => c && c.actionName == _interactHandler.actionName);
            var needEnable = _interactHandler.config.enable && _interactHandler.anyXBoxEnable;
            if (!interact)
            {
                if (!needEnable) return;
                interact = this.XAddComponent<InteractByXBox>();
                interact.actionName = _interactHandler.actionName;
                interact.Regist();
            }
            interact.XSetEnable(needEnable);

            if (_interactHandler.select?.xboxConfig is XBoxConfig select && select.enable)
            {
                interact.buttonOfSelect = select.axisAndButton;
            }
            else
            {
                interact.buttonOfSelect = EXBoxAxisAndButton.None;
            }

            if (_interactHandler.activate?.xboxConfig is XBoxConfig activate && activate.enable)
            {
                interact.buttonOfActivate = activate.axisAndButton;
            }
            else
            {
                interact.buttonOfActivate = EXBoxAxisAndButton.None;
            }

            if (_interactHandler.ui?.xboxConfig is XBoxConfig ui && ui.enable)
            {
                interact.buttonOfUI = ui.axisAndButton;
            }
            else
            {
                interact.buttonOfUI = EXBoxAxisAndButton.None;
            }
        }

        ARTStreamClient _ARTStreamClient;

        private void HandleART()
        {
            var interact = GetComponents<InteractByART>().FirstOrDefault(c => c && c.actionName == _interactHandler.actionName);
            var needEnable = _interactHandler.config.enable && _interactHandler.anyARTEnable;
            if (!interact && !needEnable) return;

            ARTManager.HandleClient(ref _ARTStreamClient, _interactHandler.config.artClientConfig, client =>
            {
                if (!interact)
                {
                    interact = this.XAddComponent<InteractByART>();
                    interact.actionName = _interactHandler.actionName;
                    interact.Regist();
                }

                interact._streamClient = client;
                interact.XSetEnable(needEnable);

                if (_interactHandler.select?.buttonARTConfig is ButtonARTConfig select && select.enable)
                {
                    interact._buttonOfSelect = select.flystickButton;
                }
                else
                {
                    interact._buttonOfSelect = new FlystickButton();
                }

                if (_interactHandler.activate?.buttonARTConfig is ButtonARTConfig activate && activate.enable)
                {
                    interact._buttonOfActivate = activate.flystickButton;
                }
                else
                {
                    interact._buttonOfActivate = new FlystickButton();
                }

                if (_interactHandler.ui?.buttonARTConfig is ButtonARTConfig ui && ui.enable)
                {
                    interact._buttonOfUI = ui.flystickButton;
                }
                else
                {
                    interact._buttonOfUI = new FlystickButton();
                }
            });
        }

        private void HandleUnityXRDevice()
        {
            var interact = GetComponents<InteractByXRDevice>().FirstOrDefault(c => c && c.actionName == _interactHandler.actionName);
            var needEnable = _interactHandler.config.enable && _interactHandler.anyUnityXRDeviceEnable;
            if (!interact && !needEnable) return;

            if (!interact)
            {
                interact = this.XAddComponent<InteractByXRDevice>();
                interact.actionName = _interactHandler.actionName;
                interact.Regist();
            }

            interact.XSetEnable(needEnable);

            if (_interactHandler.select?.buttonUnityXRDeviceConfig is ButtonUnityXRDeviceConfig select
                && select.enable)
            {
                interact._selectDeviceType = select.deviceType;
                interact._selectButton = select.button;
            }
            else
            {
                interact._selectDeviceType = EXRDeviceType.None;
                interact._selectButton = EXRButton.None;
            }

            if (_interactHandler.activate?.buttonUnityXRDeviceConfig is ButtonUnityXRDeviceConfig activate
               && activate.enable)
            {
                interact._activateDeviceType = activate.deviceType;
                interact._activateButton = activate.button;
            }
            else
            {
                interact._activateDeviceType = EXRDeviceType.None;
                interact._activateButton = EXRButton.None;
            }

            if (_interactHandler.ui?.buttonUnityXRDeviceConfig is ButtonUnityXRDeviceConfig ui
               && ui.enable)
            {
                interact._uiDeviceType = ui.deviceType;
                interact._uiButton = ui.button;
            }
            else
            {
                interact._uiDeviceType = EXRDeviceType.None;
                interact._uiButton = EXRButton.None;
            }
        }

        private void HandleUnityInputAction()
        {
            var interact = GetComponents<InteractByInputAction>().FirstOrDefault(c => c && c.actionName == _interactHandler.actionName);
            var needEnable = _interactHandler.config.enable && _interactHandler.anyUnityInputActionEnable;
            if (!interact && !needEnable) return;

#if XDREAMER_XR_INTERACTION_TOOLKIT
            var inputActionManager = GetComponentInParent<InputActionManager>();
            if (inputActionManager)
            {
                if (!interact)
                {
                    interact = this.XAddComponent<InteractByInputAction>();
                    interact.actionName = _interactHandler.actionName;
                    interact.Regist();
                }

                interact.XSetEnable(needEnable);

                var actionAssets = inputActionManager.actionAssets;
                if (_interactHandler.select?.buttonUnityInputActionConfig is ButtonUnityInputActionConfig select
                    && select.enable
                    && actionAssets.FindInputAction(select.actionName) is InputAction selectAction)
                {
                    if (interact.selectAction.action != selectAction)
                    {
                        interact.selectAction = new InputActionProperty(InputActionReference.Create(selectAction));
                    }
                }
                else
                {
                    interact.selectAction = new InputActionProperty(new InputAction());
                }
                if (_interactHandler.activate?.buttonUnityInputActionConfig is ButtonUnityInputActionConfig activate
                   && activate.enable
                   && actionAssets.FindInputAction(activate.actionName) is InputAction activateAction)
                {
                    if (interact.activateAction.action != activateAction)
                    {
                        interact.activateAction = new InputActionProperty(InputActionReference.Create(activateAction));
                    }
                }
                else
                {
                    interact.activateAction = new InputActionProperty(new InputAction());
                }
                if (_interactHandler.ui?.buttonUnityInputActionConfig is ButtonUnityInputActionConfig ui
                   && ui.enable
                   && actionAssets.FindInputAction(ui.actionName) is InputAction uiAction)
                {
                    if (interact.uiAction.action != uiAction)
                    {
                        interact.uiAction = new InputActionProperty(InputActionReference.Create(uiAction));
                    }
                }
                else
                {
                    interact.uiAction = new InputActionProperty(new InputAction());
                }
            }
#endif
        }

        /// <summary>
        /// 是选择按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfSelect(AnalogController analogController) => _interactHandler.Select();

        /// <summary>
        /// 是激活按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfActivate(AnalogController analogController) => _interactHandler.Activate();

        /// <summary>
        /// 是UI按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfUI(AnalogController analogController) => _interactHandler.UI();

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
                            value = _interactHandler.Select()
                                || _interactHandler.Activate()
                                || _interactHandler.UI();
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = _interactHandler.Select();
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = _interactHandler.Activate();
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = _interactHandler.UI();
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
                            value = _interactHandler.Select()
                                || _interactHandler.Activate()
                                || _interactHandler.UI() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = _interactHandler.Select() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = _interactHandler.Activate() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = _interactHandler.UI() ? 1 : 0;
                            return true;
                        }
                }
            }
            value = default;
            return false;
        }
    }
}
