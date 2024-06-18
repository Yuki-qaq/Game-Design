using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base.EditorWindows;
using XCSJ.EditorTools.Windows.Layouts;
using XCSJ.Extension.Base.Helpers;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// 布局-UGUI
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Layout)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class LayoutUGUIWindow : XEditorWindowWithScrollView<LayoutUGUIWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "布局-UGUI";

        /// <summary>
        /// 初始化
        /// </summary>
        [MenuItem(XDreamerMenu.EditorWindow + Title)]
        public static void Init() => OpenAndFocus();

        private static XGUIContent GetXGUIContent(string propertyName) => new XGUIContent(typeof(LayoutUGUIWindow), propertyName, true);

        /// <summary>
        /// 编号宽度
        /// </summary>
        public const int noWidth = 25;

        /// <summary>
        /// 工具按钮尺寸选项
        /// </summary>
        public GUILayoutOption[] toolButtonSizeOption => ToolEditorWindowOption.weakInstance.toolButtonSizeOption;

        /// <summary>
        /// 内容按钮尺寸选项
        /// </summary>
        public GUILayoutOption[] contentButtonSizeOption => ToolEditorWindowOption.weakInstance.contentButtonSizeOption;

        #region 基础操作

        /// <summary>
        /// 撤销
        /// </summary>
        public RectTransformUndo undo = new RectTransformUndo();

        /// <summary>
        /// 是锁定
        /// </summary>
        public bool isLocked = false;

        /// <summary>
        /// 显示基础选项
        /// </summary>
        public void ShowBaseOperation()
        {
            EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
            EditorGUI.BeginDisabledGroup(!undo.CanUndo());
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Undo), toolButtonSizeOption))
            {
                if (undo.Undo()) UICommonFun.MarkSceneDirty();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!undo.CanDo());
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Redo), toolButtonSizeOption))
            {
                if (undo.Do()) UICommonFun.MarkSceneDirty();
            }
            EditorGUI.EndDisabledGroup();

            isLocked = UICommonFun.ButtonToggle(CommonFun.NameTip(EIcon.Lock), isLocked, GUI.skin.button, toolButtonSizeOption);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(CommonFun.NameTip(EIcon.Config), toolButtonSizeOption))
            {
                XDreamerPreferences.OpenWindow<ToolEditorWindowOption>();
                XDreamerPreferences.OpenWindow<LayoutOption>();
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region 基础布局

        /// <summary>
        /// 基础布局
        /// </summary>
        [Name("基础布局")]
        public bool showBaseLayout = true;

        /// <summary>
        /// 左对齐
        /// </summary>
        [Name("左对齐")]
        [Tip("以 标准矩形变换1 为基准进行 左对齐 操作", "Align left based on standard rectangle transform 1")]
        [Attributes.Icon]
        public XGUIContent LeftAlign { get; } = GetXGUIContent(nameof(LeftAlign));

        /// <summary>
        /// 右对齐
        /// </summary>
        [Name("右对齐")]
        [Tip("以 标准矩形变换1 为基准进行 右对齐 操作", "Align right based on standard rectangle transform 1")]
        [Attributes.Icon]
        public XGUIContent RightAlign { get; } = GetXGUIContent(nameof(RightAlign));

        /// <summary>
        /// 顶对齐
        /// </summary>
        [Name("顶对齐")]
        [Tip("以 标准矩形变换1 为基准进行 顶对齐 操作", "The top alignment operation is based on the standard rectangular transformation 1")]
        [Attributes.Icon]
        public XGUIContent TopAlign { get; } = GetXGUIContent(nameof(TopAlign));

        /// <summary>
        /// 底对齐
        /// </summary>
        [Name("底对齐")]
        [Tip("以 标准矩形变换1 为基准进行 底对齐 操作", "The bottom alignment operation is based on the standard rectangular transformation 1")]
        [Attributes.Icon]
        public XGUIContent BottomAlign { get; } = GetXGUIContent(nameof(BottomAlign));

        /// <summary>
        /// 中心水平对齐
        /// </summary>
        [Name("中心水平对齐")]
        [Tip("中心水平对齐,即中间对齐;以 标准矩形变换1 为基准进行 中心水平对齐 操作", "Center horizontal alignment, i.e. middle alignment; Align the center horizontally based on the standard rectangle transformation 1")]
        [Attributes.Icon]
        public XGUIContent CenterHorizontalAlign { get; } = GetXGUIContent(nameof(CenterHorizontalAlign));

        /// <summary>
        /// 中心垂直对齐
        /// </summary>
        [Name("中心垂直对齐")]
        [Tip("中心垂直对齐,即居中对齐;以 标准矩形变换1 为基准进行 中心垂直对齐 操作")]
        [Attributes.Icon]
        public XGUIContent CenterVerticalAlign { get; } = GetXGUIContent(nameof(CenterVerticalAlign));

        /// <summary>
        /// 中心水平等间隔
        /// </summary>
        [Name("中心水平等间隔")]
        [Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 中心水平等间隔 线性补间操作", "Take the standard rectangular transformation 1 and the standard rectangular transformation 2 as the benchmark to perform the center horizontal equidistant linear interpolation operation")]
        [Attributes.Icon]
        public XGUIContent CenterHorizontalSameSpace { get; } = GetXGUIContent(nameof(CenterHorizontalSameSpace));

        /// <summary>
        /// 中心垂直等间隔
        /// </summary>
        [Name("中心垂直等间隔")]
        [Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 中心垂直等间隔 线性补间操作", "Take the standard rectangular transformation 1 and the standard rectangular transformation 2 as the benchmark to perform the center vertical equidistant linear interpolation operation")]
        [Attributes.Icon]
        public XGUIContent CenterVerticalSameSpace { get; } = GetXGUIContent(nameof(CenterVerticalSameSpace));

        /// <summary>
        /// 边界水平等间隔
        /// </summary>
        [Name("边界水平等间隔")]
        [Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 边界水平等间隔 线性补间操作", "Take the standard rectangular transformation 1 and the standard rectangular transformation 2 as the benchmark to perform the boundary horizontal equidistant linear interpolation operation")]
        [Attributes.Icon]
        public XGUIContent BoundsHorizontalSameSpace { get; } = GetXGUIContent(nameof(BoundsHorizontalSameSpace));

        /// <summary>
        /// 边界垂直等间隔
        /// </summary>
        [Name("边界垂直等间隔")]
        [Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 边界垂直等间隔 线性补间操作", "Take the standard rectangular transformation 1 and the standard rectangular transformation 2 as the benchmark to perform the boundary vertical equidistant linear interpolation operation")]
        [Attributes.Icon]
        public XGUIContent BoundsVerticalSameSpace { get; } = GetXGUIContent(nameof(BoundsVerticalSameSpace));

        /// <summary>
        /// 等宽
        /// </summary>
        [Name("等宽")]
        [Tip("以 标准矩形变换1 为基准进行 等宽 操作", "Equal width operation based on standard rectangular transformation 1")]
        [Attributes.Icon]
        public XGUIContent SameWidth { get; } = GetXGUIContent(nameof(SameWidth));

        /// <summary>
        /// 等高
        /// </summary>
        [Name("等高")]
        [Tip("以 标准矩形变换1 为基准进行 等高 操作", "Perform contour operation based on standard rectangular transformation 1")]
        [Attributes.Icon]
        public XGUIContent SameHeight { get; } = GetXGUIContent(nameof(SameHeight));

        /// <summary>
        /// 等尺寸
        /// </summary>
        [Name("等尺寸")]
        [Tip("等尺寸,即等宽高;以 标准矩形变换1 为基准进行 等尺寸 操作", "Equal size, i.e. equal width and height; Equal size operation based on standard rectangular transformation 1")]
        [Attributes.Icon]
        public XGUIContent SameSize { get; } = GetXGUIContent(nameof(SameSize));

        /// <summary>
        /// 递增宽
        /// </summary>
        [Name("递增宽")]
        [Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 递增宽 线性补间操作", "Equal size, i.e. equal width and height; Equal size operation based on standard rectangular transformation 1")]
        [Attributes.Icon]
        public XGUIContent IncreaseWidth { get; } = GetXGUIContent(nameof(IncreaseWidth));

        /// <summary>
        /// 递增高
        /// </summary>
        [Name("递增高")]
        [Tip("以 标准矩形变换1 与 标准矩形变换2 为基准进行 递增高 线性补间操作", "Take standard rectangular transformation 1 and standard rectangular transformation 2 as the benchmark for increasing linear interpolation operation")]
        [Attributes.Icon]
        public XGUIContent IncreaseHeight { get; } = GetXGUIContent(nameof(IncreaseHeight));

        /// <summary>
        /// 递增尺寸
        /// </summary>
        [Name("递增尺寸")]
        [Tip("递增尺寸,即分别递增宽高;以 标准矩形变换1 与 标准矩形变换2 为基准进行 递增尺寸 线性补间操作", "Increasing size, i.e. increasing width and height respectively; Take the standard rectangular transformation 1 and the standard rectangular transformation 2 as the benchmark to carry out the incremental size linear interpolation operation")]
        [Attributes.Icon]
        public XGUIContent IncreaseSize { get; } = GetXGUIContent(nameof(IncreaseSize));

        /// <summary>
        /// 方向重置
        /// </summary>
        [Name("方向重置")]
        [Tip("将所有矩形变换的方向重置", "Resets the direction of all rectangular transformations")]
        [Attributes.Icon]
        public XGUIContent DirectionReset { get; } = GetXGUIContent(nameof(DirectionReset));

        /// <summary>
        /// 显示基础布局
        /// </summary>
        public void ShowBaseLayout()
        {
            if (showBaseLayout = UICommonFun.Foldout(showBaseLayout, CommonFun.NameTooltip(this, nameof(showBaseLayout))))
            {
                CommonFun.BeginLayout();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(LeftAlign, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.LeftAlign(ts, standardRectTransform1));
                }
                if (GUILayout.Button(CenterVerticalAlign, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.CenterVerticalAlign(ts, standardRectTransform1));
                }
                if (GUILayout.Button(RightAlign, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.RightAlign(ts, standardRectTransform1));
                }

                GUILayout.Space(4);

                if (GUILayout.Button(CenterHorizontalSameSpace, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.CenterHorizontalSameSpace(ts, standardRectTransform1, standardRectTransform2));
                }
                if (GUILayout.Button(CenterVerticalSameSpace, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.CenterVerticalSameSpace(ts, standardRectTransform1, standardRectTransform2));
                }

                GUILayout.Space(4);

                if (GUILayout.Button(SameWidth, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.SameWidth(ts, standardRectTransform1));
                }
                if (GUILayout.Button(SameHeight, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.SameHeight(ts, standardRectTransform1));
                }
                if (GUILayout.Button(SameSize, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.SameSize(ts, standardRectTransform1));
                }
                if (GUILayout.Button(DirectionReset, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.DirectionReset(ts));
                }
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(TopAlign, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.TopAlign(ts, standardRectTransform1));
                }
                if (GUILayout.Button(CenterHorizontalAlign, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.CenterHorizontalAlign(ts, standardRectTransform1));
                }
                if (GUILayout.Button(BottomAlign, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.BottomAlign(ts, standardRectTransform1));
                }

                GUILayout.Space(4);

                if (GUILayout.Button(BoundsHorizontalSameSpace, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.BoundsHorizontalSameSpace(ts, standardRectTransform1, standardRectTransform2));
                }
                if (GUILayout.Button(BoundsVerticalSameSpace, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.BoundsVerticalSameSpace(ts, standardRectTransform1, standardRectTransform2));
                }

                GUILayout.Space(4);

                if (GUILayout.Button(IncreaseWidth, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.IncreaseWidth(ts, standardRectTransform1, standardRectTransform2));
                }
                if (GUILayout.Button(IncreaseHeight, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.IncreaseHeight(ts, standardRectTransform1, standardRectTransform2));
                }
                if (GUILayout.Button(IncreaseSize, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveRectTransforms;
                    undo.Record(ts, () => LayoutHelper.IncreaseSize(ts, standardRectTransform1, standardRectTransform2));
                }

                EditorGUILayout.EndHorizontal();

                CommonFun.EndLayout();
            }
        }

        #endregion

        #region 其它布局

        /// <summary>
        /// 布局
        /// </summary>
        public List<IRectTransformLayoutWindow> layouts = new List<IRectTransformLayoutWindow>();

        /// <summary>
        /// 其它布局
        /// </summary>
        [Name("其它布局")]
        public bool showOtherLayout = false;

        /// <summary>
        /// 显示其它布局
        /// </summary>
        public void ShowOtherLayout()
        {
            if (showOtherLayout = UICommonFun.Foldout(showOtherLayout, CommonFun.NameTooltip(this, nameof(showOtherLayout))))
            {
                CommonFun.BeginLayout();
                var ts = effectiveRectTransforms;
                foreach (var l in layouts)
                {
                    if (l.expanded = UICommonFun.Foldout(l.expanded, CommonFun.NameTooltip(l.GetType())))
                    {
                        try
                        {
                            CommonFun.BeginLayout();
                            undo.Record(ts, () => l.OnGUI(effectiveRectTransforms, standardRectTransform1, standardRectTransform2));
                        }
                        finally
                        {
                            CommonFun.EndLayout();
                        }
                    }//end expanded
                }//end foreach
                CommonFun.EndLayout();
            }
        }

        #endregion

        #region 基础信息

        /// <summary>
        /// 基础信息
        /// </summary>
        [Name("基础信息")]
        public bool showBaseInfo = true;

        /// <summary>
        /// 当前游戏对象
        /// </summary>
        [Name("当前游戏对象")]
        public GameObject currentGO = null;

        /// <summary>
        /// 使用选择集
        /// </summary>
        [Name("使用选择集")]
        public bool useSelection = true;

        /// <summary>
        /// 使用完整选择集
        /// </summary>
        [Name("使用完整选择集")]
        [Tip("勾选,对所有处于选择集中的游戏对象进行布局;不勾选，仅对选择集中激活游戏对象的子级游戏对象进行布局;", "Check to layout all game objects in the selection set; If unchecked, only the child game objects of the selected game objects will be laid out;")]
        public bool useFullSelection = true;

        /// <summary>
        /// 自动清理撤销
        /// </summary>
        [Name("自动清理撤销")]
        [Tip("当前游戏对象发生变化时，会自动将撤销操作信息清空;", "When the current game object changes, the cancellation operation information will be cleared automatically;")]
        public bool autoClearUndo = true;

        /// <summary>
        /// 当前矩形变换(只读)
        /// </summary>
        [Name("当前矩形变换(只读)")]
        [Tip("当前游戏对象所属的矩形变换", "Rectangle transform to which the current GameObject belongs")]
        public RectTransform currentRectTransform = null;

        /// <summary>
        /// 锁定
        /// </summary>
        [Name("锁定")]
        [Tip("锁定标准矩形变换1", "Lock standard rectangle transform 1")]
        public bool lockStandardRectTransform1 = false;
        
        private RectTransform _standardRectTransform1 = null;

        /// <summary>
        /// 起点矩形变换
        /// </summary>
        [Name("起点矩形变换")]
        public RectTransform standardRectTransform1
        {
            get => _standardRectTransform1;
            set
            {
                if(!lockStandardRectTransform1)
                {
                    _standardRectTransform1 = value;

                    var i = infos.FindIndex(info => info.rectTransform == _standardRectTransform1);
                    if (i > 0)
                    {
                        var tmp = infos[i];
                        infos.RemoveAt(i);
                        infos.Insert(0, tmp);
                    }
                }
            }
        }

        /// <summary>
        /// 锁定
        /// </summary>
        [Name("锁定")]
        [Tip("锁定标准矩形变换2", "Lock standard rectangular transform 2")]
        public bool lockStandardRectTransform2 = false;

        private RectTransform _standardRectTransform2 = null;

        /// <summary>
        /// 终点矩形变换
        /// </summary>
        [Name("终点矩形变换")]
        public RectTransform standardRectTransform2
        {
            get => _standardRectTransform2;
            set
            {
                if (!lockStandardRectTransform2)
                {
                    _standardRectTransform2 = value;

                    var i = infos.FindIndex(info => info.rectTransform == _standardRectTransform2);
                    if (i >= 0 && i < infos.Count - 1)
                    {
                        var tmp = infos[i];
                        infos.RemoveAt(i);
                        infos.Add(tmp);
                    }
                }
            }
        }

        /// <summary>
        /// 显示基础信息
        /// </summary>
        public void ShowBaseInfo()
        {
            if (showBaseInfo = UICommonFun.Foldout(showBaseInfo, CommonFun.NameTooltip(this, nameof(showBaseInfo))))
            {
                CommonFun.BeginLayout();

                EditorGUI.BeginDisabledGroup(isLocked);
                var go = EditorToolkitHelper.GameObjectField(CommonFun.NameTooltip(this, nameof(currentGO)), currentGO, CommonFun.NameTooltip(this, nameof(useSelection)), ref useSelection);
                useFullSelection = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(useFullSelection)), useFullSelection);
                EditorGUI.EndDisabledGroup();


                if (!isLocked && currentGO != go)
                {
                    if (autoClearUndo) undo.Clear();

                    currentRectTransform = null;
                    standardRectTransform1 = null;
                    standardRectTransform2 = null;
                    infos.Clear();
                    currentGO = go;

                    if (go)
                    {
                        currentRectTransform = go.GetComponent<RectTransform>();
                        UpdateInfos();
                    }
                }

                autoClearUndo = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(autoClearUndo)), autoClearUndo);

                //EditorGUILayout.ObjectField(CommonFun.NameTooltip(this, nameof(currentRectTransform)), currentRectTransform, typeof(RectTransform), true);

                EditorGUILayout.BeginHorizontal();
                standardRectTransform1 = (RectTransform)EditorGUILayout.ObjectField(CommonFun.NameTooltip(this, nameof(standardRectTransform1)), standardRectTransform1, typeof(RectTransform), true);
                lockStandardRectTransform1 = UICommonFun.ButtonToggle(CommonFun.NameTooltip(this, nameof(lockStandardRectTransform1)), lockStandardRectTransform1, EditorStyles.miniButtonRight, GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                standardRectTransform2 = (RectTransform)EditorGUILayout.ObjectField(CommonFun.NameTooltip(this, nameof(standardRectTransform2)), standardRectTransform2, typeof(RectTransform), true);
                lockStandardRectTransform2 = UICommonFun.ButtonToggle(CommonFun.NameTooltip(this, nameof(lockStandardRectTransform2)), lockStandardRectTransform2, EditorStyles.miniButtonRight, GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();

                CommonFun.EndLayout();
            }
        }

        #endregion

        #region 详细信息

        /// <summary>
        /// 生效类型
        /// </summary>
        [Flags]
        [Name("生效类型")]
        public enum EEffectiveType
        {
            /// <summary>
            /// 文本
            /// </summary>
            Text = 1 << 0,

            /// <summary>
            /// 图像
            /// </summary>
            Image = 1 << 1,

            /// <summary>
            /// 原始图像
            /// </summary>
            RawImage = 1 << 2,

            /// <summary>
            /// 按钮
            /// </summary>
            Button = 1 << 3,

            /// <summary>
            /// 切换
            /// </summary>
            Toggle = 1 << 4,

            /// <summary>
            /// 输入框
            /// </summary>
            InputField = 1 << 5,

            /// <summary>
            /// 滑动条
            /// </summary>
            Slider = 1 << 6,

            /// <summary>
            /// 滚动条
            /// </summary>
            Scrollbar = 1 << 7,

            /// <summary>
            /// 滚动矩形
            /// </summary>
            ScrollRect = 1 << 8,

            /// <summary>
            /// 画布
            /// </summary>
            Canvas = 1 << 9,
        }

        /// <summary>
        /// 信息
        /// </summary>
        public class Info
        {
            /// <summary>
            /// 矩形类型
            /// </summary>
            public RectTransform rectTransform;

            /// <summary>
            /// 忽略
            /// </summary>
            public bool ignore = false;
        }

        /// <summary>
        /// 生效类型
        /// </summary>
        [Name("生效类型")]
        [EnumPopup]
        public EEffectiveType effectiveType = (EEffectiveType)0x7fffffff;

        /// <summary>
        /// 是生效类型
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="effectiveType"></param>
        /// <returns></returns>
        public bool IsEffectiveType(RectTransform rectTransform, EEffectiveType effectiveType)
        {
            try
            {
                foreach (var t in EnumHelper.Enums<EEffectiveType>())
                {
                    if ((t & effectiveType) == t && rectTransform.GetComponent(t.ToString()))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 名称匹配
        /// </summary>
        [Name("名称匹配")]
        [Tip("游戏对象名中存在特定字符串", "There is a specific string in the game object name")]
        public string nameMatch = "";

        /// <summary>
        /// 是名称匹配
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="nameMatch"></param>
        /// <returns></returns>
        public bool IsNameMatch(Transform transform, string nameMatch)
        {
            try
            {
                return string.IsNullOrEmpty(nameMatch) || transform.name.IndexOf(nameMatch, StringComparison.CurrentCultureIgnoreCase) >= 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 详细信息
        /// </summary>
        [Name("详细信息")]
        public bool showDetailInfo = true;

        /// <summary>
        /// 信息
        /// </summary>
        public List<Info> infos = new List<Info>();

        /// <summary>
        /// 矩形变换
        /// </summary>
        public List<RectTransform> rectTransforms => infos.Where(i => !i.ignore).ToList(i => i.rectTransform);

        /// <summary>
        /// 生效的矩形变换
        /// </summary>
        public List<RectTransform> effectiveRectTransforms => infos.Where(i => !i.ignore && IsEffectiveType(i.rectTransform, effectiveType) && IsNameMatch(i.rectTransform, nameMatch)).ToList(i => i.rectTransform);

        private void UpdateInfos()
        {
            if (currentRectTransform)
            {
                if (useFullSelection)
                {
                    infos.Clear();
                    Selection.gameObjects.Foreach(o =>
                    {
#if CSHARP_7_3_OR_NEWER
                        if (o.transform is RectTransform rectTransform)
                        {
                            infos.Add(new Info() { rectTransform = rectTransform });
                        }
#else
                        if(o.transform is RectTransform)
                        {
                            infos.Add(new Info() { rectTransform = o.transform as RectTransform });
                        }
#endif
                    });
                }
                else
                {
                    infos = CommonFun.GetChildGameObjects(currentRectTransform).ToList(o => new Info() { rectTransform = o.transform as RectTransform });
                }

                if (order != ERankOrder.None) SetInfoOder();

                if (infos.Count > 0)
                {
                    standardRectTransform1 = infos[0].rectTransform;
                    standardRectTransform2 = infos[infos.Count - 1].rectTransform;
                }
            }
        }

        /// <summary>
        /// 排序方式
        /// </summary>
        [Name("排序方式")]
        [EnumPopup]
        public ERankOrder order = ERankOrder.None;

        void SetInfoOder()
        {
            switch (order)
            {
                case ERankOrder.Ascending:
                    {
                        infos.Sort((a, b) => a.rectTransform.name.CompareTo(b.rectTransform.name));
                        break;
                    }
                case ERankOrder.Descending:
                    {
                        infos.Sort((a, b) => b.rectTransform.name.CompareTo(a.rectTransform.name));
                        break;
                    }
                default:
                    {
                        UpdateInfos();
                        break;
                    }
            }

            if (infos.Count > 0)
            {
                standardRectTransform1 = infos[0].rectTransform;
                standardRectTransform2 = infos[infos.Count - 1].rectTransform;
            }
        }

        /// <summary>
        /// 显示详细信息
        /// </summary>
        public void ShowDetailInfo()
        {
            if (showDetailInfo = UICommonFun.Foldout(showDetailInfo, CommonFun.NameTooltip(this, nameof(showDetailInfo))))
            {
                CommonFun.BeginLayout();

                effectiveType = (EEffectiveType)EditorGUILayout.EnumFlagsField(CommonFun.NameTooltip(this, nameof(effectiveType)), effectiveType);

                EditorGUILayout.BeginHorizontal();
                nameMatch = EditorGUILayout.TextField(CommonFun.NameTooltip(this, nameof(nameMatch)), nameMatch);
                if (GUILayout.Button("置空", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                {
                    nameMatch = "";
                }
                EditorGUILayout.EndHorizontal();

                var preOder = order;
                order = (ERankOrder)UICommonFun.EnumPopup(CommonFun.NameTooltip(this, nameof(order)), order);

                if (order != preOder)
                {
                    SetInfoOder();
                }

                #region 标题
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("NO.", GUILayout.Width(noWidth));
                if (GUILayout.Button(new GUIContent("矩形变换","点击可按名称排序"), EditorStyles.label))
                {
                    infos.Sort((x, y) => x.rectTransform.name.CompareTo(y.rectTransform.name));
                }
                GUILayout.Label("", GUILayout.Width(58));
                GUILayout.Label("忽略");
                GUILayout.Label("生效类型");
                GUILayout.Label("名称匹配");
                GUILayout.Label("生效");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
#endregion

                for (int i = 0; i < infos.Count; ++i)
                {
                    UICommonFun.BeginHorizontal(i % 2 == 1);

                    var info = infos[i];

                    //"NO."
                    GUILayout.Label((i + 1).ToString(), GUILayout.Width(noWidth));

                    EditorGUILayout.ObjectField(info.rectTransform, typeof(RectTransform), true);

                    EditorGUI.BeginDisabledGroup(i == 0);
                    if (GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(18)))
                    {
                        infos[i] = infos[i - 1];
                        infos[i - 1] = info;
                        if(i == 1) standardRectTransform1 = infos[0].rectTransform;
                        if(i == infos.Count - 1) standardRectTransform2 = infos[infos.Count - 1].rectTransform;
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.BeginDisabledGroup(i == infos.Count - 1);
                    if (GUILayout.Button("↓", EditorStyles.miniButtonRight, GUILayout.Width(18)))
                    {
                        infos[i] = infos[i + 1];
                        infos[i + 1] = info;
                        if (i == 0) standardRectTransform1 = infos[0].rectTransform;
                        if (i == infos.Count - 2) standardRectTransform2 = infos[infos.Count - 1].rectTransform;
                    }
                    EditorGUI.EndDisabledGroup();

                    info.ignore = EditorGUILayout.Toggle(info.ignore);

                    var effective = IsEffectiveType(info.rectTransform, effectiveType);
                    EditorGUILayout.Toggle(effective);

                    var match = IsNameMatch(info.rectTransform, nameMatch);
                    EditorGUILayout.Toggle(match);

                    EditorGUILayout.Toggle(!info.ignore && effective && match);

                    UICommonFun.EndHorizontal();
                }
                CommonFun.EndLayout();
            }

        }

#endregion

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            TypeHelper.FindTypeInAppWithInterface(typeof(IRectTransformLayoutWindow)).ForEach(t => layouts.Add(TypeHelper.CreateInstance(t) as IRectTransformLayoutWindow));

            EditorApplication.projectChanged += InitData;
            XDreamerEditor.onBeforeCompileAllAssets += InitData;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            XDreamerEditor.onBeforeCompileAllAssets -= InitData;
            EditorApplication.projectChanged -= InitData;
        }

        /// <summary>
        /// 当绘制场景GUI
        /// </summary>
        /// <param name="sceneView"></param>
        public override void OnSceneGUI(SceneView sceneView)
        {
            var color = Handles.color;
            if (standardRectTransform1)
            {
                Handles.color = LayoutOption.weakInstance.standardColor1;
                Handles.DrawWireCube(standardRectTransform1.position, standardRectTransform1.sizeDelta);
            }
            if (standardRectTransform2 && standardRectTransform1 != standardRectTransform2)
            {
                Handles.color = LayoutOption.weakInstance.standardColor2;
                Handles.DrawWireCube(standardRectTransform2.position, standardRectTransform2.sizeDelta);
            }
            Handles.color = color;
            SceneView.RepaintAll();
        }

        private void InitData()
        {
            lockStandardRectTransform1 = false;
            lockStandardRectTransform2 = false;
            isLocked = false;
        }

        /// <summary>
        /// 当选择集变更
        /// </summary>
        protected void OnSelectionChange()
        {
            if (!isLocked && useFullSelection)
            {
                infos.Clear();
                UpdateInfos();
            }
            Repaint();
        }

        /// <summary>
        /// 当层级变更
        /// </summary>
        protected void OnHierarchyChange()
        {
            if (currentRectTransform && currentRectTransform.childCount != infos.Count)
            {
                infos.Clear();
                UpdateInfos();
            }
            Repaint();
        }

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            ShowBaseOperation();
            ShowBaseLayout();
            ShowOtherLayout();
            base.OnGUI();
        }

        /// <summary>
        /// 当绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            ShowBaseInfo();
            ShowDetailInfo();
        }
    }
}
