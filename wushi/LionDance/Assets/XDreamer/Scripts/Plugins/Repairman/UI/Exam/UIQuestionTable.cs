using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.States.Exam;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS;
using XCSJ.PluginXGUI.Base;
using System.Linq;

namespace XCSJ.PluginRepairman.UI.Exam
{
    /// <summary>
    /// 答题表格
    /// </summary>
    [Name("答题表格")]
    [RequireManager(typeof(RepairmanManager))]
    public class UIQuestionTable : View
    {
        /// <summary>
        /// 修理考试
        /// </summary>
        [Name("修理考试")]
        [StateComponentPopup(typeof(RepairExam), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RepairExam exam;

        /// <summary>
        /// 答题单元格模板
        /// </summary>
        [Name("答题单元格模板")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public UIQuestionCell questionCellTemplate;

        /// <summary>
        /// 正确颜色
        /// </summary>
        [Name("正确颜色")]
        public Color rightColor = Color.green;

        /// <summary>
        /// 错误颜色
        /// </summary>
        [Name("错误颜色")]
        public Color wrongColor = Color.red;

        /// <summary>
        /// 当前颜色
        /// </summary>
        [Name("当前颜色")]
        public Color currentColor = Color.blue;

        /// <summary>
        /// 未完成颜色
        /// </summary>
        [Name("未完成颜色")]
        public Color unfinishColor = Color.white;

        /// <summary>
        /// 可重用问题对象GUI池
        /// </summary>
        private WorkObjectPool<UIQuestionCell> guiQuestionPool = new WorkObjectPool<UIQuestionCell>();

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            if (questionCellTemplate)
            {
                questionCellTemplate.gameObject.SetActive(false);
            }

            guiQuestionPool.Init(
                () => 
                {
                    var go = questionCellTemplate.gameObject.XCloneObject();
                    if (go)
                    {
                        go.XSetParent(questionCellTemplate.transform.parent);
                        return go.GetComponent<UIQuestionCell>();
                    }
                    return default;
                },
                questionBox => questionBox.gameObject.SetActive(true),
                questionBox => questionBox.ResetState(),
                questionBox => questionBox);
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            if (!exam)
            {
                exam = SMSHelper.GetStateComponents<RepairExam>().FirstOrDefault();
            }
            if (exam)
            {
                exam.onStarted += CreateQuestionTable;
                CreateQuestionTable(exam.questions.Cast<IQuestion>().ToList());
            }
        }

        private void CreateQuestionTable(List<IQuestion> questions)
        {
            //Debug.Log("OnCreateQuestionTable!!");
            if (!enabled) return;
            if (!questionCellTemplate)
            {
                Debug.LogError("没有[答题单元格模板]资源，无法创建答题表格。");
                return;
            }

            ClearQuestionTable();

            for (int i = 0; i < questions.Count; ++i)
            {
                UIQuestionCell guiQuestion = guiQuestionPool.Alloc();
                if (guiQuestion) guiQuestion.SetData(questions[i] as IQuestion, i, this);
            }
        }

        private void ClearQuestionTable()
        {
            if (!enabled) return;
            guiQuestionPool.Clear();
        }

    }
}
