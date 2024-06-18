using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using System;
using System.Linq;
using XCSJ.Collections;
using System.Collections.Generic;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginTools;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Locomotion
{
    /// <summary>
    /// 模拟运动提供者
    /// </summary>
    [Name("模拟运动提供者")]
    [RequireManager(typeof(XXRInteractionToolkitManager), typeof(ToolsManager))]
    [DisallowMultipleComponent]
    public class AnalogLocomotionProvider
#if XDREAMER_XR_INTERACTION_TOOLKIT
        : LocomotionProvider
#else
        : InteractProvider
#endif
    {
        /// <summary>
        /// 提供者列表
        /// </summary>
        public HashSet<BaseAnalogLocomotionProvider> _providers = new HashSet<BaseAnalogLocomotionProvider>();

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="provider"></param>
        public bool Regist(BaseAnalogLocomotionProvider provider)
        {
            return provider ? _providers.Add(provider) : false;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="provider"></param>
        public void Unregist(BaseAnalogLocomotionProvider provider)
        {
            _providers.Remove(provider);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            foreach (var pIO in _providers)
            {
                if (pIO) pIO.UpdateProvider(this);
            }
        }

        /// <summary>
        /// 尝试开始运动
        /// </summary>
        /// <returns></returns>
        public bool TryBeginLocomotion()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            return BeginLocomotion();
#else
            return false;
#endif
        }

        /// <summary>
        /// 尝试结束运动
        /// </summary>
        /// <returns></returns>
        public bool TryEndLocomotion()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            return EndLocomotion();
#else
            return false;
#endif
        }
    }
}
