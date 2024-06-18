using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.PluginLogs.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMechanicalMotion;
using XCSJ.PluginMechanicalMotion.UI;

namespace XCSJ.EditorMechanicalMotion
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 平面运动机构速度设置面板
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("平面运动机构速度设置面板")]
        [XCSJ.Attributes.Icon(EIcon.UI)]
        [Tool(MechanicalMotionCategory.Title, nameof(MechanicalMotionManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(MechanicalMotionManager))]
        [Manual(typeof(PlaneMechanismViewController))]
        public static void CreatePlaneMechanismVelocityPanel(ToolContext toolContext)
        {
            EditorXGUI.Tools.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(MechanicalMotionCategory.Title + "/UI/平面运动机构速度设置面板.prefab"));
        }
    }
}