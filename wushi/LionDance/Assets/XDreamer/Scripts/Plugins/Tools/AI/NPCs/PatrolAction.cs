using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.AI.NPCs
{
    /// <summary>
    /// 巡逻动作
    /// 1、基于导航网格进行巡逻、具备【固定路径巡逻】与【随机巡逻】两种工作模式
    /// 2、【固定路径巡逻】具有循环、往返、路径点随机三种模式；路径点必须在导航网格的区域内。
    /// 3、【随机巡逻】以NPC自身为原点，在指定的半径的圆形内随机，并且随机点需命中导航网格
    /// 4、随机点之间可设定停留时长
    /// </summary>
    [Name("巡逻动作")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    public class PatrolAction : NPCAction
    {
        /// <summary>
        /// 工作模式
        /// </summary>
        public enum EWorkMode
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 固定路径巡逻
            /// </summary>
            [Name("固定路径巡逻")]
            PatrolOnFixedPath,

            /// <summary>
            /// 随机巡逻
            /// </summary>
            [Name("随机巡逻")]
            PatrolRandom,
        }

        /// <summary>
        /// 工作模式
        /// </summary>
        [Name("工作模式")]
        [EnumPopup]
        public EWorkMode _workMode = EWorkMode.PatrolRandom;

        #region 固定路径巡逻

        /// <summary>
        /// 巡逻路径规则
        /// </summary>
        public enum EPatrolPathRule
        {
            /// <summary>
            /// 循环
            /// </summary>
            [Name("循环")]
            [Tip("按路径点顺序移动；到达路径最后一个点时，选择第一个点作为目标点", "Move in order of path points; When reaching the last point of the path, select the first point as the target point")]
            Loop,

            /// <summary>
            /// 往返
            /// </summary>
            [Name("往返")]
            [Tip("按路径点顺序移动；到达路径最后一个点后再按路径点逆序移动", "Move in order of path points; After reaching the last point of the path, move in reverse order according to the path points")]
            BackTrack,

            /// <summary>
            /// 随机
            /// </summary>
            [Name("随机")]
            [Tip("在路径点上随机一个点作为移动目标", "Randomly select a point on the path point as the moving target")]
            Random,
        }

        /// <summary>
        /// 巡逻路径规则
        /// </summary>
        [Name("巡逻路径规则")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_workMode), EValidityCheckType.NotEqual, EWorkMode.PatrolOnFixedPath)]
        public EPatrolPathRule _patrolPathRule = EPatrolPathRule.Loop;

        /// <summary>
        /// 路径
        /// </summary>
        [Name("路径")]
        [Tip("路径点必须在导航网格区域内，否则无法到达", "The path point must be within the navigation grid area, otherwise it cannot be reached")]
        [HideInSuperInspector(nameof(_workMode), EValidityCheckType.NotEqual, EWorkMode.PatrolOnFixedPath)]
        public List<PatrolPoint> _pathPoints = new List<PatrolPoint>();

        /// <summary>
        /// 巡逻路径
        /// </summary>
        [Serializable]
        public class PatrolPoint
        {
            /// <summary>
            /// 变换
            /// </summary>
            [Name("变换")]
            [DynamicLabel]
            public Transform _transform;

            /// <summary>
            /// 随机延迟时长区间
            /// </summary>
            [Name("随机延迟时长区间")]
            [DynamicLabel]
            [LimitRange(0, 1000)]
            public Vector2 _randomDelayTimeRange = new Vector2(3, 10);

            /// <summary>
            /// 随机延迟时长
            /// </summary>
            /// <returns></returns>
            public float RandomDelayTime() => UnityEngine.Random.Range(_randomDelayTimeRange.x, _randomDelayTimeRange.y);
        }

        private int destPointIndex = 0;
        private int step = 1;

        private bool GetNextPointOnPath(out Vector3 destination)
        {
            var pathCount = _pathPoints.Count;
            if (pathCount == 0)
            {
                destination = default;
                return false;
            }

            // 获取路径点对应的目的地
            destPointIndex = Mathf.Clamp(destPointIndex, 0, pathCount - 1);
            destination = _pathPoints[destPointIndex]._transform.position;

            // 根据规则重新计算目的地索引
            switch (_patrolPathRule)
            {
                case EPatrolPathRule.Loop:
                    {
                        destPointIndex = (destPointIndex + 1) % pathCount;
                        break;
                    }
                case EPatrolPathRule.BackTrack:
                    {
                        if (destPointIndex == pathCount - 1)
                        {
                            step = -1;
                        }
                        if (destPointIndex == 0)
                        {
                            step = 1;
                        }

                        destPointIndex += step;
                        break;
                    }
                case EPatrolPathRule.Random:
                    {
                        destPointIndex = UnityEngine.Random.Range(0, pathCount);
                        break;
                    }
            }

            return true;
        }


        #region 检查器窗口

        /// <summary>
        /// 批量处理对象
        /// </summary>
        [Name("批量处理对象")]
        [HideInSuperInspector]
        public GameObject _batchGameObject;

        /// <summary>
        /// 包含:为True时，将 批处理游戏对象<see cref="_batchGameObject"/> 添加到路径中；无则添加，有则不变；
        /// </summary>
        [Name("包含")]
        [Tip("为True时，将 批处理游戏对象 添加到路径中；无则添加，有则不变；", "When true, add batch game objects to the path; If there is nothing, add it; if there is something, leave it unchanged;")]
        [HideInSuperInspector]
        public bool _include = true;

        /// <summary>
        /// 成员:为True时，将 批处理游戏对象<see cref="_batchGameObject"/> 的子级成员全部添加到路径中；无则添加，缺则补漏，有则不变；
        /// </summary>
        [Name("成员")]
        [Tip("为True时，将 批处理游戏对象 的子级成员全部添加到路径中；无则添加，缺则补漏，有则不变；", "When true, add all child members of the batch game object to the path; Add if there is no, fill in if there is a deficiency, and leave it unchanged if there is one;")]
        [HideInSuperInspector]
        public bool _chileren = false;

        #endregion

        #endregion

        #region 随机巡逻

        /// <summary>
        /// 随机半径
        /// </summary>
        [Name("随机半径")]
        [Min(0)]
        [HideInSuperInspector(nameof(_workMode), EValidityCheckType.NotEqual, EWorkMode.PatrolRandom)]
        public float _randomRadius = 10;

        /// <summary>
        /// 随机停留时长区间
        /// </summary>
        [Name("随机停留时长区间")]
        [HideInSuperInspector(nameof(_workMode), EValidityCheckType.NotEqual, EWorkMode.PatrolRandom)]
        [LimitRange(0, 1000)]
        public Vector2 _randomStayTimeRange = new Vector2(3, 10);

        private float delayTimeCounter = 0;
        private float delayTime = 0;

        /// <summary>
        /// 随机目标点
        /// </summary>
        /// <returns></returns>
        public bool RandomNextPoint(out Vector3 destination)
        {
            if (NavMesh.SamplePosition(transform.position + UnityEngine.Random.insideUnitSphere * _randomRadius, out var hit, 1.0f, NavMesh.AllAreas))
            {
                destination = hit.position;
                return true;
            }
            destination = default;
            return false;
        }

        #endregion

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            delayTimeCounter = delayTime = GetDelayTime();
        }

        /// <summary>
        /// 尝试获取目标点
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public override bool TryGetDestination(out Vector3 destination)
        {
            delayTimeCounter += Time.deltaTime;
            if (delayTimeCounter >= delayTime)
            {
                if (GotoNextPoint(out destination))
                {
                    delayTime = GetDelayTime();
                    delayTimeCounter = 0;
                    return true;
                }
            }
            destination = default;
            return false;
        }

        private float GetDelayTime()
        {
            switch (_workMode)
            {
                case EWorkMode.PatrolOnFixedPath: return _pathPoints.Count == 0 ? 0 : _pathPoints[destPointIndex].RandomDelayTime();
                case EWorkMode.PatrolRandom: return UnityEngine.Random.Range(_randomStayTimeRange.x, _randomStayTimeRange.y);
                default: return 0;
            }
        }

        private bool GotoNextPoint(out Vector3 destination)
        {
            switch (_workMode)
            {
                case EWorkMode.None:
                    {
                        destination = npc.position;
                        return true;
                    }
                case EWorkMode.PatrolOnFixedPath: return GetNextPointOnPath(out destination);
                case EWorkMode.PatrolRandom: return RandomNextPoint(out destination);
            }
            destination = default;
            return false;
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => _workMode != EWorkMode.None;
    }

}
