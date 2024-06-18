using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 视图
    /// </summary>
    public interface IView : IScriptableObject_LinkType
    {

    }

    /// <summary>
    /// 视图
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class View<T> : ScriptableObject_LinkType<T>, IView
        where T : View<T>, new()
    {
        /// <summary>
        /// 构造
        /// </summary>
        public View() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public View(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public View(ScriptableObject obj) : base(obj) { }
    }

    /// <summary>
    /// 视图
    /// </summary>
    [LinkType("UnityEditor.View")]
    public class View : View<View>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public View() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public View(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public View(ScriptableObject obj) : base(obj) { }
    }
}
