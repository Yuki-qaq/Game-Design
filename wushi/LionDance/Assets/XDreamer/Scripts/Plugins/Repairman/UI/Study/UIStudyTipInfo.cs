using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginRepairman.States.Study;

namespace XCSJ.PluginRepairman.UI.Study
{
    /// <summary>
    /// 学习提示信息界面
    /// </summary>
    [Name("学习提示信息界面")]
    [RequireComponent(typeof(Text))]
    public class UIStudyTipInfo : RepairStudyListener
    {
        /// <summary>
        /// 选择正确文字色彩
        /// </summary>
        [Name("选择正确文字色彩")]
        public Color rightTextColor = Color.green;

        /// <summary>
        /// 选择错误文字色彩
        /// </summary>
        [Name("选择错误文字色彩")]
        public Color wrongTextColor = Color.red;

        /// <summary>
        /// 显示零件信息
        /// </summary>
        [Name("显示零件信息")]
        public bool displayPartInfo = true;

        /// <summary>
        /// 显示工具信息
        /// </summary>
        [Name("显示工具信息")]
        public bool displayToolInfo = false;

        /// <summary>
        /// 初始化清空信息
        /// </summary>
        [Name("初始化清空信息")]
        public bool setEmptyInfoOnEnable = true;

        private Text text;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!text) text = GetComponent<Text>();

            if (setEmptyInfoOnEnable && text)
            {
                text.text = "";
            }
        }
        
        /// <summary>
        /// 当零件选择集变化
        /// </summary>
        /// <param name="selectedGO"></param>
        /// <param name="right"></param>
        protected override void OnPartSelected(GameObject selectedGO, bool right)
        {
            if (!displayPartInfo) return;

            if (text)
            {
                text.color = right ? rightTextColor : wrongTextColor;
                text.text = selectedGO ? ("零件【" + selectedGO.name + "】选择" + (right ? "正确" : "错误")) : "";
            }
        }

        /// <summary>
        /// 当工具已选择
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="right"></param>
        protected override void OnToolSelected(PluginRepairman.States.Tool tool, bool right)
        {
            if (!displayToolInfo) return;

            if (text)
            {
                text.color = right ? rightTextColor : wrongTextColor;
                text.text = tool!=null ? ("工具【" + tool.displayName + "】选择" + (right ? "正确" : "错误")) : "";
            }
        }
    }
}