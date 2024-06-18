using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// 类UnityEngine.GUILayoutGroup的关联类
    /// </summary>
    [LinkType("UnityEngine.GUILayoutGroup")]
    public class GUILayoutGroup : GUILayoutGroup<GUILayoutGroup>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public GUILayoutGroup(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected GUILayoutGroup() { }
    }

    /// <summary>
    /// 类UnityEngine.GUILayoutGroup的泛型关联类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GUILayoutGroup<T> : GUILayoutEntry<T> where T : GUILayoutGroup<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public GUILayoutGroup(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected GUILayoutGroup() { }

        #region isVertical

        /// <summary>
        /// 是垂直的 字段信息
        /// </summary>
        public static XFieldInfo isVertical_FieldInfo { get; } = GetXFieldInfo(nameof(isVertical));

        /// <summary>
        /// 是垂直的
        /// </summary>
        public bool isVertical => (bool)isVertical_FieldInfo.GetValue(obj);

        #endregion
    }
}
