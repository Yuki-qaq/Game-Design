using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.Extension.Base.Interactions.Tools
{
    /// <summary>
    /// 基础交互属性数据
    /// </summary>
    public class BaseInteractPropertyData: ITagKeyValue
    {
        /// <summary>
        /// 属性关键字
        /// </summary>
        public string key => _key.TryGetValue(out var result) ? result : "";

        /// <summary>
        /// 属性值
        /// </summary>
        public string value
        {
            get => _value.TryGetValue(out var result) ? result : "";
            set => _value._value = value;
        }

        /// <summary>
        /// 属性键
        /// </summary>
        [Name("属性键")]
        public StringPropertyValue _key = new StringPropertyValue();

        /// <summary>
        /// 属性值
        /// </summary>
        [Name("属性值")]
        public StringPropertyValue _value = new StringPropertyValue();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseInteractPropertyData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseInteractPropertyData(string key) => _key = new StringPropertyValue(key);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public BaseInteractPropertyData(string key, string value) : this(key) => _value = new StringPropertyValue(value);
    }

    #region 属性关键字特性与缓存

    /// <summary>
    /// 属性关键字提供者
    /// </summary>
    public interface IPropertyKeyProvider
    {
        /// <summary>
        /// 属性关键字信息列表
        /// </summary>
        List<PropertyKeyInfo> propertyKeyInfos { get; }
    }

    /// <summary>
    /// 属性关键字信息
    /// </summary>
    public class PropertyKeyInfo
    {
        /// <summary>
        /// 类名称:属性关键字所在类名称
        /// </summary>
        public string className { get; }

        /// <summary>
        /// 关键字名称
        /// </summary>
        public string keyName { get; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string key { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PropertyKeyInfo(string className, string keyName, string key)
        {
            this.className = className;
            this.keyName = keyName;
            this.key = key;
        }
    }

    /// <summary>
    /// 属性关键字缓存
    /// </summary>
    public class PropertyKeyCache : InstanceClass<PropertyKeyCache>
    {
        private static string[] _propertyKeys = null;

        /// <summary>
        /// 属性关键字数组
        /// </summary>
        public static string[] propertyKeys
        {
            get
            {
                if (_propertyKeys == null)
                {
                    try
                    {
                        var list = new List<(int, string)>();
                        foreach (var type in TypeHelper.FindTypeInAppWithClass(typeof(object), true, true))
                        {
                            foreach (var fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.Public))
                            {
                                var att = AttributeHelper.GetAttribute<PropertyKeyAttribute>(fieldInfo);
                                if (att != null && Converter.instance.TryConvertTo<string>(fieldInfo.GetValue(null), out var key))
                                {
                                    list.Add((att.index, key));
                                }
                            }
                        }
                        // 优先按序号排，然后按名称排
                        list.Sort((x, y) => (x.Item1 == y.Item1) ? (x.Item2.CompareTo(y.Item2)) : (x.Item1 < y.Item1 ? -1 : 1));
                        _propertyKeys = list.Cast(item => item.Item2).ToArray();
                    }
                    catch
                    {
                        _propertyKeys = new string[0];
                    }
                }
                return _propertyKeys;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PropertyKeyCache()
        {
            XDreamerEvents.onAnyAssetsChanged -= MarkDirty;
            XDreamerEvents.onAnyAssetsChanged += MarkDirty;
        }

        private static void MarkDirty()
        {
            instance.keys = null;
        }

        private List<string> keyList = new List<string>();

        private string[] keys = null;

        /// <summary>
        /// 获取缓存关键字
        /// </summary>
        /// <returns></returns>
        public string[] GetKeys()
        {
            if (keys == null)
            {
                Refresh();
            }
            return keys;
        }

        /// <summary>
        /// 刷新关键字缓存：全场景查找类型绑定器的别名对象
        /// </summary>
        public void Refresh()
        {
            keyList.Clear();

            var providers = FindPropertyKeyProviders();

            foreach (var p in providers)
            {
                foreach (var info in p.propertyKeyInfos)
                {
                    var keyPath = string.Format("{0}/{1}/{2}", info.className, info.keyName, info.key);
                    keyList.AddWithDistinct(keyPath);
                }
            }
            keyList.Sort();
            keys = keyList.ToArray();
        }

        private IEnumerable<IPropertyKeyProvider> FindPropertyKeyProviders() => ComponentCache.GetComponents<IPropertyKeyProvider>(true);
    }

    /// <summary>
    /// 属性关键字特性：主要修饰字符串常量类型的关键字
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class PropertyKeyAttribute : IndexAttribute { }

    /// <summary>
    /// 属性关键字使用者特性：主要修饰字符串类型字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class PropertyKeyUserAttribute : PropertyAttribute { }

    #endregion
}
