using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.XUnityEngine.XEvents;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;

#if XDREAMER_PICO
using Unity.XR.PXR;
#endif

namespace XCSJ.PluginPico.Tools
{
    /// <summary>
    /// 交互通过PICO手势
    /// </summary>
    [Name("交互通过PICO手势")]
    [Tip("通过PICO手势模拟交互的输入输出", "Input and output through PICO hand pose simulation interaction")]
    [Tool(XRITHelper.InteractInput, nameof(AnalogController))]
    [Tool(PicoHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    public class InteractByPICOHandPose : BaseAnalogProvider, IInteractProvider
    {
#if XDREAMER_PICO

        /// <summary>
        /// 选择手势
        /// </summary>
        [Name("选择手势")]
        public PXR_HandPose _handPoseOfSelect;

        /// <summary>
        /// 激活手势
        /// </summary>
        [Name("激活手势")]
        public PXR_HandPose _handPoseOfActivate;

        /// <summary>
        /// UI手势
        /// </summary>
        [Name("UI手势")]
        public PXR_HandPose _handPoseOfUI;

        private PXR_HandPose handPoseOfSelectOnEnable;
        private PXR_HandPose handPoseOfActivateOnEnable;
        private PXR_HandPose handPoseOfUIOnEnable;
#endif

        private bool _active = false;
        private bool _selected = false;
        private bool _ui = false;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

#if XDREAMER_PICO
            if (_handPoseOfSelect)
            {
                handPoseOfSelectOnEnable = _handPoseOfSelect;
                handPoseOfSelectOnEnable.handPoseStart.AddCall(OnSelectHandPoseStart);
                handPoseOfSelectOnEnable.handPoseEnd.AddCall(OnSelectHandPoseEnd);
            }

            if (_handPoseOfActivate)
            {
                handPoseOfActivateOnEnable = _handPoseOfActivate;
                handPoseOfActivateOnEnable.handPoseStart.AddCall(OnActivateHandPoseStart);
                handPoseOfActivateOnEnable.handPoseEnd.AddCall(OnActivateHandPoseEnd);
            }

            if (_handPoseOfUI)
            {
                handPoseOfUIOnEnable = _handPoseOfUI;
                handPoseOfUIOnEnable.handPoseStart.AddCall(OnUIHandPoseStart);
                handPoseOfUIOnEnable.handPoseEnd.AddCall(OnUIHandPoseEnd);
            }
#endif
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

#if XDREAMER_PICO
            if (handPoseOfSelectOnEnable)
            {
                handPoseOfSelectOnEnable.handPoseStart.RemoveCall(OnSelectHandPoseStart);
                handPoseOfSelectOnEnable.handPoseEnd.RemoveCall(OnSelectHandPoseEnd);
            }
            
            if (handPoseOfActivateOnEnable)
            {
                handPoseOfActivateOnEnable.handPoseStart.RemoveCall(OnActivateHandPoseStart);
                handPoseOfActivateOnEnable.handPoseEnd.RemoveCall(OnActivateHandPoseEnd);
            }

            if (handPoseOfUIOnEnable)
            {
                handPoseOfUIOnEnable.handPoseStart.RemoveCall(OnUIHandPoseStart);
                handPoseOfUIOnEnable.handPoseEnd.RemoveCall(OnUIHandPoseEnd);
            }
#endif
        }

        private void OnSelectHandPoseStart() => _selected = true;
        private void OnSelectHandPoseEnd() => _selected = false;

        private void OnActivateHandPoseStart() => _active = true;
        private void OnActivateHandPoseEnd() => _active = false;

        private void OnUIHandPoseStart() => _ui = true;
        private void OnUIHandPoseEnd() => _ui = false;

        /// <summary>
        /// 选择按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfSelect(AnalogController analogController) => _selected;

        /// <summary>
        /// 激活按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfActivate(AnalogController analogController) => _active;

        /// <summary>
        /// UI按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfUI(AnalogController analogController) => _ui;
    }
}
