using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 悬停器
    /// </summary>
    [Name("悬停器")]
    [Tool(ToolsCategory.InteractEffect, rootType = typeof(ToolsManager))]
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    public class Hover : Interactor
    {
        private InteractableEntity stayEntity;

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            HoverExit();
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => stayEntity || interactData.interactable is InteractableEntity entity && entity && entity.canHover;

        /// <summary>
        /// 检测悬停状态
        /// </summary>
        [InteractCmd]
        [Name("检测悬停状态")]
        public void CheckHoverState() => TryInteract(nameof(CheckHoverState));

        /// <summary>
        /// 检测悬停状态
        /// </summary>
        [InteractCmdFun(nameof(CheckHoverState))]
        public EInteractResult CheckHoverState(InteractData interactData)
        {
            var current = interactData.interactable;
            if (current)
            {
                if (stayEntity == current)
                {
                    TryInteract(nameof(HoverStay), interactData, stayEntity);
                }
                else
                {
                    TryHoverExit(interactData);
                    TryInteract(nameof(HoverEntry), interactData, current);
                }
            }
            else
            {
                TryHoverExit(interactData);
            }
            return EInteractResult.Success;
        }

        /// <summary>
        /// 悬停进入
        /// </summary>
        [InteractCmd]
        [Name("悬停进入")]
        public void HoverEntry() => TryInteract(nameof(HoverEntry));

        /// <summary>
        /// 悬停进入
        /// </summary>
        [InteractCmdFun(nameof(HoverEntry))]
        public EInteractResult HoverEntry(InteractData interactData)
        {
            if (interactData.interactable is InteractableEntity entity && entity && entity != stayEntity)
            {
                stayEntity = entity;
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 悬停停留
        /// </summary>
        [InteractCmd]
        [Name("悬停停留")]
        public void HoverStay() => TryInteract(nameof(HoverStay));

        /// <summary>
        /// 悬停停留
        /// </summary>
        [InteractCmdFun(nameof(HoverStay))]
        public EInteractResult HoverStay(InteractData interactData)
        {
            return stayEntity == interactData.interactable ? EInteractResult.Success : EInteractResult.Fail;
        }

        /// <summary>
        /// 悬停退出
        /// </summary>
        [InteractCmd]
        [Name("悬停退出")]
        public void HoverExit() => TryInteract(nameof(HoverExit));

        /// <summary>
        /// 悬停退出
        /// </summary>
        [InteractCmdFun(nameof(HoverExit))]
        public EInteractResult HoverExit(InteractData interactData)
        {
            if (stayEntity)
            {
                stayEntity = null;
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        private bool TryHoverExit(InteractData interactData) => stayEntity && TryInteract(nameof(HoverExit), interactData, stayEntity);

    }
}
