using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Maths;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.Languages;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 工作区间
    /// </summary>
    [Serializable]
    [LanguageFileOutput]
    [Name("工作区间")]
    public class WorkRange : ITimeClip, IPercentClip, ITTL
    {
        static WorkRange()
        {
            Converter.instance.Register<WorkRange, string>(i => i.ToString());
            Converter.instance.Register<string, WorkRange>(i => StringToWorkRange(i));
        }

        /// <summary>
        /// 百分比区间
        /// </summary>
        [Name("百分比区间")]
        public PercentRange percentRange = new PercentRange();

        /// <summary>
        /// 时间区间
        /// </summary>
        [Name("时间区间")]
        public TimeRange timeRange = new TimeRange();

        /// <summary>
        /// 构造
        /// </summary>
        public WorkRange() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="range"></param>
        public WorkRange(Vector4 range)
        {
            percentRange.percentRange = new V2D(range.x, range.y);
            timeRange.timeRange = new V2D(range.z, range.w);
        }

        /// <summary>
        /// 总时长
        /// </summary>
        public double totalTimeLength
        {
            get => MathX.Scale(timeRange.length, percentRange.length);
            set => timeRange.timeRange = value * percentRange.percentRange;
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public double beginTime { get => timeRange.beginTime; set => timeRange.beginTime = value; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public double endTime { get => timeRange.endTime; set => timeRange.endTime = value; }

        /// <summary>
        /// 时长
        /// </summary>
        public double timeLength { get => timeRange.timeLength; set => timeRange.timeLength = value; }

        /// <summary>
        /// 开始百分比
        /// </summary>
        public double beginPercent { get => percentRange.beginPercent; set => percentRange.beginPercent = value; }

        /// <summary>
        /// 结束百分比
        /// </summary>
        public double endPercent { get => percentRange.endPercent; set => percentRange.endPercent = value; }

        /// <summary>
        /// 百分比长度
        /// </summary>
        public double percentLength { get => percentRange.percentLength; set => percentRange.percentLength = value; }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Format("{0}/{1}", percentRange.ToString(), timeRange.ToString());

        /// <summary>
        /// 工作剪辑转字符串
        /// </summary>
        /// <param name="workRange"></param>
        /// <returns></returns>
        public static string WorkRangeToString(WorkRange workRange) => workRange != null ? workRange.ToString() : "";

        /// <summary>
        /// 字符串转工作剪辑
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static WorkRange StringToWorkRange(string value) => new WorkRange(CommonFun.StringToVector4(value));
    }
}
