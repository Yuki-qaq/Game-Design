using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 可脚本化单例关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScriptableSingleton_T_LinkType<T> : ScriptableObject_LinkType<T> where T : ScriptableSingleton_T_LinkType<T>, new()
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public ScriptableSingleton_T_LinkType(ScriptableObject obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public ScriptableSingleton_T_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        public ScriptableSingleton_T_LinkType() : base() { }

        #region instance

        /// <summary>
        /// 实例 属性信息
        /// </summary>
        public static XPropertyInfo instance_XPropertyInfo { get; } = GetXPropertyInfo(nameof(instance));

        /// <summary>
        /// 实例
        /// </summary>
        public static T instance
        {
            get
            {
                var _this = new T();
                _this.obj = instance_XPropertyInfo?.GetValue(null);
                return _this;
            }
        }

        #endregion
    }
}
