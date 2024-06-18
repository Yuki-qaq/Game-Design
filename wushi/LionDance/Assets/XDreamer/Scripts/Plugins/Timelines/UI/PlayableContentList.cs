using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTimelines.Tools;
using XCSJ.PluginXGUI.Windows.ListViews;

namespace XCSJ.PluginTimelines.UI
{
    /// <summary>
    /// 播放内容列表
    /// </summary>
    [Name("播放内容列表")]
    [RequireManager(typeof(TimelineManager))]
    public class PlayableContentList : ListViewModelProvider
    {
        /// <summary>
        /// 播放器控制器        
        /// </summary>
        [Name("播放器控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public PlayerController _playerController;

        /// <summary>
        /// 播放控制器
        /// </summary>
        public PlayerController playerController => this.XGetComponentInChildrenOrGlobal(ref _playerController);

        /// <summary>
        /// 可播放内容列表
        /// </summary>
        [Name("可播放内容列表")] 
        public List<PlayableContentModel> _playableContentModels = new List<PlayableContentModel>();

        /// <summary>
        /// 预加载数据
        /// </summary>
        protected override IEnumerable<ListViewItemModel> prefabModels => _playableContentModels;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (playerController) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!playerController)
            {
                enabled = false;
                Debug.LogErrorFormat("[{0}]未找到有效的播放器控制器!", CommonFun.ObjectToString(this));
            }
        }

        /// <summary>
        /// 视图项点击
        /// </summary>
        /// <param name="listViewInteractData"></param>
        internal override void OnClick(ListViewInteractData listViewInteractData)
        {
            base.OnClick(listViewInteractData);

            if (playerController && listViewInteractData.listViewItemModel is PlayableContentModel model) 
            {
                if (model.component is PlayableContent playableContent && playableContent)
                {
                    if (playerController) playerController.PlayContent(playableContent);
                }
            }
        }
    }

    /// <summary>
    /// 可播放内容模型
    /// </summary>
    [Serializable]
    public class PlayableContentModel : ComponentModel<PlayableContent> { }
}
