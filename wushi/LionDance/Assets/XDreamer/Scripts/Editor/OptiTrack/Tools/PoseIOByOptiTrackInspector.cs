using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginOptiTrack;
using XCSJ.PluginOptiTrack.Tools;

namespace XCSJ.EditorOptiTrack.Tools
{
    /// <summary>
    /// 姿态通过OptiTrack检查器
    /// </summary>
    [Name("姿态通过OptiTrack检查器")]
    [CustomEditor(typeof(PoseByOptiTrack))]
    public class PoseIOByOptiTrackInspector : BaseAnalogProviderInspector<PoseByOptiTrack>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorOptiTrackHelper.DrawSelectOptiTrackManager();
        }
    }
}
