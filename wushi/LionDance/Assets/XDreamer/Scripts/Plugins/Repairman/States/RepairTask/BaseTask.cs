using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.PluginRepairman.States.RepairTask
{
    /// <summary>
    /// 抽象任务基类
    /// </summary>
    [RequireManager(typeof(RepairmanManager))]
    [Owner(typeof(RepairmanManager))]
    public abstract class BaseTask : Step, IBaseInfo
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string showName
        {
            get
            {
                return parent.name;
            }
            set
            {
                parent.name = value;
            }
        }

        string IBaseInfo.description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {

            base.OnExit(data);
        }

        /// <summary>
        /// 帮助
        /// </summary>
        public abstract void Help();
    }
}
