using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPhysicses.Base.Recorders
{
    /// <summary>
    /// 铰链关节记录器
    /// </summary>
    public class HingeJointRecorder : JointRecorder<HingeJoint, HingeJointRecorder.Info>
    {
        /// <summary>
        /// 信息
        /// </summary>
        public class Info : ISingleRecord<HingeJoint>
        {
            /// <summary>
            /// 铰链关节
            /// </summary>
            public HingeJoint _hingeJoint;

            private bool useMotor;

            private JointMotor motor;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="hingeJoint"></param>
            public void Record(HingeJoint hingeJoint)
            {
                useMotor = hingeJoint.useMotor;
                motor = hingeJoint.motor;
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                _hingeJoint.useMotor = useMotor;
                _hingeJoint.motor = motor;
            }
        }
    }
}
