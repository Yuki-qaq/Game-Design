using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 切换命令枚举值
    /// </summary>
    public enum ESwitchCmd
    {
        /// <summary>
        /// 开
        /// </summary>
        [Name("开")]
        On,

        /// <summary>
        /// 关
        /// </summary>
        [Name("关")]
        Off,

        /// <summary>
        /// 切换
        /// </summary>
        [Name("切换")]
        Switch
    }

    /// <summary>
    /// 可开关交互对象
    /// </summary>
    public abstract class Switchable : InteractableVirtual
    {
        /// <summary>
        /// 开关态
        /// </summary>
        public virtual bool isOn
        {
            get => _isOn;
            set
            {
                if (_isOn != value)
                {
                    _isOn = value;
                }
            }
        }

        /// <summary>
        /// 开关态
        /// </summary>
        [Readonly]
        public bool _isOn;

        /// <summary>
        /// 开
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmd]
        [Name("开")]
        [InteractCmdFun(nameof(On))]
        public EInteractResult On(InteractData interactData) 
        {
            if (!isOn)
            {
                Switch(); 
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 关
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmd]
        [Name("关")]
        [InteractCmdFun(nameof(Off))]
        public EInteractResult Off(InteractData interactData)
        {
            if (isOn)
            {
                Switch();
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }


        /// <summary>
        /// 尝试切换
        /// </summary>
        public virtual void Switch()
        {
            isOn = !isOn;
        }

        /// <summary>
        /// 切换
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmd]
        [Name("切换")]
        [InteractCmdFun(nameof(Switch))]
        public EInteractResult Switch(InteractData interactData)
        {
            if (isOn)
            {
                Switch();
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }
    }

}
