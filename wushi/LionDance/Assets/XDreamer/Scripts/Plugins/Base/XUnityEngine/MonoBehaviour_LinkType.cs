using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// Mono行为关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoBehaviour_LinkType<T> : Behaviour_LinkType<T>
        where T : MonoBehaviour_LinkType<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public MonoBehaviour_LinkType(MonoBehaviour obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public MonoBehaviour_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected MonoBehaviour_LinkType() { }
    }

    /// <summary>
    /// Mono行为关联类型
    /// </summary>
    [LinkType(typeof(MonoBehaviour))]
    public class MonoBehaviour_LinkType : MonoBehaviour_LinkType<MonoBehaviour_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public MonoBehaviour_LinkType(MonoBehaviour obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public MonoBehaviour_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected MonoBehaviour_LinkType() { }
    }
}
