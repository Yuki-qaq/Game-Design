using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCameras.Characters;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Interfaces;
using XCSJ.Languages;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCameras;
using XCSJ.PluginCameras.Base;
using XCSJ.PluginCameras.Controllers;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 导航图
    /// </summary>
    [Name("导航图")]
    public class MiniMap : View
    {
        /// <summary>
        /// UI根节点
        /// </summary>
        [Group("基础设置", textEN = "Base Settings")]
        [Name("UI根节点")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public UGUIWindow _UGUIWindow = null;

        /// <summary>
        /// 导航图相机控制器
        /// </summary>
        [Name("导航图相机控制器")]
        [Tip("导航图观察相机, 用于导航图像的生成", "The navigation map observation camera is used for the generation of navigation images")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public MiniMapCameraController _miniMapCameraCotroller = null;

        /// <summary>
        /// 导航图相机控制器
        /// </summary>
        public MiniMapCameraController miniMapCameraController => this.XGetComponentInChildren(ref _miniMapCameraCotroller);

        /// <summary>
        /// 导航图相机
        /// </summary>
        public Camera miniMapCamera => _miniMapCameraCotroller ? _miniMapCameraCotroller.linkCamera : default;

        #region 外观设置

        /// <summary>
        /// 导航图类型
        /// </summary>
        [Group("外观设置", textEN = "Appearance Settings", defaultIsExpanded = false)]
        [Name("导航图类型")]
        [EnumPopup]
        public EMiniMapType _minimapType = EMiniMapType.Circle;

        /// <summary>
        /// 导航图类型
        /// </summary>
        [Name("导航图类型")]
        public enum EMiniMapType
        {
            /// <summary>
            /// 圆形
            /// </summary>
            [Name("圆形")]
            Circle,

            /// <summary>
            /// 矩形
            /// </summary>
            [Name("矩形")]
            Rect,
        }

        /// <summary>
        /// 边框
        /// </summary>
        [Name("边框")]
        public Image _border;

        /// <summary>
        /// 遮罩
        /// </summary>
        [Name("遮罩")]
        public Image _mask;

        /// <summary>
        /// 圆形边框
        /// </summary>
        [Name("圆形边框")]
        [HideInSuperInspector(nameof(_minimapType), EValidityCheckType.NotEqual, EMiniMapType.Circle)]
        public Sprite _circleBorder;

        /// <summary>
        /// 圆形遮罩
        /// </summary>
        [Name("圆形遮罩")]
        [HideInSuperInspector(nameof(_minimapType), EValidityCheckType.NotEqual, EMiniMapType.Circle)]
        public Sprite _circleMask;

        /// <summary>
        /// 矩形边框
        /// </summary>
        [Name("矩形边框")]
        [HideInSuperInspector(nameof(_minimapType), EValidityCheckType.NotEqual, EMiniMapType.Rect)]
        public Sprite _rectBorder;

        /// <summary>
        /// 矩形遮罩
        /// </summary>
        [Name("矩形遮罩")]
        [HideInSuperInspector(nameof(_minimapType), EValidityCheckType.NotEqual, EMiniMapType.Rect)]
        public Sprite _rectMask;

        #endregion

        #region 追踪设置

        /// <summary>
        /// 使用当前相机作为玩家
        /// </summary>
        [Group("追踪设置", textEN = "Track Settings", defaultIsExpanded = false)]
        [Name("使用当前相机作为玩家")]
        [Readonly(EEditorMode.Runtime)]
        public bool _useCurrentCamera = true;

        /// <summary>
        /// 玩家，导航图在自动旋转模式下会追随玩家方向
        /// </summary>
        [Name("玩家")]
        [HideInSuperInspector(nameof(_useCurrentCamera), EValidityCheckType.Equal, true)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public Transform _player = null;

        /// <summary>
        /// 禁用玩家子游戏对象作为导航项
        /// </summary>
        [Name("禁用玩家子游戏对象作为导航项")]
        [Tip("勾选时，当玩家游戏对象下的子游戏对象也是导航项时将被禁用；如果当前玩家是相机，同时禁用相机观察对象作为导航项；", "When checked, when the sub game object under the player's game object is also a navigation item, it will be disabled; If the current player is a camera, disable the camera observation object as the navigation item at the same time;")]
        public bool disablePlayerChildrenItem = true;

        /// <summary>
        /// 导航图真正关联玩家
        /// </summary>
        public Transform player { get => _realPlayer; private set => _realPlayer = value; }
        private Transform _realPlayer = null;

        /// <summary>
        /// 玩家UI
        /// </summary>
        [Name("玩家UI")]
        public MiniMapItemData _playerItemData = new MiniMapItemData();

        /// <summary>
        /// 导航图项列表管理
        /// </summary>
        [Name("非玩家项列表")]
        public List<UIItemList> items = new List<UIItemList>();

        #endregion

        /// <summary>
        /// 导航图追踪项与UI 1对1 图
        /// </summary>
        private Dictionary<Transform, MiniMapItemData> _trackedItemMap = new Dictionary<Transform, MiniMapItemData>();

        /// <summary>
        /// 相机管理器提供器
        /// </summary>
        public CameraManagerProvider cameraManagerProvider { get; private set; }

        /// <summary>
        /// 导航图大小
        /// </summary>
        public Vector2 size => _UGUIWindow ? _UGUIWindow.contentSize : Vector2.zero;

        /// <summary>
        /// 启用
        /// </summary>

        [XCSJ.Languages.LanguageTuple("Missing navigation map camera controller component", "缺少【导航图相机控制器】组件")]
        [XCSJ.Languages.LanguageTuple("Missing Camera Manager Provider component", "缺少【相机管理器提供者】组件")]
        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (!miniMapCameraController || !miniMapCamera)
            {
                enabled = false;
                Debug.LogError("Missing navigation map camera controller component".Tr(GetType()));
                return;
            }

            if (CameraManager.instance)
            {
                cameraManagerProvider = CameraManager.instance.cameraManagerProvider as CameraManagerProvider;
            }

            if (!cameraManagerProvider)
            {
                enabled = false;
                Debug.LogError("Missing Camera Manager Provider component".Tr(GetType()));
                return;
            }

            if (!_useCurrentCamera)
            {
                player = _player;
            }

            CameraControllerEvent.onEndSwitch += OnCameraSwitch;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            CameraControllerEvent.onEndSwitch -= OnCameraSwitch;
        }

        private void OnCameraSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            if (!_useCurrentCamera) return;

            var cam = CameraHelperExtension.currentCamera;
            if (cam && cam != miniMapCamera)
            {
                var currentController = cameraManagerProvider.currentCameraController;
                if (currentController)
                {
                    var tf = currentController.transform;
                    // 角色相机情况下，使用角色控制器转换
                    var cc = tf.GetComponentInParent<XCharacterController>();
                    player = cc ? cc.transform : tf;
                }
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        private void Start()
        {
            // 使用导航图存储项数据，创建运行时追踪对象
            items.ForEach(i => i.transforms.ForEach(t => CreateItem(i.itemData, t)));
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (!player) return;

            var contentSize = size;

            // 更新玩家图标
            if (_miniMapCameraCotroller._followPlayerMode != MiniMapCameraController.EFollowPlayerMode.None)
            {
                UpdateItem(_playerItemData, player, contentSize);
            }

            // 更新非玩家图标
            foreach (var kv in _trackedItemMap)
            {
                var item = kv.Key;
                var data = kv.Value;

                // 禁用玩家子游戏对象作为导航项
                if (disablePlayerChildrenItem && (player == item || item.IsChildOf(player)))
                {
                    data._miniMapItemView.gameObject.SetActive(false);
                    continue;
                }

                // 更新导航项
                UpdateItem(data, item, contentSize);
            }
        }

        /// <summary>
        /// 获取导航图点击点UI本地坐标
        /// </summary>
        /// <param name="clickPoint"></param>
        /// <param name="uiLocalPointNormalized"></param>
        /// <returns></returns>
        public bool TryGetClickPointInMap(Vector3 clickPoint, out Vector2 uiLocalPointNormalized)
        {
            uiLocalPointNormalized = default;
            if (!_UGUIWindow) return false;

            var worldCamera = _UGUIWindow.parentCanvas.worldCamera;
            var inContentArea = RectTransformUtility.RectangleContainsScreenPoint(_UGUIWindow.content, clickPoint, worldCamera);

            if (inContentArea && RectTransformUtility.ScreenPointToLocalPointInRectangle(_UGUIWindow.content, clickPoint, worldCamera, out uiLocalPointNormalized))
            {
                uiLocalPointNormalized = Rect.PointToNormalized(_UGUIWindow.content.rect, uiLocalPointNormalized);

                // 超出导航图圆形范围
                if (_minimapType == EMiniMapType.Circle)
                {
                    if ((uiLocalPointNormalized - Vector2.one / 2).magnitude > 0.5)
                    {
                        return false;
                    }
                }
            }
            else
            {
                uiLocalPointNormalized = Vector2.zero;
            }

            return inContentArea;
        }

        #region 导航项操作

        /// <summary>
        /// 更新项的位置与旋转
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="obj"></param>
        /// <param name="uiSize"></param>
        private void UpdateItem(MiniMapItemData itemData, Transform obj, Vector2 uiSize)
        {
            if (!itemData.valid || !obj) return;

            var ui = itemData._miniMapItemView;
            // 同步UI的激活与追踪对象的激活
            var active = obj.gameObject.activeInHierarchy;
            ui.gameObject.SetActive(active);
            if (active)
            {
                // 更新位置
                var point = (Vector2)miniMapCamera.WorldToViewportPoint(obj.position + itemData.positionOffset) - Vector2.one / 2;
                point.Scale(uiSize);
                ui.rectTransform.anchoredPosition = point;

                // 更新旋转
                itemData.UpdateRotation(obj);
            }
        }

        /// <summary>
        /// 创建追踪项对应真实的UI
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public RectTransform CreateItem(RectTransform ui, Transform item) => CreateItem(ui, item, Vector3.zero);

        /// <summary>
        /// 创建追踪项对应真实的UI
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public RectTransform CreateItem(MiniMapItemData itemData, Transform item)
        {
            return itemData._miniMapItemView ? CreateItem(itemData._miniMapItemView.rectTransform, item, itemData.positionOffset, itemData.followRotation, itemData.rotationYOffset) : default;
        }

        /// <summary>
        /// 创建追踪项对应真实的UI
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="item"></param>
        /// <param name="positionOffset"></param>
        /// <param name="followRotation"></param>
        /// <param name="rotationYOffset"></param>
        /// <returns></returns>
        public RectTransform CreateItem(RectTransform ui, Transform item, Vector3 positionOffset, bool followRotation = false, float rotationYOffset = 0)
        {
            if (!ui || !item) return default;

            if (_trackedItemMap.ContainsKey(item)) return default;

            var cloneUI = CloneUI(ui);
            if (cloneUI)
            {
                _trackedItemMap.Add(item, new MiniMapItemData(cloneUI, positionOffset, followRotation, rotationYOffset));
            }
            return cloneUI;
        }

        /// <summary>
        /// 销毁导航项
        /// </summary>
        public bool DestroyItem(Transform item)
        {
            if (_trackedItemMap.TryGetValue(item, out var itemData))
            {
                _trackedItemMap.Remove(item);
                if (itemData._miniMapItemView)
                {
                    itemData._miniMapItemView.gameObject.SetActive(false);
                    Destroy(itemData._miniMapItemView);
                }
            }
            return false;
        }

        /// <summary>
        /// 获取追踪项UI
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public RectTransform GetItemUI(Transform item)
        {
            if (item && _trackedItemMap.TryGetValue(item, out MiniMapItemData itemData))
            {
                return itemData._miniMapItemView.rectTransform;
            }
            return default;
        }

        /// <summary>
        /// 克隆UI
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        private RectTransform CloneUI(RectTransform ui)
        {
            var cloneGameObject = ui.gameObject.XCloneObject();
            if (cloneGameObject)
            {
                cloneGameObject.gameObject.XSetParent(ui.transform.parent);
                cloneGameObject.gameObject.XSetUniqueName(ui.name);
                cloneGameObject.transform.localScale = ui.localScale;
            }

            return cloneGameObject.GetComponent<RectTransform>();
        }

        /// <summary>
        /// 导航图项列表
        /// </summary>
        [Serializable]
        [Name("导航图项列表")]
        public class UIItemList
        {
            /// <summary>
            /// 项数据
            /// </summary>
            [Name("导航数据")]
            public MiniMapItemData itemData;

            /// <summary>
            /// 追踪项列表
            /// </summary>
            [Name("追踪项列表")]
            [OnlyMemberElements]
            public List<Transform> transforms = new List<Transform>();
        }

        #endregion

        #region 导航图方位

        /// <summary>
        /// 方位参考对象
        /// </summary>
        [Group("方位", textEN = "Direction", defaultIsExpanded = false)]
        [Name("方位参考对象")]
        [Tip("采用参考对象的Z轴朝向作为参考方向", "The z-axis orientation of the reference object is used as the reference direction")]
        public Transform _directionReference = null;

        /// <summary>
        /// 方位
        /// </summary>
        [Name("方位")]
        [HideInSuperInspector(nameof(_directionReference), EValidityCheckType.NotNull)]
        public Vector3PropertyValue _directionVector3;

        /// <summary>
        /// 获取方位
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDirection()
        {
            return _directionReference? _directionReference.forward: _directionVector3.GetValue();
        }

        /// <summary>
        /// 获取方位
        /// </summary>
        /// <returns></returns>
        public float GetDirectionYAngle()
        {
            var dir = GetDirection();
            dir.y = 0;
            return Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
        }

        #endregion
    }

    /// <summary>
    /// 导航图组件
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.MiniMap)]
    [Tool(ToolsCategory.MiniMap, nameof(MiniMap), rootType = typeof(XGUIManager))]
    [RequireManager(typeof(XGUIManager))]
    public abstract class MiniMapCompent : Interactor
    {
        /// <summary>
        /// 导航图
        /// </summary>
        [Group("导航图组件设置", textEN = "MiniMap Component Settings")]
        [Name("导航图")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public MiniMap _miniMap;

        /// <summary>
        /// 导航图
        /// </summary>
        public MiniMap miniMap => this.XGetComponentInParentOrGlobal(ref _miniMap);

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (miniMap) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        [XCSJ.Languages.LanguageTuple("Missing navigation map component", "缺少导航图组件")]        protected override void OnEnable()
        {
            base.OnEnable();

            if (!miniMap)
            {
                Debug.LogWarningFormat("Missing navigation map component".Tr());
                enabled = false;
                return;
            }
        }
    }
}
