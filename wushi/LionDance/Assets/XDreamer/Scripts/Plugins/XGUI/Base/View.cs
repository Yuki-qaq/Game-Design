using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Windows.ListViews;

namespace XCSJ.PluginXGUI.Base
{
    #region 视图接口

    /// <summary>
    /// 视图接口
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// 矩形变换
        /// </summary>
        RectTransform rectTransform { get; }

        /// <summary>
        /// 父级画布
        /// </summary>
        Canvas parentCanvas { get; }

        /// <summary>
        /// 父级视图
        /// </summary>
        IView parentView { get; }

        /// <summary>
        /// 当子级视图事件:由子级向父级传导视图发生的各种交互事件；
        /// </summary>
        /// <param name="viewInteractData">视图交互数据</param>
        void OnChildViewEvent(ViewInteractData viewInteractData);
    }

    #endregion

    #region 视图

    /// <summary>
    /// 视图：XGUI基类
    /// </summary>
    [RequireManager(typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    public abstract class View : Interactor, IView
    {
        /// <summary>
        /// 视图变换
        /// </summary>
        public RectTransform rectTransform => this.XGetComponentInChildren(ref _rectTransform);
        private RectTransform _rectTransform;

        /// <summary>
        /// 父级画布
        /// </summary>
        public Canvas parentCanvas => this.XGetComponentInParent(ref _parentCanvas);
        private Canvas _parentCanvas;

        /// <summary>
        /// 根级画布
        /// </summary>
        public Canvas rootCanvas => this.XGetComponentInParent(ref _rootCanvas, true, false);
        private Canvas _rootCanvas;

        /// <summary>
        /// 父级视图
        /// </summary>
        [Name("父级视图")]
        [Tip("当前视图以及子级视图的各种交互事件会传导给此父级视图；", "The various interaction events of the current view and child views will be transmitted to this parent view;")]
        [ComponentPopup]
        public View _parentView;

        /// <summary>
        /// 父级视图
        /// </summary>
        public View parentView { get => FindAndSetParentView(); set => this.XModifyProperty(ref _parentView, value); }

        /// <summary>
        /// 查找并设置父级视图
        /// </summary>
        /// <returns></returns>
        protected virtual View FindAndSetParentView() => this.XGetComponentInParentExcludeSelfGameObject(ref _parentView);

        /// <summary>
        /// 父级视图
        /// </summary>
        IView IView.parentView => parentView;

        /// <summary>
        /// 当子级视图事件:由子级向父级传导视图发生的各种交互事件；
        /// </summary>
        /// <param name="viewInteractData">视图交互数据</param>
        public virtual void OnChildViewEvent(ViewInteractData viewInteractData)
        {
            var parentView = this.parentView;
            if (parentView == null || parentView == this) return;
            //Debug.Log(parentView.name + "<==" + name);
            parentView.OnChildViewEvent(viewInteractData);
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (parentView == this)
            {
                _parentView = null;
                if (parentView)
                {
                    Debug.LogWarningFormat("视图的父级视图不可为视图自身:[{0}],已自动调整其父级视图为：[{2}]", 
                        this.GameObjectComponentToString(),
                        _parentView.GameObjectComponentToString());
                }
            }
        }

        /// <summary>
        /// 父级视图
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (parentView) { }
        }
    }

    #endregion

    #region 可点击视图

    /// <summary>
    /// 可点击视图
    /// </summary>
    public abstract class ClickableView : View, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 同级索引规则
        /// </summary>
        [Group("点击设置", textEN = "Click Settings", defaultIsExpanded = false)]
        [Name("同级索引规则")]
        [EnumPopup]
        public ESiblingIndexRule _siblingIndexRule = ESiblingIndexRule.None;

        private bool isPointerDown = false;

        /// <summary>
        /// 指针按下:设置窗口为同层级最前端，开始拖拽只有移动时才会触发，因此在这里进行设置才正确
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            transform.SetSiblingIndex(_siblingIndexRule);

            isPointerDown = true;
            pointDownPosition = eventData.position;
        }

        private Vector3 pointDownPosition;
        private Vector3 pointUpPosition;

        /// <summary>
        /// 点击按下弹起识别距离
        /// </summary>
        [Name("点击按下弹起识别距离")]
        [Min(1)]
        [EndGroup(true)]
        public float _clickCheckDistance = 3;

        /// <summary>
        /// 指针弹起
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            pointUpPosition = eventData.position;
            if (isPointerDown)
            {
                isPointerDown = false;

                var offset = pointUpPosition - pointDownPosition;
                if (offset.sqrMagnitude < _clickCheckDistance * _clickCheckDistance)
                {
                    OnClick(eventData);
                }
            }
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => interactData is ViewInteractData;

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="pointerEventData"></param>
        protected virtual void OnClick(PointerEventData pointerEventData)
        {
            var cmdName = GetInCmdName(nameof(OnClick));
            if (!string.IsNullOrEmpty(cmdName))
            {
                TryInteract(new ViewInteractData(pointerEventData, cmdName, this), out _);
            }
        }

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="interactData"></param>
        [InteractCmd]
        [Name("点击")]
        [InteractCmdFun(nameof(OnClick))]
        public virtual EInteractResult OnClick(ViewInteractData interactData)
        {
            OnChildViewEvent(interactData);
            return EInteractResult.Success;
        }
    }

    #endregion

    #region 可拖拽视图

    /// <summary>
    /// 可拖拽视图提供者
    /// </summary>
    public abstract class DraggableView : ClickableView, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 可拖拽
        /// </summary>
        public virtual bool canDrag { get; set; } = true;

        /// <summary>
        /// 指针按下
        /// </summary>
        private bool isBeginDrag = false;

        /// <summary>
        /// 当开始拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            CommonFun.BeginOnUI();

            if (canDrag)
            {
                isBeginDrag = true;// 保证了指针是在当前对象上按下
            }
        }

        /// <summary>
        /// 当拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (canDrag && isBeginDrag)
            {
                transform.position += (Vector3)eventData.delta;
            }
        }

        /// <summary>
        /// 当结束拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            CommonFun.EndOnUI();

            if (canDrag) isBeginDrag = false;
        }
    }

    #endregion

    #region 视图事件

    /// <summary>
    /// 视图事件
    /// </summary>
    public interface IViewEvent
    {
        /// <summary>
        /// 选择
        /// </summary>
        void Select();

        /// <summary>
        /// 点击
        /// </summary>
        void Click();
    }

    #endregion

    #region 视图交互数据

    /// <summary>
    /// 视图交互数据
    /// </summary>
    public class ViewInteractData : InteractData<ViewInteractData>
    {
        /// <summary>
        /// 模型
        /// </summary>
        public virtual object model { get; }

        /// <summary>
        /// 模型实体
        /// </summary>
        public virtual object modelEntity { get; }

        /// <summary>
        /// 视图
        /// </summary>
        public virtual IView view => interactor as IView;

        /// <summary>
        /// 视图实体
        /// </summary>
        public virtual IView viewEntity { get; }

        /// <summary>
        /// 指针事件数据
        /// </summary>
        public PointerEventData pointerEventData { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ViewInteractData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public ViewInteractData(string cmdName, InteractObject interactor, params InteractObject[] interactables)
            : base(cmdName, interactor, interactables)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="cmdParam"></param>
        /// <param name="interactables"></param>
        public ViewInteractData(string cmdName, InteractObject interactor, object cmdParam, params InteractObject[] interactables)
            : base(cmdName, interactor, cmdParam, interactables)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public ViewInteractData(PointerEventData pointerEventData, string cmdName, InteractObject interactor, params InteractObject[] interactables)
            : base(cmdName, interactor, interactables)
        {
            this.pointerEventData = pointerEventData;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="cmdParam"></param>
        /// <param name="interactables"></param>
        public ViewInteractData(PointerEventData pointerEventData, string cmdName, InteractObject interactor, object cmdParam, params InteractObject[] interactables)
            : base(cmdName, interactor, cmdParam, interactables)
        {
            this.pointerEventData = pointerEventData;
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="interactData"></param>
        public override void CopyTo(InteractData interactData)
        {
            base.CopyTo(interactData);

            if (interactData is ViewInteractData viewInteractData)
            {
                viewInteractData.pointerEventData = pointerEventData;
            }
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="interactData"></param>
        protected override void CopyTo(ViewInteractData interactData)
        {
            if (interactData != null)
            {
                interactData.pointerEventData = pointerEventData;
            }
        }
    }

    #endregion

    #region 模型宿主

    /// <summary>
    /// 模型宿主
    /// </summary>
    public interface IModelHost : IModelEventListener
    {
        /// <summary>
        /// 当视图事件：界面事件回调
        /// </summary>
        /// <param name="viewInteractData"></param>
        void OnViewEvent(ViewInteractData viewInteractData);
    }

    #endregion 
}
