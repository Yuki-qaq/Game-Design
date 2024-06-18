using UnityEngine;
using System;
using XCSJ.Attributes;

namespace XCSJ.PluginTools.HighlightingSystem.Internal
{
	/// <summary>
	/// 高亮预设
	/// </summary>
	[Name("高亮预设")]
	[Serializable]
	public struct HighlightingPreset : IEquatable<HighlightingPreset>
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string name { get { return _name; } set { _name = value; } }

		/// <summary>
		/// 填充透明度
		/// </summary>
		public float fillAlpha { get { return _fillAlpha; } set { _fillAlpha = value; } }

		/// <summary>
		/// 下采样因子
		/// </summary>
		public int downsampleFactor { get { return _downsampleFactor; } set { _downsampleFactor = value; } }

		/// <summary>
		/// 迭代
		/// </summary>
		public int iterations { get { return _iterations; } set { _iterations = value; } }

		/// <summary>
		/// 模糊最小扩散
		/// </summary>
		public float blurMinSpread { get { return _blurMinSpread; } set { _blurMinSpread = value; } }

		/// <summary>
		/// 模糊扩散
		/// </summary>
		public float blurSpread { get { return _blurSpread; } set { _blurSpread = value; } }

		/// <summary>
		/// 模糊强度
		/// </summary>
		public float blurIntensity { get { return _blurIntensity; } set { _blurIntensity = value; } }

		/// <summary>
		/// 模糊方向
		/// </summary>
		public BlurDirections blurDirections { get { return _blurDirections; } set { _blurDirections = value; } }

		[SerializeField] private string _name;
		[SerializeField] private float _fillAlpha;
		[SerializeField] private int _downsampleFactor;
		[SerializeField] private int _iterations;
		[SerializeField] private float _blurMinSpread;
		[SerializeField] private float _blurSpread;
		[SerializeField] private float _blurIntensity;
		[SerializeField] private BlurDirections _blurDirections;

		#region IEquatable implementation
		// 
		bool IEquatable<HighlightingPreset>.Equals(HighlightingPreset other)
		{
			return 
				_name == other._name && 
				_fillAlpha == other._fillAlpha && 
				_downsampleFactor == other._downsampleFactor && 
				_iterations == other._iterations && 
				_blurMinSpread == other._blurMinSpread && 
				_blurSpread == other._blurSpread && 
				_blurIntensity == other._blurIntensity && 
				_blurDirections == other._blurDirections;
		}
		#endregion
	}
}