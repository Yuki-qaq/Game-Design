using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;

namespace XCSJ.PluginTools.CrossSectionShader
{
    /// <summary>
    /// �����нӿ�
    /// </summary>
    public interface ICuttable : IInteractable { }

    /// <summary>
    /// �����ж���
    /// </summary>
    [Name("�����ж���")]
    [XCSJ.Attributes.Icon(EIcon.CrossSection)]
    [DisallowMultipleComponent]
    [Tool(ToolsExtensionCategory.Model, nameof(InteractableVirtual), rootType = typeof(ToolsManager))]
    [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
    [Owner(typeof(ToolsManager))]
    public class Cuttable : InteractableVirtual, ICuttable
    {
        /// <summary>
        /// �Ƿ��и�
        /// </summary>
        public bool cutted { get; private set; } = false;

    }
}

