using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Windows;

namespace XCSJ.PluginXGUI.Widgets
{
    /// <summary>
    /// 对话框
    /// </summary>
    [Name("对话框")]
    [RequireComponent(typeof(UGUIWindow))]
    public class DialogBox : View
    {
        /// <summary>
        /// 交互后处理规则
        /// </summary>
        public enum EAfterInteractRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 隐藏
            /// </summary>
            [Name("隐藏")]
            Hide,
        }

        /// <summary>
        /// 交互后处理规则
        /// </summary>
        [Group("对话框设置", textEN = "DialogBox Settings")]
        [Name("交互后处理规则")]
        [EnumPopup]
        public EAfterInteractRule _afterInteractRule = EAfterInteractRule.Hide;

        /// <summary>
        /// 内容音频源
        /// </summary>
        [Name("内容音频源")]
        [Tip("播放内容语音对象", "Content Audio Source")]
        public AudioSource _contentAudioSource; 

        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; private set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; private set; }

        /// <summary>
        /// 图标
        /// </summary>
        public Sprite icon { get; private set; }

        private UGUIWindow _window;

        private UGUIWindow window => this.XGetComponent<UGUIWindow>(ref _window);

        /// <summary>
        /// 对话框调用者
        /// </summary>
        public object sender { get; private set; }

        /// <summary>
        /// 禁止
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            StopContentAudio();
        }

        private void PlayContentAudio()
        {
            if (_contentAudioSource)
            {
                _contentAudioSource.Play();
            }
        }

        private void StopContentAudio()
        {
            if (_contentAudioSource)
            {
                _contentAudioSource.Stop();
            }
        }

        #region 设置对话框

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="description"></param>
        [InteractCmd]
        [Name("显示")]
        public void Show(string description) => TryInteract(new DialogBoxData(this, description, GetInCmdName(nameof(Show)), this), out _);

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="icon"></param>
        /// <param name="audioClip"></param>
        public void Show(object sender, string title, string content, Sprite icon = null, AudioClip audioClip = null) => TryInteract(new DialogBoxData(sender, title, content, icon, audioClip, GetInCmdName(nameof(Show)), this), out _);

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="dialogBoxData"></param>
        [InteractCmdFun(nameof(Show))]
        public EInteractResult Show(DialogBoxData dialogBoxData)
        {
            if (dialogBoxData == null) return EInteractResult.Fail;

            sender = dialogBoxData.sender;
            if (dialogBoxData.title != null)
            {
                title = dialogBoxData.title;
            }
            if (dialogBoxData.content != null)
            {
                var contentNoteText = GetComponentInChildren<INoteText>(true);
                if (contentNoteText != null)
                {
                    contentNoteText.noteText = content = dialogBoxData.content;
                }
            }
            if (dialogBoxData.icon)
            {
                icon = dialogBoxData.icon;
            }

            Show();

            if (_contentAudioSource && dialogBoxData.audioClip)
            {
                _contentAudioSource.clip = dialogBoxData.audioClip;
                PlayContentAudio();
            }
            return EInteractResult.Success;
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        public void Show()
        {
            gameObject.XSetActive(true);
            transform.SetAsLastSibling();
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        [InteractCmd]
        [Name("隐藏")]
        public void Hide() => TryInteract(new DialogBoxData(this, GetInCmdName(nameof(Hide)), this), out _);

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="sender"></param>
        public void Hide(object sender) => TryInteract(new DialogBoxData(sender, GetInCmdName(nameof(Hide)), this), out _);

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="dialogBoxData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Hide))]
        public EInteractResult Hide(DialogBoxData dialogBoxData)
        {
            gameObject.XSetActive(false);
            return EInteractResult.Success;
        }

        /// <summary>
        /// 设置内容
        /// </summary>
        [InteractCmd]
        [Name("设置内容")]
        public void SetContent(string content) => TryInteract(new DialogBoxData(this, content, GetInCmdName(nameof(Show)), this), out _);

        /// <summary>
        /// 是
        /// </summary>
        /// <param name="dialogBoxData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(SetContent))]
        public EInteractResult SetContent(DialogBoxData dialogBoxData)
        {
            if (dialogBoxData == null) return EInteractResult.Fail;

            content = dialogBoxData.content;
            return EInteractResult.Success;
        }

        #endregion

        #region 点击对话框

        /// <summary>
        /// 是
        /// </summary>
        [InteractCmd]
        [Name("是")]
        public void Yes() => TryInteract(nameof(Yes), content);

        /// <summary>
        /// 是
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Yes))]
        public EInteractResult Yes(InteractData interactData) => HandleRule(interactData);

        /// <summary>
        /// 否
        /// </summary>
        [InteractCmd]
        [Name("否")]
        public void No() => TryInteract(nameof(No), content);

        /// <summary>
        /// 否
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(No))]
        public EInteractResult No(InteractData interactData) => HandleRule(interactData);

        /// <summary>
        /// 点击:未回答是或否
        /// </summary>
        [InteractCmd]
        [Name("点击")]
        public void Click(string otherInfo) => TryInteract(nameof(Click), otherInfo);

        /// <summary>
        /// 点击:未回答是或否
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Click))]
        public EInteractResult Click(InteractData interactData) => HandleRule(interactData);

        private EInteractResult HandleRule(InteractData interactData)
        {
            switch (_afterInteractRule)
            {
                case EAfterInteractRule.Hide:
                    {
                        Hide();
                        break;
                    }
            }
            return EInteractResult.Success;
        } 

        #endregion
    }

    /// <summary>
    /// 对话框资产
    /// </summary>
    [Serializable]
    public class DialogBoxAsset : XGUIAsset<DialogBox> { }

    /// <summary>
    /// 对话框数据
    /// </summary>
    public class DialogBoxData : InteractData<DialogBoxData>
    {
        /// <summary>
        /// 调用者
        /// </summary>
        public object sender { get; private set; } = null;

        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; private set; } = null;

        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; private set; } = null;

        /// <summary>
        /// 图标
        /// </summary>
        public Sprite icon { get; private set; } = null;

        /// <summary>
        /// 音频
        /// </summary>
        public AudioClip audioClip { get; private set; } = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DialogBoxData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public DialogBoxData(object sender, string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, interactor, interactables)
        {
            this.sender = sender;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="content"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public DialogBoxData(object sender, string content, string cmdName, InteractObject interactor, params InteractObject[] interactables) : this(sender, cmdName, interactor, interactables)
        {
            this.content = content;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public DialogBoxData(object sender, string title, string content, string cmdName, InteractObject interactor, params InteractObject[] interactables) : this(sender, content, cmdName, interactor, interactables)
        {
            this.title = title;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="icon"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public DialogBoxData(object sender, string title, string content, Sprite icon, string cmdName, InteractObject interactor, params InteractObject[] interactables) : this(sender, title, content, cmdName, interactor, interactables)
        {
            this.icon = icon;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="icon"></param>
        /// <param name="audioClip"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public DialogBoxData(object sender, string title, string content, Sprite icon, AudioClip audioClip, string cmdName, InteractObject interactor, params InteractObject[] interactables) : this(sender, title, content, icon, cmdName, interactor, interactables)
        {
            this.audioClip = audioClip;
        }

        /// <summary>
        /// 复制到
        /// </summary>
        /// <param name="dialogBoxData"></param>
        protected override void CopyTo(DialogBoxData dialogBoxData)
        {
            dialogBoxData.sender = sender;
            dialogBoxData.title = title;
            dialogBoxData.content = content;
            dialogBoxData.icon = icon;
            dialogBoxData.audioClip = audioClip;
        }
    }
}
