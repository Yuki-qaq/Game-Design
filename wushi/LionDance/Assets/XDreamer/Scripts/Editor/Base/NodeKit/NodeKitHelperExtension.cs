using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;
using XCSJ.PluginCommonUtils.NodeKit;

namespace XCSJ.EditorExtension.Base.NodeKit
{
    /// <summary>
    /// 节点工具助手扩展
    /// </summary>
    public static class NodeKitHelperExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init() => _customStyle = null;

        private static CustomStyle _customStyle;

        /// <summary>
        /// 自定义样式
        /// </summary>
        public static CustomStyle customStyle => _customStyle ?? (_customStyle = EditorGUIUtility.isProSkin ? (CustomStyle)new ProfessionalStyle() : new PersonalStyle());

        /// <summary>
        /// 创建实体节点视图
        /// </summary>
        /// <param name="nodeModelType"></param>
        /// <param name="useForParent"></param>
        /// <param name="canvasModelTypes"></param>
        /// <returns></returns>
        public static BaseEntityNodeView CreateEntityNodeView(Type nodeModelType, bool useForParent, params Type[] canvasModelTypes)
        {
            if (nodeModelType == null) return default;

            foreach (var canvasModelType in canvasModelTypes)
            {
                var viewType = NodeViewAttribute.GetNodeViewType(nodeModelType, canvasModelType, useForParent);
                if (TypeHelper.CreateInstance(viewType) is BaseEntityNodeView nodeView) return nodeView;
            }

            return EditorNodeKitHelper.CreateEntityNodeView(nodeModelType, null, useForParent);
        }

        /// <summary>
        /// 创建连接:在指定画布视图内创建入和出节点模型的连接，并返回关联的插槽节点视图
        /// </summary>
        public static bool CreateConnection(this CanvasView canvasView, INodeModel inNodeModel, INodeModel outNodeModel)
        {
            if (inNodeModel.ObjectIsNull() || outNodeModel.ObjectIsNull()) return false;

            var inNodeView = canvasView.displayNodeViews.Find(nv => nv.nodeModel == inNodeModel);
            if (inNodeView == null) return false;

            var outNodeView = canvasView.displayNodeViews.Find(nv => nv.nodeModel == outNodeModel);
            if (outNodeView == null) return false;

            var fromSlotNodeView = inNodeView.CreateOutSlotNodeView();
            if (fromSlotNodeView != null)
            {
                var toSlotNodeView = outNodeView.CreateInSlotNodeView();
                if (toSlotNodeView != null)
                {
                    var rs = fromSlotNodeView.ConnectTo(toSlotNodeView);
                    if (rs)
                    {
                        inNodeView.Relayout();
                        outNodeView.Relayout();
                    }
                    return rs;
                }
            }
            return false;
        }

        /// <summary>
        /// 绘制节点模型检查器
        /// </summary>
        /// <param name="nodeModel"></param>
        /// <param name="inspectorWidth"></param>
        /// <returns></returns>
        public static bool DrawNodeModelInspector(this INodeModel nodeModel, float inspectorWidth)
        {
            if (nodeModel.ObjectIsNull()) return false;

            if (nodeModel is UnityEngine.Object unityObj)
            {
                DrawInspector(unityObj, inspectorWidth);
                return true;
            }
            else if (nodeModel is INodeModelHost nodeModelHost)
            {
                var unityObject = nodeModelHost.nodeModelHostUnityObject;
                if (unityObject)
                {
                    DrawInspector(unityObject, inspectorWidth);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 绘制检查器
        /// </summary>
        /// <param name="unityObject"></param>
        /// <param name="inspectorWidth"></param>
        public static void DrawInspector(UnityEngine.Object unityObject, float inspectorWidth)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(17);
            EditorGUILayout.BeginVertical();
            BaseInspector.expectedWidth = inspectorWidth;
            BaseInspector.DrawEditor(unityObject);
            BaseInspector.expectedWidth = default;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 计算匹配GUI内容矩形：根据传入内容及矩形区域，自动生成一个以原矩形中心为中心，尺寸适配GUI内容的矩形区域
        /// </summary>
        /// <param name="guiContent"></param>
        /// <param name="orgRect">原始矩形：未匹配内容时的矩形区域</param>
        /// <param name="guiStyle"></param>
        /// <returns></returns>
        public static Rect CalculateMatchGUIContentRect(this GUIContent guiContent, Rect orgRect, GUIStyle guiStyle)
        {
            var contentRect = orgRect;
            contentRect.width = guiStyle.CalcSize(guiContent).x;
            if (contentRect.width < orgRect.width)
            {
                contentRect.width = orgRect.width;
            }
            else if (contentRect.width > orgRect.width)
            {
                contentRect.center = new Vector2(orgRect.center.x, contentRect.center.y);
            }
            return contentRect;
        }

        /// <summary>
        /// 绘制导入贴图：在节点视图内容区域中心绘制
        /// </summary>
        /// <param name="nodeView"></param>
        public static void DrawImportTexture2D(this NodeView nodeView) => DrawTexture2D(nodeView, customStyle.importTexture);

        /// <summary>
        /// 绘制导出贴图：在节点视图内容区域中心绘制
        /// </summary>
        /// <param name="nodeView"></param>
        public static void DrawExportTexture2D(this NodeView nodeView) => DrawTexture2D(nodeView, customStyle.exportTexture);

        /// <summary>
        /// 绘制贴图：在节点视图内容区域中心绘制
        /// </summary>
        /// <param name="nodeView"></param>
        /// <param name="texture2D"></param>
        public static void DrawTexture2D(this NodeView nodeView, Texture2D texture2D)
        {
            if (nodeView == null) return;

            var size = customStyle.iconSize;
            GUI.DrawTexture(new Rect(nodeView.contentRect.center - size / 2, size), texture2D);
        }

        /// <summary>
        /// 绘制样式：在绘制事件时有效
        /// </summary>
        /// <param name="guiStyle"></param>
        /// <param name="rect"></param>
        public static void DrawStyle(this GUIStyle guiStyle, Rect rect)
        {
            if (guiStyle == null) return;
            if (Event.current.type == EventType.Repaint)
            {
                guiStyle.Draw(rect, false, false, false, false);
            }
        }

        /// <summary>
        /// 更新连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="fromItem"></param>
        /// <param name="toItem"></param>
        /// <param name="equleFunc"></param>
        /// <returns></returns>
        public static bool UpdateConnect<T>(this List<T> list, T fromItem, T toItem, Func<T, T, bool> equleFunc = default)
        {
            if (equleFunc == null) equleFunc = (x, y) => EqualityComparer<T>.Default.Equals(x, y);

            var i = list.FindIndex(ti => equleFunc(ti, fromItem));
            var o = list.FindIndex(ti => equleFunc(ti, toItem));

            if (i >= 0 && o >= 0)
            {
                if (i == o) { }
                else if (i < o)
                {
                    if (o - i > 1)
                    {
                        list.RemoveAt(o);
                        list.Insert(i + 1, toItem);
                    }
                }
                else
                {
                    list.RemoveAt(i);
                    list.Insert(o, fromItem);
                }
                return true;
            }
            return false;
        }

        #region 常用节点视图

        /// <summary>
        /// 绘制进入节点视图
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="title"></param>
        public static void DrawEntryNodeView(this Rect rect, string title = "") => DrawNodeView(rect, title, customStyle.greenNodeStyle, customStyle.entryTexture);

        /// <summary>
        /// 绘制退出节点视图
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="title"></param>
        public static void DrawExitNodeView(this Rect rect, string title = "") => DrawNodeView(rect, title, customStyle.redNodeStyle, customStyle.exitTexture);

        /// <summary>
        /// 绘制导入节点视图
        /// </summary>
        /// <param name="nodeView"></param>
        public static void DrawImportNodeView(this NodeView nodeView) => DrawImportNodeView(nodeView.nodeRect, nodeView.displayName);

        /// <summary>
        /// 绘制导入节点视图
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="title"></param>
        public static void DrawImportNodeView(this Rect rect, string title = "") => DrawNodeView(rect, title, customStyle.cyanNodeStyle, customStyle.importTexture);

        /// <summary>
        /// 绘制导出节点视图
        /// </summary>
        /// <param name="nodeView"></param>
        public static void DrawExportNodeView(this NodeView nodeView) => DrawExportNodeView(nodeView.nodeRect, nodeView.displayName);

        /// <summary>
        /// 绘制导出节点视图
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="title"></param>
        public static void DrawExportNodeView(this Rect rect, string title = "") => DrawNodeView(rect, title, customStyle.orangeNodeStyle, customStyle.exportTexture);

        /// <summary>
        /// 绘制节点视图
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="title"></param>
        /// <param name="backgroundStyle"></param>
        /// <param name="texture2D"></param>
        public static void DrawNodeView(this Rect rect, string title, GUIStyle backgroundStyle, Texture2D texture2D)
        {
            // 绘制背景
            backgroundStyle?.DrawStyle(rect);

            // 绘制标题
            var nameContent = CommonFun.TempContent(title, title);
            GUI.Label(nameContent.CalculateMatchGUIContentRect(new Rect(rect.position.x, rect.position.y, rect.width, CustomStyle.NodeTitleFontSize), customStyle.titleStyleWithoutBackground), nameContent, customStyle.titleStyleWithoutBackground);

            // 绘制图标
            if (texture2D)
            {
                var size = customStyle.iconSize;
                GUI.DrawTexture(new Rect(rect.center + new Vector2(0, CustomStyle.NodeTitleFontSize / 2) - size / 2, size), texture2D);
            }
        }

        #endregion

        #region 绘制连线

        /// <summary>
        /// 绘制连接线
        /// </summary>
        /// <param name="fromNodeView"></param>
        /// <param name="toNodeView"></param>
        /// <param name="drawArrow"></param>
        /// <param name="fromPoint"></param>
        /// <param name="toPoint"></param>
        public static void DrawConnectionLine(this NodeView fromNodeView, NodeView toNodeView, bool drawArrow, out Vector2 fromPoint, out Vector2 toPoint) => DrawConnectionLine(fromNodeView.nodeRect, toNodeView.nodeRect, drawArrow, out fromPoint, out toPoint);

        /// <summary>
        /// 绘制连接线
        /// </summary>
        /// <param name="fromRect"></param>
        /// <param name="toRect"></param>
        /// <param name="drawArrow"></param>
        /// <param name="fromPoint"></param>
        /// <param name="toPoint"></param>
        public static void DrawConnectionLine(this Rect fromRect, Rect toRect, bool drawArrow, out Vector2 fromPoint, out Vector2 toPoint)
        {
            var dir = CalculateDirection(fromRect, toRect, customStyle.slotNodeSize);
            fromPoint = fromRect.LayoutClockwiseSide(dir, RectOutPointOffsetValue);
            toPoint = toRect.LayoutClockwiseSide(dir.Reverse(), RectInPointOffsetValue);
            DrawConnectionLine(fromPoint, toPoint, dir, drawArrow);
        }

        /// <summary>
        /// 绘制连接线
        /// </summary>
        /// <param name="fromNodeView"></param>
        /// <param name="toNodeView"></param>
        /// <param name="drawArrow"></param>
        public static void DrawConnectionLine(this NodeView fromNodeView, NodeView toNodeView, bool drawArrow = true) => DrawConnectionLine(fromNodeView.nodeRect, toNodeView.nodeRect, drawArrow);

        /// <summary>
        /// 绘制连接线
        /// </summary>
        /// <param name="fromRect"></param>
        /// <param name="toRect"></param>
        /// <param name="drawArrow">绘制箭头</param>
        public static void DrawConnectionLine(this Rect fromRect, Rect toRect, bool drawArrow = true)
        {
            var dir = CalculateDirection(fromRect, toRect, customStyle.slotNodeSize);
            DrawConnectionLine(fromRect.LayoutClockwiseSide(dir, RectOutPointOffsetValue), toRect.LayoutClockwiseSide(dir.Reverse(), RectInPointOffsetValue), dir, drawArrow);
        }

        /// <summary>
        /// 绘制连线
        /// </summary>
        /// <param name="fromPoint"></param>
        /// <param name="toPoint"></param>
        /// <param name="drawArrow"></param>
        public static void DrawConnectionLine(this Vector2 fromPoint, Vector2 toPoint, bool drawArrow = true)
        {
            DrawConnectionLine(fromPoint, toPoint, CalculateDirection(fromPoint, toPoint), drawArrow);
        }

        /// <summary>
        /// 绘制连线
        /// </summary>
        /// <param name="fromPoint"></param>
        /// <param name="toPoint"></param>
        /// <param name="direction"></param>
        /// <param name="drawArrow"></param>
        public static void DrawConnectionLine(this Vector2 fromPoint, Vector2 toPoint, EDirection direction, bool drawArrow = true) => DrawConnectionLine(fromPoint, toPoint, direction, direction.Reverse(), drawArrow);

        /// <summary>
        /// 绘制连线
        /// </summary>
        /// <param name="fromPoint"></param>
        /// <param name="toPoint"></param>
        /// <param name="fromDirection"></param>
        /// <param name="toDirection"></param>
        /// <param name="drawArrow"></param>
        public static void DrawConnectionLine(this Vector2 fromPoint, Vector2 toPoint, EDirection fromDirection, EDirection toDirection, bool drawArrow = true)
        {
            DrawBezierLine(fromPoint, toPoint, Color.white, 3, fromDirection, toDirection);
            if (drawArrow)
            {
                var slotSize = customStyle.slotNodeSize;
                GUI.DrawTexture(new Rect(toPoint - slotSize / 2, slotSize), toDirection.Reverse().GetArrowTexture());
            }
        }

        /// <summary>
        /// 线类型
        /// </summary>
        [Name("线类型")]
        public enum ELineType
        {
            /// <summary>
            /// 贝塞尔曲线
            /// </summary>
            [Name("贝塞尔曲线")]
            Bezier,

            /// <summary>
            /// 直线
            /// </summary>
            [Name("直线")]
            Straight,

            /// <summary>
            /// 折线
            /// </summary>
            [Name("折线")]
            Stepped,
        }

        /// <summary>
        /// 绘制线
        /// </summary>
        /// <param name="lineType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="fromDir"></param>
        /// <param name="toDir"></param>
        public static void DrawLine(ELineType lineType, Vector2 from, Vector2 to, Color color, float width = 3, EDirection fromDir = EDirection.Right, EDirection toDir = EDirection.Left)
        {
            switch (lineType)
            {
                case ELineType.Bezier: DrawBezierLine(from, to, color, width, fromDir, toDir); break;
                case ELineType.Straight: DrawStraightLine(from, to, color, width); break;
                case ELineType.Stepped: DrawSteppedLine(from, to, color, width, fromDir, toDir); break;
            }
        }

        /// <summary>
        /// 绘制直线:采用贝塞尔替代直线绘制是为了让线有宽度
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="width"></param>
        public static void DrawStraightLine(this Vector2 from, Vector2 to, float width = 3) => DrawStraightLine(from, to, Color.white, false, width);

        /// <summary>
        /// 绘制直线:采用贝塞尔替代直线绘制是为了让线有宽度
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="drawArrow"></param>
        /// <param name="width"></param>
        public static void DrawStraightLine(this Vector2 from, Vector2 to, bool drawArrow, float width = 3) => DrawStraightLine(from, to, Color.white, drawArrow, width);

        /// <summary>
        /// 绘制直线:采用贝塞尔替代直线绘制是为了让线有宽度
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        public static void DrawStraightLine(this Vector2 from, Vector2 to, Color color, float width = 3) => DrawStraightLine(from, to, color, false, width);

        /// <summary>
        /// 绘制直线:采用贝塞尔替代直线绘制是为了让线有宽度
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        /// <param name="drawArrow"></param>
        /// <param name="width"></param>
        public static void DrawStraightLine(this Vector2 from, Vector2 to, Color color, bool drawArrow, float width = 3)
        {
            Vector2 toFromDir = (from - to).normalized;
            Handles.DrawBezier(from, to, from - toFromDir, to + toFromDir, color, null, width);
            if (drawArrow)
            {
                var n = new Vector2(-toFromDir.y, toFromDir.x);
                var left = toFromDir + n;
                var right = toFromDir - n;
                DrawStraightLine(left * 10 + to, to, color, false, width);
                DrawStraightLine(right * 10 + to, to, color, false, width);
            }
        }

        /// <summary>
        /// 绘制折线
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="fromDir"></param>
        /// <param name="toDir"></param>
        public static void DrawSteppedLine(this Vector2 from, Vector2 to, Color color, float width = 3, EDirection fromDir = EDirection.Right, EDirection toDir = EDirection.Right)
        {
            var offset = Vector2.zero;
            var point1 = Vector2.zero;
            var point2 = Vector2.zero;
            if (fromDir == EDirection.Right || fromDir == EDirection.Left)
            {
                if (toDir == EDirection.Right || toDir == EDirection.Left)
                {
                    offset.x = (to.x - from.x) / 2f;
                    point1 = from + offset;
                    point2 = to - offset;
                }
                else
                {
                    point1 = point2 = new Vector2(to.x, from.y);
                }
            }
            else
            {
                if (toDir == EDirection.Up || toDir == EDirection.Down)
                {
                    offset.y = (to.y - from.y) / 2f;
                    point1 = from + offset;
                    point2 = to - offset;
                }
                else
                {
                    point1 = point2 = new Vector2(from.x, to.y);
                }
            }

            Color orgColor = Handles.color;
            Handles.color = color;
            Handles.DrawPolyLine(from, point1, point2, to);
            Handles.color = orgColor;
        }

        /// <summary>
        /// 绘制贝塞尔曲线
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="dir"></param>
        public static void DrawBezierLine(this Vector2 from, Vector2 to, Color color, float width = 3, EDirection dir = EDirection.Right)
        {
            float dirLength = 20f;
            Vector2 offset = DirectionToVector(dir);
            Handles.DrawBezier(from, to, from + offset * dirLength, to - offset * dirLength, color, null, width);
        }

        /// <summary>
        /// 绘制贝塞尔曲线
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="fromDir"></param>
        /// <param name="toDir"></param>
        public static void DrawBezierLine(this Vector2 from, Vector2 to, Color color, float width = 3, EDirection fromDir = EDirection.Right, EDirection toDir = EDirection.Right)
        {
            float dirLength = 20f;
            Vector2 fromTangent = from + DirectionToVector(fromDir) * dirLength;
            Vector2 toTangent = to + DirectionToVector(toDir) * dirLength;
            Handles.DrawBezier(from, to, fromTangent, toTangent, color, null, width);
        }

        /// <summary>
        /// 绘制贝塞尔曲线
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        public static void DrawBezierLine(this Vector2 from, Vector2 to, Color color) => DrawBezierLine(from, to, color, 3, CalculateDirection(to - from));

        #endregion

        #region 绘制GUI颜色

        /// <summary>
        /// 当绘制颜色GUI:使用指定颜色绘制GUI
        /// </summary>
        /// <param name="color"></param>
        /// <param name="drawAction"></param>
        public static void OnGUIColor(Color color, Action drawAction)
        {
            if (drawAction == null) return;

            var orgColor = GUI.color;
            GUI.color = color;
            drawAction.Invoke();
            GUI.color = orgColor;
        }

        /// <summary>
        /// 当绘制背景颜色GUI:使用指定背景色绘制GUI
        /// </summary>
        /// <param name="backgroundColor"></param>
        /// <param name="drawAction"></param>
        public static void OnGUIBackgroundColor(Color backgroundColor, Action drawAction)
        {
            if (drawAction == null) return;

            var orgColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
            drawAction.Invoke();
            GUI.backgroundColor = orgColor;
        }

        #endregion

        #region 方位计算

        /// <summary>
        /// 获取相反方向
        /// </summary>
        /// <param name="nodeSide"></param>
        /// <returns></returns>
        public static EDirection Reverse(this EDirection nodeSide)
        {
            switch (nodeSide)
            {
                case EDirection.Up: return EDirection.Down;
                case EDirection.Down: return EDirection.Up;
                case EDirection.Left: return EDirection.Right;
                case EDirection.Right: return EDirection.Left;
                default: return nodeSide;
            }
        }

        /// <summary>
        /// 计算方向
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static EDirection CalculateDirection(Rect from, Rect to, Vector2 box) => CalculateDirection(from, to, box, ToolBarLayoutOption.weakInstance.layoutRule);

        /// <summary>
        /// 计算方向
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="box"></param>
        /// <param name="layoutRule"></param>
        /// <returns></returns>
        public static EDirection CalculateDirection(Rect from, Rect to, Vector2 box, ELayoutRule layoutRule)
        {
            if (!from.Overlaps(to, true))
            {
                switch (layoutRule)
                {
                    case ELayoutRule.HorizontalFirst:
                        {
                            if (to.xMin >= from.xMax + box.x) return EDirection.Right;
                            if (to.xMax <= from.xMin - box.x) return EDirection.Left;
                            break;
                        }
                    case ELayoutRule.VerticalFirst:
                        {
                            if (to.yMin >= from.yMax + box.y) return EDirection.Down;
                            if (to.yMax <= from.yMin - box.y) return EDirection.Up;
                            break;
                        }
                }
            }
            return CalculateDirection(from, to);
        }

        /// <summary>
        /// 计算方向
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static EDirection CalculateDirection(Rect from, Rect to) => CalculateDirection(to.center - from.center);

        /// <summary>
        /// 计算方向
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static EDirection CalculateDirection(Vector2 from, Vector2 to) => CalculateDirection(to - from);

        /// <summary>
        /// 计算方向
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static EDirection CalculateDirection(Vector2 dir)
        {
            if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
            {
                return (dir.y > 0) ? EDirection.Down : EDirection.Up;
            }
            else
            {
                return (dir.x > 0) ? EDirection.Right : EDirection.Left;
            }
        }

        /// <summary>
        /// 方向转向量
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 DirectionToVector(this EDirection direction)
        {
            switch (direction)
            {
                case EDirection.Up: return -Vector2.up;// 屏幕坐标向上是负增长
                case EDirection.Down: return -Vector2.down;// 屏幕坐标向下是正增长
                case EDirection.Left: return Vector2.left;
                case EDirection.Right: return Vector2.right;
                default: return Vector2.zero;
            }
        }

        /// <summary>
        /// 获取箭头贴图
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Texture GetArrowTexture(this EDirection direction)
        {
            switch (direction)
            {
                case EDirection.Up: return customStyle.upArrow;
                case EDirection.Right: return customStyle.rightArrow;
                case EDirection.Down: return customStyle.downArrow;
                case EDirection.Left: return customStyle.leftArrow;
                default: return default;
            }
        }

        #endregion

        #region 矩形边上的点计算

        /// <summary>
        /// 布局顺时针边：在矩形的四个边上计算点, 偏移起点分别为顺时针的4个角点
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="direction">方位</param>
        /// <param name="offsetValue">偏移系数：实际水平或垂直的偏移量=矩形宽或高*系数</param>
        /// <returns></returns>
        public static Vector2 LayoutClockwiseSide(this Rect rect, EDirection direction, float offsetValue = 0)
        {
            var offset = rect.size * offsetValue;
            switch (direction)
            {
                case EDirection.Up: return new Vector2(rect.x + offset.x, rect.y);
                case EDirection.Right: return new Vector2(rect.xMax, rect.y + offset.y);
                case EDirection.Down: return new Vector2(rect.xMax - offset.x, rect.yMax);
                case EDirection.Left: return new Vector2(rect.x, rect.yMax - offset.y);
                default: return rect.center;
            }
        }

        /// <summary>
        /// 布局边基于中心：四个方向边
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 LayoutSideOnCenter(this Rect rect, EDirection direction)
        {
            switch (direction)
            {
                case EDirection.Up: return rect.center + new Vector2(0, -rect.height / 2);
                case EDirection.Right: return rect.center + new Vector2(rect.width / 2, 0);
                case EDirection.Down: return rect.center + new Vector2(0, rect.height / 2);
                case EDirection.Left: return rect.center + new Vector2(-rect.width / 2, 0);
                default: return rect.center;
            }
        }

        #endregion

        #region 矩形内部绝对布局

        /// <summary>
        /// 布局内部矩形：在矩形的内方位上摆放一个被布局的矩形，并返回矩形位置
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="layoutedSize">被布局尺寸</param>
        /// <param name="rectInsideDirection">矩形内部方位</param>
        /// <returns>被布局的矩形位置</returns>
        public static Vector2 LayoutInsideRect(this Rect rect, Vector2 layoutedSize, ERectInsideDirection rectInsideDirection)
        {
            switch (rectInsideDirection)
            {
                case ERectInsideDirection.LeftUp: return rect.position;
                case ERectInsideDirection.MiddleUp: return rect.position + new Vector2(rect.width / 2 - layoutedSize.x / 2, 0);
                case ERectInsideDirection.RightUp: return new Vector2(rect.xMax - layoutedSize.x, rect.y);
                case ERectInsideDirection.RightMiddle: return new Vector2(rect.xMax - layoutedSize.x, rect.y + rect.height / 2 - layoutedSize.y / 2);
                case ERectInsideDirection.RightDown: return new Vector2(rect.xMax - layoutedSize.x, rect.yMax - layoutedSize.y);
                case ERectInsideDirection.MiddleDown: return new Vector2(rect.x + rect.width / 2 - layoutedSize.x / 2, rect.yMax - layoutedSize.y);
                case ERectInsideDirection.LeftDown: return new Vector2(rect.x, rect.yMax - layoutedSize.y);
                case ERectInsideDirection.LeftMiddle: return new Vector2(rect.x, rect.y + rect.height / 2 - layoutedSize.y / 2);
                case ERectInsideDirection.Center: return rect.center - layoutedSize / 2;
                default: return rect.position;
            }
        }

        #endregion

        #region 矩形外部绝对布局

        /// <summary>
        /// 矩形入点偏移量
        /// </summary>
        public const float RectInPointOffsetValue = 3 / 4f;

        /// <summary>
        /// 矩形出点偏移量
        /// </summary>
        public const float RectOutPointOffsetValue = 1 / 4f;

        /// <summary>
        /// 布局外矩形：在矩形的四个边上摆放一个被布局的矩形，并返回矩形位置
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="layoutedSize">被布局尺寸</param>
        /// <param name="rectOutsideDirection">矩形外部方位</param>
        /// <returns>被布局的矩形位置</returns>
        public static Vector2 LayoutOutsideRect(this Rect rect, Vector2 layoutedSize, ERectOutsideDirection rectOutsideDirection)
        {
            switch (rectOutsideDirection)
            {
                case ERectOutsideDirection.LeftUpCorner: return rect.position - layoutedSize;
                case ERectOutsideDirection.UpLeft: return new Vector2(rect.x, rect.y - layoutedSize.y);
                case ERectOutsideDirection.UpMiddle: return new Vector2(rect.x + rect.width / 2 - layoutedSize.x / 2, rect.y - layoutedSize.y);
                case ERectOutsideDirection.UpRight: return new Vector2(rect.xMax - layoutedSize.x, rect.y - layoutedSize.y);
                case ERectOutsideDirection.RightUpCorner: return new Vector2(rect.xMax, rect.y - layoutedSize.y);
                case ERectOutsideDirection.RightUp: return new Vector2(rect.xMax, rect.y);
                case ERectOutsideDirection.RightMiddle: return new Vector2(rect.xMax, rect.y + rect.height / 2 - layoutedSize.y / 2);
                case ERectOutsideDirection.RightDown: return new Vector2(rect.xMax, rect.yMax - layoutedSize.y);
                case ERectOutsideDirection.RightDownCorner: return new Vector2(rect.xMax, rect.yMax);
                case ERectOutsideDirection.DownRight: return new Vector2(rect.xMax - layoutedSize.x, rect.yMax);
                case ERectOutsideDirection.DownMiddle: return new Vector2(rect.x + rect.width / 2 - layoutedSize.x / 2, rect.yMax);
                case ERectOutsideDirection.DownLeft: return new Vector2(rect.x, rect.yMax);
                case ERectOutsideDirection.LeftDownCorner: return new Vector2(rect.x - layoutedSize.x, rect.yMax);
                case ERectOutsideDirection.LeftDown: return new Vector2(rect.x - layoutedSize.x, rect.yMax - layoutedSize.y);
                case ERectOutsideDirection.LeftMiddle: return new Vector2(rect.x - layoutedSize.x, rect.y + rect.height / 2 - layoutedSize.y / 2);
                case ERectOutsideDirection.LeftUp: return new Vector2(rect.x - layoutedSize.x, rect.y);
                default: return rect.center;
            }
        }

        /// <summary>
        /// 布局外矩形：在矩形的四个边上摆放一个被布局的矩形，并返回矩形位置
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="layoutedSize">被布局尺寸</param>
        /// <param name="direction">方位</param>
        /// <param name="offsetValue">偏移系数：实际水平或垂直的偏移量=矩形宽或高*系数</param>
        /// <returns>被布局的矩形位置</returns>
        public static Vector2 LayoutOutsideRect(this Rect rect, Vector2 layoutedSize, EDirection direction, float offsetValue)
        {
            var offset = rect.size * offsetValue;
            var halfSize = layoutedSize / 2;
            switch (direction)
            {
                case EDirection.Up: return new Vector2(rect.x + offset.x - halfSize.x, rect.y - layoutedSize.y);
                case EDirection.Right: return new Vector2(rect.xMax, rect.y + offset.y - halfSize.y);
                case EDirection.Down: return new Vector2(rect.xMax - offset.x - halfSize.x, rect.yMax);
                case EDirection.Left: return new Vector2(rect.x - layoutedSize.x, rect.yMax - offset.y - halfSize.y);
                default: return rect.position;
            }
        }

        /// <summary>
        /// 布局外部矩形8中4入位置
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="layoutedSize"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 LayoutOutsideRectIn4Of8(this Rect rect, Vector2 layoutedSize, EDirection direction) => LayoutOutsideRect(rect, layoutedSize, direction, RectInPointOffsetValue);

        /// <summary>
        /// 布局外部矩形8中4出位置
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="layoutedSize"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 LayoutOutsideRectOut4Of8(this Rect rect, Vector2 layoutedSize, EDirection direction) => LayoutOutsideRect(rect, layoutedSize, direction, RectOutPointOffsetValue);

        /// <summary>
        /// 布局外部矩形：在矩形的四个边上摆放四个被布局的矩形
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="layoutedSize">被布局尺寸</param>
        /// <param name="offsetValue">偏移系数：实际水平或垂直的偏移量=矩形宽或高*系数</param>
        /// <returns>被布局的矩形位置集合</returns>
        public static IEnumerable<Vector2> LayoutOutsideRect(this Rect rect, Vector2 layoutedSize, float offsetValue)
        {
            var offset = rect.size * offsetValue;
            var halfSize = layoutedSize / 2;
            yield return new Vector2(rect.x + offset.x - halfSize.x, rect.y - layoutedSize.y);
            yield return new Vector2(rect.xMax, rect.y + offset.y - halfSize.y);
            yield return new Vector2(rect.xMax - offset.x - halfSize.x, rect.yMax);
            yield return new Vector2(rect.x - layoutedSize.x, rect.yMax - offset.y - halfSize.y);
        }

        /// <summary>
        /// 布局外部矩形8的4个入
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="layoutedSize"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2> LayoutOutsideRectIn4Of8(this Rect rect, Vector2 layoutedSize) => LayoutOutsideRect(rect, layoutedSize, RectInPointOffsetValue);

        /// <summary>
        /// 布局外部矩形8的4个出
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="layoutedSize"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2> LayoutOutsideRectOut4Of8(this Rect rect, Vector2 layoutedSize) => LayoutOutsideRect(rect, layoutedSize, RectOutPointOffsetValue);

        /// <summary>
        /// 布局外部矩形4个中心
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="layoutedSize"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2> LayoutOutsideRect4Center(this Rect rect, Vector2 layoutedSize) => LayoutOutsideRect(rect, layoutedSize, 1 / 2f);

        #endregion

        #region 矩形外部相对布局

        /// <summary>
        /// 入本地（相对）位置缓存:参数1=主矩形尺寸，参数2=插槽尺寸，参数3=方位，参数4=插槽在主矩形的偏移量
        /// </summary>
        private static Dictionary<Vector2, Dictionary<Vector2, Dictionary<EDirection, Vector2>>> inLocalPositionCache = new Dictionary<Vector2, Dictionary<Vector2, Dictionary<EDirection, Vector2>>>();

        /// <summary>
        /// 出本地（相对）位置缓存:参数1=主矩形尺寸，参数2=插槽尺寸，参数3=方位，参数4=插槽在主矩形的偏移量
        /// </summary>
        private static Dictionary<Vector2, Dictionary<Vector2, Dictionary<EDirection, Vector2>>> outLocalPositionCache = new Dictionary<Vector2, Dictionary<Vector2, Dictionary<EDirection, Vector2>>>();

        /// <summary>
        /// 布局本地外部矩形入位置：8方位（4入4出）
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="layoutedSize"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 LayoutLocalOutsideRectIn4Of8(this Rect rect, Vector2 layoutedSize, EDirection direction) => LayoutLocalOutsideRectIn4Of8(rect, layoutedSize)[direction];

        /// <summary>
        /// 布局本地外部矩形出位置：8方位的4个出
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="layoutedSize"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 LayoutLocalOutsideRectOut4Of8(this Rect rect, Vector2 layoutedSize, EDirection direction) => LayoutLocalOutsideRectOut4Of8(rect, layoutedSize)[direction];

        /// <summary>
        /// 布局本地外部矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="layoutedSize"></param>
        /// <param name="offsetValue"></param>
        /// <returns></returns>
        public static Dictionary<EDirection, Vector2> LayoutLocalOutsideRect(this Rect rect, Vector2 layoutedSize, float offsetValue)
        {
            var directionDic = new Dictionary<EDirection, Vector2>();
            var directions = EnumHelper.Enums<EDirection>();
            int i = 0;
            foreach (var position in LayoutOutsideRect(rect, layoutedSize, offsetValue))
            {
                directionDic[directions[i]] = position - rect.position;
                ++i;
            }
            return directionDic;
        }

        /// <summary>
        /// 布局本地外部矩形入位置字典：8方位的4个入
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="layoutedSize"></param>
        /// <returns></returns>
        public static Dictionary<EDirection, Vector2> LayoutLocalOutsideRectIn4Of8(this Rect rect, Vector2 layoutedSize)
        {
            if (inLocalPositionCache.TryGetValue(rect.size, out var dic))
            {
                if (dic.TryGetValue(layoutedSize, out var result)) return result;
            }
            else
            {
                inLocalPositionCache[rect.size] = dic = new Dictionary<Vector2, Dictionary<EDirection, Vector2>>();
            }
            var directionDic = LayoutLocalOutsideRect(rect, layoutedSize, RectInPointOffsetValue);
            dic[layoutedSize] = directionDic;
            return directionDic;
        }

        /// <summary>
        /// 布局本地外部矩形出位置字典：8方位的4个出
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="layoutedSize"></param>
        /// <returns></returns>
        public static Dictionary<EDirection, Vector2> LayoutLocalOutsideRectOut4Of8(this Rect rect, Vector2 layoutedSize)
        {
            if (outLocalPositionCache.TryGetValue(rect.size, out var dic))
            {
                if (dic.TryGetValue(layoutedSize, out var result)) return result;
            }
            else
            {
                outLocalPositionCache[rect.size] = dic = new Dictionary<Vector2, Dictionary<EDirection, Vector2>>();
            }
            var directionDic = LayoutLocalOutsideRect(rect, layoutedSize, RectOutPointOffsetValue);
            dic[layoutedSize] = directionDic;
            return directionDic;
        }

        /// <summary>
        /// 布局本地外部矩形位置4个中心字典
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="layoutedSize"></param>
        /// <returns></returns>
        public static Dictionary<EDirection, Vector2> LayoutLocalOutsideRect4Center(this Rect rect, Vector2 layoutedSize) => LayoutLocalOutsideRect(rect, layoutedSize, 1 / 2f);

        #endregion

        #region 布局

        /// <summary>
        /// 网格布局
        /// </summary>
        /// <param name="beginPosition"></param>
        /// <param name="cellSize"></param>
        /// <param name="cols"></param>
        /// <param name="nodeViews"></param>
        public static void GridLayout(Vector2 beginPosition, Vector2 cellSize, int cols, params INodeView[] nodeViews) => GridLayout(nodeViews, beginPosition, cellSize, cols);

        /// <summary>
        /// 网格布局
        /// </summary>
        /// <param name="nodeViews"></param>
        /// <param name="beginPosition"></param>
        /// <param name="cellSize"></param>
        /// <param name="cols"></param>
        public static void GridLayout(this IEnumerable<INodeView> nodeViews, Vector2 beginPosition, Vector2 cellSize, int cols)
        {
            if (nodeViews == null) return;
            var i = 0;
            cols = Mathf.Max(1, cols);
            foreach (var nodeView in nodeViews)
            {
                if (nodeView != null)
                {
                    var nodeRect = nodeView.nodeRect;
                    nodeView.nodeRect = new Rect(beginPosition.x + cellSize.x * (i % cols), beginPosition.y + cellSize.y * (i / cols), nodeRect.width, nodeRect.height);
                    i++;
                }
            }
        }

        /// <summary>
        /// 中心扩展布局
        /// </summary>
        public static void CenterExtensionLayout<TNodeView>(this IEnumerable<TNodeView> nodeViews, Vector2 centerPosition, Vector2 offset) where TNodeView : INodeView
        {
            CenterExtensionLayout(nodeViews?.ToList(), centerPosition, offset);
        }

        /// <summary>
        /// 中心扩展布局
        /// </summary>
        public static void CenterExtensionLayout<TNodeView>(this List<TNodeView> nodeViews, Vector2 centerPosition, Vector2 offset) where TNodeView : INodeView
        {
            if (nodeViews == null) return;
            var halfOffset = offset / 2;
            var count = nodeViews.Count;
            var center = count - 1;
            for (int i = 0; i < count; i++)
            {
                var nodeView = nodeViews[i];
                if (nodeView != null)
                {
                    var nodeRect = nodeView.nodeRect;
                    nodeRect.center = centerPosition + halfOffset * (2 * i - center);
                    nodeView.nodeRect = nodeRect;
                }
            }
        }

        /// <summary>
        /// 布局节点视图集合
        /// </summary>
        /// <param name="nodeView"></param>
        /// <param name="direction"></param>
        /// <param name="layoutNodeViews"></param>
        public static void LayoutNodeViews<TNodeView>(this TNodeView nodeView, EDirection direction, List<TNodeView> layoutNodeViews) where TNodeView : INodeView
        {
            if (nodeView == null || layoutNodeViews == null) return;
            var centerPosition = nodeView.nodeRect.center + Vector2.Scale(direction.DirectionToVector(), customStyle.normalNodeSize * 2);
            switch (direction)
            {
                case EDirection.Up:
                case EDirection.Down:
                    {
                        direction = EDirection.Right;
                        break;
                    }
                default:
                    {
                        direction = EDirection.Down;
                        break;
                    }
            }
            var offset = Vector2.Scale(direction.DirectionToVector(), customStyle.normalNodeSize * 2);
            layoutNodeViews.CenterExtensionLayout(centerPosition, offset);
        }

        /// <summary>
        /// 布局节点视图集合
        /// </summary>
        /// <param name="nodeView"></param>
        /// <param name="direction"></param>
        /// <param name="layoutNodeViews"></param>
        public static void LayoutNodeViews<TNodeView>(this TNodeView nodeView, EDirection direction, IEnumerable<TNodeView> layoutNodeViews) where TNodeView : INodeView
        {
            LayoutNodeViews(nodeView, direction, layoutNodeViews?.ToList());
        }

        #endregion

        /// <summary>
        /// 获取Unity对象
        /// </summary>
        /// <param name="entityNodeView"></param>
        /// <returns></returns>
        public static UnityEngine.Object GetUnityObject(this BaseEntityNodeView entityNodeView) => GetNearestEntityModelInParent<UnityEngine.Object>(entityNodeView);

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <param name="entityNodeView"></param>
        /// <returns></returns>
        public static Component GetComponent(this BaseEntityNodeView entityNodeView) => GetNearestEntityModelInParent<Component>(entityNodeView);

        /// <summary>
        /// 获取游戏对象
        /// </summary>
        /// <param name="entityNodeView"></param>
        /// <returns></returns>
        public static GameObject GetGameObject(this BaseEntityNodeView entityNodeView) => GetNearestEntityModelInParent<GameObject>(entityNodeView);

        /// <summary>
        /// 在父级中获取最近的实体模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetNearestEntityModelInParent<T>(this BaseEntityNodeView entityNodeView) where T : class
        {
            var result = default(T);
            entityNodeView.ForeachParentNodeView(nodeView =>
            {
                if (nodeView.nodeModel.entityModel is T nv)
                {
                    result = nv;
                    return false;
                }
                else
                {
                    return true;
                }
            });
            return result;
        }

        /// <summary>
        /// 遍历父级节点视图
        /// </summary>
        /// <param name="entityNodeView"></param>
        /// <param name="func"></param>
        public static void ForeachParentNodeView(this BaseEntityNodeView entityNodeView, Func<BaseEntityNodeView, bool> func)
        {
            if (entityNodeView == null || func == null) return;
            if (!func(entityNodeView)) return;
            entityNodeView.parent?.ForeachParentNodeView(func);
        }

        /// <summary>
        /// 获取父级画布模型列表
        /// </summary>
        /// <param name="canvasModel"></param>
        /// <param name="includeThis"></param>
        /// <returns></returns>
        public static List<ICanvasModel> GetParentCanvasModels(this ICanvasModel canvasModel, bool includeThis = true)
        {
            List<ICanvasModel> parents = new List<ICanvasModel>();
            if (canvasModel.ObjectIsNull()) return parents;
            if (includeThis) parents.Add(canvasModel);

            var parent = canvasModel.parentCanvasModel;
            while (!parent.ObjectIsNull())
            {
                parents.Insert(0, parent);
                parent = parent.parentCanvasModel;
            };
            return parents;
        }

        /// <summary>
        /// 获取父级画布视图列表
        /// </summary>
        /// <param name="canvasModel"></param>
        /// <param name="includeThis"></param>
        /// <returns></returns>
        public static List<BaseCanvasView> GetParentCanvasViews(this BaseCanvasView canvasModel, bool includeThis = true)
        {
            List<BaseCanvasView> parents = new List<BaseCanvasView>();
            if (canvasModel == null) return parents;
            if (includeThis) parents.Add(canvasModel);

            var parent = canvasModel.parent;
            while (parent != null)
            {
                parents.Insert(0, parent);
                parent = parent.parent;
            };
            return parents;
        }

        /// <summary>
        /// 获取或创建前景画布视图
        /// </summary>
        /// <param name="nodeKitEditor"></param>
        /// <param name="canvasModel"></param>
        /// <returns></returns>
        public static BaseCanvasView GetOrCreateForegroundCanvasView(this NodeKitEditor nodeKitEditor, object canvasModel)
        {
            if (!nodeKitEditor || canvasModel.ObjectIsNull()) return default;
            if(canvasModel is ICanvasModel cm)
            {
                return GetOrCreateForegroundCanvasView(nodeKitEditor, cm);
            }
            else if(canvasModel is GameObject go)
            {
                return GetOrCreateForegroundCanvasView(nodeKitEditor, go);
            }
            return default;
        }

        /// <summary>
        /// 获取或创建前景画布视图
        /// </summary>
        /// <param name="nodeKitEditor"></param>
        /// <param name="canvasModel"></param>
        /// <returns></returns>
        public static BaseCanvasView GetOrCreateForegroundCanvasView(this NodeKitEditor nodeKitEditor, ICanvasModel canvasModel)
        {
            if (!nodeKitEditor || canvasModel.ObjectIsNull()) return default;
            return nodeKitEditor.backgroundCanvasView.GetOrCreateForegroundCanvasView(canvasModel);
        }

        /// <summary>
        /// 获取或创建前景画布视图
        /// </summary>
        /// <param name="nodeKitEditor"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static BaseCanvasView GetOrCreateForegroundCanvasView(this NodeKitEditor nodeKitEditor, GameObject gameObject)
        {
            if (!nodeKitEditor || !gameObject) return default;
            foreach (var cm in nodeKitEditor.canvasCache)
            {
                if (cm.Value is GameObjectCanvasView gameObjectCanvasView && gameObjectCanvasView.canvasModel.gameObject == gameObject) return gameObjectCanvasView;
            }
            return nodeKitEditor.backgroundCanvasView.GetOrCreateForegroundCanvasView(gameObject.GetCanvasModel());
        }

        /// <summary>
        /// 是前景画布视图
        /// </summary>
        /// <param name="nodeKitEditor"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool IsForegroundCanvasView(this NodeKitEditor nodeKitEditor, GameObject gameObject)
        {
            if (!nodeKitEditor || !gameObject) return false;
            return nodeKitEditor.foregroundCanvasView is GameObjectCanvasView gameObjectCanvasView && gameObjectCanvasView.canvasModel.gameObject == gameObject;
        }

        /// <summary>
        /// 是前景画布视图
        /// </summary>
        /// <param name="nodeKitEditor"></param>
        /// <param name="canvasModel"></param>
        /// <returns></returns>
        public static bool IsForegroundCanvasView(this NodeKitEditor nodeKitEditor, ICanvasModel canvasModel)
        {
            if (!nodeKitEditor || canvasModel.ObjectIsNull()) return false;
            return nodeKitEditor.foregroundCanvasView?.canvasModel == canvasModel;
        }

        /// <summary>
        /// 是前景画布视图
        /// </summary>
        /// <param name="canvasView"></param>
        /// <returns></returns>
        public static bool IsForegroundCanvasView(this BaseCanvasView canvasView)
        {
            if (canvasView == null) return false;
            return canvasView.nodeKitEditor.foregroundCanvasView == canvasView;
        }
    }

    #region 方位枚举

    /// <summary>
    /// 方位：顺时针方向排列
    /// </summary>
    [Name("方位")]
    public enum EDirection
    {
        /// <summary>
        /// 上
        /// </summary>
        [Name("上")]
        Up = 0,

        /// <summary>
        /// 右
        /// </summary>
        [Name("右")]
        Right,

        /// <summary>
        /// 下
        /// </summary>
        [Name("下")]
        Down,

        /// <summary>
        /// 左
        /// </summary>
        [Name("左")]
        Left,
    }

    #endregion

    #region 矩形内部方位

    /// <summary>
    /// 矩形内部方位：：顺时针方向排列
    /// </summary>
    [Name("矩形内部方位")]
    public enum ERectInsideDirection
    {
        /// <summary>
        /// 左上
        /// </summary>
        [Name("左上")]
        LeftUp = 0,

        /// <summary>
        /// 中上
        /// </summary>
        [Name("中上")]
        MiddleUp,

        /// <summary>
        /// 右上
        /// </summary>
        [Name("右上")]
        RightUp,

        /// <summary>
        /// 右中
        /// </summary>
        [Name("右中")]
        RightMiddle,

        /// <summary>
        /// 右下
        /// </summary>
        [Name("右下")]
        RightDown,

        /// <summary>
        /// 中下
        /// </summary>
        [Name("中下")]
        MiddleDown,

        /// <summary>
        /// 左下
        /// </summary>
        [Name("左下")]
        LeftDown,

        /// <summary>
        /// 左中
        /// </summary>
        [Name("左中")]
        LeftMiddle,

        /// <summary>
        /// 中心
        /// </summary>
        [Name("中心")]
        Center,
    }

    #endregion

    #region 矩形外部方位

    /// <summary>
    /// 矩形外部方位：：顺时针方向排列
    /// </summary>
    [Name("矩形外部方位")]
    public enum ERectOutsideDirection
    {
        /// <summary>
        /// 左上角
        /// </summary>
        [Name("左上角")]
        LeftUpCorner,

        /// <summary>
        /// 上左
        /// </summary>
        [Name("上左")]
        UpLeft,

        /// <summary>
        /// 上中
        /// </summary>
        [Name("上中")]
        UpMiddle,

        /// <summary>
        /// 上右
        /// </summary>
        [Name("上右")]
        UpRight,

        /// <summary>
        /// 右上角
        /// </summary>
        [Name("右上角")]
        RightUpCorner,

        /// <summary>
        /// 右上
        /// </summary>
        [Name("右上")]
        RightUp,

        /// <summary>
        /// 右中
        /// </summary>
        [Name("右中")]
        RightMiddle,

        /// <summary>
        /// 右下
        /// </summary>
        [Name("右下")]
        RightDown,

        /// <summary>
        /// 右下角
        /// </summary>
        [Name("右下角")]
        RightDownCorner,

        /// <summary>
        /// 下右
        /// </summary>
        [Name("下右")]
        DownRight,

        /// <summary>
        /// 下中
        /// </summary>
        [Name("下中")]
        DownMiddle,

        /// <summary>
        /// 下左
        /// </summary>
        [Name("下左")]
        DownLeft,

        /// <summary>
        /// 左下角
        /// </summary>
        [Name("左下角")]
        LeftDownCorner,

        /// <summary>
        /// 左下
        /// </summary>
        [Name("左下")]
        LeftDown,

        /// <summary>
        /// 左中
        /// </summary>
        [Name("左中")]
        LeftMiddle,

        /// <summary>
        /// 左上
        /// </summary>
        [Name("左上")]
        LeftUp,
    }

    #endregion
}
