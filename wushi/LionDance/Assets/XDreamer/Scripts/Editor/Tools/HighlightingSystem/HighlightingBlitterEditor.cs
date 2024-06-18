using UnityEngine;
using UnityEditor;
using XCSJ.PluginTools.HighlightingSystem;

namespace XCSJ.EditorTools.HighlightingSystem
{
	/// <summary>
	/// 高亮位图传输器编辑器
	/// </summary>
	[CustomEditor(typeof(HighlightingBlitter), true)]
	public class HighlightingBlitterEditor : Editor
	{
		/// <summary>
		/// 当绘制检查器GUI
		/// </summary>
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox("Use order of this component (relatively to other Image Effects on this camera) to control the point at which highlighting will be applied to the framebuffer (click on a little gear icon to the right and choose Move Up / Move Down).", MessageType.Info);
		}
	}
}