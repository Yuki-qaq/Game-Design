using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMMO;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 碰撞触发器:
    /// 碰撞触发事件条件：
    /// 1、双方都有碰撞体
    /// 2、运动的一方必须是刚体（当前对象可不为刚体，为被碰撞对象）
    /// 3、当前碰撞体勾选Trigger触发器（当前对象勾选触发器属性）
    /// </summary>
    [Name("碰撞体触发器")]
    [Tip("碰撞触发事件条件:\n 1、双方都有碰撞体\n 2、运动的一方必须是刚体（当前对象可不为刚体，为被碰撞对象）\n 3、至少一方勾选Trigger触发器（当前对象勾选触发器属性）", "Collision triggering event conditions: \n 1. Both parties have collision bodies \n 2. The moving party must be a rigid body (the current object cannot be a rigid body, it is the object being collided)  \n 3. At least one party should check the Trigger trigger (the trigger attribute of the current object should be checked)")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [RequireManager(typeof(ToolsManager))]
    [Tool(ToolsCategory.InteractCommon, nameof(InteractableVirtual), rootType = typeof(ToolsManager), index = 2)]
    [XCSJ.Attributes.Icon(EIcon.ColliderEnable)]
    public class ColliderTrigger : Interactor
    {
        /// <summary>
        /// 刚体
        /// </summary>
        public Rigidbody rigidbody3D
        {
            get
            {
                if (!_rigidbody3D)
                {
                    _rigidbody3D = GetComponentInParent<Rigidbody>();
                }
                return _rigidbody3D;
            }
        }
        private Rigidbody _rigidbody3D;

        /// <summary>
        /// 当交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteract(InteractData interactData) => EInteractResult.Success;

        #region 碰撞触发

        /// <summary>
        /// 当此碰撞器/刚体开始接触另一个刚体/碰撞器时，调用 <see cref="OnCollisionEnter"/>
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("碰撞进入")]
        public void OnCollisionEnter(Collision collision) => CallCollisionFinish(nameof(OnCollisionEnter), collision);

        /// <summary>
        /// 每当此碰撞器/刚体接触到刚体/碰撞器时， <see cref="OnCollisionStay"/> 将在每一帧被调用一次
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("碰撞停留")]
        public void OnCollisionStay(Collision collision) => CallCollisionFinish(nameof(OnCollisionStay), collision);

        /// <summary>
        /// 当此碰撞器/刚体停止接触另一刚体/碰撞器时调用 <see cref="OnCollisionExit"/>
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("碰撞退出")]
        public void OnCollisionExit(Collision collision) => CallCollisionFinish(nameof(OnCollisionExit), collision);

        /// <summary>
        /// 调用碰撞完成
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="collision"></param>
        protected virtual void CallCollisionFinish(string cmdName, Collision collision)
        {
            if (!_gameObjectComparer.Compare(collision.gameObject)) return;

            CallFinished(cmdName, outCmdName => new ColliderInteractData(collision, outCmdName, this), collision.gameObject.GetComponents<InteractObject>());
        }

        #endregion

        #region 触发器触发

        /// <summary>
        /// 如果另一个碰撞器进入了触发器，则调用 <see cref="OnTriggerEnter"/>
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("触发器进入")]
        public void OnTriggerEnter(Collider other) => CallTriggerFinish(nameof(OnTriggerEnter), other);

        /// <summary>
        /// 对于触动触发器的所有“另一个碰撞器”，<see cref="OnTriggerStay"/> 将在每一帧被调用一次
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("触发器停留")]
        public void OnTriggerStay(Collider other) => CallTriggerFinish(nameof(OnTriggerStay), other);

        /// <summary>
        /// 如果另一个碰撞器停止接触触发器，则调用 <see cref="OnTriggerExit"/>
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("触发器退出")]
        public void OnTriggerExit(Collider other) => CallTriggerFinish(nameof(OnTriggerExit), other);


        /// <summary>
        /// 调用触发完成
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="collider"></param>
        protected virtual void CallTriggerFinish(string cmdName, Collider collider)
        {
            if (!_gameObjectComparer.Compare(collider.gameObject)) return;

            CallFinished(cmdName, outCmdName => new ColliderInteractData(collider, outCmdName, this), collider.GetComponents<InteractObject>());
        }

        #endregion

        #region 鼠标触发

        /// <summary>
        /// 当用户在 GUIElement 或碰撞器上按鼠标按钮时调用 <see cref="OnMouseDown"/>
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("鼠标按下")]
        public void OnMouseDown() => CallFinished(nameof(OnMouseDown));

        /// <summary>
        /// 当用户在 GUIElement 或碰撞器上单击鼠标并保持按住鼠标时调用 <see cref="OnMouseDrag"/>
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("鼠标拖拽")]
        public void OnMouseDrag() => CallFinished(nameof(OnMouseDrag));

        /// <summary>
        /// 当用户松开鼠标按钮时调用 <see cref="OnMouseUp"/>
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("鼠标弹起")]
        public void OnMouseUp() => CallFinished(nameof(OnMouseUp));

        /// <summary>
        /// 当鼠标进入 GUIElement 或碰撞器时调用 <see cref="OnMouseEnter"/>
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("鼠标进入")]
        public void OnMouseEnter() => CallFinished(nameof(OnMouseEnter));

        /// <summary>
        /// 当鼠标停留在 GUIElement 或碰撞器上时每帧都调用 <see cref="OnMouseOver"/>
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("鼠标悬停")]
        public void OnMouseOver() => CallFinished(nameof(OnMouseOver));

        /// <summary>
        /// 当鼠标不再停留在 GUIElement 或碰撞器上时调用 <see cref="OnMouseExit"/>
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("鼠标退出")]
        public void OnMouseExit() => CallFinished(nameof(OnMouseExit));

        /// <summary>
        /// 仅当在同一 GUIElement 或碰撞器上按下鼠标，在松开时调用 <see cref="OnMouseUpAsButton"/>
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("鼠标按下弹起作为按钮")]
        public void OnMouseUpAsButton() => CallFinished(nameof(OnMouseUpAsButton));

        #endregion

        /// <summary>
        /// 游戏对象比较器
        /// </summary>
        [Name("游戏对象比较器")]
        [Tip("符合比较规则的游戏对象才产生碰撞触发", "Only game objects that meet the comparison rules generate collision triggers")]
        public GameObjectComparer _gameObjectComparer = new GameObjectComparer();
    }

    /// <summary>
    /// 游戏对象比较器
    /// </summary>
    [Serializable]
    public class GameObjectComparer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GameObjectComparer() { }

        /// <summary>
        /// 比较数据列表规则
        /// </summary>
        [Name("比较数据列表规则")]
        [EnumPopup]
        public ECompareDataListRule _compareDataListRule = ECompareDataListRule.All;

        /// <summary>
        /// 游戏对象比较规则
        /// </summary>
        public enum EGameObjectCompareRule
        {
            /// <summary>
            /// 游戏对象列表
            /// </summary>
            [Name("游戏对象列表")]
            GameObjectList,

            /// <summary>
            /// 游戏对象标签
            /// </summary>
            [Name("游戏对象标签")]
            GameObjectTags,

            /// <summary>
            /// 游戏对象层
            /// </summary>
            [Name("游戏对象层")]
            GameObjectLayer,

            /// <summary>
            /// 网络本地玩家
            /// </summary>
            [Name("网络本地玩家")]
            NetLocalPlayer,

            /// <summary>
            /// 网络玩家
            /// </summary>
            [Name("网络玩家")]
            NetPlayer,
        }

        /// <summary>
        /// 游戏对象比较信息
        /// </summary>
        [Serializable]
        public class GameObjectCompareInfo
        {
            /// <summary>
            /// 游戏对象比较规则
            /// </summary>
            [Name("游戏对象比较规则")]
            [EnumPopup]
            public EGameObjectCompareRule _collisionCompareRule = EGameObjectCompareRule.GameObjectList;

            /// <summary>
            /// 比较条件
            /// </summary>
            [Name("比较条件")]
            [EnumPopup]
            public ECompereCondition _compereCondition = ECompereCondition.Equal;

            /// <summary>
            /// 游戏对象列表
            /// </summary>
            [Name("游戏对象列表")]
            [HideInSuperInspector(nameof(_collisionCompareRule), EValidityCheckType.NotEqual, EGameObjectCompareRule.GameObjectList)]
            public List<GameObject> _gameObjects = new List<GameObject>();

            /// <summary>
            /// 游戏对象标签列表
            /// </summary>
            [Name("游戏对象标签列表")]
            [HideInSuperInspector(nameof(_collisionCompareRule), EValidityCheckType.NotEqual, EGameObjectCompareRule.GameObjectTags)]
            public List<string> _gameObjectTags = new List<string>();

            /// <summary>
            /// 游戏对象层
            /// </summary>
            [Name("游戏对象层")]
            public int _gameObjectLayer = -1;

            /// <summary>
            /// 比较
            /// </summary>
            /// <param name="gameObject"></param>
            /// <returns></returns>
            public bool Compare(GameObject gameObject)
            {
                if(!gameObject) return false;

                switch (_compereCondition)
                {
                    case ECompereCondition.Equal: return Match(gameObject);
                    case ECompereCondition.NotEqual: return !Match(gameObject);
                    default: return false;
                }
            }

            private bool Match(GameObject gameObject)
            {
                switch (_collisionCompareRule)
                {
                    case EGameObjectCompareRule.GameObjectList: return _gameObjects.Contains(gameObject);
                    case EGameObjectCompareRule.GameObjectTags: return _gameObjectTags.Exists(t => t == gameObject.tag);
                    case EGameObjectCompareRule.GameObjectLayer: return (_gameObjectLayer & gameObject.layer) > 0;
                    case EGameObjectCompareRule.NetLocalPlayer: return gameObject.GetComponent<INetPlayer>() is INetPlayer netPlayer && MMOHelper.localPlayer.guid == netPlayer.userGuid;
                    case EGameObjectCompareRule.NetPlayer: return gameObject.GetComponent<INetPlayer>() != null;
                    default: return false;
                }
            }
        }

        /// <summary>
        /// 碰撞比较信息列表：仅碰撞发生时有效
        /// </summary>
        [Name("碰撞比较信息列表")]
        [Tip("仅碰撞发生时有效", "Only valid when collision occurs")]
        public List<GameObjectCompareInfo> _collisionCompareInfos = new List<GameObjectCompareInfo>();

        /// <summary>
        /// 比较:比较信息列表数据为空集时：【任意】方法返回false,【所有】方法返回true。
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public bool Compare(GameObject gameObject)
        {
            if (!gameObject) return false;

            switch (_compareDataListRule)
            {
                case ECompareDataListRule.All: return _collisionCompareInfos.All(info => info.Compare(gameObject));
                case ECompareDataListRule.Any: return _collisionCompareInfos.Any(info => info.Compare(gameObject));
                default: return false;
            }
        }
    }

    /// <summary>
    /// 碰撞交互数据
    /// </summary>
    public class ColliderInteractData : InteractData<ColliderInteractData>
    {
        /// <summary>
        /// 碰撞
        /// </summary>
        public Collision collision { get; private set; }

        /// <summary>
        /// 碰撞体
        /// </summary>
        public Collider collider { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ColliderInteractData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="collision"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactObject"></param>
        /// <param name="interactObjects"></param>
        public ColliderInteractData(Collision collision, string cmdName, InteractObject interactObject, params InteractObject[] interactObjects) : base(cmdName, interactObject, interactObjects)
        {
            this.collision = collision;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactObject"></param>
        /// <param name="interactObjects"></param>
        public ColliderInteractData(Collider collider, string cmdName, InteractObject interactObject, params InteractObject[] interactObjects) : base(cmdName, interactObject, interactObjects)
        {
            this.collider = collider;
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="interactData"></param>
        protected override void CopyTo(ColliderInteractData interactData)
        {
            interactData.collision = collision;
            interactData.collider = collider;
        }
    }
}
