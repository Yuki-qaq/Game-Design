using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.DataViews.Converters
{
    /// <summary>
    /// 中文脚本转换器
    /// </summary>
    [Name("中文脚本转换器")]
    [XCSJ.Attributes.Icon(EIcon.CNScript)]
    [Tool(XGUICategory.DataConverter, nameof(BaseDataConverter))]
    public class CNScriptConverter : BaseDataConverter, IDataConverter<object, object>
    {
        /// <summary>
        /// 函数类型
        /// </summary>
        public enum EFunctionType
        {
            /// <summary>
            /// 用户自定义函数
            /// </summary>
            [Name("用户自定义函数")]
            UserDefineFun,

            /// <summary>
            /// 本地函数
            /// </summary>
            [Name("本地函数")]
            LocalFunction,
        }

        /// <summary>
        /// 函数类型
        /// </summary>
        [Name("函数类型")]
        [EnumPopup]
        public EFunctionType _functionType = EFunctionType.LocalFunction;

        /// <summary>
        /// 用户自定义转换函数
        /// </summary>
        [Name("用户自定义转换函数")]
        [UserDefineFun]
        [HideInSuperInspector(nameof(_functionType), EValidityCheckType.NotEqual, EFunctionType.UserDefineFun)]
        public string _cnScriptFunction;

        /// <summary>
        /// 本地转换函数
        /// </summary>
        [Name("本地转换函数")]
        [HideInSuperInspector(nameof(_functionType), EValidityCheckType.NotEqual, EFunctionType.LocalFunction)]
        public CustomFunction _localFunction = new CustomFunction();

        /// <summary>
        /// 变量字符串
        /// </summary>
        public const string VarString = "$_MODEL_VIEW&";

        private ReturnValue CallFunction(BaseModelView sender, string inValue)
        {
            try
            {
                ScriptManager.instance.TrySetOrAddSetHierarchyVarValue(VarString, sender);
                switch (_functionType)
                {
                    case EFunctionType.UserDefineFun: return ScriptManager.instance.ExecuteFunction(_cnScriptFunction, inValue);
                    case EFunctionType.LocalFunction: return ScriptManager.instance.ExecuteFunction(_localFunction, inValue);
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
            return ReturnValue.No;
        }

        /// <summary>
        /// 尝试转换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="input"></param>
        /// <param name="outputType"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override bool TryConvertTo(BaseModelView sender, object input, Type outputType, out object output)
        {
            if (input == null)
            {
                return base.TryConvertTo(sender, input, outputType, out output);
            }
            var result = ReturnValue.No;
            if (input is string str)
            {
                result = CallFunction(sender, str);
            }
            else if (Converter.instance.TryConvertTo(input, typeof(string), out var strValue))
            {
                result = CallFunction(sender, strValue as string);
            }

            if (!result.valid) return base.TryConvertTo(sender, input, outputType, out output);

            return base.TryConvertTo(sender, result.Value<string>(), outputType, out output);
        }
    }
}
