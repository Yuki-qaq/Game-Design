using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCameras;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 相机视图平面拖拽工具：
    /// 1、基于相机朝向和参考点构建一个平面，游戏对象在该平面上运动
    /// 2、被抓对象在3D场景中，则使用被抓对象世界位置作为参考点
    /// 3、如果被抓对象有插槽适配对象，插槽位置作为参考点
    /// </summary>
    [Name("相机视图平面拖拽工具")]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [RequireManager(typeof(ToolsManager))]
    public class DragByCameraView : Dragger
    {
        /// <summary>
        /// 缺省距离
        /// </summary>
        [Name("缺省距离")]
        [Min(0.1f)]
        public float _defaultDistance = 3;

        /// <summary>
        /// 使用插槽对齐
        /// </summary>
        [Name("使用插槽对齐")]
        public bool _useSocketAlign = true;

        /// <summary>
        /// 插槽信息对象列表
        /// </summary>
        [Name("插槽信息对象列表")]
        [Tip("拖拽时获取插槽位置并将被拖拽对象与位置进行对齐", "When dragging, obtain the slot position and align the dragged object with the position")]
        [HideInSuperInspector(nameof(_useSocketAlign), EValidityCheckType.False)]
        [ComponentPopup]
        public List<MB> _socketInfoMBs = new List<MB>();

        /// <summary>
        /// 插槽信息对象
        /// </summary>
        private List<ISocketInfo> socketInfos = new List<ISocketInfo>();

        /// <summary>
        /// 是否拖拽
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        protected override bool CanGrab(InteractData interactData, Grabbable grabbable)
        {
            // 判断射线是否在选择集对象上
            return base.CanGrab(interactData, grabbable) && grabbable && ToolsExtensionHelper.IsSelection(grabbable.transform);
        }

        // 变换位置与碰撞点的偏移量
        private Vector3 offsetGrabObjPositionToHitPoint = Vector3.zero;

        // 以射线碰撞点为点，相机朝向法线构建的平面
        private Plane camDirPlaneOnGrab;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            socketInfos.Clear();
            if (_socketInfoMBs.Count == 0)
            {
                // 全局查找
                socketInfos.AddRange(ComponentCache.GetComponents<ISocketInfo>(true));
            }
            else
            {
                foreach (var item in _socketInfoMBs)
                {
                    if (item)
                    {
                        socketInfos.Add(item.GetComponent<ISocketInfo>());
                    }
                }
            }
        }

        /// <summary>
        /// 开启拖拽
        /// </summary>
        protected override void OnGrabEnter()
        {
            base.OnGrabEnter();

            var cam = CameraHelperExtension.currentCamera;
            if (cam && grabData!=null)
            {
                var rayHit = grabData.raycastHit;
                if (rayHit.HasValue)
                {
                    offsetGrabObjPositionToHitPoint = grabbedObject.transform.position - rayHit.Value.point;
                    camDirPlaneOnGrab = new Plane(cam.transform.forward, rayHit.Value.point);
                }
            }
        }

        private bool TryGetPoseFromSocketInfos(out Pose pose)
        {
            foreach (var item in socketInfos)
            {
                if (item.TryGetPose(out pose))
                {
                    return true;
                }
            }
            pose = default;
            return false;
        }

        /// <summary>
        /// 获取位置信息
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected override bool TryGetDragPosition(out Vector3 position)
        {
            var rayData = holdData.ray;
            if (rayData.HasValue)
            {
                var ray = rayData.Value;
                var origin = ray.origin;
                var dir = ray.direction;

                float distance = -1;
                // 拖拽对象有插槽时，使用插槽对齐
                if (_useSocketAlign && TryGetPoseFromSocketInfos(out var pose))
                {
                    distance = Mathf.Abs(Vector3.Dot(dir, pose.position - origin));
                }
                else if (camDirPlaneOnGrab.Raycast(ray, out var enter)) // 使用相机平面
                {
                    distance = enter;
                }

                if (distance < 0)
                {
                    distance = _defaultDistance;
                }
                position = origin + dir * distance + offsetGrabObjPositionToHitPoint;
                return true;
            }
            return base.TryGetDragPosition(out position);
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        protected override void OnReleaseEnter()
        {
            base.OnReleaseEnter();

            offsetGrabObjPositionToHitPoint = Vector3.zero;
        }
    }

    /// <summary>
    /// 插槽信息
    /// </summary>
    public interface ISocketInfo
    {
        /// <summary>
        /// 获取插槽姿态
        /// </summary>
        /// <param name="pose"></param>
        /// <returns></returns>
        bool TryGetPose(out Pose pose);
    }
}
