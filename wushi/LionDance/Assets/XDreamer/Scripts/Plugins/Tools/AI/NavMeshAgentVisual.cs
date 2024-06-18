using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.AI
{
    /// <summary>
    /// 导航网格代理可视化
    /// </summary>
    [Name("导航网格代理可视化")]
    [XCSJ.Attributes.Icon(EIcon.Net)]
    [Tool(ToolsCategory.AI)]
    [Owner(typeof(ToolsManager))]
    public class NavMeshAgentVisual : BaseNavMeshAgent
    {
        /// <summary>
        /// 导航目标位置指示器
        /// </summary>
        [Name("导航目标位置指示器")]
        public Transform _destination;

        /// <summary>
        /// 路径线渲染器
        /// </summary>
        [Name("路径线渲染器")]
        public LineRenderer _pathLineRenderer;

        /// <summary>
        /// 路径线渲染器
        /// </summary>
        public LineRenderer pathLineRenderer => this.XGetComponent(ref _pathLineRenderer);

        /// <summary>
        /// 路径线渲染器
        /// </summary>
        [Name("路径偏移量")]
        public Vector3 _pathOffset = Vector3.zero;

        /// <summary>
        /// 距离阈值
        /// </summary>
        [Name("距离阈值")]
        [Range(0.01f, 1f)]
        public float _distanceThreshold = 0.5f;

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            var agent = this.agent;
            if (!agent.enabled || !agent.HasValidNavMesh())
            {
                OnAgentStop();
                return;
            }

            // 在半米内禁用指引对象
            if (!agent.pathPending && agent.remainingDistance <= _distanceThreshold)
            {
                OnAgentStop();
            }
            else if (agent.hasPath)
            {
                OnAgentMove();
            }
        }

        private void OnAgentMove()
        {
            SetDestinationActive(true);
            SetPathLineRendererActive(true);
        }

        private void OnAgentStop()
        {
            SetDestinationActive(false);
            SetPathLineRendererActive(false);
        }

        private void SetPathLineRendererActive(bool active)
        {
            if (_pathLineRenderer)
            {
                _pathLineRenderer.enabled = active;

                if (active)
                {
                    var pointCount = agent.path.corners.Length;
                    _pathLineRenderer.positionCount = pointCount;
                    for (int i = 0; i < pointCount; i++)
                    {
                        _pathLineRenderer.SetPosition(i, agent.path.corners[i] + _pathOffset);
                    }
                }
            }
        }

        private void SetDestinationActive(bool active)
        {
            if (_destination)
            {
                if (active)
                {
                    _destination.position = agent.destination;
                }
                _destination.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (pathLineRenderer) { }
        }
    }
}
