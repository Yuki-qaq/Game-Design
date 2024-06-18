using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 属性处理器
    /// </summary>
    [LinkType("UnityEditor.PropertyHandler")]
    public class PropertyHandler : LinkType<PropertyHandler>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public PropertyHandler(object obj) : base(obj) { }

        #region hasPropertyDrawer

        /// <summary>
        /// 有属性绘制器 字段信息
        /// </summary>
        public static XPropertyInfo hasPropertyDrawer_FieldInfo { get; } = GetXPropertyInfo(nameof(hasPropertyDrawer));

        /// <summary>
        /// 有属性绘制器
        /// </summary>
        public bool hasPropertyDrawer => (bool)hasPropertyDrawer_FieldInfo.GetValue(obj);

        #endregion

    }
}
