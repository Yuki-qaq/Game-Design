using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXRSpaceSolution.Base;

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 交互通过键码:通过键码模拟控制器交互输入输出
    /// </summary>
    [Name("交互通过键码")]
    [Tip("通过键码模拟控制器交互的输入输出", "Simulate the input and output of controller interaction through key code")]
    [Tool(XRITHelper.InteractInput, nameof(AnalogController))]
    [XCSJ.Attributes.Icon(EIcon.Keyboard)]
    public class InteractByKeyCode : BaseAnalogProvider, IInteractProvider, IActionProvider
    {
        /// <summary>
        /// 动作名
        /// </summary>
        [Name("动作名")]
        [ActionNamePopup]
        public string _actionName;

        /// <summary>
        /// 动作名
        /// </summary>
        public override string actionName { get => _actionName; set => _actionName = value; }

        /// <summary>
        /// 选择设置
        /// </summary>
        [Name("选择设置")]
        public KeyCodesSetting _settingOfSelect = new KeyCodesSetting();

        /// <summary>
        /// 激活设置
        /// </summary>
        [Name("激活设置")]
        public KeyCodesSetting _settingOfActivate = new KeyCodesSetting();

        /// <summary>
        /// UI设置
        /// </summary>
        [Name("UI设置")]
        public KeyCodesSetting _settingOfUI = new KeyCodesSetting();

        /// <summary>
        /// 负X设置
        /// </summary>
        [Name("负X设置")]
        public KeyCodesSetting _nx = new KeyCodesSetting();

        /// <summary>
        /// 正X设置
        /// </summary>
        [Name("正X设置")]
        public KeyCodesSetting _px = new KeyCodesSetting();

        /// <summary>
        /// 负Y设置
        /// </summary>
        [Name("负Y设置")]
        public KeyCodesSetting _ny = new KeyCodesSetting();

        /// <summary>
        /// 正Y设置
        /// </summary>
        [Name("正Y设置")]
        public KeyCodesSetting _py = new KeyCodesSetting();

        /// <summary>
        /// 负Z设置
        /// </summary>
        [Name("负Z设置")]
        public KeyCodesSetting _nz = new KeyCodesSetting();

        /// <summary>
        /// 正Z设置
        /// </summary>
        [Name("正Z设置")]
        public KeyCodesSetting _pz = new KeyCodesSetting();

        /// <inheritdoc/>
        public bool IsPressedOfSelect(AnalogController analogController) => _settingOfSelect.IsPressed();

        /// <inheritdoc/>
        public bool IsPressedOfActivate(AnalogController analogController) => _settingOfActivate.IsPressed();

        /// <inheritdoc/>
        public bool IsPressedOfUI(AnalogController analogController) => _settingOfUI.IsPressed();

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.XModifyProperty(() =>
            {
                _settingOfSelect._keyCode = KeyCode.Space;
                _settingOfActivate._keyCode = KeyCode.Alpha1;
                _settingOfUI._keyCode = KeyCode.Space;
            });
        }

        /// <summary>
        /// 尝试获取动作值
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="actionName"></param>
        /// <param name="value"></param>
        /// <param name="actionConfigType"></param>
        /// <returns></returns>
        public bool TryGetActionValue(AnalogController analogController, string actionName, out bool value, EActionConfigType actionConfigType = EActionConfigType.Button_InteractSelect)
        {
            if (actionName == this.actionName)
            {
                switch (actionConfigType)
                {
                    case EActionConfigType.Button_Interact:
                        {
                            value = _settingOfSelect.IsPressed() 
                                || _settingOfActivate.IsPressed()
                                || _settingOfUI.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = _settingOfSelect.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = _settingOfActivate.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = _settingOfUI.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Analog_XYZ:
                        {
                            value = _nx.IsPressed() 
                                || _px.IsPressed()
                                || _ny.IsPressed()
                                || _py.IsPressed()
                                || _nz.IsPressed()
                                || _pz.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Analog_NX:
                        {
                            value = _nx.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Analog_PX:
                        {
                            value = _px.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Analog_NY:
                        {
                            value = _ny.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Analog_PY:
                        {
                            value = _py.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Analog_NZ:
                        {
                            value = _nz.IsPressed();
                            return true;
                        }
                    case EActionConfigType.Analog_PZ:
                        {
                            value = _pz.IsPressed();
                            return true;
                        }
                }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试获取动作值
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="actionName"></param>
        /// <param name="value"></param>
        /// <param name="actionConfigType"></param>
        /// <returns></returns>
        public bool TryGetActionValue(AnalogController analogController, string actionName, out float value, EActionConfigType actionConfigType = EActionConfigType.Analog_NX)
        {
            if (actionName == this.actionName)
            {
                switch (actionConfigType)
                {
                    case EActionConfigType.Button_Interact:
                        {
                            value = _settingOfSelect.IsPressed()
                                || _settingOfActivate.IsPressed()
                                || _settingOfUI.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = _settingOfSelect.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = _settingOfActivate.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = _settingOfUI.IsPressed() ? 1 : 0;
                            return true;
                        }

                    case EActionConfigType.Analog_XYZ:
                        {
                            value = _nx.IsPressed()
                                || _px.IsPressed()
                                || _ny.IsPressed()
                                || _py.IsPressed()
                                || _nz.IsPressed()
                                || _pz.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Analog_NX:
                        {
                            value = _nx.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Analog_PX:
                        {
                            value = _px.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Analog_NY:
                        {
                            value = _ny.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Analog_PY:
                        {
                            value = _py.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Analog_NZ:
                        {
                            value = _nz.IsPressed() ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Analog_PZ:
                        {
                            value = _pz.IsPressed() ? 1 : 0;
                            return true;
                        }
                }
            }
            value = default;
            return false;
        }
    }
}
