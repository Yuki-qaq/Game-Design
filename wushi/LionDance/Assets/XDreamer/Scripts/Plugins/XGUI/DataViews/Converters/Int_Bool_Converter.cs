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
    /// 整型与布尔转换器
    /// </summary>
    [Name("整型与布尔转换器")]
    [Tool(XGUICategory.DataConverter, nameof(BaseDataConverter))]
    public class Int_Bool_Converter : BaseDataConverter, IDataConverter<int, bool>, IDataConverter<bool, int>
    {
        /// <summary>
        /// 转换规则
        /// </summary>
        public enum EConvertRule
        {
            /// <summary>
            /// 值
            /// </summary>
            Value,

            /// <summary>
            /// 标志
            /// </summary>
            Flags,
        }

        /// <summary>
        /// 转换规则
        /// </summary>
        public EConvertRule _convertRule = EConvertRule.Flags;

        /// <summary>
        /// 映射数据0
        /// </summary>
        [Serializable]
        public class MapData0 : MapData<int, bool> { }

        /// <summary>
        /// 映射数据1
        /// </summary>
        [Serializable]
        public class MapData1 : MapData<bool, int> { }

        /// <summary>
        /// 整型到布尔映射列表
        /// </summary>
        [Name("整型到布尔映射列表")]
        public List<MapData0> int_String_Map = new List<MapData0>();

        /// <summary>
        /// 布尔到整型映射列表
        /// </summary>
        [Name("布尔到整型映射列表")]
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
            if (input is int i && outputType == typeof(bool))
            {
                var data = int_String_Map.FirstOrDefault(d =>
                {
                    switch (_convertRule)
                    {
                        case EConvertRule.Value:
                            {
                                return i == d.inputValue;
                            }
                        case EConvertRule.Flags:
                            {
                                return (i & d.inputValue) == d.inputValue;
                            }
                    }
                    return false;
                });
                if (data != null)
                {
                    output = data.outputValue;
                    return true;
                }
            }
            else if (input is bool s && outputType == typeof(int))
            {
                var data = string_Int_Map.FirstOrDefault(d => d.inputValue == s);
                if (data != null)
                {
                    output =  data.outputValue;
                    return true;
                }
            }
            return base.TryConvertTo(sender, input, outputType, out output);
        }
    }
}
