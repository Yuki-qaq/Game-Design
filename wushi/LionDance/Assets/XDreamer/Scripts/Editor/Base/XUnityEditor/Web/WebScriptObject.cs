using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor.Web
{
    /// <summary>
    /// WEB脚本对象
    /// </summary>
#if UNITY_2020_1_OR_NEWER
    [Obsolete("在Unity2020.1版本中资源商店被使用网页与包管理器替代，导致UnityEditor.Web.WebScriptObject类被移除！", true)]
#endif
    [LinkType("UnityEditor.Web.WebScriptObject")]
    public class WebScriptObject : ScriptableObject_LinkType<WebScriptObject>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public WebScriptObject() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public WebScriptObject(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public WebScriptObject(ScriptableObject obj) : base(obj) { }

        #region webView

        /// <summary>
        /// WEB视图 属性信息
        /// </summary>
        public static XPropertyInfo webView_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(webView), TypeHelper.DefaultLookup);

        /// <summary>
        /// WEB视图
        /// </summary>
        public WebView webView
        {
            get
            {
                return new WebView(webView_PropertyInfo.GetValue(obj));
            }
            set
            {
                webView_PropertyInfo.SetValue(obj, value ? value.unityEngineObject : null);
            }
        }

        #endregion

        #region ProcessMessage

        /// <summary>
        /// 处理消息 方法信息
        /// </summary>
        public static XMethodInfo ProcessMessage_MethodInfo { get; } = new XMethodInfo(Type, nameof(ProcessMessage), TypeHelper.DefaultLookup);

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="jsonRequest"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool ProcessMessage(string jsonRequest, WebViewV8CallbackCSharp callback)
        {
            return (bool)ProcessMessage_MethodInfo.Invoke(obj, new object[] { jsonRequest, callback?.obj });
        }

        #endregion

        #region processMessage

        /// <summary>
        /// 处理消息 方法消息
        /// </summary>
        public static XMethodInfo processMessage_MethodInfo { get; } = new XMethodInfo(Type, nameof(processMessage), TypeHelper.DefaultLookup);

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="jsonRequest"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool processMessage(string jsonRequest, WebViewV8CallbackCSharp callback)
        {
            return (bool)processMessage_MethodInfo.Invoke(obj, new object[] { jsonRequest, callback?.obj });
        }

        #endregion
    }
}
