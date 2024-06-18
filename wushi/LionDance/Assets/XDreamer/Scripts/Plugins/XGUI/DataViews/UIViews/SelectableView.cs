using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginXGUI.DataViews.UIViews
{
    /// <summary>
    /// 可选择的视图
    /// </summary>
    [Name("可选择的视图")]
    [XCSJ.Attributes.Icon(EIcon.Button)]
    [Tool(XGUICategory.DataView, rootType = typeof(XGUIManager))]
    public class SelectableView : BaseUIView<Selectable>
    {
        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        public enum EViewMember
        {
            /// <summary>
            /// 可交互
            /// </summary>
            [Name("可交互")]
            Interactable,
        }

        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        [EnumPopup]
        public EViewMember _viewMember = EViewMember.Interactable;

        /// <summary>
        /// 视图数据类型
        /// </summary>
        public override Type viewValueType
        {
            get
            {
                switch (_viewMember)
                {
                    case EViewMember.Interactable: return typeof(bool);
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
                    case EViewMember.Interactable: return view.interactable;
                    default: return null;
                }
            }
            set
            {
                switch (_viewMember)
                {
                    case EViewMember.Interactable: view.interactable = (bool)value; break;
                }
            }
        }
    }
}