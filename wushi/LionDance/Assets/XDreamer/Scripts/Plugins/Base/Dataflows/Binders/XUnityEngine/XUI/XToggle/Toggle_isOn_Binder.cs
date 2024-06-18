using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;

namespace XCSJ.Extension.Base.Dataflows.Binders.XUnityEngine.XUI.XToggle
{
    /// <summary>
    /// 切换绑定器
    /// </summary>
    [Name("切换绑定器")]
    [DataBinder(typeof(Toggle), nameof(Toggle.isOn))]
    public class Toggle_isOn_Binder : TypeMemberDataBinder<Toggle>
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
            if (obj is Toggle entity && entity)
            {
                value = entity.isOn;
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
            if (obj is Toggle entity && entity && TryConvertTo(value, out bool outValue))
            {
                entity.isOn = outValue;
                return true;
            }
            return false;
        }
    }
}
