using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// 类UnityEngine.GUILayoutEntry的关联类
    /// </summary>
    [LinkType("UnityEngine.GUILayoutEntry")]
    public class GUILayoutEntry: GUILayoutEntry<GUILayoutEntry>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public GUILayoutEntry(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected GUILayoutEntry() { }
    }

    /// <summary>
    /// 类UnityEngine.GUILayoutEntry的泛型关联类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GUILayoutEntry<T> : LinkType<T> where T : GUILayoutEntry<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public GUILayoutEntry(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected GUILayoutEntry() { }
    }
}
