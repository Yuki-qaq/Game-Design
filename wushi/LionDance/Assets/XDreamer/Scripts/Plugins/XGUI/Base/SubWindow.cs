using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 子窗口：可嵌套
    /// </summary>
    [Name("子窗口")]
    [Tip("窗口基类；子窗口之间可嵌套", "Window base class; Child windows can be nested")]
    [DisallowMultipleComponent]
    public class SubWindow : DraggableView
    {
        #region 交互命令

        #region 拖拽命令

        /// <summary>
        /// 启用拖拽
        /// </summary>
        [InteractCmd]
        [Name("启用拖拽")]
        public void EnableDrag() => TryInteract(nameof(EnableDrag));

        /// <summary>
        /// 启用拖拽
        /// </summary>
        [InteractCmdFun(nameof(EnableDrag))]
        public void OnEnableDrag(InteractData interactData) { if (!canDrag) this.XModifyProperty(() => canDrag = true); }

        /// <summary>
        /// 禁用拖拽
        /// </summary>
        [InteractCmd]
        [Name("禁用拖拽")]
        public void DisableDrag() => TryInteract(nameof(DisableDrag));

        /// <summary>
        /// 禁用拖拽
        /// </summary>
        [InteractCmdFun(nameof(DisableDrag))]
        public void OnDisableDrag(InteractData interactData) { if (canDrag)  this.XModifyProperty(() => canDrag = false); }

        #endregion

        #region 展开折叠命令

        /// <summary>
        /// 展开
        /// </summary>
        [InteractCmd]
        [Name("展开")]
        public void Expand() => TryInteract(nameof(Expand));

        /// <summary>
        /// 展开
        /// </summary>
        [InteractCmdFun(nameof(Expand))]
        public void OnExpand(InteractData interactData) { if (!expand)  this.XModifyProperty(() => expand = true); }

        /// <summary>
        /// 折叠
        /// </summary>
        [InteractCmd]
        [Name("折叠")]
        public void Unexpand() => TryInteract(nameof(Unexpand));

        /// <summary>
        /// 折叠
        /// </summary>
        [InteractCmdFun(nameof(Unexpand))]
        public void OnUnexpand(InteractData interactData) { if (expand)  this.XModifyProperty(() => expand = false); }

        #endregion

        #region 尺寸改变命令

        /// <summary>
        /// 最大化
        /// </summary>
        [InteractCmd]
        [Name("最大化")]
        public void MaxSize() => TryInteract(nameof(MaxSize));

        /// <summary>
        /// 最大化
        /// </summary>
        [InteractCmdFun(nameof(MaxSize))]
        public void OnMaxSize(InteractData interactData) { if (!maxSize)  this.XModifyProperty(() => maxSize = true); }

        /// <summary>
        /// 正常化
        /// </summary>
        [InteractCmd]
        [Name("正常化")]
        public void NormalSize()
        {
            TryInteract(nameof(NormalSize));
        }

        /// <summary>
        /// 正常化
        /// </summary>
        [InteractCmdFun(nameof(NormalSize))]
        public void OnNormalSize(InteractData interactData) { if (maxSize)  this.XModifyProperty(() => maxSize = false); }

        /// <summary>
        /// 尺寸改变
        /// </summary>
        [InteractCmd]
        [Name("尺寸改变")]
        public void SizeChanged() => TryInteract(nameof(SizeChanged));

        /// <summary>
        /// 尺寸改变
        /// </summary>
        [InteractCmdFun(nameof(SizeChanged))]
        public void OnSizeChanged(InteractData interactData) => CallSizeChanged();

        #endregion

        #region 全屏幕命令

        /// <summary>
        /// 全屏幕
        /// </summary>
        [InteractCmd]
        [Name("全屏幕")]
        public void FullScreen() => TryInteract(nameof(FullScreen));

        /// <summary>
        /// 全屏幕
        /// </summary>
        [InteractCmdFun(nameof(FullScreen))]
        public void OnFullScreen(InteractData interactData) { if (!fullScreen) this.XModifyProperty(() => fullScreen = true); }

        /// <summary>
        /// 非全屏幕
        /// </summary>
        [InteractCmd]
        [Name("非全屏幕")]
        public void NotFullScreen() => TryInteract(nameof(NotFullScreen));

        /// <summary>
        /// 非全屏幕
        /// </summary>
        [InteractCmdFun(nameof(NotFullScreen))]
        public void OnNotFullScreen(InteractData interactData) { if (fullScreen) this.XModifyProperty(() => fullScreen = false); }

        #endregion

        #region 关闭命令

        /// <summary>
        /// 关闭
        /// </summary>
        [InteractCmd]
        [Name("关闭")]
        public void Close()
        {
            TryInteract(nameof(Close));
        }

        /// <summary>
        /// 关闭
        /// </summary>
        [InteractCmdFun(nameof(Close))]
        public void OnClose(InteractData interactData) => this.XModifyProperty(() => display = false);

        #endregion

        #endregion

        #region 可拖拽_折叠_全屏幕_最大化

        /// <summary>
        /// 基础设置
        /// </summary>
        [Group("窗口设置", textEN = "Window Settings", defaultIsExpanded = false)]
        [Name("可拖拽")]
        [Tip("勾选，窗口可拖拽;不勾选，窗口不可拖拽")]
        public bool _canDrag = true;

        /// <summary>
        /// 可拖拽
        /// </summary>
        public override bool canDrag
        {
            get
            {
                switch (_dragRule)
                {
                    case EDragRule.DisableDragOnCanvasWorldSpaceMode:
                        {
                            if (rootCanvas && rootCanvas.renderMode == RenderMode.WorldSpace)
                            {
                                return false;
                            }
                            break;
                        }
                }

                // 窗口全屏或最大化时不允许移动
                return !maxSize && !fullScreen && _canDrag;
            }
            set
            {
                if (maxSize || fullScreen) return;

                _canDrag = value;
            }
        }

        /// <summary>
        /// 拖拽规则
        /// </summary>
        public enum EDragRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 禁用拖拽当画布为世界空间模式时
            /// </summary>
            [Name("禁用拖拽当画布为世界空间模式时")]
            DisableDragOnCanvasWorldSpaceMode,
        }

        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [EnumPopup]
        public EDragRule _dragRule = EDragRule.None;

        /// <summary>
        /// 展开
        /// </summary>
        [Name("展开")]
        public bool _expand = true;

        /// <summary>
        /// 展开
        /// </summary>
        public bool expand
        {
            get => _expand;
            set
            {
                if (_content)
                {
                    _content.gameObject.SetActive(value);
                }

                _expand = value;
            }
        }

        /// <summary>
        /// 全屏
        /// </summary>
        [Name("全屏")]
        [HideInSuperInspector(nameof(_expand), EValidityCheckType.Equal, false)]
        public bool _fullScreen = false;

        /// <summary>
        /// 全屏
        /// </summary>
        public bool fullScreen
        {
            get => _fullScreen;
            set
            {
                _fullScreen = value;
                OnFullScreenChanged();
            }
        }

        private bool _maxSize = false;

        /// <summary>
        /// 最大化
        /// </summary>
        public bool maxSize
        {
            get => _maxSize;
            set
            {
                _maxSize = value;

                LayoutTitleBar(_titleDirection == EFourDirection.Top || _titleDirection == EFourDirection.Bottom);
                OnMaxSizeChanged();
            }
        }

        /// <summary>
        /// 宽
        /// </summary>
        public virtual float width { get => size.x; }

        /// <summary>
        /// 高
        /// </summary>
        public virtual float height { get => size.y; }

        /// <summary>
        /// 位置
        /// </summary>
        public virtual Vector2 positon
        {
            get => rectTransform.anchoredPosition;
            set => rectTransform.anchoredPosition = value;
        }

        /// <summary>
        /// 尺寸
        /// </summary>
        public virtual Vector2 size
        {
            get => rectTransform.rect.size;
            set
            {
                rectTransform.sizeDelta = value;

                if (expand && (!maxSize || !fullScreen))
                {
                    lastExpandSize = value;
                }
            }
        }

        /// <summary>
        /// 缩放
        /// </summary>
        public float scale
        {
            get => _scale;
            set
            {
                var realScale = Mathf.Approximately(_scale, 0) ? 0 : (value / _scale);
                size *= realScale;
                _scale = value;
            }
        }
        private float _scale = 1;

        #endregion

        #region 尺寸变化

        #region 锚点

        /// <summary>
        /// 上一次最小锚点
        /// </summary>
        protected Vector2 _lastAnchorMin = new Vector2();

        /// <summary>
        /// 上一次最大锚点
        /// </summary>
        protected Vector2 _lastAnchorMax = new Vector2();

        /// <summary>
        /// 记录锚点
        /// </summary>
        protected void RecordRectAnchor()
        {
            _lastAnchorMin = rectTransform.anchorMin;
            _lastAnchorMax = rectTransform.anchorMax;
        }

        /// <summary>
        /// 恢复锚点
        /// </summary>
        protected void RecoverRectAnchor()
        {
            rectTransform.anchorMin = _lastAnchorMin;
            rectTransform.anchorMax = _lastAnchorMax;
        }

        #endregion

        private Vector2 lastExpandPosition;
        private Vector2 lastExpandSize;

        private void RecoverNormalWindowRect()
        {
            RecoverRectAnchor();

            size = lastExpandSize;
            positon = lastExpandPosition;
        }

        /// <summary>
        /// 调用尺寸变化事件
        /// </summary>
        public void CallSizeChanged() => onSizeChanged?.Invoke(this, rectTransform.rect);

        /// <summary>
        /// 尺寸变化
        /// </summary>
        public static Action<SubWindow, Rect> onSizeChanged;

        private bool positionChangedByMaxSize = false;

        private void OnMaxSizeChanged()
        {
            if (_maxSize)
            {
                lastExpandPosition = positon;
                SetFullScreen();
            }
            else
            {
                RecoverNormalWindowRect();
            }
            CallSizeChanged();
        }

        private void SetFullScreen()
        {
            positionChangedByMaxSize = true;
            rectTransform.SetFullScreen();
        }

        private void OnFullScreenChanged()
        {
            if (title) title.gameObject.SetActive(!_fullScreen);
            if (_fullScreen)
            {
                lastExpandPosition = positon;
                SetFullScreen();
            }
            else
            {
                RecoverNormalWindowRect();
            }
            CallSizeChanged();
        }

        #endregion

        #region 标题设置

        /// <summary>
        /// 标题
        /// </summary>
        [Name("标题")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public RectTransform _title = null;

        /// <summary>
        /// 标题
        /// </summary>
        public RectTransform title { get => _title; set => _title = value; }

        /// <summary>
        /// 标题文本
        /// </summary>
        public string titleText
        {
            get => titleBar ? titleBar.titleTextValue : string.Empty;
            set
            {
                if (titleBar)
                {
                    titleBar.titleTextValue = value;
                }
            }
        }

        /// <summary>
        /// 标题栏
        /// </summary>
        protected TitleBar titleBar
        {
            get
            {
                if (!_titleBar)
                {
                    if (_title)
                    {
                        _titleBar = _title.GetComponent<TitleBar>();
                    }
                }
                return _titleBar;
            }
        }
        private TitleBar _titleBar;

        /// <summary>
        /// 打开
        /// </summary>
        public void Open() => display = true;

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="value"></param>
        public void Scale(float value) => Scale(Vector3.one * value);

        /// <summary>
        /// 缩放：三轴缩放
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(Vector3 scale) => transform.localScale = scale;

        #endregion

        #region 内容设置

        /// <summary>
        /// 内容
        /// </summary>
        [Name("内容")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public RectTransform _content = null;

        /// <summary>
        /// 窗口尺寸修改器
        /// </summary>
        [Name("窗口尺寸修改器")]
        public WindowResizer _windowResizer;

        /// <summary>
        /// 最小尺寸
        /// </summary>
        [Name("最小尺寸")]
        public Vector2 _minSize = new Vector2(250, 100);

        /// <summary>
        /// 内容
        /// </summary>
        public RectTransform content { get => _content; set => _content = value; }

        /// <summary>
        /// 内容大小
        /// </summary>
        public Vector2 contentSize => content ? content.rect.size : Vector2.zero;

        /// <summary>
        /// 显示
        /// </summary>
        public bool display { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

        #endregion

        #region Unity消息

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (init)
            {
                init = false;

                RecordRectAnchor();
                lastExpandPosition = positon;
                lastExpandSize = size; 

                Invoke(nameof(Init), 0.5f);
            }

            TransformListener.onTransformHasChanged += OnTransformHasChanged;
            TransformListener.Add(transform);
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            TransformListener.onTransformHasChanged -= OnTransformHasChanged;
            TransformListener.Remove(transform);
        }

        private void OnTransformHasChanged(Transform transform)
        {
            if (this.transform == transform)
            {
                if (!maxSize && !fullScreen)
                {
                    lastExpandPosition = positon;
                }
                else
                {
                    if (positionChangedByMaxSize)
                    {
                        positionChangedByMaxSize = false;
                    }
                    else
                    {
                        //Debug.LogWarningFormat("【{0}】在最大化和全屏时，变换不能被修改！", CommonFun.ObjectToString(this));
                        SetFullScreen();
                    }
                }
                CallSizeChanged();
            }
        }

        private bool init = true;

        private void Init()
        {
            expand = _expand;
        }

        #endregion

        #region 样式编辑

        /// <summary>
        /// 标题方位
        /// </summary>
        [Group("样式设置", textEN = "Style Settings", defaultIsExpanded = false)]
        [Name("标题方位")]
        [EnumPopup]
        public EFourDirection _titleDirection = EFourDirection.Top;

        /// <summary>
        /// 标题与内容间隙
        /// </summary>
        [Name("标题与内容间隙")]
        [Min(0)]
        public float _titleAndContentSpace = 2;

        #region 标题水平布局

        /// <summary>
        /// 宽度规则
        /// </summary>
        [Name("宽度规则")]
        public enum EWidthRule
        {
            /// <summary>
            /// 固定
            /// </summary>
            [Name("固定")]
            Fixed,

            /// <summary>
            /// 窗口宽度
            /// </summary>
            [Name("窗口宽度")]
            WindowWidth,
        }

        /// <summary>
        /// 宽度规则
        /// </summary>
        [Name("宽度规则")]
        [HideInSuperInspector(nameof(_titleDirection), EValidityCheckType.NotEqual | EValidityCheckType.And, EFourDirection.Top,
            nameof(_titleDirection), EValidityCheckType.NotEqual, EFourDirection.Bottom)]
        [EnumPopup]
        public EWidthRule _widthRule = EWidthRule.WindowWidth;

        /// <summary>
        /// 水平标题宽度
        /// </summary>
        [Name("水平标题宽度")]
        [Tip("水平布局时标题的宽度", "Width of the title in horizontal layout")]
        public float _titleWidthOnHorizontal = 200;

        /// <summary>
        /// 水平标题高度
        /// </summary>
        [Name("水平标题高度")]
        [Tip("水平布局时标题的高度", "Height of the title in horizontal layout")]
        [HideInSuperInspector(nameof(_titleDirection), EValidityCheckType.NotEqual | EValidityCheckType.And, EFourDirection.Top,
            nameof(_titleDirection), EValidityCheckType.NotEqual, EFourDirection.Bottom)]
        public float _titleHeightOnHorizontal = 40;

        /// <summary>
        /// 水平方位
        /// </summary>
        [Name("水平方位")]
        [EnumPopup]
        public EHorizontalPosition _horizontalPosition = EHorizontalPosition.Left;

        #endregion

        #region 标题垂直布局

        /// <summary>
        /// 高度规则
        /// </summary>
        [Name("高度规则")]
        public enum EHeightRule
        {
            /// <summary>
            /// 固定
            /// </summary>
            [Name("固定")]
            Fixed,

            /// <summary>
            /// 窗口高度
            /// </summary>
            [Name("窗口高度")]
            WindowHeight,
        }

        /// <summary>
        /// 高度规则
        /// </summary>
        [Name("高度规则")]
        [HideInSuperInspector(nameof(_titleDirection), EValidityCheckType.NotEqual | EValidityCheckType.And, EFourDirection.Left,
            nameof(_titleDirection), EValidityCheckType.NotEqual, EFourDirection.Right)]
        [EnumPopup]
        public EHeightRule _heightRule = EHeightRule.WindowHeight;

        /// <summary>
        /// 垂直标题宽度
        /// </summary>
        [Name("垂直标题宽度")]
        [Tip("垂直布局时标题的宽度", "Width of the title in vertical layout")]
        [HideInSuperInspector(nameof(_titleDirection), EValidityCheckType.NotEqual | EValidityCheckType.And, EFourDirection.Left,
            nameof(_titleDirection), EValidityCheckType.NotEqual, EFourDirection.Right)]
        public float _titleWidthOnVertical = 40;

        /// <summary>
        /// 垂直标题高度
        /// </summary>
        [Name("垂直标题高度")]
        [Tip("垂直布局时标题的高度", "Height of title in vertical layout")]
        public float _titleHeightOnVertical = 200;

        /// <summary>
        /// 垂直方位
        /// </summary>
        [Name("垂直方位")]
        [EnumPopup]
        public EVerticalPosition _verticalPosition = EVerticalPosition.Top;

        #endregion

        /// <summary>
        /// 设置标题栏的布局
        /// </summary>
        /// <param name="isHorizontal"></param>
        public void LayoutTitleBar(bool isHorizontal)
        {
            if (titleBar)
            {
                titleBar.Layout(isHorizontal, isHorizontal ? _titleHeightOnHorizontal : _titleWidthOnVertical);
                titleBar.SetExpandRotation(_titleDirection);
            }
        }

        #endregion
    }
}
