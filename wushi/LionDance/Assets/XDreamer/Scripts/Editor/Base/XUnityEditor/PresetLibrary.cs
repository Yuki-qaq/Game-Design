using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 与UnityEditor.PresetLibrary类对应
    /// </summary>
    public abstract class PresetLibrary<T> : ScriptableObject_LinkType<T> where T : PresetLibrary<T>, new()
    {
        /// <summary>
        /// 构造
        /// </summary>
        public PresetLibrary() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public PresetLibrary(object obj) : base(obj) { }

        #region Count

        /// <summary>
        /// 数量 方法信息
        /// </summary>
        public static XMethodInfo Count_MethodInfo { get; } = new XMethodInfo(Type, nameof(Count));

        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        public int Count() => (int)Count_MethodInfo.Invoke(obj, null);

        #endregion

        #region GetPreset

        /// <summary>
        /// 获取预设 方法信息
        /// </summary>
        public static XMethodInfo GetPreset_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetPreset));

        /// <summary>
        /// 获取预设
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetPreset(int index) => Count_MethodInfo.Invoke(obj, new object[] { index });

        #endregion

        #region Add

        /// <summary>
        /// 添加 方法信息
        /// </summary>
        public static XMethodInfo Add_MethodInfo { get; } = new XMethodInfo(Type, nameof(Add));

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="presetObject"></param>
        /// <param name="presetName"></param>
        public void Add(object presetObject, string presetName) => Add_MethodInfo.Invoke(obj, new object[] { presetObject, presetName });

        #endregion

        #region Replace

        /// <summary>
        /// 替换 方法信息
        /// </summary>
        public static XMethodInfo Replace_MethodInfo { get; } = new XMethodInfo(Type, nameof(Replace));

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newPresetObject"></param>
        public void Replace(int index, object newPresetObject) => Replace_MethodInfo.Invoke(obj, new object[] { index, newPresetObject });

        #endregion

        #region Remove

        /// <summary>
        /// 移除 方法信息
        /// </summary>
        public static XMethodInfo Remove_MethodInfo { get; } = new XMethodInfo(Type,nameof(Remove));

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index) => Remove_MethodInfo.Invoke(obj, new object[] { index });

        #endregion

        #region Move

        /// <summary>
        /// 移动 方法信息
        /// </summary>
        public static XMethodInfo Move_MethodInfo { get; } = new XMethodInfo(Type, nameof(Move));

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="index"></param>
        /// <param name="destIndex"></param>
        /// <param name="insertAfterDestIndex"></param>
        public void Move(int index, int destIndex, bool insertAfterDestIndex) => Move_MethodInfo.Invoke(obj, new object[] { index, destIndex, insertAfterDestIndex });

        #endregion

        #region Draw

        /// <summary>
        /// 绘制 矩形 整型 方法信息
        /// </summary>
        public static XMethodInfo Draw_Rect_Int_MethodInfo { get; } = new XMethodInfo(Type, nameof(Draw), new Type[] { typeof(Rect), typeof(int) });

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        public void Draw(Rect rect, int index) => Draw_Rect_Int_MethodInfo.Invoke(obj, new object[] { rect, index });

        /// <summary>
        ///  绘制 矩形 对象 方法信息
        /// </summary>
        public static XMethodInfo Draw_Rect_Object_MethodInfo { get; } = new XMethodInfo(Type, nameof(Draw), new Type[] { typeof(Rect), typeof(object) });

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="presetObject"></param>
        public void Draw(Rect rect, object presetObject) => Draw_Rect_Object_MethodInfo.Invoke(obj, new object[] { rect, presetObject });

        #endregion

        #region GetName

        /// <summary>
        /// 获取名称 方法信息
        /// </summary>
        public static XMethodInfo GetName_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetName));

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetName(int index) => (string)GetName_MethodInfo.Invoke(obj, new object[] { index });

        #endregion

        #region SetName

        /// <summary>
        /// 设置名称 方法信息
        /// </summary>
        public static XMethodInfo SetName_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetName));

        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        public void SetName(int index, string name) => SetName_MethodInfo.Invoke(obj, new object[] { index, name });

        #endregion
    }
}
