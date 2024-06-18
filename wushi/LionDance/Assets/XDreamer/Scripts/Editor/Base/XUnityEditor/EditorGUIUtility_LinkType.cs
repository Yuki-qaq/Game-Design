using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 类<see cref="EditorGUIUtility"/>的关联类型
    /// </summary>
    [LinkType(typeof(EditorGUIUtility))]
    public class EditorGUIUtility_LinkType : GUIUtility_LinkType<EditorGUIUtility_LinkType>
    {
        #region skinIndex

        /// <summary>
        /// 皮肤索引属性信息
        /// </summary>
        public static XPropertyInfo skinIndex_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(skinIndex), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 皮肤索引
        /// </summary>
        public static int skinIndex
        {
            get
            {
                return (int)skinIndex_PropertyInfo.GetValue(null);
            }
        }

        #endregion

        #region SetBoldDefaultFont

        /// <summary>
        /// 设置粗体默认字体 方法信息
        /// </summary>
        public static XMethodInfo SetBoldDefaultFont_MethodInfo { get; } = new XMethodInfo(Type,nameof(SetBoldDefaultFont),TypeHelper.StaticNotPublic);

        /// <summary>
        /// 设置粗体默认字体
        /// </summary>
        /// <param name="isBold"></param>
        public static void SetBoldDefaultFont(bool isBold)
        {
            SetBoldDefaultFont_MethodInfo.Invoke(null, new object[] { isBold });
        }

        #endregion

        #region GetIconForObject

        /// <summary>
        /// 为对象获取图标 方法信息
        /// </summary>
        public static XMethodInfo GetIconForObject_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetIconForObject), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 为对象获取图标
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Texture2D GetIconForObject(Object obj)
        {
            return (Texture2D)GetIconForObject_MethodInfo.Invoke(null, new object[] { obj });
        }

        #endregion

        #region SetIconForObject

        /// <summary>
        /// 为对象设置图标 方法信息
        /// </summary>
        public static XMethodInfo SetIconForObject_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetIconForObject), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 为对象设置图标
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="icon"></param>
        public static void SetIconForObject(Object obj, Texture2D icon)
        {
            SetIconForObject_MethodInfo.Invoke(null, new object[] { obj, icon });
        }

        #endregion

        #region GUITextureBlit2SRGBMaterial

        /// <summary>
        /// GUI纹理位图转SRGB材质 属性信息
        /// </summary>
        public static XPropertyInfo GUITextureBlit2SRGBMaterial_PropertyInfo { get; } = GetXPropertyInfo(nameof(GUITextureBlit2SRGBMaterial));

        /// <summary>
        /// GUI纹理位图转SRGB材质
        /// </summary>
        public static Material GUITextureBlit2SRGBMaterial => (Material)GUITextureBlit2SRGBMaterial_PropertyInfo.GetValue(null);

        #endregion
    }
}
