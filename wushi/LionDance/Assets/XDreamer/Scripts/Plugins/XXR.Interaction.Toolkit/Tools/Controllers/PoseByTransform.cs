using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// ��̬ͨ���任
    /// </summary>
    [Name("��̬ͨ���任")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.TrackerPose, nameof(AnalogController))]
    [XCSJ.Attributes.Icon(EIcon.GameObject)]
    [DisallowMultipleComponent]
    public class PoseByTransform : BaseAnalogProvider, IPoseProvider
    {
        /// <summary>
        /// �任
        /// </summary>
        [Name("�任")]
        public TransformPropertyValue _transformPropertyValue = new TransformPropertyValue();

        /// <summary>
        /// λ��ƫ����
        /// </summary>
        [Name("λ��ƫ����")]
        public Vector3PropertyValue _positionOffsetPropertyValue = new Vector3PropertyValue();

        /// <summary>
        /// ��תƫ����
        /// </summary>
        [Name("��תƫ����")]
        public Vector3PropertyValue _rotationOffsetPropertyValue = new Vector3PropertyValue();

        /// <summary>
        /// ����
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _transformPropertyValue._transfrom = transform;
        }

        /// <summary>
        /// ���Ի�ȡ��̬
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

