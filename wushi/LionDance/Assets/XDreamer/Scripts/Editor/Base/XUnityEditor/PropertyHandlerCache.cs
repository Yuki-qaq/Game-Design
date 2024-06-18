using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 属性处理器缓存
    /// </summary>
    [LinkType("UnityEditor.PropertyHandlerCache")]
    public class PropertyHandlerCache : LinkType<PropertyHandlerCache>
    {
        #region GetPropertyHash

        /// <summary>
        /// 获取属性哈希 方法信息
        /// </summary>
        public static XMethodInfo GetPropertyHash_MethodInfo { get; } = GetXMethodInfo(nameof(GetPropertyHash));

        /// <summary>
        /// 获取属性哈希
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static int GetPropertyHash(SerializedProperty property)
        {
            return (int)GetPropertyHash_MethodInfo.Invoke(null, new object[] { property });
        }

        #endregion
    }
}
