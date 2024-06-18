using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginXGUI.Widgets;

namespace XCSJ.PluginXGUI.States
{
    /// <summary>
    /// 显示对话框
    /// </summary>
    [Name(Title, nameof(ShowDialogBox))]
    [Tip("显示对话框", "display a dialog box")]
    [XCSJ.Attributes.Icon(EIcon.Chat)]
    [Owner(typeof(XGUIManager))]
    [ComponentMenu(XGUICategory.XGUIDirectory + Title, typeof(XGUIManager))]
    public class ShowDialogBox : StateComponent<ShowDialogBox>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "显示对话框";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(ShowDialogBox))]
        [XCSJ.Attributes.Icon(EIcon.Chat)]
        [StateLib(XGUICategory.XGUI, typeof(XGUIManager))]
        [StateComponentMenu(XGUICategory.XGUIDirectory + Title, typeof(XGUIManager))]
        public static State CreateButtonClick(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 对话框资产
        /// </summary>
        [Name("对话框资产")]
        public DialogBoxAsset _dialogBoxAsset = new DialogBoxAsset();

        private DialogBox dialogBox => _dialogBoxAsset.view;

        /// <summary>
        /// 操作规则
        /// </summary>
        [Name("操作规则")]
        public enum EOperateRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 显示
            /// </summary>
            [Name("显示")]
            Show,

            /// <summary>
            /// 隐藏
            /// </summary>
            [Name("隐藏")]
            Hiden,
        }

        /// <summary>
        /// 操作规则
        /// </summary>
        [Name("操作规则")]
        [EnumPopup]
        public EOperateRule _operateRule = EOperateRule.Show;

        /// <summary>
        /// 对话框信息
        /// </summary>
        [Flags]
        public enum EDialogBoxInfo
        {
            /// <summary>
            /// 标题
            /// </summary>
            [Name("标题")]
            Title = 1 << 0,

            /// <summary>
            /// 描述
            /// </summary>
            [Name("描述")]
            Content = 1 << 1,

            /// <summary>
            /// 图标
            /// </summary>
            [Name("图标")]
            Icon = 1 << 2,

            /// <summary>
            /// 音频
            /// </summary>
            [Name("音频")]
            Audio = 1 << 3,
        }

        /// <summary>
        /// 对话框信息
        /// </summary>
        [Name("对话框信息")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_operateRule), EValidityCheckType.NotEqual, EOperateRule.Show)]
        public EDialogBoxInfo _dialogBoxInfo = EDialogBoxInfo.Title | EDialogBoxInfo.Content ;

        /// <summary>
        /// 标题字符
        /// </summary>
        [Name("标题")]
        [HideInSuperInspector(nameof(_operateRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EOperateRule.Show, nameof(_dialogBoxInfo), EValidityCheckType.NotHasFlag, EDialogBoxInfo.Title)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public StringPropertyValue _title = new StringPropertyValue();

        /// <summary>
        /// 内容
        /// </summary>
        [Name("内容")]
        [HideInSuperInspector(nameof(_operateRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EOperateRule.Show, nameof(_dialogBoxInfo), EValidityCheckType.NotHasFlag, EDialogBoxInfo.Content)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public StringPropertyValue_TextArea _content = new StringPropertyValue_TextArea();

        /// <summary>
        /// 图标
        /// </summary>
        [Name("图标")]
        [HideInSuperInspector(nameof(_operateRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EOperateRule.Show, nameof(_dialogBoxInfo), EValidityCheckType.NotHasFlag, EDialogBoxInfo.Icon)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public SpritePropertyValue _icon = new SpritePropertyValue();

        /// <summary>
        /// 音频
        /// </summary>
        [Name("音频")]
        [HideInSuperInspector(nameof(_operateRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EOperateRule.Show, nameof(_dialogBoxInfo), EValidityCheckType.NotHasFlag, EDialogBoxInfo.Audio)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AudioClipPropertyValue _audio = new AudioClipPropertyValue();

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            var db = dialogBox;
            if (db)
            {
                switch (_operateRule)
                {
                    case EOperateRule.Show:
                        {
                            string title = null;
                            if ((_dialogBoxInfo & EDialogBoxInfo.Title) == EDialogBoxInfo.Title)
                            {
                                title = _title.GetValue();
                            }

                            string content = null;
                            if ((_dialogBoxInfo & EDialogBoxInfo.Content) == EDialogBoxInfo.Content)
                            {
                                content = _content.GetValue();
                            }

                            Sprite icon = null;
                            if ((_dialogBoxInfo & EDialogBoxInfo.Icon) == EDialogBoxInfo.Icon)
                            {
                                icon = _icon.GetValue();
                            }

                            AudioClip audioClip = null;
                            if ((_dialogBoxInfo & EDialogBoxInfo.Audio) == EDialogBoxInfo.Audio)
                            {
                                audioClip = _audio.GetValue();
                            }
                            db.Show(this, title, content, icon, audioClip);
                            break;
                        }
                    case EOperateRule.Hiden:
                        {
                            db.Hide(this);
                            break;
                        }
                }
                
            }
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => dialogBox;

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => (dialogBox ? dialogBox.name : "") + CommonFun.Name(this, nameof(_operateRule));
    }
}
