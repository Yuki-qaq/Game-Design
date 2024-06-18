using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.Converters
{
    /// <summary>
    /// 字符串与颜色转换器
    /// </summary>
    [Name("字符串与颜色转换器")]
    [Tool(XGUICategory.DataConverter, nameof(BaseDataConverter))]
    public class String_Color_Converter : BaseDataConverter, IDataConverter<string, Color>, IDataConverter<Color, string>
    {
        #region 字符串到颜色

        /// <summary>
        /// 字符串到颜色规则
        /// </summary>
        public enum EStringToColorRule
        {
            /// <summary>
            /// 默认
            /// </summary>
            [Name("默认")]
            Default,

            /// <summary>
            /// 映射
            /// </summary>
            [Name("映射")]
            Map,
        }

        /// <summary>
        /// 字符串到颜色规则
        /// </summary>
        [Name("字符串到颜色规则")]
        [EnumPopup]
        public EStringToColorRule _stringToColorRule = EStringToColorRule.Default;

        /// <summary>
        /// 映射数据0
        /// </summary>
        [Serializable]
        public class MapData0 : MapData<string, Color> { }

        /// <summary>
        /// 字符串到颜色映射列表
        /// </summary>
        [Name("字符串到颜色映射列表")]
        public List<MapData0> _string_Color_Map = new List<MapData0>();

        #endregion

        #region 颜色到字符串

        /// <summary>
        /// 颜色到字符串规则
        /// </summary>
        public enum EColorToStringRule
        {
            /// <summary>
            /// 默认
            /// </summary>
            [Name("默认")]
            Default,

            /// <summary>
            /// 映射
            /// </summary>
            [Name("映射")]
            Map,
        }

        /// <summary>
        /// 颜色到字符串规则
        /// </summary>
        [Name("颜色到字符串规则")]
        [EnumPopup]
        public EColorToStringRule _colorToStringRule = EColorToStringRule.Default;

        /// <summary>
        /// 映射数据1
        /// </summary>
        [Serializable]
        public class MapData1 : MapData<Color, string> { }

        /// <summary>
        /// 颜色到字符串映射列表
        /// </summary>
        [Name("颜色到字符串映射列表")]
        public List<MapData1> _color_string_Map = new List<MapData1>();

        #endregion

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
            if (input is string str && outputType == typeof(Color))
            {
                switch (_stringToColorRule)
                {
                    case EStringToColorRule.Map:
                        {
                            var data = _string_Color_Map.FirstOrDefault(m => m.inputValue == str);
                            if (data != null)
                            {
                                output = data.outputValue;
                                return true;
                            }
                            break;
                        }
                }
            }
            else if (input is Color color && outputType == typeof(string))
            {
                switch (_colorToStringRule)
                {
                    case EColorToStringRule.Map:
                        {
                            var data = _color_string_Map.FirstOrDefault(d => d.inputValue == color);
                            if (data != null)
                            {
                                output = data.outputValue;
                                return true;
                            }
                            break;
                        }
                }

            }
            return base.TryConvertTo(sender, input, outputType, out output);
        }

        private void Add(string str, Color color)
        {
            _string_Color_Map.Add(new MapData0() { inputValue = str, outputValue = color});
            _color_string_Map.Add(new MapData1() { inputValue = color, outputValue = str });
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            foreach(var pi in typeof(Color).GetProperties(TypeHelper.StaticPublic))
            {
                if(pi.PropertyType== typeof(Color))
                {
                    Add(pi.Name, (Color)pi.GetValue(null));
                }
            }
        }
    }
}
