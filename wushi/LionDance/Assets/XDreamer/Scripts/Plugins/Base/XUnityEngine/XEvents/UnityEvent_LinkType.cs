using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine.XEvents
{
    /// <summary>
    /// Unity事件关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnityEvent_LinkType<T> : UnityEventBase_LinkType<T>
       where T : UnityEvent_LinkType<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEvent_LinkType(UnityEvent obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEvent_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected UnityEvent_LinkType() { }

        #region GetDelegate

        /// <summary>
        /// 获取委托 方法信息
        /// </summary>
        public static XMethodInfo GetDelegate_MethodInfo { get; } = GetXMethodInfo(nameof(GetDelegate), TypeHelper.DefaultStatic);

        /// <summary>
        /// 获取委托
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BaseInvokableCall GetDelegate(UnityAction action)
        {
            return new BaseInvokableCall(GetDelegate_MethodInfo?.Invoke(null, new object[] { action }));
        }

        #endregion
    }

    /// <summary>
    /// Unity事件关联类型
    /// </summary>
    [LinkType(typeof(UnityEvent))]
    public class UnityEvent_LinkType : UnityEvent_LinkType<UnityEvent_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEvent_LinkType(UnityEvent obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEvent_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected UnityEvent_LinkType() { }
    }
}
