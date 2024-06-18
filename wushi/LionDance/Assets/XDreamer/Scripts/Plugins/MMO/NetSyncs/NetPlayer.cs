using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCameras.Characters;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCameras.Base;
using XCSJ.PluginCameras.Controllers;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络玩家
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Authentication)]
    [DisallowMultipleComponent]
    [Name("网络玩家")]
    [Tool(MMOHelperExtension.Title, nameof(NetIdentity), rootType = typeof(MMOManager))]
    [RequireComponent(typeof(NetIdentity))]
    public sealed class NetPlayer : NetProperty, INetPlayer
    {
        #region 网络玩家

        /// <summary>
        /// 显示名
        /// </summary>
        public string displayName
        {
            get
            {
                var n = nickname;
                return string.IsNullOrEmpty(n) ? name : n;
            }
        }

        /// <summary>
        /// 昵称
        /// </summary>
        [Name("昵称")]
        public string nickname
        {
            get => GetProperty(nameof(nickname))?._value;
            set => SetProperty(nameof(nickname), value);
        }

        /// <summary>
        /// 本地玩家
        /// </summary>
        public NetPlayer localPlayer => MMOHelper.localPlayer?.netPlayer as NetPlayer;

        INetPlayer INetPlayer.localPlayer => MMOHelper.localPlayer?.netPlayer;

        /// <summary>
        /// 头像
        /// </summary>
        [Name("头像")]
        public Sprite _headImage;

        #endregion

        #region 相机设置

        /// <summary>
        /// 玩家相机控制器
        /// </summary>
        [Group("相机设置", defaultIsExpanded = false)]
        [Name("玩家相机控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BaseCameraMainController _playerCameraController;

        /// <summary>
        /// 玩家相机控制器
        /// </summary>
        public BaseCameraMainController playerCameraController => this.XGetComponentInChildren(ref _playerCameraController);

        /// <summary>
        /// 相机非本地玩家处理规则
        /// </summary>
        public enum ECameraNoLocalPlayerHandleRule
        {
            /// <summary>
            /// 非激活相机控制器游戏对象
            /// </summary>
            [Name("非激活相机控制器游戏对象")]
            InactiveCameraControllerGameObject,

            /// <summary>
            /// 非激活输入与处理游戏对象:非激活玩家相机控制器子级的相机变换器和相机实体控制器游戏对象
            /// </summary>
            [Name("非激活输入与处理游戏对象")]
            [Tip("非激活玩家相机控制器子级的相机变换器和相机实体控制器游戏对象", "Camera transformer and camera entity controller game objects for inactive player camera controller children")]
            InactiveInputAndHandleGameObject,
        }

        /// <summary>
        /// 相机非本地玩家处理规则
        /// </summary>
        [Name("相机非本地玩家处理规则")]
        [EnumPopup]
        public ECameraNoLocalPlayerHandleRule _cameraNoLocalPlayerHandleRule = ECameraNoLocalPlayerHandleRule.InactiveCameraControllerGameObject;

        /// <summary>
        /// 切换相机持续时长
        /// </summary>
        [Name("切换相机持续时长")]
        [Range(0.001f, 10)]
        public float _switchCameraDuration = 1;

        /// <summary>
        /// 相机目标处理规则
        /// </summary>
        [Name("相机目标处理规则")]
        [EnumPopup]
        public ECameraTargetHandleRule _cameraTargetHandleRule = ECameraTargetHandleRule.None;

        /// <summary>
        /// 相机目标处理规则
        /// </summary>
        [Name("相机目标处理规则")]
        public enum ECameraTargetHandleRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 无
            /// </summary>
            [Name("主目标")]
            [Tip("仅将主目标设置到玩家相机控制器对应的主目标", "Only set the main target to the main target corresponding to the player's camera controller")]
            MainTarget,

            /// <summary>
            /// 目标列表
            /// </summary>
            [Name("目标列表")]
            [Tip("仅将目标列表设置到玩家相机控制器对应的目标列表", "Only set the target list to the target list corresponding to the player's camera controller")]
            Targets,

            /// <summary>
            /// 二者
            /// </summary>
            [Name("二者")]
            [Tip("将主目标与目标列表设置到玩家相机控制器对应的主目标与目标列表", "Set the main target and target list to the main target and target list corresponding to the player's camera controller")]
            Both,
        }

        /// <summary>
        /// 相机主目标
        /// </summary>
        [Name("相机主目标")]
        [HideInSuperInspector(nameof(_cameraTargetHandleRule), EValidityCheckType.Equal, ECameraTargetHandleRule.None)]
        public Transform _cameraMainTarget;

        /// <summary>
        /// 相机目标列表
        /// </summary>
        [Name("相机目标列表")]
        [HideInSuperInspector(nameof(_cameraTargetHandleRule), EValidityCheckType.Equal, ECameraTargetHandleRule.None)]
        public List<Transform> _cameraTargets = new List<Transform>();

        private BaseCameraMainController _prveCameraController;
        private Dictionary<ICameraTargetController, Transform> _tmpCameraMainTarget = new Dictionary<ICameraTargetController, Transform>();
        private Dictionary<ICameraTargetController, Transform[]> _tmpCameraTargets = new Dictionary<ICameraTargetController, Transform[]>();

        XCharacterController characterController;

        private void OnSwitchPlayerCameraCompleted()
        {
            if (!_prveCameraController)
            {
                var camera = Camera.main;
                if (camera)
                {
                    _prveCameraController = camera.GetComponentInParent<BaseCameraMainController>();
                }
            }

            //将之前的相机控制器游戏对象强制禁用
            if (_prveCameraController)
            {
                _prveCameraController.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("前置相机控制器无效！");
            }

            //保证玩家相机控制器的可用性
            _playerCameraController.enabled = true;
            _playerCameraController.gameObject.SetActive(true);

            //设置玩家相机控制器相关信息
            foreach (var t in _playerCameraController.GetComponentsInChildren<ICameraTargetController>())
            {
                _tmpCameraMainTarget[t] = t.mainTarget;
                _tmpCameraTargets[t] = t.targets;

                switch (_cameraTargetHandleRule)
                {
                    case ECameraTargetHandleRule.MainTarget:
                        {
                            t.mainTarget = _cameraMainTarget;
                            break;
                        }
                    case ECameraTargetHandleRule.Targets:
                        {
                            t.targets = _cameraTargets.ToArray();
                            break;
                        }
                    case ECameraTargetHandleRule.Both:
                        {
                            t.mainTarget = _cameraMainTarget;
                            t.targets = _cameraTargets.ToArray();
                            break;
                        }
                }
            }
        }

        private void StartLink()
        {
            _prveCameraController = null;
            _tmpCameraMainTarget.Clear();
            _tmpCameraTargets.Clear();

            if (isLocalPlayer)
            {
                //启用玩家的角色控制
                characterController = this.GetComponent<XCharacterController>();
                if (characterController)
                {
                    characterController.gameObject.XSetActive(true);
                    var characterTransformer = characterController.characterTransformer;
                    if (characterTransformer)
                    {
                        characterTransformer.gameObject.XSetActive(true);
                    }
                }

                //启用玩家相机控制器
                if (_playerCameraController)
                {
                    _playerCameraController.gameObject.SetActive(false);
                    var cameraManager = CameraManager.instance;
                    if (cameraManager)
                    {
                        //缓存当前相机控制器
                        _prveCameraController = cameraManager.GetCurrentCameraController();

                        if (_prveCameraController == _playerCameraController)//同一个相机控制器，那么认为已经切换完成
                        {
                            OnSwitchPlayerCameraCompleted();
                        }
                        else
                        {
                            //切换到当前玩家相机控制器
                            cameraManager.SwitchCameraController(_playerCameraController, _switchCameraDuration, OnSwitchPlayerCameraCompleted, true);
                        }                      
                    }
                    else
                    {
                        OnSwitchPlayerCameraCompleted();
                    }                    
                }                
            }
            else
            {
                //禁用玩家相机控制器
                if (_playerCameraController)
                {
                    var localPlayer = this.localPlayer;
                    if (localPlayer && localPlayer._playerCameraController == _playerCameraController)
                    {
                        //多个玩家共用一个相机控制器，不做处理
                    }
                    if (_playerCameraController.cameraEntityController.mainCamera == Camera.main)
                    {
                        //如果玩家控制器的主相机是当前正在使用相机，不做处理
                    }
                    else
                    {
                        switch (_cameraNoLocalPlayerHandleRule)
                        {
                            case ECameraNoLocalPlayerHandleRule.InactiveCameraControllerGameObject:
                                {
                                    _playerCameraController.gameObject.SetActive(false);
                                    break;
                                }
                            case ECameraNoLocalPlayerHandleRule.InactiveInputAndHandleGameObject:
                                {
                                    _playerCameraController.cameraEntityController.gameObject.XSetActive(false);
                                    _playerCameraController.cameraTransformer.gameObject.XSetActive(false);
                                    break;
                                }
                        }
                    }
                }

                //启用玩家的角色控制
                characterController = this.GetComponent<XCharacterController>();
                if (characterController)
                {
                    //characterController.gameObject.XSetActive(false);
                    var characterTransformer = characterController.characterTransformer;
                    if (characterTransformer)
                    {
                        characterTransformer.gameObject.XSetActive(false);
                    }
                }
            }
        }

        private void StopLink()
        {
            if (isLocalPlayer)
            {
                //还原玩家相机控制器相关信息
                if (_playerCameraController)
                {
                    foreach (var kv in _tmpCameraMainTarget)
                    {
                        kv.Key.mainTarget = kv.Value;
                    }
                    _tmpCameraMainTarget.Clear();
                    foreach (var kv in _tmpCameraTargets)
                    {
                        kv.Key.targets = kv.Value;
                    }
                    _tmpCameraTargets.Clear();
                }

                //还原之前相机控制器
                if (_prveCameraController)
                {
                    var manager = CameraManager.instance;
                    if (manager && manager.SwitchCameraController(_prveCameraController, 0, null, true))
                    {
                        //切换成功
                    }

                    //保证之前相机控制器的可用性
                    _prveCameraController.gameObject.SetActive(true);
                    _prveCameraController.enabled = true;
                }
                else
                {
                    //切换为初始相机控制器
                    var manager = CameraManager.instance;
                    if (manager)
                    {
                        manager.SwitchCameraController(manager.GetInitCameraController(), 0, null, true);
                    }
                }
            }
        }

        #endregion

        #region MB方法

        /// <summary>
        /// 唤醒：初始化
        /// </summary>
        public void Awake()
        {
            if (playerCameraController) { }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (playerCameraController) { }
            nickname = Product.Name;
        }

        #endregion

        #region MMO事件

        /// <summary>
        /// 当MMO启动玩家关联：当前对象与网络环境中的玩家产生关联时回调
        /// </summary>
        public override void OnMMOStartPlayerLink()
        {
            base.OnMMOStartPlayerLink();
            StartLink();
        }

        /// <summary>
        /// 当MMO停止玩家关联：当前对象与网络环境中的玩家解除关联时回调
        /// </summary>
        public override void OnMMOStopPlayerLink()
        {
            base.OnMMOStopPlayerLink();
            StopLink();
        }

        #endregion

        #region 本地属性

        /// <summary>
        /// 当本地属性已修改
        /// </summary>
        /// <param name="name"></param>
        public void OnLocalPropertyChanged(string name) => this.CallUnityObjectPropertyChangedEvent(name);

        /// <summary>
        /// 允许音频
        /// </summary>
        public bool allowAudio { get => playerInfo.AllowAudio(); set => playerInfo.AllowAudio(value); }

        #endregion
    }
}
