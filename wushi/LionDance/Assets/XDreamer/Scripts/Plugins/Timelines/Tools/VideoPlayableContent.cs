using UnityEngine;
using UnityEngine.Video;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTimelines.Tools
{
    /// <summary>
    /// 视频可播放内容
    /// </summary>
    [Name("视频可播放内容")]
    [Tool(TimelineCategory.MultiMedia, TimelineCategory.PlayableContent, rootType = typeof(TimelineManager))]
    [XCSJ.Attributes.Icon(EIcon.Video)]
    [RequireManager(typeof(TimelineManager))]
    [Owner(typeof(TimelineManager))]
    public class VideoPlayableContent : PlayableContent
    {
        /// <summary>
        /// 视频播放器        
        /// </summary>
        [Name("视频播放器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VideoPlayer _videoPlayer;

        /// <summary>
        /// 视频播放器
        /// </summary>
        public VideoPlayer videoPlayer
        {
            get
            {
                if (!_videoPlayer)
                {
                    this.XGetComponentInChildrenOrGlobal(ref _videoPlayer);
                }
               
                return _videoPlayer;
            }
        }

        /// <summary>
        /// 视频源
        /// </summary>
        [Name("视频源")]
        public VideoSource _videoSource = VideoSource.VideoClip;

        /// <summary>
        /// 视频剪辑
        /// </summary>
        [Name("视频剪辑")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_videoSource), EValidityCheckType.NotEqual, VideoSource.VideoClip)]
        public VideoClip _videoClip = null;

        /// <summary>
        /// 视频URL
        /// </summary>
        [Name("视频URL")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [HideInSuperInspector(nameof(_videoSource), EValidityCheckType.NotEqual, VideoSource.Url)]
        public string _videoUrl = "";

        /// <summary>
        /// 是否循环
        /// </summary>
        public override bool loop { get => videoPlayer.isLooping; set => videoPlayer.isLooping = value; }

        /// <summary>
        /// 是否处于播放态
        /// </summary>
        public override bool IsPlaying() => videoPlayer.isPlaying;

        /// <summary>
        /// 时间容差
        /// </summary>
        [Name("时间容差")]
        [Tip("当视频当前播放时间与期望时间在时间容差内时，不更新视频的播放时间;", "When the current playback time of the video is within the time tolerance of the expected time, the playback time of the video is not updated;")]
        [Range(0.001f, 1)]
        public float timeTolerance = 0.05f;

        /// <summary>
        /// 播放进度
        /// </summary>
        public override double percent
        {
            get
            {
                var vp = videoPlayer;
                if (vp)
                {
                    return vp.frame * 1.0 / vp.frameCount;
                }
                else
                {
                    return base.percent;
                }
            }
            set
            {
                var vp = videoPlayer;
                if (vp)
                {
                    var frame = (long)(value * vp.frameCount);
                    if (!MathX.Approximately(MathX.Scale(frame, vp.frameRate, vp.time, MathX.FloatCompareEpsilon), vp.time, timeTolerance))
                    {
                        vp.frame = frame;
                    }
                }
                else
                {
                    base.percent = value;
                }
            }
        }

        /// <summary>
        /// 当前播放时长
        /// </summary>
        public override double time { get => videoPlayer.time; set => videoPlayer.time = value; }

        /// <summary>
        /// 总播放时长
        /// </summary>
        public override double timeLength { get => videoPlayer.length; }

        /// <summary>
        /// 速度
        /// </summary>
        public override double speed { get => videoPlayer.playbackSpeed; set => videoPlayer.playbackSpeed = (float)value; }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (videoPlayer) { }

            _playOnEnable = false;
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!videoPlayer)
            {
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => videoPlayer && base.CanInteract(interactData);

        private void LoadVideoSource()
        {
            _videoPlayer.source = _videoSource;

            switch (_videoSource)
            {
                case VideoSource.VideoClip:
                    {
                        _videoPlayer.clip = _videoClip;
                        break;
                    }
                case VideoSource.Url:
                    {
                        _videoPlayer.url = _videoUrl;
                        break;
                    }
            }

            videoPlayer.Prepare();
        }

        private bool needLoadVideoSource = true;

        /// <summary>
        /// 当加载
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public override EInteractResult OnLoad(PlayableData playableData)
        {
            if (needLoadVideoSource)
            {
                needLoadVideoSource = false;
                LoadVideoSource();
            }
            return videoPlayer.isPrepared ? EInteractResult.Success : EInteractResult.Wait;
        }

        /// <summary>
        /// 当卸载
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public override EInteractResult OnUnload(PlayableData playableData)
        {
            needLoadVideoSource = true;
            return base.OnUnload(playableData);
        }

        /// <summary>
        /// 当播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public override EInteractResult OnPlay(PlayableData playableData)
        {
            videoPlayer.Play();

            return base.OnPlay(playableData);
        }

        /// <summary>
        /// 当暂停
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public override EInteractResult OnPause(PlayableData playableData)
        {
            videoPlayer.Pause();

            return base.OnPause(playableData);
        }

        /// <summary>
        /// 当恢复播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public override EInteractResult OnResume(PlayableData playableData)
        {
            videoPlayer.Play();

            return base.OnResume(playableData);
        }

        /// <summary>
        /// 当停止
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public override EInteractResult OnStop(PlayableData playableData)
        {
            videoPlayer.Stop();

            return base.OnStop(playableData);
        }
    }
}
