using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Events
{
    /// <summary>
    /// 运行时平台事件处理器
    /// </summary>
    [Name("运行时平台事件处理器")]
    [Tip("当组件启用时，根据运行时平台信息执行对应的Unity事件；", "When the component is enabled, execute the corresponding Unity event based on runtime platform information;")]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    [Tool(ToolsCategory.InteractOther, nameof(InteractableVirtual), rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    public class RuntimePlatformEventHandler : Interactor
    {
        /// <summary>
        /// 运行时平台Unity事件列表
        /// </summary>
        [Name("运行时平台Unity事件列表")]
        public List<RuntimePlatformUnityEvent> _runtimePlatformUnityEvents = new List<RuntimePlatformUnityEvent>();

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Invoke(Application.platform);
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="runtimePlatform"></param>
        void Invoke(RuntimePlatform runtimePlatform)
        {
            foreach(var e in _runtimePlatformUnityEvents)
            {
                if (e._runtimePlatform == runtimePlatform)
                {
                    e._unityEvent.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// 运行时平台Unity事件
    /// </summary>
    [Serializable]
    public class RuntimePlatformUnityEvent
    {
        /// <summary>
        /// 运行时平台
        /// </summary>
        [Name("运行时平台")]
        public RuntimePlatform _runtimePlatform = RuntimePlatform.WindowsPlayer;

        /// <summary>
        /// Unity事件
        /// </summary>
        [Name("Unity事件")]
        public UnityEvent _unityEvent = new UnityEvent();
    }
}
