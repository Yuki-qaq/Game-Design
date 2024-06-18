using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static XCSJ.PluginSMS.States.Motions.Rotate;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils.Tools;
using UnityEngine;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Extension.Base.Dataflows.Base;

namespace XCSJ.PluginTools.Motions
{
    /// <summary>
    /// 刚体旋转
    /// </summary>
    [Name("刚体旋转", nameof(RotateRigidbody))]
    [Tool(ToolsCategory.Motion, rootType = typeof(ToolsManager))]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    internal class RotateRigidbody : PlayableContent
    {
        /// <summary>
        /// 3D刚体
        /// </summary>
        [Group("旋转设置", textEN = "Rotate Settings")]
        [Name("3D刚体")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Tip("用于控制自动移动的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象", "Used to control automatically moving objects; If this parameter is invalid, the component object of the current parameter type on the game object where the current component is located will be used")]
        [ComponentPopup]
        public Rigidbody _rigidbody3D;

        /// <summary>
        /// 3D刚体
        /// </summary>
        public Rigidbody rigidbody3D
        {
            get
            {
                if (!_rigidbody3D)
                {
                    _rigidbody3D = GetComponent<Rigidbody>();
                }
                return _rigidbody3D;
            }
        }

        /// <summary>
        /// 旋转轴
        /// </summary>
        [Name("旋转轴")]
        public Vector3PropertyValue _axis = new Vector3PropertyValue();

        /// <summary>
        /// 旋转角度
        /// </summary>
        [Name("旋转角度")]
        public float _axisAngle = 0;

        /// <summary>
        /// 禁用后还原角度
        /// </summary>
        [Name("禁用后还原角度")]
        public bool _recoverAngleOnDisable = true;

        private Quaternion startQuaternion = Quaternion.identity;

        private bool isKinematic { get; set; } = false;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (rigidbody3D) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            var rigidbody3D = this.rigidbody3D;
            if (!rigidbody3D)
            {
                enabled = false;
                return;
            }
            startQuaternion = rigidbody3D.rotation;

            isKinematic = rigidbody3D.isKinematic;
            rigidbody3D.isKinematic = true;

            base.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            var rigidbody3D = this.rigidbody3D;
            if (rigidbody3D)
            {
                rigidbody3D.isKinematic = isKinematic;
                if (_recoverAngleOnDisable)
                {
                    rigidbody3D.rotation = startQuaternion;
                }
            }

            base.OnDisable();
        }

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="playableData"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            if (_axis.TryGetValue(out var axis))
            {
                _rigidbody3D.MoveRotation(startQuaternion * Quaternion.AngleAxis((float)MathX.Lerp(0, _axisAngle, percent.percent01OfWorkCurve), axis));
            }
        }
    }
}
