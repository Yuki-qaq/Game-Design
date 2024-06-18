using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Base;

namespace XCSJ.PluginTools.Inputs
{
    /// <summary>
    /// 鼠标输入组件提供器
    /// </summary>
    [Serializable]
    public class MouseInputComponentProvider : ComponentProvider<MouseInput> { }

    /// <summary>
    /// 模拟鼠标输入
    /// </summary>
    [Name("模拟鼠标输入")]
    [Tool(ToolsCategory.InteractInput, rootType = typeof(ToolsManager))]
    [XCSJ.Attributes.Icon(EIcon.Mouse)]
    public class AnalogMouseInput : AnalogRayInput<MouseInput, MouseInputComponentProvider>
    {
        internal List<AnalogMouseInputSource> analogMouseInputSources = new List<AnalogMouseInputSource>();

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            var left = leftPressed || leftPressedTmp;
            var right = rightPressed || rightPressedTmp;
            leftPressedTmp = false;
            rightPressedTmp = false;
            foreach (var item in analogMouseInputSources)
            {
                if (item.TryGetMouseInput(out var l, out var r))
                {
                    left |= l;
                    right |= r;
                }
            }
            AnalogInternal(left, right);
        }

        private void AnalogInternal(bool leftPressed, bool rightPressed)
        {
            if (_rayGenerater.TryGetRay(out var ray))
            {
                foreach (var component in _componentProvider.GetComponents())
                {
                    if (component)
                    {
                        component.AnalogMouseInput(this, leftPressed, rightPressed, ray);
                    }
                }
            }
        }

        private bool leftPressedTmp = false;
        private bool rightPressedTmp = false;

        /// <summary>
        /// 模拟鼠标左右键
        /// </summary>
        /// <param name="leftPressed"></param>
        /// <param name="rightPressed"></param>
        public void Analog(bool leftPressed, bool rightPressed)
        {
            this.leftPressedTmp = leftPressed;
            this.rightPressedTmp = rightPressed;
        }

        /// <summary>
        /// 左键按下
        /// </summary>
        public bool leftPressed { get; set; } = false;

        /// <summary>
        /// 右键按下
        /// </summary>
        public bool rightPressed { get; set; } = false;

        /// <summary>
        /// 左键按下
        /// </summary>
        [InteractCmd]
        [Name("左键按下")]
        public void LeftPressed() => TryInteract(nameof(LeftPressed));

        /// <summary>
        /// 左键按下
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(LeftPressed))]
        public EInteractResult LeftPressed(InteractData interactData)
        {
            leftPressed = true;
            return EInteractResult.Success;
        }

        /// <summary>
        /// 左键弹起
        /// </summary>
        [InteractCmd]
        [Name("左键弹起")]
        public void LeftRelease() => TryInteract(nameof(LeftRelease));

        /// <summary>
        /// 左键弹起
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(LeftRelease))]
        public EInteractResult LeftRelease(InteractData interactData)
        {
            leftPressed = false;
            return EInteractResult.Success;
        }

        /// <summary>
        /// 左键按下弹起切换
        /// </summary>
        [InteractCmd]
        [Name("左键按下弹起切换")]
        public void LeftSwitch() => TryInteract(nameof(LeftSwitch));

        /// <summary>
        /// 左键按下弹起切换
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(LeftSwitch))]
        public EInteractResult LeftSwitch(InteractData interactData)
        {
            leftPressed = !leftPressed;
            return EInteractResult.Success;
        }

        /// <summary>
        /// 右键按下
        /// </summary>
        [InteractCmd]
        [Name("右键按下")]
        public void RightPressed() => TryInteract(nameof(RightPressed));

        /// <summary>
        /// 右键按下
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(RightPressed))]
        public EInteractResult RightPressed(InteractData interactData)
        {
            rightPressed = true;
            return EInteractResult.Success;
        }

        /// <summary>
        /// 右键弹起
        /// </summary>
        [InteractCmd]
        [Name("右键弹起")]
        public void RightRelease() => TryInteract(nameof(RightRelease));

        /// <summary>
        /// 右键弹起
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(RightRelease))]
        public EInteractResult RightRelease(InteractData interactData)
        {
            rightPressed = false;
            return EInteractResult.Success;
        }

        /// <summary>
        /// 右键按下弹起切换
        /// </summary>
        [InteractCmd]
        [Name("右键按下弹起切换")]
        public void RightSwitch() => TryInteract(nameof(RightSwitch));

        /// <summary>
        /// 右键按下弹起切换
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(RightSwitch))]
        public EInteractResult RightSwitch(InteractData interactData)
        {
            rightPressed = !rightPressed;
            return EInteractResult.Success;
        }

    }

    /// <summary>
    /// 模拟鼠标输入源
    /// </summary>
    [RequireComponent(typeof(AnalogMouseInput))]
    [Tool(ToolsCategory.InteractInput, nameof(AnalogMouseInput), rootType = typeof(ToolsManager))]
    public abstract class AnalogMouseInputSource : InteractProvider
    {
        private AnalogMouseInput _analogMouseInput;

        /// <summary>
        /// 鼠标模拟输入
        /// </summary>
        public AnalogMouseInput analogMouseInput => this.XGetComponent<AnalogMouseInput>(ref _analogMouseInput);

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            analogMouseInput.analogMouseInputSources.Add(this);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            analogMouseInput.analogMouseInputSources.Remove(this);
        }

        /// <summary>
        /// 尝试获取鼠标输入左右键
        /// </summary>
        /// <param name="leftPressed"></param>
        /// <param name="rightPressed"></param>
        /// <returns></returns>
        public abstract bool TryGetMouseInput(out bool leftPressed, out bool rightPressed);
    }
}
