using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMMO;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginTools.AI.NPCs
{
    /// <summary>
    /// NPC : 非玩家角色
    /// 1、NPC具有【移动】和【与玩家交互】两种基本状态，并在这两种状态中循环；
    /// 2、NPC拥有一组动作列表（例如巡逻、吃饭、睡觉等），代表着NPC具备的技能
    /// 3、【外部命令】为外部产生的交互命令名称, 该交互命令为NPC自身的输入命令名称或NPC动作中的输入交互命令名称。
    /// 4、【与玩家交互】是在NPC在与玩家碰撞时触发；玩家是具有指定的Unity标签属性的游戏对象（默认标签为【Player】）或是网络玩家;与玩家交互通常为对话或买卖等
    /// 5、NPC移动使用Unity【导航网格代理】组件实现
    /// 6、NPC同一时间只能与一个玩家进行交互
    /// </summary>
    [Name("NPC")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(ToolsCategory.AI, rootType = typeof(ToolsManager))]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public class NPC : BaseNavMeshAgent
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        [Group("NPC设置", textEN = "NPC Settings", defaultIsExpanded = false)]
        [Name("显示名称")]
        public string _displayName;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string displayName => !string.IsNullOrEmpty(_displayName) ? _displayName : name;

        /// <summary>
        /// 头像
        /// </summary>
        [Name("头像")]
        public Sprite _headImage;

        /// <summary>
        /// 头像
        /// </summary>
        public Sprite headImage => _headImage;

        #region NPC状态

        #region NPC状态枚举

        /// <summary>
        /// NPC状态
        /// </summary>
        public enum ENPCState
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 开始移动
            /// </summary>
            [Name("开始移动")]
            StartMove,

            /// <summary>
            /// 移动中
            /// </summary>
            [Name("移动中")]
            Moveing,

            /// <summary>
            /// 停止移动
            /// </summary>
            [Name("停止移动")]
            StopMove,

            /// <summary>
            /// 开始与玩家交互
            /// </summary>
            [Name("开始与玩家交互")]
            StartInteractWithPlayer,

            /// <summary>
            /// 与玩家交互中
            /// </summary>
            [Name("与玩家交互中")]
            InteractingWithPlayer,

            /// <summary>
            /// 结束与玩家交互
            /// </summary>
            [Name("结束与玩家交互")]
            StopInteractWithPlayer,
        }

        #endregion

        /// <summary>
        /// NPC状态
        /// </summary>
        [Name("NPC状态")]
        [EnumPopup]
        [Readonly]
        public ENPCState _npcState = ENPCState.StopMove;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 position => transform.position;

        #endregion

        #region 动作

        /// <summary>
        /// 动作列表
        /// </summary>
        [Name("动作列表")]
        [Readonly]
        public List<NPCAction> _npcActions = new List<NPCAction>();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _npcState = ENPCState.StopMove;

            destination = position;
        }

        /// <summary>
        /// 添加动作
        /// </summary>
        /// <param name="npcAction"></param>
        /// <returns></returns>
        public bool AddAction(NPCAction npcAction) => npcAction && _npcActions.AddWithDistinct(npcAction);

        /// <summary>
        /// 移除动作
        /// </summary>
        /// <param name="npcAction"></param>
        /// <returns></returns>
        public bool RemoveAction(NPCAction npcAction) => npcAction && _npcActions.Remove(npcAction);

        /// <summary>
        /// 通过输入命令名称查找动作
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        public NPCAction FindActinByInCmdName(string cmdName) => _npcActions.Find(a => a.ContainsInCmdName(cmdName));

        /// <summary>
        /// 输入命令名称
        /// </summary>
        public override List<string> inCmdNameList
        {
            get
            {
                var list = new List<string>();
                list.AddRange(base.inCmdNameList);
                list.AddRange(inCmdNameListOfAction);
                return list;
            }
        }

        /// <summary>
        /// 输出命令名称
        /// </summary>
        public override List<string> outCmdNameList
        {
            get
            {
                var list = new List<string>();
                list.AddRange(base.outCmdNameList);
                list.AddRange(outCmdNameListOfAction);
                return list;
            }
        }

        /// <summary>
        ///  输入命令列表
        /// </summary>
        public List<string> inCmdListOfAction
        {
            get
            {
                var list = new List<string>();
                foreach (var a in _npcActions)
                {
                    if (a) list.AddRange(a.inCmdList);
                }
                return list;
            }
        }

        /// <summary>
        /// 输入命令名称列表
        /// </summary>
        public List<string> inCmdNameListOfAction
        {
            get
            {
                var list = new List<string>();
                foreach (var a in _npcActions)
                {
                    if (a) list.AddRange(a.inCmdNameList);
                }
                return list;
            }
        }

        /// <summary>
        /// 输出命令列表
        /// </summary>
        public List<string> outCmdListOfAction
        {
            get
            {
                var list = new List<string>();
                foreach (var a in _npcActions)
                {
                    if (a) list.AddRange(a.outCmdList);
                }
                return list;
            }
        }

        /// <summary>
        /// 输出命令名称列表
        /// </summary>
        public List<string> outCmdNameListOfAction
        {
            get
            {
                var list = new List<string>();
                foreach (var a in _npcActions)
                {
                    if (a) list.AddRange(a.outCmdNameList);
                }
                return list;
            }
        }

        private void ExecuteActions(NPCInteractData npcInteractData)
        {
            foreach (var a in _npcActions)
            {
                if(a) a.Execute(npcInteractData);
            }
        }

        #endregion

        #region 移动

        /// <summary>
        /// 目标点距离阈值
        /// </summary>
        [Name("目标点距离阈值")]
        [Range(0.01f, 1)]
        public float _destinationDistanceThreshold = 0.1f;

        /// <summary>
        /// 目标点：NPC移动目标点
        /// </summary>
        public Vector3 destination
        {
            get => _destination;
            set
            {
                _destination = value;

                if (!isInteractingPlayer && !IsArriveDestination(_destination))
                {
                    if (TryInteractInteranal(nameof(StartMove), _destination))
                    {
                        _npcState = ENPCState.StartMove;
                    }
                }
            }
        }
        private Vector3 _destination;

        /// <summary>
        /// NPC移动动作
        /// </summary>
        [Name("NPC移动动作")]
        public NPCAction _moveNPCAction;

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update() => UpdateState();

        private void UpdateState()
        {
            switch (_npcState)
            {
                case ENPCState.StartMove:
                    {
                        if (TryInteractInteranal(nameof(Moveing)))
                        {
                            _npcState = ENPCState.Moveing;
                        }
                        break;
                    }
                case ENPCState.Moveing:
                    {
                        if (IsAgentStop())
                        {
                            if (TryInteractInteranal(nameof(StopMove)))
                            {
                                _npcState = ENPCState.StopMove;
                            }
                        }
                        break;
                    }
                case ENPCState.StopMove:
                    {
                        if (_moveNPCAction && _moveNPCAction.enabled && _moveNPCAction.TryGetDestination(out var result))
                        {
                            destination = result;
                        }
                        break;
                    }
                case ENPCState.StartInteractWithPlayer:
                    {
                        _npcState = ENPCState.InteractingWithPlayer;
                        break;
                    }
                case ENPCState.InteractingWithPlayer:
                    {
                        break;
                    }
                case ENPCState.StopInteractWithPlayer:
                    {
                        _npcState = ENPCState.StopMove;
                        break;
                    }
            }
        }

        private bool IsArriveDestination(Vector3 destination) => (destination - position).sqrMagnitude < _destinationDistanceThreshold * _destinationDistanceThreshold;

        private bool IsAgentStop()
        {
            var org = agent.enabled;
            try
            {
                agent.enabled = true;
                return !agent.pathPending && agent.remainingDistance <= 0.5f;
            }
            finally
            {
                agent.enabled = org;
            }
        }

        #endregion

        #region 交互处理

        private bool TryInteractInteranal(string cmd) => TryInteract(new NPCInteractData(GetInCmdName(cmd), this), out _);

        private bool TryInteractInteranal(string cmd, Vector3 desitination) => TryInteract(new NPCInteractData(this,  desitination, GetOutCmdName(cmd)), out _);

        /// <summary>
        /// 尝试扩展交互：使用NPC所有的动作尝试进行交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        protected override EInteractResult OnExtensionalInteract(InteractData interactData, EInteractResult interactResult)
        {
            foreach (var item in _npcActions)
            {
                if (item.ContainsInCmdName(interactData.cmdName))
                {
                    if (item.TryInteract(interactData.Clone(), out var result))
                    {
                        return result;
                    }
                }
            }
            return base.OnExtensionalInteract(interactData, interactResult);
        }

        #endregion

        #region 移动

        /// <summary>
        /// 开始移动
        /// </summary>
        /// <param name="npcInteractData"></param>
        /// <returns></returns>
        [InteractCmd]
        [Name("开始移动")]
        [InteractCmdFun(nameof(StartMove))]
        public EInteractResult StartMove(NPCInteractData npcInteractData)
        {
            StartMove(npcInteractData.destination);
            ExecuteActions(npcInteractData);
            return EInteractResult.Success;
        }

        /// <summary>
        /// 移动中
        /// </summary>
        /// <param name="npcInteractData"></param>
        /// <returns></returns>
        [InteractCmd]
        [Name("移动中")]
        [InteractCmdFun(nameof(Moveing))]
        public EInteractResult Moveing(NPCInteractData npcInteractData) => EInteractResult.Success;

        /// <summary>
        /// 停止移动
        /// </summary>
        /// <param name="npcInteractData"></param>
        /// <returns></returns>
        [InteractCmd]
        [Name("停止移动")]
        [InteractCmdFun(nameof(StopMove))]
        public EInteractResult StopMove(NPCInteractData npcInteractData)
        {
            StopMove();
            ExecuteActions(npcInteractData);
            return EInteractResult.Success;
        }

        #endregion

        #region 与玩家交互

        /// <summary>
        /// 与玩家交互中
        /// </summary>
        public bool isInteractingPlayer => interactPlayer;

        /// <summary>
        /// 当前交互玩家
        /// </summary>
        public GameObject interactPlayer { get; private set; }

        /// <summary>
        /// 碰撞体触发器
        /// </summary>
        [Group("玩家碰撞设置", textEN = "Player Collision Settings", defaultIsExpanded = false)]
        [Name("碰撞体触发器")]
        public ColliderTrigger _colliderTrigger;

        /// <summary>
        /// 玩家碰撞规则
        /// </summary>
        public enum ECollidePlayerRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 进入时停止移动_退出时继续移动
            /// </summary>
            [Name("进入时停止移动_退出时继续移动")]
            StopMoveOnEnter_And_ContinueMoveOnExit,
        }

        /// <summary>
        /// 玩家碰撞规则
        /// </summary>
        [Name("玩家碰撞规则")]
        [EnumPopup]
        public ECollidePlayerRule _playerRule = ECollidePlayerRule.StopMoveOnEnter_And_ContinueMoveOnExit;

        /// <summary>
        /// 玩家碰撞退出后延迟时长
        /// </summary>
        [Name("玩家碰撞退出后延迟时长")]
        [Range(0, 10)]
        public float _delayTimeWhenPlayerCollisionExit = 3f;

        /// <summary>
        /// 当输入交互
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="interactData"></param>
        protected override void OnInputInteract(InteractObject sender, InteractData interactData)
        {
            base.OnInputInteract(sender, interactData);

            if (_colliderTrigger && _colliderTrigger == interactData.interactor && interactData is ColliderInteractData colliderInteractData) 
            {
                switch (interactData.cmd)
                {
                    case nameof(ColliderTrigger.OnTriggerEnter):
                        {
                            if (!interactPlayer && colliderInteractData.collider)
                            {
                                interactPlayer = colliderInteractData.collider.gameObject;
                                TryInteractInteranal(nameof(OnPlayerCollisionEnter));
                            }
                            break;
                        }
                    case nameof(ColliderTrigger.OnTriggerStay):
                        {
                            if (interactPlayer)
                            {
                                TryInteractInteranal(nameof(OnPlayerCollisionStay));
                            }
                            break;
                        }
                    case nameof(ColliderTrigger.OnTriggerExit):
                        {
                            if (interactPlayer && colliderInteractData.collider && interactPlayer == colliderInteractData.collider.gameObject)
                            {
                                TryInteractInteranal(nameof(OnPlayerCollisionExit));
                                CommonFun.DelayCall(() => interactPlayer = null, _delayTimeWhenPlayerCollisionExit);
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 当玩家碰撞进入
        /// </summary>
        /// <param name="npcInteractData"></param>
        [InteractCmd]
        [Name("当玩家碰撞进入")]
        [InteractCmdFun(nameof(OnPlayerCollisionEnter))]
        public EInteractResult OnPlayerCollisionEnter(NPCInteractData npcInteractData)
        {
            switch (_playerRule)
            {
                case ECollidePlayerRule.StopMoveOnEnter_And_ContinueMoveOnExit:
                    {
                        TryInteractInteranal(nameof(StopMove));
                        break;
                    }
            }
            _npcState = ENPCState.StartInteractWithPlayer;

            ExecuteActions(npcInteractData);
            return EInteractResult.Success;
        }

        /// <summary>
        /// 当玩家碰撞停留
        /// </summary>
        /// <param name="npcInteractData"></param>
        [InteractCmd]
        [Name("当玩家碰撞停留")]
        [InteractCmdFun(nameof(OnPlayerCollisionStay))]
        public EInteractResult OnPlayerCollisionStay(NPCInteractData npcInteractData)
        {
            if (interactPlayer)
            {
                transform.LookAt(interactPlayer.transform, Vector3.up);
            }
            return EInteractResult.Success;
        }

        /// <summary>
        /// 当玩家碰撞退出
        /// </summary>
        /// <param name="npcInteractData"></param>
        [InteractCmd]
        [Name("当玩家碰撞退出")]
        [InteractCmdFun(nameof(OnPlayerCollisionExit))]
        public EInteractResult OnPlayerCollisionExit(NPCInteractData npcInteractData)
        {
            switch (_playerRule)
            {
                case ECollidePlayerRule.StopMoveOnEnter_And_ContinueMoveOnExit:
                    {
                        TryInteractInteranal(nameof(StartMove));
                        break;
                    }
            }
            _npcState = ENPCState.StopInteractWithPlayer;
            ExecuteActions(npcInteractData);
            return EInteractResult.Success;
        }

        #endregion

        #region 外部命令

        /// <summary>
        /// 尝试获取外部命令
        /// </summary>
        /// <param name="npcInteractData"></param>
        /// <returns></returns>
        private bool TryGetOutsideCmd(out NPCInteractData npcInteractData)
        {
            if (_outsideCmds.Count > 0)
            {
                npcInteractData = _outsideCmds[0];
                _outsideCmds.RemoveAt(0);
                return true;
            }
            npcInteractData = default;
            return false;
        }

        private List<NPCInteractData> _outsideCmds = new List<NPCInteractData>();

        /// <summary>
        /// 增加外部命令
        /// </summary>
        /// <param name="npcInteractData"></param>
        /// <returns></returns>
        public bool AddOutsideCmd(NPCInteractData npcInteractData)
        {
            if (npcInteractData.npc == this)
            {
                _outsideCmds.Add(npcInteractData);
                return true;
            }
            return false;
        }

        #endregion

        #region MMO对象       

        /// <summary>
        /// NPC网络数据
        /// </summary>
        [Readonly]
        [Name("NPC网络数据")]
        public NPCNetData _npcNetData = new NPCNetData();

        /// <summary>
        /// NPC网络数据
        /// </summary>
        [Serializable]
        [Import]
        public class NPCNetData
        {
            /// <summary>
            /// 目标：NPC将要移动到的地方
            /// </summary>
            [Json(exportString = true)]
            public Vector3 destination = Vector3.zero;

            /// <summary>
            /// 位置：NPC变换的位置
            /// </summary>
            [Json(exportString = true)]
            public Vector3 position = Vector3.zero;

            /// <summary>
            /// 角度：NPC变换的欧拉角
            /// </summary>
            [Json(exportString = true)]
            public Vector3 rotation = Vector3.zero;

            /// <summary>
            /// 构造函数
            /// </summary>
            public NPCNetData() { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="destination"></param>
            /// <param name="position"></param>
            /// <param name="rotation"></param>
            public NPCNetData(Vector3 destination, Vector3 position, Vector3 rotation)
            {
                this.destination = destination;
                this.position = position;
                this.rotation = rotation;
            }

            /// <summary>
            /// 设置
            /// </summary>
            /// <param name="npcNetData"></param>
            public void Set(NPCNetData npcNetData)
            {
                this.destination = npcNetData.destination;
                this.position = npcNetData.position;
                this.rotation = npcNetData.rotation;
            }
        }

        /// <summary>
        /// 上一次数据
        /// </summary>
        [Readonly]
        [Name("上一次数据")]
        public NPCNetData _lastData =new NPCNetData();

        /// <summary>
        /// 原始数据
        /// </summary>
        [Readonly]
        [Name("原始数据")]
        public NPCNetData _originalData = new NPCNetData();

        /// <summary>
        /// 时间设置
        /// </summary>
        [Name("时间设置")]
        public TimeSetting _timeSetting = new TimeSetting();

        /// <summary>
        /// 当序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool OnSerialize(out string data)
        {
            if ((_timeSetting.NeedTry() && destination != _lastData.destination) || _dirty)
            {
                _dirty = false;

                _npcNetData.destination = destination;
                _npcNetData.position = transform.position;
                _npcNetData.rotation = transform.eulerAngles;
                _lastData.Set(_npcNetData);

                data = JsonHelper.ToJson(_npcNetData);
                return true;
            }
            data = default;
            return false;
        }

        /// <summary>
        /// 当反序列化
        /// </summary>
        /// <param name="data"></param>
        public override void OnDeserialize(Data data)
        {
            version = data.version;

            if (data.IsLocalUserSended())
            {
                _lastData.Set(_npcNetData);
                return;
            }

            var npcNetData = JsonHelper.ToObject<NPCNetData>(data.data);
            if (npcNetData != null)
            {
                _npcNetData.Set(npcNetData);
                destination = npcNetData.destination;
                transform.position = npcNetData.position;
                transform.eulerAngles = npcNetData.rotation;
            }
        }

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            version = 0;
            _originalData.Set(_npcNetData);
            _lastData.Set(_npcNetData);
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            version = 0;

            _npcNetData.Set(_originalData);
        }

        #endregion
    }

    /// <summary>
    /// NPC交互数据
    /// </summary>
    public class NPCInteractData : InteractData<NPCInteractData>
    {
        /// <summary>
        /// NPC
        /// </summary>
        public NPC npc => interactor as NPC;

        /// <summary>
        /// 移动目的地
        /// </summary>
        public Vector3 destination { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public NPCInteractData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public NPCInteractData(string cmdName, NPC npc, params InteractObject[] interactables) : base(cmdName, npc, interactables)
        { 
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="destination"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactables"></param>
        public NPCInteractData(NPC npc, Vector3 destination, string cmdName, params InteractObject[] interactables) : base(cmdName, npc, interactables)
        {
            this.destination = destination;
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="interactData"></param>
        protected override void CopyTo(NPCInteractData interactData)
        {
            interactData.destination = destination;
        }
    }
}
