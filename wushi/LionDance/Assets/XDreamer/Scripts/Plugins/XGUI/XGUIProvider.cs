using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Widgets;
using XCSJ.PluginXGUI.Windows.ColorPickers;

namespace XCSJ.PluginXGUI
{
    /// <summary>
    /// XGUI提供者
    /// </summary>
    [Name("XGUI提供者")]
    [Tip("用于管理和加载常用的XGUI资源", "Used to manage and load commonly used XGUI resources")]
    [RequireComponent(typeof(XGUIManager))]
    [RequireManager(typeof(XGUIManager))]
    [ExecuteInEditMode]
    public class XGUIProvider : InteractProvider
    {
        /// <summary>
        /// 对话框
        /// </summary>
        [Name("对话框")]
        public DialogBox _dialogBox;

        /// <summary>
        /// 日志窗口
        /// </summary>
        [Name("日志窗口")]
        public LogViewController _logViewController;

        /// <summary>
        /// 提示弹出框
        /// </summary>
        [Name("提示弹出框")]
        public TipPopup _tipPopup;

        /// <summary>
        /// 调色板
        /// </summary>
        [Name("调色板")]
        public ColorPicker _colorPicker;

        /// <summary>
        /// 弹出菜单
        /// </summary>
        [Name("弹出菜单")]
        public MenuGenerator _menuGenerator;

        private void OnValidate()
        {
            UpdateGlobalVar();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateGlobalVar();
        }

        private void UpdateGlobalVar()
        {
            XGUIHelper.dialogBox = _dialogBox;
            XGUIHelper.logViewController = _logViewController;
            XGUIHelper.tipPopup = _tipPopup;
            XGUIHelper.colorPicker = _colorPicker;
            XGUIHelper.globalMenuGenerator = _menuGenerator;
        }

        /// <summary>
        /// 获取资产
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAsset<T>() where T : MB
        {
            if (typeof(T) == typeof(DialogBox))
            {
                return _dialogBox as T;
            }
            else if (typeof(T) == typeof(LogViewController))
            {
                return _logViewController as T;
            }
            else if (typeof(T) == typeof(TipPopup))
            {
                return _tipPopup as T;
            }
            else if (typeof(T) == typeof(ColorPicker))
            {
                return _colorPicker as T;
            }
            else if (typeof(T) == typeof(MenuGenerator))
            {
                return _menuGenerator as T;
            }

            return default;
        }

        /// <summary>
        /// 设置资产
        /// </summary>
        /// <param name="component"></param>
        public void SetAsset(MonoBehaviour component)
        {
            var targetType = component.GetType();

            if (targetType == typeof(DialogBox))
            {
                _dialogBox = component as DialogBox;
            }
            else if (targetType == typeof(LogViewController))
            {
                _logViewController = component as LogViewController;
            }
            else if (targetType == typeof(TipPopup))
            {
                _tipPopup = component as TipPopup;
            }
            else if (targetType == typeof(ColorPicker))
            {
                _colorPicker = component as ColorPicker;
            }
            else if (targetType == typeof(MenuGenerator))
            {
                _menuGenerator = component as MenuGenerator;
            }

            UpdateGlobalVar();
        }

        /// <summary>
        /// 设置资产
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        public void SetAsset<T>(GameObject go) where T : MB
        {
            if (!go) return;

            var c = go.GetComponent<T>();
            if (!c) return;

            SetAsset(c);
        }

        /// <summary>
        /// 如果空设置资产
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        public static void SetAssetIfEmpty<T>(GameObject go) where T : MB
        {
            if (!go) return;

            var t = go.GetComponent<T>();
            if (!t) return;

            var asset = XGUIManager.instance.XGetOrAddComponent<XGUIProvider>();

            var target = asset.GetAsset<T>();
            if (!target)
            {
                asset.SetAsset(t);
            }
        }
    }

    /// <summary>
    /// 资产源
    /// </summary>
    public enum EAssetSource
    {
        /// <summary>
        /// 通用资产
        /// </summary>
        [Name("通用资产")]
        CommonAssets,

        /// <summary>
        /// 自定义
        /// </summary>
        [Name("自定义")]
        Custom,
    }

    /// <summary>
    /// XGUI资产
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XGUIAsset<T> where T : MB
    {
        /// <summary>
        /// 资产源
        /// </summary>
        [Name("资产源")]
        [EnumPopup]
        public EAssetSource _assetSource = EAssetSource.CommonAssets;

        /// <summary>
        /// 视图
        /// </summary>
        [Name("视图")]
        [HideInSuperInspector(nameof(_assetSource), EValidityCheckType.NotEqual, EAssetSource.Custom)]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public T _view;

        /// <summary>
        /// XGUI提供者
        /// </summary>
        private static XGUIProvider xguiProvider = null;

        /// <summary>
        /// 视图
        /// </summary>
        public T view
        {
            get
            {
                switch (_assetSource)
                {
                    case EAssetSource.CommonAssets:
                        {
                            if (!xguiProvider && XGUIManager.instance)
                            {
                                xguiProvider = XGUIManager.instance.GetComponent<XGUIProvider>();
                            }

                            if (xguiProvider)
                            {
                                return xguiProvider.GetAsset<T>();
                            }
                            break;
                        }
                    case EAssetSource.Custom: return _view;
                }
                return default;
            }
        }
    }

}
