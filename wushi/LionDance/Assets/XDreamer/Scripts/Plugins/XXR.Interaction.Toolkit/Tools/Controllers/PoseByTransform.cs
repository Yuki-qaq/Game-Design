using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 姿态通过变换
    /// </summary>
    [Name("姿态通过变换")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.TrackerPose, nameof(AnalogController))]
    [XCSJ.Attributes.Icon(EIcon.GameObject)]
    [DisallowMultipleComponent]
    public class PoseByTransform : BaseAnalogProvider, IPoseProvider
    {
        /// <summary>
        /// 变换
        /// </summary>
        [Name("变换")]
        public TransformPropertyValue _transformPropertyValue = new TransformPropertyValue();

        /// <summary>
        /// 位置偏移量
        /// </summary>
        [Name("位置偏移量")]
        public Vector3PropertyValue _positionOffsetPropertyValue = new Vector3PropertyValue();

        /// <summary>
        /// 旋转偏移量
        /// </summary>
        [Name("旋转偏移量")]
        public Vector3PropertyValue _rotationOffsetPropertyValue = new Vector3PropertyValue();

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _transformPropertyValue._transfrom = transform;
        }

        /// <summary>
        /// 尝试获取姿态
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public InputTrackingState TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation)
        {
            if (_transformPropertyValue.TryGetValue(out var t))
            {
                position = t.position + _positionOffsetPropertyValue.GetValue();
                rotation = t.rotation * Quaternion.Euler(_rotationOffsetPropertyValue.GetValue());
                return InputTrackingState.Position | InputTrackingState.Rotation;
            }
            position = default;
            rotation = default;
            return InputTrackingState.None;
        }
    }
}

