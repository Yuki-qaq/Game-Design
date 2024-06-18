using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools;

namespace XCSJ.PluginTools.CrossSectionShader
{
    /// <summary>
    /// 使用剖面材质类型
    /// </summary>
    [Name("剖切模式")]
    public enum ECutMode
    {
        /// <summary>
        /// 交集
        /// </summary>
        [Name("交集")]
        Intersection = 0,

        /// <summary>
        /// 并集
        /// </summary>
        [Name("并集")]
        Union
    }

    /// <summary>
    /// 剖面控制器
    /// </summary>
    [Name("剖面控制器")]
    [Tip("剖面控制器将剖面的朝向和位置设置到剖面Shader中", "The section controller sets the orientation and position of the section into the section shader")]
    [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
    public class PlaneCuttingController : Interactor
    {
        /// <summary>
        /// 能否剖切
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            // 交互对象中没有网格渲染器，则认为无法交互
            return interactData.interactable is InteractableEntity entity && entity && entity.GetComponentInChildren<MeshRenderer>() && entity.IsExist<Cuttable>();
        }

        /// <summary>
        /// 切割
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [Name("切割")]
        [InteractCmd]
        [InteractCmdFun(nameof(Cut))]
        public EInteractResult Cut(InteractData interactData)
        {
            CutObjects(interactData.interactable.gameObject);
            return EInteractResult.Success;
        }

        /// <summary>
        /// 还原
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [Name("还原")]
        [InteractCmd]
        [InteractCmdFun(nameof(Recover))]
        public EInteractResult Recover(InteractData interactData)
        {
            RecoverObjects(interactData.interactable.gameObject);
            return EInteractResult.Success;
        }

        /// <summary>
        /// 剖切模式
        /// </summary>
        [Group("基础设置", textEN = "Base Settings", defaultIsExpanded = false)]
        [Name("剖切模式")]
        [EnumPopup]
        public ECutMode _cutMode = ECutMode.Intersection;

        /// <summary>
        /// 三剖面控制器
        /// </summary>
        [Name("三剖面控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ThreePlanesCuttingController threePlanesCuttingController = null;

        /// <summary>
        /// 自动移动三剖面控制器：移动三剖面控制器至被剖对象包围盒中心
        /// </summary>
        [Name("自动移动三剖面控制器")]
        [Tip("移动三剖面控制器至被剖对象包围盒中心")]
        public bool _autoMoveThreePlanesCuttingControllerToBoundsCenterOfCuttedObjs = true;

        /// <summary>
        /// 使用剖面材质类型
        /// </summary>
        public ECutMode cutMode
        {
            get => _cutMode;
            set => _cutMode = value;
        }

        /// <summary>
        /// 整型使用剖面材质类型
        /// </summary>
        public int useCuttingMaterialTypeInt
        {
            get => (int)_cutMode;
            set => _cutMode = (ECutMode)value;
        }

        /// <summary>
        /// 交集剖面材质
        /// </summary>
        [Name("交集剖面材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material cuttingMaterail = null;

        /// <summary>
        /// 并集剖面材质
        /// </summary>
        [Name("并集剖面材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material unionCuttingMaterail = null;

        /// <summary>
        /// 包含子对象
        /// </summary>
        [Group("剖切对象设置",textEN = "Section Object Settings")]
        [Name("包含子对象")]
        public bool includeChildren = true;

        /// <summary>
        /// 包含非激活游戏对象
        /// </summary>
        [Name("包含非激活游戏对象")]
        public bool includeInactiveGameObject = false;

        /// <summary>
        /// 包含未启用渲染器对象
        /// </summary>
        [Name("包含未启用渲染器对象")]
        public bool includeDisableRenderer = false;

        /// <summary>
        /// 剖切对象列表
        /// </summary>
        [Name("剖切对象列表")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        public List<GameObject> cuttedObjects = new List<GameObject>();

        /// <summary>
        /// 排除剖切对象设置
        /// </summary>
        [Group("排除剖切对象设置", textEN = "Exclude Cut Object Settings", defaultIsExpanded = false)]
        [Name("包含子对象")]
        public bool excludeOjbectIncludeChildren = true;

        /// <summary>
        /// 对象列表
        /// </summary>
        [Name("对象列表")]
        [Tip("在这个列表的对象，不会被剖切", "Objects in this list will not be cut")]
        public List<GameObject> excludeOjbects = new List<GameObject>();

        private ECutMode lastCutMode = ECutMode.Intersection;

        private HashSet<GameObject> _excludeOjbectMap = new HashSet<GameObject>();

        /// <summary>
        /// 剖切shader记录器
        /// </summary>
        protected Dictionary<Material, Shader> shaderRecorder = new Dictionary<Material, Shader>();

        /// <summary>
        /// 运行时已剖切对象
        /// </summary>
        public HashSet<GameObject> cuttedObjectsOnRuntime = new HashSet<GameObject>();

        /// <summary>
        /// 启动
        /// </summary>
        protected void Start() { Cut(); }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            // 添加剖切面事件
            ThreePlanesCuttingController.cuttingPlaneActiveChanged += OnCuttingPlaneActiveChanged;

            // 初始化排除对象集合
            _excludeOjbectMap.Clear();
            var objs = excludeOjbects.Distinct();
            _excludeOjbectMap.AddRangeWithDistinct(objs);
            if (excludeOjbectIncludeChildren)
            {
                foreach (var obj in objs)
                {
                    _excludeOjbectMap.AddRangeWithDistinct(CommonFun.GetChildGameObjects(obj));
                }
            }

            lastCutMode = _cutMode;

            foreach (var go in cuttedObjects)
            {
                go.XGetOrAddComponent<Cuttable>();
            }
            CutObjects(cuttedObjects.ToArray());
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            ThreePlanesCuttingController.cuttingPlaneActiveChanged -= OnCuttingPlaneActiveChanged;

            cuttedObjectsOnRuntime.Clear();
            Recover();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (lastCutMode != _cutMode)
            {
                lastCutMode = _cutMode;
                Cut();
            }
        }

        /// <summary>
        /// 当剖面激活已变更
        /// </summary>
        /// <param name="threePlanesCuttingController"></param>
        /// <param name="planeCuttingInfo"></param>
        protected void OnCuttingPlaneActiveChanged(ThreePlanesCuttingController threePlanesCuttingController, PlaneCuttingInfo planeCuttingInfo)
        {
            if (this.threePlanesCuttingController != threePlanesCuttingController || !planeCuttingInfo) return;

            if (planeCuttingInfo.gameObject.activeSelf)
            {
                Cut();
            }
            else
            {
                // 三个剖切平面都没激活，恢复材质Shader
                if (threePlanesCuttingController.planes.ToList().All(p => p && !p.gameObject.activeSelf))
                {
                    Recover();
                }
            }
        }

        /// <summary>
        /// 剖切对象集
        /// </summary>
        /// <param name="cuttedObjs"></param>
        public void CutObjects(params GameObject[] cuttedObjs)
        {
            foreach (var obj in cuttedObjs)
            {
                if (obj)
                {
                    cuttedObjectsOnRuntime.Add(obj);
                }
            }

            // 移动三剖面控制器至被剖对象包围盒中心
            if (_autoMoveThreePlanesCuttingControllerToBoundsCenterOfCuttedObjs)
            {
                if(CommonFun.GetBounds(out var bounds, cuttedObjectsOnRuntime))
                {
                    threePlanesCuttingController.transform.position = bounds.center;
                }
            }
            Cut();
        }

        /// <summary>
        /// 恢复剖切对象
        /// </summary>
        /// <param name="cuttedObjs"></param>
        public void RecoverObjects(params GameObject[] cuttedObjs)
        {
            foreach (var cuttedObj in cuttedObjs)
            {
                if (cuttedObj)
                {
                    cuttedObjectsOnRuntime.Remove(cuttedObj);
                }
            }

            Cut();
        }

        /// <summary>
        /// 应用材质
        /// </summary>
        private Material _applyMaterial
        {
            get
            {
                switch (_cutMode)
                {
                    case ECutMode.Intersection: return cuttingMaterail;
                    case ECutMode.Union: return unionCuttingMaterail;
                    default: throw new NotImplementedException();
                }
            }
        }

        private void Cut()
        {
            // 清除剖面记录器
            Recover();

            // 剖面Shader
            Shader cuttingShader = null;
            if (_applyMaterial && ShaderHelper.GenericThreePlanesBSP.Valid(_applyMaterial))
            {
                cuttingShader = _applyMaterial.shader;
            }
            if (!cuttingShader)
            {
                Debug.LogError(CommonFun.Name(typeof(PlaneCuttingController), nameof(cuttingShader)) + "剖面Shader无效！");
                return;
            }

            // 获取剖面Shader渲染器列表
            var materials = GetMaterials();
            if (materials.Count == 0)
            {
                return;
            }

            // 设置剖面Shader
            foreach (var mat in materials)
            {
                if (!ShaderHelper.GenericThreePlanesBSP.Valid(mat))
                {
                    shaderRecorder.Add(mat, mat.shader);
                    mat.shader = cuttingShader;
                }
            }

            // 设置三剖面控制器
            if (threePlanesCuttingController)
            {
                threePlanesCuttingController.InitShaderProperty();
                threePlanesCuttingController.SetMaterials(materials.ToArray());
                threePlanesCuttingController.UpdatePlaneCuttingInfos();
            }
            else
            {
                Debug.LogError(CommonFun.Name(typeof(PlaneCuttingController), nameof(threePlanesCuttingController)) + "为空对象！");
            }
        }

        private void Recover()
        {
            foreach (var item in shaderRecorder)
            {
                item.Key.shader = item.Value;
            }
            shaderRecorder.Clear();
        }

        private List<Material> GetMaterials()
        {
            // 获取被剖对象渲染器,去除空对象和排除对象
            var validObjects = cuttedObjectsOnRuntime.Where(obj => obj && !_excludeOjbectMap.Contains(obj)).Distinct();

            var materials = new List<Material>();
            foreach (var obj in validObjects)
            {
                var renderers = includeChildren? obj.GetComponentsInChildren<Renderer>(): new Renderer[1] { obj.GetComponent<Renderer>() };
                foreach (var r in renderers)
                {
                    if (r && !_excludeOjbectMap.Contains(r.gameObject))
                    {
                        materials.AddRange(r.materials);
                    }
                }
            }
            return materials;
        }
    }

    /// <summary>
    /// 基础剖面组件
    /// </summary>
    [RequireManager(typeof(ToolsExtensionManager))]
    public abstract class BasePlaneCuttingMB : InteractProvider { }
}

