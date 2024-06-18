using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools;

namespace XCSJ.PluginSMS.States.Interactions
{
    /// <summary>
    /// 执行交互
    /// </summary>
    [ComponentMenu(ToolsCategory.InteractDirectory + Title, typeof(ToolsManager))]
    [Name(Title, nameof(ExecuteInteract))]
    [XCSJ.Attributes.Icon(EIcon.Interact)]
    public class ExecuteInteract : LifecycleExecutor<ExecuteInteract>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "执行交互";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(ToolsCategory.Interact, typeof(ToolsManager))]
        [StateComponentMenu(ToolsCategory.InteractDirectory + Title, typeof(ToolsManager))]
        [Name(Title, nameof(ExecuteInteract))]
        [XCSJ.Attributes.Icon(EIcon.Interact)]
        public static State CreateInteractEvent(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 交互信息
        /// </summary>
        [Name("交互信息")]
        public ExecuteInteractInfo _executeInteractInfo = new ExecuteInteractInfo();

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _executeInteractInfo._interactor;

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => _executeInteractInfo._inCmdName.GetValue();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode) => _executeInteractInfo.TryInteract();
    }
}
