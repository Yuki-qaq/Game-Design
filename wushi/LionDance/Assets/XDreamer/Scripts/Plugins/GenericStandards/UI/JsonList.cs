using System;
using System.Collections.Generic;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Windows.ListViews;
using XCSJ.Scripts;

namespace XCSJ.PluginGenericStandards.UI
{
    /// <summary>
    /// JSON列表
    /// </summary>
    [Name("JSON列表")]
    [RequireManager(typeof(GenericStandardScriptManager))]
    [Owner(typeof(GenericStandardScriptManager))]
    public class JsonList : ListViewModelProvider
    {
        /// <summary>
        /// JSON文本
        /// </summary>
        [Name("JSON文本")]
        public StringPropertyValue_TextArea _jsonText = new StringPropertyValue_TextArea();

        /// <summary>
        /// 预制模型
        /// </summary>
        protected override IEnumerable<ListViewItemModel> prefabModels
        {
            get
            {
                var list = new List<ListViewItemModel>();
                var jsonData = JsonHelper.ToJsonData(_jsonText.GetValue());
                if (jsonData == null) return list;
                if (jsonData.IsArray)
                {
                    foreach(var data in jsonData.arrayValue)
                    {
                        list.Add(new JsonDataModel(data));
                    }
                }
                else
                {
                    list.Add(new JsonDataModel(jsonData));
                }
                return list;
            }
        }

        /// <summary>
        /// 获取JSON数据类型
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static Type GetJsonDataType(JsonData jsonData)
        {
            if (jsonData == null) return default;
            switch (jsonData.GetJsonType())
            {
                case JsonType.None:
                case JsonType.Object:
                case JsonType.Array:
                case JsonType.String: return typeof(string);
                case JsonType.Int: return typeof(int);
                case JsonType.Long: return typeof(long);
                case JsonType.Double: return typeof(double);
                case JsonType.Boolean: return typeof(bool);
            }
            return default;
        } 
    }

    /// <summary>
    /// JSON数据模型
    /// </summary>
    public class JsonDataModel : ListViewItemModel
    {
        /// <summary>
        /// JSON数据
        /// </summary>
        public JsonData jsonData { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="jsonData"></param>
        public JsonDataModel(JsonData jsonData) : base(jsonData)
        {
            this.jsonData = jsonData;
        }

        /// <summary>
        /// 键列表
        /// </summary>
        public override IEnumerable<string> keys => jsonData.IsObject ? jsonData.objectValue.Keys : (IEnumerable<string>)Empty<string>.Array;

        /// <summary>
        /// 尝试获取模型键值类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool TryGetModelKeyValueType(string key, out Type type)
        {
            if (jsonData.IsObject)
            {
                if(jsonData.objectValue.TryGetValue(key, out var data))
                {
                    type = typeof(string);
                    return true;
                    //type = JsonList.GetJsonDataType(data);
                    //return type != null;
                }
                else
                {
                    type = default;
                    return false;
                }
            }
            else
            {
                type = typeof(string);
                return true;
                //type = JsonList.GetJsonDataType(jsonData);
                //return type != null;
            }
        }

        /// <summary>
        /// 尝试获取模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetModelKeyValue(string key, out object value)
        {
            if (jsonData.IsObject)
            {
                if (jsonData.objectValue.TryGetValue(key, out var data))
                {
                    value = data.ToString();
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
            else
            {
                value = jsonData.ToString();
                return true;
            }
        }

        /// <summary>
        /// 尝试设置取模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetModelKeyValue(string key, object value)
        {
            if (jsonData.IsObject)
            {
                jsonData[key] = new JsonData(value?.ToScriptParamString());
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
