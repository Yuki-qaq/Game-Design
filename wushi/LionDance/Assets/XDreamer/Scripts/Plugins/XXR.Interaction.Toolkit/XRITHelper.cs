using UnityEngine;
using XCSJ.Extension;

namespace XCSJ.PluginXXR.Interaction.Toolkit
{
    /// <summary>
    /// XRIT辅助类:XRIT为XR Interaction Toolkit的简写；
    /// </summary>
    public static class XRITHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "XR交互工具包";

        /// <summary>
        /// 标题英文
        /// </summary>
        public const string TitleEN = "XR Interaction Toolkit";

        /// <summary>
        /// 目录名
        /// </summary>
        public const string CategoryName = "XR";

        /// <summary>
        /// XR空间解决方案
        /// </summary>
        public const string SpaceSolution = CategoryName + "空间解决方案";

        /// <summary>
        /// XR交互输入
        /// </summary>
        public const string InteractInput = CategoryName + "交互输入";

        /// <summary>
        /// 跟踪器姿态
        /// </summary>
        public const string TrackerPose = CategoryName + "跟踪器姿态";

        /// <summary>
        /// 检查XR交互工具包是否存在
        /// </summary>
        /// <returns></returns>
        public static bool CheckPackage()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            return true;
#else
            Debug.LogWarning("插件[" + XRITHelper.Title + "]依赖库缺失,无法创建！");
            return false;
#endif
        }
    }

    /// <summary>
    /// XRIS定义
    /// </summary>
    public static class XRISDefine
    {
        /// <summary>
        /// 相机偏移
        /// </summary>
        public const string CameraOffset = "相机偏移";

        /// <summary>
        /// 主相机
        /// </summary>
        public const string MainCamera = "主相机";

        /// <summary>
        /// 左手偏移
        /// </summary>
        public const string LeftOffset = "左手偏移";

        /// <summary>
        /// 左手控制器
        /// </summary>
        public const string LeftController = "左手控制器";

        /// <summary>
        /// 右手偏移
        /// </summary>
        public const string RightOffset = "右手偏移";

        /// <summary>
        /// 右手控制器
        /// </summary>
        public const string RightController = "右手控制器";

        /// <summary>
        /// 运动系统
        /// </summary>
        public const string LocomotionSystem = "运动系统";
    }

    /// <summary>
    /// XR交互工具包分类
    /// </summary>
    public static class XRITCategory
    {
        /// <summary>
        /// 分类
        /// </summary>
        public const string Category = "XR交互工具包";

        /// <summary>
        /// 分类目录
        /// </summary>
        public const string CategoryDirectory = Category + CommonCategory.PathSplitLine;

        /// <summary>
        /// 分类前缀
        /// </summary>

        public const string CategoryPrefix = Category + CommonCategory.HorizontalLine;

        /// <summary>
        /// 交互
        /// </summary>
        public const string Interact = CategoryPrefix + "交互";

        /// <summary>
        /// 运动系统
        /// </summary>
        public const string LocomotionSystem = CategoryPrefix + "运动系统";
    }
}
