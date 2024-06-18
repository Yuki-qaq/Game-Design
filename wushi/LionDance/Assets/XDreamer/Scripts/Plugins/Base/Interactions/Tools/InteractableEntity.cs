using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.SelectionUtils;
using XCSJ.Tools;

namespace XCSJ.Extension.Base.Interactions.Tools
{
    /// <summary>
    /// 可交互实体：交互器以其为主入口与可交互组件对象进行交互
    /// </summary>
    [Name("可交互实体")]
    [Tip("交互器以其为主入口与可交互组件对象进行交互", "The interactor interacts with the interactable component object through its main entrance")]
    [XCSJ.Attributes.Icon(EIcon.Model)]
    [DisallowMultipleComponent]
    [Tool("常用", rootType = typeof(ToolsManager))]
    [Tool(ToolsCategory.InteractCommon, rootType = typeof(ToolsManager), index = 0)]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public sealed class InteractableEntity : ExtensionalInteractObject, ITool
    {
        #region 悬停

        /// <summary>
        /// 可悬停
        /// </summary>
        [Group("基础设置", textEN ="Base Settings")]
        [Name("可悬停")]
        public bool _hoverable = true;

        /// <summary>
        /// 能否悬停
        /// </summary>
        public bool canHover => _hoverable;

        /// <summary>
        /// 是否悬停
        /// </summary>
        [Readonly]
        [Name("已悬停")]
        public bool _isHovered = false;

        /// <summary>
        /// 是否悬停
        /// </summary>
        public bool isHovered => _isHovered;

        /// <summary>
        /// 能否悬停
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool CanHoverable(InteractData interactData) => canHover;

        /// <summary>
        /// 尝试悬停
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public bool TryHoverable(InteractData interactData, out EInteractResult interactResult)
        {
            switch (inCmds.GetCmd(interactData.cmdName))
            {
                case nameof(HoverEntry):
                    {
                        interactResult = HoverEntry(interactData);
                        return true;
                    }
                case nameof(HoverExit):
                    {
                        interactResult = HoverExit(interactData);
                        return true;
                    }
                case nameof(HoverStay):
                    {
                        interactResult = HoverStay(interactData);
                        return true;
                    }
            }
            interactResult = EInteractResult.Fail;
            return false;
        }

        /// <summary>
        /// 悬停进入
        /// </summary>
        [InteractCmd]
        [Name("悬停进入")]
        [InteractCmdFun(nameof(HoverEntry))]
        public EInteractResult HoverEntry(InteractData interactData)
        {
            _isHovered = true;
            return EInteractResult.Success;
        }

        /// <summary>
        /// 悬停停留
        /// </summary>
        [InteractCmd]
        [Name("悬停停留")]
        [InteractCmdFun(nameof(HoverStay))]
        public EInteractResult HoverStay(InteractData interactData)
        {
            return EInteractResult.Success;
        }

        /// <summary>
        /// 悬停退出
        /// </summary>
        [InteractCmd]
        [Name("悬停退出")]
        [InteractCmdFun(nameof(HoverExit))]
        public EInteractResult HoverExit(InteractData interactData)
        {
            _isHovered = false;
            return EInteractResult.Success;
        }

        #endregion

        #region 选择

        /// <summary>
        /// 可选择
        /// </summary>
        [Name("可选择")]
        public bool _selectable = true;

        /// <summary>
        /// 能否选择
        /// </summary>
        public bool canSelect => _selectable;

        /// <summary>
        /// 是否选择
        /// </summary>
        [Readonly]
        [Name("已选择")]
        public bool _isSelected;

        /// <summary>
        /// 是否选择
        /// </summary>
        public bool isSelected { get => _isSelected; set => _isSelected = value; }

        /// <summary>
        /// 可选择
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool CanSetectable(InteractData interactData) => canSelect;

        /// <summary>
        /// 尝试选择
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public bool TrySetectable(InteractData interactData, out EInteractResult interactResult)
        {
            switch (inCmds.GetCmd(interactData.cmdName))
            {
                case nameof(SelectionModify.Select):
                    {
                        interactResult = Select(interactData);
                        return true;
                    }
                case nameof(SelectionModify.Unselect):
                    {
                        interactResult = Unselect(interactData);
                        return true;
                    }
            }
            interactResult = EInteractResult.Fail;
            return false;
        }

        /// <summary>
        /// 选择
        /// </summary>
        [InteractCmd]
        [Name("选择")]
        public void Select()
        {
            if (canSelect)
            {
                TryInteract(nameof(Select), this);
            }
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Select))]
        public EInteractResult Select(InteractData interactData)
        {
            if (interactData.interactable == this)
            {
                isSelected = true;
                Selection.AddWithDistinct(gameObject);
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 取消选择
        /// </summary>
        [InteractCmd]
        [Name("取消选择")]
        public void Unselect() => TryInteract(nameof(Unselect), this);

        /// <summary>
        /// 取消选择
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(Unselect))]
        public EInteractResult Unselect(InteractData interactData)
        {
            if (interactData.interactable == this)
            {
                isSelected = false;
                Selection.Remove(gameObject);
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        #endregion

        #region 激活

        /// <summary>
        /// 可激活
        /// </summary>
        [Name("可激活")]
        public bool _activatable = true;

        /// <summary>
        /// 能否激活
        /// </summary>
        public bool canActive => _activatable;

        /// <summary>
        /// 已激活
        /// </summary>
        [Readonly]
        [Name("已激活")]
        public bool _isActived;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool isActived { get => _isActived; set => _isActived = value; }

        /// <summary>
        /// 可激活
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool CanActivatable(InteractData interactData) => canActive;

        /// <summary>
        /// 尝试激活
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public bool TryActivatable(InteractData interactData, out EInteractResult interactResult)
        {
            switch (inCmds.GetCmd(interactData.cmdName))
            {
                case nameof(PluginTools.Draggers.Activator.Active):
                    {
                        interactResult = Active(interactData);
                        return true;
                    }
                case nameof(PluginTools.Draggers.Activator.Unactive):
                    {
                        interactResult = Unactive(interactData);
                        return true;
                    }
            }
            interactResult = EInteractResult.Fail;
            return false;
        }

        /// <summary>
        /// 激活
        /// </summary>
        [InteractCmd]
        [Name("激活")]
        [InteractCmdFun(nameof(Active))]
        public EInteractResult Active(InteractData interactData)
        {
            isActived = true;
            return EInteractResult.Success;
        }

        /// <summary>
        /// 非激活
        /// </summary>
        [InteractCmd]
        [Name("非激活")]
        [InteractCmdFun(nameof(Unactive))]
        public EInteractResult Unactive(InteractData interactData)
        {
            isActived = false;
            return EInteractResult.Success;
        }

        #endregion

        #region 交互处理

        /// <summary>
        /// 可交互对象虚体列表
        /// </summary>
        [Name("可交互对象虚体列表")]
        [Readonly]
        public List<InteractableVirtual> _interactableVirtuals = new List<InteractableVirtual>();

        /// <summary>
        /// 包含可交互组件
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        public bool ContainInteractable(IInteractable interactable) => interactable != null && (interactable as InteractableEntity == this || _interactableVirtuals.Contains(interactable));

        /// <summary>
        /// 添加工作可交互对象
        /// </summary>
        /// <param name="interactable"></param>
        public void AddWork(InteractableVirtual interactable)
        {
            if (interactable == null || _interactableVirtuals.Contains(interactable)) return;

            _interactableVirtuals.Add(interactable);
        }

        /// <summary>
        /// 移除工作可交互对象
        /// </summary>
        /// <param name="interactable"></param>
        public void RemoveWork(InteractableVirtual interactable)
        {
            _interactableVirtuals.Remove(interactable);
        }

        /// <summary>
        /// 工作交互器中是否存在某类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsExist<T>() where T : InteractableVirtual => _interactableVirtuals.Exists(W => W is T);

        /// <summary>
        /// 全部命令
        /// </summary>
        public override List<string> inCmdList
        {
            get
            {
                _cmds.Clear();
                foreach (var cmd in inCmds.cmds)
                {
                    _cmds.Add(cmd._cmd);
                }
                foreach (var item in _interactableVirtuals)
                {
                    _cmds.AddRange(item.inCmdList);
                }
                return _cmds;
            }
        }
        private List<string> _cmds = new List<string>();

        /// <summary>
        /// 工作命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override List<string> GetWorkInCmds(InteractData interactData)
        {
            _workCmds.Clear();
            foreach (var cmd in inCmds.cmds)
            {
                _cmds.Add(cmd._cmd);
            }
            foreach (var item in _interactableVirtuals)
            {
                _workCmds.AddRange(item.GetWorkInCmds(interactData));
            }
            return _workCmds;
        }
        private List<string> _workCmds = new List<string>();

        /// <summary>
        /// 经过can校验后的可交互对象
        /// </summary>
        private List<IInteractable> canInteractables = new List<IInteractable>();

        /// <summary>
        /// 作为可交互对象能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteractAsInteractable(InteractData interactData) => interactData != null;

        /// <summary>
        /// 作为可交互对象尝试交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public override bool TryInteractAsInteractable(InteractData interactData, out EInteractResult interactResult)
        {
            if (CanHoverable(interactData) && TryHoverable(interactData, out _))
            {
                interactResult = EInteractResult.Success;
                return true;
            }
            if (CanSetectable(interactData) && TrySetectable(interactData, out _))
            {
                interactResult = EInteractResult.Success;
                return true;
            }
            if (CanActivatable(interactData) && TryActivatable(interactData, out _))
            {
                interactResult = EInteractResult.Success;
                return true;
            }

            foreach (var interactable in _interactableVirtuals)
            {
                if (interactable && interactable.CanInteractAsInteractable(interactData))
                {
                    if (interactable.TryInteractAsInteractable(interactData, out _))
                    {
                        interactResult = EInteractResult.Success;
                        return true;
                    }
                }
            }

            interactResult = EInteractResult.Fail;
            return false;
        }

        private void OnDrawGizmos()
        {
            if (CommonFun.GetBounds(out var bounds, transform, true, false, false))
            {
                var size = bounds.size / 2;
                var min = Mathf.Min(new float[] { size.x, size.y, size.z });
                Gizmos.DrawWireSphere(transform.position, min);
            }
        }

        #endregion
    }
}
