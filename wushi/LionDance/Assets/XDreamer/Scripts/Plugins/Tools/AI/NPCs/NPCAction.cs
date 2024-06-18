using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.AI.NPCs
{
    /// <summary>
    /// NPCAction
    /// </summary>
    [Tool(ToolsCategory.AI, nameof(NPCAction), rootType = typeof(ToolsManager))]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class NPCAction : Interactor
    {
        /// <summary>
        /// NPC
        /// </summary>
        [Group("动作设置", textEN = "Action Settings")]
        [Name("NPC")]
        public NPC _npc;

        /// <summary>
        /// NPC
        /// </summary>
        public NPC npc => this.XGetComponentInParent(ref _npc);

        /// <summary>
        /// 尝试获取目标点
        /// </summary>
        /// <returns></returns>
        public virtual bool TryGetDestination(out Vector3 destination)
        {
            if (npc)
            {
                destination = npc.position;
                return true;
            }
            destination = default;
            return false;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (npc)
            {
                npc.AddAction(this);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (npc)
            {
                npc.RemoveAction(this);
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="npcInteractData"></param>
        /// <returns></returns>
        public virtual bool Execute(NPCInteractData npcInteractData) => true;
    }
}
