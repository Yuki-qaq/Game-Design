using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 游戏对象闪烁:游戏对象闪烁组件是游戏对象闪烁的动画。游戏对象在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(GameObjectFlash))]
    [Tip("游戏对象闪烁组件是游戏对象闪烁的动画。游戏对象在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。", "The GameObject blinking component is the animation of the GameObject blinking. The game object flashes at the set frequency within the set time interval. After playing, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Flash)]
    [RequireComponent(typeof(GameObjectSet))]
    public class GameObjectFlash : Flash<GameObjectFlash, GameObjectFlash.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "游戏对象闪烁";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.Action, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(GameObjectFlash))]
        [XCSJ.Attributes.Icon(EIcon.Flash)]
        [Tip("游戏对象闪烁组件是游戏对象闪烁的动画。游戏对象在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。", "The GameObject blinking component is the animation of the GameObject blinking. The game object flashes at the set frequency within the set time interval. After playing, the component switches to the completed state.")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 游戏对象集
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        /// <summary>
        /// 记录器
        /// </summary>
        public class Recorder : GameObjectRecorder, IPercentRecorder<GameObjectFlash>
        {
            /// <summary>
            /// 闪烁
            /// </summary>
            public GameObjectFlash flash;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="flash"></param>
            public void Record(GameObjectFlash flash)
            {
                this.flash = flash;
                if (!flash.gameObjectSet) return;
                _records.Clear();
                Record(flash.gameObjectSet.objects);
            }

            /// <summary>
            /// 设置百分比
            /// </summary>
            /// <param name="percent"></param>
            public void SetPercent(Percent percent)
            {
                foreach (var info in _records)
                {
                    info.gameObject.SetActive(!flash.inChangeArea);
                }
            }
        }
    }
}
