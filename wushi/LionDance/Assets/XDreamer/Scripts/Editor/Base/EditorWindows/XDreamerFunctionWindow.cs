using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorExtension.Base.EditorWindows
{
    /// <summary>
    /// XDreamer功能窗口
    /// </summary>
    [Name(Title)]
    [Tip("场景相关操作编辑工具", "Scene related operation editing tools")]
    [XCSJ.Attributes.Icon(EIcon.Tool)]
    [XDreamerEditorWindow(TrHelper.DeveloperSpecific_EN)]
    public class XDreamerFunctionWindow : XEditorWindowWithScrollView<XDreamerFunctionWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = Product.Name + "功能窗口";

        /// <summary>
        /// 初始化
        /// </summary>
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();    

        /// <summary>
        /// 绘制GUI时
        /// </summary>
        protected override void OnGUI()
        {
            selectToolbar = UICommonFun.Toolbar(selectToolbar, ENameTip.Image, UICommonOption.Height24);
            base.OnGUI();
        }

        /// <summary>
        /// 绘制带滚动视图的GUI时
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            switch (selectToolbar)
            {
                case EToolbar.CommonFunction:
                    {
                        DrawCommonFunction();
                        break;
                    }
                case EToolbar.XDreamerInfo:
                    {
                        DrawSceneXDreamerInfo();
                        break;
                    }
                case EToolbar.NameCheck:
                    {
                        DrawNameCheck();
                        break;
                    }
            }
        }

        /// <summary>
        /// 被选择的工具条
        /// </summary>
        public EToolbar selectToolbar = EToolbar.CommonFunction;

        /// <summary>
        /// 工具条枚举
        /// </summary>
        [Name("工具条")]
        public enum EToolbar
        {
            /// <summary>
            /// 常用功能
            /// </summary>
            [Name("常用功能")]
            CommonFunction,

            /// <summary>
            /// 名称检查
            /// </summary>
            [Name("名称检查")]
            NameCheck,

            /// <summary>
            /// XDreamer信息
            /// </summary>
            [Name("XDreamer信息")]
            XDreamerInfo,
        }

        #region 常用功能

        [LanguageTuple("Detect Compilation Macros", "检测编译宏")]
        [LanguageTuple("Remove All XDreamer Prefix Compilation Macros", "移除所有XDreamer前缀编译宏")]
        [LanguageTuple("Find Invalid Script Cmponents For GameObject In The Current Scene", "查找当前场景游戏对象的无效脚本组件")]
        [LanguageTuple("Unity Developer Mode - On", "Unity开发者模式-开启")]
        [LanguageTuple("Unity Developer Mode - Off", "Unity开发者模式-关闭")]
        [LanguageTuple("Export Component Manager Information", "导出组件管理器信息")]
        [LanguageTuple("Export relevant information for all component managers in XDreamer", "导出XDreamer中所有组件管理器的相关信息")]
        [LanguageTuple("Save File", "保存文件")]
        private void DrawCommonFunction()
        {
            if (GUILayout.Button(Tr("Detect Compilation Macros"))) MacroAttribute.InvokeMacroMethod();

            if (GUILayout.Button(Tr("Remove All XDreamer Prefix Compilation Macros"))) XDreamerEditor.RemoveAllCompileMacroOfXDreamerPrefix();

            if (GUILayout.Button(Tr("Find Invalid Script Cmponents For GameObject In The Current Scene"))) XDreamerEditor.SearchMissingScriptsInCurrentScene();

#if XDREAMER_EDITION_DEVELOPER //开发者模式

            if (GUILayout.Button(Tr("Unity Developer Mode - On"))) EditorPrefs.SetBool("DeveloperMode", true);
            if (GUILayout.Button(Tr("Unity Developer Mode - Off"))) EditorPrefs.SetBool("DeveloperMode", false);

#endif

            if (GUILayout.Button(CommonFun.TempContent(Tr("Export Component Manager Information"), Tr("Export relevant information for all component managers in XDreamer"))))
            {
                if (string.IsNullOrEmpty(saveInfoPath)) saveInfoPath = defalutSavePath;
                saveInfoPath = EditorUtility.SaveFilePanel(Tr("Save File"), Path.GetDirectoryName(saveInfoPath), DefauleFileName, DefaultFileExt);

                if (!string.IsNullOrEmpty(saveInfoPath))
                {
                    List<CMI> list = new List<CMI>();                    
                    foreach(var type in XDreamer.GetManagerTypesInApp())
                    {
                        var cmi = new CMI();
                        cmi.guid = AttributeCache<GuidAttribute>.Get(type)?.Value ?? "";
                        cmi.name = type.Tr();
                        cmi.description = AttributeCache<TipAttribute>.Get(type)?.language?.languages?.FirstOrDefault() ?? "";
                        cmi.typeFullName = type.FullName;
                        list.Add(cmi);
                    }
                    FileHelper.OutputFile(saveInfoPath, JsonHelper.ToJson(list, true));
                }
            }
        }

        class CMI
        {
            public string guid;
            public string name;
            public string description;
            public string typeFullName;
        };

        #endregion

        #region XDreamer信息

        private const string DefauleFileName = "XDreamer-About";

        private const string DefaultFileExt = ".txt";

        private static string defalutSavePath => Application.dataPath + "/" + DefauleFileName + DefaultFileExt;

        /// <summary>
        /// 保存信息的路径
        /// </summary>
        public string saveInfoPath = "";

        /// <summary>
        /// 是否输出当前场景的组件使用信息
        /// </summary>
        [Name("场景组件信息")]
        [Tip("勾选,输出当前场景的组件使用信息", "Check to output the component usage information of the current scene")]
        public bool saveSceneComponentInfo = true;

        /// <summary>
        /// 是否输出当前目标平台下定义的XDreamer相关的编译宏
        /// </summary>
        [Name("编译宏")]
        [Tip("勾选,输出当前目标平台下定义的XDreamer相关的编译宏;", "Check to output the compiled macros related to xdreamer defined under the current target platform;")]
        public bool saveCompileMacro = true;

        [LanguageTuple("Operation", "操作")]
        [LanguageTuple("Export", "导出")]
        [LanguageTuple("Export relevant information about using XDreamer in the current scene", "导出当前场景使用XDreamer的相关信息")]
        private void DrawSceneXDreamerInfo()
        {
            try
            {
                CommonFun.BeginLayout(false);

                saveSceneComponentInfo = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(saveSceneComponentInfo)), saveSceneComponentInfo);

                saveCompileMacro = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(saveCompileMacro)), saveCompileMacro);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(Tr("Operation"));
                if (GUILayout.Button(CommonFun.TempContent(Tr("Export"), Tr("Export relevant information about using XDreamer in the current scene"))))
                {
                    if (string.IsNullOrEmpty(saveInfoPath)) saveInfoPath = defalutSavePath;
                    saveInfoPath = EditorUtility.SaveFilePanel(Tr("Save File"), Path.GetDirectoryName(saveInfoPath), DefauleFileName, DefaultFileExt);

                    SaveXDreamerInfo(saveInfoPath);
                }

                EditorGUILayout.EndHorizontal();
            }
            finally
            {
                CommonFun.EndLayout();
            }

            CommonFun.BeginLayout(false);
            EditorGUILayout.TextArea(GetXDreamerInfo(), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            CommonFun.EndLayout();
        }

        [LanguageTuple("{0} Version : \t{1}", "{0}版本:\t{1}")]
        [LanguageTuple("\r\nCore Version : \t\t{0}", "\r\n核心版本:\t\t{0}")]
        [LanguageTuple("\r\nUnity Version : \t{0}", "\r\nUnity版本:\t{0}")]
        [LanguageTuple("\r\nUnity Current Version : \t{0}", "\r\nUnity当前版本:\t{0}")]
        [LanguageTuple("\r\nUnity Build Target : \t{0}", "\r\nUnity编译目标:\t{0}")]
        [LanguageTuple("\r\nUnity Standalone Target: \t{0}", "\r\nUnity独立目标:\t{0}")]
        [LanguageTuple("\r\nComponent Information : ", "\r\n组件信息:")]
        [LanguageTuple("Enabled", "启用")]
        [LanguageTuple("\r\nCompile Macro : ", "\r\n编译宏:")]
        private string GetXDreamerInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(Tr("{0} Version : \t{1}"), Product.Name, Product.Version);
            sb.AppendFormat(Tr("\r\nCore Version : \t\t{0}"), Product.coreVersion);
            sb.AppendFormat(Tr("\r\nUnity Version : \t{0}"), Product.UnityVersion);
            sb.AppendFormat(Tr("\r\nUnity Current Version : \t{0}"), Application.unityVersion);
            sb.AppendFormat(Tr("\r\nUnity Build Target : \t{0}"), EditorUserBuildSettings.activeBuildTarget);
            sb.AppendFormat(Tr("\r\nUnity Standalone Target: \t{0}"), EditorUserBuildSettings.selectedStandaloneTarget);

            if (saveSceneComponentInfo && XDreamer.Root)
            {
                var root = XDreamer.Root;

                sb.Append(Tr("\r\nComponent Information : "));
                for (int i = 0; i < root.managerTypeInfos.Count; ++i)
                {
                    var info = root.managerTypeInfos[i];
                    var name = CommonFun.Name(info.type);
                    if (name.Length < 8) name += new string(' ', 8 - name.Length);
                    sb.AppendFormat("\r\n\t{0}\t{1}\t{2}\t{3}", i + 1, name, VersionAttribute.GetVersion(info.type), info.manager ? Tr("Enabled") : "");
                }
            }

            if (saveCompileMacro)
            {
                sb.Append(Tr("\r\nCompile Macro : "));
                var macros = Macro.GetScriptingDefineSymbols(EditorUserBuildSettings.selectedBuildTargetGroup);
                macros.Sort();
                foreach (var df in macros)
                {
                    if (df.StartsWith(XDreamerEditor.CompileMacroPrefix))
                    {
                        sb.AppendFormat("\r\n\t{0}", df);
                    }
                }
            }

            return sb.ToString();
        }

        private void SaveXDreamerInfo(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            FileHelper.OutputFile(path, GetXDreamerInfo());
        }

        #endregion

        #region 名称检查

        /// <summary>
        /// 刷新插件
        /// </summary>
        [Name("刷新插件")]
        [Tip("重新获取当前场景的插件授权信息", "Retrieve the plug-in authorization information of the current scene")]
        [XCSJ.Attributes.Icon(EIcon.Reset)]
        private static XGUIContent refreshGUIContent { get; } = new XGUIContent(typeof(XDreamerInspector), nameof(refreshGUIContent), true);

        private int noWidth = 25;
        private int resetButtonWidth = 80;


        [LanguageTuple("Not Enabled", "未启用")]
        [LanguageTuple("Reset All", "全部重置")]
        [LanguageTuple("GameObject", "游戏对象")]
        [LanguageTuple("Standard Name", "标准名称")]
        [LanguageTuple("Function", "功能")]
        [LanguageTuple("Reset", "重置")]
        private void DrawNameCheck()
        {
            var root = XDreamer.Root;
            if (!root)
            {
                UICommonFun.NotificationLayout(Product.Name + " " + Tr("Not Enabled"));
                return;
            }

            try
            {
                CommonFun.BeginLayout(false);

                if (GUILayout.Button(Tr("Reset All")))
                {
                    CommonFun.GetOrAddComponent<XDreamer>(root.gameObject);
                    root.name = XDreamer.Name;
                    foreach (var info in root.managerTypeInfos)
                    {
                        if (info.manager) info.manager.name = Manager.DefaultName(info.manager.GetType());
                    }
                    UICommonFun.MarkSceneDirty();
                }

                #region 标题

                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                //GUILayout.Label("NO.", GUILayout.Width(noWidth));
                if (GUILayout.Button(refreshGUIContent, GUIStyle.none, GUILayout.Width(noWidth), UICommonOption.Height16))
                {
                    XDreamerInspector.UpdateAllManagers();
                }

                GUILayout.Label(Tr("GameObject"));
                GUILayout.Label(Tr("Standard Name"));
                GUILayout.Label(Tr("Function"), GUILayout.Width(resetButtonWidth));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();

                #endregion

                UICommonFun.BeginHorizontal(true);
                {
                    GUILayout.Label("0", GUILayout.Width(noWidth));
                    EditorGUILayout.ObjectField(root, typeof(XDreamer), true);
                    EditorGUILayout.TextField(XDreamer.Name);

                    EditorGUI.BeginDisabledGroup(root.name == XDreamer.Name || !root.GetComponent<XDreamer>());
                    if (GUILayout.Button(Tr("Reset"), GUILayout.Width(resetButtonWidth)))
                    {
                        root.name = XDreamer.Name;
                        CommonFun.GetOrAddComponent<XDreamer>(root.gameObject);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                UICommonFun.EndHorizontal();

                for (int i = 0; i < root.managerTypeInfos.Count; ++i)
                {
                    var info = root.managerTypeInfos[i];

                    UICommonFun.BeginHorizontal(i % 2 == 1);

                    //"NO."
                    GUILayout.Label((i + 1).ToString(), GUILayout.Width(noWidth));

                    EditorGUILayout.ObjectField(info.manager, typeof(MonoBehaviour), true);

                    var defaultName = Manager.DefaultName(info.manager ? info.manager.GetType() : info.type);

                    EditorGUILayout.TextField(defaultName);

                    EditorGUI.BeginDisabledGroup(!info.manager || info.manager.name == defaultName);
                    if (GUILayout.Button(Tr("Reset"), GUILayout.Width(resetButtonWidth)) && info.manager && defaultName != info.manager.name)
                    {
                        info.manager.name = defaultName;
                        UICommonFun.MarkSceneDirty();
                    }
                    EditorGUI.EndDisabledGroup();

                    UICommonFun.EndHorizontal();
                }

            }
            finally
            {
                CommonFun.EndLayout();
            }
        }

        #endregion
    }
}
