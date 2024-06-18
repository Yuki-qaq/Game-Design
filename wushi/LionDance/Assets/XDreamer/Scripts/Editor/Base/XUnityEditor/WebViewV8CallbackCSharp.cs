using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// Web视图V8调用C#
    /// </summary>
    public class WebViewV8CallbackCSharp : LinkType<WebViewV8CallbackCSharp>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public WebViewV8CallbackCSharp() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public WebViewV8CallbackCSharp(object obj) : base(obj) { }

        #region Callback

        /// <summary>
        /// 回调 方法信息
        /// </summary>
        public static XMethodInfo Callback_MethodInfo { get; } = new XMethodInfo(Type, nameof(Callback));

        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="result"></param>
        public void Callback(string result)
        {
            Callback_MethodInfo.Invoke(obj, null);
        }

        #endregion

        #region OnDestroy

        /// <summary>
        /// 当销毁 方法信息
        /// </summary>
        public static XMethodInfo OnDestroy_MethodInfo { get; } = new XMethodInfo(Type, nameof(OnDestroy));

        /// <summary>
        /// 当销毁
        /// </summary>
        public void OnDestroy()
        {
            OnDestroy_MethodInfo.Invoke(obj, null);
        }

        #endregion

        #region DestroyCallBack

        /// <summary>
        /// 销毁回调 方法信息
        /// </summary>
        public static XMethodInfo DestroyCallBack_MethodInfo { get; } = new XMethodInfo(Type, nameof(DestroyCallBack));

        /// <summary>
        /// 销毁回调
        /// </summary>
        public void DestroyCallBack()
        {
            DestroyCallBack_MethodInfo.Invoke(obj, null);
        }

        #endregion
    }
}
