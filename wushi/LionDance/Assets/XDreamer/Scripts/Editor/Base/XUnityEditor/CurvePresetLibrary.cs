using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 与UnityEditor.CurvePresetLibrary类关联
    /// </summary>
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(CurvePresetLibrary))]
    public class CurvePresetLibrary : PresetLibrary<CurvePresetLibrary>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public CurvePresetLibrary() { }

        /// <summary>
        /// 构
        /// </summary>
        /// <param name="obj"></param>
        public CurvePresetLibrary(object obj) : base(obj) { }

        /// <summary>
        /// 获取曲线
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AnimationCurve GetCurve(int index) => GetPreset(index) as AnimationCurve;

        #region m_Presets

        /// <summary>
        /// 预设名称 字段信息
        /// </summary>
        public static XFieldInfo m_PresetsName_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_Presets), BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// 预设
        /// </summary>
        public List<CurvePreset> m_Presets
        {
            get
            {
                List<CurvePreset> curvePresets = new List<CurvePreset>();
                if (m_PresetsName_FieldInfo.GetValue(obj) is IList curvePresetList)
                {
                    foreach (var curvePreset in curvePresetList)
                    {
                        curvePresets.Add(new CurvePreset(curvePreset));
                    }
                }
                return curvePresets;
            }
        }

        #endregion

        /// <summary>
        /// 获取曲线预设
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CurvePreset GetCurvePreset(int index) => m_Presets[index];

        /// <summary>
        /// 与UnityEditor.CurvePresetLibrary+CurvePreset类对应
        /// </summary>
        [LinkType(EditorHelper.UnityEditorPrefix + nameof(CurvePresetLibrary) + "+" + nameof(CurvePreset))]
        public class CurvePreset : LinkType_Name<CurvePreset>
        {
            #region curve

            /// <summary>
            /// 曲线 属性信息
            /// </summary>
            public static XPropertyInfo curve_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(curve));

            /// <summary>
            /// 曲线
            /// </summary>
            public AnimationCurve curve
            {
                get => (AnimationCurve)curve_PropertyInfo.GetValue(obj);
                set => curve_PropertyInfo.SetValue(obj, value);
            }

            #endregion

            /// <summary>
            /// 曲线预设 动画曲线 字符串 构造信息
            /// </summary>
            public static XConstructorInfo CurvePreset_AnimationCurve_String_ConstructorInfo { get; } = new XConstructorInfo(Type, new Type[] { typeof(AnimationCurve), typeof(string) });

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="preset"></param>
            /// <param name="presetName"></param>
            public CurvePreset(AnimationCurve preset, string presetName)
            {
                CurvePreset_AnimationCurve_String_ConstructorInfo.obj?.Invoke(new object[] { preset, presetName });
            }

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="obj"></param>
            public CurvePreset(object obj) : base(obj) { }
        }
    }
}
