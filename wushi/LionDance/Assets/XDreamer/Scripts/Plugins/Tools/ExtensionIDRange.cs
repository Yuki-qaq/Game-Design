using XCSJ.Extension;

namespace XCSJ.PluginTools
{
    /// <summary>
    /// 工具扩展的ID区间
    /// </summary>
    public static class ExtensionIDRange
    {
        /// <summary>
        /// 开始35840
        /// </summary>
        public const int Begin = (int)EExtensionID._0x18;

        /// <summary>
        /// 结束36351
        /// </summary>
        public const int End = (int)EExtensionID._0x1c - 1;

        /// <summary>
        /// 片段64
        /// </summary>
        public const int Fragment = 0x40;

        /// <summary>
        /// 通用35840
        /// </summary>
        public const int Common = Begin + Fragment * 0;

        /// <summary>
        /// Mono行为35904
        /// </summary>
        public const int MonoBehaviour = Begin + Fragment * 1;

        /// <summary>
        /// 状态库35968
        /// </summary>
        public const int StateLib = Begin + Fragment * 2;

        /// <summary>
        /// 工具库36032
        /// </summary>
        public const int Tools = Begin + Fragment * 3;

        /// <summary>
        /// 编辑器36224
        /// </summary>
        public const int Editor = Begin + Fragment * 6;
    }
}
