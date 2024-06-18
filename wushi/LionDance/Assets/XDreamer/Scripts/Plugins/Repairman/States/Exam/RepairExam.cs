using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginRepairman.States.RepairTask;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.States.Exam
{
    /// <summary>
    /// 考试接口
    /// </summary>
    public interface IExam
    {
        /// <summary>
        /// 当开始
        /// </summary>
        event Action<List<IQuestion>> onStarted;

        /// <summary>
        /// 当完成
        /// </summary>
        event Action<ExamResult> onFinished;
    }

    /// <summary>
    /// 考试结果
    /// </summary>
    public class ExamResult
    {
        /// <summary>
        /// 获取分数
        /// </summary>
        public float getScore;

        /// <summary>
        /// 总分数
        /// </summary>
        public float totalScore;

        /// <summary>
        /// 结果
        /// </summary>
        public bool result;

        /// <summary>
        /// 详细信息
        /// </summary>
        public string detailInfos;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="getScore"></param>
        /// <param name="totalScore"></param>
        /// <param name="result"></param>
        /// <param name="detailInfos"></param>
        public ExamResult(float getScore, float totalScore, bool result, string detailInfos)
        {
            this.getScore = getScore;
            this.totalScore = totalScore;
            this.result = result;
            this.detailInfos = detailInfos;
        }
    }

    /// <summary>
    /// 拆装考试
    /// </summary>
    [ComponentMenu(RepairmanCategory.StepDirectory + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairExam))]
    [XCSJ.Attributes.Icon(EIcon.Exam)]
    [Tip("拆装考试组件是由多个考题构成，在修理任务流程的基础上，通过选择零件和工具进行步骤答题，每题回答完毕，结果立即显示在答题卡上。考试完毕会弹出答题结果，结果中除了显示分数，还显示具体的回答错误的步骤信息。", "The disassembly and assembly test assembly is composed of multiple test questions. On the basis of the repair task process, answer the questions step by step by selecting parts and tools. After each question is answered, the results are immediately displayed on the answer card. After the exam, the answer result will pop up. In the result, not only the score, but also the specific step information of wrong answer will be displayed.")]
    public class RepairExam : RepairGuide<RepairExam>, IExam
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "拆装考试";

        /// <summary>
        /// 创建拆装考试
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(RepairExam))]
        [StateLib(RepairmanCategory.Step, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanCategory.StepDirectory + Title, typeof(RepairmanManager))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("拆装考试组件是由多个考题构成，在拆装任务流程的基础上，通过选择零件和工具进行步骤答题，每题回答完毕，结果立即显示在答题卡上。考试完毕会弹出答题结果，结果中除了显示分数，还显示具体的回答错误的步骤信息。", "The disassembly and assembly test assembly is composed of multiple test questions. On the basis of the repair task process, answer the questions step by step by selecting parts and tools. After each question is answered, the results are immediately displayed on the answer card. After the exam, the answer result will pop up. In the result, not only the score, but also the specific step information of wrong answer will be displayed.")]
        public static State CreateRepairExam(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 总分数
        /// </summary>
        [Name("总分数")]
        [Min(0)]
        public float totalScore = 100;

        /// <summary>
        /// 答题次数
        /// </summary>
        [Name("答题次数")]
        [Range(1, 6)]
        public int answerCount = 3;

        /// <summary>
        /// 考试通过标准线
        /// </summary>
        [Name("考试通过标准线")]
        [Range(0, 1)]
        public float passLevel = 0.6f;

        private RepairQuestion currentQuestion => questions.Find(q => q.state == EQuestionState.Current);

        /// <summary>
        /// 问题
        /// </summary>
        public List<RepairQuestion> questions { get; private set; } = new List<RepairQuestion>();

        /// <summary>
        /// 问题数量
        /// </summary>
        public int QuestionCount => questions.Count;

        #region 考试事件

        /// <summary>
        /// 问题接口
        /// </summary>
        public event Action<List<IQuestion>> onStarted;

        /// <summary>
        /// 考试信息改变
        /// </summary>
        public event Action<string> onExamInfoChanged;

        private void CallExamInfoChanged(string info) => onExamInfoChanged?.Invoke(info);

        /// <summary>
        /// 考试完成
        /// </summary>
        public event Action<ExamResult> onFinished;

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            base.Init(data);
            if (_repairTaskWork)
            {
                CreateQuestions();
                SetQuestionsEnable(false);
            }
            return true;
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            ResetData();
            SetQuestionsEnable(true);
            onStarted?.Invoke(questions.ConvertAll<IQuestion>(q => q as IQuestion));
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            onFinished?.Invoke(GetExamResult());
            ScriptManager.CallUDF(_finishCallbackFun);
            SetQuestionsEnable(false);

            base.OnExit(data);
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => questions.All(q => q.Finished());

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="data"></param>
        public override void Reset(ResetData data)
        {
            base.Reset(data);
            ResetData();
        }

        /// <summary>
        /// 创建考试:依据步骤的顺序添加考题
        /// </summary>
        private void CreateQuestions()
        {
            foreach (var s in _repairTaskWork.steps)
            {
                var q = s.GetComponent<RepairQuestion>();
                if (q) questions.Add(q);
            }
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        private void ResetData()
        {
            questions.ForEach(q => q.state = EQuestionState.Unfinish);
            rightQuestions.Clear();
            errorQuestionMap.Clear();
            skipQuestions.Clear();
            // 清空UI提示信息
            CallExamInfoChanged("");
        }

        private void SetQuestionsEnable(bool enable) => questions.ForEach(q => q.enable = enable);

        /// <summary>
        /// 提示
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => totalScore + CommonFun.Name(this.GetType(), nameof(totalScore));

        #region 答题交互

        /// <summary>
        /// 回答题目
        /// </summary>
        public void Answer()
        {
            if (Selection.count == 0)
            {
                CallExamInfoChanged("请选择一个对象！");
                return;
            }

            var question = currentQuestion;
            if (!question) return;
            if (question.Answer())
            {
                RecordRight(question);
                CallExamInfoChanged("回答正确！");
            }
            else
            {
                int errorCount = RecordError(question);
                CallExamInfoChanged("回答错误，还有[" + (answerCount - errorCount) + "]次答题机会！");
                if (errorCount >= answerCount)
                {
                    SkipQuestion();
                }
            }
        }

        /// <summary>
        /// 跳过题目
        /// </summary>
        public override bool Skip() => SkipQuestion();

        private bool SkipQuestion()
        {
            var question = currentQuestion;
            if (question)
            {
                RecordSkip(question);
                question.Skip();
                CallExamInfoChanged("跳过考题！");
            }
            return question;
        }

        /// <summary>
        /// 考试结果
        /// </summary>
        /// <returns></returns>
        public ExamResult GetExamResult()
        {
            float tsw = totalScoreWeightValue;
            if (tsw > 0)
            {
                var getScore = totalScore * (tsw - loseScoreWeightValue) / tsw;
                var passScore = totalScore * passLevel;
                var isPass = getScore > passScore || Mathf.Approximately(getScore, passScore);

                string errorDatails = "";
                foreach (var question in questions)
                {
                    if (skipQuestions.Contains(question))
                    {
                        errorDatails += question.description + "(错误)\n";
                    }
                    else if (errorQuestionMap.ContainsKey(question))
                    {
                        errorDatails += question.description + "(错误" + errorQuestionMap[question].ToString() + "次)\n";
                    }
                }
                return new ExamResult(getScore, totalScore, isPass, errorDatails);
            }
            return new ExamResult(0, 0, false, "");
        }

        #endregion

        #region 分数计算 和 错误记录

        /// <summary>
        /// 答对题目索引
        /// </summary>
        protected List<RepairQuestion> rightQuestions = new List<RepairQuestion>();

        /// <summary>
        /// 正确问题数量
        /// </summary>
        public int rightQuestionCount => rightQuestions.Count;

        /// <summary>
        /// key=答错题目索引, value=次数
        /// </summary>
        protected Dictionary<RepairQuestion, int> errorQuestionMap = new Dictionary<RepairQuestion, int>();

        /// <summary>
        /// 跳过步骤
        /// </summary>
        protected List<RepairQuestion> skipQuestions = new List<RepairQuestion>();

        private float totalScoreWeightValue => questions.Sum(q => q.score);

        /// <summary>
        /// 丢失分数权值
        /// </summary>
        /// <returns></returns>
        private float loseScoreWeightValue => questions.Sum(q =>
        {
            if (skipQuestions.Contains(q)) return q.score;// 跳过
            if (errorQuestionMap.ContainsKey(q)) return q.score * errorQuestionMap[q] / answerCount;//答错的丢失分数 = 当前步骤分数*（错误次数）/ 答题总次数
            return 0;
        });

        /// <summary>
        /// 记录错误，并返回错误次数
        /// </summary>
        private int RecordError(RepairQuestion rq)
        {
            if (!errorQuestionMap.ContainsKey(rq))
            {
                errorQuestionMap[rq] = 1;
                return 1;
            }
            var errorCount = errorQuestionMap[rq];
            errorQuestionMap[rq] = ++errorCount;
            return errorCount;
        }

        /// <summary>
        /// 记录正确回答
        /// </summary>
        private void RecordRight(RepairQuestion rq) => rightQuestions.AddWithDistinct(rq);

        /// <summary>
        /// 记录跳过回答
        /// </summary>
        private void RecordSkip(RepairQuestion rq) => skipQuestions.AddWithDistinct(rq);

        #endregion
    }
}
