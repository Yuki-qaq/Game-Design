using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Base.Interactions.Tools
{
    /// <summary>
    /// 可交互虚体
    /// </summary>
    [RequireComponent(typeof(InteractableEntity))]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class InteractableVirtual : ExtensionalInteractObject
    {
        /// <summary>
        /// 依赖可交互实体
        /// </summary>
        public InteractableEntity interactableEntity => _interactableEntity ? _interactableEntity : _interactableEntity = this.XGetOrAddComponent<InteractableEntity>();
        private InteractableEntity _interactableEntity;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake() { }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            interactableEntity.AddWork(this);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            interactableEntity.RemoveWork(this);
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteractAsInteractable(InteractData interactData) => interactData != null && ContainsInCmdName(interactData.cmdName);

        /// <summary>
        /// 尝试交互：作为可交互对象，直使用交互器传递过来的交互数据并调用对应的交互处理方法
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public sealed override bool TryInteractAsInteractable(InteractData interactData, out EInteractResult interactResult) => (interactResult = CallInteractCmdFun(interactData)) != EInteractResult.Fail;
    }
}
