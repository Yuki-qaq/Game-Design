using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Collections;
using XCSJ.PluginXGUI.DataViews.UIViews;
using XCSJ.PluginXGUI.DataViews.Base;

#if XDREAMER_TEXTMESHPRO
using TMPro;
#endif

namespace XCSJ.PluginXGUI.DataViews.TMPUIViews
{
    /// <summary>
    /// TMP下拉列表视图
    /// </summary>
    [Name("TMP下拉列表视图")]
    [Tip("TextMeshPro插件的下拉列表视图", "Dropdown view of TextMeshPro plugin")]
    [XCSJ.Attributes.Icon(EIcon.Dropdown)]
    [Tool(XGUICategory.DataView, rootType = typeof(XGUIManager))]
    public class TMPDropdownView :
#if XDREAMER_TEXTMESHPRO
        BaseUIView<TMP_Dropdown>
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
            /// 值
            /// </summary>
            [Name("值")]
            Value,

            /// <summary>
            /// 显示文本
            /// </summary>
            [Name("显示文本")]
            DisplayText,
        }

        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        [EnumPopup]
        public EViewMember _viewMember = EViewMember.Value;

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

        private void OnValueChanged(int value) => ViewToModelIfCanAndTrigger();

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
                    case EViewMember.Value: return typeof(int);
                    case EViewMember.DisplayText: return typeof(string);
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
                        case EViewMember.Value: return view.value;
                        case EViewMember.DisplayText: return view.options[view.value].text;
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
                        case EViewMember.Value: view.value = (int)value; break;
                        case EViewMember.DisplayText:
                            {
                                var str = value as string;
                                var index = view.options.IndexOf(d => d.text == str);
                                if (index >= 0) view.value = index;
                                break;
                            }
                    }
                }
#endif
            }
        }
    }
}