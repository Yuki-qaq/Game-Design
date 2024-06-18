using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Inputs
{
    /// <summary>
    /// 按钮模拟输入:通过UGUI模拟输入按钮的状态或轴的值
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Button)]
    [Name("按钮模拟输入")]
    [Tip("通过UGUI模拟输入按钮的状态或轴的值", "Simulate the status of the input button or the value of the axis through ugui")]
    [Tool(XGUICategory.Input, nameof(XGUIManager))]
    public class ButtonAnalogInput : BaseAnalogInput, IPointerUpHandler, IPointerDownHandler
    {
        /// <summary>
        /// 按钮输入
        /// </summary>
        [Name("按钮输入")]
        [Input]
        public string buttonInput;

        /// <summary>
        /// 更新按钮状态或值
        /// </summary>
        [Name("更新按钮状态或值")]
        [Tip("为True时，指针按下或抬起时更新虚拟按钮的按压状态；为False时，指针按下或抬起时更新虚拟轴的值；", "When true, the pressing state of the virtual button is updated when the pointer is pressed or raised; When it is false, the value of the virtual axis is updated when the pointer is pressed or raised;")]
        public bool stateOrValue = true;

        /// <summary>
        /// 按下时的值
        /// </summary>
        [HideInSuperInspector(nameof(stateOrValue), EValidityCheckType.True)]
        [Name("按下时的值")]
        [Range(-1, 1)]
        public float downValue = 1;

        /// <summary>
        /// 抬起时的值
        /// </summary>
        [HideInSuperInspector(nameof(stateOrValue), EValidityCheckType.True)]
        [Name("抬起时的值")]
        [Range(-1, 1)]
        public float upValue = 0;

        /// <summary>
        /// 当指针按下
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (stateOrValue)
            {
                UpdateButton(buttonInput, true);
            }
            else
            {
                UpdateAxis(buttonInput, downValue);
            }
        }

        /// <summary>
        /// 当指针抬起
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (stateOrValue)
            {
                UpdateButton(buttonInput, false);
            }
            else
            {
                UpdateAxis(buttonInput, upValue);
            }
        }
    }
}
