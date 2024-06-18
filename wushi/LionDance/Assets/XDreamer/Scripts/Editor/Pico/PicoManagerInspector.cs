using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorTools;
using XCSJ.EditorXXR.Interaction.Toolkit;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginPeripheralDevice;
using XCSJ.PluginPico;
using XCSJ.PluginXXR.Interaction.Toolkit;

namespace XCSJ.EditorPico
{
    /// <summary>
    /// PICO管理器检查器
    /// </summary>
    [CustomEditor(typeof(PicoManager))]
    public class PicoManagerInspector : BaseManagerInspector<PicoManager>
    {
        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        private static readonly Macro XDREAMER_PICO = new Macro(nameof(XDREAMER_PICO), BuildTargetGroup.Android);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_ANDROID
            if (TypeHelper.Exists("Unity.XR.PXR.PXR_Manager")
                && UICommonFun.NaturalCompare(PicoHelper.VersionWeak, UnityPackageMinVersion) >= 0)
            {
                XDREAMER_PICO.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_PICO.UndefineWithSelectedBuildTargetGroup();
            }
        }

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
                if (t == typeof(PicoManager))
                {
                    EditorHelper.OutputMacroLogIfNeed(XDREAMER_PICO, typeof(PicoManager), PackageName);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (PicoManager.instance)
                    {
                        EditorHelper.OutputMacroLogIfNeed(XDREAMER_PICO, typeof(PicoManager), PackageName);
                    }
                });
            };
        }

        /// <summary>
        /// 需要的所有依赖包;需要调用包管理器
        /// </summary>
        private const string PackageName = "com.unity.xr.picoxr";

        private const string UnityPackageVersion = "2.3.0";

        private const string UnityPackageMinVersion = "2.3.0";

        #endregion

        private static CategoryList categoryList = null;

        bool allStarted = false;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(PicoManager));

            if (manager && manager.hasAccess)
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    allStarted = PeripheralDeviceInputManager.instance && PeripheralDeviceInputManager.instance.hasAccess && XXRInteractionToolkitManager.instance && XXRInteractionToolkitManager.instance.hasAccess;
                }
                else
                {
                    allStarted = XDreamer.StartManager(typeof(PeripheralDeviceInputManager), typeof(XXRInteractionToolkitManager));
                }
            }
            else
            {
                allStarted = false;
            }
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Please switch to the Android platform to use this plugin!", "请切换至Android平台使用本插件！")]
        [LanguageTuple("One Click Setting PICO Configuration", "一键设置PICO配置")]
        [LanguageTuple("One click PICO configuration completed!", "一键设置PICO配置完成！")]
        [LanguageTuple("Open [XR Plugin Management]", "打开[XR插件管理器]")]
        [LanguageTuple("Please install (or update) to Unity's [{0}] package [{1}] (included) or later!", "请安装(或更新)到Unity的[{0}]包[{1}](含)或更高版本！")]
        [LanguageTuple("Enable Required Plug-ins", "一键启用所需插件")]
        [LanguageTuple("Recommend using the PICO plugin package version: ", "推荐使用PICO插件包版本：")]
        public override void OnInspectorGUI()
        {
            UICommonFun.RichHelpBox(Tr("Recommend using the PICO plugin package version: ") + UnityPackageVersion, MessageType.Info);

#if !UNITY_ANDROID
            UICommonFun.RichHelpBox("<color=red>" + Tr("Please switch to the Android platform to use this plugin!") + "</color>", MessageType.Warning);
#endif

#if !XDREAMER_PICO

            EditorHelper.OpenPackageManagerIfNeedWithButton(XDREAMER_PICO, PackageName);

            UICommonFun.RichHelpBox("<color=red>" + string.Format(Tr("Please install (or update) to Unity's [{0}] package [{1}] (included) or later!"), PicoHelper.Title, UnityPackageMinVersion) + "</color>", MessageType.Warning);
#endif

            base.OnInspectorGUI();

            if (!allStarted && manager.hasAccess)
            {
                if (GUILayout.Button(Tr("Enable Required Plug-ins")))
                {
                    XDreamer.StartManager(typeof(PeripheralDeviceInputManager), typeof(XXRInteractionToolkitManager));
                }
            }

#if XDREAMER_PICO

            if (GUILayout.Button(Tr("One Click Setting PICO Configuration")))
            {
                if(PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel29)
                {
                    PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
                }
                if(PlayerSettings.Android.targetSdkVersion != AndroidSdkVersions.AndroidApiLevelAuto)
                {
                    PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
                }
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

                Debug.Log(Tr("One click PICO configuration completed!"));
            }

#endif
            EditorXRITHelper.DrawOpenXRPluginManagement();

            categoryList.DrawVertical();
        }
    }
}

