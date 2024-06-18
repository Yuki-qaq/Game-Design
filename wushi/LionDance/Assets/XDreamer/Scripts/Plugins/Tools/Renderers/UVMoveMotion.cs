using UnityEngine;
using System.Collections;
using XCSJ;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Interactions.Base;

namespace XCSJ.PluginTools.Renderers
{
    /// <summary>
    /// 移动UV坐标组件
    /// </summary>
    [Tool(ToolsCategory.Renderer, rootType = typeof(ToolsManager))]
    [Name("UV平移")]
    [XCSJ.Attributes.Icon(EIcon.Material)]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public class UVMoveMotion: PlayableContent
    {
        /// <summary>
        /// UV平移对象
        /// </summary>
        [Name("UV平移对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Renderer _renderer;

        /// <summary>
        /// UV移动速度
        /// </summary>
        [Name("UV移动速度")]
        public Vector2 uvOffsetSpeed = new Vector2(1, 1);

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        { 
            base.Reset();

            _renderer = GetComponentInChildren<Renderer>();
        }

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="playableData"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            base.OnSetPercent(percent, playableData);

            if (_renderer && _renderer.material)
            {
                _renderer.material.mainTextureOffset = uvOffsetSpeed * (float)percent.percent01OfWorkCurve;
            }
        }
    }
}
