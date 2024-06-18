using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginMMO;

namespace XCSJ.Extension.Base.Interactions.Base
{
    /// <summary>
    /// 命令
    /// </summary>
    public class Cmd
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        [Name("命令名称")]
        [DynamicLabel]
        public string _cmdName = "";

        /// <summary>
        /// 命令名称
        /// </summary>
        public virtual string cmdName => _cmdName;

        /// <summary>
        /// 命令
        /// </summary>
        [Name("命令")]
        [InteractCmdPopup]
        public string _cmd = "";

        /// <summary>
        /// 有效
        /// </summary>
        public virtual bool valid => !string.IsNullOrEmpty(_cmdName) && !string.IsNullOrEmpty(_cmd);
    }

    /// <summary>
    /// 命令列表
    /// </summary>
    public abstract class Cmds
    {
        /// <summary>
        /// 命令集
        /// </summary>
        public abstract IEnumerable<Cmd> cmds { get; }

        /// <summary>
        /// 命令列表
        /// </summary>
        public List<string> cmdList => cmds.ToList(item => item._cmd);

        /// <summary>
        /// 命令名称列表
        /// </summary>
        public List<string> cmdNameList => cmds.ToList(item => item._cmdName);

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cmdType"></param>
        public abstract void Reset(Type type, ECmdType cmdType);

        /// <summary>
        /// 补齐缺失命令
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cmdType"></param>
        public abstract void AddLostCmd(Type type, ECmdType cmdType);

        /// <summary>
        /// 包含命令名称
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        public virtual bool ContainsCmdName(string cmdName)
        {
            foreach (var item in cmds)
            {
                if (item._cmdName == cmdName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 包含命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual bool ContainsCmd(string cmd)
        {
            foreach (var item in cmds)
            {
                if (item._cmd == cmd)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取命令名称:通过命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string GetCmdName(string cmd) => cmds.FirstOrDefault(c => c._cmd == cmd)?._cmdName ?? "";

        /// <summary>
        /// 获取命令名称列表
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string[] GetCmdNames(string cmd) => cmds.Where(c => cmd == c._cmd).Cast(item => item._cmdName).ToArray();

        /// <summary>
        /// 获取命令名称列表
        /// </summary>
        /// <param name="inCmds"></param>
        /// <returns></returns>
        public List<string> GetCmdNames(List<string> inCmds) => cmds.Where(c => inCmds.Contains(c._cmd)).ToList(item => item._cmdName);

        /// <summary>
        /// 获取命令:通过命令名称
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        public string GetCmd(string cmdName) => cmds.FirstOrDefault(c => c._cmdName == cmdName)?._cmd ?? "";
    }

    /// <summary>
    /// 命令列表模版
    /// </summary>
    /// <typeparam name="TCmd"></typeparam>
    public class Cmds<TCmd> : Cmds where TCmd : Cmd, new()
    {
        /// <summary>
        /// 命令列表
        /// </summary>
        [Name("命令列表")]
        [OnlyMemberElements(true)]
        public List<TCmd> _cmds = new List<TCmd>();

        /// <summary>
        /// 命令列表
        /// </summary>
        public override IEnumerable<Cmd> cmds => _cmds;

        /// <summary>
        /// 包含命令名称
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        public override bool ContainsCmdName(string cmdName) => _cmds.Exists(c => c.cmdName == cmdName);

        /// <summary>
        /// 包含命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override bool ContainsCmd(string cmd) => _cmds.Exists(c => c._cmd == cmd);

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cmdType"></param>
        public override void Reset(Type type, ECmdType cmdType)
        {
            _cmds.Clear();
            if (cmdType == ECmdType.None) return;

            foreach (var d in type.GetInteractCmdDatas().interactCmdDatas)
            {
                if (d.interactCmdAttribtue.IsMatchCmdType(cmdType))
                {
                    _cmds.Add(new TCmd() { _cmdName = d.friendlyCmd, _cmd = d.cmd });
                }
            }
        }

        /// <summary>
        /// 补齐缺失命令
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cmdType"></param>
        public override void AddLostCmd(Type type, ECmdType cmdType)
        {
            if (cmdType == ECmdType.None) return;

            foreach (var d in type.GetInteractCmdDatas().interactCmdDatas)
            {
                if (d.interactCmdAttribtue.IsMatchCmdType(cmdType))
                {
                    if (!ContainsCmd(d.cmd))
                    {
                        _cmds.Add(new TCmd() { _cmdName = d.friendlyCmd, _cmd = d.cmd });
                    }
                }
            }
        }

        /// <summary>
        /// 尝试获取命令
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool TryGetCmd(string cmdName, out string cmd)
        {
            var cmdObject = _cmds.FirstOrDefault(c => c._cmdName == cmdName);
            if (cmdObject != null)
            {
                cmd = cmdObject._cmd;
                return true;
            }
            cmd = default;
            return false;
        }
    }

    /// <summary>
    /// 缺省命令
    /// </summary>
    [Serializable]
    public class DefaultCmd : Cmd { }

    /// <summary>
    /// 缺省命令列表
    /// </summary>
    [Serializable]
    public class DefaultCmds : Cmds<DefaultCmd> { }
}
