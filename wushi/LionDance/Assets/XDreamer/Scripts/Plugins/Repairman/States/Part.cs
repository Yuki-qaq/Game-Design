using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginRepairman.Tools;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.States
{
    /// <summary>
    /// 零件
    /// </summary>
    [ComponentMenu(RepairmanCategory.ModelDirectory + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(Part))]
    [XCSJ.Attributes.Icon(EIcon.Part)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(RepairmanManager))]
    [Tip("零件组件用于关联一个三维模型和图片的容器。用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。零件不能放在设备和模块之外。零件不能包含零件。", "The part component is a container used to associate a 3D model with a picture. It is realized by state. Is a data organization object in which data is provided to other state components for use. Parts cannot be placed outside equipment and modules. Parts cannot contain parts.")]
    public class Part : Item
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "零件";

        /// <summary>
        /// 创建零件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(Part))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanCategory.Model, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanCategory.ModelDirectory + Title, typeof(RepairmanManager))]
        [Tip("零件组件用于关联一个三维模型和图片的容器。用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。零件不能放在设备和模块之外。零件不能包含零件。", "The part component is a container used to associate a 3D model with a picture. It is realized by state. Is a data organization object in which data is provided to other state components for use. Parts cannot be placed outside equipment and modules. Parts cannot contain parts.")]
        public static State CreatePart(IGetStateCollection obj) => obj?.CreateNormalState(CommonFun.Name(typeof(Part)), null, typeof(Part));

        #region 属性

        /// <summary>
        /// 所属的模块
        /// </summary>
        public Module module => parent.parent.GetComponent<Module>();

        /// <summary>
        /// 设备（状态组件）
        /// </summary>
        public Device device
        {
            get
            {
                var m = module;
                if (m)
                {
                    return (m is Device d) ? d : m.device;
                }

                return null;
            }
        }

        /// <summary>
        /// 设备（工具组件）
        /// </summary>
        public Tools.Device interactDevice
        {
            get
            {
                var d = device;
                if (d)
                {
                    return d.interactPart as Tools.Device;
                }
                return default;
            }
        }

        /// <summary>
        /// 关联交互零件
        /// </summary>
        public virtual Tools.Part interactPart
        {
            get
            {
#if UNITY_EDITOR
                _interactPart = go ? go.XGetOrAddComponent<Tools.Part>() : null;
#else
                    if (!_interactPart)
                    {
                        _interactPart = go ? go.XGetOrAddComponent<Tools.Part>() : null;
                    }
#endif
                return _interactPart;
            }
        }
        private Tools.Part _interactPart;

        /// <summary>
        /// 零件装配状态
        /// </summary>
        public virtual EAssembleState assembleState
        {
            get => interactPart ? interactPart.assembleState : EAssembleState.None;
            set
            {
                if (interactPart)
                {
                    interactPart.assembleState = value;
                }
            }
        }

#endregion

        #region 状态组件方法

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            // 添加零件组件
            if (interactPart) { }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => base.DataValidity() && GetParentItem();

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => GetParentItem() ? name : "需放在模块和设备中!";

        #endregion
    }
}
