using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginOSInteracts
{
    /// <summary>
    /// Unity到OS消息命令脚本参数
    /// </summary>
    [ScriptParamType(ScriptParamType)]
    public class UnityToOSMsgCmd_ScriptParam : EnumScriptParam<EUnityToOSMsgCmd>
    {
        /// <summary>
        /// 脚本参数类型
        /// </summary>
        public const int ScriptParamType = SceneHandleRuleWhenFail_ScriptParam.ScriptParamType + 1;
    }

    /// <summary>
    /// Uinty向OS消息命令:Uinty向OS发送的各种消息命令
    /// </summary>
    [Name("Uinty向OS消息命令")]
    public enum EUnityToOSMsgCmd
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 用户自定义
        /// </summary>
        [Name("用户自定义")]
        UserDefine,

        /// <summary>
        /// 返回OS
        /// </summary>
        [Name("返回OS")]
        BackOS,

        /// <summary>
        /// Unity引擎加载完成
        /// </summary>
        [Name("Unity引擎加载完成")]
        UnityEngineLoadedFinish,

        /// <summary>
        /// 导入场景完成
        /// </summary>
        [Name("导入场景完成")]
        ImportSceneFinish,

        /// <summary>
        /// 加载场景完成
        /// </summary>
        [Name("加载场景完成")]
        [Tip("由加载成功后的场景在启动时发送；加载场景、导入并加载场景执行成功后均回调的是本消息；")]
        LoadSceneFinish,

        /// <summary>
        /// 定时消息
        /// </summary>
        [Name("定时消息")]
        [Tip("当无导入或加载场景任务时，定时回调的消息；")]
        TimedMessage,

        /// <summary>
        /// 定时消息(导入并加载中)
        /// </summary>
        [Name("定时消息(导入并加载中)")]
        [Tip("当有导入并加载场景任务时，定时回调的消息；")]
        TimedMessageInImportAndLoad,

        /// <summary>
        /// 定时消息(导入中)
        /// </summary>
        [Name("定时消息(导入中)")]
        [Tip("当有导入场景任务时，定时回调的消息；")]
        TimedMessageInImport,

        /// <summary>
        /// 定时消息(加载中)
        /// </summary>
        [Name("定时消息(加载中)")]
        [Tip("当有加载场景任务时，定时回调的消息；")]
        TimedMessageInLoad,

        /// <summary>
        /// 导入并加载场景失败
        /// </summary>
        [Name("导入并加载场景失败")]
        ImportAndLoadSceneFailed,

        /// <summary>
        /// 导入场景失败
        /// </summary>
        [Name("导入场景失败")]
        ImportSceneFailed,

        /// <summary>
        /// 加载场景失败
        /// </summary>
        [Name("加载场景失败")]
        LoadSceneFailed,

        /// <summary>
        /// 场景名称列表
        /// </summary>
        [Name("场景名称列表")]
        SceneNameList,

        /// <summary>
        /// 通过OS切换场景
        /// </summary>
        [Name("通过OS切换场景")]
        SwitchSceneByOS,

        /// <summary>
        /// 二维码扫描结果
        /// </summary>
        [Name("二维码扫描结果")]
        QRCodeScanResult,

        /// <summary>
        /// 单句XCSJ脚本执行结果
        /// </summary>
        [Name("单句XCSJ脚本执行结果")]
        SingleXCSJScriptRunResult,

        /// <summary>
        /// 卸载子场景完成
        /// </summary>
        [Name("卸载子场景完成")]
        UnloadSubSceneFinish,

        /// <summary>
        /// 卸载全部子场景完成
        /// </summary>
        [Name("卸载全部子场景完成")]
        UnloadAllSubSceneFinish,
    }
}
