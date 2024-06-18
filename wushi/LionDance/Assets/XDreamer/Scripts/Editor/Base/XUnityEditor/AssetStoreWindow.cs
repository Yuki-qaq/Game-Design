using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 资源商店窗口
    /// </summary>
    [LinkType("UnityEditor.AssetStoreWindow")]
    public class AssetStoreWindow : EditorWindow_LinkType<AssetStoreWindow>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public AssetStoreWindow(EditorWindow obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public AssetStoreWindow(object obj) : base(obj) { }


#if UNITY_2020_1_OR_NEWER

#else//在Unity2020.1版本中资源商店被使用网页与包管理器替代，本类中原诸多反射方法被移除！

        #region Init

        /// <summary>
        /// 初始化 方法信息
        /// </summary>
        public static XMethodInfo Init_MethodInfo { get; } = GetXMethodInfo(nameof(Init));

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static AssetStoreWindow Init()
        {
            return new AssetStoreWindow(Init_MethodInfo.Invoke(null, null));
        }

        #endregion

        #region webView

        /// <summary>
        /// Web视图 字段信息
        /// </summary>
        public static XFieldInfo webView_FieldInfo { get; } = GetXFieldInfo(nameof(webView));

        /// <summary>
        /// Web视图
        /// </summary>
        public WebView webView
        {

            get => new WebView(webView_FieldInfo.GetValue(obj));
            set
            {
                if (value)
                {
                    webView_FieldInfo.SetValue(obj, value.obj);
                }
            }
        }

        #endregion

#endif
    }
}
