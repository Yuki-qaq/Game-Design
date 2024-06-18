using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCameras;
using XCSJ.PluginCameras.Controllers;
using XCSJ.PluginCameras.UI;
using XCSJ.PluginXGUI;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 可抓对象列表贴图渲染器
    /// </summary>
    [Name("可抓对象列表贴图渲染器")]
    [XCSJ.Attributes.Icon(EIcon.Image)]
    [Tool(XGUICategory.Component, rootType = typeof(ToolsManager))]
    [RequireComponent(typeof(GrabbableList))]
    public class GrabbableListTextureRenderer : InteractProvider
    {
        /// <summary>
        /// 一帧渲染贴图数量
        /// </summary>
        [Name("一帧渲染贴图数量")]
        [Min(1)]
        public int _renderCountInOneFrame = 3;

        /// <summary>
        /// 渲染贴图相机
        /// </summary>
        [Name("渲染贴图相机")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RenderTextureCamera _renderTextureCamera;

        /// <summary>
        /// 渲染贴图相机
        /// </summary>
        public RenderTextureCamera renderTextureCamera => this.XGetComponentInChildrenOrGlobal(ref _renderTextureCamera, true);

        /// <summary>
        /// 渲染相机
        /// </summary>
        public Camera renderCamera => renderTextureCamera ? renderTextureCamera.renderCamera : CameraHelperExtension.currentCamera;

        /// <summary>
        /// 图像尺寸
        /// </summary>
        [Name("图像尺寸")]
        public Vector2Int _size = new Vector2Int(GameObjectPhotograph.DefaultTextureWidth, GameObjectPhotograph.DefaultTextureHeight);

        /// <summary>
        /// 拍照视角规则
        /// </summary>
        public enum ETakePhotographViewRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 当前相机视角
            /// </summary>
            [Name("当前相机视角")]
            CurrentCameraView,

            /// <summary>
            /// 变换旋转量
            /// </summary>
            [Name("变换旋转量")]
            TransformRotation,

            /// <summary>
            /// 角度
            /// </summary>
            [Name("角度")]
            Angle,
        }

        /// <summary>
        /// 全局拍照视角规则
        /// </summary>
        [Name("全局拍照视角规则")]
        [Tip("当所拍照的游戏对象上有游戏对象视图信息(GameOjectViewInfoMB)组件时，优先使用该组件提供的拍照视角信息", "When there is a gameobjectviewinfomb component on the photographed game object, the camera viewing angle information provided by the component is preferred")]
        [EnumPopup]
        public ETakePhotographViewRule _takePhotographViewRule = ETakePhotographViewRule.CurrentCameraView;

        /// <summary>
        /// 变换旋转量视角
        /// </summary>
        [Name("变换旋转量视角")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_takePhotographViewRule), EValidityCheckType.NotEqual, ETakePhotographViewRule.TransformRotation)]
        public Transform _transformView;

        /// <summary>
        /// 角度
        /// </summary>
        [Name("角度")]
        [HideInSuperInspector(nameof(_takePhotographViewRule), EValidityCheckType.NotEqual, ETakePhotographViewRule.Angle)]
        public Vector3PropertyValue _anglePropertyValue = new Vector3PropertyValue();

        private GrabbableList grabbableList;

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset()
        {
            if (renderTextureCamera) { }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            grabbableList = GetComponent<GrabbableList>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!renderCamera || _size == Vector2Int.zero)
            {
                enabled = false;
                Debug.LogErrorFormat("【{0}】缺少渲染相机或图像尺寸不能为0！", CommonFun.ObjectToString(this));
                return;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            int count = 0;
            foreach (var m in grabbableList.models)
            {
                if (m is GrabbableModel gm && !gm.texture2D)
                {
                    if (gm.texture2D = RenderGameObject(gm.gameObject))
                    {
                        ++count;
                        if (count > _renderCountInOneFrame)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private Texture2D RenderGameObject(GameObject go)
        {
            if (!go) return null;

            var cameraTransformRecorder = new TransformRecorder();
            cameraTransformRecorder.Record(renderCamera.transform);
            try
            {
                switch (_takePhotographViewRule)
                {
                    case ETakePhotographViewRule.CurrentCameraView:
                        {
                            // 使用新版相机的当前激活相机控制器所在相机作为渲染相机角度参考
                            if (CameraManager.instance && CameraManager.instance.cameraManagerProvider is CameraManagerProvider provider
                                && provider && provider.currentCameraController && provider.currentCameraController.cameraEntityController)
                            {
                                var cam = provider.currentCameraController.cameraEntityController._mainCamera;
                                if (cam)
                                {
                                    renderCamera.transform.rotation = cam.transform.rotation;
                                }
                            }
                            break;
                        }
                    case ETakePhotographViewRule.TransformRotation:
                        {
                            if (_transformView)
                            {
                                renderCamera.transform.rotation = _transformView.rotation;
                            }
                            break;
                        }
                    case ETakePhotographViewRule.Angle:
                        {
                            renderCamera.transform.eulerAngles = _anglePropertyValue.GetValue();
                            break;
                        }
                }
                return GameObjectViewInfoMB.GetTexture(renderCamera, _size, go).ToTexture2D();
            }
            finally
            {
                cameraTransformRecorder.Recover();
                cameraTransformRecorder.Clear();
            }
        }
    }
}


