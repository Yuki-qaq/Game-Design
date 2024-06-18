using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.States.Exam;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.UI.Exam
{
    /// <summary>
    /// 考试信息界面:考试过程中用于对答题情况的描述信息
    /// </summary>
    [RequireComponent(typeof(Text))]
    [RequireManager(typeof(RepairmanManager))]
    [Name("考试信息界面")]
    [Tip("考试过程中用于对答题情况的描述信息", "Descriptive information used for answering questions during the exam process")]
    public class UIExamInfo : InteractProvider
    {
        /// <summary>
        /// 拆装修理考试
        /// </summary>
        [Name("拆装修理考试")]
        [StateComponentPopup(typeof(RepairExam), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RepairExam exam;

        /// <summary>
        /// 初始化清空信息
        /// </summary>
        [Name("初始化清空信息")]
        public bool setEmptyInfoOnEnable = true;

        private Text textInfo;

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected void Awake()
        {
            textInfo = GetComponent<Text>();

            if (!exam)
            {
                exam = SMSHelper.GetStateComponents<RepairExam>().FirstOrDefault();
            }
            if (exam)
            {
                exam.onExamInfoChanged += OnExamInfoChanged;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (setEmptyInfoOnEnable && textInfo)
            {
                textInfo.text = "";
            }
        }

        /// <summary>
        /// 当考试信息变更
        /// </summary>
        /// <param name="info"></param>
        protected void OnExamInfoChanged(string info)
        {
            textInfo.text = info;
        }
    }
}
