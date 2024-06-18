using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 动画控制器记录器
    /// </summary>
    public class AnimatorRecorder : Recorder<Animator, AnimatorRecorder.Info>
    {
        /// <summary>
        /// 记录信息类
        /// </summary>
        public class Info : ISingleRecord<Animator>
        {
            private Animator animator;

            private float speed;
            private string stateName;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="animator"></param>
            public void Record(Animator animator)
            {
                this.animator = animator;
                speed = animator.speed;
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                animator.speed = speed;
            }
        }
    }
}
