using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;

namespace XCSJ.Extension.Base.Dataflows.Binders.XUnityEngine.XUI.XInputField
{
    /// <summary>
    /// 输入绑定器
    /// </summary>
    [Name("输入绑定器")]
    [DataBinder(typeof(InputField), nameof(InputField.text))]
    public class InputField_text_Binder : TypeMemberDataBinder<InputField>
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
            if (obj is InputField entity && entity)
            {
                value = entity.text;
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
            if (obj is InputField entity && entity && TryConvertTo(value, out string outValue))
            {
                entity.text = outValue;
                return true;
            }
            return false;
        }
    }
}
