#if XDREAMER_HOLOLENS
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCameras;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Components;
using XCSJ.Scripts;

namespace XCSJ.PluginHoloLens
{
    /// <summary>
    /// 空间放置
    /// </summary>
    [Serializable]
    [ComponentMenu("HoloLens/HoloLens空间放置", typeof(HoloLensManager))]
    [Name("HoloLens空间放置", nameof(SpatialPut))]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    [Tip("HoloLens空间放置组件是通过用户凝视点摆放游戏对象的触发器。用户通过看现实的场景，现实场景中会出现指示摆放位置的小箭头，用户通过点击手势触发摆放游戏对象逻辑，游戏对象会自动定位在指示点的表面，摆放完成后组件切换为完成态。", "The hololens space placement component is a trigger for placing game objects through the user's gaze point. By looking at the real scene, the user will see a small arrow indicating the placement position in the real scene. The user will trigger the logic of placing the game object by clicking the gesture, and the game object will be automatically positioned on the surface of the indication point. After the placement is completed, the component will switch to the completed state.")]
    public class SpatialPut : HoloLensStateComponent<SpatialPut>
    {
        /// <summary>
        /// 创建放置
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("HoloLens", typeof(HoloLensManager))]
        [StateComponentMenu("HoloLens/HoloLens空间放置", typeof(HoloLensManager))]
#endif
        [Name("HoloLens空间放置", nameof(SpatialPut))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("HoloLens空间放置组件是通过用户凝视点摆放游戏对象的触发器。用户通过看现实的场景，现实场景中会出现指示摆放位置的小箭头，用户通过点击手势触发摆放游戏对象逻辑，游戏对象会自动定位在指示点的表面，摆放完成后组件切换为完成态。", "The hololens space placement component is a trigger for placing game objects through the user's gaze point. By looking at the real scene, the user will see a small arrow indicating the placement position in the real scene. The user will trigger the logic of placing the game object by clicking the gesture, and the game object will be automatically positioned on the surface of the indication point. After the placement is completed, the component will switch to the completed state.")]
        public static State CreatePut(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 放置游戏对象
        /// </summary>
        [Name("放置游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject go = null;

        /// <summary>
        /// 光标对象
        /// </summary>
        [Name("光标对象")]
        public GameObject cursor = null;

        /// <summary>
        /// 空间网格可见
        /// </summary>
        [Name("空间网格可见")]
        public bool spatialMeshVisiable = true;

        /// <summary>
        /// 缺省凝视距离
        /// </summary>
        [Name("缺省凝视距离")]
        [Range(0.01f, 100)]
        public float defaultGazeDistance = 2.0f;

        /// <summary>
        /// 放置类型
        /// </summary>
        [Name("放置类型")]
        [EnumPopup]
        public EPutType putType = EPutType.Transform;

        /// <summary>
        /// 放置偏移量
        /// </summary>
        [Name("放置偏移量")]
        public Vector3 putOffset = Vector3.zero;

        /// <summary>
        /// 包围盒无效时使用变换放置类型
        /// </summary>
        [Name("包围盒无效时使用变换放置类型")]
        public bool useTransformPutTypeWhenboundInvalid = true;

        /// <summary>
        /// 放置时隐藏游戏对象
        /// </summary>
        [Name("放置时隐藏游戏对象")]
        public bool hiddenGO = true;

        /// <summary>
        /// 放置时游戏对象面对相机
        /// </summary>
        [Name("放置时游戏对象面对相机")]
        public bool faceCamera = true;

        /// <summary>
        /// 面对相机时的垂直偏转角
        /// </summary>
        [Name("面对相机时的垂直偏转角")]
        [HideInSuperInspector(nameof(faceCamera), EValidityCheckType.Equal, false)]
        [Range(0,360)]
        public float offsetYAngleWhenFaceCamera = 0;

        private Dictionary<GameObject, int> layerCache = new Dictionary<GameObject, int>();

        private int ignoreRaycastLayer = 2;

        private Vector3 putPosition = Vector3.zero;

        private Vector3 putNormal = Vector3.up;

        /// <summary>
        /// 当进入之前
        /// </summary>
        /// <param name="data"></param>
        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);

            if (cursor)
            {
                inputListener = CommonFun.GetOrAddComponent<InputListener>(cursor);
            }

            if (inputListener)
            {
                inputListener.onClickCallback += OnInputClicked;
            }

            StartPut();
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            var cam = CameraHelperExtension.currentCamera;
            if (!cam) return;

            UpdatePutPointInfo(cam.transform.position, cam.transform.forward, defaultGazeDistance);

            if (cursor)
            {
                cursor.transform.position = putPosition;
            }
        }

        /// <summary>
        /// 当退出之后
        /// </summary>
        /// <param name="data"></param>
        public override void OnAfterExit(StateData data)
        {
            if (inputListener)
            {
                inputListener.onClickCallback -= OnInputClicked;
            }

            StopPut();

            base.OnAfterExit(data);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return go;
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return go ? go.name : "";
        }

        /// <summary>
        /// 当输入点击
        /// </summary>
        /// <param name="eventData"></param>
#if XDREAMER_HOLOLENS
        protected void OnInputClicked(InputClickedEventData eventData)
#else
        protected void OnInputClicked(BaseEventData eventData)
#endif
        {
            finished = true;
        }

        private void StartPut()
        {
            if (!go) return;

            if(hiddenGO)
            {
                go.SetActive(false);
            }

#if XDREAMER_HOLOLENS
            go.SetLayerRecursively(ignoreRaycastLayer, out layerCache);

            InputManager.Instance.PushModalInputHandler(go);

            if (WorldAnchorManager.Instance) WorldAnchorManager.Instance.RemoveAnchor(go);

            if (spatialMeshVisiable && SpatialMappingManager.Instance) SpatialMappingManager.Instance.DrawVisualMeshes = true;
#else
            Log.WarningFormat("设置游戏对象到忽略射线层({0})失败", ignoreRaycastLayer);
#endif
            SetCursorActive(true);
        }

        private void StopPut()
        {
            if (!go) return;

            PutGameObject(go);

#if XDREAMER_HOLOLENS
            go.ApplyLayerCacheRecursively(layerCache);

            InputManager.Instance.PopModalInputHandler();

            if (WorldAnchorManager.Instance) WorldAnchorManager.Instance.AttachAnchor(go);

            if (SpatialMappingManager.Instance) SpatialMappingManager.Instance.DrawVisualMeshes = false;

#endif
            if (hiddenGO)
            {
                go.SetActive(true);
            }

            SetCursorActive(false);
        }

        private void SetCursorActive(bool active)
        {
            if (cursor) cursor.SetActive(active);
        }

        /// <summary>
        /// 如果空间映射检查是否被击中网格，否则使用凝视位置。
        /// </summary>
        /// <returns>获取用户面前的位置</returns>
        private bool UpdatePutPointInfo(Vector3 headPosition, Vector3 gazeDirection, float defaultGazeDistance)
        {
#if XDREAMER_HOLOLENS
            // 射线检测空间网格
            if (SpatialMappingManager.Instance && Physics.Raycast(headPosition, gazeDirection, out RaycastHit hitInfo, 30.0f, SpatialMappingManager.Instance.LayerMask))
            {
                putNormal = hitInfo.normal;
                putPosition = hitInfo.point;
                return true;
            }
            // 如果没有空间网格，则使用默认凝视距离
            else
            {
                putNormal = Vector3.up;
                putPosition = headPosition + gazeDirection * defaultGazeDistance;
                return false;
            }
#else
            return false;
#endif
        }


        private void PutGameObject(GameObject inGO)
        {
            if (!inGO) return;

            // 根据放置类型设置游戏对象位置
            switch (putType)
            {
                case EPutType.None:
                    {
                        break;
                    }
                case EPutType.Transform:
                    {
                        inGO.transform.position = putPosition;
                        break;
                    }
                case EPutType.BoundsCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, inGO))
                        {
                            var offset = inGO.transform.position - bounds.center;
                            inGO.transform.position = putPosition + offset;
                        }
                        else if (useTransformPutTypeWhenboundInvalid)
                        {
                            inGO.transform.position = putPosition;
                        }
                        break;
                    }
                case EPutType.BoundsBottomCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, inGO))
                        {
                            var offset = inGO.transform.position - bounds.center + bounds.size.y / 2 * Vector3.up;
                            inGO.transform.position = putPosition + offset;
                        }
                        else if (useTransformPutTypeWhenboundInvalid)
                        {
                            inGO.transform.position = putPosition;
                        }
                        break;
                    }
                case EPutType.SpereBoundsTangentPoint:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, inGO))
                        {
                            var offset = inGO.transform.position - bounds.center + bounds.size.magnitude / 2 * putNormal;
                            inGO.transform.position = putPosition + offset;
                        }
                        else if (useTransformPutTypeWhenboundInvalid)
                        {
                            inGO.transform.position = putPosition;
                        }
                        break;
                    }
                default:
                    break;
            }

            inGO.transform.position += putOffset;

            // 调整对象面对相机
            if (faceCamera)
            {
                var cam = CameraHelperExtension.currentCamera;
                if (cam)
                {
                    var pos = Vector3.ProjectOnPlane(cam.transform.position, Vector3.up);
                    pos.y += inGO.transform.position.y;
                    inGO.transform.LookAt(pos);
                    inGO.transform.Rotate(new Vector3(0, offsetYAngleWhenFaceCamera, 0));
                }
            }
        }

        /// <summary>
        /// 放置类型
        /// </summary>
        [Name("放置类型")]
        public enum EPutType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 变换
            /// </summary>
            [Name("变换")]
            Transform,

            /// <summary>
            /// 包围盒中心
            /// </summary>
            [Name("包围盒中心")]
            BoundsCenter,

            /// <summary>
            /// 包围盒底面中心
            /// </summary>
            [Name("包围盒底面中心")]
            BoundsBottomCenter,

            /// <summary>
            /// 球形包围盒切点
            /// </summary>
            [Name("球形包围盒切点")]
            SpereBoundsTangentPoint,
        }
    }
}