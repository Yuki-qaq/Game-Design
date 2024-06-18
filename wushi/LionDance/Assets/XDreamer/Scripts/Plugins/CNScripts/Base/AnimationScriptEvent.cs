using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginCNScripts.Base
{
    /// <summary>
    /// 动画脚本事件类型
    /// </summary>
    [Name("动画脚本事件类型")]
    public enum EAnimationScriptEventType
    {
        /// <summary>
        /// 布尔型切换事件
        /// </summary>
        [Name("布尔型切换事件")]
        BoolChange = 0,

        /// <summary>
        /// 浮点数型切换事件
        /// </summary>
        [Name("浮点数型切换事件")]
        FloatChange,

        /// <summary>
        /// 启动时执行
        /// </summary>
        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// 动画脚本事件函数
    /// </summary>
    [Name("动画脚本事件函数")]
    [Serializable]
    public class AnimationScriptEventFunction : EnumFunction<EAnimationScriptEventType> { }

    /// <summary>
    /// 动画脚本事件函数集合
    /// </summary>
    [Name("动画脚本事件函数集合")]
    [Serializable]
    public class AnimationScriptEventFunctionCollection : EnumFunctionCollection<EAnimationScriptEventType, AnimationScriptEventFunction> { }

    /// <summary>
    /// 动画脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptCategory.CNScriptMenu + Title)]
    [Tool(CNScriptCategory.ComponentEvent, nameof(ScriptManager))]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    public class AnimationScriptEvent : BaseScriptEvent<EAnimationScriptEventType, AnimationScriptEventFunction, AnimationScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "动画脚本事件";

        /// <summary>
        /// 布尔标识
        /// </summary>
        [Name("布尔标识")]
        public bool boolFlag;

        /// <summary>
        /// 布尔标识
        /// </summary>
        public bool _boolFlag { get; private set; }

        /// <summary>
        /// 浮点数标识
        /// </summary>
        [Name("浮点数标识")]
        public float floatFlag;

        /// <summary>
        /// 浮点数标识
        /// </summary>
        public float _floatFlag { get; private set; }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _boolFlag = boolFlag;
            _floatFlag = floatFlag;
            ExecuteScriptEvent(EAnimationScriptEventType.Start);
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (_boolFlag != boolFlag)
            {
                //Debug.Log(name + " , b :  " + boolFlag.ToString());
                _boolFlag = boolFlag;
                ExecuteScriptEvent(EAnimationScriptEventType.BoolChange, ScriptHelper.ReturnValueFlag + boolFlag.ToString());
            }

            if (!Mathf.Approximately(_floatFlag, floatFlag))
            {
                //Debug.Log(name + " , b :  " + floatFlag.ToString());
                _floatFlag = floatFlag;
                ExecuteScriptEvent(EAnimationScriptEventType.FloatChange, floatFlag.ToString());
            }
        }

    }
}
