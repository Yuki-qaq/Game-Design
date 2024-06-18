using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension
{
    /// <summary>
    /// XDreamer扩展配置类
    /// </summary>
    [XDreamerPreferences(index = IDRange.Begin + 1)]
    [Name(Product.Name + "-扩展")]
    [Import]
    public class XDreamerExtensionOption : XDreamerOption<XDreamerExtensionOption>
    {
        /// <summary>
        /// 版本，修改时会自动修改编译宏,所有代码重新编译;
        /// </summary>
        [Name("用户版本")]
        [Tip("针对不同层级XDreamer用户定制不同界面的版本;修改后,会自动修改编译宏,所有代码重新编译;", "Customized versions of different interfaces for different levels of xdreamer users; After modification, the compilation macro will be modified automatically and all codes will be recompiled;")]
        [EnumPopup]
        public EXDreamerEdition edition = EXDreamerEdition.Developer;

        /// <summary>
        /// 在文件夹图标上绘制XDreamer
        /// </summary>
        [Name("在文件夹图标上绘制XDreamer")]
        [Tip("在文件夹图标上绘制XDreamer", "Draw XDreamer on the folder icon")]
        public bool drawXDreamerIconOnFoulder = true;

        /// <summary>
        /// 新版本
        /// </summary>
        protected override int newVersion => 2;

        /// <summary>
        /// 当版本已变更
        /// </summary>
        /// <param name="lastVersion"></param>
        protected override void OnVersionChanged(int lastVersion)
        {
            XDreamerEdition.UpdateEditionMacro();
        }

        /// <summary>
        /// 当配置修改时
        /// </summary>
        public override void OnModified()
        {
            base.OnModified();
            XDreamerEdition.UpdateEditionMacro();
        }

        #region 文件夹图标

        [InitializeOnLoadMethod]
        private static void InitFolderIcon()
        {
            XDreamerEditor.onBeforeCompileAllAssets += ClearCache;
            EditorApplication.projectWindowItemOnGUI += DrawXDreamerIcon;
        }

        private static void ClearCache() => drawCache.Clear();

        static Dictionary<string, bool> drawCache = new Dictionary<string, bool>();

        private static void DrawXDreamerIcon(string guid, Rect rect)
        {
            if (!weakInstance.drawXDreamerIconOnFoulder) return;

            if (!drawCache.TryGetValue(guid, out var draw))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                drawCache[guid] = draw = AssetDatabase.IsValidFolder(path) && Path.GetFileName(path).StartsWith(Product.Name);
            }

            if (draw) DrawCustomIcon(guid, rect, UICommonFun.GizmosDefaultIcon);
        }

        private static void DrawCustomIcon(string guid, Rect rect, Texture texture)
        {
            if (texture)
            {
                var min = Mathf.Min(rect.width, rect.height);
                var xy = min * 0.25f;
                var wh = min * 0.5f;
                GUI.DrawTexture(new Rect(rect.x + xy, rect.y + xy, wh, wh), texture);
            }
        }

        #endregion

        #region 层级变量编辑器

        /// <summary>
        /// 层级变量编辑器显示模式
        /// </summary>
        [Name("层级变量编辑器显示模式")]
        [Tip("层级变量编辑器的变量信息的显示模式", "Display mode of variable information in hierarchical variable editing window")]
        public EDisplayMode hierarchyVarEditorWindowDisplayMode = EDisplayMode.List;

        #endregion

        /// <summary>
        /// 自动创建临时脚本
        /// </summary>
        [Name("自动创建临时脚本")]
        [Tip("根据程序执行需要，在临时脚本文件夹中自动创建各种脚本代码文件；创建的代码文件主要用于AOT编译，以解决在某些平台不允许动态创建类型导致各种执行逻辑失效的问题,同时可以提升某些代码的执行效率；", "Automatically create various script code files in the temporary script folder according to program execution needs; The code files created are mainly used for AOT compilation to solve the problem of various execution logic failures caused by not allowing dynamic type creation on certain platforms, while also improving the execution efficiency of certain codes;")]
        public bool autoCreateTmpScripts = true;

        #region 后期处理打包配置列表

        /// <summary>
        /// 后期处理打包配置列表
        /// </summary>
        [Name("后期处理打包配置列表")]
        public List<PostProcessBuildConfig> postProcessBuildConfigs = new List<PostProcessBuildConfig>();

        /// <summary>
        /// 展开配置列表
        /// </summary>
        public bool expandConfings = true;

        #endregion
    }

    /// <summary>
    /// 后期处理打包配置：打包完成后，继续处理的配置信息；
    /// </summary>
    [Serializable]
    public class PostProcessBuildConfig
    {
        /// <summary>
        /// 打包平台
        /// </summary>
        [Name("打包平台")]
        public BuildTarget buildTarget = BuildTarget.WebGL;

        /// <summary>
        /// 后期处理打包规则
        /// </summary>
        [Name("后期处理打包规则")]
        public EPostProcessBuildRule postProcessBuildRule = EPostProcessBuildRule.PackagingBuiltProjectFolderAsZip;
    }

    /// <summary>
    /// 后期处理打包规则
    /// </summary>
    public enum EPostProcessBuildRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 启动构建项目
        /// </summary>
        [Name("启动构建项目")]
        StartBuiltProject,

        /// <summary>
        /// 打包构建项目文件夹为ZIP
        /// </summary>
        [Name("打包构建项目文件夹为ZIP")]
        PackagingBuiltProjectFolderAsZip,
    }

    /// <summary>
    /// XDreamer扩展配置类编辑器
    /// </summary>
    [CommonEditor(typeof(XDreamerExtensionOption))]
    public class XDreamerExtensionOptionEditor : XDreamerOptionEditor<XDreamerExtensionOption>
    {
        /// <summary>
        /// 当绘制GUI
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public override bool OnGUI(object obj, FieldInfo fieldInfo)
        {
            var preference = this.preference;
            switch (fieldInfo.Name)
            {
                case nameof(preference.expandConfings): return true;
                case nameof(preference.postProcessBuildConfigs):
                    {
                        DrawPostProcessBuildConfigs(preference);
                        return true;
                    }
            }

            return base.OnGUI(obj, fieldInfo);
        }

        private void DrawPostProcessBuildConfigs(XDreamerExtensionOption option)
        {
            option.expandConfings = UICommonFun.Foldout(option.expandConfings, typeof(XDreamerExtensionOption).TrLabel(nameof(preference.postProcessBuildConfigs)), true, EditorStyles.foldout, null, () => {
                if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonRight, UICommonOption.WH20x16))
                {
                    CommonFun.FocusControl();
                    option.postProcessBuildConfigs.Add(new PostProcessBuildConfig()
                    {
                        buildTarget = EditorUserBuildSettings.activeBuildTarget,
                        postProcessBuildRule = EPostProcessBuildRule.PackagingBuiltProjectFolderAsZip
                    });

                }
            });
            if (!option.expandConfings) return;
            XEditorGUI.DrawList(option.postProcessBuildConfigs,
              () =>
              {
                  GUILayout.Label("打包平台", UICommonOption.Width200);
                  GUILayout.Label("后期处理打包规则");
              }, (config, i) =>
              {
                  config.buildTarget = (BuildTarget)UICommonFun.EnumPopup(config.buildTarget, UICommonOption.Width200);
                  config.postProcessBuildRule = (EPostProcessBuildRule)UICommonFun.EnumPopup(config.postProcessBuildRule);
              });
        }
    }
}
