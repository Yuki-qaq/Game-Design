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
    /// 整型与字符串转换器
    /// </summary>
    [Name("整型与字符串转换器")]
    [Tool(XGUICategory.DataConverter, nameof(BaseDataConverter))]
    public class Int_String_Converter : BaseDataConverter, IDataConverter<int, string>, IDataConverter<string, int>
    {
        /// <summary>
        /// 映射数据0
        /// </summary>
        [Serializable]
        public class MapData0 : MapData<int, string> { }

        /// <summary>
        /// 映射数据1
        /// </summary>
        [Serializable]
        public class MapData1 : MapData<string, int> { }

        /// <summary>
        /// 整型到字符串映射列表
        /// </summary>
        [Name("整型到字符串映射列表")]
        public List<MapData0> int_String_Map = new List<MapData0>();

        /// <summary>
        /// 字符串到整型映射列表
        /// </summary>
        [Name("字符串到整型映射列表")]
        public List<MapData1> string_Int_Map = new List<MapData1>();

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
            if (input is int i && outputType == typeof(string))
            {
                var data = int_String_Map.FirstOrDefault(d=>d.inputValue == i);
                if (data != null)
                {
                    output = data.outputValue;
                    return true;    
                }
            }
            else if(input is string s && outputType == typeof(int))
            {
                var data = string_Int_Map.FirstOrDefault(d => d.inputValue == s);
                if (data != null)
                {
                    output = data.outputValue;
                    return true;
                }
            }
            return base.TryConvertTo(sender, input, outputType, out output);
        }
    }

    /// <summary>
    /// 映射数据
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    [Name("映射数据")]
    [Serializable]
    public class MapData<TInput,TOutput>
    {
        /// <summary>
        /// 输入值
        /// </summary>
        [Name("输入值")]
        public TInput inputValue;

        /// <summary>
        /// 输出值
        /// </summary>
        [Name("输出值")]
        public TOutput outputValue;
    }
}
