using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginLogs.States
{
    /// <summary>
    /// 日志：日志组件是输出日志的执行体，用于调试状态机执行流。
    /// </summary>
    [Name(Title, nameof(Log))]
    [XCSJ.Attributes.Icon(EIcon.Log)]
    [Tip("日志组件是输出日志的执行体，用于调试状态机执行流。", "The log component is the execution body of the output log, which is used to debug the execution flow of the state machine.")]
    [ComponentMenu(LogCategory.TitleDirectory + Title, typeof(LogManager))]
    public class Log : LifecycleExecutor<Log>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "日志";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(Log))]
        [Tip("日志组件是输出日志的执行体，用于调试状态机执行流。", "The log component is the execution body of the output log, which is used to debug the execution flow of the state machine.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(LogCategory.Title, typeof(LogManager))]
        [StateComponentMenu(LogCategory.TitleDirectory + Title, typeof(LogManager))]
        [StateLib(CommonCategory.CommonUse, typeof(LogManager))]
        [StateComponentMenu(CommonCategory.CommonUseDirectory + Title, typeof(LogManager))]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 日志级别
        /// </summary>
        [Name("日志级别")]
        [EnumPopup] 
        public ELogLevel logLevel = ELogLevel.Debug;

        /// <summary>
        /// 日志标签
        /// </summary>
        [Name("日志标签")]
        public string logTag = "";

        /// <summary>
        /// 日志
        /// </summary>
        [Name("日志")]
        [TextArea]
        [FormerlySerializedAs(nameof(_log))]
        public string _log = "";

        /// <summary>
        /// 日志
        /// </summary>
        public string log => _log;

        /// <summary>
        /// 日志列表
        /// </summary>
        [Name("日志列表")]
        public List<StringPropertyValue_TextArea> _logs = new List<StringPropertyValue_TextArea>();

        /// <summary>
        /// 日志列表的数据
        /// </summary>
        public string logs => _logs.ToString(s => s.GetValue(), "");

        /// <summary>
        /// 完整日志
        /// </summary>
        public string fullLog => _log + logs;

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            try
            {
                XCSJ.Log.Output(fullLog, logLevel, logTag);
            }
            catch (Exception ex)
            {
                XCSJ.Log.Exception(nameof(Log) + "执行输出时异常:" + ex);
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => log + _logs.ToString(s => s.ToFriendlyString(), "");
    }
}
