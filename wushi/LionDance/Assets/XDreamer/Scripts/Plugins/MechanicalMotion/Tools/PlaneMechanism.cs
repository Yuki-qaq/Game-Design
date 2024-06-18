using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;

namespace XCSJ.PluginMechanicalMotion.Tools
{
    /// <summary>
    /// 平面运动机构
    /// </summary>
    public abstract class PlaneMechanism : Mechanism
    {
        #region 平面数据

        /// <summary>
        /// 运动目标
        /// </summary>
        [Name("运动目标")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _motionTarget;

        /// <summary>
        /// 运动目标
        /// </summary>
        public Transform motionTarget
        {
            get
            {
                if (!_motionTarget)
                {
                    _motionTarget = GetComponent<Transform>();
                }
                return _motionTarget;
            }
        }

        /// <summary>
        /// 运动平面
        /// </summary>
        [Name("运动平面")]
        public PointDirectionData _plane = new PointDirectionData();

        /// <summary>
        /// 初始朝向
        /// </summary>
        [Name("初始朝向")]
        [Tip("初始朝向在平面内；该点在运动目标本地坐标系下定义")]
        public Vector3Data _initDirection = new Vector3Data();

        #endregion

        #region 速度量

        /// <summary>
        /// 初始朝向：世界坐标系
        /// </summary>
        public virtual Vector3 initDirection => _initDirection.data;

        /// <summary>
        /// 内部速度量：标量
        /// </summary>
        protected virtual double velocityInternal { get; set; }

        /// <summary>
        /// 速度量：标量
        /// </summary>
        public double velocity
        {
            get => velocityInternal;
            set
            {
                if (MathX.Approximately(velocityInternal, value)) return;

                var old = velocityInternal;
                velocityInternal = value;
                onEventCallback?.Invoke(this, new EventData(EEventType.VelocityChanged, old));
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 事件回调：参数1=运动机构，参数2=事件数据
        /// </summary>
        public static event Action<PlaneMechanism, EventData> onEventCallback;

        /// <summary>
        /// 事件类型
        /// </summary>
        public enum EEventType
        {
            /// <summary>
            /// 速度改变
            /// </summary>
            [Name("速度改变")]
            VelocityChanged,

            /// <summary>
            /// 值改变
            /// </summary>
            [Name("值改变")]
            ValueChanged,

            /// <summary>
            /// 到达最小值
            /// </summary>
            [Name("到达最小值")]
            MinValue,

            /// <summary>
            /// 到达最大值
            /// </summary>
            [Name("到达最大值")]
            MaxValue,
        }

        /// <summary>
        /// 事件数据
        /// </summary>
        public class EventData
        {
            /// <summary>
            /// 事件类型
            /// </summary>
            public EEventType eventType { get; private set; }

            /// <summary>
            /// 旧值，可能是速度也可能是值
            /// </summary>
            public double oldValue { get; private set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="eventType"></param>
            public EventData(EEventType eventType)
            {
                this.eventType = eventType;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="eventType"></param>
            /// <param name="oldValue"></param>
            public EventData(EEventType eventType, double oldValue)
            {
                this.eventType = eventType;
                this.oldValue = oldValue;
            }
        }

        #endregion

        #region 数值量

        /// <summary>
        /// 限定运动
        /// </summary>
        [Group("运动设置", textEN = "Motion Settings")]
        [Name("限定运动")]
        public bool _isLimit = false;

        /// <summary>
        /// 限定范围
        /// </summary>
        [Name("限定范围")]
        [Tip("基于游戏对象初始量的相对值")]
        [LimitRange(-360, 360)]
        [HideInSuperInspector(nameof(_isLimit), EValidityCheckType.False)]
        public Vector2 _range = new Vector2(0, 90);

        /// <summary>
        /// 当前值
        /// </summary>
        public virtual double currentValue 
        { 
            get => _currentValue; 
            set
            {
                var newValue = MathX.Clamp(value, minValue, maxValue);
                if (MathX.Approximately(_currentValue, newValue)) return;

                var old = _currentValue;
                _currentValue = newValue;
                onEventCallback?.Invoke(this, new EventData(EEventType.ValueChanged, old));
            }
        }

        /// <summary>
        /// 当前值
        /// </summary>
        protected double _currentValue;

        /// <summary>
        /// 最小值
        /// </summary>
        protected double _minValue { get; set; } = Double.MinValue;

        /// <summary>
        /// 最小值
        /// </summary>
        public double minValue
        {
            get => _isLimit ? _range.x : _minValue;
            protected set
            {
                if (value > maxValue) value = maxValue;

                if (_isLimit)
                {
                    _range.x = (float)value;
                }
                else
                {
                    _minValue = value;
                }
            }
        }

        /// <summary>
        /// 最大值
        /// </summary>
        protected double _maxValue { get; set; } = Double.MaxValue;

        /// <summary>
        /// 最大值
        /// </summary>
        public double maxValue
        {
            get => _isLimit ? _range.y : _maxValue;
            protected set
            {
                if (value > minValue) value = minValue;

                if (_isLimit)
                {
                    _range.y = (float)value;
                }
                else
                {
                    _maxValue = value;
                }
            }
        }

        /// <summary>
        /// 偏移值：运动开始后与程序初始启动值的偏差
        /// </summary>
        public double offsetValue { get => currentValue - initValue; protected set => currentValue = initValue + value; }

        /// <summary>
        /// 位移偏差值
        /// </summary>
        public virtual double displacementOffset { get => offsetValue; set => offsetValue = value; }

        /// <summary>
        /// 目标值
        /// </summary>
        public double targetValue { get; private set; }

        private bool followTargetValue = false;

        /// <summary>
        /// 程序启动初始值
        /// </summary>
        private double initValue = 0;

        /// <summary>
        /// 程序启动初始速度
        /// </summary>
        private double initVelocity = 0;

        #endregion

        #region Unity生命周期事件

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (motionTarget) { }

            _plane.Reset(motionTarget);
            _initDirection.SetTransform(motionTarget);
            _initDirection._dataType = EVector3DataType.TransformForward;
        }

        /// <summary>
        /// 有效回调
        /// </summary>
        protected virtual void OnValidate()
        {
            if (_range.x > _range.y) _range.x = _range.y;

            currentValue = MathX.Clamp(currentValue, minValue, maxValue);
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            currentValue = MathX.Clamp(currentValue, minValue, maxValue);

            UpdateNetData();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected virtual void Start()
        {
            initValue = currentValue;
            initVelocity = velocity;
        }

        /// <summary>
        /// 重置为初始化
        /// </summary>
        public void ResetInit()
        {
            velocity = initVelocity;
            SetInitValue();
        }

        /// <summary>
        /// 能否执行运动
        /// </summary>
        /// <returns></returns>
        public override bool CanDoMotion()
        {
            // 速度不为0，并且没有达到最小最大值时，认为可以运动
            return (!MathX.ApproximatelyZero(velocity)
                && !(velocity < 0 && MathX.Approximately(currentValue, minValue))
                && !(velocity > 0 && MathX.Approximately(currentValue, maxValue)));
        }

        /// <summary>
        /// 执行运动
        /// </summary>
        public override void DoMotion()
        {
            double offset = velocityInternal * Time.deltaTime;

            var newValue = currentValue + offset;

            // 限定在最小和最大值之间
            offset = MathX.Clamp(newValue, minValue, maxValue) - currentValue;

            // 有目标时，使用补间方法
            if (followTargetValue)
            {
                // 到达或越过目标长度点, 则设定为到达目标
                if (MathX.Approximately(currentValue, targetValue)
                    || (currentValue < targetValue && newValue >= targetValue)
                    || (currentValue > targetValue && newValue <= targetValue))
                {
                    followTargetValue = false;
                    velocity = 0;
                    offset = targetValue - currentValue;
                }
            }
            // 到达最小值最大值则设置速度为0
            var reachMaxValue = (currentValue < maxValue && newValue >= maxValue);
            var reachMinValue = (currentValue > minValue && newValue <= minValue);

            currentValue += offset;

            // 先加当前值后进行回调
            if (reachMaxValue)
            {
                onEventCallback?.Invoke(this, new EventData(EEventType.MaxValue));
                velocity = 0;
            }

            if (reachMinValue)
            {
                onEventCallback?.Invoke(this, new EventData(EEventType.MinValue));
                velocity = 0;
            }
        }

        #endregion

        #region 设置值

        /// <summary>
        /// 设置期望到达的目标值和时间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetTargetValueByTime(double value, double time)
        {
            var rs = TryGetValue(value, out var validValue);
            if (rs)
            {
                // 当时间为0时，表示瞬间到达目标
                if (MathX.ApproximatelyZero(time))
                {
                    SetValueInternal(value);
                }
                else // 时间不为0时，计算移动速度
                {
                    SetTargetValueVelocity(validValue, (targetValue - currentValue) / time);
                }
            }
            return rs;
        }

        /// <summary>
        /// 设置期望到达的目标值和速度
        /// </summary>
        /// <param name="value"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public bool SetTargetValueByVelocity(double value, double velocity)
        {
            var rs = TryGetValue(value, out var validValue);
            if (rs)
            {
                // 调整速度方向，保证对象能按正确方向移动到目标值
                SetTargetValueVelocity(validValue, validValue > currentValue ? velocity : -velocity);
            }
            return rs;
        }

        private void SetTargetValueVelocity(double value, double velocity)
        {
            targetValue = value;

            this.velocity = velocity;
            followTargetValue = true;
        }

        /// <summary>
        /// 直接设定目标值, 并将速度量设置为0
        /// </summary>
        /// <param name="value"></param>
        public bool SetValue(double value)
        {
            var rs = TryGetValue(value, out var validValue);
            if (rs)
            {
                SetValueInternal(validValue);
            }
            return rs;
        }

        private void SetValueInternal(double value)
        {
            velocity = 0;
            currentValue = value;
        }

        private bool TryGetValue(double inValue, out double outValue)
        {
            outValue = MathX.Clamp(inValue, minValue, maxValue);

            return !MathX.Approximately(currentValue, outValue);
        }

        /// <summary>
        /// 设置为最小值
        /// </summary>
        public virtual void SetMinValue() => SetValueInternal(minValue);

        /// <summary>
        /// 设置为最大值
        /// </summary>
        public virtual void SetMaxValue() => SetValueInternal(maxValue);

        /// <summary>
        /// 设置为初始值
        /// </summary>
        public virtual void SetInitValue() => SetValueInternal(initValue);

        #endregion

        #region 设置偏移值

        /// <summary>
        /// 设置偏移量
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetOffsetByTime(double offset, double time) => SetTargetValueByTime(currentValue + offset, time);

        /// <summary>
        /// 设置偏移量
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetOffsetByVelocity(double offset, double time) => SetTargetValueByTime(currentValue + offset, time);

        /// <summary>
        /// 设置偏移量
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool SetOffset(double offset) => SetValue(currentValue + offset);

        #endregion

        #region MMO对象

       /// <summary>
       /// 平面机构网络数据
       /// </summary>
        [Serializable]
        [Import]
        public class PlaneMechanismNetData
        {
            /// <summary>
            /// 速度
            /// </summary>
            [Readonly]
            [Name("速度")]
            public double velocity = 0;

            /// <summary>
            /// 当前值
            /// </summary>
            [Readonly]
            [Name("当前值")]
            public double currentValue = 0;

            /// <summary>
            /// 构造函数
            /// </summary>
            public PlaneMechanismNetData() { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="velocity"></param>
            /// <param name="currentValue"></param>
            public PlaneMechanismNetData(double velocity, double currentValue)
            {
                this.velocity = velocity;
                this.currentValue = currentValue;
            }

            /// <summary>
            /// 复制到
            /// </summary>
            /// <param name="planeMechanismNetData"></param>
            public void CopyTo(PlaneMechanismNetData planeMechanismNetData)
            {
                if (planeMechanismNetData!=null)
                {
                    planeMechanismNetData.velocity = this.velocity;
                    planeMechanismNetData.currentValue = this.currentValue;
                }
            }

            /// <summary>
            /// 相等
            /// </summary>
            /// <param name="planeMechanismNetData"></param>
            /// <returns></returns>
            public bool Equals(PlaneMechanismNetData planeMechanismNetData)
            {
                return MathX.Approximately(velocity, planeMechanismNetData.velocity);
            }
        }

        /// <summary>
        /// 平面机构网络数据
        /// </summary>
        [Group("MMO数据", textEN = "MMO Datas", defaultIsExpanded = false)]
        [Readonly]
        [Name("平面机构网络数据")]
        public PlaneMechanismNetData _planeMechanismNetData = new PlaneMechanismNetData();

        /// <summary>
        /// 上一次数据
        /// </summary>
        [Readonly]
        [Name("上一次数据")]
        public PlaneMechanismNetData _lastData = new PlaneMechanismNetData();

        /// <summary>
        /// 原始数据
        /// </summary>
        [Readonly]
        [Name("原始数据")]
        [EndGroup(true)]
        public PlaneMechanismNetData _originalData = new PlaneMechanismNetData();

        private void UpdateNetData()
        {
            _planeMechanismNetData.velocity = velocity;
            _planeMechanismNetData.currentValue = currentValue;
        }

        /// <summary>
        /// 当序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool OnSerialize(out string data)
        {
            UpdateNetData();
            if (!_lastData.Equals(_planeMechanismNetData) || _dirty)
            {
                _dirty = false;
                _planeMechanismNetData.CopyTo(_lastData);
                data = JsonHelper.ToJson(_planeMechanismNetData);
                return true;
            }
            data = default;
            return false;
        }

        /// <summary>
        /// 当反序列化
        /// </summary>
        /// <param name="data"></param>
        public override void OnDeserialize(Data data)
        {
            version = data.version;

            if (data.IsLocalUserSended())
            {
                _planeMechanismNetData.CopyTo(_lastData);
                return;
            }

            var netData = JsonHelper.ToObject<PlaneMechanismNetData>(data.data);
            netData?.CopyTo(_planeMechanismNetData);

            velocity = _planeMechanismNetData.velocity;
            currentValue = _planeMechanismNetData.currentValue;
        }

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            version = 0;

            _planeMechanismNetData.CopyTo(_originalData);
            _planeMechanismNetData.CopyTo(_lastData);
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            version = 0;
            _originalData.CopyTo(_planeMechanismNetData);
        }

        #endregion
    }
}
