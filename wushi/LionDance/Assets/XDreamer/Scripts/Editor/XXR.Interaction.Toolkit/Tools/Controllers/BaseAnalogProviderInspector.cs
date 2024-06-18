using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;

namespace XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 基础模拟提供者检查器
    /// </summary>
    [CustomEditor(typeof(BaseAnalogProvider), true)]
    public class BaseAnalogProviderInspector : BaseAnalogProviderInspector<BaseAnalogProvider>
    {
    }

    /// <summary>
    /// 基础模拟提供者检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseAnalogProviderInspector<T> : MBInspector<T>
        where T : BaseAnalogProvider
    {
    }
}
