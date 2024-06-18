using XCSJ.Extension;

namespace XCSJ.PluginStereoView
{
    /// <summary>
    /// ID区间
    /// </summary>
    public static class IDRange
    {
        /// <summary>
        /// 开始，35584
        /// </summary>
        public const int Begin = (int)EExtensionID._0x16;

        /// <summary>
        /// 结束，35712-1=35711
        /// </summary>
        public const int End = (int)EExtensionID._0x17 - 1;

        /// <summary>
        /// 片段24
        /// </summary>
        public const int Fragment = 0x18;

        /// <summary>
        /// 通用35584
        /// </summary>
        public const int Common = Begin + Fragment * 0;

        /// <summary>
        /// Mono行为35608
        /// </summary>
        public const int MonoBehaviour = Begin + Fragment * 1;

        /// <summary>
        /// 状态库35632
        /// </summary>
        public const int StateLib = Begin + Fragment * 2;

        /// <summary>
        /// 工具库35656
        /// </summary>
        public const int Tools = Begin + Fragment * 3;

        /// <summary>
        /// 编辑器35680
        /// </summary>
        public const int Editor = Begin + Fragment * 4;
    }
}
