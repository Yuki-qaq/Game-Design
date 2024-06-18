using System;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 与UnityEditor.ScriptableObjectSaveLoadHelper{CurvePresetLibrary}类关联
    /// </summary>
    [LinkType(nameof(StaticMethod_GetLinkType), ELinkTypeMode.StaticMethod_GetLinkType)]
    public class ScriptableObjectSaveLoadHelper_CurvePresetLibrary : ScriptableObjectSaveLoadHelper<ScriptableObjectSaveLoadHelper_CurvePresetLibrary, CurvePresetLibrary>
    {
        /// <summary>
        /// 静态方法获取关联类型
        /// </summary>
        /// <returns></returns>
        public static Type StaticMethod_GetLinkType() => PresetLibraryEditor_CurvePresetLibrary.m_SaveLoadHelper_FieldInfo.obj.FieldType;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public ScriptableObjectSaveLoadHelper_CurvePresetLibrary(object obj) : base(obj) { }
    }
}
