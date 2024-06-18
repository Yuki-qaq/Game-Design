#if XDREAMER_HOLOLENS
using HoloToolkit.Unity.InputModule;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Components;

namespace XCSJ.PluginHoloLens
{
    /// <summary>
    /// HoloLens点击
    /// </summary>
    [Serializable]
    [ComponentMenu("HoloLens/HoloLens点击", typeof(HoloLensManager))]
    [Name("HoloLens点击", nameof(Click))]
    [XCSJ.Attributes.Icon(EIcon.Click)]
    [KeyNode(nameof(EIcon.Trigger), "触发器")]
    [Tip("HoloLens点击组件事件是检测用户是否凝视某个游戏对象并做出点击的手势的触发器。当条件满足后，组件切换为完成态。", "Hololens click component event is a trigger to detect whether a user stares at a game object and makes a click gesture. When the conditions are met, the component switches to the completed state.")]
    public class Click : HoloLensStateComponent<Click>
    {
        /// <summary>
        /// 创建点击
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("HoloLens", typeof(HoloLensManager))]
        [StateComponentMenu("HoloLens/HoloLens点击", typeof(HoloLensManager))]
#endif
        [Name("HoloLens点击", nameof(Click))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("HoloLens点击组件事件是检测用户是否凝视某个游戏对象并做出点击的手势的触发器。当条件满足后，组件切换为完成态。", "Hololens click component event is a trigger to detect whether a user stares at a game object and makes a click gesture. When the conditions are met, the component switches to the completed state.")]
        public static State CreateClick(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject go;

        /// <summary>
        /// 点击类型
        /// </summary>
        [Name("点击类型")]
        [EnumPopup]
        public EClick clickType = EClick.Click;

        /// <summary>
        /// 自动添加碰撞体
        /// </summary>
        [Name("自动添加碰撞体")]
        [Tip("没有碰撞体，点击事件就不会产生！", "Without collision body, click event will not occur!")]
        public bool autoAddCollider = true;

        /// <summary>
        /// 游戏对象
        /// </summary>
        protected override GameObject gameObj => go;

        private Collider collider;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            if (go)
            {
                collider = go.GetComponent<Collider>();
                if (!collider && autoAddCollider)
                {
                    collider = go.GetComponent<MeshRenderer>() ? (go.AddComponent<MeshCollider>() as Collider) : (go.AddComponent<BoxCollider>() as Collider);
                }
            }
            return base.Init(data);
        }

        /// <summary>
        /// 当进入之前
        /// </summary>
        /// <param name="data"></param>
        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);

            if (inputListener)
            {
                inputListener.onDownCallback += OnInputDown;
                inputListener.onUpCallback += OnInputUp;
                inputListener.onClickCallback += OnInputClicked;
            }
        }

        /// <summary>
        /// 当退出之后
        /// </summary>
        /// <param name="data"></param>
        public override void OnAfterExit(StateData data)
        {
            if (inputListener)
            {
                inputListener.onDownCallback -= OnInputDown;
                inputListener.onUpCallback -= OnInputUp;
                inputListener.onClickCallback -= OnInputClicked;
            }

            base.OnAfterExit(data);
        }

        /// <summary>
        /// 当输入按下
        /// </summary>
        /// <param name="eventData"></param>
#if XDREAMER_HOLOLENS
        protected void OnInputDown(InputEventData eventData)
#else
        protected void OnInputDown(BaseEventData eventData)
#endif
        {
            if (clickType == EClick.Down)
            {
                finished = true;
            }
        }

        /// <summary>
        /// 当输入弹起
        /// </summary>
        /// <param name="eventData"></param>
#if XDREAMER_HOLOLENS
        protected void OnInputUp(InputEventData eventData)
#else
        protected void OnInputUp(BaseEventData eventData)
#endif
        {
            if (clickType == EClick.Up)
            {
                finished = true;
            }
        }

        /// <summary>
        /// 当输入点击
        /// </summary>
        /// <param name="eventData"></param>
#if XDREAMER_HOLOLENS
        protected void OnInputClicked(InputClickedEventData eventData)
#else
        protected void OnInputClicked(BaseEventData eventData)
#endif
        {
            if(clickType== EClick.Click)
            {
                finished = true;
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return go;
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return (go ? go.name : "") + "[" + CommonFun.Name(clickType) + "]";
        }
    }
}
