using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginPhysicses.Tools.Collisions;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginPhysicses.Tools.Destructions
{
    /// <summary>
    /// 伤害类型
    /// </summary>
    public enum EDamageType
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
    }

    /// <summary>
    /// 破坏交互数据
    /// </summary>
    public class BreakableData : ColliderInteractData
    {
        /// <summary>
        /// 伤害类型
        /// </summary>
        public EDamageType damageType { get; private set; }

        /// <summary>
        /// 伤害值
        /// </summary>
        public float damageValue  { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="damageType"></param>
        /// <param name="damageValue"></param>
        /// <param name="collision"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public BreakableData(EDamageType damageType, float damageValue, Collision collision, string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(collision,  cmdName, interactor, interactables)
        {
            this.damageType = damageType;
            this.damageValue = damageValue;
        }
    }

    /// <summary>
    /// 破坏器:通过物理碰撞事件触发将破坏力主动作用于可破坏对象
    /// </summary>
    [RequireManager(typeof(PhysicsManager))]
    [Owner(typeof(PhysicsManager))]
    public abstract class Breaker : ColliderTrigger
    {
        /// <summary>
        /// 伤害类型
        /// </summary>
        [Name("伤害类型")]
        public EDamageType _damageType = EDamageType.None;

        /// <summary>
        /// 伤害值
        /// </summary>
        [Name("伤害值")]
        public float _damageValue = 10;

        /// <summary>
        /// 调用碰撞完成
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="collision"></param>
        protected override void CallCollisionFinish(string cmdName, Collision collision)
        {
            CallFinished(new BreakableData(_damageType, _damageValue, collision, cmdName, this, collision.gameObject.GetComponents<InteractObject>()));
        }
    }
}
