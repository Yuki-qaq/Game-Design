using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using UnityEngine.XR;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginXRSpaceSolution.Base;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 基础模拟提供者：模拟控制器的输入输出的基类
    /// </summary>
    [Name("基础模拟提供者")]
    [Tip("模拟控制器的输入输出的基类", "Base class for analog controller input and output")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [XCSJ.Attributes.Icon(EIcon.Click)]
    public abstract class BaseAnalogProvider : Interactor, IAnalogProvider
    {
        /// <summary>
        /// 模拟控制器
        /// </summary>
        [Name("模拟控制器")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AnalogController _analogController;

        /// <summary>
        /// 模拟控制器
        /// </summary>
        public AnalogController analogController => this.XGetComponentInParent(ref _analogController);

        /// <summary>
        /// 动作名
        /// </summary>
        public virtual string actionName { get; set; } = "";

        /// <summary>
        /// 注册
        /// </summary>
        public void Regist()
        {
            var analogController = this.analogController;
            if (analogController)
            {
                analogController.Regist(this);
            }
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        public void Unregist()
        {
            var analogController = this.analogController;
            if (analogController)
            {
                analogController.Unregist(this);
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Regist();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            Unregist();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            Regist();
        }
    }

    /// <summary>
    /// 模拟提供者
    /// </summary>
    public interface IAnalogProvider
    {
        /// <summary>
        /// 动作名
        /// </summary>
        string actionName { get; }
    }

    /// <summary>
    /// 姿态提供者
    /// </summary>
    public interface IPoseProvider : IAnalogProvider
    {

#if XDREAMER_XR_INTERACTION_TOOLKIT

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// 尝试获取姿态
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        InputTrackingState TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation);

#else
        /// <summary>
        /// 尝试获取姿态
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        PoseDataFlags TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation);

#endif

#endif
    }

    /// <summary>
    /// 交互提供者
    /// </summary>
    public interface IInteractProvider : IAnalogProvider
    {
        /// <summary>
        /// 是否按下选择键
        /// </summary>
        /// <returns></returns>
        bool IsPressedOfSelect(AnalogController analogController);

        /// <summary>
        /// 是否按下激活键
        /// </summary>
        /// <returns></returns>
        bool IsPressedOfActivate(AnalogController analogController);

        /// <summary>
        /// 是否按下UI键
        /// </summary>
        /// <returns></returns>
        bool IsPressedOfUI(AnalogController analogController);
    }

    /// <summary>
    /// 动作
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// 动作名
        /// </summary>
        string actionName { get; }

        /// <summary>
        /// 尝试获取动作值：布尔类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="actionConfigType"></param>
        /// <returns></returns>
        bool TryGetActionValue(out bool value, EActionConfigType actionConfigType = EActionConfigType.Button_InteractSelect);

        /// <summary>
        /// 尝试获取动作值：浮点数类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="actionConfigType"></param>
        /// <returns></returns>
        bool TryGetActionValue(out float value, EActionConfigType actionConfigType = EActionConfigType.Analog_NX);
    }

    /// <summary>
    /// 动作提供者
    /// </summary>
    public interface IActionProvider : IAnalogProvider
    {
        /// <summary>
        /// 尝试获取动作值：布尔类型
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="actionName"></param>
        /// <param name="value"></param>
        /// <param name="actionConfigType"></param>
        /// <returns></returns>
        bool TryGetActionValue(AnalogController analogController, string actionName, out bool value, EActionConfigType actionConfigType = EActionConfigType.Button_InteractSelect);

        /// <summary>
        /// 尝试获取动作值：浮点数类型
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="actionName"></param>
        /// <param name="value"></param>
        /// <param name="actionConfigType"></param>
        /// <returns></returns>
        bool TryGetActionValue(AnalogController analogController, string actionName, out float value, EActionConfigType actionConfigType = EActionConfigType.Analog_NX);
    }

    /// <summary>
    /// 触觉脉冲提供者
    /// </summary>
    public interface IHapticImpulseProvider : IAnalogProvider
    {
        /// <summary>
        /// 发送触觉脉冲
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="amplitude">播放脉冲的振幅（从0.0到1.0）</param>
        /// <param name="duration">播放触觉脉冲的持续时间（秒）</param>
        /// <returns></returns>
        bool SendHapticImpulse(AnalogController analogController, float amplitude, float duration);
    }
}
