using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Events
{
    /// <summary>
    /// 自定义命令交互器
    /// </summary>
    [Name("自定义命令交互器")]
    [XCSJ.Attributes.Icon(EIcon.Command)]
    [Tool(ToolsCategory.InteractOther, rootType = typeof(ToolsManager))]
    public sealed class CustomCmdInteractor : Interactor
    {
        /// <summary>
        /// 静态实例
        /// </summary>
        public static CustomCmdInteractor instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = UnityObjectExtension.GetComponentInGlobal<CustomCmdInteractor>();
                    if (!_instance)
                    {
                        _instance = ToolsManager.instance.XAddComponent<CustomCmdInteractor>();
                    }
                }
                return _instance;
            }
        }
        private static CustomCmdInteractor _instance;

        /// <summary>
        /// 唤醒
        /// </summary>
        private void Awake()
        {
            _instance = this;
        }

        /// <summary>
        /// 扩展命令
        /// </summary>
        public override string[] extendedCmds => _customCmds.Cast(d => d._cmd).Distinct().ToArray();

        /// <summary>
        /// 尝试获取扩展命令通过扩展友好命令
        /// </summary>
        /// <param name="extendedFriendlyCmd"></param>
        /// <param name="extendedCmd"></param>
        /// <returns></returns>
        public override bool TryGetExtendedCmdByExtendedFriendlyCmd(string extendedFriendlyCmd, out string extendedCmd)
        {
            if(_customCmds.Any(d => d._cmd == extendedFriendlyCmd))
            {
                extendedCmd = extendedFriendlyCmd;
                return true;
            }
            return base.TryGetExtendedCmdByExtendedFriendlyCmd(extendedFriendlyCmd, out extendedCmd);
        }

        /// <summary>
        /// 尝试获取扩展友好命令通过扩展命令
        /// </summary>
        /// <param name="extendedCmd"></param>
        /// <param name="extendedFriendlyCmd"></param>
        /// <returns></returns>
        public override bool TryGetExtendedFriendlyCmdByExtendedCmd(string extendedCmd, out string extendedFriendlyCmd)
        {
            if (_customCmds.Any(d => d._cmd == extendedCmd))
            {
                extendedFriendlyCmd = extendedCmd;
                return true;
            }
            return base.TryGetExtendedFriendlyCmdByExtendedCmd(extendedCmd, out extendedFriendlyCmd);
        }

        /// <summary>
        /// 自定义命令列表
        /// </summary>
        [Name("自定义命令列表")]
        public List<InteractorUnityEventData> _customCmds = new List<InteractorUnityEventData>();

        /// <summary>
        /// 当扩展交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        protected override EInteractResult OnExtensionalInteract(InteractData interactData, EInteractResult interactResult)
        {
            if (Invoke(interactData)) return EInteractResult.Success;
            return base.OnExtensionalInteract(interactData, interactResult);
        }

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="interactData"></param>
        private bool Invoke(InteractData interactData)
        {
            var result = false;
            foreach (var data in _customCmds)
            {
                if (data._cmd == interactData.cmd)
                {
                    data._interactorUnityEvent.Invoke(interactData);
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 调用交互完成
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactables"></param>
        public void CallInteractFinished(string cmdName, params InteractObject[] interactables) => CallFinished(new InteractData(cmdName, this, interactables));
    }

    /// <summary>
    /// 交互器Unity事件数据
    /// </summary>
    [Serializable]
    public class InteractorUnityEventData
    {
        /// <summary>
        /// 命令
        /// </summary>
        [Name("命令")]
        public string _cmd = "";

        /// <summary>
        /// 交互器事件
        /// </summary>
        [Name("交互器事件")]
        public InteractUnityEvent _interactorUnityEvent = new InteractUnityEvent();
    }
}
