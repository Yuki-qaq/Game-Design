using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.Converters
{
    /// <summary>
    /// 枚举与字符串转换器
    /// </summary>
    [Name("枚举与字符串转换器")]
    [Tool(XGUICategory.DataConverter, nameof(BaseDataConverter))]
    public class Enum_String_Converter : BaseDataConverter, IDataConverter<Enum, string>, IDataConverter<string, Enum>
    {
        /// <summary>
        /// 枚举字符串类型
        /// </summary>
        [Name("枚举字符串类型")]
        [EnumPopup]
        public EEnumStringType _enumStringType = EEnumStringType.Default;

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
            if (input is Enum e && outputType == typeof(string))
            {
                output = EnumStringCache.Get(e, _enumStringType);
                return true;
            }
            else if (input is string s && outputType.IsEnum)
            {
                try
                {
                    output = EnumValueCache.Get(outputType, s, _enumStringType);
                    if (output != null)
                    {
                        return true;
                    }
                }
                catch
                {
                    
                }
            }
            return base.TryConvertTo(sender, input, outputType, out output);
        }
    }
}
