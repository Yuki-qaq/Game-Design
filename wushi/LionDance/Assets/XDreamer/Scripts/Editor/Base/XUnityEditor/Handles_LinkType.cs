using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 类<see cref="Handles"/>关联类型
    /// </summary>
    [LinkType(typeof(Handles))]
    public class Handles_LinkType : LinkType<Handles_LinkType>
    {
        #region EmitGUIGeometryForCamera

        /// <summary>
        /// 为相机反射GUI图形 方法信息
        /// </summary>
        public static XMethodInfo EmitGUIGeometryForCamera_MethodInfo { get; } = GetXMethodInfo(nameof(EmitGUIGeometryForCamera));

        /// <summary>
        /// 为相机反射GUI图形
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public static void EmitGUIGeometryForCamera(Camera source, Camera dest)
        {
            EmitGUIGeometryForCamera_MethodInfo.Invoke(null, new object[] { source, dest });
        }

        #endregion
    }
}
