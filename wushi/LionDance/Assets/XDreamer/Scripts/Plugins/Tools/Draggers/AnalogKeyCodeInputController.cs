using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginXRSpaceSolution.Base;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 模拟键码输入控制器
    /// </summary>
    [Name("模拟键码输入控制器")]
    public class AnalogKeyCodeInputController : Interactor
    {
        /// <summary>
        /// 模拟键码输入
        /// </summary>
        [Name("模拟键码输入")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AnalogKeyCodeInput _analogKeyCodeInput = null;

        /// <summary>
        /// 模拟键码输入
        /// </summary>
        public AnalogKeyCodeInput analogKeyCodeInput => this.XGetComponentInChildrenOrGlobal<AnalogKeyCodeInput>(ref _analogKeyCodeInput);

        /// <summary>
        /// 命令名称
        /// </summary>
        [Name("命令名称")]
        public string _cmdName = "";

        /// <summary>
        /// 命令名称
        /// </summary>
        public string cmdName { get => _cmdName; set => _cmdName = value; }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void ExecuteCmd() => ExecuteCmdInternal(cmdName);

        /// <summary>
        /// 执行主菜单命令
        /// </summary>
        public void ExecuteMainMenuCmd() => ExecuteCmdInternal(EnumHelper.GetEnumString(EActionName.MainMenu, EEnumStringType.NameAttribute));

        private void ExecuteCmdInternal(string cmdName) => analogKeyCodeInput.TryInteract(cmdName);

}
}
