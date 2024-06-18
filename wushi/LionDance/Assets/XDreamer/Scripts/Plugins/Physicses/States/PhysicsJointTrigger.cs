using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginPhysicses.Tools.Gadgets;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginPhysicses.States
{
    /// <summary>
    /// 物理关节触发器：物理机关档位与指定值相等时触发状态切换为完成态
    /// </summary>
    [ComponentMenu(PhysicsCategory.TitleDirectory + Title, typeof(PhysicsManager))]
    [Name(Title, nameof(PhysicsJointTrigger))]
    [Tip("物理关节档位与指定值相等时触发状态切换为完成态", "When the gear of the physical joint is equal to the specified value, the trigger state is switched to the completed state")]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [DisallowMultipleComponent]
    [Owner(typeof(PhysicsManager))]
    public class PhysicsJointTrigger : Trigger<PhysicsJointTrigger>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "物理关节触发器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(PhysicsCategory.Title, typeof(PhysicsManager))]
        [StateComponentMenu(PhysicsCategory.TitleDirectory + Title, typeof(PhysicsManager))]
        [Name(Title, nameof(PhysicsJointTrigger))]
        [Tip("物理关节档位与指定值相等时触发状态切换为完成态", "When the gear of the physical joint is equal to the specified value, the trigger state is switched to the completed state")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 物理关节
        /// </summary>
        [Name("物理关节")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public PhysicsJoint _physicsJoint;

        /// <summary>
        /// 比较档位值
        /// </summary>
        [Name("比较档位值")]
        public bool _compareStepIndex = false;

        /// <summary>
        /// 档位比较规则
        /// </summary>
        public enum EStepCompareRule
        {
            /// <summary>
            /// 相等
            /// </summary>
            [Name("相等")]
            Equals,

            /// <summary>
            /// 不等
            /// </summary>
            [Name("不等")]
            NotEquals,

            /// <summary>
            /// 切换到
            /// </summary>
            [Name("切换到")]
            [Tip("从其他档位切换到当前档位值", "Switching from other gears to the current gear value")]
            SwithTo,

            /// <summary>
            /// 切换自
            /// </summary>
            [Name("切换自")]
            [Tip("从当前档位值切换到其他档位值", "Switching from the current gear value to another gear value")]
            SwithFrom,

            /// <summary>
            /// 切换
            /// </summary>
            [Name("切换")]
            Swith,
        }

        /// <summary>
        /// 档位比较规则
        /// </summary>
        [Name("档位比较规则")]
        [EnumPopup]
        public EStepCompareRule _stepCompareRule = EStepCompareRule.Equals;

        /// <summary>
        /// 档位值
        /// </summary>
        [Name("档位值")]
        [Tip("值需要在物理关节所设定的档位区间范围内")]
        [HideInSuperInspector(nameof(_compareStepIndex), EValidityCheckType.Equal | EValidityCheckType.Or, false, nameof(_stepCompareRule), EValidityCheckType.Equal, EStepCompareRule.Swith)]
        public PositiveIntPropertyValue _stepIndex = new PositiveIntPropertyValue(0);

        /// <summary>
        /// 档位值变量
        /// </summary>
        [Name("档位值变量")]
        [VarString]
        public string _stepVar;

        private int lastIndex;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            if (_physicsJoint)
            {
                lastIndex = _physicsJoint.currentStep;
            }
            PhysicsJoint._onStepChanged += OnStepChanged;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            PhysicsJoint._onStepChanged -= OnStepChanged;
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _physicsJoint;

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            var tip = _physicsJoint ? _physicsJoint.name : "";
            if (_compareStepIndex && _stepIndex.TryGetValue(out var value))
            {
                tip += ":" + value;
            }
            return tip;
        }

        private void OnStepChanged(PhysicsJoint physicsJoint, int stepIndex)
        {
            if (_physicsJoint != physicsJoint) return;

            if (_compareStepIndex)
            {
                if (_stepIndex.TryGetValue(out var value))
                {

                    switch (_stepCompareRule)
                    {
                        case EStepCompareRule.Equals:
                            {
                                finished = value == stepIndex;
                                break;
                            }
                        case EStepCompareRule.NotEquals:
                            {
                                finished = value != stepIndex;
                                break;
                            }
                        case EStepCompareRule.SwithTo:
                            {
                                finished = (lastIndex != stepIndex && value == stepIndex);
                                break;
                            }
                        case EStepCompareRule.SwithFrom:
                            {
                                finished = (lastIndex == value && value != stepIndex);
                                break;
                            }
                        case EStepCompareRule.Swith:
                            {
                                finished = lastIndex != value;
                                break;
                            }
                    }
                }

                lastIndex = stepIndex;
            }
            else
            {
                finished = true;
            }

            if (finished)
            {
                _stepVar.TrySetOrAddSetHierarchyVarValue(stepIndex);
            }
        }
    }
}