using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Events;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络变换
    /// </summary>
    [Attributes.Icon(EIcon.Transform)]
    [DisallowMultipleComponent]
    [Name("网络变换")]
    [Tool(MMOHelperExtension.Title, nameof(NetIdentity), rootType = typeof(MMOManager))]
    public sealed class NetTransform : NetMB, IReset
    {
        /// <summary>
        /// 同步模式
        /// </summary>
        [Group("同步检测设置")]
        [Name("同步模式")]
        [EnumPopup]
        [FormerlySerializedAs(nameof(syncMode))]
        public ESyncMode _syncMode = ESyncMode.Transform;

        /// <summary>
        /// 同步模式
        /// </summary>
        public ESyncMode syncMode => _syncMode;

        /// <summary>
        /// 位置阈值
        /// </summary>
        [Name("位置阈值")]
        [Range(0, 0.1f)]
        [FormerlySerializedAs(nameof(positionTheshold))]
        public float _positionTheshold = 0.005f;

        /// <summary>
        /// 位置阈值
        /// </summary>
        public float positionTheshold => _positionTheshold;

        /// <summary>
        /// 位置捕捉阈值
        /// </summary>
        [Name("位置捕捉阈值")]
        [Tip("如果移动更新的目标位置与当前位置超过本值，那么当前对象将直接移动到目标位置，而不做平滑移动；单位：米(默认);", "If the updated target position and the current position exceed this value, the current object will move directly to the target position without smooth movement; Unit: meter (default);")]
        [Range(0.1f, 10)]
        [FormerlySerializedAs(nameof(_positionSnapThreshold))]
        public float _positionSnapThreshold = 5;

        /// <summary>
        /// 位置捕捉阈值
        /// </summary>
        public float positionSnapThreshold => _positionSnapThreshold;

        /// <summary>
        /// 速度阈值
        /// </summary>
        [Name("速度阈值")]
        [Range(0, 0.1f)]
        [FormerlySerializedAs(nameof(velocityTheshold))]
        public float _velocityTheshold = 0.001f;

        /// <summary>
        /// 速度阈值
        /// </summary>
        public float velocityTheshold => _velocityTheshold;

        /// <summary>
        /// 角度阈值
        /// </summary>
        [Name("角度阈值")]
        [Tip("单位：度", "Unit: degrees")]
        [Range(0, 1f)]
        [FormerlySerializedAs(nameof(angleTheshold))]
        public float _angleTheshold = 0.1f;

        /// <summary>
        /// 角度阈值
        /// </summary>
        public float angleTheshold => _angleTheshold;

        /// <summary>
        /// 角度捕捉阈值
        /// </summary>
        [Name("角度捕捉阈值")]
        [Tip("如果旋转更新的目标角度与当前角度超过本值，那么当前对象将直接旋转到目标角度，而不做平滑旋转；单位：度;", "If the target angle updated by rotation and the current angle exceed this value, the current object will rotate directly to the target angle without smooth rotation; Unit: degree;")]
        [Range(1f, 90f)]
        [FormerlySerializedAs(nameof(angleSnapThreshold))]
        public float _angleSnapThreshold = 30f;

        /// <summary>
        /// 角度捕捉阈值
        /// </summary>
        public float angleSnapThreshold => _angleSnapThreshold;

        /// <summary>
        /// 角速度阈值
        /// </summary>
        [Name("角速度阈值")]
        [Range(0, 0.1f)]
        [FormerlySerializedAs(nameof(angularVelocityTheshold))]
        public float _angularVelocityTheshold = 0.01f;

        /// <summary>
        /// 角速度阈值
        /// </summary>
        public float angularVelocityTheshold => _angularVelocityTheshold;

        /// <summary>
        /// 缩放阈值
        /// </summary>
        [Name("缩放阈值")]
        [Range(0, 0.1f)]
        [FormerlySerializedAs(nameof(scaleTheshold))]
        public float _scaleTheshold = 0.1f;

        /// <summary>
        /// 缩放阈值
        /// </summary>
        public float scaleTheshold => _scaleTheshold;

        /// <summary>
        /// 缩放速度阈值
        /// </summary>
        [Name("缩放速度阈值")]
        [Range(0, 0.1f)]
        [FormerlySerializedAs(nameof(scaleVelocityTheshold))]
        public float _scaleVelocityTheshold = 0.01f;

        /// <summary>
        /// 缩放速度阈值
        /// </summary>
        public float scaleVelocityTheshold => _scaleVelocityTheshold;

        /// <summary>
        /// 数据
        /// </summary>
        [Group("同步数据")]
        [Readonly]
        [Name("数据")]
        [SyncVar(sync = false)]
        public TransformData _data = new TransformData();

        /// <summary>
        /// 目标数据
        /// </summary>
        [Readonly]
        [Name("目标数据")]
        public TransformData _targetData = new TransformData();

        /// <summary>
        /// 上一次数据
        /// </summary>
        [Readonly]
        [Name("上一次数据")]
        public TransformData _prevData = new TransformData();

        /// <summary>
        /// 原始数据
        /// </summary>
        [Readonly]
        [Name("原始数据")]
        public TransformData _originalData = new TransformData();

        /// <summary>
        /// 移动提前系数，用于运动补间时预测移动提前的系数
        /// </summary>
        [Group("其他设置")]
        [Name("移动提前系数")]
        [Tip("用于运动补间时预测游戏对象移动前移的系数", "The coefficient used to predict the movement and forward movement of the game object when making up the gap")]
        [Range(0.01f, 2f)]
        [FormerlySerializedAs(nameof(moveAheadRatio))]
        public float _moveAheadRatio = 0.8f;

        /// <summary>
        /// 移动提前系数
        /// </summary>
        public float moveAheadRatio => _moveAheadRatio;

        /// <summary>
        /// 旋转提前系数，用于运动补间时预测旋转提前的系数
        /// </summary>
        [Name("旋转提前系数")]
        [Tip("用于运动补间时预测游戏对象旋转提前的系数", "The coefficient used to predict the rotation advance of the game object during motion compensation")]
        [Range(0.01f, 2f)]
        [FormerlySerializedAs(nameof(rotateAheadRatio))]
        public float _rotateAheadRatio = 0.8f;

        /// <summary>
        /// 旋转提前系数
        /// </summary>
        public float rotateAheadRatio => _rotateAheadRatio;

        private Transform _transform;
        private Rigidbody _rigidbody;
        private Rigidbody2D _rigidbody2D;
        private CharacterController characterController;

        private void CacheData()
        {
            if (_transform) return;
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>(); ;
            _rigidbody2D = GetComponent<Rigidbody2D>(); ;
            characterController = GetComponent<CharacterController>();
        }

        /// <summary>
        /// 唤醒：初始化
        /// </summary>
        public void Awake()
        {
            CacheData();
        }

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            CacheData();
            base.OnMMOEnterRoomCompleted(result);

            _data.SetData(_transform);
            _prevData = new TransformData();//保证至少同步一次
            _targetData.SetData(_data);
            _originalData.SetData(_data);
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            base.OnMMOExitRoomCompleted();
            _originalData.SetTransform(_transform);
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        public void FixedUpdate()
        {
            if (!MMOHelper.isEnteredRoom || !interpolate) return;
            switch (_syncMode)
            {
                case ESyncMode.Rigidbody:
                    {
                        if (interpolateMove) //平滑移动
                        {
                            var offset = _targetData._position - _rigidbody.position;
                            if (offset.sqrMagnitude >= _positionTheshold * _positionTheshold)
                            {
                                if (_rigidbody.isKinematic)//对于运动学刚体通过移动位置进行补间
                                {
                                    _rigidbody.MovePosition(_rigidbody.position + offset.normalized * _targetData.speed * Time.fixedDeltaTime);
                                }
                                else//对于非运动学刚体通过设置速度进行补间
                                {
                                    _rigidbody.velocity = _targetData._velocity;
                                }

                                _targetData._position += _targetData._velocity * Time.fixedDeltaTime * _moveAheadRatio;
                            }
                            else
                            {
                                interpolateMove = false;
                            }
                        }

                        if (interpolateRotate)//平滑旋转
                        {
                            var targetRotation = _targetData.rotation;
                            var angle = Quaternion.Angle(targetRotation, _rigidbody.rotation);
                            if (angle >= _angleTheshold)
                            {
                                _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, Time.fixedDeltaTime / intervalTime));
                                //_rigidbody.angularVelocity = targetData.angularVelocity;
                                _targetData._eulerAngles += _targetData._angularVelocity * Mathf.Rad2Deg * Time.fixedDeltaTime * _rotateAheadRatio;
                            }
                            else
                            {
                                interpolateRotate = false;
                            }
                        }
                        break;
                    }
                case ESyncMode.Rigidbody2D:
                    {
                        if (interpolateMove) //平滑移动
                        {
                            var offset = (Vector2)_targetData._position - _rigidbody2D.position;
                            if (offset.sqrMagnitude >= _positionTheshold * _positionTheshold)
                            {
                                if (_rigidbody2D.isKinematic)//对于运动学2D刚体通过移动位置进行补间
                                {
                                    _rigidbody2D.MovePosition(_rigidbody2D.position + offset.normalized * _targetData.speed * Time.fixedDeltaTime);
                                }
                                else//对于非运动学2D刚体通过设置速度进行补间
                                {
                                    _rigidbody2D.velocity = _targetData._velocity;
                                }
                                _targetData._position += _targetData._velocity * Time.fixedDeltaTime * _moveAheadRatio;
                            }
                            else
                            {
                                interpolateMove = false;
                            }
                        }

                        if (interpolateRotate)//平滑旋转
                        {
                            var angle = Math.Abs(_rigidbody2D.rotation - _targetData._eulerAngles.z);
                            if (angle >= _angleTheshold)
                            {
                                _rigidbody2D.MoveRotation(Mathf.LerpAngle(_rigidbody2D.rotation, _targetData._eulerAngles.z, Time.fixedDeltaTime / intervalTime));
                                _targetData._eulerAngles.z += _targetData._angularVelocity.z * Time.fixedDeltaTime * _rotateAheadRatio;
                            }
                            else
                            {
                                interpolateRotate = false;
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!MMOHelper.isEnteredRoom || !interpolate) return;
            switch (_syncMode)
            {
                case ESyncMode.Transform:
                    {
                        if (interpolateMove) //平滑移动
                        {
                            var offset = _targetData._position - _transform.position;
                            if (offset.sqrMagnitude >= _positionTheshold * _positionTheshold)
                            {
                                _transform.position += offset.normalized * _targetData.speed * Time.deltaTime;

                                _targetData._position += _targetData._velocity * Time.deltaTime * _moveAheadRatio;
                            }
                            else
                            {
                                interpolateMove = false;
                            }
                        }

                        if (interpolateRotate)//平滑旋转
                        {
                            var targetRotation = _targetData.rotation;
                            var angle = Quaternion.Angle(targetRotation, _transform.rotation);
                            if (angle >= _angleTheshold)
                            {
                                _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.deltaTime / intervalTime);
                                _targetData._eulerAngles += _targetData._angularVelocity * Time.deltaTime * _rotateAheadRatio;
                            }
                            else
                            {
                                interpolateRotate = false;
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            intervalTime = 0.05f;
        }

        bool interpolateMove = false;
        bool interpolateRotate = false;
        bool interpolate
        {
            get => interpolateMove || interpolateRotate;
            set
            {
                interpolateMove = value;
                interpolateRotate = value;
            }
        }

        private void HandleInterpolateOnTransform()
        {
            if (!interpolate)//不需要平滑,直接使数据保持一致
            {
                _data.SetData(_transform);
                _prevData.SetData(_data);
            }
        }

        HashSet<int> waitReceive = new HashSet<int>();

        /// <summary>
        /// 当定时检查修改
        /// </summary>
        /// <returns></returns>
        protected override bool OnTimedCheckChange()
        {
            if (interpolate || waitReceive.Contains(version)) return false;

            return _data.SetData(_syncMode, _transform, _rigidbody, _rigidbody2D, characterController) && HasChange(_prevData, _data);
        }

        /// <summary>
        /// 当序列化数据
        /// </summary>
        /// <returns></returns>
        protected override string OnSerializeData()
        {
            waitReceive.Add(version);
            return _data.ToJson();
        }

        /// <summary>
        /// 当反序列化数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataObj"></param>
        protected override void OnDeserializeData(string data, Data dataObj)
        {
            if (JsonHelper.ToObject<TransformData>(data) is TransformData transformData)
            {
                waitReceive.RemoveWhere(i => i <= dataObj.version);
                if (this.IsLocalUserSended(dataObj))//当前用户发送的数据不需要补间处理
                {
                    interpolate = false;
                    if (!HasChange(this._data, transformData))
                    {
                        _prevData.SetData(this._data);
                    }
                    //Log.DebugFormat("1 == 接收=={0}=={1}", name, data);
                    return;
                }

                //Log.DebugFormat("接收=={0}=={1}", name, data);
                _targetData = transformData;
                switch (_syncMode)
                {
                    case ESyncMode.Transform:
                        {
                            if (!_transform)
                            {
                                interpolate = false;
                                return;
                            }

                            //平滑移动分析
                            {
                                float sqrMagnitude = (_targetData._position - _transform.position).sqrMagnitude;
                                if (sqrMagnitude > _positionSnapThreshold * _positionSnapThreshold || _targetData._velocity == Vector3.zero)
                                {
                                    _transform.position = _targetData._position;
                                    interpolateMove = false;
                                }
                                else
                                {
                                    interpolateMove = true;
                                    _targetData.SetSpeed(Mathf.Sqrt(sqrMagnitude), intervalTime);
                                }
                            }

                            //平滑旋转分析
                            {
                                var targetRotation = _targetData.rotation;
                                var angle = Quaternion.Angle(_transform.rotation, targetRotation);
                                if (angle > _angleSnapThreshold || _targetData._angularVelocity == Vector3.zero)
                                {
                                    _transform.rotation = targetRotation;
                                    interpolateRotate = false;
                                }
                                else
                                {
                                    interpolateRotate = true;
                                }
                            }

                            _transform.localScale = _targetData._localScale;
                            HandleInterpolateOnTransform();
                            break;
                        }
                    case ESyncMode.Rigidbody:
                        {
                            if (!_rigidbody)
                            {
                                interpolate = false;
                                return;
                            }

                            //平滑移动分析
                            {
                                float sqrMagnitude = (_rigidbody.position - _targetData._position).sqrMagnitude;
                                if (sqrMagnitude > _positionSnapThreshold || _targetData._velocity == Vector3.zero)
                                {
                                    _rigidbody.position = _targetData._position;
                                    _rigidbody.velocity = _targetData._velocity;
                                    interpolateMove = false;
                                }
                                else
                                {
                                    interpolateMove = true;
                                    _targetData.SetSpeed(Mathf.Sqrt(sqrMagnitude), intervalTime);

                                    if (MathX.ApproximatelyZero(_targetData._velocity.y))//纵向速度(Y)为0时，强制设置纵坐标(Y)同步
                                    {
                                        var pos = _rigidbody.position;
                                        _rigidbody.position = new Vector3(pos.x, _targetData._position.y, pos.z);
                                    }
                                }
                            }

                            //平滑旋转分析
                            {
                                var targetRotation = _targetData.rotation;
                                var angle = Quaternion.Angle(_rigidbody.rotation, targetRotation);
                                if (angle > _angleSnapThreshold || _targetData._angularVelocity == Vector3.zero)
                                {
                                    _rigidbody.rotation = targetRotation;
                                    _rigidbody.angularVelocity = _targetData._angularVelocity;
                                    interpolateRotate = false;
                                }
                                else
                                {
                                    interpolateRotate = true;
                                    _rigidbody.angularVelocity = _targetData._angularVelocity;
                                }
                            }

                            _transform.localScale = _targetData._localScale;

                            //_data.SetRigidbody(_rigidbody, _transform);
                            break;
                        }
                    case ESyncMode.Rigidbody2D:
                        {
                            if (!_rigidbody2D)
                            {
                                interpolate = false;
                                return;
                            }

                            //平滑移动分析
                            {
                                float sqrMagnitude = (_rigidbody2D.position - (Vector2)_targetData._position).sqrMagnitude;
                                if (sqrMagnitude > _positionSnapThreshold || _targetData._velocity == Vector3.zero)
                                {
                                    _rigidbody2D.position = _targetData._position;
                                    _rigidbody2D.velocity = _targetData._velocity;
                                    interpolateMove = false;
                                }
                                else
                                {
                                    interpolateMove = true;
                                    _targetData.SetSpeed(Mathf.Sqrt(sqrMagnitude), intervalTime);
                                }
                            }

                            //平滑旋转分析
                            {
                                var angle = Math.Abs(_rigidbody2D.rotation - _targetData._eulerAngles.z);
                                if (angle > _angleSnapThreshold || Mathf.Approximately(_targetData._angularVelocity.z, 0))
                                {
                                    _rigidbody2D.rotation = _targetData._eulerAngles.z;
                                    _rigidbody2D.angularVelocity = _targetData._angularVelocity.z;
                                    interpolateRotate = false;
                                }
                                else
                                {
                                    interpolateRotate = true;
                                }
                            }

                            _transform.localScale = _targetData._localScale;

                            //_data.SetRigidbody2D(_rigidbody2D, _transform);
                            break;
                        }
                    case ESyncMode.CharacterController:
                        {
                            if (!characterController) return;
                            transformData.SetCharacterController(characterController, _transform);
                            break;
                        }
                }
            }
            else
            {
                Log.WarningFormat("对象[{0}]执行[{1}]反序列化数据[{2}]时失败！", CommonFun.GameObjectComponentToString(this), nameof(OnDeserializeData), data);
            }
        }

        private bool HasChange(TransformData lData, TransformData rData)
        {
            if (lData == rData) return false;

            if ((lData._position - rData._position).sqrMagnitude >= _positionTheshold * _positionTheshold) return true;
            if ((lData._velocity - rData._velocity).sqrMagnitude >= _velocityTheshold * _velocityTheshold) return true;

            if (Quaternion.Angle(Quaternion.Euler(lData._eulerAngles), Quaternion.Euler(rData._eulerAngles)) >= _angleTheshold) return true;
            if ((lData._angularVelocity - rData._angularVelocity).sqrMagnitude >= _angularVelocityTheshold * _angularVelocityTheshold) return true;

            if ((lData._localScale - rData._localScale).sqrMagnitude >= _scaleTheshold * _scaleTheshold) return true;
            if ((lData._scaleVelocity - rData._scaleVelocity).sqrMagnitude >= _scaleVelocityTheshold * _scaleVelocityTheshold) return true;

            return false;
        }

        /// <summary>
        /// 检查当前游戏对象上的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void CheckComponent<T>() where T : Component
        {
            if (!GetComponent<T>())
            {
                Log.WarningFormat("游戏对象[{0}]上没有组件[{1}]({2}),无法设置[{3}]({4})类型的同步模式！",
                    CommonFun.GameObjectToString(this.gameObject),
                    CommonFun.Name(typeof(T)),
                    typeof(T).FullName,
                    CommonFun.Name(_syncMode),
                    _syncMode.ToString());
                _syncMode = ESyncMode.Transform;
            }
        }

        /// <summary>
        /// 当有效性
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            switch (_syncMode)
            {
                case ESyncMode.Rigidbody:
                    {
                        CheckComponent<Rigidbody>();
                        break;
                    }
                case ESyncMode.Rigidbody2D:
                    {
                        CheckComponent<Rigidbody2D>();
                        break;
                    }
                case ESyncMode.CharacterController:
                    {
                        CheckComponent<CharacterController>();
                        break;
                    }
            }
        }

        /// <summary>
        /// 变换数据
        /// </summary>
        [Name("变换数据")]
        [Serializable]
        [Import]
        public class TransformData : JsonObject<TransformData>
        {
            /// <summary>
            /// 位置
            /// </summary>
            [Json(exportString = true)]
            [Name("位置")]
            [Readonly]
            public Vector3 _position;

            /// <summary>
            /// 速度
            /// </summary>
            [Json(exportString = true)]
            [Name("速度")]
            [Readonly]
            public Vector3 _velocity;

            internal float speed;
            internal void SetSpeed(float distance, float time)
            {
                speed = _velocity.magnitude;
                if (Mathf.Approximately(speed, 0))
                {
                    speed = distance / time;
                }
            }

            /// <summary>
            /// 角度
            /// </summary>
            [Json(exportString = true)]
            [Name("角度")]
            [Tip("欧拉角度，单位：度", "Euler angle in degrees")]
            [Readonly]
            public Vector3 _eulerAngles;

            internal Quaternion rotation => Quaternion.Euler(_eulerAngles);

            /// <summary>
            /// 角速度
            /// </summary>
            [Json(exportString = true)]
            [Name("角速度")]
            [Tip("欧拉角速度，默认单位：度/秒 ；同步类型为刚体时,单位:弧度/秒；", "Euler angular velocity, default unit: degrees / second; When the synchronization type is rigid body, the unit is radian / S;")]
            [Readonly]
            public Vector3 _angularVelocity;

            /// <summary>
            /// 本地缩放
            /// </summary>
            [Json(exportString = true)]
            [Name("本地缩放")]
            [Readonly]
            public Vector3 _localScale;

            /// <summary>
            /// 缩放速度
            /// </summary>
            [Json(exportString = true)]
            [Name("缩放速度")]
            [Readonly]
            public Vector3 _scaleVelocity;

            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="data"></param>
            public void SetData(TransformData data)
            {
                this._position = data._position;
                this._velocity = data._velocity;

                this._eulerAngles = data._eulerAngles;
                this._angularVelocity = data._angularVelocity;

                this._localScale = data._localScale;
                this._scaleVelocity = data._scaleVelocity;
            }

            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="syncMode"></param>
            /// <param name="transform"></param>
            /// <param name="rigidbody"></param>
            /// <param name="rigidbody2D"></param>
            /// <param name="characterController"></param>
            /// <returns></returns>
            public bool SetData(ESyncMode syncMode, Transform transform, Rigidbody rigidbody, Rigidbody2D rigidbody2D, CharacterController characterController)
            {
                switch (syncMode)
                {
                    case ESyncMode.Transform:
                        {
                            SetData(transform);
                            break;
                        }
                    case ESyncMode.Rigidbody:
                        {
                            if (!rigidbody) return false;
                            SetData(rigidbody, transform);
                            break;
                        }
                    case ESyncMode.Rigidbody2D:
                        {
                            if (!rigidbody2D) return false;
                            SetData(rigidbody2D, transform);
                            break;
                        }
                    case ESyncMode.CharacterController:
                        {
                            if (!characterController) return false;
                            SetData(characterController, transform);
                            break;
                        }
                    case ESyncMode.None:
                    default:
                        {
                            return false;
                        }
                }
                return true;
            }

            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="transform"></param>
            public void SetData(Transform transform)
            {
                this._position = transform.position;
                //this.velocity = Vector3.zero;

                this._eulerAngles = transform.eulerAngles;
                //this.angularVelocity = Vector3.zero;

                this._localScale = transform.localScale;
                //this.scaleVelocity = Vector3.zero;
            }

            /// <summary>
            /// 设置变换
            /// </summary>
            /// <param name="transform"></param>
            public void SetTransform(Transform transform)
            {
                if (!transform) return;

                transform.position = this._position;

                transform.eulerAngles = this._eulerAngles;

                transform.localScale = this._localScale;
            }

            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="rigidbody"></param>
            /// <param name="transform"></param>
            public void SetData(Rigidbody rigidbody, Transform transform)
            {
                this._position = rigidbody.position;
                this._velocity = rigidbody.velocity;

                this._eulerAngles = rigidbody.rotation.eulerAngles;
                this._angularVelocity = rigidbody.angularVelocity;

                this._localScale = transform.localScale;
            }

            /// <summary>
            /// 设置刚体
            /// </summary>
            /// <param name="rigidbody"></param>
            /// <param name="transform"></param>
            public void SetRigidbody(Rigidbody rigidbody, Transform transform)
            {
                rigidbody.MovePosition(this._position);
                rigidbody.velocity = this._velocity;

                rigidbody.MoveRotation(Quaternion.Euler(_eulerAngles));
                rigidbody.angularVelocity = this._angularVelocity;

                transform.localScale = this._localScale;
            }

            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="rigidbody2D"></param>
            /// <param name="transform"></param>
            public void SetData(Rigidbody2D rigidbody2D, Transform transform)
            {
                this._position = rigidbody2D.position;
                this._velocity = rigidbody2D.velocity;

                this._eulerAngles = new Vector3(0, 0, rigidbody2D.rotation);
                this._angularVelocity = new Vector3(0, 0, rigidbody2D.angularVelocity);

                this._localScale = transform.localScale;
            }

            /// <summary>
            /// 设置刚体2D
            /// </summary>
            /// <param name="rigidbody2D"></param>
            /// <param name="transform"></param>
            public void SetRigidbody2D(Rigidbody2D rigidbody2D, Transform transform)
            {
                transform.position = this._position;
                rigidbody2D.velocity = this._velocity;

                rigidbody2D.MoveRotation(this._eulerAngles.z);
                rigidbody2D.angularVelocity = this._angularVelocity.z;

                transform.localScale = this._localScale;
            }

            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="characterController"></param>
            /// <param name="transform"></param>
            public void SetData(CharacterController characterController, Transform transform)
            {
                SetData(transform);
            }

            /// <summary>
            /// 设置角色控制器
            /// </summary>
            /// <param name="characterController"></param>
            /// <param name="transform"></param>
            public void SetCharacterController(CharacterController characterController, Transform transform)
            {
                SetTransform(transform);
            }
        }

        /// <summary>
        /// 同步模式
        /// </summary>
        [Name("同步模式")]
        public enum ESyncMode
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 变换
            /// </summary>
            [Name("变换")]
            Transform,

            /// <summary>
            /// 刚体
            /// </summary>
            [Name("刚体")]
            Rigidbody,

            /// <summary>
            /// 2D刚体
            /// </summary>
            [Name("2D刚体")]
            Rigidbody2D,

            /// <summary>
            /// 角色控制器
            /// </summary>
            [Name("角色控制器")]
            [Tip("目前对角色控制器同步模式支持不友好", "Currently, it is unfriendly to support role controller synchronization mode")]
            CharacterController,
        }
    }
}
