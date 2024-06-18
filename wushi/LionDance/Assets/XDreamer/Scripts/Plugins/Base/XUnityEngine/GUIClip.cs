using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// GUI剪辑
    /// </summary>
    [LinkType("UnityEngine.GUIClip")]
    public class GUIClip : LinkType<GUIClip>
    {
        #region Unclip

        /// <summary>
        /// 松开 矩形 方法信息
        /// </summary>
        public static XMethodInfo Unclip_Rect_MethodInfo { get; } = new XMethodInfo(Type, nameof(Unclip), new Type[] { typeof(Rect) }, TypeHelper.DefaultLookup);

        /// <summary>
        /// 松开
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect Unclip(Rect rect)
        {
            return (Rect)Unclip_Rect_MethodInfo.Invoke(null, new object[] { rect });
        }

        /// <summary>
        /// 松开 二维向量 方法信息
        /// </summary>
        public static XMethodInfo Unclip_Vector2_MethodInfo { get; } = new XMethodInfo(Type, nameof(Unclip), new Type[] { typeof(Vector2) }, TypeHelper.DefaultLookup);

        /// <summary>
        /// 松开
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Rect Unclip(Vector2 pos)
        {
            return (Rect)Unclip_Vector2_MethodInfo.Invoke(null, new object[] { pos });
        }

        #endregion
    }
}
