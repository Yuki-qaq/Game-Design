using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;

namespace XCSJ.EditorExtension.Base.XAssets.Libs
{
    /// <summary>
    /// 编辑器常量
    /// </summary>
    public static class EditorConst
    {
        /// <summary>
        /// 纹理扩展名
        /// </summary>
        public static string[] TextureExts = { ".tga", ".png", ".jpg", ".tif", ".psd", ".exr" };

        /// <summary>
        /// 音频扩展名
        /// </summary>
        public static string[] AudioExts = { ".mp3", ".wma", ".rm", ".wav", ".midi", ".ape", ".flac" };

        /// <summary>
        /// 材质扩展名
        /// </summary>
        public static string[] MaterialExts = { ".mat" };

        /// <summary>
        /// 模型扩展名
        /// </summary>
        public static string[] ModelExts = { ".fbx", ".asset", ".obj" };

        /// <summary>
        /// 动画扩展名
        /// </summary>
        public static string[] AnimationExts = { ".anim" };

        /// <summary>
        /// 元扩展名
        /// </summary>
        public static string[] MetaExts = { ".meta" };

        /// <summary>
        /// 着色器扩展名
        /// </summary>
        public static string[] ShaderExts = { ".shader" };

        /// <summary>
        /// 脚本扩展名
        /// </summary>
        public static string[] ScriptExts = { ".cs" };

        /// <summary>
        /// 预制体扩展名
        /// </summary>
        public static string[] PrefabExts = { ".prefab" };

        /// <summary>
        /// 物理材质扩展名
        /// </summary>
        public static string[] PhysicMaterialExts = { ".physicMaterial" };

        /// <summary>
        /// 2D物理材质扩展名
        /// </summary>
        public static string[] PhysicMaterial2DExts = { ".physicMaterial2D" };

        /// <summary>
        /// 资产扩展名
        /// </summary>
        public static string[] AssetExts = { ".asset" };

        /// <summary>
        /// 音频混合器扩展名
        /// </summary>
        public static string[] AudioMixerExts = { ".mixer" };

        /// <summary>
        /// 光晕扩展名
        /// </summary>
        public static string[] FlareExts = { ".flare" };

        /// <summary>
        /// 渲染纹理扩展名
        /// </summary>
        public static string[] RenderTextureExts = { ".renderTexture" };

        /// <summary>
        /// 光照参数扩展名
        /// </summary>
        public static string[] LightmapParametersExts = { ".giparams" };

        /// <summary>
        /// 精灵图谱扩展名
        /// </summary>
        public static string[] SpriteAtlasExts = { ".spriteatlas" };

        /// <summary>
        /// 动画控制器扩展名
        /// </summary>
        public static string[] AnimatorControllerExts = { ".controller" };

        /// <summary>
        /// 动画覆盖控制器扩展名
        /// </summary>
        public static string[] AnimatorOverrideControllerExts = { ".overrideController" };

        /// <summary>
        /// 化身遮罩扩展名
        /// </summary>
        public static string[] AvatarMaskExts = { ".mask" };

        /// <summary>
        /// 时间轴扩展名
        /// </summary>
        public static string[] TimelineExts = { ".playable" };

        /// <summary>
        /// GUI皮肤扩展名
        /// </summary>
        public static string[] GUISkinExts = { ".guiskin" };

        /// <summary>
        /// 字体扩展名
        /// </summary>
        public static string[] FontExts = { ".fontsettings", ".TTF" };

        /// <summary>
        /// 立方体贴图扩展名
        /// </summary>
        public static string[] CubemapExts = { ".cubemap" };

        /// <summary>
        /// 画笔扩展名
        /// </summary>
        public static string[] BrushExts = { ".brush" };

        /// <summary>
        /// 地形层扩展名
        /// </summary>
        public static string[] TerrainLayerExts = { ".terrainlayer" };

        /// <summary>
        /// Unity包扩展名
        /// </summary>
        public static string[] UnityPackageExts = { ".unitypackage" };

        /// <summary>
        /// 场景扩展名
        /// </summary>
        public static string[] SceneExts = { ".unity" };

        /// <summary>
        /// 安卓平台
        /// </summary>
        public static string PlatformAndroid = "Android";

        /// <summary>
        /// IOS平台
        /// </summary>
        public static string PlatformIos = "iPhone";

        /// <summary>
        /// 独立平台
        /// </summary>
        public static string PlatformStandalones = "Standalones";

        /// <summary>
        /// 编辑器动画剪辑名
        /// </summary>
        public static string EDITOR_ANICLIP_NAME = "__preview__Take 001";

        /// <summary>
        /// 编辑器控制名
        /// </summary>
        public static string[] EDITOR_CONTROL_NAMES = {"AnimatorStateMachine",
            "AnimatorStateTransition", "AnimatorState", "AnimatorTransition", "BlendTree" };
    }
}
