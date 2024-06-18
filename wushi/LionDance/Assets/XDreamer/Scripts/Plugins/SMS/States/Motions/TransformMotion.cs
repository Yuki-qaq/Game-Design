using UnityEngine;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 变换动作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [RequireComponent(typeof(GameObjectSet))]
    public abstract class TransformMotion<T> : Motion<T, TransformMotion<T>.Recorder> where T : TransformMotion<T>
    {
        /// <summary>
        /// 游戏对象集
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="info"></param>
        /// <param name="percent"></param>
        protected abstract void SetPercent(TransformRecorder.Info info, Percent percent);

        /// <summary>
        /// 记录器
        /// </summary>
        public class Recorder : TransformRecorder, IPercentRecorder<T>
        {
            private T motion;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="motion"></param>
            public void Record(T motion)
            {
                this.motion = motion;
                if (!motion.gameObjectSet) return;
                _records.Clear();
                Record(motion.gameObjectSet.objects);
            }

            /// <summary>
            /// 设置百分比
            /// </summary>
            /// <param name="percent"></param>
            public void SetPercent(Percent percent)
            {
                try
                {
                    foreach (var i in _records)
                    {
                        motion.SetPercent(i, percent);
                    }
                }
                catch { }
            }
        }
    }
}
