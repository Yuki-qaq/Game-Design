using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;

namespace XCSJ.Extension.Base.Dataflows.Binders.XUnityEngine.XUI.XScrollbar
{
    /// <summary>
    /// 滚动条绑定器
    /// </summary>
    [Name("滚动条绑定器")]
    [DataBinder(typeof(Scrollbar), nameof(Scrollbar.value))]
    public class Scrollbar_value_Binder : TypeMemberDataBinder<Scrollbar>
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
            if (obj is Scrollbar entity && entity)
            {
                value = entity.value;
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
            if (obj is Scrollbar entity && entity && TryConvertTo(value, out float outValue))
            {
                entity.value = outValue;
                return true;
            }
            return false;
        }
    }
}
