using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Hands
{
    /// <summary>
    /// 手指
    /// </summary>
    [Name("手指")]
    public class Finger : InteractProvider
    {
        /// <summary>
        /// 手指指尖：用于探测碰撞(不属于关节)
        /// </summary>
        [Name("指尖")]
        [Tip("用于探测碰撞(不属于关节)", "Used to detect collisions (not joints)")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _tip;

        /// <summary>
        /// 指尖半径
        /// </summary>
        [Name("指尖半径")]
        public float _tipRadius = 0.1f;

        private void OnValidate()
        {
            //InitJoints();
        }

        #region 关节

        /// <summary>
        /// 手指关节
        /// </summary>
        [Name("手指关节")] 
        [Tip("关节从手掌到手指尖", "Joint from palm to finger tip")]
        public List<Transform> _joints = new List<Transform>();

        /// <summary>
        /// 手指关节
        /// </summary>
        public List<Transform> joints
        {
            get
            {
                if (_joints.Count==0 || !initJointFlag)
                {
                    InitJoints();
                }
                return _joints;
            }
        }

        /// <summary>
        /// 手指关节数量
        /// </summary>
        public int jointCount => joints.Count;

        private bool initJointFlag = false;

        /// <summary>
        /// 初始化关节
        /// </summary>
        [ContextMenu("初始化关节")]
        public void InitJoints()
        {
            if (!_tip || !_tip.IsChildOf(transform)) return;

            _joints.Clear();
            var t = _tip;
            while (t && t != transform)
            {
                t = t.parent;
                _joints.Insert(0, t);
            }
        }

        #endregion

        #region 弯曲

        /// <summary>
        /// 弯曲手指
        /// </summary>
        /// <param name="bendDegree"></param>
        public void Bend(float bendDegree) => minPose.BlendPoseDataToFinger(maxPose, bendDegree, this);

        /// <summary>
        /// 展开手指：最小弯曲手指
        /// </summary>
        [ContextMenu("展开手指")]
        public void BendMin() => Bend(0);

        /// <summary>
        /// 握紧手指：最大弯曲
        /// </summary>
        [ContextMenu("握紧手指")]
        public void BendMax() => Bend(1);

        /// <summary>
        /// 通过碰撞检查机制弯曲手指
        /// </summary>
        /// <param name="stepCount"></param>
        /// <returns></returns>
        public bool BendWithCollision(int stepCount = 5)
        {
            BendMin();
            if (stepCount == 0) return false;

            Collider[] collideResults = new Collider[2];
            for (float i = 0; i <= stepCount; i++)
            {
                var bendValue = i / stepCount;
                Bend(bendValue);

                int count = Physics.OverlapSphereNonAlloc(_tip.position, _tipRadius, collideResults);
                if (count>0)
                {
                    if (i != 0)
                    {
                        // 前一步没碰撞，后一步碰撞了，使用其中间量
                        bendValue -= 1 / (stepCount * 2f);
                        Bend(bendValue);
                    }
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 姿势

        /// <summary>
        /// 展开手指姿态数据
        /// </summary>
        public FingerPoseData minPose = new FingerPoseData();

        /// <summary>
        /// 握紧手指姿态数据
        /// </summary>
        public FingerPoseData maxPose = new FingerPoseData();

        /// <summary>
        /// 保存展开手指姿态数据
        /// </summary>
        [ContextMenu("保存展开手指数据")]
        public void SaveMinPose()
        {
            minPose.FingerToData(this);
        }

        /// <summary>
        /// 保存握紧手指数据
        /// </summary>
        [ContextMenu("保存握紧手指数据")]
        public void SaveMaxPose()
        {
            maxPose.FingerToData(this);
        }
        #endregion

        private void OnDrawGizmos()
        {
            if (!_tip) return;

            var orgColor = Gizmos.color;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_tip.position, _tipRadius);
            Gizmos.color = orgColor;
        }

        private void OnDrawGizmosSelected()
        {
            var orgColor = Gizmos.color;
            Gizmos.color = Color.blue;
            var children = transform.GetComponentsInChildren<CapsuleCollider>();
            foreach (var cap in children)
            {
                Gizmos.DrawWireSphere(cap.bounds.center, _tipRadius);
            }
            Gizmos.color = orgColor;
        }
    }

    /// <summary>
    /// 手指弯曲度输入接口
    /// </summary>
    public interface IFingerInput
    {
        /// <summary>
        /// 手指弯曲度
        /// </summary>
        float[] bends { get; }
    }

    /// <summary>
    /// 高级手指输入
    /// </summary>
    public interface IComplexFingerInput : IFingerInput
    {

    }

    /// <summary>
    /// 手指姿势数据
    /// </summary>
    [Serializable]
    public class FingerPoseData
    {
        /// <summary>
        /// 手指关节姿态数据
        /// </summary>
        public Pose[] jointPoses = new Pose[0];

        /// <summary>
        /// 姿态数据数量
        /// </summary>
        public int dataCount => jointPoses.Length;

        /// <summary>
        /// 手指姿势数据构造函数
        /// </summary>
        public FingerPoseData() { }

        /// <summary>
        /// 手指姿势数据构造函数
        /// </summary>
        /// <param name="finger"></param>
        public FingerPoseData(Finger finger)
        {
            FingerToData(finger);
        }

        /// <summary>
        /// 使用姿态数据设置手指
        /// </summary>
        /// <param name="finger"></param>
        /// <returns></returns>
        public bool DataToFinger(Finger finger)
        {
            if (!finger) return false;

            var count = dataCount;
            if (count != finger.jointCount)
            {
                return false;
            }

            for (int i = 0; i < count; i++)
            {
                var t = finger.joints[i];
                var p = jointPoses[i];
                t.XSetLocalPosition(p.position);
                t.XSetLocalRotation(p.rotation);
            }
            return true;
        }

        /// <summary>
        /// 读取手指姿态数据
        /// </summary>
        /// <param name="finger"></param>
        public void FingerToData(Finger finger)
        {
            if (!finger) return;

            int count = finger.jointCount;
            jointPoses = new Pose[count];
            for (int i = 0; i < count; i++)
            {
                var t = finger.joints[i];
                jointPoses[i] = new Pose(t.localPosition, t.localRotation);
            }
        }
    }
}
