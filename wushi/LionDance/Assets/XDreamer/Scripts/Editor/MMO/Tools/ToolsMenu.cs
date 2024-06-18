using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMMO;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginMMO.Tools;

namespace XCSJ.EditorMMO.Tools
{
    /// <summary>
    /// 工具库MMO菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        #region 角色

        /// <summary>
        /// 创建网络玩家
        /// </summary>
        /// <param name="gameObject"></param>
        private static void CreateNetPlayer(GameObject gameObject)
        {
            //网络标识组件
            gameObject.XGetOrAddComponent<NetIdentity>();

            //网络玩家组件
            var netPlayer = gameObject.XGetOrAddComponent<NetPlayer>();
            netPlayer.XModifyProperty(() =>
            {
                netPlayer.intervalTime = 0.05f;
            });

            //网络变换组件
            var netTransform = gameObject.XGetOrAddComponent<NetTransform>();
            netTransform.XModifyProperty(() =>
            {
                netTransform._syncMode = NetTransform.ESyncMode.Rigidbody;
            });
        }

        /// <summary>
        /// 空玩家:创建不带任何角色信息的网络玩家游戏对象
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("空玩家")]
        [Tip("创建不带任何角色信息的网络玩家游戏对象", "Create an online player game object without any character information")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(MMOHelperExtension.Character, rootType = typeof(MMOManager), index = -1, groupRule = EToolGroupRule.Create)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(NetPlayer))]
        public static void EmptyPlayer(ToolContext toolContext)
        {
            var player = UnityObjectHelper.CreateGameObject("空玩家");

            //创建网络玩家
            CreateNetPlayer(player);

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, player);
        }

        /// <summary>
        /// 空角色:创建带角色信息但不带角色模型与角色动画器的网络玩家游戏对象
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("空角色")]
        [Tip("创建带角色信息但不带角色模型与角色动画器的网络玩家游戏对象", "Create an online player game object with character information but without character model and character animator")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(MMOHelperExtension.Character, rootType = typeof(MMOManager), index = 0, groupRule = EToolGroupRule.Create)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(NetPlayer))]
        public static void EmptyCharacter(ToolContext toolContext)
        {
            var character = EditorCameras.Characters.Tools.ToolsMenu.EmptyCharacter(toolContext);

            //创建网络玩家
            CreateNetPlayer(character);
        }

        /// <summary>
        /// 胶囊小黄人角色:创建带角色信息且带胶囊小黄人角色模型与角色动画器的网络玩家游戏对象
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("胶囊小黄人角色")]
        [Tip("创建带角色信息且带胶囊小黄人角色模型与角色动画器的网络玩家游戏对象", "Create an online player game object with character information and capsule small yellow man character model and character animator")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(MMOHelperExtension.Character, rootType = typeof(MMOManager), index = 1, groupRule = EToolGroupRule.Create)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(NetPlayer))]
        public static void DummyCharacter(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(MMOHelperExtension.Title + "/胶囊小黄人角色.prefab"));
        }

        /// <summary>
        /// Ethan角色:创建带角色信息且带Ethan角色模型与角色动画器的网络玩家游戏对象
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("Ethan角色")]
        [Tip("创建带角色信息且带Ethan角色模型与角色动画器的网络玩家游戏对象", "Create an online player game object with character information and Ethan character model and character animator")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(MMOHelperExtension.Character, rootType = typeof(MMOManager), index = 2, groupRule = EToolGroupRule.Create)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(NetPlayer))]
        public static void EhtanCharacter(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(MMOHelperExtension.Title + "/Ethan角色.prefab"));
        }

        #endregion

        #region UI

        /// <summary>
        /// MMO房间列表窗口
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("MMO房间列表窗口")]
        [XCSJ.Attributes.Icon(EIcon.Home)]
        [Tool(MMOHelperExtension.UI, nameof(MMOManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(MMOProvider))]
        public static void CreateMMORoomsWindow(ToolContext toolContext)
        {
            EditorXGUI.Tools.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(MMOHelperExtension.Title + "/MMO房间列表窗口.prefab"));
        }

        /// <summary>
        /// MMO服务连接窗口
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("MMO服务连接窗口")]
        [XCSJ.Attributes.Icon(EIcon.Server)]
        [Tool(MMOHelperExtension.UI, nameof(MMOManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(MMOProvider))]
        public static void CreateMMOServerConnectionWindow(ToolContext toolContext)
        {
            EditorXGUI.Tools.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(MMOHelperExtension.Title + "/MMO服务连接窗口.prefab"));
        }

        /// <summary>
        /// MMO玩家生成窗口
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("MMO玩家生成窗口")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(MMOHelperExtension.UI, nameof(MMOManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(MMOProvider))]
        public static void CreateMMOPlayerGenerateWindow(ToolContext toolContext)
        {
            EditorXGUI.Tools.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(MMOHelperExtension.Title + "/MMO玩家生成窗口.prefab"));
        }

        /// <summary>
        /// MMO信息窗口
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("MMO信息窗口")]
        [XCSJ.Attributes.Icon(EIcon.Data)]
        [Tool(MMOHelperExtension.UI, nameof(MMOManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(MMOProvider))]
        public static void CreateMMOInfomationWindow(ToolContext toolContext)
        {
            EditorXGUI.Tools.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(MMOHelperExtension.Title + "/MMO信息窗口.prefab"));
        }

        /// <summary>
        /// MMO用户聊天窗口
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("MMO用户聊天窗口")]
        [XCSJ.Attributes.Icon(EIcon.Chat)]
        [Tool(MMOHelperExtension.UI, nameof(MMOManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(MMOProvider))]
        public static void CreateMMOUserChatWindow(ToolContext toolContext)
        {
            EditorXGUI.Tools.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(MMOHelperExtension.Title + "/MMO用户聊天窗口.prefab"));
        }

        #endregion
    }
}
