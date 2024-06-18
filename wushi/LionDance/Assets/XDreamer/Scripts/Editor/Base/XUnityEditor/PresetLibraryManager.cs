using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 预设库管理器
    /// </summary>
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(PresetLibraryManager))]
    public class PresetLibraryManager : UnityEngine_Object<PresetLibraryManager>
    {
        /// <summary>
        /// 库缓存
        /// </summary>
        [LinkType(EditorHelper.UnityEditorPrefix + nameof(PresetLibraryManager) + "+" + nameof(LibraryCache))]
        public class LibraryCache : LinkType<LibraryCache>
        {
            #region loadedLibraries

            /// <summary>
            /// 已加载的库 属性信息
            /// </summary>
            public static XPropertyInfo loadedLibraries_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(loadedLibraries));

            /// <summary>
            /// 已加载的库
            /// </summary>
            public List<ScriptableObject> loadedLibraries
            {
                get => (List<ScriptableObject>)loadedLibraries_PropertyInfo.GetValue(obj);
            }

            #endregion

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="obj"></param>
            public LibraryCache(object obj) : base(obj) { }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        private PresetLibraryManager(object obj) : base(obj) { }

        #region instance

        /// <summary>
        /// 实例 属性信息
        /// </summary>
        public static XPropertyInfo instance_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(instance), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        
        /// <summary>
        /// 实例
        /// </summary>
        public static PresetLibraryManager instance => new PresetLibraryManager(instance_PropertyInfo.GetValue(null));

        #endregion

        #region m_LibraryCaches

        /// <summary>
        /// 库缓存 字段信息
        /// </summary>
        public static XFieldInfo m_LibraryCaches_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_LibraryCaches), BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// 库缓存
        /// </summary>
        public List<LibraryCache> m_LibraryCaches
        {
            get
            {
                List<LibraryCache> libraryCaches = new List<LibraryCache>();
                if (m_LibraryCaches_FieldInfo.GetValue(obj) is IList libraryCacheList)
                {
                    foreach (var curvePreset in libraryCacheList)
                    {
                        libraryCaches.Add(new LibraryCache(curvePreset));
                    }
                }
                return libraryCaches;
            }
        }

        #endregion

        #region GetPresetLibraryCache

        /// <summary>
        /// 获取预设库缓存 方法信息
        /// </summary>
        public static XMethodInfo GetPresetLibraryCache_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetPresetLibraryCache));

        /// <summary>
        /// 获取预设库缓存
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public LibraryCache GetPresetLibraryCache(string identifier)
        {
            return new LibraryCache(GetPresetLibraryCache_MethodInfo.Invoke(obj, new object[] { identifier }));
        }

        #endregion
    }
}
