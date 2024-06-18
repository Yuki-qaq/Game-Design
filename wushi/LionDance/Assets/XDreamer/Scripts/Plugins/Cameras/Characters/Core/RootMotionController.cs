using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginCameras.Characters
{
    /// <summary>
    /// 根运动控制器：用于获取“Animator”根运动速度向量（animVelocity）的帮助程序组件。该组件必须附加到具有“Animator”组件的游戏对象。
    /// </summary>
    [Name("根运动控制器")]
    [Tip("用于获取“Animator”根运动速度向量（animVelocity）的帮助程序组件。该组件必须附加到具有“Animator”组件的游戏对象。", "Helper component to get 'Animator' root-motion velocity vector (animVelocity).This must be attached to a game object with an 'Animator' component.")]
    [RequireComponent(typeof(Animator))]
    public sealed class RootMotionController : BaseCharacterMB
    {
        #region 字段

        /// <summary>
        /// 动画器
        /// </summary>
        public Animator animator
        {
            get
            {
                
                return _animator;
            }
        }
        private Animator _animator;

        #endregion

        #region 属性

        /// <summary>
        /// The animation velocity vector.
        /// </summary>

        public Vector3 animVelocity { get; private set; }

        #endregion

        #region MB方法

        /// <summary>
        /// 唤醒
        /// </summary>
        public void Awake()
        {
            if (!_animator)
            {
                _animator = GetComponent<Animator>();

                if (_animator == null)
                {
                    Debug.LogError(
                        string.Format(
                            "RootMotionController: There is no 'Animator' attached to the '{0}' game object.\n" +
                            "Please attach a 'Animator' to the '{0}' game object",
                            name));
                }
            }
        }

        /// <summary>
        /// 当动画器移动
        /// </summary>
        public void OnAnimatorMove()
        {
            // Compute movement velocity from animation

            var deltaTime = Time.deltaTime;
            if (deltaTime > 0.0f && animator)
                animVelocity = animator.deltaPosition / deltaTime;
        }

        #endregion
    }
}