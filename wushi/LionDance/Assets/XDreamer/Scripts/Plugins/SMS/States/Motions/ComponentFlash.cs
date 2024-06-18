using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Components;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 组件闪烁:组件闪烁组件是组件闪烁的动画。组件在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(ComponentFlash))]
    [Tip("组件闪烁组件是组件闪烁的动画。组件在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。", "Component blinking component is the animation of component blinking. The component flashes at the set frequency within the set time interval. After playing, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Component)]
    [RequireComponent(typeof(ComponentSet))]
    public class ComponentFlash : Flash<ComponentFlash, ComponentFlash.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "组件闪烁";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.Action, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(ComponentFlash))]
        [Tip("组件闪烁组件是组件闪烁的动画。组件在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。", "Component blinking component is the animation of component blinking. The component flashes at the set frequency within the set time interval. After playing, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EIcon.Component)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 组件集
        /// </summary>
        public ComponentSet componentSet => GetComponent<ComponentSet>(true);

        /// <summary>
        /// 记录器
        /// </summary>
        public class Recorder : ComponentRecorder, IPercentRecorder<ComponentFlash>
        {
            /// <summary>
            /// 闪烁
            /// </summary>
            public ComponentFlash flash;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="flash"></param>
            public void Record(ComponentFlash flash)
            {
                this.flash = flash;
                if (!flash.componentSet) return;
                _records.Clear();
                Record(flash.componentSet.objects);
            }

            /// <summary>
            /// 设置百分
            /// </summary>
            /// <param name="percent"></param>
            public void SetPercent(Percent percent)
            {
                foreach (var info in _records)
                {
                    info.component.XSetEnable(!flash.inChangeArea);
                }
            }
        }
    }
}
