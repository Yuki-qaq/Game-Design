using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Inputs
{
    /// <summary>
    /// 键码按钮模拟输入:通过UGUI模拟输入键码按钮的状态
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Keyboard)]
    [Name("键码按钮模拟输入")]
    [Tip("通过UGUI模拟输入键码按钮的状态", "Simulate the status of input key code button through UGUI")]
    [Tool(XGUICategory.Input, nameof(XGUIManager))]
    public class KeyCodeButtonAnalogInput : BaseAnalogInput, IPointerUpHandler, IPointerDownHandler
    {
        /// <summary>
        /// 键码
        /// </summary>
        [Name("键码")]
        [Input]
        public KeyCode keyCode;

        /// <summary>
        /// 当指针按下
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateButton(keyCode.ToString(), true);
        }

        /// <summary>
        /// 当指针弹起
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            UpdateButton(keyCode.ToString(), false);
        }
    }
}
