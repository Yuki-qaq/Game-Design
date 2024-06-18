using UnityEngine.UI;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine.XUI
{
    /// <summary>
    /// 可遮罩图形关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaskableGraphic_LinkType<T> : Graphic_LinkType<T>
         where T : MaskableGraphic_LinkType<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public MaskableGraphic_LinkType(MaskableGraphic obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public MaskableGraphic_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected MaskableGraphic_LinkType() { }
    }

    /// <summary>
    /// 可遮罩图形关联类型
    /// </summary>
    [LinkType(typeof(MaskableGraphic))]
    public class MaskableGraphic_LinkType : MaskableGraphic_LinkType<MaskableGraphic_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public MaskableGraphic_LinkType(MaskableGraphic obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public MaskableGraphic_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected MaskableGraphic_LinkType() { }
    }
}
