using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Extension;

namespace XCSJ.PluginNetInteract
{
    /// <summary>
    /// 网络交互辅助类
    /// </summary>
    public static class NetInteractHelper
    {
        /// <summary>
        /// 默认端口
        /// </summary>
        public const int DefaultPort = 18888;
    }

    /// <summary>
    /// 网络交互分类
    /// </summary>
    public static class NetInteractCategory
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "网络交互";

        /// <summary>
        /// 标题目录
        /// </summary>
        public const string TitleDirectory = Title + CommonCategory.PathSplitLine;

        /// <summary>
        /// 标题前缀
        /// </summary>
        public const string TitlePrefix = Title + CommonCategory.HorizontalLine;

        /// <summary>
        /// 服务器
        /// </summary>
        public const string Server = TitlePrefix + "服务器";

        /// <summary>
        /// 服务器目录
        /// </summary>
        public const string ServerDirectory = Server + CommonCategory.PathSplitLine;

        /// <summary>
        /// 客户端
        /// </summary>
        public const string Client = TitlePrefix + "客户端";


        /// <summary>
        /// 客户端目录
        /// </summary>
        public const string ClientDirectory = Client + CommonCategory.PathSplitLine;
    }
}
