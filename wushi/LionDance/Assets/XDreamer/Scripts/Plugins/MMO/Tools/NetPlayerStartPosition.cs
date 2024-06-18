using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.PluginMMO.Tools
{
    /// <summary>
    /// 网络玩家起始位置
    /// </summary>
    [RequireManager(typeof(MMOManager))]
    [Name("网络玩家起始位置")]
    [Tip("将当前游戏对象的变换加入MMO玩家生成器的玩家起始位置列表", "Add the transformation of the current game object to the player starting position list of MMO player generator")]
    [DisallowMultipleComponent]
    [Tool(MMOHelperExtension.Title)]
    [XCSJ.Attributes.Icon(EIcon.Teleport)]
    public sealed class NetPlayerStartPosition : InteractProvider
    {
        /// <summary>
        /// 起始位置名称
        /// </summary>
        public string startPositionName => name;
    }
}
