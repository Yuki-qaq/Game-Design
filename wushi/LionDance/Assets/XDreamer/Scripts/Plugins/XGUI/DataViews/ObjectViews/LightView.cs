using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.ObjectViews
{
    /// <summary>
    /// 灯光视图
    /// </summary>
    [Name("灯光视图")]
    [XCSJ.Attributes.Icon(EIcon.Lightning)]
    public class LightView : ComponentView
    {
        /// <summary>
        /// 灯光
        /// </summary>
        [Name("灯光")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Light _light;

        /// <summary>
        /// 灯光
        /// </summary>
        public Light lightObject => this.XGetComponentInParentOrGlobal(ref _light);

        /// <summary>
        /// Unity对象
        /// </summary>
        public override Object unityObject => this;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (lightObject) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            if (lightObject) { }

            base.OnEnable();
        }

        /// <summary>
        /// 灯光位置
        /// </summary>
        [Name("灯光位置")]
        public Vector3 position
        {
            get => _light ? _light.transform.position : default;
            set
            {
                if (_light) _light.transform.position = value;
            }
        }

        /// <summary>
        /// 灯光角度
        /// </summary>
        [Name("灯光角度")]
        public Vector3 angle
        {
            get => _light ? _light.transform.eulerAngles : default;
            set
            {
                if (_light) _light.transform.eulerAngles = value;
            }
        }

        /// <summary>
        /// 灯光颜色
        /// </summary>
        [Name("灯光颜色")]
        public Color color
        {
            get => _light ? _light.color : default;
            set
            {
                if (_light) _light.color = value;
            }
        }

        /// <summary>
        /// 灯光强度
        /// </summary>
        [Name("灯光强度")]
        public float intensity
        {
            get => _light ? _light.intensity : default;
            set
            {
                if (_light) _light.intensity = value;
            }
        }
    }
}
