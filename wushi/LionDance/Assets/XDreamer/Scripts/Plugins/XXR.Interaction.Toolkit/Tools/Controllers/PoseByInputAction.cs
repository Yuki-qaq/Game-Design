using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginCameras.Tools.Base;
using UnityEngine.XR;

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
    /// 姿态通过输入动作
    /// </summary>
    [Name("姿态通过输入动作")]
    [Tip("通过Unity新版输入系统的输入动作模拟控制器姿态的输入输出", "Simulate the input and output of controller posture through the input actions of the Unity new version input system")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.TrackerPose, nameof(AnalogController))]
    [XCSJ.Attributes.Icon(EIcon.Target)]
    [DisallowMultipleComponent]
    public class PoseByInputAction : BaseAnalogProvider, IPoseProvider
    {
#if XDREAMER_INPUT_SYSTEM && XDREAMER_XR_INTERACTION_TOOLKIT

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
        /// 尝试获取姿态
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public InputTrackingState TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation)
        {
            var posAction = _positionAction.action;
            var rotAction = _rotationAction.action;
            var inputTrackingStateAction = _trackingStateAction.action;

            var hasPositionAction = posAction != null;
            var hasRotationAction = rotAction != null;

            position = default;
            rotation = default;

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
                position = pos;
            }

            // Update rotation
            if (hasRotationAction && (inputTrackingState & InputTrackingState.Rotation) != 0)
            {
                var rot = rotAction.ReadValue<Quaternion>();
                rotation = rot;
            }
            return inputTrackingState;
        }
#endif
    }
}
