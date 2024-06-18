using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络UGUI对象
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.UIEvent)]
    [DisallowMultipleComponent]
    [Name("网络UGUI对象")]
    [Tool(MMOHelper.CategoryName, nameof(NetIdentity), rootType = typeof(MMOManager))]
    public class NetUGUI : NetMB
    {
        /// <summary>
        /// UGUI类型
        /// </summary>
        [Name("UGUI类型")]
        [EnumPopup]
        public EUGUIType _uguiType = EUGUIType.Toggle;

        #region Toggle

        /// <summary>
        /// Toggle切换
        /// </summary>
        [Name("Toggle切换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_uguiType), EValidityCheckType.NotEqual, EUGUIType.Toggle)]
        public Toggle _toggle;

        /// <summary>
        /// Toggle状态
        /// </summary>
        [SyncVar]
        [Readonly]
        [Name("Toggle状态")]
        public bool _isOn = false;

        /// <summary>
        /// 上一次Toggle状态
        /// </summary>
        [Readonly]
        [Name("上一次Toggle状态")]
        public bool _lastIsOn = false;

        /// <summary>
        /// 原始Toggle状态
        /// </summary>
        [Readonly]
        [Name("原始Toggle状态")]
        public bool _originalIsOn = false;

        #endregion

        #region DropDown

        /// <summary>
        /// DropDown下拉列表
        /// </summary>
        [Name("DropDown下拉列表")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_uguiType), EValidityCheckType.NotEqual, EUGUIType.DropDown)]
        public Dropdown _dropDown = null;

        /// <summary>
        /// 索引
        /// </summary>
        [SyncVar]
        [Readonly]
        [Name("索引")]
        public int _index = 0;

        /// <summary>
        /// 上一次索引
        /// </summary>
        [Readonly]
        [Name("上一次索引")]
        public int _lastIndex = 0;

        /// <summary>
        /// 原始索引
        /// </summary>
        [Readonly]
        [Name("原始索引")]
        public int _originalIndex = 0;

        #endregion

        #region Slider

        /// <summary>
        /// Slider滑动条
        /// </summary>
        [Name("Slider滑动条")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_uguiType), EValidityCheckType.NotEqual, EUGUIType.Slider)]
        public Slider _slider = null;

        /// <summary>
        /// 滑动条值
        /// </summary>
        [SyncVar]
        [Readonly]
        [Name("滑动条值")]
        public float _sliderValue = 0;

        /// <summary>
        /// 上一次滑动条值
        /// </summary>
        [Readonly]
        [Name("上一次滑动条值")]
        public float _lastSliderValue = 0;

        /// <summary>
        /// 原始滑动条值
        /// </summary>
        [Readonly]
        [Name("原始滑动条值")]
        public float _originalSliderValue = 0;

        #endregion

        /// <summary>
        /// 重置：默认在所有游戏对象上查找Toggle，并进行初始化
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _toggle = GetComponent<Toggle>();
        }

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            base.OnMMOEnterRoomCompleted(result);

            if (!dataValid) return;

            switch (_uguiType)
            {
                case EUGUIType.Toggle: _originalIsOn = _lastIsOn = _isOn = _toggle.isOn; break;
                case EUGUIType.DropDown: _originalIndex = _lastIndex = _index = _dropDown.value; break;
                case EUGUIType.Slider: _originalSliderValue = _lastSliderValue = _sliderValue = _slider.value; break;
                default: break;
            }
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            base.OnMMOExitRoomCompleted();

            if (!dataValid) return;

            switch (_uguiType)
            {
                case EUGUIType.Toggle: _toggle.isOn = _originalIsOn; break;
                case EUGUIType.DropDown: _dropDown.value = _originalIndex; break;
                case EUGUIType.Slider: _slider.value = _originalSliderValue; break;
                default: break;
            }
        }

        /// <summary>
        /// 当定时检查修改
        /// </summary>
        /// <returns></returns>
        protected override bool OnTimedCheckChange()
        {
            if (!dataValid) return false;

            switch (_uguiType)
            {
                case EUGUIType.Toggle:
                    {
                        _isOn = _toggle.isOn;
                        return _isOn != _lastIsOn;
                    }
                case EUGUIType.DropDown:
                    {
                        _index = _dropDown.value;
                        return _index != _lastIndex;
                    }
                case EUGUIType.Slider:
                    {
                        _sliderValue = _slider.value;
                        return _sliderValue != _lastSliderValue;
                    }
                default: return false;
            }
        }

        /// <summary>
        /// 当同步变量已修改
        /// </summary>
        protected override void OnSyncVarChanged()
        {
            base.OnSyncVarChanged();

            if (!dataValid) return;

            switch (_uguiType)
            {
                case EUGUIType.Toggle:
                    {
                        _toggle.isOn = _isOn;
                        _lastIsOn = _isOn;
                        break;
                    }
                case EUGUIType.DropDown:
                    {
                        _dropDown.value = _index;
                        _lastIndex = _index;
                        break;
                    }
                case EUGUIType.Slider:
                    {
                        _slider.value = _sliderValue;
                        _lastSliderValue = _sliderValue;
                        break;
                    }
                default:
                    break;
            }
        }

        private bool dataValid
        {
            get
            {
                switch (_uguiType)
                {
                    case EUGUIType.Toggle: return _toggle;
                    case EUGUIType.DropDown: return _dropDown;
                    case EUGUIType.Slider: return _slider;
                    default:return false;
                }
            }
        }

        /// <summary>
        /// UGUI类型
        /// </summary>
        [Name("UGUI类型")]
        public enum EUGUIType
        {
            /// <summary>
            /// 切换
            /// </summary>
            [Name("切换")]
            Toggle,

            /// <summary>
            /// 下拉框
            /// </summary>
            [Name("下拉框")]
            DropDown,

            /// <summary>
            /// 滑动条
            /// </summary>
            [Name("滑动条")]
            Slider,
        }
    }
}

