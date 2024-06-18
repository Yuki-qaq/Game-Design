using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine.XEvents
{
    /// <summary>
    /// 基础可调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseInvokableCall<T> : LinkType<T>
       where T : BaseInvokableCall<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public BaseInvokableCall(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected BaseInvokableCall() { }
    }

    /// <summary>
    /// 基础可调用
    /// </summary>
    [LinkType("UnityEngine.Events.BaseInvokableCall")]
    public class BaseInvokableCall : BaseInvokableCall<BaseInvokableCall>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public BaseInvokableCall(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected BaseInvokableCall() { }
    }
}
