using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using XCSJ.Extension;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginPhysicses.Tools.Collisions;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginTools.PropertyDatas;
using XCSJ.Scripts;

namespace XCSJ.PluginTools
{
    /// <summary>
    /// 工具库扩展助手
    /// </summary>
    public static class ToolsExtensionHelper
    {
        /// <summary>
        /// 设置线渲染器样式
        /// </summary>
        /// <param name="lineRenderer"></param>
        /// <param name="lineStyle"></param>
        public static void SetLineRendererStyle(this LineRenderer lineRenderer, LineStyle lineStyle)
        {
            if (!lineRenderer || !lineStyle) return;

            lineRenderer.material = lineStyle.mat;
            lineRenderer.material.color = lineStyle.color;
            lineRenderer.startWidth = lineStyle.width;
            lineRenderer.endWidth = lineStyle.width;
            lineRenderer.startColor = lineStyle.color;
            lineRenderer.endColor = lineStyle.color;
            lineRenderer.allowOcclusionWhenDynamic = lineStyle.occlusion;
        }

        /// <summary>
        /// 是否在选择集中
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool IsSelection(Component component) => component && IsSelection(component.transform);

        /// <summary>
        /// 是否在选择集中
        /// </summary>
        /// <param name="inTransform"></param>
        /// <returns></returns>
        public static bool IsSelection(Transform inTransform)
        {
            if (!inTransform) return false;

            foreach (var item in Selection.selections)
            {
                if (item && (item == inTransform.gameObject || inTransform.IsChildOf(item.transform)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加可抓交互组件
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        public static bool AddGrabbale(this GameObject gameObject, out Grabbable grabbable)
        {
            if (!gameObject.GetComponent<InteractableEntity>())
            {
                gameObject.XAddComponent<InteractableEntity>();
            }

            if (!gameObject.GetComponent<Grabbable>())
            {
                grabbable = gameObject.XAddComponent<Grabbable>();
                if (grabbable)
                {
                    grabbable.Reset();
                }
                return true;
            }

            grabbable = default;
            return false;
        }

        /// <summary>
        /// 创建一个与输入游戏对象同层级且变换数据相同的游戏对象
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="name"></param>
        /// <param name="isSynTransformData"></param>
        /// <returns></returns>
        public static GameObject CreateGameObjectAtSameSibling(GameObject gameObject, string name, bool isSynTransformData = true)
        {
            if (!gameObject) return null;

            var go = UnityObjectHelper.CreateGameObject(name);
            if (go)
            {
                var sourceTransform = gameObject.transform;
                var targetTransform = go.transform;

                targetTransform.SetParent(sourceTransform.parent);
                targetTransform.SetSiblingIndex(sourceTransform.GetSiblingIndex() + 1);

                if (isSynTransformData)
                {
                    targetTransform.localPosition = sourceTransform.localPosition;
                    targetTransform.localRotation = sourceTransform.localRotation;
                    targetTransform.localScale = sourceTransform.localScale;
                }
            }
            return go;
        }

        /// <summary>
        /// 从交互数据中提取位置信息
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="position"></param>
        /// <param name="tryParent">尝试从父级数据上获取位置信息</param>
        /// <returns></returns>
        public static bool TryGetPosition(this InteractData interactData, out Vector3 position, bool tryParent = false)
        {
            if (interactData != null)
            {
                if (interactData is ColliderInteractData collisionData && collisionData.collision!=null)
                {
                    if (collisionData.collision.contactCount > 0)
                    {
                        position = collisionData.collision.GetContact(0).point;
                        return true;
                    }
                }
                else if (interactData is RayInteractData rayInteractData)
                {
                    if (rayInteractData.raycastHit.HasValue)
                    {
                        position = rayInteractData.raycastHit.Value.point;
                        return true;
                    }
                }
                else if (interactData.interactable)
                {
                    position = interactData.interactable.transform.position;
                    return true;
                }
            }

            if (tryParent)
            {
                TryGetPosition(interactData.parent, out position, tryParent);
            }

            position = Vector3.zero;
            return false;
        }

        static bool? validNavMesh;

        /// <summary>
        /// 检查导航网格有效性:检查当前场景导航网格的有效性，即当前场景存在有效的导航网格；
        /// </summary>
        /// <returns></returns>
        public static bool CheckNavMeshValidity(this NavMeshAgent navMeshAgent)
        {
            if (!navMeshAgent) return false;
            validNavMesh = null;
            var enabled = navMeshAgent.enabled;
            try
            {
#if XDREAMER_UNITY_AI_NAVIGATION
                if (Unity.AI.Navigation.NavMeshSurface.activeSurfaces.Count > 0)
                {
                    validNavMesh = true;
                    return true;
                }
#endif
                Application.logMessageReceived += OnLogCallback;

                navMeshAgent.enabled = false;
                navMeshAgent.enabled = true;

                Application.logMessageReceived -= OnLogCallback;

                if (!validNavMesh.HasValue) validNavMesh = true;
            }
            catch(Exception ex)
            {
                ex.HandleException(nameof(CheckNavMeshValidity));
            }
            finally
            {
                navMeshAgent.enabled = enabled;
                if (!validNavMesh.HasValue) validNavMesh = false;
            }
            return validNavMesh.Value;
        }

        /// <summary>
        /// 当日志回调时
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private static void OnLogCallback(string condition, string stackTrace, LogType type)
        {
            if (!validNavMesh.HasValue)
            {
                if (condition == "Failed to create agent because there is no valid NavMesh")
                {
                    validNavMesh = false;
                }
            }
        }

        /// <summary>
        /// 有有效的导航网格:判断当前场景有有效的导航网格；
        /// </summary>
        /// <param name="navMeshAgent"></param>
        /// <returns></returns>
        public static bool HasValidNavMesh(this NavMeshAgent navMeshAgent)
        {
            if (validNavMesh.HasValue) return validNavMesh.Value;
            return CheckNavMeshValidity(navMeshAgent);
        }
    }

    /// <summary>
    /// 工具库分类
    /// </summary>
    public static class ToolsCategory
    {
        /// <summary>
        /// 工具库前缀
        /// </summary>
        public const string ToolsPrefix = ToolsManager.Title + CommonCategory.HorizontalLine;

        /// <summary>
        /// 交互
        /// </summary>
        public const string Interact = ToolsPrefix + "交互";

        /// <summary>
        /// 交互目录
        /// </summary>
        public const string InteractDirectory = Interact + CommonCategory.PathSplitLine;

        /// <summary>
        /// 交互常用
        /// </summary>
        public const string InteractCommon = Interact + "常用";

        /// <summary>
        /// 交互事件
        /// </summary>
        public const string InteractEvent = Interact + "事件";

        /// <summary>
        /// 交互输入
        /// </summary>
        public const string InteractInput = Interact + "输入";

        /// <summary>
        /// 交互特效
        /// </summary>
        public const string InteractEffect = Interact + "特效";

        /// <summary>
        /// 交互其它
        /// </summary>
        public const string InteractOther = Interact + "其它";

        /// <summary>
        /// UGUI
        /// </summary>
        public const string UGUI = ToolsPrefix + CommonCategory.UGUI;

        /// <summary>
        /// 多媒体
        /// </summary>
        public const string MultiMedia = ToolsPrefix + CommonCategory.MultiMedia;

        /// <summary>
        /// 导航图
        /// </summary>
        public const string MiniMap = ToolsPrefix + "导航图";

        /// <summary>
        /// 控制
        /// </summary>
        public const string Control = ToolsPrefix + "控制";

        /// <summary>
        /// 标注
        /// </summary>
        public const string Note = ToolsPrefix + "标注";

        /// <summary>
        /// 渲染器
        /// </summary>
        public const string Renderer = ToolsPrefix + "渲染器";

        /// <summary>
        /// 游戏对象
        /// </summary>
        public const string GameObject = ToolsPrefix + CommonCategory.GameObject;

        /// <summary>
        /// 自动
        /// </summary>
        public const string AI = ToolsPrefix + "AI";

        /// <summary>
        /// 动作
        /// </summary>
        public const string Motion = ToolsPrefix + "动作";

        /// <summary>
        /// 选择集
        /// </summary>
        public const string SelectionSet = ToolsPrefix + CommonCategory.SelectionSet;
    }

    /// <summary>
    /// 工具库扩展分类
    /// </summary>
    public static class ToolsExtensionCategory
    {
        /// <summary>
        /// 工具库扩展
        /// </summary>
        public const string ToolsExtension = ToolsManager.Title + "扩展";

        /// <summary>
        /// 工具库扩展前缀
        /// </summary>
        public const string ToolsExtensionPrefix = ToolsExtension + CommonCategory.HorizontalLine;

        /// <summary>
        /// 模型
        /// </summary>
        public const string Model = ToolsExtensionPrefix + CommonCategory.Model;

        /// <summary>
        /// 人体
        /// </summary>
        public const string HumanBody = ToolsExtensionPrefix + "人体";
    }
}
