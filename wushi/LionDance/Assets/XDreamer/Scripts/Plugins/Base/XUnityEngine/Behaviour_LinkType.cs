using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// 行为关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Behaviour_LinkType<T> : Component_LinkType<T>
        where T : Behaviour_LinkType<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Behaviour_LinkType(Behaviour obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Behaviour_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected Behaviour_LinkType() { }
    }

    /// <summary>
    /// 行为关联类型
    /// </summary>
    [LinkType(typeof(Behaviour))]
    public class Behaviour_LinkType : Behaviour_LinkType<Behaviour_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Behaviour_LinkType(Behaviour obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Behaviour_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected Behaviour_LinkType() { }
    }
}
