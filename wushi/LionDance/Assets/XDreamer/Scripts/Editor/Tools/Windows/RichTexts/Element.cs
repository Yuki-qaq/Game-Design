using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.RichTexts
{
    /// <summary>
    /// 元素泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [Import]
    public class Element<T> : IOnAfterDeserialize
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        public string guid = Guid.NewGuid().ToString();

        /// <summary>
        /// 值
        /// </summary>
        public T value = default;

        /// <summary>
        /// 构造
        /// </summary>
        public Element() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public Element(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Converter.instance.TryConvertTo(value, out string text) ? text : "";
        }

        /// <summary>
        /// 当序列化之后
        /// </summary>
        /// <param name="serializeContext"></param>
        public virtual void OnAfterDeserialize(ISerializeContext serializeContext) { }

        #region 字符串处理

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Element<T> Parse(string text)
        {
            if (Converter.instance.TryConvertTo(text, out T value))
            {
                return new Element<T>(value);
            }
            return null;
        }

        /// <summary>
        /// 尝试分析
        /// </summary>
        /// <param name="text"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool TryParse(string text, out Element<T> element)
        {
            if (Converter.instance.TryConvertTo(text, out T value))
            {
                element = new Element<T>(value);
                return true;
            }
            element = null;
            return false;
        }

        /// <summary>
        /// 隐式转为元素
        /// </summary>
        /// <param name="text"></param>
        public static implicit operator Element<T>(string text) => Parse(text);

        #endregion
    }

    /// <summary>
    /// 元素
    /// </summary>
    [Serializable]
    [Name("元素")]
    public class Element : Element<string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Element() { value = ""; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public Element(string value) : base(value) { }

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public new static Element Parse(string text)
        {
            if (Converter.instance.TryConvertTo(text, out string value))
            {
                return new Element(value);
            }
            return null;
        }

        /// <summary>
        /// 尝试分析
        /// </summary>
        /// <param name="text"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool TryParse(string text, out Element element)
        {
            if (Converter.instance.TryConvertTo(text, out string value))
            {
                element = new Element(value);
                return true;
            }
            element = null;
            return false;
        }

        /// <summary>
        /// 隐式转换为元素
        /// </summary>
        /// <param name="text"></param>

        public static implicit operator Element(string text)
        {
            return Parse(text);
        }
    }
}
