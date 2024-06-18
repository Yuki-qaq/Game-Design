using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.DataViews.UIViews
{
    /// <summary>
    /// 图像视图
    /// </summary>
    [Name("图像视图")]
    [XCSJ.Attributes.Icon(EIcon.Image)]
    [Tool(XGUICategory.DataView, rootType = typeof(XGUIManager))]
    public class ImageView : BaseUIView<Image>
    {
        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        public enum EViewMember
        {
            /// <summary>
            /// 精灵
            /// </summary>
            [Name("精灵")]
            Sprite,

            /// <summary>
            /// 颜色
            /// </summary>
            [Name("颜色")]
            Color,

            /// <summary>
            /// 材质
            /// </summary>
            [Name("材质")]
            Material,
        }

        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        [EnumPopup]
        public EViewMember _viewMember = EViewMember.Sprite;

        /// <summary>
        /// 视图数据类型
        /// </summary>
        public override Type viewValueType
        {
            get
            {
                switch (_viewMember)
                {
                    case EViewMember.Sprite: return typeof(Sprite);
                    case EViewMember.Color: return typeof(Color);
                    case EViewMember.Material: return typeof(Material);
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
                    case EViewMember.Sprite: return view.sprite;
                    case EViewMember.Color: return view.color;
                    case EViewMember.Material: return view.material;
                    default: return null;
                }
            }
            set
            {
                switch (_viewMember)
                {
                    case EViewMember.Sprite: view.sprite = value as Sprite; break;
                    case EViewMember.Color: view.color = (Color)value; break;
                    case EViewMember.Material: view.material = value as Material; break;
                }
            }
        }
    }
}