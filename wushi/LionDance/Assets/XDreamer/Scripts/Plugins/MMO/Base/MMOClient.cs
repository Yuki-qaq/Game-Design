using System;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Net;
using XCSJ.Net;

namespace XCSJ.PluginMMO.Base
{
    /// <summary>
    /// MMO客户端
    /// </summary>
    public class MMOClient : CrossPlatformTcpClient, IMMOClient
    {
        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="address"></param>
        /// <param name="path"></param>
        /// <param name="onConnect"></param>
        /// <returns></returns>
        public bool ConnectAsync(IAddress address, string path, Action<IMMOClient> onConnect)
        {
            return base.ConnectAsync(address, path, c => onConnect?.Invoke
             (c as IMMOClient));
        }
    }


    /// <summary>
    /// 网络状态属性值
    /// </summary>
    [Serializable]
    [Name("网络状态属性值")]
    public class ENetStatePropertyValue : EnumPropertyValue<ENetState> { }
}
