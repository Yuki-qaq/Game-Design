using UnityEngine;
using System.Collections;

namespace XCSJ.PluginTools.HighlightingSystem.Internal
{
	/// <summary>
	/// 着色器属性ID
	/// </summary>
	// Shader property ID cached constants
	[UnityEngine.Internal.ExcludeFromDocs]
	public class ShaderPropertyID
	{
		/// <summary>
		/// 主纹理
		/// </summary>
		static public readonly int _MainTex = Shader.PropertyToID("_MainTex");

		/// <summary>
		/// 截止
		/// </summary>
		static public readonly int _Cutoff = Shader.PropertyToID("_Cutoff");

		/// <summary>
		/// 高亮显示强度
		/// </summary>
		static public readonly int _HighlightingIntensity = Shader.PropertyToID("_HighlightingIntensity");

		/// <summary>
		/// 高亮显示剔除
		/// </summary>
		static public readonly int _HighlightingCull = Shader.PropertyToID("_HighlightingCull");

		/// <summary>
		/// 高亮显示颜色
		/// </summary>
		static public readonly int _HighlightingColor = Shader.PropertyToID("_HighlightingColor");

		/// <summary>
		/// 高亮显示模糊偏移
		/// </summary>
		static public readonly int _HighlightingBlurOffset = Shader.PropertyToID("_HighlightingBlurOffset");

		/// <summary>
		/// 高亮显示填充透明度
		/// </summary>
		static public readonly int _HighlightingFillAlpha = Shader.PropertyToID("_HighlightingFillAlpha");

		/// <summary>
		/// 高亮显示缓存
		/// </summary>
		static public readonly int _HighlightingBuffer = Shader.PropertyToID("_HighlightingBuffer");

		/// <summary>
		/// 高亮显示模糊1
		/// </summary>
		static public readonly int _HighlightingBlur1 = Shader.PropertyToID("_HighlightingBlur1");

		/// <summary>
		/// 高亮显示模糊2
		/// </summary>
		static public readonly int _HighlightingBlur2 = Shader.PropertyToID("_HighlightingBlur2");
	}
}