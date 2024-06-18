using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.LitJson;

namespace XCSJ.EditorTools.Windows.RichTexts
{
    /// <summary>
    /// 内容
    /// </summary>
    [Name("内容")]
    [Serializable]
    public class Content : Element
    {
        /// <summary>
        /// 名称
        /// </summary>
        public Element name = new Element(nameof(name));

        /// <summary>
        /// 提示
        /// </summary>
        public Element tip = new Element(nameof(tip));

        /// <summary>
        /// 内容
        /// </summary>
        [Json(false)]
        public GUIContent content => new GUIContent(name.value, tip.value);
    }
}
