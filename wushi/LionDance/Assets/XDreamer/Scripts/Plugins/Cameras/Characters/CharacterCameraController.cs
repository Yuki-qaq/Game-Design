using XCSJ.Attributes;
using XCSJ.PluginCameras.Characters.Base;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCameras.Base;
using XCSJ.PluginTools;

namespace XCSJ.PluginCameras.Characters
{
    /// <summary>
    /// 角色相机控制器
    /// </summary>
    [Name("角色相机控制器")]
    [XCSJ.Attributes.Icon(EIcon.Authentication)]
    [Tool(CharacterCategory.Title, rootType = typeof(ToolsManager))]
    public class CharacterCameraController : BaseCharacterCameraController
    {
        /// <summary>
        /// 相机控制器
        /// </summary>
        [Name("相机控制器")]
        public BaseCameraMainController _cameraMainController;

        /// <summary>
        /// 相机控制器
        /// </summary>
        public override BaseCameraMainController cameraMainController
        {
            get
            {
                if (!_cameraMainController)
                {
                    _cameraMainController = mainController.GetComponentInChildren<BaseCameraMainController>();
                }
                return _cameraMainController;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!cameraMainController) { }
        }
    }
}
