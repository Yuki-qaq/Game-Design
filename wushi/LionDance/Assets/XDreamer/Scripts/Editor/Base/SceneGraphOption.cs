using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.LitJson;

namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// 场景图形绘制
    /// </summary>
    [XDreamerPreferences]
    [Name("场景图形绘制")]
    [Import]
    public class SceneGraphOption : XDreamerOption<SceneGraphOption>
    {
        /// <summary>
        /// 文字尺寸
        /// </summary>
        [Name("文字尺寸")]
        public int labelSize = 16;

        /// <summary>
        /// 文字颜色
        /// </summary>
        [Name("文字颜色")]
        [Json(exportString = true)]
        public Color labelColor = Color.white;

        /// <summary>
        /// 线颜色
        /// </summary>
        [Name("线颜色")]
        [Json(exportString = true)]
        public Color lineColor = Color.cyan;

        /// <summary>
        /// 面颜色
        /// </summary>
        [Name("面颜色")]
        [Json(exportString = true)]
        public Color planeColor = new Color(0, 1, 1, 0.2f);

        /// <summary>
        /// 关键点颜色
        /// </summary>
        [Name("关键点颜色")]
        [Json(exportString = true)]
        public Color keyPointColor = Color.magenta;

        /// <summary>
        /// 大球体尺寸
        /// </summary>
        [Name("大球体尺寸")]
        public float bigSphereRadius = 0.4f;

        /// <summary>
        /// 正常球体尺寸
        /// </summary>
        [Name("正常球体尺寸")]
        public float sphereRadius = 0.2f;

        /// <summary>
        /// 小球体尺寸
        /// </summary>
        [Name("小球体尺寸")]
        public float smallSphereRadius = 0.1f;

        /// <summary>
        /// 半径
        /// </summary>
        [Name("半径")]
        public float radius = 1f;

        /// <summary>
        /// 箭头颜色
        /// </summary>
        [Name("箭头颜色")]
        [Json(exportString = true)]
        public Color arrowColor = Color.yellow;

        /// <summary>
        /// 箭头长度
        /// </summary>
        [Name("箭头长度")]
        public float arrowLength = 2f;
    }
}
