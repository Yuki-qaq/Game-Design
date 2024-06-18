using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;
using XCSJ.PluginTools;

namespace XCSJ.PluginRepairman.States.Study
{
    /// <summary>
    /// 拆装学习监听器
    /// </summary>
    [RequireManager(typeof(RepairmanManager))]
    public abstract class RepairStudyListener : InteractProvider
    {
        /// <summary>
        /// 拆装修理学习
        /// </summary>
        [Name("拆装修理学习")]
        [StateComponentPopup(typeof(RepairStudy), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RepairStudy study;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!study)
            {
                study = SMSHelper.GetStateComponents<RepairStudy>().FirstOrDefault();
            }

            if (study)
            {
                study.onPartSelected += OnPartSelected;
                study.onToolSelected += OnToolSelected;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (study)
            {
                study.onPartSelected -= OnPartSelected;
                study.onToolSelected -= OnToolSelected;
            }
        }

        /// <summary>
        /// 当零件已选择
        /// </summary>
        /// <param name="selectedGO"></param>
        /// <param name="right"></param>
        protected abstract void OnPartSelected(GameObject selectedGO, bool right);

        /// <summary>
        /// 当工具已选择
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="right"></param>
        protected abstract void OnToolSelected(PluginRepairman.States.Tool tool, bool right);
    }
}
