using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Extension.Base.Extensions;
using System.Linq;
using static XCSJ.PluginMMO.MMOHelper;
using XCSJ.PluginMMO.Chats;
using XCSJ.PluginMMO.Base;
using XCSJ.PluginXGUI.Windows.ListViews;
using XCSJ.PluginXGUI.Base;
using System.Collections;
using XCSJ.Extension.Base.Audios;

namespace XCSJ.PluginMMO.Chats
{
    /// <summary>
    /// 网络聊天
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Chat)]
    [DisallowMultipleComponent]
    [Name("网络聊天")]
    [Tool(MMOHelperExtension.Title, nameof(NetIdentity), rootType = typeof(MMOManager))]
    public sealed class NetChat : NetMB, IListHost
    {
        #region 模型宿主

        /// <summary>
        /// 当试图事件
        /// </summary>
        /// <param name="viewInteractData"></param>
        public void OnViewEvent(ViewInteractData viewInteractData)
        {            
            //Debug.Log("OnViewEvent.interactor: " + viewInteractData.interactor.GameObjectComponentToString());
            //Debug.Log("OnViewEvent.cmdName: " + viewInteractData.cmdName);
            //Debug.Log("OnViewEvent.cmdParam: " + viewInteractData.cmdParam);
        }

        /// <summary>
        /// 是否选择
        /// </summary>
        /// <param name="list"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsSelected(IList list, object model)
        {
            if (list == null || model == null) return false;

            return list == chatContents && model is ChatContent chatContent && chatContents.Contains(chatContent);
        }

        #endregion

        #region 聊天信息

        /// <summary>
        /// 最大数目：存储的聊天记录的最大条数
        /// </summary>
        public const int Max = 64;

        /// <summary>
        /// 最大数目
        /// </summary>
        [Name("最大数目")]
        [Tip("-1表示不限制聊天记录的数目；否者超过对应数目后，最早的聊天记录会被移除；", "-1 means unlimited number of chat records; If not, the earliest chat record will be removed after exceeding the corresponding number;")]
        [Range(-1, NetChat.Max)]
        public int _maxCount = NetChat.Max;

        /// <summary>
        /// 输出系统信息
        /// </summary>
        [Name("输出系统信息")]
        [Tip("输出在MMO网络环境中发生的各种事件信息，包括玩家的进入、退出等", "Output various event information in MMO network environment, including player entry, exit, etc")]
        public bool _outputSystemInfo = true;

        /// <summary>
        /// 发送缓存
        /// </summary>
        [Name("发送缓存")]
        public ChatContent _sendBuffer = new ChatContent();

        /// <summary>
        /// 内容类型
        /// </summary>
        public EContentType contentType
        {
            get => _sendBuffer.contentType;
            set
            {
                try
                {
                    if (!MMOHelper.isEnteredRoom) return;
                    _sendBuffer.contentType = value;
                }
                finally
                {
                    this.CallUnityObjectPropertyChangedEvent(nameof(contentType));
                }
            }
        }

        /// <summary>
        /// 显示文本
        /// </summary>
        public string displayText
        {
            get
            {
                switch (contentType)
                {
                    case EContentType.Voice:
                        {
                            return supportVoice ? MMOHelperExtension.DurationInfo(_voiceChat.voiceRecordDuration, _voiceChat.voiceMaxDuration) : MMOHelperExtension.DurationInfo(0, 0);
                        }
                }
                return _sendBuffer.content;
            }
            set
            {
                try
                {
                    if (!MMOHelper.isEnteredRoom) return;
                    switch (contentType)
                    {
                        case EContentType.Voice: return;
                    }
                    _sendBuffer.content = value;
                }
                finally
                {
                    this.CallUnityObjectPropertyChangedEvent(nameof(displayText));
                }
            }
        }

        /// <summary>
        /// 已选择的聊天内容
        /// </summary>
        public ChatContent selectedChatContent { get; set; }

        /// <summary>
        /// 聊天内容列表
        /// </summary>
        [Name("聊天内容列表")]
        public List<ChatContent> _chatContents = new List<ChatContent>();

        /// <summary>
        /// 聊天内容列表
        /// </summary>
        public List<ChatContent> chatContents => _chatContents;

        /// <summary>
        /// 添加聊天信息
        /// </summary>
        /// <param name="chatContent"></param>
        private void AddChatInfo(ChatContent chatContent)
        {
            chatContent.netChat = this;
            _chatContents.Add(chatContent);
            this.CallUnityObjectPropertyChangedEvent(nameof(chatContents));
        }

        /// <summary>
        /// 清理聊天信息
        /// </summary>
        private void ClearChatInfos()
        {
            _chatContents.Clear();
            this.CallUnityObjectPropertyChangedEvent(nameof(chatContents));
        }

        /// <summary>
        /// 限定数量
        /// </summary>
        private void LimitCount()
        {
            if (_maxCount == -1) return;
            var deleteCount = _chatContents.Count - _maxCount;
            if (deleteCount <= 0) return;
            if (deleteCount >= _chatContents.Count)
            {
                ClearChatInfos();
                return;
            }
            //执行删除
            _chatContents.RemoveRange(0, deleteCount);
            this.CallUnityObjectPropertyChangedEvent(nameof(chatContents));
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            ensureSyncBuffer.Clear();

            ClearChatInfos();
        }

        /// <summary>
        /// 输出系统信息
        /// </summary>
        /// <param name="text"></param>
        private void OutputSystemInfo(string text) => AddChatInfo(new SystemInfo(text));

        #endregion

        #region MMO对象

        /// <summary>
        /// 确保同步缓存
        /// </summary>
        EnsureSyncBuffer<ChatContent> _ensureSyncBuffer;

        /// <summary>
        /// 确保同步缓存
        /// </summary>
        private EnsureSyncBuffer<ChatContent> ensureSyncBuffer => _ensureSyncBuffer ?? (_ensureSyncBuffer = new EnsureSyncBuffer<ChatContent>(this));

        /// <summary>
        /// 当定时检查修改
        /// </summary>
        /// <returns></returns>
        protected override bool OnTimedCheckChange() => ensureSyncBuffer.HasWaitSend();

        /// <summary>
        /// 当序列化数据
        /// </summary>
        /// <returns></returns>
        protected override string OnSerializeData() => ensureSyncBuffer.GetSend();

        /// <summary>
        /// 当反序列化数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataObj"></param>
        protected override void OnDeserializeData(string data, Data dataObj)
        {
            //本地用户的聊天信息
            if (ensureSyncBuffer.OnReceive(dataObj, list0 =>
            {
                foreach (var i in list0)
                {
                    i.sended = true;
                }
            }))
            {
                //本地用户发送的聊天信息不再处理
                return;
            }

            //其他用户的聊天信息
            if (JsonHelper.ToObject<List<ChatContent>>(data) is List<ChatContent> list)
            {
                //Debug.Log("OnDeserializeData: " + list.Count);
                foreach (var i in list)
                {
                    AddChatInfo(i);
                }

                LimitCount();

                if (_tryAutoPlay && _voiceChat) _voiceChat.OnReceiveChatContent(list);
            }
        }

        #endregion

        #region MMO事件     

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            //Debug.Log(nameof(OnMMOExitRoomCompleted) + ": " + name);
            base.OnMMOExitRoomCompleted();
            Clear();
        }

        /// <summary>
        /// 当MMO房间增加用户
        /// </summary>
        /// <param name="guid"></param>
        public override void OnMMORoomAddUser(string guid)
        {
            base.OnMMORoomAddUser(guid);
            if (_outputSystemInfo && guid != MMOHelper.userGuid)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                OutputSystemInfo(string.Format("Player[{0}](GUID:{1}) enter room !", MMOHelper.GetPlayer(guid)?.displayName, guid));
#else
                OutputSystemInfo(string.Format("玩家[{0}](编号:{1})进入房间！", MMOHelper.GetPlayer(guid)?.displayName, guid));
#endif
            }
        }

        /// <summary>
        /// 当MMO房间移除用户
        /// </summary>
        /// <param name="guid"></param>
        public override void OnMMORoomRemoveUser(string guid)
        {
            base.OnMMORoomRemoveUser(guid);
            if (_outputSystemInfo && guid != MMOHelper.userGuid)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                OutputSystemInfo(string.Format("Player[{0}](GUID:{1}) exit room !", MMOHelper.GetPlayer(guid)?.displayName, guid));
#else
                OutputSystemInfo(string.Format("玩家[{0}](编号:{1})退出房间！", MMOHelper.GetPlayer(guid)?.displayName, guid));
#endif
            }
        }

        #endregion

        #region Unity 消息

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (voiceChat) { }
            _sendBuffer.netChat = this;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            StopRecord();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (voiceChat) { }
        }

        #endregion

        #region 发送语音

        /// <summary>
        /// 内部发送
        /// </summary>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <param name="summary"></param>
        /// <param name="contentState"></param>
        /// <returns></returns>
        private bool InternalSend(string content, EContentType contentType, string summary = "", EContentState contentState = EContentState.Content)
        {
            if (!MMOHelper.roomSynced) return false;

            switch (contentType)
            {
                case EContentType.Text:
                    {
                        //文本内容不可为空
                        if (string.IsNullOrEmpty(content)) return false;
                        break;
                    }
            }

            var chatInfo = new ChatContent()
            {
                netChat = this,
                userGuid = MMOHelper.userGuid,
                contentType = contentType,
                contentState = contentState,
                content = content,
                summary = summary,
            };

            ensureSyncBuffer.Send(chatInfo);
            AddChatInfo(chatInfo);

            LimitCount();
            MarkDirty();

            return true;
        }

        /// <summary>
        /// 发送：将发送缓存的聊天信息发送
        /// </summary>
        public void Send()
        {
            switch(contentType)
            {
                case EContentType.Text:
                    {
                        if (InternalSend(displayText, EContentType.Text))
                        {
                            displayText = "";
                        }
                        break;
                    }
                case EContentType.Voice:
                    {
                        StopRecordAndSendVoice();
                        break;
                    }
            }
            MarkDirty();
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="text"></param>
        public void Send(string text) => InternalSend(text, EContentType.Text);

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="audioClip"></param>
        /// <returns></returns>
        public void Send(AudioClip audioClip)
        {
            if (!MMOHelper.roomSynced || !supportVoice) return;
            if (audioClip.TryEncodeToWAV64(out var voiceText))
            {
                InternalSend(voiceText, EContentType.Voice, UnityVoiceChat.SummaryPrefix + audioClip.length.ToString(), EContentState.Content);
            }
        }

        #endregion

        #region 语音聊天

        /// <summary>
        /// 尝试自动播放
        /// </summary>
        [Name("尝试自动播放")]
        [Tip("收到聊天内容时，是否尝试自动播放聊天内容", "Do you try to automatically play when receiving chat contents")]
        public bool _tryAutoPlay = true;

        /// <summary>
        /// 语音聊天
        /// </summary>
        [Name("语音聊天")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public VoiceChat _voiceChat;

        /// <summary>
        /// 语音聊天
        /// </summary>
        public VoiceChat voiceChat => this.XGetComponentInChildren(ref _voiceChat);

        /// <summary>
        /// 支持语音
        /// </summary>
        public bool supportVoice => _voiceChat && _voiceChat.supportMicrophone;

        /// <summary>
        /// 在语音录制中
        /// </summary>
        public bool isVoiceRecording => supportVoice && _voiceChat.isVoiceRecording;

        /// <summary>
        /// 启用语音
        /// </summary>
        public bool enableVoice
        {
            get => contentType == EContentType.Voice;
            set
            {
                if (value && supportVoice) contentType = EContentType.Voice;
                else
                {
                    StopRecord();
                    contentType = EContentType.Text;
                }

                this.CallUnityObjectPropertyChangedEvent(nameof(enableVoice));
            }
        }

        /// <summary>
        /// 启用录制
        /// </summary>
        /// <returns></returns>
        public bool enableRecord
        {
            get => isVoiceRecording;
            set
            {
                if (value) StartRecord();
                else StopRecord();
                this.CallUnityObjectPropertyChangedEvent(nameof(enableRecord));
            }
        }

        /// <summary>
        /// 启用语音并开始录音
        /// </summary>
        public void EnableVoiceAndStartRecord()
        {
            enableVoice = true;
            StartRecord();
        }

        /// <summary>
        /// 开始录音：开启语音的录;如果未启用语音时，无法开始录音；
        /// </summary>
        public void StartRecord()
        {
            if (enableVoice 
                && MMOHelper.isEnteredRoom 
                && supportVoice 
                && !isVoiceRecording
                && _voiceChat.StartVoice())
            {
                InternalSend("", EContentType.Voice, _voiceChat.beginSummary, EContentState.Begin);

                AutoStopRecordOnMaxRecordDuration();
                this.CallUnityObjectPropertyChangedEvent();
            }
        }

        /// <summary>
        /// 停止录音：结束语音的录制
        /// </summary>
        public void StopRecord()
        {
            if (enableVoice && supportVoice && isVoiceRecording)
            {
                InternalSend("", EContentType.Voice, _voiceChat.endSummary, EContentState.End);

                _voiceChat.StopVoice();
                this.CallUnityObjectPropertyChangedEvent();
            }
        }

        /// <summary>
        /// 停止录音并发送语音
        /// </summary>
        public void StopRecordAndSendVoice()
        {
            if (enableVoice && supportVoice)
            {
                StopRecord();

                //var b = DateTime.Now;
                if (_voiceChat.TryEncodeToVoiceText(out var voiceText, out var voiceDuration))
                {
                    //Debug.Log("Send 0: "+(DateTime.Now - b).TotalSeconds);
                    InternalSend(voiceText, EContentType.Voice, voiceDuration.ToString(), EContentState.Content);
                    //Debug.Log("Send 1: " + (DateTime.Now - b).TotalSeconds);
                }
                //Debug.Log("Send 2: " + (DateTime.Now - b).TotalSeconds);

                this.CallUnityObjectPropertyChangedEvent();
            }
        }

        /// <summary>
        /// 停止录音并发送语音之后禁用语音
        /// </summary>
        public void StopRecordAndSendVoiceThenDisableVoice()
        {
            StopRecordAndSendVoice();
            enableVoice = false;
        }

        /// <summary>
        /// 自动停止录音当最大录音时长时:在开始录音之后调用
        /// </summary>
        private void AutoStopRecordOnMaxRecordDuration()
        {
            if (isVoiceRecording)
            {
                var waitTime = _voiceChat.voiceMaxDuration - _voiceChat.voiceRecordDuration;
                if (waitTime > 0)
                {
                    CommonFun.DelayCall(StopRecord, waitTime, nameof(AutoStopRecordOnMaxRecordDuration));
                }
                else
                {
                    StopRecord();
                }
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="chatContent"></param>
        public void Play(ChatContent chatContent)
        {
            if (_voiceChat)
            {
                _voiceChat.Play(chatContent);
            }
        }

        /// <summary>
        /// 播放已选择的
        /// </summary>
        public void PlaySelected() => Play(selectedChatContent);

        #endregion
    }
}
