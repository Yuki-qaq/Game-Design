using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.LitJson;

namespace XCSJ.EditorRepairman
{
    /// <summary>
    /// 拆装修理教学
    /// </summary>
    [XDreamerPreferences]
    [Name("拆装修理教学")]
    [Import]
    public class RepairmanTeachingOption : XDreamerOption<RepairmanTeachingOption>
    {
        /// <summary>
        /// 答题表格尺寸
        /// </summary>
        [Name("答题表格尺寸")]
        [Json(exportString = true)]
        public Vector2 questionTableSize = new Vector2(200, 100);

        /// <summary>
        /// 答题单元格尺寸
        /// </summary>
        [Name("答题单元格尺寸")]
        [Json(exportString = true)]
        public Vector2 questionCellSize = new Vector2(40, 40);

        /// <summary>
        /// 答题单元格间距
        /// </summary>
        [Name("答题单元格间距")]
        [Json(exportString = true)]
        public Vector2 CellSpaceSize = new Vector2(1, 1);
    }
}
