using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginXGUI.ViewControllers;
using XCSJ.PluginXGUI.Windows.ImageBrowers;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.CNScripts
{
    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    [ScriptEnum(typeof(XGUIManager))]
    public enum EScriptID
    {
        /// <summary>
        /// 结束
        /// </summary>
        _Begin = IDRange.Begin,

        #region XGUI-目录
        /// <summary>
        /// XGUI
        /// </summary>
        [ScriptName(nameof(XGUI), nameof(XGUI), EGrammarType.Category)]
        [ScriptDescription("XGUI的相关脚本目录；")]
        #endregion
        XGUI,

        #region 图片浏览器控制
        /// <summary>
        /// 图片浏览器控制
        /// </summary>
        [ScriptName("图片浏览器控制", nameof(ImageBrowerControl))]
        [ScriptDescription("图片浏览器相关的各种基础控制命令")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "图片浏览器:", typeof(ImageBrower))]
        [ScriptParams(2, EParamType.Combo, "控制命令:", "全屏", "取消全屏", "上一张", "下一张","跳转至指定页")]
        #endregion 
        ImageBrowerControl,

        #region 获取画布控制器渲染模式
        /// <summary>
        /// 获取画布控制器渲染模式
        /// </summary>
        [ScriptName("获取画布控制器渲染模式", nameof(GetCanvasControllerRenderMode))]
        [ScriptDescription("获取画布控制器渲染模式")]
        [ScriptReturn("成功返回 渲染模式 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "画布控制器:", typeof(CanvasController))]
        #endregion 
        GetCanvasControllerRenderMode,

        #region 设置画布控制器渲染模式
        /// <summary>
        /// 设置画布控制器渲染模式
        /// </summary>
        [ScriptName("设置画布控制器渲染模式", nameof(SetCanvasControllerRenderMode))]
        [ScriptDescription("设置画布控制器渲染模式")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "画布控制器:", typeof(CanvasController))]
        [ScriptParams(2, EParamType.Combo, "渲染模式:", "屏幕空间覆盖", "屏幕空间相机", "世界空间")]
        #endregion 
        SetCanvasControllerRenderMode
    }
}
