using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Extension;

namespace XCSJ.PluginVehicleDrive
{
    /// <summary>
    /// 车辆助手扩展
    /// </summary>
    public static class VehicleDriveHelper
    {
    }

    /// <summary>
    /// 车辆驾驶分类
    /// </summary>
    public static class VehicleDriveCategory
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "车辆驾驶";

        /// <summary>
        /// 标题目录
        /// </summary>
        public const string TitleDirectory = Title + CommonCategory.PathSplitLine;

        /// <summary>
        /// 标题前缀
        /// </summary>
        public const string TitlePrefix = Title + CommonCategory.HorizontalLine;

        /// <summary>
        /// 组件菜单路径
        /// </summary>
        public const string ComponentMenuPath = "/" + Title + "/";

        /// <summary>
        /// 组件
        /// </summary>
        public const string Component = TitlePrefix + "组件";

        /// <summary>
        /// 发动机 : 核心部件名称
        /// </summary>
        public const string EngineName = "发动机";

        /// <summary>
        /// 制动器 : 核心部件名称
        /// </summary>
        public const string BrakeName = "制动器";

        /// <summary>
        /// 变速箱 : 核心部件名称
        /// </summary>
        public const string GearBoxName = "变速箱";

        /// <summary>
        /// 转向器 : 核心部件名称
        /// </summary>
        public const string SteerName = "转向器";

        /// <summary>
        /// 燃料 : 核心部件名称
        /// </summary>
        public const string FuelName = "燃料";

        /// <summary>
        /// 灯光控制器 : 核心部件名称
        /// </summary>
        public const string VehicleLightControllerName = "灯光控制器";
    }
}
