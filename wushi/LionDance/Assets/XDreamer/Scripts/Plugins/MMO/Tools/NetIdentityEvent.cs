using System;
using UnityEngine;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.Tools
{
    /// <summary>
    /// 网络标识事件
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Event)]
    [Name("网络标识事件")]
    [Tip("用于捕获网络标识组件的各种网络事件", "Used to capture various network events of the network identification component")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    [DisallowMultipleComponent]
    [Tool(MMOHelperExtension.Title, nameof(NetIdentity), index = 0)]
    public class NetIdentityEvent : Interactor
    {
        /// <summary>
        /// 网络标识
        /// </summary>
        [Name("网络标识")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public NetIdentity _netIdentity;

        /// <summary>
        /// 网络标识
        /// </summary>
        public override NetIdentity netIdentity => this.XGetComponentInParentOrParentChildrenOrGlobal(ref _netIdentity);

        /// <summary>
        /// 变量字符串
        /// </summary>
        [Name("变量字符串")]
        [Tip("将网络事件回调的额外参数信息存储到当前变量中", "Store the additional parameter information of the network event callback into the current variable")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _varString;

        /// <summary>
        /// 将值设置到变量
        /// </summary>
        /// <param name="value">值</param>
        protected void SetToVariable(string value) => _varString.TrySetOrAddSetHierarchyVarValue(value);

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            NetIdentity.onMMOWillStart += OnMMOWillStart;
            NetIdentity.onMMOStartCompleted += OnMMOStartCompleted;
            NetIdentity.onMMOStoped += OnMMOStoped;
            NetIdentity.onMMOEnterRoomCompleted += OnMMOEnterRoomCompleted;
            NetIdentity.onMMOExitRoomCompleted += OnMMOExitRoomCompleted;
            NetIdentity.onMMORoomAddUser += OnMMORoomAddUser;
            NetIdentity.onMMORoomRemoveUser += OnMMORoomRemoveUser;
            NetIdentity.onMMORoomAddPlayer += OnMMORoomAddPlayer;
            NetIdentity.onMMORoomRemovePlayer += OnMMORoomRemovePlayer;
            NetIdentity.onMMOClonedAsNew += OnMMOClonedAsNew;
            NetIdentity.onMMOClonedAsTemplate += OnMMOClonedAsTemplate;
            NetIdentity.onMMOWillDestroy += OnMMOWillDestroy;
            NetIdentity.onMMOStartPlayerLink += OnMMOStartPlayerLink;
            NetIdentity.onMMOStopPlayerLink += OnMMOStopPlayerLink;
            NetIdentity.onMMOStartControlAccess += OnMMOStartControlAccess;
            NetIdentity.onMMOStopControlAccess += OnMMOStopControlAccess;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            NetIdentity.onMMOWillStart -= OnMMOWillStart;
            NetIdentity.onMMOStartCompleted -= OnMMOStartCompleted;
            NetIdentity.onMMOStoped -= OnMMOStoped;
            NetIdentity.onMMOEnterRoomCompleted -= OnMMOEnterRoomCompleted;
            NetIdentity.onMMOExitRoomCompleted -= OnMMOExitRoomCompleted;
            NetIdentity.onMMORoomAddUser -= OnMMORoomAddUser;
            NetIdentity.onMMORoomRemoveUser -= OnMMORoomRemoveUser;
            NetIdentity.onMMORoomAddPlayer -= OnMMORoomAddPlayer;
            NetIdentity.onMMORoomRemovePlayer -= OnMMORoomRemovePlayer;
            NetIdentity.onMMOClonedAsNew -= OnMMOClonedAsNew;
            NetIdentity.onMMOClonedAsTemplate -= OnMMOClonedAsTemplate;
            NetIdentity.onMMOWillDestroy -= OnMMOWillDestroy;
            NetIdentity.onMMOStartPlayerLink -= OnMMOStartPlayerLink;
            NetIdentity.onMMOStopPlayerLink -= OnMMOStopPlayerLink;
            NetIdentity.onMMOStartControlAccess -= OnMMOStartControlAccess;
            NetIdentity.onMMOStopControlAccess -= OnMMOStopControlAccess;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (netIdentity) { }
        }

        #region 网络标识事件回调

        /// <summary>
        /// 当MMO将要启动事件
        /// </summary>
        [Group("当MMO将要启动事件", defaultIsExpanded = false)]
        [Name("当MMO将要启动事件")]
        public NetIdentityUnityEvent _onMMOWillStartEvent = new NetIdentityUnityEvent();

        private void OnMMOWillStart(NetIdentity netIdentity)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMOWillStartEvent.Invoke(netIdentity);
        }

        /// <summary>
        /// 当MMO启动完成事件
        /// </summary>
        [Name("当MMO启动完成事件")]
        [Group("当MMO启动完成事件", defaultIsExpanded = false)]
        public NetIdentityEACodeUnityEvent _onMMOStartCompletedEvent = new NetIdentityEACodeUnityEvent();

        private void OnMMOStartCompleted(NetIdentity netIdentity, EACode result)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMOStartCompletedEvent.Invoke(netIdentity, result);
            SetToVariable(result.ToString());
        }

        /// <summary>
        /// 当MMO已停止事件
        /// </summary>
        [Name("当MMO已停止事件")]
        [Group("当MMO已停止事件", defaultIsExpanded = false)]
        public NetIdentityUnityEvent _onMMOStopedEvent = new NetIdentityUnityEvent();

        private void OnMMOStoped(NetIdentity netIdentity)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMOStopedEvent.Invoke(netIdentity);
        }

        /// <summary>
        /// 当MMO进入房间完成事件
        /// </summary>
        [Name("当MMO进入房间完成事件")]
        [Group("当MMO进入房间完成事件", defaultIsExpanded = false)]
        public NetIdentityEACodeUnityEvent _onMMOEnterRoomCompletedEvent = new NetIdentityEACodeUnityEvent();

        private void OnMMOEnterRoomCompleted(NetIdentity netIdentity, EACode result)
        {
            if (netIdentity != this._netIdentity) return;
            //Log.Debug(name + ".OnMMOEnterRoomCompleted ：" + result);
            _onMMOEnterRoomCompletedEvent.Invoke(netIdentity, result);
            SetToVariable(result.ToString());
        }

        /// <summary>
        /// 当MMO退出房间完成事件
        /// </summary>
        [Name("当MMO退出房间完成事件")]
        [Group("当MMO退出房间完成事件", defaultIsExpanded = false)]
        public NetIdentityUnityEvent _onMMOExitRoomCompletedEvent = new NetIdentityUnityEvent();

        private void OnMMOExitRoomCompleted(NetIdentity netIdentity)
        {
            if (netIdentity != this._netIdentity) return;
            //Log.Debug(name + ".OnMMOExitRoomCompleted ：");
            _onMMOExitRoomCompletedEvent.Invoke(netIdentity);
        }

        /// <summary>
        /// 当MMO房间增加用户事件
        /// </summary>
        [Name("当MMO房间增加用户事件")]
        [Group("当MMO房间增加用户事件", defaultIsExpanded = false)]
        public NetIdentityStringUnityEvent _onMMORoomAddUserEvent = new NetIdentityStringUnityEvent();

        private void OnMMORoomAddUser(NetIdentity netIdentity, string userGuid)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMORoomAddUserEvent.Invoke(netIdentity, userGuid);
            SetToVariable(userGuid);
        }

        /// <summary>
        /// 当MMO房间移除用户事件
        /// </summary>
        [Name("当MMO房间移除用户事件")]
        [Group("当MMO房间移除用户事件", defaultIsExpanded = false)]
        public NetIdentityStringUnityEvent _onMMORoomRemoveUserEvent = new NetIdentityStringUnityEvent();

        private void OnMMORoomRemoveUser(NetIdentity netIdentity, string userGuid)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMORoomRemoveUserEvent.Invoke(netIdentity, userGuid);
            SetToVariable(userGuid);
        }

        /// <summary>
        /// 当MMO房间增加玩家事件
        /// </summary>
        [Name("当MMO房间增加玩家事件")]
        [Group("当MMO房间增加玩家事件", defaultIsExpanded = false)]
        public NetIdentityStringUnityEvent _onMMORoomAddPlayerEvent = new NetIdentityStringUnityEvent();

        private void OnMMORoomAddPlayer(NetIdentity netIdentity, string playerGuid)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMORoomAddPlayerEvent.Invoke(netIdentity, playerGuid);
            SetToVariable(playerGuid);
        }

        /// <summary>
        /// 当MMO房间增加玩家事件
        /// </summary>
        [Name("当MMO房间增加玩家事件")]
        [Group("当MMO房间增加玩家事件", defaultIsExpanded = false)]
        public NetIdentityStringUnityEvent _onMMORoomRemovePlayerEvent = new NetIdentityStringUnityEvent();

        private void OnMMORoomRemovePlayer(NetIdentity netIdentity, string playerGuid)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMORoomRemovePlayerEvent.Invoke(netIdentity, playerGuid);
            SetToVariable(playerGuid);
        }

        /// <summary>
        /// 当MMO已克隆作为新对象事件
        /// </summary>
        [Name("当MMO已克隆作为新对象事件")]
        [Group("当MMO已克隆作为新对象事件", defaultIsExpanded = false)]
        public NetIdentityUnityEvent _onMMOClonedAsNewEvent = new NetIdentityUnityEvent();

        private void OnMMOClonedAsNew(NetIdentity netIdentity)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMOClonedAsNewEvent.Invoke(netIdentity);
        }

        /// <summary>
        /// 当MMO已克隆作为模版事件
        /// </summary>
        [Name("当MMO已克隆作为模版事件")]
        [Group("当MMO已克隆作为模版事件", defaultIsExpanded = false)]
        public NetIdentityCloneUnityEvent _onMMOClonedAsTemplateEvent = new NetIdentityCloneUnityEvent();

        private void OnMMOClonedAsTemplate(NetIdentity netIdentity, NetIdentity newNetIdentity)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMOClonedAsTemplateEvent.Invoke(netIdentity, newNetIdentity);
            SetToVariable(CommonFun.GameObjectComponentToString(newNetIdentity));
        }

        /// <summary>
        /// 当MMO将要销毁事件
        /// </summary>
        [Name("当MMO将要销毁事件")]
        [Group("当MMO将要销毁事件", defaultIsExpanded = false)]
        public NetIdentityUnityEvent _onMMOWillDestroyEvent = new NetIdentityUnityEvent();

        private void OnMMOWillDestroy(NetIdentity netIdentity)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMOWillDestroyEvent.Invoke(netIdentity);
        }

        /// <summary>
        /// 当MMO启动玩家关联事件
        /// </summary>
        [Name("当MMO启动玩家关联事件")]
        [Group("当MMO启动玩家关联事件", defaultIsExpanded = false)]
        public NetIdentityUnityEvent _onMMOStartPlayerLinkEvent = new NetIdentityUnityEvent();

        /// <summary>
        /// 当MMO启动本地玩家关联事件
        /// </summary>
        [Name("当MMO启动本地玩家关联事件")]
        [Group("当MMO启动本地玩家关联事件", defaultIsExpanded = false)]
        public NetIdentityUnityEvent _onMMOStartLocalPlayerLinkEvent = new NetIdentityUnityEvent();

        /// <summary>
        /// 当MMO启动非本地玩家关联事件
        /// </summary>
        [Name("当MMO启动非本地玩家关联事件")]
        [Group("当MMO启动非本地玩家关联事件", defaultIsExpanded = false)]
        public NetIdentityUnityEvent _onMMOStartNoLocalPlayerLinkEvent = new NetIdentityUnityEvent();

        private void OnMMOStartPlayerLink(NetIdentity netIdentity)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMOStartPlayerLinkEvent.Invoke(netIdentity);
            if (netIdentity.isLocalPlayer)
            {
                _onMMOStartLocalPlayerLinkEvent.Invoke(netIdentity);
            }
            else
            {
                _onMMOStartNoLocalPlayerLinkEvent.Invoke(netIdentity);
            }
        }

        /// <summary>
        /// 当MMO停止玩家关联事件
        /// </summary>
        [Name("当MMO停止玩家关联事件")]
        [Group("当MMO停止玩家关联事件", defaultIsExpanded = false)]
        public NetIdentityUnityEvent _onMMOStopPlayerLinkEvent = new NetIdentityUnityEvent();

        private void OnMMOStopPlayerLink(NetIdentity netIdentity)
        {
            if (netIdentity != this._netIdentity) return;
            _onMMOStopPlayerLinkEvent.Invoke(netIdentity);
        }

        /// <summary>
        /// 当MMO启动控制权限事件
        /// </summary>
        [Name("当MMO启动控制权限事件")]
        [Group("当MMO启动控制权限事件", defaultIsExpanded = false)]
        public NetIdentityUnityEvent _onMMOStartControlAccessEvent = new NetIdentityUnityEvent();

        private void OnMMOStartControlAccess(NetIdentity netIdentity)
        {
            if (netIdentity != this._netIdentity) return;
            //Log.Debug(name + ".当MMO启动控制权限事件 ：");
            _onMMOStartControlAccessEvent.Invoke(netIdentity);
        }

        /// <summary>
        /// 当MMO停止控制权限事件
        /// </summary>
        [Name("当MMO停止控制权限事件")]
        [Group("当MMO停止控制权限事件", defaultIsExpanded = false)]
        public NetIdentityUnityEvent _onMMOStopControlAccessEvent = new NetIdentityUnityEvent();

        private void OnMMOStopControlAccess(NetIdentity netIdentity)
        {
            if (netIdentity != this._netIdentity) return;
            //Log.Debug(name + ".当MMO停止控制权限事件 ：");
            _onMMOStopControlAccessEvent.Invoke(netIdentity);
        }

        #endregion 
    }

    /// <summary>
    /// 网络标识Unity事件
    /// </summary>
    [Serializable]
    public class NetIdentityUnityEvent : UnityEvent<NetIdentity> { }

    /// <summary>
    /// 网络标识答案码Unity事件
    /// </summary>
    [Serializable]
    public class NetIdentityEACodeUnityEvent : UnityEvent<NetIdentity, EACode> { }

    /// <summary>
    /// 网络标识字符串Unity事件
    /// </summary>
    [Serializable]
    public class NetIdentityStringUnityEvent : UnityEvent<NetIdentity, string> { }

    /// <summary>
    /// 网络标识克隆Unity事件
    /// </summary>
    [Serializable]
    public class NetIdentityCloneUnityEvent : UnityEvent<NetIdentity, NetIdentity> { }
}
