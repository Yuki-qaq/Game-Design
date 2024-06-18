using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.TimeLine;

namespace XCSJ.PluginSMS.States.Show.UI
{
    /// <summary>
    /// 步骤组信息界面
    /// </summary>
    [RequireManager(typeof(SMSManager))]
    public abstract class GUIStepGroupInfo : InteractProvider
    {
        /// <summary>
        /// 自动查找步骤组
        /// </summary>
        [Name("自动查找步骤组")]
        public bool autoFindStepGroup = true;

        /// <summary>
        /// 步骤组
        /// </summary>
        [Name("步骤组")]
        [Tip("为空时,自动查找正在运行的播放器对应的播放内容的步骤组", "When empty, automatically find the step group of playing content corresponding to the running player")]
        [StateComponentPopup(typeof(StepGroup), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(autoFindStepGroup), EValidityCheckType.Equal, true)]
        public StepGroup stepGroup = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            StepGroup.onStepChanged += OnStepChanged;

            TimeLinePlayer.onPlayerStateChanged += OnPlayerStateChanged;
            TimeLinePlayer.onPlayerPercentChanged += OnPlayerPercentChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            StepGroup.onStepChanged -= OnStepChanged;

            TimeLinePlayer.onPlayerStateChanged -= OnPlayerStateChanged;
            TimeLinePlayer.onPlayerPercentChanged -= OnPlayerPercentChanged;
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected virtual void Start()
        {
            OnStepChanged(stepGroup);
        }

        private void OnStepChanged(StepGroup stepGroup, Step oldStep, Step newStep)
        {
            // 自动适配当前播放器
            if (autoFindStepGroup)
            {
                OnStepChanged(StepGroupHelper.GetRoot(stepGroup) as StepGroup);
            }
            else if (this.stepGroup == stepGroup)
            {
                OnStepChanged(stepGroup);
            }
        }

        /// <summary>
        /// 当播放器状态已变更
        /// </summary>
        /// <param name="timeLinePlayer"></param>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        public void OnPlayerStateChanged(TimeLinePlayer timeLinePlayer, EPlayerState oldState, EPlayerState newState)
        {
            if (!autoFindStepGroup && !timeLinePlayer && !timeLinePlayer.playContent)
            {
                return;
            }

            switch (newState)
            {
                case EPlayerState.Play:
                    {
                        SetPlayer(timeLinePlayer);
                        OnStepChanged(stepGroupOfCurrentPlayer);
                        break;
                    }
                case EPlayerState.Finished:
                    {
                        currentPlayer = null;
                        stepGroupOfCurrentPlayer = null;
                        break;
                    }
            }
        }

        /// <summary>
        /// 当播放器百分比已变更
        /// </summary>
        /// <param name="player"></param>
        /// <param name="percent"></param>
        public void OnPlayerPercentChanged(TimeLinePlayer player, double percent)
        {
            if (!currentPlayer)
            {
                SetPlayer(player);
            }
            else if (currentPlayer != player)
            {
                return;
            }

            OnStepChanged(stepGroupOfCurrentPlayer);
        }

        private void SetPlayer(TimeLinePlayer player)
        {
            currentPlayer = player;
            stepGroupOfCurrentPlayer = currentPlayer.playContent.GetComponent<StepGroup>();
        }

        private TimeLinePlayer currentPlayer = null;
        private StepGroup stepGroupOfCurrentPlayer = null;

        /// <summary>
        /// 当步骤已变更
        /// </summary>
        /// <param name="group"></param>
        protected abstract void OnStepChanged(StepGroup group);
    }
}
