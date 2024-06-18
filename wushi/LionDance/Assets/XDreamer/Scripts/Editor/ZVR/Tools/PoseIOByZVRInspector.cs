using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginZVR;
using XCSJ.PluginZVR.Tools;

namespace XCSJ.EditorZVR.Tools
{
    /// <summary>
    /// 姿态通过ZVR检查器
    /// </summary>
    [Name("姿态通过ZVR检查器")]
    [CustomEditor(typeof(PoseByZVR))]
    public class PoseIOByZVRInspector : BaseAnalogProviderInspector<PoseByZVR>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorZVRHelper.DrawSelectZVRManager();
        }
    }
}
