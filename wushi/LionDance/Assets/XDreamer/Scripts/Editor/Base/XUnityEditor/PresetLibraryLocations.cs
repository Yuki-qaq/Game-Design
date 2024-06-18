using System.Collections.Generic;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 预设库位置
    /// </summary>
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(PresetLibraryLocations))]
    public class PresetLibraryLocations : LinkType<PresetLibraryLocations>
    {
        #region GetAvailableFilesWithExtensionOnTheHDD

        /// <summary>
        /// 在HDD上获取具有扩展功能的可用文件 方法信息
        /// </summary>
        public static XMethodInfo GetAvailableFilesWithExtensionOnTheHDD_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetAvailableFilesWithExtensionOnTheHDD));

        /// <summary>
        /// 在HDD上获取具有扩展功能的可用文件
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <param name="fileExtensionWithoutDot"></param>
        /// <returns></returns>
        public static List<string> GetAvailableFilesWithExtensionOnTheHDD(PresetFileLocation fileLocation, string fileExtensionWithoutDot)
        {
            return GetAvailableFilesWithExtensionOnTheHDD_MethodInfo.Invoke(null, new object[] { (int)fileLocation, fileExtensionWithoutDot }) as List<string>;
        }

        #endregion
    }
}
