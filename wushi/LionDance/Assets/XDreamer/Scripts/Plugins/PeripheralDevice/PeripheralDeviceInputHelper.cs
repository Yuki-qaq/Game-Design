using System.Collections.Generic;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace XCSJ.PluginPeripheralDevice
{
    /// <summary>
    /// 外部设备输入辅助类
    /// </summary>
    public static class PeripheralDeviceInputHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "外部设备输入";

        /// <summary>
        /// 标题XR
        /// </summary>
        public const string TitleXR = "外部设备输入-XR";

#if XDREAMER_INPUT_SYSTEM

        /// <summary>
        /// 查找输入动作
        /// </summary>
        /// <param name="actionAssets"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public static InputAction FindInputAction(this List<InputActionAsset> actionAssets, string actionName)
        {
            foreach (var asset in actionAssets)
            {
                if (asset.FindAction(actionName) is InputAction inputAction) return inputAction;
            }
            return default;
        }

        /// <summary>
        /// 是按压
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool IsPressed(this InputAction action)
        {
            if (action == null)
                return false;

            return action.phase == InputActionPhase.Performed;
        }

        /// <summary>
        /// 是按压
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool IsPressed(this InputActionProperty action) => IsPressed(action.action);

#endif
    }
}
