using System;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine.XUI
{
    /// <summary>
    /// 滑动条关联类型
    /// </summary>
    [LinkType(typeof(Slider))]
    public class Slider_LinkType : Selectable_LinkType<Slider_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Slider_LinkType(Slider obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Slider_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected Slider_LinkType() { }

        #region UpdateVisuals

        /// <summary>
        /// 更新可视 方法信息
        /// </summary>
        public static XMethodInfo UpdateVisuals_MethodInfo { get; } = new XMethodInfo(Type, nameof(UpdateVisuals), TypeHelper.InstanceNotPublic);

        /// <summary>
        /// 更新可视
        /// </summary>
        public void UpdateVisuals() => UpdateVisuals_MethodInfo.Invoke(obj, null);

        #endregion

        #region Set

        /// <summary>
        /// 设置浮点数 方法信息
        /// </summary>
        public static XMethodInfo Set_Float_MethodInfo { get; } = new XMethodInfo(Type, nameof(Set), new Type[] { typeof(float) }, TypeHelper.InstanceNotPublic);

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="input"></param>
        public void Set(float input) => Set_Float_MethodInfo.Invoke(obj, new object[] { input });

        /// <summary>
        /// 设置浮点数布尔 方法信息
        /// </summary>
        public static XMethodInfo Set_Float_Bool_MethodInfo { get; } = new XMethodInfo(Type, nameof(Set), new Type[] { typeof(float), typeof(bool) }, TypeHelper.InstanceNotPublic);

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sendCallback"></param>
        public void Set(float input, bool sendCallback) => Set_Float_Bool_MethodInfo.Invoke(obj, new object[] { input, sendCallback });

        #endregion
    }
}
