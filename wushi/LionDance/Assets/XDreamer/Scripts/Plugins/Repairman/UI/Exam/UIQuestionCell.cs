using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.States.Exam;

namespace XCSJ.PluginRepairman.UI.Exam
{
    /// <summary>
    /// 问题方格
    /// </summary>
    [Name("问题方格")]
    [RequireManager(typeof(RepairmanManager))]
    public class UIQuestionCell : InteractProvider
    {
        /// <summary>
        /// 题目文字描述
        /// </summary>
        [Name("题目文字描述")]
        public Text questionText;

        /// <summary>
        /// 题目图片
        /// </summary>
        [Name("题目图片")]
        public Image img;

        private UIQuestionTable questionTable;

        private IQuestion question;

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset() => FindComponents();

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake() => FindComponents();

        private void FindComponents()
        {
            if (!questionText)
            {
                questionText = GetComponent<Text>();
            }

            if (!img)
            {
                img = GetComponent<Image>();
            }
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="question"></param>
        /// <param name="questionIndex"></param>
        /// <param name="questionTable"></param>
        public void SetData(IQuestion question, int questionIndex, UIQuestionTable questionTable)
        {
            this.questionTable = questionTable;
            this.question = question;

            transform.SetSiblingIndex(questionIndex);
            if (questionText)
            {
                questionText.text = (questionIndex + 1).ToString();
            }
        }

        private EQuestionState oldState = EQuestionState.Unfinish;

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (question!=null && question.state!=oldState)
            {
                //Debug.Log("ResetState ==> oldState:" + oldState + ",question.state"+ question.state);
                oldState = question.state;
                SetState(oldState);
            }
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void ResetState()
        {
            //Debug.Log("ResetState ==> GUIQuestionBox:" + question.description+ ",state:"+ oldState);
            SetState(EQuestionState.Unfinish);
            //Debug.Log("ResetState ==> state:" + oldState);
        }

        private void SetState(EQuestionState questionState)
        {
            oldState = questionState;

            if (!img) return;

            switch (oldState)
            {
                case EQuestionState.Current:
                    {
                        img.color = questionTable.currentColor;
                        break;
                    }
                case EQuestionState.Unfinish:
                    {
                        img.color = questionTable.unfinishColor;
                        break;
                    }
                case EQuestionState.Right:
                    {
                        img.color = questionTable.rightColor;
                        break;
                    }
                case EQuestionState.Wrong:
                    {
                        img.color = questionTable.wrongColor;
                        break;
                    }
            }
        }
    }
}
