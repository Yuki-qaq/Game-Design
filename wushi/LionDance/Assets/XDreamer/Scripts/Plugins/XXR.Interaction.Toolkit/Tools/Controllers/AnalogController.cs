using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using System;
using System.Linq;
using XCSJ.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using XCSJ.PluginTools.Inputs;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Base;
using XCSJ.PluginXRSpaceSolution.Base;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 模拟控制器
    /// </summary>
    [Name("模拟控制器")]
    [RequireManager(typeof(XXRInteractionToolkitManager), typeof(ToolsManager))]
    public class AnalogController
#if XDREAMER_XR_INTERACTION_TOOLKIT
        : XRBaseController
#else
        : InteractProvider
#endif
    {
        /// <summary>
        /// 姿态动作名
        /// </summary>
        [Header("跟踪器姿态")]
        [Name("姿态动作名")]
        [ActionNamePopup]
        public string _poseActionName = "";

        /// <summary>
        /// 姿态提供者
        /// </summary>
        [Name("姿态提供者")]
        [Tip("跟踪器姿态提供者", "Tracker pose provider")]
        [ComponentPopup(typeof(IPoseProvider), searchFlags = ESearchFlags.Default, overrideLabel = true)]
        public BaseAnalogProvider _poseProvider;

        /// <summary>
        /// 交互动作名
        /// </summary>
        [Header("标准交互输入")]
        [Name("交互动作名")]
        [ActionNamePopup]
        public List<string> _interactActionNames = new List<string>();

        /// <summary>
        /// 交互提供者
        /// </summary>
        [Name("交互提供者")]
        [Tip("交互提供者", "Interact provider")]
        [ComponentPopup(typeof(IInteractProvider), searchFlags = ESearchFlags.Default, overrideLabel = true)]
        public List<BaseAnalogProvider> _interactProviders = new List<BaseAnalogProvider>();

        /// <summary>
        /// 模拟鼠标输入
        /// </summary>
        [Name("模拟鼠标输入")]
        [ComponentPopup]
        public AnalogMouseInput _analogMouseInput;

        /// <summary>
        /// 模拟鼠标输入
        /// </summary>
        public AnalogMouseInput analogMouseInput => this.XGetComponent(ref _analogMouseInput);

        /// <summary>
        /// 自定义动作名
        /// </summary>
        [Header("自定义交互输入")]
        [Name("自定义动作名")]
        [ActionNamePopup]
        public List<string> _customActionNames = new List<string>();

        /// <summary>
        /// 动作提供者
        /// </summary>
        [Name("动作提供者")]
        [Tip("动作提供者", "Action Provider")]
        [ComponentPopup(typeof(IActionProvider), searchFlags = ESearchFlags.Default, overrideLabel = true)]
        public List<BaseAnalogProvider> _customActionProviders = new List<BaseAnalogProvider>();

        /// <summary>
        /// 模拟键码输入
        /// </summary>
        [Name("模拟键码输入")]
        [ComponentPopup]
        public AnalogKeyCodeInput _analogKeyCodeInput;

        /// <summary>
        /// 模拟键码输入
        /// </summary>
        public AnalogKeyCodeInput analogKeyCodeInput => this.XGetComponent(ref _analogKeyCodeInput);

        /// <summary>
        /// 触觉脉冲提供者
        /// </summary>
        [Header("其他")]
        [Name("触觉脉冲提供者")]
        [Tip("触觉脉冲提供者", "Haptic Impulse Provider")]
        [ComponentPopup(typeof(IHapticImpulseProvider), searchFlags = ESearchFlags.Default, overrideLabel = true)]
        public List<BaseAnalogProvider> _hapticImpulseProviders = new List<BaseAnalogProvider>();

        /// <summary>
        /// 确保模拟鼠标输入组件的有效性
        /// </summary>
        private void EnsureAnalogMouseInputValid()
        {
            if (!analogMouseInput) _analogMouseInput = this.XGetOrAddComponent<AnalogMouseInput>();
        }

        /// <summary>
        /// 确保模拟键码输入组件的有效性
        /// </summary>
        private void EnsureAnalogKeyCodeInputValid()
        {
            if (!analogKeyCodeInput) _analogKeyCodeInput = this.XGetOrAddComponent<AnalogKeyCodeInput>();
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected void OnDestroy()
        {
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            EnsureValid();
            EnsureAnalogMouseInputValid();
            EnsureAnalogKeyCodeInputValid();
        }

#endif

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            EnsureAnalogMouseInputValid();
            EnsureAnalogKeyCodeInputValid();

            var providers = GetComponents<BaseAnalogProvider>();
            this.XModifyProperty(() =>
            {
                foreach (var p in providers) Regist(p);
            });
            EnsureValid();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public void Regist<T>(T provider) where T : BaseAnalogProvider
        {
            if (!provider) return;
            if (provider is IPoseProvider poseProvider && poseProvider.actionName == _poseActionName)
            {
                if (_poseProvider == provider) { }
                else if (!_poseProvider) _poseProvider = provider;
                else
                {
                    Debug.LogWarningFormat("游戏对象[{0}]上模拟控制器组件，已注册有效姿态提供者[{1}]组件,无法再注册[{2}]组件！",
                        CommonFun.GameObjectToString(gameObject),
                        CommonFun.GameObjectComponentToString(_poseProvider),
                        CommonFun.GameObjectComponentToString(provider));
                }
            }
            if (provider is IInteractProvider interactProvider && _interactActionNames.Contains(interactProvider.actionName))
            {
                _interactProviders.AddWithDistinct(provider);
            }
            if (provider is IHapticImpulseProvider)
            {
                _hapticImpulseProviders.AddWithDistinct(provider);
            }
            if (provider is IActionProvider actionProvider && _customActionNames.Contains(actionProvider.actionName))
            {
                _customActionProviders.AddWithDistinct(provider);
            }
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public void Unregist<T>(T provider) where T : BaseAnalogProvider
        {
            if (!provider) return;
            if (provider is IPoseProvider)
            {
                if (provider == _poseProvider) _poseProvider = default;
            }
            if (provider is IInteractProvider)
            {
                _interactProviders.Remove(provider);
            }
            if (provider is IHapticImpulseProvider)
            {
                _hapticImpulseProviders.Remove(provider);
            }
            if (provider is IActionProvider)
            {
                _customActionProviders.Remove(provider);
            }
        }

        private void EnsureValid()
        {
            if (_poseProvider && !(_poseProvider is IPoseProvider))
            {
                this.XModifyProperty(ref _poseProvider, default);
            }

            if (_interactProviders.Any(p => !p || !(p is IInteractProvider)))
            {
                this.XModifyProperty(() => _interactProviders.RemoveAll(p => !p || !(p is IInteractProvider)));
            }

            if (_hapticImpulseProviders.Any(p => !p || !(p is IHapticImpulseProvider)))
            {
                this.XModifyProperty(() => _hapticImpulseProviders.RemoveAll(p => !p || !(p is IHapticImpulseProvider)));
            }

            if (_customActionProviders.Any(p => !p || !(p is IActionProvider)))
            {
                this.XModifyProperty(() => _customActionProviders.RemoveAll(p => !p || !(p is IActionProvider)));
            }
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT

        /// <summary>
        /// 更新跟踪输入：更新当前模拟控制器的姿态，即位置与旋转
        /// </summary>
        /// <param name="controllerState"></param>
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            if (controllerState == null) return;

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            if (_poseProvider is IPoseProvider provider)
            {
                controllerState.inputTrackingState = provider.TryGetPose(this, out controllerState.position, out controllerState.rotation);
            }
            else
            {
                controllerState.inputTrackingState = InputTrackingState.None;
            }
#else
            if (_poseProvider is IPoseProvider provider)
            {
                controllerState.poseDataFlags = provider.TryGetPose(this, out controllerState.position, out controllerState.rotation);
            }
            else
            {
                controllerState.poseDataFlags = PoseDataFlags.NoData;
            }
#endif
        }

        /// <summary>
        /// 更新输入：更新交互输入，包括选择，激活，UI交互
        /// </summary>
        /// <param name="controllerState"></param>
        protected override void UpdateInput(XRControllerState controllerState)
        {
            var active = false;
            var select = false;
            var ui = false;

            foreach (var p in _interactProviders)
            {
                if (p is IInteractProvider provider)
                {
                    active = active || provider.IsPressedOfActivate(this);
                    select = select || provider.IsPressedOfSelect(this);
                    ui = ui || provider.IsPressedOfUI(this);
                }
            }

            controllerState.ResetFrameDependentStates();
            controllerState.activateInteractionState.SetFrameState(active);
            controllerState.selectInteractionState.SetFrameState(select);
            controllerState.uiPressInteractionState.SetFrameState(ui);

            if (_analogMouseInput) _analogMouseInput.Analog(select, active);
            if (_analogKeyCodeInput)
            {
                _analogKeyCodeInput.Analog(actionName => _customActionProviders.Any(p => p is IActionProvider provider && provider.TryGetActionValue(this, actionName, out bool pressed) && pressed));
            }
        }

        /// <summary>
        /// 发送触觉脉冲
        /// </summary>
        /// <param name="amplitude">播放脉冲的振幅（从0.0到1.0）</param>
        /// <param name="duration">播放触觉脉冲的持续时间（秒）</param>
        /// <returns></returns>
        public override bool SendHapticImpulse(float amplitude, float duration)
        {
            var send = false;
            foreach (var p in _hapticImpulseProviders)
            {
                if (p is IHapticImpulseProvider provider)
                {
                    send = provider.SendHapticImpulse(this, amplitude, duration) || send;
                }
            }
            return send;
        }

#endif
    }
}
