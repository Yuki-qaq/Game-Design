using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 脚本特性工具
    /// </summary>
    [LinkType("UnityEditor.ScriptAttributeUtility")]
    public class ScriptAttributeUtility : LinkType<ScriptAttributeUtility>
    {
        #region s_SharedNullHandler

        /// <summary>
        /// 共享的空处理器 字段信息
        /// </summary>
        public static XFieldInfo s_SharedNullHandler_FieldInfo { get; } = GetXFieldInfo(nameof(s_SharedNullHandler));

        private static PropertyHandler s_SharedNullHandler
        {
            get
            {
                return new PropertyHandler(s_SharedNullHandler_FieldInfo.GetValue(null));
            }
        }

        #endregion

        #region s_NextHandler

        /// <summary>
        /// 下一个处理器 字段信息
        /// </summary>
        public static XFieldInfo s_NextHandler_FieldInfo { get; } = GetXFieldInfo(nameof(s_NextHandler));

        /// <summary>
        /// 下一个处理器
        /// </summary>
        private static PropertyHandler s_NextHandler
        {
            get
            {
                return new PropertyHandler(s_NextHandler_FieldInfo.GetValue(null));
            }
        }

        #endregion

        #region GetHandler

        /// <summary>
        /// 获取处理器 方法信息
        /// </summary>
        public static XMethodInfo GetHandler_MethodInfo { get; } = GetXMethodInfo(nameof(GetHandler));

        /// <summary>
        /// 获取处理器
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static PropertyHandler GetHandler(SerializedProperty property)
        {
            return new PropertyHandler(GetHandler_MethodInfo.Invoke(null, new object[] { property }));
        }

        #endregion

        /// <summary>
        /// 有自定义属性绘制器
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool HasCustomPropetyDrawer(SerializedProperty property)
        {
            var handler = GetHandler(property);
            return handler.hasPropertyDrawer && GetHandler(property).obj != s_SharedNullHandler.obj;
        }

        #region GetFieldInfoFromProperty

        /// <summary>
        /// 从属性获取字段信息 方法信息
        /// </summary>
        public static XMethodInfo GetFieldInfoFromProperty_MethodInfo { get; } = GetXMethodInfo(nameof(GetFieldInfoFromProperty));

        /// <summary>
        /// 从属性获取字段信息
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type)
        {
            var ps = new object[] { property, default(Type) };
            var fieldInfo = GetFieldInfoFromProperty_MethodInfo.InvokeStatic(ps) as FieldInfo;
            type = ps[1] as Type;
            return fieldInfo;
        }

        #endregion

        #region GetFieldInfoFromPropertyPath

        /// <summary>
        /// 从属性路径获取字段信息 方法信息
        /// </summary>
        public static XMethodInfo GetFieldInfoFromPropertyPath_MethodInfo { get; } = GetXMethodInfo(nameof(GetFieldInfoFromPropertyPath));

        /// <summary>
        /// 从属性路径获取字段信息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfoFromPropertyPath(Type host, string path, out Type type)
        {
            var ps = new object[] { host, path, default(Type) };
            var fieldInfo = GetFieldInfoFromPropertyPath_MethodInfo.InvokeStatic(ps) as FieldInfo;
            type = ps[2] as Type;
            return fieldInfo;
        }

        #endregion

        /// <summary>
        /// 获取类型的绘制类型 方法信息
        /// </summary>
        public static XMethodInfo GetDrawerTypeForType_MethodInfo { get; } = GetXMethodInfo(nameof(GetDrawerTypeForType));

        /// <summary>
        /// 获取类型的绘制类型
        /// </summary>
        /// <param name="type">类型：<see cref="CustomPropertyDrawer"/>构造函数的传入的参数类型</param>
        /// <returns>被<see cref="CustomPropertyDrawer"/>修饰且继承自<see cref="GUIDrawer"/>的类型</returns>
        public static Type GetDrawerTypeForType(Type type) => GetDrawerTypeForType_MethodInfo.InvokeStatic<Type>(new object[] { type });
    }
}
