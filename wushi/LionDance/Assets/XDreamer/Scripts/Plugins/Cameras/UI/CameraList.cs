using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCameras.Base;
using XCSJ.PluginCameras.Controllers;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Windows.ListViews;

namespace XCSJ.PluginCameras.UI
{
    /// <summary>
    /// 相机列表
    /// </summary>
    [Name("相机列表")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(CameraManager))]
    public class CameraList : ListViewModelProvider
    {
        #region 相机设置

        /// <summary>
        /// 相机切换时间
        /// </summary>
        [Name("相机切换时间")]
        [Min(0)]
        public float _duration = 1f;

        /// <summary>
        /// 相机视图尺寸
        /// </summary>
        [Name("相机视图尺寸")]
        public Vector2Int _viewSize = new Vector2Int(256, 256);

        /// <summary>
        /// 相机列表数据源
        /// </summary>
        [Name("相机列表数据源")]
        [EnumPopup]
        public ECameraListDataSource _cameraListDataSource = ECameraListDataSource.All;

        /// <summary>
        /// 相机视图项数据查找规则
        /// </summary>
        public enum ECameraListDataSource
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 全部
            /// </summary>
            [Name("全部")]
            All,

            /// <summary>
            /// 自定义
            /// </summary>
            [Name("自定义")]
            Custom,

            /// <summary>
            /// 除自定义外全部
            /// </summary>
            [Name("除自定义外全部")]
            AllWithoutCustom,
        }

        /// <summary>
        /// 自定义相机列表
        /// </summary>
        [Name("自定义相机列表")]
        [HideInSuperInspector(nameof(_cameraListDataSource), EValidityCheckType.NotEqual | EValidityCheckType.And, ECameraListDataSource.Custom, nameof(_cameraListDataSource), EValidityCheckType.NotEqual, ECameraListDataSource.AllWithoutCustom)]
        public List<CameraModel> _customCameraModels = new List<CameraModel>();

        #endregion

        #region Unity 消息

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            CameraControllerEvent.onEndSwitch += RenderCameraView;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            CameraControllerEvent.onEndSwitch -= RenderCameraView;
        }

        #endregion

        /// <summary>
        /// 预加载数据
        /// </summary>
        protected override IEnumerable<ListViewItemModel> prefabModels
        {
            get 
            {  
                var list = new List<CameraModel>();
                switch (_cameraListDataSource)
                {
                    case ECameraListDataSource.All:
                        {
                            foreach (var c in ComponentCache.GetComponents<BaseCameraMainController>(true))
                            {
                                list.Add(new CameraModel(c, c.cameraOwner!= null ? c.cameraOwner.ownerGameObject.name : "", ToTexture2D(c)));
                            }
                            list.Sort((x,y) => string.Compare(x.title, y.title));
                            break;
                        }
                    case ECameraListDataSource.Custom:
                        {
                            foreach (var model in _customCameraModels)
                            {
                                if (model.unityObject)
                                {
                                    if(!model.texture2D)
                                    {
                                        model.texture2D = ToTexture2D(model.unityObject);
                                    }
                                    list.Add(model);
                                }
                            }
                            break;
                        }
                    case ECameraListDataSource.AllWithoutCustom:
                        {
                            foreach (var c in ComponentCache.GetComponents<BaseCameraMainController>(true))
                            {
                                if (!_customCameraModels.Exists(item => item.unityObject == c))
                                {
                                    list.Add(new CameraModel(c, c.cameraOwner != null ? c.cameraOwner.ownerGameObject.name : "", ToTexture2D(c)));
                                }
                            }
                            break;
                        }
                }
                return list;
            }
        }

        /// <summary>
        /// 视图项数据点击
        /// </summary>
        /// <param name="listViewInteractData"></param>
        internal override void OnClick(ListViewInteractData listViewInteractData)
        {
            if (listViewInteractData.listViewItemModel is CameraModel cameraModel && cameraModel.component is BaseCameraMainController camController && camController)
            {
                if (CameraManager.instance)
                {
                    CameraManager.instance.GetProvider().SwitchCameraController(camController, _duration, null, true);
                }
            }
        }        

        private void RenderCameraView(BaseCameraMainController from, BaseCameraMainController to)
        {
            foreach (var item in listView.displayModels)
            {
                if (item is CameraModel model)
                {
                    model.texture2D = ToTexture2D(model.baseCameraMainController);
                }
            }
        }

        private Texture2D ToTexture2D(BaseCameraMainController cameraController) => cameraController.Render(_viewSize).ToTexture2D();
    }

    /// <summary>
    /// 相机模型
    /// </summary>
    [Name("相机模型")]
    [Serializable]
    public class CameraModel : ComponentModel<BaseCameraMainController>
    {
        /// <summary>
        /// 基础相机主控制器
        /// </summary>
        public BaseCameraMainController baseCameraMainController => unityObject;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CameraModel() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        public CameraModel(BaseCameraMainController component) : base(component) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="texture2D"></param>
        public CameraModel(BaseCameraMainController component, string title, Texture2D texture2D) : base(component, title, texture2D) { }

        /// <summary>
        /// 标题:
        /// 1、首先获取序列化的标题。
        /// 2、1为空时，使用相机控制器拥有者所在游戏对象的名称
        /// 3、2为空时，使用相机控制器所在游戏对象的名称
        /// </summary>
        public override string title
        {
            get
            {
                if (string.IsNullOrEmpty(base.title) && baseCameraMainController && baseCameraMainController.ownerGameObject)
                {
                    base.title = baseCameraMainController.ownerGameObject.name;
                }
                return base.title;
            }
            set => base.title = value;
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public override bool selected => unityObject ? unityObject == CameraManager.instance.GetCurrentCameraController() : base.selected;
    }
}
