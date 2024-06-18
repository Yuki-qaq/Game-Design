using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.EditorExtension.Base.XUnityEditor.PackageManager;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXAR.Foundation;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using static XCSJ.PluginXAR.Foundation.Base.Tools.BaseTracker;
using static XCSJ.PluginXAR.Foundation.Images.Tools.TrackerEvent;

namespace XCSJ.EditorXAR.Foundation
{
    /// <summary>
    /// AR Foundation管理器检查器
    /// </summary>
    [Name("AR Foundation管理器检查器")]
    [CustomEditor(typeof(XARFoundationManager))]
    public class XARFoundationManagerInspector : BaseManagerInspector<XARFoundationManager>
    {
        /// <summary>
        /// 需要的所有依赖包;需要调用包管理器
        /// </summary>
        public const string UnityPackageName = "com.unity.xr.arfoundation";

        private const string UnityPackageVersion = "4.2.0";

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
                if (t == typeof(XARFoundationManager))
                {
                    EditorHelper.OutputMacroLogIfNeed(XDREAMER_AR_FOUNDATION,typeof(XARFoundationManager), UnityPackageName);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (XARFoundationManager.instance)
                    {
                        EditorHelper.OutputMacroLogIfNeed(XDREAMER_AR_FOUNDATION, typeof(XARFoundationManager), UnityPackageName);
                    }
                });
            };
        }

        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        private static readonly Macro XDREAMER_AR_FOUNDATION = new Macro(nameof(XDREAMER_AR_FOUNDATION), BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_2020_3_OR_NEWER
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
            if (TypeHelper.Exists("UnityEngine.XR.ARFoundation.ARSession")
                && PackageInfo_LinkType.GetPackageInfoByPackageName(UnityPackageName) is UnityEditor.PackageManager.PackageInfo packageInfo
                && UICommonFun.NaturalCompare(packageInfo.version, "4.2.0") >= 0)
            {
                XDREAMER_AR_FOUNDATION.DefineIfNoDefined();
            }
            else
#endif
#endif
            {
                XDREAMER_AR_FOUNDATION.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Please install (or update) to Unity's [{0}] package [4.2.0] (included) or later!", "请安装(或更新)到Unity的[{0}]包[4.2.0](含)或更高版本！")]
        [LanguageTuple("The [{1}] package based plug-in extension provided by {0} only supports [Unity2020.3.0] (included) or later versions!", "{0}提供的基于[{1}]包的插件扩展，仅支持[Unity2020.3.0](含)或更高版本中使用！")]
        [LanguageTuple("One Click Configuration", "一键配置")]
        [LanguageTuple("Remove the graphics API because AR Core does not support it: ", "因AR Core不支持，移除图形API：")]
        [LanguageTuple("Since ar core no longer supports 32-bit apps running on 64 bit devices after AR Core SDK 1.19.0, it is necessary to set the script backend to il2cpp mode and enable armv7 and arm64 on the target architecture", "因AR Core SDK 1.19.0之后不再支持64位设备上运行32位App，因此需将脚本后端设置为IL2CPP模式，并在目标架构上启用ARMv7和ARM64")]
        [LanguageTuple("Recommend using the AR Foundation plugin package version: ", "推荐使用AR Foundation插件包版本：")]
        public override void OnInspectorGUI()
        {
            UICommonFun.RichHelpBox(Tr("Recommend using the AR Foundation plugin package version: ") + UnityPackageVersion, MessageType.Info);

            #region 检测是否需要打开包管理器

#if UNITY_2020_3_OR_NEWER
            EditorHelper.OpenPackageManagerIfNeedWithButton(XDREAMER_AR_FOUNDATION, UnityPackageName);

#if !XDREAMER_AR_FOUNDATION
            UICommonFun.RichHelpBox("<color=red>" + string.Format(Tr("Please install (or update) to Unity's [{0}] package [4.2.0] (included) or later!"), XARFoundationHelper.Title) + "</color>", MessageType.Warning);
#endif

#else
            UICommonFun.RichHelpBox("<color=red>" + string.Format(Tr("The [{1}] package based plug-in extension provided by {0} only supports [Unity2020.3.0] (included) or later versions!"), Product.Name, XARFoundationHelper.Title) + "</color>", MessageType.Warning);
#endif

            #endregion

            base.OnInspectorGUI();

            #region 一键配置

#if XDREAMER_AR_FOUNDATION

            if (GUILayout.Button(Tr("One Click Configuration")))
            {
#if UNITY_ANDROID

                //移除Vulkan
                Debug.Log(Tr("Remove the graphics API because AR Core does not support it: ") + GraphicsDeviceType.Vulkan);
                var types = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                if (types.Any(t => t == GraphicsDeviceType.Vulkan))
                {
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, types.Where(t => t != GraphicsDeviceType.Vulkan).ToArray());
                }

                // 将脚本后端设置为IL2CPP模式，并在目标架构上启用ARMv7和ARM64
                Debug.Log(Tr("Since ar core no longer supports 32-bit apps running on 64 bit devices after AR Core SDK 1.19.0, it is necessary to set the script backend to il2cpp mode and enable armv7 and arm64 on the target architecture"));
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
#endif
            }

#endif

            #endregion

            DrawDetailInfos();
        }

        /// <summary>
        /// 跟踪器列表
        /// </summary>
        [Name("跟踪器列表")]
        [Tip("当前场景中所有的跟踪器对象", "All tracker objects in the current scene")]
        private static bool _display = true;

        /// <summary>
        /// 跟踪器
        /// </summary>
        [Name("跟踪器")]
        [Tip("跟踪器所在的游戏对象；本项只读；", "The game object where the tracker is located; This item is read-only;")]
        public bool tracker;

        /// <summary>
        /// 变换同步规则
        /// </summary>
        [Name("变换同步规则")]
        [Tip("变换同步规则", "Transform synchronization rule")]
        public bool transformSyncRule;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(tracker)));
            GUILayout.Label(TrLabel(nameof(transformSyncRule)), UICommonOption.Width120);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(BaseTracker), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as BaseTracker;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //虚拟屏幕
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //屏幕尺寸
                EditorGUI.BeginChangeCheck();
                var rule = UICommonFun.EnumPopup(component._transformSyncRule, UICommonOption.Width120);
                if (EditorGUI.EndChangeCheck())
                {
                    component.transformSyncRule = (ETransformSyncRule)rule;
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }

    /// <summary>
    /// 跟踪器回调事件绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(TrackerCallbackEvent))]
    public class CameraControllerCallbackEventDrawer : EnumUnityEventDrawer<ETrackEvent>
    {
        /// <summary>
        /// 当绘制枚举
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected override void OnDrawEnum(Rect position, SerializedProperty property, GUIContent label)
        {
            var esp = property.FindPropertyRelative(nameof(TrackerCallbackEvent._trackEvent));
            EditorGUI.BeginChangeCheck();
            var eValue = UICommonFun.EnumPopup(position, PropertyData.GetPropertyData(esp).trLabel, (ETrackEvent)esp.intValue);
            if (EditorGUI.EndChangeCheck())
            {
                esp.intValue = (int)(ETrackEvent)eValue;
            }
        }
    }
}

