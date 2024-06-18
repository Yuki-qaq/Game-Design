using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Effects;
using System.Collections.Generic;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Collections;

namespace XCSJ.PluginTools.Outlines
{
    /// <summary>
    /// 轮廓线
    /// </summary>
    [Name("轮廓线")]
    [RequireManager(typeof(ToolsManager))]
    public class Outline : InteractProvider, IOutline
    {
        /// <summary>
        /// 使用自定义参数
        /// </summary>
        [Name("使用自定义参数")]
        public bool _useCustom = false;

        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        public Color _color = Color.green;

        /// <summary>
        /// 宽度
        /// </summary>
        [Name("宽度")]
        public float _width = 2;

        /// <summary>
        /// 无遮挡
        /// </summary>
        [Name("无遮挡")]
        public bool _overlay = true;

#if XDREAMER_PROJECT_URP

        private Color orgColor;

        private float orgWidth;

        private bool orgDrawOnTop;

        private static OutlineFeature outlineFeature
        {
            get
            {
                if (!_outlineFeature)
                {
                    _outlineFeature = Resources.FindObjectsOfTypeAll<OutlineFeature>().FirstOrDefault();

                    if (!outlineFeature)
                    {
                        Debug.LogError("轮廓线特色未找到!");
                    }
                }
                return _outlineFeature;
            }
        }
        private static OutlineFeature _outlineFeature = null;

        private Renderer[] renderers = null;

        private List<RendererRenderingLayerMaskRecorder> rendererRenderingLayerMaskRecorders = new List<RendererRenderingLayerMaskRecorder>();

        protected override void OnEnable()
        {
            base.OnEnable();

            SetOutline();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            RecoverOutline();
        }

        private void OnTransformChildrenChanged()
        {
            RecoverOutline();
            ResetData();
            SetOutline();
        }

        private void SetOutline()
        {
            // 设置特性颜色
            orgColor = outlineFeature.settings.color;
            orgWidth = outlineFeature.settings.size;
            orgDrawOnTop = outlineFeature.settings.drawOnTop;

            if (_useCustom || outliner == null)
            {
                outlineFeature.settings.color = _color;
                outlineFeature.settings.size = _width;
                outlineFeature.settings.drawOnTop = _overlay;
            }
            else
            {
                outlineFeature.settings.color = outliner.color;
                outlineFeature.settings.size = outliner.width;
                outlineFeature.settings.drawOnTop = outliner.overlay;
            }

            // 设置渲染层级
            if (renderers == null)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }

            foreach (var r in renderers)
            {
                var recorder = new RendererRenderingLayerMaskRecorder();
                recorder.Record(r);
                rendererRenderingLayerMaskRecorders.Add(recorder);
                r.renderingLayerMask = outlineFeature.settings.renderingLayerMask;
            }
        }

        private void RecoverOutline()
        {
            // 还原渲染层级
            foreach (var r in rendererRenderingLayerMaskRecorders)
            {
                r.Recover();
            }

            // 还原特性颜色
            outlineFeature.settings.color = orgColor;
            outlineFeature.settings.size = orgWidth;
            outlineFeature.settings.drawOnTop = orgDrawOnTop;
        }

        private void ResetData()
        {
            renderers = null;
            rendererRenderingLayerMaskRecorders.Clear();
        }

#endif

        #region IOutline

        /// <summary>
        /// 可显示
        /// </summary>
        public bool canDisplay { get => enabled; set => enabled = value; }

        /// <summary>
        /// 是显示
        /// </summary>
        public bool isDisplay { get => enabled; set { } }

        private IOutlineData outliner;

        /// <summary>
        /// 开始显示
        /// </summary>
        /// <param name="outliner"></param>
        public void StartDisplay(IOutlineData outliner)
        {
            this.outliner = outliner;
            if (enabled)// 如果已经启用，则先禁用再启用
            {
                enabled = false;
            }
            enabled = true;
        }

        /// <summary>
        /// 结束显示
        /// </summary>
        public void StopDisplay()
        {
            enabled = false;
        }

        #endregion
    }
}
