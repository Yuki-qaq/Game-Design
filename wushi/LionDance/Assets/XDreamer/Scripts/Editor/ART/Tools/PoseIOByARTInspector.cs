using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginART;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// 姿态通过ART检查器
    /// </summary>
    [CustomEditor(typeof(PoseByART))]
    [Name("姿态通过ART检查器 ")]
    public class PoseIOByARTInspector : BaseAnalogProviderInspector<PoseByART>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorARTHelper.DrawSelectARTManager();
        }
    }
}
