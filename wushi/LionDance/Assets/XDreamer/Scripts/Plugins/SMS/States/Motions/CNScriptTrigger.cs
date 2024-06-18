using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCNScripts;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 脚本触发
    /// </summary>
    [Name("脚本触发")]
    public enum ECNScriptTrigger
    {
        /// <summary>
        /// 工作剪辑触发
        /// </summary>
        [Name("工作剪辑触发")]
        [Tip("在工作剪辑的某个百分比上触发", "Triggered on a percentage of the working clip")]
        OnTrigger,
    }

    /// <summary>
    /// 脚本触发函数
    /// </summary>
    [Name("脚本触发函数")]
    [Serializable]
    public class CNScriptTriggerFunction : EnumFunction<ECNScriptTrigger> { }

    /// <summary>
    /// 脚本触发函数集合
    /// </summary>
    [Name("脚本触发函数集合")]
    [Serializable]
    public class CNScriptTriggerFunctionCollection : EnumFunctionCollection<ECNScriptTrigger, CNScriptTriggerFunction> { }

    /// <summary>
    /// 脚本触发:使用中文脚本编写控制逻辑,并在某个区间中触发
    /// </summary>
    [Serializable]
    [ComponentMenu(CNScriptCategory.TitleDirectory + Title, typeof(ScriptManager))]
    [Name(Title, nameof(CNScriptTrigger))]
    [Tip("使用中文脚本编写控制逻辑,并在某个区间中触发", "Use Chinese script to write control logic and trigger it in a certain interval")]
    [XCSJ.Attributes.Icon(EIcon.CNScript)]
    [KeyNode(nameof(EIcon.Play), "工作剪辑")]
    public class CNScriptTrigger : StateScriptComponent<CNScriptTrigger, ECNScriptTrigger, CNScriptTriggerFunction, CNScriptTriggerFunctionCollection>, 
        IWorkClip, 
        ITriggerPoint
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "脚本触发";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(CNScriptCategory.Title, typeof(ScriptManager))]
        [StateComponentMenu(CNScriptCategory.TitleDirectory + Title, typeof(ScriptManager))]
        [Name(Title, nameof(CNScriptTrigger))]
        [Tip("使用中文脚本编写控制逻辑,并在某个区间中触发", "Use Chinese script to write control logic and trigger it in a certain interval")]
        [XCSJ.Attributes.Icon(EIcon.CNScript)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 工作区间
        /// </summary>
        [Group("区间触发")]
        [Name("工作区间")]
        [Tip("当前组件在状态生命周期内的工作区间(时间与百分比)信息", "Information about the working interval (time and percentage) of the current component in the state life cycle")]
        [OnlyMemberElements]
        public WorkRange workRange = new WorkRange();

        /// <summary>
        /// 循环
        /// </summary>
        public bool loop => false;

        /// <summary>
        /// 单次时长
        /// </summary>
        public double onceTimeLength => workRange.totalTimeLength;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            _progress = 0;
            base.OnEntry(data);
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            SetTimeOfState(parent.timeLengthWithSpeedSinceEntry);
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);
        }

        #region 工作剪辑 && 进度

        /// <summary>
        /// 总时长
        /// </summary>
        public double totalTimeLength
        {
            get { return workRange.totalTimeLength; }
            set { workRange.totalTimeLength = value; }
        }

        /// <summary>
        /// 进度
        /// </summary>
        protected double _progress = 0;

        /// <summary>
        /// 进度
        /// </summary>
        public override double progress
        {
            get { return MathX.Clamp01(_progress); }
            set { SetPercent(value); }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public virtual double beginTime
        {
            get { return workRange.timeRange.timeRange.x; }
            set { workRange.timeRange.timeRange.x = value; }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public virtual double endTime
        {
            get { return workRange.timeRange.timeRange.y; }
            set { workRange.timeRange.timeRange.y = value; }
        }

        /// <summary>
        /// 时长
        /// </summary>
        public virtual double timeLength
        {
            get { return endTime - beginTime; }
            set { endTime = beginTime + value; }
        }

        /// <summary>
        /// 开始百分比
        /// </summary>
        public virtual double beginPercent
        {
            get { return workRange.percentRange.percentRange.x; }
            set { workRange.percentRange.percentRange.x = value; }
        }

        /// <summary>
        /// 结束百分比
        /// </summary>
        public virtual double endPercent
        {
            get { return workRange.percentRange.percentRange.y; }
            set { workRange.percentRange.percentRange.y = value; }
        }

        /// <summary>
        /// 百分比长
        /// </summary>
        public virtual double percentLength
        {
            get { return endPercent - beginPercent; }
            set { endPercent = MathX.Clamp01(beginPercent + value); }
        }

        /// <summary>
        /// 设置状态时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public virtual bool SetTimeOfState(double time) => SetTimeOfState(time, null);

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public virtual bool SetTime(double time) => SetTime(time, null);

        /// <summary>
        /// 设置状态百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public virtual bool SetPercentOfState(double percent) => SetPercentOfState(percent, null);

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public virtual bool SetPercent(double percent) => SetPercent(percent, null);
        
        /// <summary>
        /// 检查完成
        /// </summary>
        protected virtual void CheckFinished()
        {
            if (!finished)
            {
                finished = MathX.Approximately(progress, 1) || MathX.ApproximatelyZero(timeLength);
            }
        }

        /// <summary>
        /// 有效
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public bool Validity(double ttl) => MathX.Approximately(ttl, totalTimeLength, WorkClip.Epsilon) && WorkClip.WorkClipValidity(this);

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetTime(double time, StateData stateData)
        {
            return SetPercent(workRange.timeRange.NormalizeOfRelativeLeft(time), stateData);
        }

        /// <summary>
        /// 设置状态时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetTimeOfState(double time, StateData stateData)
        {
            return SetTime(time - beginTime, stateData);
        }

        /// <summary>
        /// 设置状态百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetPercentOfState(double percent, StateData stateData)
        {
            return SetPercent(workRange.percentRange.Normalize(percent), stateData);
        }

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetPercent(double percent, StateData stateData)
        {
            if (!valid) return false;

            // 初始化过后
            if (lastPercent >= 0)
            {
                switch (direction)
                {
                    case ETriggerDirection.Increase:
                        {
                            if (triggerPercent > lastPercent && triggerPercent < percent)
                            {
                                OnTrigger();
                            }
                            break;
                        }
                    case ETriggerDirection.Descending:
                        {
                            if (triggerPercent < lastPercent && triggerPercent > percent)
                            {
                                OnTrigger();
                            }
                            break;
                        }
                    case ETriggerDirection.Both:
                        {
                            if ((triggerPercent > lastPercent && triggerPercent < percent) ||
                                (triggerPercent < lastPercent && triggerPercent > percent))
                            {
                                OnTrigger();
                            }
                            break;
                        }
                }
            }
            lastPercent = percent;

            _progress = percent;
            CheckFinished();
            return true;
        }

        #endregion

        /// <summary>
        /// 触发百分比
        /// </summary>
        [Name("触发百分比")]
        [Range(0, 1)]
        public double _triggerPercent = 0.05f;

        /// <summary>
        /// 递增方向
        /// </summary>
        [Name("递增方向")]
        [EnumPopup]
        public ETriggerDirection _direction = ETriggerDirection.Increase;

        /// <summary>
        /// 上次百分比
        /// </summary>
        protected double lastPercent = 0;

        private bool _valid = true;

        /// <summary>
        /// 当触发
        /// </summary>
        protected virtual void OnTrigger()
        {
            ExecuteScriptEvent(ECNScriptTrigger.OnTrigger);
        }

        /// <summary>
        /// 当播放器状态已变更
        /// </summary>
        /// <param name="player"></param>
        /// <param name="lastPlayerState"></param>
        public void OnPlayerStateChanged(IWorkClipPlayer player, EPlayerState lastPlayerState) { }

        void IPlayerEvent.OnPlayerStateChanged(IPlayer player, EPlayerState lastPlayerState) => OnPlayerStateChanged(player as IWorkClipPlayer, lastPlayerState);

        /// <summary>
        /// 当越界
        /// </summary>
        /// <param name="player"></param>
        /// <param name="outOfBoundsMode"></param>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        /// <param name="lastPercent"></param>
        /// <param name="stateWorkClip"></param>
        public void OnOutOfBounds(IWorkClipPlayer player, EOutOfBoundsMode outOfBoundsMode, double percent, StateData stateData, double lastPercent, IStateWorkClip stateWorkClip) { }

        /// <summary>
        /// 触发百分比
        /// </summary>
        public virtual double triggerPercent
        {
            get { return this._triggerPercent; }
            set { this._triggerPercent = value; }
        }

        /// <summary>
        /// 方向
        /// </summary>
        public virtual ETriggerDirection direction
        {
            get { return this._direction; }
            set { this._direction = value; }
        }

        /// <summary>
        /// 有效
        /// </summary>
        public virtual bool valid
        {
            get { return _valid; }
            set { _valid = value; }
        }
    }
}
