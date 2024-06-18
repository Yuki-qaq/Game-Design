using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络游戏对象激活
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    [DisallowMultipleComponent]
    [Name("网络游戏对象激活")]
    [Tool(MMOHelperExtension.Title, nameof(NetIdentity), rootType = typeof(MMOManager))]
    public class NetGameObjectActive : NetMB, IAwake
    {
        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject _go;

        /// <summary>
        /// 激活状态
        /// </summary>
        [SyncVar]
        [Readonly]
        [Name("激活状态")]
        public bool _activeState = false;

        /// <summary>
        /// 上一次激活状态
        /// </summary>
        [Readonly]
        [Name("上一次激活状态")]
        public bool _lastActiveState = false;

        /// <summary>
        /// 原始激活状态
        /// </summary>
        [Readonly]
        [Name("原始激活状态")]
        public bool _originalActiveState = false;

        /// <summary>
        /// 唤醒：初始化
        /// </summary>
        public void Awake()
        {
            if (!_go)
            {
                _go = gameObject;
            }
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!_go)
            {
                enabled = false;
                Debug.LogErrorFormat("【{0}】中的游戏对象不能为空！", CommonFun.ObjectToString(this));
                return;
            }
        }

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            base.OnMMOEnterRoomCompleted(result);
            if(_go) _originalActiveState = _lastActiveState = _activeState = _go.activeSelf;
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            base.OnMMOExitRoomCompleted();
            if (_go) _go.SetActive(_originalActiveState);
        }

        /// <summary>
        /// 当定时检查修改
        /// </summary>
        /// <returns></returns>
        protected override bool OnTimedCheckChange()
        {
            _activeState = _go.activeSelf;
            return _activeState != _lastActiveState;
        }

        /// <summary>
        /// 当同步变量已修改
        /// </summary>
        protected override void OnSyncVarChanged()
        {
            base.OnSyncVarChanged();

            _go.SetActive(_activeState);
            _lastActiveState = _activeState;
        }
    }
}
