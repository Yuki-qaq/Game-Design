using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXRSpaceSolution.Base;
using static XCSJ.PluginTools.Inputs.KeyCodeInput;
using static XCSJ.PluginTools.Inputs.RayCmd;

namespace XCSJ.PluginTools.Inputs
{
    /// <summary>
    /// 键码输入
    /// </summary>
    [Name("键码输入")]
    [Tool(ToolsCategory.InteractInput, rootType = typeof(ToolsManager))]
    [XCSJ.Attributes.Icon(EIcon.Keyboard)]
    public sealed class KeyCodeInput : RayInput<KeyCodeCmd, KeyCodeCmds>
    {
        #region KeyCode模拟输入

        /// <summary>
        /// 模拟键盘输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keyCodes"></param>
        public void AnalogKeyCodeInput(UnityEngine.Object sender, params KeyCode[] keyCodes)
        {
            var data = new HashSet<KeyCode>();
            foreach (var keyCode in keyCodes)
            {
                data.Add(keyCode);
            };
            AnalogKeyCodeInput(sender, data);
        }

        /// <summary>
        /// 模拟键盘输入
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="keyCode">键码</param>
        /// <param name="pressed">按下</param>
        /// <param name="rayOrgin">射线原点</param>
        /// <param name="rayDirection">射线方向</param>
        public void AnalogKeyCodeInput(UnityEngine.Object sender, KeyCode keyCode, bool pressed, Vector3 rayOrgin, Vector3 rayDirection)
        {
            AnalogKeyCodeInput(sender, keyCode, pressed, new Ray(rayOrgin, rayDirection));
        }

        /// <summary>
        /// 模拟键盘输入
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="keyCode">键码</param>
        /// <param name="pressed">按下</param>
        /// <param name="ray">射线</param>
        public void AnalogKeyCodeInput(UnityEngine.Object sender, KeyCode keyCode, bool pressed, Ray? ray = null)
        {
            if (!sender) return;

            var data = new HashSet<KeyCode>();
            data.Add(keyCode);
            AnalogKeyCodeInput(sender, data, ray);
        }

        /// <summary>
        /// 模拟键盘输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keyCodes"></param>
        /// <param name="ray"></param>
        public void AnalogKeyCodeInput(UnityEngine.Object sender, HashSet<KeyCode> keyCodes, Ray? ray = null)
        {
            if (!sender) return;

            analogKeyCodeDatas.Add((sender, keyCodes, ray));
        }

        private List<(UnityEngine.Object, HashSet<KeyCode>, Ray?)> analogKeyCodeDatas = new List<(UnityEngine.Object, HashSet<KeyCode>, Ray?)>();

        #endregion

        #region 命令名称模拟输入

        /// <summary>
        /// 模拟键码输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cmdName"></param>
        /// <param name="pressed"></param>
        public void AnalogKeyCodeInput(UnityEngine.Object sender, string cmdName, bool pressed)
        {
            analogCmdNameDatas.Add((sender, cmdName, pressed));
        }

        private List<(UnityEngine.Object, string, bool)> analogCmdNameDatas = new List<(UnityEngine.Object, string, bool)>();

        #endregion

        /// <summary>
        /// 检测命令集合是否执行
        /// </summary>
        protected override void OnCheckCmds()
        {
            base.OnCheckCmds();

            analogKeyCodeDatas.Clear();
            analogCmdNameDatas.Clear();
        }

        /// <summary>
        /// 检测命令是否执行
        /// </summary>
        /// <param name="rayCmd"></param>
        protected override void OnCheckCmd(KeyCodeCmd rayCmd)
        {
            base.OnCheckCmd(rayCmd);

            if (rayCmd.CanAnalogInput())
            {
                foreach (var data in analogKeyCodeDatas)
                {
                    if (rayCmd.TryHandleAnalogInput(data, this, out var rayInteractData))
                    {
                        TryInteract(rayInteractData, out _);
                    }
                }
                foreach(var data in analogCmdNameDatas)
                {
                    if (data.Item2 == rayCmd.cmdName)
                    {
                        if (rayCmd.TryHandleAnalogInput(data, this, out var rayInteractData))
                        {
                            TryInteract(rayInteractData, out _);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建交互数据
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactables"></param>
        /// <returns></returns>
        protected override InteractData CreateInteractData(string cmdName, params InteractObject[] interactables)
        {
            return new KeyCodeRayInteractData(cmdName, this, interactables);
        }

        /// <summary>
        /// 创建键码射线交互数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keyCodeCmd"></param>
        /// <param name="ray"></param>
        /// <param name="raycastHit"></param>
        /// <param name="rayMaxDistance"></param>
        /// <param name="layerMask"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        /// <returns></returns>
        protected override RayInteractData CreateRayInteractData(UnityEngine.Object sender, KeyCodeCmd keyCodeCmd, Ray? ray, RaycastHit? raycastHit, float rayMaxDistance, LayerMask layerMask, string cmdName, InteractObject interactor, params InteractObject[] interactables)
        {
            return new KeyCodeRayInteractData(keyCodeCmd._keyCodes, keyCodeCmd.sender, ray, raycastHit, rayMaxDistance, layerMask, cmdName, interactor, interactables);
        }

        /// <summary>
        /// 尝试交互：绕过按键和按压状态检测，此时将输入命令当作命令名称进行处理
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="interactObjects"></param>
        /// <returns></returns>
        public override bool TryInteract(string cmd, params InteractObject[] interactObjects)
        {            
            var keyCodeCmd = _cmds._cmds.Find(item => cmd == item.cmdName);
            if (keyCodeCmd!=null)
            {
                return TryInteract(CreateRayInteractData(this, keyCodeCmd, keyCodeCmd.cmdName, keyCodeCmd.GetRay()), out _);
            }
            return false;
        }

        /// <summary>
        /// 键码射线交互数据
        /// </summary>
        public class KeyCodeRayInteractData : RayInteractData
        {
            /// <summary>
            /// 键码列表
            /// </summary>
            public List<KeyCode> keyCodes { get; private set; } = new List<KeyCode>();

            /// <summary>
            /// 默认构造函数
            /// </summary>
            private KeyCodeRayInteractData() { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="cmdName"></param>
            /// <param name="interactor"></param>
            /// <param name="interactables"></param>
            public KeyCodeRayInteractData(string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, interactor, interactables) { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="keyCode"></param>
            /// <param name="sender"></param>
            /// <param name="ray"></param>
            /// <param name="raycastHit"></param>
            /// <param name="rayMaxDistance"></param>
            /// <param name="layerMask"></param>
            /// <param name="cmdName"></param>
            /// <param name="interactor"></param>
            /// <param name="interactables"></param>
            public KeyCodeRayInteractData(KeyCode keyCode, UnityEngine.Object sender, Ray? ray, RaycastHit? raycastHit, float rayMaxDistance, LayerMask layerMask, string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(sender, ray, raycastHit, rayMaxDistance, layerMask, cmdName, interactor, interactables)
            {
                this.keyCodes.Add(keyCode);
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="keyCodes"></param>
            /// <param name="sender"></param>
            /// <param name="ray"></param>
            /// <param name="raycastHit"></param>
            /// <param name="rayMaxDistance"></param>
            /// <param name="layerMask"></param>
            /// <param name="cmdName"></param>
            /// <param name="interactor"></param>
            /// <param name="interactables"></param>
            public KeyCodeRayInteractData(List<KeyCode> keyCodes, UnityEngine.Object sender, Ray? ray, RaycastHit? raycastHit, float rayMaxDistance, LayerMask layerMask, string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(sender, ray, raycastHit, rayMaxDistance, layerMask, cmdName, interactor, interactables)
            {
                this.keyCodes.AddRange(keyCodes);
            }

            /// <summary>
            /// 创建实例
            /// </summary>
            /// <returns></returns>
            protected override InteractData CreateInstance() => new KeyCodeRayInteractData();

            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="interactData"></param>
            public override void CopyTo(InteractData interactData)
            {
                base.CopyTo(interactData);

                if (interactData is KeyCodeRayInteractData keyCodeRayInteractData)
                {
                    keyCodeRayInteractData.keyCodes.AddRange(keyCodes);
                }
            }
        }

        /// <summary>
        /// 键码命令列表
        /// </summary>
        [Serializable]
        public class KeyCodeCmds : Cmds<KeyCodeCmd> { }

        /// <summary>
        /// 键码命令
        /// </summary>
        [Serializable]
        public class KeyCodeCmd : RayCmd
        {
            /// <summary>
            /// 键码列表
            /// </summary>
            [Name("键码列表")]
            public List<KeyCode> _keyCodes = new List<KeyCode>();

            /// <summary>
            /// 按下
            /// </summary>
            protected override bool Pressed() => _keyCodes.All(kc => XInput.GetKeyDown(kc), false);

            /// <summary>
            /// 保持
            /// </summary>
            protected override bool Keep() => _keyCodes.All(kc => XInput.GetKey(kc), false);

            /// <summary>
            /// 弹起
            /// </summary>
            protected override bool Release() => _keyCodes.All(kc => XInput.GetKeyUp(kc), false);

            /// <summary>
            /// 构造函数
            /// </summary>
            public KeyCodeCmd() { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="keyCodes"></param>
            /// <param name="cmdName"></param>
            /// <param name="uiRule"></param>
            /// <param name="pressState"></param>
            /// <param name="inputMode"></param>
            /// <param name="pressMaxDistance"></param>
            /// <param name="fixedTime"></param>
            public KeyCodeCmd(KeyCode[] keyCodes, string cmdName, EUIRule uiRule = EUIRule.InvalidOnAnyUI, EPressState pressState = EPressState.Pressed, EInputMode inputMode = EInputMode.Standard | EInputMode.Analog, float pressMaxDistance = 10, float fixedTime = 3)
            {
                _keyCodes.AddRangeWithDistinct(keyCodes);
                _cmdName = cmdName;
                _inputMode = inputMode;
                _uiRule = uiRule;
                _triggerState = pressState;
                _pressMaxDistance = pressMaxDistance;
                _fixedTime = fixedTime;
            }


            #region KeyCode模拟输入

            private Dictionary<UnityEngine.Object, AnalogCmd> analogKeyCodeMap = new Dictionary<UnityEngine.Object, AnalogCmd>();

            /// <summary>
            /// 尝试处理交互输入
            /// </summary>
            /// <param name="data"></param>
            /// <param name="keyCodeInput"></param>
            /// <param name="rayInteractData"></param>
            /// <returns></returns>
            public bool TryHandleAnalogInput((UnityEngine.Object, HashSet<KeyCode>, Ray?) data, KeyCodeInput keyCodeInput, out RayInteractData rayInteractData)
            {
                var sender = data.Item1;
                var analogCmd = GetAnalogKeyCode(sender);

                var keyCodeSet = data.Item2;
                var isPressed = _keyCodes.All(kc => keyCodeSet.Contains(kc), false);

                // 更新模拟命令对象的按压状态
                var ray = data.Item3;
                analogCmd.SetPressState(isPressed, ray);

                // 判断是否需要执行交互
                if (analogCmd.NeedInvokeInteract(this))
                {
                    rayInteractData = keyCodeInput.CreateRayInteractData(sender, this, cmdName, ray);

                    return analogCmd.CanInteract(rayInteractData, this);
                }

                rayInteractData = default;
                return false;
            }

            private AnalogCmd GetAnalogKeyCode(UnityEngine.Object sender)
            {
                if (!analogKeyCodeMap.TryGetValue(sender, out var analogCmd))
                {
                    analogKeyCodeMap[sender] = analogCmd = new AnalogCmd();
                }
                return analogCmd;
            }

            #endregion

            #region 命令名称模拟输入

            /// <summary>
            /// 尝试处理模拟输入
            /// </summary>
            /// <param name="data"></param>
            /// <param name="keyCodeInput"></param>
            /// <param name="rayInteractData"></param>
            /// <returns></returns>
            public bool TryHandleAnalogInput((UnityEngine.Object, string, bool) data, KeyCodeInput keyCodeInput, out RayInteractData rayInteractData)
            {
                var sender = data.Item1;
                var analogCmd = GetAnalogCmdName(sender);

                var isPressed = data.Item3;

                // 更新模拟命令对象的按压状态
                analogCmd.SetPressState(isPressed, default);

                // 判断是否需要执行交互
                if (analogCmd.NeedInvokeInteract(this))
                {
                    rayInteractData = keyCodeInput.CreateRayInteractData(sender, this, cmdName);

                    return analogCmd.CanInteract(rayInteractData, this);
                }

                rayInteractData = default;
                return false;
            }


            private Dictionary<UnityEngine.Object, AnalogCmd> analogCmdNameMap = new Dictionary<UnityEngine.Object, AnalogCmd>();

            private AnalogCmd GetAnalogCmdName(UnityEngine.Object sender)
            {
                if (!analogCmdNameMap.TryGetValue(sender, out var analogCmd))
                {
                    analogCmdNameMap[sender] = analogCmd = new AnalogCmd();
                }
                return analogCmd;
            }

            #endregion
        }

        /// <summary>
        /// 重置命令列表
        /// </summary>
        public override void ResetCmds()
        {
            base.ResetCmds();

            CreateDefaultKeyCodeCmd();
        }

        private void CreateDefaultKeyCodeCmd()
        {
            this.XModifyProperty(() =>
            {
                _cmds._cmds.Add(new KeyCodeCmd(new KeyCode[] { KeyCode.Escape }, EnumHelper.GetEnumString(EActionName.MainMenu, EEnumStringType.NameAttribute), EUIRule.None, EPressState.PressedAndReleased));
            });
        }
    }
}
