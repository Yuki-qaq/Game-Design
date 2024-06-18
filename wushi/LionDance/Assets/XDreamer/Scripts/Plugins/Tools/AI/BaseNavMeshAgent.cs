using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCameras.Characters.Tools;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.AI
{
    /// <summary>
    /// 基础导航网格代理
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class BaseNavMeshAgent : Interactor
    {
        private NavMeshAgent _agent;

        /// <summary>
        /// 导航网格代理
        /// </summary>
        public NavMeshAgent agent => this.XGetComponent<NavMeshAgent>(ref _agent);

        private INavMeshAgentController navMeshAgentController;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!agent)
            {
                enabled= false;
                return;
            }

            navMeshAgentController = GetComponentInChildren<INavMeshAgentController>();
            if (navMeshAgentController == null)
            {
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// 开始移动
        /// </summary>
        /// <param name="destination"></param>
        protected void StartMove(Vector3 destination)
        {
            navMeshAgentController?.SetAgentDestination(destination);
        }

        /// <summary>
        /// 停止移动
        /// </summary>
        protected void StopMove()
        {
            navMeshAgentController?.ResetAgentPath();
        }
    }
}
