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

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.SpatialTracking;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 相机变换通过输入动作：使用输入动作的定位数据控制相机变换的姿态（位置与旋转）
    /// </summary>
    [Name("相机变换通过XR头盔设备")]
    [Tip("使用输入动作的定位数据控制相机变换的姿态（位置与旋转）", "Use the positioning data of input action to control the attitude (position and rotation) of the camera")]
    [Tool(CameraCategory.Component, nameof(CameraTransformer))]
    //[Tool(XRITHelper.Title)]
    [RequireManager(typeof(XXRInteractionToolkitManager), typeof(CameraManager))]
    [XCSJ.Attributes.Icon(EIcon.Camera)]
    [DisallowMultipleComponent]
    [Owner(typeof(XXRInteractionToolkitManager))]
    public class CameraTransformByInputAction : BaseCameraTransformController
    {
#if XDREAMER_INPUT_SYSTEM

        /// <summary>
        /// 位置动作
        /// </summary>
        [Name("位置动作")]
        public InputActionProperty _positionAction = new InputActionProperty();

        /// <summary>
        /// 位置动作
        /// </summary>
        public InputActionProperty positionAction
        {
            get => _positionAction;
            set => SetInputActionProperty(ref _positionAction, value);
        }

        /// <summary>
        /// 旋转动作
        /// </summary>
        [Name("旋转动作")]
        public InputActionProperty _rotationAction = new InputActionProperty();

        /// <summary>
        /// 旋转动作
        /// </summary>
        public InputActionProperty rotationAction
        {
            get => _rotationAction;
            set => SetInputActionProperty(ref _rotationAction, value);
        }

        /// <summary>
        /// 跟踪状态动作
        /// </summary>
        [Name("跟踪状态动作")]
        public InputActionProperty _trackingStateAction = new InputActionProperty();

        /// <summary>
        /// 跟踪状态动作
        /// </summary>
        public InputActionProperty trackingStateAction
        {
            get => _trackingStateAction;
            set => SetInputActionProperty(ref _trackingStateAction, value);
        }

        void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
        {
            var isPlaying = Application.isPlaying;
#if !UNITY_WEBGL
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
            _positionAction.EnableDirectAction();
            _rotationAction.EnableDirectAction();
            _trackingStateAction.EnableDirectAction();
#endif
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            
#if !UNITY_WEBGL
            _positionAction.DisableDirectAction();
            _rotationAction.DisableDirectAction();
            _trackingStateAction.DisableDirectAction();
#endif
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            var posAction = _positionAction.action;
            var rotAction = _rotationAction.action;
            var inputTrackingStateAction = _trackingStateAction.action;

            var hasPositionAction = posAction != null;
            var hasRotationAction = rotAction != null;

            // Update inputTrackingState
            InputTrackingState inputTrackingState;
            // Actions without bindings are considered empty and will fallback
            if (inputTrackingStateAction != null && inputTrackingStateAction.bindings.Count > 0)
            {
                inputTrackingState = (InputTrackingState)inputTrackingStateAction.ReadValue<int>();
            }
            else
            {
                // Fallback to the device trackingState if m_TrackingStateAction is not valid
                var positionTrackedDevice = hasPositionAction ? posAction.activeControl?.device as TrackedDevice : null;
                var rotationTrackedDevice = hasRotationAction ? rotAction.activeControl?.device as TrackedDevice : null;
                var positionTrackingState = InputTrackingState.None;

                if (positionTrackedDevice != null)
                    positionTrackingState = (InputTrackingState)positionTrackedDevice.trackingState.ReadValue();

                // If the tracking devices are different only the InputTrackingState.Position and InputTrackingState.Position flags will be considered
                if (positionTrackedDevice != rotationTrackedDevice)
                {
                    var rotationTrackingState = InputTrackingState.None;
                    if (rotationTrackedDevice != null)
                        rotationTrackingState = (InputTrackingState)rotationTrackedDevice.trackingState.ReadValue();

                    positionTrackingState &= InputTrackingState.Position;
                    rotationTrackingState &= InputTrackingState.Rotation;
                    inputTrackingState = positionTrackingState | rotationTrackingState;
                }
                else
                {
                    inputTrackingState = positionTrackingState;
                }
            }

            // Update position
            if (hasPositionAction && (inputTrackingState & InputTrackingState.Position) != 0)
            {
                var pos = posAction.ReadValue<Vector3>();
                cameraTransformer.localPosition = pos;
            }

            // Update rotation
            if (hasRotationAction && (inputTrackingState & InputTrackingState.Rotation) != 0)
            {
                var rot = rotAction.ReadValue<Quaternion>();
                cameraTransformer.localRotation = rot;
            }
        }

#endif
        }
    }
