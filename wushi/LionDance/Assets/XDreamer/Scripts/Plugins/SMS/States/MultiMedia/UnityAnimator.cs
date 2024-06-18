using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.MultiMedia
{
    /// <summary>
    /// Unity动画:Unity动画组件是播放Unity的Animator动画的对象。可设置播放整个动画的中间一段区间。播放完毕后，组件切换为完成态。
    /// </summary>
    [Name(Title, nameof(UnityAnimator))]
    [Tip("Unity动画组件是播放Unity的Animator动画的对象。可设置播放整个动画的中间一段区间。播放完毕后，组件切换为完成态。", "The Unity animation component is an object that plays Unity's Animator animation. You can set the middle interval for playing the entire animation. After playing, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Animator)]
    [ComponentMenu(SMSCategory.MultiMediaDirectory + Title, typeof(SMSManager))]
    public class UnityAnimator : WorkClip<UnityAnimator>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Unity动画";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.MultiMedia, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.MultiMediaDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(UnityAnimator))]
        [Tip("Unity动画组件是播放Unity的Animator动画的对象。可设置播放整个动画的中间一段区间。播放完毕后，组件切换为完成态。", "The Unity animation component is an object that plays Unity's Animator animation. You can set the middle interval for playing the entire animation. After playing, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// Animator
        /// </summary>
        [Group("Animator属性")]
        [Name("Animator")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(Animator))]
        public Animator _animator = null;

        /// <summary>
        /// 层索引
        /// </summary>
        [Name("层索引")]
        public int _layerIndex = 0;

        /// <summary>
        /// 状态名称
        /// </summary>
        [Name("状态名称")]
        public string _stateName = "Take 001";

        /// <summary>
        /// 退出处理规则
        /// </summary>
        public enum EExitHandleRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 恢复状态
            /// </summary>
            [Name("恢复状态")]
            RecoverState,

            /// <summary>
            /// 播放状态
            /// </summary>
            [Name("播放状态")]
            PlayState,
        }

        /// <summary>
        /// 退出处理规则
        /// </summary>
        [Name("退出处理规则")]
        [EnumPopup]
        public EExitHandleRule _exitHandleRule = EExitHandleRule.None;

        /// <summary>
        /// Animator的状态名称列表
        /// </summary>
        [HideInSuperInspector]
        public List<string> _stateNames = new List<string>();
        private string lastPlayStateName = "";

        /// <summary>
        /// 退出后播放状态名称
        /// </summary>
        [Name("退出后播放状态名称")]
        [HideInSuperInspector(nameof(_exitHandleRule), EValidityCheckType.NotEqual, EExitHandleRule.PlayState)]
        public string _playStateNameOnExit = "Take 001";

        /// <summary>
        /// 播放模式
        /// </summary>
        public enum EPlayMode
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 全部
            /// </summary>
            [Name("全部")]
            Whole,

            /// <summary>
            /// 区间
            /// </summary>
            [Name("区间")]
            Range,
        }

        /// <summary>
        /// 播放模式
        /// </summary>
        [Name("播放模式")]
        [EnumPopup]
        public EPlayMode _playMode = EPlayMode.Whole;

        /// <summary>
        /// Take区间
        /// </summary>
        [Name("Take区间")]
        [Tip("状态关联的动画(动作)在完整动画(FBX)上的帧区间;", "The frame interval of the animation (action) associated with the state on the complete animation (FBX);")]
        [HideInSuperInspector(nameof(_playMode), EValidityCheckType.NotEqual, EPlayMode.Range)]
        public Vector2Int _takeRange = new Vector2Int();

        /// <summary>
        /// 播放区间
        /// </summary>
        [Name("播放区间")]
        [HideInSuperInspector(nameof(_playMode), EValidityCheckType.NotEqual, EPlayMode.Range)]
        public Vector2Int _playRange = new Vector2Int();

        /// <summary>
        /// 帧总数量
        /// </summary>
        public int takeFrameCount => _takeRange.y - _takeRange.x;

        /// <summary>
        /// 播放帧数量
        /// </summary>
        public int playFrameCount => _playRange.y - _playRange.x;

        /// <summary>
        /// 播放帧起始量与总帧起始量插值
        /// </summary>
        public int takeToPlayFrameCount => _playRange.x - _takeRange.x;

        private AnimatorRecorder animatorRecorder = new AnimatorRecorder();

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="data"></param>
        public override void Reset(ResetData data)
        {
            base.Reset(data);
            switch (data.dataRule)
            {
                case EDataRule.Init:
                case EDataRule.Entry:
                    {
                        SetPercent(0);
                        break;
                    }
            }
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            if (_animator)
            {
                animatorRecorder.Clear();
                animatorRecorder.Record(_animator);

                lastPlayStateName = "";
                var state = _animator.GetCurrentAnimatorStateInfo(_layerIndex);
                foreach (var item in _stateNames)
                {
                    if (state.IsName(item))
                    {
                        lastPlayStateName = item;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            if (_animator)
            {
                switch (_exitHandleRule)
                {
                    case EExitHandleRule.RecoverState:
                        {
                            animatorRecorder.Recover();
                            _animator.Play(lastPlayStateName, _layerIndex);
                            break;
                        }
                    case EExitHandleRule.PlayState:
                        {
                            animatorRecorder.Recover();
                            _animator.Play(_playStateNameOnExit, _layerIndex);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="percent"></param>
        protected void PlayAnimator(double percent)
        {
            try
            {
                if (!_animator) return;
                _animator.speed = 0;
                _animator.Play(_stateName, _layerIndex, (float)percent);
            }
            catch (Exception ex)
            {
                LogException(this, nameof(PlayAnimator), ex);
            }
        }

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            switch (_playMode)
            {
                case EPlayMode.Whole:
                    {
                        PlayAnimator(percent.percent01OfWorkCurve);
                        break;
                    }
                case EPlayMode.Range:
                    {
                        PlayAnimator(PlayToTake(percent.percent01OfWorkCurve));
                        break;
                    }
            }
        }
       
        /// <summary>
        /// 数据有效
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _animator;

        /// <summary>
        /// 将播放区间百分比转化Take区间百分比
        /// </summary>
        /// <param name="percent">播放区间百分比</param>
        /// <returns>转化后的Take区间百分比</returns>
        private double PlayToTake(double percent)
        {
            var takeFrameCount = this.takeFrameCount;
            return takeFrameCount == 0 ? 0 : (percent * playFrameCount + takeToPlayFrameCount) / takeFrameCount;
        }

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_playMode)
            {
                case EPlayMode.Range:
                    {
                        return base.ToFriendlyString() + _playRange.ToString();
                    }
            }
            return _stateName;
        }
    }
}
