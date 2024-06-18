using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXAR.Foundation.Base.Tools;

#if XDREAMER_AR_FOUNDATION
#endif

namespace XCSJ.PluginXAR.Foundation.Images.Tools
{
    /// <summary>
    /// 跟踪器事件
    /// </summary>
    [Name("跟踪器事件")]
    [Tool(XARFoundationHelper.Title, nameof(BaseTracker))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    public class TrackerEvent : BaseARMB, IDynamicLabel
    {
        /// <summary>
        /// 跟踪器
        /// </summary>
        [Name("跟踪器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public BaseTracker _tracker;

        /// <summary>
        /// 跟踪器
        /// </summary>
        public BaseTracker tracker => this.XGetComponentInParent(ref _tracker);
        
        /// <summary>
        /// 事件列表
        /// </summary>
        [Name("事件列表")]
        public List<TrackerCallbackEvent> _events = new List<TrackerCallbackEvent>();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            BaseTracker.onTrackerChanged += OnTrackerChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            BaseTracker.onTrackerChanged -= OnTrackerChanged;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!tracker) { }
        }

        private void OnTrackerChanged(BaseTracker tracker, ETrackEvent trackEvent)
        {
            if (tracker != this.tracker) return;

            Handle(tracker, trackEvent);
        }

        private void Handle(BaseTracker tracker, ETrackEvent trackEvent)
        {
            foreach (var e in _events)
            {
                e.InvokeTrackEvent(tracker, trackEvent);
            }
        }

        #region IDynamicLabel

        GUIContent IDynamicLabel.GetDynamicLabel(string propertyPath, FieldInfo fieldInfo, GUIContent label)
        {
            switch (fieldInfo.Name)
            {
                case nameof(_events):
                    {
                        if (PropertyPathHelper.TryGetLastArrayElementIndex(propertyPath, out var index) && index < _events.Count)
                        {
                            var c = CommonFun.NameTip(_events[index]._trackEvent);
                            return new GUIContent((index + 1).ToString() + "." + c.text, c.tooltip);
                        }
                        break;
                    }
            }
            return null;
        }

        #endregion

        /// <summary>
        /// 跟踪器回调事件
        /// </summary>
        [Serializable]
        public class TrackerCallbackEvent : UnityEvent<BaseTracker, ETrackEvent>
        {
            /// <summary>
            /// 跟踪事件
            /// </summary>
            [Name("跟踪事件")]
            [EnumPopup]
            public ETrackEvent _trackEvent = ETrackEvent.None;

            /// <summary>
            /// 调用跟踪事件
            /// </summary>
            /// <param name="tracker"></param>
            /// <param name="trackEvent"></param>
            public void InvokeTrackEvent(BaseTracker tracker, ETrackEvent trackEvent)
            {
                if (trackEvent != _trackEvent) return;
                Invoke(tracker, trackEvent);
            }
        }
    }
}
