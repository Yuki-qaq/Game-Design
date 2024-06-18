using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.PluginMMO.Tests
{
    /// <summary>
    /// 测试:用于输出MMO网络环境下的各种命令信息
    /// </summary>
    [Name("测试")]
    [Tip("用于输出MMO网络环境下的各种命令与数据信息", "It is used to output various commands and data information in MMO network environment")]
    [RequireManager(typeof(MMOManager))]
    public class Test : InteractProvider, IOnEnable, IOnDisable
    {
        /// <summary>
        /// 输出
        /// </summary>
        [Name("输出")]
        public bool _output = true;

        /// <summary>
        /// 命令列表
        /// </summary>
        [Name("命令列表")]
        [EnumPopup(typeof(ECmd))]
        public List<ECmd> _cmds = new List<ECmd>();

        private void OnCmd(Cmd cmd)
        {
            if (!_output) return;
            if (_cmds.Any(c => cmd.cmd == c))
            {
                var jsonString = JsonHelper.ToJsonData(cmd.data).ToJson(true);
                Log.DebugFormat("用户：{0}={1}={2}={3}", cmd.userGuid, cmd.cmd, cmd.extend, jsonString);
            }
        }

        /// <summary>
        /// 输出所有命令
        /// </summary>
        [ContextMenu("输出所有命令")]
        public void OutputAllCmd()
        {
            _output = true;
            _cmds.Clear();
            _cmds.AddRange(EnumCache<ECmd>.Array);
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            MMOHelper.RegisterAllCmd(OnCmd);
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            MMOHelper.UnregisterAllCmd(OnCmd);
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="log"></param>
        public void OutputLog(string log) => Log.Debug(log);
    }
}
