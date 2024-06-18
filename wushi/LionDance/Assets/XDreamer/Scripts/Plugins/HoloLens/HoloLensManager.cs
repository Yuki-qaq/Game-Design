using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginHoloLens
{
    /// <summary>
    /// HoloLens
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOption.Optional)]
    [Name("HoloLens")]
    [Tip("实现微软HoloLens设备的事件监听、手势交互输入等操作", "Realize event monitoring, gesture interactive input and other operations of Microsoft HoloLens device")]
    [Guid("F5DB5EE3-6824-46E9-894A-65405A68561E")]
    [Version("24.129")]
    public class HoloLensManager : BaseManager<HoloLensManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts()
        {
            return Script.GetScriptsOfEnum<EHoloLensScriptID>(this);
        }

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
            //switch ((HoloLensScriptID)id)
            //{
                
            //}
            return ReturnValue.No;
        }
    }
}

