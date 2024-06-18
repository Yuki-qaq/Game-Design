using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States.GameObjects;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 渲染器区间处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [RequireComponent(typeof(GameObjectSet))]
    public abstract class RendererRangeHandle<T> : RangeHandle<T, RendererRangeHandle<T>.Recorder>, IRendererRangeHandle
        where T : RendererRangeHandle<T>
    {
        /// <summary>
        /// 操作对象
        /// </summary>
        [Name("操作对象")]
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        /// <summary>
        /// 包含成员
        /// </summary>
        [Name("包含成员")]
        public bool includeChildren = true;

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="recorder"></param>
        /// <param name="percent"></param>
        protected abstract void OnSetPercent(Recorder recorder, Percent percent);

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="recorder"></param>
        /// <param name="boolValue"></param>
        /// <param name="lifecycleEventLite"></param>
        protected abstract void OnSetPercent(Recorder recorder, EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate);

        /// <summary>
        /// 记录器
        /// </summary>
        public class Recorder : RendererRecorder, IRangeHandleRecorder<T>
        {
            /// <summary>
            /// 区间处理
            /// </summary>
            public T rangeHandle;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="rangeHandle"></param>
            public void Record(T rangeHandle)
            {
                this.rangeHandle = rangeHandle;
                if (!rangeHandle.gameObjectSet) return;
                _records.Clear();

                foreach (var go in rangeHandle.gameObjectSet.objects)
                {
                    if (go)
                    {
                        Record(go);
                        if (rangeHandle.includeChildren) Record(go.GetComponentsInChildren<Renderer>());
                    }
                }
            }

            /// <summary>
            /// 设置百分比
            /// </summary>
            /// <param name="percent"></param>
            public void SetPercent(Percent percent)
            {
                rangeHandle.OnSetPercent(this, percent);
            }

            /// <summary>
            /// 设置区间
            /// </summary>
            /// <param name="boolValue"></param>
            /// <param name="lifecycleEventLite"></param>
            public void SetRange(EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate)
            {
                rangeHandle.OnSetPercent(this, boolValue, lifecycleEventLite);
            }
        }
    }

    /// <summary>
    /// 渲染器区间处理
    /// </summary>
    public interface IRendererRangeHandle : IRangeHandle { }

    /// <summary>
    /// 渲染器区间处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class RendererRangeHandle<T, TValue> : RendererRangeHandle<T>
       where T : RendererRangeHandle<T, TValue>
    {
        /// <summary>
        /// 进入时值
        /// </summary>
        [Name("进入时值")]
        [HideInSuperInspector(nameof(onEntry), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(onEntry), EValidityCheckType.Equal, EBool.None)]
        public TValue onEntryValue = default;

        /// <summary>
        /// 区间左值
        /// </summary>
        [Name("区间左值")]
        [HideInSuperInspector(nameof(leftRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(leftRange), EValidityCheckType.Equal, EBool.None)]
        public TValue leftValue = default;

        /// <summary>
        /// 区间内值
        /// </summary>
        [Name("区间内值")]
        [HideInSuperInspector(nameof(inRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(inRange), EValidityCheckType.Equal, EBool.None)]
        public TValue inValue = default;

        /// <summary>
        /// 区间右值
        /// </summary>
        [Name("区间右值")]
        [HideInSuperInspector(nameof(rightRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(rightRange), EValidityCheckType.Equal, EBool.None)]
        public TValue rightValue = default;

        /// <summary>
        /// 退出时值
        /// </summary>
        [Name("退出时值")]
        [HideInSuperInspector(nameof(onExit), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(onExit), EValidityCheckType.Equal, EBool.None)]
        public TValue onExitValue = default;

        /// <summary>
        /// 当进入切换标记
        /// </summary>
        protected bool onEntrySwitchFlag = false;

        /// <summary>
        /// 左切换标记
        /// </summary>
        protected bool leftSwitchFlag = false;

        /// <summary>
        /// 内切换标记
        /// </summary>
        protected bool inSwitchFlag = false;

        /// <summary>
        /// 右切换标记
        /// </summary>
        protected bool rightSwitchFlag = false;

        /// <summary>
        /// 当退出切换标记
        /// </summary>
        protected bool onExitSwitchFlag = false;

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="recorder"></param>
        /// <param name="percent"></param>
        protected override void OnSetPercent(Recorder recorder, Percent percent)
        {
            if (percent.leftRange)
            {
                OnSetValue(recorder, leftRange, ref leftValue, ref leftSwitchFlag);
            }
            else if (percent.rightRange)
            {
                OnSetValue(recorder, rightRange, ref rightValue, ref rightSwitchFlag);
            }
            else
            {
                OnSetValue(recorder, inRange, ref inValue, ref inSwitchFlag);
            }
        }

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="recorder"></param>
        /// <param name="boolValue"></param>
        /// <param name="lifecycleEventLite"></param>
        protected override void OnSetPercent(Recorder recorder, EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate)
        {
            switch (lifecycleEventLite)
            {
                case ELifecycleEventLite.OnEntry:
                    {
                        OnSetValue(recorder, onEntry, ref onEntryValue, ref onEntrySwitchFlag);
                        break;
                    }
                case ELifecycleEventLite.OnExit:
                    {
                        OnSetValue(recorder, onExit, ref onExitValue, ref onExitSwitchFlag);
                        break;
                    }
            }
        }

        /// <summary>
        /// 当设置值
        /// </summary>
        /// <param name="recorder"></param>
        /// <param name="boolValue"></param>
        /// <param name="value"></param>
        /// <param name="switchFlag"></param>
        protected abstract void OnSetValue(Recorder recorder, EBool boolValue, ref TValue value, ref bool switchFlag);
    }
}
