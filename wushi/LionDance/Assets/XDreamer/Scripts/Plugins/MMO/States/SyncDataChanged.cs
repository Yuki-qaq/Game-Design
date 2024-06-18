using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Tools;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 同步数据变化:网络组件中有同步数据发生变化时触发，即反序列数据完成时的回调事件（包括同步变量、自定义的数据）；网络到本地；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Data)]
    [ComponentMenu(MMOHelperExtension.TitleDirectory + Title, typeof(MMOManager))]
    [Name(Title, nameof(SyncDataChanged))]
    [Tip("网络组件中有同步数据发生变化时触发，即反序列数据完成时的回调事件（包括同步变量、自定义的数据）；网络到本地；", "Triggered when the synchronization data in the network component changes, that is, the callback event when the inverse sequence data is completed (including synchronization variables and user-defined data); Network to local;")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    public class SyncDataChanged : Trigger<SyncDataChanged>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "同步数据变化";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(MMOHelperExtension.Title, typeof(MMOManager))]
        [StateComponentMenu(MMOHelperExtension.TitleDirectory + Title, typeof(MMOManager))]
        [Name(Title, nameof(SyncDataChanged))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("网络组件中有同步数据发生变化时触发，即反序列数据完成时的回调事件（包括同步变量、自定义的数据）；网络到本地；", "Triggered when the synchronization data in the network component changes, that is, the callback event when the inverse sequence data is completed (including synchronization variables and user-defined data); Network to local;")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 网络组件
        /// </summary>
        [Name("网络组件")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public NetMB _netMB;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            NetMB.onSyncDataChanged += OnSyncDataChanged;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            NetMB.onSyncDataChanged -= OnSyncDataChanged;
        }

        private void OnSyncDataChanged(NetMB netMB)
        {
            if (netMB && this._netMB == netMB)
            {
                finished = true;
            }
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _netMB ? _netMB.name : "";// base.ToFriendlyString();
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && _netMB;
        }
    }
}
