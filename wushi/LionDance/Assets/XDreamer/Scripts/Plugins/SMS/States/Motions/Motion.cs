using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 动作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Motion<T> : WorkClip<T>, IMotion
        where T : Motion<T>, IMotion
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            base.Init(data);
            initRecorder.Record(self);
            return true;
        }

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            recorder.SetPercent(percent);
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            entryRecorder.Clear();
            entryRecorder.Record(self);
            base.OnEntry(data);
        }

        /// <summary>
        /// 记录器
        /// </summary>
        public virtual IPercentRecorder<T> recorder => useInitData ? initRecorder : entryRecorder;

        /// <summary>
        /// 初始记录器
        /// </summary>
        public abstract IPercentRecorder<T> initRecorder { get; }

        /// <summary>
        /// 进入记录器
        /// </summary>
        public abstract IPercentRecorder<T> entryRecorder { get; }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="data"></param>
        public override void Reset(ResetData data)
        {
            base.Reset(data);
            switch(data.dataRule)
            {
                case EDataRule.Init:
                    {
                        initRecorder.Recover();
                        break;
                    }
                case EDataRule.Entry:
                    {
                        entryRecorder.Recover();
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 动作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRecorder"></typeparam>
    public abstract class Motion<T, TRecorder> : Motion<T>
        where T : Motion<T, TRecorder>
        where TRecorder : class, IPercentRecorder<T>, new()
    {
        /// <summary>
        /// 初始记录器
        /// </summary>
        public override IPercentRecorder<T> initRecorder { get; } = new TRecorder();

        /// <summary>
        /// 进入记录器
        /// </summary>
        public override IPercentRecorder<T> entryRecorder { get; } = new TRecorder();
    }

    /// <summary>
    /// 动作
    /// </summary>
    public interface IMotion : ILoopWorkClip { }

}
