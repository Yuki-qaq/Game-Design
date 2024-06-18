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

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Locomotion.Teleportation
{
    /// <summary>
    /// 传送区域
    /// </summary>
    [Name("传送区域")]
    [RequireManager(typeof(XXRInteractionToolkitManager), typeof(ToolsManager))]
    [DisallowMultipleComponent]
    public class TeleportationAnchorExtension
#if XDREAMER_XR_INTERACTION_TOOLKIT
        : TeleportationAnchor
#else
        : InteractProvider
#endif
    {
        /// <summary>
        /// 自动刷新传送提供者
        /// </summary>
        [Name("自动刷新传送提供者")]
        public bool _autoRefreshTeleportationProvider = true;

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
#if XDREAMER_XR_INTERACTION_TOOLKIT
            RefreshTeleportationProvider();

            MB.onEnable += OnMBOnEnable;
            MB.onDisable += OnMBOnDisable;
#endif
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

#if XDREAMER_XR_INTERACTION_TOOLKIT
            MB.onEnable -= OnMBOnEnable;
            MB.onDisable -= OnMBOnDisable;
#endif
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT

        void RefreshTeleportationProvider()
        {
            if (!_autoRefreshTeleportationProvider) return;
            if (teleportationProvider && teleportationProvider.isActiveAndEnabled) { }
            else
            {
                teleportationProvider = FindObjectOfType<TeleportationProvider>();
            }
        }

        void OnMBOnEnable(MB mb)
        {
            if (!_autoRefreshTeleportationProvider) return;
            if (teleportationProvider && teleportationProvider.isActiveAndEnabled) return;

            var tp = mb.GetComponent<TeleportationProvider>();
            if (tp)
            {
                if (tp.isActiveAndEnabled)
                {
                    teleportationProvider = tp;
                }
                else
                {
                    CommonFun.DelayCall(() =>
                    {
                        if (!tp || !tp.isActiveAndEnabled) return;
                        if (!_autoRefreshTeleportationProvider) return;
                        if (teleportationProvider && teleportationProvider.isActiveAndEnabled) return;

                        teleportationProvider = tp;
                    });
                }
            }
        }

        void OnMBOnDisable(MB mb)
        {
            if (!_autoRefreshTeleportationProvider) return;
            if (teleportationProvider && teleportationProvider.gameObject == mb.gameObject)
            {
                //可能将禁用
                var tp = mb.GetComponent<TeleportationProvider>();
                if (tp && tp == teleportationProvider)
                {
                    CommonFun.DelayCall(RefreshTeleportationProvider);
                }
            }
        }
#endif
    }
}
