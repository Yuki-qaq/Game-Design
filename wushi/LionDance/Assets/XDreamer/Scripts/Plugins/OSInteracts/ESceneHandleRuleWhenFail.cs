using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginOSInteracts
{
    /// <summary>
    /// 当失败时场景处理规则脚本参数
    /// </summary>
    [ScriptParamType(ScriptParamType)]
    public class SceneHandleRuleWhenFail_ScriptParam : EnumScriptParam<ESceneHandleRuleWhenFail>
    {
        /// <summary>
        /// 脚本参数类型
        /// </summary>
        public const int ScriptParamType = (int)EOSInteractScriptID._Begin;
    }

    /// <summary>
    /// 场景加载失败时场景处理规则
    /// </summary>
    [Name("场景加载失败时场景处理规则")]
    public enum ESceneHandleRuleWhenFail
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [Tip("不做任何操作")]
        None,

        /// <summary>
        /// 加载之前场景
        /// </summary>
        [Name("加载之前场景")]
        [Tip("加载处于记录状态的之前场景；如果之前场景不存在则，触发'加载主场景并返回OS'操作；")]
        LoadPreviousScene,

        /// <summary>
        /// 加载主场景并返回OS
        /// </summary>
        [Name("加载主场景并返回OS")]
        LoadMainSceneAndBackOS,

        /// <summary>
        /// 返回OS
        /// </summary>
        [Name("返回OS")]
        BackOS,

        /// <summary>
        /// 加载主场景
        /// </summary>
        [Name("加载主场景")]
        LoadMainScene,

        /// <summary>
        /// 关闭程序
        /// </summary>
        [Name("关闭程序")]
        ApplicationQuit,
    }
}
