using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using XCSJ.PluginZVR.CNScripts;
using XCSJ.PluginXRSpaceSolution.Base;

namespace XCSJ.PluginZVR
{
    /// <summary>
    /// ZVR:用于对接ZVR官方插件包的管理器组件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOption.Optional)]
    [Name(ZVRHelper.Title)]
    [Tip("用于对接ZVR官方插件包的管理器组件", "Manager component for docking ZVR official plug-in package")]
    [Guid("A9459A51-D7D4-477F-B139-A26DBDD82CFE")]
    [Version("24.129")]
    public sealed class ZVRManager : BaseManager<ZVRManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts() => Script.GetScriptsOfEnum<EScriptID>(this);

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param) => ReturnValue.No;

#if XDREAMER_ZVR

        /// <summary>
        /// 处理客户端
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientConfig"></param>
        /// <param name="action"></param>
        public static void HandleClient(ref ZvrGokuStreamClient client, ZVRClientConfig clientConfig, Action<ZvrGokuStreamClient> action)
        {
            if (instance && clientConfig != null)
            {
                instance.InternalHandleClient(ref client, clientConfig, action);
            }
        }

        void InternalHandleClient(ref ZvrGokuStreamClient client, ZVRClientConfig clientConfig, Action<ZvrGokuStreamClient> action)
        {
            if (!hasAccess) return;
            if (client)//已经创建
            {
                action?.Invoke(client);
                if (!CheckClient(client, clientConfig))
                {
                    UpdateClient(client, clientConfig);
                    client.enabled = false;
                    var c = client;
                    CommonFun.DelayCall(() => { c.enabled = true; });
                }
            }
            else
            {
                var tmpClient = GetOrAdd(clientConfig, out var isAdd);
                if (tmpClient)
                {
                    if (isAdd) client = tmpClient;
                    action?.Invoke(tmpClient);
                    tmpClient.enabled = true;//确保启用
                }
            }
        }

        /// <summary>
        /// 检查客户端
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientConfig"></param>
        /// <returns></returns>
        public static bool CheckClient(ZvrGokuStreamClient client, ZVRClientConfig clientConfig)
        {
            if (clientConfig == null || clientConfig == null) return false;
            return client.ServerAddress == clientConfig.serverAddress
                 && client.LocalAddress == clientConfig.localAddress
                 && client.ConnectionType == (ZvrGokuStreamClient.ClientConnectionType)clientConfig.connectionType
                 && client.ServerCommandPort == clientConfig.serverCommandPort
                 && client.ServerDataPort == clientConfig.serverDataPort;
        }

        /// <summary>
        /// 更新客户端
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientConfig"></param>
        /// <returns></returns>
        public static bool UpdateClient(ZvrGokuStreamClient client, ZVRClientConfig clientConfig)
        {
            if (clientConfig == null || clientConfig == null) return false;
            client.ServerAddress = clientConfig.serverAddress;
            client.LocalAddress = clientConfig.localAddress;
            client.ConnectionType = (ZvrGokuStreamClient.ClientConnectionType)clientConfig.connectionType;
            client.ServerCommandPort = clientConfig.serverCommandPort;
            client.ServerDataPort = clientConfig.serverDataPort;
            return true;
        }

        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public ZvrGokuStreamClient GetOrAdd(ZVRClientConfig clientConfig, out bool isAdd)
        {
            if (clientConfig == null || clientConfig == null)
            {
                isAdd = default;
                return default;
            }
            isAdd = false;
            foreach (var c in GetComponents<ZvrGokuStreamClient>())
            {
                if (c.ServerAddress == "239.8.192.168"
                    && c.LocalAddress == "127.0.0.1"
                    && c.ConnectionType == ZvrGokuStreamClient.ClientConnectionType.Multicast
                    && c.ServerCommandPort == 15515
                    && c.ServerDataPort == 15516)
                {
                    if (UpdateClient(c, clientConfig))
                    {
                        isAdd = true;
                    }
                    return c;
                }
                if (CheckClient(c, clientConfig))
                {
                    return c;
                }
            }
            var client = this.XAddComponent<ZvrGokuStreamClient>();
            if (UpdateClient(client, clientConfig))
            {
                isAdd = true;
            }
            return client;
        }
#endif
    }
}
