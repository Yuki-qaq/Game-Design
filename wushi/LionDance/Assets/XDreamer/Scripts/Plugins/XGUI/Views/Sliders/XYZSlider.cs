using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// XYZ滑块面板
    /// </summary>
    [Name("XYZ滑块面板")]
    [Serializable]
    public class XYZSlider
    {
        /// <summary>
        /// XYZ面板
        /// </summary>
        [Name("XYZ面板")]
        public RectTransform root = null;

        /// <summary>
        /// X
        /// </summary>
        [Name("X")]
        public Slider x = null;

        /// <summary>
        /// Y
        /// </summary>
        [Name("Y")]
        public Slider y = null;

        /// <summary>
        /// Z
        /// </summary>
        [Name("Z")]
        public Slider z = null;

        /// <summary>
        /// 设置根激活
        /// </summary>
        /// <param name="active"></param>
        public void SetRootActive(bool active)
        {
            if (root)
            {
                root.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="value"></param>
        public void GetValue(ref Vector3 value)
        {
            if (x)
            {
                value.x = x.value;
            }

            if (y)
            {
                value.y = y.value;
            }

            if (z)
            {
                value.z = z.value;
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(Vector3 value)
        {
            if (x)
            {
                x.value = value.x;
            }

            if (y)
            {
                y.value = value.y;
            }

            if (z)
            {
                z.value = value.z;
            }
        }

        private Vector3 recordValue = Vector3.zero;

        /// <summary>
        /// 记录数据
        /// </summary>
        public void RecordData()
        {
            GetValue(ref recordValue);
        }

        /// <summary>
        /// 恢复数据
        /// </summary>
        public void RecoverData()
        {
            SetValue(recordValue);
        }

        /// <summary>
        /// 设置区间
        /// </summary>
        /// <param name="rangeX"></param>
        /// <param name="rangeY"></param>
        /// <param name="rangeZ"></param>
        public void SetRange(Vector2 rangeX, Vector2 rangeY, Vector2 rangeZ)
        {
            if (x)
            {
                x.minValue = rangeX.x;
                x.minValue = rangeX.y;
            }

            if (y)
            {
                y.minValue = rangeY.x;
                y.minValue = rangeY.y;
            }

            if (z)
            {
                z.minValue = rangeZ.x;
                z.minValue = rangeZ.y;
            }
        }
    }
}
