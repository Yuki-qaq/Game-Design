using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// WEB视图
    /// </summary>
    public interface IWebView : IScriptableObject_LinkType
    {

    }

    /// <summary>
    /// WEB视图回调
    /// </summary>
    public interface IWebWiewCallback
    {
        /// <summary>
        /// <p>WebView在执行LoadURL,LoadFile时底层上自动调用本函数</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        void OnInitScripting();

        /// <summary>
        /// 当WebView对象被销毁、被其它WditorWindow遮挡等情况下，被执行的回调函数
        /// </summary>
        void OnBecameInvisible();

        /// <summary>
        /// <p>WebView加载无效或错误页面时回调</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        /// <param name="url">发生错误的Url</param>
        void OnLoadError(string url);

        /// <summary>
        /// <p>WebView的定位路径发生变化时回调；可捕获框架内子页面的URL</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        /// <param name="url">定位后的Url</param>
        void OnLocationChanged(string url);
    }

    /// <summary>
    /// WEB视图
    /// </summary>
#if UNITY_2020_1_OR_NEWER
    [Obsolete("在Unity2020.1版本中资源商店被使用网页与包管理器替代，导致UnityEditor.WebView类被移除！", true)]
#endif
    [LinkType("UnityEditor.WebView")]
    public class WebView : ScriptableObject_LinkType<WebView>, IWebView
    {
        /// <summary>
        /// 构造
        /// </summary>
        public WebView() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public WebView(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public WebView(ScriptableObject obj) : base(obj) { }

        /// <summary>
        /// 隐式转布尔
        /// </summary>
        /// <param name="webView"></param>
        public static implicit operator bool(WebView webView)
        {
            return webView != null && !webView.IntPtrIsNull();
        }

        /// <summary>
        /// 当销毁
        /// </summary>
        public void OnDestroy()
        {
            this.DestroyWebView();
        }

        #region DestroyWebView

        private static XMethodInfo DestroyWebView_MethodInfo { get; } = new XMethodInfo(Type, nameof(DestroyWebView), TypeHelper.InstanceNotPublic);

        private void DestroyWebView()
        {
            //Debug.Log("DestroyWebView");
            DestroyWebView_MethodInfo.Invoke(obj, Empty<object>.Array);
            obj = null;
        }

        #endregion

        #region InitWebView

        /// <summary>
        /// 初始WEB视图 方法信息
        /// </summary>
        public static XMethodInfo InitWebView_MethodInfo { get; } = new XMethodInfo(Type, nameof(InitWebView), TypeHelper.DefaultLookup);

        /// <summary>
        /// 初始WEB视图
        /// </summary>
        /// <param name="host"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="showResizeHandle"></param>
        public void InitWebView(GUIView host, int x, int y, int width, int height, bool showResizeHandle)
        {
            InitWebView_MethodInfo.Invoke(obj, new object[] { host.scriptableObject, x, y, width, height, showResizeHandle });
        }

        /// <summary>
        /// 初始WEB视图
        /// </summary>
        /// <param name="host"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="showResizeHandle"></param>
        public void InitWebView(IGUIView host, int x, int y, int width, int height, bool showResizeHandle)
        {
            InitWebView_MethodInfo.Invoke(obj, new object[] { host.scriptableObject, x, y, width, height, showResizeHandle });
        }

        #endregion

        /// <summary>
        /// 调用JS方法
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        public void InvokeJSMethod(string objectName, string name, params object[] args)
        {
            if (!scriptableObject) return;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(objectName);
            stringBuilder.Append('.');
            stringBuilder.Append(name);
            stringBuilder.Append('(');
            bool flag = true;
            foreach (object obj in args)
            {
                if (!flag)
                {
                    stringBuilder.Append(',');
                }
                bool flag2 = obj is string;
                if (flag2)
                {
                    stringBuilder.Append('"');
                }
                stringBuilder.Append(obj);
                if (flag2)
                {
                    stringBuilder.Append('"');
                }
                flag = false;
            }
            stringBuilder.Append(");");
            ExecuteJavascript(stringBuilder.ToString());
        }

        #region ExecuteJavascript

        /// <summary>
        /// 执行JS 方法信息
        /// </summary>
        public static XMethodInfo ExecuteJavascript_MethodInfo { get; } = new XMethodInfo(Type, nameof(ExecuteJavascript), TypeHelper.DefaultLookup);

        /// <summary>
        /// 执行JS
        /// </summary>
        /// <param name="scriptCode"></param>
        public void ExecuteJavascript(string scriptCode)
        {
            ExecuteJavascript_MethodInfo.Invoke(obj, new object[] { scriptCode });
        }

        #endregion

        private string _uri = "";

        /// <summary>
        /// URI
        /// </summary>
        public string uri
        {
            get => _uri;
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    Load(_uri);
                }
            }
        }

        /// <summary>
        /// 是文件协议
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool IsFileProtocol(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return false;
            if (uri.StartsWith("file", true, CultureInfo.CurrentCulture)) return true;
            if (Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri value))
            {
                try
                {
                    return value.IsFile;
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// 是任意协议
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool IsAnyProtocol(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return false;
            return uri.Contains(":/") || uri.Contains(":\\");
        }

        /// <summary>
        /// 是HTTP URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsHttpUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return url.StartsWith("http", true, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 尝试转HTTP URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string TryToHttpUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return "";
            if (IsAnyProtocol(url)) return url;
            return "http://" + url;
        }

        private static string[] UrlString = new string[] { "www.", ".cn", ".com" };

        /// <summary>
        /// 有任意URL字符串
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool HasAnyUrlString(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return UrlString.Any(s => url.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="uri"></param>
        public void Load(string uri)
        {
            if (string.IsNullOrEmpty(uri) || !scriptableObject) return;
            if (IsFileProtocol(uri))
            {
                LoadFile(uri);
            }
            else if (IsHttpUrl(uri) || IsAnyProtocol(uri))
            {
                LoadURL(uri);
            }
            else if (HasAnyUrlString(uri))
            {
                LoadURL(TryToHttpUrl(uri));
            }
            else
            {
                string path = Path.Combine(Uri.EscapeUriString(Path.Combine(EditorApplication.applicationContentsPath, "Resources")), uri);
                LoadFile(path);
            }
        }

        #region LoadURL

        /// <summary>
        /// 加载URL 方法信息
        /// </summary>
        public static XMethodInfo LoadURL_MethodInfo { get; } = new XMethodInfo(Type, nameof(LoadURL), TypeHelper.DefaultLookup);

        /// <summary>
        /// 加载URL
        /// </summary>
        /// <param name="url"></param>
        public void LoadURL(string url)
        {
            //Debug.Log("LoadURL:" + url);
            LoadURL_MethodInfo.Invoke(obj, new object[] { this.uri = url });
        }

        #endregion

        #region LoadFile

        /// <summary>
        /// 加载文件 方法信息
        /// </summary>
        public static XMethodInfo LoadFile_MethodInfo { get; } = new XMethodInfo(Type, nameof(LoadFile), TypeHelper.DefaultLookup);

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="path"></param>
        public void LoadFile(string path)
        {
            //Debug.Log("LoadFile:" + url);
            LoadFile_MethodInfo.Invoke(obj, new object[] { this.uri = path });
        }

        #endregion

        #region DefineScriptObject

        /// <summary>
        /// 定义脚本对象 方法信息
        /// </summary>
        public static XMethodInfo DefineScriptObject_MethodInfo { get; } = new XMethodInfo(Type, nameof(DefineScriptObject), TypeHelper.DefaultLookup);

        /// <summary>
        /// 定义脚本对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool DefineScriptObject(string path, ScriptableObject obj)
        {
            return (bool)DefineScriptObject_MethodInfo.Invoke(this.obj, new object[] { path, obj });
        }

        #endregion

        #region SetDelegateObject

        /// <summary>
        /// 设置委托对象 方法信息
        /// </summary>
        public static XMethodInfo SetDelegateObject_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetDelegateObject), TypeHelper.DefaultLookup);

        /// <summary>
        /// 设置委托对象
        /// </summary>
        /// <param name="value"></param>
        public void SetDelegateObject(ScriptableObject value)
        {
            SetDelegateObject_MethodInfo.Invoke(obj, new object[] { value });
        }

        /// <summary>
        /// 设置委托对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void SetDelegateObject<T>(T value) where T : ScriptableObject, IWebWiewCallback
        {
            SetDelegateObject_MethodInfo.Invoke(obj, new object[] { value });
        }

        #endregion

        #region SetHostView

        /// <summary>
        /// 设置宿主视 方法信息
        /// </summary>
        public static XMethodInfo SetHostView_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetHostView), TypeHelper.DefaultLookup);

        /// <summary>
        /// 设置宿主视
        /// </summary>
        /// <param name="view"></param>
        public void SetHostView(GUIView view)
        {
            SetHostView_MethodInfo.Invoke(obj, new object[] { view ? view.scriptableObject : null });
        }

        /// <summary>
        /// 设置宿主视图
        /// </summary>
        /// <param name="view"></param>
        public void SetHostView(IGUIView view)
        {
            SetHostView_MethodInfo.Invoke(obj, new object[] { view?.obj });
        }

        #endregion

        #region SetSizeAndPosition

        /// <summary>
        /// 设置尺寸与位置 方法信息
        /// </summary>
        public static XMethodInfo SetSizeAndPosition_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetSizeAndPosition), TypeHelper.DefaultLookup);

        /// <summary>
        /// 设置尺寸与位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSizeAndPosition(int x, int y, int width, int height)
        {
            SetSizeAndPosition_MethodInfo.Invoke(obj, new object[] { x, y, width, height });
        }

        #endregion

        #region SetFocus

        /// <summary>
        /// 设置焦点 方法信息
        /// </summary>
        public static XMethodInfo SetFocus_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetFocus), TypeHelper.DefaultLookup);

        /// <summary>
        /// 设置焦点
        /// </summary>
        /// <param name="value"></param>
        public void SetFocus(bool value)
        {
            SetFocus_MethodInfo.Invoke(obj, new object[] { value });
        }

        #endregion

        #region HasApplicationFocus

        /// <summary>
        /// 有应用程序焦点 方法信息
        /// </summary>
        public static XMethodInfo HasApplicationFocus_MethodInfo { get; } = new XMethodInfo(Type, nameof(HasApplicationFocus), TypeHelper.DefaultLookup);

        /// <summary>
        /// 有应用程序焦点
        /// </summary>
        /// <returns></returns>
        public bool HasApplicationFocus()
        {
            return (bool)HasApplicationFocus_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region SetApplicationFocus

        /// <summary>
        /// 设置应用程序焦点 方法信息
        /// </summary>
        public static XMethodInfo SetApplicationFocus_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetApplicationFocus), TypeHelper.DefaultLookup);

        /// <summary>
        /// 设置应用程序焦点
        /// </summary>
        /// <param name="applicationFocus"></param>
        public void SetApplicationFocus(bool applicationFocus)
        {
            SetApplicationFocus_MethodInfo.Invoke(obj, new object[] { applicationFocus });
        }

        #endregion

        #region Show

        /// <summary>
        /// 显示 方法信息
        /// </summary>
        public static XMethodInfo Show_MethodInfo { get; } = new XMethodInfo(Type, nameof(Show), TypeHelper.DefaultLookup);

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            Show_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region Hide

        /// <summary>
        /// 隐藏 方法信息
        /// </summary>
        public static XMethodInfo Hide_MethodInfo { get; } = new XMethodInfo(Type, nameof(Hide), TypeHelper.DefaultLookup);

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            Hide_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region Back

        /// <summary>
        /// 后退 方法信息
        /// </summary>
        public static XMethodInfo Back_MethodInfo { get; } = new XMethodInfo(Type, nameof(Back), TypeHelper.DefaultLookup);

        /// <summary>
        /// 后退
        /// </summary>
        public void Back()
        {
            Back_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region Forward

        /// <summary>
        /// 前进 方法信息
        /// </summary>
        public static XMethodInfo Forward_MethodInfo { get; } = new XMethodInfo(Type, nameof(Forward), TypeHelper.DefaultLookup);

        /// <summary>
        /// 前进
        /// </summary>
        public void Forward()
        {
            Forward_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region SendOnEvent

        /// <summary>
        /// 发送事件 方法信息
        /// </summary>
        public static XMethodInfo SendOnEvent_MethodInfo { get; } = new XMethodInfo(Type, nameof(SendOnEvent), TypeHelper.DefaultLookup);

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="jsonStr"></param>
        public void SendOnEvent(string jsonStr)
        {
            SendOnEvent_MethodInfo.Invoke(obj, new object[] { jsonStr });
        }

        #endregion

        #region Reload

        /// <summary>
        /// 重新加载 方法信息
        /// </summary>
        public static XMethodInfo Reload_MethodInfo { get; } = new XMethodInfo(Type, nameof(Reload), TypeHelper.DefaultLookup);

        /// <summary>
        /// 重新加载
        /// </summary>
        public void Reload()
        {
            Reload_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region AllowRightClickMenu

        /// <summary>
        /// 允许右键点击菜单 方法信息
        /// </summary>
        public static XMethodInfo AllowRightClickMenu_MethodInfo { get; } = new XMethodInfo(Type, nameof(AllowRightClickMenu), TypeHelper.DefaultLookup);

        /// <summary>
        /// 允许右键点击菜单
        /// </summary>
        /// <param name="allowRightClickMenu"></param>
        public void AllowRightClickMenu(bool allowRightClickMenu)
        {
            AllowRightClickMenu_MethodInfo.Invoke(obj, new object[] { allowRightClickMenu });
        }

        #endregion

        #region ShowDevTools

        /// <summary>
        /// 显示开发工具 方法信息
        /// </summary>
        public static XMethodInfo ShowDevTools_MethodInfo { get; } = new XMethodInfo(Type, nameof(ShowDevTools), TypeHelper.DefaultLookup);

        /// <summary>
        /// 显示开发工具
        /// </summary>
        public void ShowDevTools()
        {
            ShowDevTools_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region ToggleMaximize

        /// <summary>
        /// 切换最大化 方法信息
        /// </summary>
        public static XMethodInfo ToggleMaximize_MethodInfo { get; } = new XMethodInfo(Type, nameof(ToggleMaximize), TypeHelper.DefaultLookup);

        /// <summary>
        /// 切换最大化
        /// </summary>
        public void ToggleMaximize()
        {
            ToggleMaximize_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region OnDomainReload

        /// <summary>
        /// 当域重新加载 方法信息
        /// </summary>
        public static XMethodInfo OnDomainReload_MethodInfo { get; } = new XMethodInfo(Type, nameof(OnDomainReload), TypeHelper.DefaultLookup);

        /// <summary>
        /// 当域重新加载
        /// </summary>
        public void OnDomainReload()
        {
            OnDomainReload_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region IntPtrIsNull

        /// <summary>
        /// 整型指针空 方法信息
        /// </summary>
        public static XMethodInfo IntPtrIsNull_MethodInfo { get; } = new XMethodInfo(Type, nameof(IntPtrIsNull), TypeHelper.InstanceNotPublic);

        /// <summary>
        /// 整型指针空
        /// </summary>
        /// <returns></returns>
        private bool IntPtrIsNull()
        {
            var x = IntPtrIsNull_MethodInfo.Invoke(obj, Empty<object>.Array);
            //return (bool)IntPtrIsNull_MethodInfo.Invoke(obj, Empty<object>.Array);
            return (bool)x;
        }

        #endregion
    }
}
