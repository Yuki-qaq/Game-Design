using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginHTCVive;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXXR.Interaction.Toolkit;

namespace XCSJ.EditorXRSpaceSolution.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        const string XRSpaceTitle = "XR交互空间";

        /// <summary>
        /// 创建同时支持主动与被动立体模式切换的大空间动捕型、支持直连与串流模式切换的OpenXR型、支持飞行与角色相机模式切换的普通型XR交互空间；支持运行时不同型式XR交互空间的相互切换；支持一次打包发布，不同XR运行时平台使用；
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.SpaceSolution)]
        [Name(XRSpaceTitle)]
        [Tip("创建同时支持主动与被动立体模式切换的大空间动捕型、支持直连与串流模式切换的OpenXR型、支持飞行与角色相机模式切换的普通型XR交互空间；支持运行时不同型式XR交互空间的相互切换；支持一次打包发布，不同XR运行时平台使用；", "Create a large space motion capture model that supports both active and passive stereo mode switching, an OpenXR model that supports direct and streaming mode switching, and a regular XR interactive space that supports flight and character camera mode switching; Support the mutual switching of different types of XR interaction spaces during runtime; Supports one-time packaging and publishing for use on different XR runtime platforms;")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(XRSpaceSolutionManager), typeof(StereoViewManager))]
        [Manual(typeof(XRSpaceSolutionManager))]
        public static void CreateXRIS(ToolContext toolContext)
        {
            EditorXRSpaceSolutionHelper.CreateXRIS(XRSpaceTitle);
        }
    }
}
