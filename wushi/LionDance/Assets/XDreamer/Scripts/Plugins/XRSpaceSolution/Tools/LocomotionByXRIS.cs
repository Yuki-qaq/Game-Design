using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCameras;
using XCSJ.PluginCameras.Controllers;
using XCSJ.PluginCameras.Tools.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPeripheralDevice;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.Extension.Base.Interactions.Tools;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#endif

namespace XCSJ.PluginXRSpaceSolution.Tools
{
    /// <summary>
    /// 运动通过XRIS
    /// </summary>
    [Name("运动通过XRIS")]
    [Tip("通过XRIS控制运动", "Control locomotion through XRIS")]
    [RequireManager(typeof(XRSpaceSolutionManager))]
    public class LocomotionByXRIS : InteractProvider
    {
        /// <summary>
        /// 动作名
        /// </summary>
        public string _actionName = "";

        /// <summary>
        /// 动作名
        /// </summary>
        public string actionName { get => _actionName; set => _actionName = value; }

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

#if XDREAMER_XR_INTERACTION_TOOLKIT

        /// <summary>
        /// 基于动作的连续移动提供者
        /// </summary>
        [Name("基于动作的连续移动提供者")]
        public ActionBasedContinuousMoveProvider _actionBasedContinuousMoveProvider;

        /// <summary>
        /// 基于动作的连续转动提供者
        /// </summary>
        [Name("基于动作的连续转动提供者")]
        public ActionBasedContinuousTurnProvider _actionBasedContinuousTurnProvider;

        void SetLeft(InputAction inputAction)
        {
            if (_actionBasedContinuousMoveProvider)
            {
                if (inputAction == null)
                {
                    _actionBasedContinuousMoveProvider.leftHandMoveAction = new InputActionProperty(new InputAction());
                }
                else if (_actionBasedContinuousMoveProvider.leftHandMoveAction.action != inputAction)
                {
                    _actionBasedContinuousMoveProvider.leftHandMoveAction = new InputActionProperty(InputActionReference.Create(inputAction));
                }
            }
            if (_actionBasedContinuousTurnProvider)
            {
                if (inputAction == null)
                {
                    _actionBasedContinuousTurnProvider.leftHandTurnAction = new InputActionProperty(new InputAction());
                }
                else if (_actionBasedContinuousTurnProvider.leftHandTurnAction.action != inputAction)
                {
                    _actionBasedContinuousTurnProvider.leftHandTurnAction = new InputActionProperty(InputActionReference.Create(inputAction));
                }
            }
        }

        void SetRight(InputAction inputAction)
        {
            if (_actionBasedContinuousMoveProvider)
            {
                if (inputAction == null)
                {
                    _actionBasedContinuousMoveProvider.rightHandMoveAction = new InputActionProperty(new InputAction());
                }
                else if (_actionBasedContinuousMoveProvider.rightHandMoveAction.action != inputAction)
                {
                    _actionBasedContinuousMoveProvider.rightHandMoveAction = new InputActionProperty(InputActionReference.Create(inputAction));
                }
            }
            if (_actionBasedContinuousTurnProvider)
            {
                if (inputAction == null)
                {
                    _actionBasedContinuousTurnProvider.rightHandTurnAction = new InputActionProperty(new InputAction());
                }
                else if (_actionBasedContinuousTurnProvider.rightHandTurnAction.action != inputAction)
                {
                    _actionBasedContinuousTurnProvider.rightHandTurnAction = new InputActionProperty(InputActionReference.Create(inputAction));
                }
            }
        }

#endif

        private void OnXRAnswerReceived(XRAnswer answer)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            if (answer is XRSpaceConfigA spaceConfigA)
            {
                if (spaceConfigA.GetActionInfo(actionName) is ActionInfo actionInfo)
                {
                    var inputActionManager = GetComponentInParent<InputActionManager>();
                    if (actionInfo.buttonInteract.enable && inputActionManager)
                    {
                        //启用
                        _actionBasedContinuousMoveProvider.XSetEnable(true);
                        _actionBasedContinuousTurnProvider.XSetEnable(true);
                        var actionAssets = inputActionManager.actionAssets;

                        if (actionInfo.interactSelect.UnityInputActionEnable)//左
                        {
                            SetLeft(actionAssets.FindInputAction(actionInfo.interactSelect.buttonUnityInputActionConfig.actionName));
                        }
                        else
                        {
                            SetLeft(default);
                        }
                        if (actionInfo.interactActivate.UnityInputActionEnable)//右
                        {
                            SetRight(actionAssets.FindInputAction(actionInfo.interactActivate.buttonUnityInputActionConfig.actionName));
                        }
                        else
                        {
                            SetRight(default);
                        }
                    }
                    else
                    {
                        //禁用
                        _actionBasedContinuousMoveProvider.XSetEnable(false);
                        _actionBasedContinuousTurnProvider.XSetEnable(false);
                    }
                }
            }
#endif
        }
    }
}
