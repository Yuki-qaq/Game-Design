using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Hands
{
    /// <summary>
    /// 脚步:通过身体和地面位置关系计算脚步位置并移动脚
    /// </summary>
    [Name("脚步")]
    [Tip("通过身体位置计算脚步与地面的位置", "Calculate foot position and move feet based on the relationship between body and ground position")]
    [XCSJ.Attributes.Icon(EIcon.Foot)]
    [RequireManager(typeof(ToolsExtensionManager))]
    [Tool(ToolsExtensionCategory.HumanBody, rootType = typeof(ToolsExtensionManager))]
    public class FootStep : Interactor
    {
        /// <summary>
        /// 脚
        /// </summary>
        [Group("脚步设置", textEN = "FootStep Settings")]
        [Name("脚")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _foot;

        /// <summary>
        /// 脚位置偏移
        /// </summary>
        [Name("脚位置偏移")]
        public Vector3 _footPositionOffset = Vector3.zero;

        /// <summary>
        /// 脚角度偏移
        /// </summary>
        [Name("脚角度偏移")]
        public Vector3 _footAngleOffset = Vector3.zero;

        /// <summary>
        /// 地面层
        /// </summary>
        [Name("地面层")]
        public LayerMask _groundLayer = Physics.DefaultRaycastLayers;

        /// <summary>
        /// 脚步长度
        /// </summary>
        [Name("脚步长度")]
        [Min(0)]
        public float _stepLength = .3f;

        /// <summary>
        /// 脚步高度
        /// </summary>
        [Name("脚步高度")]
        [Min(0)]
        public float _stepHeight = .3f;

        /// <summary>
        /// 脚移动速度
        /// </summary>
        [Name("脚移动速度")]
        [Min(0)]
        public float _footMoveSpeed = 5;

        /// <summary>
        /// 目标位置
        /// </summary>
        public Vector3 targetPosition { get; private set; } = Vector3.zero;

        private float footToBodyHorizontalDistance = 0;
        private float lerpValue = 1;

        private Vector3 lastPosition, currentPosition;
        private Vector3 lastNorm, currentNorm, targetNorm;

        private HumanBody body;
        private FootStep otherFootStep;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _foot = transform;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            body = GetComponentInParent<HumanBody>();
            if (!body)
            {
                enabled = false;
                return;
            }
            otherFootStep = body._leftFootStep != this ? body._leftFootStep : body._rightFootStep;

            if (!_foot) _foot = transform;

            ResetData();
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public void ResetData()
        {
            footToBodyHorizontalDistance = _foot.localPosition.x;
            currentPosition = targetPosition = lastPosition = _foot.position;
            currentNorm = targetNorm = lastNorm = Vector3.up;
            lerpValue = 1;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            // 每帧刷新使脚定在上次记录的点上，不会随着父级而漂移
            Move();
        }

        /// <summary>
        /// 尝试移动
        /// </summary>
        /// <returns></returns>
        public bool TryMove()
        {
            if (IsMove() || (otherFootStep && otherFootStep.IsMove())) return false;

            // 使用身体位置构建垂直于脚面的射线
            var ray = new Ray(body.transform.position + (body.transform.right * footToBodyHorizontalDistance) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out var hit, 10, _groundLayer.value))
            {
                var distance = _stepLength;
                if (otherFootStep)
                {
                    distance = Mathf.Clamp(Vector3.Distance(targetPosition, otherFootStep.targetPosition), _stepLength, _stepLength * 2);
                }
                if (Vector3.Distance(targetPosition, hit.point) > distance)
                {
                    lerpValue = 0;
                    // 判断前进还是后退
                    int direction = body.transform.InverseTransformPoint(hit.point).z > body.transform.InverseTransformPoint(targetPosition).z ? 1 : -1;
                    targetPosition = hit.point + body.transform.forward * direction * _stepLength;
                    targetNorm = hit.normal;
                    return true;
                }
            }

            return false;
        }

        private void Move()
        {
            if (lerpValue < 1)
            {
                lerpValue += Time.deltaTime * _footMoveSpeed;

                if (lerpValue > 1)
                {
                    currentPosition = lastPosition = targetPosition;
                    currentNorm = lastNorm = targetNorm;
                }
                else
                {
                    // 使用正弦曲线半周期模拟脚的抬起和下降
                    var tempPos = Vector3.Lerp(lastPosition, targetPosition, lerpValue);
                    tempPos.y += Mathf.Sin(lerpValue * Mathf.PI) * _stepHeight;
                    currentPosition = tempPos;

                    currentNorm = Vector3.Lerp(lastNorm, targetNorm, lerpValue);
                }
            }

            _foot.position = currentPosition + _footPositionOffset;
            _foot.rotation = Quaternion.LookRotation(body.transform.forward, currentNorm) * Quaternion.Euler(_footAngleOffset);
        }

        /// <summary>
        /// 是否移动
        /// </summary>
        /// <returns></returns>
        public bool IsMove() => lerpValue < 1;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 0.1f);
        }
    }
}
