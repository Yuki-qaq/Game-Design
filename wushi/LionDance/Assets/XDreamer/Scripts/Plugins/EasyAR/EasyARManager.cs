using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using System.Runtime.InteropServices;
using XCSJ.Attributes;
using XCSJ.Algorithms;
#if XDREAMER_EASYAR_4_1_0
using easyar;
#endif

namespace XCSJ.PluginEasyAR
{
    /// <summary>
    /// EasyAR:用于与第三方插件EasyAR对接的管理器插件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [Name(EasyARHelper.Title)]
    [Tip("用于与第三方插件EasyAR对接的管理器插件", "Manager plug-in for interfacing with third-party plug-in EasyAR")]
    [Guid("95F33829-4331-4C07-91B8-AFFEDF1CBD28")]
    [ComponentOption(EComponentOption.Optional)]
    [Version("24.129")]
    [Index(index = IndexAttribute.DefaultIndex + 20)]
    public class EasyARManager : BaseManager<EasyARManager>
    {
        /// <summary>
        /// EasyAR组件
        /// </summary>
        [Name("EasyAR组件")]
        [Tip("EasyAR的根节点核心组件", "The root node core component of EasyAR")]
#if XDREAMER_EASYAR_4_1_0
        public EasyARController easyAR;
#else
        public Component easyAR;
#endif

        /// <summary>
        /// 相机设备
        /// </summary>
        [Name("相机设备")]
        [Tip("EasyAR的相机设备组件", "EasyAR camera device components")]
#if XDREAMER_EASYAR_4_1_0
        public VideoCameraDevice cameraDevice;
#else
        public Component cameraDevice;
#endif

#if XDREAMER_EASYAR_4_1_0
        private ARSession arSession;
#else
        private Component arSession;
#endif

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts() => Script.GetScriptsOfEnum<EEasyARScriptID>(this);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
#if XDREAMER_EASYAR_4_1_0
            switch ((EEasyARScriptID)id)
                {
                    case EEasyARScriptID.EasyARCameraDeviceOpenAndStart:
                        {
                            var cameraDevice = GetCameraDevice(param[0] as VideoCameraDevice);
                            if (!cameraDevice) break;
                            cameraDevice.Open();
                            return ReturnValue.Yes;
                        }
                    case EEasyARScriptID.EasyARCameraDeviceClose:
                        {
                            var cameraDevice = GetCameraDevice(param[0] as VideoCameraDevice);
                            if (!cameraDevice) break;
                            cameraDevice.Close();
                            return ReturnValue.Yes;
                        }
                    case EEasyARScriptID.EasyARCameraDeviceStartCapture:
                        {
                            var arSession = GetARSession(param[0] as ARSession);
                            if (!arSession) break;
                            var trackers = arSession.GetComponentsInChildren<FrameFilter>(true);
                            foreach(var tracker in trackers) tracker.enabled = true;
                            return ReturnValue.Yes; 
                        }
                    case EEasyARScriptID.EasyARCameraDeviceStopCapture:
                        {
                            var arSession = GetARSession(param[0] as ARSession);
                            if (!arSession) break;
                            var trackers = arSession.GetComponentsInChildren<FrameFilter>(true);
                            foreach (var tracker in trackers) tracker.enabled = false;
                            return ReturnValue.Yes;
                        }
                    case EEasyARScriptID.EasyARSwitchCameraDeviceType:
                        {
                            var cameraDevice = GetCameraDevice(param[0] as VideoCameraDevice);
                            if (!cameraDevice) break;

                            cameraDevice.CameraOpenMethod = VideoCameraDevice.CameraDeviceOpenMethod.DeviceType;
                            CameraDeviceType devicetype = cameraDevice.CameraType;
                            switch (param[1] as string)
                            {
                                case "前置摄像头":
                                    {
                                        devicetype = CameraDeviceType.Front;
                                        break;
                                    }
                                case "后置摄像头":
                                    {
                                        devicetype = CameraDeviceType.Back;
                                        break;
                                    }
                                case "切换":
                                    {
                                        switch (cameraDevice.CameraType)
                                        {
                                            case CameraDeviceType.Front:
                                                {
                                                    devicetype = CameraDeviceType.Back;
                                                    break;
                                                }
                                            case CameraDeviceType.Back:
                                            case CameraDeviceType.Unknown:
                                            default:
                                                {
                                                    devicetype = CameraDeviceType.Front;
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                            }

                            if (cameraDevice.CameraType == devicetype) break;

                            cameraDevice.CameraType = devicetype;

                            cameraDevice.Close();
                            cameraDevice.Open();
                            return ReturnValue.Yes;
                        }
                    default: break;
                }
#endif
            return ReturnValue.No;
        }

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
#if XDREAMER_EASYAR_4_1_0
            easyAR = InitEasyAR(easyAR);

            cameraDevice = cameraDevice ? cameraDevice : FindObjectOfType<VideoCameraDevice>();

            arSession = arSession ? arSession: FindObjectOfType<ARSession>();
#if !UNITY_EDITOR
            if (!easyAR)
            {
                Log.DebugFormat("未找到: {0} 组件!", typeof(EasyARController));
            }
#endif

#endif
        }

#if XDREAMER_EASYAR_4_1_0
        public VideoCameraDevice GetCameraDevice(VideoCameraDevice cameraDevice)
        {
            return cameraDevice ? cameraDevice : this.cameraDevice;
        }

        public ARSession GetARSession(ARSession arSession)
        {
            return arSession ? arSession : this.arSession;
        }

        public static EasyARController InitEasyAR(EasyARController easyAR)
        {
            return easyAR ? easyAR : ((instance && instance.easyAR) ? instance.easyAR : FindObjectOfType<EasyARController>());
        }

        public static VideoCameraDevice InitCameraDevice(VideoCameraDevice cameraDevice)
        {
            return cameraDevice ? cameraDevice : ((instance && instance.cameraDevice) ? instance.cameraDevice : FindObjectOfType<VideoCameraDevice>());
        }
#else

        /// <summary>
        /// 获取相机设备
        /// </summary>
        /// <param name="cameraDevice"></param>
        /// <returns></returns>
        public MonoBehaviour GetCameraDevice(MonoBehaviour cameraDevice) => null;

        /// <summary>
        /// 初始化EasyAR
        /// </summary>
        /// <param name="easyAR"></param>
        /// <returns></returns>
        public static MonoBehaviour InitEasyAR(MonoBehaviour easyAR) => null;

        /// <summary>
        /// 初始化相机设备
        /// </summary>
        /// <param name="cameraDevice"></param>
        /// <returns></returns>
        public static MonoBehaviour InitCameraDevice(MonoBehaviour cameraDevice) => null;

#endif
    }
}
