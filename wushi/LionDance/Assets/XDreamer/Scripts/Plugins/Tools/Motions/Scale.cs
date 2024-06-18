using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Motions
{
    /// <summary>
    /// 缩放动画
    /// </summary>
    [Name("缩放", nameof(Scale))]
    [Tool(ToolsCategory.Motion, rootType = typeof(ToolsManager))]
    [XCSJ.Attributes.Icon(EIcon.Scale)]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public class Scale : PlayableContentWithTransform
    {
        /// <summary>
        /// 缩放值
        /// </summary>
        [Group("缩放设置", textEN = "Scale Settings")]
        [Name("缩放值")]
        public Vector3Value _scaleValue = new Vector3Value();

        /// <summary>
        /// 相对:为True时最终缩放值=当前变换的缩放量X缩放值;为False时最终缩放值为缩放值
        /// </summary>
        [Name("相对")]
        [Tip("为True时最终缩放值=当前变换的缩放量X缩放值;为False时最终缩放值为缩放值")]
        public bool _relative = false;

        private bool TryGetScale(out Vector3 scaleValue)
        {
            if (_scaleValue.TryGetScale(out scaleValue))
            {
                if (_relative)
                {
                    scaleValue = Vector3.Scale(scaleOnEnable, scaleValue);
                }
                return true;
            }
            return false;
        }

        private Vector3 scaleOnEnable = Vector3.one;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            scaleOnEnable = targetTransform.localScale;
            base.OnEnable();
        }

        /// <summary>
        /// 设置百分比回调
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="playableData"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            if (TryGetScale(out var scaleValue))
            {
                targetTransform.localScale = Vector3.Lerp(scaleOnEnable, scaleValue, (float)percent.percent01OfWorkCurve);
            }
        }
    }
}
