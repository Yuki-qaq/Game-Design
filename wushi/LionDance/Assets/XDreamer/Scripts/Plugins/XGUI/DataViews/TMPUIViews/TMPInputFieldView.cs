using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.DataViews.UIViews;
using XCSJ.PluginXGUI.DataViews.Base;

#if XDREAMER_TEXTMESHPRO
using TMPro;
#endif

namespace XCSJ.PluginXGUI.DataViews.TMPUIViews
{
    /// <summary>
    /// TMP输入框视图
    /// </summary>
    [Name("TMP输入框视图")]
    [XCSJ.Attributes.Icon(EIcon.InputField)]
    [Tip("TextMeshPro插件的输入框视图", "InputField view of TextMeshPro plugin")]
    [Tool(XGUICategory.DataView, rootType = typeof(XGUIManager))]
    public class TMPInputFieldView :
#if XDREAMER_TEXTMESHPRO
        BaseUIView<TMP_InputField>
#else
        BaseModelView
#endif

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
        }

        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        [EnumPopup]
        public EViewMember _viewMember = EViewMember.Text;

#if XDREAMER_TEXTMESHPRO

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

        private void OnValueChanged(string value) => ViewToModelIfCanAndTrigger();

#endif

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
                }
                return null;
            }
        }

        /// <summary>
        /// 视图数据值
        /// </summary>
        public override object viewValue
        {
            get
            {
#if XDREAMER_TEXTMESHPRO
                if (view)
                {
                    switch (_viewMember)
                    {
                        case EViewMember.Text: return view.text;
                    }
                }
#endif
                return null;
            }
            set
            {
#if XDREAMER_TEXTMESHPRO
                if (view)
                {
                    switch (_viewMember)
                    {
                        case EViewMember.Text: view.text = value.ToString(); break;
                    }
                }
#endif
            }
        }
    }
}