using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 拖拽交互器
    /// </summary>
    public abstract class Dragger : SelectionListener
    {
        #region 交互

        /// <summary>
        /// 抓状态
        /// </summary>
        public enum EGrabState
        {
            /// <summary>
            /// 无：空手状态
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 抓进入
            /// </summary>
            [Name("抓开始")]
            GrabEnter,

            /// <summary>
            /// 抓过程中
            /// </summary>
            [Name("抓过程中")]
            Grabing,

            /// <summary>
            /// 抓退出
            /// </summary>
            [Name("抓结束")]
            GrabExit,

            /// <summary>
            /// 持有物品
            /// </summary>
            [Name("保持对象")]
            Holding,

            /// <summary>
            /// 放进入
            /// </summary>
            [Name("放开始")]
            ReleaseEnter,

            /// <summary>
            /// 放置过程中
            /// </summary>
            [Name("放过程中")]
            Releaseing,

            /// <summary>
            /// 放退出
            /// </summary>
            [Name("放结束")]
            ReleaseExit,
        }

        /// <summary>
        /// 抓状态
        /// </summary>
        public virtual EGrabState grabState { get => _grabState; protected set => _grabState = value; }

        /// <summary>
        /// 拖拽设置
        /// </summary>
        [Group("基础设置", textEN = "Base Settings")]
        [SerializeField]
        [Readonly]
        private EGrabState _grabState = EGrabState.None;

        /// <summary>
        /// 当前抓取器正在执行过程处理工作，无法接收外部指令
        /// </summary>
        public bool busy => grabState != EGrabState.None && grabState != EGrabState.Holding;

        /// <summary>
        /// 拖拽时标识UI
        /// </summary>
        protected virtual bool countUI => true;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            ResetData();
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            if (busy) return false;

            switch (GetInCmd(interactData.cmdName))
            {
                case nameof(Grab):
                    {
                        _grabData = interactData;
                        return CanGrab(interactData, GetGrabbable(_grabData.interactable));
                    }
                case nameof(Hold):
                    {
                        _holdData = interactData;
                        return CanHold(interactData, GetGrabbable(_holdData.interactable));
                    }
                case nameof(Release):
                    {
                        _releaseData = interactData;
                        return CanRelease(interactData, GetGrabbable(_releaseData.interactable));
                    }
            }
            return base.CanInteract(interactData);
        }

        /// <summary>
        /// 固定更新:主要操作物理对象，因此逻辑在固定更新中执行
        /// </summary>
        protected virtual void FixedUpdate()
        {
            try
            {
                switch (grabState)
                {
                    case EGrabState.GrabEnter:
                        {
                            _holdData = null;

                            OnGrabEnter();

                            BeginOnUI();

                            grabbedObject.TryInteractAsInteractable(_grabData, out _);

                            grabState = EGrabState.Grabing;

                            break;
                        }
                    case EGrabState.Grabing:
                        {
                            if (IsGrabbing())
                            {
                                OnGrabbing();
                            }
                            else
                            {
                                grabState = EGrabState.GrabExit;
                            }
                            break;
                        }
                    case EGrabState.GrabExit:
                        {
                            OnGrabExit();

                            grabState = EGrabState.Holding;

                            _grabData = null;

                            break;
                        }
                    case EGrabState.Holding:
                        {
                            if (_holdData != null)
                            {
                                OnHolding();

                                var data = CreateHoldData();
                                if (data != null)
                                {
                                    grabbedObject.TryInteractAsInteractable(data, out _);
                                }
                            }

                            break;
                        }
                    case EGrabState.ReleaseEnter:
                        {
                            _holdData = null;

                            OnReleaseEnter();

                            grabState = EGrabState.Releaseing;

                            break;
                        }
                    case EGrabState.Releaseing:
                        {
                            if (IsReleaseing())
                            {
                                OnReleaseing();
                            }
                            else
                            {
                                grabState = EGrabState.ReleaseExit;
                            }
                            break;
                        }
                    case EGrabState.ReleaseExit:
                        {
                            OnReleaseExit();

                            grabbedObject.TryInteractAsInteractable(_releaseData, out _);

                            ResetData();

                            break;
                        }
                }
            }
            catch (Exception e)
            {
                ResetData();
                e.HandleException();
            }
       }

        /// <summary>
        /// 重置数据
        /// </summary>
        public virtual void ResetData()
        {
            grabState = EGrabState.None;

            if (grabbedObject)
            {
                grabbedObject.RemoveGrabUsage(this);
            }
            grabbedObject = null;

            EndOnUI();

            _grabData = null;
            _holdData = null;
            _releaseData = null;
        }

        private bool isBeginOnUI = false;

        private void BeginOnUI()
        {
            if (countUI)
            {
                if (!isBeginOnUI)
                {
                    isBeginOnUI = true;
                    CommonFun.BeginOnUI();
                }
            }
        }

        private void EndOnUI()
        {
            if (countUI)
            {
                if (isBeginOnUI)
                {
                    isBeginOnUI = false;
                    CommonFun.EndOnUI();
                }
            }
        } 

        #endregion

        #region 抓

        /// <summary>
        /// 抓对象
        /// </summary>
        public Grabbable grabbedObject 
        { 
            get => _grabbedObject;
            protected set
            {
                _grabbedObject = value;
            }
        }

        /// <summary>
        /// 抓对象
        /// </summary>
        [Readonly]
        //[HideInSuperInspector]
        public Grabbable _grabbedObject;

        private InteractData _grabData = null;

        /// <summary>
        /// 抓数据
        /// </summary>
        public RayInteractData grabData => _grabData as RayInteractData;

        /// <summary>
        /// 抓
        /// </summary>
        [InteractCmd]
        [Name("抓")]
        public void Grab() => TryInteract(_grabData, out _);

        /// <summary>
        /// 抓
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Grab))]
        public EInteractResult Grab(InteractData interactData)
        {
            grabbedObject = GetGrabbableOnGab();
            if (grabbedObject)
            {
                grabState = EGrabState.GrabEnter;
            }
            return EInteractResult.Success;
        }

        /// <summary>
        /// 能否抓
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        protected virtual bool CanGrab(InteractData interactData, Grabbable grabbable) => !grabbedObject && grabState == EGrabState.None && (grabbable && grabbable.enabled && grabbable.CanInteractAsInteractable(interactData));

        /// <summary>
        /// 正在抓
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsGrabbing() => false;

        /// <summary>
        /// 抓进入
        /// </summary>
        protected virtual void OnGrabEnter() { }

        /// <summary>
        /// 抓过程中
        /// </summary>
        protected virtual void OnGrabbing() { }

        /// <summary>
        /// 抓退出
        /// </summary>
        protected virtual void OnGrabExit() { }

        /// <summary>
        /// 获取可抓对象
        /// </summary>
        /// <returns></returns>
        protected virtual Grabbable GetGrabbableOnGab() => GetGrabbable(_grabData.interactable);

        /// <summary>
        /// 获取可抓对象
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected virtual Grabbable GetGrabbable(InteractObject interactable) => interactable? interactable.GetComponent<Grabbable>():default;

        #endregion 抓

        #region 保持

        private InteractData _holdData = null;

        /// <summary>
        /// 保持数据
        /// </summary>
        public RayInteractData holdData => _holdData as RayInteractData;

        /// <summary>
        /// 保持
        /// </summary>
        [InteractCmd]
        [Name("保持")]
        public void Hold() => TryInteract(_holdData, out _);

        /// <summary>
        /// 保持
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Hold))]
        public EInteractResult Hold(InteractData interactData)
        {
            return grabState == EGrabState.Holding ? EInteractResult.Success : EInteractResult.Fail;
        }

        /// <summary>
        /// 能否保持
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        protected virtual bool CanHold(InteractData interactData, Grabbable grabbable) => grabbedObject && grabState == EGrabState.Holding && grabbedObject.CanInteractAsInteractable(interactData);

        /// <summary>
        /// 保持中
        /// </summary>
        protected virtual void OnHolding() { }

        /// <summary>
        /// 尝试获取拖拽位置数据
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryGetDragPosition(out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        /// <summary>
        /// 尝试获取拖拽旋转数据
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryGetDragRotation(out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        /// <summary>
        /// 尝试获取拖拽缩放数据
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryGetDragScale(out Vector3 scale)
        {
            scale = Vector3.one;
            return false;
        }

        /// <summary>
        /// 创建拖拽交互数据
        /// </summary>
        /// <returns></returns>
        protected virtual GrabInteractData CreateHoldData()
        {
            Vector3? position = null;
            Quaternion? rotation = null;
            Vector3? scale = null;

            if (TryGetDragPosition(out var pos))
            {
                position = pos;
            }

            if (TryGetDragRotation(out var r))
            {
                rotation = r;
            }

            if (TryGetDragScale(out var s))
            {
                scale = s;
            }

            // 没有真实数据，也会产生空数据，保证可抓对象能接受所有当前与之交互的拖拽器发送的保持数据
            return new GrabInteractData(position, rotation, scale, GetInCmdName(nameof(Hold)), this, grabbedObject);
        }

        #endregion 保持

        #region 放

        private InteractData _releaseData = null;

        /// <summary>
        /// 放数据
        /// </summary>
        public RayInteractData releaseData => _releaseData as RayInteractData;

        /// <summary>
        /// 放
        /// </summary>
        [InteractCmd]
        [Name("放")]
        public void Release() => TryInteract(_releaseData, out _);

        /// <summary>
        /// 保持
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Release))]
        public EInteractResult Release(InteractData interactData)
        {
            if (grabbedObject)
            {
                grabState = EGrabState.ReleaseEnter;
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 能否放
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        protected virtual bool CanRelease(InteractData interactData, Grabbable grabbable) => grabbedObject && grabState == EGrabState.Holding && grabbedObject.CanInteractAsInteractable(interactData);

        /// <summary>
        /// 是否正在放
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsReleaseing() => false;

        /// <summary>
        /// 放进入
        /// </summary>
        protected virtual void OnReleaseEnter() { }

        /// <summary>
        /// 放过程中
        /// </summary>
        protected virtual void OnReleaseing() { }

        /// <summary>
        /// 放退出
        /// </summary>
        protected virtual void OnReleaseExit() { }

        #endregion 放

        /// <summary>
        /// 默认键
        /// </summary>
        public const string DefautKey = nameof(Dragger);

        /// <summary>
        /// 获取用户键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override string GetUsageKey(string key = null) => DefautKey;
    }
}