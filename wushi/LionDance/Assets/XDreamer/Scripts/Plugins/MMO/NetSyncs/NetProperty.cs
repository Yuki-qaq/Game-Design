using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络属性
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [DisallowMultipleComponent]
    [Name("网络属性")]
    [Tool(MMOHelperExtension.Title, nameof(NetIdentity), rootType = typeof(MMOManager))]
    public class NetProperty : NetMB, IModelKeyValue
    {
        static NetProperty()
        {
            Converter.instance.RegisterClass<List<Property>>();
        }

        /// <summary>
        /// 属性列表:属性列表中的属性信息在发生数据修改时会进行网络同步
        /// </summary>
        [SyncVar]
        [Name("属性列表")]
        [Tip("属性列表中的属性信息在发生数据修改时会进行网络同步", "The attribute information in the attribute list will be synchronized when data is modified")]
        public List<Property> _propertys = new List<Property>();

        /// <summary>
        /// 键列表
        /// </summary>
        public IEnumerable<string> keys => throw new NotImplementedException();

        /// <summary>
        /// 当有新属性值时回调
        /// </summary>
        public static event Action<NetProperty, Property> onNewProperty;

        /// <summary>
        /// 当属性值变化时回调
        /// </summary>
        public static event Action<NetProperty, Property, string> onValueChanged;

        /// <summary>
        /// 设置属性；无则添加有则覆盖
        /// </summary>
        /// <param name="name">不可为空字符串或null</param>
        /// <param name="value"></param>
        /// <param name="mustSet">强制设置；默认情况下如果新值与旧值相同，不执行设置；当本参数为True时，不论新值与旧值是否相同都会触发修改机制执行网络同步</param>
        /// <returns></returns>
        public Property SetProperty(string name, string value, bool mustSet = false)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var property = GetProperty(name);
            if (property == null)
            {
                property = new Property() { _name = name };
                this.XModifyProperty(() =>
                {
                    _propertys.Add(property);
                });
                MarkDirty();

                onNewProperty?.Invoke(this, property);
            }
            if (property._value != value || mustSet)
            {
                var oldValue = property._value;
                this.XModifyProperty(() =>
                {
                    property._value = value;
                });
                MarkDirty();

                onValueChanged?.Invoke(this, property, oldValue);
            }            
            return property;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Property GetProperty(string name) => _propertys.FirstOrDefault(p => p._name == name);

        /// <summary>
        /// 定时检测修改时回调；可用于检测检测同步变量（即被SyncVarAttribute修饰的变量）或数据是否反生变化；
        /// </summary>
        /// <returns></returns>
        protected override bool OnTimedCheckChange()
        {
            //return base.OnTimedCheckChange();
            return false;
        }

        #region 模型键值

        /// <summary>
        /// 尝试获取模型键值类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool TryGetModelKeyValueType(string key, out Type type)
        {
            type = typeof(string);
            return true;
        }

        /// <summary>
        /// 尝试获取模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetModelKeyValue(string key, out object value)
        {
            if(GetProperty(key) is Property property)
            {
                value = property._value;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试设置模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySetModelKeyValue(string key, object value) => SetProperty(key, value as string ?? "") != null;

        #endregion
    }

    /// <summary>
    /// 属性
    /// </summary>
    [Serializable]
    [Name("属性")]
    public class Property
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Name("名称")]
        public string _name;

        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        public string _value;
    }
}
