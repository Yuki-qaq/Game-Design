using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;

#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Base
{
    /// <summary>
    /// 输入设备助手
    /// </summary>
    public static class InputDeviceHelper
    {
#if XDREAMER_XR_INTERACTION_TOOLKIT

        /// <summary>
        /// 转按钮
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static InputHelpers.Button ToButton(this EXRButton button) => (InputHelpers.Button)button;

        /// <summary>
        /// 是按压
        /// </summary>
        /// <param name="device"></param>
        /// <param name="button"></param>
        /// <param name="isPressed"></param>
        /// <param name="pressThreshold"></param>
        /// <returns></returns>
        public static bool IsPressed(this EXRDeviceType device, EXRButton button, out bool isPressed, float pressThreshold = -1)
        {
            return device.GetDevice().IsPressed(button.ToButton(), out isPressed, pressThreshold);
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="device"></param>
        /// <param name="button"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValue(this EXRDeviceType device, EXRButton button, out float value)
        {
            return device.GetDevice().TryReadSingleValue(button.ToButton(), out value);
        }

        /// <summary>
        /// 获取设备类型
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public static InputDevice GetDevice(this EXRDeviceType deviceType)
        {
            switch (deviceType)
            {
                case EXRDeviceType.HMD: return InputDevices.GetDeviceAtXRNode(XRNode.Head);
                case EXRDeviceType.Left: return InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                case EXRDeviceType.Right: return InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            }
            return default;
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// 获取头盔的位置与旋转
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static InputTrackingState TryGetPose(EXRDeviceType deviceType, out Vector3 position, out Quaternion rotation)
        {
            var inputTrackingState = InputTrackingState.None;

            switch (deviceType)
            {
                case EXRDeviceType.HMD:
                    {
                        var dev = deviceType.GetDevice();
                        if (dev.isValid)
                        {
                            if (dev.TryGetFeatureValue(CommonUsages.centerEyePosition, out position))
                            {
                                inputTrackingState |= InputTrackingState.Position;
                            }
                            if (dev.TryGetFeatureValue(CommonUsages.centerEyeRotation, out rotation))
                            {
                                inputTrackingState |= InputTrackingState.Rotation;
                            }
                            return inputTrackingState;
                        }
                        break;
                    }
                case EXRDeviceType.Left:
                case EXRDeviceType.Right:
                    {
                        var dev = deviceType.GetDevice();
                        if (dev.isValid)
                        {
                            if (dev.TryGetFeatureValue(CommonUsages.devicePosition, out position))
                            {
                                inputTrackingState |= InputTrackingState.Position;
                            }
                            if (dev.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
                            {
                                inputTrackingState |= InputTrackingState.Rotation;
                            }
                            return inputTrackingState;
                        }
                        break;
                    }
            }
            position = default;
            rotation = default;
            return inputTrackingState;
        }

#else

        /// <summary>
        /// 获取头盔的位置与旋转
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static PoseDataFlags TryGetPose(EXRDeviceType deviceType, out Vector3 position, out Quaternion rotation)
        {
            var poseDataFlag = PoseDataFlags.NoData;

            switch (deviceType)
            {
                case EXRDeviceType.HMD:
                    {
                        var dev = deviceType.GetDevice();
                        if (dev.isValid)
                        {
                            if (dev.TryGetFeatureValue(CommonUsages.centerEyePosition, out position))
                            {
                                poseDataFlag |= PoseDataFlags.Position;
                            }
                            if (dev.TryGetFeatureValue(CommonUsages.centerEyeRotation, out rotation))
                            {
                                poseDataFlag |= PoseDataFlags.Rotation;
                            }
                            return poseDataFlag;
                        }
                        break;
                    }
                case EXRDeviceType.Left:
                case EXRDeviceType.Right:
                    {
                        var dev = deviceType.GetDevice();
                        if (dev.isValid)
                        {
                            if (dev.TryGetFeatureValue(CommonUsages.devicePosition, out position))
                            {
                                poseDataFlag |= PoseDataFlags.Position;
                            }
                            if (dev.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
                            {
                                poseDataFlag |= PoseDataFlags.Rotation;
                            }
                            return poseDataFlag;
                        }
                        break;
                    }
            }
            position = default;
            rotation = default;
            return poseDataFlag;
        }
#endif

#endif
    }

    /// <summary>
    /// 手柄规则
    /// </summary>
    [Name("手柄规则")]
    public enum EHandRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 左
        /// </summary>
        [Name("左")]
        Left,

        /// <summary>
        /// 右
        /// </summary>
        [Name("右")]
        Right,

        /// <summary>
        /// 任意
        /// </summary>
        [Name("任意")]
        [Tip("左或右任意一个", "Either left or right")]
        Any,

        /// <summary>
        /// 左和右
        /// </summary>
        [Name("左和右")]
        [Tip("左和右同时", "Left and right at the same time")]
        All,
    }
}
