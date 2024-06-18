using System;
using System.ComponentModel;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 自动连接网络:断线后会自动重新尝试连接网络
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Net)]
    [ComponentMenu(MMOHelperExtension.TitleDirectory + Title, typeof(MMOManager))]
    [Name(Title, nameof(AutoConnectNet))]
    [Tip("断线后会自动重新尝试连接网络", "After the network is disconnected, it will automatically try to reconnect")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    public class AutoConnectNet : Trigger<AutoConnectNet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "自动连接网络";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(MMOHelperExtension.Title, typeof(MMOManager))]
        [StateComponentMenu(MMOHelperExtension.TitleDirectory + Title, typeof(MMOManager))]
        [Name(Title, nameof(AutoConnectNet))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("连接网络，断线后会自动重新尝试连接网络", "After the network is disconnected, it will automatically try to reconnect")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 重连间隔时间
        /// </summary>
        [Name("重连间隔时间")]
        [Tip("网络连接失败时，自动重连的间隔时间；单位秒；", "The interval between automatic reconnection when the network connection fails; Unit seconds;")]
        [Range(3, 1000)]
        public float _delayTime = 5;

        /// <summary>
        /// 设置
        /// </summary>
        [Name("设置")]
        [Serializable]
        [LanguageFileOutput]
        public class Setting
        {
            /// <summary>
            /// 规则
            /// </summary>
            [Name("规则")]
            [Tip("自动连接或询问", "Automatic connection or inquiry")]
            [EnumPopup]
            public TimeSetting.ERule rule = TimeSetting.ERule.Auto;

            /// <summary>
            /// 自动
            /// </summary>
            public bool auto => rule == TimeSetting.ERule.Auto;
        }

        /// <summary>
        /// 连接配置
        /// </summary>
        [Name("连接配置")]
        [Tip("自动或询问连接", "Automatic or inquiry connection")]
        public Setting _connect = new Setting();

        /// <summary>
        /// 仅连接
        /// </summary>
        public bool onlyConnect => _connect.auto && !_enterRoomSetting.auto;

        /// <summary>
        /// 进入房间配置
        /// </summary>
        [Name("进入房间配置")]
        [Tip("自动或询问配置", "Automatic or query configuration")]
        public Setting _enterRoomSetting = new Setting();

        /// <summary>
        /// 加入房间规则
        /// </summary>
        [Name("加入房间规则")]
        [EnumPopup]
        public EEnterRoomRule _enterRoomRule = EEnterRoomRule.Default;

        private float timeCounter = 0;

        private MMOManager mmo => MMOManager.instance;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            // 设置时间上线，先连接一次
            timeCounter = _delayTime;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            timeCounter += Time.deltaTime;
            if (timeCounter > _delayTime)
            {
                timeCounter = 0;

                if (forceInit)
                {
                    forceInit = false;
                    mmo.StopMMO();
                }

                AutoConnect();
            }
        }

        private bool forceInit = false;

        private void AutoConnect()
        {
            if (MMOHelper.isEnteredRoom)
            {
                finished = true;
                return;
            }

            if (!MMOHelper.started)
            {
                if (_connect.auto) mmo.StartMMO();
                return;
            }

            if (MMOHelper.connected)
            {
                if (onlyConnect)
                {
                    finished = true;
                    return;
                }

                if (_enterRoomSetting.auto)
                {
                    if (MMOHelper.canEnterRoom)
                    {
                        mmo._enterRoomRule = _enterRoomRule;
                        mmo.EnterRoom();
                    }
                }
                else
                {
                    finished = true;
                }
            }
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _delayTime + "秒";
        }
    }
}