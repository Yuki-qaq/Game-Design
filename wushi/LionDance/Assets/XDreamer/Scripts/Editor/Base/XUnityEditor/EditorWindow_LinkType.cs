using System;
using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 编辑器窗口关联类型
    /// </summary>
    public interface IEditorWindow_LinkType : IScriptableObject_LinkType
    {
        /// <summary>
        /// 编辑器窗口
        /// </summary>
        EditorWindow editorWindow { get; }
        
        /// <summary>
        /// 父级
        /// </summary>
        HostView m_Parent { get; }
    }

    /// <summary>
    /// 编辑器窗口关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditorWindow_LinkType<T> : ScriptableObject_LinkType<T>, IEditorWindow_LinkType
        where T : EditorWindow_LinkType<T>
    {
        /// <summary>
        /// 编辑器窗口
        /// </summary>
        public EditorWindow editorWindow => obj as EditorWindow;

        /// <summary>
        /// 构造
        /// </summary>
        protected EditorWindow_LinkType() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public EditorWindow_LinkType(EditorWindow obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public EditorWindow_LinkType(object obj) : base(obj) { }

        #region m_Parent

        /// <summary>
        /// 父级 字段信息
        /// </summary>
        public static XFieldInfo m_Parent_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_Parent), TypeHelper.DefaultInstance);

        /// <summary>
        /// 父级
        /// </summary>
        public HostView m_Parent
        {
            get
            {
                return new HostView(m_Parent_FieldInfo?.GetValue(obj));
            }
        }

        #endregion

        #region docked

        /// <summary>
        /// 对接 属性信息
        /// </summary>
        public static XPropertyInfo docked_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(docked), TypeHelper.InstanceNotPublic);

        /// <summary>
        /// 对接
        /// </summary>
        public bool docked
        {
            get
            {
                return (bool)docked_PropertyInfo.GetValue(obj);
            }
        }

        #endregion

        #region hasFocus

        /// <summary>
        /// 有焦点 属性信息
        /// </summary>
        public static XPropertyInfo hasFocus_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(hasFocus), TypeHelper.InstanceNotPublic);

        /// <summary>
        /// 有焦点
        /// </summary>
        public bool hasFocus
        {
            get
            {
                return (bool)hasFocus_PropertyInfo.GetValue(obj);
            }
        }

        #endregion

        #region RepaintImmediately

        /// <summary>
        /// 立即重绘 方法信息
        /// </summary>
        public static XMethodInfo RepaintImmediately_MethodInfo { get; } = GetXMethodInfo(nameof(RepaintImmediately), TypeHelper.InstanceNotPublicHierarchy);

        /// <summary>
        /// 立即重绘
        /// </summary>
        public void RepaintImmediately()
        {
            RepaintImmediately_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion
    }

    /// <summary>
    /// 编辑器窗口关联信息
    /// </summary>
    [LinkType(typeof(EditorWindow))]
    public class EditorWindow_LinkType : EditorWindow_LinkType<EditorWindow_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        protected EditorWindow_LinkType() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public EditorWindow_LinkType(EditorWindow obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public EditorWindow_LinkType(object obj) : base(obj) { }
    }
}
