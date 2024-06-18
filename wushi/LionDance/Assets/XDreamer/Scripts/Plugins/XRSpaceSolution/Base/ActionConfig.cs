using System;
using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.Extension.Base.UVRPN.Core;

namespace XCSJ.PluginXRSpaceSolution.Base
{
    /// <summary>
    /// 动作配置
    /// </summary>
    [Serializable]
    public class ActionConfig
    {
        /// <summary>
        /// 动作名
        /// </summary>
        [Name("动作名")]
        [ActionNamePopup]
        public StringPropertyValue _actionName = new StringPropertyValue();

        /// <summary>
        /// 动作名
        /// </summary>
        public string actionName
        {
            get => _actionName.GetValue();
            set
            {
                if (_actionName._propertyValueType == EPropertyValueType.Value)
                {
                    _actionName._value = value;
                }
            }
        }
    }

    /// <summary>
    /// 动作名弹出式特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ActionNamePopupAttribute : PropertyAttribute { }

    /// <summary>
    /// 姿态处理
    /// </summary>
    [Serializable]
    public class PoseHandler : ActionConfig
    {
        /// <summary>
        /// 姿态
        /// </summary>
        public PoseConfig pose { get; set; } = new PoseConfig();

        /// <summary>
        /// 源坐标系
        /// </summary>
        public CoordinateSystem srcCS { get; set; } = new CoordinateSystem();

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public bool Handle(XRAnswer answer)
        {
            if (answer is XRSpaceConfigA spaceConfigA)
            {
                if (spaceConfigA.GetActionInfo(actionName) is ActionInfo actionInfo)
                {
                    pose = actionInfo.pose ?? pose;
                }
                srcCS.scale = pose.scale;
                srcCS.lengthUnits = pose.lengtheUnit;
                srcCS.Set(pose.xAxis, pose.yAxis, pose.zAxis);
                return true;
            }
            else if (answer is ResetConfigA resetConfigA && resetConfigA.actionName == actionName)
            {
                needReset = true;
            }
            return false;
        }

        private bool needReset = false;

        /// <summary>
        /// 尝试获取姿态
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public bool TryGetPose(out Vector3 position, out Quaternion rotation)
        {
            try
            {
                if (needReset)//需要重置姿态
                {
                    position = default;
                    rotation = default;
                    needReset = false;
                    return true;
                }
                if (!pose.enable || !srcCS.isValid)//动作启用且坐标系有效
                {
                    position = default;
                    rotation = default;
                    return needReset;
                }

                var address = pose.address;
                position = VRPN_NativeBridge.TrackerPos(address, pose.channel);
                rotation = VRPN_NativeBridge.TrackerQuat(address, pose.channel);

                if (Mathf.Approximately(position.x, InvalidValid)
                    || Mathf.Approximately(position.y, InvalidValid)
                    || Mathf.Approximately(position.z, InvalidValid))
                {
                    return false;
                }

                //进行坐标系转化
                position = srcCS.ConvertPositionToUnity(position);
                rotation = srcCS.ConvertRotationToUnity(rotation).rotation;
                return true;
            }
            catch
            {
                //
            }
            position = default;
            rotation = default;
            return false;
        }

        /// <summary>
        /// 无效值
        /// </summary>
        public const float InvalidValid = -505f;
    }

    /// <summary>
    /// 交互处理
    /// </summary>
    [Serializable]
    public class InteractHandler : ActionConfig
    {
        /// <summary>
        /// 按钮交互
        /// </summary>
        public ButtonInteractConfig config { get; set; } = new ButtonInteractConfig();

        /// <summary>
        /// 允许按钮交互
        /// </summary>
        public bool allowButtonInteract => config.enable;

        /// <summary>
        /// 允许选择
        /// </summary>
        public bool allowSelect { get; private set; } = false;

        /// <summary>
        /// 选择
        /// </summary>
        public ButtonConfig select { get; set; } = new ButtonConfig();

        /// <summary>
        /// 允许选择
        /// </summary>
        public bool allowActivate { get; private set; } = false;

        /// <summary>
        /// 激活
        /// </summary>
        public ButtonConfig activate { get; set; } = new ButtonConfig();

        /// <summary>
        /// 允许UI
        /// </summary>
        public bool allowUI { get; private set; } = false;

        /// <summary>
        /// UI
        /// </summary>
        public ButtonConfig ui { get; set; } = new ButtonConfig();

        /// <summary>
        /// 任意VRPN启用
        /// </summary>
        public bool anyVRPNEnable => select.enable || activate.enable || ui.enable;

        /// <summary>
        /// 任意XBox启用
        /// </summary>
        public bool anyXBoxEnable => select.xboxEnable || activate.xboxEnable || ui.xboxEnable;

        /// <summary>
        /// 任意ART启用
        /// </summary>
        public bool anyARTEnable => select.ARTEnable || activate.ARTEnable || ui.ARTEnable;

        /// <summary>
        /// 任意UnityXR设备启用
        /// </summary>
        public bool anyUnityXRDeviceEnable => select.UnityXRDeviceEnable || activate.UnityXRDeviceEnable || ui.UnityXRDeviceEnable;

        /// <summary>
        /// 任意Unity输入动作启用
        /// </summary>
        public bool anyUnityInputActionEnable => select.UnityInputActionEnable || activate.UnityInputActionEnable || ui.UnityInputActionEnable;

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="answer"></param>
        public bool Handle(XRAnswer answer)
        {
            if (answer is XRSpaceConfigA spaceConfigA)
            {
                if(spaceConfigA.GetActionInfo(actionName) is ActionInfo actionInfo)
                {
                    config = actionInfo.buttonInteract ?? config;

                    select = actionInfo.interactSelect ?? select;
                    allowSelect = allowButtonInteract && select.enable;

                    activate = actionInfo.interactActivate ?? activate;
                    allowActivate = allowButtonInteract && activate.enable;

                    ui = actionInfo.interactUI ?? ui;
                    allowUI = allowButtonInteract && ui.enable;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <returns></returns>
        public bool Select() => allowSelect && VRPN_NativeBridge.Button(select.address, select.channel);

        /// <summary>
        /// 激活
        /// </summary>
        /// <returns></returns>
        public bool Activate() => allowActivate && VRPN_NativeBridge.Button(activate.address, activate.channel);

        /// <summary>
        /// UI
        /// </summary>
        /// <returns></returns>
        public bool UI() => allowUI && ui.enable && VRPN_NativeBridge.Button(ui.address, ui.channel);
    }

    /// <summary>
    /// 变换处理
    /// </summary>
    [Serializable]
    public class TransformHandler : ActionConfig
    {
        /// <summary>
        /// 配置
        /// </summary>
        public AnalogXYZConfig config { get; set; } = new AnalogXYZConfig();

        /// <summary>
        /// 速度
        /// </summary>
        public Vector3 speed { get; private set; } = new Vector3(1, 1, 1);

        /// <summary>
        /// 变换TRS
        /// </summary>
        public ETransformTRS transformTRS { get; private set; } = ETransformTRS.None;

        /// <summary>
        /// 负X
        /// </summary>
        public AnalogVrpnConfig nx { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 正X
        /// </summary>
        public AnalogVrpnConfig px { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 负Y
        /// </summary>
        public AnalogVrpnConfig ny { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 正Y
        /// </summary>
        public AnalogVrpnConfig py { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 负Z
        /// </summary>
        public AnalogVrpnConfig nz { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 正Z
        /// </summary>
        public AnalogVrpnConfig pz { get; set; } = new AnalogVrpnConfig();

        /// <summary>
        /// 任意VRPN启用
        /// </summary>
        public bool anyVRPNEnable => nx.enable || px.enable || ny.enable || py.enable || nz.enable || pz.enable;

        /// <summary>
        /// 任意XBox启用
        /// </summary>
        public bool anyXBoxEnable => nx.xboxEnable || px.xboxEnable || ny.xboxEnable || py.xboxEnable || nz.xboxEnable || pz.xboxEnable;

        /// <summary>
        /// 任意ART启用
        /// </summary>
        public bool anyARTEnable => nx.ARTEnable || px.ARTEnable || ny.ARTEnable || py.ARTEnable || nz.ARTEnable || pz.ARTEnable;

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="answer"></param>
        public bool Handle(XRAnswer answer)
        {
            if (answer is XRSpaceConfigA spaceConfigA)
            {
                if (spaceConfigA.GetActionInfo(actionName) is ActionInfo actionInfo)
                {
                    config = actionInfo.analogXYZ ?? config;
                    speed = config.speed.ToVector3();
                    transformTRS = config.transformTRS;

                    nx = actionInfo.nx ?? nx;
                    px = actionInfo.px ?? px;
                    ny = actionInfo.ny ?? ny;
                    py = actionInfo.py ?? py;
                    nz = actionInfo.nz ?? nz;
                    pz = actionInfo.pz ?? pz;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取X偏移
        /// </summary>
        /// <returns></returns>
        public double GetXOffset() => px.GetDstAnalog() - nx.GetDstAnalog();

        /// <summary>
        /// 获取Y偏移
        /// </summary>
        /// <returns></returns>
        public double GetYOffset() => py.GetDstAnalog() - ny.GetDstAnalog();

        /// <summary>
        /// 获取Z偏移
        /// </summary>
        /// <returns></returns>
        public double GetZOffset() => pz.GetDstAnalog() - nz.GetDstAnalog();

        /// <summary>
        /// 获取偏移
        /// </summary>
        /// <returns></returns>
        public Vector3 GetOffset() => new Vector3((float)GetXOffset(), (float)GetYOffset(), (float)GetZOffset());

        /// <summary>
        /// 获取速度偏移
        /// </summary>
        /// <returns></returns>
        public Vector3 GetSpeedOffset() => Vector3.Scale(GetOffset(), speed);
    }

    /// <summary>
    /// VRPN组手
    /// </summary>
    public static class VRPNHelper
    {
        /// <summary>
        /// 获取源模拟量
        /// </summary>
        /// <param name="analogVrpnConfig"></param>
        /// <returns></returns>
        public static double GetSrcAnalog(this AnalogVrpnConfig analogVrpnConfig) => analogVrpnConfig.enable ? VRPN_NativeBridge.Analog(analogVrpnConfig.address, analogVrpnConfig.channel) : 0;

        /// <summary>
        /// 获取目标模拟量
        /// </summary>
        /// <param name="analogVrpnConfig"></param>
        /// <returns></returns>
        public static double GetDstAnalog(this AnalogVrpnConfig analogVrpnConfig) => analogVrpnConfig.GetDstValue(analogVrpnConfig.GetSrcAnalog());
    }
}
