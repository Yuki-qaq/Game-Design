using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络颜色
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Color)]
    [DisallowMultipleComponent]
    [Name("网络颜色")]
    [Tool(MMOHelperExtension.Title, nameof(NetIdentity), rootType = typeof(MMOManager))]
    public sealed class NetColor : NetMB, IAwake
    {
        /// <summary>
        /// 渲染器
        /// </summary>
        [Name("渲染器")]
        [Readonly(EEditorMode.Runtime)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Renderer _renderer;

        /// <summary>
        /// 颜色
        /// </summary>
        [SyncVar]
        [Readonly]
        [Name("颜色")]
        public List<Color> _colors = new List<Color>();

        /// <summary>
        /// 之前颜色
        /// </summary>
        [Readonly]
        [Name("之前颜色")]
        public List<Color> _prevColors = new List<Color>();

        /// <summary>
        /// 原始颜色
        /// </summary>
        [Readonly]
        [Name("原始颜色")]
        public List<Color> _originalColors = new List<Color>();

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _renderer = GetComponent<Renderer>();
        }

        /// <summary>
        /// 唤醒：初始化
        /// </summary>
        public void Awake()
        {
            if (!_renderer)
            {
                _renderer = GetComponent<Renderer>();
            }
        }

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            base.OnMMOEnterRoomCompleted(result);

            if (result == EACode.Success)
            {
                GetRendererColor();
                _originalColors.AddRange(_colors);
                _prevColors.AddRange(_colors);
            }
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            base.OnMMOExitRoomCompleted();

            SetRendererColor(_originalColors);
            _originalColors.Clear();
            _prevColors.Clear();
        }

        /// <summary>
        /// 当定时检查修改
        /// </summary>
        /// <returns></returns>
        protected override bool OnTimedCheckChange()
        {
            GetRendererColor();

            for (int i = 0; i < _colors.Count; i++)
            {
                if (i < _prevColors.Count)
                {
                    if (_colors[i] != _prevColors[i])
                    {
                        _prevColors.Clear();
                        _prevColors.AddRange(_colors);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 当同步变量已修改
        /// </summary>
        protected override void OnSyncVarChanged()
        {
            base.OnSyncVarChanged();

            SetRendererColor(_colors);
        }

        private void GetRendererColor()
        {
            if (_renderer)
            {
                _colors.Clear();
                foreach (var m in _renderer.materials)
                {
                    _colors.Add(m ? m.color : Color.clear);
                }
            }
        }

        private void SetRendererColor(List<Color> colorList)
        {
            if (_renderer && colorList != null)
            {
                for (int i = 0; i < colorList.Count && i < _renderer.materials.Length; i++)
                {
                    var m = _renderer.materials[i];
                    if (m)
                    {
                        m.color = colorList[i];
                    }
                }
            }
        }
    }
}
