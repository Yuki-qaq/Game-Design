using System.Collections.Generic;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Buttons
{
    /// <summary>
    /// 按钮交互执行器
    /// </summary>
    [Name("按钮交互执行器")]
    [XCSJ.Attributes.Icon(EIcon.Button)]
    [Tip("通过按钮（Button）点击触发执行交互", "Trigger execution interaction by clicking a button")]
    [Tool(XGUICategory.Component, nameof(XGUIManager))]
    public class ButtonInteractExecuter : View
    {
        /// <summary>
        /// 按钮
        /// </summary>
        [Name("按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Button _button;

        /// <summary>
        /// 交互执行列表
        /// </summary>
        [Name("交互执行列表")]
        public List<ExecuteInteractInfo> _executeInteractInfos = new List<ExecuteInteractInfo>();

        private Button buttonOnEnable;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.XGetComponent(ref _button);
        }

        /// <summary>
        /// 当启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!_button)
            {
                _button = GetComponent<Button>();
            }
            if (_button)
            {
                buttonOnEnable = _button;
                buttonOnEnable.onClick.AddListener(OnClick);
            }
        }

        /// <summary>
        /// 当禁用：解除UI事件绑定
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

        private void OnClick()
        {
            foreach (var item in _executeInteractInfos)
            {
                item.TryInteract();
            }
        }
    }
}
