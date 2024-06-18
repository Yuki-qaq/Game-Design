using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.PropertyDatas;

namespace XCSJ.PluginRepairman.Tools
{
    /// <summary>
    /// 模块：
    /// 1、零件的集合
    ///    1.1、零件不为实体零件，而是零件分类名称信息
    ///    1.2、记录零件插槽信息。包括在设备空间下的位置与旋转信息（本地）和零件运动约束（移动或旋转）
    ///    1.3、零件必须以模块为直接父级，否则中间层级也需要转化为一个模块
    /// 2、模块与模块之间可嵌套
    /// 3、记录零件之间的装配约束关系
    /// 4、零件管理。
    ///    4.1、当零件装配到模块中时，零件为模块的子级；当零件从模块中移除时，零件父级变为空（即场景根级）；
    ///    4.2、拖拽模式管理。当点击到模块下的零件运动受限时，拖拽的是整个模块，零件运动不受限时，拖拽的是零件
    ///    4.3、零件在模块中时为装配态；零件不在模块中时为拆卸态（即游离态），并且零件本身不再能查询到模块信息
    /// </summary>
    [Name("模块")]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Engine)]
    public class Module : Part
    {
        /// <summary>
        /// 关联状态组件
        /// </summary>
        [Group("零件设置", textEN = "Part Settings")]
        [Name("关联状态组件")]
        [StateComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public States.Module _moduleSC = null;

        /// <summary>
        /// 关联状态组件
        /// </summary>
        public States.Module moduleSC
        {
            get
            {
                if (!_moduleSC || _moduleSC.gameObject != gameObject)
                {
                    _moduleSC = SMSHelper.GetStateComponents<States.Module>().Find(msc => msc.gameObject == gameObject);
                }
                return _moduleSC;
            }
        }

        /// <summary>
        /// 可拖拽
        /// </summary>
        [Name("可拖拽")]
        public bool _canDrag = false;

        /// <summary>
        /// 子模块对象
        /// </summary>
        public List<Module> childModules { get; protected set; } = new List<Module>();

        /// <summary>
        /// 子零件对象
        /// </summary>
        public List<Part> childParts { get; protected set; } = new List<Part>();

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            _partAssemblyNodes.ForEach(ps => ps._module = this);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            InitChildPart();

            AddConstraint();

            CheckPartAssemblyState();
        }

        /// <summary>
        /// 检测零件装配状态
        /// </summary>
        protected virtual void CheckPartAssemblyState() => this.TryAssemblyPart();

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            childModules.Clear();
            childParts.Clear();

            RemoveConstraint();
        }

        /// <summary>
        /// 模块在父级的插槽中位置是匹配的，子级插槽数据也是满的情况下才是装配态
        /// </summary>
        public override EAssembleState assembleState 
        {
            get
            {
                bool allFull = _partAssemblyNodes.All(ps => ps.full);
                if (!allFull)
                {
                    return EAssembleState.Disassembled;
                }
                return base.assembleState;
            }
            internal set => base.assembleState = value; 
        }

        /// <summary>
        /// 能否拖拽
        /// </summary>
        /// <param name="dragger"></param>
        /// <returns></returns>
        protected override bool CanGrabbed(Dragger dragger) => base.CanGrabbed(dragger) && _canDrag;

        private void InitChildPart()
        {
            foreach (var part in GetComponentsInChildren<Part>(true))
            {
                if (part == this || part.parentModule != this) continue;

                if (part is Module module && module)
                {
                    childModules.Add(module);
                }
                else
                {
                    childParts.Add(part);
                }
            }
        }

        #region 零件装配信息

        /// <summary>
        /// 零件装配节点列表
        /// </summary>
        [Name("零件装配节点列表")]
        public List<SerializablePartAssemblyNode> _partAssemblyNodes = new List<SerializablePartAssemblyNode>();

        /// <summary>
        /// 已装配节点数量（不包含子模型）
        /// </summary>
        public int assembledNodeCount => _partAssemblyNodes.Count(n => n.full);

        /// <summary>
        /// 已装配节点总数量（包含子模块）
        /// </summary>
        public int assembledNodeTotalCount => assembledNodeCount + childModules.Sum(m => m.assembledNodeTotalCount);

        /// <summary>
        /// 已拆卸节点数量（包含子模块）
        /// </summary>
        public int disassembledNodeCount => _partAssemblyNodes.Count(n => n.empty);

        /// <summary>
        /// 已拆卸节点总数量（包含子模块）
        /// </summary>
        public int disassembledNodeTotalCount => disassembledNodeCount + childModules.Sum(m => m.disassembledNodeCount);

        /// <summary>
        /// 获取已装配零件节点列表
        /// </summary>
        /// <returns></returns>
        public List<SerializablePartAssemblyNode> GetAssembledNodes() => _partAssemblyNodes.Where(n => n.full).ToList();

        /// <summary>
        /// 获取已装配零件列表
        /// </summary>
        /// <returns></returns>
        public List<Part> GetAssemblyPart() => GetAssembledNodes().Cast(n => n.assembledPart).ToList();

        /// <summary>
        /// 获取已拆卸零件节点列表
        /// </summary>
        /// <returns></returns>
        public List<SerializablePartAssemblyNode> GetDisassembledNodes() => _partAssemblyNodes.Where(n => n.empty).ToList();

        /// <summary>
        /// 能装配
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool CanAssembly(Part part)
        {
            var ps = GetNearestEmptyPartAssemblyNode(part);
            return ps != null && ps.InSnapDistance(part);
        }

        private SerializablePartAssemblyNode FindNodeByPrototype(Part part) => _partAssemblyNodes.Find(n => n._partPrototype == part);

        /// <summary>
        /// 查找离零件最近空的零件装配节点
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public PartAssemblyNode GetNearestEmptyPartAssemblyNode(Part part)
        {
            if (!part) return null;

            return GetNearestEmptyPartAssemblyNode(part, part.transform.position);
        }

        /// <summary>
        /// 获取空零件装配节点列表
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public List<PartAssemblyNode> GetEmptyPartAssemblyNodes(Part part) => new List<PartAssemblyNode>(_partAssemblyNodes.Where(node => node.empty && node.ExistSameRepacePartTag(part) && node.canAssembly));

        /// <summary>
        /// 获取空的匹配可替换零件标签的零件装配节点
        /// </summary>
        /// <param name="part"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public PartAssemblyNode GetNearestEmptyPartAssemblyNode(Part part, Vector3 position)
        {
            PartAssemblyNode result = null;
            float nearestDistance = Mathf.Infinity;
            foreach (var info in GetEmptyPartAssemblyNodes(part))
            {
                float distance = Vector3.SqrMagnitude(position - info.position);
                if (distance < nearestDistance)
                {
                    result = info;
                    nearestDistance = distance;
                }
            }

            foreach (var m in childModules)
            {
                var info = m.GetNearestEmptyPartAssemblyNode(part, position);
                if (info != null)
                {
                    var distance = Vector3.SqrMagnitude(position - info.position);
                    if (distance < nearestDistance)
                    {
                        result = info;
                        nearestDistance = distance;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 已装配零件
        /// </summary>
        public List<Part> assembledParts => _partAssemblyNodes.Where(n => n.assembledPart).Cast(n => n.assembledPart).ToList();

        /// <summary>
        /// 获取可装配零件节点列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PartAssemblyNode> GetCanAssemblyParts() => _partAssemblyNodes.Where(data => data.canAssembly);

        /// <summary>
        /// 可装配零件列表:这里获取的是零件原型
        /// </summary>
        public List<Part> canAssemblyParts => GetCanAssemblyParts().Cast(n => n._partPrototype).ToList();

        /// <summary>
        /// 获取可自由拆卸零件节点列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PartAssemblyNode> GetCanDisassemblyParts() => _partAssemblyNodes.Where(data => data.canDisassembly);

        /// <summary>
        /// 可拆卸零件列表
        /// </summary>
        public List<Part> canDisassemblyParts => GetCanDisassemblyParts().Cast(n => n.assembledPart).ToList();

        #endregion

        #region 零件装配约束

        /// <summary>
        /// 零件装配约束数据：装配约束关系在启用后会将信息转移值装配节点上。当前信息只做序列化使用
        /// </summary>
        [Name("零件装配约束数据")]
        public List<PartAssemblyConstraint> _partAssemblyConstraints = new List<PartAssemblyConstraint>();

        /// <summary>
        /// 零件装配约束数据
        /// </summary>
        public List<PartAssemblyConstraint> partAssemblyConstraints => _partAssemblyConstraints;

        /// <summary>
        /// 添加装配约束关系：给当前模型中的每个装配节点注入约束关系
        /// </summary>
        protected void AddConstraint()
        {
            foreach (var item in _partAssemblyConstraints)
            {
                var toNode = FindNodeByPrototype(item._toPart);
                if (toNode != null)
                {
                    toNode.AddAssemblyConstraintNode(FindNodeByPrototype(item._fromPart));
                }
            }

            foreach (var m in childModules)
            {
                m.AddConstraint();
            }
        }

        /// <summary>
        /// 移除装配约束关系：从当前模型中的每个装配节点中移除约束关系
        /// </summary>
        protected void RemoveConstraint()
        {
            // 给每个零件对象注入装配约束关系
            foreach (var item in _partAssemblyConstraints)
            {
                var toNode = FindNodeByPrototype(item._toPart);
                if (toNode != null)
                {
                    toNode.RemoveAssemblyConstraintNode(FindNodeByPrototype(item._fromPart));
                }
            }

            foreach (var m in childModules)
            {
                m.RemoveConstraint();
            }
        }

        /// <summary>
        /// 获取装配顺序：并行零件也被串行化
        /// </summary>
        /// <returns>返回零件与装配深度元组列表</returns>
        public List<(PartAssemblyNode, int)> GetAssemblyOrder() => BreadthSearchPart(GetCanAssemblyParts(), true);

        /// <summary>
        /// 获取拆卸顺序：并行零件也被串行化
        /// </summary>
        /// <returns>返回零件与装配深度元组列表</returns>
        public List<(PartAssemblyNode, int)> GetDisassemblyOrder() => BreadthSearchPart(GetCanDisassemblyParts(), false);

        /// <summary>
        /// 广度遍历零件约束关系，输出顺序化结果
        /// </summary>
        /// <param name="inParts"></param>
        /// <param name="isAssembly"></param>
        /// <returns>返回零件与装配深度元组列表</returns>
        protected List<(PartAssemblyNode, int)> BreadthSearchPart(IEnumerable<PartAssemblyNode> inParts, bool isAssembly)
        {
            var result = new List<(PartAssemblyNode, int)>();
            var visitedParts = new HashSet<PartAssemblyNode>();

            var queue = new Queue<(PartAssemblyNode, int)>();
            inParts.Foreach(p => queue.Enqueue((p, 1)));
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentPart = current.Item1;
                var currentDeep = current.Item2;
                if (!visitedParts.Contains(currentPart))
                {
                    visitedParts.Add(currentPart);
                    result.Add((currentPart, currentDeep));

                    List<PartAssemblyNode> searchParts = isAssembly ? currentPart.disassemblyConstraintNodes : currentPart.assemblyConstraintNodes;

                    searchParts.ForEach(p =>
                    {
                        if (!visitedParts.Contains(p))
                        {
                            queue.Enqueue((currentPart, currentDeep + 1));
                        }
                    });
                }
            }

            return result;
        }

        #endregion
    }

    #region 零件装配约束

    /// <summary>
    /// 零件装配约束
    /// </summary>
    [Serializable]
    public class PartAssemblyConstraint
    {
        /// <summary>
        /// 先装配
        /// </summary>
        [Name("先装配")]
        [Readonly(EEditorMode.Runtime)]
        [DynamicLabel]
        public Part _fromPart;

        /// <summary>
        /// 后装配
        /// </summary>
        [Name("后装配")]
        [Readonly(EEditorMode.Runtime)]
        [DynamicLabel]
        public Part _toPart;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PartAssemblyConstraint() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fromPart"></param>
        /// <param name="toPart"></param>
        public PartAssemblyConstraint(Part fromPart, Part toPart)
        {
            _fromPart = fromPart;
            _toPart = toPart;
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (_fromPart && _toPart)
            {
                return _fromPart.GetHashCode() & _toPart.GetHashCode();
            }
            return base.GetHashCode();
        }

        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is PartAssemblyConstraint pac && pac._fromPart == _fromPart && pac._toPart == _toPart;
        }
    }

    #endregion
}
