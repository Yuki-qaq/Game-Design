using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCameras;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginXBox.Base;
using XCSJ.PluginXBox.Tools;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.Extension.Base.UVRPN.Core;
using XCSJ.PluginART.Tools;
using XCSJ.PluginART.Base;
using XCSJ.PluginART;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;

namespace XCSJ.PluginXRSpaceSolution.Tools
{
    /// <summary>
    /// 变换通过XRIS
    /// </summary>
    [Name("变换通过XRIS")]
    [Tip("通过XRIS控制变换的TRS(位置、旋转、缩放)", "TRS (position, rotation, scaling) controlled by XRIS transformation")]
    [RequireManager(typeof(XRSpaceSolutionManager))]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    public class TransformByXRIS : InteractProvider
    {
        /// <summary>
        /// 目标变换
        /// </summary>
        [Name("目标变换")]
        public Transform _targetTransform;

        /// <summary>
        /// 变换处理
        /// </summary>
        [Name("变换处理")]
        [OnlyMemberElements]
        public TransformHandler _transformHandler = new TransformHandler();

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            XDreamerEvents.onXRAnswerReceived += OnXRAnswerReceived;

            if (!_targetTransform) _targetTransform = transform;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            XDreamerEvents.onXRAnswerReceived -= OnXRAnswerReceived;
        }

        private void OnXRAnswerReceived(XRAnswer answer)
        {
            if (!_transformHandler.Handle(answer)) return;

            HandleXBox();
            HandleART();
        }

        private void HandleXBox()
        {
            var transformByXBox = GetComponents<TransformByXBox>().FirstOrDefault(c => c && c.usageForXRIS == _transformHandler.actionName);
            var needEnable = _transformHandler.config.enable && _transformHandler.anyXBoxEnable;
            if (!transformByXBox)
            {
                if (!needEnable) return;
                transformByXBox = this.XAddComponent<TransformByXBox>();
                transformByXBox._targetTransform = this._targetTransform;
                transformByXBox.usageForXRIS = _transformHandler.actionName;
                transformByXBox.extensionTRS = ExtensionTRS;
            }

            transformByXBox._speed = _transformHandler.speed;
            transformByXBox._transformTRS = _transformHandler.transformTRS;
            transformByXBox.XSetEnable(needEnable);

            var controlData = transformByXBox._controlData;

            controlData._nx = _transformHandler.nx.xboxConfig.enable ? _transformHandler.nx.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
            controlData._nxDeadZone = _transformHandler.nx.xboxConfig.deadZone.ToVector2();
            controlData._nxDstValue = _transformHandler.nx.xboxDstValue.ToVector2();

            controlData._px = _transformHandler.px.xboxConfig.enable ? _transformHandler.px.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
            controlData._pxDeadZone = _transformHandler.px.xboxConfig.deadZone.ToVector2();
            controlData._pxDstValue = _transformHandler.px.xboxDstValue.ToVector2();

            controlData._ny = _transformHandler.ny.xboxConfig.enable ? _transformHandler.ny.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
            controlData._nyDeadZone = _transformHandler.ny.xboxConfig.deadZone.ToVector2();
            controlData._nyDstValue = _transformHandler.ny.xboxDstValue.ToVector2();

            controlData._py = _transformHandler.py.xboxConfig.enable ? _transformHandler.py.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
            controlData._pyDeadZone = _transformHandler.py.xboxConfig.deadZone.ToVector2();
            controlData._pyDstValue = _transformHandler.py.xboxDstValue.ToVector2();

            controlData._nz = _transformHandler.nz.xboxConfig.enable ? _transformHandler.nz.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
            controlData._nzDeadZone = _transformHandler.nz.xboxConfig.deadZone.ToVector2();
            controlData._nzDstValue = _transformHandler.nz.xboxDstValue.ToVector2();

            controlData._pz = _transformHandler.pz.xboxConfig.enable ? _transformHandler.pz.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
            controlData._pzDeadZone = _transformHandler.pz.xboxConfig.deadZone.ToVector2();
            controlData._pzDstValue = _transformHandler.pz.xboxDstValue.ToVector2();
        }


        ARTStreamClient _ARTStreamClient;

        private void HandleART()
        {
            var transformByARTFlystick = GetComponents<TransformByARTFlystick>().FirstOrDefault(c => c && c.usageForXRIS == _transformHandler.actionName);
            var needEnable = _transformHandler.config.enable && _transformHandler.anyARTEnable;
            if (!transformByARTFlystick && !needEnable) return;

            ARTManager.HandleClient(ref _ARTStreamClient, _transformHandler.config.artClientConfig, client =>
            {
                if (!transformByARTFlystick)
                {
                    transformByARTFlystick = this.XAddComponent<TransformByARTFlystick>();
                    transformByARTFlystick._targetTransform = this._targetTransform;
                    transformByARTFlystick.usageForXRIS = _transformHandler.actionName;
                    transformByARTFlystick.extensionTRS = ExtensionTRS;
                }

                transformByARTFlystick._speed = _transformHandler.speed;
                transformByARTFlystick._transformTRS = _transformHandler.transformTRS;
                transformByARTFlystick.XSetEnable(needEnable);

                var controlData = transformByARTFlystick._controlData;
                
                controlData._nx = _transformHandler.nx.buttonARTConfig.enable ? _transformHandler.nx.buttonARTConfig.flystickButton : new FlystickButton();
                controlData._nxDstValue = _transformHandler.nx.artDstValue.ToVector2();

                controlData._px = _transformHandler.px.buttonARTConfig.enable ? _transformHandler.px.buttonARTConfig.flystickButton : new FlystickButton();
                controlData._pxDstValue = _transformHandler.px.artDstValue.ToVector2();

                controlData._ny = _transformHandler.ny.buttonARTConfig.enable ? _transformHandler.ny.buttonARTConfig.flystickButton : new FlystickButton();
                controlData._nyDstValue = _transformHandler.ny.artDstValue.ToVector2();

                controlData._py = _transformHandler.py.buttonARTConfig.enable ? _transformHandler.py.buttonARTConfig.flystickButton : new FlystickButton();
                controlData._pyDstValue = _transformHandler.py.artDstValue.ToVector2();

                controlData._nz = _transformHandler.nz.buttonARTConfig.enable ? _transformHandler.nz.buttonARTConfig.flystickButton : new FlystickButton();
                controlData._nzDstValue = _transformHandler.nz.artDstValue.ToVector2();

                controlData._pz = _transformHandler.pz.buttonARTConfig.enable ? _transformHandler.pz.buttonARTConfig.flystickButton : new FlystickButton();
                controlData._pzDstValue = _transformHandler.pz.artDstValue.ToVector2();
            });        
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!_transformHandler.config.enable) return;

            var offset = _transformHandler.GetSpeedOffset() * Time.deltaTime;

            _transformHandler.transformTRS.TRS(_targetTransform, offset, ExtensionTRS);
        }

        /// <summary>
        /// 执行TRS
        /// </summary>
        /// <param name="transformTRS"></param>
        /// <param name="transform"></param>
        /// <param name="offset"></param>
        public void ExtensionTRS(ETransformTRS transformTRS, Transform transform, Vector3 offset)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            switch (transformTRS)
            {
                case ETransformTRS.WorldRotateYAroundHMD:
                    {
                        var xrOrigin = transform.GetComponent<Unity.XR.CoreUtils.XROrigin>();
                        if (!xrOrigin) return;
                        xrOrigin.RotateAroundCameraUsingOriginUp(offset.y);
                        break;
                    }
                case ETransformTRS.TranslateByMHDProjection:
                    {
                        var xrOrigin = transform.GetComponent<XROriginOwner>();
                        if (!xrOrigin) return;

                        var up = transform.up;
                        var ct = xrOrigin.hmd.transform;

                        var fp = Vector3.ProjectOnPlane(ct.forward, up);
                        var rp = Vector3.ProjectOnPlane(ct.right, up);

                        transform.position += (fp * offset.z + up * offset.y + rp * offset.x);
                        break;
                    }
                case ETransformTRS.TranslateByMHDMainCameraProjection:
                    {
                        var xrOrigin = transform.GetComponent<XROriginOwner>();
                        if (!xrOrigin) return;

                        var up = transform.up;
                        var ct = xrOrigin.hmdMainCamera.transform;

                        var fp = Vector3.ProjectOnPlane(ct.forward, up);
                        var rp = Vector3.ProjectOnPlane(ct.right, up);

                        transform.position += (fp * offset.z + up * offset.y + rp * offset.x);
                        break;
                    }
            }
#endif
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            _transformHandler.actionName = "变换TRS";
        }
    }
}
