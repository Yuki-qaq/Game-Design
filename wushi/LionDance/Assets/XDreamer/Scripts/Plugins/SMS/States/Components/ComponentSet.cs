using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 组件集合
    /// </summary>
    [Serializable]
    [ComponentMenu(CommonCategory.CommonUseDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(ComponentSet))]
    [Tip(Title, "Component set")]
    [XCSJ.Attributes.Icon(EIcon.Component)]
    [DisallowMultipleComponent]
    public class ComponentSet : ObjectSet<ComponentSet, UnityEngine.Component>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "组件集合";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        [Name(Title, nameof(ComponentSet))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip(Title, "Component set")]
        public static void Create(IGetStateCollection obj) => CreateNormalState(obj);
    }
}
