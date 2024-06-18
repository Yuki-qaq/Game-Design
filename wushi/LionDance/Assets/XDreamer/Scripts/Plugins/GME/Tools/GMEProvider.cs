using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using XCSJ.PluginGME.CNScripts;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Extension.Base.Extensions;
using System.Linq;

#if XDREAMER_GME
using GME;
#endif

namespace XCSJ.PluginGME.Tools
{
    /// <summary>
    /// GME提供者
    /// </summary>
    [Name("GME提供者")]
    [Owner(typeof(GMEManager))]
    [RequireManager(typeof(GMEManager))]
    [RequireComponent(typeof(GMEManager))]
    public class GMEProvider : Interactor
    {
        static GMEProvider _instance;

        /// <summary>
        /// 实例
        /// </summary>
        public static GMEProvider instance
        {
            get
            {
                if (!_instance)
                {
                    //1、从GME管理器上找
                    var m = GMEManager.instance;
                    if (m) _instance = m.XGetOrAddComponent<GMEProvider>();

                    //2、从全局找
                    if (!_instance) _instance = CommonFun.GetComponentsInChildren<GMEProvider>(true).FirstOrDefault();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 管理器
        /// </summary>
        [Name("管理器")]
        [HideInSuperInspector]
        public GMEManager _manager;

        /// <summary>
        /// 管理器
        /// </summary>
        public GMEManager manager => this.XGetComponent(ref _manager);

        #region Unity 消息

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

#if XDREAMER_GME
            ITMGContext.GetInstance().OnEnterRoomCompleteEvent += OnEnterRoomComplete;
            ITMGContext.GetInstance().OnExitRoomCompleteEvent += OnExitRoomComplete;
            ITMGContext.GetInstance().OnRoomDisconnectEvent += OnRoomDisconnect;
            ITMGContext.GetInstance().OnEndpointsUpdateInfoEvent += OnEndpointsUpdateInfo;
#endif

            InitGME();
            EnterRoom();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            ExitRoom();
            UninitGME();

#if XDREAMER_GME
            ITMGContext.GetInstance().OnEnterRoomCompleteEvent -= OnEnterRoomComplete;
            ITMGContext.GetInstance().OnExitRoomCompleteEvent -= OnExitRoomComplete;
            ITMGContext.GetInstance().OnRoomDisconnectEvent -= OnRoomDisconnect;
            ITMGContext.GetInstance().OnEndpointsUpdateInfoEvent -= OnEndpointsUpdateInfo;
#endif
        }

        private void Update()
        {
#if XDREAMER_GME
            //ITMGContext.GetInstance().Poll();
            QAVNative.QAVSDK_Poll();
#endif
        }

        private void OnApplicationFocus(bool hasFocus)
        {
#if XDREAMER_GME
            if (hasFocus)
            {
                ITMGContext.GetInstance().Resume();
            }
            else
            {
                ITMGContext.GetInstance().Pause();
            }

#endif
        }

        #endregion

        #region 基础信息

        /// <summary>
        /// 应用ID：来自【腾讯云控制台】的GME服务提供的AppID；
        /// </summary>
        [Name("应用ID")]
        [Tip("来自【腾讯云控制台】的GME服务提供的AppID；", "AppID provided by GME service from Tencent Cloud Console;")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _appID = "";

        /// <summary>
        /// 应用ID
        /// </summary>
        public string appID => _appID;

        /// <summary>
        /// 权限密钥：在GME服务中申请应用后鉴权信息中的权限密钥；
        /// </summary>
        [Name("权限密钥")]
        [Tip("在GME服务中申请应用后鉴权信息中的权限密钥；", "Apply for the permission key in the post application authentication information in the GME service;")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _authkey = "";

        /// <summary>
        /// 权限密钥
        /// </summary>
        public string authKey => _authkey;

        /// <summary>
        /// 用户ID：即GME中初始化SDK使用的OpenID参数；此参数由App开发者自行制定，App内不重复的编号；须遵循GME规范；默认只支持 Int64 类型（转为 string 传入）；
        /// </summary>
        [Name("用户ID")]
        [Tip("即GME中初始化SDK使用的OpenID参数；此参数由App开发者自行制定，App内不重复的编号；须遵循GME规范；默认只支持 Int64 类型（转为 string 传入）；", "The OpenID parameter used to initialize the SDK in GME; This parameter is set by the app developer themselves, and there are no duplicate numbers within the app; Must comply with GME regulations;By default, only Int64 types are supported (converted to string input);")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _userID = "";

        /// <summary>
        /// 用户ID
        /// </summary>
        public string userID
        {
            get => _userID;
            set
            {
                try
                {
                    if (init) return;
                    _userID = value;
                }
                finally
                {
                    this.CallUnityObjectPropertyChangedEvent(nameof(userID));
                }
            }
        }

        /// <summary>
        /// 用户ID整型64字符串
        /// </summary>
        public string userIDInt64String
        {
            get => _userID;
            set
            {
                try
                {
                    if (init) return;
                    if (long.TryParse(value, out long uid))
                    {
                        _userID = uid.ToString();
                    }
                }
                finally
                {
                    this.CallUnityObjectPropertyChangedEvent(nameof(userIDInt64String));
                }
            }
        }

        /// <summary>
        /// 用户ID整型64
        /// </summary>
        public long userIDInt64
        {
            get => long.TryParse(_userID, out long uid) ? uid : 0;
            set
            {
                try
                {
                    if (init) return;
                    _userID = value.ToString();
                }
                finally
                {
                    this.CallUnityObjectPropertyChangedEvent(nameof(userIDInt64));
                }
            }
        }

        /// <summary>
        /// 开放ID
        /// </summary>
        public string openID => _userID;

        /// <summary>
        /// 初始化：GME初始化的标记量
        /// </summary>
        private bool init = false;

        /// <summary>
        /// 初始化GME
        /// </summary>
        public void InitGME()
        {
#if XDREAMER_GME
            if (init) return;

            int ret = ITMGContext.GetInstance().Init(appID, openID);
            if (ret != QAVError.OK)
            {
                Log.DebugFormat("Init GME Failed {0}", ret);
                return;
            }
            init = true;
#endif
        }

        /// <summary>
        /// 反初始化GME
        /// </summary>
        public void UninitGME()
        {
            if (!init) return;
#if XDREAMER_GME
            ITMGContext.GetInstance().Uninit();
#endif
        }

        #endregion

        #region 房间

        /// <summary>
        /// 房间号:最大支持127字符
        /// </summary>
        [Name("房间号")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _roomID = "";

        /// <summary>
        /// 房间ID
        /// </summary>
        public string roomID
        {
            get => _roomID;
            set
            {
                try
                {
                    if (inRoom) return;
                    _roomID = value;
                }
                finally
                {
                    this.CallUnityObjectPropertyChangedEvent(nameof(roomID));
                }
            }
        }

        /// <summary>
        /// 房间类型
        /// </summary>
        [Name("房间类型")]
        [EnumPopup]
        public EITMGRoomType _roomType = EITMGRoomType.Fluency;

        /// <summary>
        /// 房间类型
        /// </summary>
        public EITMGRoomType roomType => _roomType;

#if XDREAMER_GME

        /// <summary>
        /// 正在进入房间中
        /// </summary>
        bool inEnterRooming = false;

#endif

        /// <summary>
        /// 在房间中
        /// </summary>
        public bool inRoom
        {
            get
            {
#if XDREAMER_GME
                return !inEnterRooming && ITMGContext.GetInstance().IsRoomEntered();
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// 本地鉴权
        /// </summary>
        private byte[] authBuffer;

        /// <summary>
        /// 进入房间
        /// </summary>
        public void EnterRoom()
        {
#if XDREAMER_GME
            if (!init || inEnterRooming || ITMGContext.GetInstance().IsRoomEntered()) return;
            inEnterRooming = true;

            authBuffer = QAVAuthBuffer.GenAuthBuffer(int.Parse(appID), roomID, userID, authKey);
            ITMGContext.GetInstance().EnterRoom(roomID, (ITMGRoomType)roomType, authBuffer);
#endif
        }

        /// <summary>
        /// 当加入房间完成
        /// </summary>
        /// <param name="err"></param>
        /// <param name="errInfo"></param>
        void OnEnterRoomComplete(int err, string errInfo)
        {
#if XDREAMER_GME
            inEnterRooming = false;
            if (err != 0)
            {
                Log.WarningFormat("join room failed, err:{0}, errInfo:{1}", err, errInfo);
                return;
            }
            else
            {
                //加入房间成功
            }
#endif
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        public void ExitRoom()
        {
#if XDREAMER_GME
            ITMGContext.GetInstance().ExitRoom();
#endif
        }

        /// <summary>
        /// 当退出房间完成
        /// </summary>
        void OnExitRoomComplete()
        {
            //退出房间后的处理
        }

        /// <summary>
        /// 当房间断开连接
        /// </summary>
        /// <param name="result"></param>
        /// <param name="error_info"></param>
        void OnRoomDisconnect(int result, string error_info) { }

        /// <summary>
        /// 切换房间:快速切换实时语音房间。此接口在进房后调用。切换房间后，不重置设备，即如果在此房间已经是打开麦克风状态，在切换房间后也会是打开麦克风状态。
        /// </summary>
        /// <param name="targetRoomID"></param>
        protected int SwitchRoom(string targetRoomID)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetRoom().SwitchRoom(targetRoomID, authBuffer);
#else
            return 0;
#endif
        }

        #endregion

        #region 房间管理

        /// <summary>
        /// 当终端更新信息
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="count"></param>
        /// <param name="openIdList"></param>
        void OnEndpointsUpdateInfo(int eventID, int count, string[] openIdList)
        {
            switch (eventID)
            {
                case 1:// ITMGContext.EVENT_ID_ENDPOINT_ENTER:
                    {
                        //有成员进入房间
                        break;
                    }
                case 2:// ITMGContext.EVENT_ID_ENDPOINT_EXIT:
                    {
                        //有成员退出房间
                        break;
                    }
                case 5:// ITMGContext.EVENT_ID_ENDPOINT_HAS_AUDIO:
                    {
                        //有成员发送音频包
                        break;
                    }
                case 6: //ITMGContext.EVENT_ID_ENDPOINT_NO_AUDIO:
                    {
                        //有成员停止发送音频包
                        break;
                    }
            }
        }

        #endregion

        #region 音频黑名单

        /// <summary>
        /// 添加音频黑名单:房间中禁言某成员;只对本端生效，不会影响其他端;
        /// </summary>
        /// <param name="userID">用户ID：需添加黑名单的用户OpenID</param>
        public void AddAudioBlackList(string userID)
        {
#if XDREAMER_GME
            ITMGContext.GetInstance().GetAudioCtrl().AddAudioBlackList(userID);
#endif
        }

        /// <summary>
        /// 删除音频黑名单:移除禁言
        /// </summary>
        /// <param name="userID">用户ID：需移除黑名单的用户OpenID</param>
        public void RemoveAudioBlackList(string userID)
        {
#if XDREAMER_GME
            ITMGContext.GetInstance().GetAudioCtrl().RemoveAudioBlackList(userID);
#endif
        }

        #endregion

        #region 麦克风

        /// <summary>
        /// 启用麦克风
        /// </summary>
        public bool enableMic { get => GetMicState() == 1; set => EnableMic(true); }

        /// <summary>
        /// 启用麦克风:开启或关闭麦克风;加入房间默认不打开麦克风;功能等价于同时调用启用音频采集设备、启用音频流发送；
        /// </summary>
        /// <param name="isEnabled">如果需要打开麦克风，则传入的参数为 true，如果关闭麦克风，则参数为 false</param>
        /// <returns></returns>
        public int EnableMic(bool isEnabled)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().EnableMic(isEnabled);
#else
            return 0;
#endif
        }

        /// <summary>
        /// 麦克风状态获取
        /// </summary>
        /// <returns>0为关闭麦克风状态，返回值1为打开麦克风状态</returns>
        public int GetMicState()
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().GetMicState();
#else
            return 0;
#endif
        }

        /// <summary>
        /// 启用音频采集设备
        /// </summary>
        public bool enableAudioCaptureDevice { get => IsAudioCaptureDeviceEnabled(); set => EnableAudioCaptureDevice(true); }

        /// <summary>
        /// 启用音频采集设备:开启或关闭采集设备;加入房间默认不打开设备;只能在进房后调用此接口，退房会自动关闭设备;在移动端，打开采集设备通常会伴随权限申请，音量类型调整等操作。
        /// </summary>
        /// <param name="isEnabled">如果需要打开采集设备，则传入的参数为 true，如果关闭采集设备，则参数为 false</param>
        /// <returns></returns>
        public int EnableAudioCaptureDevice(bool isEnabled)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().EnableAudioCaptureDevice(isEnabled);
#else
            return 0;
#endif
        }

        /// <summary>
        /// 采集设备状态获取
        /// </summary>
        /// <returns></returns>
        public bool IsAudioCaptureDeviceEnabled()
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().IsAudioCaptureDeviceEnabled();
#else
            return false;
#endif
        }

        /// <summary>
        /// 启用音频流发送
        /// </summary>
        public bool enableAudioSend { get => IsAudioSendEnabled(); set => EnableAudioSend(true); }

        /// <summary>
        /// 启用音频流发送：打开或关闭音频流发送；如果采集设备已经打开，那么会发送采集到的音频数据。如果采集设备没有打开，那么仍旧无声。
        /// </summary>
        /// <param name="isEnabled">如果需要打开音频上行，则传入的参数为 true，如果关闭音频上行，则参数为 false</param>
        /// <returns></returns>
        public int EnableAudioSend(bool isEnabled)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().EnableAudioSend(isEnabled);
#else
            return 0;
#endif
        }

        /// <summary>
        /// 音频流发送状态获取
        /// </summary>
        /// <returns></returns>
        public bool IsAudioSendEnabled()
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().IsAudioSendEnabled();
#else
            return false;
#endif
        }

        /// <summary>
        /// 获取麦克风实时音量:建议20ms获取一次。值域为0 - 100，通过此接口可以获取到麦克风采集到的实时音量情况。
        /// </summary>
        /// <returns></returns>
        public int GetMicLevel()
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().GetMicLevel();
#else
            return 0;
#endif
        }

        /// <summary>
        /// 获取音频流发送实时音量:用于获取自己音频流发送实时音量;值域为0 - 100;
        /// </summary>
        /// <returns></returns>
        public int GetSendStreamLevel()
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().GetSendStreamLevel();
#else
            return 0;
#endif
        }

        /// <summary>
        /// 麦克风音量
        /// </summary>
        public int micVolume { get => GetMicVolume(); set => SetMicVolume(value); }

        /// <summary>
        /// 设置麦克风软件音量：用于设置麦克风的音量，相当于对采集的声音做衰减或增益。
        /// </summary>
        /// <param name="volume">麦克风的音量：取值范围为 0-200，数值为0的时候表示静音，当数值为100的时候表示音量不增不减，默认数值为100；</param>
        /// <returns></returns>
        public int SetMicVolume(int volume)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().SetMicVolume(Mathf.Clamp(volume, 0, 200));
#else
            return 0;
#endif
        }

        /// <summary>
        /// 获取麦克风软件音量
        /// </summary>
        /// <returns>返回值为101代表没调用过设置麦克风软件音量</returns>
        public int GetMicVolume()
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().GetMicVolume();
#else
            return 0;
#endif
        }

        #endregion

        #region 扬声器

        /// <summary>
        /// 启用扬声器
        /// </summary>
        public bool enableSpeaker { get => GetSpeakerState() == 1; set => EnableSpeaker(value); }

        /// <summary>
        /// 启用扬声器：开启或关闭扬声器；功能等价于同时调用启用音频播放设备、启用音频流接收；
        /// </summary>
        /// <param name="isEnabled">如果需要关闭扬声器，则传入的参数为 false，如果打开扬声器，则参数为 true</param>
        /// <returns></returns>
        public int EnableSpeaker(bool isEnabled)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().EnableSpeaker(isEnabled);
#else
            return 0;
#endif
        }

        /// <summary>
        /// 扬声器状态获取
        /// </summary>
        /// <returns>返回值0为关闭扬声器状态，返回值1为打开扬声器状态</returns>
        public int GetSpeakerState()
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().GetSpeakerState();
#else
            return 0;
#endif
        }

        /// <summary>
        /// 启用音频播放设备
        /// </summary>
        public bool enableAudioPlayDevice { get => IsAudioPlayDeviceEnabled(); set => EnableAudioPlayDevice(value); }

        /// <summary>
        /// 启用音频播放设备：开启或关闭音频播放设备
        /// </summary>
        /// <param name="isEnabled">如果需要关闭播放设备，则传入的参数为 false，如果打开播放设备，则参数为 true</param>
        /// <returns></returns>
        public int EnableAudioPlayDevice(bool isEnabled)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().EnableAudioPlayDevice(isEnabled);
#else
            return 0;
#endif
        }

        /// <summary>
        /// 播放设备状态获取
        /// </summary>
        /// <returns></returns>
        public bool IsAudioPlayDeviceEnabled()
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().IsAudioPlayDeviceEnabled();
#else
            return false;
#endif
        }

        /// <summary>
        /// 启用音频流接收
        /// </summary>
        public bool enableAudioRecv { get => IsAudioRecvEnabled(); set => EnableAudioRecv(value); }

        /// <summary>
        /// 启用音频流接收：用于打开或关闭音频流接收。如果播放设备已经打开，那么会播放房间里其他人的音频数据。如果播放设备没有打开，那么仍旧无声。
        /// </summary>
        /// <param name="isEnabled">如果需要打开音频下行，则传入的参数为 true，如果关闭音频下行，则参数为 false</param>
        /// <returns></returns>
        public int EnableAudioRecv(bool isEnabled)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().EnableAudioRecv(isEnabled);
#else
            return 0;
#endif
        }

        /// <summary>
        /// 音频流接收状态获取
        /// </summary>
        /// <returns></returns>
        public bool IsAudioRecvEnabled()
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().IsAudioRecvEnabled();
#else
            return false;
#endif
        }

        /// <summary>
        /// 获取扬声器实时音量:建议20ms获取一次
        /// </summary>
        /// <returns></returns>
        public int GetSpeakerLevel()
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().GetSpeakerLevel();
#else
            return 0;
#endif
        }

        /// <summary>
        /// 获取房间内其他成员音频流音量:取值范围为0 - 200
        /// </summary>
        /// <param name="userID">用户ID：房间其他成员的OpenID</param>
        /// <returns></returns>
        public int GetRecvStreamLevel(string userID)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().GetRecvStreamLevel(userID);
#else
            return 0;
#endif
        }

        /// <summary>
        /// 动态设置房间内某成员音量:用于设置房间内某成员的说话音量大小，此设置只在本端生效。
        /// </summary>
        /// <param name="userID">用户ID：房间其他成员的OpenID</param>
        /// <param name="volume">音量:建议[0-200]，其中100为默认值</param>
        /// <returns></returns>
        public int SetSpeakerVolumeByOpenID(string userID, int volume)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().SetSpeakerVolumeByOpenID(userID, volume);
#else
            return 0;
#endif
        }

        /// <summary>
        /// 扬声器音量
        /// </summary>
        public int speakerVolume { get => GetSpeakerVolume(); set => SetSpeakerVolume(value); }

        /// <summary>
        /// 设置扬声器音量
        /// </summary>
        /// <param name="volume">音量:范围0 - 200，当数值为0时，表示静音，当数值为100时，表示音量不增不减，默认数值为100。</param>
        /// <returns></returns>
        public int SetSpeakerVolume(int volume)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().SetSpeakerVolume(volume);
#else
            return 0;
#endif
        }

        /// <summary>
        /// 获取扬声器的音量:返回值为101代表没调用设置扬声器音量;
        /// </summary>
        /// <returns></returns>
        public int GetSpeakerVolume()
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetAudioCtrl().GetSpeakerVolume();
#else
            return 0;
#endif
        }

        #endregion

        #region

        /// <summary>
        /// 限制最大语音信息时长:限制最大语音消息的长度，最大支持58秒。
        /// </summary>
        /// <param name="msTime">毫秒时间</param>
        /// <returns></returns>
        public int SetMaxMessageLength(int msTime)
        {
#if XDREAMER_GME
            return ITMGContext.GetInstance().GetPttCtrl().SetMaxMessageLength(msTime);
#else
            return 0;
#endif
        }

        #endregion
    }

    #region ITMG房间类型

    /// <summary>
    /// ITMG房间类型
    /// </summary>
    public enum EITMGRoomType
    {
        /// <summary>
        /// 流畅:流畅的音质
        /// </summary>
        [Name("流畅")]
        Fluency = 1,

        /// <summary>
        /// 标准:良好的音质
        /// </summary>
        [Name("标准")]
        Standard = 2,

        /// <summary>
        /// 高质量:高清音质
        /// </summary>
        [Name("高质量")]
        HighQuality = 3,
    }

    #endregion
}
