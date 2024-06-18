using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 单一组件启用
    /// </summary>
    /// <typeparam name="TStateCmponent"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    public abstract class SingleComponentEnable<TStateCmponent, TComponent> : StateComponent<TStateCmponent>
        where TStateCmponent : SingleComponentEnable<TStateCmponent, TComponent>
        where TComponent : Component
    {
        /// <summary>
        /// 组件
        /// </summary>
        [Name("组件")]
        [ComponentPopup]
        public TComponent component;

        /// <summary>
        /// 初始化启用
        /// </summary>
        [Name("初始化启用")]
        [EnumPopup]
        public EBool initEnable = EBool.None;

        /// <summary>
        /// 进入启用
        /// </summary>
        [Name("进入启用")]
        [EnumPopup]
        public EBool entryEnable = EBool.Yes;

        /// <summary>
        /// 退出启用
        /// </summary>
        [Name("退出启用")]
        [EnumPopup]
        public EBool exitEnable = EBool.No;

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
            component.XSetEnable(enable);
        }
    }
}
