using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using static XCSJ.PluginMMO.MMOHelper;

namespace XCSJ.PluginMMO.Chats
{
    /// <summary>
    /// 聊天内容
    /// </summary>
    [Import]
    [Serializable]
    [Name("聊天内容")]
    public class ChatContent : JsonObject<ChatContent>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public ChatContent() { }

        /// <summary>
        /// 网络聊天
        /// </summary>
        internal NetChat netChat { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        [Name("用户编号")]
        [Readonly]
        public string userGuid = "";

        /// <summary>
        /// 内容类型
        /// </summary>
        [Name("内容类型")]
        [EnumPopup]
        [Readonly]
        public EContentType contentType = EContentType.Text;

        /// <summary>
        /// 内容状态
        /// </summary>
        [Name("内容状态")]
        [EnumPopup]
        [Readonly]
        public EContentState contentState = EContentState.Content;

        /// <summary>
        /// 概要
        /// </summary>
        [Name("概要")]
        [Readonly]
        public string summary = "";

        /// <summary>
        /// 内容
        /// </summary>
        [Name("内容")]
        [Readonly]
        public string content = "";

        /// <summary>
        /// 显示文本
        /// </summary>
        [Json(false)]
        public string displayText => string.IsNullOrEmpty(summary) ? content : summary;

        /// <summary>
        /// 用户名
        /// </summary>
        [Json(false)]
        public virtual string userName => MMOHelper.GetUserDisplayName(userGuid);

        /// <summary>
        /// 玩家信息
        /// </summary>
        private PlayerInfo _playerInfo;

        /// <summary>
        /// 玩家信息
        /// </summary>
        [Json(false)]
        public PlayerInfo playerInfo => _playerInfo ?? (_playerInfo = MMOHelper.GetPlayer(userGuid));

        /// <summary>
        /// 已发送
        /// </summary>
        [Name("已发送")]
        [Readonly]
        [Json(false)]
        public bool _sended = false;

        /// <summary>
        /// 已发送
        /// </summary>
        [Json(false)]
        public bool sended
        {
            get => _sended;
            set
            {
                if (_sended) return;
                _sended = value;
            }
        }

        /// <summary>
        /// 已读
        /// </summary>
        [Name("已读")]
        [Readonly]
        [Json(false)]
        public bool _read = false;

        /// <summary>
        /// 已读
        /// </summary>
        [Json(false)]
        public bool read
        {
            get => _read;
            set
            {
                if (_read) return;
                _read = value;
            }
        }

        /// <summary>
        /// 在读取中
        /// </summary>
        [Name("在读取中")]
        [Readonly]
        [Json(false)]
        public bool _inReading = false;

        /// <summary>
        /// 在读取中
        /// </summary>
        [Json(false)]
        public bool inReading { get => _inReading; set => _inReading = value; }

        /// <summary>
        /// 语音数据
        /// </summary>
        [Json(false)]
        public VoiceData voiceData { get; set; }

        /// <summary>
        /// 语音时长：单位为秒；
        /// </summary>
        [Json(false)]
        public float voiceDuration => voiceData != null ? voiceData.voiceDuration : 0;

        /// <summary>
        /// 当点击用户名
        /// </summary>
        public void OnClickUserName()
        {
            Debug.Log(nameof(OnClickUserName) + ":" + userName);
        }

        /// <summary>
        /// 读取：读取聊天信息的内容文本；会直接修改已读标记量；如果是语音会尝试调用扬声器进行播放；
        /// </summary>
        public void Read()
        {
            //在读取中,不可重复读取
            if (inReading) return;

            Debug.Log(nameof(Read) + ".contentType:" + contentType.ToString());
            Debug.Log(nameof(Read) + ".contentState:" + contentState.ToString());
            Debug.Log(nameof(Read) + ".summary:" + summary);
            Debug.Log(nameof(Read) + ".content[" + content.Length + "]:" + content);

            read = true;
            if (netChat)
            {
                netChat.Play(this);
            }
        }
    }

    /// <summary>
    /// 语音数据
    /// </summary>
    public abstract class VoiceData
    {
        /// <summary>
        /// 语音时长：单位为秒；
        /// </summary>
        public abstract float voiceDuration { get; }
    }

    /// <summary>
    /// 系统信息
    /// </summary>
    [Import]
    [Serializable]
    [Name("系统信息")]
    public class SystemInfo : ChatContent
    {
        /// <summary>
        /// 用户名
        /// </summary>
#if UNITY_WEBGL && !UNITY_EDITOR
        public override string userName => "<color=#00FF00FF>[System]</color>";
#else
        public override string userName => "<color=#00FF00FF>[系统]</color>";
#endif

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="content"></param>
        public SystemInfo(string content)
        {
            this.content = content;
        }
    }

    /// <summary>
    /// 内容类型
    /// </summary>
    [Name("内容类型")]
    public enum EContentType
    {
        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        Text = 0,

        /// <summary>
        /// 语音
        /// </summary>
        [Name("语音")]
        Voice = 10,
    }

    /// <summary>
    /// 内容状态
    /// </summary>
    public enum EContentState
    {
        /// <summary>
        /// 内容
        /// </summary>
        [Name("内容")]
        Content,

        /// <summary>
        /// 开始
        /// </summary>
        [Name("开始")]
        Begin,

        /// <summary>
        /// 结束
        /// </summary>
        [Name("结束")]
        End,
    }
}
