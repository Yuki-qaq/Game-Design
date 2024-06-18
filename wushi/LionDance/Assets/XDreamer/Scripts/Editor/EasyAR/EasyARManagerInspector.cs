using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginEasyAR;
using UnityEditor.Callbacks;
using XCSJ.EditorExtension.Base;
using UnityEditor.SceneManagement;
using XCSJ.Attributes;
using XCSJ.Languages;
#if XDREAMER_EASYAR_4_1_0
using easyar;
#endif

namespace XCSJ.EditorEasyAR
{
    /// <summary>
    /// EasyAR检查器
    /// </summary>
    [Name("EasyAR检查器")]
    [CustomEditor(typeof(EasyARManager))]
    public class EasyARManagerInspector : BaseManagerInspector<EasyARManager>
    {
        #region 编译宏

        private static readonly Macro XDREAMER_EASYAR_4_1_0 = new Macro(nameof(XDREAMER_EASYAR_4_1_0), BuildTargetGroup.Standalone, BuildTargetGroup.iOS, BuildTargetGroup.Android);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
            if (TypeHelper.ExistsAndAssemblyFileExists("easyar.EasyARController", true, false))
            {
                EditorHelper.ApplyThirdPartyConfig(typeof(EasyARManagerInspector), UnityPackageVersion);
                XDREAMER_EASYAR_4_1_0.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_EASYAR_4_1_0.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

        /// <summary>
        /// Unity包名
        /// </summary>
        private const string UnityPackageName = "EasyARSenseUnityPlugin_4.1.0.811.unitypackage";

        private const string UnityPackageVersion = "4.1.0";

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            //编辑器运行时不处理
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            InitMacro();

            XDreamerInspector.onCreatedManager += (t) =>
            {
                if (t == typeof(EasyARManager))
                {
                    EditorHelper.OutputMacroLogIfNeed(XDREAMER_EASYAR_4_1_0, typeof(EasyARManager), UnityPackageName);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (EasyARManager.instance)
                    {
                        EditorHelper.OutputMacroLogIfNeed(XDREAMER_EASYAR_4_1_0, typeof(EasyARManager), UnityPackageName);
                    }
                });
            };
        }

#if XDREAMER_EASYAR_4_1_0

        /// <summary>
        /// 平面跟踪器
        /// </summary>
        public SurfaceTrackerFrameFilter surfaceTracker;
#endif

        /// <summary>
        /// 标记
        /// </summary>
        public ImageTargetMB[] markers;

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Start Surface Tracking", "开启表面跟踪")]
        [LanguageTuple("Add Image (Marker) Target Recognition", "添加 图片(Marker)目标识别")]
        [LanguageTuple("Add World Root Node", "添加 世界根节点")]
        [LanguageTuple("Recommend using the EasyAR plugin package version: ", "推荐使用EasyAR插件包版本：")]
        public override void OnInspectorGUI()
        {
            UICommonFun.RichHelpBox(Tr("Recommend using the EasyAR plugin package version: ") + UnityPackageVersion, MessageType.Info);

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_EASYAR_4_1_0, UnityPackageName, typeof(EasyARManager));

            base.OnInspectorGUI();

#if XDREAMER_EASYAR_4_1_0

            #region 绘制开启表面跟踪

            if (!EditorGUILayout.Toggle(Tr("Start Surface Tracking"), (surfaceTracker && surfaceTracker.gameObject.activeSelf)))
            {
                if (surfaceTracker) surfaceTracker.gameObject.SetActive(false);
            }
            else
            {
                if (!surfaceTracker)
                {
                    surfaceTracker = LoadSurfaceTracker(targetObject.easyAR.transform).GetComponent<SurfaceTrackerFrameFilter>();
                    if (surfaceTracker) surfaceTracker.transform.localScale = Vector3.one;
                }
                else
                {
                    surfaceTracker.gameObject.SetActive(true);
                }
            }

            #endregion 绘制开启表面跟踪

#endif

            #region 图片(Marker)目标识别

            EditorGUILayout.Separator();
            if (GUILayout.Button(Tr("Add Image (Marker) Target Recognition")))
            {
                var imageMarker = LoadImageTargetMarker(targetObject.transform);
                if (imageMarker) imageMarker.transform.localScale = Vector3.one;
            }

            #endregion

#if XDREAMER_EASYAR_4_1_0

            #region 添加WorldRoot

            if ((surfaceTracker && surfaceTracker.gameObject.activeSelf) && GUILayout.Button(Tr("Add World Root Node")))
            {
                var worldRoot = GameObject.Find("WorldRoot");
                if (!worldRoot)
                {
                    worldRoot = LoadWorldRoot();
                    if (worldRoot) worldRoot.transform.localScale = Vector3.one;
                }
                targetObject.easyAR.GetComponent<ARSession>().WorldRootController = worldRoot.GetComponent<WorldRootController>();
            }
            
            #endregion 添加WorldRoot
#endif
        }

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(targetObject.easyAR):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (!EditorGUILayout.Toggle((targetObject.easyAR && targetObject.easyAR.gameObject.activeSelf), GUILayout.Width(20)))
                        {
                            if (targetObject.easyAR) targetObject.easyAR.gameObject.SetActive(false);
                        }
                        else
                        {
                            if (!targetObject.easyAR)
                            {
                                serializedProperty.objectReferenceValue = LoadEasyAR();
                            }
                            else
                            {
                                targetObject.easyAR.gameObject.SetActive(true);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }


        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
#if XDREAMER_EASYAR_4_1_0
            if (!targetObject.easyAR)
            {
                //先在当前场景中查找组件
                targetObject.easyAR = EasyARManager.InitEasyAR(null);

                //没找到，那么从预制件加载~
                if (!targetObject.easyAR) targetObject.easyAR = LoadEasyAR();
            }
            if (!targetObject.cameraDevice)
            {
                targetObject.cameraDevice = EasyARManager.InitCameraDevice(null);
            }
#endif

#if XDREAMER_EASYAR_4_1_0
            if (!surfaceTracker)
            {
                surfaceTracker = targetObject.easyAR.GetComponentInChildren<SurfaceTrackerFrameFilter>(true);
            }
#endif
            base.OnEnable();
        }

#if XDREAMER_EASYAR_4_1_0
        private easyar.EasyARController LoadEasyAR()
        {
            var go = UICommonFun.LoadAndInstantiateFromAssets("Assets/EasyAR/Prefabs/Composites/EasyAR_ImageTracker-1.prefab", "EasyAR_Startup", targetObject.transform);
            if (!go) return null;
            go.transform.localScale = Vector3.one;
            GameObject mainCamera = GameObject.Find("Main Camera");
            if (!mainCamera) {
                mainCamera = new GameObject("Main Camera");
                mainCamera.tag = "MainCamera";
                mainCamera.AddComponent<Camera>();
            }
            mainCamera.transform.parent = go.transform;
            Camera camera = mainCamera.GetComponent<Camera>();
            if (camera) camera.clearFlags = CameraClearFlags.SolidColor;
            return go.GetComponent<easyar.EasyARController>();
        }

        private GameObject LoadSurfaceTracker(Transform parent)
        {
            return UICommonFun.LoadAndInstantiateFromAssets("Assets/EasyAR/Prefabs/Primitives/SurfaceTracker.prefab", "SurfaceTracker", parent);
        }

        private GameObject LoadWorldRoot()
        {
            return UICommonFun.LoadAndInstantiateFromAssets("Assets/EasyAR/Prefabs/Primitives/WorldRoot.prefab", "WorldRoot", targetObject.transform);
        }


#else
        private Component LoadEasyAR() => null;

#endif

        private GameObject LoadImageTargetMarker(Transform parent)
        {
            var path = UICommonFun.GetAssetsPath(EFolder.ThirdParty);
            return UICommonFun.LoadAndInstantiateFromAssets(path + "/EasyAR/Prefabs/ImageTargetOfMarker.prefab", "ImageTargetOfMarker", parent);
        }        
    }
}
