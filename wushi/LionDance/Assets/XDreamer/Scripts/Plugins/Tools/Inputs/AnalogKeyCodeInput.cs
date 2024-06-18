using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Base;

namespace XCSJ.PluginTools.Inputs
{
    /// <summary>
    /// 模拟键码输入
    /// </summary>
    [Name("模拟键码输入")]
    [Tool(ToolsCategory.InteractInput, rootType = typeof(ToolsManager))]
    [XCSJ.Attributes.Icon(EIcon.Keyboard)]
    public class AnalogKeyCodeInput : ComponentInteractor<KeyCodeInput, KeyCodeInputComponentProvider>
    {
        /// <summary>
        /// 可模拟命令名称
        /// </summary>
        public IEnumerable<string> canAnalogCmdNames
        {
            get
            {
                foreach (var keyCodeInput in _componentProvider.GetComponents())
                {
                    foreach (var keyCodeCmd in keyCodeInput._cmds._cmds)
                    {
                        yield return keyCodeCmd.cmdName;
                    }
                }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            foreach (var component in _componentProvider.GetComponents())
            {
                if (component)
                {
                    component.AnalogKeyCodeInput(this, interactKeyCode);
                }
            }
        }

        /// <summary>
        /// 模拟
        /// </summary>
        /// <param name="func"></param>
        public void Analog(Func<string, bool> func)
        {
            if (func == null) return;

            foreach (var keyCodeInput in _componentProvider.GetComponents())
            {
                foreach (var keyCodeCmd in keyCodeInput._cmds._cmds)
                {
                    keyCodeInput.AnalogKeyCodeInput(this, keyCodeCmd.cmdName, func.Invoke(keyCodeCmd.cmdName));
                }
            }
        }

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="cmdName"></param>
        public void TryInteract(string cmdName)
        {
            foreach (var component in _componentProvider.GetComponents())
            {
                if (component)
                {
                    component.TryInteract(cmdName);
                }
            }
        }

        #region 键码按下弹起交互命令

        /// <summary>
        /// 交互系统产生的键码数据, 只记录按下态键码
        /// </summary>
        private HashSet<KeyCode> interactKeyCode = new HashSet<KeyCode>();

        private bool SetKeyCodePressed(InteractData interactData, bool isPressed)
        {
            if (interactData.cmdParam is KeyCode keyCode)
            {
                if (isPressed)
                {
                    interactKeyCode.Add(keyCode);
                }
                else
                {
                    interactKeyCode.Remove(keyCode);
                }
                return true;
            }
            return false;
        }

        private bool SwitchKeyCodePressed(InteractData interactData)
        {
            if (interactData.cmdParam is KeyCode keyCode)
            {
                if (interactKeyCode.Contains(keyCode))
                {
                    interactKeyCode.Remove(keyCode);
                }
                else
                {
                    interactKeyCode.Add(keyCode);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 键码按下
        /// </summary>
        [InteractCmd]
        [Name("键码按下")]
        public void KeyCodePressed(KeyCode keyCode) => base.TryInteract(nameof(KeyCodePressed), keyCode);

        /// <summary>
        /// 键码按下
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(KeyCodePressed))]
        public EInteractResult KeyCodePressed(InteractData interactData) => SetKeyCodePressed(interactData, true) ? EInteractResult.Success : EInteractResult.Fail;

        /// <summary>
        /// 键码弹起
        /// </summary>
        [InteractCmd]
        [Name("键码弹起")]
        public void KeyCodeRelease(KeyCode keyCode) => base.TryInteract(nameof(KeyCodeRelease), keyCode);

        /// <summary>
        /// 键码弹起
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(KeyCodeRelease))]
        public EInteractResult KeyCodeRelease(InteractData interactData) => SetKeyCodePressed(interactData, false) ? EInteractResult.Success : EInteractResult.Fail;

        /// <summary>
        /// 键码按下弹起切换
        /// </summary>
        [InteractCmd]
        [Name("键码按下弹起切换")]
        public void KeyCodeSwitch(KeyCode keyCode) => base.TryInteract(nameof(KeyCodeSwitch), keyCode);

        /// <summary>
        /// 键码按下弹起切换
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(KeyCodeSwitch))]
        public EInteractResult KeyCodeSwitch(InteractData interactData) => SwitchKeyCodePressed(interactData) ? EInteractResult.Success : EInteractResult.Fail;

        #endregion
    }

    /// <summary>
    /// 键码输入组件提供器
    /// </summary>
    [Serializable]
    public class KeyCodeInputComponentProvider : ComponentProvider<KeyCodeInput> { }
}
