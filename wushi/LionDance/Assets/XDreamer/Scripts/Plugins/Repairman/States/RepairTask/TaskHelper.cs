using System.Collections.Generic;
using XCSJ.PluginRepairman.Tasks;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.PluginRepairman.States.RepairTask
{
    /// <summary>
    /// 任务组手
    /// </summary>
    public class TaskHelper 
    {
        /// <summary>
        /// 搜索任务
        /// </summary>
        /// <param name="stateCollection"></param>
        /// <returns></returns>
        public static List<ITask> SearchTask(StateCollection stateCollection)
        {
            List<ITask> tasks = new List<ITask>();
            if (stateCollection)
            {
                List<State> states = new List<State>();
                TraversalHelper.TryGetStatesWithBreadthFirst((State)stateCollection.entryState, s => stateCollection.Contains(s), out states);

                states.ForEach(s =>
                {
                    var it = s.GetComponent<ITask>();
                    if (it != null)
                    {
                        CreateTaskList(it, tasks);
                    }
                });
            }
            return tasks;
        }

        private static void CreateTaskList(ITask task, List<ITask> taskList)
        {
            taskList.Add(task);

            if (task is ITaskList taskGroup)
            {
                foreach (var childTask in taskGroup.tasks)
                {
                    CreateTaskList(childTask, taskList);
                }
            }
        }

        /// <summary>
        /// 获取任务数量
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static int GetTaskCount(ITask task)
        {
            if (task is ITaskList)
            {
                int count = 0;
                foreach (var t in (task as ITaskList).tasks)
                {
                    count += GetTaskCount(t);
                }
                return count;
            }
            return 1;
        }

        /// <summary>
        /// 查找输入状态内的修理步骤或者步骤剪辑对应的修理步骤
        /// </summary>
        public static Step FindStepInPreviousState(Transition parent)
        {
            if (!parent.inState) return null;

            var step = parent.inState.GetComponent<Step>();
            if (step) return step;

            var stepClip = parent.inState.GetComponent<StepClip>();
            return stepClip ? stepClip.step : null;
        }
    }
}
