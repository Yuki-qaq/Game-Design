using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 编辑器GUI关联类型
    /// </summary>
    [LinkType(typeof(EditorGUI))]
    public class EditorGUI_LinkType : LinkType<EditorGUI_LinkType>
    {
        #region SetCurveEditorWindowCurve

        /// <summary>
        /// 设置曲线编辑器窗口曲线 方法信息
        /// </summary>
        public static XMethodInfo SetCurveEditorWindowCurve_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetCurveEditorWindowCurve), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 设置曲线编辑器窗口曲线
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="color"></param>
        public static void SetCurveEditorWindowCurve(AnimationCurve value, SerializedProperty property, Color color)
        {
            SetCurveEditorWindowCurve_MethodInfo.Invoke(null, new object[] { value, property, color });
        }

        #endregion

        #region ShowCurvePopup

        /// <summary>
        /// 设置曲线弹出方法信息
        /// </summary>
        public static XMethodInfo ShowCurvePopup_MethodInfo { get; } = new XMethodInfo(Type, nameof(ShowCurvePopup), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 设置曲线弹出
        /// </summary>
        /// <param name="ranges"></param>
        public static void ShowCurvePopup(Rect ranges)
        {
            ShowCurvePopup_MethodInfo.Invoke(null, new object[] { ranges });
        }

        #endregion
    }
}
