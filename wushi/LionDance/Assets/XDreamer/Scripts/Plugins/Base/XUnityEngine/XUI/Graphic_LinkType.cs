using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine.XEventSystems;

namespace XCSJ.Extension.Base.XUnityEngine.XUI
{
    /// <summary>
    /// 图形关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Graphic_LinkType<T>: UIBehaviour_LinkType<T>
         where T : Graphic_LinkType<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Graphic_LinkType(Graphic obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Graphic_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected Graphic_LinkType() { }
    }

    /// <summary>
    /// 图形关联类型
    /// </summary>
    [LinkType(typeof(Graphic))]
    public class Graphic_LinkType : Graphic_LinkType<Graphic_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Graphic_LinkType(Graphic obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Graphic_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected Graphic_LinkType() { }
    }
}
