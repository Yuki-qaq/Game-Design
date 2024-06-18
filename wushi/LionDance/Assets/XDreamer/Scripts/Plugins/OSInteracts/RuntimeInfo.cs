using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginOSInteracts
{
    /// <summary>
    /// 运行时信息
    /// </summary>
#if UNITY_5_6_OR_NEWER
    [Obsolete("本类不再使用", true)]
#endif
    [Serializable]
    [Name("运行时信息")]
    public class RuntimeInfo
    {
        /// <summary>
        /// 默认发送消息
        /// </summary>
        [Name("默认发送消息")]
        public bool defaultSendMessage = true;

        /// <summary>
        /// 信息
        /// </summary>
        [Name("信息")]
        [OnlyMemberElements]
        public List<Info> infos = new List<Info>();

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            infos.Add(new Info());
        }

        /// <summary>
        /// 需要发送消息
        /// </summary>
        /// <returns></returns>
        public bool NeedSendMessage()
        {
            foreach (var info in infos)
            {
                if (info.runtimePlatform == Application.platform) return info.sendMessage;
            }
            return defaultSendMessage;
        }

        /// <summary>
        /// 信息
        /// </summary>
        [Name("信息")]
        [Serializable]
        public class Info
        {
            /// <summary>
            /// 运行时平台
            /// </summary>
            [Name("运行时平台")]
            public RuntimePlatform runtimePlatform = RuntimePlatform.WebGLPlayer;

            /// <summary>
            /// 发送消息
            /// </summary>
            [Name("发送消息")]
            public bool sendMessage = false;
        }
    }
}
