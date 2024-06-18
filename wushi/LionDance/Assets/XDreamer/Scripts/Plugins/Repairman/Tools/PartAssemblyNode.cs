using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginRepairman.States;

namespace XCSJ.PluginRepairman.Tools
{
    /// <summary>
    /// 零件装配节点:
    /// 1、零件装配节点由其父级【模块】创建，用于记录零件在模型下的姿态和标签信息
    /// 2、节点中存储着零件在模块下的位置与角度，为本地坐标。
    /// 3、零件世界坐标需要将本地坐标+其所属模块的坐标
    /// 4、零件装配节点存储装配约束信息。（装配约束节点和拆卸约束节点）
    /// </summary>
    public class PartAssemblyNode
    {
        /// <summary>
        /// 在设备完全装配好的情况下指向的零件原型对象
        /// </summary>
        [Name("零件原型")]
        [DynamicLabel]
        [Readonly(EEditorMode.Runtime)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Part _partPrototype;

        /// <summary>
        /// 直接父级：当零件重新装配到模块或设备中时，设定当前对象为其父级
        /// </summary>
        [Name("直接父级")]
        [DynamicLabel]
        public Transform _directParent = null;

        /// <summary>
        /// 本地位置
        /// </summary>
        [Name("本地位置")]
        [SerializeField]
        [HideInSuperInspector]
        private Vector3 _localPosition = Vector3.zero;

        /// <summary>
        /// 本地旋转
        /// </summary>
        [Name("本地旋转")]
        [SerializeField]
        [HideInSuperInspector]
        private Quaternion _localRotation = Quaternion.identity;

        /// <summary>
        /// 吸附距离
        /// </summary>
        [Name("吸附距离")]
        [Min(0)]
        public float _snapDistance = 1;

        /// <summary>
        /// 姿态:世界
        /// </summary>
        public Pose pose => new Pose(position, rotation);

        /// <summary>
        /// 位置:世界
        /// </summary>
        public Vector3 position => parent.position + _localPosition;

        /// <summary>
        /// 旋转：世界
        /// </summary>
        public Quaternion rotation => parent.rotation * _localRotation;

        private Transform parent => _directParent ? _directParent : _module.transform;

        /// <summary>
        /// 已装配到插槽上的零件对象
        /// </summary>
        public Part assembledPart
        {
            get => _assembledPart;
            set
            {
                if (_assembledPart != value)
                {
                    if (_assembledPart)
                    {
                        _assembledPart.partAssemblyNode = null;
                        _assembledPart.assembleState = EAssembleState.Disassembled;

                        if (!_directParent)
                        {
                            _directParent = _assembledPart.transform.parent;
                        }
                        if (deviceOfModule)
                        {
                            deviceOfModule.AddDisassembledPart(_assembledPart);
                        }
                    }

                    _assembledPart = value;

                    if (_assembledPart)
                    {
                        _assembledPart.partAssemblyNode = this;
                        _assembledPart.assembleState = EAssembleState.Assembled;

                        if (deviceOfModule)
                        {
                            deviceOfModule.RemoveDisassembledPart(_assembledPart);
                        }
                        _assembledPart.transform.SetParent(parent);
                    }
                }
            }
        }

        /// <summary>
        /// 已装配零件
        /// </summary>
        [Name("已装配零件")]
        [Readonly]
        [DynamicLabel]
        public Part _assembledPart;

        /// <summary>
        /// 装配态
        /// </summary>
        public bool full => assembledPart;

        /// <summary>
        /// 拆卸态
        /// </summary>
        public bool empty => !full;

        /// <summary>
        /// 所属模块
        /// </summary>
        [Name("所属模块")]
        [Readonly]
        [DynamicLabel]
        public Module _module;

        private Device deviceOfModule
        {
            get
            {
                if (!_deviceOfModule && _module)
                {
                    _deviceOfModule = _module.GetComponentInParent<Device>();
                }
                return _deviceOfModule;
            }
        }
        private Device _deviceOfModule;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PartAssemblyNode() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="part"></param>
        public PartAssemblyNode(Part part)
        {
            _partPrototype = part;
            _directParent = _partPrototype.transform.parent;
            _localPosition = part.transform.localPosition;
            _localRotation = part.transform.localRotation;

            if (CommonFun.GetBounds(out var bounds, part.gameObject))
            {
                _snapDistance = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            }
        }

        /// <summary>
        /// 标签相同
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool ExistSameRepacePartTag(Part part) => _partPrototype && _partPrototype.IsMatchRepacePartTag(part);

        /// <summary>
        /// 获取零件到当前存储姿态的距离
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public float GetDistance(Part part) => part ? Vector3.Distance(position, part.transform.position) : 0;

        /// <summary>
        /// 在零件吸附距离内
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool InSnapDistance(Part part) => part && GetDistance(part) <= _snapDistance;

        /// <summary>
        /// 尝试吸附
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool TrySnap(Part part)
        {
            if (empty && InSnapDistance(part))
            {
                assembledPart = part;
                SyncPartPose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新零件状态
        /// </summary>
        public void UpdatePartState()
        {
            if (InSnapDistance(assembledPart))
            {
                SyncPartPose();
            }
            else
            {
                assembledPart = null;
            }
        }

        private void SyncPartPose()
        {
            if (assembledPart.transform.parent == parent)
            {
                assembledPart.transform.localPosition = _localPosition;
                assembledPart.transform.localRotation = _localRotation;
            }
            else
            {
                assembledPart.transform.position = position;
                assembledPart.transform.rotation = rotation;
            }
        }

        /// <summary>
        /// 添加装配约束节点
        /// </summary>
        /// <param name="partAssemblyNode"></param>
        public void AddAssemblyConstraintNode(PartAssemblyNode partAssemblyNode)
        {
            if (partAssemblyNode == null) return;

            assemblyConstraintNodes.AddWithDistinct(partAssemblyNode);
            partAssemblyNode.disassemblyConstraintNodes.AddWithDistinct(this);
        }

        /// <summary>
        /// 移出装配约束节点
        /// </summary>
        /// <param name="partAssemblyNode"></param>
        public void RemoveAssemblyConstraintNode(PartAssemblyNode partAssemblyNode)
        {
            if (partAssemblyNode == null) return;

            assemblyConstraintNodes.Remove(partAssemblyNode);
            partAssemblyNode.disassemblyConstraintNodes.Remove(this);
        }

        /// <summary>
        /// 能否执行装配：只有【装配约束节点列表】内所有节点的状态都为装配态，当前节点才能进行装配
        /// </summary>
        public bool canAssembly => empty && assemblyConstraintNodes.All(n => n.full);

        /// <summary>
        /// 装配约束节点列表：只有【装配约束节点列表】内所有节点的状态都为装配态，当前节点才能进行装配
        /// </summary>
        public List<PartAssemblyNode> assemblyConstraintNodes { get; private set; } = new List<PartAssemblyNode>();

        /// <summary>
        /// 能否拆卸：只有【拆卸约束节点列表】内所有节点的状态都为拆卸，当前节点才能允许执行拆卸
        /// </summary>
        public bool canDisassembly => full && disassemblyConstraintNodes.All(p => p.empty);

        /// <summary>
        /// 拆卸约束节点列表：只有【拆卸约束节点列表】内所有节点的状态都为拆卸，当前节点才能允许执行拆卸
        /// </summary>
        public List<PartAssemblyNode> disassemblyConstraintNodes { get; private set; } = new List<PartAssemblyNode>();
    }

    /// <summary>
    /// 可序列化零件装配节点
    /// </summary>
    [Serializable]
    public class SerializablePartAssemblyNode : PartAssemblyNode
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SerializablePartAssemblyNode() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="part"></param>
        public SerializablePartAssemblyNode(Part part) : base(part) { }
    }
}
