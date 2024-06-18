using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.Extension.Base.Kernel
{
    /// <summary>
    /// 默认路径处理器
    /// </summary>
    public class DefaultPathHandler : InstanceClass<DefaultPathHandler>, IPathHandler
    {
        /// <summary>
        /// 持久化数据路径
        /// </summary>
        public string persistentDataPath
        {
            get
            {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                return Application.persistentDataPath + "/";
#else
                return Application.streamingAssetsPath + "/";
#endif
            }
        }

        /// <summary>
        /// 文件协议
        /// </summary>
        public string fileProtocol
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
                return "file://";
#else
                return "";
#endif
            }
        }
    }
}
