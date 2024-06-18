using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.ViewControllers;

namespace XCSJ.PluginXGUI.Views.ScrollViews
{
    /// <summary>
    /// UI项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UIItem<T> : View where T : Component
    {
        /// <summary>
        /// 容器
        /// </summary>
        public UIContainer<T> container { get; set; }
    }
}
