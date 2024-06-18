using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;
using XCSJ.PluginMMO.Chats;

#if XDREAMER_GME
using GME;
#endif

namespace XCSJ.PluginGME.Tools
{
    /// <summary>
    /// GME语音聊天
    /// </summary>
    [DisallowMultipleComponent]
    [Name("GME语音聊天")]
    public class GMEVoiceChat : VoiceChat
    {
        /// <summary>
        /// 概要前缀
        /// </summary>
        public const string SummaryPrefix = nameof(GMEVoiceChat) + "-";

        /// <summary>
        /// 开始概要
        /// </summary>
        public override string beginSummary => SummaryPrefix;

        /// <summary>
        /// 结束概要
        /// </summary>
        public override string endSummary => SummaryPrefix;

        #region Unity消息

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            playNext = true;
            EnterRoom();
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            ExitRoom();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected override void Update()
        {
            TryPlayWaitedChatInfos();
        }

        /// <summary>
        /// 尝试播放等待的聊天信息
        /// </summary>
        private void TryPlayWaitedChatInfos()
        {
            if (playNext)
            {
                //处理已播放完成的聊天信息
                if (playingChatContent != null)
                {
                    playingChatContent.inReading = false;
                    playingChatContent = null;
                }

                //有待播放的
                if (_waitPlayChatContents.Count > 0)
                {
                    playingChatContent = _waitPlayChatContents[0];
                    _waitPlayChatContents.RemoveAt(0);

                    playingChatContent.read = true;
                    playingChatContent.inReading = true;
                    InternalPlay(playingChatContent);
                }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (gmeProvider) { }
        }

        #endregion

        #region 扬声器：语音消息播放

        /// <summary>
        /// 支持扬声器
        /// </summary>
        public override bool supportSpeaker => gmeProvider && _gmeProvider.inRoom;

        /// <summary>
        /// 是语音播放中
        /// </summary>
        public override bool isVoicePlaying => gmeProvider && _gmeProvider.enableSpeaker;

        class GMEAudioData : VoiceData
        {
            /// <summary>
            /// 语音时长：单位为秒；
            /// </summary>
            internal float _voiceDuration = 0;

            /// <summary>
            /// 语音时长：单位为秒；
            /// </summary>
            public override float voiceDuration => _voiceDuration;
        }

        bool playNext = true;

        /// <summary>
        /// 内部播放
        /// </summary>
        /// <param name="chatContent"></param>
        private void InternalPlay(ChatContent chatContent)
        {
            var gme = gmeProvider;
            if (gme)
            {
                Debug.Log(nameof(GMEVoiceChat) + ".summary: " + chatContent.summary);
                Debug.Log(nameof(GMEVoiceChat) + ".content: " + chatContent.content);

                if (chatContent.voiceData == null)
                {
                    GMEAudioData data0 = new GMEAudioData();
                    chatContent.voiceData = data0;

                    if (float.TryParse(chatContent.summary, out var f0))
                    {
                        data0._voiceDuration = f0;
                    }
                    else if (float.TryParse(chatContent.content.Replace(SummaryPrefix, ""), out var f1))
                    {
                        data0._voiceDuration = f1;
                    }
                }
                
                if(chatContent.voiceData is GMEAudioData data)
                {
                    playNext = false;
                    Debug.Log("playNext 0:" + data.voiceDuration);
                    CommonFun.DelayCall(() =>
                    {
                        playNext = true;
                        Debug.Log("playNext 1:" + data.voiceDuration);
                    }, data.voiceDuration);
                }
                else
                {
                    playNext = true;
                    Debug.Log("playNext 2:" + chatContent.summary);
                }
            }
        }

        /// <summary>
        /// 是可播放的：判断聊天内容是否是可播放的，即当前组件可播放此聊天内容；
        /// </summary>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public override bool IsPlayable(ChatContent chatContent)
        {
            return chatContent != null
                && chatContent.contentType == EContentType.Voice
                && chatContent.summary.StartsWith(SummaryPrefix)
                && chatContent.playerInfo.AllowAudio();
        }

        #endregion

        #region 麦克风：语音消息录音

        /// <summary>
        /// 语音最大时长
        /// </summary>
        [Group("麦克风", textEN = "Microphone")]
        [Name("语音最大时长")]
        [Tip("期望录制的音频最大时长", "Maximum audio duration expected to be recorded")]
        [Range(1, 58)]
        public int _voiceMaxDuration = 15;

        /// <summary>
        /// 语音最大时长：单位为秒；
        /// </summary>
        public override float voiceMaxDuration => _voiceMaxDuration;

        /// <summary>
        /// 语音录制时长：单位为秒；
        /// </summary>
        public override float voiceRecordDuration
        {
            get
            {
                return Math.Min((float)(isVoiceRecording ? (DateTime.Now - startTime).TotalSeconds : hasRecord ? (stopTime - startTime).TotalSeconds : 0), voiceMaxDuration);
            }
        }

        /// <summary>
        /// 支持麦克风
        /// </summary>
        public override bool supportMicrophone => gmeProvider && _gmeProvider.inRoom;

        /// <summary>
        /// 是语音录制中
        /// </summary>
        public override bool isVoiceRecording => gmeProvider && _gmeProvider.enableMic;

        /// <summary>
        /// 启动时间
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// 停止时间
        /// </summary>
        private DateTime stopTime;

        private bool hasRecord = false;

        /// <summary>
        /// 启动语音：开启语音的录制
        /// </summary>
        /// <returns></returns>
        public override bool StartVoice()
        {
            var gme = gmeProvider;
            if (gme && !gme.enableMic)
            {
                startTime = DateTime.Now;
                hasRecord = false;

                gme.enableMic = true;
                return gme.enableMic;
            }
            return false;
        }

        /// <summary>
        /// 停止语音：结束语音的录制
        /// </summary>
        public override void StopVoice()
        {
            var gme = gmeProvider;
            if (gme && gme.enableMic)
            {
                stopTime = DateTime.Now;
                hasRecord = true;

                gme.enableMic = false;
            }
        }

        /// <summary>
        /// 尝试编码为语音文本
        /// </summary>
        /// <param name="voiceText"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        public override bool TryEncodeToVoiceText(out string voiceText, out string summary)
        {
            var gme = gmeProvider;
            if (hasRecord && gme && !gme.enableMic)
            {
                summary = SummaryPrefix + voiceRecordDuration;
                voiceText = "";
                return true;
            }
            voiceText = default;
            summary = default;
            return false;
        }

        #endregion

        #region MMO事件

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            base.OnMMOEnterRoomCompleted(result);
            if (result == EACode.Success)
            {
                playNext = true;
                EnterRoom();
            }
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            base.OnMMOExitRoomCompleted();
            ExitRoom();
        }

        #endregion

        #region GME处理

        /// <summary>
        /// GME提供者
        /// </summary>
        [Name("GME提供者")]
        [EndGroup]
        [ComponentPopup]
        public GMEProvider _gmeProvider;

        /// <summary>
        /// GME提供者
        /// </summary>
        public GMEProvider gmeProvider
        {
            get
            {
                if (!_gmeProvider)
                {
                    _gmeProvider = GMEProvider.instance;
                    this.XGetComponentInChildrenOrGlobal(ref _gmeProvider);

                    this.CallUnityObjectPropertyChangedEvent(nameof(gmeProvider));
                }
                return _gmeProvider;
            }
        }

        private void EnterRoom()
        {
            var gme = gmeProvider;
            if (gme && !gme.inRoom && MMOHelper.isEnteredRoom)
            {
                gme.userIDInt64 = GMEHelper.CreateOpenID(MMOHelper.roomInfo.appGuid, MMOHelper.userGuid);
                gme.roomID = MMOHelper.roomGuid;

                gme.InitGME();
                gme.SetMaxMessageLength(_voiceMaxDuration * 1000);
                gme.EnterRoom();

                //启用扬声器
                gme.enableSpeaker = true;
            }
        }

        private void ExitRoom()
        {
            var gme = gmeProvider;
            if (gme)
            {
                gme.ExitRoom();
                gme.UninitGME();
            }
        }

        #endregion
    }
}
