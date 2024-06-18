using UnityEngine;

namespace XCSJ.PluginCameras.Characters
{
    /// <summary>
    /// 地面命中
    /// </summary>
    public struct GroundHit
    {
        #region FIELDS

        private float _ledgeDistance;

        private float _stepHeight;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// 在地面上
        /// </summary>
        public bool isOnGround { get; set; }

        /// <summary>
        /// 是在有效地面
        /// </summary>
        public bool isValidGround { get; set; }

        /// <summary>
        /// 是在突出的实边
        /// </summary>
        public bool isOnLedgeSolidSide { get; set; }

        /// <summary>
        /// 是在突出的空边
        /// </summary>
        public bool isOnLedgeEmptySide { get; set; }

        /// <summary>
        /// 突出距离
        /// </summary>
        public float ledgeDistance
        {
            get { return _ledgeDistance; }
            set { _ledgeDistance = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 是在台阶上
        /// </summary>
        public bool isOnStep { get; set; }

        /// <summary>
        /// 台阶高度
        /// </summary>
        public float stepHeight
        {
            get { return _stepHeight; }
            set { _stepHeight = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 地面点
        /// </summary>
        public Vector3 groundPoint { get; set; }

        /// <summary>
        /// 地面法向量
        /// </summary>
        public Vector3 groundNormal { get; set; }

        /// <summary>
        /// 地面距离
        /// </summary>
        public float groundDistance { get; private set; }

        /// <summary>
        /// 地面碰撞体
        /// </summary>
        public Collider groundCollider { get; private set; }

        /// <summary>
        /// 地面刚体
        /// </summary>
        public Rigidbody groundRigidbody { get; private set; }

        /// <summary>
        /// 面法向量
        /// </summary>
        public Vector3 surfaceNormal { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="other"></param>
        public GroundHit(GroundHit other) : this()
        {
            isOnGround = other.isOnGround;
            isValidGround = other.isValidGround;

            isOnLedgeSolidSide = other.isOnLedgeSolidSide;
            isOnLedgeEmptySide = other.isOnLedgeEmptySide;
            ledgeDistance = other.ledgeDistance;

            isOnStep = other.isOnStep;
            stepHeight = other.stepHeight;

            groundPoint = other.groundPoint;
            groundNormal = other.groundNormal;
            groundDistance = Mathf.Max(0.0f, other.groundDistance);
            groundCollider = other.groundCollider;
            groundRigidbody = other.groundRigidbody;

            surfaceNormal = other.surfaceNormal;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="hitInfo"></param>
        public GroundHit(RaycastHit hitInfo) : this()
        {
            SetFrom(hitInfo);
        }

        /// <summary>
        /// 从源设置
        /// </summary>
        /// <param name="hitInfo"></param>
        public void SetFrom(RaycastHit hitInfo)
        {
            groundPoint = hitInfo.point;
            groundNormal = hitInfo.normal;
            groundDistance = Mathf.Max(0.0f, hitInfo.distance);
            groundCollider = hitInfo.collider;
            groundRigidbody = hitInfo.rigidbody;
        }

        #endregion
    }
}
