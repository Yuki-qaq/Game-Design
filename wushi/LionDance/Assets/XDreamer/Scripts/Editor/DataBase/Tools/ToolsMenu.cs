using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginDataBase;

namespace XCSJ.EditorDataBase.Tools
{
    /// <summary>
    /// 数据库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        #region 结果集窗口

        /// <summary>
        /// 创建结果集窗口
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>       
        [Tool(DBCategory.Title, rootType = typeof(Canvas), groupRule = EToolGroupRule.None)]
        [XCSJ.Attributes.Icon(EIcon.Window)]
        [Name("结果集窗口")]
        [Tip("包含动态列表模式和动态键值模式", "Includes dynamic list mode and dynamic key value mode")]
        [RequireManager(typeof(DBManager))]
        [Manual(typeof(DBManager))]
        public static void CreateResultSetWindow(ToolContext toolContext)
        {
            MenuHelper.DrawMenu("结果集窗口", m =>
            {
                m.AddMenuItem("动态列表模式", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(DBCategory.Title+"/结果集窗口动态列表模式.prefab");
                    if (go)
                    {
                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                    }
                });

                m.AddMenuItem("动态键值模式", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(DBCategory.Title + "/结果集窗口动态键值模式.prefab");
                    if (go)
                    {
                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                    }
                });
            });
        }

        #endregion
    }
}
