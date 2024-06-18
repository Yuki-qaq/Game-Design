using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using static UnityEngine.UI.InputField;

namespace XCSJ.PluginXGUI.DataViews.UIViews
{
    /// <summary>
    /// 输入框视图
    /// </summary>
    [Name("输入框视图")]
    [XCSJ.Attributes.Icon(EIcon.InputField)]
    [Tool(XGUICategory.DataView, rootType = typeof(XGUIManager), index = 1)]
    public class InputFieldView : BaseUIView<InputField>
    {
        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        public enum EViewMember
        {
            /// <summary>
            /// 文本
            /// </summary>
            [Name("文本")]
            Text,

            /// <summary>
            /// 字符数量限定
            /// </summary>
            [Name("字符数量限定")]
            CharacterLimit,

            /// <summary>
            /// 内容类型
            /// </summary>
            [Name("内容类型")]
            ContentType,

            /// <summary>
            /// 行类型
            /// </summary>
            [Name("行类型")]
            LineType,

            /// <summary>
            /// 选中颜色
            /// </summary>
            [Name("选中颜色")]
            SelectionColor,

            /// <summary>
            /// 移动端隐藏输入键盘
            /// </summary>
            [Name("移动端隐藏输入键盘")]
            HideMobileInput,

            /// <summary>
            /// 只读
            /// </summary>
            [Name("只读")]
            ReadOnly,
        }

        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        [EnumPopup]
        public EViewMember _viewMember = EViewMember.Text;

        /// <summary>
        /// 视图数据类型
        /// </summary>
        public override Type viewValueType
        {
            get
            {
                switch (_viewMember)
                {
                    case EViewMember.Text: return typeof(string);
                    case EViewMember.CharacterLimit: return typeof(int);
                    case EViewMember.ContentType: return typeof(ContentType);
                    case EViewMember.LineType: return typeof(LineType);
                    case EViewMember.SelectionColor: return typeof(Color);
                    case EViewMember.HideMobileInput: return typeof(bool);
                    case EViewMember.ReadOnly: return typeof(bool);
                    default: return null;
                }
            }
        }

        /// <summary>
        /// 视图数据值
        /// </summary>
        public override object viewValue 
        {
            get
            {
                switch (_viewMember)
                {
                    case EViewMember.Text: return view.text;
                    case EViewMember.CharacterLimit: return view.characterLimit;
                    case EViewMember.ContentType: return view.contentType;
                    case EViewMember.LineType: return view.lineType;
                    case EViewMember.SelectionColor: return view.selectionColor;
                    case EViewMember.HideMobileInput: return view.shouldHideMobileInput;
                    case EViewMember.ReadOnly: return view.readOnly;
                    default: return null;
                }
            }
            set
            {
                switch (_viewMember)
                {
                    case EViewMember.Text: view.text = value.ToString(); break;
                    case EViewMember.CharacterLimit: view.characterLimit = (int)value; break;
                    case EViewMember.ContentType: view.contentType = (ContentType)value; break;
                    case EViewMember.LineType: view.lineType = (LineType)value; break;
                    case EViewMember.SelectionColor: view.selectionColor = (Color)value; break;
                    case EViewMember.HideMobileInput: view.shouldHideMobileInput = (bool)value; break;
                    case EViewMember.ReadOnly: view.readOnly = (bool)value; break;
                }
            }
        }

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (viewOnEnable)
            {
                viewOnEnable.onValueChanged.AddListener(OnValueChanged);
            }
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (viewOnEnable)
            {
                viewOnEnable.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        private void OnValueChanged(string value)
        {
            if (_viewMember == EViewMember.Text)
            {
                ViewToModelIfCanAndTrigger();
            }
        }
    }
}