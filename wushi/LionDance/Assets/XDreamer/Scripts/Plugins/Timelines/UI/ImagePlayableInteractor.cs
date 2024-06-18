using UnityEngine;
using UnityEngine.UI;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;

namespace XCSJ.PluginTimelines.UI
{
    /// <summary>
    /// 图片可播放交互器
    /// </summary>
    public class ImagePlayableInteractor : PlayableContent
    {
        /// <summary>
        /// 图片
        /// </summary>
        public Image _image;

        /// <summary>
        /// 源颜色
        /// </summary>
        public Color _sourceColor = Color.white;

        /// <summary>
        /// 目标颜色
        /// </summary>
        public Color _targetColor = Color.white;

        /// <summary>
        /// 重置为默认值
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!_image)
            {
                this.XGetComponent<Image>(ref _image);
            }
        }

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="playableData"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            _image.color = Color.Lerp(_sourceColor, _targetColor, (float)percent.percent01OfWorkCurve);
        }
    }
}

