using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 闪烁
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRecorder"></typeparam>
    public abstract class Flash<T, TRecorder> : Motion<T, TRecorder>, IFlash
        where T : Flash<T, TRecorder>
        where TRecorder : class, IPercentRecorder<T>, new()
    {
        /// <summary>
        /// 闪烁模式
        /// </summary>
        [Name("闪烁模式")]
        [EnumPopup]
        public EFlashMode flashMode = EFlashMode.Frequency;

        /// <summary>
        /// 频率
        /// </summary>
        [Name("频率")]
        [Tip("单位时间（1秒）内的闪烁次数", "Number of flashes per unit time (1 second)")]
        [Range(0, 100)]
        [HideInSuperInspector(nameof(flashMode), EValidityCheckType.NotEqual, EFlashMode.Frequency)]
        public double hz = 2;

        /// <summary>
        /// 次数
        /// </summary>
        [Name("次数")]
        [Tip("在有效时长内的闪烁次数", "Number of flashes within the effective duration")]
        [Range(0, 100)]
        [HideInSuperInspector(nameof(flashMode), EValidityCheckType.NotEqual, EFlashMode.Count)]
        public double count = 6;

        /// <summary>
        /// 闪烁次数
        /// </summary>
        public double flashCount
        {
            get
            {
                switch (flashMode)
                {
                    case EFlashMode.Frequency: return timeLength * hz;
                    case EFlashMode.Count: return count;
                    default: return 0;
                }
            }
        }

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            inChangeArea = InChangeArea(percent.percent01OfWorkCurve);
            base.OnSetPercent(percent, stateData);
        }

        /// <summary>
        /// 在修改区域
        /// </summary>
        public bool inChangeArea { get; private set; } = false;

        /// <summary>
        /// 在修改区域
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        protected bool InChangeArea(double percentage)
        {
            return MathX.FloorToInt(flashCount * 2 * percentage) % 2 == 1;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            //默认在闪烁结束时，恢复原状态
            percentOnExit = 0;
            base.Reset();
        }
    }

    /// <summary>
    /// 闪烁接口
    /// </summary>
    public interface IFlash : IMotion
    {
        /// <summary>
        /// 有效时长内的闪烁次数
        /// </summary>
        double flashCount { get; }
    }

    /// <summary>
    /// 闪烁模式
    /// </summary>
    [Name("闪烁模式")]
    public enum EFlashMode
    {
        /// <summary>
        /// 无处理
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 频率,即单位时间（1秒）内的闪烁次数
        /// </summary>
        [Name("频率")]
        [Tip("单位时间（1秒）内的闪烁次数", "Number of flashes per unit time (1 second)")]
        Frequency,

        /// <summary>
        /// 次数,即在有效时长内的闪烁次数
        /// </summary>
        [Name("次数")]
        [Tip("在有效时长内的闪烁次数", "Number of flashes within the effective duration")]
        Count,
    }
}
