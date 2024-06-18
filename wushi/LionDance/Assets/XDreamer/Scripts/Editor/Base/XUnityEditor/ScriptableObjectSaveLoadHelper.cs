using System.Reflection;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 与UnityEditor.ScriptableObjectSaveLoadHelper{T}泛型类关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TScriptableObject"></typeparam>
    public abstract class ScriptableObjectSaveLoadHelper<T, TScriptableObject> : LinkType<T>
        where T : ScriptableObjectSaveLoadHelper<T, TScriptableObject>
        where TScriptableObject : ScriptableObject_LinkType<TScriptableObject>, new()
    {
        /// <summary>
        /// 构造
        /// </summary>
        public ScriptableObjectSaveLoadHelper() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public ScriptableObjectSaveLoadHelper(object obj) : base(obj) { }

        #region fileExtensionWithoutDot

        /// <summary>
        /// 不带点的文件扩展名 属性信息
        /// </summary>
        public static XPropertyInfo fileExtensionWithoutDot_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(fileExtensionWithoutDot), BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

        /// <summary>
        /// 不带点的文件扩展名
        /// </summary>
        public string fileExtensionWithoutDot
        {
            get => fileExtensionWithoutDot_PropertyInfo.GetValue(obj) as string;
            set => fileExtensionWithoutDot_PropertyInfo.SetValue(obj, value);
        }

        #endregion
    }
}
