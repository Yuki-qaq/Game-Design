using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.NodeKit;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Base.Interactions.Tools
{
    #region 交互器

    /// <summary>
    /// 交互器
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class Interactor<TInteractorInput> : ExtensionalInteractObject, IInteractInputer where TInteractorInput : class, IInteractInput, new()
    {
        /// <summary>
        /// 交互输入器
        /// </summary>
        public override IInteractInputer interactInputer => this;

        /// <summary>        
        /// 交互输入列表
        /// </summary>
        [Name("交互输入列表")]
        [FormerlySerializedAs("_interactorInputs")]
        public List<TInteractorInput> _interactInputs = new List<TInteractorInput>();

        /// <summary>
        /// 交互器输入列表
        /// </summary>
        public IEnumerable<IInteractInput> interactInputs => _interactInputs;
    }

    /// <summary>
    /// 交互器
    /// </summary>
    public abstract class Interactor : Interactor<InteractorInput> { }

    #endregion

    #region 交互输入

    /// <summary>
    /// 交互输入
    /// </summary>
    [Serializable]
    public class InteractorInput : InteractComparer, IInteractInput, INodeModel
    {
        /// <summary>
        /// 比较后处理规则
        /// </summary>
        public enum EMatchHandleRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 替换命令
            /// </summary>
            [Name("替换命令")]
            ReplaceCmd,

            /// <summary>
            /// 替换命令参数
            /// </summary>
            [Name("替换命令参数")]
            ReplaceCmdParam,

            /// <summary>
            /// 替换命令和命令参数
            /// </summary>
            [Name("替换命令和命令参数")]
            ReplaceCmdAndCmdParam,
        }

        /// <summary>
        /// 比较后处理规则
        /// </summary>
        [Name("比较后处理规则")]
        [EnumPopup]
        public EMatchHandleRule _matchHandleRule = EMatchHandleRule.ReplaceCmd;

        /// <summary>
        /// 替换命令名称
        /// </summary>
        [Name("替换命令名称")]
        [Tip("当前所属交互对象的输入命令名称", "The input command name of the current interaction object")]
        public StringPropertyValue _repalceCmdName = new StringPropertyValue();

        /// <summary>
        /// 替换命令参数
        /// </summary>
        [Name("替换命令参数")]
        [Tip("当前所属交互对象的输入命令参数", "Input command parameters of the current interaction object")]
        public StringPropertyValue _repalceCmdParam = new StringPropertyValue();

        /// <summary>
        /// 能否处理
        /// </summary>
        /// <param name="owner">当前对象所属的交互器</param>
        /// <param name="sender">产生交互数据的交互器</param>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool CanHandle(InteractObject owner, InteractObject sender, InteractData interactData)
        {
            return Compare(sender, interactData);
        }

        /// <summary>
        /// 尝试处理
        /// </summary>
        /// <param name="interactor">产生交互数据的交互器</param>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public InteractData Handle(InteractObject interactor, InteractData interactData) => Handle(_matchHandleRule, interactor, interactData);

        /// <summary>
        /// 尝试处理
        /// </summary>
        /// <param name="matchHandleRule">匹配后处理处理规则</param>
        /// <param name="interactor">产生交互数据的交互器</param>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public InteractData Handle(EMatchHandleRule matchHandleRule, InteractObject interactor, InteractData interactData)
        {
            if (interactData == null) return null;

            switch (matchHandleRule)
            {
                case EMatchHandleRule.ReplaceCmd:
                    {
                        if (_repalceCmdName.TryGetValue(out var value))
                        {
                            interactData.SetCmdName(value, interactor);
                        }
                        break;
                    }
                case EMatchHandleRule.ReplaceCmdParam:
                    {
                        if (_repalceCmdParam.TryGetValue(out var value))
                        {
                            interactData.SetCmdParam(value);
                        }
                        break;
                    }
                case EMatchHandleRule.ReplaceCmdAndCmdParam:
                    {
                        Handle(EMatchHandleRule.ReplaceCmd, interactor, interactData);
                        Handle(EMatchHandleRule.ReplaceCmdParam, interactor, interactData);
                        break;
                    }
            }

            return interactData;
        }

        #region 节点模型

        object INodeModel.entityModel => this;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string displayName => ToFriendlyString();

        /// <summary>
        /// 节点矩形
        /// </summary>
        public Rect _nodeRect = new Rect();

        /// <summary>
        /// 节点矩形
        /// </summary>
        public Rect nodeRect { get => _nodeRect; set => _nodeRect = value; }

        /// <summary>
        /// 是有效内容
        /// </summary>
        public bool isValidContent => true;

        #endregion

    }

    #endregion
}
