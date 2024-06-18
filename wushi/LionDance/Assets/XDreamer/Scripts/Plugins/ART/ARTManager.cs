using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using XCSJ.PluginART.CNScripts;
using XCSJ.PluginART.Tools;
using XCSJ.PluginART.Base;

namespace XCSJ.PluginART
{
    /// <summary>
    /// ART:用于通过网络对接ART官方软件的管理器组件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOption.Optional)]
    [Name(ARTHelper.Title)]
    [Tip("用于通过网络对接ART官方软件的管理器组件", "Manager component for connecting ART official software through network")]
    [Guid("791DDF36-29C5-4B1A-9211-F27BC5AD3F45")]
    [Version("24.129")]
    public sealed class ARTManager : BaseManager<ARTManager>
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

        /// <summary>
        /// 处理客户端
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientConfig"></param>
        /// <param name="action"></param>
        public static void HandleClient(ref ARTStreamClient client, ARTClientConfig clientConfig, Action<ARTStreamClient> action)
        {
            if (instance && clientConfig != null)
            {
                instance.InternalHandleClient(ref client, clientConfig, action);
            }
        }

        void InternalHandleClient(ref ARTStreamClient client, ARTClientConfig clientConfig, Action<ARTStreamClient> action)
        {
            if (!hasAccess) return;
            if (client)//已经创建
            {
                action?.Invoke(client);
                if (!CheckClient(client, clientConfig))
                {
                    UpdateClient(client, clientConfig);
                    client.ReenabledDelay();
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
        public static bool CheckClient(ARTStreamClient client, ARTClientConfig clientConfig)
        {
            if (clientConfig == null || clientConfig == null) return false;
            return client._serverHost == clientConfig.serverHost
                 && client._serverPort == clientConfig.serverPort
                 && client._dataPort == clientConfig.dataPort
                 && client._remoteType == clientConfig.remoteType;
        }

        /// <summary>
        /// 更新客户端
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientConfig"></param>
        /// <returns></returns>
        public static bool UpdateClient(ARTStreamClient client, ARTClientConfig clientConfig)
        {
            if (clientConfig == null || clientConfig == null) return false;
            client._serverHost = clientConfig.serverHost;
            client._serverPort = (ushort)clientConfig.serverPort;
            client._dataPort = (ushort)clientConfig.dataPort;
            client._remoteType = clientConfig.remoteType;
            return true;
        }

        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public ARTStreamClient GetOrAdd(ARTClientConfig clientConfig, out bool isAdd)
        {
            if (clientConfig == null || clientConfig == null)
            {
                isAdd = default;
                return default;
            }
            isAdd = false;
            foreach (var c in GetComponents<ARTStreamClient>())
            {
                if (string.IsNullOrEmpty(c._serverHost))
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
            var client = this.XAddComponent<ARTStreamClient>();
            if (UpdateClient(client, clientConfig))
            {
                isAdd = true;
            }
            return client;
        }
    }
}
