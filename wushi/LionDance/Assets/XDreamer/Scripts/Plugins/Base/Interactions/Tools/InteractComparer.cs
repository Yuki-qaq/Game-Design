using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Helper;
using XCSJ.PluginCNScripts;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.Scripts;
using static XCSJ.PluginTools.Motions.InputWaitPlayer;

namespace XCSJ.Extension.Base.Interactions.Tools
{
    /// <summary>
    /// 比较对象类型
    /// </summary>
    public enum ECompareObjectType
    {
        /// <summary>
        /// 交互器
        /// </summary>
        [Name("交互器")]
        Interactor,

        /// <summary>
        /// 命令名称
        /// </summary>
        [Name("命令名称")]
        CmdName,

        /// <summary>
        /// 可交互对象
        /// </summary>
        [Name("可交互对象")]
        Interactable,

        /// <summary>
        /// 标签值
        /// </summary>
        [Name("标签值")]
        TagValue,

        /// <summary>
        /// 命令参数字符串
        /// </summary>
        [Name("命令参数字符串")]
        CmdParamString,

        /// <summary>
        /// 命令参数表达式字符串
        /// </summary>
        [Name("命令参数表达式字符串")]
        [Tip("命令参数表达式字符串键和值都匹配", "Command parameter expression string key and value match")]
        CmdParamExpressionString,

        /// <summary>
        /// 命令参数表达式字符串键
        /// </summary>
        [Name("命令参数表达式字符串键")]
        [Tip("命令参数表达式字符串键匹配", "Command parameter expression string key match")]
        CmdParamExpressionStringKey,

        /// <summary>
        /// 命令参数表达式字符串值
        /// </summary>
        [Name("命令参数表达式字符串值")]
        [Tip("命令参数表达式字符串值匹配", "Command parameter expression string value match")]
        CmdParamExpressionStringValue,

        /// <summary>
        /// 命令参数Unity对象
        /// </summary>
        [Name("命令参数Unity对象")]
        CmdParamUnityObject,
    }

    /// <summary>
    /// 比较条件
    /// </summary>
    public enum ECompereCondition
    {
        /// <summary>
        /// 相等
        /// </summary>
        [Name("相等")]
        Equal,

        /// <summary>
        /// 不等
        /// </summary>
        [Name("不等")]
        NotEqual,
    }

    /// <summary>
    /// 基础比较数据
    /// </summary>
    public abstract class BaseCompareData
    {
        /// <summary>
        /// 比较对象类型
        /// </summary>
        [Name("比较对象类型")]
#if UNITY_2021_3_OR_NEWER
        [DynamicLabel]
#endif
        [EnumPopup]
        public ECompareObjectType _compareObjectType = ECompareObjectType.Interactor;

        /// <summary>
        /// 比较条件
        /// </summary>
        [Name("比较条件")]
#if UNITY_2021_3_OR_NEWER
        [DynamicLabel]
#endif
        [EnumPopup]
        public ECompereCondition _compereCondition = ECompereCondition.Equal;

        /// <summary>
        /// 输出命令名称
        /// </summary>
        [Name("输出命令名称")]
        [HideInSuperInspector(nameof(_compareObjectType), EValidityCheckType.NotEqual, ECompareObjectType.CmdName)]
        public StringPropertyValue _outCmdName = new StringPropertyValue();

        /// <summary>
        /// 标签值
        /// </summary>
        [Name("标签值")]
        [HideInSuperInspector(nameof(_compareObjectType), EValidityCheckType.NotEqual, ECompareObjectType.TagValue)]
        public StringPropertyValue _tagvalue = new StringPropertyValue();

        /// <summary>
        /// 命令参数字符串
        /// </summary>
        [Name("命令参数字符串")]
        [HideInSuperInspector(nameof(_compareObjectType), EValidityCheckType.NotEqual, ECompareObjectType.CmdParamString)]
        public StringPropertyValue _cmdParamString = new StringPropertyValue();

        /// <summary>
        /// 命令参数表达式字符串键
        /// </summary>
        [Name("命令参数表达式字符串键")]
        [HideInSuperInspector(nameof(_compareObjectType), EValidityCheckType.NotEqual | EValidityCheckType.Or, ECompareObjectType.CmdParamExpressionString, nameof(_compareObjectType), EValidityCheckType.NotEqual, ECompareObjectType.CmdParamExpressionStringKey)]
        public StringPropertyValue _cmdParamExpressionStringKey = new StringPropertyValue();

        /// <summary>
        /// 命令参数表达式字符串值
        /// </summary>
        [Name("命令参数表达式字符串值")]
        [HideInSuperInspector(nameof(_compareObjectType), EValidityCheckType.NotEqual | EValidityCheckType.Or, ECompareObjectType.CmdParamExpressionString, nameof(_compareObjectType), EValidityCheckType.NotEqual, ECompareObjectType.CmdParamExpressionStringValue)]
        public StringPropertyValue _cmdParamExpressionStringValue = new StringPropertyValue();

        /// <summary>
        /// 命令参数Unity对象
        /// </summary>
        [Name("命令参数Unity对象")]
        [HideInSuperInspector(nameof(_compareObjectType), EValidityCheckType.NotEqual, ECompareObjectType.CmdParamUnityObject)]
        public UnityObjectPropertyValue _cmdParamUnityObject = new UnityObjectPropertyValue();

        /// <summary>
        /// 交互器
        /// </summary>
        public abstract InteractObject interactor { get; }

        /// <summary>
        /// 可交互对象
        /// </summary>
        public abstract InteractObject interactable { get; }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool Compere(InteractData interactData)
        {
            switch (_compereCondition)
            {
                case ECompereCondition.Equal: return IsEquals(interactData);
                case ECompereCondition.NotEqual: return !IsEquals(interactData);
            }
            return false;
        }

        private bool IsEquals(InteractData interactData)
        {
            switch (_compareObjectType)
            {
                case ECompareObjectType.Interactor: return InteractorEquals(interactData.interactor);
                case ECompareObjectType.CmdName: return CmdNameEquals(interactData.cmdName);
                case ECompareObjectType.Interactable: return InteractableEquals(interactData.interactable);
                case ECompareObjectType.TagValue: return TagValueEquals(interactData.interactor.tag);
                case ECompareObjectType.CmdParamString: return CmdParamStringEquals(interactData.cmdParam);
                case ECompareObjectType.CmdParamExpressionString: return CmdParamExpressionStringEquals(interactData.cmdParam);
                case ECompareObjectType.CmdParamExpressionStringKey: return CmdParamExpressionStringKeyEquals(interactData.cmdParam);
                case ECompareObjectType.CmdParamExpressionStringValue: return CmdParamExpressionStringValueEquals(interactData.cmdParam);
                case ECompareObjectType.CmdParamUnityObject: return CmdParamUnityObjectEquals(interactData.cmdParam);
            }
            return false;
        }

        private bool InteractorEquals(InteractObject interactor) => interactor != null && interactor == this.interactor;

        private bool CmdNameEquals(string cmdName) => !string.IsNullOrEmpty(cmdName) && cmdName == _outCmdName.GetValue();

        private bool InteractableEquals(InteractObject interactable) => interactable && interactable == this.interactable;

        private bool TagValueEquals(string tagValue) => !string.IsNullOrEmpty(tagValue) && tagValue == _tagvalue.GetValue();

        private bool CmdParamStringEquals(object cmdParam) => cmdParam != null && cmdParam.ObjectToString() == _cmdParamString.GetValue();

        private bool CmdParamExpressionStringEquals(object cmdParam)
        {
            if (cmdParam != null && ExpressionStringAnalysisResult.TryParse(cmdParam.ToString(), out var result) && !string.IsNullOrEmpty(result.key))
            {
                return result.key == _cmdParamExpressionStringKey.GetValue() && result.value == _cmdParamExpressionStringValue.GetValue();
            }
            return false;
        }

        private bool CmdParamExpressionStringKeyEquals(object cmdParam)
        {
            if (cmdParam != null && ExpressionStringAnalysisResult.TryParse(cmdParam.ToString(), out var result) && !string.IsNullOrEmpty(result.key))
            {
                return result.key == _cmdParamExpressionStringKey.GetValue();
            }
            return false;
        }

        private bool CmdParamExpressionStringValueEquals(object cmdParam)
        {
            if (cmdParam != null && ExpressionStringAnalysisResult.TryParse(cmdParam.ToString(), out var result) && !string.IsNullOrEmpty(result.key))
            {
                return result.value == _cmdParamExpressionStringValue.GetValue();
            }
            return false;
        }

        private bool CmdParamUnityObjectEquals(object cmdParam) => (cmdParam as UnityEngine.Object) == _cmdParamUnityObject.GetValue();

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public bool DataValidity()
        {
            switch (_compareObjectType)
            {
                case ECompareObjectType.Interactor: return interactor;
                case ECompareObjectType.CmdName: return !string.IsNullOrEmpty(_outCmdName.GetValue());
                case ECompareObjectType.Interactable: return interactable;
                case ECompareObjectType.TagValue: return !string.IsNullOrEmpty(_tagvalue.GetValue());
                case ECompareObjectType.CmdParamString: return !string.IsNullOrEmpty(_cmdParamString.GetValue());
                case ECompareObjectType.CmdParamExpressionString:
                case ECompareObjectType.CmdParamExpressionStringKey: return !string.IsNullOrEmpty(_cmdParamExpressionStringKey.GetValue());
                case ECompareObjectType.CmdParamExpressionStringValue:
                case ECompareObjectType.CmdParamUnityObject: return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 交互数据模版
    /// </summary>
    /// <typeparam name="TInteractor"></typeparam>
    /// <typeparam name="TInteractable"></typeparam>
    public class BaseCompareData<TInteractor, TInteractable> : BaseCompareData
        where TInteractor : InteractObject
        where TInteractable : InteractObject
    {
        /// <summary>
        /// 交互器
        /// </summary>
        [Name("交互器")]
        [HideInSuperInspector(nameof(_compareObjectType), EValidityCheckType.NotEqual, ECompareObjectType.Interactor)]
        [ComponentPopup]
        public TInteractor _interactor;

        /// <summary>
        /// 可交互对象
        /// </summary>
        [Name("可交互对象")]
        [HideInSuperInspector(nameof(_compareObjectType), EValidityCheckType.NotEqual, ECompareObjectType.Interactable)]
        [ComponentPopup]
        public TInteractable _interactable;

        /// <summary>
        /// 交互器
        /// </summary>
        public override InteractObject interactor => _interactor;

        /// <summary>
        /// 可交互对象
        /// </summary>
        public override InteractObject interactable => _interactable;
    }

    /// <summary>
    /// 比较数据列表规则
    /// </summary>
    public enum ECompareDataListRule
    {
        /// <summary>
        /// 所有
        /// </summary>
        [Name("所有")]
        [Tip("所有比较条件都匹配", "All comparison criteria match")]
        All,

        /// <summary>
        /// 任意
        /// </summary>
        [Name("任意")]
        [Tip("比较条件任意一个匹配", "Compare any matching condition")]
        Any,
    }

    /// <summary>
    /// 基础交互比较器
    /// </summary>
    public abstract class BaseInteractComparer<T> where T : BaseCompareData
    {
        /// <summary>
        /// 交互状态
        /// </summary>
        [Name("交互状态")]
        [EnumPopup]
        public EInteractState _interactState = EInteractState.Finished;

        /// <summary>
        /// 比较数据列表规则
        /// </summary>
        [Name("比较数据列表规则")]
        [EnumPopup]
        public ECompareDataListRule _compareDataListRule = ECompareDataListRule.All;

        /// <summary>
        /// 比较数据列表
        /// </summary>
        [Name("比较数据列表")]
        [Tip("列表数量为0时比较结果为False", "The comparison result is false when the number of lists is 0")]
        public List<T> _compereDatas = new List<T>();

        /// <summary>
        /// 比较数据列表
        /// </summary>
        protected virtual List<T> compereDatas => _compereDatas;

        /// <summary>
        /// 输入交互器列表
        /// </summary>
        public List<InteractObject> inputInteractors => compereDatas.Where(d => d._compareObjectType == ECompareObjectType.Interactor).Cast(c => c.interactor).ToList();

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool Compare(InteractData interactData) => Compare(interactData.interactor, interactData);

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool Compare(InteractObject sender, InteractData interactData)
        {
            if (interactData == null || _interactState != interactData.interactState || compereDatas.Count == 0) return false;

            switch (_compareDataListRule)
            {
                case ECompareDataListRule.All: return compereDatas.All(d => d.Compere(interactData));
                case ECompareDataListRule.Any: return compereDatas.Any(d => d.Compere(interactData));
            }
            return false;
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public bool DataValidity() => compereDatas.All(d => d.DataValidity());
    }

    /// <summary>
    /// 比较数据
    /// </summary>
    [Serializable]
    public class CompareData : BaseCompareData<InteractObject, InteractObject> { }

    /// <summary>
    /// 交互比较器：用于接收交互，并比较命令字符串、交互器和可交互对象是否匹配
    /// </summary>
    [Serializable]
    public class InteractComparer : BaseInteractComparer<CompareData>
    {
        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString()
        {
            if (_compereDatas.Count > 0)
            {
                return _compereDatas.Find(d => d._compareObjectType == ECompareObjectType.CmdName)?._outCmdName.GetValue() ?? "";
            }
            return "";
        }
    }

    /// <summary>
    /// 执行交互信息：用于执行（发起）交互
    /// </summary>
    [Serializable]
    public class ExecuteInteractInfo : ExecuteInteractInfo<ExtensionalInteractObject>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExecuteInteractInfo() { }
    }

    /// <summary>
    /// 命令参数规则
    /// </summary>
    public enum ECmdParamRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 字符串
        /// </summary>
        [Name("字符串")]
        String,

        /// <summary>
        /// 表达式字符串
        /// </summary>
        [Name("表达式字符串")]
        ExpressionString,

        /// <summary>
        /// 输入源
        /// </summary>
        [Name("输入源")]
        [Tip("使用执行交互方法的传入参数", "Using incoming parameters for executing interactive methods")]
        InputSource,

        /// <summary>
        /// 表达式值使用输入源
        /// </summary>
        [Name("表达式值使用输入源")]
        [Tip("表达式值使用执行交互方法的传入参数", "The expression value uses the incoming parameters of the execution interaction method")]
        ExpressionValueUseInputSource,

        /// <summary>
        /// Unity对象
        /// </summary>
        [Name("Unity对象")]
        UnityObject,
    }

    /// <summary>
    /// 执行交互信息
    /// </summary>
    public class ExecuteInteractInfo<T> where T : ExtensionalInteractObject
    {
        /// <summary>
        /// 交互器
        /// </summary>
        [Name("交互器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public T _interactor;

        /// <summary>
        /// 命令名称
        /// </summary>
        [Name("输入命令名称")]
        public StringPropertyValue _inCmdName = new StringPropertyValue();

        /// <summary>
        /// 可交互对象
        /// </summary>
        [Name("可交互对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public InteractableEntity _interactableEntity = null;

        /// <summary>
        /// 命令参数规则
        /// </summary>
        [Name("命令参数规则")]
        [EnumPopup]
        public ECmdParamRule _cmdParamRule = ECmdParamRule.None;

        /// <summary>
        /// 命令参数字符串
        /// </summary>
        [Name("命令参数字符串")]
        [HideInSuperInspector(nameof(_cmdParamRule), EValidityCheckType.NotEqual, ECmdParamRule.String)]
        public StringPropertyValue _cmdParamString = new StringPropertyValue();

        /// <summary>
        /// 命令参数表达式字符串键
        /// </summary>
        [Name("命令参数表达式字符串键")]
        [HideInSuperInspector(nameof(_cmdParamRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, ECmdParamRule.ExpressionString, nameof(_cmdParamRule), EValidityCheckType.NotEqual, ECmdParamRule.ExpressionValueUseInputSource)]
        public StringPropertyValue _cmdParamExpressionStringKey = new StringPropertyValue();

        /// <summary>
        /// 命令参数表达式字符串值
        /// </summary>
        [Name("命令参数表达式字符串值")]
        [HideInSuperInspector(nameof(_cmdParamRule), EValidityCheckType.NotEqual, ECmdParamRule.ExpressionString)]
        public StringPropertyValue _cmdParamExpressionStringValue = new StringPropertyValue();

        /// <summary>
        /// Unity对象
        /// </summary>
        [Name("Unity对象")]
        [HideInSuperInspector(nameof(_cmdParamRule), EValidityCheckType.NotEqual, ECmdParamRule.UnityObject)]
        public UnityObjectPropertyValue _unityObject = new UnityObjectPropertyValue();

        /// <summary>
        /// 执行交互
        /// </summary>
        /// <param name="cmdParam"></param>
        /// <returns></returns>
        public virtual bool TryInteract(object cmdParam = null)
        {
            try
            {
                if (_interactor && _inCmdName.TryGetValue(out var cmdName))
                {
                    object targetCmdParam = null;
                    switch (_cmdParamRule)
                    {
                        case ECmdParamRule.String:
                            {
                                targetCmdParam = _cmdParamString.GetValue();
                                break;
                            }
                        case ECmdParamRule.ExpressionString:
                            {
                                targetCmdParam = string.Format("{0}:{1}", _cmdParamExpressionStringKey.GetValue(), _cmdParamExpressionStringValue.GetValue());
                                break;
                            }
                        case ECmdParamRule.InputSource:
                            {
                                targetCmdParam = cmdParam;
                                break;
                            }
                        case ECmdParamRule.ExpressionValueUseInputSource:
                            {
                                targetCmdParam = string.Format("{0}:{1}", _cmdParamExpressionStringKey.GetValue(), cmdParam != null ? cmdParam.ObjectToString() : "");
                                break;
                            }
                        case ECmdParamRule.UnityObject:
                            {
                                targetCmdParam = _unityObject.GetValue();
                                break;
                            }
                    }
                    return _interactor.TryInteract(_interactor.GetInCmd(cmdName), targetCmdParam, _interactableEntity);
                }
                return false;
            }
            catch(Exception ex)
            {
                ex.HandleException(nameof(ExecuteInteractInfo));
                return false;
            }
        }
    }
}
