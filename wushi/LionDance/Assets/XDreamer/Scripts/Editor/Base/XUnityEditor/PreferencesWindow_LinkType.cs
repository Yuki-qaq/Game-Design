using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 首选项窗口关联类型
    /// </summary>
#if UNITY_2018_3_OR_NEWER
    [Obsolete("因UnityEditor.PreferencesWindow被移除,本类不可再被使用", true)]
#endif
    [LinkType(TypeFullName)]
    public class PreferencesWindow_LinkType : SettingsWindow_LinkType<PreferencesWindow_LinkType>
    {
        private const string TypeFullName = EditorHelper.UnityEditorPrefix + "PreferencesWindow";

        /// <summary>
        /// 段落
        /// </summary>
        [LinkType(TypeFullName + "+" + nameof(Section))]
        public class Section : LinkType<Section>
        {
            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="obj"></param>
            public Section(object obj) : base(obj) { }

            #region content

            /// <summary>
            /// 内容字段信息
            /// </summary>
            public static XFieldInfo content_FieldInfo { get; } = new XFieldInfo(Type, nameof(content), BindingFlags.Instance | BindingFlags.Public);

            /// <summary>
            /// 内容
            /// </summary>
            public GUIContent content
            {
                get => content_FieldInfo.GetValue(obj) as GUIContent;
            }

            #endregion
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="editorWindow"></param>
        public PreferencesWindow_LinkType(EditorWindow editorWindow) : base(editorWindow) { }

        #region m_Sections

        /// <summary>
        /// 段落 字段信息
        /// </summary>
        public static XFieldInfo m_Sections_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_Sections), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 段落
        /// </summary>
        public List<Section> m_Sections
        {
            get
            {
                var list = new List<Section>();
                IList sections = m_Sections_FieldInfo.GetValue(obj) as IList;
                for (int i = 0; i < sections.Count; i++)
                {
                    list.Add(new Section(sections[i]));
                }
                return list;
            }
        }

        #endregion

        #region m_SelectedSectionIndex

        /// <summary>
        /// 选择的段落索引 字段信息
        /// </summary>
        public static XFieldInfo m_SelectedSectionIndex_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_SelectedSectionIndex), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 选择的段落索引
        /// </summary>
        public int m_SelectedSectionIndex
        {
            get => (int)m_SelectedSectionIndex_FieldInfo.GetValue(obj);
            set => m_SelectedSectionIndex_FieldInfo.SetValue(obj, value);
        }

        #endregion

        #region selectedSectionIndex

        /// <summary>
        /// 选择的段落索引 属性信息
        /// </summary>
        public static XPropertyInfo selectedSectionIndex_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(selectedSectionIndex), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 选择的段落索引
        /// </summary>
        public int selectedSectionIndex
        {
            get => (int)selectedSectionIndex_PropertyInfo.GetValue(obj);
            set => selectedSectionIndex_PropertyInfo.SetValue(obj, value);
        }

        #endregion

        #region ShowPreferencesWindow

        /// <summary>
        /// 显示首选项窗口 方法信息
        /// </summary>
        public static XMethodInfo ShowPreferencesWindow_MethodInfo { get; } = new XMethodInfo(Type, nameof(ShowPreferencesWindow), BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// 显示首选项窗口
        /// </summary>
        public static void ShowPreferencesWindow()
        {
            ShowPreferencesWindow_MethodInfo.Invoke(null, null);
        }

        #endregion
    }
}
