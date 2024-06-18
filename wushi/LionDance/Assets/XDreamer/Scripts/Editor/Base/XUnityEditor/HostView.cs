using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 宿主视图
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HostView<T> : GUIView<T>
        where T : HostView<T>, new()
    {
        /// <summary>
        /// 构造
        /// </summary>
        public HostView() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public HostView(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public HostView(ScriptableObject obj) : base(obj) { }
    }

    /// <summary>
    /// 宿主视图
    /// </summary>
    [LinkType("UnityEditor.HostView")]
    public class HostView : HostView<HostView>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public HostView() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public HostView(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public HostView(ScriptableObject obj) : base(obj) { }
    }
}
