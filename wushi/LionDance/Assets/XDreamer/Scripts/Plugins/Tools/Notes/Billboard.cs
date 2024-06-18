using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCameras;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Inputs;

namespace XCSJ.PluginTools.Notes
{
    /// <summary>
    /// 广告牌类型
    /// </summary>
    [Name("广告牌类型")]
    public enum EBillboardLookatRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [Tip("不受相机朝向影响", "Not affected by camera orientation")]
        None,

        /// <summary>
        /// 朝向相机
        /// </summary>
        [Name("朝向相机")]
        [Tip("当前游戏对象的自身坐标系Z轴始终朝向相机；类似粒子或者精灵朝向相机的效果", "The Z axis of the current game object's own coordinate system always faces the camera; Similar to the effect of particles or sprites facing the camera")]
        LookAtCamera,

        /// <summary>
        /// BB树
        /// </summary>
        [Name("BB树")]
        [Tip("当前游戏对象绕世界坐标系Y轴（与自身坐标系Y轴方向重合）旋转，且其自身坐标系X0Y面（Z轴正方向）始终朝向相机", "The current GameObject rotates around the Y-axis of the world coordinate system (coincident with the y-axis direction of its own coordinate system), and its own coordinate system x0y plane (positive direction of Z-axis) always faces the camera")]
        BillboardTree,

        /// <summary>
        /// 朝向游戏对象
        /// </summary>
        [Name("朝向游戏对象")]
        [Tip("当前游戏对象的自身坐标系Z轴始终朝向游戏对象", "The Z axis of the current GameObject's own coordinate system always faces the GameObject")]
        LookAtGameObject,

        /// <summary>
        /// 背向相机
        /// </summary>
        [Name("背向相机")]
        [Tip("当前游戏对象的自身坐标系Z轴始终背向相机；与朝向相机的方向相反", "The Z axis of the current game object's own coordinate system always faces away from the camera; Opposite the direction toward the camera")]
        BackToCamera,

        /// <summary>
        /// 背向游戏对象
        /// </summary>
        [Name("背向游戏对象")]
        [Tip("当前游戏对象的自身坐标系Z轴始终背向游戏对象；与朝向游戏对象的方向相反", "The Z axis of the current game object's own coordinate system always faces away from the game object; Opposite the direction toward the GameObject")]
        BackToGameObject,

        /// <summary>
        /// 与相机朝向对齐
        /// </summary>
        [Name("与相机朝向对齐")]
        [Tip("将当前游戏对象的自身坐标系对齐轴与相机Z轴朝向对齐", "Align the current GameObject's own coordinate system with the camera's Z axis")]
        AlginCameraDirection,

        /// <summary>
        /// 与游戏对象朝向对齐
        /// </summary>
        [Name("与游戏对象朝向对齐")]
        [Tip("将当前游戏对象的自身坐标系对齐轴与游戏对象Z轴朝向对齐", "Align the current GameObject's own coordinate system with the Z axis of the GameObject")]
        AlginGameObjectDirection,

        /// <summary>
        /// 使用射线碰撞检测器
        /// </summary>
        [Name("使用射线碰撞检测器")]
        [Tip("使用射线碰撞检测器提供的焦点作为广告牌的朝向参考点", "Use the focus provided by the ray collision detector as the orientation reference point for the billboard")]
        UseRayHitDetector,
    }

    /// <summary>
    /// 广告牌
    /// </summary>
    [Tool(ToolsCategory.Note)]
    [Name("广告牌")]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    [XCSJ.Attributes.Icon(EIcon.Screen)]
    public class Billboard : InteractProvider
    {
        /// <summary>
        /// 目标变换
        /// </summary>
        [Name("目标变换")]
        public Transform _targetTransform;

        /// <summary>
        /// 目标变换
        /// </summary>
        public Transform targetTransform => this.XGetComponent(ref _targetTransform);

        /// <summary>
        /// 广告牌朝向规则
        /// </summary>
        [Name("广告牌朝向规则")]
        [EnumPopup]
        public EBillboardLookatRule _billboardLookatRule = EBillboardLookatRule.LookAtCamera;

        /// <summary>
        /// BB树模式
        /// </summary>
        public enum EBillboardTreeMode
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 面向相机
            /// </summary>
            [Name("面向相机")]
            FaceToCamera,

            /// <summary>
            /// 面向相机基于正反面
            /// </summary>
            [Name("面向相机基于正反面")]
            [Tip("程序初始化方向为正面，绕Y轴旋转180度为反面；本对象以正面或反面面对相机", "The program initialization direction is positive, and rotating 180 degrees with Y axls is negative; This object faces the camera in front or back")]
            FaceToCameraByFrontOrBack
        }

        /// <summary>
        /// BB树模式
        /// </summary>
        [Name("BB树模式")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_billboardLookatRule), EValidityCheckType.NotEqual, EBillboardLookatRule.BillboardTree)]
        public EBillboardTreeMode _billboardTreeMode = EBillboardTreeMode.FaceToCamera;

        /// <summary>
        /// 对齐轴
        /// </summary>
        [Name("对齐轴")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_billboardLookatRule), EValidityCheckType.Equal, EBillboardLookatRule.None)]
        public EAlginAxis _alginAxis = EAlginAxis.Z_P;

        /// <summary>
        /// 对齐轴
        /// </summary>
        public enum EAlginAxis
        {
            /// <summary>
            /// X正轴
            /// </summary>
            [Name("X正轴")]
            X_P,

            /// <summary>
            /// X负轴
            /// </summary>
            [Name("X负轴")]
            X_N,

            /// <summary>
            /// Y正轴
            /// </summary>
            [Name("Y正轴")]
            Y_P,

            /// <summary>
            /// Y负轴
            /// </summary>
            [Name("Y负轴")]
            Y_N,

            /// <summary>
            /// Z正轴
            /// </summary>
            [Name("Z正轴")]
            Z_P,

            /// <summary>
            /// Z负轴
            /// </summary>
            [Name("Z负轴")]
            Z_N
        }

        /// <summary>
        /// 朝向对象
        /// </summary>
        [Name("朝向对象")]
        [Tip("根据广告牌类型不同，本参数有不同的解释", "This parameter has different explanations according to different types of billboards")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_billboardLookatRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EBillboardLookatRule.LookAtGameObject, nameof(_billboardLookatRule), EValidityCheckType.NotEqual, EBillboardLookatRule.BackToGameObject)]
        public GameObject lookAtObject;

        /// <summary>
        /// 射线碰撞检测器
        /// </summary>
        [Name("射线碰撞检测器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_billboardLookatRule), EValidityCheckType.NotEqual, EBillboardLookatRule.UseRayHitDetector)]
        public RayHitDetector _rayHitDetector;

        /// <summary>
        /// 启用自动缩放
        /// </summary>
        [Name("启用自动缩放")]
        public bool isScaleEnable = false;

        /// <summary>
        /// 缩放系数
        /// </summary>
        [Name("缩放系数")]
        [Range(0.001f, 100)]
        [HideInSuperInspector(nameof(isScaleEnable), EValidityCheckType.Equal, false)]
        public float scaleRatio = 0.1f;

        /// <summary>
        /// 跟随规则
        /// </summary>
        public enum EFollowRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 当启用
            /// </summary>
            [Name("当启用")]
            OnEnable,

            /// <summary>
            /// 当更新
            /// </summary>
            [Name("当更新")]
            OnUpdate,
        }

        /// <summary>
        /// 跟随规则
        /// </summary>
        [Name("跟随规则")]
        [EnumPopup]
        public EFollowRule _followRule = EFollowRule.None;

        /// <summary>
        /// 跟随变换规则
        /// </summary>
        public enum EFollowTransformRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 当前相机
            /// </summary>
            [Name("当前相机")]
            CurrentCamera,

            /// <summary>
            /// 固定
            /// </summary>
            [Name("固定")]
            Fixed,
        }

        /// <summary>
        /// 跟随变换规则
        /// </summary>
        [Name("跟随变换规则")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_followRule), EValidityCheckType.Equal, EFollowRule.None)]
        public EFollowTransformRule _followTransformRule = EFollowTransformRule.CurrentCamera;

        /// <summary>
        /// 跟随变换
        /// </summary>
        [Name("跟随变换")]
        [HideInSuperInspector(nameof(_followRule), EValidityCheckType.Equal | EValidityCheckType.Or, EFollowRule.None, nameof(_followTransformRule), EValidityCheckType.NotEqual, EFollowTransformRule.Fixed)]
        public TransformPropertyValue _followTransform = new TransformPropertyValue();

        /// <summary>
        /// 跟随本地偏移量
        /// </summary>
        [Name("跟随本地偏移量")]
        [HideInSuperInspector(nameof(_followRule), EValidityCheckType.Equal, EFollowRule.None)]
        public Vector3PropertyValue _followLocalOffset =new Vector3PropertyValue();

        /// <summary>
        /// 跟随本地偏移量X
        /// </summary>
        public float followLocalOffsetX
        {
            get => _followLocalOffset.propertyValue.x;
            set
            {
                var tmp = _followLocalOffset.propertyValue;
                tmp.x = value;
                _followLocalOffset.propertyValue = tmp;
            }
        }

        /// <summary>
        /// 跟随本地偏移量Y
        /// </summary>
        public float followLocalOffsetY
        {
            get => _followLocalOffset.propertyValue.y;
            set
            {
                var tmp = _followLocalOffset.propertyValue;
                tmp.y = value;
                _followLocalOffset.propertyValue = tmp;
            }
        }

        /// <summary>
        /// 跟随本地偏移量Z
        /// </summary>
        public float followLocalOffsetZ
        {
            get => _followLocalOffset.propertyValue.z;
            set
            {
                var tmp = _followLocalOffset.propertyValue;
                tmp.z = value;
                _followLocalOffset.propertyValue = tmp;
            }
        }

        /// <summary>
        /// 初始化缩放值
        /// </summary>
        private Vector3 initScaleValue;

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            initScaleValue = transform.localScale;
        }

        private Vector3 forwardOnEnable = Vector3.forward;

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            switch (_followRule)
            {
                case EFollowRule.OnEnable:
                    {
                        Follow();
                        break;
                    }
            }

            forwardOnEnable = transform.forward;
        }

        /// <summary>
        /// 延时更新
        /// </summary>
        protected void Update()
        {
            switch (_followRule)
            {
                case EFollowRule.OnUpdate:
                    {
                        Follow();
                        break;
                    }
            }
            Align();
        }

        private void Align()
        {

            Vector3 pos = Vector3.zero;
            Vector3 dir = Vector3.forward;
            switch (_billboardLookatRule)
            {
                case EBillboardLookatRule.None: break;
                case EBillboardLookatRule.LookAtCamera:
                case EBillboardLookatRule.BackToCamera:
                case EBillboardLookatRule.BillboardTree:
                case EBillboardLookatRule.AlginCameraDirection:
                    {
                        var go = CameraHelperExtension.currentCamera?.gameObject;
                        if (go)
                        {
                            pos = go.transform.position;
                            dir = go.transform.forward;
                        }
                        break;
                    }
                case EBillboardLookatRule.LookAtGameObject:
                case EBillboardLookatRule.BackToGameObject:
                case EBillboardLookatRule.AlginGameObjectDirection:
                    {
                        if (lookAtObject)
                        {
                            pos = lookAtObject.transform.position;
                            dir = lookAtObject.transform.forward;
                        }
                        break;
                    }
                case EBillboardLookatRule.UseRayHitDetector:
                    {
                        pos = _rayHitDetector.focusPoint;
                        break;
                    }
                default: return;
            }

            var transform = targetTransform;
            if (isScaleEnable)
            {
                float distance = (transform.position - pos).magnitude;
                var scaleValue = Vector3.one * distance * scaleRatio;
                transform.localScale = Vector3.Scale(initScaleValue, scaleValue);
            }

            switch (_billboardLookatRule)
            {
                case EBillboardLookatRule.LookAtCamera:
                case EBillboardLookatRule.LookAtGameObject:
                case EBillboardLookatRule.UseRayHitDetector:
                    {
                        transform.LookAt(pos);
                        break;
                    }
                case EBillboardLookatRule.BillboardTree:
                    {
                        switch (_billboardTreeMode)
                        {
                            case EBillboardTreeMode.FaceToCamera:
                                {
                                    var tmpPos = Vector3.ProjectOnPlane(pos, Vector3.up);
                                    tmpPos.y += transform.position.y;
                                    transform.LookAt(tmpPos);
                                    break;
                                }
                            case EBillboardTreeMode.FaceToCameraByFrontOrBack:
                                {
                                    transform.forward = Vector3.Angle(forwardOnEnable, pos - transform.position) > 90 ? -forwardOnEnable : forwardOnEnable;
                                    break;
                                }
                        }
                        break;
                    }
                case EBillboardLookatRule.BackToCamera:
                case EBillboardLookatRule.BackToGameObject:
                    {
                        transform.LookAt(transform.position * 2 - pos);
                        break;
                    }
                case EBillboardLookatRule.AlginCameraDirection:
                case EBillboardLookatRule.AlginGameObjectDirection:
                    {
                        transform.forward = dir;
                        break;
                    }
                default: break;
            }

            switch (_alginAxis)
            {
                case EAlginAxis.X_P:
                    {
                        transform.Rotate(new Vector3(0, -90, 0));
                        break;
                    }
                case EAlginAxis.X_N:
                    {
                        transform.Rotate(new Vector3(0, 90, 0));
                        break;
                    }
                case EAlginAxis.Y_P:
                    {
                        transform.Rotate(new Vector3(90, 0, 0));
                        break;
                    }
                case EAlginAxis.Y_N:
                    {
                        transform.Rotate(new Vector3(-90, 0, 0));
                        break;
                    }
                case EAlginAxis.Z_P:
                    {
                        break;
                    }
                case EAlginAxis.Z_N:
                    {
                        transform.Rotate(new Vector3(0, 180, 0));
                        break;
                    }
            }
        }

        private void Follow()
        {
            Transform ft = null;
            switch (_followTransformRule)
            {
                case EFollowTransformRule.CurrentCamera:
                    {
                        var cam = CameraHelperExtension.currentCamera;
                        if (cam)
                        {
                            ft = cam.transform;
                        }
                        break;
                    }
                case EFollowTransformRule.Fixed:
                    {
                        if (_followTransform.TryGetValue(out var t))
                        {
                            ft = t;
                        }
                        break;
                    }
            }
            if (ft)
            {
                targetTransform.position = ft.position;
                if (_followLocalOffset.TryGetValue(out var localOffset))
                {
                    targetTransform.position += ft.forward * localOffset.z + ft.right * localOffset.x + ft.up * localOffset.y;
                }
            }
        }
    }
}
