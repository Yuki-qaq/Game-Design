using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using XCSJ.EditorXXR.Interaction.Toolkit;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCameras;
using XCSJ.PluginCameras.Controllers;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXBox.Base;
using XCSJ.PluginXBox.Tools;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXRSpaceSolution.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#endif

namespace XCSJ.EditorXRSpaceSolution
{
    /// <summary>
    /// 编辑器XR空间解决方案组手
    /// </summary>
    [LanguageFileOutput]
    public static class EditorXRSpaceSolutionHelper
    {
        /// <summary>
        /// 绘制选中管理器
        /// </summary>
        [LanguageTuple("Select [{0}] Manager", "选中[{0}]管理器")]
        public static void DrawSelectManager()
        {
            if (GUILayout.Button(string.Format("Select [{0}] Manager".Tr(typeof(EditorXRITHelper)), typeof(XRSpaceSolutionManager).Tr())) && XRSpaceSolutionManager.instance)
            {
                Selection.activeObject = XRSpaceSolutionManager.instance;
            }
        }

        /// <summary>
        /// 创建带XBox的XR空间
        /// </summary>
        /// <param name="screenCount"></param>
        /// <param name="onScreenCreated"></param>
        /// <param name="createXROriginFunc"></param>
        /// <param name="onCameraLinkedToScreen"></param>
        /// <param name="xrSpaceName"></param>
        /// <returns></returns>
        public static XRSpace CreateXRSpace_XBox(int screenCount, Action<ScreenGroup, List<VirtualScreen>> onScreenCreated, Func<(MonoBehaviour, CameraController, Transform, Transform)> createXROriginFunc, Action<int, Camera, VirtualScreen> onCameraLinkedToScreen, string xrSpaceName)
        {
            var xrSpace = CreateXRSpace(screenCount, onScreenCreated, () =>
            {
                var (origin, hmd, leftHand, rightHand) = createXROriginFunc();
                if (origin)//XRRig->XROrigin
                {
                    if (leftHand)
                    {
                        var interact = leftHand.XGetOrAddComponent<InteractByXBox>();
                        if (interact)
                        {
                        }
                    }

                    if (rightHand)
                    {
                        var interact = rightHand.XGetOrAddComponent<InteractByXBox>();
                        if (interact)
                        {
                            interact.buttonOfActivate = EXBoxAxisAndButton.RightTrigger;
                            interact.buttonOfSelect = EXBoxAxisAndButton.RightBumper;
                            interact.buttonOfUI = EXBoxAxisAndButton.RightTrigger;
                        }
                    }
                }
                return (origin, hmd, leftHand, rightHand);
            }, onCameraLinkedToScreen, xrSpaceName);
            if (xrSpace)
            {
                var move = xrSpace.XAddComponent<TransformByXBox>();
                move.SetDefaultMove();

                var rotate = xrSpace.XAddComponent<TransformByXBox>();
                rotate.SetDefaultRotateWorldY();
            }
            return xrSpace;
        }

        /// <summary>
        /// 创建XR空间
        /// </summary>
        /// <param name="screenCount"></param>
        /// <param name="onScreenCreated"></param>
        /// <param name="createXROriginFunc"></param>
        /// <param name="onCameraLinkedToScreen"></param>
        /// <param name="xrSpaceName"></param>
        /// <returns></returns>
        public static XRSpace CreateXRSpace(int screenCount, Action<ScreenGroup, List<VirtualScreen>> onScreenCreated, Func<(MonoBehaviour, CameraController, Transform, Transform)> createXROriginFunc, Action<int, Camera, VirtualScreen> onCameraLinkedToScreen, string xrSpaceName)
        {
            if (screenCount < 1)
            {
                Debug.LogWarning("创建XR空间时，构建的屏幕数" + screenCount.ToString() + "不能低于1个！");
                return null;
            }
            if (createXROriginFunc == null)
            {
                Debug.LogWarning("创建XR空间时，构建XR装备的方法不能为空！");
                return null;
            }

#if !XDREAMER_XR_INTERACTION_TOOLKIT
            Debug.LogWarning("插件[" + XRITHelper.Title + "]依赖库缺失,无法创建！");
            return default;
#else
            #region XR原点

            var (origin, hmd, leftHand, rightHand) = createXROriginFunc();
            if (!origin)
            {
                Debug.LogWarning("创建XR空间时，构建XR原点的方法返回值无效！");
                return null;
            }

            #endregion

            var (xrSpace, spaceOffset, screenGroup) = CreateXRSpace(origin);

            #region 屏幕组

            List<VirtualScreen> screens = new List<VirtualScreen>();
            if (screenGroup)
            {
                var screenGroupTransform = screenGroup.transform;
                for (int i = 0; i < screenCount; i++)
                {
                    screens.Add(VirtualScreen.CreateScreen("屏幕" + i.ToString(), screenGroupTransform));
                }
                onScreenCreated?.Invoke(screenGroup, screens);
            }

            #endregion

            #region 相机与虚拟屏幕关联

            if (hmd)
            {
                var cameraParent = hmd.cameraEntityController;
                var cameraParentTransform = cameraParent.transform;

                var camera0 = cameraParent.mainCamera;
                var screen0 = screens[0];

                camera0.XSetName("相机0");
                var cameraProjection1 = camera0.XGetOrAddComponent<CameraProjection>();
                cameraProjection1.screen = screen0;
                onCameraLinkedToScreen?.Invoke(0, camera0, screen0);
                camera0.XGetOrAddComponent<AudioListener>().XSetEnable(true);

                for (int i = 1; i < screenCount; i++)
                {
                    var camera = camera0.gameObject.XCloneObject().GetComponent<Camera>();//通过组件所在游戏对象完成克隆，才能支持撤销

                    camera.XSetName("相机" + i.ToString());
                    camera.transform.XSetTransformParent(cameraParentTransform);
                    camera.transform.XResetLocalPRS();

                    var screen = screens[i];
                    camera.XGetOrAddComponent<CameraProjection>().screen = screen;
                    onCameraLinkedToScreen?.Invoke(i, camera, screen);

                    camera.GetComponent<AudioListener>().XSetEnable(false);
                }

                //刷新相机实体控制器的相机列表
                cameraParent.UpdateCamears();
            }

            #endregion

            return xrSpace;
#endif
        }

        private static Vector3 DefaultScreenGroupLocalPosition { get; } = new Vector3(0, 1, 2);

        /// <summary>
        /// 创建XR空间
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        private static (XRSpace xrSpace, Transform spaceOffset, ScreenGroup screenGroup) CreateXRSpace(MonoBehaviour origin)
        {
            //创建XR空间组件
            var xrSpace = origin.XGetOrAddComponent<XRSpace>();

            //空间偏移
            var spaceOffset = xrSpace.XCreateChild<Transform>("空间偏移");
            xrSpace.Reset();
            xrSpace.spaceOffset = spaceOffset;

            //屏幕组
            var screenGroup = spaceOffset.XCreateChild<ScreenGroup>("屏幕组");
            screenGroup.transform.XSetLocalPosition(DefaultScreenGroupLocalPosition);
            xrSpace.screenGroup = screenGroup;

            return (xrSpace, spaceOffset, screenGroup);
        }

        private static CameraController CreateHMD() => XRSpaceSolutionHelper.CreateHMD<CameraPoseByXRIS>();

        /// <summary>
        /// 创建XR交互空间
        /// </summary>
        /// <param name="xrSpaceName"></param>
        /// <returns></returns>
        public static XRSpace CreateXRIS(string xrSpaceName)
        {
            var origin = EditorXRITHelper.Create(out var hmd, out var leftHand, out var rightHand, out var locomotionSystem, CreateHMD, (c, cc) =>
            {
                c.gameObject.XDestoryObject();
                var cp = cc.cameraEntityController.mainCamera.XGetOrAddComponent<CameraProjection>();
                cp.updateMode = CameraProjection.EUpdateMode.UnityVirtualScreen;//修改默认为Unity,以确保在没有XRIS时退化为普通应用可使用的状态；
            });
            if (!origin) return default;

            // 创建XR空间
            var (xrSpace, spaceOffset, screenGroup) = CreateXRSpace(origin);
            xrSpace.gameObject.XSetUniqueName(xrSpaceName ?? "XR交互空间");

            if (xrSpace)
            {
#if XDREAMER_XR_INTERACTION_TOOLKIT
                var inputActionManager = xrSpace.XGetOrAddComponent<InputActionManager>();
                if (!EditorXRITHelper.SetInputActionManagerPreset(inputActionManager, out var has) && !has)
                {
                    Debug.LogWarning(XROriginOwnerInspector.NoActionAssetText.Tr(typeof(XROriginOwnerInspector)));
                }
#endif
            }

            #region 运动

            if (locomotionSystem)
            {
                var spaceMove = locomotionSystem.XAddComponent<TransformByXRIS>();
                spaceMove._targetTransform = xrSpace.transform;
                spaceMove._transformHandler.actionName = CommonFun.Name(EActionName.SpaceMove);

                var spaceRotate = locomotionSystem.XAddComponent<TransformByXRIS>();
                spaceRotate._targetTransform = xrSpace.transform;
                spaceRotate._transformHandler.actionName = CommonFun.Name(EActionName.SpaceRotate);

                var locomotionMove = locomotionSystem.XAddComponent<LocomotionByXRIS>();
                locomotionMove.actionName = CommonFun.Name(EActionName.SpaceMove);
                var locomotionRotate = locomotionSystem.XAddComponent<LocomotionByXRIS>();
                locomotionRotate.actionName = CommonFun.Name(EActionName.SpaceRotate);

#if XDREAMER_XR_INTERACTION_TOOLKIT
                locomotionMove._actionBasedContinuousMoveProvider = locomotionSystem.XGetOrAddComponent<ActionBasedContinuousMoveProvider>();
                locomotionRotate._actionBasedContinuousTurnProvider = locomotionSystem.XGetOrAddComponent<ActionBasedContinuousTurnProvider>();
#endif
            }

            #endregion

            #region 屏幕组

            if (screenGroup)
            {
                VirtualScreen.CreateScreen("屏幕", screenGroup.transform);

                //与相机透视尝试创建关联
                foreach (var p in hmd.GetComponentsInChildren<CameraProjection>())
                {
                    if (p)
                    {
                        //置空后使其尝试关联新创建的屏幕对象
                        p._screen = default;
                        if (p.screen) { }
                    }
                }
            }

            #endregion

            #region 左手

            if (leftHand)
            {
                var actionName = CommonFun.Name(EActionName.LeftHand);
                var analogController = leftHand.XGetOrAddComponent<AnalogController>();

                var pose = analogController.XGetOrAddComponent<PoseByXRIS>();
                pose._poseHandler.actionName = actionName;
                analogController._poseProvider = pose;

                var lv = EditorTools.EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("XR/左手可视化.prefab");
                if (lv)
                {
                    lv.XSetParent(leftHand);

                    var transform = lv.transform;
                    transform.XResetLocalPRS();

#if XDREAMER_XR_INTERACTION_TOOLKIT
                    leftHand.GetComponent<XRInteractorLineVisual>().XDestoryObject();
#endif
                    var iv = leftHand.XGetOrAddComponent<XRInteractorVisual>();
                    iv._modelRoot = transform.Find("模型");

                    var reticle = transform.Find("标线");
                    if (reticle)
                    {
                        var validReticle = reticle.Find("有效");
                        if (validReticle) iv.validReticle = validReticle.gameObject;

                        var invalidReticle = reticle.Find("无效");
                        if (invalidReticle) iv.invalidReticle = invalidReticle.gameObject;

                        var blockedReticle = reticle.Find("阻挡");
                        if (blockedReticle) iv.blockedReticle = blockedReticle.gameObject;
                    }
                }
            }

            #endregion

            #region 右手

            if (rightHand)
            {
                var actionName = CommonFun.Name(EActionName.RigthHand);
                var analogController = rightHand.XGetOrAddComponent<AnalogController>();

                var pose = analogController.XGetOrAddComponent<PoseByXRIS>();
                pose._poseHandler.actionName = actionName;
                analogController._poseProvider = pose;

                var rv = EditorTools.EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("XR/右手可视化.prefab");
                if (rv)
                {
                    rv.XSetParent(rightHand);

                    var transform = rv.transform;
                    transform.XResetLocalPRS();

#if XDREAMER_XR_INTERACTION_TOOLKIT
                    rightHand.GetComponent<XRInteractorLineVisual>().XDestoryObject();
#endif
                    var iv = rightHand.XGetOrAddComponent<XRInteractorVisual>();
                    iv._modelRoot = transform.Find("模型");

                    var reticle = transform.Find("标线");
                    if (reticle)
                    {
                        var validReticle = reticle.Find("有效");
                        if (validReticle) iv.validReticle = validReticle.gameObject;

                        var invalidReticle = reticle.Find("无效");
                        if (invalidReticle) iv.invalidReticle = invalidReticle.gameObject;

                        var blockedReticle = reticle.Find("阻挡");
                        if (blockedReticle) iv.blockedReticle = blockedReticle.gameObject;
                    }
                }
            }

            #endregion

            return xrSpace;
        }
    }
}
