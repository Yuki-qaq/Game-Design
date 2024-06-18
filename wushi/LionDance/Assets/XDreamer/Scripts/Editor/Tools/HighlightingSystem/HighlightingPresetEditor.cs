using UnityEditor;
using UnityEngine;
using XCSJ.PluginTools.HighlightingSystem.Internal;

namespace XCSJ.EditorTools.HighlightingSystem
{
	/// <summary>
	/// 高亮预设
	/// </summary>
	[CustomPropertyDrawer(typeof(HighlightingPreset), true)]
	public class HighlightingPresetEditor : PropertyDrawer
	{
		#region Static Fields and Constants

		/// <summary>
		/// 填充透明度标签
		/// </summary>
		static public readonly GUIContent labelFillAlpha = new GUIContent("Fill Alpha", "Inner fill alpha value.");

		/// <summary>
		/// 缩减取样标签
		/// </summary>
		static public readonly GUIContent labelDownsampling = new GUIContent("Downsampling:", "Downsampling factor.");

		/// <summary>
		/// 迭代标签
		/// </summary>
		static public readonly GUIContent labelIterations = new GUIContent("Iterations:", "Blur iterations. Number of blur iterations performed. Larger number means more blur.");

		/// <summary>
		/// 模糊最小扩散标签
		/// </summary>
		static public readonly GUIContent labelBlurMinSpread = new GUIContent("Min Spread:", "Blur Min Spread. Lower values give better looking blur, but require more iterations to get large blurs. Pixel offset for each blur iteration is calculated as 'Min Spread + Spread * Iteration Index'. Usually 'Min Spread + Spread' value is between 0.5 and 1.0.");

		/// <summary>
		/// 模糊扩散标签
		/// </summary>
		static public readonly GUIContent labelBlurSpread = new GUIContent("Spread:", "Blur Spread. Lower values give better looking blur, but require more iterations to get large blurs. Pixel offset for each blur iteration is calculated as 'Min Spread + Spread * Iteration Index'. Usually 'Min Spread + Spread' value is between 0.5 and 1.0.");

		/// <summary>
		/// 模糊强度标签
		/// </summary>
		static public readonly GUIContent labelBlurIntensity = new GUIContent("Intensity:", "Highlighting Intensity. Internally defines the value by which highlighting buffer alpha channel will be multiplied after each blur iteration.");
		
		/// <summary>
		/// 模糊方向标签
		/// </summary>
		static public readonly GUIContent labelBlurDirections = new GUIContent("Blur Directions:", "Blur directions.");

		/// <summary>
		/// 缩减取样选项
		/// </summary>
		static public readonly GUIContent[] downsampleOptions = new GUIContent[] { new GUIContent("None"), new GUIContent("Half"), new GUIContent("Quarter") };

		/// <summary>
		/// 缩减取样获取
		/// </summary>
		static public readonly int[] downsampleGet = new int[] { -1, 0, 1, -1, 2 };     // maps downsampleFactor to the downsampleOptions index

		/// <summary>
		/// 缩减取样设置
		/// </summary>
		static public readonly int[] downsampleSet = new int[] {     1, 2,     4 };		// maps downsampleOptions index to the downsampleFactor

		/// <summary>
		/// 模糊方向
		/// </summary>
		static public readonly GUIContent[] blurDirections = new GUIContent[] { new GUIContent("Diagonal"), new GUIContent("Straight"), new GUIContent("All") };

		private const float rowSpace = 2f;
		#endregion

		#region Private Fields
		private Rect[] rects = new Rect[8];
		#endregion

		#region PropertyDrawer

		/// <summary>
		/// 获取属性高度
		/// </summary>
		/// <param name="property"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			int l = rects.Length;
			return 16f * l + rowSpace * (l - 1);
		}

		/// <summary>
		/// 当绘制GUI
		/// </summary>
		/// <param name="position"></param>
		/// <param name="property"></param>
		/// <param name="label"></param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rects
			HighlightingRendererEditor.GetRowRects(position, rowSpace, rects);

			// Find properties
			SerializedProperty propertyName = property.FindPropertyRelative("_name");
			SerializedProperty propertyFillAlpha = property.FindPropertyRelative("_fillAlpha");
			SerializedProperty propertyDownsampleFactor = property.FindPropertyRelative("_downsampleFactor");
			SerializedProperty propertyIterations = property.FindPropertyRelative("_iterations");
			SerializedProperty propertyBlurMinSpread = property.FindPropertyRelative("_blurMinSpread");
			SerializedProperty propertyBlurSpread = property.FindPropertyRelative("_blurSpread");
			SerializedProperty propertyBlurIntensty = property.FindPropertyRelative("_blurIntensity");
			SerializedProperty propertyBlurDirections = property.FindPropertyRelative("_blurDirections");

			// Draw properties
			int index = 0;
			propertyName.stringValue = EditorGUI.TextField(rects[index], propertyName.stringValue); index++;
			propertyFillAlpha.floatValue = EditorGUI.Slider(rects[index], labelFillAlpha, propertyFillAlpha.floatValue, 0f, 1f); index++;
			propertyDownsampleFactor.intValue = downsampleSet[EditorGUI.Popup(rects[index], labelDownsampling, downsampleGet[propertyDownsampleFactor.intValue], downsampleOptions)]; index++;
			propertyIterations.intValue = Mathf.Clamp(EditorGUI.IntField(rects[index], labelIterations, propertyIterations.intValue), 0, 50); index++;
			propertyBlurMinSpread.floatValue = EditorGUI.Slider(rects[index], labelBlurMinSpread, propertyBlurMinSpread.floatValue, 0f, 3f); index++;
			propertyBlurSpread.floatValue = EditorGUI.Slider(rects[index], labelBlurSpread, propertyBlurSpread.floatValue, 0f, 3f); index++;
			propertyBlurIntensty.floatValue = EditorGUI.Slider(rects[index], labelBlurIntensity, propertyBlurIntensty.floatValue, 0f, 1f); index++;
			propertyBlurDirections.enumValueIndex = (int)EditorGUI.Popup(rects[index], labelBlurDirections, propertyBlurDirections.enumValueIndex, blurDirections); index++;

			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
		#endregion
	}
}