using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 步骤剪辑：步骤剪辑组件是将动画组件关联到步骤上的对象。多个步骤剪辑构成一个步骤，组件激活后随即切换为完成态。
    /// </summary>
    [ComponentMenu(RepairmanCategory.StepDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(StepClip))]
    [Tip("步骤剪辑组件是将动画组件关联到步骤上的对象。多个步骤剪辑构成一个步骤，组件激活后随即切换为完成态。", "A step clip component is an object that associates an animation component to a step. Multiple step clips constitute one step, and the component will switch to the completed state immediately after activation.")]
    [XCSJ.Attributes.Icon(EIcon.StepClip)]
    [DisallowMultipleComponent]
    public class StepClip : StateComponent<StepClip>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "步骤剪辑";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(RepairmanCategory.Step, typeof(SMSManager))]
        [StateComponentMenu(RepairmanCategory.StepDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(StepClip))]
        [Tip("步骤剪辑组件是将动画组件关联到步骤上的对象。多个步骤剪辑构成一个步骤，组件激活后随即切换为完成态。", "A step clip component is an object that associates an animation component to a step. Multiple step clips constitute one step, and the component will switch to the completed state immediately after activation.")]
        [XCSJ.Attributes.Icon(EIcon.StepClip)]
        public static State CreateStepClip(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 步骤
        /// </summary>
        [Name("步骤", nameof(Step))]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup(typeof(Step), stateCollectionType = EStateCollectionType.Current, searchFlags = ESearchFlags.DefaultChildrenOptimize)]
        public Step step;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {            
            if (step)
            {
                step.AddClip(this);
            }
            clipState = EStepState.Unfinished;

            return base.Init(data);
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            clipState = EStepState.Active;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            clipState = EStepState.Finished;

            if (step)
            {
                step.OnStepClipStateChanged(this, clipState);
            }

            base.OnExit(data);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => step;

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;

        /// <summary>
        /// 剪辑状态
        /// </summary>
        public EStepState clipState { get; set; } = EStepState.None;

        /// <summary>
        /// 重置剪辑状态
        /// </summary>
        public void ResetClipState() { clipState = EStepState.Unfinished; }
    }
}
