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
    /// TMP文本视图
    /// </summary>
    [Name("TMP文本视图")]
    [Tip("TextMeshPro插件的文本视图", "Text view of TextMeshPro plugin")]
    [XCSJ.Attributes.Icon(EIcon.Text)]
    [Tool(XGUICategory.DataView, rootType = typeof(XGUIManager))]
    public class TMPTextView :
#if XDREAMER_TEXTMESHPRO
        BaseUIView<TMP_Text>
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

        /// <summary>
        /// 文本处理器
        /// </summary>
        [Name("文本处理器")]
        [HideInSuperInspector(nameof(_viewMember), EValidityCheckType.NotEqual, EViewMember.Text)]
        public TextHandler _textHandler = new TextHandler();

#if XDREAMER_TEXTMESHPRO

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            viewOnEnable = view;
            if (viewOnEnable)
            {
                viewOnEnable.RegisterDirtyVerticesCallback(OnTextChanged);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (viewOnEnable)
            {
                viewOnEnable.UnregisterDirtyVerticesCallback(OnTextChanged);
            }
        }

        private void OnTextChanged() => viewOnEnable.text = _textHandler.Handle(viewOnEnable.text);

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