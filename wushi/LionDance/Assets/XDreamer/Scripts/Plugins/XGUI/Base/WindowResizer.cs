using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Inputs;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 窗口尺寸修改器
    /// </summary>
    [Name("窗口尺寸修改器")]
    public class WindowResizer : DraggableView, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// 控制窗口
        /// </summary>
        [Name("控制窗口")]
        [Readonly(EEditorMode.Runtime)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public SubWindow _subWindow;

        private Rect _screenRect = new Rect(0, 0, 0, 0);
        private Image image = null;

        /// <summary>
        /// 控制规则
        /// </summary>
        public enum EControlRule
        {
            /// <summary>
            /// 水平
            /// </summary>
            [Name("水平")]
            HHorizontal,

            /// <summary>
            /// 垂直
            /// </summary>
            [Name("垂直")]
            Vertical,

            /// <summary>
            /// 同时：同时控制水平和垂直
            /// </summary>
            [Name("同时")]
            [Tip("同时控制水平和垂直", "Control both horizontal and vertical simultaneously")]
            Both,

            /// <summary>
            /// 等比：尺寸变换与源窗口比例一致
            /// </summary>
            [Name("等比")]
            [Tip("尺寸变换与源窗口比例一致", "The size transformation is consistent with the proportion of the source window")]
            EqualRatio,
        }

        /// <summary>
        /// 控制规则
        /// </summary>
        [Name("控制规则")]
        [EnumPopup]
        public EControlRule _controlRule = EControlRule.Both;

        /// <summary>
        /// 控制规则
        /// </summary>
        public EControlRule controlRule { get => _controlRule; set => _controlRule = value; }

        /// <summary>
        /// 显示规则
        /// </summary>
        public enum EShowRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 移入显示移出隐藏
            /// </summary>
            [Name("移入显示移出隐藏")]
            EnterShow_ExitHide,
        }

        /// <summary>
        /// 显示规则
        /// </summary>
        [Name("显示规则")]
        [EnumPopup]
        public EShowRule _showRule = EShowRule.EnterShow_ExitHide;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            image = GetComponent<Image>();

            OnPointerExit(null); // 初始化
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            _screenRect.width = Screen.width;
            _screenRect.height = Screen.height;

            SetWindowLeftTopPivot();
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDrag(PointerEventData eventData)
        {
            if (!_subWindow || _subWindow.maxSize || _subWindow.fullScreen) return;

            var offset = new Vector2(eventData.delta.x, -eventData.delta.y);// rectTransform坐标系是左下为原点
            var windowRect = _subWindow.rectTransform.GetScreenRect();
            windowRect.size += offset;

            offset = GetValidOffset(windowRect, offset);

            offset = GetOffsetByRule(_controlRule, offset);

            _subWindow.size += offset;

            _subWindow.CallSizeChanged();
        }

        private Vector2 GetValidOffset(Rect windowRect, Vector2 offset)
        {
            if (!IsValidWith(windowRect))
            {
                offset.x = 0;
            }
            if (!IsValidHeight(windowRect))
            {
                offset.y = 0;
            }
            return offset;
        }

        private Vector2 GetOffsetByRule(EControlRule controlRule, Vector2 offset)
        {
            switch (controlRule)
            {
                case EControlRule.HHorizontal:
                    {
                        offset.y = 0;
                        break;
                    }
                case EControlRule.Vertical:
                    {
                        offset.x = 0;
                        break;
                    }
                case EControlRule.Both:
                    {
                        // 按下shift等于等比例缩放
                        if (XInput.GetKey(KeyCode.LeftShift) || XInput.GetKey(KeyCode.RightShift))
                        {
                            return GetOffsetByRule(EControlRule.EqualRatio, offset);
                        }
                        break;
                    }
                case EControlRule.EqualRatio:
                    {
                        offset = new Vector2(1, _subWindow.size.y / _subWindow.size.x) * (Mathf.Abs(offset.x) <
                        Mathf.Abs(offset.y) ? offset.x : offset.y);
                        break;
                    }
            }
            return offset;
        }

        /// <summary>
        /// 有效宽度：宽度不能小于窗口最小尺寸宽度且不能超过屏幕宽度
        /// </summary>
        /// <param name="windowRect"></param>
        /// <returns></returns>
        private bool IsValidWith(Rect windowRect) => windowRect.width > _subWindow._minSize.x && (windowRect.x + windowRect.width) < _screenRect.width;

        /// <summary>
        /// 有效高度：高度不能小于窗口最小尺寸高度且不能超过屏幕高度
        /// </summary>
        /// <param name="windowRect"></param>
        /// <returns></returns>
        private bool IsValidHeight(Rect windowRect) => windowRect.height > _subWindow._minSize.y && (windowRect.y - windowRect.height) > 0;

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            RecoverWindowPivot();
            _subWindow.CallSizeChanged();
        }

        /// <summary>
        /// 当指针进入
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_showRule == EShowRule.EnterShow_ExitHide && image)
            {
                var color = image.color;
                color.a = 1;
                image.color = color;
            }
        }

        /// <summary>
        /// 当指针退出
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (_showRule == EShowRule.EnterShow_ExitHide && image)
            {
                var color = image.color;
                color.a = 0;
                image.color = color;
            }
        }

        #region 窗口中心和位置设定

        private Vector2 orgPivot = Vector2.zero;
        private Vector3 pivotOffset = Vector2.zero;

        private void SetWindowLeftTopPivot()
        {
            if (_subWindow)
            {
                // 拖动窗口前，将窗口的中心设置为左上角
                orgPivot = _subWindow.rectTransform.pivot;
                _subWindow.rectTransform.pivot = new Vector2(0, 1);
                pivotOffset = orgPivot - _subWindow.rectTransform.pivot;

                // 因修改了中心点，所以需要重新设置位置
                _subWindow.rectTransform.position -= (Vector3)(pivotOffset * _subWindow.rectTransform.sizeDelta);
            }
        }

        private void RecoverWindowPivot()
        {
            if (_subWindow)
            {
                // 因修改了中心点，所以需要重新设置位置
                _subWindow.rectTransform.position += (Vector3)(pivotOffset * _subWindow.rectTransform.sizeDelta);
                _subWindow.rectTransform.pivot = orgPivot;
            }
        }
        #endregion
    }
}
