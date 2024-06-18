using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXBox.Base;
using XCSJ.PluginXRSpaceSolution.Base;

namespace XCSJ.PluginXBox.Tools
{
    /// <summary>
    /// 变换通过XBox
    /// </summary>
    [Name("变换通过XBox")]
    [Tip("默认通过XBox的轴或按钮控制变换的移动、旋转、缩放", "By default, the movement, rotation and scaling of the transformation are controlled through the axis or button of Xbox")]
    [Tool(XBoxHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.Model)]
    [RequireManager(typeof(XBoxManager))]
    [Owner(typeof(XBoxManager))]
    public class TransformByXBox : InteractProvider
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
        /// 目标变换
        /// </summary>
        [Name("目标变换")]
        public Transform _targetTransform;

        /// <summary>
        /// 目标变换
        /// </summary>
        public Transform targetTransform
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
        public XBoxControlData _controlData = new XBoxControlData();

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
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (targetTransform) { }
        }

        internal Action<ETransformTRS, Transform, Vector3> extensionTRS;

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!_targetTransform) return;
            var offset = Vector3.Scale(_controlData.GetOffset(), _speed) * Time.deltaTime;

            _transformTRS.TRS(_targetTransform, offset, extensionTRS);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (targetTransform) { }
        }
    }

    /// <summary>
    /// XBox控制数据
    /// </summary>
    [Serializable]
    [Name("XBox控制数据")]
    [Tip("使用XBox对XYZ进行控制三轴的数据类")]
    public class XBoxControlData
    {
        /// <summary>
        /// 负X:对应X值减小
        /// </summary>
        [Name("负X")]
        [Tip("对应X值减小", "The corresponding X value decreases")]
        [EnumPopup]
        public EXBoxAxisAndButton _nx = EXBoxAxisAndButton.LeftStickLeft;

        /// <summary>
        /// 负X死区
        /// </summary>
        [Name("负X死区")]
        [LimitRange(0, 1)]
        public Vector2 _nxDeadZone = new Vector2(0.1f, 0.9f);

        /// <summary>
        /// 负X目标值
        /// </summary>
        [Name("负X目标值")]
        public Vector2 _nxDstValue = new Vector2(0, 1);

        /// <summary>
        /// 正X:对应X值增加
        /// </summary>
        [Name("正X")]
        [Tip("对应X值增加", "Corresponding X value increase")]
        [EnumPopup]
        public EXBoxAxisAndButton _px = EXBoxAxisAndButton.LeftStickRight;

        /// <summary>
        /// 正X死区
        /// </summary>
        [Name("正X死区")]
        [LimitRange(0, 1)]
        public Vector2 _pxDeadZone = new Vector2(0.1f, 0.9f);

        /// <summary>
        /// 正X目标值
        /// </summary>
        [Name("正X目标值")]
        public Vector2 _pxDstValue = new Vector2(0, 1);

        /// <summary>
        /// 负Y:对应Y值减小
        /// </summary>
        [Name("负Y")]
        [Tip("对应Y值减小", "The corresponding Y value decreases")]
        [EnumPopup]
        public EXBoxAxisAndButton _ny = EXBoxAxisAndButton.DpadDown;

        /// <summary>
        /// 负Y死区
        /// </summary>
        [Name("负Y死区")]
        [LimitRange(0, 1)]
        public Vector2 _nyDeadZone = new Vector2(0.1f, 0.9f);

        /// <summary>
        /// 负Y目标值
        /// </summary>
        [Name("负Y目标值")]
        public Vector2 _nyDstValue = new Vector2(0, 1);

        /// <summary>
        /// 正Y:对应Y值增加
        /// </summary>
        [Name("正Y")]
        [Tip("对应Y值增加", "Corresponding Y value increase")]
        [EnumPopup]
        public EXBoxAxisAndButton _py = EXBoxAxisAndButton.DpadUp;

        /// <summary>
        /// 正Y死区
        /// </summary>
        [Name("正Y死区")]
        [LimitRange(0, 1)]
        public Vector2 _pyDeadZone = new Vector2(0.1f, 0.9f);

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
        [EnumPopup]
        public EXBoxAxisAndButton _nz = EXBoxAxisAndButton.LeftStickDown;

        /// <summary>
        /// 负Z死区
        /// </summary>
        [Name("负Z死区")]
        [LimitRange(0, 1)]
        public Vector2 _nzDeadZone = new Vector2(0.1f, 0.9f);

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
        [EnumPopup]
        public EXBoxAxisAndButton _pz = EXBoxAxisAndButton.LeftStickUp;

        /// <summary>
        /// 正Z死区
        /// </summary>
        [Name("正Z死区")]
        [LimitRange(0, 1)]
        public Vector2 _pzDeadZone = new Vector2(0.1f, 0.9f);

        /// <summary>
        /// 正Z目标值
        /// </summary>
        [Name("正Z目标值")]
        public Vector2 _pzDstValue = new Vector2(0, 1);

        /// <summary>
        /// 设置默认移动
        /// </summary>
        public void SetDefaultMove()
        {
            _nx = EXBoxAxisAndButton.LeftStickLeft;
            _px = EXBoxAxisAndButton.LeftStickRight;
            _ny = EXBoxAxisAndButton.DpadDown;
            _py = EXBoxAxisAndButton.DpadUp;
            _nz = EXBoxAxisAndButton.LeftStickDown;
            _pz = EXBoxAxisAndButton.LeftStickUp;
        }

        /// <summary>
        /// 设置默认旋转X
        /// </summary>
        public void SetDefaultRotateX()
        {
            _nx = EXBoxAxisAndButton.RightStickUp;
            _px = EXBoxAxisAndButton.RightStickDown;
        }

        /// <summary>
        /// 设置默认旋转Y
        /// </summary>
        public void SetDefaultRotateY()
        {
            _ny = EXBoxAxisAndButton.RightStickLeft;
            _py = EXBoxAxisAndButton.RightStickRight;
        }

        /// <summary>
        /// 设置全部无
        /// </summary>
        public void SetAllNone()
        {
            _nx = EXBoxAxisAndButton.None;
            _px = EXBoxAxisAndButton.None;
            _ny = EXBoxAxisAndButton.None;
            _py = EXBoxAxisAndButton.None;
            _nz = EXBoxAxisAndButton.None;
            _pz = EXBoxAxisAndButton.None;
        }

        /// <summary>
        /// 获取偏移值
        /// </summary>
        /// <returns></returns>
        public Vector3 GetOffset()
        {
            var x = _px.GetDstValue(_pxDeadZone, _pxDstValue) - _nx.GetDstValue(_nxDeadZone, _nxDstValue);
            var y = _py.GetDstValue(_pyDeadZone, _pyDstValue) - _ny.GetDstValue(_nyDeadZone, _nyDstValue);
            var z = _pz.GetDstValue(_pzDeadZone, _pzDstValue) - _nz.GetDstValue(_nzDeadZone, _nzDstValue);
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 获取Y轴偏移:XZ保持为0；
        /// </summary>
        /// <returns></returns>
        public Vector3 GetYOffset()
        {
            var y = _py.GetDstValue(_pyDeadZone, _pyDstValue) - _ny.GetDstValue(_nyDeadZone, _nyDstValue);
            return new Vector3(0, y, 0);
        }
    }
}
