using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTimelines;

namespace XCSJ.PluginSMS.States.TimeLine
{
    /// <summary>
    /// 时间轴播放器操作:时间轴播放器操作组件是控制播放器的播放、暂停和停止的对象。控制操作完成之后，切换为完成态。
    /// </summary>
    [Name(Title, nameof(TimeLinePlayerOperation))]
    [Tip("时间轴播放器操作组件是控制播放器的播放、暂停和停止的对象。控制操作完成之后，切换为完成态。", "The timeline player operation component is an object that controls the playback, pause and stop of the player. After the control operation is completed, switch to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Play)]
    [DisallowMultipleComponent]
    [ComponentMenu("时间轴/" + Title, typeof(TimelineManager))]
    public class TimeLinePlayerOperation : LifecycleExecutor<TimeLinePlayerOperation>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "时间轴播放器操作";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(TimeLinePlayerOperation))]
        [Tip("时间轴播放器操作组件是控制播放器的播放、暂停和停止的对象。控制操作完成之后，切换为完成态。", "The timeline player operation component is an object that controls the playback, pause and stop of the player. After the control operation is completed, switch to the completed state.")]
        [XCSJ.Attributes.Icon(EIcon.Play)]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("时间轴", typeof(TimelineManager))]
        [StateComponentMenu("时间轴/" + Title, typeof(TimelineManager))]
#endif
        public static State CreateTimeLinePlayerController(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 播放器
        /// </summary>
        [Name("播放器")]
        [StateComponentPopup(typeof(TimeLinePlayer))]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public TimeLinePlayer timeLinePlayer = null;

        /// <summary>
        /// 进入播放控制
        /// </summary>
        [Name("进入播放控制")]
        [EnumPopup]
        public EPlayControl playControl = EPlayControl.Play;

        /// <summary>
        /// 播放内容
        /// </summary>
        [Name("播放内容")]
        [HideInSuperInspector(nameof(playControl), EValidityCheckType.NotEqual, EPlayControl.SetContent,
            nameof(playControl), EValidityCheckType.NotEqual, EPlayControl.SetContentAndPlay)]
        public TimeLinePlayContent playContent;

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="data"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData data, EExecuteMode executeMode)
        {
            SetPlayer(playControl);
        }

        private void SetPlayer(EPlayControl playControl)
        {
            if (!timeLinePlayer) return;

            switch (playControl)
            {
                case EPlayControl.Play:
                    {
                        timeLinePlayer.Play();
                        break;
                    }
                case EPlayControl.Pause:
                    {
                        timeLinePlayer.Pause();
                        break;
                    }
                case EPlayControl.Replay:
                    {
                        timeLinePlayer.Replay();
                        break;
                    }
                case EPlayControl.SetContent:
                    {
                        timeLinePlayer.SetPlayContent(playContent);
                        break;
                    }
                case EPlayControl.SetContentAndPlay:
                    {
                        timeLinePlayer.SetPlayContent(playContent);
                        timeLinePlayer.Play();
                        break;
                    }
                case EPlayControl.SwitchLoop:
                    {
                        timeLinePlayer.isLoop = !timeLinePlayer.isLoop;
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return timeLinePlayer;
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return (timeLinePlayer ? timeLinePlayer.parent.name : "") + "." + CommonFun.Name(playControl);
        }
    }

    /// <summary>
    /// 播放操作
    /// </summary>
    [Name("播放操作")]
    public enum EPlayControl
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 播放
        /// </summary>
        [Name("播放")]
        Play,

        /// <summary>
        /// 暂停
        /// </summary>
        [Name("暂停")]
        Pause,

        /// <summary>
        /// 重播
        /// </summary>
        [Name("重播")]
        Replay,

        /// <summary>
        /// 设置播放内容
        /// </summary>
        [Name("设置播放内容")]
        SetContent,

        /// <summary>
        /// 设置播放内容并播放
        /// </summary>
        [Name("设置播放内容并播放")]
        SetContentAndPlay,

        /// <summary>
        /// 切换循环
        /// </summary>
        [Name("切换循环")]
        SwitchLoop,

        /// <summary>
        /// 循环开启
        /// </summary>
        [Name("循环开启")]
        LoopOn,

        /// <summary>
        /// 循环关闭
        /// </summary>
        [Name("循环关闭")]
        LoopOff,
    }
}
