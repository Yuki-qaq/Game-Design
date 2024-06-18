using UnityEngine;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Interactions.Tools;
using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
using Unity.XR.CoreUtils;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools
{
    /// <summary>
    /// XR交互器可视化
    /// </summary>
    [Name("XR交互器可视化")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_1_1_OR_NEWER
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_LineVisual)]
#endif
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    public class XRInteractorVisual : Interactor
#if XDREAMER_XR_INTERACTION_TOOLKIT
        , IXRCustomReticleProvider
#endif
    {
        const float k_MinLineWidth = 0.0001f;
        const float k_MaxLineWidth = 0.05f;

        /// <summary>
        /// 线宽度
        /// </summary>
        [Range(k_MinLineWidth, k_MaxLineWidth)]
        [Name("线宽度")]
        [Tip("控制线条的宽度", "Controls the width of the line")]
        public float _lineWidth = 0.02f;

        /// <summary>
        /// Controls the width of the line.
        /// </summary>
        public float lineWidth
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;

#if XDREAMER_XR_INTERACTION_TOOLKIT
                m_PerformSetup = true;
#endif
            }
        }

        /// <summary>
        /// 覆盖交互器线长度
        /// </summary>
        [Name("覆盖交互器线长度")]
        [Tip("一个布尔值，用于控制Unity用于确定线条长度的源", "A boolean value that controls which source Unity uses to determine the length of the line")]
        public bool _overrideInteractorLineLength = true;

        /// <summary>
        /// A boolean value that controls which source Unity uses to determine the length of the line.
        /// Set to <see langword="true"/> to use the Line Length set by this behavior.
        /// Set to <see langword="false"/> to have the length of the line determined by the Interactor.
        /// </summary>
        /// <seealso cref="lineLength"/>
        public bool overrideInteractorLineLength
        {
            get => _overrideInteractorLineLength;
            set => _overrideInteractorLineLength = value;
        }

        /// <summary>
        /// 线长度
        /// </summary>
        [Name("线长度")]
        [Tip("控制替代时的线长度", "Controls the length of the line when overriding")]
        public float _lineLength = 10f;

        /// <summary>
        /// Controls the length of the line when overriding.
        /// </summary>
        /// <seealso cref="overrideInteractorLineLength"/>
        public float lineLength
        {
            get => _lineLength;
            set => _lineLength = value;
        }

        /// <summary>
        /// 宽度曲线
        /// </summary>
        [Name("宽度曲线")]
        [Tip("控制直线从开始到结束的相对宽度", "Controls the relative width of the line from start to end")]
        public AnimationCurve _widthCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        /// <summary>
        /// Controls the relative width of the line from start to end.
        /// </summary>
        public AnimationCurve widthCurve
        {
            get => _widthCurve;
            set
            {
                _widthCurve = value;

#if XDREAMER_XR_INTERACTION_TOOLKIT
                m_PerformSetup = true;
#endif
            }
        }

        /// <summary>
        /// 有效渐变色
        /// </summary>
        [Name("有效渐变色")]
        [Tip("将线的颜色控制为从开始到结束的渐变色，以指示有效状态", "Controls the color of the line as a gradient from start to end to indicate a valid state")]
        public Gradient _validColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.green, 0f), new GradientColorKey(Color.green, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        /// <summary>
        /// Controls the color of the line as a gradient from start to end to indicate a valid state.
        /// </summary>
        public Gradient validColorGradient
        {
            get => _validColorGradient;
            set => _validColorGradient = value;
        }

        /// <summary>
        /// 无效渐变色
        /// </summary>
        [Name("无效渐变色")]
        [Tip("将线的颜色控制为从开始到结束的渐变，以指示无效状态", "Controls the color of the line as a gradient from start to end to indicate an invalid state")]
        public Gradient _invalidColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.red, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        /// <summary>
        /// Controls the color of the line as a gradient from start to end to indicate an invalid state.
        /// </summary>
        public Gradient invalidColorGradient
        {
            get => _invalidColorGradient;
            set => _invalidColorGradient = value;
        }

        /// <summary>
        /// 阻挡渐变色
        /// </summary>
        [Name("阻挡渐变色")]
        [Tip("将线的颜色控制为从开始到结束的渐变，以指示交互程序具有有效目标但选择被阻止的状态", "Controls the color of the line as a gradient from start to end to indicate a state where the interactor has a valid target but selection is blocked")]
        public Gradient _blockedColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.yellow, 0f), new GradientColorKey(Color.yellow, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        /// <summary>
        /// Controls the color of the line as a gradient from start to end to indicate a state where the interactor has
        /// a valid target but selection is blocked.
        /// </summary>
        public Gradient blockedColorGradient
        {
            get => _blockedColorGradient;
            set => _blockedColorGradient = value;
        }

        /// <summary>
        /// 强制选择作为有效状态
        /// </summary>
        [Name("强制选择作为有效状态")]
        [Tip("无论交互程序是否具有任何有效目标，在交互程序选择可交互程序时强制使用有效的状态视觉效果", "Forces the use of valid state visuals while the interactor is selecting an interactable, whether or not the Interactor has any valid targets")]
        public bool _treatSelectionAsValidState;

        /// <summary>
        /// Forces the use of valid state visuals while the interactor is selecting an interactable, whether or not the Interactor has any valid targets.
        /// </summary>
        /// <seealso cref="validColorGradient"/>
        public bool treatSelectionAsValidState
        {
            get => _treatSelectionAsValidState;
            set => _treatSelectionAsValidState = value;
        }

        /// <summary>
        /// 平滑移动
        /// </summary>
        [Name("平滑移动")]
        [Tip("控制渲染的分段是否从目标分段延迟并平滑地跟随目标分段", "Controls whether the rendered segments will be delayed from and smoothly follow the target segments")]
        public bool _smoothMovement;

        /// <summary>
        /// Controls whether the rendered segments will be delayed from and smoothly follow the target segments.
        /// </summary>
        /// <seealso cref="followTightness"/>
        /// <seealso cref="snapThresholdDistance"/>
        public bool smoothMovement
        {
            get => _smoothMovement;
            set => _smoothMovement = value;
        }

        /// <summary>
        /// 跟随紧度
        /// </summary>
        [Name("跟随紧度")]
        [Tip("控制启用【平滑移动】时渲染分段跟随目标分段的速度", "Controls the speed that the rendered segments follow the target segments when Smooth Movement is enabled")]
        public float _followTightness = 10f;

        /// <summary>
        /// Controls the speed that the rendered segments follow the target segments when Smooth Movement is enabled.
        /// </summary>
        /// <seealso cref="smoothMovement"/>
        /// <seealso cref="snapThresholdDistance"/>
        public float followTightness
        {
            get => _followTightness;
            set => _followTightness = value;
        }

        /// <summary>
        /// 吸附阈值距离
        /// </summary>
        [Name("吸附阈值距离")]
        [Tip("控制启用【平滑移动】时两个连续帧处的线点之间的阈值距离，以将渲染的线段捕捉到目标线段", "Controls the threshold distance between line points at two consecutive frames to snap rendered segments to target segments when Smooth Movement is enabled")]
        public float _snapThresholdDistance = 10f;

        /// <summary>
        /// Controls the threshold distance between line points at two consecutive frames to snap rendered segments to target segments when Smooth Movement is enabled.
        /// </summary>
        /// <seealso cref="smoothMovement"/>
        /// <seealso cref="followTightness"/>
        public float snapThresholdDistance
        {
            get => _snapThresholdDistance;
            set => _snapThresholdDistance = value;
        }

        /// <summary>
        /// 有效标线
        /// </summary>
        [Name("有效标线")]
        [Tip("存储有效时出现在行末尾的标线", "Stores the reticle that appears at the end of the line when it is valid")]
        public GameObject _validReticle;

        /// <summary>
        /// Stores the reticle that appears at the end of the line when it is valid.
        /// </summary>
        /// <remarks>
        /// Unity will instantiate it while playing when it is a Prefab asset.
        /// </remarks>
        public GameObject validReticle
        {
            get => _validReticle;
            set
            {
                _validReticle = value;
                if (Application.isPlaying)
                    SetupValidReticle();
            }
        }

        /// <summary>
        /// 无效标线
        /// </summary>
        [Name("无效标线")]
        [Tip("存储无效时出现在行末尾的标线", "Stores the reticle that appears at the end of the line when it is invalid")]
        public GameObject _invalidReticle;

        /// <summary>
        /// Stores the reticle that appears at the end of the line when it is invalid.
        /// </summary>
        /// <remarks>
        /// Unity will instantiate it while playing when it is a Prefab asset.
        /// </remarks>
        public GameObject invalidReticle
        {
            get => _invalidReticle;
            set
            {
                _invalidReticle = value;
                if (Application.isPlaying)
                    SetupInvalidReticle();
            }
        }

        /// <summary>
        /// 阻挡标线
        /// </summary>
        [Name("阻挡标线")]
        [Tip("存储交互程序具有有效目标但选择被阻止时出现在行末尾的标线", "Stores the reticle that appears at the end of the line when the interactor has a valid target but selection is blocked")]
        public GameObject _blockedReticle;

        /// <summary>
        /// Stores the reticle that appears at the end of the line when the interactor has a valid target but selection is blocked.
        /// </summary>
        /// <remarks>
        /// Unity will instantiate it while playing when it is a Prefab asset.
        /// </remarks>
        public GameObject blockedReticle
        {
            get => _blockedReticle;
            set
            {
                _blockedReticle = value;
                if (Application.isPlaying)
                    SetupBlockedReticle();
            }
        }

        /// <summary>
        /// 第一次射线碰撞时的停止线
        /// </summary>
        [Name("第一次射线碰撞时的停止线")]
        [Tip("控制此行为是否始终在第一次光线投射命中时缩短线条，即使无效", "Controls whether this behavior always cuts the line short at the first ray cast hit, even when invalid")]
        public bool _stopLineAtFirstRaycastHit = true;

        /// <summary>
        /// Controls whether this behavior always cuts the line short at the first ray cast hit, even when invalid.
        /// </summary>
        /// <remarks>
        /// The line will always stop short at valid targets, even if this property is set to false.
        /// If you wish this line to pass through valid targets, they must be placed on a different layer.
        /// <see langword="true"/> means to do the same even when pointing at an invalid target.
        /// <see langword="false"/> means the line will continue to the configured line length.
        /// </remarks>
        public bool stopLineAtFirstRaycastHit
        {
            get => _stopLineAtFirstRaycastHit;
            set => _stopLineAtFirstRaycastHit = value;
        }

        /// <summary>
        /// 选择时停止线
        /// </summary>
        [Name("选择时停止线")]
        [Tip("控制线是否会停在交互程序选择的最近的交互程序表的附着点（如果有）", "Controls whether the line will stop at the attach point of the closest interactable selected by the interactor, if there is one")]
        public bool _stopLineAtSelection;

        /// <summary>
        /// Controls whether the line will stop at the attach point of the closest interactable selected by the interactor, if there is one.
        /// </summary>
        public bool stopLineAtSelection
        {
            get => _stopLineAtSelection;
            set => _stopLineAtSelection = value;
        }

        /// <summary>
        /// 如果可获得吸附终点
        /// </summary>
        [Name("如果可获得吸附终点")]
        [Tip("控制如果光线击中可交互吸附容器，可视化线是否会捕捉端点", "Controls whether the visualized line will snap endpoint if the ray hits a XRInteractableSnapVolume")]
        public bool _snapEndpointIfAvailable = true;

        /// <summary>
        /// 模型根
        /// </summary>
        [Name("模型根")]
        public Transform _modelRoot;

        /// <summary>
        /// 当前模型名称
        /// </summary>
        public string currentModelName
        {
            get
            {
                if (!_modelRoot) return "";

                foreach(Transform t in _modelRoot)
                {
                    if (t.gameObject.activeInHierarchy)
                    {
                        return t.name;
                    }
                }
                return "";
            }
            set
            {
                if (!_modelRoot) return;

                foreach (Transform t in _modelRoot)
                {
                    t.gameObject.SetActive(t.name == value);
                }
            }
        }

        /// <summary>
        /// Controls whether the visualized line will snap endpoint if the ray hits a XRInteractableSnapVolume.
        /// </summary>
        /// <remarks>
        /// Currently snapping only works with an <see cref="XRRayInteractor"/>.
        /// </remarks>
        public bool snapEndpointIfAvailable
        {
            get => _snapEndpointIfAvailable;
            set => _snapEndpointIfAvailable = value;
        }


        Vector3 m_ReticlePos;
        Vector3 m_ReticleNormal;
        int m_EndPositionInLine;

#if XDREAMER_XR_INTERACTION_TOOLKIT
        bool m_SnapCurve = true;
        bool m_PerformSetup;
#endif
        GameObject m_ReticleToUse;

        LineRenderer m_LineRenderer;


#if XDREAMER_XR_INTERACTION_TOOLKIT

        // interface to get target point
        ILineRenderable m_LineRenderable;
        IXRSelectInteractor m_LineRenderableAsSelectInteractor;
        XRBaseInteractor m_LineRenderableAsBaseInteractor;
        XRRayInteractor m_LineRenderableAsRayInteractor;
#endif

        // reusable lists of target points
        Vector3[] m_TargetPoints;
        int m_NoTargetPoints = -1;

        // reusable lists of rendered points
        Vector3[] m_RenderPoints;

#if XDREAMER_XR_INTERACTION_TOOLKIT
        int m_NoRenderPoints = -1;
#endif

        // reusable lists of rendered points to smooth movement
        Vector3[] m_PreviousRenderPoints;
#if XDREAMER_XR_INTERACTION_TOOLKIT
        int m_NoPreviousRenderPoints = -1;
#endif

        readonly Vector3[] m_ClearArray = { Vector3.zero, Vector3.zero };

        GameObject m_CustomReticle;
#if XDREAMER_XR_INTERACTION_TOOLKIT
        bool m_CustomReticleAttached;
#endif

        // Snapping 
        bool m_Snapping;
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_3_2_OR_NEWER
        XRInteractableSnapVolume m_XRInteractableSnapVolume;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT
        int m_NumberOfSegmentsForBendableLine = 20;
#endif

        // List of raycast points from m_LineRenderable
        Vector3[] m_LineRenderablePoints = Array.Empty<Vector3>();

        // Most recent hit information
        Vector3 m_CurrentHitPoint;
        bool m_HasHitInfo;
        bool m_ValidHit;

        // The position at which we want to render the end point (used for bending ray visuals)
        Vector3 m_CurrentRenderEndpoint;
        // Previously hit collider 
        Collider m_PreviousCollider;

#if XDREAMER_XR_INTERACTION_TOOLKIT

        XROrigin m_XROrigin;

        /// <summary>
        /// Cached reference to an <see cref="XROrigin"/> found with <see cref="UnityEngine.Object.FindObjectOfType{Type}()"/>.
        /// </summary>
        static XROrigin s_XROriginCache;

#endif

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            // Don't need to do anything; method kept for backwards compatibility.
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnValidate()
        {
            if (Application.isPlaying)
                UpdateSettings();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            m_LineRenderable = GetComponent<ILineRenderable>();
            if (m_LineRenderable != null)
            {
                m_LineRenderableAsBaseInteractor = m_LineRenderable as XRBaseInteractor;
                m_LineRenderableAsSelectInteractor = m_LineRenderable as IXRSelectInteractor;
                m_LineRenderableAsRayInteractor = m_LineRenderable as XRRayInteractor;
            }
#endif

            FindXROrigin();
            SetupValidReticle();
            SetupInvalidReticle();
            SetupBlockedReticle();
            ClearLineRenderer();
            UpdateSettings();
        }

        private AnalogController analogController;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

#if XDREAMER_XR_INTERACTION_TOOLKIT
            m_SnapCurve = true;
#endif
            if (m_ReticleToUse != null)
            {
                m_ReticleToUse.SetActive(false);
                m_ReticleToUse = null;
            }

            Application.onBeforeRender += OnBeforeRenderLineVisual;

            analogController = GetComponent<AnalogController>();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_LineRenderer != null)
                m_LineRenderer.enabled = false;

            if (m_ReticleToUse != null)
            {
                m_ReticleToUse.SetActive(false);
                m_ReticleToUse = null;
            }

            Application.onBeforeRender -= OnBeforeRenderLineVisual;
        }

        void ClearLineRenderer()
        {
            if (TryFindLineRenderer())
            {
                m_LineRenderer.SetPositions(m_ClearArray);
                m_LineRenderer.positionCount = 0;
            }
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT

        [BeforeRenderOrder(XRInteractionUpdateOrder.k_BeforeRenderLineVisual)]
#endif
        void OnBeforeRenderLineVisual()
        {
            UpdateLineVisual();
        }

        /// <summary>
        /// Tries to get the hit info from the current <see cref="ILineRenderable"/>
        /// </summary>
        /// <returns>Returns whether or not we have a valid hit in this current frame.</returns>
        bool UpdateCurrentHitInfo()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            m_LineRenderable.GetLinePoints(ref m_LineRenderablePoints, out _);

            Collider hitCollider = null;
            m_Snapping = false;

            if (m_LineRenderablePoints.Length < 1)
                return false;

            if (m_LineRenderable.TryGetHitInfo(out m_CurrentHitPoint, out m_ReticleNormal, out m_EndPositionInLine, out m_ValidHit))
            {
                m_HasHitInfo = true;
                m_CurrentRenderEndpoint = m_CurrentHitPoint;
                if (m_ValidHit && _snapEndpointIfAvailable && m_LineRenderableAsRayInteractor != null && !m_LineRenderableAsRayInteractor.hasSelection)
                {
                    // When hovering a new collider, check if it has a specified snapping volume, if it does then get the closest point on it
                    if (m_LineRenderableAsRayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit, out _))
                        hitCollider = raycastHit.collider;

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_3_2_OR_NEWER
                    if (hitCollider != m_PreviousCollider && hitCollider != null)
                        m_LineRenderableAsBaseInteractor.interactionManager.TryGetInteractableForCollider(hitCollider, out _, out m_XRInteractableSnapVolume);

                    if (m_XRInteractableSnapVolume != null)
                    {
                        m_CurrentRenderEndpoint = m_XRInteractableSnapVolume.GetClosestPoint(m_CurrentRenderEndpoint);
                        m_EndPositionInLine = m_NumberOfSegmentsForBendableLine - 1; // Override hit index because we're going to use a custom line where the hit point is the end
                        m_Snapping = true;
                    }
#endif
                }
                if (hitCollider == null)
                {
                    if (m_LineRenderableAsRayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit, out _))
                        hitCollider = raycastHit.collider;
                }
            }
            else
            {
                m_CurrentRenderEndpoint = (m_LineRenderablePoints.Length > 0) ? m_LineRenderablePoints[m_LineRenderablePoints.Length - 1] : Vector3.zero;
            }

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_3_2_OR_NEWER
            if (hitCollider == null)
                m_XRInteractableSnapVolume = null;
#endif

            m_PreviousCollider = hitCollider;
#endif
            return m_ValidHit;
        }

        /// <summary>
        /// Calculates the target render points based on the targeted snapped endpoint and the actual position of the raycast line.  
        /// </summary>
        void CalculateSnapRenderPoints()
        {
            var startPosition = m_LineRenderablePoints.Length > 0 ? m_LineRenderablePoints[0] : Vector3.zero;
            var forward = Vector3.Normalize(m_CurrentHitPoint - startPosition);
            var straightLineEndPoint = startPosition + forward * Vector3.Distance(m_CurrentRenderEndpoint, startPosition);

            var normalizedPointValue = 0f;
            var increment = 1f / (m_NoTargetPoints - 1);

            for (var i = 0; i < m_NoTargetPoints; i++)
            {
                var manipToEndPoint = Vector3.LerpUnclamped(startPosition, m_CurrentRenderEndpoint, normalizedPointValue);
                var manipToAnchor = Vector3.LerpUnclamped(startPosition, straightLineEndPoint, normalizedPointValue);
                m_TargetPoints[i] = Vector3.LerpUnclamped(manipToAnchor, manipToEndPoint, normalizedPointValue);
                normalizedPointValue += increment;
            }
        }

        internal void UpdateLineVisual()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            UpdateCurrentHitInfo();

            if (m_PerformSetup)
            {
                UpdateSettings();
                m_PerformSetup = false;
            }

            if (m_LineRenderer == null)
                return;

            if (m_LineRenderer.useWorldSpace && m_XROrigin != null)
            {
                // Update line width with user scale
                var xrOrigin = m_XROrigin.Origin;
                var userScale = xrOrigin != null ? xrOrigin.transform.localScale.x : 1f;
                m_LineRenderer.widthMultiplier = userScale * Mathf.Clamp(_lineWidth, k_MinLineWidth, k_MaxLineWidth);
            }

            if (m_LineRenderable == null)
            {
                m_LineRenderer.enabled = false;
                return;
            }

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_3_2_OR_NEWER
            if (m_LineRenderableAsBaseInteractor != null &&
                m_LineRenderableAsBaseInteractor.disableVisualsWhenBlockedInGroup &&
                m_LineRenderableAsBaseInteractor.IsBlockedByInteractionWithinGroup())
            {
                m_LineRenderer.enabled = false;
                return;
            }
#endif

            m_NoRenderPoints = 0;

            // Get all the line sample points from the ILineRenderable interface
            if (!m_LineRenderable.GetLinePoints(ref m_TargetPoints, out m_NoTargetPoints))
            {
                m_LineRenderer.enabled = false;
                ClearLineRenderer();
                return;
            }

            // If we're snapping, override the target points from the m_LineRenderable and use custom point data
            if (m_Snapping)
            {
                m_NoTargetPoints = m_NumberOfSegmentsForBendableLine;
                if (m_TargetPoints == null || m_TargetPoints.Length < m_NumberOfSegmentsForBendableLine)
                    m_TargetPoints = new Vector3[m_NumberOfSegmentsForBendableLine];
                CalculateSnapRenderPoints();
            }

            // Sanity check.
            if (m_TargetPoints == null ||
                m_TargetPoints.Length == 0 ||
                m_NoTargetPoints == 0 ||
                m_NoTargetPoints > m_TargetPoints.Length)
            {
                m_LineRenderer.enabled = false;
                ClearLineRenderer();
                return;
            }

            // Make sure we have the correct sized arrays for everything.
            if (m_RenderPoints == null || m_RenderPoints.Length < m_NoTargetPoints)
            {
                m_RenderPoints = new Vector3[m_NoTargetPoints];
                m_PreviousRenderPoints = new Vector3[m_NoTargetPoints];
                m_NoRenderPoints = 0;
                m_NoPreviousRenderPoints = 0;
            }

            // Unchanged
            // If there is a big movement (snap turn, teleportation), snap the curve
            if (m_NoPreviousRenderPoints != m_NoTargetPoints)
            {
                m_SnapCurve = true;
            }
            else
            {
                // Compare the two endpoints of the curve, as that will have the largest delta.
                if (m_PreviousRenderPoints != null &&
                    m_NoPreviousRenderPoints > 0 &&
                    m_NoPreviousRenderPoints <= m_PreviousRenderPoints.Length &&
                    m_TargetPoints != null &&
                    m_NoTargetPoints > 0 &&
                    m_NoTargetPoints <= m_TargetPoints.Length)
                {
                    var prevPointIndex = m_NoPreviousRenderPoints - 1;
                    var currPointIndex = m_NoTargetPoints - 1;
                    if (Vector3.Distance(m_PreviousRenderPoints[prevPointIndex], m_TargetPoints[currPointIndex]) > _snapThresholdDistance)
                    {
                        m_SnapCurve = true;
                    }
                }
            }

            // If the line hits, insert reticle position into the list for smoothing.
            // Remove the last point in the list to keep the number of points consistent.
            if (m_HasHitInfo)
            {
                m_ReticlePos = m_CurrentRenderEndpoint;

                // End the line at the current hit point.
                if ((m_ValidHit || _stopLineAtFirstRaycastHit) && m_EndPositionInLine > 0 && m_EndPositionInLine < m_NoTargetPoints)
                {
                    // The hit position might not lie within the line segment, for example if a sphere cast is used, so use a point projected onto the
                    // segment so that the endpoint is continuous with the rest of the curve.
                    var lastSegmentStartPoint = m_TargetPoints[m_EndPositionInLine - 1];
                    var lastSegmentEndPoint = m_TargetPoints[m_EndPositionInLine];
                    var lastSegment = lastSegmentEndPoint - lastSegmentStartPoint;
                    var projectedHitSegment = Vector3.Project(m_ReticlePos - lastSegmentStartPoint, lastSegment);

                    // Don't bend the line backwards
                    if (Vector3.Dot(projectedHitSegment, lastSegment) < 0)
                        projectedHitSegment = Vector3.zero;

                    m_ReticlePos = lastSegmentStartPoint + projectedHitSegment;
                    m_TargetPoints[m_EndPositionInLine] = m_ReticlePos;
                    m_NoTargetPoints = m_EndPositionInLine + 1;
                }
            }

            // Stop line if there is a selection 
            var hasSelection = m_LineRenderableAsSelectInteractor != null && m_LineRenderableAsSelectInteractor.hasSelection;
            if (_stopLineAtSelection && hasSelection)
            {
                // Use the selected interactable closest to the start of the line.
                var interactablesSelected = m_LineRenderableAsSelectInteractor.interactablesSelected;
                var firstPoint = m_TargetPoints[0];
                var closestEndPoint = m_LineRenderableAsSelectInteractor.GetAttachTransform(interactablesSelected[0]).position;
                var closestSqDistance = Vector3.SqrMagnitude(closestEndPoint - firstPoint);
                for (var i = 1; i < interactablesSelected.Count; i++)
                {
                    var endPoint = m_LineRenderableAsSelectInteractor.GetAttachTransform(interactablesSelected[i]).position;
                    var sqDistance = Vector3.SqrMagnitude(endPoint - firstPoint);
                    if (sqDistance < closestSqDistance)
                    {
                        closestEndPoint = endPoint;
                        closestSqDistance = sqDistance;
                    }
                }

                // Only stop at selection if it is closer than the current end point.
                var currentEndSqDistance = Vector3.SqrMagnitude(m_TargetPoints[m_EndPositionInLine] - firstPoint);
                if (closestSqDistance < currentEndSqDistance || m_EndPositionInLine == 0)
                {
                    // Find out where the selection point belongs in the line points. Use the closest target point.
                    var endPositionForSelection = 1;
                    var sqDistanceFromEndPoint = Vector3.SqrMagnitude(m_TargetPoints[endPositionForSelection] - closestEndPoint);
                    for (var i = 2; i < m_NoTargetPoints; i++)
                    {
                        var sqDistance = Vector3.SqrMagnitude(m_TargetPoints[i] - closestEndPoint);
                        if (sqDistance < sqDistanceFromEndPoint)
                        {
                            endPositionForSelection = i;
                            sqDistanceFromEndPoint = sqDistance;
                        }
                        else
                        {
                            break;
                        }
                    }

                    m_EndPositionInLine = endPositionForSelection;
                    m_NoTargetPoints = m_EndPositionInLine + 1;
                    m_ReticlePos = closestEndPoint;
                    m_ReticleNormal = Vector3.Normalize(m_TargetPoints[m_EndPositionInLine - 1] - m_ReticlePos);
                    m_TargetPoints[m_EndPositionInLine] = m_ReticlePos;
                }
            }

            if (_smoothMovement && (m_NoPreviousRenderPoints == m_NoTargetPoints) && !m_SnapCurve)
            {
                // Smooth movement by having render points follow target points
                var length = 0f;
                var maxRenderPoints = m_RenderPoints.Length;
                for (var i = 0; i < m_NoTargetPoints && m_NoRenderPoints < maxRenderPoints; ++i)
                {
                    var smoothPoint = Vector3.Lerp(m_PreviousRenderPoints[i], m_TargetPoints[i], _followTightness * Time.deltaTime);

                    if (_overrideInteractorLineLength)
                    {
                        if (m_NoRenderPoints > 0 && m_RenderPoints.Length > 0)
                        {
                            var segLength = Vector3.Distance(m_RenderPoints[m_NoRenderPoints - 1], smoothPoint);
                            length += segLength;
                            if (length > _lineLength)
                            {
                                var delta = length - _lineLength;
                                // Re-project final point to match the desired length
                                smoothPoint = Vector3.Lerp(m_RenderPoints[m_NoRenderPoints - 1], smoothPoint, delta / segLength);
                                m_RenderPoints[m_NoRenderPoints] = smoothPoint;
                                m_NoRenderPoints++;
                                break;
                            }
                        }

                        m_RenderPoints[m_NoRenderPoints] = smoothPoint;
                        m_NoRenderPoints++;
                    }
                    else
                    {
                        m_RenderPoints[m_NoRenderPoints] = smoothPoint;
                        m_NoRenderPoints++;
                    }
                }
            }
            else
            {
                if (_overrideInteractorLineLength)
                {
                    var length = 0f;
                    var maxRenderPoints = m_RenderPoints.Length;
                    for (var i = 0; i < m_NoTargetPoints && m_NoRenderPoints < maxRenderPoints; ++i)
                    {
                        var newPoint = m_TargetPoints[i];
                        if (m_NoRenderPoints > 0 && m_RenderPoints.Length > 0)
                        {
                            var segLength = Vector3.Distance(m_RenderPoints[m_NoRenderPoints - 1], newPoint);
                            length += segLength;
                            if (length > _lineLength)
                            {
                                var delta = length - _lineLength;
                                // Re-project final point to match the desired length
                                var resolvedPoint = Vector3.Lerp(m_RenderPoints[m_NoRenderPoints - 1], newPoint, 1 - (delta / segLength));
                                m_RenderPoints[m_NoRenderPoints] = resolvedPoint;
                                m_NoRenderPoints++;
                                break;
                            }
                        }

                        m_RenderPoints[m_NoRenderPoints] = newPoint;
                        m_NoRenderPoints++;
                    }
                }
                else
                {
                    Array.Copy(m_TargetPoints, m_RenderPoints, m_NoTargetPoints);
                    m_NoRenderPoints = m_NoTargetPoints;
                }
            }

            var validHover = false;
            var validSelectMaybe = false;
            if (m_PreviousCollider)
            {
                var entity = m_PreviousCollider.GetComponent<InteractableEntity>();
                if (entity != null)
                {
                    validHover = entity.canHover;
                    validSelectMaybe = entity.canSelect;
                }
            }

            // When a straight line has only two points and color gradients have more than two keys,
            // interpolate points between the two points to enable better color gradient effects.
            if (validHover || validSelectMaybe || m_ValidHit || _treatSelectionAsValidState && hasSelection)
            {
                // Use regular valid state visuals unless we are hovering and selection is blocked.
                // We use regular valid state visuals if not hovering because the blocked state does not apply
                // (e.g. we could have a valid target that is UI and therefore not hoverable or selectable as an interactable).
                var useBlockedVisuals = validHover && !validSelectMaybe;

                if (validSelectMaybe) { }
                else if (!useBlockedVisuals && !hasSelection && m_LineRenderableAsBaseInteractor != null && m_LineRenderableAsBaseInteractor.hasHover)
                {
                    var interactionManager = m_LineRenderableAsBaseInteractor.interactionManager;

                    var canSelectSomething = false;
                    foreach (var interactable in m_LineRenderableAsBaseInteractor.interactablesHovered)
                    {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_3_2_OR_NEWER
                        if (interactable is IXRSelectInteractable selectInteractable && interactionManager.IsSelectPossible(m_LineRenderableAsBaseInteractor, selectInteractable))
                        {
                            canSelectSomething = true;
                            break;
                        }
#else
                        if (interactable is IXRSelectInteractable selectInteractable)
                        {
                            canSelectSomething = true;
                            break;
                        }
#endif
                    }

                    useBlockedVisuals = !canSelectSomething;
                }

                m_LineRenderer.colorGradient = useBlockedVisuals ? _blockedColorGradient : _validColorGradient;
                // Set reticle position and show reticle
                var previouslyUsedReticle = m_ReticleToUse;
                var validStateReticle = useBlockedVisuals ? _blockedReticle : _validReticle;
                m_ReticleToUse = m_CustomReticleAttached ? m_CustomReticle : validStateReticle;
                //if (previouslyUsedReticle != null && previouslyUsedReticle != m_ReticleToUse)
                //    previouslyUsedReticle.SetActive(false);

                if (m_ReticleToUse != null)
                {
                    m_ReticleToUse.transform.position = m_ReticlePos;

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_3_2_OR_NEWER
                    var hoverInteractor = m_LineRenderable as IXRHoverInteractor;
                    if (hoverInteractor?.GetOldestInteractableHovered() is IXRReticleDirectionProvider reticleDirectionProvider)
                    {
                        reticleDirectionProvider.GetReticleDirection(hoverInteractor, m_ReticleNormal, out var reticleUp, out var reticleForward);
                        if (reticleForward.HasValue)
                            m_ReticleToUse.transform.rotation = Quaternion.LookRotation(reticleForward.Value, reticleUp);
                        else
                            m_ReticleToUse.transform.up = reticleUp;
                    }
                    else
#endif
                    {
                        m_ReticleToUse.transform.up = m_ReticleNormal;
                    }

                    //m_ReticleToUse.SetActive(true);
                }
                UpdateReticleActiveState(m_ReticleToUse);
            }
            else
            {
                m_LineRenderer.colorGradient = _invalidColorGradient;
                m_ReticleToUse = invalidReticle;
                //if (m_ReticleToUse != null)
                //{
                //    m_ReticleToUse.SetActive(false);
                //    m_ReticleToUse = null;
                //}
                UpdateReticleActiveState(m_ReticleToUse);
            }

            if (m_NoRenderPoints >= 2)
            {
                m_LineRenderer.enabled = true;
                m_LineRenderer.positionCount = m_NoRenderPoints;
                m_LineRenderer.SetPositions(m_RenderPoints);
            }
            else
            {
                m_LineRenderer.enabled = false;
                ClearLineRenderer();
                return;
            }

            // Update previous points
            Array.Copy(m_RenderPoints, m_PreviousRenderPoints, m_NoRenderPoints);
            m_NoPreviousRenderPoints = m_NoRenderPoints;
            m_SnapCurve = false;

#endif
        }

        private void UpdateReticleActiveState(GameObject activeReticle)
        {
            if (validReticle)
            {
                validReticle.SetActive(validReticle == activeReticle);
            }
            if (invalidReticle)
            {
                invalidReticle.SetActive(invalidReticle == activeReticle);
            }
            if (blockedReticle)
            {
                blockedReticle.SetActive(blockedReticle == activeReticle);
            }
        }

        void UpdateSettings()
        {
            if (TryFindLineRenderer())
            {
                m_LineRenderer.widthMultiplier = Mathf.Clamp(_lineWidth, k_MinLineWidth, k_MaxLineWidth);
                m_LineRenderer.widthCurve = _widthCurve;
#if XDREAMER_XR_INTERACTION_TOOLKIT
                m_SnapCurve = true;
#endif
            }
        }

        bool TryFindLineRenderer()
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            if (m_LineRenderer == null)
            {
                Debug.LogWarning("No Line Renderer found for Interactor Line Visual.", this);
                enabled = false;
                return false;
            }
            return true;
        }

        void FindXROrigin()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            if (m_XROrigin != null)
                return;

            if (s_XROriginCache == null)
                s_XROriginCache = FindObjectOfType<XROrigin>();

            m_XROrigin = s_XROriginCache;
#endif
        }

        void SetupValidReticle()
        {
            if (_validReticle == null)
                return;

            // Instantiate if the reticle is a Prefab asset rather than a scene GameObject
            if (!_validReticle.scene.IsValid())
                _validReticle = Instantiate(_validReticle);

            _validReticle.SetActive(false);
        }

        void SetupInvalidReticle()
        {
            if (_invalidReticle == null) return;

            // Instantiate if the reticle is a Prefab asset rather than a scene GameObject
            if (!_invalidReticle.scene.IsValid())
                _invalidReticle = Instantiate(_invalidReticle);

            _invalidReticle.SetActive(false);
        }

        void SetupBlockedReticle()
        {
            if (_blockedReticle == null)
                return;

            // Instantiate if the reticle is a Prefab asset rather than a scene GameObject
            if (!_blockedReticle.scene.IsValid())
                _blockedReticle = Instantiate(_blockedReticle);

            _blockedReticle.SetActive(false);
        }

        /// <inheritdoc />
        public bool AttachCustomReticle(GameObject reticleInstance)
        {
            m_CustomReticle = reticleInstance;
#if XDREAMER_XR_INTERACTION_TOOLKIT
            m_CustomReticleAttached = true;
#endif
            return true;
        }

        /// <inheritdoc />
        public bool RemoveCustomReticle()
        {
            m_CustomReticle = null;
#if XDREAMER_XR_INTERACTION_TOOLKIT
            m_CustomReticleAttached = false;
#endif
            return true;
        }
    }
}
