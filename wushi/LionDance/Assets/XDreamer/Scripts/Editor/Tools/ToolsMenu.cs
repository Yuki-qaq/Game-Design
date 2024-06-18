using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.ProjectSettings;
using XCSJ.EditorTools.Inspectors;
using XCSJ.Extension;
using XCSJ.Languages;
using XCSJ.PluginCamera;
using XCSJ.PluginCameras;
using XCSJ.PluginCameras.UI;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Draggers.TRSHandles;
using XCSJ.PluginTools.ExplodedViews.Tools;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginTools.Notes;
using XCSJ.PluginTools.Notes.Dimensionings;
using XCSJ.PluginTools.Notes.Tips;
using XCSJ.PluginTools.Points;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginTools.UI;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Widgets;
using XCSJ.PluginXGUI.Windows;

namespace XCSJ.EditorTools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        #region 标注

        /// <summary>
        /// 缩放式热点
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(ToolsCategory.Note, rootType = typeof(ToolsManager), index = 1, groupRule = EToolGroupRule.Create)]
        [Tool(CommonCategory.CommonUse, categoryIndex = 0, rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [XCSJ.Attributes.Icon(EIcon.Hotspot)]
        [Name("热点")]
        [Tip("包含移入热点和点击提示热点", "Including move in hotspot and click prompt hotspot")]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(Tip))]
        public static void CreateHotspot(ToolContext toolContext)
        {
            MenuHelper.DrawMenu("热点菜单", m =>
            {
                m.AddMenuItem("移入提示热点", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/移入提示热点.prefab");
                    if (go)
                    {
                        CreateTip(go.GetComponent<Tip>());
                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                    }
                });

                m.AddMenuItem("点击提示热点", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/点击提示热点.prefab");
                    if (go)
                    {
                        CreateTip(go.GetComponent<Tip>());
                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                    }
                });
            });
        }

        /// <summary>
        /// 静态批注
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(ToolsCategory.Note, rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [Tool(CommonCategory.CommonUse, rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [XCSJ.Attributes.Icon(EIcon.Note)]
        [Name("静态批注")]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(UGUILineNote3D))]
        public static void CreateStaticNote(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/批注-3D.prefab");
            if (go)
            {
                RectTransform button = null;
                if (go.GetComponent<UGUILineNote3D>() is UGUILineNote3D line)
                {
                    button = UGUILineHelper.CreateButtonNote();
                    button.name = "批注文字";
                    button.GetComponentInChildren<Text>().text = "批注文字";
                    button.sizeDelta = new Vector2(80, button.sizeDelta.y);
                    line.rectTransform = button;
                }

                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                if (button) GameObjectUtility.EnsureUniqueNameForSibling(button.gameObject);
            }
        }


        /// <summary>
        /// 动画批注
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(ToolsCategory.Note, rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [Tool(CommonCategory.CommonUse, rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [XCSJ.Attributes.Icon(EIcon.Note)]
        [Name("动画批注")]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(UGUILineNote3D))]
        public static void CreateAnimationNote(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/动画批注.prefab");
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
            }
        }

        /// <summary>
        /// 创建提示
        /// </summary>
        /// <param name="tip"></param>
        public static void CreateTip(Tip tip)
        {
            if (tip)
            {
                var go = EditorXGUI.Tools.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/提示.prefab"), "提示组");
                if (go) 
                {
                    tip._tipPopupAsset._view = go.GetComponentInChildren<TipPopup>();
                }
            }
        }

        /// <summary>
        /// 测量标注
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>       
        [Tool(ToolsCategory.Note, rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [Name("测量标注")]
        [Tip("两点确定一个距离尺寸标注；三个点确定一个角度尺寸标注", "two points to determine a distance dimension;three points to determine an angular dimension")]
        [XCSJ.Attributes.Icon(EIcon.Note)]
        [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
        [Manual(typeof(DistanceDimensioning))]
        public static void CreateLabelWindowTemplate(ToolContext toolContext)
        {
            MenuHelper.DrawMenu("测量标注", m =>
            {
                m.AddMenuItem("距离", () =>
                {
                    EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/距离测量.prefab"));
                });
                m.AddMenuItem("角度", () =>
                {
                    EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/角度测量.prefab"));
                });
            });
        }

        /// <summary>
        /// 测量工具
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(ToolsCategory.Note, rootType = typeof(ToolsManager), groupRule = EToolGroupRule.None)]
        [Tip("可用于测量长度、高度、角度和面积", "It can be used to measure length, height, angle and area")]
        [Name("测量工具")]
        [XCSJ.Attributes.Icon(EIcon.Tool)]
        [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
        [Manual(typeof(SegmentCreater))]
        public static void CreateMeasureTool(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("线段创建器.prefab");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
            var segmentCreater = go.GetComponentInChildren<SegmentCreater>();

            var ugui = XCSJ.EditorXGUI.Tools.ToolsMenu.LoadPrefab_DefaultXGUIPath("测量控制窗口.prefab");
            XCSJ.EditorXGUI.Tools.ToolsMenu.CreateUIInCanvas(() => ugui);
            ugui.transform.position += new Vector3(-50, 0, 0);
            var segmentViewController = ugui.GetComponentInChildren<SegmentCreaterViewController>();
            segmentViewController._segmentCreater = segmentCreater;
        }

        #endregion

        #region 全息影像

        /// <summary>
        /// 全息影像相机路径
        /// </summary>
        public const string HolographicImageCameraPath = "全息图像/全息影像相机.prefab";

        /// <summary>
        /// 全息影像相名称
        /// </summary>
        public const string HolographicImageCameraName = "全息影像相机";

        /// <summary>
        /// 创建全息影像相机
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(CameraCategory.Title, rootType = typeof(CameraManager))]
        [Name(HolographicImageCameraName)]
        [Tip("将视图划分为斜45度的四个区域，并将4个相机图像投影到对应的4个区域中。", "The view is divided into four areas with an inclination of 45 degrees, and four camera images are projected into the corresponding four areas.")]
        [XCSJ.Attributes.Icon(EIcon.AR)]
        [RequireManager(typeof(CameraManager))]
        [Manual(typeof(Camera))]
        public static void CreateHolographicImageCamera(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath(HolographicImageCameraPath));
        }

        #endregion

        #region 拖拽功能

        /// <summary>
        /// 可抓对象列表
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("可抓对象列表")]
        [Tip("可抓对象列表", "Grabbable Object list")]
        [XCSJ.Attributes.Icon(EIcon.Put)]
        [Tool(XGUICategory.ListView, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool(ToolsCategory.GameObject, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        [Manual(typeof(GrabbableList))]
        public static GameObject CreateGrabbableListWindow(ToolContext toolContext)
        {
            var gameObjectList = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("可抓对象列表.prefab");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, gameObjectList);
            return gameObjectList;
        }

        /// <summary>
        /// 创建一键拖拽工具(具有射线拖拽工具、相机视图平面拖拽工具和三轴平移旋转缩放等功能)
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("一键拖拽工具")]
        [Tool(ToolsCategory.SelectionSet, disallowMultiple = true, rootType = typeof(ToolsManager), index = 1)]
        [Tool("常用", disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Tip("具有射线拖拽工具、相机视图平面拖拽工具和三轴平移旋转缩放等功能", "Featuring functions such as ray drag tool, camera view plane drag tool, and three-axis translation, rotation, and scaling")]
        [XCSJ.Attributes.Icon(EIcon.Drag)]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(CombinationDragToolController))]
        public static void CreateDragTool(ToolContext toolContext)
        {
            CreateDragTool(toolContext, true);
        }

        /// <summary>
        /// 为XR空间中的左右手创建一键拖拽工具功能
        /// </summary>
        /// <param name="XRSpaceGameObject"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void CreateDragToolForXRSpace(GameObject XRSpaceGameObject,Transform left,Transform right)
        {
            CreateDragToolForXRSpaceInternal(XRSpaceGameObject, left, "左", new Vector3(-500, -10, 0));
            CreateDragToolForXRSpaceInternal(XRSpaceGameObject, right, "右", new Vector3(500, -10, 0));
        }

        private static void CreateDragToolForXRSpaceInternal(GameObject XRSpaceGameObject, Transform handController, string windowTitle, Vector3 position)
        {
            if (!handController) return;

            var go = CreateDragTool(null, true, viewController =>
            {
                var window = viewController.GetComponentInChildren<UGUIWindow>();
                if (window)
                {
                    window.titleText = windowTitle;
                    window.positon = position;
                }
            });
            if (go)
            {
                go.XSetParent(handController);
                go.transform.XResetLocalPRS();

                // 射线拖拽工具默认使用固定距离拖拽
                foreach (var dragByRay in go.GetComponentsInChildren<DragByRay>())
                {
                    dragByRay._rayDragRule = DragByRay.ERayDragRule.FixedDistance;
                }

                // 将所有鼠标输入改成模拟输入模式
                foreach (var mouseInput in go.GetComponentsInChildren<MouseInput>())
                {
                    mouseInput.SetInputMode(RayCmd.EInputMode.Analog);
                }

                // 设置一键拖拽射线对象为控制器对象
                var cdController = go.GetComponentInChildren<CombinationDragToolController>();
                if (cdController && cdController.analogMouseInputController)
                {
                    // 隐藏原来的射线表示对象
                    var oldRayTransform = cdController.analogMouseInputController.rayTransform;
                    if (oldRayTransform && oldRayTransform != handController.transform)
                    {
                        oldRayTransform.gameObject.SetActive(false);
                    }

                    if (cdController.analogMouseInputController)
                    {
                        cdController.analogMouseInputController.rayTransform = handController.transform;

                        var input = cdController.analogMouseInputController.analogMouseInput;
                        if (input)
                        {
                            // 设置模拟鼠标输入的点和方向对象
                            input._rayGenerater.SetPointDirectionTarget(handController.transform);
                        }

                        // 设置广告牌追踪对象
                        var billboard = cdController.analogMouseInputController.GetComponentInChildren<Billboard>();
                        if (billboard)
                        {
                            billboard._targetTransform = handController.transform;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 射线拖拽工具
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(ToolsCategory.SelectionSet, disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Name("射线拖拽工具")]
        [Tip("创建[选择集修改器]、[射线拖拽摆放工具]和[选择集渲染器]三个组件,实现拖拽功能。", "One click to create [selection set modifier], [selection set drag tool] and [selection set boundary renderer] to realize the drag function.")]
        [XCSJ.Attributes.Icon(EIcon.Drag)]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(DragByRay))]
        public static void CreateSelectionDragByRay(ToolContext toolContext)
        {
            CreateDragTool(toolContext, false, controller => controller.dragByRay = true);
        }

        /// <summary>
        /// 创建相机视图平面拖拽工具
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(ToolsCategory.SelectionSet, disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Name("相机视图平面拖拽工具")]
        [Tip("创建[选择集修改器]、[相机视图平面拖拽工具]和[选择集渲染器]三个组件,实现拖拽功能。", "One click to create [selection set modifier], [selection set drag tool] and [selection set boundary renderer] to realize the drag function.")]
        [XCSJ.Attributes.Icon(EIcon.Drag)]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(DragByCameraView))]
        public static void CreateSelectionDragToolByOnce(ToolContext toolContext)
        {
            CreateDragTool(toolContext, false, controller => controller.dragByCameraView = true);
        }

        /// <summary>
        /// 创建平移旋转缩放工具
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(ToolsCategory.SelectionSet, disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Name("平移旋转缩放工具")]
        [Tip("创建[选择集修改器]、[平移旋转缩放工具]和[选择集渲染器]三个组件,实现拖拽功能。", "One click to create [selection set modifier], [selection set drag tool] and [selection set boundary renderer] to realize the drag function.")]
        [XCSJ.Attributes.Icon(EIcon.Drag)]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(TRSHandle))]
        public static void CreateSelectionTRSTool(ToolContext toolContext)
        {
            CreateDragTool(toolContext, false, controller => controller.trsHandleMove = true);
        }

        private static GameObject CreateDragTool(ToolContext toolContext, bool activeUI = true, Action<CombinationDragToolViewController> onCreateFun = null)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("一键拖拽工具.prefab");
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);

                var viewController = go.GetComponentInChildren<CombinationDragToolViewController>();
                onCreateFun?.Invoke(viewController);

                viewController.gameObject.XSetActive(activeUI);
            }
            return go;
        }

        /// <summary>
        /// 创建渲染纹理相机
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(ToolsCategory.Renderer, disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Name(RenderTextureCamera.Title)]
        [Tip("用于表现选择游戏对象的效果", "Used to represent the effect of selecting game objects")]
        [XCSJ.Attributes.Icon(EIcon.Camera)]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(RenderTextureCamera))]
        public static RenderTextureCamera CreateRenderTextureCamera(ToolContext toolContext)
        {
            var rt = GameObject.FindObjectOfType<RenderTextureCamera>();
            if (!rt)
            {
                var go = UnityObjectHelper.CreateGameObject(RenderTextureCamera.Title);
                if (go)
                {
                    go.XSetParent(ToolsManager.instance.gameObject);

                    // 相机专用渲染图层
                    XTagManager.AddLayer(RenderTextureCamera.Layer);
                    rt = go.XAddComponent<RenderTextureCamera>();
                    rt.renderCamera.cullingMask = 1 << LayerMask.NameToLayer(RenderTextureCamera.Layer);

                    EditorGUIUtility.PingObject(go);
                }
            }
            return rt;
        }

        #endregion

        /// <summary>
        /// 材质列表
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("材质列表")]
        [XCSJ.Attributes.Icon(EIcon.List)]
        [RequireManager(typeof(XGUIManager))]
        [Tool(XGUICategory.ListView, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Manual(typeof(MaterialList))]
        public static void CreateMaterialList(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("材质列表.prefab"));
        }

        /// <summary>
        /// 爆炸图-带控制界面
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("爆炸图-带控制界面")]
        [XCSJ.Attributes.Icon(EIcon.ExplodedView)]
        [RequireManager(typeof(ToolsExtensionManager))]
        [Tool(ToolsExtensionCategory.Model, rootType = typeof(ToolsExtensionManager), groupRule = EToolGroupRule.None)]
        [Manual(typeof(ExplodedView))]
        public static void CreateExplodedViewWithUI(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("爆炸图.prefab"));
        }
    }
}
