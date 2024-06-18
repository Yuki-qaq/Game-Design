using System.Reflection;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 与UnityEditor.PresetLibraryEditor{T}泛型类关联
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TPresetLibrary"></typeparam>
    public abstract class PresetLibraryEditor<T, TPresetLibrary> : LinkType<T>
        where T : PresetLibraryEditor<T, TPresetLibrary>
        where TPresetLibrary : PresetLibrary<TPresetLibrary>, new()
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public PresetLibraryEditor(object obj) : base(obj) { }

        #region DeletePreset

        /// <summary>
        /// 删除预设 方法信息
        /// </summary>
        public static XMethodInfo DeletePreset_MethodInfo { get; } = new XMethodInfo(Type, nameof(DeletePreset));

        /// <summary>
        /// 删除预设
        /// </summary>
        /// <param name="presetIndex"></param>
        public void DeletePreset(int presetIndex) => DeletePreset_MethodInfo.Invoke(obj, new object[] { presetIndex });

        #endregion

        #region ReplacePreset

        /// <summary>
        /// 替换预设 方法信息
        /// </summary>
        public static XMethodInfo ReplacePreset_MethodInfo { get; } = new XMethodInfo(Type, nameof(ReplacePreset));

        /// <summary>
        /// 替换预设
        /// </summary>
        /// <param name="presetIndex"></param>
        /// <param name="presetObject"></param>
        public void ReplacePreset(int presetIndex, object presetObject) => ReplacePreset_MethodInfo.Invoke(obj, new object[] { presetIndex, presetObject });

        #endregion

        #region MovePreset

        /// <summary>
        /// 移动预设 方法信息
        /// </summary>
        public static XMethodInfo MovePreset_MethodInfo { get; } = new XMethodInfo(Type, nameof(MovePreset));

        /// <summary>
        /// 移动预设
        /// </summary>
        /// <param name="presetIndex"></param>
        /// <param name="destPresetIndex"></param>
        /// <param name="insertAfterDestIndex"></param>
        public void MovePreset(int presetIndex, int destPresetIndex, bool insertAfterDestIndex) => MovePreset_MethodInfo.Invoke(obj, new object[] { presetIndex, destPresetIndex, insertAfterDestIndex });

        #endregion

        #region CreateNewPreset

        /// <summary>
        /// 创建新预设 方法信息
        /// </summary>
        public static XMethodInfo CreateNewPreset_MethodInfo { get; } = new XMethodInfo(Type, nameof(CreateNewPreset));

        /// <summary>
        /// 创建新预设
        /// </summary>
        /// <param name="presetObject"></param>
        /// <param name="presetName"></param>
        public void CreateNewPreset(object presetObject, string presetName) => CreateNewPreset_MethodInfo.Invoke(obj, new object[] { presetObject, presetName });

        #endregion

        #region GetCurrentLib

        /// <summary>
        /// 获取当前库 方法信息
        /// </summary>
        public static XMethodInfo GetCurrentLib_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetCurrentLib));

        /// <summary>
        /// 获取当前库
        /// </summary>
        /// <returns></returns>
        public TPresetLibrary GetCurrentLib() => New(GetCurrentLib_MethodInfo.Invoke(obj, null));

        #endregion

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract TPresetLibrary New(object obj);

        #region LibraryModeChange

        /// <summary>
        /// 库模式修改 方法信息
        /// </summary>
        public static XMethodInfo LibraryModeChange_MethodInfo { get; } = new XMethodInfo(Type, nameof(LibraryModeChange), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

        /// <summary>
        /// 库模式修改
        /// </summary>
        /// <param name="userData"></param>
        public void LibraryModeChange(object userData) => LibraryModeChange_MethodInfo.Invoke(obj, new object[] { userData });

        /// <summary>
        /// 库模式修改
        /// </summary>
        /// <param name="presetLibraryFileFullPath"></param>
        public void LibraryModeChange(string presetLibraryFileFullPath) => LibraryModeChange_MethodInfo.Invoke(obj, new object[] { presetLibraryFileFullPath });

        #endregion

        #region CreateNewLibraryCallback

        /// <summary>
        /// 创建新库回调 方法信息
        /// </summary>
        public static XMethodInfo CreateNewLibraryCallback_MethodInfo { get; } = new XMethodInfo(Type, nameof(CreateNewLibraryCallback), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

        /// <summary>
        /// 创建新库回调
        /// </summary>
        /// <param name="libraryName"></param>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public string CreateNewLibraryCallback(string libraryName, PresetFileLocation fileLocation) => CreateNewLibraryCallback_MethodInfo.Invoke(obj, new object[] { libraryName, (int)fileLocation }) as string;

        #endregion
    }
}
