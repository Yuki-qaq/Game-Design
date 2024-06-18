using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;

namespace XCSJ.Extension.Base.Dataflows.Binders.XUnityEngine.XUIXDropdown.XDropdown
{
    /// <summary>
    /// 下拉选择列表绑定器
    /// </summary>
    [Name("下拉选择列表绑定器")]
    [DataBinder(typeof(Dropdown), nameof(Dropdown.value))]
    public class Dropdown_value_Binder : TypeMemberDataBinder<Dropdown>
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
            if (obj is Dropdown entity && entity)
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
            if (obj is Dropdown entity && entity && TryConvertTo(value, out int outValue))
            {
                entity.value = outValue;
                return true;
            }
            return false;
        }
    }
}
