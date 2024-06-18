using System;
using System.Collections;
using XCSJ.PluginCommonUtils;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.Attributes;
#if UNITY_2018_1_OR_NEWER
using UnityEngine.XR;
#endif

namespace XCSJ.Extension.Base.Helpers
{
    /// <summary>
    /// XR设备旧版:适用于Unity2019（含）之前版本的针对XR操作与处理；
    /// </summary>
    [Name("XR设备旧版")]
    public enum EXRDeviceTypeLegacy
    {
        /// <summary>
        /// None
        /// </summary>
        [Name("None")]
        None = 0,

        /// <summary>
        /// Oculus
        /// </summary>
        [Name("Oculus")]
        Oculus,

        /// <summary>
        /// OpenVR
        /// </summary>
        [Name("OpenVR")]
        OpenVR,

        /// <summary>
        /// MockHMD
        /// </summary>
        [Name("MockHMD")]
        [Tip("Mock HMD", "Mock HMD")]
        Mock_HMD,

        /// <summary>
        /// cardboard
        /// </summary>
        [Name("cardboard")]
        Cardboard,

        /// <summary>
        /// stereo
        /// </summary>
        [Name("stereo")]
        [Tip("Stereo Display (non head-mounted)", "Stereo Display (non head-mounted)")]
        Stereo_Display_Non_Head_Mounted,
    }

    /// <summary>
    /// XR处理器
    /// </summary>
    public class XRHelper
    {
        /// <summary>
        /// VR启用
        /// </summary>
        public static bool VREnable
        {
            get
            {
#if UNITY_2018_1_OR_NEWER
                return XRSettings.enabled && !(string.IsNullOrEmpty(XRSettings.loadedDeviceName) || XRSettings.loadedDeviceName == "None");
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// 设备右下
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static bool DeviceValid(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName)) return false;
#if UNITY_2018_1_OR_NEWER
           // Debug.Log("XRSettings.enabled:"+ XRSettings.enabled);
           // Debug.Log("deviceName:" + deviceName);
           // Debug.Log("XRSettings.supportedDevices,:" + XRSettings.supportedDevices.ToStringDirect());
           //Debug.Log("Array.IndexOf(XRSettings.supportedDevices, deviceName):" + Array.IndexOf(XRSettings.supportedDevices, deviceName));
            return /*XRSettings.enabled && */Array.IndexOf(XRSettings.supportedDevices, deviceName) >= 0;
#else
            return true;
#endif
        }

        /// <summary>
        /// 切换设备
        /// </summary>
        /// <param name="device"></param>
        /// <param name="on"></param>
        public static void SwitchDevice(EXRDeviceTypeLegacy device, bool on = true)
        {            
            SwitchDevice(CommonFun.Name(device), on);
        }

        /// <summary>
        /// 切换设备
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="on"></param>
        public static void SwitchDevice(string deviceName, bool on = true)
        {
            if (!DeviceValid(deviceName)) return;
#if UNITY_2018_1_OR_NEWER
            if (XRSettings.loadedDeviceName == deviceName)
            {
                if (on || !VREnable) return;
                deviceName = "None";
            }
            else
            {
                if (!on) return;
            }
#endif
            DeviceOn(deviceName);
        }

        /// <summary>
        /// 设备开如果处于关
        /// </summary>
        /// <param name="deviceName"></param>
        public static void DeviceOnIfOff(string deviceName)
        {
            if (DeviceValid(deviceName) && XRSettings.loadedDeviceName != deviceName)
            {
                DeviceOn(deviceName);
            }
        }

        private static void DeviceOn(string deviceName)
        {
            GlobalMB.instance.StartCoroutine(LoadDevice(deviceName));
        }

        private static IEnumerator LoadDevice(string newDevice)
        {
#if UNITY_2018_1_OR_NEWER
            XRSettings.LoadDeviceByName(newDevice);
            yield return null;
            XRSettings.enabled = true;
#endif
        }
    }
}
