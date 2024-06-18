using UnityEngine.Playables;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;


namespace XCSJ.PluginSMS.States.MultiMedia
{
    /// <summary>
    /// 可播放导引器事件：捕获并触发可播放导引器事件
    /// </summary>
    [ComponentMenu(SMSCategory.MultiMediaDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(PlayableDirector))]
    [Tip("捕获并触发可播放导引器事件", "Capture and trigger playable navigator events")]
    [XCSJ.Attributes.Icon(EIcon.Timer)]
    public class XPlayableDirectorEvent : Trigger<XPlayableDirectorEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "可播放导引器事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.MultiMedia, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.MultiMediaDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(PlayableDirector))]
        [Tip("捕获并触发可播放导引器事件", "Capture and trigger playable navigator events")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateAudio(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 可播放导引器
        /// </summary>
        [Name("可播放导引器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(PlayableDirector))]
        public PlayableDirector playableDirector;

        /// <summary>
        /// 可播放导引器事件
        /// </summary>
        [Name(Title)]
        public enum EPlayableDirectorEvent
        {
            /// <summary>
            /// 任意
            /// </summary>
            [Name("任意")]
            [Tip("有任意事件发生时均触发", "Triggered when any event occurs")]
            Any = -1,

            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            [Tip("不捕获任意事件", "Do not capture any events")]
            None = 0,

            /// <summary>
            /// 播放
            /// </summary>
            [Name("播放")]
            [Tip("播放时触发", "Triggered during playback")]
            Played,

            /// <summary>
            /// 暂停
            /// </summary>
            [Name("暂停")]
            [Tip("暂停时触发", "Triggered on pause")]
            Paused,

            /// <summary>
            /// 停止
            /// </summary>
            [Name("停止")]
            [Tip("停止时触发", "Triggered when stopped")]
            Stopped,
        }

        /// <summary>
        /// 可播放导引器事件
        /// </summary>
        [Name(Title)]
        [Tip("期望捕获的事件类型", "Type of event expected to be captured")]
        [EnumPopup]
        public EPlayableDirectorEvent playableDirectorEvent = EPlayableDirectorEvent.Played;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            if (playableDirector)
            {
                playableDirector.played += OnPlayed;
                playableDirector.paused += OnPaused;
                playableDirector.stopped += OnStopped;
            }
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            if (playableDirector)
            {
                playableDirector.played -= OnPlayed;
                playableDirector.paused -= OnPaused;
                playableDirector.stopped -= OnStopped;
            }
        }

        private void OnPlayed(PlayableDirector playableDirector)
        {
            switch (playableDirectorEvent)
            {
                case EPlayableDirectorEvent.Any:
                case EPlayableDirectorEvent.Played:
                    {
                        finished = true;
                        break;
                    }
            }
        }

        private void OnPaused(PlayableDirector playableDirector)
        {
            switch (playableDirectorEvent)
            {
                case EPlayableDirectorEvent.Any:
                case EPlayableDirectorEvent.Paused:
                    {
                        finished = true;
                        break;
                    }
            }
        }

        private void OnStopped(PlayableDirector playableDirector)
        {
            switch (playableDirectorEvent)
            {
                case EPlayableDirectorEvent.Any:
                case EPlayableDirectorEvent.Stopped
:
                    {
                        finished = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            if (!playableDirector) return "";
            return playableDirector.name + " " + CommonFun.Name(playableDirectorEvent);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && playableDirector;
        }
    }
}
