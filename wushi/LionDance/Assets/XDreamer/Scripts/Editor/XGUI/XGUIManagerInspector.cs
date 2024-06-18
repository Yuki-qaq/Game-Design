using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginVuforia;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorXGUI
{
    /// <summary>
    /// XGUI管理器检查器
    /// </summary>
    [Name("XGUI管理器检查器")]
    [CustomEditor(typeof(XGUIManager), true)]
    public class XGUIManagerInspector : BaseManagerInspector<XGUIManager>
    {

        #region TextMeshPro编译宏

        /// <summary>
        /// 宏
        /// </summary>
        private static readonly Macro XDREAMER_TEXTMESHPRO = new Macro(nameof(XDREAMER_TEXTMESHPRO), BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA);

        private const string UnityPackageName = "TextMeshPro";

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_ISO || UNITY_WSA
            if (TypeHelper.Exists("TMPro.TMP_Text"))
            {
                XDREAMER_TEXTMESHPRO.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_TEXTMESHPRO.UndefineWithSelectedBuildTargetGroup();
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
                if (t == typeof(XGUIManager))
                {
                    EditorHelper.OutputMacroLogIfNeed(XDREAMER_TEXTMESHPRO, typeof(XGUIManager), UnityPackageName);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (VuforiaManager.instance)
                    {
                        EditorHelper.OutputMacroLogIfNeed(XDREAMER_TEXTMESHPRO, typeof(XGUIManager), UnityPackageName);
                    }
                });
            };
        }

        #endregion


        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawSubWindowList();
        }

        /// <summary>
        /// 窗口列表
        /// </summary>
        [Name("窗口列表")]
        [Tip("当前场景中所有的子窗口对象", "All child window objects in the current scene")]
        public bool _display = true;

        /// <summary>
        /// 窗口
        /// </summary>
        [Name("窗口")]
        [Tip("窗口组件所在的游戏对象；本项只读；", "The game object where the window component is located; This item is read-only;")]
        public bool window;

        private void DrawSubWindowList()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();
            {
                // 标题
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    GUILayout.Label("NO.", UICommonOption.Width32);
                    GUILayout.Label(TrLabel(nameof(window)).text);
                }
                EditorGUILayout.EndHorizontal();

                // 子级内容
                int i = 0;
                Type type = null;
                foreach (var component in CommonFun.GetComponentsInChildren<SubWindow>(true))
                {
                    if (type == null) type = component.GetType();

                    UICommonFun.BeginHorizontal(i);
                    {
                        //编号
                        EditorGUILayout.LabelField((++i).ToString(), UICommonOption.Width32);

                        //组件
                        EditorGUILayout.ObjectField(component, type, true);
                    }
                    UICommonFun.EndHorizontal();
                }
            }
            CommonFun.EndLayout();
        }

    }
}
