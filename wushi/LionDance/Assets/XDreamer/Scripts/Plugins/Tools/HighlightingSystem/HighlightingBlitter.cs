using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XCSJ.PluginTools.HighlightingSystem.Internal;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;

namespace XCSJ.PluginTools.HighlightingSystem
{
	/// <summary>
	/// 高亮位图传输器
	/// </summary>
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("XDreamer/Highlighting System/Highlighting Blitter", 3)]
	[Name("高亮位图传输器")]
	public class HighlightingBlitter : BaseHighlighterMB
	{
		/// <summary>
		/// 渲染器
		/// </summary>
		protected List<HighlightingBase> renderers = new List<HighlightingBase>();

		#region MonoBehaviour
		/// <summary>
		/// 当渲染图像
		/// </summary>
		/// <param name="src"></param>
		/// <param name="dst"></param>
		protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			bool oddEven = true;
			for (int i = 0; i < renderers.Count; i++)
			{
				HighlightingBase renderer = renderers[i];
				if (oddEven)
				{
					renderer.Blit(src, dst);
				}
				else
				{
					renderer.Blit(dst, src);
				}

				oddEven = !oddEven;
			}

			// Additional blit because final result should be in dst RenderTexture
			if (oddEven)
			{
				Graphics.Blit(src, dst);
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// 注册
		/// </summary>
		/// <param name="renderer"></param>
		[UnityEngine.Internal.ExcludeFromDocs]
		public virtual void Register(HighlightingBase renderer)
		{
			if (!renderers.Contains(renderer))
			{
				renderers.Add(renderer);
			}

			enabled = renderers.Count > 0;
		}

		/// <summary>
		/// 取消注册
		/// </summary>
		/// <param name="renderer"></param>
		[UnityEngine.Internal.ExcludeFromDocs]
		public virtual void Unregister(HighlightingBase renderer)
		{
			int index = renderers.IndexOf(renderer);
			if (index != -1)
			{
				renderers.RemoveAt(index);
			}

			enabled = renderers.Count > 0;
		}
		#endregion
	}

	/// <summary>
	/// 基础高亮组件
	/// </summary>
	[RequireManager(typeof(ToolsManager))]
	[Owner(typeof(ToolsManager))]
	public abstract class BaseHighlighterMB : InteractProvider { }
}