using XCSJ.Attributes;
using XCSJ.Scripts;

namespace XCSJ.PluginOSInteracts
{
    /// <summary>
    /// OS到Unity消息命令脚本参数
    /// </summary>
    [ScriptParamType(ScriptParamType)]
    public class OSToUnityMsgCmd_ScriptParam : EnumScriptParam<EOSToUnityMsgCmd>
    {
        /// <summary>
        /// 脚本参数
        /// </summary>
        public const int ScriptParamType = SceneHandleRuleWhenFail_ScriptParam.ScriptParamType + 2;
    }

    /// <summary>
    /// OS向Uinty消息命令:OS向Uinty发送的各种消息命令
    /// </summary>
    [Name("OS向Uinty消息命令")]
    public enum EOSToUnityMsgCmd
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 导入并加载场景
        /// </summary>
        [Name("导入并加载场景")]
        ImportAndLoadScene,

        /// <summary>
        /// 导入场景
        /// </summary>
        [Name("导入场景")]
        ImportScene,

        /// <summary>
        /// 加载场景
        /// </summary>
        [Name("加载场景")]
        LoadScene,

        /// <summary>
        /// 加载或导入并加载场景
        /// </summary>
        [Name("加载或导入并加载场景")]
        LoadOrImportAndLoadScene,

        /// <summary>
        /// 卸载子场景
        /// </summary>
        [Name("卸载子场景")]
        UnloadSubScene,

        /// <summary>
        /// 卸载子场景(通过索引)
        /// </summary>
        [Name("卸载子场景(通过索引)")]
        UnloadSubSceneByIndex,

        /// <summary>
        /// 卸载全部子场景
        /// </summary>
        [Name("卸载全部子场景")]
        UnloadAllSubScene,

        /// <summary>
        /// 请求场景名称列表
        /// </summary>
        [Name("请求场景名称列表")]
        RequestSceneNameList,

        /// <summary>
        /// 用户自定义
        /// </summary>
        [Name("用户自定义")]
        UserDefine,

        /// <summary>
        /// 调用自定义函数
        /// </summary>
        [Name("调用自定义函数")]
        CallUserDefineFun,

        /// <summary>
        /// 执行XCSJ脚本
        /// </summary>
        [Name("执行XCSJ脚本")]
        RunXCSJScript,

        /// <summary>
        /// 执行单句XCSJ脚本并返回结果
        /// </summary>
        [Name("执行单句XCSJ脚本并返回结果")]
        RunSingleXCSJScriptAndReturnResult,

        /// <summary>
        /// 请求图片二维码扫描
        /// </summary>
        [Name("请求图片二维码扫描")]
        RequestImageQRCodeScan,

        /// <summary>
        /// OS通知参数
        /// </summary>
        [Name("OS通知参数")]
        OSNotifyParams,
    }
}
