using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 渲染器区间动作:用于在左中右播放区间修改渲染器属性的动画，属性包含颜色、透明度、材质和组件启用。组件在设定的时间区间内执行修改属性的动作，播放完毕后，组件切换为完成态。如果渲染器没有材质，则执行失败。
    /// </summary>
    [ComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(RendererRangeMotion))]
    [Tip("用于在左中右播放区间修改渲染器属性的动画，属性包含颜色、透明度、材质和组件启用。组件在设定的时间区间内执行修改属性的动作，播放完毕后，组件切换为完成态。如果渲染器没有材质，则执行失败。", "An animation used to modify the renderer attributes in the left, middle, and right playback intervals, including color, transparency, material, and component enablement. The component executes the action of modifying attributes within the set time interval, and after playing, the component switches to the completed state. If the renderer does not have materials, the execution fails.")]
    [XCSJ.Attributes.Icon(EIcon.Material)]
    public class RendererRangeMotion : RendererRangeHandle<RendererRangeMotion>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渲染器区间动作";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.Action, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(RendererRangeMotion))]
        [Tip("用于在左中右播放区间修改渲染器属性的动画，属性包含颜色、透明度、材质和组件启用。组件在设定的时间区间内执行修改属性的动作，播放完毕后，组件切换为完成态。如果渲染器没有材质，则执行失败。", "An animation used to modify the renderer attributes in the left, middle, and right playback intervals, including color, transparency, material, and component enablement. The component executes the action of modifying attributes within the set time interval, and after playing, the component switches to the completed state. If the renderer does not have materials, the execution fails.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        private bool onEntrySwitchFlag = false;
        private bool leftSwitchFlag = false;
        private bool inSwitchFlag = false;
        private bool rightSwitchFlag = false;
        private bool onExitSwitchFlag = false;

        /// <summary>
        /// 操作类型
        /// </summary>
        public enum EOperationType
        {
            /// <summary>
            /// 颜色
            /// </summary>
            [Name("颜色")]
            Color,

            /// <summary>
            /// 透明度
            /// </summary>
            [Name("透明度")]
            Alpha,

            /// <summary>
            /// 材质
            /// </summary>
            [Name("材质")]
            Material,

            /// <summary>
            /// 启用
            /// </summary>
            [Name("启用")]
            Enable,
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        [Name("操作类型")]
        [EnumPopup]
        public EOperationType _operationType = EOperationType.Color;

        #region 颜色

        /// <summary>
        /// 进入时颜色值
        /// </summary>
        [Name("进入时颜色值")]
        [HideInSuperInspector(nameof(onEntry), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(onEntry), EValidityCheckType.Equal, EBool.None)]
        public UnityEngine.Color _onEntryColorValue = default;

        /// <summary>
        /// 区间左颜色值
        /// </summary>
        [Name("区间左颜色值")]
        [HideInSuperInspector(nameof(leftRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(leftRange), EValidityCheckType.Equal, EBool.None)]
        public UnityEngine.Color _leftColorValue = default;

        /// <summary>
        /// 区间内颜色值
        /// </summary>
        [Name("区间内颜色值")]
        [HideInSuperInspector(nameof(inRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(inRange), EValidityCheckType.Equal, EBool.None)]
        public UnityEngine.Color _inColorValue = default;

        /// <summary>
        /// 区间右颜色值
        /// </summary>
        [Name("区间右颜色值")]
        [HideInSuperInspector(nameof(rightRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(rightRange), EValidityCheckType.Equal, EBool.None)]
        public UnityEngine.Color _rightColorValue = default;

        /// <summary>
        /// 退出时颜色值
        /// </summary>
        [Name("退出时颜色值")]
        [HideInSuperInspector(nameof(onExit), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(onExit), EValidityCheckType.Equal, EBool.None)]
        public UnityEngine.Color _onExitColorValue = default;

        private void SetColor(Recorder recorder, EBool boolValue, ref UnityEngine.Color value, ref bool switchFlag)
        {
            switch (boolValue)
            {
                case EBool.Yes:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.SetColor(value);
                        }
                        break;
                    }
                case EBool.No:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.RecoverColor();
                        }
                        break;
                    }
                case EBool.Switch:
                    {
                        if (switchFlag = !switchFlag)
                        {
                            foreach (var info in recorder._records)
                            {
                                info.SetColor(value);
                            }
                        }
                        else
                        {
                            foreach (var info in recorder._records)
                            {
                                info.RecoverColor();
                            }
                        }
                        break;
                    }
            }
        }

        #endregion

        #region 透明度

        /// <summary>
        /// 进入时透明度值
        /// </summary>
        [Name("进入时透明度值")]
        [HideInSuperInspector(nameof(onEntry), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(onEntry), EValidityCheckType.Equal, EBool.None)]
        public float _onEntryAlphaValue = default;

        /// <summary>
        /// 区间左透明度值
        /// </summary>
        [Name("区间左透明度值")]
        [HideInSuperInspector(nameof(leftRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(leftRange), EValidityCheckType.Equal, EBool.None)]
        public float _leftAlphaValue = default;

        /// <summary>
        /// 区间内透明度值
        /// </summary>
        [Name("区间内透明度值")]
        [HideInSuperInspector(nameof(inRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(inRange), EValidityCheckType.Equal, EBool.None)]
        public float _inAlphaValue = default;

        /// <summary>
        /// 区间右透明度值
        /// </summary>
        [Name("区间右透明度值")]
        [HideInSuperInspector(nameof(rightRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(rightRange), EValidityCheckType.Equal, EBool.None)]
        public float _rightAlphaValue = default;

        /// <summary>
        /// 退出时透明度值
        /// </summary>
        [Name("退出时透明度值")]
        [HideInSuperInspector(nameof(onExit), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(onExit), EValidityCheckType.Equal, EBool.None)]
        public float _onExitAlphaValue = default;

        private void SetAlpha(Recorder recorder, EBool boolValue, ref float value, ref bool switchFlag)
        {
            switch (boolValue)
            {
                case EBool.Yes:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.SetAlpha(value);
                        }
                        break;
                    }
                case EBool.No:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.RecoverColor();
                        }
                        break;
                    }
                case EBool.Switch:
                    {
                        if (switchFlag = !switchFlag)
                        {
                            foreach (var info in recorder._records)
                            {
                                info.SetAlpha(value);
                            }
                        }
                        else
                        {
                            foreach (var info in recorder._records)
                            {
                                info.RecoverColor();
                            }
                        }
                        break;
                    }
            }
        }

        #endregion

        #region 材质

        /// <summary>
        /// 设置材质规则
        /// </summary>
        [Name("设置材质规则")]
        public enum ESetMaterialRule
        {
            /// <summary>
            /// 直接替换
            /// </summary>
            [Name("直接替换")]
            Direct,

            /// <summary>
            /// 填充
            /// </summary>
            [Name("填充")]
            [Tip("保持原材质个数不变，进行填充", "Keep the number of original materials unchanged for filling")]
            Fill
        }

        /// <summary>
        /// 设置材质规则
        /// </summary>
        [Name("设置材质规则")]
        [EnumPopup]
        public ESetMaterialRule _setMaterialRule = ESetMaterialRule.Direct;

        /// <summary>
        /// 进入时材质值
        /// </summary>
        [Name("进入时材质值")]
        [HideInSuperInspector(nameof(onEntry), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(onEntry), EValidityCheckType.Equal, EBool.None)]
        public Material[] _onEntryMaterialValue = default;

        /// <summary>
        /// 区间左材质值
        /// </summary>
        [Name("区间左材质值")]
        [HideInSuperInspector(nameof(leftRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(leftRange), EValidityCheckType.Equal, EBool.None)]
        public Material[] _leftMaterialValue = default;

        /// <summary>
        /// 区间内材质值
        /// </summary>
        [Name("区间内材质值")]
        [HideInSuperInspector(nameof(inRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(inRange), EValidityCheckType.Equal, EBool.None)]
        public Material[] _inMaterialValue = default;

        /// <summary>
        /// 区间右材质值
        /// </summary>
        [Name("区间右材质值")]
        [HideInSuperInspector(nameof(rightRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(rightRange), EValidityCheckType.Equal, EBool.None)]
        public Material[] _rightMaterialValue = default;

        /// <summary>
        /// 退出时材质值
        /// </summary>
        [Name("退出时材质值")]
        [HideInSuperInspector(nameof(onExit), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(onExit), EValidityCheckType.Equal, EBool.None)]
        public Material[] _onExitMaterialValue = default;

        private void SetMaterial(Recorder recorder, EBool boolValue, ref Material[] value, ref bool switchFlag)
        {
            switch (boolValue)
            {
                case EBool.Yes:
                    {
                        SetMaterial(recorder, value, _setMaterialRule);
                        break;
                    }
                case EBool.No:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.RecoverMaterial();
                        }
                        break;
                    }
                case EBool.Switch:
                    {
                        if (switchFlag = !switchFlag)
                        {
                            SetMaterial(recorder, value, _setMaterialRule);
                        }
                        else
                        {
                            foreach (var info in recorder._records)
                            {
                                info.RecoverMaterial();
                            }
                        }
                        break;
                    }
            }
        }

        private void SetMaterial(Recorder recorder, Material[] value, ESetMaterialRule setMaterialRule)
        {
            switch (setMaterialRule)
            {
                case ESetMaterialRule.Fill:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.FillMaterialSize(value);
                        }
                        break;
                    }
                case ESetMaterialRule.Direct:
                default:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.SetMaterial(value);
                        }
                        break;
                    }
            }
        }

        #endregion

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _onEntryColorValue = UnityEngine.Color.green;
            _leftColorValue = UnityEngine.Color.green;
            _inColorValue = UnityEngine.Color.green;
            _rightColorValue = UnityEngine.Color.green;
            _onExitColorValue = UnityEngine.Color.green;

            _onEntryAlphaValue = 0;
            _leftAlphaValue = 1;
            _inAlphaValue = 1;
            _rightAlphaValue = 1;
            _onExitAlphaValue = 1;
        }

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="recorder"></param>
        /// <param name="boolValue"></param>
        /// <param name="lifecycleEventLite"></param>
        protected override void OnSetPercent(Recorder recorder, EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate)
        {
            if (_operationType == EOperationType.Enable)
            {
                foreach (var info in recorder._records)
                {
                    info.SetEnabled(boolValue);
                }
            }
            else
            {
                switch (lifecycleEventLite)
                {
                    case ELifecycleEventLite.OnEntry:
                        {
                            switch (_operationType)
                            {
                                case EOperationType.Color: SetColor(recorder, onEntry, ref _onEntryColorValue, ref onEntrySwitchFlag); break;
                                case EOperationType.Alpha: SetAlpha(recorder, onEntry, ref _onEntryAlphaValue, ref onEntrySwitchFlag); break;
                                case EOperationType.Material: SetMaterial(recorder, onEntry, ref _onEntryMaterialValue, ref onEntrySwitchFlag); break;
                            }
                            break;
                        }
                    case ELifecycleEventLite.OnExit:
                        {
                            switch (_operationType)
                            {
                                case EOperationType.Color: SetColor(recorder, onExit, ref _onExitColorValue, ref onExitSwitchFlag); break;
                                case EOperationType.Alpha: SetAlpha(recorder, onExit, ref _onExitAlphaValue, ref onExitSwitchFlag); break;
                                case EOperationType.Material: SetMaterial(recorder, onExit, ref _onExitMaterialValue, ref onExitSwitchFlag); break;
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="recorder"></param>
        /// <param name="percent"></param>
        protected override void OnSetPercent(Recorder recorder, Percent percent)
        {
            if (percent.leftRange)
            {
                switch (_operationType)
                {
                    case EOperationType.Color: SetColor(recorder, leftRange, ref _leftColorValue, ref leftSwitchFlag); break;
                    case EOperationType.Alpha: SetAlpha(recorder, leftRange, ref _leftAlphaValue, ref leftSwitchFlag); break;
                    case EOperationType.Material: SetMaterial(recorder, leftRange, ref _leftMaterialValue, ref leftSwitchFlag); break;
                    case EOperationType.Enable: OnSetPercent(recorder, leftRange); break;
                }
            }
            else if (percent.rightRange)
            {
                switch (_operationType)
                {
                    case EOperationType.Color: SetColor(recorder, rightRange, ref _rightColorValue, ref rightSwitchFlag); break;
                    case EOperationType.Alpha: SetAlpha(recorder, rightRange, ref _rightAlphaValue, ref rightSwitchFlag); break;
                    case EOperationType.Material: SetMaterial(recorder, rightRange, ref _rightMaterialValue, ref rightSwitchFlag); break;
                    case EOperationType.Enable: OnSetPercent(recorder, rightRange); break;
                }
            }
            else
            {
                switch (_operationType)
                {
                    case EOperationType.Color: SetColor(recorder, inRange, ref _inColorValue, ref inSwitchFlag); break;
                    case EOperationType.Alpha: SetAlpha(recorder, inRange, ref _inAlphaValue, ref inSwitchFlag); break;
                    case EOperationType.Material: SetMaterial(recorder, inRange, ref _inMaterialValue, ref inSwitchFlag); break;
                    case EOperationType.Enable: OnSetPercent(recorder, inRange); break;
                }
            }
        }
    }
}
