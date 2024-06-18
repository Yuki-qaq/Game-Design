#if XDREAMER_HOLOLENS
using HoloToolkit.Unity.InputModule;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Components;

namespace XCSJ.PluginHoloLens
{
    /// <summary>
    /// 语音识别
    /// </summary>
    [Serializable]
    [ComponentMenu("HoloLens/HoloLens语音识别", typeof(HoloLensManager))]
    [Name("HoloLens语音识别", nameof(Speech))]
    [XCSJ.Attributes.Icon(EIcon.Chat)]
    [KeyNode(nameof(EIcon.Trigger), "触发器")]
    [Tip("HoloLens语音识别组件是用于检测用户是否说出指定话语的触发器。当用户说出指定话语后，组件切换为完成态。", "Hololens speech recognition component is a trigger used to detect whether the user speaks a specified utterance. When the user utters the specified words, the component switches to the completed state.")]
    public class Speech : HoloLensStateComponent<Speech>, ITrigger
    {
        /// <summary>
        /// 创建语音识别
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("HoloLens", typeof(HoloLensManager))]
        [StateComponentMenu("HoloLens/HoloLens语音识别", typeof(HoloLensManager))]
#endif
        [Name("HoloLens语音识别", nameof(Speech))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("HoloLens语音识别组件是用于检测用户是否说出指定话语的触发器。当用户说出指定话语后，组件切换为完成态。", "Hololens speech recognition component is a trigger used to detect whether the user speaks a specified utterance. When the user utters the specified words, the component switches to the completed state.")]
        public static State CreateSpeech(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 关键词
        /// </summary>
        [Name("关键词")]
        public List<string> keyWords = new List<string>();

        /// <summary>
        /// 全局监听
        /// </summary>
        [Name("全局监听")]
        [Tip("勾选:HoloLens视点不需要聚焦游戏对象;不勾选:HoloLens视点必须聚焦到游戏对象上说话，才能识别;", "Check: the hololens viewpoint does not need to focus on the game object; Uncheck: the viewpoint of hololens must focus on the game object to speak before it can be recognized;")]
        public bool isGlobal = true;

        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [HideInSuperInspector(nameof(isGlobal), EValidityCheckType.Equal, true)]
        public GameObject gameObject = null;

        private InputListener speechListener;

        /// <summary>
        /// 当进入之前
        /// </summary>
        /// <param name="data"></param>
        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);

            if (HoloLensManager.instance && HoloLensManager.instance.hasAccess && DataValidity())
            {
                GameObject go = null;
                if (isGlobal)
                {
                    if (InputListener.global)
                    {
                        go = InputListener.global.gameObject;
                    }
                }
                else
                {
                    go = gameObject;
                }

                if (go)
                {
                    speechListener = CommonFun.GetOrAddComponent<InputListener>(go);
                    speechListener.AddKeyWords(this.keyWords, isGlobal);
                }
            }

            if (speechListener)
            {
                speechListener.onSpeechCallback += OnSpeechKeywordRecognized;
            }
        }

        /// <summary>
        /// 当退出之后
        /// </summary>
        /// <param name="data"></param>
        public override void OnAfterExit(StateData data)
        {
            if (speechListener)
            {
                speechListener.onSpeechCallback -= OnSpeechKeywordRecognized;
                speechListener.RemoveKeyWords(this.keyWords, isGlobal);
            }

            base.OnAfterExit(data);
        }       

        /// <summary>
        /// 当语音关键字识别
        /// </summary>
        /// <param name="eventData"></param>
#if XDREAMER_HOLOLENS
        protected void OnSpeechKeywordRecognized(SpeechEventData eventData)
#else
        protected void OnSpeechKeywordRecognized(BaseEventData eventData)
#endif
        {
#if XDREAMER_HOLOLENS
            if (keyWords.Contains(eventData.RecognizedText))
#endif
            {
                finished = true;
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return keyWords.Count > 0 && keyWords.Exists(kw=>!string.IsNullOrEmpty(kw));
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return keyWords.Count>0? keyWords[0]:"";
        }
    }
}
