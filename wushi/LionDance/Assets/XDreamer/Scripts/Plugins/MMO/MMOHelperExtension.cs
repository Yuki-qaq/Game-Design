using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Caches;
using XCSJ.Extension;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using static XCSJ.PluginMMO.MMOHelper;

namespace XCSJ.PluginMMO
{
    /// <summary>
    /// MMO组手扩展
    /// </summary>
    public static class MMOHelperExtension
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "MMO";

        /// <summary>
        /// 标题目录
        /// </summary>
        public const string TitleDirectory = Title + CommonCategory.PathSplitLine;

        /// <summary>
        /// 标题前缀
        /// </summary>
        public const string TitlePrefix = Title + CommonCategory.HorizontalLine;

        /// <summary>
        /// MMO角色
        /// </summary>
        public const string Character = TitlePrefix + "角色";

        /// <summary>
        /// MMO UI
        /// </summary>
        public const string UI = TitlePrefix + "UI";

        /// <summary>
        /// 时长信息
        /// </summary>
        /// <param name="duration">时长</param>
        /// <param name="totalDuration">总时长</param>
        /// <returns></returns>
        public static string DurationInfo(float duration, float totalDuration) => string.Format("{0:0.00}/{1:0.00}", duration, totalDuration);

        #region 玩家扩展

        /// <summary>
        /// 允许音频：获取
        /// </summary>
        /// <param name="playerInfo"></param>
        /// <returns></returns>
        public static bool AllowAudio(this PlayerInfo playerInfo) => playerInfo?.GetLocalProperty(nameof(AllowAudio)) is bool b ? b : true;

        /// <summary>
        /// 允许音频：设置
        /// </summary>
        /// <param name="playerInfo"></param>
        /// <param name="allowAudio"></param>
        public static void AllowAudio(this PlayerInfo playerInfo, bool allowAudio) => playerInfo?.SetLocalProperty(nameof(AllowAudio), allowAudio);

        #endregion
    }
}
