using XCSJ.EditorCameras.Controllers;
using XCSJ.EditorTools.Windows;
using XCSJ.PluginCamera;
using XCSJ.PluginCameras.Base;
using XCSJ.PluginCameras.Controllers;

namespace XCSJ.EditorCameras.Tools
{
    /// <summary>
    /// 相机控制器列表查看器
    /// </summary>
    [ToolObjectViewerEditor(typeof(BaseCameraMainController), true)]
    public class CameraControllerListViewer : ToolObjectViewerEditor
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        public override void OnGUI()
        {
            var manager = CameraManager.instance;
            if (manager)
            {
                CameraManagerProviderInspector.DrawCameraControllersInternal(CameraManager.instance.GetProvider(), components);
            }
            else
            {
                base.OnGUI();
            }
        }
    }
}
