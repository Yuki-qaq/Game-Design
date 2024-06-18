using XCSJ.Attributes;
using XCSJ.PluginCNScripts;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Base.Interactions.Tools
{
    /// <summary>
    /// 交互提供者：提供交互所需数据
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class InteractProvider : BaseInteractProvider, ITagPropertyHost, IReferenceObject
    {
        #region 引用对象

        /// <summary>
        /// 引用变量字符串
        /// </summary>
        [Group("基础交互设置", textEN = "Base Interact Settings", defaultIsExpanded = false)]
        [Name("引用变量字符串")]
        [Tip("使用当前组件对象在执行某些特定的交互方法时,传入参数可使用的基于当前组件对象的引用变量字符串，以完成一些特殊参数赋值操作；", "When using the current component object to execute certain specific interaction methods, the reference variable string based on the current component object can be used to pass in parameters to complete some special parameter assignment operations;")]
        [ReferenceVarString]
        public string _referenceVarString = "";

        /// <summary>
        /// 引用对象变量字符串
        /// </summary>
        private string _referenceObjectVarString = null;

        /// <summary>
        /// 引用对象变量字符串
        /// </summary>
        public string referenceObjectVarString => _referenceObjectVarString ?? (_referenceObjectVarString = GetType().GetRootReferenceVarString());

        #endregion

        #region 标签属性

        /// <summary>
        /// 标签属性
        /// </summary>
        [Name("标签属性")]
        [EndGroup(true)]
        public TagProperty _tagProperty = new TagProperty();

        /// <summary>
        /// 标签属性
        /// </summary>
        public ITagProperty tagProperty => _tagProperty;

        #endregion
    }
}
