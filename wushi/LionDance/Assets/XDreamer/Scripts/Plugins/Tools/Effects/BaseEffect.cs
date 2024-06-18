using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.PropertyDatas;
using System;
using XCSJ.Extension.Base.Attributes;
using static XCSJ.PluginTools.Effects.BaseEffect;

namespace XCSJ.PluginTools.Effects
{
    /// <summary>
    /// 颜色效果
    /// </summary>
    public interface IColorEffect
    {
        /// <summary>
        /// 颜色
        /// </summary>
        Color color { get; set; }
    }

    /// <summary>
    /// 效果基类
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    [Tool(ToolsCategory.InteractEffect, nameof(BaseEffect), rootType = typeof(ToolsExtensionManager))]
    public abstract class BaseEffect : InteractProvider
    {
        /// <summary>
        /// 效果关键字
        /// </summary>
        [Group("数据设置", textEN = "Data Settings")]
        [Name("效果关键字")]
        [PropertyKeyUser]
        public string _effectKey;

        /// <summary>
        /// 效果关键字
        /// </summary>
        public string effectKey => string.IsNullOrEmpty(_effectKey) ? _effectKey = _tagProperty.firstKey : _effectKey;

        /// <summary>
        /// 效果数据源
        /// </summary>
        public enum EEffectDataSource
        {
            /// <summary>
            /// 可交互对象的交互属性
            /// </summary>
            [EnumFieldName("可交互对象的交互属性")]
            [Tip("从【效果】正在作用的可交互对象上查找匹配【效果关键字】的交互属性组件上的属性值", "Find the attribute values on the interactive attribute components that match the 'Effect Keyword' from the interactive object that the 'Effect' is currently working on")]
            InteractableInteractProperty,

            /// <summary>
            /// 自身的交互属性
            /// </summary>
            [EnumFieldName("自身的交互属性")]
            [Tip("从【效果】组件所在的游戏对象上查找匹配【效果关键字】的交互属性组件上的属性值", "Find the attribute values on the interactive attribute components that match the 'Effect Keyword' on the game object where the 'Effect' component is located")]
            SelfInteractProperty,

            /// <summary>
            /// 自身标签属性
            /// </summary>
            [EnumFieldName("自身标签属性")]
            [Tip("从【效果】组件上的标签属性上查找匹配【效果关键字】的标签值", "Find the tag value that matches the 'Effect Keyword' from the tag attribute on the 'Effect' component")]
            SelfTag,
        }

        /// <summary>
        /// 效果数据源列表：使用效果关键字并根据列表中的顺序查找数据源，找到后即返回，不再继续查找
        /// </summary>
        [Name("效果数据源列表")]
        [Tip("根据列表中的顺序查找数据源，找到后即返回，不再继续查找", "Search for data sources in the order in the list, and once found, return without continuing to search")]
        [EnumPopup]
        public List<EEffectDataSource> _effectDataSources = new List<EEffectDataSource>();

        /// <summary>
        /// 叠加效果
        /// </summary>
        [Name("叠加效果")]
        [EndGroup(true)]
        public bool _overlayEffect = false;

        /// <summary>
        /// 叠加效果
        /// </summary>
        public bool overlayEffect => _overlayEffect;

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            if (string.IsNullOrEmpty(effectKey)) { }

            _effectDataSources.Add(EEffectDataSource.InteractableInteractProperty);
            _effectDataSources.Add(EEffectDataSource.SelfInteractProperty);
            _effectDataSources.Add(EEffectDataSource.SelfTag);
        }

        /// <summary>
        /// 获取效果数据
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        protected BaseInteractPropertyData GetEffectData(GameObject go)
        {
            if (string.IsNullOrEmpty(effectKey)) return default;
            referenceObjectVarString.TrySetOrAddSetHierarchyVarValue(this);
            foreach (var item in _effectDataSources)
            {
                var data = GetEffectData(item, go, effectKey);
                if (data != null)
                {
                    return data;
                }
            }
            return default;
        }

        /// <summary>
        /// 获取效果数据
        /// </summary>
        /// <param name="effectDataSource"></param>
        /// <param name="go"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private BaseInteractPropertyData GetEffectData(EEffectDataSource effectDataSource, GameObject go, string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            switch (effectDataSource)
            {
                case EEffectDataSource.InteractableInteractProperty: return InteractPropertyHelper.GetInteractPropertyValue(go, key);
                case EEffectDataSource.SelfInteractProperty: return InteractPropertyHelper.GetInteractPropertyValue(gameObject, key);
                case EEffectDataSource.SelfTag:return _tagProperty.GetFirstData(key);
                default: return null;
            }
        }

        /// <summary>
        /// 启用可视化特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public abstract void EnableEffect(InteractData interactData, GameObject gameObject);

        /// <summary>
        /// 禁用可视化特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public abstract void DisableEffect(InteractData interactData, GameObject gameObject);

        /// <summary>
        /// 特效工作中
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public virtual void EffectWorking(InteractData interactData, GameObject gameObject) { }
    }
}
