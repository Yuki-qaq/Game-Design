using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCameras.Characters;
using XCSJ.EditorCommonUtils;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorCameras.Characters
{
    /// <summary>
    /// 鼠标查看检查器
    /// </summary>
    [Name("鼠标查看检查器")]
    [CustomEditor(typeof(ObsoleteMouseLook), true)]
    public class MouseLookInspector : MBInspector<ObsoleteMouseLook>
    {
    }
}
