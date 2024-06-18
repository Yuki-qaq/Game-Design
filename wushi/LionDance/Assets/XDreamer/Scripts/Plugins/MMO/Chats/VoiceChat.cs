using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.Chats
{
    /// <summary>
    /// 语音聊天
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Audio)]
    [Name("语音聊天")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    [RequireComponent(typeof(NetChat))]
    [Tool(MMOHelperExtension.Title, nameof(NetChat), rootType = typeof(MMOManager))]
    public abstract class VoiceChat : ChatContentProcessor
    {
        /// <summary>
        /// 支持的内容类型
        /// </summary>
        public override EContentType supportContentType => EContentType.Voice;

        /// <summary>
        /// 开始概要
        /// </summary>
        public abstract string beginSummary { get; }

        /// <summary>
        /// 结束概要
        /// </summary>
        public abstract string endSummary { get; }

        NetChat _netChat;

        /// <summary>
        /// 网络聊天
        /// </summary>
        public NetChat netChat => this.XGetComponent(ref _netChat);

        #region Unity 消息

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            MMOHelper.onEnterRoomCompleted += OnMMOEnterRoomCompleted;
            MMOHelper.onExitRoomCompleted += OnMMOExitRoomCompleted;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            MMOHelper.onEnterRoomCompleted -= OnMMOEnterRoomCompleted;
            MMOHelper.onExitRoomCompleted -= OnMMOExitRoomCompleted;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected abstract void Update();

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            var netChat = this.netChat;
            if (netChat && netChat.voiceChat) { }
        }

        #endregion

        #region 扬声器：语音消息播放

        /// <summary>
        /// 待播放聊天内容列表
        /// </summary>
        [Name("待播放聊天内容列表")]
        public List<ChatContent> _waitPlayChatContents = new List<ChatContent>();

        /// <summary>
        /// 播放中的聊天内容
        /// </summary>
        public ChatContent playingChatContent { get; protected set; }

        /// <summary>
        /// 支持扬声器
        /// </summary>
        public abstract bool supportSpeaker { get; }

        /// <summary>
        /// 是语音播放中
        /// </summary>
        public abstract bool isVoicePlaying { get; }

        /// <summary>
        /// 语音时长：单位为秒；
        /// </summary>
        public float voiceDuration => playingChatContent != null ? playingChatContent.voiceDuration : 0;

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="chatContent"></param>
        public override void Play(ChatContent chatContent)
        {
            if (!IsPlayable(chatContent)) return;
            _waitPlayChatContents.AddWithDistinct(chatContent);
        }

        #endregion

        #region 麦克风：语音消息录音

        /// <summary>
        /// 语音最大时长：单位为秒；
        /// </summary>
        public abstract float voiceMaxDuration { get; }

        /// <summary>
        /// 语音录制时长：单位为秒；
        /// </summary>
        public abstract float voiceRecordDuration { get; }

        /// <summary>
        /// 支持麦克风
        /// </summary>
        public abstract bool supportMicrophone { get; }

        /// <summary>
        /// 是语音录制中
        /// </summary>
        public abstract bool isVoiceRecording { get; }

        /// <summary>
        /// 启动语音：开启语音的录制
        /// </summary>
        /// <returns></returns>
        public abstract bool StartVoice();

        /// <summary>
        /// 停止语音：结束语音的录制
        /// </summary>
        public abstract void StopVoice();

        /// <summary>
        /// 尝试编码为语音文本
        /// </summary>
        /// <param name="voiceText">语音文本</param>
        /// <param name="summary">概要</param>
        /// <returns></returns>
        public abstract bool TryEncodeToVoiceText(out string voiceText, out string summary);

        #endregion

        #region 网络聊天事件

        /// <summary>
        /// 当接收到聊天内容
        /// </summary>
        /// <param name="chatContents"></param>
        public virtual void OnReceiveChatContent(List<ChatContent> chatContents)
        {
            foreach (var content in chatContents)
            {
                if(IsPlayable(content))
                {
                    _waitPlayChatContents.Add(content);
                }
            }
        }

        #endregion

        #region MMO事件

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        public virtual void OnMMOEnterRoomCompleted(EACode result) { }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public virtual void OnMMOExitRoomCompleted()
        {
            _waitPlayChatContents.Clear();
            this.CallUnityObjectPropertyChangedEvent(nameof(_waitPlayChatContents));
        }

        #endregion
    }

    /// <summary>
    /// 聊天内容处理器
    /// </summary>
    public abstract class ChatContentProcessor : InteractProvider
    {
        /// <summary>
        /// 支持的内容类型
        /// </summary>
        public abstract EContentType supportContentType { get; }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="chatContent"></param>
        public abstract void Play(ChatContent chatContent);

        /// <summary>
        /// 是可播放的：判断聊天内容是否是可播放的，即当前组件可播放此聊天内容；
        /// </summary>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public abstract bool IsPlayable(ChatContent chatContent);
    }
}
