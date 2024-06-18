using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.ComponentModel;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Base.Units;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginStereoView.Base;
using XCSJ.PluginXBox.Base;
using XCSJ.PluginART.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

#if UNITY_2018_1_OR_NEWER
using UnityEngine;
#endif

namespace XCSJ.PluginXRSpaceSolution.Base
{
    /// <summary>
    /// XR扩展问题
    /// </summary>
    public abstract class XRExtensionQ : XRQuestion { }

    /// <summary>
    /// XR扩展答案
    /// </summary>
    public abstract class XRExtensionA : XRAnswer { }

    /// <summary>
    /// 重置配置答案
    /// </summary>
    [Import]
    public class ResetConfigA : XRExtensionA
    {
        /// <summary>
        /// 重置配置
        /// </summary>
        public EResetConfig resetConfig { get; set; } = EResetConfig.Pose;

        /// <summary>
        /// 动作名
        /// </summary>
        public string actionName { get; set; } = "";
    }

    /// <summary>
    /// 重置配置
    /// </summary>
    public enum EResetConfig
    {
        /// <summary>
        /// 姿态
        /// </summary>
        Pose,
    }

    /// <summary>
    /// 延时函数答案：用于延时执行脚本函数
    /// </summary>
    [Import]
    public class DelayFunctionA : XRExtensionA
    {
        /// <summary>
        /// 延时事件
        /// </summary>
        public float delayTime { get; set; } = 0;

        /// <summary>
        /// 脚本字符串列表
        /// </summary>
        public List<string> scriptStrings { get; set; } = new List<string>();
    }

    /// <summary>
    /// XR空间配置答案:空间显示与跟踪交互
    /// </summary>
    [Import]
    public class XRSpaceConfigA : XRExtensionA, IName, ICustomEnumStringConverter
    {
        string IName.name { get => "XR交互空间"; set { } }

        /// <summary>
        /// 构造
        /// </summary>
        public XRSpaceConfigA()
        {
            Init();
        }

        void Init()
        {
            cameras.ForEach(i => i.SetConfig(this));
            screens.ForEach(i => i.SetConfig(this));
            actions.ForEach(i => i.SetConfig(this));
            fastConfig.SetConfig(this);
        }

        /// <summary>
        /// 配置模式
        /// </summary>
        internal EConfigMode configMode { get => fastConfig.configMode; set => fastConfig.configMode = value; }

        #region 快速配置

        /// <summary>
        /// 快速配置
        /// </summary>
        [Browsable(false)]
        public FastConfig fastConfig { get; set; } = new FastConfig();

        #endregion

        #region 空间配置


        bool _isDirty = false;

        internal bool isDirty
        {
            get => _isDirty;
            set
            {
                _isDirty = value;
            }
        }

        internal int dirtyVersion = 0;

        /// <summary>
        /// 标记脏
        /// </summary>
        /// <param name="effectFastConfig"></param>
        public void MarkDirty(bool effectFastConfig = true)
        {
            isDirty = true;
            dirtyVersion++;
            fastConfig.MarkDirtyIfNeed(effectFastConfig);
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public XRSpaceConfigA Clone() => FromJson<XRSpaceConfigA>(this.ToJson());

        /// <summary>
        /// 当反序列化之后回调
        /// </summary>
        /// <param name="serializeContext">序列化上下文</param>
        public override void OnAfterDeserialize(ISerializeContext serializeContext)
        {
            base.OnAfterDeserialize(serializeContext);
            Init();
            fastConfig.CheckConfigMode();
        }

        /// <summary>
        /// 构建内置配置
        /// </summary>
        public void CreateBuildinConfig()
        {
            var screen = AddScreen("屏幕前", "前");
            screen.screenPose.position = new V3F(0, 1, 2);

            AddCamera("相机前", "屏幕前");

            CreateBuildinActions();
        }

        /// <summary>
        /// 构建内置动作列表
        /// </summary>
        public void CreateBuildinActions()
        {
            foreach (var a in EnumCache<EActionName>.Array)
            {
                AddAction(NameAttribute.ValueName<NameAttribute>(EnumFieldInfoCache.GetFieldInfo(a)));
            }
        }

        #endregion

        #region 空间偏移

        /// <summary>
        /// 空间偏移
        /// </summary>
        [Browsable(false)]
        public Pose spaceOffset { get; set; } = new Pose();

        /// <summary>
        /// 空间偏移位置
        /// </summary>
        [Category("空间偏移")]
        [DisplayName("位置")]
        [Description("XR交互空间的子级对象；用于统一管理XR交互空间中客观静态存在的对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中的地面的空间原点；")]
        [Json(false)]
        public V3F spaceOffset_Position { get => spaceOffset.position; set => spaceOffset.position = value; }

        /// <summary>
        /// 空间偏移旋转
        /// </summary>
        [Category("空间偏移")]
        [DisplayName("旋转")]
        [Description("XR交互空间的子级对象；用于统一管理XR交互空间中客观静态存在的对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中的地面的空间原点；")]
        [Json(false)]
        public V3F spaceOffset_Rotation { get => spaceOffset.rotation; set => spaceOffset.rotation = value; }

        #endregion

        #region 屏幕组

        /// <summary>
        /// 屏幕组偏移
        /// </summary>
        [Browsable(false)]
        public Pose screenGroupOffset { get; set; } = new Pose();

        /// <summary>
        /// 屏幕组偏移位置
        /// </summary>
        [Category("屏幕组")]
        [DisplayName("位置")]
        [Description("空间偏移的子级对象；用于统一管理XR交互空间中客观静态存在的所有虚拟屏幕对象；即其位置与旋转信息均相对于空间偏移对象进行处理；通常可以理解为XR实验室中的地面上摆放的多个屏幕设备的组合体的空间原点；")]
        [Json(false)]
        public V3F screenGroupOffset_Position { get => screenGroupOffset.position; set => screenGroupOffset.position = value; }

        /// <summary>
        /// 屏幕组偏移旋转
        /// </summary>
        [Category("屏幕组")]
        [DisplayName("旋转")]
        [Description("空间偏移的子级对象；用于统一管理XR交互空间中客观静态存在的所有虚拟屏幕对象；即其位置与旋转信息均相对于空间偏移对象进行处理；通常可以理解为XR实验室中的地面上摆放的多个屏幕设备的组合体的空间原点；")]
        [Json(false)]
        public V3F screenGroupOffset_Rotation { get => screenGroupOffset.rotation; set => screenGroupOffset.rotation = value; }

        #endregion

        #region 相机控制

        /// <summary>
        /// 相机控制
        /// </summary>
        [Category("相机控制")]
        [Description("XR交互空间内相机控制器属性参数的全局配置；")]
        [Browsable(false)]
        public UnityCameraControl unityCameraControl { get; set; } = new UnityCameraControl();

        /// <summary>
        /// 启用屏幕相机关联
        /// </summary>
        [Category("相机控制")]
        [DisplayName("启用屏幕相机关联")]
        [Description("用于整体控制是否启用相机与用户自定义屏幕的关联；多用于在多屏幕（不同角度）、有姿态跟踪（即动作捕捉）设备情况下的立体显示控制；影响【相机透视】组件的【更新模式】参数；为是时，使用【自定义虚拟屏幕】做计算；为否时，使用【Unity虚拟屏幕】做计算；")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool enableScreenCameraLink { get => unityCameraControl.enableScreenCameraLink; set => unityCameraControl.enableScreenCameraLink = value; }

        /// <summary>
        /// 左右眼交换
        /// </summary>
        [Category("相机控制")]
        [DisplayName("左右眼交换")]
        [Description("启用时将左右眼的信息交换；")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool switchLREye { get => unityCameraControl.switchLREye; set => unityCameraControl.switchLREye = value; }

        /// <summary>
        /// 左右眼矩阵模式
        /// </summary>
        [Category("相机控制")]
        [DisplayName("左右眼矩阵模式")]
        [Description("立体渲染启用时本参数有效，用于基于当前相机的左右眼透视矩阵与视图矩阵的不同计算方法；影响【相机透视】组件的【左右眼矩阵模式】参数；")]
        [TypeConverter(typeof(EnumStringConverter))]
        [Json(false)]
        public ELREyeMatrixMode LREyeMatrixMode { get => unityCameraControl.LREyeMatrixMode; set => unityCameraControl.LREyeMatrixMode = value; }

        /// <summary>
        /// 相机变换处理规则
        /// </summary>
        [Category("相机控制")]
        [DisplayName("相机变换处理规则")]
        [Description("当配置更新时，相机变换的处理规则；")]
        [TypeConverter(typeof(EnumStringConverter))]
        [Json(false)]
        public ECameraTransformHandleRule cameraTransformHandleRule { get => unityCameraControl.cameraTransformHandleRule; set => unityCameraControl.cameraTransformHandleRule = value; }

        /// <summary>
        /// 允许直接变换控制
        /// </summary>
        [Category("相机控制")]
        [DisplayName("允许直接变换控制")]
        [Description("允许不同输入方式（包括XRIS、ART、OptiTrack、ZVR、XR头盔设备等）直接控制相机的变换信息（T位置、R旋转、S缩放）")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool allowDirectTransformControl { get => unityCameraControl.allowDirectTransformControl; set => unityCameraControl.allowDirectTransformControl = value; }

        /// <summary>
        /// 允许移动控制
        /// </summary>
        [Category("相机控制")]
        [DisplayName("允许移动控制")]
        [Description("允许通过鼠标、键盘、触摸、手柄等输入方式控制相机的移动")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool allowMoveControl { get => unityCameraControl.allowMoveControl; set => unityCameraControl.allowMoveControl = value; }

        /// <summary>
        /// 允许旋转控制
        /// </summary>
        [Category("相机控制")]
        [DisplayName("允许旋转控制")]
        [Description("允许通过鼠标、键盘、触摸、手柄等输入方式控制相机的旋转")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool allowRotateControl { get => unityCameraControl.allowRotateControl; set => unityCameraControl.allowRotateControl = value; }

        /// <summary>
        /// 允许屏幕边界控制
        /// </summary>
        [Category("相机控制")]
        [DisplayName("允许屏幕边界控制")]
        [Description("允许鼠标在屏幕边界时控制相机的移动；在参数【允许移动控制】启用时，本参数设置才可生效；")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool allowScreenBoundaryControl { get => unityCameraControl.allowScreenBoundaryControl; set => unityCameraControl.allowScreenBoundaryControl = value; }

        #endregion

        #region 相机配置

        /// <summary>
        /// 相机配置
        /// </summary>
        [Category("相机配置")]
        [DisplayName("相机配置")]
        [Description("XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Browsable(false)]
        public UnityCameraConfig unityCameraConfig { get; set; } = new UnityCameraConfig();

        /// <summary>
        /// 裁剪面-近
        /// </summary>
        [Category("相机配置")]
        [DisplayName("裁剪面-近")]
        [Description("近剪裁平面与摄影机的距离，单位为Unity世界单位，默认为米；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        public float nearClipPlane { get => unityCameraConfig.nearClipPlane; set => unityCameraConfig.nearClipPlane = value; }

        /// <summary>
        /// 裁剪面-远
        /// </summary>
        [Category("相机配置")]
        [DisplayName("裁剪面-远")]
        [Description("远剪裁平面与摄影机的距离，单位为Unity世界单位，默认为米；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        public float farClipPlane { get => unityCameraConfig.farClipPlane; set => unityCameraConfig.farClipPlane = value; }

        /// <summary>
        /// 渲染路径
        /// </summary>
        [Category("相机配置")]
        [DisplayName("渲染路径")]
        [Description("如果可能的话，应该使用的渲染路径；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        //[TypeConverter(typeof(EnumStringConverter))]
        public ERenderingPath renderingPath { get => unityCameraConfig.renderingPath; set => unityCameraConfig.renderingPath = value; }

        /// <summary>
        /// 允许HDR
        /// </summary>
        [Category("相机配置")]
        [DisplayName("允许HDR")]
        [Description("高动态范围渲染；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool allowHDR { get => unityCameraConfig.allowHDR; set => unityCameraConfig.allowHDR = value; }

        /// <summary>
        /// 允许MSAA
        /// </summary>
        [Category("相机配置")]
        [DisplayName("允许MSAA")]
        [Description("MSAA渲染；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool allowMSAA { get => unityCameraConfig.allowMSAA; set => unityCameraConfig.allowMSAA = value; }

        /// <summary>
        /// 允许动态分辨率
        /// </summary>
        [Category("相机配置")]
        [DisplayName("允许动态分辨率")]
        [Description("动态分辨率缩放；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool allowDynamicResolution { get => unityCameraConfig.allowDynamicResolution; set => unityCameraConfig.allowDynamicResolution = value; }

        /// <summary>
        /// 立体分离
        /// </summary>
        [Category("相机配置")]
        [DisplayName("立体分离")]
        [Description("虚拟眼睛之间的距离；单位为米；使用此选项可以查询或设置当前的眼睛间距；请注意，大多数VR设备都提供该值，在这种情况下，设置该值将没有任何效果；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        public float stereoSeparation { get => unityCameraConfig.stereoSeparation; set => unityCameraConfig.stereoSeparation = value; }

        /// <summary>
        /// 立体融合
        /// </summary>
        [Category("相机配置")]
        [DisplayName("立体融合")]
        [Description("到虚拟眼睛汇聚点的距离；单位为米；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        public float stereoConvergence { get => unityCameraConfig.stereoConvergence; set => unityCameraConfig.stereoConvergence = value; }


        #endregion

        #region 相机偏移

        /// <summary>
        /// 相机偏移
        /// </summary>
        [Browsable(false)]
        public ObjectOffset cameraOffset { get; set; } = new ObjectOffset();

        /// <summary>
        /// 启用相机
        /// </summary>
        [Category("相机偏移")]
        [DisplayName("启用相机")]
        [Description("用于控制[相机偏移]游戏对象是否激活；XR交互空间的子级对象；用于管理XR交互空间中需要跟踪交互的HMD对象；通常可以理解为XR实验室中人员穿戴的头盔设备的空间原点；")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool enableCamera { get => cameraOffset.enable; set => cameraOffset.enable = value; }

        /// <summary>
        /// 相机偏移位置
        /// </summary>
        [Category("相机偏移")]
        [DisplayName("位置")]
        [Description("XR交互空间的子级对象；用于管理XR交互空间中需要跟踪交互的HMD对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中人员穿戴的头盔设备的空间原点；")]
        [Json(false)]
        public V3F cameraOffset_Position { get => cameraOffset.position; set => cameraOffset.position = value; }

        /// <summary>
        /// 相机偏移旋转
        /// </summary>
        [Category("相机偏移")]
        [DisplayName("旋转")]
        [Description("XR交互空间的子级对象；用于管理XR交互空间中需要跟踪交互的HMD对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中人员穿戴的头盔设备的空间原点；")]
        [Json(false)]
        public V3F cameraOffset_Rotation { get => cameraOffset.rotation; set => cameraOffset.rotation = value; }

        #endregion

        #region 左手

        /// <summary>
        /// 左手控制器
        /// </summary>
        [Browsable(false)]
        public XRControllerConfig leftController { get; set; } = new XRControllerConfig();

        /// <summary>
        /// 左手模型名
        /// </summary>
        [Category("左手控制器")]
        [DisplayName("模型名")]
        [Description("用于控制[左手控制器]模型对应显示的游戏对象；")]
        [TypeConverter(typeof(CustomEnumStringConverter))]
        [AllowNotInCustomEnumStrings]
        [Json(false)]
        public string leftModelName { get => leftController.modelName; set => leftController.modelName = value; }

        /// <summary>
        /// 获取自定义枚举字符串
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string[] GetCustomEnumStrings(ITypeDescriptorContext context)
        {
            switch (context.PropertyDescriptor.Name)
            {
                case nameof(leftModelName):
                case nameof(rightModelName):
                    {
                        return XRControllerConfig.buildinModelNames;
                    }
            }
            return Empty<string>.Array;
        }

        #endregion

        #region 左手偏移

        /// <summary>
        /// 左手偏移
        /// </summary>
        [Browsable(false)]
        public ObjectOffset leftOffset { get; set; } = new ObjectOffset();

        /// <summary>
        /// 启用左手
        /// </summary>
        [Category("左手偏移")]
        [DisplayName("启用左手")]
        [Description("用于控制[左手偏移]游戏对象是否激活；XR交互空间的子级对象；用于统一管理XR交互空间中需要跟踪交互的左手对象；通常可以理解为XR实验室中人员手持的左手控制器设备的空间原点；")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool enableLeft { get => leftOffset.enable; set => leftOffset.enable = value; }

        /// <summary>
        /// 左手偏移位置
        /// </summary>
        [Category("左手偏移")]
        [DisplayName("位置")]
        [Description("XR交互空间的子级对象；用于统一管理XR交互空间中需要跟踪交互的左手对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中人员手持的左手控制器设备的空间原点；")]
        [Json(false)]
        public V3F leftOffset_Position { get => leftOffset.position; set => leftOffset.position = value; }

        /// <summary>
        /// 左手偏移旋转
        /// </summary>
        [Category("左手偏移")]
        [DisplayName("旋转")]
        [Description("XR交互空间的子级对象；用于统一管理XR交互空间中需要跟踪交互的左手对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中人员手持的左手控制器设备的空间原点；")]
        [Json(false)]
        public V3F leftOffset_Rotation { get => leftOffset.rotation; set => leftOffset.rotation = value; }

        #endregion

        #region 右手

        /// <summary>
        /// 右手控制器
        /// </summary>
        [Browsable(false)]
        public XRControllerConfig rightController { get; set; } = new XRControllerConfig();

        /// <summary>
        /// 右手模型名
        /// </summary>
        [Category("右手控制器")]
        [DisplayName("模型名")]
        [Description("用于控制[右手控制器]模型对应显示的游戏对象；")]
        [TypeConverter(typeof(CustomEnumStringConverter))]
        [AllowNotInCustomEnumStrings]
        [Json(false)]
        public string rightModelName { get => rightController.modelName; set => rightController.modelName = value; }

        #endregion

        #region 右手偏移

        /// <summary>
        /// 右手偏移
        /// </summary>
        [Browsable(false)]
        public ObjectOffset rightOffset { get; set; } = new ObjectOffset();

        /// <summary>
        /// 启用右手
        /// </summary>
        [Category("右手偏移")]
        [DisplayName("启用右手")]
        [Description("用于控制[右手偏移]游戏对象是否激活；XR交互空间的子级对象；用于统一管理XR交互空间中需要跟踪交互的左手对象；通常可以理解为XR实验室中人员手持的左手控制器设备的空间原点；")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool enableRight { get => rightOffset.enable; set => rightOffset.enable = value; }

        /// <summary>
        /// 右手偏移位置
        /// </summary>
        [Category("右手偏移")]
        [DisplayName("位置")]
        [Description("XR交互空间的子级对象；用于统一管理XR交互空间中需要跟踪交互的右手对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中人员手持的右手控制器设备的空间原点；")]
        [Json(false)]
        public V3F rightOffset_Position { get => rightOffset.position; set => rightOffset.position = value; }

        /// <summary>
        /// 右手偏移旋转
        /// </summary>
        [Category("右手偏移")]
        [DisplayName("旋转")]
        [Description("XR交互空间的子级对象；用于统一管理XR交互空间中需要跟踪交互的右手对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中人员手持的右手控制器设备的空间原点；")]
        [Json(false)]
        public V3F rightOffset_Rotation { get => rightOffset.rotation; set => rightOffset.rotation = value; }

        #endregion

        #region 屏幕列表

        /// <summary>
        /// 屏幕列表
        /// </summary>
        [Browsable(false)]
        public List<ScreenInfo> screens { get; set; } = new List<ScreenInfo>();

        /// <summary>
        /// 获取屏幕
        /// </summary>
        /// <param name="screenName"></param>
        /// <returns></returns>
        public ScreenInfo GetScreen(string screenName) => screens.FirstOrDefault(i => i.name == screenName);

        /// <summary>
        /// 添加屏幕：存在同名的则不执行添加，并返回该同名的对象
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="screenDisplayName"></param>
        /// <returns></returns>
        internal ScreenInfo AddScreen(string screenName, string screenDisplayName)
        {
            var screenInfo = GetScreen(screenName);
            if (screenInfo != null) return screenInfo;

            screenInfo = new ScreenInfo() { name = screenName, displayName = screenDisplayName };
            screenInfo.SetConfig(this);
            screens.Add(screenInfo);
            return screenInfo;
        }

        internal ScreenInfo MustAddScreen()
        {
            int i = 1;
            string name;
            do
            {
                name = "屏幕" + i;
                i++;
            } while (GetScreen(name) != null);
            return AddScreen(name, name);
        }

        /// <summary>
        /// 设置屏幕数量：即确保屏幕列表中至少有对应数量的屏幕，将多余的屏幕全部删除；少补多删；
        /// </summary>
        /// <param name="count"></param>
        internal void SetScreenCount(int count)
        {
            if (count <= 0)
            {
                screens.Clear();
                return;
            }
            int deleteCount = screens.Count - count;
            if (deleteCount == 0) return;

            if (deleteCount > 0)//多删
            {
                screens.RemoveRange(count, deleteCount);
            }
            else//少补
            {
                while (deleteCount++ < 0)
                {
                    MustAddScreen();
                }
            }
        }

        #endregion

        #region 相机列表

        /// <summary>
        /// 相机列表
        /// </summary>
        [Browsable(false)]
        public List<CameraInfo> cameras { get; set; } = new List<CameraInfo>();

        /// <summary>
        /// 获取相机
        /// </summary>
        /// <param name="cameraName"></param>
        /// <returns></returns>
        public CameraInfo GetCamera(string cameraName) => cameras.FirstOrDefault(i => i.name == cameraName);

        /// <summary>
        /// 添加相机：存在同名的则不执行添加，并返回该同名的对象
        /// </summary>
        internal CameraInfo AddCamera(string cameraName, string screenName)
        {
            var cameraInfo = GetCamera(cameraName);
            if (cameraInfo != null) return cameraInfo;

            cameraInfo = new CameraInfo() { name = cameraName, screen = screenName };
            cameraInfo.SetConfig(this);
            cameras.Add(cameraInfo);
            return cameraInfo;
        }

        internal CameraInfo MustAddCamera()
        {
            int i = 1;
            string name;
            do
            {
                name = "相机" + i;
                i++;
            } while (GetCamera(name) != null);
            return AddCamera(name, screens.FirstOrDefault()?.name ?? "");
        }

        /// <summary>
        /// 设置相机数量：即确保相机列表中至少有对应数量的相机，将多余的相机全部删除；少补多删；
        /// </summary>
        /// <param name="count"></param>
        internal void SetCameraCount(int count)
        {
            if (count <= 0)
            {
                cameras.Clear();
                return;
            }
            int deleteCount = cameras.Count - count;
            if (deleteCount == 0) return;

            if (deleteCount > 0)//多删
            {
                cameras.RemoveRange(count, deleteCount);
            }
            else//少补
            {
                while (deleteCount++ < 0)
                {
                    MustAddCamera();
                }
            }
        }

        #endregion

        #region 动作列表

        /// <summary>
        /// 动作列表
        /// </summary>
        [Browsable(false)]
        public List<ActionInfo> actions { get; set; } = new List<ActionInfo>();

        /// <summary>
        /// 获取动作信息
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public ActionInfo GetActionInfo(string actionName) => actions.FirstOrDefault(i => i.name == actionName);

        /// <summary>
        /// 添加动作：存在同名的则不执行添加，并返回该同名的对象
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public ActionInfo AddAction(string actionName)
        {
            var actionInfo = GetActionInfo(actionName);
            if (actionInfo != null) return actionInfo;

            actionInfo = new ActionInfo() { name = actionName };
            actionInfo.SetConfig(this);
            actions.Add(actionInfo);
            return actionInfo;
        }

        #endregion
    }

    #region 快速配置

    /// <summary>
    /// 用于快速配置的基础信息
    /// </summary>
    public class BaseInfoForFastConfig : AbstractInfo
    {
        /// <summary>
        /// 快速配置
        /// </summary>
        internal FastConfig fastConfig => config?.fastConfig;
    }

    /// <summary>
    /// 配置模式
    /// </summary>
    public enum EConfigMode
    {
        /// <summary>
        /// 快速
        /// </summary>
        Fast = 0,

        /// <summary>
        /// 专业
        /// </summary>
        Professional,
    }

    /// <summary>
    /// 屏幕模式
    /// </summary>
    public enum EScreenMode
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknow,

        /// <summary>
        /// 直幕
        /// </summary>
        One = 10,

        /// <summary>
        /// 双折幕
        /// </summary>
        Two = 20,

        /// <summary>
        /// 三折幕FLR
        /// </summary>
        Three_FLR = 30,

        /// <summary>
        /// 三折幕FLD
        /// </summary>
        Three_FLD,

        /// <summary>
        /// 四折幕
        /// </summary>
        Four = 40,

        /// <summary>
        /// 弧幕:由多块屏幕组合成带有弧度的屏幕；
        /// </summary>
        Radian = 100,
    }

    /// <summary>
    /// 标准屏幕
    /// </summary>
    [Import]
    public class FrontScreen : BaseInfoForFastConfig
    {
        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public float screenWidth { get; set; } = 1.92f;

        /// <summary>
        /// 屏幕高度
        /// </summary>
        public float screenHeigth { get; set; } = 1.08f;

        /// <summary>
        /// 屏幕底端到地面距离
        /// </summary>
        public float screenDownToGroundDistance { get; set; } = 0;

        /// <summary>
        /// 屏幕到原点距离
        /// </summary>
        public float screenToOriginDistance { get; set; } = 2;
    }

    /// <summary>
    /// 其他屏幕
    /// </summary>
    [Import]
    public class OtherScreen : BaseInfoForFastConfig
    {
        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public float screenWidth { get; set; } = 1.92f;

        /// <summary>
        /// 屏幕高度
        /// </summary>
        public float screenHeigth { get; set; } = 1.08f;

        /// <summary>
        /// 与前幕夹角
        /// </summary>
        public float toFrontScreenAngle { get; set; } = 90f;
    }

    /// <summary>
    /// 弧幕:由多块屏幕组合成带有弧度的屏幕；
    /// </summary>
    [Import]
    public class RadianScreen : BaseInfoForFastConfig
    {
        /// <summary>
        /// 半径
        /// </summary>
        public float radius { get; set; } = 4;

        private float _angle = 90;

        /// <summary>
        /// 角度：单位度；
        /// </summary>
        public float angle { get => _angle; set => _angle = MathX.Clamp(value, 0, 360); }

        /// <summary>
        /// 宽度
        /// </summary>
        [Json(false)]
        public float width => MathX.Min(2 * radius, MathX.Abs(MathX.Sin(angle * MathX.Deg2Rad / 2) * radius * 2));

        /// <summary>
        /// 弧长
        /// </summary>
        [Json(false)]
        public float arcLength => angle * MathX.Deg2Rad * radius;

        /// <summary>
        /// 宽度/弧长
        /// </summary>
        [Json(false)]
        public string widthArcLength => width.ToString("f2") + "/" + arcLength.ToString("f2");

        /// <summary>
        /// 高度
        /// </summary>
        public float height { get; set; } = 2;

        /// <summary>
        /// 屏幕宽间距
        /// </summary>
        public float screenWidthSpace { get; set; } = 0;

        /// <summary>
        /// 屏幕高间距
        /// </summary>
        public float screenHeightSpace { get; set; } = 0;

        /// <summary>
        /// 屏幕宽数量
        /// </summary>
        public int screenWidthCount { get; set; } = 6;

        /// <summary>
        /// 屏幕高数量
        /// </summary>
        public int screenHeightCount { get; set; } = 2;

        /// <summary>
        /// 屏幕底端到地面距离
        /// </summary>
        public float screenDownToGroundDistance { get; set; } = 0;

        /// <summary>
        /// 屏幕到原点距离
        /// </summary>
        public float screenToOriginDistance { get; set; } = 4;
    }

    /// <summary>
    /// 立体模式
    /// </summary>
    [Name("立体模式")]
    public enum EStereoMode
    {
        /// <summary>
        /// 未知：由专业配置模式确定的立体模式；
        /// </summary>
        [Name("未知")]
        Unknow = -1,

        /// <summary>
        /// 平面:即无立体
        /// </summary>
        [Name("平面")]
        Plane = 0,

        /// <summary>
        /// 主动立体
        /// </summary>
        [Name("主动立体")]
        ActiveStereo,

        /// <summary>
        /// 被动立体左右
        /// </summary>
        [Name("被动立体左右")]
        PassiveStereo_LR,

        /// <summary>
        /// 被动立体上下
        /// </summary>
        [Name("被动立体上下")]
        PassiveStereo_UD,
    }

    /// <summary>
    /// 相机配置
    /// </summary>
    [Import]
    public class CameraConfig : BaseInfoForFastConfig
    {
        /// <summary>
        /// 屏幕相机关联
        /// </summary>
        [Json(false)]
        public bool screenCameraLink
        {
            get
            {
                if (config != null) return config.enableScreenCameraLink;
                return true;
            }
            set
            {
                if (config != null) config.enableScreenCameraLink = value;
            }
        }

        /// <summary>
        /// 左右眼交换
        /// </summary>
        [Json(false)]
        public bool switchLREye
        {
            get
            {
                if (config != null) return config.switchLREye;
                return false;
            }
            set
            {
                if (config != null) config.switchLREye = value;
            }
        }

        /// <summary>
        /// 眼睛间距
        /// </summary>
        [Json(false)]
        public float eyeSpace
        {
            get
            {
                if (config != null) return config.stereoSeparation;
                return UnityCameraConfig.StereoSeparation;
            }
            set
            {
                if (config != null) config.stereoSeparation = value;
            }
        }

        /// <summary>
        /// 聚焦距离：眼睛焦距
        /// </summary>
        [Json(false)]
        public float eyeFocalDistance
        {
            get
            {
                if (config != null) return config.stereoConvergence;
                return UnityCameraConfig.StereoConvergence;
            }
            set
            {
                if (config != null) config.stereoConvergence = value;
            }
        }

        /// <summary>
        /// 近裁剪面
        /// </summary>
        [Json(false)]
        public float nearClipPlane
        {
            get
            {
                if (config != null) return config.nearClipPlane;
                return UnityCameraConfig.NearClipPlane;
            }
            set
            {
                if (config != null) config.nearClipPlane = value;
            }
        }

        /// <summary>
        /// 远裁剪面
        /// </summary>
        [Json(false)]
        public float farClipPlane
        {
            get
            {
                if (config != null) return config.farClipPlane;
                return UnityCameraConfig.FarClipPlane;
            }
            set
            {
                if (config != null) config.farClipPlane = value;
            }
        }

        /// <summary>
        /// 网格布局
        /// </summary>
        public bool gridLayout { get; set; } = true;
    }

    /// <summary>
    /// 空间型式
    /// </summary>
    public enum ESpatialPattern
    {
        /// <summary>
        /// 大空间动捕
        /// </summary>
        LargeSpaceMotionCapture,

        /// <summary>
        /// XR头显
        /// </summary>
        XRHMD,

        /// <summary>
        /// 普通
        /// </summary>
        Normal,
    }

    /// <summary>
    /// 快速配置
    /// </summary>
    [Import]
    public class FastConfig : BaseInfo, IOnAfterDeserialize
    {
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config"></param>
        public override void SetConfig(XRSpaceConfigA config)
        {
            base.SetConfig(config);

            frontScreen.SetConfig(config);
            leftScreen.SetConfig(config);
            rightScreen.SetConfig(config);
            downScreen.SetConfig(config);

            cameraConfig.SetConfig(config);
        }

        internal void CheckConfigMode()
        {
            if (!hasDeserializ)
            {
                _configMode = EConfigMode.Professional;
            }
        }

        //用于处理升级数据
        bool hasDeserializ = false;

        /// <summary>
        /// 当反序列化之后
        /// </summary>
        /// <param name="serializeContext"></param>
        public void OnAfterDeserialize(ISerializeContext serializeContext)
        {
            hasDeserializ = true;
        }

        /// <summary>
        /// 可用
        /// </summary>
        [Category("基础信息")]
        [DisplayName("可用")]
        [Description("标识当前信息是否生效；")]
        [Field(index = 1)]
        [TypeConverter(typeof(BoolStringConverter))]
        public override bool enable { get => configMode == EConfigMode.Fast; set => configMode = value ? EConfigMode.Fast : EConfigMode.Professional; }

        private EConfigMode _configMode = EConfigMode.Fast;

        /// <summary>
        /// 配置模式
        /// </summary>
        [Json(false)]
        public EConfigMode configMode
        {
            get => _configMode;
            set
            {
                if (value == _configMode) return;
                switch (value)
                {
                    case EConfigMode.Professional:
                        {
                            _configMode = EConfigMode.Professional;
                            if (config != null) config.isDirty = true;
                            break;
                        }
                    default:
                        {
                            FromXRConfigIfNeed();
                            _configMode = EConfigMode.Fast;
                            if (config != null) config.isDirty = true;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 屏幕模式
        /// </summary>
        public EScreenMode screenMode { get; set; } = EScreenMode.Unknow;

        /// <summary>
        /// 前幕
        /// </summary>
        public FrontScreen frontScreen { get; set; } = new FrontScreen();

        /// <summary>
        /// 左幕
        /// </summary>
        public OtherScreen leftScreen { get; set; } = new OtherScreen();

        /// <summary>
        /// 右幕
        /// </summary>
        public OtherScreen rightScreen { get; set; } = new OtherScreen();

        /// <summary>
        /// 下幕
        /// </summary>
        public OtherScreen downScreen { get; set; } = new OtherScreen();

        /// <summary>
        /// 弧幕
        /// </summary>
        public RadianScreen radianScreen { get; set; } = new RadianScreen();

        /// <summary>
        /// 立体模式
        /// </summary>
        public EStereoMode stereoMode { get; set; } = EStereoMode.Unknow;

        /// <summary>
        /// 相机配置
        /// </summary>
        public CameraConfig cameraConfig { get; set; } = new CameraConfig();

        /// <summary>
        /// 空间型式
        /// </summary>
        public ESpatialPattern spatialPattern { get; set; } = ESpatialPattern.LargeSpaceMotionCapture;

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="spatialPattern"></param>
        public void Set(ESpatialPattern spatialPattern)
        {
            this.spatialPattern = spatialPattern;
            switch (spatialPattern)
            {
                case ESpatialPattern.XRHMD:
                    {
                        cameraConfig.screenCameraLink = false;
                        config.switchLREye = false;
                        config.LREyeMatrixMode = ELREyeMatrixMode.None;
                        screenMode = EScreenMode.One;
                        stereoMode = EStereoMode.Plane;
                        break;
                    }
                case ESpatialPattern.Normal:
                    {
                        cameraConfig.screenCameraLink = false;
                        config.switchLREye = false;
                        config.LREyeMatrixMode = ELREyeMatrixMode.None;
                        screenMode = EScreenMode.One;
                        stereoMode = EStereoMode.Plane;
                        break;
                    }
                default:
                    {
                        cameraConfig.screenCameraLink = true;
                        config.LREyeMatrixMode = ELREyeMatrixMode.CompleteCalculation;
                        switch (stereoMode)
                        {
                            case EStereoMode.ActiveStereo:
                            case EStereoMode.PassiveStereo_LR:
                            case EStereoMode.PassiveStereo_UD:
                                {
                                    break;
                                }
                            default:
                                {
                                    stereoMode = EStereoMode.ActiveStereo;
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        internal int dirtyVersion = 0;

        /// <summary>
        /// 如果需要标记脏
        /// </summary>
        /// <param name="effectFastConfig"></param>
        public void MarkDirtyIfNeed(bool effectFastConfig)
        {
            if (enable)
            {
                dirtyVersion = config.dirtyVersion;
                ToXRConfig();
            }
            else if (!effectFastConfig)//不影响快速配置，那么同步更新版本
            {
                dirtyVersion = config.dirtyVersion;
            }
        }

        /// <summary>
        /// 配置处理器
        /// </summary>
        internal static IXRConfigHandler configHandler = null;

        /// <summary>
        /// 转XR配置：将快速配置的信息提交到专业配置参数中；
        /// </summary>
        private void ToXRConfig()
        {
            configHandler?.FastConfigToProfessionalConfig(this);
        }

        /// <summary>
        /// 从XR配置转化：由专业配置尝试转为快速配置；
        /// </summary>
        private void FromXRConfigIfNeed()
        {
            if (config.dirtyVersion == dirtyVersion) return;
            dirtyVersion = config.dirtyVersion;

            configHandler?.ProfessionalConfigToFastConfig(this);
        }
    }

    /// <summary>
    /// XR配置处理器
    /// </summary>
    interface IXRConfigHandler
    {
        /// <summary>
        /// 快速配置转专业配置
        /// </summary>
        /// <param name="fastConfig"></param>
        void FastConfigToProfessionalConfig(FastConfig fastConfig);

        /// <summary>
        /// 专业配置转快速配置
        /// </summary>
        /// <param name="fastConfig"></param>
        void ProfessionalConfigToFastConfig(FastConfig fastConfig);
    }

    #endregion

    #region 基础信息

    /// <summary>
    /// 抽象信息
    /// </summary>
    [Import]
    public abstract class AbstractInfo
    {
        /// <summary>
        /// 配置
        /// </summary>
        internal XRSpaceConfigA config { get; private set; }

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config"></param>
        public virtual void SetConfig(XRSpaceConfigA config) => this.config = config;
    }

    /// <summary>
    /// 基础信息
    /// </summary>
    public abstract class BaseInfo : AbstractInfo
    {
        /// <summary>
        /// 可用
        /// </summary>
        [Category("基础信息")]
        [DisplayName("启用")]
        [Description("标识当前信息是否生效；")]
        [Field(index = 1)]
        [TypeConverter(typeof(BoolStringConverter))]
        public virtual bool enable { get; set; } = false;
    }

    #endregion

    #region Unity相机控制

    /// <summary>
    /// Unity相机控制
    /// </summary>
    [Import]
    public class UnityCameraControl
    {
        /// <summary>
        /// 启用屏幕相机关联
        /// </summary>
        public bool enableScreenCameraLink { get; set; } = true;

        /// <summary>
        /// 左右眼交换：切换左右眼；
        /// </summary>
        public bool switchLREye { get; set; } = false;

        /// <summary>
        /// 左右眼矩阵模式
        /// </summary>
        public ELREyeMatrixMode LREyeMatrixMode { get; set; } = ELREyeMatrixMode.None;

        /// <summary>
        /// 相机相机变换处理规则
        /// </summary>
        public ECameraTransformHandleRule cameraTransformHandleRule { get; set; } = ECameraTransformHandleRule.None;

        /// <summary>
        /// 允许直接变换控制
        /// </summary>
        public bool allowDirectTransformControl { get; set; } = true;

        /// <summary>
        /// 允许移动控制
        /// </summary>
        public bool allowMoveControl { get; set; } = true;

        /// <summary>
        /// 允许旋转控制
        /// </summary>
        public bool allowRotateControl { get; set; } = true;

        /// <summary>
        /// 允许屏幕边界控制
        /// </summary>
        public bool allowScreenBoundaryControl { get; set; } = false;
    }

    /// <summary>
    /// 相机相机变换处理规则
    /// </summary>
    public enum ECameraTransformHandleRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [Tip("即不对相机的变换做任何额外处理")]
        None,

        /// <summary>
        /// 重置
        /// </summary>
        [Name("重置")]
        [Tip("将相机的变换信息重置到缺省状态，即相对父级位置、旋转均为0，缩放全为1的初始状态；")]
        Reset,

        /// <summary>
        /// 重置到启动
        /// </summary>
        [Name("重置到启动")]
        [Tip("将相机的变换信息重置到程序启动时记录的状态")]
        ResetToStart,
    }

    #endregion

    #region Unity相机配置

    /// <summary>
    /// Unity相机配置
    /// </summary>
    [Import]
    public class UnityCameraConfig
    {
        internal const float NearClipPlane = 0.01f;
        internal const float FarClipPlane = 1000;

        /// <summary>
        /// 近裁剪面
        /// </summary>
        public float nearClipPlane { get; set; } = NearClipPlane;

        /// <summary>
        /// 远裁剪面
        /// </summary>
        public float farClipPlane { get; set; } = FarClipPlane;

        /// <summary>
        /// 渲染路径
        /// </summary>
        public ERenderingPath renderingPath { get; set; } = ERenderingPath.Forward;

        /// <summary>
        /// 允许HDR
        /// </summary>
        public bool allowHDR { get; set; } = false;

        /// <summary>
        /// 允许多重采样抗锯齿
        /// </summary>
        public bool allowMSAA { get; set; } = false;

        /// <summary>
        /// 允许动态分辨率
        /// </summary>
        public bool allowDynamicResolution { get; set; } = false;

        internal const float StereoSeparation = 0.022f;
        internal const float StereoConvergence = 10;

        /// <summary>
        /// 立体分离
        /// </summary>
        public float stereoSeparation { get; set; } = StereoSeparation;

        /// <summary>
        /// 立体融合
        /// </summary>
        public float stereoConvergence { get; set; } = StereoConvergence;


#if UNITY_2019_4_OR_NEWER

        /// <summary>
        /// 设置相机配置
        /// </summary>
        /// <param name="camera"></param>
        public void SetCameraConfig(Camera camera)
        {
            camera.nearClipPlane = nearClipPlane;
            camera.farClipPlane = farClipPlane;
            camera.renderingPath = (RenderingPath)(int)renderingPath;
            camera.allowHDR = allowHDR;
            camera.allowMSAA = allowMSAA;
            camera.allowDynamicResolution = allowDynamicResolution;
            camera.stereoSeparation = stereoSeparation;
            camera.stereoConvergence = stereoConvergence;
        }

#endif

    }

    /// <summary>
    /// 阀门适配模式：用于指定Camera.sensorSize定义的传感器门（传感器帧）的方式的枚举适合分辨率门（渲染帧）。
    /// </summary>
    public enum EGateFitMode
    {
        /// <summary>
        /// 无：拉伸传感器门，使其完全适合分辨率门。
        /// </summary>
        [Name("无")]
        [Tip("拉伸传感器门，使其完全适合分辨率门。", "Stretch the sensor gate to fit exactly into the resolution gate.")]
        None = 0,

        /// <summary>
        /// 垂直：将分辨率门垂直安装在传感器门内。
        /// </summary>
        [Name("垂直")]
        [Tip("将分辨率门垂直安装在传感器门内。", "Fit the resolution gate vertically within the sensor gate.")]
        Vertical = 1,

        /// <summary>
        /// 水平：将分辨率门水平安装在传感器门内。
        /// </summary>
        [Name("水平")]
        [Tip("将分辨率门水平安装在传感器门内。", "Fit the resolution gate horizontally within the sensor gate.")]
        Horizontal = 2,

        /// <summary>
        /// 填充：自动选择水平或垂直配合，使传感器门完全配合在分辨率门内。
        /// </summary>
        [Name("填充")]
        [Tip("自动选择水平或垂直配合，使传感器门完全配合在分辨率门内。", "Automatically selects a horizontal or vertical fit so that the sensor gate fits completely inside the resolution gate.")]
        Fill = 3,

        /// <summary>
        /// 过扫描：自动选择水平或垂直拟合，以便渲染帧完全适合分辨率门内部。
        /// </summary>
        [Name("过扫描")]
        [Tip("自动选择水平或垂直拟合，以便渲染帧完全适合分辨率门内部。", "Automatically selects a horizontal or vertical fit so that the render frame fits completely inside the resolution gate.")]
        Overscan = 4
    }

    /// <summary>
    /// 渲染路径
    /// </summary>
    public enum ERenderingPath
    {
        /// <summary>
        /// 用户往家设置
        /// </summary>
        UsePlayerSettings = -1,

        /// <summary>
        /// 顶点照明
        /// </summary>
        VertexLit = 0,

        /// <summary>
        /// 前向渲染
        /// </summary>
        Forward = 1,

        /// <summary>
        /// 延迟照明:延迟照明（传统）
        /// </summary>
        DeferredLighting = 2,

        /// <summary>
        /// 延迟着色
        /// </summary>
        DeferredShading = 3
    }

    #endregion

    #region 相机

    /// <summary>
    /// 相机立体目标眼罩
    /// </summary>
    public enum ECameraStereoTargetEyeMask
    {
        /// <summary>
        /// 不要将任何一只眼睛渲染到HMD。
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 仅将左眼渲染到HMD。
        /// </summary>
        [Name("左眼")]
        Left = 1,

        /// <summary>
        /// 仅将右眼渲染到HMD
        /// </summary>
        [Name("右眼")]
        Right = 2,

        /// <summary>
        /// 将双眼渲染到HMD
        /// </summary>
        [Name("双眼")]
        Both = 3
    }

    /// <summary>
    /// 相机目标显示
    /// </summary>
    public enum ECameraTargetDisplay
    {
        /// <summary>
        /// 显示1
        /// </summary>
        Display1,

        /// <summary>
        /// 显示2
        /// </summary>
        Display2,

        /// <summary>
        /// 显示3
        /// </summary>
        Display3,

        /// <summary>
        /// 显示4
        /// </summary>
        Display4,

        /// <summary>
        /// 显示5
        /// </summary>
        Display5,

        /// <summary>
        /// 显示6
        /// </summary>
        Display6,

        /// <summary>
        /// 显示7
        /// </summary>
        Display7,

        /// <summary>
        /// 显示8
        /// </summary>
        Display8,
    }

    /// <summary>
    /// 相机信息
    /// </summary>
    [Import]
    public class CameraInfo : AbstractInfo, ICustomEnumStringConverter, IName
    {
        #region 信息

        /// <summary>
        /// 名称
        /// </summary>
        [Category("基础信息")]
        [DisplayName("名称")]
        [Description("当前相机的名称信息，会同步为Unity内标识对应的相机游戏对象的名称；")]
        [Field(index = 0)]
        public string name { get; set; } = "";

        /// <summary>
        /// 视口矩形
        /// </summary>
        [Category("基础信息")]
        [DisplayName("视口矩形")]
        [Description("基于Unity视口矩形坐标系：左下角为(0,0)原点，向右为X轴正方向，向上为Y轴正方向；XY与宽度高度区间值默认为[0，1]；")]
        [Field(index = 2)]
        [Json(exportString = true)]
        [ColumnHeader(150)]
        public RectF viewportRect { get => _viewportRect; set => _viewportRect = value; }

        /// <summary>
        /// 视口矩形
        /// </summary>
        protected RectF _viewportRect = new RectF(0, 0, 1, 1);

        /// <summary>
        /// 屏幕
        /// </summary>
        [Category("基础信息")]
        [DisplayName("屏幕")]
        [Description("当前相机进行投影计算时使用的屏幕名称；")]
        [Field(index = 3)]
        [TypeConverter(typeof(CustomEnumStringConverter))]
        public string screen { get; set; } = "";

        /// <summary>
        /// 相机立体目标眼罩
        /// </summary>
        [Category("基础信息")]
        [DisplayName("相机立体目标眼罩")]
        [Description("定义“摄影机”渲染到VR显示器的哪只眼睛；")]
        [Field(index = 6)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ECameraStereoTargetEyeMask stereoTargetEye { get; set; } = ECameraStereoTargetEyeMask.Both;

        /// <summary>
        /// 相机目标显示
        /// </summary>
        [Category("基础信息")]
        [DisplayName("相机目标显示")]
        [Description("设置此相机的目标显示,即该设置相机渲染到指定的显示中；支持的最大显示（例如显示器）数量为 8；")]
        [Field(index = 7)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ECameraTargetDisplay targetDisplay { get; set; } = ECameraTargetDisplay.Display1;

        #endregion

        #region 相机变换

        /// <summary>
        /// 相机偏移
        /// </summary>
        [Browsable(false)]
        [Field(ignore = true)]
        public Pose cameraOffset { get; set; } = new Pose();

        /// <summary>
        /// 相机变换位置
        /// </summary>
        [Category("相机变换")]
        [DisplayName("位置")]
        [Description("相对头盔类设备（初始时可认为是相机偏移对象）的位置偏移信息；以米为单位；使用Unity内左手坐标系,X右Y上Z前；X轴偏移量为左右水平距离差；Y轴偏移量为垂直距离差；Z轴偏移量为前后水平距离差；")]
        [Field(index = 4)]
        [Json(false)]
        public V3F cameraOffset_Position { get => cameraOffset.position; set => cameraOffset.position = value; }

        /// <summary>
        /// 相机变换旋转
        /// </summary>
        [Category("相机变换")]
        [DisplayName("旋转")]
        [Description("相机相对头盔类设备（初始时可认为是相机偏移对象）的旋转欧拉角度信息；以度为单位；使用Unity内左手坐标系,X右Y上Z前；")]
        [Field(index = 5)]
        [Json(false)]
        public V3F cameraOffset_Rotation { get => cameraOffset.rotation; set => cameraOffset.rotation = value; }

        #endregion

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public CameraInfo Clone() => new CameraInfo().CopyDataFrom(this);

        /// <summary>
        /// 从源拷贝数据
        /// </summary>
        /// <param name="cameraInfo"></param>
        /// <returns></returns>
        public CameraInfo CopyDataFrom(CameraInfo cameraInfo)
        {
            if (cameraInfo != null)
            {
                this.SetConfig(cameraInfo.config);

                this.name = cameraInfo.name;
                this.viewportRect = cameraInfo.viewportRect;
                this.screen = cameraInfo.screen;
                this.stereoTargetEye = cameraInfo.stereoTargetEye;
                this.targetDisplay = cameraInfo.targetDisplay;
                this.cameraOffset = cameraInfo.cameraOffset.Clone();
            }
            return this;
        }

        /// <summary>
        /// 获取自定义枚举字符串
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string[] GetCustomEnumStrings(ITypeDescriptorContext context)
        {
            switch (context.PropertyDescriptor.Name)
            {
                case nameof(screen):
                    {
                        if (config != null)
                        {
                            return config.screens.Cast(s => s.name).ToArray();
                        }
                        break;
                    }
            }
            return Empty<string>.Array;
        }
    }

    #endregion

    #region 屏幕

    /// <summary>
    /// 屏幕信息
    /// </summary>
    [Import]
    public class ScreenInfo : AbstractInfo, ICustomEnumStringConverter, IName
    {
        internal const float DefaultZ = 0.01F;

        #region 信息

        /// <summary>
        /// 名称
        /// </summary>
        [Category("基础信息")]
        [DisplayName("名称")]
        [Description("当前屏幕的名称信息，会同步为Unity内标识对应的屏幕游戏对象的名称；")]
        [Field(index = 0)]
        public string name { get; set; } = "";

        /// <summary>
        /// 显示名
        /// </summary>
        [Category("基础信息")]
        [DisplayName("显示名")]
        [Description("当前屏幕的显示名信息，会同步为Unity内标识对应的屏幕对象的提示信息名称；")]
        [Field(index = 2)]
        public string displayName { get; set; } = "";

        /// <summary>
        /// 屏幕尺寸
        /// </summary>
        [Category("基础信息")]
        [DisplayName("屏幕尺寸")]
        [Description("可理解为当前屏幕的物理客观尺寸信息，即表示屏幕立方体的宽高厚信息；以米为单位；使用Unity内左手坐标系,X右Y上Z前；X轴长度为当前屏幕的物理客观尺寸的宽度信息；Y轴长度为当前屏幕的物理客观尺寸的高度信息；Z轴长度为当前屏幕的物理客观尺寸的厚度信息，计算相机与屏幕的投影关系时，本值不参与运算，即使用默认值即可；")]
        [Field(index = 3)]
        [Json(exportString = true)]
        public V3F screenSize { get => _screenSize; set => _screenSize = value; }

        /// <summary>
        /// 屏幕尺寸
        /// </summary>
        protected V3F _screenSize = new V3F(4, 2, DefaultZ);

        /// <summary>
        /// 屏幕姿态模式
        /// </summary>
        [Category("基础信息")]
        [DisplayName("屏幕姿态模式")]
        [Description("用于标识屏幕的姿态如何定位")]
        [ColumnHeader(100)]
        [Field(index = 4)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EScreenPoseMode screenPoseMode { get; set; } = EScreenPoseMode.ScreenPose;

        /// <summary>
        /// 屏幕姿态信息
        /// </summary>
        [Category("基础信息")]
        [DisplayName("屏幕姿态信息")]
        [Description("屏幕姿态定位的结果信息")]
        [ColumnHeader(200)]
        [Field(index = 5)]
        [Json(false)]
        public string screen
        {
            get
            {
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose: return position.ToString();
                    case EScreenPoseMode.AnchorLink: return screenAnchorLinkInfo.ToString();
                    default: return "<无效的屏幕姿态模式>";
                }
            }
        }

        /// <summary>
        /// 有效性
        /// </summary>
        [Category("基础信息")]
        [DisplayName("有效性")]
        [Description("当前屏幕是否有效的屏幕，包括检查屏幕的名称、尺寸、位置、旋转、锚点关联等信息是否有效；")]
        [Field(index = 10)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool valid
        {
            get
            {
                //检查屏幕名称
                if (string.IsNullOrEmpty(name)) return false;
                //检查屏幕尺寸
                if (screenSize.x <= 0 || screenSize.y <= 0 || screenSize.z <= 0) return false;
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose: return true;
                    case EScreenPoseMode.AnchorLink: return !string.IsNullOrEmpty(standardScreen) && standardScreen != name && screenAnchorLinkInfo.standardScreenAnchor != ERectAnchor.None && screenAnchorLinkInfo.screenAnchor != ERectAnchor.None;
                }
                return false;
            }
        }

        #endregion

        #region 屏幕位置

        /// <summary>
        /// 屏幕姿态
        /// </summary>
        [Browsable(false)]
        [Field(ignore = true)]
        public Pose screenPose { get; set; } = new Pose();

        /// <summary>
        /// 屏幕位置
        /// </summary>
        [Category("屏幕姿态-屏幕位置")]
        [DisplayName("位置")]
        [Description("相对屏幕组的屏幕位置；以米为单位；以米为单位；使用Unity内左手坐标系,X右Y上Z前；X轴偏移量为屏幕中心到屏幕组对象的左右水平距离差；Y轴偏移量为屏幕中心到屏幕组对象的垂直距离差；Z轴偏移量为屏幕中心到屏幕组对象的前后水平距离差；")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F position { get => screenPose.position; set => screenPose.position = value; }

        /// <summary>
        /// 地面到屏幕下边沿距离
        /// </summary>
        [Category("屏幕姿态-屏幕位置")]
        [DisplayName("地面到屏幕下边沿距离")]
        [Description("空间偏移对象到的屏幕下边沿垂直距离差,即当前屏幕位置Y轴偏移量的修正;地面0高度以上为正，地面以下为负值;当前屏幕的【屏幕姿态模式】为【屏幕姿态】时本参数有效；以米为单位；使用Unity内左手坐标系,X右Y上Z前；")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenDownToSpaceOffsetDistance
        {
            get
            {
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose:
                        {
                            if (config != null)
                            {
                                return screenPose.position.y - screenSize.y / 2 + config.screenGroupOffset.position.y;
                            }
                            break;
                        }
                }
                return 0;
            }
            set
            {
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose:
                        {
                            if (config != null)
                            {
                                screenPose._position.y = screenSize.y / 2 - config.screenGroupOffset.position.y + value;
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 屏幕组到屏幕下边沿距离
        /// </summary>
        [Category("屏幕姿态-屏幕位置")]
        [DisplayName("屏幕组到屏幕下边沿距离")]
        [Description("屏幕组对象到屏幕下边沿的垂直距离差,即当前屏幕位置Y轴偏移量的修正;当前屏幕的【屏幕姿态模式】为【屏幕姿态】时本参数有效；以米为单位；使用Unity内左手坐标系,X右Y上Z前；")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenDownToScreenGroupDistance
        {
            get
            {
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose:
                        {
                            return screenPose.position.y - screenSize.y / 2;
                        }
                }
                return 0;
            }
            set
            {
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose:
                        {
                            screenPose._position.y = screenSize.y / 2 + value;
                            break;
                        }
                }
            }
        }

        #endregion

        #region 屏幕旋转

        /// <summary>
        /// 屏幕旋转
        /// </summary>
        [Category("屏幕姿态-屏幕旋转")]
        [DisplayName("旋转")]
        [Description("相对屏幕组的屏幕旋转欧拉角度,以度为单位；使用Unity内左手坐标系,X右Y上Z前；X为屏幕中心相对屏幕组对象的X轴旋转量；Y为屏幕中心相对屏幕组对象的Y轴旋转量；Z为屏幕中心相对屏幕组对象的Z轴旋转量；")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F rotation { get => screenPose.rotation; set => screenPose.rotation = value; }

        #endregion

        #region 锚点关联

        /// <summary>
        /// 屏幕锚点关联信息
        /// </summary>
        [Category("锚点关联")]
        [DisplayName("屏幕锚点关联信息")]
        [Field(ignore = true)]
        [Browsable(false)]
        public ScreenAnchorLinkInfo screenAnchorLinkInfo { get; set; } = new ScreenAnchorLinkInfo();

        #endregion

        #region 锚点关联-标准屏幕

        /// <summary>
        /// 标准屏幕
        /// </summary>
        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕")]
        [Description("标准屏幕的名称；用于定义当前屏幕以哪个屏幕为标准进行姿态的定位；")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(CustomEnumStringConverter))]
        public string standardScreen { get => screenAnchorLinkInfo.standardScreen; set => screenAnchorLinkInfo.standardScreen = value; }

        /// <summary>
        /// 标准屏幕锚点
        /// </summary>
        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点")]
        [Description("标准屏幕的矩形平面内的特征点作为锚点")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ERectAnchor standardScreenAnchor { get => screenAnchorLinkInfo.standardScreenAnchor; set => screenAnchorLinkInfo.standardScreenAnchor = value; }

        /// <summary>
        /// 标准屏幕锚点偏移空间类型
        /// </summary>
        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点偏移空间类型")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EAnchorOffsetSpaceType standardScreenAnchorOffsetSpaceType { get => screenAnchorLinkInfo.standardScreenAnchorOffsetSpaceType; set => screenAnchorLinkInfo.standardScreenAnchorOffsetSpaceType = value; }

        /// <summary>
        /// 标准屏幕锚点偏移
        /// </summary>
        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点偏移")]
        [Description("标准屏幕锚点三轴偏移值；以米为单位；使用Unity内左手坐标系,X右Y上Z前；X为标准屏幕锚点X轴偏移值；Y为标准屏幕锚点Y轴偏移值；Z为标准屏幕锚点Z轴偏移值；")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F standardScreenAnchorOffset { get => screenAnchorLinkInfo.standardScreenAnchorOffset; set => screenAnchorLinkInfo.standardScreenAnchorOffset = value; }

        #endregion

        #region 锚点关联-当前屏幕

        /// <summary>
        /// 屏幕锚点
        /// </summary>
        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点")]
        [Description("当前屏幕的矩形平面内的特征点作为锚点")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ERectAnchor screenAnchor { get => screenAnchorLinkInfo.screenAnchor; set => screenAnchorLinkInfo.screenAnchor = value; }

        /// <summary>
        /// 屏幕锚点偏移空间类型
        /// </summary>
        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点偏移空间类型")]
        [Description("当前屏幕锚点三轴偏移值")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EAnchorOffsetSpaceType screenAnchorOffsetSpaceType { get => screenAnchorLinkInfo.screenAnchorOffsetSpaceType; set => screenAnchorLinkInfo.screenAnchorOffsetSpaceType = value; }

        /// <summary>
        /// 屏幕锚点偏移
        /// </summary>
        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点偏移")]
        [Description("当前屏幕锚点三轴偏移值；以米为单位；使用Unity内左手坐标系,X右Y上Z前；X为当前屏幕锚点X轴偏移值；Y为当前屏幕锚点Y轴偏移值；Z为当前屏幕锚点Z轴偏移值；")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F screenAnchorOffset { get => screenAnchorLinkInfo.screenAnchorOffset; set => screenAnchorLinkInfo.screenAnchorOffset = value; }

        /// <summary>
        /// 屏幕锚点旋转
        /// </summary>
        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点旋转")]
        [Description("当前屏幕锚点三轴旋转值；以度为单位；使用Unity内左手坐标系,X右Y上Z前；X为当前屏幕锚点Z轴旋转值；Y轴当前屏幕锚点Y轴旋转值；Z为当前屏幕锚点Z轴旋转值；")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F linkRotation { get => screenAnchorLinkInfo.linkRotation; set => screenAnchorLinkInfo.linkRotation = value; }

        #endregion

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public ScreenInfo Clone() => new ScreenInfo().CopyDataFrom(this);

        /// <summary>
        /// 从源拷贝数据
        /// </summary>
        /// <param name="screenInfo"></param>
        /// <returns></returns>
        public ScreenInfo CopyDataFrom(ScreenInfo screenInfo)
        {
            if (screenInfo != null)
            {
                this.SetConfig(screenInfo.config);

                this.name = screenInfo.name;
                this.displayName = screenInfo.displayName;
                this.screenSize = screenInfo.screenSize;
                this.screenPoseMode = screenInfo.screenPoseMode;
                this.screenPose = screenInfo.screenPose.Clone();
                this.screenAnchorLinkInfo = screenInfo.screenAnchorLinkInfo.Clone();
            }
            return this;
        }

        /// <summary>
        /// 获取自定义枚举字符串
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string[] GetCustomEnumStrings(ITypeDescriptorContext context)
        {
            switch (context.PropertyDescriptor.Name)
            {
                case nameof(standardScreen):
                    {
                        if (config != null)
                        {
                            return config.screens.Cast(s => s.name).ToArray();
                        }
                        break;
                    }
            }
            return Empty<string>.Array;
        }
    }

    /// <summary>
    /// 屏幕姿态模式
    /// </summary>
    [Name("屏幕姿态模式")]
    public enum EScreenPoseMode
    {
        /// <summary>
        /// 屏幕姿态
        /// </summary>
        [Name("屏幕姿态")]
        ScreenPose,

        /// <summary>
        /// 锚点关联
        /// </summary>
        [Name("锚点关联")]
        AnchorLink,
    }

    /// <summary>
    /// 场景关联锚点信息
    /// </summary>
    [Import]
    public class ScreenAnchorLinkInfo
    {
        /// <summary>
        /// 标准屏幕
        /// </summary>
        public string standardScreen { get; set; } = "";

        /// <summary>
        /// 标准屏幕锚点
        /// </summary>
        public ERectAnchor standardScreenAnchor { get; set; } = ERectAnchor.Center;

        /// <summary>
        /// 标准屏幕锚点偏移
        /// </summary>
        [Json(exportString = true)]
        public V3F standardScreenAnchorOffset { get => _standardScreenAnchorOffset; set => _standardScreenAnchorOffset = value; }

        internal V3F _standardScreenAnchorOffset = new V3F();

        /// <summary>
        /// 标准屏幕锚点偏移空间类型
        /// </summary>
        public EAnchorOffsetSpaceType standardScreenAnchorOffsetSpaceType { get; set; } = EAnchorOffsetSpaceType.Local;

        /// <summary>
        /// 屏幕锚点
        /// </summary>
        public ERectAnchor screenAnchor { get; set; } = ERectAnchor.Center;

        /// <summary>
        /// 屏幕锚点偏移
        /// </summary>
        [Json(exportString = true)]
        public V3F screenAnchorOffset { get => _screenAnchorOffset; set => _screenAnchorOffset = value; }

        internal V3F _screenAnchorOffset = new V3F();

        /// <summary>
        /// 屏幕锚点偏移空间类型
        /// </summary>
        public EAnchorOffsetSpaceType screenAnchorOffsetSpaceType { get; set; } = EAnchorOffsetSpaceType.Local;

        /// <summary>
        /// 关联旋转
        /// </summary>
        [Json(exportString = true)]
        public V3F linkRotation { get => _linkRotation; set => _linkRotation = value; }

        internal V3F _linkRotation = new V3F();

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}.{1}-->{2}-->当前.{3}", standardScreen, NameCache.Get(standardScreenAnchor), (string)linkRotation, NameCache.Get(screenAnchor));
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public ScreenAnchorLinkInfo Clone() => new ScreenAnchorLinkInfo().CopyDataFrom(this);

        /// <summary>
        /// 从源拷贝数据
        /// </summary>
        /// <param name="screenAnchorLinkInfo"></param>
        /// <returns></returns>
        public ScreenAnchorLinkInfo CopyDataFrom(ScreenAnchorLinkInfo screenAnchorLinkInfo)
        {
            if (screenAnchorLinkInfo != null)
            {
                this.standardScreen = screenAnchorLinkInfo.standardScreen;
                this.standardScreenAnchor = screenAnchorLinkInfo.standardScreenAnchor;
                this.standardScreenAnchorOffset = screenAnchorLinkInfo.standardScreenAnchorOffset;
                this.standardScreenAnchorOffsetSpaceType = screenAnchorLinkInfo.standardScreenAnchorOffsetSpaceType;

                this.screenAnchor = screenAnchorLinkInfo.screenAnchor;
                this.screenAnchorOffset = screenAnchorLinkInfo.screenAnchorOffset;
                this.screenAnchorOffsetSpaceType = screenAnchorLinkInfo.screenAnchorOffsetSpaceType;

                this.linkRotation = screenAnchorLinkInfo.linkRotation;
            }
            return this;
        }
    }

    /// <summary>
    /// 锚点偏移空间类型
    /// </summary>
    [Name("锚点偏移空间类型")]
    public enum EAnchorOffsetSpaceType
    {
        /// <summary>
        /// 世界
        /// </summary>
        [Name("世界")]
        World = 0,

        /// <summary>
        /// 本地
        /// </summary>
        [Name("本地")]
        Local,
    }

    #endregion

    #region 动作

    #region 动作配置类型

    /// <summary>
    /// 动作配置类型
    /// </summary>
    [Name("动作配置类型")]
    [ColumnHeader(120)]
    public enum EActionConfigType
    {
        /// <summary>
        /// 基础
        /// </summary>
        [Name("基础")]
        [Tip("动作的基础信息")]
        Base = 0,

        /// <summary>
        /// 跟踪器-姿态
        /// </summary>
        [Name("跟踪器-姿态")]
        [Tip("基于VRPN记录跟踪器的姿态（位置和方向）；会对跟踪器对应的目标基于本地坐标系进行相同的姿态更新；")]
        Tracker_Pose,

        /// <summary>
        /// 按钮-交互
        /// </summary>
        [Name("按钮-交互")]
        [Tip("用于配置按钮对目标做不同交互的控制操作；")]
        Button_Interact,

        /// <summary>
        /// 按钮-交互选择
        /// </summary>
        [Name("按钮-交互选择")]
        [Tip("基于VRPN记录按钮的按下与释放事件；可简单理解为对目标执行鼠标左键操作；也可理解为左操作；")]
        Button_InteractSelect,

        /// <summary>
        /// 按钮-交互激活
        /// </summary>
        [Name("按钮-交互激活")]
        [Tip("基于VRPN记录按钮的按下与释放事件；可简单理解为对目标执行鼠标右键操作；也可理解为右操作；")]
        Button_InteractActivate,

        /// <summary>
        /// 按钮-交互UI
        /// </summary>
        [Name("按钮-交互UI")]
        [Tip("基于VRPN记录按钮的按下与释放事件；可简单理解为对UI执行鼠标左键操作；")]
        Button_InteractUI,

        /// <summary>
        /// 模拟量-XYZ
        /// </summary>
        [Name("模拟量-XYZ")]
        [Tip("用于配置模拟量对XYZ各轴的控制操作；")]
        Analog_XYZ,

        /// <summary>
        /// 模拟量-负X
        /// </summary>
        [Name("模拟量-负X")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的X值做减操作；也可理解为左操作；")]
        Analog_NX,

        /// <summary>
        /// 模拟量-正X
        /// </summary>
        [Name("模拟量-正X")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的X值做加操作；也可理解为右操作；")]
        Analog_PX,

        /// <summary>
        /// 模拟量-负Y
        /// </summary>
        [Name("模拟量-负Y")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的Y值做减操作；也可理解为下操作；")]
        Analog_NY,

        /// <summary>
        /// 模拟量-正Y
        /// </summary>
        [Name("模拟量-正Y")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的Y值做加操作；也可理解为上操作；")]
        Analog_PY,

        /// <summary>
        /// 模拟量-负Z
        /// </summary>
        [Name("模拟量-负Z")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的Z值做减操作；也可理解为后操作；")]
        Analog_NZ,

        /// <summary>
        /// 模拟量-正Z
        /// </summary>
        [Name("模拟量-正Z")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的Z值做加操作；也可理解为前操作；")]
        Analog_PZ,
    }

    /// <summary>
    /// 动作配置类型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ActionConfigTypeAttribute : Attribute
    {
        /// <summary>
        /// 动作配置类型
        /// </summary>
        public EActionConfigType actionConfigType { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="actionConfigType"></param>
        public ActionConfigTypeAttribute(EActionConfigType actionConfigType)
        {
            this.actionConfigType = actionConfigType;
        }
    }

    #endregion

    /// <summary>
    /// 动作信息
    /// </summary>
    public class ActionInfo : AbstractInfo
    {
        private string To(bool value) => value ? "启用" : "";

        #region 基础信息

        /// <summary>
        /// 名称：动作名称
        /// </summary>
        [Category("基础信息")]
        [DisplayName("名称")]
        [Field(index = 0)]
        [ColumnHeader(100)]
        public string name { get; set; } = "";

        #endregion

        #region 跟踪器

        /// <summary>
        /// 姿态
        /// </summary>
        [Browsable(false)]
        [ColumnHeader(ignore = true)]
        [ActionConfigType(EActionConfigType.Tracker_Pose)]
        public PoseConfig pose { get; set; } = new PoseConfig();

        /// <summary>
        /// 姿态
        /// </summary>
        [Category("跟踪器")]
        [DisplayName("姿态")]
        [Field(index = 1)]
        [ColumnHeader(36)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        [Browsable(false)]
        public string poseEnable => To(pose.isValid);

        #endregion

        #region 按钮交互

        /// <summary>
        /// 按钮交互
        /// </summary>
        [Browsable(false)]
        [ColumnHeader(ignore = true)]
        [ActionConfigType(EActionConfigType.Button_Interact)]
        public ButtonInteractConfig buttonInteract { get; set; } = new ButtonInteractConfig();

        #endregion

        #region 按钮

        /// <summary>
        /// 交互选择：左键
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Button_InteractSelect)]
        public ButtonConfig interactSelect { get; set; } = new ButtonConfig();

        /// <summary>
        /// 交互选择
        /// </summary>
        [Category("按钮")]
        [DisplayName("交互选择")]
        [Field(index = 2)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        [Browsable(false)]
        public string interactSelectEnable => To(buttonInteract.enable && interactSelect.CanInteract());

        /// <summary>
        /// 交互激活：右键
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Button_InteractActivate)]
        public ButtonConfig interactActivate { get; set; } = new ButtonConfig();

        /// <summary>
        /// 交互激活
        /// </summary>
        [Category("按钮")]
        [DisplayName("交互激活")]
        [Field(index = 3)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        [Browsable(false)]
        public string interactActivateEnable => To(buttonInteract.enable && interactActivate.CanInteract());

        /// <summary>
        /// 交互UI
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Button_InteractUI)]
        public ButtonConfig interactUI { get; set; } = new ButtonConfig();

        /// <summary>
        /// 交互UI
        /// </summary>
        [Category("按钮")]
        [DisplayName("交互UI")]
        [Field(index = 4)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        [Browsable(false)]
        public string interactUIEnable => To(buttonInteract.enable && interactUI.CanInteract());

        #endregion

        #region 模拟量XYZ

        /// <summary>
        /// 模拟量XYZ
        /// </summary>
        [Browsable(false)]
        [ColumnHeader(ignore = true)]
        [ActionConfigType(EActionConfigType.Analog_XYZ)]
        public AnalogXYZConfig analogXYZ { get; set; } = new AnalogXYZConfig();

        #endregion

        #region 模拟量

        /// <summary>
        /// 负X
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_NX)]
        public AnalogVrpnConfig nx { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 负X
        /// </summary>
        [Category("模拟量")]
        [DisplayName("负X")]
        [Field(index = 5)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        [Browsable(false)]
        public string nxEnable => To(analogXYZ.enable && nx.CanInteract());

        /// <summary>
        /// 正X
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_PX)]
        public AnalogVrpnConfig px { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 正X
        /// </summary>
        [Category("模拟量")]
        [DisplayName("正X")]
        [Field(index = 6)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        [Browsable(false)]
        public string pxEnable => To(analogXYZ.enable && px.CanInteract());

        /// <summary>
        /// 负Y
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_NY)]
        public AnalogVrpnConfig ny { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 负Y
        /// </summary>
        [Category("模拟量")]
        [DisplayName("负Y")]
        [Field(index = 7)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        [Browsable(false)]
        public string nyEnable => To(analogXYZ.enable && ny.CanInteract());

        /// <summary>
        /// 正Y
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_PY)]
        public AnalogVrpnConfig py { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 正Y
        /// </summary>
        [Category("模拟量")]
        [DisplayName("正Y")]
        [Field(index = 8)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        [Browsable(false)]
        public string pyEnable => To(analogXYZ.enable && py.CanInteract());

        /// <summary>
        /// 负Z
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_NZ)]
        public AnalogVrpnConfig nz { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 负Z
        /// </summary>
        [Category("模拟量")]
        [DisplayName("负Z")]
        [Field(index = 9)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        [Browsable(false)]
        public string nzEnable => To(analogXYZ.enable && nz.CanInteract());

        /// <summary>
        /// 正Z
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_PZ)]
        public AnalogVrpnConfig pz { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 正Z
        /// </summary>
        [Category("模拟量")]
        [DisplayName("正Z")]
        [Field(index = 10)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        [Browsable(false)]
        public string pzEnable => To(analogXYZ.enable && pz.CanInteract());

        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public ActionInfo()
        {
            SetChildrenConfig();
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config"></param>
        public override void SetConfig(XRSpaceConfigA config)
        {
            base.SetConfig(config);
            SetChildrenConfig();
        }

        private void SetChildrenConfig()
        {
            this.pose.actionInfo = this;

            this.buttonInteract.actionInfo = this;
            this.interactSelect.actionInfo = this;
            this.interactActivate.actionInfo = this;
            this.interactUI.actionInfo = this;

            this.analogXYZ.actionInfo = this;
            this.nx.actionInfo = this;
            this.px.actionInfo = this;
            this.ny.actionInfo = this;
            this.py.actionInfo = this;
            this.nz.actionInfo = this;
            this.pz.actionInfo = this;
        }

        /// <summary>
        /// 获取动作配置
        /// </summary>
        /// <param name="actionConfigType"></param>
        /// <returns></returns>
        public object GetActionConfig(EActionConfigType actionConfigType)
        {
            if (actionConfigType == EActionConfigType.Base) return this;
            var propertyInfo = PropertyInfosCache.Get(this.GetType(), TypeHelper.InstancePublic).FirstOrDefault(pi =>
            {
                return AttributeCache<ActionConfigTypeAttribute>.Get(pi) is ActionConfigTypeAttribute attribute && attribute.actionConfigType == actionConfigType;
            });
            return propertyInfo?.GetValue(this, Empty<object>.Array) ?? this;
        }
    }

    #endregion

    #region 内置动作名

    /// <summary>
    /// 内置动作名
    /// </summary>
    public enum EActionName
    {
        /// <summary>
        /// HMD
        /// </summary>
        [Name("HMD")]
        [Tip("头盔显示器")]
        HMD,

        /// <summary>
        /// 左手
        /// </summary>
        [Name("左手")]
        LeftHand,

        /// <summary>
        /// 右手
        /// </summary>
        [Name("右手")]
        RigthHand,

        /// <summary>
        /// 空间移动
        /// </summary>
        [Name("空间移动")]
        SpaceMove,

        /// <summary>
        /// 空间旋转
        /// </summary>
        [Name("空间旋转")]
        SpaceRotate,

        /// <summary>
        /// 主菜单
        /// </summary>
        [Name("主菜单")]
        MainMenu,
    }

    #endregion

    #region 用于动作信息的基础信息

    /// <summary>
    /// 用于动作信息的基础信息
    /// </summary>
    [Import]
    public class BaseInfoForActionInfo : BaseInfo
    {
        internal ActionInfo actionInfo { get; set; }
    }

    #endregion

    #region VRPN配置

    /// <summary>
    /// VRPN配置
    /// </summary>
    [Import]
    public class VrpnConfig : BaseInfoForActionInfo
    {
        /// <summary>
        /// VRPN启用
        /// </summary>
        [Category("VRPN配置")]
        [DisplayName("VRPN启用")]
        [TypeConverter(typeof(BoolStringConverter))]
        public override bool enable { get => base.enable; set => base.enable = value; }

        /// <summary>
        /// 主机名
        /// </summary>
        [Category("VRPN配置")]
        [DisplayName("主机名")]
        [Description("VRPN服务所在的地址；如期望指明端口可使用IP:Port的形式，如127.0.0.1:3883；VRPN的默认端口3883；")]
        public string hostname { get => _hostname; set { _hostname = value; UpdateAddress(); } }

        internal string _hostname = "127.0.0.1";

        /// <summary>
        /// 对象名
        /// </summary>
        [Category("VRPN配置")]
        [DisplayName("对象名")]
        [Description("对应VRPN中不同设备设备类型（Tracker/Button/Analog/Dial/ForceDevice等）的具体对象的名称")]
        public string objectName { get => _objectName; set { _objectName = value; UpdateAddress(); } }

        internal string _objectName = "";

        /// <summary>
        /// VRPN通信使用的地址
        /// </summary>
        [Category("VRPN配置")]
        [DisplayName("地址")]
        [Description("VRPN通信使用的地址")]
        [Json(false)]
        public string address => _address;

        internal string _address = "";

        /// <summary>
        /// 更新地址
        /// </summary>
        /// <returns></returns>
        public string UpdateAddress() => _address = objectName + "@" + hostname;

        /// <summary>
        /// 通道
        /// </summary>
        [Category("VRPN配置")]
        [DisplayName("通道")]
        [Description("VRPN通信使用的通道")]
        public int channel { get; set; } = 0;

        /// <summary>
        /// 构造
        /// </summary>
        public VrpnConfig()
        {
            UpdateAddress();
        }
    }

    #endregion

    #region 姿态配置

    /// <summary>
    /// 姿态配置
    /// </summary>
    [Import]
    public class PoseConfig : VrpnConfig, ICustomEnumStringConverter
    {
        internal bool isValid => enable || UnityXRDeviceEnable || UnityInputActionEnable || ARTEnable || OptiTrackEnable || ZVREnable;

        void SetAllEnable(bool enable)
        {
            this.enable = enable;
            this.UnityXRDeviceEnable = enable;
            this.UnityInputActionEnable = enable;
            this.ARTEnable = enable;
            this.OptiTrackEnable = enable;
            this.ZVREnable = enable;
        }

        /// <summary>
        /// VRPN启用
        /// </summary>
        [Category("VRPN配置")]
        [DisplayName("VRPN启用")]
        [Description("至多允许一个姿态配置项生效，即不同姿态配置项的期望启用时互斥；")]
        [TypeConverter(typeof(BoolStringConverter))]
        public override bool enable
        {
            get => base.enable;
            set
            {
                if (value) SetAllEnable(false);
                base.enable = value;
            }
        }

        #region 源坐标系

        private ELengthUnits _lengtheUnit = ELengthUnits.M;

        /// <summary>
        /// 长度单位
        /// </summary>
        [Category("源坐标系")]
        [DisplayName("长度单位")]
        [Description("源坐标系的长度单位；即1[单位长度]=[比例尺]米；长度单位为自定义时，可自定义设置比例尺；")]
        [TypeConverter(typeof(EnumStringConverter))]
        public ELengthUnits lengtheUnit
        {
            get => _lengtheUnit;
            set
            {
                _lengtheUnit = value;
                var s = (float)_lengtheUnit.ScaleToDefault();
                _scale = new V3F(s, s, s);
            }
        }

        private V3F _scale = new V3F(1, 1, 1);

        /// <summary>
        /// 比例尺：当前坐标系的1单位转为默认标准1单位(即1米)时的值；即1[单位长度]=[比例尺]米；长度单位为自定义时，可自定义设置比例尺；
        /// </summary>
        [Category("源坐标系")]
        [DisplayName("比例尺")]
        [Description("源坐标系的1单位转为默认标准1单位(即1米)时的值；即1[单位长度]=[比例尺]米；长度单位为自定义时，可自定义设置比例尺；")]
        [Json(exportString = true)]
        public V3F scale
        {
            get => _scale;
            set
            {
                if (lengtheUnit == ELengthUnits.Custom)
                {
                    _scale = value;
                }
            }
        }

        /// <summary>
        /// X轴
        /// </summary>
        [Category("源坐标系")]
        [DisplayName("X轴")]
        [TypeConverter(typeof(EnumStringConverter))]
        public EAxisDirection xAxis { get; set; } = EAxisDirection.R;

        /// <summary>
        /// Y轴
        /// </summary>
        [Category("源坐标系")]
        [DisplayName("Y轴")]
        [TypeConverter(typeof(EnumStringConverter))]
        public EAxisDirection yAxis { get; set; } = EAxisDirection.U;

        /// <summary>
        /// Z轴
        /// </summary>
        [Category("源坐标系")]
        [DisplayName("Z轴")]
        [TypeConverter(typeof(EnumStringConverter))]
        public EAxisDirection zAxis { get; set; } = EAxisDirection.F;

        #endregion

        #region UnityXR设备

        /// <summary>
        /// 姿态UnityXR设备配置
        /// </summary>
        [Browsable(false)]
        public PoseUnityXRDeviceConfig poseUnityXRDeviceConfig { get; set; } = new PoseUnityXRDeviceConfig();

        /// <summary>
        /// UnityXR设备启用
        /// </summary>
        [Category("UnityXR设备配置")]
        [DisplayName("UnityXR设备启用")]
        [Description("基于UnityXR设备机制进行功能实现；至多允许一个姿态配置项生效，即不同姿态配置项的期望启用时互斥；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool UnityXRDeviceEnable
        {
            get => poseUnityXRDeviceConfig.enable;
            set
            {
                if (value) SetAllEnable(false);
                poseUnityXRDeviceConfig.enable = value;
            }
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        [Category("UnityXR设备配置")]
        [DisplayName("设备类型")]
        [Description("基于UnityXR设备机制进行功能实现；至多允许一个姿态配置项生效，即不同姿态配置项的期望启用时互斥；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EXRDeviceType deviceType { get => poseUnityXRDeviceConfig.deviceType; set => poseUnityXRDeviceConfig.deviceType = value; }

        #endregion

        #region Unity输入动作

        /// <summary>
        /// 姿态Unity输入动作配置
        /// </summary>
        [Browsable(false)]
        public PoseUnityInputActionConfig poseUnityInputActionConfig { get; set; } = new PoseUnityInputActionConfig();

        /// <summary>
        /// Unity输入动作启用
        /// </summary>
        [Category("Unity输入动作配置")]
        [DisplayName("Unity输入动作启用")]
        [Description("基于Unity新版输入系统的输入动作机制进行功能实现；至多允许一个姿态配置项生效，即不同姿态配置项的期望启用时互斥；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool UnityInputActionEnable
        {
            get => poseUnityInputActionConfig.enable;
            set
            {
                if (value) SetAllEnable(false);
                poseUnityInputActionConfig.enable = value;
            }
        }

        /// <summary>
        /// 位置动作名
        /// </summary>
        [Category("Unity输入动作配置")]
        [DisplayName("位置动作名")]
        [Description("基于Unity新版输入系统的输入动作机制进行功能实现；至多允许一个姿态配置项生效，即不同姿态配置项的期望启用时互斥；")]
        [Json(false)]
        [TypeConverter(typeof(CustomEnumStringConverter))]
        [AllowNotInCustomEnumStrings]
        public string positionActionName { get => poseUnityInputActionConfig.positionActionName; set => poseUnityInputActionConfig.positionActionName = value; }

        /// <summary>
        /// 旋转动作名
        /// </summary>
        [Category("Unity输入动作配置")]
        [DisplayName("旋转动作名")]
        [Description("基于Unity新版输入系统的输入动作机制进行功能实现；至多允许一个姿态配置项生效，即不同姿态配置项的期望启用时互斥；")]
        [Json(false)]
        [TypeConverter(typeof(CustomEnumStringConverter))]
        [AllowNotInCustomEnumStrings]
        public string rotationActionName { get => poseUnityInputActionConfig.rotationActionName; set => poseUnityInputActionConfig.rotationActionName = value; }

        /// <summary>
        /// 跟踪状态动作名
        /// </summary>
        [Category("Unity输入动作配置")]
        [DisplayName("跟踪状态动作名")]
        [Description("基于Unity新版输入系统的输入动作机制进行功能实现；至多允许一个姿态配置项生效，即不同姿态配置项的期望启用时互斥；")]
        [Json(false)]
        [TypeConverter(typeof(CustomEnumStringConverter))]
        [AllowNotInCustomEnumStrings]
        public string trackingStateActionName { get => poseUnityInputActionConfig.trackingStateActionName; set => poseUnityInputActionConfig.trackingStateActionName = value; }

        /// <summary>
        /// 获取自定义枚举字符串
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string[] GetCustomEnumStrings(ITypeDescriptorContext context)
        {
            switch (context.PropertyDescriptor.Name)
            {
                case nameof(positionActionName):
                case nameof(rotationActionName):
                case nameof(trackingStateActionName):
                    {
                        return XRHelper.builtinActionNames;
                    }
            }
            return Empty<string>.Array;
        }

        #endregion

        #region


        #endregion

        #region ART

        /// <summary>
        /// 姿态ART配置
        /// </summary>
        [Browsable(false)]
        public PoseARTConfig poseARTConfig { get; set; } = new PoseARTConfig();

        /// <summary>
        /// ART启用
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("ART启用")]
        [Description("ART直连在对应控制的计算机设备上时，ART配置才可生效；至多允许一个姿态配置项生效，即不同姿态配置项的期望启用时互斥；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool ARTEnable
        {
            get => poseARTConfig.enable;
            set
            {
                if (value) SetAllEnable(false);
                poseARTConfig.enable = value;
            }
        }

        /// <summary>
        /// ART服务器主机
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("服务器主机")]
        [Description("ART的服务器主机地址信息；")]
        [Json(false)]
        public string ARTServerHost { get => poseARTConfig.clientConfig.serverHost; set => poseARTConfig.clientConfig.serverHost = value; }

        /// <summary>
        /// ART服务器端口
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("服务器端口")]
        [Description("使用ART时，如仅接收数据本值为0即可；如使用DTrack2时，本端口值默认值为50105；")]
        [Json(false)]
        public int ARTServerPort { get => poseARTConfig.clientConfig.serverPort; set => poseARTConfig.clientConfig.serverPort = value; }

        /// <summary>
        /// ART数据端口
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("数据端口")]
        [Description("使用ART时，处理数据的端口；")]
        [Json(false)]
        public int ARTDataPort { get => poseARTConfig.clientConfig.dataPort; set => poseARTConfig.clientConfig.dataPort = value; }

        /// <summary>
        /// ART远程类型
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("远程类型")]
        [Description("使用ART时，远程服务器的类型；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ERemoteType ARTRemoteType { get => poseARTConfig.clientConfig.remoteType; set => poseARTConfig.clientConfig.remoteType = value; }

        /// <summary>
        /// 使用源坐标系
        /// </summary>
        [Category("直连ART配置-刚体")]
        [DisplayName("使用源坐标系")]
        [Description("使用ART时，是否将源坐标系信息同步给ART流客户端；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool useSrcSC { get => poseARTConfig.useSrcSC; set => poseARTConfig.useSrcSC = value; }

        /// <summary>
        /// ART数据类型
        /// </summary>
        [Category("直连ART配置-刚体")]
        [DisplayName("数据类型")]
        [Description("使用ART时，刚体对象的数据类型；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EDataType dataType { get => poseARTConfig.dataType; set => poseARTConfig.dataType = value; }

        /// <summary>
        /// ART刚体ID
        /// </summary>
        [Category("直连ART配置-刚体")]
        [DisplayName("刚体ID")]
        [Description("使用ART时，用于与ART软件进行数据流通信的刚体ID；")]
        [Json(false)]
        public int rigidBodyIDART { get => poseARTConfig.rigidBodyID; set => poseARTConfig.rigidBodyID = value; }

        #endregion

        #region OptiTrack

        /// <summary>
        /// 姿态OptiTrack配置
        /// </summary>
        [Browsable(false)]
        public PoseOptiTrackConfig poseOptiTrackConfig { get; set; } = new PoseOptiTrackConfig();

        /// <summary>
        /// OptiTrack启用
        /// </summary>
        [Category("直连OptiTrack配置")]
        [DisplayName("OptiTrack启用")]
        [Description("OptiTrack直连在对应控制的计算机设备上时，OptiTrack配置才可生效；至多允许一个姿态配置项生效，即不同姿态配置项的期望启用时互斥；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool OptiTrackEnable
        {
            get => poseOptiTrackConfig.enable;
            set
            {
                if (value) SetAllEnable(false);
                poseOptiTrackConfig.enable = value;
            }
        }

        /// <summary>
        /// 服务器地址
        /// </summary>
        [Category("直连OptiTrack配置")]
        [DisplayName("服务器地址")]
        [Description("OptiTrack的服务器地址信息；")]
        [Json(false)]
        public string serverAddress { get => poseOptiTrackConfig.clientConfig.serverAddress; set => poseOptiTrackConfig.clientConfig.serverAddress = value; }

        /// <summary>
        /// 本地地址
        /// </summary>
        [Category("直连OptiTrack配置")]
        [DisplayName("本地地址")]
        [Description("OptiTrack的本地地址信息；")]
        [Json(false)]
        public string localAddress { get => poseOptiTrackConfig.clientConfig.localAddress; set => poseOptiTrackConfig.clientConfig.localAddress = value; }

        /// <summary>
        /// 连接类型
        /// </summary>
        [Category("直连OptiTrack配置")]
        [DisplayName("连接类型")]
        [Description("OptiTrack的连接类型信息；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EOptiTrackClientConnectionType connectionType { get => poseOptiTrackConfig.clientConfig.connectionType; set => poseOptiTrackConfig.clientConfig.connectionType = value; }

        /// <summary>
        /// 骨架坐标
        /// </summary>
        [Category("直连OptiTrack配置")]
        [DisplayName("骨架坐标")]
        [Description("OptiTrack的骨架坐标信息；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EOptiTrackStreamingCoordinatesValues skeletonCoordinates { get => poseOptiTrackConfig.clientConfig.skeletonCoordinates; set => poseOptiTrackConfig.clientConfig.skeletonCoordinates = value; }

        /// <summary>
        /// 骨骼命名约定
        /// </summary>
        [Category("直连OptiTrack配置")]
        [DisplayName("骨骼命名约定")]
        [Description("OptiTrack的骨骼命名约定信息；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EOptiTrackOptitrackBoneNameConvention boneNamingConvention { get => poseOptiTrackConfig.clientConfig.boneNamingConvention; set => poseOptiTrackConfig.clientConfig.boneNamingConvention = value; }

        /// <summary>
        /// OptiTrack刚体ID
        /// </summary>
        [Category("直连OptiTrack配置-刚体")]
        [DisplayName("刚体ID")]
        [Description("使用OptiTrack时，用于与OptiTrack-Motive软件进行数据流通信的刚体ID；")]
        [Json(false)]
        public int rigidBodyIDOptiTrack { get => poseOptiTrackConfig.rigidBodyID; set => poseOptiTrackConfig.rigidBodyID = value; }

        #endregion

        #region ZVR

        /// <summary>
        /// 姿态ZVR配置
        /// </summary>
        [Browsable(false)]
        public PoseZVRConfig poseZVRConfig { get; set; } = new PoseZVRConfig();

        /// <summary>
        /// ZVR启用
        /// </summary>
        [Category("直连ZVR配置")]
        [DisplayName("ZVR启用")]
        [Description("ZVR直连在对应控制的计算机设备上时，ZVR配置才可生效；至多允许一个姿态配置项生效，即不同姿态配置项的期望启用时互斥；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool ZVREnable
        {
            get => poseZVRConfig.enable;
            set
            {
                if (value) SetAllEnable(false);
                poseZVRConfig.enable = value;
            }
        }

        /// <summary>
        /// ZVR服务器地址
        /// </summary>
        [Category("直连ZVR配置")]
        [DisplayName("服务器地址")]
        [Description("ZVR的服务器地址信息；")]
        [Json(false)]
        public string serverAddressZVR { get => poseZVRConfig.clientConfig.serverAddress; set => poseZVRConfig.clientConfig.serverAddress = value; }

        /// <summary>
        /// ZVR本地地址
        /// </summary>
        [Category("直连ZVR配置")]
        [DisplayName("本地地址")]
        [Description("ZVR的本地地址信息；")]
        [Json(false)]
        public string localAddressZVR { get => poseZVRConfig.clientConfig.localAddress; set => poseZVRConfig.clientConfig.localAddress = value; }

        /// <summary>
        /// ZVR连接类型
        /// </summary>
        [Category("直连ZVR配置")]
        [DisplayName("连接类型")]
        [Description("ZVR的连接类型信息；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EZVRClientConnectionType connectionTypeZVR { get => poseZVRConfig.clientConfig.connectionType; set => poseZVRConfig.clientConfig.connectionType = value; }

        /// <summary>
        /// ZVR服务器命令端口
        /// </summary>
        [Category("直连ZVR配置")]
        [DisplayName("服务器命令端口")]
        [Description("ZVR的服务器命令端口信息；")]
        [Json(false)]
        public ushort serverCommandPort { get => poseZVRConfig.clientConfig.serverCommandPort; set => poseZVRConfig.clientConfig.serverCommandPort = value; }

        /// <summary>
        /// ZVR服务器数据端口
        /// </summary>
        [Category("直连ZVR配置")]
        [DisplayName("服务器数据端口")]
        [Description("ZVR的服务器数据端口信息；")]
        [Json(false)]
        public ushort serverDataPort { get => poseZVRConfig.clientConfig.serverDataPort; set => poseZVRConfig.clientConfig.serverDataPort = value; }

        /// <summary>
        /// ZVR刚体ID
        /// </summary>
        [Category("直连ZVR配置-刚体")]
        [DisplayName("刚体ID")]
        [Description("使用ZVR时，用于与ZVR-ActiveCenter软件进行数据流通信的刚体ID；")]
        [Json(false)]
        public int rigidBodyIDZVR { get => poseZVRConfig.rigidBodyID; set => poseZVRConfig.rigidBodyID = value; }

        #endregion
    }

    #endregion

    #region 姿态UnityXR设备配置

    /// <summary>
    /// 姿态UnityXR设备配置
    /// </summary>
    [Import]
    public class PoseUnityXRDeviceConfig : BaseInfoForActionInfo
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public EXRDeviceType deviceType { get; set; } = EXRDeviceType.None;
    }

    #endregion

    #region 按钮UnityXR设备配置

    /// <summary>
    /// 按钮UnityXR设备配置
    /// </summary>
    [Import]
    public class ButtonUnityXRDeviceConfig : BaseInfoForActionInfo
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public EXRDeviceType deviceType { get; set; } = EXRDeviceType.None;

        /// <summary>
        /// 按钮
        /// </summary>
        public EXRButton button { get; set; } = EXRButton.None;
    }

    #endregion

    #region 姿态Unity输入动作配置

    /// <summary>
    /// 姿态Unity输入动作配置
    /// </summary>
    [Import]
    public class PoseUnityInputActionConfig : BaseInfoForActionInfo
    {
        /// <summary>
        /// 位置动作名
        /// </summary>
        public string positionActionName { get; set; } = "";

        /// <summary>
        /// 旋转动作名
        /// </summary>
        public string rotationActionName { get; set; } = "";

        /// <summary>
        /// 跟踪状态动作名
        /// </summary>
        public string trackingStateActionName { get; set; } = "";
    }

    #endregion

    #region 按钮Unity输入动作配置

    /// <summary>
    /// 按钮Unity输入动作配置
    /// </summary>
    [Import]
    public class ButtonUnityInputActionConfig : BaseInfoForActionInfo
    {
        /// <summary>
        /// 动作名
        /// </summary>
        public string actionName { get; set; } = "";
    }

    #endregion

    #region 姿态ART配置

    /// <summary>
    /// 姿态ART配置
    /// </summary>
    [Import]
    public class PoseARTConfig : BaseInfoForActionInfo
    {
        /// <summary>
        /// 客户端配置
        /// </summary>
        public ARTClientConfig clientConfig { get; set; } = new ARTClientConfig();

        /// <summary>
        /// 使用源坐标系
        /// </summary>
        public bool useSrcSC { get; set; } = false;

        /// <summary>
        /// 数据类型
        /// </summary>
        public EDataType dataType { get; set; } = EDataType.Body;

        /// <summary>
        /// 刚体ID
        /// </summary>
        public int rigidBodyID { get; set; } = 0;
    }

    #endregion

    #region 按钮ART配置

    /// <summary>
    /// 按钮ART配置
    /// </summary>
    [Import]
    public class ButtonARTConfig : BaseInfoForActionInfo
    {
        /// <summary>
        /// Flystick按钮
        /// </summary>
        public FlystickButton flystickButton { get; set; } = new FlystickButton();
    }

    #endregion

    #region OptiTrack客户端配置

    /// <summary>
    /// OptiTrack客户端连接类型
    /// </summary>
    public enum EOptiTrackClientConnectionType
    {
        /// <summary>
        /// 多播
        /// </summary>
        Multicast,

        /// <summary>
        /// 单播
        /// </summary>
        Unicast
    }

    /// <summary>
    /// OptiTrack流客户端坐标系值
    /// </summary>
    public enum EOptiTrackStreamingCoordinatesValues
    {
        /// <summary>
        /// 本地
        /// </summary>
        Local,

        /// <summary>
        /// 世界
        /// </summary>
        Global
    }

    /// <summary>
    /// OptiTrack跟踪骨骼命名约束
    /// </summary>
    public enum EOptiTrackOptitrackBoneNameConvention
    {
        /// <summary>
        /// Motive
        /// </summary>
        Motive,

        /// <summary>
        /// FBX
        /// </summary>
        FBX,

        /// <summary>
        /// BVH
        /// </summary>
        BVH,
    }

    /// <summary>
    /// OptiTrack客户端配置
    /// </summary>
    public class OptiTrackClientConfig
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string serverAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// 本地地址
        /// </summary>
        public string localAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// 连接类型
        /// </summary>
        public EOptiTrackClientConnectionType connectionType { get; set; } = EOptiTrackClientConnectionType.Multicast;

        /// <summary>
        /// 骨架坐标
        /// </summary>
        public EOptiTrackStreamingCoordinatesValues skeletonCoordinates { get; set; } = EOptiTrackStreamingCoordinatesValues.Local;

        /// <summary>
        /// 骨骼命名约定
        /// </summary>
        public EOptiTrackOptitrackBoneNameConvention boneNamingConvention { get; set; } = EOptiTrackOptitrackBoneNameConvention.Motive;
    }

    #endregion

    #region 姿态OptiTrack配置

    /// <summary>
    /// 姿态OptiTrack配置
    /// </summary>
    [Import]
    public class PoseOptiTrackConfig : BaseInfoForActionInfo
    {
        /// <summary>
        /// 客户端配置
        /// </summary>
        public OptiTrackClientConfig clientConfig { get; set; } = new OptiTrackClientConfig();

        /// <summary>
        /// 刚体ID
        /// </summary>
        public int rigidBodyID { get; set; } = 0;
    }

    #endregion

    #region ZVR客户端配置

    /// <summary>
    /// ZVR客户端连接类型
    /// </summary>
    public enum EZVRClientConnectionType
    {
        /// <summary>
        /// 多播
        /// </summary>
        Multicast,

        /// <summary>
        /// 广播
        /// </summary>
        Boardcast,
    }

    /// <summary>
    /// ZVR客户端配置
    /// </summary>
    public class ZVRClientConfig
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string serverAddress { get; set; } = "239.8.192.168";

        /// <summary>
        /// 本地地址
        /// </summary>
        public string localAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// 连接类型
        /// </summary>
        public EZVRClientConnectionType connectionType { get; set; } = EZVRClientConnectionType.Multicast;

        /// <summary>
        /// 服务器命令端口
        /// </summary>
        public ushort serverCommandPort { get; set; } = 15515;

        /// <summary>
        /// 服务器数据端口
        /// </summary>
        public ushort serverDataPort { get; set; } = 15516;
    }

    #endregion

    #region 姿态ZVR配置

    /// <summary>
    /// 姿态ZVR配置
    /// </summary>
    [Import]
    public class PoseZVRConfig : BaseInfoForActionInfo
    {
        /// <summary>
        /// 客户端配置
        /// </summary>
        public ZVRClientConfig clientConfig { get; set; } = new ZVRClientConfig();

        /// <summary>
        /// 刚体ID
        /// </summary>
        public int rigidBodyID { get; set; } = 0;
    }

    #endregion

    #region 按钮配置

    /// <summary>
    /// 按钮配置
    /// </summary>
    [Import]
    public class ButtonConfig : VrpnConfig, ICustomEnumStringConverter
    {
        /// <summary>
        /// 构造
        /// </summary>
        public ButtonConfig()
        {
            xboxConfig.deadZone = new V2F(0.5f, 1f);
        }

        /// <summary>
        /// 能交互
        /// </summary>
        /// <returns></returns>
        public bool CanInteract() => enable || xboxEnable || ARTEnable || UnityInputActionEnable || UnityXRDeviceEnable;

        #region XBox

        /// <summary>
        /// XBox配置
        /// </summary>
        [Browsable(false)]
        public XBoxConfig xboxConfig { get; set; } = new XBoxConfig();

        /// <summary>
        /// XBox启用
        /// </summary>
        [Category("直连XBox配置")]
        [DisplayName("XBox启用")]
        [Description("XBox直连在对应控制的计算机设备上时，XBox配置才可生效；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool xboxEnable { get => xboxConfig.enable; set => xboxConfig.enable = value; }

        /// <summary>
        /// XBox轴与按钮
        /// </summary>
        [Category("直连XBox配置")]
        [DisplayName("轴与按钮")]
        [Description("XBox直连在对应控制的计算机设备上时，XBox配置才可生效；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EXBoxAxisAndButton axisAndButton_XBox { get => xboxConfig.axisAndButton; set => xboxConfig.axisAndButton = value; }

        /// <summary>
        /// XBox死区小值
        /// </summary>
        [Category("直连XBox配置")]
        [DisplayName("死区小值")]
        [Description("XBox的值小于本值时，则认为0值；在死区区间内时，如果做按钮则为1，如果做模拟量，则做0到1的线性补间值；XBox直连在对应控制的计算机设备上时，此配置才可生效；")]
        [Json(false)]
        public float deadZoneMinValue_XBox { get => xboxConfig.deadZone.x; set => xboxConfig._deadZone.x = value; }

        /// <summary>
        /// XBox死区大值
        /// </summary>
        [Category("直连XBox配置")]
        [DisplayName("死区大值")]
        [Description("XBox的值大于本值时，则认为1值；在死区区间内时，如果按钮则为1，如果模拟量则做0到1的线性补间值；XBox直连在对应控制的计算机设备上时，此配置才可生效；")]
        [Json(false)]
        public float deadZoneMaxValue_XBox { get => xboxConfig.deadZone.y; set => xboxConfig._deadZone.y = value; }

        #endregion

        #region ART

        /// <summary>
        /// 按钮ART配置
        /// </summary>
        [Browsable(false)]
        public ButtonARTConfig buttonARTConfig { get; set; } = new ButtonARTConfig();

        /// <summary>
        /// ART启用
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("ART启用")]
        [Description("ART直连在对应控制的计算机设备上时，ART配置才可生效；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool ARTEnable { get => buttonARTConfig.enable; set => buttonARTConfig.enable = value; }

        /// <summary>
        /// Flystick编号
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("Flystick编号")]
        [Description("使用ART-Flystick手柄时，从0开始的Flystick有效编号；如果值为-1标识任意Flystick输入设备；")]
        [Json(false)]
        public int flysitckID { get => buttonARTConfig.flystickButton.flysitckID; set => buttonARTConfig.flystickButton.flysitckID = value; }

        /// <summary>
        /// Flystick手柄
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("Flystick手柄")]
        [Description("使用ART-Flystick手柄时，Flystick手柄版本；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EFlystick flystick { get => buttonARTConfig.flystickButton.flystick; set => buttonARTConfig.flystickButton.flystick = value; }
       
        /// <summary>
        /// Flystick1按钮
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("Flystick1按钮")]
        [Description("使用ART-Flystick手柄时，Flystick1手柄上的按钮；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EFlystick1Switchs flystick1Switchs { get => buttonARTConfig.flystickButton.flystick1Switchs; set => buttonARTConfig.flystickButton.flystick1Switchs = value; }

        /// <summary>
        /// Flystick2按钮
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("Flystick2按钮")]
        [Description("使用ART-Flystick手柄时，Flystick2手柄上的按钮；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EFlystick2Switchs flystick2Switchs { get => buttonARTConfig.flystickButton.flystick2Switchs; set => buttonARTConfig.flystickButton.flystick2Switchs = value; }

        /// <summary>
        /// Flystick3按钮
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("Flystick3按钮")]
        [Description("使用ART-Flystick手柄时，Flystick3手柄上的按钮；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EFlystick3Switchs flystick3Switchs { get => buttonARTConfig.flystickButton.flystick3Switchs; set => buttonARTConfig.flystickButton.flystick3Switchs = value; }

        /// <summary>
        /// Flystick死区小值
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("死区小值")]
        [Description("Flystick的值小于本值时，则认为0值；在死区区间内时，如果按钮则为1且认为事件成立，如果做模拟量则做0到1的线性补间值；Flystick直连在对应控制的计算机设备上时，此配置才可生效；")]
        [Json(false)]
        public float deadZoneMinValue_Flystick { get => buttonARTConfig.flystickButton.deadZone.x; set => buttonARTConfig.flystickButton._deadZone.x = value; }

        /// <summary>
        /// Flystick死区大值
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("死区大值")]
        [Description("Flystick的值大于本值时，则认为1值；在死区区间内时，如果按钮则为1且认为事件成立，如果做模拟量则做0到1的线性补间值；Flystick直连在对应控制的计算机设备上时，此配置才可生效；")]
        [Json(false)]
        public float deadZoneMaxValue_Flystick { get => buttonARTConfig.flystickButton.deadZone.y; set => buttonARTConfig.flystickButton._deadZone.y = value; }

        #endregion

        #region UnityXR设备

        /// <summary>
        /// UnityXR设备配置
        /// </summary>
        [Browsable(false)]
        public ButtonUnityXRDeviceConfig buttonUnityXRDeviceConfig { get; set; } = new ButtonUnityXRDeviceConfig();

        /// <summary>
        /// UnityXR设备启用
        /// </summary>
        [Category("UnityXR设备配置")]
        [DisplayName("UnityXR设备启用")]
        [Description("基于UnityXR设备机制进行功能实现；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool UnityXRDeviceEnable { get => buttonUnityXRDeviceConfig.enable; set => buttonUnityXRDeviceConfig.enable = value; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [Category("UnityXR设备配置")]
        [DisplayName("设备类型")]
        [Description("基于UnityXR设备机制进行功能实现；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EXRDeviceType deviceType { get => buttonUnityXRDeviceConfig.deviceType; set => buttonUnityXRDeviceConfig.deviceType = value; }

        /// <summary>
        /// 按钮
        /// </summary>
        [Category("UnityXR设备配置")]
        [DisplayName("按钮")]
        [Description("基于UnityXR设备机制进行功能实现；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EXRButton button { get => buttonUnityXRDeviceConfig.button; set => buttonUnityXRDeviceConfig.button = value; }

        #endregion

        #region Unity输入动作

        /// <summary>
        /// 按钮Unity输入动作配置
        /// </summary>
        [Browsable(false)]
        public ButtonUnityInputActionConfig buttonUnityInputActionConfig { get; set; } = new ButtonUnityInputActionConfig();

        /// <summary>
        /// Unity输入动作启用
        /// </summary>
        [Category("Unity输入动作配置")]
        [DisplayName("Unity输入动作启用")]
        [Description("基于Unity新版输入系统的输入动作机制进行功能实现；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool UnityInputActionEnable { get => buttonUnityInputActionConfig.enable; set => buttonUnityInputActionConfig.enable = value; }

        /// <summary>
        /// 动作名
        /// </summary>
        [Category("Unity输入动作配置")]
        [DisplayName("动作名")]
        [Description("基于Unity新版输入系统的输入动作机制进行功能实现；")]
        [Json(false)]
        [TypeConverter(typeof(CustomEnumStringConverter))]
        [AllowNotInCustomEnumStrings]
        public string actionName { get => buttonUnityInputActionConfig.actionName; set => buttonUnityInputActionConfig.actionName = value; }

        /// <summary>
        /// 获取自定义枚举字符串
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string[] GetCustomEnumStrings(ITypeDescriptorContext context)
        {
            switch (context.PropertyDescriptor.Name)
            {
                case nameof(actionName):
                    {
                        return XRHelper.builtinActionNames;
                    }
            }
            return Empty<string>.Array;
        }

        #endregion
    }

    #endregion

    #region 模拟量VRPN配置

    /// <summary>
    /// 模拟量VRPN配置
    /// </summary>
    [Import]
    public class AnalogVrpnConfig : ButtonConfig
    {
        /// <summary>
        /// 构造
        /// </summary>
        public AnalogVrpnConfig()
        {
            xboxConfig.deadZone = new V2F(0.1f, 0.9f);
        }

        #region VRPN配置-模拟量

        /// <summary>
        /// 模拟量配置
        /// </summary>
        [Browsable(false)]
        public AnalogConfig analogConfig { get; set; } = new AnalogConfig();

        /// <summary>
        /// 源最小值
        /// </summary>
        [Category("VRPN配置-模拟量")]
        [DisplayName("源最小值")]
        [Description("当前VRPN可以提供的模拟量的最小值")]
        [Json(false)]
        public double srcMinValue { get => analogConfig._srcValue.x; set => analogConfig._srcValue.x = value; }

        /// <summary>
        /// 源最大值
        /// </summary>
        [Category("VRPN配置-模拟量")]
        [DisplayName("源最大值")]
        [Description("当前VRPN可以提供的模拟量的最大值")]
        [Json(false)]
        public double srcMaxValue { get => analogConfig._srcValue.y; set => analogConfig._srcValue.y = value; }

        /// <summary>
        /// 死区小值
        /// </summary>
        [Category("VRPN配置-模拟量")]
        [DisplayName("死区小值")]
        [Description("源模拟量的值小于本值时，则处理为目标小值；在死区区间内时，对源模拟量的值做目标小值到目标大值的线性补间处理；")]
        [Json(false)]
        public double deadZoneMinValue { get => analogConfig._deadZone.x; set => analogConfig._deadZone.x = value; }

        /// <summary>
        /// 死区大值
        /// </summary>
        [Category("VRPN配置-模拟量")]
        [DisplayName("死区大值")]
        [Description("源模拟量的值大于本值时，则处理为目标大值；在死区区间内时，对源模拟量的值做目标小值到目标大值的线性补间处理；")]
        [Json(false)]
        public double deadZoneMaxValue { get => analogConfig._deadZone.y; set => analogConfig._deadZone.y = value; }

        /// <summary>
        /// 目标小值
        /// </summary>
        [Category("VRPN配置-模拟量")]
        [DisplayName("目标小值")]
        [Description("对源模拟量的值经过死区线性补间处理时，为后续处理提供的小值；对应死区小值；正常情况下，目标小值与目标大值其中一个值为0,另一个非0；")]
        [Json(false)]
        public double dstMinValue { get => analogConfig._dstValue.x; set => analogConfig._dstValue.x = value; }

        /// <summary>
        /// 目标大值
        /// </summary>
        [Category("VRPN配置-模拟量")]
        [DisplayName("目标大值")]
        [Description("对源模拟量的值经过死区线性补间处理时，为后续处理提供的大值；对应死区大值；正常情况下，目标小值与目标大值其中一个值为0,另一个非0；")]
        [Json(false)]
        public double dstMaxValue { get => analogConfig._dstValue.y; set => analogConfig._dstValue.y = value; }

        /// <summary>
        /// 获取目标值
        /// </summary>
        /// <param name="srcValue"></param>
        /// <returns></returns>
        public double GetDstValue(double srcValue) => srcValue.ToDstValue(deadZoneMinValue, deadZoneMaxValue, dstMinValue, dstMaxValue);

        #endregion

        #region XBOX

        /// <summary>
        /// XBOX目标值
        /// </summary>
        [Json(exportString = true)]
        [Browsable(false)]
        public V2F xboxDstValue { get => _xboxDstValue; set => _xboxDstValue = value; }

        internal V2F _xboxDstValue = new V2F(0, 1);

        /// <summary>
        /// 目标小值
        /// </summary>
        [Category("直连XBox配置")]
        [DisplayName("目标小值")]
        [Description("对源模拟量的值经过死区线性补间处理时，为后续处理提供的小值；对应死区小值；正常情况下，目标小值与目标大值其中一个值为0,另一个非0；")]
        [Json(false)]
        public float xboxDstMinValue { get => _xboxDstValue.x; set => _xboxDstValue.x = value; }

        /// <summary>
        /// 目标大值
        /// </summary>
        [Category("直连XBox配置")]
        [DisplayName("目标大值")]
        [Description("对源模拟量的值经过死区线性补间处理时，为后续处理提供的大值；对应死区大值；正常情况下，目标小值与目标大值其中一个值为0,另一个非0；")]
        [Json(false)]
        public float xboxDstMaxValue { get => _xboxDstValue.y; set => _xboxDstValue.y = value; }

        #endregion

        #region ART

        /// <summary>
        /// ART目标值
        /// </summary>
        [Json(exportString = true)]
        [Browsable(false)]
        public V2F artDstValue { get => _artDstValue; set => _artDstValue = value; }

        internal V2F _artDstValue = new V2F(0, 1);

        /// <summary>
        /// 目标小值
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("目标小值")]
        [Description("对源模拟量的值经过死区线性补间处理时，为后续处理提供的小值；对应死区小值；正常情况下，目标小值与目标大值其中一个值为0,另一个非0；")]
        [Json(false)]
        public float artDstMinValue { get => _artDstValue.x; set => _artDstValue.x = value; }

        /// <summary>
        /// 目标大值
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("目标大值")]
        [Description("对源模拟量的值经过死区线性补间处理时，为后续处理提供的大值；对应死区大值；正常情况下，目标小值与目标大值其中一个值为0,另一个非0；")]
        [Json(false)]
        public float artDstMaxValue { get => _artDstValue.y; set => _artDstValue.y = value; }

        #endregion
    }

    #endregion

    #region 模拟量配置

    /// <summary>
    /// 模拟量配置
    /// </summary>
    [Import]
    public class AnalogConfig
    {
        /// <summary>
        /// 源值
        /// </summary>
        [Json(exportString = true)]
        public V2D srcValue { get => _srcValue; set => _srcValue = value; }

        internal V2D _srcValue = new V2D(0, 1);

        /// <summary>
        /// 死区
        /// </summary>
        [Json(exportString = true)]
        public V2D deadZone { get => _deadZone; set => _deadZone = value; }

        internal V2D _deadZone = new V2D(0, 1);

        /// <summary>
        /// 目标值
        /// </summary>
        [Json(exportString = true)]
        public V2D dstValue { get => _dstValue; set => _dstValue = value; }

        internal V2D _dstValue = new V2D(0, 1);
    }

    #endregion

    #region 按钮交互配置

    /// <summary>
    /// 按钮交互配置
    /// </summary>
    [Import]
    public class ButtonInteractConfig : BaseInfoForActionInfo
    {
        #region ART

        /// <summary>
        /// ART客户端配置
        /// </summary>
        [Browsable(false)]
        public ARTClientConfig artClientConfig { get; set; } = new ARTClientConfig();

        /// <summary>
        /// ART服务器主机
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("服务器主机")]
        [Description("ART的服务器主机地址信息；")]
        [Json(false)]
        public string ARTServerHost { get => artClientConfig.serverHost; set => artClientConfig.serverHost = value; }

        /// <summary>
        /// ART服务器端口
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("服务器端口")]
        [Description("使用ART时，如仅接收数据本值为0即可；如使用DTrack2时，本端口值默认值为50105；")]
        [Json(false)]
        public int ARTServerPort { get => artClientConfig.serverPort; set => artClientConfig.serverPort = value; }

        /// <summary>
        /// ART数据端口
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("数据端口")]
        [Description("使用ART时，处理数据的端口；")]
        [Json(false)]
        public int ARTDataPort { get => artClientConfig.dataPort; set => artClientConfig.dataPort = value; }

        /// <summary>
        /// ART远程类型
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("远程类型")]
        [Description("使用ART时，远程服务器的类型；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ERemoteType ARTRemoteType { get => artClientConfig.remoteType; set => artClientConfig.remoteType = value; }

        #endregion
    }

    #endregion

    #region 模拟量XYZ配置

    /// <summary>
    /// 模拟量XYZ配置
    /// </summary>
    [Import]
    public class AnalogXYZConfig : BaseInfoForActionInfo
    {
        /// <summary>
        /// 速度
        /// </summary>
        [Category("基础信息")]
        [DisplayName("速度")]
        [ColumnHeader(ignore = true)]
        [Json(exportString = true)]
        public V3F speed { get => _speed; set => _speed = value; }

        internal V3F _speed = new V3F(1, 1, 1);

        /// <summary>
        /// 变换TRS
        /// </summary>
        [Category("基础信息")]
        [DisplayName("变换TRS")]
        [TypeConverter(typeof(EnumStringConverter))]
        public ETransformTRS transformTRS { get; set; } = ETransformTRS.None;

        #region ART

        /// <summary>
        /// ART客户端配置
        /// </summary>
        [Browsable(false)]
        public ARTClientConfig artClientConfig { get; set; } = new ARTClientConfig();

        /// <summary>
        /// ART服务器主机
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("服务器主机")]
        [Description("ART的服务器主机地址信息；")]
        [Json(false)]
        public string ARTServerHost { get => artClientConfig.serverHost; set => artClientConfig.serverHost = value; }

        /// <summary>
        /// ART服务器端口
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("服务器端口")]
        [Description("使用ART时，如仅接收数据本值为0即可；如使用DTrack2时，本端口值默认值为50105；")]
        [Json(false)]
        public int ARTServerPort { get => artClientConfig.serverPort; set => artClientConfig.serverPort = value; }

        /// <summary>
        /// ART数据端口
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("数据端口")]
        [Description("使用ART时，处理数据的端口；")]
        [Json(false)]
        public int ARTDataPort { get => artClientConfig.dataPort; set => artClientConfig.dataPort = value; }

        /// <summary>
        /// ART远程类型
        /// </summary>
        [Category("直连ART配置")]
        [DisplayName("远程类型")]
        [Description("使用ART时，远程服务器的类型；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ERemoteType ARTRemoteType { get => artClientConfig.remoteType; set => artClientConfig.remoteType = value; }

        #endregion
    }

    #endregion

    #region XBox配置

    /// <summary>
    /// XBox配置
    /// </summary>
    [Import]
    public class XBoxConfig : BaseInfoForActionInfo
    {
        /// <summary>
        /// 轴与按钮
        /// </summary>
        public EXBoxAxisAndButton axisAndButton { get; set; } = EXBoxAxisAndButton.None;

        /// <summary>
        /// 死区
        /// </summary>
        [Json(exportString = true)]
        public V2F deadZone { get => _deadZone; set => _deadZone = value; }

        internal V2F _deadZone = new V2F();
    }

    #endregion

    #region 姿态

    /// <summary>
    /// 姿态
    /// </summary>
    [Import]
    public class Pose
    {
        /// <summary>
        /// 位置
        /// </summary>
        [Json(exportString = true)]
        public V3F position { get => _position; set => _position = value; }

        internal V3F _position = new V3F();

        /// <summary>
        /// 旋转
        /// </summary>
        [Json(exportString = true)]
        public V3F rotation { get => _rotation; set => _rotation = value; }

        internal V3F _rotation = new V3F();

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public Pose Clone() => new Pose().CopyDataFrom(this);

        /// <summary>
        /// 从姿态复制数据
        /// </summary>
        /// <param name="pose"></param>
        /// <returns></returns>
        public Pose CopyDataFrom(Pose pose)
        {
            if (pose != null)
            {
                this.position = pose.position;
                this.rotation = pose.rotation;
            }
            return this;
        }
    }

    /// <summary>
    /// 对象偏移
    /// </summary>
    [Import]
    public class ObjectOffset : Pose
    {
        /// <summary>
        /// 启用
        /// </summary>
        public bool enable { get; set; } = true;
    }

    /// <summary>
    /// TRS数据
    /// </summary>
    public class TRSData : Pose
    {
        /// <summary>
        /// 缩放
        /// </summary>
        [Json(exportString = true)]
        public V3F scale { get => _scale; set => _scale = value; }

        internal V3F _scale = new V3F(1, 1, 1);
    }

    #endregion

    #region XR控制器配置

    /// <summary>
    /// XR控制器配置
    /// </summary>
    [Import]
    public class XRControllerConfig
    {
        /// <summary>
        /// 模型名
        /// </summary>
        public string modelName { get; set; } = "";

        /// <summary>
        /// 内置模型名数组
        /// </summary>
        internal static string[] buildinModelNames { get; } = new string[]
        {
            "",
            "手",
            "PICO4",
            "HTCVive",
            "RhinoX",
            "RhinoX Pro"
        };
    }

    #endregion

    #region 变换TRS

    /// <summary>
    /// 变换TRS
    /// </summary>
    public enum ETransformTRS
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 世界移动
        /// </summary>
        [Name("世界移动")]
        WorldTranslate,

        /// <summary>
        /// 世界旋转
        /// </summary>
        [Name("世界旋转")]
        WorldRotate,

        /// <summary>
        /// 本地移动
        /// </summary>
        [Name("本地移动")]
        LocalTranslate,

        /// <summary>
        /// 本地旋转
        /// </summary>
        [Name("本地旋转")]
        LocalRotate,

        /// <summary>
        /// 本地缩放
        /// </summary>
        [Name("本地缩放")]
        LocalScale,

        /// <summary>
        /// 世界旋转Y_本地旋转X
        /// </summary>
        [Name("世界旋转Y_本地旋转X")]
        WorldRotateY_LocalRotateX,

        /// <summary>
        /// 世界旋转Y_本地旋转XZ
        /// </summary>
        [Name("世界旋转Y_本地旋转XZ")]
        WorldRotateY_LocalRotateXZ,

        /// <summary>
        /// 世界旋转Y围绕HMD
        /// </summary>
        [Name("世界旋转Y围绕HMD")]
        WorldRotateYAroundHMD,

        /// <summary>
        /// 移动通过HMD投影
        /// </summary>
        [Name("移动通过HMD投影")]
        TranslateByMHDProjection,

        /// <summary>
        /// 移动通过HMD主相机投影
        /// </summary>
        [Name("移动通过HMD主相机投影")]
        TranslateByMHDMainCameraProjection,
    }

    /// <summary>
    /// 变换TRS组手
    /// </summary>
    public static class TransformTRSHelper
    {
#if UNITY_2018_3_OR_NEWER

        /// <summary>
        /// 对变换执行TRS操作
        /// </summary>
        /// <param name="transformTRS"></param>
        /// <param name="transform"></param>
        /// <param name="offset"></param>
        /// <param name="extensionTRS"></param>
        public static void TRS(this ETransformTRS transformTRS, Transform transform, Vector3 offset, Action<ETransformTRS, Transform,Vector3> extensionTRS = null)
        {
            if (!transform) return;
            switch (transformTRS)
            {
                case ETransformTRS.WorldTranslate:
                    {
                        transform.Translate(offset, Space.World);
                        break;
                    }
                case ETransformTRS.WorldRotate:
                    {
                        transform.Rotate(offset, Space.World);
                        break;
                    }
                case ETransformTRS.LocalTranslate:
                    {
                        transform.Translate(offset, Space.Self);
                        break;
                    }
                case ETransformTRS.LocalRotate:
                    {
                        transform.Rotate(offset, Space.Self);
                        break;
                    }
                case ETransformTRS.LocalScale:
                    {
                        transform.localScale += offset;
                        break;
                    }
                case ETransformTRS.WorldRotateY_LocalRotateX:
                    {
                        transform.Rotate(new Vector3(offset.x, 0, 0), Space.Self);
                        transform.Rotate(new Vector3(0, offset.y, 0), Space.World);
                        break;
                    }
                case ETransformTRS.WorldRotateY_LocalRotateXZ:
                    {
                        transform.Rotate(new Vector3(offset.x, 0, offset.z), Space.Self);
                        transform.Rotate(new Vector3(0, offset.y, 0), Space.World);
                        break;
                    }
                default:
                    {
                        extensionTRS?.Invoke(transformTRS, transform, offset);
                        break;
                    }
            }
        }
#endif
    }

    #endregion
}
