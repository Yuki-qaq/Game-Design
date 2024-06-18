using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension;

namespace XCSJ.PluginTimelines
{
    /// <summary>
    /// 时间轴助手
    /// </summary>
    public static class TimelineHelper
    {
        /// <summary>
        /// 播放时间转换函数
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public static string ConvertPlayerTimeFormat(double seconds, EPlayerTimeFormat timeFormat)
        {
            try
            {
                switch (timeFormat)
                {
                    case EPlayerTimeFormat.s:
                        {
                            return seconds.ToString();
                        }
                    case EPlayerTimeFormat.f:
                        {
                            return (seconds * 1000).ToString();
                        }
                    case EPlayerTimeFormat.mm__ss:
                        {
                            return TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
                        }
                    case EPlayerTimeFormat.hh__mm__ss:
                        {
                            return TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");
                        }
                    default: return "";
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return "";
            }
        }
    }

    /// <summary>
    /// 时间轴分类
    /// </summary>
    public static class TimelineCategory
    {
        /// <summary>
        /// 时间轴
        /// </summary>
        public const string Category = TimelineManager.Title;

        /// <summary>
        /// 时间轴前缀
        /// </summary>
        public const string TimePrefix = Category + CommonCategory.HorizontalLine;

        /// <summary>
        /// 多媒体
        /// </summary>
        public const string MultiMedia = TimePrefix + CommonCategory.MultiMedia;

        /// <summary>
        /// 播放内容
        /// </summary>
        public const string PlayableContent = TimePrefix + "播放内容";
    }

    /// <summary>
    /// 播放时间格式
    /// </summary>
    [Name("播放时间格式")]
    public enum EPlayerTimeFormat
    {
        /// <summary>
        /// 秒
        /// </summary>
        [Name("秒")]
        s = 0,

        /// <summary>
        /// 分:秒
        /// </summary>
        [Name("分:秒")]
        mm__ss,

        /// <summary>
        /// 时:分:秒
        /// </summary>
        [Name("时:分:秒")]
        hh__mm__ss,

        /// <summary>
        /// 毫秒
        /// </summary>
        [Name("毫秒")]
        f,
    }
}
