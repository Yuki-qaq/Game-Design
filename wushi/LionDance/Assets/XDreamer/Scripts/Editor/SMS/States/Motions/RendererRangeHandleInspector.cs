using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 渲染区间处理检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RendererRangeHandleInspector<T> : RangeHandleInspector<T> where T : RendererRangeHandle<T>
    {
    }
}
