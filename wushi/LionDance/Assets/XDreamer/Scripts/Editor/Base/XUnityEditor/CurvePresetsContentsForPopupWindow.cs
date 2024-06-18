using System.Reflection;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 屈原预设常量用于弹出窗口
    /// </summary>
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(CurvePresetsContentsForPopupWindow))]
    public class CurvePresetsContentsForPopupWindow : LinkType<CurvePresetsContentsForPopupWindow>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public CurvePresetsContentsForPopupWindow(object obj) : base(obj) { }

        #region m_CurveLibraryEditor

        /// <summary>
        /// 曲线库编辑器 字段信息
        /// </summary>
        public static XFieldInfo m_CurveLibraryEditor_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_CurveLibraryEditor), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 曲线库编辑器
        /// </summary>
        public PresetLibraryEditor_CurvePresetLibrary m_CurveLibraryEditor => new PresetLibraryEditor_CurvePresetLibrary(m_CurveLibraryEditor_FieldInfo.GetValue(obj));

        #endregion

        #region m_CurveLibraryType

        /// <summary>
        /// 曲线库类型 字段信息
        /// </summary>
        public static XFieldInfo m_CurveLibraryType_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_CurveLibraryType), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 曲线库类型
        /// </summary>
        public CurveLibraryType m_CurveLibraryType
        {
            get => (CurveLibraryType)m_CurveLibraryType_FieldInfo.GetValue(obj);
            set => m_CurveLibraryType_FieldInfo.obj?.SetValue(obj, (int)value);
        }

        #endregion

        #region GetExtension

        /// <summary>
        /// 获取扩展 方法信息
        /// </summary>
        public static XMethodInfo GetExtension_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetExtension), BindingFlags.Static| BindingFlags.NonPublic);

        /// <summary>
        /// 获取扩展
        /// </summary>
        /// <param name="curveLibraryType"></param>
        /// <returns></returns>
        public static string GetExtension(CurveLibraryType curveLibraryType) => GetExtension_MethodInfo.Invoke(null, new object[] { (int)curveLibraryType }) as string;

        #endregion
    }
}
