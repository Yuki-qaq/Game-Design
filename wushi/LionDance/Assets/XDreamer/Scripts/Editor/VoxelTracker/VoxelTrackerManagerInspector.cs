using UnityEditor;
using UnityEditor.SceneManagement;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginVoxelTracker;

namespace XCSJ.EditorVoxelTracker
{
    /// <summary>
    /// VoxelTracker检查器
    /// </summary>
    [Name("VoxelTracker检查器")]
    [CustomEditor(typeof(VoxelTrackerManager))]
    public class VoxelTrackerManagerInspector : BaseManagerInspector<VoxelTrackerManager>
    {
        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        private static readonly Macro XDREAMER_VOXELTRACKER = new Macro(nameof(XDREAMER_VOXELTRACKER), BuildTargetGroup.Standalone);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_STANDALONE
            if (TypeHelper.Exists("VoxelStationUtil.VoxelCore"))
            {
                EditorHelper.ApplyThirdPartyConfig(typeof(VoxelTrackerManagerInspector), UnityPackageVersion);
                XDREAMER_VOXELTRACKER.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_VOXELTRACKER.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

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
                if (t == typeof(VoxelTrackerManager))
                {
                    EditorHelper.OutputMacroLogIfNeed(XDREAMER_VOXELTRACKER, typeof(VoxelTrackerManager), UnityPackageName);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (VoxelTrackerManager.instance)
                    {
                        EditorHelper.OutputMacroLogIfNeed(XDREAMER_VOXELTRACKER, typeof(VoxelTrackerManager), UnityPackageName);
                    }
                });
            };
        }

        private const string UnityPackageName = "VoxelSense SDK for Unity v1.4.0.unitypackage";

        private const string UnityPackageVersion = "1.4.0";

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("The VoxelTracker plug-in package is not imported into the current project!", "当前工程未导入VoxelTracker插件包！")]
        [LanguageTuple("Recommend using the VoxelTracker plugin package version: ", "推荐使用VoxelTracker插件包版本：")]
        public override void OnInspectorGUI()
        {
            UICommonFun.RichHelpBox(Tr("Recommend using the VoxelTracker plugin package version: ") + UnityPackageVersion, MessageType.Info);

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_VOXELTRACKER, UnityPackageName, typeof(VoxelTrackerManager));

            base.OnInspectorGUI();

#if XDREAMER_VOXELTRACKER


#else
            UICommonFun.RichHelpBox(Tr("The VoxelTracker plug -in package is not imported into the current project!"), MessageType.Warning);
#endif
        }
    }
}
