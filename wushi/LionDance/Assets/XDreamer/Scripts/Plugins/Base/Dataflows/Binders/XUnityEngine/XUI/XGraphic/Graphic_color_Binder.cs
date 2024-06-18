using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;

namespace XCSJ.Extension.Base.Dataflows.Binders.XUnityEngine.XUI.XGraphic
{
    /// <summary>
    /// 颜色绑定器
    /// </summary>
    [Name("颜色绑定器")]
    [DataBinder(typeof(Graphic), nameof(Graphic.color))]
    public class Graphic_color_Binder : TypeMemberDataBinder<Graphic>
    {
        /// <summary>
        /// 尝试获取成员值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool TryGetMemberValue(Type type, object obj, string memberName, out object value, object[] index = null)
        {
            if (obj is Graphic entity && entity)
            {
                value = entity.color;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试设置成员值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool TrySetMemberValue(Type type, object obj, string memberName, object value, object[] index = null)
        {
            if (obj is Graphic entity && entity && TryConvertTo(value, out Color outValue))
            {
                entity.color = outValue;
                return true;
            }
            return false;
        }
    }
}