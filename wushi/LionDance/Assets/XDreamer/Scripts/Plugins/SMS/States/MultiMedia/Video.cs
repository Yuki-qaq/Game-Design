using UnityEngine;
using UnityEngine.Video;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.MultiMedia
{
    /// <summary>
    /// 视频：视频组件是播放Unity的VideoPlayer的对象。播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSCategory.MultiMediaDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(Video))]
    [Tip("视频组件是播放Unity的VideoPlayer的对象。播放完毕后，组件切换为完成态。", "The video playback object of unitovideo player is. After playing, the component switches to the finished state.")]
    [XCSJ.Attributes.Icon(EIcon.Video)]
    public class Video : WorkClip<Video>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "视频";

        /// <summary>
        /// 创建状态
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.MultiMedia, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.MultiMedia + "/视频", typeof(SMSManager))]
        [Name("视频", nameof(Video))]
        [Tip("视频组件是播放Unity的VideoPlayer的对象。播放完毕后，组件切换为完成态。", "The video playback object of unitovideo player is. After playing, the component switches to the finished state.")]
        [XCSJ.Attributes.Icon(EIcon.Video)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 视频播放器
        /// </summary>
        [Group("视频属性")]
        [Name("视频播放器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(VideoPlayer))]
        public VideoPlayer videoPlayer;

        /// <summary>
        /// 时间容差
        /// </summary>
        [Name("时间容差")]
        [Tip("当视频当前播放时间与期望时间在时间容差内时，不更新视频的播放时间;", "When the current playback time of the video is within the time tolerance of the expected time, the playback time of the video is not updated;")]
        [Range(0.001f, 1)]
        public float timeTolerance = 0.05f;

        /// <summary>
        /// 无效
        /// </summary>
        public bool invalid => !videoPlayer || !videoPlayer.clip || MathX.ApproximatelyZero(timeLength) || MathX.ApproximatelyZero(videoPlayer.clip.length);

        /// <summary>
        /// 当设置百分比时触发播放：设置百分比时，如果没有播放，则设定播放
        /// </summary>
        public bool triggerPlayWhenSetPercent { get; set; } = true;

        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            if (invalid) return;
            //对视频速度做调整
            videoPlayer.playbackSpeed = (float)MathX.Scale(videoPlayer.clip.length, timeLength, 1, MathX.FloatCompareEpsilon);
            videoPlayer.Play();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (invalid) return;
            videoPlayer.frame = 0;
            videoPlayer.Stop();
        }

        /// <summary>
        /// 当错误
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            if (invalid) return;            
            Play();
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            Stop();
            base.OnExit(data);
        }

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            if (invalid) return;
            var length = videoPlayer.clip.length;
            var p = percent.percent01OfWorkCurve;
            if (MathX.Approximately(p, 1) || MathX.Approximately(p, 0))
            {
                Stop();
            }
            else
            {
                var frame = (long)(p * videoPlayer.frameCount);
                if (!MathX.Approximately(MathX.Scale(frame, videoPlayer.frameRate, videoPlayer.time, MathX.FloatCompareEpsilon), videoPlayer.time, timeTolerance))
                {
                    //Log.Debug("Update Video frame:"+frame);
                    videoPlayer.frame = frame;
                }
                if (!videoPlayer.isPlaying && triggerPlayWhenSetPercent) Play();
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && !invalid;
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="data"></param>
        public override void Reset(ResetData data)
        {
            base.Reset(data);
            switch (data.dataRule)
            {
                case EDataRule.Init:
                case EDataRule.Entry:
                    {
                        Stop();
                        break;
                    }
            }
        }
    }
}
