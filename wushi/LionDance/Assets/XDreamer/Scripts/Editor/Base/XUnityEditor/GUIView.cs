using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// GUI视图
    /// </summary>
    public interface IGUIView: IView
    {
        /// <summary>
        /// 重绘
        /// </summary>
        void Repaint();
    }

    /// <summary>
    /// GUI视图
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GUIView<T> : View<T>, IGUIView
        where T : GUIView<T>, new()
    {
        /// <summary>
        /// 构造
        /// </summary>
        public GUIView() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public GUIView(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public GUIView(ScriptableObject obj) : base(obj) { }

        #region hasFocus

        /// <summary>
        /// 有焦点 属性信息
        /// </summary>
        public static XPropertyInfo hasFocus_PropertyInfo { get; } = GetXPropertyInfo(nameof(hasFocus));

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

        #region Repaint

        /// <summary>
        /// 重绘 方法信息
        /// </summary>
        public static XMethodInfo Repaint_MethodInfo { get; } = new XMethodInfo(Type, nameof(Repaint));

        /// <summary>
        /// 重绘
        /// </summary>
        public void Repaint()
        {
            Repaint_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion
    }

    /// <summary>
    /// GUI视图
    /// </summary>
    [LinkType("UnityEditor.GUIView")]
    public class GUIView : GUIView<GUIView>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public GUIView() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public GUIView(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public GUIView(ScriptableObject obj) : base(obj) { }
    }
}
