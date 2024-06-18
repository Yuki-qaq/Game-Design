using XCSJ.Algorithms;
using XCSJ.Attributes;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 预设文件定位
    /// </summary>
    [Name("预设文件定位")]
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(PresetFileLocation))]
    public enum PresetFileLocation
    {
        /// <summary>
        /// 首选项文件夹
        /// </summary>
        PreferencesFolder,

        /// <summary>
        /// 工程文件夹
        /// </summary>
        ProjectFolder
    }
}
