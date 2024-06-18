using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Windows.ListViews;
using static XCSJ.PluginMMO.MMOHelper;

namespace XCSJ.PluginMMO.Tools
{
    /// <summary>
    /// MMO提供者
    /// </summary>
    [Name("MMO提供者")]
    [Tip("为MMO控制提供各种接口功能，同时提供MMO底层的各种数据运行信息；", "Provide various interface functions for MMO control, while also providing various data operation information at the bottom of MMO;")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    [RequireComponent(typeof(MMOManager))]
    [DisallowMultipleComponent]
    public class MMOProvider : Interactor, IListHost
    {
        static MMOProvider _instance;

        /// <summary>
        /// 实例:静态实例对象
        /// </summary>
        public static MMOProvider instance
        {
            get
            {
                if (!_instance)
                {
                    //1、从MMO管理器上找
                    var m = MMOManager.instance;
                    if (m) _instance = m.XGetOrAddComponent<MMOProvider>();

                    //2、从全局找
                    if (!_instance) _instance = CommonFun.GetComponentsInChildren<MMOProvider>(true).FirstOrDefault();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 管理器
        /// </summary>
        [Name("管理器")]
        [HideInSuperInspector]
        public MMOManager _manager;

        /// <summary>
        /// 管理器
        /// </summary>
        public MMOManager manager => this.XGetComponent(ref _manager);

        #region 模型宿主

        /// <summary>
        /// 当试图事件
        /// </summary>
        /// <param name="viewInteractData"></param>
        public void OnViewEvent(ViewInteractData viewInteractData)
        {
            switch (viewInteractData.inCmd)
            {
                case nameof(IViewEvent.Select):
                    {
                        var modelEntity = viewInteractData.modelEntity;
                        if (modelEntity is RoomInfo room)
                        {
                            selectedRoom = rooms.Find(r => r.roomGuid == room.roomGuid);
                        }
                        else if (modelEntity is NetPlayer player)
                        {
                            selectedPlayerCharacterPrototype = player;
                        }
                        else if (modelEntity is NetPlayerStartPosition startPosition)
                        {
                            selectedPlayerStartPosition = startPosition;
                        }
                        break;
                    }
                case nameof(IViewEvent.Click):
                    {
                        var thisCmd = GetInCmd(viewInteractData.cmdParam as string) ?? "";
                        switch (thisCmd)
                        {
                            case nameof(EnterDesignatedRoom):
                                {
                                    EnterDesignatedRoom(selectedRoom?.roomGuid);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 是否选择
        /// </summary>
        /// <param name="list"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsSelected(IList list, object model)
        {
            if (list == null || model == null) return false;

            if (list == playerCharacterPrototypes)
            {
                return model == (object)_selectedPlayerCharacterPrototype;
            }
            else if (list == playerStartPositions)
            {
                return model == (object)_selectedPlayerStartPosition;
            }
            return false;
        }

        #endregion

        #region Unity 消息

        private void Awake()
        {
            _instance = this;

            MMOHelper.onNetStateChanged += OnNetStateChanged;
            MMOHelper.onWillStart += OnWillStart;
            MMOHelper.onStartCompleted += OnStartCompleted;
            MMOHelper.onStoped += OnStoped;
            MMOHelper.onEnterRoomCompleted += OnEnterRoomCompleted;
            MMOHelper.onRoomListChanged += OnRoomListChanged;
            MMOHelper.onRoomAddUser += OnRoomAddUser;
            MMOHelper.onRoomRemoveUser += OnRoomRemoveUser;
        }

        private void OnDestroy()
        {
            MMOHelper.onNetStateChanged -= OnNetStateChanged;
            MMOHelper.onWillStart -= OnWillStart;
            MMOHelper.onStartCompleted -= OnStartCompleted;
            MMOHelper.onStoped -= OnStoped;
            MMOHelper.onEnterRoomCompleted -= OnEnterRoomCompleted;
            MMOHelper.onRoomListChanged -= OnRoomListChanged;
            MMOHelper.onRoomAddUser -= OnRoomAddUser;
            MMOHelper.onRoomRemoveUser -= OnRoomRemoveUser;
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            CommonFun.DelayCall(StartMMO);
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            StopMMO();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (manager) { }
        }

        #endregion

        #region MMO事件

        void OnNetStateChanged(ENetState oldState, ENetState newState)
        {
            this.CallUnityObjectPropertyChangedEvent();
            switch (newState)
            {
                case ENetState.SyncRoomed:
                    {
                        _autoCreatePlayer.AutoInvoke(CreatePlayer);
                        break;
                    }
            }
        }

        void OnWillStart()
        {
            startMMOResult = EInteractResult.Wait;
        }

        void OnStartCompleted(EACode code)
        {
            if (code == EACode.Success)
            {
                startMMOResult = EInteractResult.Success;
                _autoEnterDefaultRoom.AutoInvoke(EnterDefaultRoom);
            }
            else
            {
                startMMOResult = EInteractResult.Fail;
            }
        }

        void OnStoped()
        {
            startMMOResult = EInteractResult.None;
        }

        void OnEnterRoomCompleted(EACode code) { }

        void OnRoomListChanged()
        {
            this.CallUnityObjectPropertyChangedEvent(nameof(rooms));
        }

        void OnRoomAddUser(string userGuid)
        {
            this.CallUnityObjectPropertyChangedEvent(nameof(players));
        }

        void OnRoomRemoveUser(string userGuid)
        {
            this.CallUnityObjectPropertyChangedEvent(nameof(players));
        }

        #endregion

        #region MMO网络信息只读

        /// <summary>
        /// 会话编号
        /// </summary>
        [Name("会话编号")]
        public string sessionGuid => MMOHelper.sessionGuid;

        /// <summary>
        /// 本地用户编号
        /// </summary>
        [Name("本地用户编号")]
        public string localUserGuid => MMOHelper.userGuid;

        /// <summary>
        /// 房间编号
        /// </summary>
        [Name("房间编号")]
        public string roomGuid => MMOHelper.roomGuid;

        /// <summary>
        /// 网络状态
        /// </summary>
        [Name("网络状态")]
        public ENetState netState => MMOHelper.netState;

        /// <summary>
        /// Ping值:单位：毫秒ms
        /// </summary>
        [Name("Ping")]
        [Tip("单位：毫秒", "Unit: ms")]
        public double ping => MMOHelper.ping;

        /// <summary>
        /// 返回码
        /// </summary>
        [Name("返回码")]
        public EACode aCode => MMOHelper.aCode;

        /// <summary>
        /// 同步房间的数据已处理数量
        /// </summary>
        [Name("同步房间的数据已处理数量")]
        public double asyncRoomDataHandledCount => MMOHelper.asyncRoomDataHandledCount;

        /// <summary>
        /// 同步房间的数据总数量
        /// </summary>
        [Name("同步房间的数据总数量")]
        public double asyncRoomDataTotalCount => MMOHelper.asyncRoomDataTotalCount;

        /// <summary>
        /// 同步房间进度
        /// </summary>
        [Name("同步房间进度")]
        public double asyncRoomProgress => MMOHelper.asyncRoomProgress;

        #endregion

        #region MMO网络

        private EInteractResult startMMOResult = EInteractResult.None;

        /// <summary>
        /// 启动MMO
        /// </summary>
        [Name("启动MMO")]
        [InteractCmd]
        public void StartMMO() => StartMMO(default);

        /// <summary>
        /// 启动MMO
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(StartMMO))]
        public EInteractResult StartMMO(InteractData interactData)
        {
            if (startMMOResult != EInteractResult.None) return startMMOResult;
            manager.StartMMO();
            return startMMOResult;
        }

        /// <summary>
        /// 停止MMO
        /// </summary>
        [Name("停止MMO")]
        [InteractCmd]
        public void StopMMO() => StopMMO(default);

        /// <summary>
        /// 停止MMO
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(StopMMO))]
        public EInteractResult StopMMO(InteractData interactData)
        {
            manager.StopMMO();
            return EInteractResult.Success;
        }

        /// <summary>
        /// 切换MMO工作模式
        /// </summary>
        [Name("切换MMO工作模式")]
        [InteractCmd]
        public void SwitchMMOWorkMode() => OnSwitchMMOWorkMode(default);

        /// <summary>
        /// 当切换MMO工作模式
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(SwitchMMOWorkMode))]
        public EInteractResult OnSwitchMMOWorkMode(InteractData interactData)
        {
            StopMMO();
            manager.isNetWorkMode = !manager.isNetWorkMode;
            return StartMMO(default);
        }

        #endregion

        #region MMO房间

        /// <summary>
        /// 自动进入默认房间
        /// </summary>
        [Name("自动进入默认房间")]
        [Tip("MMO网络成功启动后，是否自动进入默认房间", "After the MMO network is successfully started, will it automatically enter the default room")]
        public TimeSetting _autoEnterDefaultRoom = new TimeSetting();

        /// <summary>
        /// 已选择的房间
        /// </summary>
        RoomInfo _selectedRoom;

        /// <summary>
        /// 已选择的房间
        /// </summary>
        public RoomInfo selectedRoom
        {
            get => _selectedRoom;
            set
            {
                try
                {
                    _selectedRoom = value;
                }
                finally
                {
                    this.CallUnityObjectPropertyChangedEvent(nameof(selectedRoom));
                }
            }
        }

        /// <summary>
        /// 当前房间
        /// </summary>
        public RoomInfo currentRoom => MMOHelper.roomInfo;

        /// <summary>
        /// 房间列表
        /// </summary>
        public List<RoomInfo> rooms => MMOHelper.rooms;

        /// <summary>
        /// 房间名称
        /// </summary>
        public string roomName 
        {
            get
            {
                var mmo = MMOManager.instance;
                if (mmo) return mmo._roomName;
                return "";
            }
            set
            {
                var mmo = MMOManager.instance;
                if (mmo)
                {
                    mmo._roomName = value;
                    this.CallUnityObjectPropertyChangedEvent(nameof(roomName));
                }
            }
        }

        /// <summary>
        /// 房间列表
        /// </summary>
        public void RoomList() => MMOHelper.RoomList();

        /// <summary>
        /// 进入房间
        /// </summary>
        public void EnterRoom() => MMOHelper.EnterRoom();

        /// <summary>
        /// 退出房间
        /// </summary>
        public void ExitRoom() => MMOHelper.ExitRoom();

        /// <summary>
        /// 进入默认房间
        /// </summary>
        public void EnterDefaultRoom() => manager.EnterDefaultRoom();

        /// <summary>
        /// 进入新建房间
        /// </summary>
        public void EnterNewRoom() => manager.EnterNewRoom();

        /// <summary>
        /// 进入指定房间
        /// </summary>
        [Name("进入指定房间")]
        [InteractCmd]
        public void EnterDesignatedRoom(string roomGuid) => EnterDesignatedRoom(new InteractData(GetInCmdName(nameof(EnterDesignatedRoom)), this, roomGuid));

        /// <summary>
        /// 进入指定房间
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(EnterDesignatedRoom))]
        public EInteractResult EnterDesignatedRoom(InteractData interactData)
        {
            var roomGuid = interactData?.cmdParam as string;
            //Debug.Log("EnterDesignatedRoom: " + roomGuid);
            if (string.IsNullOrEmpty(roomGuid)) return EInteractResult.None;
            manager.EnterDesignatedRoom(roomGuid);
            return EInteractResult.Success;
        }

        #endregion

        #region MMO玩家

        /// <summary>
        /// 自动创建玩家
        /// </summary>
        [Name("自动创建玩家")]
        [Tip("当成功进入MMO房间后，是否自动随机创建玩家角色", "Will player characters be automatically randomly created after successfully entering the MMO room")]
        public TimeSetting _autoCreatePlayer = new TimeSetting();

        /// <summary>
        /// 玩家列表
        /// </summary>
        public List<PlayerInfo> players => MMOHelper.playerList;

        /// <summary>
        /// 玩家：本地用户创建玩家角色
        /// </summary>
        [Group("MMO玩家")]
        [Name("玩家")]
        [Readonly]
        public NetPlayer _player;

        /// <summary>
        /// 玩家：本地用户创建玩家角色
        /// </summary>
        public NetPlayer player
        {
            get => _player;
            private set => _player = value;
        }

        /// <summary>
        /// 创建玩家
        /// </summary>
        /// <param name="playerNickname"></param>
        public void CreatePlayer(string playerNickname)
        {
            selectedPlayerNickname = playerNickname;
            CreatePlayer();
        }

        /// <summary>
        /// 创建玩家：根据当前已选择的玩家角色原型、玩家起始位置、玩家昵称为本地用户创建玩家角色；如果已经创建无法重复创建；
        /// </summary>
        public void CreatePlayer()
        {
            if (player || !MMOHelper.roomSynced)
            {
                return;
            }

            if (!(MMOHelper.localPlayer is MMOHelper.PlayerInfo playerInfo))
            {
                foreach (var p in playerList)
                {
                    Log.Debug(p.guid);
                }
                Log.WarningFormat("在[{0}]({1})生成玩家时，未找到有效的本地玩家信息！",
                    CommonFun.Name(GetType()),
                    GetType().Name);
                return;
            }

            var netPlayer = GetPlayerCharacterPrototype();
            if (!netPlayer)
            {
                Log.WarningFormat("在[{0}]({1})生成玩家时，未找到有效的玩家角色原型！",
                    CommonFun.Name(GetType()),
                    GetType().Name);
                return;
            }

            var netIdentity = netPlayer.netIdentity;
            if (!netIdentity)
            {
                Log.WarningFormat("在[{0}]({1})生成玩家时，玩家角色原型[{2}]游戏对象上未找到组件[3]({4})！",
                    CommonFun.Name(GetType()),
                    GetType().Name,
                    netPlayer.name,
                    CommonFun.Name(typeof(NetIdentity)),
                    typeof(NetIdentity).Name);
                return;
            }

            MMOHelper.AddPlayer(netIdentity, newNetIdentity =>
            {
                //网络玩家
                var player = newNetIdentity.GetComponent<NetPlayer>();

                //起始位置
                var startPosition = GetPlayerStartPosition();
                if (startPosition)
                {
                    var playerTransform = player.transform;
                    var spTransform = startPosition.transform;
                    playerTransform.position = spTransform.position;
                    playerTransform.rotation = spTransform.rotation;
                }

                //更新玩家属性
                player.nickname = GetPlayerNickName();

                //设置玩家
                this.player = player;
            });
        }

        /// <summary>
        /// 重新创建玩家
        /// </summary>
        public void RecreatePlayer()
        {
            DeletePlayer();
            CreatePlayer();
        }

        /// <summary>
        /// 随机重新创建玩家
        /// </summary>
        public void RerandomRecreatePlayer()
        {
            DeletePlayer();
            selectedPlayerCharacterPrototype = null;
            selectedPlayerStartPosition = null;
            CreatePlayer();
        }

        /// <summary>
        /// 删除玩家：删除本地用户对应的玩家角色
        /// </summary>
        public void DeletePlayer()
        {
            MMOHelper.DeletePlayer();
            _player = null;
        }

        #endregion

        #region MMO玩家角色原型

        /// <summary>
        /// 玩家角色原型列表
        /// </summary>
        [Group("MMO玩家角色原型")]
        [Name("玩家角色原型列表")]
        [Array]
        public List<NetPlayer> _playerCharacterPrototypes = new List<NetPlayer>();

        /// <summary>
        /// 玩家角色原型列表
        /// </summary>
        public List<NetPlayer> playerCharacterPrototypes => _playerCharacterPrototypes;

        /// <summary>
        /// 已选择的玩家角色原型
        /// </summary>
        [Name("已选择的玩家角色原型")]
        [ComponentPopup]
        public NetPlayer _selectedPlayerCharacterPrototype;

        /// <summary>
        /// 已选择的玩家角色原型
        /// </summary>
        public NetPlayer selectedPlayerCharacterPrototype
        {
            get => _selectedPlayerCharacterPrototype;
            set { if (!player && value != _selectedPlayerCharacterPrototype) this.XModifyProperty(ref _selectedPlayerCharacterPrototype, value, nameof(selectedPlayerCharacterPrototype)); }
        }

        /// <summary>
        /// 随机玩家角色原型
        /// </summary>
        public NetPlayer RandomPlayerCharacterPrototype()
        {
            var count = _playerCharacterPrototypes.Count;
            selectedPlayerCharacterPrototype = count == 0 ? default : _playerCharacterPrototypes[UnityEngine.Random.Range(0, count)];
            return selectedPlayerCharacterPrototype;
        }

        /// <summary>
        /// 获取玩家角色原型
        /// </summary>
        /// <returns></returns>
        private NetPlayer GetPlayerCharacterPrototype() => _selectedPlayerCharacterPrototype ? _selectedPlayerCharacterPrototype : RandomPlayerCharacterPrototype();

        /// <summary>
        /// 选择玩家角色原型
        /// </summary>
        [Name("选择玩家角色原型")]
        [InteractCmd]
        [InteractCmdFun(nameof(SelectPlayerCharacterPrototype))]
        public EInteractResult SelectPlayerCharacterPrototype(InteractData interactData)
        {
            //Debug.Log(nameof(SelectPlayerCharacterPrototype));
            if (interactData is ViewInteractData viewInteractData)
            {
                var modelEntity = viewInteractData.modelEntity;
                if (modelEntity is NetPlayer player)
                {
                    selectedPlayerCharacterPrototype = player;
                }
            }
            return EInteractResult.Success;
        }

        #endregion

        #region MMO玩家起始位置

        /// <summary>
        /// 玩家起始位置列表
        /// </summary>
        [Group("MMO玩家起始位置")]
        [Name("玩家起始位置列表")]
        [Array]
        public List<NetPlayerStartPosition> _playerStartPositions = new List<NetPlayerStartPosition>();

        /// <summary>
        /// 玩家起始位置列表
        /// </summary>
        public List<NetPlayerStartPosition> playerStartPositions => _playerStartPositions;

        /// <summary>
        /// 已选择的玩家起始位置
        /// </summary>
        [Name("已选择的玩家起始位置")]
        [ComponentPopup]
        public NetPlayerStartPosition _selectedPlayerStartPosition;

        /// <summary>
        /// 已选择的玩家起始位置
        /// </summary>
        public NetPlayerStartPosition selectedPlayerStartPosition
        {
            get => _selectedPlayerStartPosition;
            set { if (!player && value != _selectedPlayerStartPosition) this.XModifyProperty(ref _selectedPlayerStartPosition, value, nameof(selectedPlayerStartPosition)); }
        }

        /// <summary>
        /// 随机玩家起始位置
        /// </summary>
        /// <returns></returns>
        public NetPlayerStartPosition RandomPlayerStartPosition()
        {
            var count = _playerStartPositions.Count;
            selectedPlayerStartPosition = count == 0 ? default : _playerStartPositions[UnityEngine.Random.Range(0, count)];
            return selectedPlayerStartPosition;
        }

        /// <summary>
        /// 获取玩家起始位置
        /// </summary>
        /// <returns></returns>
        private NetPlayerStartPosition GetPlayerStartPosition() => _selectedPlayerStartPosition ? _selectedPlayerStartPosition : RandomPlayerStartPosition();

        /// <summary>
        /// 选择玩家起始位置
        /// </summary>
        [Name("选择玩家起始位置")]
        [InteractCmd]
        [InteractCmdFun(nameof(SelectPlayerStartPosition))]
        public EInteractResult SelectPlayerStartPosition(InteractData interactData)
        {
            //Debug.Log(nameof(SelectPlayerStartPosition));
            if (interactData is ViewInteractData viewInteractData)
            {
                var modelEntity = viewInteractData.modelEntity;
                if (modelEntity is NetPlayerStartPosition startPosition)
                {
                    selectedPlayerStartPosition = startPosition;
                }
            }
            return EInteractResult.Success;
        }

        #endregion

        #region MMO玩家昵称

        /// <summary>
        /// 玩家昵称列表
        /// </summary>
        [Group("MMO玩家昵称")]
        [Name("玩家昵称列表")]
        [Array]
        public List<string> _playerNicknames = new List<string>();

        /// <summary>
        /// 玩家昵称列表
        /// </summary>
        public List<string> playerNicknames => _playerNicknames;

        /// <summary>
        /// 已选择的玩家昵称
        /// </summary>
        [Name("已选择的玩家昵称")]
        public string _selectedPlayerNickname = nameof(XDreamer);

        /// <summary>
        /// 已选择的玩家昵称
        /// </summary>
        public string selectedPlayerNickname
        {
            get => _selectedPlayerNickname;
            set
            {
                if (_selectedPlayerNickname == value) return;
                if (player)
                {
                    player.nickname = value;
                }
                this.XModifyProperty(ref _selectedPlayerNickname, value, nameof(selectedPlayerNickname));
            }
        }

        /// <summary>
        /// 随机玩家昵称
        /// </summary>
        /// <returns></returns>
        public string RandomPlayerNickname()
        {
            var count = _playerNicknames.Count;
            selectedPlayerNickname = count == 0 ? nameof(XDreamer) : _playerNicknames[RandomHelper.Next(0, count)];
            return selectedPlayerNickname;
        }

        /// <summary>
        /// 获取玩家昵称
        /// </summary>
        /// <returns></returns>
        private string GetPlayerNickName() => !string.IsNullOrEmpty(_selectedPlayerNickname) ? _selectedPlayerNickname : RandomPlayerNickname();

        #endregion
    }
}
