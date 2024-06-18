using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using static XCSJ.PluginTools.Draggers.SingleSocket;

namespace XCSJ.PluginTools.Hands
{
    /// <summary>
    /// 人体:计算身体抬高降低和双脚迈步
    /// </summary>
    [Name("人体")]
    [Tip("计算身体抬高降低和双脚迈步", "Calculate body lifting and lowering as well as walking with both feet")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [RequireManager(typeof(ToolsExtensionManager))]
    [Tool(ToolsExtensionCategory.HumanBody, rootType = typeof(ToolsExtensionManager))]
    public class HumanBody : Interactor
    {
        /// <summary>
        /// 跟随目标
        /// </summary>
        [Serializable]
        public class FollowTarget
        {
            /// <summary>
            /// 目标
            /// </summary>
            [Name("目标")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            public Transform _target;

            /// <summary>
            /// 跟随者
            /// </summary>
            [Name("跟随者")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            public Transform _follower;

            /// <summary>
            /// 姿态类型
            /// </summary>
            [Name("姿态类型")]
            [EnumPopup]
            public EPoseType _poseType = EPoseType.PositionAndRotation;

            /// <summary>
            /// 位移偏移空间类型
            /// </summary>
            [Name("位移偏移空间类型")]
            [EnumPopup]
            [HideInSuperInspector(nameof(_poseType), EValidityCheckType.Equal, EPoseType.Rotation)]
            public ESpaceType _positionOffsetSpaceType = ESpaceType.Local;

            /// <summary>
            /// 位移偏移量
            /// </summary>
            [Name("位移偏移量")]
            [HideInSuperInspector(nameof(_poseType), EValidityCheckType.Equal, EPoseType.Rotation)]
            public Vector3PropertyValue _positionOffset = new Vector3PropertyValue();

            /// <summary>
            /// 角度偏移量
            /// </summary>
            [Name("角度偏移量")]
            [HideInSuperInspector(nameof(_poseType), EValidityCheckType.Equal, EPoseType.Position)]
            public Vector3PropertyValue _angleOffset = new Vector3PropertyValue();

            /// <summary>
            /// 跟随
            /// </summary>
            public void Follow()
            {
                if (_follower && _target && _follower != _target)
                {
                    SynPose(_poseType);
                }
            }

            private void SynPose(EPoseType poseType)
            {
                switch (poseType)
                {
                    case EPoseType.PositionAndRotation:
                        {
                            SynPose(EPoseType.Rotation);
                            SynPose(EPoseType.Position);
                            break;
                        }
                    case EPoseType.Position:
                        {
                            var offset = _positionOffset.GetValue();
                            switch (_positionOffsetSpaceType)
                            {
                                case ESpaceType.World:
                                    {
                                        _follower.position = _target.position + offset;
                                        break;
                                    }
                                case ESpaceType.Local:
                                    {
                                        _follower.position = _target.position + offset.x * _follower.right + offset.y * _follower.up + offset.z * _follower.forward;
                                        break;
                                    }
                            }
                            break;
                        }
                    case EPoseType.Rotation:
                        {
                            _follower.rotation = _target.rotation * Quaternion.Euler(_angleOffset.GetValue());
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 头
        /// </summary>
        [Name("头")]
        public FollowTarget _head = new FollowTarget();

        /// <summary>
        /// 身体
        /// </summary>
        [Name("身体")]
        public FollowTarget _body = new FollowTarget();

        /// <summary>
        /// 左手
        /// </summary>
        [Name("左手")]
        public FollowTarget _leftHand = new FollowTarget();

        /// <summary>
        /// 右手
        /// </summary>
        [Name("右手")]
        public FollowTarget _rightHand = new FollowTarget();

        /// <summary>
        /// 身体朝向规则
        /// </summary>
        public enum EBodyForwardRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 头和手
            /// </summary>
            [Name("头和手")]
            HeadHand,
        }

        /// <summary>
        /// 身体朝向规则
        /// </summary>
        [Name("身体朝向规则")]
        [EnumPopup]
        public EBodyForwardRule _bodyForwardRule = EBodyForwardRule.HeadHand;

        /// <summary>
        /// 左脚步
        /// </summary>
        [Name("左脚步")]
        public FootStep _leftFootStep;

        /// <summary>
        /// 右脚步
        /// </summary>
        [Name("右脚步")]
        public FootStep _rightFootStep;

        /// <summary>
        /// 其他跟随目标列表
        /// </summary>
        [Name("其他跟随目标列表")]
        public List<FollowTarget> _otherFollowTargetList = new List<FollowTarget>();

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _body._follower = transform;
        }

        /// <summary>
        /// 如果启用 Behaviour，则在每一帧都将调用 LateUpdate
        /// </summary>
        protected void LateUpdate()
        {
            _head.Follow();
            _body.Follow();
            _leftHand.Follow();
            _rightHand.Follow();

            UpdateBodyForward();

            UpdateFoot();

            foreach (var f in _otherFollowTargetList)
            {
                f.Follow();
            }
        }

        /// <summary>
        /// 计算身体在XZ平面朝向
        /// </summary>
        private void UpdateBodyForward()
        {
            var body = _body._follower;
            if (!body) return;

            switch (_bodyForwardRule)
            {
                case EBodyForwardRule.HeadHand:
                    {
                        var head = _head._follower;
                        if (!head) return;

                        Vector3 f1, f2, f3;
                        f1 = f2 = f3 = body.forward;

                        f1 = Vector3.ProjectOnPlane(head.forward, Vector3.up).normalized;

                        var lh = _leftHand._follower;
                        if (lh)
                        {
                            f2 = Vector3.ProjectOnPlane(lh.position - head.position, Vector3.up).normalized;
                        }
                        var rh = _rightHand._follower;
                        if (rh)
                        {
                            f3 = Vector3.ProjectOnPlane(rh.position - head.position, Vector3.up).normalized;
                        }

                        body.forward = (f1 + f2 + f3) / 3;
                        break;
                    }
            }
        }

        private void UpdateFoot()
        {
            if (_leftFootStep)
            {
                _leftFootStep.TryMove();
            }

            if (_rightFootStep)
            {
                _rightFootStep.TryMove();
            }
        }
    }
}

