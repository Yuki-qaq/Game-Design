using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象集激活
    /// </summary>
    [Name("游戏对象集激活")]
    [XCSJ.Attributes.Icon(EIcon.GameObject)]
    [Tool(ToolsCategory.InteractOther, rootType = typeof(ToolsManager))]
    public class GameObjectsActive : Interactor
    {
        /// <summary>
        /// 游戏对象源
        /// </summary>
        public enum EGameObjectSource
        {
            /// <summary>
            /// 无"
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 游戏对象列表
            /// </summary>
            [Name("游戏对象列表")]
            GameObjectList,

            /// <summary>
            /// 可交互对象
            /// </summary>
            [Name("可交互对象")]
            Interactable,
        }

        /// <summary>
        /// 游戏对象数据源
        /// </summary>
        [Name("游戏对象数据源")]
        [EnumPopup]
        public EGameObjectSource _gameObjectSource = EGameObjectSource.GameObjectList;

        /// <summary>
        /// 游戏对象列表
        /// </summary>
        [Name("游戏对象列表")]
        [HideInSuperInspector(nameof(_gameObjectSource), EValidityCheckType.NotEqual, EGameObjectSource.GameObjectList)]
        public GameObject[] _gameObjects = new GameObject[0];

        /// <summary>
        /// 激活游戏对象集
        /// </summary>
        [InteractCmd]
        [Name("激活游戏对象集")]
        public void ActiveGameObjects() => TryInteract(nameof(ActiveGameObjects));

        /// <summary>
        /// 激活游戏对象集
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(ActiveGameObjects))]
        public EInteractResult ActiveGameObjects(InteractData interactData)
        {
            Operate(EBool.Yes);
            return EInteractResult.Success;
        }

        /// <summary>
        /// 非激活游戏对象集
        /// </summary>
        [InteractCmd]
        [Name("非激活游戏对象集")]
        public void InactiveGameObjects() => TryInteract(nameof(InactiveGameObjects));

        /// <summary>
        /// 非激活游戏对象集
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(InactiveGameObjects))]
        public EInteractResult InactiveGameObjects(InteractData interactData)
        {
            Operate(EBool.No);
            return EInteractResult.Success;
        }

        /// <summary>
        /// 切换激活游戏对象集
        /// </summary>
        [InteractCmd]
        [Name("切换激活游戏对象集")]
        public void SwitchActiveGameObjects() => TryInteract(nameof(SwitchActiveGameObjects));

        /// <summary>
        /// 切换激活游戏对象集
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(SwitchActiveGameObjects))]
        public EInteractResult SwitchActiveGameObjects(InteractData interactData)
        {
            Operate(EBool.Switch);
            return EInteractResult.Success;
        }

        private void Operate(EBool active, InteractData interactData)
        {
            switch (_gameObjectSource)
            {
                case EGameObjectSource.GameObjectList:
                    {
                        Operate(active, _gameObjects);
                        break;
                    }
                case EGameObjectSource.Interactable:
                    {
                        foreach (var item in interactData.interactables)
                        {
                            if (item)
                            {
                                Operate(active, item.gameObject);
                            }
                        }
                        break;
                    }
            }
        }

        private void Operate(EBool active, params GameObject[] gameObjects) 
        {
            foreach (var go in _gameObjects)
            {
                if (!go) continue;

                go.XSetActive(CommonFun.BoolChange(go.activeSelf, active));
            }
        }
    }
}
