using System.Collections.Generic;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 组件记录器
    /// </summary>
    public class ComponentRecorder : Recorder<Component, ComponentRecorder.Info>
    {
        /// <summary>
        /// 信息
        /// </summary>
        public class Info : ISingleRecord<Component>
        {
            /// <summary>
            /// 组件
            /// </summary>
            public Component component;

            /// <summary>
            /// 启用
            /// </summary>
            public bool enabled;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="component"></param>
            public void Record(Component component)
            {
                this.component = component;
                enabled = CommonFun.GetComponentEnabled(component);
            }
            
            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                component.XSetEnable(enabled);
            }
        }
    }

    /// <summary>
    /// 碰撞体记录器:提速访问组件
    /// </summary>
    public class ColliderRecorder : Recorder<Collider, ColliderRecorder.Info>
    {
        /// <summary>
        /// 记录信息
        /// </summary>
        public class Info : ISingleRecord<Collider>
        {
            /// <summary>
            /// 碰撞器
            /// </summary>
            public Collider collider;

            private bool enabled;

            /// <summary>
            /// 记录碰撞体
            /// </summary>
            /// <param name="collider"></param>
            public void Record(Collider collider)
            {
                this.collider = collider;
                enabled = collider.enabled;
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                collider.enabled = enabled;
            }

            /// <summary>
            /// 设置启用
            /// </summary>
            /// <param name="enabled"></param>
            public void SetEnable(bool enabled)
            {
                collider.enabled = enabled;
            }
        }
    }

}
