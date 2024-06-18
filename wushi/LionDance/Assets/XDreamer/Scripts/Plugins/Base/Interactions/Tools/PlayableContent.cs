using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginMMO;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.Extension.Base.Interactions.Tools
{
    /// <summary>
    /// 可播放内容
    /// 1、定义动画数据、工作曲线、循环特征等数据，并计算和设置百分比
    /// 2、可作为播放内容进行被加载、被卸载，被播放、被停止、被暂停和被恢复
    /// 3、可执行加载内容、卸载内容，播放、停止、继续和恢复命令
    /// </summary>
    public abstract class PlayableContent : WorkClipInteractor, IPlayableContent, IContentPlayer
    {
        /// <summary>
        /// 启用时播放
        /// </summary>
        [Name("启用时播放")]
        public bool _playOnEnable = true;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            ResetPlayData();
            if (_playOnEnable)
            {
                Load(loadContentOnEnable, this as IPlayableContentHost, isLoad => Play());
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            Unload(loadedContent, this as IPlayableContentHost);

            base.OnDisable();
        }

        /// <summary>
        /// 如果 MonoBehaviour 已启用，则在每一帧都调用 Update
        /// </summary>
        protected virtual void Update()
        {
            if (isControlled) return;

            switch (playerState)
            {
                case EPlayerState.Play:
                case EPlayerState.Resume:
                    {
                        playerState = EPlayerState.Playing;
                        break;
                    }
                case EPlayerState.Playing:
                    {
                        _timeCounter += Time.deltaTime * speed;
                        percent = _timeCounter / timeLength;
                        break;
                    }
            }
        }

        #region 可播放内容功能  

        /// <summary>
        /// 当内容加载
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnLoad(PlayableData playableData) => EInteractResult.Success;

        /// <summary>
        /// 当内容卸载
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnUnload(PlayableData playableData) => EInteractResult.Success;

        /// <summary>
        /// 当播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnPlay(PlayableData playableData) => EInteractResult.Success;

        /// <summary>
        /// 当暂停
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnPause(PlayableData playableData) => EInteractResult.Success;

        /// <summary>
        /// 当恢复播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnResume(PlayableData playableData) => EInteractResult.Success;

        /// <summary>
        /// 当停止
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnStop(PlayableData playableData) => EInteractResult.Success;

        /// <summary>
        /// 播放进度
        /// </summary>
        public virtual double percent
        {
            get => percentData.percent01;
            set
            {
                SetPercentInternel(value);

                if (!isControlled)// 自主播放时外部UI会当前对象时间
                {
                    _timeCounter = timeLength * value;
                }
            }
        }

        private PlayableData defaultPlayableData
        {
            get
            {
                if (_defaultPlayableData == null) _defaultPlayableData = new PlayableData(this, this);
                return _defaultPlayableData;
            }
        }
        private PlayableData _defaultPlayableData = null;

        private void SetPercentInternel(double inPercent)
        {
            OnSetPercent(percentData.Update(inPercent), defaultPlayableData);

            if (isControlled) return;

            if (inPercent >= 0)
            {
                switch (loopType)
                {
                    case ELoopType.None:
                        {
                            if (percentData.percent >= 1 || MathX.Approximately(percentData.percent01, 1) || MathX.ApproximatelyZero(timeLength))
                            {
                                playerState = EPlayerState.Finished;
                                PlayFinish();
                            }
                            break;
                        }
                    case ELoopType.Loop:
                    case ELoopType.PingPong:
                        {
                            if (MathX.ApproximatelyZero(timeLength))
                            {
                                playerState = EPlayerState.Finished;
                                PlayFinish();
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 设置内容百分比回调
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="playableData"></param>
        public virtual void OnSetPercent(Percent percent, PlayableData playableData) 
        {
            // 当播放内容不等于自身时更新
            if (loadedContent!=null && loadedContent != (IPlayableContent)this)
            {
                loadedContent.percent = percent.percent01OfWorkCurve;
            }
        }

        /// <summary>
        /// 被播放器调用，处理越界发生时的百分比
        /// </summary>
        /// <param name="outOfBoundsMode">越界模式</param>
        /// <param name="percent">当前百分比</param>
        /// <param name="playableData">状态数据对象</param>
        /// <param name="lastPercent">上一次的百分比</param>
        public virtual void OnOutOfBounds(EOutOfBoundsMode outOfBoundsMode, double percent, PlayableData playableData, double lastPercent)
        {
            switch (outOfBoundsMode)
            {
                case EOutOfBoundsMode.Left:
                    {
                        SetPercentInternel(0);
                        break;
                    }
                case EOutOfBoundsMode.Right:
                    {
                        SetPercentInternel(1);
                        break;
                    }
            }
        }

        /// <summary>
        /// 播放完成:输出命令
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("播放完成")]
        public virtual void PlayFinish() => CallFinished(new PlayableData(EInteractResult.Success, GetOutCmdName(nameof(PlayFinish)), this, loadedContent, this, host));

        /// <summary>
        /// 当播放完成
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(PlayFinish))]
        public EInteractResult OnPlayFinish(PlayableData playableData)
        {
            return playerState == EPlayerState.Finished ? EInteractResult.Success : EInteractResult.Fail;
        }

        #endregion

        #region 播放器功能

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => interactData is PlayableData playableData && playableData.playableContent != null;

        /// <summary>
        /// 创建交互数据
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactables"></param>
        /// <returns></returns>
        protected override InteractData CreateInteractData(string cmdName, params InteractObject[] interactables)
        {
            return new PlayableData(cmdName, this, this, this);
        }

        /// <summary>
        /// 播放宿主：自播放时，对象为空
        /// </summary>
        public IPlayableContentHost host 
        {
            get => _host;
            set
            {
                // 不允许自己作为自己宿主
                if (this is IPlayableContentHost h && h == value) return;

                _host = value;
            }
        }
        private IPlayableContentHost _host;

        /// <summary>
        /// 内容播放器
        /// </summary>
        public IContentPlayer player => this;

        /// <summary>
        /// 宿主播放器
        /// </summary>
        public IContentPlayer hostPlayer => host as IContentPlayer;

        /// <summary>
        /// 外部控制播放
        /// </summary>
        public bool isControlled => host != null;

        /// <summary>
        /// 播放状态
        /// </summary>
        public virtual EPlayerState playerState
        {
            get => isControlled ? EPlayerState.BeganControlled : _playerState;
            protected set => _playerState = value;
        }

        /// <summary>
        /// 播放状态
        /// </summary>
        [Readonly]
        [Name("播放状态")]
        [EnumPopup]
        public EPlayerState _playerState = EPlayerState.None;

        /// <summary>
        /// 是否已加载内容
        /// </summary>
        public bool isLoaded => loadedContent != null;

        /// <summary>
        /// 正在加载内容
        /// </summary>
        public IPlayableContent loadingContent { get; private set; } = null;

        /// <summary>
        /// 已加载内容对象
        /// </summary>
        public virtual IPlayableContent loadedContent { get; protected set; } = null;

        /// <summary>
        /// 重置数据
        /// </summary>
        protected void ResetPlayData()
        {
            _timeCounter = 0;
            percent = 0;
        }

        private double _timeCounter = 0;

        /// <summary>
        /// 当前时间
        /// </summary>
        public virtual double time { get => _timeCounter; set => _timeCounter = value; }

        /// <summary>
        /// 播放速度
        /// </summary>
        public virtual double speed { get; set; } = 1;

        /// <summary>
        /// 缺省加载播放内容
        /// </summary>
        protected virtual IPlayableContent loadContentOnEnable => this;

        /// <summary>
        /// 是否播放中
        /// </summary>
        /// <returns></returns>
        public virtual bool IsPlaying() => hostPlayer != null ? hostPlayer.IsPlaying() : playerState == EPlayerState.Playing;

        /// <summary>
        /// 恢复或播放
        /// </summary>
        public virtual bool ResumeOrPlay() => playerState == EPlayerState.Pause ? Resume() : Play();

        /// <summary>
        /// 加载并播放
        /// </summary>
        /// <param name="playableContent"></param>
        /// <param name="host"></param>
        public void LoadAndPlay(IPlayableContent playableContent, IPlayableContentHost host = null)
        {
            Load(playableContent, host, isLoad => 
            {
                if (isLoad)
                {
                    Play();
                }
            });
        }

        /// <summary>
        /// 停止并卸载
        /// </summary>
        public void StopAndUnLoad(IPlayableContentHost host = null)
        {
            Stop();

            Unload(loadedContent, host);
        }

        /// <summary>
        /// 加载可播放内容
        /// </summary>
        /// <param name="playableContent"></param>
        /// <param name="host"></param>
        /// <param name="onLoad"></param>
        public void Load(IPlayableContent playableContent, IPlayableContentHost host = null, Action<bool> onLoad = null) =>
            TryInteract(nameof(Load), playableContent, host, onLoad, true);

        /// <summary>
        /// 卸载可播放内容
        /// </summary>
        /// <param name="playableContent"></param>
        /// <param name="host"></param>
        /// <param name="onUnload"></param>
        public void Unload(IPlayableContent playableContent, IPlayableContentHost host = null, Action<bool> onUnload = null) => TryInteract(nameof(Unload), playableContent, host, onUnload, false);

        /// <summary>
        /// 加载内容
        /// </summary>
        [InteractCmd]
        [Name("加载内容")]
        public void Load() => TryInteract(nameof(Load));

        /// <summary>
        /// 加载内容
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Load))]
        public EInteractResult Load(PlayableData playableData) 
        {
            if (!isLoaded)
            {
                playerState = EPlayerState.LoadContent;
                playContentPathOfNet = CommonFun.ObjectToString(playableData.playableContent as PlayableContent);

                loadingContent = playableData.playableContent;
                var rs = playableData.playableContent.OnLoad(playableData);
                switch (rs)
                {
                    case EInteractResult.Success:
                        {
                            loadingContent = null;
                            loadedContent = playableData.playableContent;
                            playableData.playableContent.host = playableData.playableContentHost;
                            playerState = EPlayerState.LoadedContent;
                            playableData.onLoad?.Invoke(true);
                            break;
                        }
                    case EInteractResult.Fail:
                        {
                            loadingContent = null;
                            playableData.onLoad?.Invoke(false);
                            break;
                        }
                }
                return rs;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 卸载内容
        /// </summary>
        [InteractCmd]
        [Name("卸载内容")]
        public void Unload() => Unload(loadedContent);

        /// <summary>
        /// 卸载内容
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Unload))]
        public EInteractResult Unload(PlayableData playableData)
        {
            if (isLoaded)
            {
                playerState = EPlayerState.UnloadContent;

                var rs = playableData.playableContent.OnUnload(playableData);
                switch (rs)
                {
                    case EInteractResult.Success:
                        {
                            loadedContent = null;
                            playableData.playableContent.host = null;
                            playerState = EPlayerState.UnloadedContent;
                            playableData.onUnload?.Invoke(true);
                            break;
                        }
                    case EInteractResult.Fail:
                        {
                            playableData.onUnload?.Invoke(false);
                            break;
                        }
                }
                return rs;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 播放
        /// </summary>
        [InteractCmd]
        [Name("播放")]
        public virtual bool Play() => TryInteract(nameof(Play), (loadedContent ?? loadingContent) ?? loadContentOnEnable);

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Play))]
        public EInteractResult Play(PlayableData playableData)
        {
            if(!MathX.ApproximatelyZero(timeLength) && !isControlled)
            {
                switch (playerState)
                {
                    case EPlayerState.LoadContent:
                        {
                            return EInteractResult.Wait;
                        }
                    case EPlayerState.None:
                    case EPlayerState.Free:
                    case EPlayerState.Stop:
                    case EPlayerState.Finished:
                    case EPlayerState.LoadedContent:
                        {
                            if (isLoaded)
                            {
                                playerState = EPlayerState.Play;
                                ResetPlayData();
                                return playableData.playableContent.OnPlay(playableData);
                            }
                            else
                            {
                                if (playerState != EPlayerState.LoadContent)
                                {
                                    Load(playableData.playableContent);
                                }
                                return EInteractResult.Wait;
                            }
                        }
                }
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        [InteractCmd]
        [Name("暂停")]
        public virtual bool Pause() => TryInteract(nameof(Pause), loadedContent);

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Pause))]
        public EInteractResult Pause(PlayableData playableData)
        {
            if (isLoaded && !isControlled && playerState == EPlayerState.Playing)
            {
                playerState = EPlayerState.Pause;

                return playableData.playableContent.OnPause(playableData);
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        [InteractCmd]
        [Name("恢复")]
        public virtual bool Resume() => TryInteract(nameof(Resume), loadedContent);

        /// <summary>
        /// 恢复播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Resume))]
        public EInteractResult Resume(PlayableData playableData)
        {
            if (isLoaded && !isControlled && playerState == EPlayerState.Pause)
            {
                playerState = EPlayerState.Resume;

                return playableData.playableContent.OnResume(playableData);
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 停止
        /// </summary>
        [InteractCmd]
        [Name("停止")]
        public virtual bool Stop() => TryInteract(nameof(Stop), loadedContent);

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Stop))]
        public EInteractResult Stop(PlayableData playableData)
        {
            if (isLoaded && !isControlled)
            {
                playerState = EPlayerState.Stop;

                var rs = playableData.playableContent.OnStop(playableData);
                if (rs == EInteractResult.Success)
                {
                    playerState = EPlayerState.Free;
                }
                return rs;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="playableContent"></param>
        /// <param name="host"></param>
        /// <param name="loadOrUnloadFun"></param>
        /// <param name="isLoad"></param>
        /// <returns></returns>
        protected bool TryInteract(string cmd, IPlayableContent playableContent, IPlayableContentHost host = null, Action<bool> loadOrUnloadFun = null, bool isLoad = true)
        {
            if (playableContent == null) return false;

            return TryInteract(new PlayableData(GetInCmdName(cmd), this, playableContent, this, host, loadOrUnloadFun, isLoad), out var result) && result == EInteractResult.Success;
        }

        #endregion

        #region MMO对象        

        /// <summary>
        /// 可播放内容网络数据
        /// </summary>
        [Group("MMO数据", textEN = "MMO Datas", defaultIsExpanded = false)]
        [Readonly]
        [Name("可播放内容网络数据")]
        public PlayableContentNetData _playableContentNetData = new PlayableContentNetData();

        /// <summary>
        /// 可播放内容网络数据
        /// </summary>
        [Serializable]
        [Import]
        public class PlayableContentNetData
        {
            /// <summary>
            /// 播放内容路径
            /// </summary>
            public string playContentPath = "";

            /// <summary>
            /// 播放状态
            /// </summary>
            public EPlayerState playerState = EPlayerState.None;

            /// <summary>
            /// 构造函数
            /// </summary>
            public PlayableContentNetData() { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="playContentPath"></param>
            /// <param name="playerState"></param>
            public PlayableContentNetData(string playContentPath, EPlayerState playerState) 
            {
                this.playContentPath = playContentPath;
                this.playerState = playerState;
            }

            /// <summary>
            /// 设置
            /// </summary>
            /// <param name="playableContentNetData"></param>
            public void Set(PlayableContentNetData playableContentNetData)
            {
                this.playContentPath = playableContentNetData.playContentPath;
                this.playerState = playableContentNetData.playerState;
            }

            /// <summary>
            /// 相等
            /// </summary>
            /// <param name="playableContentNetData"></param>
            /// <returns></returns>
            public bool Equals(PlayableContentNetData playableContentNetData)
            {
                return playContentPath == playableContentNetData.playContentPath && playerState == playableContentNetData.playerState;
            }
        }

        /// <summary>
        /// 上一次数据
        /// </summary>
        [Readonly]
        [Name("上一次数据")]
        public PlayableContentNetData _lastData = new PlayableContentNetData();

        /// <summary>
        /// 原始数据
        /// </summary>
        [Readonly]
        [Name("原始数据")]
        [EndGroup(true)]
        public PlayableContentNetData _originalData = new PlayableContentNetData();

        private string playContentPathOfNet = "";

        private bool IsNetSyncState(EPlayerState playerState)
        {
            switch (playerState)
            {
                case EPlayerState.LoadContent:
                case EPlayerState.Play:
                case EPlayerState.Playing:
                case EPlayerState.Stop:
                case EPlayerState.Pause:
                case EPlayerState.Resume:
                    return true;
            }
            return false;
        }

        private bool CanSerialize()
        {
            if (playContentPathOfNet != _lastData.playContentPath) return true;
            return IsNetSyncState(playerState) && (playerState != _lastData.playerState || playerState != _playableContentNetData.playerState);
        }

        /// <summary>
        /// 当序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool OnSerialize(out string data)
        {
            if (_dirty || CanSerialize())
            {
                _dirty = false;

                _playableContentNetData.playContentPath = playContentPathOfNet;
                _playableContentNetData.playerState = playerState;
                _lastData.Set(_playableContentNetData);

                data = JsonHelper.ToJson(_playableContentNetData);
                return true;
            }
            data = default;
            return false;
        }

        /// <summary>
        /// 当反序列化
        /// </summary>
        /// <param name="data"></param>
        public override void OnDeserialize(Data data)
        {
            version = data.version;

            if (data.IsLocalUserSended())
            {
                _lastData.Set(_playableContentNetData);
                return;
            }

            var netData = JsonHelper.ToObject<PlayableContentNetData>(data.data);
            if (netData != null)
            {
                if (netData.Equals(_playableContentNetData)) return;
                _playableContentNetData.Set(netData);
                var newContentPath = netData.playContentPath;

                switch (netData.playerState)
                {
                    case EPlayerState.LoadContent:
                        {
                            var newContent = CommonFun.StringToGameObjectComponent(newContentPath) as PlayableContent;
                            if (playContentPathOfNet != newContentPath)
                            {
                                Stop();
                                Unload();
                            }
                            Load(newContent);
                            break;
                        }
                    case EPlayerState.Play:
                        {
                            Play();
                            break;
                        }
                    case EPlayerState.Playing:
                        {
                            if (!isLoaded && playerState != EPlayerState.LoadContent)
                            {
                                LoadAndPlay(CommonFun.StringToGameObjectComponent(netData.playContentPath) as PlayableContent);
                            }
                            else
                            {
                                ResumeOrPlay();
                            }
                            break;
                        }
                    case EPlayerState.Stop:
                        {
                            Stop();
                            break;
                        }
                    case EPlayerState.Pause:
                        {
                            Pause();
                            break;
                        }
                    case EPlayerState.Resume:
                        {
                            Resume();
                            break;
                        }
                }

                playContentPathOfNet = newContentPath;
            }
        }

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            version = 0;

            _originalData.Set(_playableContentNetData);
            _lastData.Set(_playableContentNetData);
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            version = 0;

            _playableContentNetData.Set(_originalData);
        }      

        #endregion
    }
}
