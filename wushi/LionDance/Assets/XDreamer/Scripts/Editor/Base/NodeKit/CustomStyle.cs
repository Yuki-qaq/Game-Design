using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.NodeKit
{
    /// <summary>
    /// 自定义样式
    /// </summary>
    public class CustomStyle : DelayGUIContent<CustomStyle>
    {
        #region 常用

        /// <summary>
        /// 普通节点尺寸
        /// </summary>
        public Vector2 normalNodeSize = new Vector2(120, 50);

        /// <summary>
        /// 跳转节点尺寸
        /// </summary>
        public Vector2 transitionNodeSize = new Vector2(20, 20);

        /// <summary>
        /// 插槽节点尺寸
        /// </summary>
        public Vector2 slotNodeSize = new Vector2(20, 20);

        /// <summary>
        /// 图标尺寸
        /// </summary>
        public Vector2 iconSize = new Vector2(20, 20);

        /// <summary>
        /// 画布工具栏按钮尺寸
        /// </summary>
        public Vector2 canvasToolBarButtonSize = new Vector2(32, 32);

        /// <summary>
        /// 画布工具栏按钮间距
        /// </summary>
        public Vector2 canvasToolBarButtonSpace = new Vector2(10, 10);

        /// <summary>
        /// 已选择颜色
        /// </summary>
        public Color selectedColor => Color.yellow;

        /// <summary>
        /// 内容无效颜色
        /// </summary>
        public Color contentInvalidColor => Color.red;

        /// <summary>
        /// 激活颜色
        /// </summary>
        public Color activeColor => Color.green;

        /// <summary>
        /// 繁忙颜色
        /// </summary>
        public Color busyColor => Color.magenta;

        /// <summary>
        /// 工具条按钮样式
        /// </summary>
        public XGUIStyle statebarBtn { get; } = new XGUIStyle("toolbarButton", 12);

        /// <summary>
        /// 图标按钮样式，用于显示比较大的图标
        /// </summary>
        public XGUIStyle iconButtonStyle { get; } = new XGUIStyle("IconButton");

        /// <summary>
        /// 后退
        /// </summary>
        [XCSJ.Attributes.Icon(EIcon.Backward)]
        [Name("后退")]
        public XGUIContent backContent { get; } = GetXGUIContent(nameof(backContent));

        /// <summary>
        /// 前进
        /// </summary>
        [XCSJ.Attributes.Icon(EIcon.Forward)]
        [Name("前进")]
        public XGUIContent forwardContent { get; } = GetXGUIContent(nameof(forwardContent));

        /// <summary>
        /// 缩放
        /// </summary>
        [Name("重置缩放")]
        [Tip("设置缩放值为1", "Set the zoom value to 1")]
        [XCSJ.Attributes.Icon(EIcon.Scale)]
        public bool resetScale;

        /// <summary>
        /// 缩放
        /// </summary>
        [Name("自动链接活跃节点")]
        [Tip("Unity编辑器运行时，画布自动将活跃节点设置到画布中心", "When the unity editor runs, the canvas automatically sets the active node to the center of the canvas")]
        [XCSJ.Attributes.Icon(EIcon.Link)]
        public bool autoLinkActiveNode;

        /// <summary>
        /// 缩放
        /// </summary>
        [Name("自动选中活跃节点")]
        [Tip("Unity编辑器运行时，画布自动选中活跃节点", "When the unity editor runs, the canvas automatically selects the active node")]
        [XCSJ.Attributes.Icon(EIcon.Target)]
        public bool autoSelectActiveNode;

        /// <summary>
        /// 帮助风格
        /// </summary>
        public XGUIStyle helpBoxStyle { get; } = new XGUIStyle("HelpBox");

        /// <summary>
        /// 系统组风格
        /// </summary>
        public XGUIStyle groupBoxStyle = new XGUIStyle("GroupBox");

        /// <summary>
        /// 内容文本字体
        /// </summary>
        public XGUIStyle contentTextStyle = new XGUIStyle(GUIStyleCreater.PrefixName + "NodeEditor_NodeInfo");

        /// <summary>
        /// 居中文本
        /// </summary>
        public XGUIStyle middleCenterLabelStyle { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
        {
            s.alignment = TextAnchor.MiddleCenter;
        });

        /// <summary>
        /// 居中文本
        /// </summary>
        public XGUIStyle progressLabelStyle { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
        {
            s.alignment = TextAnchor.MiddleCenter;
            s.normal.textColor = Color.white;
        });

        /// <summary>
        /// 不在搜索内的节点风格
        /// </summary>
        public XGUIStyle notInSearchNodeStyle { get; } = new XGUIStyle("LightmapEditorSelectedHighlight", s =>
        {
            s.name = nameof(notInSearchNodeStyle);
            s.richText = true;
            s.overflow = new RectOffset(0, 0, 0, 0);
            s.normal.background = EditorIconHelper.GetIconInLib(EIcon.Border);
        }, XGUIStyle.DefaultValidFuncWithNormalBackground);

        /// <summary>
        /// 节点选中边框样式
        /// </summary>
        public XGUIStyle selectionBorderStyle { get; private set; } = new XGUIStyle(EGUIStyle.SelectionBorder);

        /// <summary>
        /// 选择框样式
        /// </summary>
        public XGUIStyle selectionRectStyle { get; } = new XGUIStyle("SelectionRect");

        /// <summary>
        /// 工具栏
        /// </summary>
        [Name("工具")]
        [Tip("工具", "Tool")]
        [XCSJ.Attributes.Icon(EIcon.Tool)]
        public bool tool;

        /// <summary>
        /// 连入
        /// </summary>
        [Name("连入")]
        [Tip("Connect in", "连入")]
        [XCSJ.Attributes.Icon(EIcon.Forward)]
        public XGUIContent connectIn { get; } = GetXGUIContent(nameof(connectIn));

        /// <summary>
        /// 右连出向
        /// </summary>
        [Name("连出")]
        [Tip("Connect out", "连出")]
        [XCSJ.Attributes.Icon(EIcon.Forward)]
        public XGUIContent connectOut { get; } = GetXGUIContent(nameof(connectOut));

        /// <summary>
        /// 图标样式
        /// </summary>
        public XGUIStyle iconStyle { get; private set; } = new XGUIStyle(EGUIStyle.Button_NoneBackground);

        /// <summary>
        /// 分割线风格
        /// </summary>
        public XGUIStyle separatorStyle { get; private set; } = new XGUIStyle(EGUIStyle.Separator);

        #endregion

        #region 标题

        /// <summary>
        /// 标题字体尺寸
        /// </summary>
        public const int NodeTitleFontSize = 13;

        /// <summary>
        /// 标题高度
        /// </summary>
        public float nodeTitleHeight = 20;

        /// <summary>
        /// 节点内容高度
        /// </summary>
        public float nodeContentHeight = 16;

        /// <summary>
        /// 组标题采用风格
        /// </summary>
        public virtual XGUIStyle contentStyle
        {
            get
            {
                var s = titleStyle.style;
                if (!s.normal.background) s.normal.background = Texture2DHelper.GetTexture2D(GraphOption.weakInstance.perNodeContentColor);
                return titleStyle;
            }
        }

        /// <summary>
        /// 标题栏
        /// </summary>
        public virtual XGUIStyle titleStyle { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
        {
            s.overflow = new RectOffset(-2, -2, -2, -2);
            s.normal.background = Texture2DHelper.GetTexture2D(GraphOption.weakInstance.perNodeContentColor);
        });

        /// <summary>
        /// 无背景标题栏
        /// </summary>
        public virtual XGUIStyle titleStyleWithoutBackground { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
        {
            s.fontSize = NodeTitleFontSize;
            s.alignment = TextAnchor.MiddleCenter;
        });

        /// <summary>
        /// 标题内容节点背景
        /// </summary>
        [XCSJ.Attributes.Icon]
        public XGUIStyle titleContentNodeBackgroundStyle = new XGUIStyle("flow node 0", s =>
        {
            //s.normal.background = IconHelper.GetIconInLib(typeof(NodeEditorStyle).GetField(nameof(titleContentNodeStyle)));
            //s.border = new RectOffset(32, 32, 32, 32);
            //s.padding = new RectOffset(16, 16, 4, 16);            
        });

        #endregion

        #region 组

        /// <summary>
        /// 标题栏
        /// </summary>
        public XGUIStyle groupTitleStyle { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
        {
            s.fontSize = NodeTitleFontSize;
            s.alignment = TextAnchor.MiddleLeft;
            s.normal.textColor = Color.white;
        });

        /// <summary>
        /// 组标题采用风格
        /// </summary>
        public XGUIStyle groupTitleBackgroundStyle
        {
            get
            {
                var s = _groupTitleBackgroundStyle.style;
                if (!s.normal.background) s.normal.background = Texture2DHelper.GetTexture2D(_groupTitleColor);
                return _groupTitleBackgroundStyle;
            }
        }

        private XGUIStyle _groupTitleBackgroundStyle = new XGUIStyle(nameof(GUIStyle.none), s =>
        {
            s.normal.background = Texture2DHelper.GetTexture2D(_groupTitleColor);
            s.overflow = new RectOffset(-1, -1, -1, -1);
        });

        private static Color _groupTitleColor = new Color(0.2f, 0.55f, 0.2f, 0.8f);

        /// <summary>
        /// 最小尺寸:设置当前组包含节点的最小尺寸
        /// </summary>
        [XCSJ.Attributes.Icon(EIcon.BestSize)]
        [Name("最小尺寸")]
        [Tip("设置当前组包含节点的最小尺寸", "Contains the minimum size setting for the current node group")]
        public XGUIContent groupMinSize { get; } = GetXGUIContent(nameof(groupMinSize));

        /// <summary>
        /// 缩放尺寸:改变组的尺寸
        /// </summary>
        [XCSJ.Attributes.Icon(EIcon.ScaleSize)]
        [Name("缩放尺寸")]
        [Tip("改变组的尺寸", "Change the size of the group")]
        public XGUIContent groupScaleSize { get; } = GetXGUIContent(nameof(groupScaleSize));

        #endregion

        #region 状态条内容及样式

        /// <summary>
        /// 分类按钮内容
        /// </summary>
        [Name("分类")]
        [Tip("分类")]
        [XCSJ.Attributes.Icon(EIcon.Category)]
        public XGUIContent categoryButtonContent { get; } = GetXGUIContent(nameof(categoryButtonContent));

        /// <summary>
        /// 检查器按钮内容
        /// </summary>
        [Name("检查器")]
        [Tip("检查器")]
        [XCSJ.Attributes.Icon(EIcon.Property)]
        public XGUIContent inspectorButtonContent { get; } = GetXGUIContent(nameof(inspectorButtonContent));

        /// <summary>
        /// 导航条最左侧样式
        /// </summary>
        public XGUIStyle navigationBarLeftStyle = new XGUIStyle(EGUIStyle.NavigationLeftBar);

        /// <summary>
        /// 导航条中间样式
        /// </summary>       
        public XGUIStyle navigationBarMidStyle = new XGUIStyle(EGUIStyle.NavigationMiddleBar);

        #endregion

        #region 层级节点样式

        private const int treeFontSize = 13;

        /// <summary>
        /// 组标题采用风格
        /// </summary>
        public XGUIStyle treeNodeCanvasSelectedStyle
        {
            get
            {
                var s = _treeNodeCanvasSelectedStyle.style;
                if (!s.normal.background)
                {
                    s.normal.background = UICommonOption.GetSelectedTexture();
                }
                return _treeNodeCanvasSelectedStyle;
            }
        }

        private XGUIStyle _treeNodeCanvasSelectedStyle = new XGUIStyle(nameof(GUIStyle.none), s =>
        {
            s.normal.textColor = Color.white;
            s.alignment = TextAnchor.MiddleLeft;
            s.fontSize = treeFontSize;
            s.normal.background = null;
        });

        /// <summary>
        /// 树形节点正常样式
        /// </summary>
        public XGUIStyle treeNodeNormalStyle { get; } = new XGUIStyle(nameof(EditorStyles.label), s =>
        {
            s.fontSize = treeFontSize;
            s.alignment = TextAnchor.MiddleLeft;
        });

        /// <summary>
        /// 树形节点不可用状态
        /// </summary>
        public XGUIStyle treeNodeDisableStyle { get; } = new XGUIStyle(nameof(EditorStyles.label), s =>
        {
            s.margin = new RectOffset();
            s.normal.textColor = new Color(0.4f, 0.4f, 0.4f, 1);
        });

        /// <summary>
        /// 树形节点正常样式
        /// </summary>
        public XGUIStyle treeIconNodeNormalStyle { get; } = new XGUIStyle(nameof(EditorStyles.helpBox), s =>
        {
            //s.border = new RectOffset(3, 3, 0, 3);
        });

        /// <summary>
        /// 选中样式
        /// </summary>
        public XGUIStyle treeIconNodeSelectionBorder { get; } = new XGUIStyle("LightmapEditorSelectedHighlight", s =>
        {
            s.name = nameof(treeIconNodeSelectionBorder);
            s.overflow = new RectOffset(3, 3, 3, 3);
            s.normal.background = EditorIconHelper.GetIconInLib(EIcon.Border);
        }, XGUIStyle.DefaultValidFuncWithNormalBackground);

        #endregion

        #region 小地图

        /// <summary>
        /// 小地图项颜色
        /// </summary>
        public Color miniMapItemColor => Color.gray;

        /// <summary>
        /// 小地图父级项颜色
        /// </summary>
        public Color miniMapParentItemColor { get; } = new Color(1, 0.4f, 0.1f);

        /// <summary>
        /// 小地图控制按钮尺寸
        /// </summary>
        public Vector2 miniMapControlButtonSize = new Vector2(20, 20);

        /// <summary>
        /// 小地图尺寸
        /// </summary>
        public Vector2 miniMapSize = new Vector2(160, 160);

        /// <summary>
        /// 左上角：左上角小地图
        /// </summary>
        [Name("左上角")]
        [Tip("小地图")]
        [XCSJ.Attributes.Icon(EIcon.LeftUpCorner)]
        public XGUIContent LeftUpCorner { get; } = GetXGUIContent(nameof(LeftUpCorner));

        /// <summary>
        /// 画布工具栏:用于控制画布工具栏的显示隐藏
        /// </summary>
        [Name("画布工具栏")]
        [Tip("用于控制画布工具栏的显示隐藏", "Used to control the display and hiding of the canvas toolbar")]
        [XCSJ.Attributes.Icon(EIcon.RightUpCorner)]
        public XGUIContent canvasToolBarLabel { get; } = GetXGUIContent(nameof(canvasToolBarLabel));

        /// <summary>
        /// 左下角:左下角小地图
        /// </summary>
        [Name("左下角")]
        [Tip("左下角小地图", "Small map in the lower left corner")]
        [XCSJ.Attributes.Icon(EIcon.LeftDownCorner)]
        public XGUIContent LeftBottomCorner { get; } = GetXGUIContent(nameof(LeftBottomCorner));

        /// <summary>
        /// 小地图:用于控制小地图的显示隐藏
        /// </summary>
        [Name("小地图")]
        [Tip("用于控制小地图的显示隐藏", "Used to control the display and hiding of mini maps")]
        [XCSJ.Attributes.Icon(EIcon.RightDownCorner)]
        public XGUIContent miniMapLabel { get; } = GetXGUIContent(nameof(miniMapLabel));

        /// <summary>
        /// 小地图样式
        /// </summary>
        public virtual XGUIStyle miniMapStyle { get; } = new XGUIStyle(GUIStyleCreater.PrefixName + "NodeEditor_MiniBox");

        #endregion

        #region 节点样式

        /// <summary>
        /// 灰色节点
        /// </summary>
        public XGUIStyle grayNodeStyle { get; } = new XGUIStyle("flow node 0");

        /// <summary>
        /// 蓝色节点
        /// </summary>
        public XGUIStyle blueNodeStyle { get; } = new XGUIStyle("flow node 1");

        /// <summary>
        /// 青色节点
        /// </summary>
        public XGUIStyle cyanNodeStyle { get; } = new XGUIStyle("flow node 2");

        /// <summary>
        /// 绿色节点
        /// </summary>
        public XGUIStyle greenNodeStyle { get; } = new XGUIStyle("flow node 3");

        /// <summary>
        /// 橙色节点
        /// </summary>
        public XGUIStyle orangeNodeStyle { get; } = new XGUIStyle("flow node 5");

        /// <summary>
        /// 红色节点
        /// </summary>
        public XGUIStyle redNodeStyle { get; } = new XGUIStyle("flow node 6");

        /// <summary>
        /// 黄色节点
        /// </summary>
        public XGUIStyle yellowNodeStyle { get; } = new XGUIStyle("flow node hex 4");

        /// <summary>
        /// MB节点风格
        /// </summary>
        public XGUIStyle MBNodeStyle { get; } = new XGUIStyle("flow node 2");

        /// <summary>
        /// 父级节点风格
        /// </summary>
        public XGUIStyle parentNodeStyle { get; } = new XGUIStyle("flow node 5");

        /// <summary>
        /// 组节点风格
        /// </summary>
        public XGUIStyle groupNodeStyle { get; } = new XGUIStyle("U2D.createRect");

        #endregion

        #region 节点箭头

        /// <summary>
        /// 上箭头
        /// </summary>
        public Texture2D upArrow = EditorIconHelper.GetIconInLib(EIcon.ArrowHeadUp_1);

        /// <summary>
        /// 下箭头
        /// </summary>
        public Texture2D downArrow = EditorIconHelper.GetIconInLib(EIcon.ArrowHeadDown_1);

        /// <summary>
        /// 左箭头
        /// </summary>
        public Texture2D leftArrow = EditorIconHelper.GetIconInLib(EIcon.ArrowHeadLeft_1);

        /// <summary>
        /// 右箭头
        /// </summary>
        public Texture2D rightArrow = EditorIconHelper.GetIconInLib(EIcon.ArrowHeadRight_1);

        #endregion

        #region 常用图标

        /// <summary>
        /// 进入贴图
        /// </summary>
        public Texture2D entryTexture = EditorIconHelper.GetIconInLib(EIcon.Enter);

        /// <summary>
        /// 退出贴图
        /// </summary>
        public Texture2D exitTexture = EditorIconHelper.GetIconInLib(EIcon.Exit);

        /// <summary>
        /// 更新贴图
        /// </summary>
        public Texture2D updateTexture = EditorIconHelper.GetIconInLib(EIcon.Loop);

        /// <summary>
        /// 导入贴图
        /// </summary>
        public Texture2D importTexture = EditorIconHelper.GetIconInLib(EIcon.Import);

        /// <summary>
        /// 导出贴图
        /// </summary>
        public Texture2D exportTexture = EditorIconHelper.GetIconInLib(EIcon.Export);

        /// <summary>
        /// 前进贴图
        /// </summary>
        public Texture2D forwardTexture = EditorIconHelper.GetIconInLib(EIcon.Forward);

        /// <summary>
        /// 后退贴图
        /// </summary>
        public Texture2D backTexture = EditorIconHelper.GetIconInLib(EIcon.Backward);

        /// <summary>
        /// 配置贴图
        /// </summary>
        public Texture2D configTexture = EditorIconHelper.GetIconInLib(EIcon.Config);

        /// <summary>
        /// 列表贴图
        /// </summary>
        public Texture2D listTexture = EditorIconHelper.GetIconInLib(EIcon.List);

        /// <summary>
        /// 工具贴图
        /// </summary>
        public Texture2D toolTexture = EditorIconHelper.GetIconInLib(EIcon.Tool);

        #endregion

    }

    /// <summary>
    /// 延时GUI内容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelayGUIContent<T>
    {
        /// <summary>
        /// 获取GUIContent
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="emptyTextWhenHasImage"></param>
        /// <returns></returns>
        protected static XGUIContent GetXGUIContent(string propertyName, bool emptyTextWhenHasImage = true) => new XGUIContent(typeof(T), propertyName, emptyTextWhenHasImage);
    }

    /// <summary>
    /// 个人样式
    /// </summary>
    public class PersonalStyle : CustomStyle 
    {
        /// <summary>
        /// 组标题采用风格
        /// </summary>
        public override XGUIStyle contentStyle
        {
            get
            {
                var s = titleStyle.style;
                if (!s.normal.background)
                {
                    s.normal.background = Texture2DHelper.GetTexture2D(GraphOption.weakInstance.perNodeContentColor);
                }
                return titleStyle;
            }
        }

        /// <summary>
        /// 标题栏
        /// </summary>
        public override XGUIStyle titleStyle { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
        {
            s.overflow = new RectOffset(-2, -2, -2, -2);
            s.normal.background = Texture2DHelper.GetTexture2D(GraphOption.weakInstance.perNodeContentColor);
        });

        /// <summary>
        /// 无背景标题栏
        /// </summary>
        public override XGUIStyle titleStyleWithoutBackground { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
        {
            s.fontSize = NodeTitleFontSize;
            s.alignment = TextAnchor.MiddleCenter;
            s.normal.textColor = Color.black;
        });
    }

    /// <summary>
    /// 专业样式
    /// </summary>
    public class ProfessionalStyle : CustomStyle 
    {
        /// <summary>
        /// 组标题采用风格
        /// </summary>
        public override XGUIStyle contentStyle
        {
            get
            {
                var s = titleStyle.style;
                if (!s.normal.background)
                {
                    s.normal.background = Texture2DHelper.GetTexture2D(GraphOption.weakInstance.proNodeContentColor);
                }
                return titleStyle;
            }
        }

        /// <summary>
        /// 标题栏
        /// </summary>
        public override XGUIStyle titleStyle { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
        {
            s.overflow = new RectOffset(-2, -2, -2, -2);
            s.normal.background = Texture2DHelper.GetTexture2D(GraphOption.weakInstance.proNodeContentColor);
        });

        /// <summary>
        /// 无背景标题栏
        /// </summary>
        public override XGUIStyle titleStyleWithoutBackground { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
        {
            s.fontSize = NodeTitleFontSize;
            s.alignment = TextAnchor.MiddleCenter;
            s.normal.textColor = Color.white;
        });
    }
}
