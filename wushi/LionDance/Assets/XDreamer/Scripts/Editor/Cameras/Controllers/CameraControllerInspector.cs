using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCameras.Controllers;
using XCSJ.PluginCameras.Tools.Controllers;

namespace XCSJ.EditorCameras.Controllers
{
    /// <summary>
    /// 相机控制器检查器
    /// </summary>
    [Name("相机控制器检查器")]
    [CustomEditor(typeof(CameraController), true)]
    public class CameraControllerInspector : BaseCameraMainControllerInspector<CameraController>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(CameraController)));

        /// <summary>
        /// 相机移动通过鼠标
        /// </summary>
        [Name("相机移动通过鼠标")]
        [Tip("默认通过鼠标在屏幕窗口中位置控制相机的移动", "By default, the movement of the camera is controlled by the position of the mouse in the screen window")]
        CameraMoveByMouse _cameraMoveByMouse;

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if(mb)
            {
                _cameraMoveByMouse = mb.GetComponentInChildren<CameraMoveByMouse>();
            }
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(_cameraMoveByMouse)
            {
                var enabled = _cameraMoveByMouse.enabled;
                var enabledNew = EditorGUILayout.Toggle(TrLabel(nameof(_cameraMoveByMouse)), enabled);
                if (enabledNew != enabled)
                {
                    _cameraMoveByMouse.XSetEnable(enabledNew);
                }
            }

            EditorCameraHelperExtension.DrawCurrentCameraAsInitCamera(() => targetObject);
            EditorCameraHelperExtension.DrawSelectCameraManager();
            EditorCameraHelperExtension.AlignWithView(targetObject.transform);
            CategoryListExtension.DrawVertical(categoryList);
        }
    }
}
