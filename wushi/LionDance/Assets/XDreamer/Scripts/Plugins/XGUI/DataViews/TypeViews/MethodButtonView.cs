using System;
using System.Reflection;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.TypeViews
{
    /// <summary>
    /// 方法按钮数据视图：通过按钮点击调用绑定方法
    /// </summary>
    [Name("方法按钮数据视图")]
    [DataViewAttribute(typeof(MethodInfo))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class MethodButtonView : BaseModelView
    {
        /// <summary>
        /// 按钮
        /// </summary>
        [Name("按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Button _callMethodButton;

        private Button buttonOnEnable;

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_callMethodButton)
            {
                buttonOnEnable = _callMethodButton;
                buttonOnEnable.onClick.AddListener(OnClick);
            }
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (buttonOnEnable)
            {
                buttonOnEnable.onClick.RemoveListener(OnClick);
                buttonOnEnable = null;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this._modelViewDataLinkMode = EModelViewDataLinkMode.ViewToModel;
            this._modelToViewDataUpdateRule = EDataUpdateRule.None;
            this._fieldPropertyMethodBinder._bindType = Extension.Base.Dataflows.Binders.EBindType.Method;
            this.XGetComponent(ref _callMethodButton);
        }

        private void OnClick() => ViewToModelIfCanAndTrigger();

        /// <summary>
        /// 视图值类型：按钮无视图值类型
        /// </summary>
        public override Type viewValueType => null;

        /// <summary>
        /// 视图值：按钮无视图值
        /// </summary>
        public override object viewValue { get => null; set { } }

        /// <summary>
        /// 将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        protected override bool OnViewToModel()
        {
            //base.OnViewToModel();
            SetModelValue(default);
            CallChildViewToModel(default, viewValue);
            return true;
        }
    }
}
