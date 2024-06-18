using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.States.RepairTask;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;
using XCSJ.PluginXGUI.Windows.TreeViews;

namespace XCSJ.PluginRepairman.UI.RepairTask
{
    /// <summary>
    /// 树视图任务工作数据
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UITreeView))]
    [Name("树视图任务工作数据")]
    [RequireManager(typeof(RepairmanManager))]
    public class UITreeViewTaskWorkData : InteractProvider
    {
        /// <summary>
        /// 拆装任务
        /// </summary>
        [Name("拆装任务")]
        [Tip("状态机系统中的计划状态组件", "Planning state component in state machine system")]
        [StateComponentPopup(typeof(RepairTaskWork), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RepairTaskWork repairTaskWork;

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            try
            {
                if (!repairTaskWork)
                {
                    repairTaskWork = SMSHelper.GetStateComponents<RepairTaskWork>().FirstOrDefault();
                }
                if (repairTaskWork)
                {
                    var tree = GetComponent<UITreeView>();
                    tree.data = repairTaskWork;
                    tree.Clear();
                    tree.Create();
                }
            }
            catch { }
        }
    }
}