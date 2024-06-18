using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses;
using XCSJ.PluginPhysicses.Tools.Weapons;

namespace XCSJ.EditorPhysicses
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 物理系统发射器
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("发射器")]
        [XCSJ.Attributes.Icon(EIcon.Mono)]
        [RequireManager(typeof(PhysicsManager))]
        [Manual(typeof(Shooter))]
        [Tool(PhysicsCategory.Title, rootType = typeof(PhysicsManager), groupRule = EToolGroupRule.None)]
        public static void CreateGun(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("物理系统/发射器.prefab"));
        }
    }
}
