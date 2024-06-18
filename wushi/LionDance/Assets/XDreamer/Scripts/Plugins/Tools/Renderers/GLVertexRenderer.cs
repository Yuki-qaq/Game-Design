using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Tweens;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using ZXing.QrCode.Internal;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Renderers
{
    /// <summary>
    /// GL顶点渲染器
    /// </summary>
    [Tool(ToolsCategory.Renderer, rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Render)]
    [Name("GL顶点渲染器")]
    public class GLVertexRenderer : ToolRenderer
    {
        /// <summary>
        /// 材质
        /// </summary>
        [Name("材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material material;

        /// <summary>
        /// 模式
        /// </summary>
        [Name("模式")]
        [EnumPopup]
        public EGLMode mode = EGLMode.LINES;

        /// <summary>
        /// 线条类型
        /// </summary>
        [Name("线条类型")]
        public enum ELineType
        {
            /// <summary>
            /// 线段
            /// </summary>
            [Name("线段")]
            LineSegments,

            /// <summary>
            /// 多边形线
            /// </summary>
            [Name("多边形线")]
            PolyLine,

            /// <summary>
            /// 贝塞尔
            /// </summary>
            [Name("贝塞尔")]
            Bezier,

            /// <summary>
            /// 贝塞尔多边形
            /// </summary>
            [Name("贝塞尔多边形")]
            BezierPolygon,
        }

        /// <summary>
        /// 线条类型
        /// </summary>
        [Name("线条类型")]
        [HideInSuperInspector(nameof(mode), EValidityCheckType.NotEqual, EGLMode.LINES)]
        [EnumPopup]
        public ELineType lineType = ELineType.PolyLine;

        /// <summary>
        /// 顶点类型
        /// </summary>
        [Name("顶点类型")]
        public enum EVertexType
        {
            /// <summary>
            /// 变换
            /// </summary>
            [Name("变换")]
            [Tip("使用变换的世界位置坐标", "Use transformed world position coordinates")]
            Transform,

            /// <summary>
            /// 点
            /// </summary>
            [Name("点")]
            [Tip("基于世界坐标的点")]
            Point,
        }

        /// <summary>
        /// 顶点类型
        /// </summary>
        [Name("顶点类型")]
        [EnumPopup]
        public EVertexType vertexType = EVertexType.Transform;

        /// <summary>
        /// 变换列表
        /// </summary>
        [Name("变换列表")]
        [HideInSuperInspector(nameof(vertexType), EValidityCheckType.NotEqual, EVertexType.Transform)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public List<Transform> transforms = new List<Transform>();

        /// <summary>
        /// 点列表
        /// </summary>
        [Name("点列表")]
        [HideInSuperInspector(nameof(vertexType), EValidityCheckType.NotEqual, EVertexType.Point)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public List<Vector3> points = new List<Vector3>();

        /// <summary>
        /// 点类型
        /// </summary>
        [Name("点类型")]
        [EnumPopup]
        public EPointType pointType = EPointType.World;

        /// <summary>
        /// 点类型
        /// </summary>
        [Name("点类型")]
        public enum EPointType
        {
            /// <summary>
            /// 世界
            /// </summary>
            [Name("世界")]
            World,

            /// <summary>
            /// 本地
            /// </summary>
            [Name("本地")]
            Local,

            /// <summary>
            /// 本地旋转
            /// </summary>
            [Name("本地旋转")]
            LocalRotation,
        }

        /// <summary>
        /// 颜色列表
        /// </summary>
        [Name("颜色列表")]
        public Color[] colors = new Color[] { };

        /// <summary>
        /// 缺省颜色
        /// </summary>
        [Name("缺省颜色")]
        public Color defaultColor = Color.white;

        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Color GetColor(int i) => (i < 0 || i >= colors.Length) ? defaultColor : colors[i];

        /// <summary>
        /// 顶点
        /// </summary>
        public List<Vector3> vertexes
        {
            get
            {
                switch (vertexType)
                {
                    case EVertexType.Transform: return transforms.Where(t => t).ToList(t => t.position);
                    case EVertexType.Point:
                        {
                            switch (pointType)
                            {
                                case EPointType.Local:
                                    {
                                        var t = this.transform;
                                        return points.ToList(p => p + t.position);
                                    }
                                case EPointType.LocalRotation:
                                    {
                                        var t = this.transform;
                                        return points.ToList(p => t.rotation * p + t.position);
                                    }
                                case EPointType.World:
                                default:
                                    {
                                        return points;
                                    }
                            }
                        }
                    default: throw new ArgumentException("无效顶点类型:" + vertexType.ToString(), nameof(vertexType));
                }
            }
        }

        private void DrawVertexes()
        {
            var vertexes = this.vertexes;
            for (int i = 0; i < vertexes.Count; i++)
            {
                GL.Color(GetColor(i));
                GL.Vertex(vertexes[i]);
            }
        }

        /// <summary>
        /// 当渲染对象
        /// </summary>
        public void OnRenderObject()
        {
            if (GLHelper.Begin(material, transform, mode))
            {
                switch (mode)
                {
                    case EGLMode.LINES:
                        {
                            switch (lineType)
                            {
                                case ELineType.LineSegments:
                                    {
                                        DrawVertexes();
                                        break;
                                    }
                                case ELineType.PolyLine:
                                    {
                                        var vertexes = this.vertexes;
                                        for (int i = 1; i < vertexes.Count; i++)
                                        {
                                            GL.Color(GetColor(i - 1));
                                            GL.Vertex(vertexes[i - 1]);

                                            GL.Color(GetColor(i));
                                            GL.Vertex(vertexes[i]);
                                        }
                                        break;
                                    }
                                case ELineType.Bezier:
                                    {
                                        GLHelper.DrawBezierLine(vertexes.ToArray(), colors, defaultColor);
                                        break;
                                    }
                                case ELineType.BezierPolygon:
                                    {
                                        var path01 = MathU.BezierPolygonPathControlPointGenerator(vertexes.ToArray());
                                        GLHelper.DrawLine(path01, colors, defaultColor);
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            DrawVertexes();
                            break;
                        }
                }

                GLHelper.End();
            }          
        }

#if UNITY_EDITOR

        /// <summary>
        /// 当绘制选中的Gizmos
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) return;

            switch (mode)
            {
                case EGLMode.LINES:
                    {
                        var vertexes = this.vertexes;
                        switch (lineType)
                        {
                            case ELineType.LineSegments:
                                {
                                    var count = vertexes.Count;
                                    vertexes = vertexes.GetRange(0, count % 2 == 0 ? count : count - 1);
                                    UnityEditor.Handles.DrawLines(vertexes.ToArray());
                                    break;
                                }
                            case ELineType.PolyLine:
                                {
                                    UnityEditor.Handles.DrawPolyLine(vertexes.ToArray());
                                    break;
                                }
                            case ELineType.Bezier:
                                {
                                    XGizmos.DrawBezierLine(vertexes.ToArray(), defaultColor);
                                    break;
                                }
                            case ELineType.BezierPolygon:
                                {
                                    var path01 = MathU.BezierPolygonPathControlPointGenerator(vertexes.ToArray());
                                    XGizmos.DrawLine(path01, defaultColor);
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

#endif
    }
}
