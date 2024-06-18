using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 本地化数据
    /// </summary>
    [LinkType("UnityEditor.LocalizationDatabase")]
    public class LocalizationDatabase : LinkType<LocalizationDatabase>
    {
        #region currentEditorLanguage

        /// <summary>
        /// 当前编辑器语言 属性信息
        /// </summary>
        public static XPropertyInfo currentEditorLanguage_PropertyInfo { get; } = GetXPropertyInfo(nameof(currentEditorLanguage));

        /// <summary>
        /// 当前编辑器语言
        /// </summary>
        public static SystemLanguage currentEditorLanguage
        {
            get => currentEditorLanguage_PropertyInfo.GetStaticValue(null, SystemLanguage.English);
            set => currentEditorLanguage_PropertyInfo.SetStaticValue(value);
        }

        #endregion

        #region GetDefaultEditorLanguage

        /// <summary>
        /// 获取默认编辑器语言 方法信息
        /// </summary>
        public static XMethodInfo GetDefaultEditorLanguage_MethodInfo { get; } = GetXMethodInfo(nameof(GetDefaultEditorLanguage));

        /// <summary>
        /// 获取默认编辑器语言
        /// </summary>
        /// <returns></returns>
        public static SystemLanguage GetDefaultEditorLanguage() => GetDefaultEditorLanguage_MethodInfo.InvokeStaticEmpty<SystemLanguage>();

        #endregion

        #region GetAvailableEditorLanguages

        /// <summary>
        /// 获取可用的编辑器语言 方法信息
        /// </summary>
        public static XMethodInfo GetAvailableEditorLanguages_MethodInfo { get; } = GetXMethodInfo(nameof(GetAvailableEditorLanguages));

        /// <summary>
        /// 获取可用的编辑器语言
        /// </summary>
        /// <returns></returns>
        public static SystemLanguage[] GetAvailableEditorLanguages() => GetAvailableEditorLanguages_MethodInfo.InvokeStaticEmpty<SystemLanguage[]>();

        #endregion

        #region GetLocalizationResourceFolder

        /// <summary>
        /// 获取本地化资源文件夹 方法信息
        /// </summary>
        public static XMethodInfo GetLocalizationResourceFolder_MethodInfo { get; } = GetXMethodInfo(nameof(GetLocalizationResourceFolder));

        /// <summary>
        /// 获取本地化资源文件夹
        /// </summary>
        /// <returns></returns>
        public static string GetLocalizationResourceFolder() => GetLocalizationResourceFolder_MethodInfo.InvokeStaticEmpty<string>();

        #endregion

        #region GetContextGroupName

        /// <summary>
        /// 获取内容组名 方法信息
        /// </summary>
        public static XMethodInfo GetContextGroupName_MethodInfo { get; } = GetXMethodInfo(nameof(GetContextGroupName));

        /// <summary>
        /// 获取内容组名
        /// </summary>
        /// <returns></returns>
        public static string GetContextGroupName() => GetContextGroupName_MethodInfo.InvokeStaticEmpty<string>();

        #endregion
    }
}
