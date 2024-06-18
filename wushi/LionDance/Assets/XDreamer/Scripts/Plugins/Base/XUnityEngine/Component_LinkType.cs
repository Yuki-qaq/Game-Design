using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// 组件关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Component_LinkType<T> : UnityEngine_Object<T>
        where T : Component_LinkType<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Component_LinkType(Component obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Component_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected Component_LinkType() { }
    }

    /// <summary>
    /// 组件关联类型
    /// </summary>
    [LinkType(typeof(Component))]
    public class Component_LinkType : Component_LinkType<Component_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Component_LinkType(Component obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Component_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected Component_LinkType() { }
    }
}
