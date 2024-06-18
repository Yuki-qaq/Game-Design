using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Items;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit.UI;
#endif

namespace XCSJ.PluginXGUI.ViewControllers
{
    /// <summary>
    /// 画布控制器:用于控制画布在屏幕或世界空间中的显示与隐藏
    /// </summary>
    [Name("画布控制器")]
    [Tip("用于控制画布在屏幕与世界空间中的显示与隐藏", "Used to control the display and hiding of the canvas on the screen and in world space")]
    [XCSJ.Attributes.Icon(EIcon.UI)]
    [Tool(XGUICategory.Component, nameof(XGUIManager))]
    public class CanvasController : BaseViewController
    {
        /// <summary>
        /// 画布
        /// </summary>
        [Name("画布")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Canvas _canvas;

        /// <summary>
        /// 画布插槽
        /// </summary>
        [Name("画布插槽")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public SingleSocket _canvasSocket;

        /// <summary>
        /// 控制规则
        /// </summary>
        public enum EControlRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 控制插槽游戏对象激活
            /// </summary>
            [Name("控制插槽游戏对象激活")]
            ControlSocketGameObjectActive,
        }

        /// <summary>
        /// 控制规则
        /// </summary>
        [Name("控制规则")]
        [EnumPopup]
        public EControlRule _controlRule = EControlRule.ControlSocketGameObjectActive;

        /// <summary>
        /// 渲染模式
        /// </summary>
        [Name("渲染模式")]
        [Readonly]
        public RenderMode _renderMode = RenderMode.ScreenSpaceOverlay;

        /// <summary>
        /// 渲染模式
        /// </summary>
        public RenderMode renderMode
        {
            get => _renderMode;
            set
            {
                // 模式变换且画布激活时，重新隐藏显示
                if (_renderMode != value && _canvas.gameObject.activeInHierarchy)
                {
                    OnHideCanvas(_renderMode);
                    OnDisplayCanvas(value);
                }
                _renderMode = value;
                if (_canvas) _canvas.renderMode = value;
            }
        }

        private Grabbable grabbable;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            if (_canvas)
            {
                _renderMode = _canvas.renderMode;
            }
        }

        private bool orgSocketGameObjectActive = false;

#if XDREAMER_XR_INTERACTION_TOOLKIT
        TrackedDeviceGraphicRaycaster trackedDeviceGraphicRaycaster;
#endif

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_canvas)
            {
                grabbable = _canvas.GetComponent<Grabbable>();

#if XDREAMER_XR_INTERACTION_TOOLKIT
                if (!_canvas.GetComponent<TrackedDeviceGraphicRaycaster>())
                {
                    trackedDeviceGraphicRaycaster = _canvas.XAddComponent<TrackedDeviceGraphicRaycaster>();
                }
#endif
            }
            if (_canvasSocket)
            {
                orgSocketGameObjectActive = _canvasSocket.gameObject.activeSelf;
            }
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (_canvasSocket)
            {
                _canvasSocket.gameObject.SetActive(orgSocketGameObjectActive);
            }

#if XDREAMER_XR_INTERACTION_TOOLKIT
            if (trackedDeviceGraphicRaycaster)
            {
                trackedDeviceGraphicRaycaster.XDestoryObject();
            }
#endif
        }

        /// <summary>
        /// 显示2D画布
        /// </summary>
        [InteractCmd]
        [Name("显示2D画布")]
        public bool Display2DCanvas() => TryInteract(nameof(Display2DCanvas));

        /// <summary>
        /// 当显示2D画布
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Display2DCanvas))]
        public EInteractResult OnDisplay2DCanvas(InteractData interactData)
        {
            renderMode = RenderMode.ScreenSpaceOverlay;
            return OnDisplayCanvas(renderMode) ? EInteractResult.Success : EInteractResult.Fail;
        }

        /// <summary>
        /// 显示3D画布
        /// </summary>
        [InteractCmd]
        [Name("显示3D画布")]
        public bool Display3DCanvas() => TryInteract(nameof(Display3DCanvas));

        /// <summary>
        /// 当显示3D画布
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Display3DCanvas))]
        public EInteractResult OnDisplay3DCanvas(InteractData interactData)
        {
            renderMode = RenderMode.WorldSpace;
            return OnDisplayCanvas(renderMode) ? EInteractResult.Success : EInteractResult.Fail;
        }

        /// <summary>
        /// 显示画布
        /// </summary>
        [InteractCmd]
        [Name("显示画布")]
        public bool DisplayCanvas() => TryInteract(nameof(DisplayCanvas));

        /// <summary>
        /// 当显示画布
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(DisplayCanvas))]
        public EInteractResult OnDisplayCanvas(InteractData interactData) => OnDisplayCanvas(renderMode) ? EInteractResult.Success : EInteractResult.Fail;

        private bool OnDisplayCanvas(RenderMode renderMode)
        {
            if (!_canvas) return false;
            switch (renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                case RenderMode.ScreenSpaceCamera:
                    {
                        _canvas.gameObject.SetActive(true);
                        return true;
                    }
                case RenderMode.WorldSpace:
                    {
                        if (_canvasSocket && grabbable)
                        {
                            switch (_controlRule)
                            {
                                case EControlRule.ControlSocketGameObjectActive:
                                    {
                                        _canvasSocket.gameObject.SetActive(true);
                                        break;
                                    }
                            }
                            _canvasSocket.Grab(grabbable);
                        }
                        _canvas.gameObject.SetActive(true);
                        return true;
                    }
            }
            return false;
        }

        /// <summary>
        /// 隐藏画布
        /// </summary>
        [InteractCmd]
        [Name("隐藏画布")]
        public bool HideCanvas() => TryInteract(nameof(HideCanvas));

        /// <summary>
        /// 当隐藏画布
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(HideCanvas))]
        public EInteractResult OnHideCanvas(InteractData interactData) => OnHideCanvas(_renderMode) ? EInteractResult.Success : EInteractResult.Fail;

        private bool OnHideCanvas(RenderMode renderMode)
        {
            if (!_canvas) return false;
            switch (renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                case RenderMode.ScreenSpaceCamera:
                    {
                        _canvas.gameObject.SetActive(false);
                        return true;
                    }
                case RenderMode.WorldSpace:
                    {
                        if (_canvasSocket && grabbable)
                        {
                            _canvasSocket.Release(grabbable);
                            switch (_controlRule)
                            {
                                case EControlRule.ControlSocketGameObjectActive:
                                    {
                                        _canvasSocket.gameObject.SetActive(false);
                                        break;
                                    }
                            }
                        }
                        _canvas.gameObject.SetActive(false);
                        return true;
                    }
            }
            return false;
        }

        /// <summary>
        /// 切换画布显隐
        /// </summary>
        [InteractCmd]
        [Name("切换画布显隐")]
        public void SwitchCanvasDisplayHide() => TryInteract(nameof(SwitchCanvasDisplayHide));

        /// <summary>
        /// 当切换画布显隐
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(SwitchCanvasDisplayHide))]
        public EInteractResult OnSwitchCanvasDisplayHide(InteractData interactData)
        {
            if (_canvas)
            {
                if (_canvas.gameObject.activeSelf)
                {
                    if (HideCanvas()) return EInteractResult.Success;
                }
                else
                {
                    if (DisplayCanvas()) return EInteractResult.Success;
                }
            }
            return EInteractResult.Fail;
        }
    }
}
