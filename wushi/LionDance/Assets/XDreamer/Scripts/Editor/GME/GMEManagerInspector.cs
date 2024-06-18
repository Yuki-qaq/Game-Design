using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginGME;
using XCSJ.PluginGME.Tools;

namespace XCSJ.EditorGME
{
    /// <summary>
    /// GME管理器检查器
    /// </summary>
    [Name("GME管理器检查器")]
    [CustomEditor(typeof(GMEManager))]
    public class GMEManagerInspector : BaseManagerInspector<GMEManager>
    {
        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        private static readonly Macro XDREAMER_GME = new Macro(nameof(XDREAMER_GME), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 版本宏
        /// </summary>
        private static readonly Macro XDREAMER_GME_2_9_9 = new Macro(nameof(XDREAMER_GME_2_9_9), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_STANDALONE
            if (TypeHelper.Exists("GME.ITMGContext"))
            {
                EditorHelper.ApplyThirdPartyConfig(typeof(GMEManagerInspector), UnityPackageVersion);
                XDREAMER_GME.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_GME.UndefineWithSelectedBuildTargetGroup();
                XDREAMER_GME_2_9_9.UndefineWithSelectedBuildTargetGroup();
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
                if (t == typeof(GMEManager))
                {
                    EditorHelper.OutputMacroLogIfNeed(XDREAMER_GME, typeof(GMEManager), UnityPackageName);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (GMEManager.instance)
                    {
                        EditorHelper.OutputMacroLogIfNeed(XDREAMER_GME, typeof(GMEManager), UnityPackageName);
                    }
                });
            };
        }

        private const string UnityPackageName = "XDreamer_Pure_GME_Unity_Plugin_2.9.9.unitypackage";

        private const string UnityPackageVersion = "2.9.9";

        /// <summary>
        /// 启用时
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            var manager = this.manager;
            if (manager && manager.hasAccess)
            {
                manager.XGetOrAddComponent<GMEProvider>();
            }
        }

        /// <summary>
        /// 当检查器绘制
        /// </summary>
        [LanguageTuple("The GME plug-in package is not imported into the current project", "当前工程未导入GME插件包！")]
        [LanguageTuple("Create [GME Provider]", "创建[GME提供者]")]
        [LanguageTuple("Recommend using the GME plugin package version: ", "推荐使用GME插件包版本：")]
        public override void OnInspectorGUI()
        {
            UICommonFun.RichHelpBox(Tr("Recommend using the GME plugin package version: ") + UnityPackageVersion, MessageType.Info);

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_GME, UnityPackageName, typeof(GMEManager));

            base.OnInspectorGUI();

#if XDREAMER_GME

            if(GUILayout.Button(Tr("Create [GME Provider]")))
            {
                manager.XGetOrAddComponent<GMEProvider>();
            }
#else
            UICommonFun.RichHelpBox(Tr("The GME plug-in package is not imported into the current project !"), MessageType.Warning);
#endif

            //DrawDetailInfos();
        }
    }
}
