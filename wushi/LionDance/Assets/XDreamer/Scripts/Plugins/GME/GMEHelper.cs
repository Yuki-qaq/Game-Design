using XCSJ.Extension;

namespace XCSJ.PluginGME
{
    /// <summary>
    /// GME组手
    /// </summary>
    public static class GMEHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "GME";

        /// <summary>
        /// 创建开放ID
        /// </summary>
        /// <param name="high32"></param>
        /// <param name="low32"></param>
        /// <returns></returns>
        public static long CreateOpenID(string high32,string low32)
        {
            long high = high32.GetHashCode();
            long low = low32.GetHashCode();
            return (high << 32) | low;
        }
    }

    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值，39552
        /// </summary>
        public const int Begin = (int)EExtensionID._0x35;

        /// <summary>
        /// 结束值，39680-1=39679
        /// </summary>
        public const int End = (int)EExtensionID._0x36 - 1;
    }
}