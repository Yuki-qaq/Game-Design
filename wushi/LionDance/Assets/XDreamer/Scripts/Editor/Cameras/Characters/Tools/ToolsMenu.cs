using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCameras.Characters;
using XCSJ.PluginCameras.Characters.Tools;
using XCSJ.Languages;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMMO;
using XCSJ.PluginTools.AI;
using XCSJ.PluginTools.AI.NPCs;
using XCSJ.PluginTools.Renderers;

namespace XCSJ.EditorCameras.Characters.Tools
{
    /// <summary>
    /// 工具库角色菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 空角色
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("空角色")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(CharacterCategory.Title, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        [Manual(typeof(XCharacterController))]
        public static GameObject EmptyCharacter(ToolContext toolContext)
        {
            var characterController = CharacterHelper.CreateEmptyCharacter("空角色");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, characterController.gameObject);
            return characterController.gameObject;
        }

        /// <summary>
        /// 胶囊小黄人角色
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("胶囊小黄人角色")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(CharacterCategory.Title, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        [Manual(typeof(XCharacterController))]
        public static GameObject DummyCharacter(ToolContext toolContext)
        {
            //创建带相机控制器的角色控制器
            var characterController = CharacterHelper.CreateCharacterControllerWithCameraController("胶囊小黄人角色");

            //角色模型
            var characterModel = characterController.XCreateChild<Transform>("角色模型");

            //加载角色并设置层级关系
            var character = EditorCharacterHelper.LoadDummy();
            character.XSetParent(characterModel);
            character.transform.XResetLocalPRS();

            //查找角色虚拟眼睛游戏对象
            var eye = CommonFun.GetChildGameObject(character, "Cube");

            //纠正角色相机的目标对象
            characterController.characterCameraController.cameraMainController.cameraTargetController.mainTarget = eye ? eye.transform : character.transform;

            //根据角色调整胶囊碰撞体
            var capsuleCollider = characterController.GetComponent<CapsuleCollider>();
            capsuleCollider.XModifyProperty(() =>
            {
                capsuleCollider.center = new Vector3(0f, 1f, 0f);
                capsuleCollider.radius = 0.5f;
                capsuleCollider.height = 2.0f;
            });

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, characterController.gameObject);
            return characterController.gameObject;
        }

        /// <summary>
        /// Ethan角色
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("Ethan角色")]
        [Tip("该角色作为玩家化身在三维场景中移动与交互", "This character acts as a player avatar to move and interact in a 3D scene")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(CharacterCategory.Title, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        [Manual(typeof(XCharacterController))]
        public static void EthanCharacter(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("角色/Ethan角色.prefab"));
        }

        /// <summary>
        /// Ethan角色(NPC)
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("Ethan角色(NPC)")]
        [Tip("具备巡逻动作的NPC对象", "NPC objects with patrol actions")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(CharacterCategory.Title, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        [Manual(typeof(XCharacterController))]
        public static void EthanCharacterNPC(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("角色/Ethan角色(NPC).prefab"));
        }
    }
}
