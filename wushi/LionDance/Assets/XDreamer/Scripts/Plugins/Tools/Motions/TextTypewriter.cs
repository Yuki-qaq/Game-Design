using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.LineNotes;

namespace XCSJ.PluginTools.Motions
{
    /// <summary>
    /// 打字动画:用于播放打字效果
    /// </summary>
    [Name("打字动画", nameof(TextTypewriter))]
    [XCSJ.Attributes.Icon(EIcon.Text)]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    [Tool(ToolsCategory.Motion, rootType = typeof(ToolsManager))]
    public class TextTypewriter : PlayableContent, INoteText
    {
        /// <summary>
        /// 播放文本
        /// </summary>
        [Name("播放文本")]
        public string _playText = "";

        /// <summary>
        /// 标注文本
        /// </summary>
        public string noteText 
        { 
            get => _playText; 
            set 
            {
                // 播放内容发生改变
                if (_playText != value) 
                {
                    _playText = value;
                }
            }
        }

        /// <summary>
        /// 时间长度
        /// </summary>
        public override double timeLength 
        { 
            get
            {
                if (_playMode == EPlayMode.FixedSpeed)
                {
                    return base.timeLength = _playText.Length / _speed;
                }
                return base.timeLength;
            }
            set
            {
                if (_playMode != EPlayMode.FixedSpeed)
                {
                    base.timeLength = value;
                }
            }
        }

        /// <summary>
        /// 单次循环时长
        /// </summary>
        public override double onceTimeLength
        {
            get => _playMode == EPlayMode.FixedSpeed ? base.timeLength : base.onceTimeLength;
            set
            {
                if (_playMode != EPlayMode.FixedSpeed)
                {
                    base.onceTimeLength = value;
                }
            }
        }

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _text;

        /// <summary>
        /// 播放模式
        /// </summary>
        public enum EPlayMode
        {
            /// <summary>
            /// 固定时长
            /// </summary>
            [Name("固定时长")]
            FixedTime,

            /// <summary>
            /// 固定速度
            /// </summary>
            [Name("固定速度")]
            FixedSpeed,
        }

        /// <summary>
        /// 播放模式
        /// </summary>
        [Name("播放模式")]
        [EnumPopup]
        public EPlayMode _playMode = EPlayMode.FixedSpeed;

        /// <summary>
        /// 速度
        /// </summary>
        [Name("速度(字/秒)")]
        [Min(0)]
        [HideInSuperInspector(nameof(_playMode), EValidityCheckType.NotEqual, EPlayMode.FixedSpeed)]
        public int _speed = 4;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!_text)
            {
                _text = GetComponent<Text>();
            }
            _playText = _text.text;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            if (_text && !string.IsNullOrEmpty(_text.text) && string.IsNullOrEmpty(noteText))
            {
                noteText = _text.text;
                _text.text = "";
            }

            base.OnEnable();
        }

        /// <summary>
        /// 设置百分比回调
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="playableData"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            if (!string.IsNullOrEmpty(noteText))
            {
                _text.text = noteText.Substring(0, Mathf.RoundToInt(Mathf.Lerp(0f, noteText.Length, (float)percent.percent01OfWorkCurve)));
            }
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => ContainsInCmdName(interactData.cmdName);

        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="text"></param>
        [InteractCmd]
        [Name("设置文本")]
        public void SetText(string text) => TryInteract(nameof(SetText), text);

        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(SetText))]
        public EInteractResult SetText(InteractData interactData)
        {
            if (interactData.cmdParam is string text)
            {
                noteText = text;
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }
    }
}
