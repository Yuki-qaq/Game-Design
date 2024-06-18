using UnityEngine.EventSystems;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine.XEventSystems
{
    /// <summary>
    /// UI行为关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIBehaviour_LinkType<T> : MonoBehaviour_LinkType<T>
        where T : UIBehaviour_LinkType<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UIBehaviour_LinkType(UIBehaviour obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UIBehaviour_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected UIBehaviour_LinkType() { }
    }

    /// <summary>
    /// UI行为关联类型
    /// </summary>
    [LinkType(typeof(UIBehaviour))]
    public class UIBehaviour_LinkType : UIBehaviour_LinkType<UIBehaviour_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UIBehaviour_LinkType(UIBehaviour obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UIBehaviour_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected UIBehaviour_LinkType() { }
    }
}
