using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginRepairman.Tools;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.PluginRepairman.States.RepairTask
{
    /// <summary>
    /// 拆装任务零件视图：
    /// 1、使用拆装任务步骤中的零件通过拆装助手生成不可交互的零件列表
    /// 2、当拆装步骤激活时，启用零件列表中对应视图项的可交互性
    /// 3、零件装配后，将零件上的交互对象设置为禁用
    /// </summary>
    [Name(Title, nameof(RepairTaskWorkPartView))]
    [XCSJ.Attributes.Icon(EIcon.Task)]
    [Tip("使用拆装任务步骤中的零件通过拆装助手生成不可交互的零件列表；当拆装步骤激活时，启用零件列表中对应视图项的可交互性", "Use the parts in the disassembly task steps to generate an interactive parts list through the disassembly assistant; Enable interactivity of corresponding view items in the parts list when the disassembly and assembly steps are activated")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RepairTaskWork))]
    [Owner(typeof(RepairmanManager))]
    [ComponentMenu(RepairmanCategory.StepDirectory + Title, typeof(RepairmanManager))]
    public sealed class RepairTaskWorkPartView : StateComponent<RepairTaskWorkPartView>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "拆装任务零件视图";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(RepairTaskWorkPartView))]
        [Tip("使用拆装任务步骤中的零件通过拆装助手生成不可交互的零件列表；当拆装步骤激活时，启用零件列表中对应视图项的可交互性", "Use the parts in the disassembly task steps to generate an interactive parts list through the disassembly assistant; Enable interactivity of corresponding view items in the parts list when the disassembly and assembly steps are activated")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanCategory.Step, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanCategory.StepDirectory + Title, typeof(RepairmanManager))]
        public static State CreateTaskWork(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(RepairTaskWorkPartView)), null, typeof(RepairTaskWorkPartView));
        }

        /// <summary>
        /// 拆装助手
        /// </summary>
        [Name("拆装助手")]
        [Tip("通过拆装助手创建当前拼装任务的零件视图", "Create a part view of the current assembly task through the disassembly assistant")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public RepairAssistant _repairAssistant;

        /// <summary>
        /// 拆装助手
        /// </summary>
        public RepairAssistant repairAssistant => this.XGetComponentInGlobal<RepairAssistant>(ref _repairAssistant);

        private RepairTaskWork _repairTaskWork;
        private RepairTaskWork repairTaskWork => _repairTaskWork ? _repairTaskWork : _repairTaskWork = GetComponent<RepairTaskWork>();

        /// <summary>
        /// 拼装进度
        /// </summary>
        public float taskProgress { get => _taskProgress; private set => _taskProgress = value; }
        private float _taskProgress = 0;

        /// <summary>
        /// 零件总数
        /// </summary>
        public int totalPartCount { get; private set; } = 0;

        /// <summary>
        /// 完成拼装零件数
        /// </summary>
        public int finishPartCount { get; private set; } = 0;

        private ComponentRecorder componentRecorder = new ComponentRecorder();

        private bool dataValidOnBeforeEntry = false;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            dataValidOnBeforeEntry = DataValidity();
            if (dataValidOnBeforeEntry)
            {
                totalPartCount = repairTaskWork.partCount + repairTaskWork.moduleCount;
                finishPartCount = 0;
                taskProgress = 0;

                // 创建零件视图
                var list = new List<Tools.Part>();
                foreach (var part in repairTaskWork.parts)
                {
                    var interactPart = part.interactPart;
                    list.Add(interactPart);
                    componentRecorder.Record(interactPart.GetComponentsInChildren<AbstractInteract>());
                }
                _partModels = repairAssistant.AddPartsToGrabbableList(list, false);

                StepGroup.onStepActive += OnStepActive;
                StepGroup.onStepFinish += OnStepFinish;

                InteractObject.onInteractFinished += OnInteractFinished;
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            if (dataValidOnBeforeEntry)
            {
                StepGroup.onStepActive -= OnStepActive;
                StepGroup.onStepFinish -= OnStepFinish;

                InteractObject.onInteractFinished -= OnInteractFinished;

                // 移除所有零件视图
                _partModels.Reverse();
                if (repairAssistant) repairAssistant.RemovePartsFromGrabbableList(_partModels);
                _partModels.Clear();

                componentRecorder.Recover();
            }
        }

        /// <summary>
        /// 有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => repairAssistant;

        private void OnInteractFinished(InteractObject interactObject, InteractData interactData)
        {
            // 监听零件装配态
            if (interactObject is Tools.Part interactPart && interactData.cmd == nameof(Tools.Part.Assembled)
                && interactPart.assembleState == EAssembleState.Assembled)
            {
                if (_partModels.Exists(pm => pm.part == interactPart))
                {
                    // 禁用零件上可交互组件交互
                    foreach (var item in interactPart.GetComponentsInChildren<AbstractInteract>())
                    {
                        item.enabled = false;
                    }
                    ++finishPartCount;
                    taskProgress = 1.0f * finishPartCount / totalPartCount;
                }
            }
        }

        /// <summary>
        /// 零件列表数据集
        /// </summary>
        private List<PartModel> _partModels = new List<PartModel>();

        private void OnStepActive(StepGroup stepGroup, Step step) => UpdatePartModel(stepGroup, step, true);

        private void OnStepFinish(StepGroup stepGroup, Step step) => UpdatePartModel(stepGroup, step, false);

        private void UpdatePartModel(StepGroup stepGroup, Step step, bool stepActive)
        {
            // 根据步骤调整列表视图中项的可交互性
            if (stepGroup == repairTaskWork && step is RepairStep repairStep && repairStep && repairTaskWork.ContainsRepairStep(repairStep))
            {
                foreach (var part in repairStep.selectedParts)
                {
                    if (!part) continue;

                    foreach (var m in _partModels)
                    {
                        m.SetInteractable(part.interactPart, stepActive);
                    }
                }
            }
        }
    }
}
