using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.LineNotes
{
    /// <summary>
    /// 线样式
    /// </summary>
    [Name("线样式")]
    public class LineStyle : InteractProvider
    {
        /// <summary>
        /// 宽度
        /// </summary>
        [Name("宽度")]
        [Range(0, 100)]
        [Tip("=0为系统细线", "=0 is the system thin line")]
        public float width = 0;

        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        public Color color = Color.green;

        /// <summary>
        /// 材质
        /// </summary>
        [Name("材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material mat;

        /// <summary>
        /// 遮挡
        /// </summary>
        [Name("遮挡")]
        public bool occlusion = true;
    }
}
