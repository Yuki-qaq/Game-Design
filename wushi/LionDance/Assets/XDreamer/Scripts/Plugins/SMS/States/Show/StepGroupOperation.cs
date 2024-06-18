using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 步骤组操作:步骤组操作组件是对步骤组进行控制的执行体。控制包括【跳转上一步】、【跳转下一步】、【重置当前步骤】、【跳转到开始步骤】和【跳转到结束步骤】等操作，组件执行完毕后切换为完成态。
    /// </summary>
    //[ComponentMenu(SMSCategory.ShowDirectory+ Title, typeof(SMSManager))]
    [Name(Title, nameof(StepGroupOperation))]
    [XCSJ.Attributes.Icon(index = 33647)]
    [DisallowMultipleComponent]
    [Tip("步骤组操作组件是对步骤组进行控制的执行体。控制包括【跳转上一步】、【跳转下一步】、【重置当前步骤】、【跳转到开始步骤】和【跳转到结束步骤】等操作，组件执行完毕后切换为完成态。", "The step group operation component is the actuator that controls the step group. Control includes [jump to previous step], [jump to next step], [reset current step], [jump to start step] and [jump to end step], etc. after the component is executed, it switches to the completed state.")]
    public abstract class StepGroupOperation : LifecycleExecutor<StepGroupOperation>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "步骤组操作";

        ///// <summary>
        ///// 创建
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //[StateLib(SMSCategory.Show, typeof(SMSManager))]
        //[StateComponentMenu(SMSCategory.ShowDirectory+ Title, typeof(SMSManager))]
        //[Name(Title, nameof(StepGroupOperation))]
        //[Tip("步骤组操作组件是对步骤组进行控制的执行体。控制包括【跳转上一步】、【跳转下一步】、【重置当前步骤】、【跳转到开始步骤】和【跳转到结束步骤】等操作，组件执行完毕后切换为完成态。", "The step group operation component is the actuator that controls the step group. Control includes [jump to previous step], [jump to next step], [reset current step], [jump to start step] and [jump to end step], etc. after the component is executed, it switches to the completed state.")]
        //[XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        //public static State CreateStepGroupController(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 步骤组
        /// </summary>
        [Name("步骤组")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup()]
        public StepGroup stepGroup = null;

        /// <summary>
        /// 步骤组操作
        /// </summary>
        [Name("步骤组操作")]
        [EnumPopup]
        public EStepGroupOperation stepGroupOperation = EStepGroupOperation.None;

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="data"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData data, EExecuteMode executeMode)
        {
            OperateStepGroup(stepGroupOperation);
        }

        private void OperateStepGroup(EStepGroupOperation stepGroupOperation)
        {
            if (!stepGroup) return;

            switch (stepGroupOperation)
            {
                case EStepGroupOperation.None:
                    break;
                case EStepGroupOperation.GotoPreviousStep:
                    {
                        stepGroup.GotoPreviousStep();
                        break;
                    }
                case EStepGroupOperation.GotoNextStep:
                    {
                        stepGroup.GotoNextStep();
                        break;
                    }
                case EStepGroupOperation.ResetCurrentStep:
                    {
                        stepGroup.ResetCurrentStep();
                        break;
                    }
                case EStepGroupOperation.GotoFirstStep:
                    {
                        stepGroup.GotoFirstStep();
                        break;
                    }                    
                case EStepGroupOperation.GotoLastStep:
                    {
                        stepGroup.GotoLastStep();
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => stepGroup;

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => CommonFun.Name(stepGroupOperation);

        /// <summary>
        /// 步骤组操作
        /// </summary>
        [Name("步骤组操作")]
        public enum EStepGroupOperation
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 跳转到上一个步骤
            /// </summary>
            [Name("跳转到上一个步骤")]
            GotoPreviousStep,

            /// <summary>
            /// 跳转到下一个步骤
            /// </summary>
            [Name("跳转到下一个步骤")]
            GotoNextStep,

            /// <summary>
            /// 重置当前步骤
            /// </summary>
            [Name("重置当前步骤")]
            ResetCurrentStep,

            /// <summary>
            /// 跳转到开始步骤
            /// </summary>
            [Name("跳转到开始步骤")]
            GotoFirstStep,

            /// <summary>
            /// 跳转到结束步骤
            /// </summary>
            [Name("跳转到结束步骤")]
            GotoLastStep,
        }
    }
}
