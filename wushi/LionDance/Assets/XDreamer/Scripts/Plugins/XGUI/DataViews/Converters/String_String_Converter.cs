using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.Converters
{
    /// <summary>
    /// 字符串与字符串转换器
    /// </summary>
    [Name("字符串与字符串转换器")]
    [Tool(XGUICategory.DataConverter, nameof(BaseDataConverter))]
    public class String_String_Converter : BaseDataConverter, IDataConverter<string, string>
    {
        /// <summary>
        /// 映射数据
        /// </summary>
        [Serializable]
        public class MapData0 : MapData<string, string> { }

        /// <summary>
        /// 字符串到字符串映射列表
        /// </summary>
        [Name("字符串到字符串映射列表")]
        public List<MapData0> string_String_Map = new List<MapData0>();

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
            if (input is string s && outputType == typeof(string))
            {
                var data = string_String_Map.FirstOrDefault(d => d.inputValue == s);
                if (data != null)
                {
                    output = data.outputValue;
                    return true;
                }
            }
            return base.TryConvertTo(sender, input, outputType, out output);
        }
    }
}
