using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// <see cref="SerializedProperty"/>关联类型
    /// </summary>
    [LinkType(typeof(SerializedProperty))]
    public class SerializedProperty_LinkType : LinkType<SerializedProperty_LinkType>
    {
        /// <summary>
        /// 序列化属性
        /// </summary>
        public SerializedProperty serializedProperty => obj as SerializedProperty;

        /// <summary>
        /// 构造
        /// </summary>
        public SerializedProperty_LinkType() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="serializedProperty"></param>
        public SerializedProperty_LinkType(SerializedProperty serializedProperty) : base(serializedProperty) { }

        #region isValid

        /// <summary>
        /// 是有效的 属性信息
        /// </summary>
        public static XPropertyInfo isValid_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(isValid), TypeHelper.InstanceNotPublic);

        /// <summary>
        /// 是有效的
        /// </summary>
        public bool isValid => isValid_PropertyInfo.GetValue<bool>(obj);

        #endregion

        #region gradientValue

        /// <summary>
        /// 渐变色值 属性信息
        /// </summary>
        public static XPropertyInfo gradientValue_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(gradientValue), TypeHelper.InstanceNotPublic);

        /// <summary>
        /// 渐变色值
        /// </summary>
        public Gradient gradientValue
        {
            get => gradientValue_PropertyInfo.GetValue(obj) as Gradient;
            set => gradientValue_PropertyInfo.SetValue(obj, value);
        }

        #endregion

        #region hashCodeForPropertyPathWithoutArrayIndex

        /// <summary>
        /// 没有数组索引的属性路径的哈希代码 属性信息
        /// </summary>
        public static XPropertyInfo hashCodeForPropertyPathWithoutArrayIndex_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(hashCodeForPropertyPathWithoutArrayIndex), TypeHelper.InstanceNotPublic);

        /// <summary>
        /// 没有数组索引的属性路径的哈希代码
        /// </summary>
        public int hashCodeForPropertyPathWithoutArrayIndex
        {
            get
            {
                try
                {
                    return (int)gradientValue_PropertyInfo.GetValue(obj); 
                }
                catch
                {
                    ////Debug.Log("serializedProperty.propertyPath.GetHashCode():" + serializedProperty.propertyPath.GetHashCode());
                    return serializedProperty.propertyPath.GetHashCode();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// <see cref="SerializedProperty"/>扩展：为与常规的扩展去重名，增加下划线间隔扩展类
    /// </summary>
    public static class SerializedProperty_Extension
    {
        private static Dictionary<string, int> hashCodes = new Dictionary<string, int>();

        /// <summary>
        /// 通过缓存获取哈希码用于无数组索引的属性路径
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <returns></returns>
        public static int GetHashCodeForPropertyPathWithoutArrayIndexByCache(this SerializedProperty serializedProperty)
        {
            var path = serializedProperty.propertyPath;
            if (hashCodes.TryGetValue(path, out var hashCode)) return hashCode;
            hashCodes[path] = hashCode = serializedProperty.GetHashCodeForPropertyPathWithoutArrayIndex();
            return hashCode;
        }

        /// <summary>
        /// 获取哈希码用于无数组索引的属性路径
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <returns></returns>
        public static int GetHashCodeForPropertyPathWithoutArrayIndex(this SerializedProperty serializedProperty) => new SerializedProperty_LinkType(serializedProperty).hashCodeForPropertyPathWithoutArrayIndex;

        /// <summary>
        /// 通过缓存获取属性哈希
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static int GetPropertyHashByCache(this SerializedProperty property)
        {
            var obj = property.serializedObject.targetObject;
            if (!obj)
            {
                return 0;
            }
            int num = obj.GetInstanceID() ^ property.GetHashCodeForPropertyPathWithoutArrayIndexByCache();
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                num ^= property.objectReferenceInstanceIDValue;
            }
            return num;
        }

        /// <summary>
        /// 获取属性哈希
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static int GetPropertyHash(this SerializedProperty property)
        {
            var obj = property.serializedObject.targetObject;
            if (!obj)
            {
                return 0;
            }
            int num = obj.GetInstanceID() ^ property.GetHashCodeForPropertyPathWithoutArrayIndex();
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                num ^= property.objectReferenceInstanceIDValue;
            }
            return num;
        }

        /// <summary>
        /// 通过缓存获取字段信息
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfoByCache(this SerializedProperty property) => TypePropertyPathFieldInfoCache.GetCacheValue(property.serializedObject.targetObject.GetType(), property.propertyPath);

        /// <summary>
        /// 获取字段信息
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo(this SerializedProperty property) => ScriptAttributeUtility.GetFieldInfoFromProperty(property, out _);

        /// <summary>
        /// 获取GUI内容
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static GUIContent GetGUIContent(this SerializedProperty property) => TypePropertyPathGUIContentCache.GetCacheValue(property.serializedObject.targetObject.GetType(), property.propertyPath);
    }

}
