using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集修改
    /// </summary>
    [Name("选择集修改器")]
    [Tip("通过鼠标点击、触摸点击实现基于游戏对象选择集的修改", "The modification based on game object selection set is realized by mouse click and touch click")]
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    [Tool(ToolsCategory.SelectionSet, disallowMultiple = true, rootType = typeof(ToolsManager))]
    public class SelectionModify : SelectionListener
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            // 禁用时取消选择
            Unselect();
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        public bool canInteract { get; set; } = true;

        /// <summary>
        /// 当前选中游戏对象名称：用于界面绑定的属性对象
        /// </summary>
        public string selectedGameObjectName => _currentEntity ? _currentEntity.name : "";

        /// <summary>
        /// 当前选择对象
        /// </summary>
        public GameObject currentSelection => _currentEntity ? _currentEntity.gameObject : default;

        /// <summary>
        /// 当前选择的变换
        /// </summary>
        public Transform currentSelectionTransform => _currentEntity ? _currentEntity.transform : default;

        /// <summary>
        /// 当前选中游戏对象
        /// </summary>
        [Readonly]
        [Name("当前选中游戏对象")]
        public InteractableEntity _currentEntity;

        /// <summary>
        /// 选择
        /// </summary>
        public void Select(GameObject gameObject) => TryInteract(nameof(Select), gameObject.GetComponent<InteractableEntity>());

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => canInteract && (base.CanInteract(interactData) || _currentEntity);

        /// <summary>
        /// 选择
        /// </summary>
        [InteractCmd]
        [Name("选择")]
        public void Select() => TryInteract(nameof(Select));

        /// <summary>
        /// 选择
        /// </summary>
        [InteractCmdFun(nameof(Select))]
        public EInteractResult Select(InteractData interactData)
        {
            // 调用取消选择
            var component = interactData.interactable;
            if (!component || _currentEntity != component)
            {
                UnselectInternal(interactData);
            }

            if (component && component is InteractableEntity entity && entity)
            {
                _currentEntity = entity;
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 取消选择
        /// </summary>
        [InteractCmd]
        [Name("取消选择")]
        public void Unselect() => UnselectInternal();

        /// <summary>
        /// 取消选择
        /// </summary>
        [InteractCmdFun(nameof(Unselect))]
        public EInteractResult Unselect(InteractData interactData)
        {
            var component = interactData.interactable;
            if (_currentEntity && component && component.gameObject == _currentEntity.gameObject)
            {
                _currentEntity = null;
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        private void UnselectInternal(InteractData interactData = null)
        {
            if (_currentEntity)
            {
                TryInteract(nameof(Unselect), interactData, _currentEntity);
            }
        }
    }
}