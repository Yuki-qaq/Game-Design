using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.Layouts
{
    /// <summary>
    /// 圆形窗口
    /// </summary>
    [Name("圆形")]
    public class CircleWindow : IRectTransformLayoutWindow, ITransformLayoutWindow
    {
        /// <summary>
        /// 展开
        /// </summary>
        [Name("展开")]
        public bool expanded { get; set; } = true;

        /// <summary>
        /// 面向量
        /// </summary>
        [Name("面向量")]
        public Vector3 planeNormal = Vector3.up;

        /// <summary>
        /// 中心
        /// </summary>
        [Name("中心")]
        public Vector3 center = new Vector3();

        /// <summary>
        /// 半径
        /// </summary>
        [Name("半径")]
        public float r = 160;

        /// <summary>
        /// 起始角度
        /// </summary>
        [Name("起始角度")]
        public float beginAngle = 0f;

        /// <summary>
        /// 方向
        /// </summary>
        [Name("方向")]
        public Circle.EDirection direction = Circle.EDirection.None;

        /// <summary>
        /// 布局
        /// </summary>
        [Name("布局")]
        [Tip("圆形布局", "Circular layout")]
        [XCSJ.Attributes.Icon(index = 36233)]
        public XGUIContent CircleLayout { get; } = new XGUIContent(typeof(CircleWindow), nameof(CircleLayout));

        private void ShareDraw(Action onLayoutButtonClicked, Action onAfterDrawLayoutButton, params Transform[] standards)
        {
            EditorGUILayout.BeginHorizontal();
            center = EditorGUILayout.Vector3Field(CommonFun.NameTooltip(this, nameof(center)), center);
            if (GUILayout.Button(CommonFun.TempContent("父级中心", "以 标准(矩形)变换1 的父级(矩形)变换为基准"), GUILayout.Width(80)) && standards != null && standards.Length > 1)
            {
                var pt = standards[0].parent as Transform;
                if (pt)
                {
                    center = pt.position;
                }
            }
            EditorGUILayout.EndHorizontal();

            r = EditorGUILayout.FloatField(CommonFun.NameTooltip(this, nameof(r)), r);

            beginAngle = EditorGUILayout.Slider(CommonFun.NameTooltip(this, nameof(beginAngle)), beginAngle, 0, 360);

            direction = (Circle.EDirection)UICommonFun.EnumPopup(CommonFun.NameTooltip(this, nameof(direction)), direction);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("操作");
            if (GUILayout.Button(CircleLayout, ToolEditorWindowOption.weakInstance.defaultButtonSizeOption))
            {
                onLayoutButtonClicked?.Invoke();
            }
            onAfterDrawLayoutButton?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        bool ILayoutWindow<RectTransform>.OnGUI(List<RectTransform> list, params RectTransform[] standards)
        {
            bool ret = false;

            ShareDraw(() =>
            {
                Circle.Layout(list, center, r, beginAngle, direction);
                ret = true;
            }, null, standards as Transform[]);

            return ret;
        }

        bool ILayoutWindow<Transform>.OnGUI(List<Transform> list, params Transform[] standards)
        {
            bool ret = false;

            planeNormal = EditorGUILayout.Vector3Field(CommonFun.NameTooltip(this, nameof(planeNormal)), planeNormal);

            ShareDraw(() =>
            {
                Circle.Layout(planeNormal, list, center, r, beginAngle, direction);
                ret = true;
            }, null, standards as Transform[]);

            return ret;
        }
    }
}
