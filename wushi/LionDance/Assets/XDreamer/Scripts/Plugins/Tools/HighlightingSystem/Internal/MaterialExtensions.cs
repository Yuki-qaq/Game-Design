using System;
using UnityEngine;

namespace XCSJ.PluginTools.HighlightingSystem.Internal
{
	/// <summary>
	/// 材质扩展
	/// </summary>
	[UnityEngine.Internal.ExcludeFromDocs]
	static public class MaterialExtensions
	{
		/// <summary>
		/// 设置关键字
		/// </summary>
		/// <param name="material"></param>
		/// <param name="keyword"></param>
		/// <param name="state"></param>
		static public void SetKeyword(this Material material, string keyword, bool state)
		{
			if (state) { material.EnableKeyword(keyword); }
			else { material.DisableKeyword(keyword); }
		}
	}
}