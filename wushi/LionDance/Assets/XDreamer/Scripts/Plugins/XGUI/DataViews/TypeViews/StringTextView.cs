using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.TypeViews
{
    /// <summary>
    /// 字符串文本数据视图
    /// </summary>
    [Name("字符串文本数据视图")]
    [DataViewAttribute(typeof(string))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class StringTextView : BaseModelView
    {
        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _stringView;

        /// <summary>
        /// 视图值类型
        /// </summary>
        public override Type viewValueType => typeof(string);

        /// <summary>
        /// 视图值
        /// </summary>
        public override object viewValue { get => _stringView.text; set => _stringView.text = (string)value; }
    }
}
