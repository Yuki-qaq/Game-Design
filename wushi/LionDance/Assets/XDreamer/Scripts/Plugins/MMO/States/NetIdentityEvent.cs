using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginMMO.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 网络标识事件
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.ID)]
    [ComponentMenu(MMOHelperExtension.TitleDirectory + Title, typeof(MMOManager))]
    [Name(Title, nameof(NetIdentityEvent))]
    [Tip("用于捕获网络标识组件的各种网络事件", "Used to capture various network events of the network identification component")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    public class NetIdentityEvent : Trigger<NetIdentityEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "网络标识事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(MMOHelperExtension.Title, typeof(MMOManager))]
        [StateComponentMenu(MMOHelperExtension.TitleDirectory + Title, typeof(MMOManager))]
        [Name(Title, nameof(NetIdentityEvent))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("用于捕获网络标识组件的各种网络事件", "Used to capture various network events of the network identification component")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 网络标识
        /// </summary>
        [Name("网络标识")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public NetIdentity _netIdentity;

        /// <summary>
        /// MMO事件类型
        /// </summary>
        [Name("MMO事件类型")]
        [Tip("期望捕获的MMO事件类型", "Type of MMO event expected to be captured")]
        [EnumPopup]
        public EMMOEventType _mmoEventType = EMMOEventType.None;

        /// <summary>
        /// 变量名
        /// </summary>
        [Name("变量名")]
        [Tip("将网络事件回调的额外参数信息存储到当前变量中", "Store the additional parameter information of the network event callback into the current variable")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _variableName;

        /// <summary>
        /// 将值设置到变量
        /// </summary>
        /// <param name="value">值</param>
        protected void SetToVariable(string value) => _variableName.TrySetOrAddSetHierarchyVarValue(value);

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

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
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

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
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return string.Format("{0}.{1}", _netIdentity ? _netIdentity.name : "", CommonFun.Name(_mmoEventType));
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && _netIdentity;
        }

        #region 网络标识事件回调

        private void OnMMOWillStart(NetIdentity netIdentity)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOWillStart) return;
            if (netIdentity == this._netIdentity) finished = true;
        }

        private void OnMMOStartCompleted(NetIdentity netIdentity, EACode result)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOStartCompleted) return;
            if (netIdentity == this._netIdentity)
            {
                finished = true;
                SetToVariable(result.ToString());
            }
        }

        private void OnMMOStoped(NetIdentity netIdentity)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOStoped) return;
            if (netIdentity == this._netIdentity) finished = true;
        }

        private void OnMMOEnterRoomCompleted(NetIdentity netIdentity, EACode result)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOEnterRoomCompleted) return;
            if (netIdentity == this._netIdentity)
            {
                finished = true;
                SetToVariable(result.ToString());
            }
        }

        private void OnMMOExitRoomCompleted(NetIdentity netIdentity)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOExitRoomCompleted) return;
            if (netIdentity == this._netIdentity) finished = true;
        }

        private void OnMMORoomAddUser(NetIdentity netIdentity, string userGuid)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMORoomAddUser) return;
            if (netIdentity == this._netIdentity)
            {
                finished = true;
                SetToVariable(userGuid);
            }
        }

        private void OnMMORoomRemoveUser(NetIdentity netIdentity, string userGuid)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMORoomRemoveUser) return;
            if (netIdentity == this._netIdentity)
            {
                finished = true;
                SetToVariable(userGuid);
            }
        }

        private void OnMMORoomAddPlayer(NetIdentity netIdentity, string playerGuid)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMORoomAddPlayer) return;
            if (netIdentity == this._netIdentity)
            {
                finished = true;
                SetToVariable(playerGuid);
            }
        }

        private void OnMMORoomRemovePlayer(NetIdentity netIdentity, string playerGuid)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMORoomRemovePlayer) return;
            if (netIdentity == this._netIdentity)
            {
                finished = true;
                SetToVariable(playerGuid);
            }
        }

        private void OnMMOClonedAsNew(NetIdentity netIdentity)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOClonedAsNew) return;
            if (netIdentity == this._netIdentity) finished = true;
        }

        private void OnMMOClonedAsTemplate(NetIdentity netIdentity, NetIdentity newNetIdentity)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOClonedAsTemplate) return;
            if (netIdentity == this._netIdentity)
            {
                finished = true;
                SetToVariable(CommonFun.GameObjectComponentToString(newNetIdentity));
            }
        }

        private void OnMMOWillDestroy(NetIdentity netIdentity)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOWillDestroy) return;
            if (netIdentity == this._netIdentity) finished = true;
        }

        private void OnMMOStartPlayerLink(NetIdentity netIdentity)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOStartPlayerLink) return;
            if (netIdentity == this._netIdentity) finished = true;
        }

        private void OnMMOStopPlayerLink(NetIdentity netIdentity)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOStopPlayerLink) return;
            if (netIdentity == this._netIdentity) finished = true;
        }

        private void OnMMOStartControlAccess(NetIdentity netIdentity)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOStartControlAccess) return;
            if (netIdentity == this._netIdentity)
            {
                finished = true;
            }
        }

        private void OnMMOStopControlAccess(NetIdentity netIdentity)
        {
            if (finished || _mmoEventType != EMMOEventType.OnMMOStopControlAccess) return;
            if (netIdentity == this._netIdentity)
            {
                finished = true;
            }
        }

        #endregion
    }
}
