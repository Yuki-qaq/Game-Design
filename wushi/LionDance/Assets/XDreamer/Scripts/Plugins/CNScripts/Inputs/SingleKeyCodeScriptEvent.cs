using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginCNScripts.Inputs
{
    /// <summary>
    /// 单一按键脚本事件类型
    /// </summary>
    [Name("单一按键脚本事件类型")]
    public enum ESingleKeyCodeScriptEventType
    {
        /// <summary>
        /// 按下时
        /// </summary>
        [Name("按下时执行")]
        Down,

        /// <summary>
        /// 按下中时
        /// </summary>
        [Name("按下中时执行")]
        Pressed,

        /// <summary>
        /// 弹起时
        /// </summary>
        [Name("弹起时执行")]
        Up,

        /// <summary>
        /// 启动时执行
        /// </summary>
        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// 单一按键脚本事件函数
    /// </summary>
    [Name("单一按键脚本事件函数")]
    [Serializable]
    public class SingleKeyCodeScriptEventFunction : EnumFunction<ESingleKeyCodeScriptEventType> { }

    /// <summary>
    /// 单一按键脚本事件函数集合
    /// </summary>
    [Name("单一按键脚本事件函数集合")]
    [Serializable]
    public class SingleKeyCodeScriptEventFunctionCollection : EnumFunctionCollection<ESingleKeyCodeScriptEventType, SingleKeyCodeScriptEventFunction> { }

    /// <summary>
    /// 单一按键脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptCategory.InputMenu + Title)]
    [Tool(CNScriptCategory.Input, nameof(ScriptManager))]
    public class SingleKeyCodeScriptEvent : BaseScriptEvent<ESingleKeyCodeScriptEventType, SingleKeyCodeScriptEventFunction, SingleKeyCodeScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "单一按键脚本事件";

        /// <summary>
        /// 按键码
        /// </summary>
        [Name("按键码")]
        [Tip("单一按键码脚本事件检测的按键码类型", "Key code type of single key code script event detection")]
        public KeyCode keyCode = KeyCode.None;

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(ESingleKeyCodeScriptEventType.Start);
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                ExecuteScriptEvent(ESingleKeyCodeScriptEventType.Down, keyCode.ToString());
            }
            else if (Input.GetKey(keyCode))
            {
                ExecuteScriptEvent(ESingleKeyCodeScriptEventType.Pressed, keyCode.ToString());
            }
            else if (Input.GetKeyUp(keyCode))
            {
                ExecuteScriptEvent(ESingleKeyCodeScriptEventType.Up, keyCode.ToString());
            }
        }
    }
}
