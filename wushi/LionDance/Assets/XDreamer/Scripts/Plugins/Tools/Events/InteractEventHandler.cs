using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;
using static XCSJ.PluginTools.Events.InteractEventHandler;

namespace XCSJ.PluginTools.Events
{
    /// <summary>
    /// 交互事件处理器:监听交互事件，比较器匹配后执行UnityEvent
    /// </summary>
    [Name("交互事件处理器")]
    [Tip("监听交互事件，比较匹配后执行UnityEvent", "Listen for interactive events, and execute UnityEvent after comparing and matching")]
    [XCSJ.Attributes.Icon(EIcon.Interact)]
    [Tool(ToolsCategory.InteractCommon, nameof(InteractableVirtual), rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    public class InteractEventHandler : Interactor<InteractUnityEventExecuter>
    {
        /// <summary>
        /// 当输入交互
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="interactData"></param>
        protected override void OnInputInteract(InteractObject sender, InteractData interactData)
        {
            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.SetSystemVariableValues(this.gameObject.GameObjectToString());
                scriptManager.TrySetOrAddSetHierarchyVarValue(this.referenceObjectVarString, this);
            }

            base.OnInputInteract(sender, interactData);
        }

        /// <summary>
        /// 交互Unity事件数据
        /// </summary>
        [Serializable]
        public class InteractUnityEventExecuter : InteractComparer, IInteractInput
        {
            /// <summary>
            /// 交互Unity事件
            /// </summary>
            [Name("交互Unity事件")]
            public InteractUnityEvent _interactUnityEvent = new InteractUnityEvent();

            /// <summary>
            /// 交互回调函数
            /// </summary>
            [Name("交互回调函数")]
            public CustomFunction _interactCallbackFunction = new CustomFunction();

            /// <summary>
            /// 交互执行列表
            /// </summary>
            [Name("交互执行列表")]
            public List<ExecuteInteractInfo> _executeInteractInfos = new List<ExecuteInteractInfo>();

            /// <summary>
            /// 能否处理
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="sender"></param>
            /// <param name="interactData"></param>
            /// <returns></returns>
            public bool CanHandle(InteractObject owner, InteractObject sender, InteractData interactData)
            {
                return Compare(interactData);
            }

            /// <summary>
            /// 执行Unity定义事件
            /// </summary>
            /// <param name="interactor"></param>
            /// <param name="interactData"></param>
            /// <returns></returns>
            public InteractData Handle(InteractObject interactor, InteractData interactData)
            {
                _interactUnityEvent.Invoke(interactData);
                var scriptManager = ScriptManager.instance;
                if (scriptManager)
                {
                    scriptManager.ExecuteFunction(_interactCallbackFunction);
                }
                foreach (var item in _executeInteractInfos)
                {
                    item.TryInteract();
                }
                return interactData;
            }

        }

        /// <summary>
        /// 直接处理
        /// </summary>
        public void DirectHandle()
        {
            foreach (var item in _interactInputs)
            {
                item.Handle(null, null);
            }
        }
    }

    /// <summary>
    /// 交互Unity事件
    /// </summary>
    [Serializable]
    public class InteractUnityEvent : UnityEvent<InteractData> { }
}
