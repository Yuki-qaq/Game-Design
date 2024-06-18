using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.DataViews.UIViews
{
    /// <summary>
    /// 滑动条视图
    /// </summary>
    [Name("滑动条视图")]
    [XCSJ.Attributes.Icon(EIcon.Slider)]
    [Tool(XGUICategory.DataView, rootType = typeof(XGUIManager), index = 1)]
    public class SliderView : BaseUIView<Slider>
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
        }

        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        [EnumPopup]
        public EViewMember _viewMember = EViewMember.Value;

        /// <summary>
        /// 视图数据类型
        /// </summary>
        public override Type viewValueType
        {
            get
            {
                switch (_viewMember)
                {
                    case EViewMember.Value:return typeof(float);
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
                    case EViewMember.Value: return view.value;
                    default: return null;
                }
            }
            set
            {
                switch (_viewMember)
                {
                    case EViewMember.Value: view.value = (float)value; break; 
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

        private void OnValueChanged(float value)
        {
            if (_viewMember == EViewMember.Value)
            {
                ViewToModelIfCanAndTrigger();
            }
        }

    }
}
