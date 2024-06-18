using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Audios;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.PluginMMO.Chats
{
    /// <summary>
    /// Unity语音聊天
    /// </summary>
    [DisallowMultipleComponent]
    [Name("Unity语音聊天")]
    [RequireComponent(typeof(AudioSource))]
    public sealed class UnityVoiceChat : VoiceChat
    {
        /// <summary>
        /// 概要前缀
        /// </summary>
        public const string SummaryPrefix = nameof(UnityVoiceChat) + "-";

        /// <summary>
        /// 开始概要
        /// </summary>
        public override string beginSummary => SummaryPrefix;

        /// <summary>
        /// 结束概要
        /// </summary>
        public override string endSummary => SummaryPrefix;

        #region Unity 消息

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!audioSource)
            {
                audioSource = GetComponent<AudioSource>();
            }
#if UNITY_WEBGL && !UNITY_EDITOR
#else
            //获取设备麦克风
            var devices = Microphone.devices;
            if (devices == null || devices.Length == 0)
            {
                Log.Warning("当前设备上未找到有效的麦克风！");
                validDevice = false;
            }
            else
            {
                validDevice = true;
                if (Array.IndexOf(devices, device) < 0)
                {
                    device = devices[0];
                }
            }
#endif
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
            if (!isVoicePlaying && !isVoiceRecording)
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

        #endregion

        #region 扬声器：语音消息播放

        /// <summary>
        /// 支持扬声器
        /// </summary>
        public override bool supportSpeaker => audioSource;

        /// <summary>
        /// 是语音播放中
        /// </summary>
        public override bool isVoicePlaying => audioSource && (audioSource.isPlaying || pausePlay);

        /// <summary>
        /// 音频源
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// Unity音频数据
        /// </summary>
        class UnityAudioData : VoiceData
        {
            /// <summary>
            /// 音频剪辑
            /// </summary>
            public AudioClip audioClip;

            /// <summary>
            /// 语音时长：单位为秒；
            /// </summary>
            public override float voiceDuration => audioClip ? audioClip.length : 0;
        }

        /// <summary>
        /// 内部播放
        /// </summary>
        /// <param name="chatContent"></param>
        private void InternalPlay(ChatContent chatContent)
        {
            if (audioSource)
            {
                Debug.Log(nameof(UnityVoiceChat) + ".summary: " + chatContent.summary);
                Debug.Log(nameof(UnityVoiceChat) + ".content: " + chatContent.content);

                if (chatContent.voiceData == null)
                {
                    UnityAudioData data0 = new UnityAudioData();
                    chatContent.voiceData = data0;

                    if (chatContent.content.TryDecodeFromWAV64(out AudioClip audioClip))
                    {
                        data0.audioClip = audioClip;
                    }
                }
                
                if (chatContent.voiceData is UnityAudioData data && data.audioClip)
                {
                    audioSource.clip = data.audioClip;
                    audioSource.Play();
                }
            }
        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        private bool pausePlay = false;

        /// <summary>
        /// 内部暂停播放
        /// </summary>
        private void InternalPausePlay()
        {
            if (isVoicePlaying)
            {
                pausePlay = true;
                audioSource.Pause();
            }
        }

        /// <summary>
        /// 内部取消暂停播放：继续播放
        /// </summary>
        private void InternalUnPausePlay()
        {
            if (audioSource && pausePlay)
            {
                audioSource.UnPause();
                pausePlay = false;
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
                && chatContent.contentState == EContentState.Content
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
        [Range(5, 60)]
        public int _voiceMaxDuration = 15;

        /// <summary>
        /// 频率
        /// </summary>
        [Name("频率")]
        [Tip("音频采样频率", "Audio sampling frequency")]
        public int _frequency = 44100;

        /// <summary>
        /// 循环
        /// </summary>
        [Name("循环")]
        [Tip("标识在达到音频时长时是否应继续录制，并从音频剪辑的开头开始环绕和录制；", "Identify whether recording should continue when the audio duration is reached, and surround and record from the beginning of the audio clip;")]
        public bool _loop = false;

        /// <summary>
        /// 音频剪辑
        /// </summary>
        [Readonly]
        [Name("音频剪辑")]
        public AudioClip _audioClip;

        /// <summary>
        /// 设备
        /// </summary>
        [Readonly]
        [Name("设备")]
        public string _device = "";

        private bool validDevice = false;

        /// <summary>
        /// 设备
        /// </summary>
        public string device
        {
            get => validDevice ? _device : "";
            set => _device = value;
        }

        /// <summary>
        /// 音频数据长度:单位：Float4字节；
        /// </summary>
        [Readonly]
        [Name("音频数据长度")]
        public int _audioDataLength = 0;

#if UNITY_WEBGL && !UNITY_EDITOR
#else

        /// <summary>
        /// 启动时间
        /// </summary>
        private DateTime startTime;
        
        /// <summary>
        /// 停止时间
        /// </summary>
        private DateTime stopTime;

#endif

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
#if UNITY_WEBGL && !UNITY_EDITOR
                return 0;
#else
                return Math.Min((float)(isVoiceRecording ? (DateTime.Now - startTime).TotalSeconds : _audioDataLength > 0 ? (stopTime - startTime).TotalSeconds : 0), voiceMaxDuration);
#endif
            }
        }

        /// <summary>
        /// 支持麦克风
        /// </summary>
        public override bool supportMicrophone => !string.IsNullOrEmpty(device);

        /// <summary>
        /// 是语音记录中
        /// </summary>
        public override bool isVoiceRecording
        {
            get
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                return false;
#else
                return supportMicrophone && _audioClip && Microphone.IsRecording(device);
#endif
            }
        }

        /// <summary>
        /// 启动语音：开启语音的录制
        /// </summary>
        /// <returns></returns>
        public override bool StartVoice()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
#else
            InternalPausePlay();
            if (supportMicrophone && !Microphone.IsRecording(device))
            {
                startTime = DateTime.Now;

                _audioDataLength = 0;
                _audioClip = Microphone.Start(device, _loop, _voiceMaxDuration, _frequency);
                return _audioClip;
            }
#endif
            return false;
        }

        /// <summary>
        /// 停止语音：结束语音的录制
        /// </summary>
        public override void StopVoice()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
#else
            if (isVoiceRecording)
            {
                stopTime = DateTime.Now;
                _audioDataLength = Microphone.GetPosition(device);
                Microphone.End(device);
            }
            InternalUnPausePlay();
#endif
        }

        /// <summary>
        /// 尝试编码为语音文本
        /// </summary>
        /// <param name="voiceText"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        public override bool TryEncodeToVoiceText(out string voiceText, out string summary)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
#else
            if (supportMicrophone && _audioClip && !Microphone.IsRecording(device) && _audioDataLength > 0)
            {
                var oldAudioClip = _audioClip;
                _audioClip = null;

                var datas = new float[_audioDataLength];
                oldAudioClip.GetData(datas, 0);

                var newAudioClip = AudioClip.Create(nameof(UnityVoiceChat), _audioDataLength, oldAudioClip.channels, oldAudioClip.frequency, false);
                newAudioClip.SetData(datas, 0);

                summary = SummaryPrefix + newAudioClip.length.ToString();
                try
                {
                    return newAudioClip.TryEncodeToWAV64(out voiceText);
                }
                catch (Exception ex)
                {
                    ex.HandleException(nameof(TryEncodeToVoiceText));
                }
                finally
                {
                    oldAudioClip.XDestoryObject();
                    newAudioClip.XDestoryObject();
                }
            }
#endif
            voiceText = default;
            summary = default;
            return false;

        }

        #endregion
    }
}
