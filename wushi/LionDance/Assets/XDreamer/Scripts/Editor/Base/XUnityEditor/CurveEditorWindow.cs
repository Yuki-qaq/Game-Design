using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 曲线编辑器窗口
    /// </summary>
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(CurveEditorWindow))]
    public class CurveEditorWindow : EditorWindow_LinkType<CurveEditorWindow>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        private CurveEditorWindow(object obj) : base(obj) { }

        #region instance

        /// <summary>
        /// 实例 属性信息
        /// </summary>
        public static XPropertyInfo instance_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(instance), TypeHelper.StaticPublicHierarchy);

        /// <summary>
        /// 实例
        /// </summary>
        public static CurveEditorWindow instance => new CurveEditorWindow(instance_PropertyInfo.GetValue(null));

        #endregion

        #region currentPresetLibrary

        /// <summary>
        /// 当前预设库 属性信息
        /// </summary>
        public static XPropertyInfo currentPresetLibrary_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(currentPresetLibrary), BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        /// 当前预设库
        /// </summary>
        public string currentPresetLibrary => currentPresetLibrary_PropertyInfo.GetValue(obj) as string;

        #endregion

        #region m_CurvePresets

        /// <summary>
        /// 曲线预设 字段信息
        /// </summary>
        public static XFieldInfo m_CurvePresets_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_CurvePresets), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 曲线预设
        /// </summary>
        public CurvePresetsContentsForPopupWindow m_CurvePresets => new CurvePresetsContentsForPopupWindow(m_CurvePresets_FieldInfo.GetValue(obj));

        /// <summary>
        /// 曲线预设有效
        /// </summary>
        public CurvePresetsContentsForPopupWindow curvePresetsValid
        {
            get
            {
                InitCurvePresets();
                return m_CurvePresets;
            }
        }

        #endregion

        #region InitCurvePresets

        /// <summary>
        /// 初始化曲线预设 方法信息
        /// </summary>
        public static XMethodInfo InitCurvePresets_MethodInfo { get; } = new XMethodInfo(Type, nameof(InitCurvePresets), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 初始化曲线预设
        /// </summary>
        public void InitCurvePresets() => InitCurvePresets_MethodInfo.Invoke(obj, null);

        #endregion

        private void SetCurveLibraryType(CurveLibraryType curveLibraryType)
        {
            curvePresetsValid.m_CurveLibraryEditor.m_SaveLoadHelper.fileExtensionWithoutDot = CurvePresetsContentsForPopupWindow.GetExtension(curveLibraryType);
            //Debug.Log(curvePresetsValid.m_CurveLibraryEditor.m_SaveLoadHelper.fileExtensionWithoutDot);
        }

        /// <summary>
        /// 创建新库
        /// </summary>
        /// <param name="curvePresetLibraryName"></param>
        /// <param name="presetFileLocation"></param>
        /// <param name="curveLibraryType"></param>
        /// <returns></returns>
        public string CreateNewLibrary(string curvePresetLibraryName, PresetFileLocation presetFileLocation, CurveLibraryType curveLibraryType)
        {
            instance.SetCurveLibraryType(curveLibraryType);
            return curvePresetsValid.m_CurveLibraryEditor.CreateNewLibraryCallback(curvePresetLibraryName, presetFileLocation);
        }

        /// <summary>
        /// 显示曲线编辑窗口
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="ranges"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static AnimationCurve ShowCurveEditorWindow(AnimationCurve value, SerializedProperty property, Rect ranges, Color color)
        {
            EditorGUI_LinkType.SetCurveEditorWindowCurve(value, property, color);
            EditorGUI_LinkType.ShowCurvePopup(ranges);

            Event.current.Use();
            GUIUtility.ExitGUI();

            return property != null ? property.animationCurveValue : value;
        }
    }
}
