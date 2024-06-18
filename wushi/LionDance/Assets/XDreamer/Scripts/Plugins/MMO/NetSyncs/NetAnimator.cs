using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络动画
    /// </summary>
    [Attributes.Icon(EIcon.Animator)]
    [DisallowMultipleComponent]
    [Name("网络动画")]
    [Tool(MMOHelperExtension.Title, nameof(NetIdentity), rootType = typeof(MMOManager))]
    [RequireComponent(typeof(Animator))]
    public sealed class NetAnimator : NetMB
    {
        /// <summary>
        /// 动画
        /// </summary>
        [Group("动画")]
        [Name("动画")]
        [SerializeField]
        private Animator _animator;

        /// <summary>
        /// 动画
        /// </summary>
        public Animator animator
        {
            get
            {
                if (!_animator)
                {
                    _animator = GetComponent<Animator>();
                }
                return _animator;
            }
            set => _animator = value;
        }

        /// <summary>
        /// 数据
        /// </summary>
        [Name("数据")]
        [SyncVar(sync = false)]
        public AnimatorData _data = new AnimatorData();

        /// <summary>
        /// 目标数据
        /// </summary>
        [Readonly]
        [Name("目标数据")]
        public AnimatorData _targetData = new AnimatorData();

        /// <summary>
        /// 上一次数据
        /// </summary>
        [Readonly]
        [Name("上一次数据")]
        public AnimatorData _prevData = new AnimatorData();

        /// <summary>
        /// 原始数据
        /// </summary>
        [Readonly]
        [Name("原始数据")]
        public AnimatorData _originalData = new AnimatorData();

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            base.OnMMOEnterRoomCompleted(result);
            if (result == EACode.Success && _data.SetData(animator))
            {
                _prevData = new AnimatorData();//保证至少同步一次
                _targetData.SetData(_data);
                _originalData.SetData(_data);
            }
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            base.OnMMOExitRoomCompleted();
            if (!animator) return;
            _originalData.SetAnimator(animator);
        }

        HashSet<int> waitReceive = new HashSet<int>();

        /// <summary>
        /// 当定时检查修改
        /// </summary>
        /// <returns></returns>
        protected override bool OnTimedCheckChange()
        {
            if (waitReceive.Contains(version)) return false;
            return _data.SetData(animator) && HasChange(_prevData, _data);
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
            if (animator && AnimatorData.FromJson(data) is AnimatorData animatorData)
            {
                waitReceive.RemoveWhere(i => i <= dataObj.version);
                if (this.IsLocalUserSended(dataObj))
                {
                    if (!HasChange(this._data, animatorData))
                    {
                        _prevData.SetData(this._data);
                    }
                    return;
                }
                Debug.LogFormat("{0}->{1}", dataObj.userGuid, transform.parent.name);

                _targetData = animatorData;

                _targetData.SetAnimator(animator);
            }
        }

        private bool HasChange(AnimatorData lData, AnimatorData rData)
        {
            if (lData == rData) return false;
            if (lData.parameters.Count != rData.parameters.Count) return true;

            for (int i = 0; i < lData.parameters.Count; i++)
            {
                if (lData.parameters[i].HasChange(rData.parameters[i])) return true;
            }

            return false;
        }

        /// <summary>
        /// 动画数据
        /// </summary>
        [Name("动画数据")]
        [Serializable]
        public class AnimatorData : JsonObject<AnimatorData>
        {
            /// <summary>
            /// 参数列表
            /// </summary>
            [Name("参数列表")]
            [ArrayElement(EArrayElementHandleRule.CanDelete)]
            public List<Parameter> parameters = new List<Parameter>();

            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="data"></param>
            public void SetData(AnimatorData data)
            {
                parameters.Clear();
                foreach (var p in data.parameters)
                {
                    this.parameters.Add(new Parameter(p));
                }
            }

            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="animator"></param>
            /// <returns></returns>
            public bool SetData(Animator animator)
            {
                //Debug.LogWarning("animator:" + CommonFun.GameObjectToString(animator.gameObject) + "==" + animator.isActiveAndEnabled);
                if (!animator || !animator.isActiveAndEnabled) return false;

                foreach (var p in parameters)
                {
                    p.SetValue(animator);
                }

                return true;
            }

            /// <summary>
            /// 设置动画
            /// </summary>
            /// <param name="animator"></param>
            public void SetAnimator(Animator animator)
            {
                if (!animator || !animator.isActiveAndEnabled) return;
                foreach (var p in parameters)
                {
                    p.SetAnimator(animator);
                }
            }
        }

        /// <summary>
        /// 参数
        /// </summary>
        [Name("参数")]
        [Serializable]
        [Import]
        public class Parameter
        {
            /// <summary>
            /// 类型
            /// </summary>
            [Readonly]
            [Name("类型")]
            public AnimatorControllerParameterType _type;

            /// <summary>
            /// 名称
            /// </summary>
            [Readonly]
            [Name("名称")]
            public string _name;

            /// <summary>
            /// 值
            /// </summary>
            [Readonly]
            [Name("值")]
            public string _value;

            /// <summary>
            /// 布尔值
            /// </summary>
            [Json(false)]
            public bool boolValue => Converter.instance.ConvertTo<bool>(_value);

            /// <summary>
            /// 整型值
            /// </summary>
            [Json(false)]
            public int intValue => Converter.instance.ConvertTo<int>(_value);

            /// <summary>
            /// 浮点值
            /// </summary>
            [Json(false)]
            public float floatValue => Converter.instance.ConvertTo<float>(_value);

            /// <summary>
            /// 构造
            /// </summary>
            public Parameter() { }

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="parameter"></param>
            public Parameter(Parameter parameter)
            {
                SetValue(parameter);
            }

            /// <summary>
            /// 设置值
            /// </summary>
            /// <param name="parameter"></param>
            public void SetValue(Parameter parameter)
            {
                this._type = parameter._type;
                this._name = parameter._name;
                this._value = parameter._value;
            }

            /// <summary>
            /// 设置值
            /// </summary>
            /// <param name="animator"></param>
            public void SetValue(Animator animator)
            {
                switch (_type)
                {
                    case AnimatorControllerParameterType.Bool:
                        {
                            _value = animator.GetBool(_name).ToString();
                            break;
                        }
                    case AnimatorControllerParameterType.Int:
                        {
                            _value = animator.GetInteger(_name).ToString();
                            break;
                        }
                    case AnimatorControllerParameterType.Float:
                        {
                            _value = animator.GetFloat(_name).ToString();
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException("不支持" + _type.ToString() + "参数类型");
                        }
                }
            }

            /// <summary>
            /// 设置动画
            /// </summary>
            /// <param name="animator"></param>
            public void SetAnimator(Animator animator)
            {
                switch (_type)
                {
                    case AnimatorControllerParameterType.Bool:
                        {
                            animator.SetBool(_name, boolValue);
                            break;
                        }
                    case AnimatorControllerParameterType.Int:
                        {
                            animator.SetBool(_name, boolValue);
                            break;
                        }
                    case AnimatorControllerParameterType.Float:
                        {
                            animator.SetBool(_name, boolValue);
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException("不支持" + _type.ToString() + "参数类型");
                        }
                }
            }

            /// <summary>
            /// 有修改
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public bool HasChange(Parameter parameter)
            {
                if (this == parameter) return false;
                if (this._type != parameter._type) return true;
                if (this._name != parameter._name) return true;
                if (this._value != parameter._value) return true;
                return false;
            }
        }
    }
}
