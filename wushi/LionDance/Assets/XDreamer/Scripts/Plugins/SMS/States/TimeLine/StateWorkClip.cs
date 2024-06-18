using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.PluginSMS.States.TimeLine
{
    /// <summary>
    /// 状态工作剪辑
    /// </summary>
    [Name("状态工作剪辑")]
    [Serializable]
    public class StateWorkClip : IStateWorkClip
    {
        /// <summary>
        /// 状态
        /// </summary>
        [Name("状态")]
        [SerializeField]
        public State state = null;

        /// <summary>
        /// 工作区间
        /// </summary>
        [Name("工作区间")]
        [Tip("播放控制主要信息保存数据结构", "Playback control main information saving data structure")]
        [SerializeField]
        public WorkRange workRange = new WorkRange();

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get => state.name; set { } }

        /// <summary>
        /// 速度
        /// </summary>
        [Name("速度")]
        public double speed = 1;

        /// <summary>
        /// 循环次数
        /// </summary>
        public double loopCount
        {
            get => MathX.Scale(speed * timeLength, onceTimeLength);
            set => speed = MathX.Scale(onceTimeLength * value, timeLength, 1);
        }

        private StateWorkClipSet _stateWorkClipSet;

        /// <summary>
        /// 状态工作剪辑集
        /// </summary>
        public StateWorkClipSet stateWorkClipSet
        {
            get
            {
                if (!_stateWorkClipSet)
                {
                    _stateWorkClipSet = state.GetComponent<StateWorkClipSet>();
                }
                return _stateWorkClipSet;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="state"></param>
        public StateWorkClip(State state)
        {
            this.state = state;
        }

        /// <summary>
        /// 状态到百分比
        /// </summary>
        /// <param name="state"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public bool StateToPercent(State state, ref double percent)
        {
            if (stateWorkClipSet && stateWorkClipSet.StateToPercent(state, ref percent))
            {
                percent = percent * oncePercentLengthWithSpeed + beginPercent;
                return true;
            }
            else if (this.state == state)
            {
                percent = beginPercent;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 百分比到状态
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="outStates"></param>
        public void PercentToState(double percent, List<State> outStates)
        {
            if (workRange.percentRange.In(percent))
            {
                if (stateWorkClipSet)
                {
                    stateWorkClipSet.PercentToStates(GetNormalizeLocalPercentOfLoop(percent), outStates);
                }
                else
                {
                    outStates.Add(state);
                }
            }
        }

        /// <summary>
        /// 获取循环的规格化局部百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        protected double GetNormalizeLocalPercentOfLoop(double percent)
        {
            var lp = GetLocalPercent(percent);

            return Percent.Loop01(lp);
        }

        /// <summary>
        /// 获取本地百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        protected double GetLocalPercent(double percent)
        {
            var v = percentLength * onceTimeLength;
            return MathX.ApproximatelyZero(v) ? 0 : ((percent - beginPercent) * timeLength * speed / v);
        }

        /// <summary>
        /// 有效触发点
        /// </summary>
        /// <param name="valid"></param>
        public void ValidTriggerPoint(bool valid)
        {
            if (state)
            {
                foreach (var c in state.components)
                {
                    if (c is ITriggerPoint triggerPoint)
                    {
                        triggerPoint.valid = valid;
                    }
                }
            }
        }

        #region IStateWorkClip

        /// <summary>
        /// 总时长
        /// </summary>
        public double totalTimeLength { get { return timeLength; } set { timeLength = value; } }

        /// <summary>
        /// 开始时间
        /// </summary>
        public double beginTime { get { return workRange.timeRange.timeRange.x; } set { workRange.timeRange.timeRange.x = value; } }

        /// <summary>
        /// 结束时间
        /// </summary>
        public double endTime { get { return workRange.timeRange.timeRange.y; } set { workRange.timeRange.timeRange.y = value; } }

        /// <summary>
        /// 时长
        /// </summary>
        public double timeLength { get { return workRange.timeRange.length; } set { workRange.timeRange.timeRange.y = workRange.timeRange.timeRange.x + value; } }

        /// <summary>
        /// 开始百分比
        /// </summary>
        public double beginPercent { get { return workRange.percentRange.percentRange.x; } set { workRange.percentRange.percentRange.x = value; } }

        /// <summary>
        /// 结束百分比
        /// </summary>
        public double endPercent { get { return workRange.percentRange.percentRange.y; } set { workRange.percentRange.percentRange.y = value; } }

        /// <summary>
        /// 百分比时长
        /// </summary>
        public double percentLength { get { return workRange.percentRange.length; } set { workRange.percentRange.percentRange.y = workRange.percentRange.percentRange.x + value; } }

        /// <summary>
        /// 循环
        /// </summary>
        public bool loop => !MathX.Approximately(loopCount, 1);

        /// <summary>
        /// 单次时长
        /// </summary>
        public double onceTimeLength => state.onceTimeLength;

        /// <summary>
        /// 带速度的单次时长
        /// </summary>
        public double onceTimeLengthWithSpeed => state.onceTimeLength / speed;

        /// <summary>
        /// 档次时长
        /// </summary>
        public double oncePercentLength
        {
            get
            {
                var tl = timeLength;
                return MathX.ApproximatelyZero(tl) ? 0 : (percentLength * onceTimeLength / tl);
            }
        }

        /// <summary>
        /// 带速度单次百分比长
        /// </summary>
        public double oncePercentLengthWithSpeed
        {
            get
            {
                var tl = timeLength * speed;
                return MathX.ApproximatelyZero(tl) ? 0 : (percentLength * onceTimeLength / tl);
            }
        }

        double ISpeed.speed { get => speed; set => speed = value; }
        State IStateWorkClip.state { get => state; set => state = value; }

        /// <summary>
        /// 设置时长
        /// </summary>
        /// <param name="timeLength"></param>
        /// <param name="ttl"></param>
        public void SetTimeLength(double timeLength, double ttl)
        {
            this.timeLength = timeLength;
            this.endPercent = this.endTime / ttl;
        }

        /// <summary>
        /// 设置状态的时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetTimeOfState(double time) => SetTime(time - beginTime);

        /// <summary>
        /// 设置状态的时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetTimeOfState(double time, StateData stateData) => SetTime(time - beginTime, stateData);

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetTime(double time)
        {
            var otl = onceTimeLength;
            return SetPercent(MathX.ApproximatelyZero(otl) ? 0 : (time * speed / otl));
        }

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetTime(double time, StateData stateData)
        {
            var otl = onceTimeLength;
            return SetPercent(MathX.ApproximatelyZero(otl) ? 0 : (time * speed / otl), stateData);
        }

        /// <summary>
        /// 设置状态的百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public bool SetPercentOfState(double percent) => SetPercent(GetLocalPercent(percent));

        /// <summary>
        /// 设置状态的百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetPercentOfState(double percent, StateData stateData) => SetPercent(GetLocalPercent(percent), stateData);

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public bool SetPercent(double percent) => state.SetProgress(percent);

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetPercent(double percent, StateData stateData) => state.SetPercent(percent, stateData);

        /// <summary>
        /// 当进入设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        public void OnEntrySetPercent(double percent, StateData stateData)
        {
            state.OnEntry(stateData);
            SetPercentOfState(percent, stateData);
        }

        /// <summary>
        /// 当退出设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        public void OnExitSetPercent(double percent, StateData stateData)
        {
            SetPercentOfState(percent, stateData);
            state.OnExit(stateData);
        }

        /// <summary>
        /// 当越界发生时回调；
        /// </summary>
        /// <param name="player">工作剪辑播放器对象</param>
        /// <param name="outOfBoundsMode">越界模式</param>
        /// <param name="percent">当前百分比</param>
        /// <param name="stateData">状态数据对象</param>
        /// <param name="lastPercent">上一次的百分比</param>
        /// <param name="stateWorkClip">状态工作剪辑对象</param>
        public void OnOutOfBounds(IWorkClipPlayer player, EOutOfBoundsMode outOfBoundsMode, double percent, StateData stateData, double lastPercent, IStateWorkClip stateWorkClip)
        {
            state.OnOutOfBounds(player, outOfBoundsMode, percent, stateData, lastPercent, stateWorkClip);
        }

        /// <summary>
        /// 当工作剪辑播放器的播放状态发生变化时回调
        /// </summary>
        /// <param name="player">工作剪辑播放器对象</param>
        /// <param name="lastPlayerState">上次的工作剪辑播放器的播放状态</param>
        public void OnPlayerStateChanged(IWorkClipPlayer player, EPlayerState lastPlayerState)
        {
            state.OnPlayerStateChanged(player, lastPlayerState);
        }

        void IPlayerEvent.OnPlayerStateChanged(IPlayer player, EPlayerState lastPlayerState) => OnPlayerStateChanged(player as IWorkClipPlayer, lastPlayerState);

        /// <summary>
        /// 有效的
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public bool Validity(double ttl) => MathX.Approximately(ttl, totalTimeLength, WorkClip.Epsilon) && WorkClip.WorkClipValidity(this);

        #endregion       

        /// <summary>
        /// 重置时长
        /// </summary>
        /// <param name="parentTimeLength"></param>
        public void ResetTimeLength(double parentTimeLength)
        {
            beginTime = parentTimeLength * beginPercent;
            endTime = parentTimeLength * endPercent;
        }
    }
}
