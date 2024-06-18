using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginDataBase;
using XCSJ.PluginTools.Notes.Tips;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.ViewControllers;

namespace XCSJ.PluginXGUI.Widgets
{
    /// <summary>
    /// 提示弹出框
    /// </summary>
    [Name("提示弹出框")]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class TipPopup : View
    {
        /// <summary>
        /// 提示弹出时所在画布排序序号
        /// </summary>
        [Name("提示弹出时所在画布排序序号")]
        [Tip("提示弹出时，提示所在画布将被设定为当前值；值越大，画布渲染绘制越靠前", "When the prompt pops up, the canvas where the prompt is located will be set to the current value; The higher the value, the more forward the canvas rendering is drawn")]
        public int _canvasSortOrderIndexOnShow = 100;

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        public Text _description;

        /// <summary>
        /// 内容扩展宽度
        /// </summary>
        [Name("内容扩展宽度")]
        [Min(0)]
        public int _contentExtendWidth = 20;

        private CanvasGroup canvasGroup
        {
            get
            {
                if (!_canvasGroup)
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }
        private CanvasGroup _canvasGroup;

        #region Unity消息

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (canvasGroup) { }
            SetEffect(0);

            MB.onDisable += OnMBDisable;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            MB.onDisable -= OnMBDisable;
        }

        private void OnMBDisable(MB mb)
        {
            if (mb && mb == tip)
            {
                Hide(tip);
            }
        }

        #endregion

        #region 显示

        private int canvasSortOrderOnShow;

        /// <summary>
        /// 提示状态
        /// </summary>
        public ETipPopupState tipState
        {
            get => _tipState;
            private set
            {
                _tipState = value;

                switch (tipState)
                {
                    case ETipPopupState.BeginShow:
                        {
                            if (parentCanvas)
                            {
                                canvasSortOrderOnShow = parentCanvas.sortingOrder;
                                parentCanvas.sortingOrder = _canvasSortOrderIndexOnShow;// 临时提升画布的绘制顺序，保证提示在最前端
                            }
                            beginShow?.Invoke(this);
                            break;
                        }
                    case ETipPopupState.EndShow:
                        {
                            SetEffect(1);
                            endShow?.Invoke(this);
                            break;
                        }
                    case ETipPopupState.BeginHide:
                        {
                            beginHide?.Invoke(this); 
                            break;
                        }
                    case ETipPopupState.EndHide:
                        {
                            if (parentCanvas)
                            {
                                parentCanvas.sortingOrder = canvasSortOrderOnShow;
                            }
                            SetEffect(0);
                            endHide?.Invoke(this);
                            break;
                        }
                }
            }
        }
        private ETipPopupState _tipState = ETipPopupState.EndHide;

        /// <summary>
        /// 触发提示对象
        /// </summary>
        public Tip tip { get; private set; }

        private Vector3 showPosition = Vector3.zero;
        private Vector3 showDirection = Vector3.zero;

        /// <summary>
        /// 延时显示提示
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="delayShowTime"></param>
        /// <param name="description"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        public void DelayShow(Tip tip, float delayShowTime, string description, Vector2 position, Vector2 direction)
        {
            if (CheckShow(tip))
            {
                showPosition = position;
                showDirection= direction;

                if (_description && !string.IsNullOrEmpty(description))
                {
                    _description.text = description;
                    rectTransform.sizeDelta = new Vector2(_description.preferredWidth + _contentExtendWidth, rectTransform.sizeDelta.y);
                }

                tipState = ETipPopupState.DelayShow;

                Invoke(nameof(BeginShow), delayShowTime);
            }
        }

        private bool CheckShow(Tip tip)
        {
            // 如果为相同的触发器，并且上次状态将要处于或者已经处于显示状态了
            if (this.tip == tip && (tipState == ETipPopupState.DelayShow || tipState == ETipPopupState.BeginShow || tipState == ETipPopupState.EndShow))
            {
                return false;
            }

            // 如果上次在隐藏中，则先执行隐藏，完成结束态
            if (tipState == ETipPopupState.BeginHide)
            {
                tipState = ETipPopupState.EndHide;
            }

            // 设置初始进度
            if (this.tip == tip)
            {
                if (tipState == ETipPopupState.BeginHide)
                {
                    // 之前是隐藏态，为了连贯透明度，将前进度进行反转
                    currentPercent = 1 - currentPercent;
                }
            }
            else
            {
                currentPercent = 0;
            }

            StopEffect();
            this.tip = tip;

            SetEffect(0);

            rectTransform.SetAsLastSibling();// 设置为同层级最前端显示
            return true;
        }

        private void BeginShow()
        {
            tipState = ETipPopupState.BeginShow;

            UpdatePosition();

            StartEffect();
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        public void UpdatePosition()
        {
            if (tip)
            {
                // 将提示对象保持在屏幕范围内
                var halfSize = rectTransform.sizeDelta / 2;
                showPosition.x = Mathf.Clamp(showPosition.x, halfSize.x, Screen.width - halfSize.x);
                showPosition.y = Mathf.Clamp(showPosition.y, halfSize.y, Screen.height - halfSize.y);

                rectTransform.position = (Vector2)showPosition + Vector2.Scale(showDirection, halfSize);
            }
        }

        /// <summary>
        /// 开始显示
        /// </summary>
        public static event Action<TipPopup> beginShow;

        /// <summary>
        /// 结束显示
        /// </summary>
        public static event Action<TipPopup> endShow;

        #endregion

        #region 隐藏

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="delayHideTime"></param>
        public void Hide(Tip tip, float delayHideTime = 0.3f)
        {
            // 如果为相同的触发器，并且上次状态将要处于或者已经处于隐藏状态了
            if ((this.tip == tip || !this.tip) 
                && (tipState == ETipPopupState.DelayHide || tipState == ETipPopupState.BeginHide || tipState == ETipPopupState.EndHide))
            {
                return;
            }

            // 如果上次在显示中，则先执行显示，完成结束态
            if (tipState == ETipPopupState.BeginShow)
            {
                tipState = ETipPopupState.EndShow;
            }

            // 设置初始进度
            if (this.tip == tip)
            {
                if (tipState == ETipPopupState.BeginShow)
                {
                    // 之前是显示态，为了连贯透明度，将前进度进行反转
                    currentPercent = 1 - currentPercent;
                }
            }
            else
            {
                currentPercent = 0;
            }

            this.tip = tip;

            StopEffect();

            DelayHide(delayHideTime);
        }

        private void DelayHide(float delayHideTime)
        {
            tipState = ETipPopupState.DelayHide;
            Invoke(nameof(BeginHide), delayHideTime);
        }

        private void BeginHide()
        {
            tipState = ETipPopupState.BeginHide;

            StartEffect();
        }

        /// <summary>
        /// 开始隐藏
        /// </summary>
        public static event Action<TipPopup> beginHide;

        /// <summary>
        /// 结束隐藏
        /// </summary>
        public static event Action<TipPopup> endHide;

        #endregion

        #region 显示或隐藏效果

        /// <summary>
        /// 效果规则
        /// </summary>
        [Group("弹出设置", textEN = "Popup Settings")]
        [Name("效果规则")]
        [EnumPopup]
        public EEffectRule _effectRule = EEffectRule.Scale;

        /// <summary>
        /// 效果规则
        /// </summary>
        public enum EEffectRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 缩放
            /// </summary>
            [Name("缩放")]
            Scale,

            /// <summary>
            /// 透明
            /// </summary>
            [Name("透明")]
            Transparent,
        }

        /// <summary>
        /// 显示效果时间
        /// </summary>
        [Name("显示效果时间")]
        [Range(0, 3)]
        public float _showEffectTime = 0.3f;

        /// <summary>
        /// 隐藏效果时间
        /// </summary>
        [Name("隐藏效果时间")]
        [Range(0, 3)]
        public float _hideEffectTime = 0.1f;

        private void StartEffect()
        {
            if (!gameObject.activeSelf) gameObject.XSetActive(true);

            StartCoroutine(nameof(DoEffect));
        }

        private void StopEffect()
        {
            CancelInvoke();
            StopAllCoroutines();
        }

        private float currentPercent = 0;

        private float effectMaxTime
        {
            get
            {
                if (tipState == ETipPopupState.BeginShow)
                {
                    return _showEffectTime;
                }
                else if (tipState == ETipPopupState.BeginHide)
                {
                    return _hideEffectTime;
                }
                return 1;
            }
        }

        private IEnumerator DoEffect()
        {
            while (currentPercent < 1)
            {
                currentPercent += Time.deltaTime/ effectMaxTime;

                var percent = Mathf.Clamp(currentPercent, 0, 1);
                if (tipState == ETipPopupState.BeginHide) percent = 1 - percent;

                SetEffect(percent);
                yield return null;
            }

            FinishEffect();

            yield return null;
        }

        private void FinishEffect()
        {
            currentPercent = 0;

            if (tipState == ETipPopupState.BeginShow)
            {
                tipState = ETipPopupState.EndShow;
            }
            else if (tipState == ETipPopupState.BeginHide)
            {
                tipState = ETipPopupState.EndHide;
                tip = null;
            }
        }

        private void SetEffect(float percent)
        {
            switch (_effectRule)
            {
                case EEffectRule.Scale: SetScale(percent); break; 
                case EEffectRule.Transparent: SetAlpha(percent); break; 
            }
        }

        private void SetScale(float scale)
        {
            rectTransform.localScale = Vector3.one * scale;
        }

        private void SetAlpha(float alpha)
        {
            if(_canvasGroup) _canvasGroup.alpha = alpha;
        }

        #endregion
    }

    /// <summary>
    /// 弹出提示资产
    /// </summary>
    [Serializable]
    public class TipPopupAsset : XGUIAsset<TipPopup> { }

    /// <summary>
    /// 提示弹出框状态
    /// </summary>
    public enum ETipPopupState
    {
        /// <summary>
        /// 延迟显示
        /// </summary>
        [Name("延迟显示")]
        DelayShow,

        /// <summary>
        /// 开始显示
        /// </summary>
        [Name("开始显示")]
        BeginShow,

        /// <summary>
        /// 结束显示
        /// </summary>
        [Name("结束显示")]
        EndShow,

        /// <summary>
        /// 延迟隐藏
        /// </summary>
        [Name("延迟隐藏")]
        DelayHide,

        /// <summary>
        /// 开始隐藏
        /// </summary>
        [Name("开始隐藏")]
        BeginHide,

        /// <summary>
        /// 结束隐藏
        /// </summary>
        [Name("结束隐藏")]
        EndHide,
    }
}
