using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;

namespace XCSJ.PluginTools.HighlightingSystem.Internal
{
	/// <summary>
	/// 高亮渲染器
	/// </summary>
	[Name("高亮渲染器")]
	[DisallowMultipleComponent]
	[AddComponentMenu("")]  // Hide in 'Add Component' menu
	[UnityEngine.Internal.ExcludeFromDocs]
	public class HighlighterRenderer : BaseHighlighterMB
	{
		[UnityEngine.Internal.ExcludeFromDocs]
		private struct Data
		{
			public Material material;
			public int submeshIndex;
			public bool transparent;
		}

		#region Constants
		// Default transparency cutoff value (used for shaders without _Cutoff property)
		static private float transparentCutoff = 0.5f;

		// Flags to hide and don't save this component in editor
		private const HideFlags flags = HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild;
		
		// Cull Off
		private const int cullOff = (int)CullMode.Off;
		#endregion

		#region Static Fields
		static private readonly string sRenderType = "RenderType";
		static private readonly string sOpaque = "Opaque";
		static private readonly string sTransparent = "Transparent";
		static private readonly string sTransparentCutout = "TransparentCutout";
		static private readonly string sMainTex = "_MainTex";
		#endregion

		#region Public Fields

		/// <summary>
		/// 是活跃的
		/// </summary>
		public bool isAlive = false;
		#endregion

		#region Private Fields
		private Renderer r;
		private List<Data> data = new List<Data>();
		#endregion

		#region MonoBehaviour
		
		/// <summary>
		/// 唤醒初始化
		/// </summary>
		void Awake()
		{
			this.hideFlags = flags;
		}

		/// <summary>
		/// 启用
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			EndOfFrame.AddListener(OnEndOfFrame);
		}

		/// <summary>
		/// 禁用
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			EndOfFrame.RemoveListener(OnEndOfFrame);
		}

		// Called once (before OnPreRender) for each camera if the object is visible. 
		// The function is called during the culling process just before rendering each culled object.
		void OnWillRenderObject()
		{
			HighlightingBase.SetVisible(this);
		}

		// 
		void OnDestroy()
		{
			// Data will be null if Undo / Redo was performed in Editor to delete / restore object with this component
			if (data == null) { return; }

			for (int i = 0, imax = data.Count; i < imax; i++)
			{
				Data d = data[i];
				// Unity never garbage-collects unreferenced materials, so it is our responsibility to destroy them
				if (d.transparent)
				{
					Destroy(d.material);
				}
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="sharedOpaqueMaterial"></param>
		/// <param name="transparentShader"></param>
		/// <param name="submeshIndices"></param>
		public void Initialize(Material sharedOpaqueMaterial, Shader transparentShader, List<int> submeshIndices)
		{
			data.Clear();

			r = GetComponent<Renderer>();
			Material[] materials = r.sharedMaterials;
			int materialsLength = materials.Length;

			if (materials == null || materialsLength == 0) { return; }

			// Highlight all submeshes
			if (submeshIndices.Count == 1 && submeshIndices[0] == -1)
			{
				submeshIndices.Clear();
				for (int i = 0; i < materialsLength; i++)
				{
					submeshIndices.Add(i);
				}
			}

			// Prepare specified submeshIndices
			for (int i = 0, imax = submeshIndices.Count; i < imax; i++)
			{
				int submeshIndex = submeshIndices[i];

				// Invalid submeshIndex
				if (submeshIndex >= materialsLength) { continue; }

				Material sourceMat = materials[submeshIndex];

				if (sourceMat == null) { continue; }

				Data d = new Data();

				string tag = sourceMat.GetTag(sRenderType, true, sOpaque);
				if (tag == sTransparent || tag == sTransparentCutout)
				{
					Material replacementMat = new Material(transparentShader);

					// To render both sides of the Sprite
					if (r is SpriteRenderer) { replacementMat.SetInt(ShaderPropertyID._HighlightingCull, cullOff); }

					if (sourceMat.HasProperty(ShaderPropertyID._MainTex))
					{
						replacementMat.SetTexture(ShaderPropertyID._MainTex, sourceMat.mainTexture);
						replacementMat.SetTextureOffset(sMainTex, sourceMat.mainTextureOffset);
						replacementMat.SetTextureScale(sMainTex, sourceMat.mainTextureScale);
					}

					int cutoff = ShaderPropertyID._Cutoff;
					replacementMat.SetFloat(cutoff, sourceMat.HasProperty(cutoff) ? sourceMat.GetFloat(cutoff) : transparentCutoff);

					d.material = replacementMat;
					d.transparent = true;
				}
				else
				{
					d.material = sharedOpaqueMaterial;
					d.transparent = false;
				}

				d.submeshIndex = submeshIndex;
				data.Add(d);
			}
		}

		/// <summary>
		/// 设置覆盖
		/// </summary>
		/// <param name="overlay"></param>
		public void SetOverlay(bool overlay)
		{
			for (int i = 0, imax = data.Count; i < imax; i++)
			{
				Data d = data[i];
				if (d.transparent)
				{
					d.material.SetKeyword(HighlighterCore.keywordOverlay, overlay);
				}
			}
		}

		/// <summary>
		/// 设置颜色:将给定颜色设置为此渲染器的所有透明材质上的高亮显示颜色
		/// </summary>
		/// <param name="clr"></param>
		public void SetColor(Color clr)
		{
			for (int i = 0, imax = data.Count; i < imax; i++)
			{
				Data d = data[i];
				if (d.transparent)
				{
					d.material.SetColor(ShaderPropertyID._HighlightingColor, clr);
				}
			}
		}

		/// <summary>
		/// 填充缓存
		/// </summary>
		/// <param name="buffer"></param>
		public void FillBuffer(CommandBuffer buffer)
		{
			for (int i = 0, imax = data.Count; i < imax; i++)
			{
				Data d = data[i];
				buffer.DrawRenderer(r, d.material, d.submeshIndex);
			}
		}

		/// <summary>
		/// 是有效的
		/// </summary>
		/// <returns></returns>
		public bool IsValid()
		{
			return r != null;
		}
		#endregion

		#region Private Methods
		// 
		private void OnEndOfFrame()
		{
			if (!isAlive)
			{
				Destroy(this);
			}
		}
		#endregion
	}
}