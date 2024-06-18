using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.LitJson;

namespace XCSJ.PluginXGUI.Windows.Weathers
{
    /// <summary>
    /// 天气数据
    /// </summary>
    [Serializable]
    [Import]
    [Name("天气数据")]
    public class WeatherData
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string message;

        /// <summary>
        /// 状态
        /// </summary>
        public int status;

        /// <summary>
        /// 日期
        /// </summary>
        public string date;

        /// <summary>
        /// 时间
        /// </summary>
        public string time;

        /// <summary>
        /// 城市信息
        /// </summary>
        public CityInfo cityInfo;

        /// <summary>
        /// 数据
        /// </summary>
        public Data data;
    }

    /// <summary>
    /// 城市信息
    /// </summary>
    [Serializable]
    [Import]
    [Name("城市信息")]
    public class CityInfo
    {
        /// <summary>
        /// 城市
        /// </summary>
        public string city;

        /// <summary>
        /// 城市编码
        /// </summary>
        public string citykey;

        /// <summary>
        /// 父级
        /// </summary>
        public string parent;

        /// <summary>
        /// 更新时间
        /// </summary>
        public string updateTime;
    }

    /// <summary>
    /// 数据
    /// </summary>
    [Serializable]
    [Import]
    [Name("数据")]
    public class Data
    {
        /// <summary>
        /// 湿度
        /// </summary>
        public string shidu;

        /// <summary>
        /// PM2.5
        /// </summary>
        public float pm25;

        /// <summary>
        /// PM10
        /// </summary>
        public float pm10;

        /// <summary>
        /// 质量
        /// </summary>
        public string quality;

        /// <summary>
        /// 温度
        /// </summary>
        public string wendu;

        /// <summary>
        /// 感冒指数
        /// </summary>
        public string ganmao;

        /// <summary>
        /// 天气预报
        /// </summary>
        public List<Forecast> forecast;

        /// <summary>
        /// 昨天
        /// </summary>
        public Forecast yesterday;
    }

    /// <summary>
    /// 预报
    /// </summary>
    [Serializable]
    [Import]
    [Name("预报")]
    public class Forecast
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string date;

        /// <summary>
        /// 高温
        /// </summary>
        public string high;

        /// <summary>
        /// 低温
        /// </summary>
        public string low;

        /// <summary>
        /// 年月日
        /// </summary>
        public string ymd;

        /// <summary>
        /// 星期
        /// </summary>
        public string week;

        /// <summary>
        /// 日出
        /// </summary>
        public string sunrise;

        /// <summary>
        /// 日落
        /// </summary>
        public string sunset;

        /// <summary>
        /// 空气质量指数
        /// </summary>
        public int aqi;

        /// <summary>
        /// 风向
        /// </summary>
        public string fx;

        /// <summary>
        /// 风力
        /// </summary>
        public string fl;

        /// <summary>
        /// 天气类型
        /// </summary>
        public string type;

        /// <summary>
        /// 提示
        /// </summary>
        public string notice;
    }
}
