using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Renderers
{
    /// <summary>
    /// 工具渲染器
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class ToolRenderer : InteractProvider
    {
    }
}
