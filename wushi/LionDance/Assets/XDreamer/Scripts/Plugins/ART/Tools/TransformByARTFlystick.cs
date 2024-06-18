using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginART.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXRSpaceSolution.Base;

namespace XCSJ.PluginART.Tools
{
    /// <summary>
    /// 变换通过ART Flystick:通过与ART进行数据流通信，控制目标变换的姿态（位置与旋转）
    /// </summary> 
    [Name("变换通过ART Flystick")]
    [Tip("通过与ART进行数据流通信，控制目标变换的姿态（位置与旋转）", "Through data flow communication with art, the attitude (position and rotation) of the target is controlled")]
    [Tool(ARTHelper.Title, rootType = typeof(ARTManager))]
    public class TransformByARTFlystick : BaseTransformByART
    {
        /// <summary>
        /// XRIS用途
        /// </summary>
        public string usageForXRIS
        {
            get => _tagProperty.GetFirstValue(nameof(usageForXRIS));
            set => _tagProperty.SetFirstValue(nameof(usageForXRIS), value);
        }

        /// <summary>
        /// 目标变换:用于控制的目标变换
        /// </summary>
        [Name("目标变换")]
        [Tip("用于控制的目标变换", "Target transformation for control")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _targetTransform;

        /// <summary>
        /// 目标变换
        /// </summary>
        public override Transform targetTransform
        {
            get
            {
                if (!_targetTransform)
                {
                    _targetTransform = this.transform;
                }
                return _targetTransform;
            }
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        public override EDataType dataType { get => EDataType.FlyStick; set { } }

        /// <summary>
        /// 刚体ID
        /// </summary>
        public override int rigidBodyID
        {
            get => 0;
            set => this.XModifyProperty(() => _controlData.SetFlysitckID(value));
        }

        /// <summary>
        /// 空间类型
        /// </summary>
        public override ESpaceType spaceType => ESpaceType.Local;

        /// <summary>
        /// 速度
        /// </summary>
        [Name("速度")]
        public Vector3 _speed = Vector3.one;

        /// <summary>
        /// 变换TRS
        /// </summary>
        [Name("变换TRS")]
        [EnumPopup]
        public ETransformTRS _transformTRS = ETransformTRS.None;

        /// <summary>
        /// 控制数据
        /// </summary>
        [Name("控制数据")]
        public FlystickButtonControlData _controlData = new FlystickButtonControlData();

        /// <summary>
        /// 设置默认移动
        /// </summary>
        public void SetDefaultMove()
        {
            this.XModifyProperty(() =>
            {
                _transformTRS = ETransformTRS.LocalTranslate;
                _controlData.SetDefaultMove();
                _speed = Vector3.one;
            });
        }

        /// <summary>
        /// 设置默认旋转世界Y
        /// </summary>
        public void SetDefaultRotateWorldY()
        {
            this.XModifyProperty(() =>
            {
                _transformTRS = ETransformTRS.WorldRotate;
                _controlData.SetAllNone();
                _controlData.SetDefaultRotateY();
                _speed = new Vector3(1, 20, 1);
            });
        }

        /// <summary>
        /// 设置默认旋转本地X
        /// </summary>
        public void SetDefaultRotateLocalX()
        {
            this.XModifyProperty(() =>
            {
                _transformTRS = ETransformTRS.LocalRotate;
                _controlData.SetAllNone();
                _controlData.SetDefaultRotateX();
                _speed = new Vector3(20, 1, 1);
            });
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (targetTransform) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (targetTransform) { }
        }

        internal Action<ETransformTRS, Transform, Vector3> extensionTRS;

        /// <summary>
        /// 更新姿态
        /// </summary>
        protected override void UpdatePose()
        {
            //base.UpdatePose();
            if (!_streamClient || !_targetTransform) return;

            var offset = Vector3.Scale(_controlData.GetOffset(_streamClient), _speed) * Time.deltaTime;
            _transformTRS.TRS(_targetTransform, offset, extensionTRS);
        }
    }


    /// <summary>
    /// Flystick按钮数据
    /// </summary>
    [Serializable]
    [Name("Flystick按钮数据")]
    public class FlystickButtonControlData
    {
        /// <summary>
        /// 负X:对应X值减小
        /// </summary>
        [Name("负X")]
        [Tip("对应X值减小", "The corresponding X value decreases")]
        public FlystickButton _nx = new FlystickButton();

        /// <summary>
        /// 负X目标值
        /// </summary>
        [Name("负X目标值")]
        public Vector2 _nxDstValue = new Vector2(0, 1);

        /// <summary>
        /// 正X:对应X值增加
        /// </summary>
        [Name("正X")]
        [Tip("对应X值增加", "Corresponding x value increase")]
        public FlystickButton _px = new FlystickButton();

        /// <summary>
        /// 正X目标值
        /// </summary>
        [Name("正X目标值")]
        public Vector2 _pxDstValue = new Vector2(0, 1);

        /// <summary>
        /// 负Y:对应Y值减小
        /// </summary>
        [Name("负Y")]
        [Tip("对应Y值减小", "The corresponding y value decreases")]
        public FlystickButton _ny = new FlystickButton();

        /// <summary>
        /// 负Y目标值
        /// </summary>
        [Name("负Y目标值")]
        public Vector2 _nyDstValue = new Vector2(0, 1);

        /// <summary>
        /// 正Y:对应Y值增加
        /// </summary>
        [Name("正Y")]
        [Tip("对应Y值增加", "The corresponding y value increases")]
        public FlystickButton _py = new FlystickButton();

        /// <summary>
        /// 正Y目标值
        /// </summary>
        [Name("正Y目标值")]
        public Vector2 _pyDstValue = new Vector2(0, 1);

        /// <summary>
        /// 负Z:对应Z值减小
        /// </summary>
        [Name("负Z")]
        [Tip("对应Z值减小", "The corresponding Z value decreases")]
        public FlystickButton _nz = new FlystickButton();

        /// <summary>
        /// 负Z目标值
        /// </summary>
        [Name("负Z目标值")]
        public Vector2 _nzDstValue = new Vector2(0, 1);

        /// <summary>
        /// 正Z:对应Z值增加
        /// </summary>
        [Name("正Z")]
        [Tip("对应Z值增加", "Corresponding Z value increase")]
        public FlystickButton _pz = new FlystickButton();

        /// <summary>
        /// 正Z目标值
        /// </summary>
        [Name("正Z目标值")]
        public Vector2 _pzDstValue = new Vector2(0, 1);

        /// <summary>
        /// 设置Flystick编号
        /// </summary>
        /// <param name="id"></param>
        public void SetFlysitckID(int id)
        {
            _nx.flysitckID = id;
            _px.flysitckID = id;
            _ny.flysitckID = id;
            _py.flysitckID = id;
            _nz.flysitckID = id;
            _pz.flysitckID = id;
        }

        /// <summary>
        /// 设置默认移动
        /// </summary>
        public void SetDefaultMove()
        {
            _nx.flystick = EFlystick.Flystick2;
            _nx.flystick2Switchs = EFlystick2Switchs.JoystickLeft;

            _px.flystick = EFlystick.Flystick2;
            _px.flystick2Switchs = EFlystick2Switchs.JoystickRight;

            _ny.flystick = EFlystick.Flystick2;
            _ny.flystick2Switchs = EFlystick2Switchs.None;

            _py.flystick = EFlystick.Flystick2;
            _py.flystick2Switchs = EFlystick2Switchs.None;

            _nz.flystick = EFlystick.Flystick2;
            _nz.flystick2Switchs = EFlystick2Switchs.JoystickDown;

            _pz.flystick = EFlystick.Flystick2;
            _pz.flystick2Switchs = EFlystick2Switchs.JoystickUp;
        }

        /// <summary>
        /// 设置默认旋转X
        /// </summary>
        public void SetDefaultRotateX()
        {
            _nx.flystick2Switchs = EFlystick2Switchs.JoystickUp;
            _px.flystick2Switchs = EFlystick2Switchs.JoystickDown;
        }

        /// <summary>
        /// 设置默认旋转Y
        /// </summary>
        public void SetDefaultRotateY()
        {
            _ny.flystick2Switchs = EFlystick2Switchs.JoystickLeft;
            _py.flystick2Switchs = EFlystick2Switchs.JoystickRight;
        }

        /// <summary>
        /// 设置全部无
        /// </summary>
        public void SetAllNone()
        {
            _nx.flystick2Switchs = EFlystick2Switchs.None;
            _px.flystick2Switchs = EFlystick2Switchs.None;
            _ny.flystick2Switchs = EFlystick2Switchs.None;
            _py.flystick2Switchs = EFlystick2Switchs.None;
            _nz.flystick2Switchs = EFlystick2Switchs.None;
            _pz.flystick2Switchs = EFlystick2Switchs.None;
        }

        /// <summary>
        /// 获取偏移值
        /// </summary>
        /// <param name="streamClient"></param>
        /// <returns></returns>
        public Vector3 GetOffset(ARTStreamClient streamClient)
        {
            var x = _px.GetDstValue(streamClient, _pxDstValue) - _nx.GetDstValue(streamClient, _nxDstValue);
            var y = _py.GetDstValue(streamClient, _pyDstValue) - _ny.GetDstValue(streamClient, _nyDstValue);
            var z = _pz.GetDstValue(streamClient, _pzDstValue) - _nz.GetDstValue(streamClient, _nzDstValue);
            return new Vector3(x, y, z);
        }
    }
}
