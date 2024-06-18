using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Interactions
{
    /// <summary>
    /// 交互事件
    /// </summary>
    [ComponentMenu(ToolsCategory.InteractDirectory + Title, typeof(ToolsManager))]
    [Name(Title, nameof(InteractEvent))]
    [Tip("用于监听交互事件的触发器", "Trigger for listening to interact event")]
    [XCSJ.Attributes.Icon(EIcon.Interact)]
    public class InteractEvent : Trigger<InteractEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "交互事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(ToolsCategory.Interact, typeof(ToolsManager))]
        [StateComponentMenu(ToolsCategory.InteractDirectory + Title, typeof(ToolsManager))]
        [Name(Title, nameof(InteractEvent))]
        [XCSJ.Attributes.Icon(EIcon.Event)]
        public static State CreateInteractEvent(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 交互比较器
        /// </summary>
        [Name("交互比较器")]
        public InteractComparer _interactComparer = new InteractComparer();

        /// <summary>
        /// 命令名称变量
        /// </summary>
        [Group("交互结果存储变量", textEN = "Interact Result Save Variable", defaultIsExpanded = false)]
        [Name("命令名称变量")]
        [Tip("组件处于完成态才进行赋值操作", "Assign a value only when the component is in the completed state")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _cmdNameVariable = "";

        /// <summary>
        /// 命令参数变量
        /// </summary>
        [Name("命令参数变量")]
        [Tip("组件处于完成态才进行赋值操作", "Assign a value only when the component is in the completed state")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _cmdParamVariable = "";

        /// <summary>
        /// 命令参数表达式字符串键变量
        /// </summary>
        [Name("命令参数表达式字符串键变量")]
        [Tip("组件处于完成态才进行赋值操作", "Assign a value only when the component is in the completed state")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _cmdParamExpressionStringKeyVariable = "";

        /// <summary>
        /// 命令参数表达式字符串值变量
        /// </summary>
        [Name("命令参数表达式字符串值变量")]
        [Tip("组件处于完成态才进行赋值操作", "Assign a value only when the component is in the completed state")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _cmdParamExpressionStringValueVariable = "";

        /// <summary>
        /// 交互器变量
        /// </summary>
        [Name("交互器变量")]
        [Tip("组件处于完成态才进行赋值操作", "Assign a value only when the component is in the completed state")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _interactorVariable = "";

        /// <summary>
        /// 交互器名称变量
        /// </summary>
        [Name("交互器名称变量")]
        [Tip("组件处于完成态才进行赋值操作", "Assign a value only when the component is in the completed state")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _interactorNameVariable = "";

        /// <summary>
        /// 可交互对象变量
        /// </summary>
        [Name("可交互对象变量")]
        [Tip("组件处于完成态才进行赋值操作", "Assign a value only when the component is in the completed state")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _interactableVariable = "";

        /// <summary>
        /// 可交互对象名称变量
        /// </summary>
        [Name("可交互对象名称变量")]
        [Tip("组件处于完成态才进行赋值操作", "Assign a value only when the component is in the completed state")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _interactableNameVariable = "";

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            InteractObject.onInteractEntry += OnInteract;
            InteractObject.onInteractProcessing += OnInteract;
            InteractObject.onInteractFinished += OnInteract;
            InteractObject.onInteractAborted += OnInteract;
            InteractObject.onInteractExit += OnInteract;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            InteractObject.onInteractEntry -= OnInteract;
            InteractObject.onInteractProcessing -= OnInteract;
            InteractObject.onInteractFinished -= OnInteract;
            InteractObject.onInteractAborted -= OnInteract;
            InteractObject.onInteractExit -= OnInteract;
        }

        private void OnInteract(InteractObject interactor, InteractData interactData)
        {
            if (!finished && _interactComparer.Compare(interactData))
            {
                finished = true;
                var instance = ScriptManager.instance;
                if (instance)
                {
                    instance.TrySetHierarchyVarValue(_cmdNameVariable, interactData.cmdName);
                    instance.TrySetHierarchyVarValue(_cmdParamVariable, interactData.cmdParam);

                    if (interactData.cmdParam != null && ExpressionStringAnalysisResult.TryParse(interactData.cmdParam.ToString(), out var result) && !string.IsNullOrEmpty(result.key))
                    {
                        instance.TrySetHierarchyVarValue(_cmdParamExpressionStringKeyVariable, result.key);
                        instance.TrySetHierarchyVarValue(_cmdParamExpressionStringValueVariable, result.value);
                    }

                    instance.TrySetHierarchyVarValue(_interactorVariable, interactor);
                    instance.TrySetHierarchyVarValue(_interactorNameVariable, interactor ? interactor.name : "");

                    var interactable = interactData.interactable;
                    instance.TrySetHierarchyVarValue(_interactableVariable, interactable);
                    instance.TrySetHierarchyVarValue(_interactableNameVariable, interactable ? interactable.name : "");
                }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _interactComparer.DataValidity();

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => _interactComparer.ToFriendlyString();
    }
}
