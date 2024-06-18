using System;
using UnityEngine;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorExtension.Base.XUnityEditor.Web;
using XCSJ.PluginCommonUtils;
using System.Timers;
using System.IO;
using XCSJ.Extension.Base.XUnityEngine;

namespace XCSJ.EditorExtension.Base.EditorWindows.Web
{
    /// <summary>
    /// Web视图窗口
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if UNITY_2020_1_OR_NEWER
    [Obsolete("在Unity2020.1版本中资源商店被使用网页与包管理器替代，导致UnityEditor.WebView类被移除，本类同步标记过期！", true)]
#endif
    public abstract class WebWiewWindow<T> : XEditorWindow<T>, IHasCustomMenu, IWebWiewCallback
        where T : WebWiewWindow<T>
    {
        /// <summary>
        /// 当前
        /// </summary>
        public EditorWindow_LinkType _this { get; private set; } = null;

        #region IHasCustomMenu

        /// <summary>
        /// 添加项到菜单
        /// </summary>
        /// <param name="menu"></param>
        public override void AddItemsToMenu(GenericMenu menu)
        {
            base.AddItemsToMenu(menu);
            menu.AddItem(EditorGUIUtility.TrTextContent(nameof(Reload), (string)null, (Texture)null), false, this.Reload);
            menu.AddItem(EditorGUIUtility.TrTextContent(nameof(ForceReload), (string)null, (Texture)null), false, this.ForceReload);
            menu.AddItem(EditorGUIUtility.TrTextContent(nameof(About), (string)null, (Texture)null), false, this.About);
            menu.AddItem(EditorGUIUtility.TrTextContent(nameof(AboutWebView), (string)null, (Texture)null), false, this.AboutWebView);
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        public void Reload()
        {
            //Debug.Log("Reload");
            this.m_IsDocked = _this.docked;
            this.webView.Reload();
        }

        /// <summary>
        /// 强制重新加载此页面后回调
        /// </summary>
        protected abstract void OnForceReload();

        /// <summary>
        /// 强制重新加载
        /// </summary>
        public void ForceReload()
        {
            DestroyWebView();
            OnForceReload();
        }

        /// <summary>
        /// 关于
        /// </summary>
        public void About()
        {
            if (webView)
            {
                this.webView.LoadURL(Product.URL);
            }
        }

        /// <summary>
        /// 关于WEB视图
        /// </summary>
        public void AboutWebView()
        {
            if (webView)
            {
                this.webView.LoadURL("chrome://version");
            }
        }

        #endregion

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _this = new EditorWindow_LinkType(this);

            EditorApplication.update += TrackFocusState;

            EditorApplication.wantsToQuit += OnWantsToQuit;
            wantsToQuit = false;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            EditorApplication.update -= TrackFocusState;
            EditorApplication.wantsToQuit -= OnWantsToQuit;

            if (!wantsToQuit) DestroyWebView();
        }

        /// <summary>
        /// 当聚焦
        /// </summary>
        protected void OnFocus()
        {
            this.SetFocus(true);
        }

        /// <summary>
        /// 当失去焦点
        /// </summary>
        protected void OnLostFocus()
        {
            this.SetFocus(false);
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            DrawWebView(GUIClip.Unclip(new Rect(0f, 0, base.position.width, base.position.height)));
        }

        /// <summary>
        /// 党校会
        /// </summary>
        protected void OnDestroy()
        {
            DestroyWebView();
        }

        #region WebView销毁

        /// <summary>
        /// 销毁WEB视图
        /// </summary>
        protected void DestroyWebView()
        {
            //Debug.Log("DestroyWebView");
            if (webView)
            {
                DestroyImmediate(this.webView.unityEngineObject);
                webView = null;
            }
        }

        private bool wantsToQuit = false;
        private bool OnWantsToQuit()
        {
            wantsToQuit = true;
            return true;
        }

        #endregion

        #region WebView

        /// <summary>
        /// WEB视图
        /// </summary>
        public virtual WebView webView { get; protected set; }

        /// <summary>
        /// 脚本对象
        /// </summary>
        public WebScriptObject scriptObject { get; private set; }

        /// <summary>
        /// 绘制WEB视图
        /// </summary>
        /// <param name="rect"></param>
        protected void DrawWebView(Rect rect)
        {
            if (!webView)
            {
                this.InitWebView(rect);
            }

#if UNITY_EDITOR_WIN
#else
            if (Event.current.type == EventType.Repaint && this.m_HasDelayedRefresh)
            {
                this.Refresh();
                this.m_HasDelayedRefresh = false;
            }
#endif
            if (Event.current.type == EventType.Repaint)
            {
                this.webView.SetHostView(_this.m_Parent);
                this.webView.SetSizeAndPosition((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
                this.UpdateDockStatusIfNeeded();
            }
        }

        /// <summary>
        /// 初始化WEB视图
        /// </summary>
        /// <param name="webViewRect"></param>
        protected void InitWebView(Rect webViewRect)
        {
            this.m_IsDocked = _this.docked;
            if (!webView)
            {
                int x = (int)webViewRect.x;
                int y = (int)webViewRect.y;
                int width = (int)webViewRect.width;
                int height = (int)webViewRect.height;
                this.webView = WebView.CreateInstance();
                this.webView.InitWebView(_this.m_Parent, x, y, width, height, false);
                this.webView.hideFlags = HideFlags.HideAndDontSave;
                this.SetFocus(_this.hasFocus);
            }
            this.webView.SetDelegateObject(this);
            LoadUri();
            this.SetFocus(true);
            OnInitWebView();
        }

        /// <summary>
        /// 执行InitWebView后的回调函数 
        /// </summary>
        protected abstract void OnInitWebView();

        /// <summary>
        /// 初始化打开的Uri字符串
        /// </summary>
        protected virtual string initialOpenUri { get; } = Product.URL;

        /// <summary>
        /// 加载URI
        /// </summary>
        protected void LoadUri()
        {
            if (webView)
            {
                webView.Load(initialOpenUri);
            }
        }

        /// <summary>
        /// 调用JS方法
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        public void InvokeJSMethod(string objectName, string name, params object[] args)
        {
            if (webView)
            {
                webView.InvokeJSMethod(objectName, name, args);
            }
        }

        private bool m_IsDocked;
        private void UpdateDockStatusIfNeeded()
        {
            var docked = _this.docked;
            if (this.m_IsDocked != docked)
            {
                this.m_IsDocked = docked;
                OnDockChanged(docked);
            }
        }

        /// <summary>
        /// 当停靠已更改
        /// </summary>
        /// <param name="docked"></param>
        protected virtual void OnDockChanged(bool docked)
        {
            //Debug.Log("OnDockChanged:"+ docked);
            ForceReload();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            if (webView)
            {
                this.webView.Hide();
                this.webView.Show();
            }
        }

        #endregion

        #region WebView Focus

        /// <summary>
        /// 获取焦点
        /// </summary>
        protected bool m_GotFocus = false;

        /// <summary>
        /// 同步焦点
        /// </summary>
        protected bool m_SyncingFocus = false;

        /// <summary>
        /// 聚焦状态
        /// </summary>
        protected void TrackFocusState()
        {
            if (this.m_GotFocus && (UnityEngine.Object)EditorWindow.focusedWindow != (UnityEngine.Object)this)
            {
                this.OnTakeFocus();
            }
        }

        /// <summary>
        /// 当获取焦点
        /// </summary>
        protected virtual void OnGotFocus()
        {
            //Debug.Log("OnGotFocus");
            this.m_GotFocus = true;
            base.Focus();
            if ((bool)_this.m_Parent)
            {
                _this.m_Parent.Repaint();
            }
        }

        /// <summary>
        /// 当聚焦
        /// </summary>
        protected virtual void OnTakeFocus()
        {
            //Debug.Log("OnTakeFocus");
            this.m_GotFocus = false;
            if ((bool)_this.m_Parent)
            {
                _this.m_Parent.Repaint();
            }
        }

#if UNITY_EDITOR_WIN
#else
        private bool m_HasDelayedRefresh = false;
#endif

        private void SetFocus(bool value)
        {
            if (!this.m_SyncingFocus)
            {
                this.m_SyncingFocus = true;
                if (this.webView)
                {
                    if (value)
                    {
                        this.webView.SetHostView(_this.m_Parent);
#if UNITY_EDITOR_WIN
                        this.webView.Show();
#else
                        m_HasDelayedRefresh = true;
#endif
                    }
                    //设置true之后会导致在当前窗口其它位置无法输入字符
                    //this.webView.SetApplicationFocus(_this.m_Parent && _this.m_Parent.hasFocus && _this.hasFocus);
                    this.webView.SetFocus(value);
                }
                this.m_SyncingFocus = false;
            }
        }

        #endregion

        #region IWebWiewCallback

        /// <summary>
        /// 当WebView对象被销毁、被其它WditorWindow遮挡等情况下，被执行的回调函数
        /// </summary>
        public virtual void OnBecameInvisible()
        {
            //Debug.Log("OnBecameInvisible");
            if ((bool)this.webView)
            {
                this.webView.SetHostView(null);
            }
        }

        /// <summary>
        /// <p>WebView在执行LoadURL,LoadFile时底层上自动调用本函数</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        public virtual void OnInitScripting()
        {
            //Debug.Log("OnInitScripting");
            this.SetScriptObject();
        }

        /// <summary>
        /// 设置脚本对象
        /// </summary>
        protected void SetScriptObject()
        {
            if ((bool)this.webView)
            {
                this.CreateScriptObject();
                var ret = this.webView.DefineScriptObject("window.unityScriptObject", this.scriptObject.scriptableObject);
                //Debug.Log("SetScriptObject: " + ret);
            }
        }
        private void CreateScriptObject()
        {
            if (!scriptObject)
            {
                this.scriptObject = WebScriptObject.CreateInstance();
                this.scriptObject.hideFlags = HideFlags.HideAndDontSave;
                this.scriptObject.webView = this.webView;
            }
        }

        /// <summary>
        /// <p>WebView加载无效或错误页面时回调</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        /// <param name="url">发生错误的Url</param>
        public abstract void OnLoadError(string url);

        /// <summary>
        /// <p>WebView的定位路径发生变化时回调；可捕获框架内子页面的URL</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        /// <param name="url">定位后的Url</param>
        public abstract void OnLocationChanged(string url);

        #endregion
    }
}
