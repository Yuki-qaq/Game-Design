using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.GameObjects;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 游戏对象激活区间:游戏对象激活区间组件是游戏对象激活或者非激活的动画。组件在设定的时间区间内执行游戏对象的激活或非激活的命令，播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSCategory.ActionDirectory+ Title, typeof(SMSManager))]
    [Name(Title, nameof(GameObjectActiveRange))]
    [Tip("游戏对象激活区间组件是游戏对象激活或者非激活的动画。组件在设定的时间区间内执行游戏对象的激活或非激活的命令，播放完毕后，组件切换为完成态。", "The game object activation interval component is the active or inactive animation of the game object. The component executes the active or inactive command of the game object within the set time interval. After playing, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    [RequireComponent(typeof(GameObjectSet))]
    public class GameObjectActiveRange : RangeHandle<GameObjectActiveRange, GameObjectActiveRange.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "游戏对象激活区间";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.Action, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(GameObjectActiveRange))]
        [Tip("游戏对象激活区间组件是游戏对象激活或者非激活的动画。组件在设定的时间区间内执行游戏对象的激活或非激活的命令，播放完毕后，组件切换为完成态。", "The game object activation interval component is the active or inactive animation of the game object. The component executes the active or inactive command of the game object within the set time interval. After playing, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 游戏对象集
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        /// <summary>
        /// 记录器
        /// </summary>
        public class Recorder : GameObjectRecorder, IRangeHandleRecorder<GameObjectActiveRange>
        {
            /// <summary>
            /// 区间处理
            /// </summary>
            public GameObjectActiveRange rangeHandle;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="rangeHandle"></param>
            public void Record(GameObjectActiveRange rangeHandle)
            {
                this.rangeHandle = rangeHandle;
                if (!rangeHandle.gameObjectSet) return;
                _records.Clear();

                Record(rangeHandle.gameObjectSet.objects);
            }

            /// <summary>
            /// 设置百分比
            /// </summary>
            /// <param name="percent"></param>
            public void SetPercent(Percent percent)
            {
                if (percent.leftRange)
                {
                    SetRange(rangeHandle.leftRange);
                }
                else if (percent.rightRange)
                {
                    SetRange(rangeHandle.rightRange);
                }
                else
                {
                    SetRange(rangeHandle.inRange);
                }
            }

            /// <summary>
            /// 设置区间
            /// </summary>
            /// <param name="boolValue"></param>
            /// <param name="lifecycleEventLite"></param>
            public void SetRange(EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate)
            {
                foreach (var info in _records)
                {
                    info.SetActive(boolValue);
                }
            }
        }
    }
}
