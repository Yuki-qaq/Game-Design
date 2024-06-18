using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginCNScripts
{
    /// <summary>
    /// 中文脚本组手
    /// </summary>
    public static class CNScriptHelper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            HierarchyKeyExtensionHelper.Init();
        }

        /// <summary>
        /// 分析表达式并设置变量值
        /// </summary>
        /// <param name="expressionString"></param>
        /// <param name="referenceObject"></param>
        /// <returns></returns>
        public static string ParseExpressionAndSetVarValue(string expressionString, IReferenceObject referenceObject)
        {
            if (referenceObject == null) return expressionString;
            return ParseExpressionAndSetVarValue(expressionString, referenceObject.referenceObjectVarString, referenceObject);
        }

        /// <summary>
        /// 分析表达式并设置变量值
        /// </summary>
        /// <param name="expressionString"></param>
        /// <param name="varString"></param>
        /// <param name="varValue"></param>
        /// <returns></returns>
        public static string ParseExpressionAndSetVarValue(string expressionString, string varString, object varValue)
        {
            if (ExpressionStringAnalysisResult.TryParse(expressionString, out var result) && result.hasMarker)
            {
                var sm = ScriptManager.instance;
                if (sm)
                {
                    sm.TrySetOrAddSetHierarchyVarValue(varString, varValue);

                    // 执行表达式字符串
                    if (result.TryCalculateExpression(sm, out object calculateResult)
                        && Converter.instance.TryConvertTo<string>(calculateResult, out var value))
                    {
                        return value;
                    }
                }
            }
            return expressionString;
        }

        static Dictionary<Type, string> cache = new Dictionary<Type, string>();

        /// <summary>
        /// 获取根引用变量字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetRootReferenceVarString(this object obj) => GetRootReferenceVarString(obj?.GetType());

        /// <summary>
        /// 获取根引用变量字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetRootReferenceVarString(this Type type)
        {
            if (type == null) return "";
            if (!cache.TryGetValue(type, out var value))
            {
                value = ScriptHelper.VarFlag + "_" + StringHelper.StringTransform(type.Name).Replace(" ", "_").ToUpper() + EVarScope.Reference.ToVarScopeString();
                cache[type] = value;
            }
            return value;
        }
    }

    /// <summary>
    /// 引用对象
    /// </summary>
    public interface IReferenceObject
    {
        /// <summary>
        /// 引用对象变量字符串
        /// </summary>
        string referenceObjectVarString { get; }
    }

    /// <summary>
    /// 引用变量字符串特性
    /// </summary>
    public class ReferenceVarStringAttribute : PropertyAttribute
    {
        /// <summary>
        /// 根引用变量字符
        /// </summary>
        public string rootReferenceVarString { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="rootReferenceVarString"></param>
        public ReferenceVarStringAttribute(string rootReferenceVarString = "")
        {
            this.rootReferenceVarString = rootReferenceVarString;
        }
    }

    /// <summary>
    /// 设置变量字符串值
    /// </summary>
    [Serializable]
    public class SetVarStringValue
    {
        /// <summary>
        /// 结果变量字符串
        /// </summary>
        [Name("结果变量字符串")]
        [Tip("将成功执行的结果信息存储在结果变量字符串对应的变量中", "Store the successful execution result information in the variable corresponding to the result variable string")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _resultVarString;

        /// <summary>
        /// 结果变量字符串列表
        /// </summary>
        [Name("结果变量字符串列表")]
        [Tip("将成功执行的结果信息存储在结果变量字符串列表内每个变量字符串对应的变量中", "Store the successful execution result information in the variable corresponding to each variable string in the result variable string list")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public List<string> _resultVarStrings = new List<string>();

        /// <summary>
        /// 设置变量值
        /// </summary>
        /// <param name="varValue"></param>
        /// <returns></returns>
        public bool SetVarValue(object varValue) => _resultVarString.TrySetOrAddSetHierarchyVarValue(varValue, _resultVarStrings);
    }

    /// <summary>
    /// 中文脚本分类
    /// </summary>
    public static class CNScriptCategory
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "中文脚本";

        /// <summary>
        /// 标题前缀
        /// </summary>
        public const string TitlePrefix = Title + CommonCategory.HorizontalLine;

        /// <summary>
        /// 标题目录
        /// </summary>
        public const string TitleDirectory = Title + CommonCategory.PathSplitLine;

        /// <summary>
        /// 变量
        /// </summary>
        public const string Var = TitlePrefix + "变量";

        /// <summary>
        /// 组件
        /// </summary>
        public const string ComponentEvent = TitlePrefix + CommonCategory.Component + CommonCategory.Event;

        /// <summary>
        /// UGUI
        /// </summary>
        public const string UGUI = TitlePrefix + CommonCategory.UGUIEvent;

        /// <summary>
        /// 输入
        /// </summary>
        public const string Input = TitlePrefix + CommonCategory.Input;

        /// <summary>
        /// 中文脚本菜单
        /// </summary>
        public const string CNScriptMenu = Product.Name + "/中文脚本/";

        /// <summary>
        /// 输入菜单
        /// </summary>
        public const string InputMenu = CNScriptMenu + "输入/";

        /// <summary>
        /// UGUI菜单
        /// </summary>
        public const string UGUIMenu = CNScriptMenu + "UGUI/";
    }
}
