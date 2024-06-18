using System;
using XCSJ.Attributes;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 数值比较类型
    /// </summary>
    [Name("数值比较类型")]
    public enum ENumberValueCompareRule
    {
        /// <summary>
        /// 等于
        /// </summary>
        [Name("等于")]
        Equal = 0,

        /// <summary>
        /// 不等于
        /// </summary>
        [Name("不等于")]
        NotEqual,

        /// <summary>
        /// 小于
        /// </summary>
        [Name("小于")]
        Less,

        /// <summary>
        /// 小于等于
        /// </summary>
        [Name("小于等于")]
        LessEqual,

        /// <summary>
        /// 大于
        /// </summary>
        [Name("大于")]
        Greater,

        /// <summary>
        /// 大于等于
        /// </summary>
        [Name("大于等于")]
        GreaterEqual,

        /// <summary>
        /// 变化时
        /// </summary>
        [Name("变化时")]
        Changed,
    }

    /// <summary>
    /// 数值比较触发器
    /// </summary>
    [Name("数值比较触发器")]
    [Serializable]
    public class FloatValueTrigger
    {
        /// <summary>
        /// 比较规则
        /// </summary>
        [Name("比较规则")]
        [EnumPopup]
        public ENumberValueCompareRule compareRule = ENumberValueCompareRule.Changed;

        /// <summary>
        /// 触发值
        /// </summary>
        [Name("触发值")]
        [HideInSuperInspector(nameof(compareRule), EValidityCheckType.Equal, ENumberValueCompareRule.Changed)]
        public float triggerValue = 1;

        /// <summary>
        /// 是触发
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsTrigger(float value)
        {
            switch (compareRule)
            {
                case ENumberValueCompareRule.Equal: return MathX.Approximately(value, triggerValue);
                case ENumberValueCompareRule.Greater: return value > triggerValue;
                case ENumberValueCompareRule.GreaterEqual: return value >= triggerValue;
                case ENumberValueCompareRule.Less: return value < triggerValue;
                case ENumberValueCompareRule.LessEqual: return value <= triggerValue;
                case ENumberValueCompareRule.NotEqual: return !MathX.Approximately(value, triggerValue);
                case ENumberValueCompareRule.Changed: return true;
            }

            return false;
        }
    }

}
