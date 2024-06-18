using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 触发点
    /// </summary>
    public interface ITriggerPoint
    {
        /// <summary>
        /// 触发百分比
        /// </summary>
        double triggerPercent { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        ETriggerDirection direction { get; set; }

        /// <summary>
        /// 有效
        /// </summary>
        bool valid { get; set; }
    }

    /// <summary>
    /// 触发方向
    /// </summary>
    [Name("触发方向")]
    public enum ETriggerDirection
    {
        /// <summary>
        /// 递增方向
        /// </summary>
        [Name("递增方向")]
        Increase,

        /// <summary>
        /// 递减方向
        /// </summary>
        [Name("递减方向")]
        Descending,

        /// <summary>
        /// 双向
        /// </summary>
        [Name("双向")]
        Both,
    }

    /// <summary>
    /// 工作剪辑触发器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WorkClipTriggger<T> : WorkClip<T>, ITriggerPoint where T : WorkClipTriggger<T>
    {
        /// <summary>
        /// 触发百分比
        /// </summary>
        [Group("触发")]
        [Name("触发百分比")]
        [Range(0,1)]
        public double _triggerPercent = 0.05f;

        /// <summary>
        /// 方向
        /// </summary>
        [Name("方向")]
        [EnumPopup]
        public ETriggerDirection _direction = ETriggerDirection.Increase;

        private bool _valid = true;

        /// <summary>
        /// 上次百分比
        /// </summary>
        protected double lastPercent = -1;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            if (valid) lastPercent = 0;
            //Debug.Log("OnEntry name:" + parent.name + ",valid:"+ valid+ ",lastPercent:"+ lastPercent);
        }

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            if (!valid) return;
            var p = percent.percentOfWorkCurve;
            // 初始化过后
            if (lastPercent >= 0)
            {
                switch(direction)
                {
                    case ETriggerDirection.Increase:
                        {
                            if (triggerPercent > lastPercent && triggerPercent < p)
                            {
                                OnTrigger();
                                    //Debug.Log("trigger name:" + parent.name + ",percent:" + percent + ",lastPercent:" + lastPercent + ",triggerPercent:" + triggerPercent);
                            }
                            break;
                        }
                    case ETriggerDirection.Descending:
                        {
                            if (triggerPercent < lastPercent && triggerPercent > p)
                            {
                                OnTrigger();
                            }
                            break;
                        }
                    case ETriggerDirection.Both:
                        {
                            if ((triggerPercent > lastPercent && triggerPercent < p) ||
                                (triggerPercent < lastPercent && triggerPercent > p))
                            {
                                OnTrigger();
                            }
                            break;
                        }
                }
            }
            //Debug.Log("trigger name 1:" + parent.name + ",percent:" + percent + ",lastPercent:" + lastPercent + ",triggerPercent:" + triggerPercent);
            lastPercent = p;
        }

        /// <summary>
        /// 当触发
        /// </summary>
        protected abstract void OnTrigger();

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
