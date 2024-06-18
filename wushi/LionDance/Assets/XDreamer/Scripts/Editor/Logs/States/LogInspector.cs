using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;

namespace XCSJ.EditorLogs.States
{
    /// <summary>
    /// 日志检查器
    /// </summary>
    [Name("日志检查器")]
    [CustomEditor(typeof(XCSJ.PluginLogs.States.Log), true)]
    public class LogInspector : StateComponentInspector<XCSJ.PluginLogs.States.Log>
    {
        
    }
}
