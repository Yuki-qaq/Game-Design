using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools.Items;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Widgets;

namespace XCSJ.PluginTools.AI.NPCs
{
    /// <summary>
    /// 谈话动作：存储这对话内容，并通过对话框窗口展示对话内容
    /// </summary>
    [Name("谈话动作")]
    [XCSJ.Attributes.Icon(EIcon.Chat)]
    public class TalkAction : NPCAction
    {
        /// <summary>
        /// 谈话规则
        /// </summary>
        public enum ETalkRule
        {
            /// <summary>
            /// 顺序
            /// </summary>
            [Name("顺序")]
            Order,

            /// <summary>
            /// 随机
            /// </summary>
            [Name("随机")]
            Random,
        }

        /// <summary>
        /// 谈话规则
        /// </summary>
        [Name("谈话规则")]
        [EnumPopup]
        public ETalkRule _talkRule = ETalkRule.Random;

        /// <summary>
        /// 玩家碰撞规则
        /// </summary>
        public enum ECollidePlayerRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 进入时开始谈话_退出时停止谈话
            /// </summary>
            [Name("进入时开始谈话_退出时停止谈话")]
            BeginTalkOnEnter_And_EndTalkOnExit,
        }

        /// <summary>
        /// 玩家碰撞规则
        /// </summary>
        [Name("玩家碰撞规则")]
        [EnumPopup]
        public ECollidePlayerRule _playerRule = ECollidePlayerRule.BeginTalkOnEnter_And_EndTalkOnExit;

        /// <summary>
        /// 游戏对象比较器
        /// </summary>
        [Name("游戏对象比较器")]
        [Tip("符合比较规则的游戏对象才触发对话", "Only game objects that meet the comparison rules trigger the conversation")]
        public GameObjectComparer _gameObjectComparer = new GameObjectComparer();

        private DialogBox dialogBox => _dialogBoxAsset.view;

        /// <summary>
        /// 对话框资产
        /// </summary>
        [Name("对话框资产")]
        public DialogBoxAsset _dialogBoxAsset = new DialogBoxAsset();

        /// <summary>
        /// 聊天数据列表
        /// </summary>
        [Name("聊天数据列表")]
        public List<TalkInfo> _talkInfos = new List<TalkInfo>();

        private TalkInfo currentTalkInfo = null;
        private int talkInfoIndex = -1;

        private TalkInfo GetCurrentTalkInfo()
        {
            var count = _talkInfos.Count;
            if (count > 0)
            {
                switch (_talkRule)
                {
                    case ETalkRule.Order: talkInfoIndex = (talkInfoIndex + 1) % count; break;
                    case ETalkRule.Random: talkInfoIndex = UnityEngine.Random.Range(0, count); break;
                    default: talkInfoIndex = 0; break;
                }
                return _talkInfos[talkInfoIndex];
            }
                
            return default;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!dialogBox)
            {
                Debug.LogErrorFormat("【{0}】缺少对话框对象", CommonFun.ObjectToString(this));
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// 当输入交互
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="interactData"></param>
        protected override void OnInputInteract(InteractObject sender, InteractData interactData)
        {
            base.OnInputInteract(sender, interactData);

            if (sender == dialogBox)
            {
                switch (interactData.cmd)
                {
                    case nameof(DialogBox.Yes):
                        {
                            NextTalk();
                            break;
                        }
                    case nameof(DialogBox.No):
                        {
                            EndTalk();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="npcInteractData"></param>
        /// <returns></returns>
        public override bool Execute(NPCInteractData npcInteractData)
        {
            switch (_playerRule)
            {
                case ECollidePlayerRule.BeginTalkOnEnter_And_EndTalkOnExit:
                    {
                        switch (npcInteractData.cmd)
                        {
                            case nameof(NPC.OnPlayerCollisionEnter):
                                {
                                    if (_gameObjectComparer.Compare(npc.interactPlayer))
                                    {
                                        BeginTalk();
                                    }
                                    return true;
                                }
                            case nameof(NPC.OnPlayerCollisionExit):
                                {
                                    if (_gameObjectComparer.Compare(npc.interactPlayer))
                                    {
                                        EndTalk();
                                    }
                                    return true;
                                }
                        }
                        break;
                    }
            }

            return base.Execute(npcInteractData);
        }

        /// <summary>
        /// 开始谈话
        /// </summary>
        [InteractCmd]
        [Name("开始谈话")]
        public void BeginTalk() => TryInteract(nameof(BeginTalk));

        /// <summary>
        /// 当开始谈话
        /// </summary>
        [InteractCmdFun(nameof(BeginTalk))]
        public void OnBeginTalk()
        {
            NextTalk();
        }

        /// <summary>
        /// 下一句话
        /// </summary>
        public void NextTalk()
        {
            currentTalkInfo = GetCurrentTalkInfo();

            if (currentTalkInfo != null)
            {
                if (dialogBox)
                {
                    dialogBox.Show(this, npc.displayName, currentTalkInfo._content, npc.headImage, currentTalkInfo._audioClip);
                }
            }
            else
            {
                EndTalk();
            }
        }

        /// <summary>
        /// 结束谈话
        /// </summary>
        [InteractCmd]
        [Name("结束谈话")]
        public void EndTalk() => TryInteract(nameof(EndTalk));

        /// <summary>
        /// 当结束谈话
        /// </summary>
        [InteractCmdFun(nameof(EndTalk))]
        public void OnEndTalk()
        {
            if (dialogBox) dialogBox.Hide();
        }
    }

    /// <summary>
    /// 谈话信息
    /// </summary>
    [Serializable]
    public class TalkInfo
    {
        /// <summary>
        /// 内容
        /// </summary>
        [Name("内容")]
        [TextArea]
        public string _content;

        /// <summary>
        /// 音频
        /// </summary>
        [Name("音频")]
        [DynamicLabel]
        public AudioClip _audioClip;
    }
}
