using UnityEngine;
using UnityEngine.AI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCameras.Characters.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Extension.Base.Extensions;

namespace XCSJ.PluginCameras.Characters.Tools
{
    /// <summary>
    /// 角色移动在代理中通过输入
    /// </summary>
    [Name("角色移动在代理中通过输入")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterCategory.Title, nameof(CharacterTransformer))]
    public class CharacterMoveInAgentByInput : BaseCharacterMoveController, INavMeshAgentInput
    {
        /// <summary>
        /// 导航网格代理
        /// </summary>
        [Group("导航网格代理")]
        [Name("导航网格代理")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public NavMeshAgent _navMeshAgent;

        /// <summary>
        /// 导航网格代理
        /// </summary>
        public NavMeshAgent navMeshAgent => this.XGetComponentInParent(ref _navMeshAgent);

        /// <summary>
        /// 自动制动:标识代理是否应该自动刹车以避免超出目的地点；如果为True，那么此代理会在靠近目的地时自动刹车。
        /// </summary>
        [Name("自动制动")]
        [Tip("标识代理是否应该自动刹车以避免超出目的地点；如果为True，那么此代理会在靠近目的地时自动刹车。", "Identify whether the agent should brake automatically to avoid exceeding the destination; If true, the agent automatically brakes as it approaches its destination.")]
        public bool _autoBraking = true;

        /// <summary>
        /// 自动制动:标识代理是否应该自动刹车以避免超出目的地点；如果为True，那么此代理会在靠近目的地时自动刹车。
        /// </summary>
        public bool autoBraking
        {
            get { return _autoBraking; }
            set
            {
                _autoBraking = value;

                if (_navMeshAgent) _navMeshAgent.autoBraking = _autoBraking;
            }
        }

        /// <summary>
        /// 制动距离:距离目标位置多远时，开始制动（即减速区域的长度值）
        /// </summary>
        [Name("制动距离")]
        [Tip("距离目标位置多远时，开始制动（即减速区域的长度值）", "Start braking when how far away from the target position (i.e. the length value of deceleration area)")]
        public float _brakingDistance = 2.0f;

        /// <summary>
        /// 制动距离:距离目标位置多远时，开始制动（即减速区域的长度值）
        /// </summary>
        public float brakingDistance
        {
            get { return _brakingDistance; }
            set { _brakingDistance = Mathf.Max(0.0001f, value); }
        }

        /// <summary>
        /// 代理的剩余距离与制动距离的比率（0-1范围）。
        /// 如果没有自动制动或代理的剩余距离大于制动距离，则等于1。
        /// 如果代理的剩余距离小于制动距离，则小于1。
        /// </summary>
        public float brakingRatio
        {
            get
            {
                if (!autoBraking || !_navMeshAgent ) return 1f;

                return _navMeshAgent.hasPath ? Mathf.Clamp(_navMeshAgent.remainingDistance / brakingDistance, 0.1f, 1f) : 1f;
            }
        }

        /// <summary>
        /// 停止距离:在距离目标位置多远时，即可以停止
        /// </summary>
        [Name("停止距离")]
        [Tip("在距离目标位置多远时，即可以停止", "It can stop when it is far from the target position")]
        [SerializeField]
        private float _stoppingDistance = 1.0f;

        /// <summary>
        /// 停止距离:在距离目标位置多远时，即可以停止
        /// </summary>
        public float stoppingDistance
        {
            get { return _stoppingDistance; }
            set
            {
                _stoppingDistance = Mathf.Max(0.0f, value);

                if (_navMeshAgent) _navMeshAgent.stoppingDistance = _stoppingDistance;
            }
        }

        /// <summary>
        /// 地面层:标识哪些层被认为是地面(即可行走区域)；在由地面点击检测使用。
        /// </summary>
        [Name("地面层")]
        [Tip("标识哪些层被认为是地面(即可行走区域)；在由地面点击检测使用。", "Identify which layers are considered to be the ground (i.e. walking area); Used by ground click detection.")]
        public LayerMask _groundMask = 1;

        /// <summary>
        /// 查询触发器交互:当点击地面检测碰撞体时，是否检测触发器
        /// </summary>
        [Name("查询触发器交互")]
        [Tip("当点击地面检测碰撞体时，是否检测触发器", "Is the trigger detected when clicking on the ground to detect collision objects")]
        public QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.Ignore;

        /// <summary>
        /// 移动模式
        /// </summary>
        [Group("移动", defaultIsExpanded = false)]
        [Name("移动模式")]
        [EnumPopup]
        public EMoveMode _moveMode = EMoveMode.World;

        /// <summary>
        /// 输入阈值
        /// </summary>
        [Name("输入阈值")]
        [Range(0.01f, 1f)]
        public float _inputThreshold = 0.1f;

        /// <summary>
        /// 目标位置输入:点击触发移动到目标位置
        /// </summary>
        [Name("目标位置输入")]
        [Tip("点击触发移动到目标位置", "Click trigger to move to the target position")]
        public InputAxis _destinationInput = new InputAxis();

        #region 输入处理

        /// <summary>
        /// 输入处理
        /// </summary>
        [Name("输入处理")]
        public InputHandler _inputHandler = new InputHandler();

        #endregion

        /// <summary>
        /// 主相机
        /// </summary>
        public Camera mainCamera => mainController.characterCameraController ? mainController.characterCameraController.characterCamera : default;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (navMeshAgent) { }

            _destinationInput._inputName = "Fire2";//鼠标右键
            _destinationInput._mouseButtons.Add(EMouseButton.Always);
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            //当导航网格代理无效时直接禁用当前组件
            if (!navMeshAgent) enabled = false;
        }

        Vector3 mousePositionDown;

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            var cam = mainCamera;
            if (!cam) return;

            if (_inputHandler.GetInput() is IInput input)
            {
                if (_destinationInput.CanInput(input))
                {
                    if (input.GetButtonDown(_destinationInput._inputName))
                    {
                        mousePositionDown = XInput.mousePosition;
                    }
                    else if (input.GetButtonUp(_destinationInput._inputName))
                    {
                        var mousePosition = XInput.mousePosition;
                        if((mousePosition - mousePositionDown).sqrMagnitude< _inputThreshold* _inputThreshold)
                        {
                            //将代理目标设置为地面命中点
                            var ray = cam.ScreenPointToRay(mousePosition);
                            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _groundMask.value, _queryTriggerInteraction))
                            {
                                mainController.SetAgentDestination(hitInfo.point);
                            }
                        }                        
                    }
                }
            }
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        public void FixedUpdate()
        {
            if (TryGetMoveDirection(out Vector3 moveDirection))
            {
                mainController.Move(moveDirection, (int)_moveMode, this);
            }
        }

        private bool TryGetMoveDirection(out Vector3 moveDirection)
        {
            if (_navMeshAgent.hasPath)
            {
                // 如未到目的地,角色移动方向时进给代理的所需速度（仅横向）
                if (_navMeshAgent.remainingDistance > stoppingDistance)
                {
                    moveDirection = _navMeshAgent.desiredVelocity.OnlyXZ();
                    return true;
                }
                else
                {
                    // 到达目的地,重置停止代理并清除其路径
                    mainController.ResetAgentPath();
                }
            }

            moveDirection = Vector3.zero;
            return false;
        }
    }

    /// <summary>
    /// 导航网格代理输入
    /// </summary>
    public interface INavMeshAgentInput : ICharacterTransformInputController { }

    /// <summary>
    /// 导航网格代理控制器
    /// </summary>
    public interface INavMeshAgentController
    {
        /// <summary>
        /// 重置代理路径
        /// </summary>
        void ResetAgentPath();

        /// <summary>
        /// 设置代理目标
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        bool SetAgentDestination(Vector3 position);
    }
}
