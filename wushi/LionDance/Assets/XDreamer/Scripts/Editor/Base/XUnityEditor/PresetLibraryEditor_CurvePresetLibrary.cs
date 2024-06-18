using System;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 与UnityEditor.PresetLibraryEditor+CurvePresetLibrary类关联类
    /// </summary>
    [LinkType(nameof(StaticMethod_GetLinkType), ELinkTypeMode.StaticMethod_GetLinkType)]
    public class PresetLibraryEditor_CurvePresetLibrary : PresetLibraryEditor<PresetLibraryEditor_CurvePresetLibrary, CurvePresetLibrary>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public PresetLibraryEditor_CurvePresetLibrary(object obj) : base(obj) { }

        /// <summary>
        /// 静态方法获取关联类型
        /// </summary>
        /// <returns></returns>
        public static Type StaticMethod_GetLinkType() => CurvePresetsContentsForPopupWindow.m_CurveLibraryEditor_FieldInfo.obj.FieldType;

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override CurvePresetLibrary New(object obj) => new CurvePresetLibrary(obj);

        #region m_SaveLoadHelper

        /// <summary>
        /// 保存加载组手 字段信息
        /// </summary>
        public static XFieldInfo m_SaveLoadHelper_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_SaveLoadHelper), TypeHelper.InstanceNotPublicHierarchy);

        /// <summary>
        /// 保存加载组手
        /// </summary>
        public ScriptableObjectSaveLoadHelper_CurvePresetLibrary m_SaveLoadHelper
        {
            get
            {
                return new ScriptableObjectSaveLoadHelper_CurvePresetLibrary(m_SaveLoadHelper_FieldInfo.GetValue(obj));
            }
        }

        #endregion
    }
}
