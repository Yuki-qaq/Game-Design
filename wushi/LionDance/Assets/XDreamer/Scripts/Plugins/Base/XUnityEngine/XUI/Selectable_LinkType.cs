using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine.XEventSystems;

namespace XCSJ.Extension.Base.XUnityEngine.XUI
{
    /// <summary>
    /// 可选择的关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Selectable_LinkType<T> : UIBehaviour_LinkType<T>
        where T : Selectable_LinkType<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Selectable_LinkType(Selectable obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Selectable_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected Selectable_LinkType() { }
    }

    /// <summary>
    /// 可选择的关联类型
    /// </summary>
    [LinkType(typeof(Selectable))]
    public class Selectable_LinkType : Selectable_LinkType<Selectable_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Selectable_LinkType(Selectable obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Selectable_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected Selectable_LinkType() { }
    }
}
