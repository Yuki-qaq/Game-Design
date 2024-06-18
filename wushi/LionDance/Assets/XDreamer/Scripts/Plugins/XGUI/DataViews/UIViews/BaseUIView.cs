using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.UIViews
{
    /// <summary>
    /// 基础UI视图：将底层数据和上层视图对象进行绑定
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public abstract class BaseUIView<TView> : BaseModelView where TView:Component
    {
        /// <summary>
        /// 视图
        /// </summary>
        [Name("视图")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public TView _view;

        /// <summary>
        /// 视图
        /// </summary>
        public TView view => this.XGetComponent(ref _view);

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (view) { }
        }

        /// <summary>
        /// 启用时视图
        /// </summary>
        protected TView viewOnEnable;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            viewOnEnable = view;
        }
    }
}
