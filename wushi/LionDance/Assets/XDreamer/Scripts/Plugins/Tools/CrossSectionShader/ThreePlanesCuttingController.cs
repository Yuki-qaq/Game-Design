using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Maths;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.PluginTools.CrossSectionShader
{
    /// <summary>
    /// 三剖面控制器
    /// </summary>
    [Name("三剖面控制器")]
    public class ThreePlanesCuttingController : BasePlaneCuttingMB
    {
        /// <summary>
        /// X剖面
        /// </summary>
        [Name("X剖面")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public PlaneCuttingInfo planeX;

        /// <summary>
        /// Y剖面
        /// </summary>
        [Name("Y剖面")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public PlaneCuttingInfo planeY;

        /// <summary>
        /// Z剖面
        /// </summary>
        [Name("Z剖面")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public PlaneCuttingInfo planeZ;

        /// <summary>
        /// 剖面激活已变更
        /// </summary>
        public static event Action<ThreePlanesCuttingController, PlaneCuttingInfo> cuttingPlaneActiveChanged = null;

        /// <summary>
        /// 面列表
        /// </summary>
        public PlaneCuttingInfo[] planes => new PlaneCuttingInfo[] { planeX, planeY, planeZ };

        /// <summary>
        /// 材质列表
        /// </summary>
        protected Material[] materials { get; set; }

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected void Awake()
        {
            InitShaderProperty();
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected void Start()
        {
            
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            PlaneCuttingInfo.onChanged += OnPlaneCuttingInfoChanged;

            UpdatePlaneCuttingInfos();

            // 检查三个剖面
            if (!planeX)
            {
                Debug.LogError(CommonFun.Name(typeof(ThreePlanesCuttingController), nameof(planeX)) + "为空对象！");
                return;
            }

            if (!planeY)
            {
                Debug.LogError(CommonFun.Name(typeof(ThreePlanesCuttingController), nameof(planeY)) + "为空对象！");
                return;
            }

            if (!planeZ)
            {
                Debug.LogError(CommonFun.Name(typeof(ThreePlanesCuttingController), nameof(planeZ)) + "为空对象！");
                return;
            }            
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            PlaneCuttingInfo.onChanged -= OnPlaneCuttingInfoChanged;

            try
            {
                SetInvalidPlaneCuttingInfos();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            CheckChange();
        }

        private Vector3 lastPosition = Vector3.zero;

        /// <summary>
        /// 当已变更
        /// </summary>
        public static event Action<ThreePlanesCuttingController> onChanged = null;

        /// <summary>
        /// 设置材质
        /// </summary>
        /// <param name="materials"></param>
        public void SetMaterials(Material[] materials) { this.materials = materials; }

        /// <summary>
        /// 检查修改
        /// </summary>
        public void CheckChange()
        {
            // 向量和位置没有变化，则不设置Shader
            if (MathX.ApproximatelyZero((lastPosition - transform.position).sqrMagnitude))
            {
                return;
            }

            CallChanged(transform);
        }

        /// <summary>
        /// 调用已修改
        /// </summary>
        /// <param name="transform"></param>
        protected void CallChanged(Transform transform)
        {
            UpdatePlaneCuttingInfos();

            onChanged?.Invoke(this);

            lastPosition = transform.position;
        }

        /// <summary>
        /// 初始化着色器属性
        /// </summary>
        public void InitShaderProperty()
        {
            if (planeX)
            {
                planeX.SetShaderPropertyName(ShaderHelper.GenericThreePlanesBSP._Plane1Normal, ShaderHelper.GenericThreePlanesBSP._Plane1Position);
            }

            if (planeY)
            {
                planeY.SetShaderPropertyName(ShaderHelper.GenericThreePlanesBSP._Plane2Normal, ShaderHelper.GenericThreePlanesBSP._Plane2Position);
            }

            if (planeZ)
            {
                planeZ.SetShaderPropertyName(ShaderHelper.GenericThreePlanesBSP._Plane3Normal, ShaderHelper.GenericThreePlanesBSP._Plane3Position);
            }
        }

        /// <summary>
        /// 获取激活平面
        /// </summary>
        /// <returns></returns>
        public PlaneCuttingInfo GetActivePlane()
        {
            if (planeX && planeX.isActiveAndEnabled) return planeX;
            if (planeY && planeY.isActiveAndEnabled) return planeY;
            if (planeZ && planeZ.isActiveAndEnabled) return planeZ;

            return null;            
        }

        /// <summary>
        /// 当剖面信息已变更
        /// </summary>
        /// <param name="planeCuttingInfo"></param>
        protected void OnPlaneCuttingInfoChanged(PlaneCuttingInfo planeCuttingInfo)
        {
            UpdatePlaneCuttingInfos();
        }

        /// <summary>
        /// 设置无效的剖面信息
        /// </summary>
        public void SetInvalidPlaneCuttingInfos()
        {
            if (planeX) planeX.SetRendererInfoInvalid();
            if (planeY) planeY.SetRendererInfoInvalid();
            if (planeZ) planeZ.SetRendererInfoInvalid();
        }

        /// <summary>
        /// 更新剖面信息
        /// </summary>
        public void UpdatePlaneCuttingInfos()
        {
            if (materials == null) return;

            var plane = GetActivePlane();

            if (plane)
            {
                UpdatePlaneCuttingInfo(planeX, plane);
                UpdatePlaneCuttingInfo(planeY, plane);
                UpdatePlaneCuttingInfo(planeZ, plane);
            }
            else
            {
                SetInvalidPlaneCuttingInfos();
            }
        }

        private void UpdatePlaneCuttingInfo(PlaneCuttingInfo planeCuttingInfo, PlaneCuttingInfo activePlane)
        {
            if (!planeCuttingInfo) return;
            
            if (planeCuttingInfo.isActiveAndEnabled)
            {
                planeCuttingInfo.SetMaterialInfo(materials);
            }
            // 当前剖面无效，则使用一个有效剖面的向量与位置去设置无效剖面的Shader属性
            else if (activePlane)
            {
                
                planeCuttingInfo.SetMaterialInfo(activePlane.Normal, activePlane.Position, materials);
            }
        }

        /// <summary>
        /// 激活X平面
        /// </summary>
        /// <param name="active"></param>
        public void ActivePlaneX(bool active)
        {
            if (planeX)
            {
                planeX.gameObject.SetActive(active);
                cuttingPlaneActiveChanged?.Invoke(this, planeX);
            }
        }

        /// <summary>
        /// 激活Y平面
        /// </summary>
        /// <param name="active"></param>
        public void ActivePlaneY(bool active)
        {
            if (planeY)
            {
                planeY.gameObject.SetActive(active);
                cuttingPlaneActiveChanged?.Invoke(this, planeY);
            }
        }

        /// <summary>
        /// 激活Z平面
        /// </summary>
        /// <param name="active"></param>
        public void ActivePlaneZ(bool active)
        {
            if (planeZ)
            {
                planeZ.gameObject.SetActive(active);
                cuttingPlaneActiveChanged?.Invoke(this, planeZ);
            }
        }

        /// <summary>
        /// 激活轴
        /// </summary>
        /// <param name="active"></param>
        public void ActiveAixs(bool active)
        {
            if (planeX && planeX.axis) planeX.axis.SetActive(active);
            if (planeY && planeY.axis) planeY.axis.SetActive(active);
            if (planeZ && planeZ.axis) planeZ.axis.SetActive(active);
        }

        /// <summary>
        /// 激活平面
        /// </summary>
        /// <param name="active"></param>
        public void ActivePlane(bool active)
        {
            if (planeX && planeX.plane) planeX.plane.SetActive(active);
            if (planeY && planeY.plane) planeY.plane.SetActive(active);
            if (planeZ && planeZ.plane) planeZ.plane.SetActive(active);
        }

        /// <summary>
        /// 激活剖面
        /// </summary>
        /// <param name="active"></param>
        public void ActiveCuttingPlane(bool active)
        {
            if (planeX && planeX.cuttingPlane) planeX.cuttingPlane.SetActive(active);
            if (planeY && planeY.cuttingPlane) planeY.cuttingPlane.SetActive(active);
            if (planeZ && planeZ.cuttingPlane) planeZ.cuttingPlane.SetActive(active);
        }
    }
}
