using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginART.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginXRSpaceSolution.Base;

namespace XCSJ.PluginART.Tools
{
    /// <summary>
    /// 交互通过ART
    /// </summary>
    [Name("交互通过ART")]
    [Tip("通过ART模拟控制器交互的输入输出", "Input and output of controller interaction through ART simulation")]
    [RequireManager(typeof(ARTManager), typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.InteractInput, nameof(AnalogController))]
    [Tool(ARTHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [Owner(typeof(ARTManager))]
    public class InteractByART : BaseAnalogProvider, IInteractProvider, IHapticImpulseProvider, IARTFlystickObject, IActionProvider
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
        /// ART流客户端:用于与ART进行数据流通信的ART流客户端
        /// </summary>
        [Name("ART流客户端")]
        [Tip("用于与ART进行数据流通信的ART流客户端", "ART stream client for data stream communication with ART")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ARTStreamClient _streamClient;

        /// <summary>
        /// ART流客户端
        /// </summary>
        public ARTStreamClient streamClient => this.XGetComponentInGlobal(ref _streamClient, true);

        /// <summary>
        /// 数据类型
        /// </summary>
        public EDataType dataType { get => EDataType.FlyStick; set { } }

        /// <summary>
        /// 刚体ID
        /// </summary>
        public int rigidBodyID { get => flysitckID; set => flysitckID = value; }

        /// <summary>
        /// 拥有者
        /// </summary>
        public IARTObjectOwner owner => this.GetARTObjectOwner();

        IComponentOwner IHasOwner<IComponentOwner>.owner => owner;

        IOwner IHasOwner.owner => owner;

        /// <summary>
        /// Flystick编号
        /// </summary>
        public int flysitckID
        {
            get => _buttonOfActivate._flysitckID;
            set
            {
                if (_buttonOfActivate._flysitckID != value
                    || _buttonOfSelect._flysitckID != value
                    || _buttonOfUI._flysitckID != value)
                {
                    this.XModifyProperty(() =>
                    {
                        _buttonOfActivate._flysitckID = value;
                        _buttonOfSelect._flysitckID = value;
                        _buttonOfUI._flysitckID = value;
                    });
                }
            }
        }

        /// <summary>
        /// Flystick手柄
        /// </summary>
        public EFlystick flystick
        {
            get => _buttonOfActivate._flystick;
            set
            {
                if (_buttonOfActivate._flystick != value
                    || _buttonOfSelect._flystick != value
                    || _buttonOfUI._flystick != value)
                {
                    this.XModifyProperty(() =>
                    {
                        _buttonOfActivate._flystick = value;
                        _buttonOfSelect._flystick = value;
                        _buttonOfUI._flystick = value;
                    });
                }
            }
        }

        /// <summary>
        /// 死区
        /// </summary>
        public Vector2 deadZone
        {
            get => _buttonOfActivate._deadZone;
            set
            {
                this.XModifyProperty(() =>
                {
                    _buttonOfActivate._deadZone = value;
                    _buttonOfSelect._deadZone = value;
                    _buttonOfUI._deadZone = value;
                });
            }
        }

        /// <summary>
        /// 选择按钮
        /// </summary>
        [Name("选择按钮")]
        public FlystickButton _buttonOfSelect = new FlystickButton();

        /// <summary>
        /// 选择按钮的Flystick编号
        /// </summary>
        public int flysitckIDOfSelect
        {
            get => _buttonOfSelect._flysitckID;
            set => this.XModifyProperty(ref _buttonOfSelect._flysitckID, value);
        }

        /// <summary>
        /// 激活按钮
        /// </summary>
        [Name("激活按钮")]
        public FlystickButton _buttonOfActivate = new FlystickButton();

        /// <summary>
        /// 激活按钮的Flystick编号
        /// </summary>
        public int flysitckIDOfActivate
        {
            get => _buttonOfActivate._flysitckID;
            set => this.XModifyProperty(ref _buttonOfActivate._flysitckID, value);
        }

        /// <summary>
        /// UI按钮
        /// </summary>
        [Name("UI按钮")]
        public FlystickButton _buttonOfUI = new FlystickButton();

        /// <summary>
        /// UI按钮的Flystick编号
        /// </summary>
        public int flysitckIDOfUI
        {
            get => _buttonOfUI._flysitckID;
            set => this.XModifyProperty(ref _buttonOfUI._flysitckID, value);
        }        

        /// <summary>
        /// 是否按下激活键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfActivate(AnalogController analogController) => _buttonOfActivate.IsPressed(_streamClient);

        /// <summary>
        /// 是否按下选择键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfSelect(AnalogController analogController) => _buttonOfSelect.IsPressed(_streamClient);

        /// <summary>
        /// 是否按下UI键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfUI(AnalogController analogController) => _buttonOfUI.IsPressed(_streamClient);

        /// <summary>
        /// 发送触觉脉冲
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="amplitude"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool SendHapticImpulse(AnalogController analogController, float amplitude, float duration) => false;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (streamClient) { }
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
                            value = _buttonOfActivate.IsPressed(_streamClient)
                                || _buttonOfSelect.IsPressed(_streamClient)
                                || _buttonOfUI.IsPressed(_streamClient);
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = _buttonOfActivate.IsPressed(_streamClient);
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = _buttonOfSelect.IsPressed(_streamClient);
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = _buttonOfUI.IsPressed(_streamClient);
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
                            value = _buttonOfActivate.IsPressed(_streamClient)
                                || _buttonOfSelect.IsPressed(_streamClient)
                                || _buttonOfUI.IsPressed(_streamClient) ? 1 : 0;
                            return true;
                        }
                    case EActionConfigType.Base:
                    case EActionConfigType.Button_InteractSelect:
                        {
                            value = _buttonOfActivate.GetValue(_streamClient);
                            return true;
                        }
                    case EActionConfigType.Button_InteractActivate:
                        {
                            value = _buttonOfSelect.GetValue(_streamClient);
                            return true;
                        }
                    case EActionConfigType.Button_InteractUI:
                        {
                            value = _buttonOfUI.GetValue(_streamClient);
                            return true;
                        }
                }
            }
            value = default;
            return false;
        }
    }
}
