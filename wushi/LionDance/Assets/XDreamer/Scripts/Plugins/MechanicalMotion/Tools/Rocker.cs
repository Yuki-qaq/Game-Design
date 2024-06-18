using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMechanicalMotion.Tools
{
    /// <summary>
    /// 摇杆:朝向指定关节摆动的机构
    /// </summary>
    [Name("摇杆")]
    [XCSJ.Attributes.Icon()]
    [Tool(MechanicalMotionCategory.Title, rootType = typeof(MechanicalMotionManager))]
    public class Rocker : Mechanism
    {
        /// <summary>
        /// 对齐规则
        /// </summary>
        public enum EAlignRule
        {
            /// <summary>
            /// 朝向变换
            /// </summary>
            [Name("朝向变换")]
            LookAtTransform,

            /// <summary>
            /// 朝向对齐方向
            /// </summary>
            [Name("朝向对齐方向")]
            ForwardAlignDirection,
        }

        /// <summary>
        /// 对齐规则
        /// </summary>
        [Name("对齐规则")]
        [EnumPopup]
        public EAlignRule _alignRule = EAlignRule.LookAtTransform;

        /// <summary>
        /// 朝向关节
        /// </summary>
        [Name("朝向关节")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_alignRule), EValidityCheckType.NotEqual, EAlignRule.LookAtTransform)]
        public Transform _lookatJoint;

        /// <summary>
        /// 对齐方向
        /// </summary>
        [Name("对齐方向")]
        [HideInSuperInspector(nameof(_alignRule), EValidityCheckType.NotEqual, EAlignRule.ForwardAlignDirection)]
        public Vector3Data _alignDirection = new Vector3Data();

        /// <summary>
        /// 朝向上向量
        /// </summary>
        [Name("朝向上向量")]
        [Tip("设定变换朝向时，需指定一个参考上向量才能正确设定朝向", "When setting the transformation orientation, you need to specify a reference upper vector to correctly set the orientation")]
        public Vector3Data _lookatUpAxis = new Vector3Data();

        /// <summary>
        /// 重置：设定朝向上向量引用为自身,默认方向为上
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _lookatUpAxis._transform._transfrom = transform;
            _lookatUpAxis._dataType = EVector3DataType.Vector3;
            _lookatUpAxis._vector3._value = Vector3.up;
        }

        /// <summary>
        /// 能否做运动
        /// </summary>
        /// <returns></returns>
        public override bool CanDoMotion()
        {
            switch (_alignRule)
            {
                case EAlignRule.LookAtTransform: return _lookatJoint;
                case EAlignRule.ForwardAlignDirection: return _alignDirection.data != Vector3.zero;
                default: return false;
            }
        }

        /// <summary>
        /// 执行运动
        /// </summary>
        public override void DoMotion()
        {
            switch (_alignRule)
            {
                case EAlignRule.LookAtTransform:
                    {
                        transform.LookAt(_lookatJoint, _lookatUpAxis.data);
                        break;
                    }
                case EAlignRule.ForwardAlignDirection:
                    {
                        transform.LookAt(transform.position + _alignDirection.data, _lookatUpAxis.data);
                        break;
                    }
            }
        }
    }
}
