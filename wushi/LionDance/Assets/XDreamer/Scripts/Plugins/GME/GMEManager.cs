using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using XCSJ.PluginGME.CNScripts;
using XCSJ.PluginGME.Tools;
using XCSJ.Extension.Base.Extensions;

namespace XCSJ.PluginGME
{
    /// <summary>
    /// GME:用于对接腾讯游戏多媒体引擎（GME）官方软件的管理器组件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Professional)]
    [ComponentOption(EComponentOption.Optional)]
    [Name(GMEHelper.Title)]
    [Tip("用于对接腾讯游戏多媒体引擎（GME）官方软件的管理器组件", "Manager component for docking with Tencent Gaming Multimedia Engine (GME) official software")]
    [Guid("AC24B1EB-A756-4547-BAC7-00C904DB6892")]
    [Version("24.129")]
    public sealed class GMEManager : BaseManager<GMEManager>
    {
        /// <summary>
        /// GME提供者
        /// </summary>
        [Name("GME提供者")]
        [ComponentPopup]
        public GMEProvider _provider;

        /// <summary>
        /// 获取或
        /// </summary>
        public GMEProvider provider => this.XGetComponent(ref _provider);

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
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _provider = this.XGetOrAddComponent<GMEProvider>();
        }
    }
}
