using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 组件启用：组件启用组件是控制Unity组件启用禁用的执行体。随着状态生命周期发生的事件（进入和退出），启用和禁用设置的Unity组件，组件激活后即刻切换为完成态。
    /// </summary>
    [ComponentMenu(CommonCategory.CommonUseDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(ComponentEnable))]
    [XCSJ.Attributes.Icon(EIcon.ComponentEnable)]
    [RequireComponent(typeof(ComponentSet))]
    [Tip("组件启用组件是控制Unity组件启用禁用的执行体。随着状态生命周期发生的事件（进入和退出），启用和禁用设置的Unity组件，组件激活后即刻切换为完成态。", "The component enabling component is the actuator that controls the enabling and disabling of unity components. With the events (entry and exit) occurring in the state life cycle, enable and disable the set unity component, and switch to the completed state immediately after the component is activated.")]
    public class ComponentEnable : StateComponent<ComponentEnable>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "组件启用";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(CommonCategory.CommonUse, typeof(SMSManager))]
        [StateComponentMenu(CommonCategory.CommonUseDirectory + Title, typeof(SMSManager))]
        [StateLib(SMSCategory.Component, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.ComponentDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(ComponentEnable))]
        [Tip("组件启用组件是控制Unity组件启用禁用的执行体。随着状态生命周期发生的事件（进入和退出），启用和禁用设置的Unity组件，组件激活后即刻切换为完成态。", "The component enabling component is the actuator that controls the enabling and disabling of unity components. With the events (entry and exit) occurring in the state life cycle, enable and disable the set unity component, and switch to the completed state immediately after the component is activated.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateComponentEnable(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 初始化
        /// </summary>
        [Name("初始化")]
        [EnumPopup]
        public EBool initEnable = EBool.None;

        /// <summary>
        /// 进入
        /// </summary>
        [Name("进入")]
        [EnumPopup]
        public EBool entryEnable = EBool.Yes;

        /// <summary>
        /// 退出
        /// </summary>
        [Name("退出")]
        [EnumPopup]
        public EBool exitEnable = EBool.None;

        /// <summary>
        /// 组件集
        /// </summary>
        public ComponentSet componentSet => GetComponent<ComponentSet>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            SetEnable(initEnable);
            return base.Init(data);
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            SetEnable(entryEnable);
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            SetEnable(exitEnable);
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished()
        {
            return true;
        }

        /// <summary>
        /// 设置启用
        /// </summary>
        /// <param name="enable"></param>
        public void SetEnable(EBool enable)
        {
            foreach (UnityEngine.Component c in componentSet.objects)
            {
                c.XSetEnable(enable);
            }
        }
    }
}
