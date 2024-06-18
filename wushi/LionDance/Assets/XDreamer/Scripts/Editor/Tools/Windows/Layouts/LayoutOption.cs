using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.LitJson;

namespace XCSJ.EditorTools.Windows.Layouts
{
    /// <summary>
    /// 布局选项
    /// </summary>
    [XDreamerPreferences]
    [Name("工具包窗口-布局窗口")]
    [Import]
    public class LayoutOption : XDreamerOption<LayoutOption>
    {
        /// <summary>
        /// 标准颜色1
        /// </summary>
        [Name("标准颜色1")]
        [Json(exportString = true)]
        public Color standardColor1 = Color.red;

        /// <summary>
        /// 标准颜色2
        /// </summary>
        [Name("标准颜色2")]
        [Json(exportString = true)]
        public Color standardColor2 = Color.green;
    }
}
