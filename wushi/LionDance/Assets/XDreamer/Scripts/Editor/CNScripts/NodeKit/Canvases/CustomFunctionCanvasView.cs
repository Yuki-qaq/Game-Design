using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.EditorCNScripts.NodeKit.Canvases
{
    /// <summary>
    /// 自定义函数画布视图
    /// </summary>
    [CanvasView(typeof(CustomFunction))]
    public class CustomFunctionCanvasView : CanvasView<CustomFunction>
    {
        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            scriptListDrawer.onVisible += OnVisible;
            scriptListDrawer.onDrawScript += OnDrawScript;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            scriptListDrawer.onVisible -= OnVisible;
            scriptListDrawer.onDrawScript -= OnDrawScript;
        }

        private void DrawFirstMiniMapItem(NodeView first, Vector2 leftTopOffset, MiniMapData miniMapData)
        {
            var firstNodeRect = first.nodeRect;
            var entryRect = firstNodeRect;
            entryRect.position += new Vector2(0, -100);
            entryRect.position = (entryRect.position + leftTopOffset) * miniMapData.scaleValue + miniMapData.miniMapBoundsRect.position;
            entryRect.size *= miniMapData.scaleValue;
            CommonFun.DrawColorGUI(Color.green, () => GUI.Box(entryRect, GUIContent.none, customStyle.miniMapStyle));
        }

        private void DrawLastMiniMapItem(NodeView last, Vector2 leftTopOffset, MiniMapData miniMapData)
        {
            var lastNodeRect = last.nodeRect;
            var exitRect = lastNodeRect;
            exitRect.position += new Vector2(0, 100);
            exitRect.position = (exitRect.position + leftTopOffset) * miniMapData.scaleValue + miniMapData.miniMapBoundsRect.position;
            exitRect.size *= miniMapData.scaleValue;
            CommonFun.DrawColorGUI(Color.red, () => GUI.Box(exitRect, GUIContent.none, customStyle.miniMapStyle));
        }

        /// <summary>
        /// 当绘制小地图项集合
        /// </summary>
        /// <param name="miniMapData"></param>
        protected internal override void OnGUIMiniMapItems(MiniMapData miniMapData)
        {
            base.OnGUIMiniMapItems(miniMapData);

            var count = childrenNodeViews.Count;
            switch (count)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        Vector2 leftTopOffset = -new Vector2(miniMapData.boundsRect.x < 0 ? miniMapData.boundsRect.x : 0, miniMapData.boundsRect.y < 0 ? miniMapData.boundsRect.y : 0);

                        var first = childrenNodeViews[0];
                        DrawFirstMiniMapItem(first, leftTopOffset, miniMapData);
                        DrawLastMiniMapItem(first, leftTopOffset, miniMapData);
                        break;
                    }
                default:
                    {
                        Vector2 leftTopOffset = -new Vector2(miniMapData.boundsRect.x < 0 ? miniMapData.boundsRect.x : 0, miniMapData.boundsRect.y < 0 ? miniMapData.boundsRect.y : 0);

                        DrawFirstMiniMapItem(childrenNodeViews[0], leftTopOffset, miniMapData);
                        DrawLastMiniMapItem(childrenNodeViews[count - 1], leftTopOffset, miniMapData);
                        break;
                    }
            }
        }

        private void DrawFirst(NodeView first)
        {
            var firstNodeRect = first.nodeRect;
            var entryRect = firstNodeRect;
            entryRect.position += new Vector2(0, -100);
            NodeKitHelperExtension.DrawEntryNodeView(entryRect, "进入");
            NodeKitHelperExtension.DrawConnectionLine(entryRect, firstNodeRect, true);
        }

        private void DrawLast(NodeView last)
        {
            var lastNodeRect = last.nodeRect;
            var exitRect = lastNodeRect;
            exitRect.position += new Vector2(0, 100);
            NodeKitHelperExtension.DrawConnectionLine(lastNodeRect, exitRect, true);
            NodeKitHelperExtension.DrawExitNodeView(exitRect, "退出");
        }

        private void DrawGrammerLine(NodeView fromNodeView, NodeView toNodeView, int offset, string middleText, bool isRightOffset)
        {
            var x = customStyle.normalNodeSize.x;
            var xOffset = offset * x;
            var offsetDirection = isRightOffset ? EDirection.Right : EDirection.Left;
            var fromPoint0 = fromNodeView.nodeRect.LayoutClockwiseSide(offsetDirection, NodeKitHelperExtension.RectOutPointOffsetValue);
            var toPoint0 = toNodeView.nodeRect.LayoutClockwiseSide(offsetDirection, NodeKitHelperExtension.RectInPointOffsetValue);
            var midPoint = new Vector2(isRightOffset ? (Mathf.Max(fromPoint0.x, toPoint0.x) + xOffset) : (Mathf.Min(fromPoint0.x, toPoint0.x) - xOffset), (fromPoint0.y + toPoint0.y) / 2);
            var fromPoint1 = new Vector2(midPoint.x, fromPoint0.y);
            var toPoint1 = new Vector2(midPoint.x, toPoint0.y);

            fromPoint0.DrawStraightLine(fromPoint1);
            fromPoint1.DrawStraightLine(toPoint1);
            toPoint1.DrawConnectionLine(toPoint0, offsetDirection.Reverse(), offsetDirection);
            GUI.Label(new Rect(midPoint, new Vector2(x, 20)), middleText);
        }

        /// <summary>
        /// 当绘制缩放区域
        /// </summary>
        [LanguageTuple("Y", "是")]
        [LanguageTuple("N", "否")]
        [LanguageTuple("Region", "区域")]
        protected internal override void OnGUIZoomArea()
        {
            var count = childrenNodeViews.Count;
            switch (count)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        var first = childrenNodeViews[0];
                        DrawFirst(first);
                        DrawLast(first);
                        break;
                    }
                default:
                    {
                        var conditions = new Dictionary<int, Data>();
                        var loops = new Dictionary<int, Data>();
                        var datas = new Stack<Data>();
                        DrawFirst(childrenNodeViews[0]);
                        for (int i = 0; i < count; i++)
                        {
                            var node = childrenNodeViews[i];
                            var notLast = i != count - 1;
                            Vector2 fromPoint = default;
                            Vector2 toPoint = default;
                            if (notLast)
                            {
                                node.DrawConnectionLine(childrenNodeViews[i + 1], true, out fromPoint, out toPoint);
                            }
                            if (node.nodeModel is ScriptString scriptString)
                            {
                                var grammarType = scriptString.scriptString.GetGrammarTypeByScriptString();
                                switch (grammarType)
                                {
                                    case EGrammarType.Begin:
                                        {
                                            if (notLast)
                                            {
                                                datas.Push(new Data(i, node, grammarType));
                                            }
                                            break;
                                        }
                                    case EGrammarType.If:
                                    case EGrammarType.Loop:
                                        {
                                            if (notLast)
                                            {
                                                datas.Push(new Data(i, node, grammarType));
                                                GUI.Label(new Rect((fromPoint + toPoint) / 2, new Vector2(20, 20)), "Y".Tr(GetType()));
                                            }
                                            break;
                                        }
                                    case EGrammarType.ElseIf:
                                    case EGrammarType.Else:
                                        {
                                            if (datas.Count > 0)
                                            {
                                                var top = datas.Peek();
                                                switch (top.grammarType)
                                                {
                                                    case EGrammarType.If:
                                                    case EGrammarType.ElseIf:
                                                        {
                                                            conditions[i] = top;
                                                            var offset = 0;
                                                            var offsetMax = 0;
                                                            foreach (var kv in conditions)
                                                            {
                                                                if (kv.Key <= i && kv.Value.index >= top.index)
                                                                {
                                                                    offset++;
                                                                    if (kv.Value.offset > offsetMax)
                                                                    {
                                                                        offsetMax = kv.Value.offset;
                                                                    }
                                                                }
                                                            }
                                                            if (offset > offsetMax + 1)
                                                            {
                                                                offset = offsetMax + 1;
                                                            }
                                                            top.offset = offset;
                                                            datas.Pop();
                                                            DrawGrammerLine(top.nodeView, node, offset, "N".Tr(GetType()), false);
                                                            break;
                                                        }
                                                }
                                            }
                                            if (notLast)
                                            {
                                                datas.Push(new Data(i, node, grammarType));
                                                GUI.Label(new Rect((fromPoint + toPoint) / 2, new Vector2(20, 20)), "Y".Tr(GetType()));
                                            }
                                            break;
                                        }
                                    case EGrammarType.EndIf:
                                        {
                                            if (datas.Count > 0)
                                            {
                                                var top = datas.Peek();
                                                switch (top.grammarType)
                                                {
                                                    case EGrammarType.If:
                                                    case EGrammarType.ElseIf:
                                                    case EGrammarType.Else:
                                                        {
                                                            conditions[i] = top;
                                                            var offset = 0;
                                                            var offsetMax = 0;
                                                            foreach (var kv in conditions)
                                                            {
                                                                if(kv.Key <= i && kv.Value.index >= top.index)
                                                                {
                                                                    offset++;
                                                                    if(kv.Value.offset> offsetMax)
                                                                    {
                                                                        offsetMax = kv.Value.offset;
                                                                    }
                                                                }
                                                            }
                                                            if (offset > offsetMax + 1)
                                                            {
                                                                offset = offsetMax + 1;
                                                            }
                                                            top.offset = offset;

                                                            datas.Pop();
                                                            DrawGrammerLine(top.nodeView, node, offset, "N".Tr(GetType()), false);
                                                            break;
                                                        }
                                                }
                                            }
                                            break;
                                        }
                                    case EGrammarType.EndLoop:
                                        {
                                            if (datas.Count > 0)
                                            {
                                                var top = datas.Peek();
                                                switch (top.grammarType)
                                                {
                                                    case EGrammarType.Loop:
                                                        {
                                                            loops[i] = top;
                                                            var offset = 0;
                                                            var offsetMax = 0;
                                                            foreach (var kv in loops)
                                                            {
                                                                if (kv.Key <= i && kv.Value.index >= top.index)
                                                                {
                                                                    offset++;
                                                                    if (kv.Value.offset > offsetMax)
                                                                    {
                                                                        offsetMax = kv.Value.offset;
                                                                    }
                                                                }
                                                            }
                                                            if (offset > offsetMax + 1)
                                                            {
                                                                offset = offsetMax + 1;
                                                            }
                                                            top.offset = offset;

                                                            datas.Pop();
                                                            DrawGrammerLine(top.nodeView, node, offset, "N".Tr(GetType()), true);
                                                            break;
                                                        }
                                                }
                                            }
                                            break;
                                        }
                                    case EGrammarType.End:
                                        {
                                            if (datas.Count > 0)
                                            {
                                                var top = datas.Peek();
                                                switch (top.grammarType)
                                                {
                                                    case EGrammarType.Begin:
                                                        {
                                                            loops[i] = top;
                                                            var offset = 0;
                                                            var offsetMax = 0;
                                                            foreach (var kv in loops)
                                                            {
                                                                if (kv.Key <= i && kv.Value.index >= top.index)
                                                                {
                                                                    offset++;
                                                                    if (kv.Value.offset > offsetMax)
                                                                    {
                                                                        offsetMax = kv.Value.offset;
                                                                    }
                                                                }
                                                            }
                                                            if (offset > offsetMax + 1)
                                                            {
                                                                offset = offsetMax + 1;
                                                            }
                                                            top.offset = offset;

                                                            datas.Pop();
                                                            DrawGrammerLine(top.nodeView, node, offset, "Region".Tr(GetType()), true);
                                                            break;
                                                        }
                                                }
                                            }
                                            break;
                                        }
                                }
                            }
                        }
                        DrawLast(childrenNodeViews[count - 1]);
                        break;
                    }
            }

            base.OnGUIZoomArea();
        }

        class Data
        {
            public int index;
            public NodeView nodeView;
            public EGrammarType grammarType;
            public int offset = 1;

            public Data(int index, NodeView nodeView, EGrammarType grammarType)
            {
                this.index = index;
                this.nodeView = nodeView;
                this.grammarType = grammarType;
            }
        }

        /// <summary>
        /// 当绘制检查器
        /// </summary>
        protected internal override void OnGUIInspector()
        {
            if (nodeSelections.Any())
            {
                base.OnGUIInspector();
            }
            else
            {
                EditorGUILayout.TextField("Name".Tr(), nodeModel.displayName);
                DrawSriptList();
            }
        }

        #region 脚本列表

        /// <summary>
        /// 选择脚本
        /// </summary>
        Script selectScript { get; set; }

        ScriptListDrawer _scriptListDrawer;

        ScriptListDrawer scriptListDrawer => _scriptListDrawer ?? (_scriptListDrawer = new ScriptListDrawer(() => selectScript, value => selectScript = value));

        /// <summary>
        /// 左侧 滚动条区域的位置信息
        /// </summary>
        Vector2 leftAreaScrollPos = new Vector2(0, 0);

        void OnVisible(float y) => leftAreaScrollPos.y = y;

        void OnDrawScript(ScriptCategory scriptCategory)
        {
            if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonLeft, UICommonOption.Width20, UICommonOption.Height20))
            {
                selectScript = scriptCategory.script;

                var ss = new ScriptStringDrawer();
                ss.script = scriptCategory.script;

                canvasModel.XAddScriptString(ss.ToScriptString());
            }
        }

        private void DrawSriptList()
        {
            scriptListDrawer.HandleEvent();
            scriptListDrawer.DrawSearch();

            leftAreaScrollPos = EditorGUILayout.BeginScrollView(leftAreaScrollPos, false, false, GUILayout.ExpandHeight(true));
            scriptListDrawer.DrawScriptList();
            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region 画布工具栏

        /// <summary>
        /// 自动布局
        /// </summary>
        [CanvasToolBar(nameof(AutoLayout))]
        public override void AutoLayout()
        {
            //base.AutoLayout();
            var normalNodeSize = customStyle.normalNodeSize;
            var cellSize = normalNodeSize * 2;
            if (parentNodeView != null)
            {
                parentNodeView.nodeRect = new Rect(normalNodeSize, parentNodeView.nodeRect.size);
            }
            childrenNodeViews.GridLayout(normalNodeSize + cellSize, cellSize, 1);
        }

        #endregion
    }
}
