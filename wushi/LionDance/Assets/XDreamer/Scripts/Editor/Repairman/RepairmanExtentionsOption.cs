using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.LitJson;

namespace XCSJ.EditorRepairman
{
    /// <summary>
    /// 拆装修理扩展
    /// </summary>
    [XDreamerPreferences]
    [Name("拆装修理扩展")]
    [Import]
    public class RepairmanExtentionsOption : XDreamerOption<RepairmanExtentionsOption>
    {
        /// <summary>
        /// 零件列表尺寸
        /// </summary>
        [Name("零件列表尺寸")]
        [Json(exportString = true)]
        public Vector2 partListSize = new Vector2(100, 200);

        /// <summary>
        /// 零件项尺寸
        /// </summary>
        [Name("零件项尺寸")]
        [Json(exportString = true)]
        public Vector2 partItemSize = new Vector2(80, 30);

        /// <summary>
        /// 工具包尺寸
        /// </summary>
        [Name("工具包尺寸")]
        [Json(exportString = true)]
        public Vector2 toolBagSize = new Vector2(200, 100);

        /// <summary>
        /// 工具项尺寸
        /// </summary>
        [Name("工具项尺寸")]
        [Json(exportString = true)]
        public Vector2 toolItemSize = new Vector2(60, 60);

        /// <summary>
        /// 项间距
        /// </summary>
        [Name("项间距")]
        [Json(exportString = true)]
        public Vector2 CellSpaceSize = new Vector2(1, 1);
    }
}
