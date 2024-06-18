using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.TimeLine;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 根步骤组
    /// </summary>
    public abstract class StepGroupRoot : StepGroup
    {
        /// <summary>
        /// 播放内容
        /// </summary>
        [Name("播放内容")]
        [Tip("关联播放内容，让计划随着时间轴播放器播放移动播放步骤", "Associate the playback content and let the plan move the playback step with the timeline player")]
        //[ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup(typeof(TimeLinePlayContent), stateCollectionType = EStateCollectionType.Root)]
        public TimeLinePlayContent timeLinePlayContent;

        /// <summary>
        /// 节点类型
        /// </summary>
        public override ETreeNodeType nodeType => ETreeNodeType.Root;

        /// <summary>
        /// 当创建
        /// </summary>
        public override void OnCreated()
        {
            base.OnCreated();

            if (!timeLinePlayContent) timeLinePlayContent = GetComponent<TimeLinePlayContent>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            BindPlayContent();
            return base.Init(data);
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            // 状态激活后，不在与播放内容关联，防止出现错误播放
            UnBindPlayContent();
            
            base.OnEntry(data);
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            BindPlayContent();
            
            base.OnExit(data);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => !string.IsNullOrEmpty(description);

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => "";

        private event Action<State[]> onStateChanged;

        private void BindPlayContent()
        {
            if (timeLinePlayContent)
            {
                // 播放器 => 步骤组
                timeLinePlayContent.onPlayContentElementChanged += OnPlayContentElementChanged;
                timeLinePlayContent.onPlay += OnPlay;
                timeLinePlayContent.onStop += OnStop;

                // 步骤组 => 播放内容
                this.onStateChanged += timeLinePlayContent.PlayContentElements;
            }
        }

        private void UnBindPlayContent()
        {
            if (timeLinePlayContent)
            {
                timeLinePlayContent.onPlayContentElementChanged -= OnPlayContentElementChanged;
                timeLinePlayContent.onPlay -= OnPlay;
                timeLinePlayContent.onStop -= OnStop;
                this.onStateChanged -= timeLinePlayContent.PlayContentElements;
            }
        }

        /// <summary>
        /// 播放器 => 步骤组
        /// </summary>
        public void OnPlayContentElementChanged(TimeLinePlayContent timeLinePlayContent, List<State> lastElements, List<State> newElements, double percent)
        {
            playContentChanged = true;
            foreach (var s in newElements)
            {
                SetCurrent(s);
            }
            playContentChanged = false;
        }

        private bool playContentChanged = false;

        /// <summary>
        /// 选择
        /// </summary>
        public override void Select()
        {
            base.Select();

            if (!playContentChanged)
            {
                onStateChanged?.Invoke(GetSelectedStates().ToArray());
            }
        }

        /// <summary>
        /// 当播放
        /// </summary>
        /// <param name="timeLinePlayContent"></param>
        public void OnPlay(TimeLinePlayContent timeLinePlayContent) { }

        /// <summary>
        /// 当停止
        /// </summary>
        /// <param name="timeLinePlayContent"></param>
        public void OnStop(TimeLinePlayContent timeLinePlayContent) { UnSelect(); }

        /// <summary>
        /// 转到状态
        /// </summary>
        /// <param name="state"></param>
        public override void GotoState(State state)
        {
            if (timeLinePlayContent)
            {
                timeLinePlayContent.SetPercent(state);
            }
        }
    }
}
