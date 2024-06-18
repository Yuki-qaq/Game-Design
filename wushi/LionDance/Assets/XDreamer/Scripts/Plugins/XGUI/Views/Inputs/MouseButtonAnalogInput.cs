using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Inputs
{
    /// <summary>
    /// 鼠标按钮模拟输入:通过UGUI模拟输入鼠标按钮的状态
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Mouse)]
    [Name("鼠标按钮模拟输入")]
    [Tip("通过UGUI模拟输入鼠标按钮的状态", "Input the status of mouse button through UGUI simulation")]
    [Tool(XGUICategory.Input, nameof(XGUIManager))]
    public class MouseButtonAnalogInput : BaseAnalogInput, IPointerUpHandler, IPointerDownHandler
    {
        /// <summary>
        /// 鼠标按钮
        /// </summary>
        [Name("鼠标按钮")]
        [EnumPopup]
        public EMouseButton _mouseButton = EMouseButton.Left;

        /// <summary>
        /// 当指针按下
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateMouseButton(_mouseButton, true);
        }

        /// <summary>
        /// 当指针弹起
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            UpdateMouseButton(_mouseButton, false);
        }

        /// <summary>
        /// 更新鼠标按钮
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <param name="downOrUp"></param>
        public void UpdateMouseButton(EMouseButton mouseButton, bool downOrUp)
        {
            foreach (var keyCode in mouseButton.GetKeyCodes())
            {
                UpdateButton(keyCode.ToString(), downOrUp);
            }
        }
    }
}
