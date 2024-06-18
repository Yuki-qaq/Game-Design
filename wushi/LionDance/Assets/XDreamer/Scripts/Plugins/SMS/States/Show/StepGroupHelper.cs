using System.Collections.Generic;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 步骤组组手
    /// </summary>
    public class StepGroupHelper
    {
        /// <summary>
        /// 获取根
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Step GetRoot(Step step)
        {
            if (!step) return null;
            return step.parentStep == null ? step : GetRoot(step.parentStep);
        }

        /// <summary>
        /// 转到状态
        /// </summary>
        /// <param name="stepGroup"></param>
        /// <param name="state"></param>
        public static void GotoState(StepGroup stepGroup, State state)
        {
            var root = GetRoot(stepGroup) as StepGroup;
            if (root)
            {
                root.GotoState(state);
            }
        }

        /// <summary>
        /// 获取选中步骤全局索引
        /// </summary>
        public static int FindSelectedIndexInGlobal(StepGroup stepGroup, bool includeGroup=false)
        {
            var list = GetAllStep(stepGroup);
            int selectedIndex = -1;
            int count = 0;
            foreach (var item in list)
            {
                if (item is StepGroup && !includeGroup) continue;

                ++count;
                if (item.selected)
                {
                    selectedIndex = count;
                }
            }
            return selectedIndex;
        }

        /// <summary>
        /// 构建步骤树形结构
        /// </summary>
        public static List<Step> InitStepTree(StepGroup stepGroup)
        {
            if (!stepGroup) return new List<Step>();

            stepGroup.ClearChildren();
            stepGroup.steps = SMSHelper.GetStateComponentsWithBreadthFirst<Step>(stepGroup.parent as StateCollection);
            foreach (var s in stepGroup.steps)
            {
                stepGroup.AddChildren(s);

                if (s is StepGroup group && group)
                {
                    InitStepTree(group);
                }
            }

            return stepGroup.steps;
        }

        private static Dictionary<StepGroup, List<Step>> allStepDic = new Dictionary<StepGroup, List<Step>>();

        /// <summary>
        /// 从全局获取当前步骤
        /// </summary>
        /// <param name="stepGroup"></param>
        /// <returns></returns>
        public static Step GetCurrentStepInGlobal(StepGroup stepGroup)
        {
            var steps = GetAllStep(stepGroup);
            return steps.FindLast(s => s.stepState == EStepState.Active);
        }

        /// <summary>
        /// 从全局获取下个步骤
        /// </summary>
        /// <param name="stepGroup"></param>
        /// <returns></returns>
        public static Step GetNextStepInGlobal(StepGroup stepGroup)
        {
            var steps = GetAllStep(stepGroup);
            var index = steps.FindLastIndex(s=>s.stepState==EStepState.Active);
            while ((++index) >= 0 && index < steps.Count)
            {
                var step = steps[index];
                if (!(step is StepGroup))
                {
                    return step;
                }
            }
            return null;
        }

        /// <summary>
        /// 从全局获取之前步骤
        /// </summary>
        /// <param name="stepGroup"></param>
        /// <returns></returns>
        public static Step GetPreviousStepInGlobal(StepGroup stepGroup)
        {
            var steps = GetAllStep(stepGroup);
            var index = steps.FindLastIndex(s => s.stepState == EStepState.Active);
            while((--index)>=0)
            {
                var step = steps[index];
                if (!(step is StepGroup))
                {
                    return step;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取所有步骤
        /// </summary>
        /// <param name="stepGroup"></param>
        /// <returns></returns>
        public static List<Step> GetAllStep(StepGroup stepGroup)
        {
            List<Step> allStep = new List<Step>();
            if (!stepGroup) return allStep;

            if (!allStepDic.TryGetValue(stepGroup, out allStep))
            {
                allStep = GetStepsInChildren(stepGroup);
                allStepDic.Add(stepGroup, allStep);
            }
            return allStep;
        }

        // 将stepGroup下所有步骤，构建为一整个链表
        private static List<Step> GetStepsInChildren(StepGroup stepGroup)
        {
            var stepList = new List<Step>();
            if (!stepGroup) return stepList;

            foreach (var s in stepGroup.steps)
            {               
                stepList.Add(s);

                if (s is StepGroup group && group)
                {
                    stepList.AddRange(GetStepsInChildren(group));
                }
            }
            return stepList;
        }

        /// <summary>
        /// 查找步骤列表并更新子级
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static List<Step> FindStepListAndUpdateChildren(StepGroup group)
        {
            if (!group) return new List<Step>();

            group.ClearChildren();
            group.steps = SMSHelper.GetStateComponentsWithBreadthFirst<Step>(group.parent as StateCollection);
            foreach (var s in group.steps)
            {
                group.AddChildren(s);

                if (s is StepGroup g && g)
                {
                    FindStepListAndUpdateChildren(g);
                }
            }
            return group.steps;
        }
    }
}
