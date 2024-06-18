using XCSJ.Attributes;

namespace XCSJ.PluginXXR.Interaction.Toolkit.Base
{
    /// <summary>
    /// XR组手
    /// </summary>
    public static class XRHelper
    {
        /// <summary>
        /// 内置动作名数组：XRIT标准资产中定义的所有输入动作名
        /// </summary>
        public static string[] builtinActionNames { get; } = new string[]
        {
            "",
            "XRI Head/Position",
            "XRI Head/Rotation",
            "XRI Head/Tracking State",

            "XRI LeftHand/Position",
            "XRI LeftHand/Rotation",
            "XRI LeftHand/Tracking State",
            "XRI LeftHand/Haptic Device",
            "XRI LeftHand Interaction/Select",
            "XRI LeftHand Interaction/Select Value",
            "XRI LeftHand Interaction/Activate",
            "XRI LeftHand Interaction/Activate Value",
            "XRI LeftHand Interaction/UI Press",
            "XRI LeftHand Interaction/UI Press Value",
            "XRI LeftHand Interaction/Rotate Anchor",
            "XRI LeftHand Interaction/Translate Anchor",
            "XRI LeftHand Locomotion/Teleport Select",
            "XRI LeftHand Locomotion/Teleport Mode Activate",
            "XRI LeftHand Locomotion/Teleport Mode Cancel",
            "XRI LeftHand Locomotion/Move",
            "XRI LeftHand Locomotion/Turn",

            "XRI RightHand/Position",
            "XRI RightHand/Rotation",
            "XRI RightHand/Tracking State",
            "XRI RightHand/Haptic Device",
            "XRI RightHand Interaction/Select",
            "XRI RightHand Interaction/Select Value",
            "XRI RightHand Interaction/Activate",
            "XRI RightHand Interaction/Activate Value",
            "XRI RightHand Interaction/UI Press",
            "XRI RightHand Interaction/UI Press Value",
            "XRI RightHand Interaction/Rotate Anchor",
            "XRI RightHand Interaction/Translate Anchor",
            "XRI RightHand Locomotion/Teleport Select",
            "XRI RightHand Locomotion/Teleport Mode Activate",
            "XRI RightHand Locomotion/Teleport Mode Cancel",
            "XRI RightHand Locomotion/Move",
            "XRI RightHand Locomotion/Turn",

            "XRI UI/Navigate",
            "XRI UI/Submit",
            "XRI UI/Cancel",
            "XRI UI/Point",
            "XRI UI/Click",
            "XRI UI/ScrollWheel",
            "XRI UI/MiddleClick",
            "XRI UI/RightClick",
        };
    }

    #region XR设备类型

    /// <summary>
    /// XR设备类型
    /// </summary>
    [Name("XR设备类型")]
    public enum EXRDeviceType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 头
        /// </summary>
        [Name("头")]
        HMD,

        /// <summary>
        /// 左
        /// </summary>
        [Name("左")]
        Left,

        /// <summary>
        /// 右
        /// </summary>
        [Name("右")]
        Right,
    }

    #endregion

    #region XR按钮

    /// <summary>
    /// XR按钮
    /// </summary>
    public enum EXRButton
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 菜单按钮
        /// </summary>
        [Name("菜单按钮")]
        MenuButton = 1,

        /// <summary>
        /// 触发
        /// </summary>
        [Name("触发")]
        Trigger = 2,

        /// <summary>
        /// 抓
        /// </summary>
        [Name("抓")]
        Grip = 3,

        /// <summary>
        /// 触发按钮
        /// </summary>
        [Name("触发按钮")]
        TriggerButton = 4,

        /// <summary>
        /// 抓按钮
        /// </summary>
        [Name("抓按钮")]
        GripButton = 5,

        /// <summary>
        /// 初级按钮
        /// </summary>
        [Name("初级按钮")]
        PrimaryButton = 6,

        /// <summary>
        /// 初级触摸
        /// </summary>
        [Name("初级触摸")]
        PrimaryTouch = 7,

        /// <summary>
        /// 次级按钮
        /// </summary>
        [Name("次级按钮")]
        SecondaryButton = 8,

        /// <summary>
        /// 次级触摸
        /// </summary>
        [Name("次级触摸")]
        SecondaryTouch = 9,

        /// <summary>
        /// 初级2D轴触摸
        /// </summary>
        [Name("初级2D轴触摸")]
        Primary2DAxisTouch = 10,

        /// <summary>
        /// 初级2D轴点击
        /// </summary>
        [Name("初级2D轴点击")]
        Primary2DAxisClick = 11,

        /// <summary>
        /// 次级2D轴触摸
        /// </summary>
        [Name("次级2D轴触摸")]
        Secondary2DAxisTouch = 12,

        /// <summary>
        /// 次级2D轴点击
        /// </summary>
        [Name("次级2D轴点击")]
        Secondary2DAxisClick = 13,

        /// <summary>
        /// 初级轴2D上
        /// </summary>
        [Name("初级轴2D上")]
        PrimaryAxis2DUp = 14,

        /// <summary>
        /// 初级轴2D下
        /// </summary>
        [Name("初级轴2D下")]
        PrimaryAxis2DDown = 15,

        /// <summary>
        /// 初级轴2D左
        /// </summary>
        [Name("初级轴2D左")]
        PrimaryAxis2DLeft = 16,

        /// <summary>
        /// 初级轴2D右
        /// </summary>
        [Name("初级轴2D右")]
        PrimaryAxis2DRight = 17,

        /// <summary>
        /// 次级轴2D上
        /// </summary>
        [Name("次级轴2D上")]
        SecondaryAxis2DUp = 18,

        /// <summary>
        /// 次级轴2D下
        /// </summary>
        [Name("次级轴2D下")]
        SecondaryAxis2DDown = 19,

        /// <summary>
        /// 次级轴2D左
        /// </summary>
        [Name("次级轴2D左")]
        SecondaryAxis2DLeft = 20,

        /// <summary>
        /// 次级轴2D右
        /// </summary>
        [Name("次级轴2D右")]
        SecondaryAxis2DRight = 21
    }

    #endregion
}
