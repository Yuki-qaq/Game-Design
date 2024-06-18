using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginART;
using XCSJ.PluginART.Tools;
using XCSJ.PluginCameras;
using XCSJ.PluginCameras.Controllers;
using XCSJ.PluginCameras.Tools.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginOptiTrack;
using XCSJ.PluginOptiTrack.Tools;
using XCSJ.PluginPeripheralDevice;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginZVR;
using XCSJ.PluginZVR.Tools;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#endif

namespace XCSJ.PluginXRSpaceSolution.Tools
{
    /// <summary>
    /// 相机姿态通过XRIS
    /// </summary>
    [Name("相机姿态通过XRIS")]
    [Tip("通过XR网络控制相机的姿态（移动与旋转）", "Control camera pose (movement and rotation) through XRIS")]
    [RequireManager(typeof(XRSpaceSolutionManager), typeof(XXRInteractionToolkitManager))]
    [Tool(CameraCategory.Component, nameof(CameraTransformer))]
    //[Tool(XRSpaceSolutionHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.Position)]
    [DisallowMultipleComponent]
    [Owner(typeof(XRSpaceSolutionManager))]
    public class CameraPoseByXRIS : BaseCameraTransformController
    {
        /// <summary>
        /// 姿态处理
        /// </summary>
        [Name("姿态处理")]
        [OnlyMemberElements]
        public PoseHandler _poseHandler = new PoseHandler();

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

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (_poseHandler.TryGetPose(out var position, out var rotation))
            {
                var mainTransform = cameraTransformer.mainTransform;
                mainTransform.localPosition = position;
                mainTransform.localRotation = rotation;
            }
        }

        ARTStreamClient _ARTStreamClient;

#if XDREAMER_OPTITRACK

        OptitrackStreamingClient _OptitrackStreamingClient;
#endif

#if XDREAMER_ZVR

        ZvrGokuStreamClient _ZvrGokuStreamClient;
#endif

        void DisableCameraTransformART()
        {
            var cameraTransformByART = this.GetComponent<CameraTransformByART>();
            if (cameraTransformByART)
            {
                cameraTransformByART.enabled = false;
            }
        }

        void DisableCameraTransformOptiTrack()
        {
            var cameraTransformByOptiTrack = this.GetComponent<CameraTransformByOptiTrack>();
            if (cameraTransformByOptiTrack)
            {
                cameraTransformByOptiTrack.enabled = false;
            }
        }

        void DisableCameraTransformZVR()
        {
            var cameraTransformByZVR = this.GetComponent<CameraTransformByZVR>();
            if (cameraTransformByZVR)
            {
                cameraTransformByZVR.enabled = false;
            }
        }

        void DisableCameraTransformInputAction()
        {
            var cameraTransformByInputAction = this.GetComponent<CameraTransformByInputAction>();
            if (cameraTransformByInputAction)
            {
                cameraTransformByInputAction.enabled = false;
            }
        }

        void DisableCameraTransformXRDevice()
        {
            var cameraTransformByXRDevice = this.GetComponent<CameraTransformByXRDevice>();
            if (cameraTransformByXRDevice)
            {
                cameraTransformByXRDevice.enabled = false;
            }
        }

        void DisableAllCameraTransform()
        {
            DisableCameraTransformXRDevice();
            DisableCameraTransformInputAction();
            DisableCameraTransformART();
            DisableCameraTransformOptiTrack();
            DisableCameraTransformZVR();
        }

        private void OnXRAnswerReceived(XRAnswer answer)
        {
            if (!_poseHandler.Handle(answer)) return;

            if (_poseHandler.pose.enable)
            {
                DisableAllCameraTransform();
                return;
            }
            if (_poseHandler.pose.poseUnityXRDeviceConfig is PoseUnityXRDeviceConfig poseUnityXRDeviceConfig && poseUnityXRDeviceConfig.enable)
            {
                var cameraTransformByXRDevice = this.XGetOrAddComponent<CameraTransformByXRDevice>();
                if (cameraTransformByXRDevice)
                {
                    cameraTransformByXRDevice._deviceType = poseUnityXRDeviceConfig.deviceType;
                    cameraTransformByXRDevice.enabled = true;//确保启用
                }

                DisableCameraTransformInputAction();
                DisableCameraTransformART();
                DisableCameraTransformOptiTrack();
                DisableCameraTransformZVR();
                return;
            }
            if (_poseHandler.pose.poseUnityInputActionConfig is PoseUnityInputActionConfig poseUnityInputActionConfig && poseUnityInputActionConfig.enable)
            {
                var cameraTransformByInputAction = this.XGetOrAddComponent<CameraTransformByInputAction>();
                if (cameraTransformByInputAction)
                {
#if XDREAMER_XR_INTERACTION_TOOLKIT
                    var inputActionManager = GetComponentInParent<InputActionManager>();
                    if (inputActionManager)
                    {
                        var actionAssets = inputActionManager.actionAssets;
                        if (actionAssets.FindInputAction(poseUnityInputActionConfig.positionActionName) is InputAction positionAction)
                        {
                            if (cameraTransformByInputAction.positionAction.action != positionAction)
                            {
                                cameraTransformByInputAction.positionAction = new InputActionProperty(InputActionReference.Create(positionAction));
                            }
                        }
                        else
                        {
                            cameraTransformByInputAction.positionAction = new InputActionProperty(new InputAction());
                        }
                        if (actionAssets.FindInputAction(poseUnityInputActionConfig.rotationActionName) is InputAction rotationAction)
                        {
                            if (cameraTransformByInputAction.rotationAction.action != rotationAction)
                            {
                                cameraTransformByInputAction.rotationAction = new InputActionProperty(InputActionReference.Create(rotationAction));
                            }
                        }
                        else
                        {
                            cameraTransformByInputAction.rotationAction = new InputActionProperty(new InputAction());
                        }
                        if (actionAssets.FindInputAction(poseUnityInputActionConfig.trackingStateActionName) is InputAction trackingStateAction)
                        {
                            if (cameraTransformByInputAction.trackingStateAction.action != trackingStateAction)
                            {
                                cameraTransformByInputAction.trackingStateAction = new InputActionProperty(InputActionReference.Create(trackingStateAction));
                            }
                        }
                        else
                        { 
                            cameraTransformByInputAction.trackingStateAction = new InputActionProperty(new InputAction());
                        }
                    }
#endif
                    cameraTransformByInputAction.enabled = true;//确保启用
                }

                DisableCameraTransformXRDevice();
                DisableCameraTransformART();
                DisableCameraTransformOptiTrack();
                DisableCameraTransformZVR();
                return;
            }
            if (_poseHandler.pose.poseARTConfig is PoseARTConfig poseARTConfig && poseARTConfig.enable)
            {
                ARTManager.HandleClient(ref _ARTStreamClient, poseARTConfig.clientConfig, client =>
                {
                    if (poseARTConfig.useSrcSC) client._coordinateSystem = _poseHandler.srcCS.coordinateSystem;
                    var cameraTransformByART = this.XGetOrAddComponent<CameraTransformByART>();
                    if (cameraTransformByART)
                    {
                        cameraTransformByART._streamClient = client;
                        cameraTransformByART.dataType = poseARTConfig.dataType;
                        cameraTransformByART.rigidBodyID = poseARTConfig.rigidBodyID;
                        cameraTransformByART.enabled = true;//确保启用
                    }
                });

                DisableCameraTransformXRDevice();
                DisableCameraTransformInputAction();
                DisableCameraTransformOptiTrack();
                DisableCameraTransformZVR();
                return;
            }
            if (_poseHandler.pose.poseOptiTrackConfig is PoseOptiTrackConfig poseOptiTrackConfig && poseOptiTrackConfig.enable)
            {
#if XDREAMER_OPTITRACK
                OptiTrackManager.HandleClient(ref _OptitrackStreamingClient, poseOptiTrackConfig.clientConfig, client =>
                {
                    var cameraTransformByOptiTrack = this.XGetOrAddComponent<CameraTransformByOptiTrack>();
                    if (cameraTransformByOptiTrack)
                    {
                        cameraTransformByOptiTrack._streamingClient = client;
                        cameraTransformByOptiTrack.rigidBodyID = poseOptiTrackConfig.rigidBodyID;
                        cameraTransformByOptiTrack.enabled = true;//确保启用
                    }
                });
#endif
                DisableCameraTransformXRDevice();
                DisableCameraTransformInputAction();
                DisableCameraTransformART();
                DisableCameraTransformZVR();
                return;
            }
            if (_poseHandler.pose.poseZVRConfig is PoseZVRConfig poseZVRConfig && poseZVRConfig.enable)
            {
#if XDREAMER_ZVR
                ZVRManager.HandleClient(ref _ZvrGokuStreamClient, poseZVRConfig.clientConfig, client =>
                {
                    var cameraTransformByZVR = this.XGetOrAddComponent<CameraTransformByZVR>();
                    if (cameraTransformByZVR)
                    {
                        cameraTransformByZVR._streamClient = client;
                        cameraTransformByZVR.rigidBodyID = poseZVRConfig.rigidBodyID;
                        cameraTransformByZVR.enabled = true;//确保启用
                    }
                });
#endif
                DisableCameraTransformXRDevice();
                DisableCameraTransformInputAction();
                DisableCameraTransformART();
                DisableCameraTransformOptiTrack();
                return;
            }

            DisableAllCameraTransform();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _poseHandler.actionName = CommonFun.Name(EActionName.HMD); ;
        }
    }
}
