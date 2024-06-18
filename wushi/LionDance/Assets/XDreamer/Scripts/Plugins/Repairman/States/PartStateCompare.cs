using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Tools;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginRepairman.States
{
    /// <summary>
    /// 零件状态比较
    /// </summary>
    [ComponentMenu(RepairmanCategory.ModelDirectory + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(Part))]
    [XCSJ.Attributes.Icon(EIcon.Part)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(RepairmanManager))]
    public class PartStateCompare : BasePropertyCompare<PartStateCompare>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "零件状态比较";

        /// <summary>
        /// 创建零件状态比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(Part))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanCategory.Model, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanCategory.ModelDirectory + Title, typeof(RepairmanManager))]
        public static State CreatePart(IGetStateCollection obj) => obj?.CreateNormalState(Title, null, typeof(PartStateCompare));

        /// <summary>
        /// 零件
        /// </summary>
        [Name("零件")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Tools.Part _interactPart;

        /// <summary>
        /// 零件装配状态
        /// </summary>
        [Name("零件装配状态")]
        [EnumPopup]
        public EAssembleState _assembleState = EAssembleState.None;

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            Check();
        }

        private void Check()
        {
            if (finished || !_interactPart) return;

            finished = _interactPart.assembleState == _assembleState;
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => base.DataValidity() && _interactPart;

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => _interactPart ? _interactPart.name : "" + "等于" + CommonFun.Name(_assembleState);
    }
}
