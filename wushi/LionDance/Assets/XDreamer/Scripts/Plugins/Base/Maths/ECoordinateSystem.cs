using System;
using XCSJ.Attributes;
using XCSJ.Caches;
using System.Collections.Generic;
using XCSJ.Helper;
using XCSJ.Extension.Base.Units;
using XCSJ.Maths;

#if UNITY_2018_3_OR_NEWER
using XCSJ.Extension.Base.Attributes;
using UnityEngine;
#endif

namespace XCSJ.Extension.Base.Maths
{
    /// <summary>
    /// 坐标系枚举
    /// </summary>
    [Name("坐标系")]
    public enum ECoordinateSystem
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Name("未知")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("未知")]
#endif
        Unknow = 0,

        #region X右

        #region X右/Y上/Z前

        /// <summary>
        /// X右/Y上/Z前
        /// </summary>
        [Name("X右/Y上/Z前")]
        [Tip("左手坐标系；Unity中默认的坐标系，也是本程序集中使用的默认坐标系；在本枚举定义范围内，提到默认坐标系时，均指本枚举对应的坐标系；", "Left hand coordinate system; The default coordinate system in unity is also the default coordinate system used in this assembly; Within the scope of this enumeration definition, the reference to the default coordinate system refers to the coordinate system corresponding to this enumeration;")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X右/Y上/Z前")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left)]
        [CoordinateSystemConvertTest(1, 2, 3)]
        XR_YU_ZF = CoordinateSystemHelper.R | (CoordinateSystemHelper.U << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.F << CoordinateSystemHelper.Z),

        #endregion

        #region X右/Y上/Z后

        /// <summary>
        /// X右/Y上/Z后
        /// </summary>
        [Name("X右/Y上/Z后")]
        [Tip("右手坐标系；与Unity坐标系Z轴取反；教课书中多使用本坐标系做空间几何的讲解；", "Right hand coordinate system; Reverse the Z axis of the Unity coordinate system; This coordinate system is often used in teaching books to explain spatial geometry;")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X右/Y上/Z后")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1)]
        [CoordinateSystemConvertTest(1, 2, -3)]
        XR_YU_ZB = CoordinateSystemHelper.R | (CoordinateSystemHelper.U << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.B << CoordinateSystemHelper.Z),

        #endregion

        #region X右/Y下/Z前

        /// <summary>
        /// X右/Y下/Z前
        /// </summary>
        [Name("X右/Y下/Z前")]
        [Tip("右手坐标系；与Unity坐标系Y轴取反；", "Right hand coordinate system; Reverse the Y axis of the Unity coordinate system;")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X右/Y下/Z前")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, yScale = -1)]
        [CoordinateSystemConvertTest(1, -2, 3)]
        XR_YD_ZF = CoordinateSystemHelper.R | (CoordinateSystemHelper.D << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.F << CoordinateSystemHelper.Z),

        #endregion

        #region X右/Y下/Z后

        /// <summary>
        /// X右/Y下/Z后
        /// </summary>
        [Name("X右/Y下/Z后")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X右/Y下/Z后")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, xRotate = 180)]
        [CoordinateSystemConvertTest(1, -2, -3)]
        XR_YD_ZB = CoordinateSystemHelper.R | (CoordinateSystemHelper.D << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.B << CoordinateSystemHelper.Z),

        #endregion

        #region X右/Y前/Z上

        /// <summary>
        /// X右/Y前/Z上
        /// </summary>
        [Name("X右/Y前/Z上")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X右/Y前/Z上")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, xRotate = -90)]
        [CoordinateSystemConvertTest(1, 3, 2)]
        XR_YF_ZU = CoordinateSystemHelper.R | (CoordinateSystemHelper.F << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.U << CoordinateSystemHelper.Z),

        #endregion

        #region X右/Y前/Z下

        /// <summary>
        /// X右/Y前/Z下
        /// </summary>
        [Name("X右/Y前/Z下")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X右/Y前/Z下")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, xRotate = 90)]
        [CoordinateSystemConvertTest(1, 3, -2)]
        XR_YF_ZD = CoordinateSystemHelper.R | (CoordinateSystemHelper.F << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.D << CoordinateSystemHelper.Z),

        #endregion

        #region X右/Y后/Z上

        /// <summary>
        /// X右/Y后/Z上
        /// </summary>
        [Name("X右/Y后/Z上")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X右/Y后/Z上")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, xRotate = -90)]
        [CoordinateSystemConvertTest(1, -3, 2)]
        XR_YB_ZU = CoordinateSystemHelper.R | (CoordinateSystemHelper.B << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.U << CoordinateSystemHelper.Z),

        #endregion

        #region X右/Y后/Z下

        /// <summary>
        /// X右/Y后/Z下
        /// </summary>
        [Name("X右/Y后/Z下")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X右/Y后/Z下")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, xRotate = 90)]
        [CoordinateSystemConvertTest(1, -3, -2)]
        XR_YB_ZD = CoordinateSystemHelper.R | (CoordinateSystemHelper.B << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.D << CoordinateSystemHelper.Z),

        #endregion

        #endregion

        #region X左

        #region X左/Y上/Z前

        /// <summary>
        /// X左/Y上/Z前
        /// </summary>
        [Name("X左/Y上/Z前")]
        [Tip("右手坐标系；与Unity坐标系X轴取反；", "Right hand coordinate system; Reverse the X axis of the Unity coordinate system;")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X左/Y上/Z前")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, xScale = -1)]
        [CoordinateSystemConvertTest(-1, 2, 3)]
        XL_YU_ZF = CoordinateSystemHelper.L | (CoordinateSystemHelper.U << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.F << CoordinateSystemHelper.Z),

        #endregion

        #region X左/Y上/Z后

        /// <summary>
        /// X左/Y上/Z后
        /// </summary>
        [Name("X左/Y上/Z后")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X左/Y上/Z后")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, yRotate = 180)]
        [CoordinateSystemConvertTest(-1, 2, -3)]
        XL_YU_ZB = CoordinateSystemHelper.L | (CoordinateSystemHelper.U << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.B << CoordinateSystemHelper.Z),

        #endregion

        #region X左/Y下/Z前

        /// <summary>
        /// X左/Y下/Z前
        /// </summary>
        [Name("X左/Y下/Z前")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X左/Y下/Z前")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, zRotate = 180)]
        [CoordinateSystemConvertTest(-1, -2, 3)]
        XL_YD_ZF = CoordinateSystemHelper.L | (CoordinateSystemHelper.D << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.F << CoordinateSystemHelper.Z),

        #endregion

        #region X左/Y下/Z后

        /// <summary>
        /// X左/Y下/Z后
        /// </summary>
        [Name("X左/Y下/Z后")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X左/Y下/Z后")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, zRotate = 180)]
        [CoordinateSystemConvertTest(-1, -2, -3)]
        XL_YD_ZB = CoordinateSystemHelper.L | (CoordinateSystemHelper.D << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.B << CoordinateSystemHelper.Z),

        #endregion

        #region X左/Y前/Z上

        /// <summary>
        /// X左/Y前/Z上
        /// </summary>
        [Name("X左/Y前/Z上")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X左/Y前/Z上")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, xRotate = -90, yRotate = 180)]
        [CoordinateSystemConvertTest(-1, 3, 2)]
        XL_YF_ZU = CoordinateSystemHelper.L | (CoordinateSystemHelper.F << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.U << CoordinateSystemHelper.Z),

        #endregion

        #region X左/Y前/Z下

        /// <summary>
        /// X左/Y前/Z下
        /// </summary>
        [Name("X左/Y前/Z下")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X左/Y前/Z下")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, xScale = -1, needRotate = true, xRotate = 90)]
        [CoordinateSystemConvertTest(-1, 3, -2)]
        XL_YF_ZD = CoordinateSystemHelper.L | (CoordinateSystemHelper.F << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.D << CoordinateSystemHelper.Z),

        #endregion

        #region X左/Y后/Z上

        /// <summary>
        /// X左/Y后/Z上
        /// </summary>
        [Name("X左/Y后/Z上")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X左/Y后/Z上")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, xScale = -1, needRotate = true, xRotate = -90)]
        [CoordinateSystemConvertTest(-1, -3, 2)]
        XL_YB_ZU = CoordinateSystemHelper.L | (CoordinateSystemHelper.B << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.U << CoordinateSystemHelper.Z),

        #endregion

        #region X左/Y后/Z下

        /// <summary>
        /// X左/Y后/Z下
        /// </summary>
        [Name("X左/Y后/Z下")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X左/Y后/Z下")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, xRotate = 90, yRotate = 180)]
        [CoordinateSystemConvertTest(-1, -3, -2)]
        XL_YB_ZD = CoordinateSystemHelper.L | (CoordinateSystemHelper.B << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.D << CoordinateSystemHelper.Z),

        #endregion

        #endregion

        #region X上

        #region X上/Y右/Z前

        /// <summary>
        /// X上/Y右/Z前
        /// </summary>
        [Name("X上/Y右/Z前")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X上/Y右/Z前")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, xScale = -1, needRotate = true, zRotate = 90)]
        [CoordinateSystemConvertTest(2, 1, 3)]
        XU_YR_ZF = CoordinateSystemHelper.U | (CoordinateSystemHelper.R << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.F << CoordinateSystemHelper.Z),

        #endregion

        #region X上/Y右/Z后

        /// <summary>
        /// X上/Y右/Z后
        /// </summary>
        [Name("X上/Y右/Z后")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X上/Y右/Z后")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, yRotate = 180, zRotate = 90)]
        [CoordinateSystemConvertTest(2, 1, -3)]
        XU_YR_ZB = CoordinateSystemHelper.U | (CoordinateSystemHelper.R << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.B << CoordinateSystemHelper.Z),

        #endregion

        #region X上/Y左/Z前

        /// <summary>
        /// X上/Y左/Z前
        /// </summary>
        [Name("X上/Y左/Z前")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X上/Y左/Z前")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, zRotate = 90)]
        [CoordinateSystemConvertTest(2, -1, 3)]
        XU_YL_ZF = CoordinateSystemHelper.U | (CoordinateSystemHelper.L << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.F << CoordinateSystemHelper.Z),

        #endregion

        #region X上/Y左/Z后

        /// <summary>
        /// X上/Y左/Z后
        /// </summary>
        [Name("X上/Y左/Z后")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X上/Y左/Z后")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, zRotate = 90, scaleLeft = false)]
        [CoordinateSystemConvertTest(2, -1, -3)]
        XU_YL_ZB = CoordinateSystemHelper.U | (CoordinateSystemHelper.L << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.B << CoordinateSystemHelper.Z),

        #endregion

        #region X上/Y前/Z右

        /// <summary>
        /// X上/Y前/Z右
        /// </summary>
        [Name("X上/Y前/Z右")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X上/Y前/Z右")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, yRotate = 90, zRotate = 90)]
        [CoordinateSystemConvertTest(2, 3, 1)]
        XU_YF_ZR = CoordinateSystemHelper.U | (CoordinateSystemHelper.F << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.R << CoordinateSystemHelper.Z),

        #endregion

        #region X上/Y前/Z左

        /// <summary>
        /// X上/Y前/Z左
        /// </summary>
        [Name("X上/Y前/Z左")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X上/Y前/Z左")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, yRotate = 90, zRotate = 90, scaleLeft = false)]
        [CoordinateSystemConvertTest(2, 3, -1)]
        XU_YF_ZL = CoordinateSystemHelper.U | (CoordinateSystemHelper.F << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.L << CoordinateSystemHelper.Z),

        #endregion

        #region X上/Y后/Z右

        /// <summary>
        /// X上/Y后/Z右
        /// </summary>
        [Name("X上/Y后/Z右")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X上/Y后/Z右")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, yRotate = -90, zRotate = 90, scaleLeft = false)]
        [CoordinateSystemConvertTest(2, -3, 1)]
        XU_YB_ZR = CoordinateSystemHelper.U | (CoordinateSystemHelper.B << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.R << CoordinateSystemHelper.Z),

        #endregion

        #region X上/Y后/Z左

        /// <summary>
        /// X上/Y后/Z左
        /// </summary>
        [Name("X上/Y后/Z左")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X上/Y后/Z左")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, yRotate = -90, zRotate = 90)]
        [CoordinateSystemConvertTest(2, -3, -1)]
        XU_YB_ZL = CoordinateSystemHelper.U | (CoordinateSystemHelper.B << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.L << CoordinateSystemHelper.Z),

        #endregion

        #endregion

        #region X下

        #region X下/Y右/Z前

        /// <summary>
        /// X下/Y右/Z前
        /// </summary>
        [Name("X下/Y右/Z前")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X下/Y右/Z前")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, zRotate = -90)]
        [CoordinateSystemConvertTest(-2, 1, 3)]
        XD_YR_ZF = CoordinateSystemHelper.D | (CoordinateSystemHelper.R << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.F << CoordinateSystemHelper.Z),

        #endregion

        #region X下/Y右/Z后

        /// <summary>
        /// X下/Y右/Z后
        /// </summary>
        [Name("X下/Y右/Z后")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X下/Y右/Z后")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, zRotate = -90, scaleLeft = false)]
        [CoordinateSystemConvertTest(-2, 1, -3)]
        XD_YR_ZB = CoordinateSystemHelper.D | (CoordinateSystemHelper.R << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.B << CoordinateSystemHelper.Z),

        #endregion

        #region X下/Y左/Z前

        /// <summary>
        /// X下/Y左/Z前
        /// </summary>
        [Name("X下/Y左/Z前")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X下/Y左/Z前")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, xScale = -1, needRotate = true, zRotate = -90)]
        [CoordinateSystemConvertTest(-2, -1, 3)]
        XD_YL_ZF = CoordinateSystemHelper.D | (CoordinateSystemHelper.L << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.F << CoordinateSystemHelper.Z),

        #endregion

        #region X下/Y左/Z后

        /// <summary>
        /// X下/Y左/Z后
        /// </summary>
        [Name("X下/Y左/Z后")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X下/Y左/Z后")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, yRotate = 180, zRotate = -90)]
        [CoordinateSystemConvertTest(-2, -1, -3)]
        XD_YL_ZB = CoordinateSystemHelper.D | (CoordinateSystemHelper.L << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.B << CoordinateSystemHelper.Z),

        #endregion

        #region X下/Y前/Z右

        /// <summary>
        /// X下/Y前/Z右
        /// </summary>
        [Name("X下/Y前/Z右")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X下/Y前/Z右")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, yRotate = -90, zRotate = -90, scaleLeft = false)]
        [CoordinateSystemConvertTest(-2, 3, 1)]
        XD_YF_ZR = CoordinateSystemHelper.D | (CoordinateSystemHelper.F << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.R << CoordinateSystemHelper.Z),

        #endregion

        #region X下/Y前/Z左

        /// <summary>
        /// X下/Y前/Z左
        /// </summary>
        [Name("X下/Y前/Z左")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X下/Y前/Z左")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, yRotate = -90, zRotate = -90)]
        [CoordinateSystemConvertTest(-2, 3, -1)]
        XD_YF_ZL = CoordinateSystemHelper.D | (CoordinateSystemHelper.F << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.L << CoordinateSystemHelper.Z),

        #endregion

        #region X下/Y后/Z右

        /// <summary>
        /// X下/Y后/Z右
        /// </summary>
        [Name("X下/Y后/Z右")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X下/Y后/Z右")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, yRotate = 90, zRotate = -90)]
        [CoordinateSystemConvertTest(-2, -3, 1)]
        XD_YB_ZR = CoordinateSystemHelper.D | (CoordinateSystemHelper.B << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.R << CoordinateSystemHelper.Z),

        #endregion

        #region X下/Y后/Z左

        /// <summary>
        /// X下/Y后/Z左
        /// </summary>
        [Name("X下/Y后/Z左")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X下/Y后/Z左")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, yRotate = 90, zRotate = -90, scaleLeft = false)]
        [CoordinateSystemConvertTest(-2, -3, -1)]
        XD_YB_ZL = CoordinateSystemHelper.D | (CoordinateSystemHelper.B << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.L << CoordinateSystemHelper.Z),

        #endregion

        #endregion

        #region X前

        #region X前/Y右/Z上

        /// <summary>
        /// X前/Y右/Z上
        /// </summary>
        [Name("X前/Y右/Z上")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X前/Y右/Z上")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, xRotate = -90, yRotate = -90)]
        [CoordinateSystemConvertTest(3, 1, 2)]
        XF_YR_ZU = CoordinateSystemHelper.F | (CoordinateSystemHelper.R << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.U << CoordinateSystemHelper.Z),

        #endregion

        #region X前/Y右/Z下

        /// <summary>
        /// X前/Y右/Z下
        /// </summary>
        [Name("X前/Y右/Z下")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X前/Y右/Z下")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, xRotate = -90, yRotate = -90, scaleLeft = false)]
        [CoordinateSystemConvertTest(3, 1, -2)]
        XF_YR_ZD = CoordinateSystemHelper.F | (CoordinateSystemHelper.R << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.D << CoordinateSystemHelper.Z),

        #endregion

        #region X前/Y左/Z上

        /// <summary>
        /// X前/Y左/Z上
        /// </summary>
        [Name("X前/Y左/Z上")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X前/Y左/Z上")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, xRotate = 90, yRotate = -90, scaleLeft = false)]
        [CoordinateSystemConvertTest(3, -1, 2)]
        XF_YL_ZU = CoordinateSystemHelper.F | (CoordinateSystemHelper.L << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.U << CoordinateSystemHelper.Z),

        #endregion

        #region X前/Y左/Z下

        /// <summary>
        /// X前/Y左/Z下
        /// </summary>
        [Name("X前/Y左/Z下")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X前/Y左/Z下")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, xRotate = 90, yRotate = -90)]
        [CoordinateSystemConvertTest(3, -1, -2)]
        XF_YL_ZD = CoordinateSystemHelper.F | (CoordinateSystemHelper.L << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.D << CoordinateSystemHelper.Z),

        #endregion

        #region X前/Y上/Z右

        /// <summary>
        /// X前/Y上/Z右
        /// </summary>
        [Name("X前/Y上/Z右")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X前/Y上/Z右")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, yRotate = -90, scaleLeft = false)]
        [CoordinateSystemConvertTest(3, 2, 1)]
        XF_YU_ZR = CoordinateSystemHelper.F | (CoordinateSystemHelper.U << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.R << CoordinateSystemHelper.Z),

        #endregion

        #region X前/Y上/Z左

        /// <summary>
        /// X前/Y上/Z左
        /// </summary>
        [Name("X前/Y上/Z左")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X前/Y上/Z左")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, yRotate = -90)]
        [CoordinateSystemConvertTest(3, 2, -1)]
        XF_YU_ZL = CoordinateSystemHelper.F | (CoordinateSystemHelper.U << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.L << CoordinateSystemHelper.Z),

        #endregion

        #region X前/Y下/Z右

        /// <summary>
        /// X前/Y下/Z右
        /// </summary>
        [Name("X前/Y下/Z右")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X前/Y下/Z右")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, xRotate = 180, yRotate = -90)]
        [CoordinateSystemConvertTest(3, -2, 1)]
        XF_YD_ZR = CoordinateSystemHelper.F | (CoordinateSystemHelper.D << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.R << CoordinateSystemHelper.Z),

        #endregion

        #region X前/Y下/Z左

        /// <summary>
        /// X前/Y下/Z左
        /// </summary>
        [Name("X前/Y下/Z左")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X前/Y下/Z左")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, xRotate = 180, yRotate = -90, scaleLeft = false)]
        [CoordinateSystemConvertTest(3, -2, -1)]
        XF_YD_ZL = CoordinateSystemHelper.F | (CoordinateSystemHelper.D << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.L << CoordinateSystemHelper.Z),

        #endregion

        #endregion

        #region X后

        #region X后/Y右/Z上

        /// <summary>
        /// X后/Y右/Z上
        /// </summary>
        [Name("X后/Y右/Z上")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X后/Y右/Z上")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, xRotate = 90, yRotate = 90, scaleLeft = false)]
        [CoordinateSystemConvertTest(-3, 1, 2)]
        XB_YR_ZU = CoordinateSystemHelper.B | (CoordinateSystemHelper.R << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.U << CoordinateSystemHelper.Z),

        #endregion

        #region X后/Y右/Z下

        /// <summary>
        /// X后/Y右/Z下
        /// </summary>
        [Name("X后/Y右/Z下")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X后/Y右/Z下")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, xRotate = 90, yRotate = 90)]
        [CoordinateSystemConvertTest(-3, 1, -2)]
        XB_YR_ZD = CoordinateSystemHelper.B | (CoordinateSystemHelper.R << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.D << CoordinateSystemHelper.Z),

        #endregion

        #region X后/Y左/Z上

        /// <summary>
        /// X后/Y左/Z上
        /// </summary>
        [Name("X后/Y左/Z上")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X后/Y左/Z上")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, xRotate = -90, yRotate = 90)]
        [CoordinateSystemConvertTest(-3, -1, 2)]
        XB_YL_ZU = CoordinateSystemHelper.B | (CoordinateSystemHelper.L << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.U << CoordinateSystemHelper.Z),

        #endregion

        #region X后/Y左/Z下

        /// <summary>
        /// X后/Y左/Z下
        /// </summary>
        [Name("X后/Y左/Z下")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X后/Y左/Z下")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, xRotate = -90, yRotate = 90, scaleLeft = false)]
        [CoordinateSystemConvertTest(-3, -1, -2)]
        XB_YL_ZD = CoordinateSystemHelper.B | (CoordinateSystemHelper.L << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.D << CoordinateSystemHelper.Z),

        #endregion

        #region X后/Y上/Z右

        /// <summary>
        /// X后/Y上/Z右
        /// </summary>
        [Name("X后/Y上/Z右")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X后/Y上/Z右")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, yRotate = 90)]
        [CoordinateSystemConvertTest(-3, 2, 1)]
        XB_YU_ZR = CoordinateSystemHelper.B | (CoordinateSystemHelper.U << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.R << CoordinateSystemHelper.Z),

        #endregion

        #region X后/Y上/Z左

        /// <summary>
        /// X后/Y上/Z左
        /// </summary>
        [Name("X后/Y上/Z左")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X后/Y上/Z左")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, yRotate = 90, scaleLeft = false)]
        [CoordinateSystemConvertTest(-3, 2, -1)]
        XB_YU_ZL = CoordinateSystemHelper.B | (CoordinateSystemHelper.U << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.L << CoordinateSystemHelper.Z),

        #endregion

        #region X后/Y下/Z右

        /// <summary>
        /// X后/Y下/Z右
        /// </summary>
        [Name("X后/Y下/Z右")]
        [Tip("右手坐标系；", "Right hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X后/Y下/Z右")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Right, needScale = true, zScale = -1, needRotate = true, xRotate = 180, yRotate = 90, scaleLeft = false)]
        [CoordinateSystemConvertTest(-3, -2, 1)]
        XB_YD_ZR = CoordinateSystemHelper.B | (CoordinateSystemHelper.D << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.R << CoordinateSystemHelper.Z),

        #endregion

        #region X后/Y下/Z左

        /// <summary>
        /// X后/Y下/Z左
        /// </summary>
        [Name("X后/Y下/Z左")]
        [Tip("左手坐标系；", "Left hand coordinate system; ")]
#if UNITY_2018_3_OR_NEWER
        [EnumFieldName("X后/Y下/Z左")]
#endif
        [CoordinateSystemConvert(ECoordinateSystemMode.Left, needRotate = true, xRotate = 180, yRotate = 90)]
        [CoordinateSystemConvertTest(-3, -2, -1)]
        XB_YD_ZL = CoordinateSystemHelper.B | (CoordinateSystemHelper.D << CoordinateSystemHelper.Y) | (CoordinateSystemHelper.L << CoordinateSystemHelper.Z),

        #endregion

        #endregion
    }

    #region 坐标系转换测试特性

    /// <summary>
    /// 坐标系转换测试特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class CoordinateSystemConvertTestAttribute : Attribute
    {
        /// <summary>
        /// 当前坐标系X
        /// </summary>
        public float x { get; private set; }

        /// <summary>
        /// 当前坐标系Y
        /// </summary>
        public float y { get; private set; }

        /// <summary>
        /// 当前坐标系Z
        /// </summary>
        public float z { get; private set; }

        /// <summary>
        /// Unity坐标X
        /// </summary>
        public float ux { get; private set; }

        /// <summary>
        /// Unity坐标Y
        /// </summary>
        public float uy { get; private set; }

        /// <summary>
        /// Unity坐标Z
        /// </summary>
        public float uz { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="ux"></param>
        /// <param name="uy"></param>
        /// <param name="uz"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public CoordinateSystemConvertTestAttribute(float x, float y, float z, float ux = 1, float uy = 2, float uz = 3)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            this.ux = ux;
            this.uy = uy;
            this.uz = uz;
        }
    }

    #endregion

    #region 坐标系转换特性

    /// <summary>
    /// 坐标系转换特性:基于Unity坐标系将源坐标系进行缩放（镜像）或旋转
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class CoordinateSystemConvertAttribute : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="coordinateSystemMode"></param>
        public CoordinateSystemConvertAttribute(ECoordinateSystemMode coordinateSystemMode)
        {
            this.coordinateSystemMode = coordinateSystemMode;
        }

        /// <summary>
        /// 坐标系模式
        /// </summary>
        public ECoordinateSystemMode coordinateSystemMode { get; private set; } = ECoordinateSystemMode.Unknow;

        /// <summary>
        /// 缩放在左
        /// </summary>
        public bool scaleLeft { get; set; } = true;

        /// <summary>
        /// 需要缩放
        /// </summary>
        public bool needScale { get; set; } = false;

        /// <summary>
        /// X缩放值
        /// </summary>
        public float xScale { get; set; } = 1;

        /// <summary>
        /// Y缩放值
        /// </summary>
        public float yScale { get; set; } = 1;

        /// <summary>
        /// Z缩放值
        /// </summary>
        public float zScale { get; set; } = 1;

        /// <summary>
        /// 需要旋转
        /// </summary>
        public bool needRotate { get; set; } = false;

        /// <summary>
        /// X旋转欧拉角度值
        /// </summary>
        public float xRotate { get; set; } = 0;

        /// <summary>
        /// Y旋转欧拉角度值
        /// </summary>
        public float yRotate { get; set; } = 0;

        /// <summary>
        /// Z旋转欧拉角度值
        /// </summary>
        public float zRotate { get; set; } = 0;
    }

    #endregion

    #region 轴方向

    /// <summary>
    /// 轴方向
    /// </summary>
    public enum EAxisDirection
    {
        /// <summary>
        /// 右
        /// </summary>
        [Name("右")]
        R = CoordinateSystemHelper.R,

        /// <summary>
        /// 左
        /// </summary>
        [Name("左")]
        L = CoordinateSystemHelper.L,

        /// <summary>
        /// 上
        /// </summary>
        [Name("上")]
        U = CoordinateSystemHelper.U,

        /// <summary>
        /// 下
        /// </summary>
        [Name("下")]
        D = CoordinateSystemHelper.D,

        /// <summary>
        /// 前
        /// </summary>
        [Name("前")]
        F = CoordinateSystemHelper.F,

        /// <summary>
        /// 后
        /// </summary>
        [Name("后")]
        B = CoordinateSystemHelper.B,
    }

    #endregion

    #region 坐标系模式

    /// <summary>
    /// 坐标系模式：左手或右手坐标系
    /// </summary>
    public enum ECoordinateSystemMode
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Name("未知")]
        Unknow,

        /// <summary>
        /// 左手
        /// </summary>
        [Name("左手")]
        Left,

        /// <summary>
        /// 右手
        /// </summary>
        [Name("右手")]
        Right,
    }

    #endregion

    #region 坐标系

    /// <summary>
    /// 坐标系
    /// </summary>
    public sealed class CoordinateSystem
    {
        /// <summary>
        /// X轴
        /// </summary>
        public EAxisDirection xAxis { get; private set; } = EAxisDirection.R;

        /// <summary>
        /// Y轴
        /// </summary>
        public EAxisDirection yAxis { get; private set; } = EAxisDirection.U;

        /// <summary>
        /// Z轴
        /// </summary>
        public EAxisDirection zAxis { get; private set; } = EAxisDirection.F;

        /// <summary>
        /// 坐标系统
        /// </summary>
        public ECoordinateSystem coordinateSystem { get; private set; } = ECoordinateSystem.Unknow;

        /// <summary>
        /// 坐标系模式
        /// </summary>
        public ECoordinateSystemMode coordinateSystemMode { get; private set; } = ECoordinateSystemMode.Unknow;

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool isValid { get; private set; } = false;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        public CoordinateSystem(EAxisDirection xAxis = EAxisDirection.R, EAxisDirection yAxis = EAxisDirection.U, EAxisDirection zAxis = EAxisDirection.F)
        {
            Set(xAxis, yAxis, zAxis);
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="coordinateSystem"></param>
        public CoordinateSystem(ECoordinateSystem coordinateSystem)
        {
            Set(coordinateSystem);
        }

        /// <summary>
        /// 设置无效坐标系统
        /// </summary>
        public void SetInvalid() => Set(EAxisDirection.R, EAxisDirection.R, EAxisDirection.R);

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="coordinateSystem"></param>
        public void Set(ECoordinateSystem coordinateSystem)
        {
            coordinateSystem.Parse(out var x, out var y, out var z);
            Set(x, y, z);
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        public void Set(EAxisDirection xAxis = EAxisDirection.R, EAxisDirection yAxis = EAxisDirection.U, EAxisDirection zAxis = EAxisDirection.F)
        {
            this.xAxis = xAxis;
            this.yAxis = yAxis;
            this.zAxis = zAxis;

            Validate();
        }

        /// <summary>
        /// 有效
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            coordinateSystem = CoordinateSystemHelper.ToCoordinateSystem(xAxis, yAxis, zAxis);
            coordinateSystemMode = coordinateSystem.GetCoordinateSystemMode();

            isValid = coordinateSystem.IsValidCoordinateSystem();
            if (isValid)
            {
                InitConvertMatrix();
            }
            return isValid;
        }

        private ELengthUnits _lengthUnits = ELengthUnits.M;

        /// <summary>
        /// 长度单位：即坐标系中1值表示现实中哪种1单位的长度；
        /// </summary>
        public ELengthUnits lengthUnits
        {
            get => _lengthUnits;
            set
            {
                _lengthUnits = value;
                if (isValid) InitConvertMatrix();
            }
        }

        /// <summary>
        /// 比例尺：当前坐标系的1单位转为默认标准1单位(即1米)时的值；即1[单位长度]=[比例尺]米；长度单位为自定义时，可自定义设置比例尺；
        /// </summary>
        public V3F scale = new V3F(1, 1, 1);

#if UNITY_2018_3_OR_NEWER

        #region 当前坐标系到Unity坐标系

        /// <summary>
        /// 当前坐标系到Unity坐标系的转换矩阵
        /// </summary>
        public Matrix4x4 convertMatrixToUnity { get; private set; }

        /// <summary>
        /// 当前坐标系到Unity坐标系的转换矩阵的逆矩阵
        /// </summary>
        public Matrix4x4 convertMatrixToUnityInverse { get; private set; }

        /// <summary>
        /// 转换位置到Unity：将当前坐标系的源位置点转换到Unity坐标系的位置点
        /// </summary>
        /// <param name="srcPosition">当前坐标系的源位置点</param>
        /// <returns></returns>
        public Vector3 ConvertPositionToUnity(Vector3 srcPosition) => convertMatrixToUnity.MultiplyPoint3x4(srcPosition);

        /// <summary>
        /// 转换源旋转矩阵到Unity：将当前坐标系的源旋转矩阵转换到Unity坐标系的旋转矩阵
        /// </summary>
        /// <param name="srcRotateMatrix">当前坐标系的源旋转矩阵</param>
        /// <returns></returns>
        public Matrix4x4 ConvertRotationToUnity(Matrix4x4 srcRotateMatrix) => convertMatrixToUnity * srcRotateMatrix * convertMatrixToUnityInverse;

        /// <summary>
        /// 转换源旋转到Unity：将当前坐标系的源旋转转换到Unity坐标系的旋转
        /// </summary>
        /// <param name="srcRotation">当前坐标系的源旋转</param>
        /// <returns></returns>
        public Matrix4x4 ConvertRotationToUnity(Quaternion srcRotation) => ConvertRotationToUnity(Matrix4x4.Rotate(srcRotation));

        #endregion

        #region Unity坐标系到当前坐标系

        /// <summary>
        /// Unity坐标系到当前坐标系的转换矩阵
        /// </summary>
        public Matrix4x4 convertMatrixFromUnity { get; private set; }

        /// <summary>
        ///  Unity坐标系到当前坐标系的转换矩阵的逆矩阵
        /// </summary>
        public Matrix4x4 convertMatrixFromUnityInverse { get; private set; }

        /// <summary>
        /// 从Unity转换位置：将Unity坐标系的源位置点转换到当前坐标系的位置点
        /// </summary>
        /// <param name="srcPosition">Unity坐标系的源位置</param>
        /// <returns></returns>
        public Vector3 ConvertPositionFromUnity(Vector3 srcPosition) => convertMatrixFromUnity.MultiplyPoint3x4(srcPosition);

        /// <summary>
        /// 从Unity转换源旋转矩阵：将Unity坐标系的源旋转矩阵转换到当前坐标系的旋转矩阵
        /// </summary>
        /// <param name="srcRotateMatrix">Unity坐标系的源旋转矩</param>
        /// <returns></returns>
        public Matrix4x4 ConvertRotationFromUnity(Matrix4x4 srcRotateMatrix) => convertMatrixFromUnity * srcRotateMatrix * convertMatrixFromUnityInverse;

        /// <summary>
        /// 从Unity转换源旋转：将Unity坐标系的源旋转转换到当前坐标系的旋转
        /// </summary>
        /// <param name="srcRotation">Unity坐标系的源旋转</param>
        /// <returns></returns>
        public Matrix4x4 ConvertRotationFromUnity(Quaternion srcRotation) => ConvertRotationFromUnity(Matrix4x4.Rotate(srcRotation));

        #endregion

        private void InitConvertMatrix()
        {
            Vector3 scale;
            switch (lengthUnits)
            {
                case ELengthUnits.Custom: scale = this.scale.ToVector3(); break;
                default:
                    {
                        var s = (float)lengthUnits.ScaleToDefault();
                        scale = new Vector3(s, s, s);
                        break;
                    }
            }
            var unitConvertMatrix = Matrix4x4.Scale(scale);

            convertMatrixToUnity = coordinateSystem.GetConvertMatrixToUnity() * unitConvertMatrix;
            convertMatrixToUnityInverse = convertMatrixToUnity.inverse;

            convertMatrixFromUnity = convertMatrixToUnityInverse;
            convertMatrixFromUnityInverse = convertMatrixToUnity;
        }

#else

        private void InitConvertMatrix() { }

#endif
    }

    #endregion

    #region 坐标系助手

    /// <summary>
    /// 坐标系助手
    /// </summary>
    public static class CoordinateSystemHelper
    {
        /// <summary>
        /// 右
        /// </summary>
        internal const int R = 1 << 0;

        /// <summary>
        /// 左
        /// </summary>
        internal const int L = 1 << 1;

        /// <summary>
        /// 上
        /// </summary>
        internal const int U = 1 << 2;

        /// <summary>
        /// 下
        /// </summary>
        internal const int D = 1 << 3;

        /// <summary>
        /// 前
        /// </summary>
        internal const int F = 1 << 4;

        /// <summary>
        /// 后
        /// </summary>
        internal const int B = 1 << 5;

        /// <summary>
        /// X位移量：代码中默认不使用
        /// </summary>
        internal const int X = 0;

        /// <summary>
        /// Y位移量
        /// </summary>
        internal const int Y = 10;

        /// <summary>
        /// Z位移量
        /// </summary>
        internal const int Z = 20;

        /// <summary>
        /// 默认坐标系
        /// </summary>
        public const ECoordinateSystem UnityCoordinateSystem = ECoordinateSystem.XR_YU_ZF;

        /// <summary>
        /// 所有的轴方向
        /// </summary>
        public const EAxisDirection AllEAxisDirection = EAxisDirection.R | EAxisDirection.L | EAxisDirection.U | EAxisDirection.D | EAxisDirection.F | EAxisDirection.B;

        /// <summary>
        /// 所有的轴方向
        /// </summary>
        public const int All = (int)AllEAxisDirection;

        /// <summary>
        /// 转坐标系统枚举
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        /// <returns></returns>
        public static ECoordinateSystem ToCoordinateSystem(EAxisDirection xAxis, EAxisDirection yAxis, EAxisDirection zAxis) => (ECoordinateSystem)((int)xAxis | ((int)yAxis) << Y | ((int)zAxis) << Z);

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        public static void Parse(this ECoordinateSystem coordinateSystem, out EAxisDirection xAxis, out EAxisDirection yAxis, out EAxisDirection zAxis)
        {
            xAxis = (EAxisDirection)((int)coordinateSystem & All);
            yAxis = (EAxisDirection)(((int)coordinateSystem >> Y) & All);
            zAxis = (EAxisDirection)(((int)coordinateSystem >> Z) & All);
        }

        private static Dictionary<ECoordinateSystem, bool> cache = new Dictionary<ECoordinateSystem, bool>();

        /// <summary>
        /// 是有效的坐标系
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        public static bool IsValidCoordinateSystem(this ECoordinateSystem coordinateSystem)
        {
            if (cache.TryGetValue(coordinateSystem, out var valid)) return valid;

            coordinateSystem.Parse(out var x, out var y, out var z);
            valid = x.IsValidSingleAxisDirection() && y.IsValidSingleAxisDirection() && z.IsValidSingleAxisDirection();
            if (valid)
            {
                var xyz = (int)(x | y | z);
                valid = (xyz & (R | L)) != 0 && (xyz & (U | D)) != 0 && (xyz & (F | B)) != 0;
            }
            cache[coordinateSystem] = valid;

            return valid;
        }

        /// <summary>
        /// 是有效的单一轴方向
        /// </summary>
        /// <param name="xyzAxis"></param>
        /// <returns></returns>
        public static bool IsValidSingleAxisDirection(this EAxisDirection xyzAxis)
        {
            if ((xyzAxis & (~AllEAxisDirection)) != 0) return false;
            bool valid = false;
            foreach (var a in EnumCache<EAxisDirection>.Array)
            {
                if ((a & xyzAxis) != 0)
                {
                    if (valid) return false;
                    valid = true;
                }
            }
            return valid;
        }

        /// <summary>
        /// 转轴方向字符串
        /// </summary>
        /// <param name="xyzAxis"></param>
        /// <param name="enumStringType"></param>
        /// <returns></returns>
        public static string ToAxisDirectionString(this EAxisDirection xyzAxis, EEnumStringType enumStringType = EEnumStringType.Default)
        {
            var str = "";
            foreach (var a in EnumCache<EAxisDirection>.Array)
            {
                if ((a & xyzAxis) != 0)
                {
                    str += EnumStringCache.Get(a, enumStringType);
                }
            }
            return str;
        }

        /// <summary>
        /// 转坐标系字符串
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <param name="enumStringType"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToCoordinateSystemString(this ECoordinateSystem coordinateSystem, EEnumStringType enumStringType = EEnumStringType.Default, string separator = "_")
        {
            coordinateSystem.Parse(out var x, out var y, out var z);
            return nameof(X) + x.ToAxisDirectionString(enumStringType) + separator + nameof(Y) + y.ToAxisDirectionString(enumStringType) + separator + nameof(Z) + z.ToAxisDirectionString(enumStringType);
        }

        /// <summary>
        /// 获取坐标系模式
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        public static ECoordinateSystemMode GetCoordinateSystemMode(this ECoordinateSystem coordinateSystem)
        {
            var attribute = AttributeCache<CoordinateSystemConvertAttribute>.GetOfField(coordinateSystem);
            return attribute != null ? attribute.coordinateSystemMode : ECoordinateSystemMode.Unknow;
        }
    }

    #endregion    
}
