#if XDREAMER_HOLOLENS
using HoloToolkit.Unity.InputModule;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Components;

namespace XCSJ.PluginHoloLens
{
    /// <summary>
    /// HoloLens聚焦
    /// </summary>
    [Serializable]
    [ComponentMenu("HoloLens/HoloLens聚焦", typeof(HoloLensManager))]
    [Name("HoloLens聚焦", nameof(Focus))]
    [XCSJ.Attributes.Icon(EIcon.Focus)]
    [KeyNode(nameof(EIcon.Trigger),"触发器")]
    [Tip("HoloLens聚焦组件是用于判断用户是否凝视在某个游戏对象的触发器。当用户凝视在指定的游戏对象上后，组件切换为完成态。", "The hololens focus component is a trigger used to determine whether the user is staring at a game object. When the user stares at the specified game object, the component switches to the completed state.")]
    public class Focus : HoloLensStateComponent<Focus>, ITrigger
    {
        /// <summary>
        /// 创建聚焦
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("HoloLens", typeof(HoloLensManager))]
        [StateComponentMenu("HoloLens/HoloLens聚焦", typeof(HoloLensManager))]
#endif
        [Name("HoloLens聚焦", nameof(Focus))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("HoloLens聚焦组件是用于判断用户是否凝视在某个游戏对象的触发器。当用户凝视在指定的游戏对象上后，组件切换为完成态。", "The hololens focus component is a trigger used to determine whether the user is staring at a game object. When the user stares at the specified game object, the component switches to the completed state.")]
        public static State CreateFocus(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject go;

        private Collider collider;

        /// <summary>
        /// 聚焦类型
        /// </summary>
        [Name("聚焦类型")]
        [EnumPopup]
        public EFocus fucusType = EFocus.Enter;

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
                inputListener.onFocusEnterCallback += OnFocusEnter;
                inputListener.onFocusExitCallback += OnFocusExit;
            }
        }

        /// <summary>
        /// 当退出之后
        /// </summary>
        /// <param name="data"></param>
        public override void OnAfterExit(StateData data)
        {
            if(inputListener)
            {
                inputListener.onFocusEnterCallback -= OnFocusEnter;
                inputListener.onFocusExitCallback -= OnFocusExit;
            }

            base.OnAfterExit(data);
        }

        /// <summary>
        /// 当聚焦进入
        /// </summary>
        protected void OnFocusEnter()
        {
            if (fucusType == EFocus.Enter)
            {
                finished = true;
            }
        }

        /// <summary>
        /// 当聚焦退出
        /// </summary>
        protected void OnFocusExit()
        {
            if (fucusType == EFocus.Exit)
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
            return (go ? go.name : "") +"["+CommonFun.Name(fucusType)+"]";
        }
    }
}
