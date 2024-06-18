using UnityEngine.UI;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine.XUI
{
    /// <summary>
    /// 文本关联类型
    /// </summary>
    [LinkType(typeof(Text))]
    public class Text_LinkType : MaskableGraphic_LinkType<Text_LinkType>
    {
        /// <summary>
        /// 文本
        /// </summary>
        public Text text => obj as Text;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Text_LinkType(Text obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Text_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected Text_LinkType() { }
    }
}
