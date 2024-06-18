using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 网络标识比较:将网络标识属性的值与待比较值进行比较判断；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.ID)]
    [ComponentMenu(MMOHelperExtension.TitleDirectory + Title, typeof(MMOManager))]
    [Name(Title, nameof(NetIdentityCompare))]
    [Tip("将网络标识属性的值与待比较值进行比较判断", "Compare and judge the values of network identification attributes with the values to be compared")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    public class NetIdentityCompare : Trigger<NetIdentityCompare>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "网络标识比较";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(MMOHelperExtension.Title, typeof(MMOManager))]
        [StateComponentMenu(MMOHelperExtension.TitleDirectory + Title, typeof(MMOManager))]
        [Name(Title, nameof(NetIdentityCompare))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("将网络标识属性的值与待比较值进行比较判断", "Compare and judge the values of network identification attributes with the values to be compared")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 网络标识
        /// </summary>
        [Name("网络标识")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public NetIdentity _netIdentity;

        /// <summary>
        /// 网络标识
        /// </summary>
        public NetIdentity netIdentity { get => _netIdentity; set => _netIdentity = value; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public enum EPropertyName
        {
            /// <summary>
            /// 控制权限
            /// </summary>
            [Name("控制权限")]
            ControlAccess,

            /// <summary>
            /// 唯一权限
            /// </summary>
            [Name("唯一权限")]
            UniqueAccess,

            /// <summary>
            /// 权限
            /// </summary>
            [Name("权限")]
            Access,

            /// <summary>
            /// 本地权限
            /// </summary>
            [Name("本地权限")]
            LocalAccess,

            /// <summary>
            /// 动态本地权限
            /// </summary>
            [Name("动态本地权限")]
            DynamicLocalAccess,
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        [EnumPopup]
        public EPropertyName _propertyName = EPropertyName.ControlAccess;

        /// <summary>
        /// 比较运算符
        /// </summary>
        [Name("比较运算符")]
        [FormerlySerializedAs("compareType")]
        [FormerlySerializedAs("_compareType")]
        [EnumPopup]
        public ECompareOperator _compareOperator = ECompareOperator.Equal;

        /// <summary>
        /// 比较运算符
        /// </summary>
        public ECompareOperator compareOperator { get => _compareOperator; set => _compareOperator = value; }

        /// <summary>
        /// 待比较值
        /// </summary>
        [Name("待比较值")]
        public Argument _compareValue = new Argument();

        /// <summary>
        /// 比较规则
        /// </summary>
        [Name("比较规则")]
        [FormerlySerializedAs(nameof(compareRule))]
        [EnumPopup]
        public ECompareRule _compareRule = ECompareRule.String;

        /// <summary>
        /// 比较规则
        /// </summary>
        public ECompareRule compareRule { get => _compareRule; set => _compareRule = value; }

        private object GetProperty()
        {
            switch (_propertyName)
            {
                case EPropertyName.ControlAccess: return netIdentity.hasControlAccess;
                case EPropertyName.UniqueAccess: return netIdentity.hasUniqueAccess;
                case EPropertyName.Access: return netIdentity.access;
                case EPropertyName.LocalAccess: return netIdentity.localAccess;
                case EPropertyName.DynamicLocalAccess: return netIdentity.dynamicLocalAccess;
            }
            return default;
        }

        private bool Check()
        {
            if (netIdentity && _compareValue.GetValueToString() is string tmpCompareValue)
            {
                return VariableCompareHelper.ValueCompareValue(GetProperty(), compareOperator, tmpCompareValue, compareRule);
            }
            return false;
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => Check();

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => CommonFun.Name(_propertyName);

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => netIdentity;
    }
}
