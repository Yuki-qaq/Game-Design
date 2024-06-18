using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Components;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.DataViews.Base
{
    /// <summary>
    /// 基础数据转换器
    /// </summary>
    [RequireManager(typeof(XGUIManager))]
    [XCSJ.Attributes.Icon(EIcon.Switch)]
    public abstract class BaseDataConverter : InteractProvider
    {
        /// <summary>
        /// 尝试转换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="input"></param>
        /// <param name="outputType"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public virtual bool TryConvertTo(BaseModelView sender, object input, Type outputType, out object output)
        {
            return Converter.instance.TryConvertTo(input, outputType, out output);
        }
    }

    /// <summary>
    /// 数据转换器接口
    /// </summary>
    public interface IDataConverter { }

    /// <summary>
    /// 数据转换器接口：限定输入输出类型的数据转换器
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public interface IDataConverter<TInput, TOutput> : IDataConverter { }
}
