using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginTools.HighlightingSystem.Internal;
using XCSJ.PluginTools.Effects;

namespace XCSJ.PluginTools.HighlightingSystem
{
	/// <summary>
	/// 循环模式
	/// </summary>
	[Name("循环模式")]
	public enum LoopMode
	{
		/// <summary>
		/// 一次
		/// </summary>
		Once, 

		/// <summary>
		/// 循环
		/// </summary>
		Loop, 

		/// <summary>
		/// 乒乓
		/// </summary>
		PingPong, 

		/// <summary>
		/// 永远约束
		/// </summary>
		ClampForever, 
	}

	/// <summary>
	/// 缓慢
	/// </summary>
	[Name("缓慢")]
	public enum Easing
	{
		/// <summary>
		/// 线性
		/// </summary>
		Linear,

		/// <summary>
		/// 四次方入
		/// </summary>
		QuadIn, 

		/// <summary>
		/// 四次方出
		/// </summary>
		QuadOut, 

		/// <summary>
		/// 四次方日出
		/// </summary>
		QuadInOut, 

		/// <summary>
		/// 三次方入
		/// </summary>
		CubicIn, 

		/// <summary>
		/// 三次方出
		/// </summary>
		CubicOut, 

		/// <summary>
		/// 三次方入出
		/// </summary>
		CubicInOut, 

		/// <summary>
		/// 正弦入
		/// </summary>
		SineIn, 

		/// <summary>
		/// 正弦出
		/// </summary>
		SineOut,

		/// <summary>
		/// 正弦
		/// </summary>
		SineInOut, 
	}

	/// <summary>
	/// 渲染器过滤器模式
	/// </summary>
	[Name("渲染器过滤器模式")]
	public enum RendererFilterMode
	{
		/// <summary>
		/// 无
		/// </summary>
		None, 

		/// <summary>
		/// 包括
		/// </summary>
		Include,

		/// <summary>
		/// 排除
		/// </summary>
		Exclude, 
	}

	/// <summary>
	/// 高亮器
	/// </summary>
	[Name("高亮器")]
	[AddComponentMenu("XDreamer/Highlighting System/Highlighter", 0)]
	public class Highlighter : HighlighterCore, IOutline
	{
		#region IOutline

		/// <summary>
		/// 能显示
		/// </summary>
		public bool canDisplay { get => enabled; set => enabled = value; }

		/// <summary>
		/// 是显示
		/// </summary>
		public bool isDisplay { get => enabled; set { } }

		/// <summary>
		/// 开始显示
		/// </summary>
		/// <param name="outliner"></param>
		public void StartDisplay(IOutlineData outliner)
		{
			constantColor = outliner.color;
			this.overlay = outliner.overlay;

			StopDisplay();

			ConstantOnImmediate(constantColor);
		}

		/// <summary>
		/// 停止显示
		/// </summary>
		public void StopDisplay()
		{
			TweenStop();
			TweenSet(false);

			ConstantOffImmediate();
		}

		#endregion

		#region Constants

		/// <summary>
		/// 半π
		/// </summary>
		protected const float HALFPI = Mathf.PI * 0.5f;
		#endregion

		#region Inspector Fields
		/* General */

		/// <summary>
		/// When set to true - highlighting will be rendered on top of all other geometry. 
		/// </summary>
		public bool overlay
		{
			get { return _overlay; }
			set { _overlay = value; }
		}

		/// <summary>
		/// Controls occluder mode. 
		/// </summary>
		public bool occluder
		{
			get { return _occluder; }
			set { _occluder = value; }
		}
		
		/// <summary>
		/// 补间
		/// </summary>
		public bool tween
		{
			get { return _tween; }
			set { TweenSet(value); }
		}

		/// <summary>
		/// 补间渐变色
		/// </summary>
		public Gradient tweenGradient
		{
			get { return _tweenGradient; }
			set { _tweenGradient = value; }
		}

		/// <summary>
		/// 补间时长
		/// </summary>
		public float tweenDuration
		{
			get { return _tweenDuration; }
			set { _tweenDuration = value; ValidateRanges(); }
		}

		/// <summary>
		/// 补间延时
		/// </summary>
		public float tweenDelay
		{
			get { return _tweenDelay; }
			set { _tweenDelay = value; }
		}

		/// <summary>
		/// 补间使用非缩放时间
		/// </summary>
		public bool tweenUseUnscaledTime
		{
			get { return _tweenUseUnscaledTime; }
			set
			{
				if (_tweenUseUnscaledTime != value)
				{
					float delta = GetTweenTime() - _tweenStart;
					_tweenUseUnscaledTime = value;
					_tweenStart = GetTweenTime() - delta;
				}
			}
		}

		/// <summary>
		/// 补间循环
		/// </summary>
		public LoopMode tweenLoop
		{
			get { return _tweenLoop; }
			set { _tweenLoop = value; }
		}

		/// <summary>
		/// 补间缓慢
		/// </summary>
		public Easing tweenEasing
		{
			get { return _tweenEasing; }
			set { _tweenEasing = value; }
		}

		/// <summary>
		/// 补间反转
		/// </summary>
		public bool tweenReverse
		{
			get { return _tweenReverse; }
			set { _tweenReverse = value; }
		}

		/// <summary>
		/// 补间重复次数
		/// </summary>
		public int tweenRepeatCount
		{
			get { return _tweenRepeatCount; }
			set { _tweenRepeatCount = value; }
		}


		/// <summary>
		/// 常量
		/// </summary>
		public bool constant
		{
			get { return _constant; }
			set { ConstantSet(value); }
		}

		/// <summary>
		/// 常量颜色
		/// </summary>
		public Color constantColor
		{
			get { return _constantColor; }
			set { _constantColor = value; }
		}

		/// <summary>
		/// 常量渐变时间
		/// </summary>
		public float constantFadeTime
		{
			set
			{
				_constantFadeInTime = value;
				_constantFadeOutTime = value;
				ValidateRanges();
				ConstantSet();
			}
		}

		/// <summary>
		/// 常量渐入时间
		/// </summary>
		public float constantFadeInTime
		{
			get { return _constantFadeInTime; }
			set
			{
				_constantFadeInTime = value;
				ValidateRanges();
				if (_constant) { ConstantSet(); }
			}
		}

		/// <summary>
		/// 常量渐出时间
		/// </summary>
		public float constantFadeOutTime
		{
			get { return _constantFadeOutTime; }
			set
			{
				_constantFadeOutTime = value;
				ValidateRanges();
				if (!_constant) { ConstantSet(); }
			}
		}

		/// <summary>
		/// 常量使用非缩放时间
		/// </summary>
		public bool constantUseUnscaledTime
		{
			get { return _constantUseUnscaledTime; }
			set
			{
				if (_constantUseUnscaledTime != value)
				{
					float delta = GetConstantTime() - _constantStart;
					_constantUseUnscaledTime = value;
					_constantStart = GetConstantTime() - delta;
				}
			}
		}

		/// <summary>
		/// 常量缓慢
		/// </summary>
		public Easing constantEasing
		{
			get { return _constantEasing; }
			set { _constantEasing = value; }
		}

		/// <summary>
		/// 过滤模式
		/// </summary>
		public RendererFilterMode filterMode
		{
			get { return _filterMode; }
			set
			{
				if (_filterMode != value)
				{
					_filterMode = value;
					// Update highlighted Renderers
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Make sure to trigger SetDirty() after modifying this list
		/// </summary>
		public List<Transform> filterList
		{
			get { return _filterList; }
		}

		/// <summary>
		/// 要使用的渲染器过滤器
		/// </summary>
		protected override RendererFilter rendererFilterToUse
		{
			get
			{
				if (rendererFilter != null)
				{
					return rendererFilter;
				}
				else if (_filterMode == RendererFilterMode.None)
				{
					return globalRendererFilter != null ? globalRendererFilter : DefaultRendererFilter;
				}
				else if (_filterMode == RendererFilterMode.Include)
				{
					return TransformFilterInclude;
				}
				else if (_filterMode == RendererFilterMode.Exclude)
				{
					return TransformFilterExclude;
				}
		
				// Should never happen
				return DefaultRendererFilter;
			}
		}
		#endregion

		#region Protected Fields

		/// <summary>
		/// 覆盖
		/// </summary>
		[SerializeField] protected bool _overlay;

		/// <summary>
		/// 遮挡
		/// </summary>
		[SerializeField] protected bool _occluder;

		/// <summary>
		/// 悬停颜色
		/// </summary>
		protected Color _hoverColor = Color.red;

		/// <summary>
		/// 悬停帧
		/// </summary>
		protected int _hoverFrame = -1;

		/// <summary>
		/// 补间
		/// </summary>
		[SerializeField] protected bool _tween = false;

		/// <summary>
		/// 补间渐变色
		/// </summary>
		[SerializeField] protected Gradient _tweenGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(0f, 1f, 1f, 1f), 0f), 
				new GradientColorKey(new Color(0f, 1f, 1f, 1f), 1f), 
			}, 
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(0f, 0f), 
				new GradientAlphaKey(1f, 1f), 
			}
		};

		/// <summary>
		/// 补间时长
		/// </summary>
		[SerializeField] protected float _tweenDuration = 1f;

		/// <summary>
		/// 补间反转
		/// </summary>
		[SerializeField] protected bool _tweenReverse = false;

		/// <summary>
		/// 补间循环
		/// </summary>
		[SerializeField] protected LoopMode _tweenLoop = LoopMode.PingPong;

		/// <summary>
		/// 补间缓慢
		/// </summary>
		[SerializeField] protected Easing _tweenEasing = Easing.Linear;

		/// <summary>
		/// 补间延时
		/// </summary>
		[SerializeField] protected float _tweenDelay = 0f;

		/// <summary>
		/// 补间重复次数
		/// </summary>
		[SerializeField] protected int _tweenRepeatCount = -1;

		/// <summary>
		/// 补间使用非缩放时间
		/// </summary>
		[SerializeField] protected bool _tweenUseUnscaledTime = false;
		
		/// <summary>
		/// 常量
		/// </summary>
		[SerializeField] protected bool _constant = false;

		/// <summary>
		/// 常量颜色
		/// </summary>
		[SerializeField] protected Color _constantColor = Color.yellow;

		/// <summary>
		/// 常量渐入时间
		/// </summary>
		[SerializeField] protected float _constantFadeInTime = 0.1f;

		/// <summary>
		/// 常量渐出时间
		/// </summary>
		[SerializeField] protected float _constantFadeOutTime = 0.25f;

		/// <summary>
		/// 常量缓慢
		/// </summary>
		[SerializeField] protected Easing _constantEasing = Easing.Linear;

		/// <summary>
		/// 常量使用非缩放时间
		/// </summary>
		[SerializeField] protected bool _constantUseUnscaledTime = false;

		/// <summary>
		/// 过滤模式
		/// </summary>
		[SerializeField] protected RendererFilterMode _filterMode = RendererFilterMode.None;

		/// <summary>
		/// 过滤器列表
		/// </summary>
		[SerializeField] protected List<Transform> _filterList = new List<Transform>();

		/// <summary>
		/// 补间启用
		/// </summary>
		protected bool _tweenEnabled;

		/// <summary>
		/// 补间开始：补间开始的时间
		/// </summary>
		protected float _tweenStart;      

		/// <summary>
		/// 常量启用
		/// </summary>
		protected bool _constantEnabled;

		/// <summary>
		/// 常量开始
		/// </summary>
		protected float _constantStart;

		/// <summary>
		/// 常量时长
		/// </summary>
		protected float _constantDuration;
		#endregion

		#region Protected Accessors

		/// <summary>
		/// 悬停
		/// </summary>
		protected bool hover
		{
			get { return _hoverFrame == Time.frameCount; }
			set { _hoverFrame = value ? Time.frameCount : -1; }
		}

		/// <summary>
		/// 常量值：常量值总是从0增加到1（对于淡入和淡出过渡）
		/// </summary>
		[Name("常量值")]
		[Tip("常量值总是从0增加到1（对于淡入和淡出过渡）", "ConstantValue is always increasing from 0 to 1 (for both fade in and out transitions)")]
		protected float constantValue
		{
			get { return _constantDuration > 0f ? Mathf.Clamp01((GetConstantTime() - _constantStart) / _constantDuration) : 1f; }
		}
		#endregion
		
		#region MonoBehaviour
		
		/// <summary>
		/// 当确定应用动画器属性
		/// </summary>
		protected virtual void OnDidApplyAnimationProperties()
		{
			ValidateAll();
		}
		
		#if UNITY_EDITOR
		// 
		void OnValidate()
		{
			ValidateAll();
		}
		
		// 
		void Reset()
		{
			ValidateAll();
		}
		#endif
		#endregion
		
		#region MonoBehaviour Overrides
		
		/// <summary>
		/// 当安全启用
		/// </summary>
		protected override void OnEnableSafe()
		{
			ValidateAll();
		}
		
		/// <summary>
		/// 当安全禁用
		/// </summary>
		protected override void OnDisableSafe()
		{
			// Make sure transition won't continue if component will be re-enabled
			_tweenEnabled = false;
			_constantEnabled = false;
			_constantStart = GetConstantTime() - _constantDuration;
		}
		#endregion
		
		#region Validation
		/// <summary>
		/// 验证全部
		/// </summary>
		protected void ValidateAll()
		{
			ValidateRanges();
			TweenSet();
			ConstantSet();
			SetDirty();
		}

		/// <summary>
		/// 验证区间
		/// </summary>
		protected void ValidateRanges()
		{
			if (_tweenDuration < 0f) { _tweenDuration = 0f; }
			if (_constantFadeInTime < 0f) { _constantFadeInTime = 0f; }
			if (_constantFadeOutTime < 0f) { _constantFadeInTime = 0f; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Turn on one-frame highlighting with specified color.
		/// Can be called multiple times per update, color only from the latest call will be used.
		/// </summary>
		/// <param name='color'>
		/// Highlighting color.
		/// </param>
		public void Hover(Color color)
		{
			_hoverColor = color;
			hover = true;
		}

		/// <summary>
		/// Fade in constant highlighting using specified transition duration.
		/// </summary>
		/// <param name="fadeTime">
		/// Transition time.
		/// </param>
		public void ConstantOn(float fadeTime = 0.25f)
		{
			ConstantSet(fadeTime, true);
		}

		/// <summary>
		/// Fade in constant highlighting using specified color and transition duration.
		/// </summary>
		/// <param name="color">
		/// Constant highlighting color.
		/// </param>
		/// <param name="fadeTime">
		/// Transition duration.
		/// </param>
		public void ConstantOn(Color color, float fadeTime = 0.25f)
		{
			_constantColor = color;
			ConstantSet(fadeTime, true);
		}

		/// <summary>
		/// Fade out constant highlighting using specified transition duration.
		/// </summary>
		/// <param name="fadeTime">
		/// Transition time.
		/// </param>
		public void ConstantOff(float fadeTime = 0.25f)
		{
			ConstantSet(fadeTime, false);
		}

		/// <summary>
		/// Switch constant highlighting using specified transition duration.
		/// </summary>
		/// <param name="fadeTime">
		/// Transition time.
		/// </param>
		public void ConstantSwitch(float fadeTime = 0.25f)
		{
			ConstantSet(fadeTime, !_constant);
		}

		/// <summary>
		/// Turn on constant highlighting immediately (without fading in).
		/// </summary>
		public void ConstantOnImmediate()
		{
			ConstantSet(0f, true);
		}
		
		/// <summary>
		/// Turn on constant highlighting using specified color immediately (without fading in).
		/// </summary>
		/// <param name='color'>
		/// Constant highlighting color.
		/// </param>
		public void ConstantOnImmediate(Color color)
		{
			_constantColor = color;
			ConstantSet(0f, true);
		}
		
		/// <summary>
		/// Turn off constant highlighting immediately (without fading out).
		/// </summary>
		public void ConstantOffImmediate()
		{
			ConstantSet(0f, false);
		}
		
		/// <summary>
		/// Switch constant highlighting immediately (without fading in/out).
		/// </summary>
		public void ConstantSwitchImmediate()
		{
			ConstantSet(0f, !_constant);
		}

		/// <summary>
		/// Turn off all types of highlighting (occlusion mode remains intact). 
		/// </summary>
		public void Off()
		{
			hover = false;
			TweenSet(false);
			ConstantSet(0f, false);
		}

		
		/// <summary>
		/// 补间开始
		/// </summary>
		public void TweenStart()
		{
			TweenSet(true);
		}

		/// <summary>
		/// 补间结束
		/// </summary>
		public void TweenStop()
		{
			TweenSet(false);
		}

		/// <summary>
		/// 补间设置：更新状态，设置中间值。
		/// </summary>
		/// <param name="value"></param>
		public void TweenSet(bool value)
		{
			_tween = value;
			if (_tweenEnabled != _tween)
			{
				_tweenEnabled = _tween;
				_tweenStart = GetTweenTime();
			}
		}

		/// <summary>
		/// 常量设置：更新常量状态，设置常量值。
		/// </summary>
		/// <param name="fadeTime"></param>
		/// <param name="value"></param>
		public void ConstantSet(float fadeTime, bool value)
		{
			// Order matters. Should always set _constantDuration prior to _constantEnabled

			// Transition duration
			if (fadeTime < 0f) { fadeTime = 0f; }
			if (_constantDuration != fadeTime)
			{
				float timeNow = GetConstantTime();
				// Recalculate start time if duration changed
				_constantStart = _constantDuration > 0f ? timeNow - (fadeTime / _constantDuration) * (timeNow - _constantStart) : timeNow - fadeTime;
				_constantDuration = fadeTime;
			}

			// Transition target
			_constant = value;
			if (_constantEnabled != _constant)
			{
				_constantEnabled = _constant;
				// Recalculate start time if value changed
				_constantStart = GetConstantTime() - _constantDuration * (1f - constantValue);
			}
		}
		#endregion

		#region Protected Methods
		
		/// <summary>
		/// 补间设置
		/// </summary>
		protected void TweenSet()
		{
			TweenSet(_tween);
		}
		
		/// <summary>
		/// 常量设置
		/// </summary>
		protected void ConstantSet()
		{
			ConstantSet(_constant);
		}

		/// <summary>
		/// 常量设置
		/// </summary>
		/// <param name="value"></param>
		protected void ConstantSet(bool value)
		{
			ConstantSet(value ? constantFadeInTime : _constantFadeOutTime, value);
		}

		/// <summary>
		/// 更新高亮：更新高亮颜色；
		/// </summary>
		protected override void UpdateHighlighting()
		{
			// Hover
			if (hover)
			{
				color = _hoverColor;
				mode = _overlay ? HighlighterMode.Overlay : HighlighterMode.Default;
				return;
			}

			// Tween
			if (_tweenEnabled)
			{
				float delta = GetTweenTime() - (_tweenStart + _tweenDelay);
				if (delta >= 0f)
				{
					float t = _tweenDuration > 0f ? delta / _tweenDuration : 0f;
					t = Loop(t, _tweenLoop, _tweenReverse, _tweenRepeatCount);
					if (t >= 0f)
					{
						t = Ease(t, _tweenEasing);
						color = _tweenGradient.Evaluate(t);
						mode = _overlay ? HighlighterMode.Overlay : HighlighterMode.Default;
						return;
					}
				}
			}

			// Constant
			float c = _constantEnabled ? constantValue : 1f - constantValue;
			if (c > 0f)
			{
				c = Ease(c, _constantEasing);
				color = _constantColor;
				color.a *= c;
				mode = _overlay ? HighlighterMode.Overlay : HighlighterMode.Default;
				return;
			}

			// Occluder
			if (_occluder)
			{
				mode = HighlighterMode.Occluder;
				return;
			}

			// Disabled
			mode = HighlighterMode.Disabled;
		}
		#endregion

		#region Protected Methods

		/// <summary>
		/// 转换筛选器包含
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="submeshIndices"></param>
		/// <returns></returns>
		protected bool TransformFilterInclude(Renderer renderer, List<int> submeshIndices)
		{
			Transform child = renderer.transform;

			for (int i = 0; i < _filterList.Count; i++)
			{
				Transform parent = _filterList[i];
				if (parent == null) { continue; }

				// Highlight if child of inclusion list transform
				if (child.IsChildOf(parent))
				{
					// Highlight all submeshes
					submeshIndices.Add(-1);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 转换筛选器排除
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="submeshIndices"></param>
		/// <returns></returns>
		protected bool TransformFilterExclude(Renderer renderer, List<int> submeshIndices)
		{
			Transform child = renderer.transform;

			for (int i = 0; i < _filterList.Count; i++)
			{
				Transform parent = _filterList[i];
				if (parent == null) { continue; }

				// Do not highlight if child of exclusion list transform
				if (child.IsChildOf(parent)) { return false; }
			}

			// Highlight all submeshes
			submeshIndices.Add(-1);

			return true;
		}

		/// <summary>
		/// 循环
		/// </summary>
		/// <param name="x"></param>
		/// <param name="loop"></param>
		/// <param name="reverse"></param>
		/// <param name="repeatCount"></param>
		/// <returns></returns>
		protected float Loop(float x, LoopMode loop, bool reverse = false, int repeatCount = -1)
		{
			float y = -1f;

			switch (loop)
			{
				default:
				case LoopMode.Once:
					if (x >= 0f && x <= 1f) { y = x; }
					break;
				case LoopMode.Loop:
					int n1 = Mathf.FloorToInt(x);
					if (repeatCount < 0 || n1 < repeatCount)
					{
						y = x - n1;
					}
					break;
				case LoopMode.PingPong:
					// Performs 0-1-0 transition in tweenDuration time
					//int n2 = Mathf.FloorToInt(x);
					//if (repeatCount < 0 || n2 < repeatCount)
					//{
					//	y = 1f - Mathf.Abs((x - n2) * 2f - 1f);
					//}

					// Performs 0-1-0 transition in 2 * tweenDuration time, 
					// so the 0-1 transition is performed with the same speed as in Loop mode. 
					int n2 = Mathf.FloorToInt(x * 0.5f);
					if (repeatCount < 0 || n2 < repeatCount)
					{
						y = 1f - Mathf.Abs(x - n2 * 2f - 1f);
					}
					break;
				case LoopMode.ClampForever:
					y = Mathf.Clamp01(x);
					break;
			}

			// Reverse. Check y for positive value since there is no loop if it's negative. 
			if (y >= 0f && reverse) { y = 1f - y; }

			return y;
		}

		/// <summary>
		/// 缓解
		/// </summary>
		/// <param name="x"></param>
		/// <param name="easing"></param>
		/// <returns></returns>
		protected float Ease(float x, Easing easing)
		{
			x = Mathf.Clamp01(x);
			float y;
			switch (easing)
			{
				// Linear
				default:
				case Easing.Linear:
					y = x;
					break;
				
				// Quad
				case Easing.QuadIn:
					y = x * x;
					break;
				case Easing.QuadOut:
					y = -x * (x - 2f);
					break;
				case Easing.QuadInOut:
					y = x < 0.5f ? 2f * x * x : 2f * x * (2f - x) - 1f;
					break;
				
				// Cubic
				case Easing.CubicIn:
					y = x * x * x;
					break;
				case Easing.CubicOut:
					x = x - 1f;
					y = x * x * x + 1f;
					break;
				case Easing.CubicInOut:
					if (x < 0.5f)
					{
						y = 4f * x * x * x;
					}
					else
					{
						x = 2f * x - 2f;
						y = 0.5f * (x * x * x + 2f);
					}
					break;
				
				// Sine
				case Easing.SineIn:
					y = 1f - Mathf.Cos(x * HALFPI);
					break;
				case Easing.SineOut:
					y = Mathf.Sin(x * HALFPI);
					break;
				case Easing.SineInOut:
					y = -0.5f * (Mathf.Cos(x * Mathf.PI) - 1f);
					break;

				//y = x * x * (3f - 2f * x);
			}
			return y;
		}

		/// <summary>
		/// 获取补间时间
		/// </summary>
		/// <returns></returns>
		protected float GetTweenTime()
		{
			return _tweenUseUnscaledTime ? Time.unscaledTime : Time.time;
		}

		/// <summary>
		/// 获取恒定时间
		/// </summary>
		/// <returns></returns>
		protected float GetConstantTime()
		{
			return _constantUseUnscaledTime ? Time.unscaledTime : Time.time;
		}
		#endregion

		#region Static Methods

		/// <summary>
		/// HSV转RGB
		/// </summary>
		/// <param name="hue"></param>
		/// <param name="saturation"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		static public Color HSVToRGB(float hue, float saturation, float value)
		{
			float x = 6f * Mathf.Clamp01(hue);
			saturation = Mathf.Clamp01(saturation);
			value = Mathf.Clamp01(value);
			return new Color
				(
					value * (1f + (Mathf.Clamp01(Mathf.Max(2f - x, x - 4f)) - 1f) * saturation), 
					value * (1f + (Mathf.Clamp01(Mathf.Min(x, 4f - x)) - 1f) * saturation), 
					value * (1f + (Mathf.Clamp01(Mathf.Min(x - 2f, 6f - x)) - 1f) * saturation)
				);
		}
		#endregion

		#region DEPRECATED
		private GradientColorKey[] flashingColorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(0f, 1f, 1f, 0f), 0f), 
			new GradientColorKey(new Color(0f, 1f, 1f, 1f), 1f)
		};
		private GradientAlphaKey[] flashingAlphaKeys = new GradientAlphaKey[]
		{
			new GradientAlphaKey(1f, 0f), 
			new GradientAlphaKey(1f, 1f), 
		};

		/// <summary>
		/// DEPRECATED. Use hover = true; instead. 
		/// </summary>
		[System.Obsolete]
		public void On()
		{
			hover = true;
		}

		/// <summary>
		/// DEPRECATED. Use Hover(Color color) instead. 
		/// </summary>
		[System.Obsolete]
		public void On(Color color)
		{
			Hover(color);
		}
		
		/// <summary>
		/// DEPRECATED. Use Hover(new Color(1f, 0f, 0f, 1f)); instead. 
		/// </summary>
		[System.Obsolete]
		public void OnParams(Color color)
		{
			_hoverColor = color;
		}
		
		/// <summary>
		/// DEPRECATED. Use constantColor = new Color(0f, 1f, 1f, 1f); instead. 
		/// </summary>
		[System.Obsolete]
		public void ConstantParams(Color color)
		{
			_constantColor = color;
		}
		
		/// <summary>
		/// DEPRECATED. Use tweenGradient and tweenDuration instead. 
		/// </summary>
		[System.Obsolete]
		public void FlashingParams(Color color1, Color color2, float freq)
		{
			flashingColorKeys[0].color = color1;
			flashingColorKeys[1].color = color2;
			_tweenGradient.SetKeys(flashingColorKeys, flashingAlphaKeys);
			_tweenDuration = 1f / freq;
		}

		/// <summary>
		/// DEPRECATED. Use TweenStart() instead. 
		/// </summary>
		[System.Obsolete]
		public void FlashingOn()
		{
			TweenSet(true);
		}
		
		/// <summary>
		/// DEPRECATED. Use tweenGradient instead. 
		/// </summary>
		[System.Obsolete]
		public void FlashingOn(Color color1, Color color2)
		{
			flashingColorKeys[0].color = color1;
			flashingColorKeys[1].color = color2;
			_tweenGradient.SetKeys(flashingColorKeys, flashingAlphaKeys);
			TweenSet(true);
		}
		
		/// <summary>
		/// DEPRECATED. Use tweenGradient and tweenDuration instead. 
		/// </summary>
		[System.Obsolete]
		public void FlashingOn(Color color1, Color color2, float freq)
		{
			flashingColorKeys[0].color = color1;
			flashingColorKeys[1].color = color2;
			_tweenGradient.SetKeys(flashingColorKeys, flashingAlphaKeys);
			_tweenDuration = 1f / freq;
			TweenSet(true);
		}

		/// <summary>
		/// DEPRECATED. Use tweenDuration instead. 
		/// </summary>
		[System.Obsolete]
		public void FlashingOn(float freq)
		{
			_tweenDuration = 1f / freq;
			TweenSet(true);
		}

		/// <summary>
		/// DEPRECATED. Use TweenStop() instead. 
		/// </summary>
		[System.Obsolete]
		public void FlashingOff()
		{
			TweenSet(false);
		}

		/// <summary>
		/// DEPRECATED. Use TweenStart() and TweenStop() instead. 
		/// </summary>
		[System.Obsolete]
		public void FlashingSwitch()
		{
			tween = !tween;
		}
		#endregion
	}
}