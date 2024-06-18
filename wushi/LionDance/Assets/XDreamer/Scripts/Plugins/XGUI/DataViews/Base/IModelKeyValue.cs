using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Components;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.DataViews.Base
{
    /// <summary>
    /// 模型键值
    /// </summary>
    public interface IModelKeyValue 
    {
        /// <summary>
        /// 键列表
        /// </summary>
        IEnumerable<string> keys { get; }

        /// <summary>
        /// 尝试获取模型键值类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool TryGetModelKeyValueType(string key, out Type type);

        /// <summary>
        /// 尝试获取模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetModelKeyValue(string key, out object value);

        /// <summary>
        /// 尝试设置模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TrySetModelKeyValue(string key, object value);
    }

    /// <summary>
    /// 模型键值宿主
    /// </summary>
    public interface IModelKeyValueHost
    {
        /// <summary>
        /// 模型键值
        /// </summary>
        IModelKeyValue modelKeyValue { get; }
    }

    /// <summary>
    /// 模型键值宿主修改器
    /// </summary>
    public interface IModelKeyValueHostModifier
    {
        /// <summary>
        /// 模型
        /// </summary>
        object model { get; }

        /// <summary>
        /// 键
        /// </summary>
        string key { get; }

        /// <summary>
        /// 设置模型键
        /// </summary>
        /// <param name="model"></param>
        /// <param name="key"></param>
        void SetModelKey(object model, string key);
    }

    /// <summary>
    /// 模型键值助手
    /// </summary>
    public static class ModelKeyValueHelper
    {
        /// <summary>
        /// 获取模型键值类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Type GetModelKeyValueType(this object obj, string key)
        {
            if (obj == null) return null;

            if (obj is IModelKeyValue modelKeyValue && modelKeyValue.TryGetModelKeyValueType(key, out var type)) return type;

            var memberInfo = MemberInfoCache.GetMemberInfo(obj.GetType(), key);
            if (memberInfo == null) return null;

            if (memberInfo is FieldInfo fieldInfo) return fieldInfo.FieldType;
            if (memberInfo is PropertyInfo propertyInfo) return propertyInfo.PropertyType;
            if (memberInfo is MethodInfo methodInfo) return methodInfo.ReturnType;

            return default;
        }

        /// <summary>
        /// 获取模型键值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetModelKeyValue(this object obj, string key)
        {
            if (obj == null) return null;

            if (obj is IModelKeyValue modelKeyValue && modelKeyValue.TryGetModelKeyValue(key, out var value)) return value;

            var memberInfo = MemberInfoCache.GetMemberInfo(obj.GetType(), key);
            if (memberInfo == null) return null;

            if (memberInfo is FieldInfo fieldInfo) return fieldInfo.GetValue(obj);
            if (memberInfo is PropertyInfo propertyInfo) return propertyInfo.GetValue(obj);
            if (memberInfo is MethodInfo methodInfo) return methodInfo.Invoke(obj, null);

            return default;
        }

        /// <summary>
        /// 设置模型键值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetModelKeyValue(this object obj, string key, object value)
        {
            if (obj == null) return false;

            if (obj is IModelKeyValue modelKeyValue && modelKeyValue.TrySetModelKeyValue(key, value)) return true;

            var memberInfo = MemberInfoCache.GetMemberInfo(obj.GetType(), key);
            if (memberInfo == null) return false;

            if (memberInfo is FieldInfo fieldInfo)
            {
                fieldInfo.SetValue(obj, value);
                return true;
            }
            if (memberInfo is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(obj, value);
                return true;
            }
            if (memberInfo is MethodInfo methodInfo) return true;

            return default;
        }
    }

    /// <summary>
    /// 模型键特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ModelKeyAttribute : PropertyAttribute
    {
        /// <summary>
        /// 按钮宽度
        /// </summary>
        public float buttonWidth = 80;
    }

    /// <summary>
    /// 模型键属性值
    /// </summary>
    [Name("模型键属性值")]
    [Serializable]
    [PropertyType(typeof(string))]
    public class ModelKeyPropertyValue: BasePropertyValue<string>
    {
        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        [ModelKey]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
        public string _value = "";

        /// <summary>
        /// 属性值
        /// </summary>
        public override string propertyValue { get => _value; set => _value = value; }

        /// <summary>
        /// 构造
        /// </summary>
        public ModelKeyPropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public ModelKeyPropertyValue(string value) { this._value = value; }
    }
}

