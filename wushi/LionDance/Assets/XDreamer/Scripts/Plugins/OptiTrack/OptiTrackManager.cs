using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginOptiTrack.CNScripts;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginOptiTrack
{
    /// <summary>
    /// OptiTrack:用于对接OptiTrack官方插件包的管理器组件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOption.Optional)]
    [Name(OptiTrackHelper.Title)]
    [Tip("用于对接OptiTrack官方插件包的管理器组件", "Manager component for docking with OptiTrack official plug-in package")]
    [Guid("3CCCFED9-B0F9-4C42-B453-919EDEB861AE")]
    [Version("24.129")]
    public sealed class OptiTrackManager : BaseManager<OptiTrackManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts() => Script.GetScriptsOfEnum<EScriptID>(this);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
            //switch((EScriptID)id)
            //{

            //}
            return ReturnValue.No;
        }

#if XDREAMER_OPTITRACK

        /// <summary>
        /// 处理客户端
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientConfig"></param>
        /// <param name="action"></param>
        public static void HandleClient(ref OptitrackStreamingClient client, OptiTrackClientConfig clientConfig, Action<OptitrackStreamingClient> action)
        {
            if (instance && clientConfig != null)
            {
                instance.InternalHandleClient(ref client, clientConfig, action);
            }
        }

        void InternalHandleClient(ref OptitrackStreamingClient client, OptiTrackClientConfig clientConfig, Action<OptitrackStreamingClient> action)
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
        public static bool CheckClient(OptitrackStreamingClient client, OptiTrackClientConfig clientConfig)
        {
            if (clientConfig == null || clientConfig == null) return false;
            return client.ServerAddress == clientConfig.serverAddress
                 && client.LocalAddress == clientConfig.localAddress
                 && client.ConnectionType == (OptitrackStreamingClient.ClientConnectionType)clientConfig.connectionType
                 && client.SkeletonCoordinates == (StreamingCoordinatesValues)clientConfig.skeletonCoordinates
                 && client.BoneNamingConvention == (OptitrackBoneNameConvention)clientConfig.boneNamingConvention;
        }

        /// <summary>
        /// 更新客户端
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientConfig"></param>
        /// <returns></returns>
        public static bool UpdateClient(OptitrackStreamingClient client, OptiTrackClientConfig clientConfig)
        {
            if (clientConfig == null || clientConfig == null) return false;
            client.ServerAddress = clientConfig.serverAddress;
            client.LocalAddress = clientConfig.localAddress;
            client.ConnectionType = (OptitrackStreamingClient.ClientConnectionType)clientConfig.connectionType;
            client.SkeletonCoordinates = (StreamingCoordinatesValues)clientConfig.skeletonCoordinates;
            client.BoneNamingConvention = (OptitrackBoneNameConvention)clientConfig.boneNamingConvention;
            return true;
        }

        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public OptitrackStreamingClient GetOrAdd(OptiTrackClientConfig clientConfig, out bool isAdd)
        {
            if (clientConfig == null || clientConfig == null)
            {
                isAdd = default;
                return default;
            }
            isAdd = false;
            foreach (var c in GetComponents<OptitrackStreamingClient>())
            {
                if (c.ServerAddress == "127.0.0.1"
                    && c.LocalAddress == "127.0.0.1"
                    && c.ConnectionType == OptitrackStreamingClient.ClientConnectionType.Multicast
                    && c.SkeletonCoordinates == StreamingCoordinatesValues.Local
                    && c.BoneNamingConvention == OptitrackBoneNameConvention.Motive)
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
            var client = this.XAddComponent<OptitrackStreamingClient>();
            if (UpdateClient(client, clientConfig))
            {
                isAdd = true;
            }
            return client;
        }
#endif
    }
}
