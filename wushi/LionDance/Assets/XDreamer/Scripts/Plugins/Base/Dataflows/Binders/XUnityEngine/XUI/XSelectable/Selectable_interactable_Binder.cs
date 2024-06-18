using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;

namespace XCSJ.Extension.Base.Dataflows.Binders.XUnityEngine.XUI.XSelectable
{
    /// <summary>
    /// 可交互对象绑定器 
    /// </summary>
    [Name("可交互对象绑定器")]
    [DataBinder(typeof(Selectable), nameof(Selectable.interactable))]
    public class Selectable_interactable_Binder : TypeMemberDataBinder<Selectable>
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
            if (obj is Selectable entity && entity)
            {
                value = entity.interactable;
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
            if (obj is Selectable entity && entity && TryConvertTo(value, out bool outValue))
            {
                entity.interactable = outValue;
                return true;
            }
            return false;
        }
    }
}
