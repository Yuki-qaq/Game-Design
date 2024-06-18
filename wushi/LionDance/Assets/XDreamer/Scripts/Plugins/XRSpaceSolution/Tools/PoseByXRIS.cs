using System;
using System.Linq;
using System.Collections.Generic;
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
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.Extension.Base.UVRPN.Core;
using XCSJ.PluginART.Tools;
using XCSJ.PluginART;
using XCSJ.PluginOptiTrack;
using XCSJ.PluginOptiTrack.Tools;
using XCSJ.PluginZVR;
using XCSJ.PluginZVR.Tools;
using XCSJ.PluginPeripheralDevice;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#endif

namespace XCSJ.PluginXRSpaceSolution.Tools
{
    /// <summary>
    /// 姿态通过XRIS
    /// </summary>
    [Name("姿态通过XRIS")]
    [Tip("通过XRIS模拟控制器姿态的输入输出", "Analog input and output of controller pose through XRIS")]
    [RequireManager(typeof(XRSpaceSolutionManager), typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.TrackerPose, nameof(AnalogController))]
    //[Tool(XRSpaceSolutionHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AnalogController))]
    public class PoseByXRIS : BaseAnalogProvider, IPoseProvider
    {
        /// <summary>
        /// 姿态
        /// </summary>
        [Name("姿态处理")]
        [OnlyMemberElements]
        public PoseHandler _poseHandler = new PoseHandler();

        /// <summary>
        /// 动作名
        /// </summary>
        public override string actionName { get => _poseHandler.actionName; set => _poseHandler.actionName = value; }

        /// <summary>
        /// 当启用时处理交互
        /// </summary>
        [Name("当启用时处理交互")]
        [EnumPopup]
        public EHandleInteractOnEnable _handleInteractOnEnable = EHandleInteractOnEnable.AddAndEnabled;

        /// <summary>
        /// 当启用时处理交互
        /// </summary>
        public enum EHandleInteractOnEnable
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 添加并启用
            /// </summary>
            [Name("添加并启用")]
            AddAndEnabled,
        }

        /// <summary>
        /// 当禁用时处理交互
        /// </summary>
        [Name("当禁用时处理交互")]
        [EnumPopup]
        public EHandleInteractOnDisable _handleInteractOnDisable = EHandleInteractOnDisable.DisableAdded;

        /// <summary>
        /// 当当禁用时处理交互
        /// </summary>
        public enum EHandleInteractOnDisable
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 禁用已添加的
            /// </summary>
            [Name("禁用已添加的")]
            DisableAdded,
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            XDreamerEvents.onXRAnswerReceived += OnXRAnswerReceived;
            if (_handleInteractOnEnable == EHandleInteractOnEnable.AddAndEnabled)
            {
                var analogController = this.analogController;
                if (analogController)
                {
                    foreach (var an in analogController._interactActionNames)
                    {
                        Add(an);
                    }
                    foreach (var an in analogController._customActionNames)
                    {
                        Add(an);
                    }
                }
            }            
        }

        InteractByXRIS Add(string interactActionName)
        {
            if (string.IsNullOrEmpty(interactActionName)) return default;
            if (interacts.TryGetValue(interactActionName,out var interact) && interact)
            {
                interact.XSetEnable(true);
                return interact;
            }
            interact = this.GetComponents<InteractByXRIS>().FirstOrDefault(i => i.actionName == interactActionName);
            if (interact) return interact;

            interact = this.XAddComponent<InteractByXRIS>();
            interact.actionName = interactActionName;
            interacts[interactActionName] = interact;
            interact.Regist();
            return interact;
        }

        Dictionary<string, InteractByXRIS> interacts = new Dictionary<string, InteractByXRIS>();

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            XDreamerEvents.onXRAnswerReceived -= OnXRAnswerReceived;

            if (_handleInteractOnDisable == EHandleInteractOnDisable.DisableAdded)
            {
                foreach (var i in interacts)
                {
                    i.Value.XSetEnable(false);
                }
            }               
        }

        void SetAnalogControllerPoseProvider(BaseAnalogProvider poseProvider)
        {
            if(poseProvider && poseProvider is IPoseProvider)
            {
                var ac = analogController;
                if (ac)
                {
                    ac._poseProvider = poseProvider;
                }
            }           
        }

        ARTStreamClient _ARTStreamClient;

#if XDREAMER_OPTITRACK

        OptitrackStreamingClient _OptitrackStreamingClient;
#endif

#if XDREAMER_ZVR

        ZvrGokuStreamClient _ZvrGokuStreamClient;
#endif

        private void OnXRAnswerReceived(XRAnswer answer)
        {
            if (_poseHandler.Handle(answer))
            {
                if (_poseHandler.pose.enable)
                {
                    SetAnalogControllerPoseProvider(this);
                }
                else if (_poseHandler.pose.poseUnityXRDeviceConfig is PoseUnityXRDeviceConfig poseUnityXRDeviceConfig && poseUnityXRDeviceConfig.enable)
                {

                    var pose = this.XGetOrAddComponent<PoseByXRDevice>();
                    if (pose)
                    {
                        pose.actionName = this.actionName;
                        SetAnalogControllerPoseProvider(pose);
                        pose._deviceType = poseUnityXRDeviceConfig.deviceType;
                        pose.enabled = true;//确保启用
                    }
                }
                else if (_poseHandler.pose.poseUnityInputActionConfig is PoseUnityInputActionConfig poseUnityInputActionConfig && poseUnityInputActionConfig.enable)
                {
                    var pose = this.XGetOrAddComponent<PoseByInputAction>();
                    if (pose)
                    {
                        pose.actionName = this.actionName;
                        SetAnalogControllerPoseProvider(pose);

#if XDREAMER_XR_INTERACTION_TOOLKIT
                        var inputActionManager = GetComponentInParent<InputActionManager>();
                        if (inputActionManager)
                        {
                            var actionAssets = inputActionManager.actionAssets;
                            if (actionAssets.FindInputAction(poseUnityInputActionConfig.positionActionName) is InputAction positionAction)
                            {
                                if (pose.positionAction.action != positionAction)
                                {
                                    pose.positionAction = new InputActionProperty(InputActionReference.Create(positionAction));
                                }
                            }
                            else
                            {
                                pose.positionAction = new InputActionProperty(new InputAction());
                            }
                            if (actionAssets.FindInputAction(poseUnityInputActionConfig.rotationActionName) is InputAction rotationAction)
                            {
                                if (pose.rotationAction.action != rotationAction)
                                {
                                    pose.rotationAction = new InputActionProperty(InputActionReference.Create(rotationAction));
                                }
                            }
                            else
                            {
                                pose.rotationAction = new InputActionProperty(new InputAction());
                            }
                            if (actionAssets.FindInputAction(poseUnityInputActionConfig.trackingStateActionName) is InputAction trackingStateAction)
                            {
                                if (pose.trackingStateAction.action != trackingStateAction)
                                {
                                    pose.trackingStateAction = new InputActionProperty(InputActionReference.Create(trackingStateAction));
                                }
                            }
                            else
                            {
                                pose.trackingStateAction = new InputActionProperty(new InputAction());
                            }
                        }
#endif
                        pose.enabled = true;//确保启用
                    }
                }
                else if (_poseHandler.pose.poseARTConfig is PoseARTConfig poseARTConfig && poseARTConfig.enable)
                {
                    ARTManager.HandleClient(ref _ARTStreamClient, poseARTConfig.clientConfig, client =>
                    {
                        if (poseARTConfig.useSrcSC) client._coordinateSystem = _poseHandler.srcCS.coordinateSystem;
                        var pose = this.XGetOrAddComponent<PoseByART>();
                        if (pose)
                        {
                            pose.actionName = this.actionName;
                            SetAnalogControllerPoseProvider(pose);

                            pose._streamClient = client;
                            pose.dataType = poseARTConfig.dataType;
                            pose.rigidBodyID = poseARTConfig.rigidBodyID;
                            pose.enabled = true;//确保启用
                        }
                    });
                }
                else if (_poseHandler.pose.poseOptiTrackConfig is PoseOptiTrackConfig poseOptiTrackConfig && poseOptiTrackConfig.enable)
                {
#if XDREAMER_OPTITRACK
                    OptiTrackManager.HandleClient(ref _OptitrackStreamingClient, poseOptiTrackConfig.clientConfig, client =>
                    {
                        var pose = this.XGetOrAddComponent<PoseByOptiTrack>();
                        if (pose)
                        {
                            pose.actionName = this.actionName;
                            SetAnalogControllerPoseProvider(pose);

                            pose._streamingClient = client;
                            pose.rigidBodyID = poseOptiTrackConfig.rigidBodyID;
                            pose.enabled = true;//确保启用
                        }
                    });
#endif
                }
                else if (_poseHandler.pose.poseZVRConfig is PoseZVRConfig poseZVRConfig && poseZVRConfig.enable)
                {
#if XDREAMER_ZVR
                    ZVRManager.HandleClient(ref _ZvrGokuStreamClient, poseZVRConfig.clientConfig, client =>
                    {
                        var pose = this.XGetOrAddComponent<PoseByZVR>();
                        if (pose)
                        {
                            pose.actionName = this.actionName;
                            SetAnalogControllerPoseProvider(pose);

                            pose._streamingClient = client;
                            pose.rigidBodyID = poseZVRConfig.rigidBodyID;
                            pose.enabled = true;//确保启用
                        }
                    });
#endif
                }
                else
                {
                    SetAnalogControllerPoseProvider(this);
                }
            }
        }

        /// <summary>
        /// 尝试获取姿态
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public InputTrackingState TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation) => _poseHandler.TryGetPose(out position, out rotation) ? InputTrackingState.Position | InputTrackingState.Rotation : InputTrackingState.None;
    }
}
