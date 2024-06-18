using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;
using XCSJ.Tools;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 获取网络属性：获取网络属性的值并赋值给变量
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [ComponentMenu(MMOHelperExtension.TitleDirectory + Title, typeof(MMOManager))]
    [Name(Title, nameof(GetNetProperty))]
    [Tip("获取网络属性的值并赋值给变量", "Get the value of the network attribute and assign it to the variable")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    public class GetNetProperty : LifecycleExecutor<GetNetProperty>, IDropdownPopupAttribute, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "获取网络属性";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(MMOHelperExtension.Title, typeof(MMOManager))]
        [StateComponentMenu(MMOHelperExtension.TitleDirectory + Title, typeof(MMOManager))]
        [Name(Title, nameof(GetNetProperty))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("获取网络属性的值并赋值给变量", "Get the value of the network attribute and assign it to the variable")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 网络属性
        /// </summary>
        [Name("网络属性")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [FormerlySerializedAs(nameof(netProperty))]
        public NetProperty _netProperty;

        /// <summary>
        /// 网络属性
        /// </summary>
        public NetProperty netProperty { get => _netProperty; set => _netProperty = value; }

        /// <summary>
        /// 属性名
        /// </summary>
        [Name("属性名")]
        [Tip("期望获取属性的名称", "Expected to get the name of the property")]
        [NetPropertyName]
        [FormerlySerializedAs(nameof(propertyName))]
        public string _propertyName;

        /// <summary>
        /// 属性名
        /// </summary>
        public string propertyName { get => _propertyName; set => _propertyName = value; }

        /// <summary>
        /// 变量名
        /// </summary>
        [Name("变量名")]
        [Tip("将获取到的属性值存储在变量名对应的变量中", "Store the obtained attribute value in the variable corresponding to the variable name")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        [FormerlySerializedAs(nameof(variableName))]
        public string _variableName;

        /// <summary>
        /// 变量名
        /// </summary>
        public string variableName { get => _variableName; set => _variableName = value; }

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _variableName);
        }

        #endregion

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            if (netProperty && netProperty.GetProperty(propertyName) is Property property)
            {
                variableName.TrySetOrAddSetHierarchyVarValue(property._value);
            }
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return variableName + VariableCompareHelper.ToAbbreviations(ECompareOperator.Equal) + propertyName + ".属性值";
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && netProperty;
        }

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(NetPropertyNameAttribute):
                    {
                        options = netProperty ? netProperty._propertys.Cast(p => p._name).ToArray() : Empty<string>.Array;
                        return true;
                    }
            }
            options = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetOption(string purpose, string propertyPath, object propertyValue, out string option)
        {
            switch (purpose)
            {
                case nameof(NetPropertyNameAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
            }
            option = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetPropertyValue(string purpose, string propertyPath, string option, out object propertyValue)
        {
            switch (purpose)
            {
                case nameof(NetPropertyNameAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
            }
            propertyValue = default;
            return false;
        }
    }
}
