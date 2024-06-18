using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginRepairman.Tasks;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.PluginRepairman.States.RepairTask
{
    /// <summary>
    /// 拆装指南
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [RequireManager(typeof(RepairmanManager))]
    [Owner(typeof(RepairmanManager))]
    [Serializable]
    public class RepairGuide<T> : StateComponent<T>, ITask where T : RepairGuide<T>
    {
        /// <summary>
        /// 拆装任务
        /// </summary>
        [Name("拆装任务")]
        [Tip("拆装任务是拆装步骤和修理步骤组连线的组合", "Disassembly task is the combination of disassembly steps and repair steps")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup(typeof(RepairTaskWork), stateCollectionType = EStateCollectionType.Root)]
        public RepairTaskWork _repairTaskWork;

        /// <summary>
        /// 结果展示列表
        /// </summary>
        [Name("结果展示列表")]
        [Tip("任务开始时隐藏，任务完成后展示", "Hide at the beginning of the task and display after the task is completed")]
        public List<GameObject> _resultList = new List<GameObject>();

        /// <summary>
        /// 开始调用函数
        /// </summary>
        [Name("开始调用函数")]
        [UserDefineFun]
        public string _startCallbackFun;

        /// <summary>
        /// 完成调用函数
        /// </summary>
        [Name("完成调用函数")]
        [UserDefineFun]
        public string _finishCallbackFun;
        
        /// <summary>
        /// 零件列表
        /// </summary>
        protected List<Part> parts = new List<Part>();

        /// <summary>
        /// 工具列表
        /// </summary>
        protected List<Tool> tools = new List<Tool>();

        /// <summary>
        /// 显示名称
        /// </summary>
        public string showName { get => parent.name; set => parent.name = value; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public ETaskState taskState { get; set; }

        /// <summary>
        /// 当创建
        /// </summary>
        public override void OnCreated()
        {
            base.OnCreated();

            _repairTaskWork = SMSHelper.GetStateComponents<RepairTaskWork>().FirstOrDefault(); 
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            SetResultListActive(false);
        }

        private void SetResultListActive(bool active)
        {
            _resultList.ForEach(r =>
            {
                if (r)
                {
                    r.SetActive(active);
                }
            });
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            base.Init(data);
            
            parts = SMSHelper.GetStateComponents<Part>();
            tools = SMSHelper.GetStateComponents<Tool>();

            return true;
        }

        private bool orgAutoSelectPartAndTool = false;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            Selection.selectionChanged += OnSelectionPartChanged;
            ToolSelection.selectionChanged += OnSelectionToolChanged;

            Selection.selection = null;
            ToolSelection.Clear();
            ScriptManager.CallUDF(_startCallbackFun);

            orgAutoSelectPartAndTool = RepairStep.autoSelectPartAndTool;
            RepairStep.autoSelectPartAndTool = false;

            SetResultListActive(false);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            Selection.selectionChanged -= OnSelectionPartChanged;
            ToolSelection.selectionChanged -= OnSelectionToolChanged;

            RepairStep.autoSelectPartAndTool = orgAutoSelectPartAndTool;
            SetResultListActive(true);

            Selection.Clear();

            base.OnExit(data);
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => _repairTaskWork ? _repairTaskWork.stepState == PluginSMS.States.Show.EStepState.Finished : true;

        /// <summary>
        /// 获取当前步骤
        /// </summary>
        /// <returns></returns>
        public RepairStep GetCurrentStep() => StepGroupHelper.GetCurrentStepInGlobal(_repairTaskWork) as RepairStep;

        /// <summary>
        /// 检查
        /// </summary>
        /// <returns></returns>
        public bool Check() => true;

        /// <summary>
        /// 跳过
        /// </summary>
        /// <returns></returns>
        public virtual bool Skip() => true;

        /// <summary>
        /// 帮助
        /// </summary>
        void ITask.Help() { }

        /// <summary>
        /// 数据有效
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _repairTaskWork;

        /// <summary>
        /// 当选择零件变化
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected virtual void OnSelectionPartChanged(GameObject[] oldSelections, bool flag) { }

        /// <summary>
        /// 当选择工具变化
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected virtual void OnSelectionToolChanged(ITool[] oldSelections, bool flag) { }

        /// <summary>
        /// 是否是当前激活任务的零件
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        protected bool IsCurrentActiveTaskPart(GameObject go)
        {
            if (!go) return false;

            var curStep = GetCurrentStep();
            return curStep ? curStep.selectedParts.Exists(p => p.gameObject == go):false;
        }

        /// <summary>
        /// 是否是当前激活任务的工具游戏对象
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        protected bool IsGameObjectCurrentActiveTaskTool(GameObject go)
        {
            if (!go) return false;

            var curStep = GetCurrentStep();
            return curStep ? curStep.selectedTools.Exists(t => t.gameObject == go) : false;
        }

        /// <summary>
        /// 是否是当前激活任务的工具
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        protected bool IsCurrentActiveTaskTool(Tool tool)
        {
            if (tool==null) return false;

            var curStep = GetCurrentStep() as RepairStep;
            return curStep ? curStep.selectedTools.Exists(t => t.displayName==tool.displayName) : false;
        }
    }
}
