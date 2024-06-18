using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.EditorExtension.Base;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools.PropertyDatas;

namespace XCSJ.EditorTools.PropertyDatas
{
    /// <summary>
    /// 交互属性检查器
    /// </summary>
    [Name("交互属性检查器")]
    [CustomEditor(typeof(InteractProperty), true)]
    [CanEditMultipleObjects]
    public class InteractPropertyInspector : InteractProviderInspector<InteractProperty>
    {
        
    }

    /// <summary>
    /// 交互属性数据绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(InteractPropertyData))]
    public class InteractPropertyDataDrawer : PropertyDrawerAsArrayElement<InteractPropertyDataDrawer.Data>
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public class Data : ArrayElementData
        {
            #region 序列化属性

            /// <summary>
            /// 关键字序列化属性
            /// </summary>
            public SerializedProperty keySP;

            /// <summary>
            /// 关键字值类型序列化属性
            /// </summary>
            public SerializedProperty keyValueTypeSP;

            /// <summary>
            /// 关键字值值序列化属性
            /// </summary>
            public SerializedProperty keyValueValueSP;

            /// <summary>
            /// 值序列化属性
            /// </summary>
            public SerializedProperty valueSP;

            /// <summary>
            /// 颜色类型值序列化属性
            /// </summary>
            public SerializedProperty colorSP;

            /// <summary>
            /// 贴图类型值序列化属性
            /// </summary>
            public SerializedProperty textureSP;

            /// <summary>
            /// 材质类型值序列化属性
            /// </summary>
            public SerializedProperty materailSP;

            /// <summary>
            /// 音频类型值序列化属性
            /// </summary>
            public SerializedProperty audioClipSP;

            /// <summary>
            /// 视频类型值序列化属性
            /// </summary>
            public SerializedProperty videoClipSP;

            #endregion

            /// <summary>
            /// 显示
            /// </summary>
            public bool display = true;

            /// <summary>
            /// 显示高级值
            /// </summary>
            public bool displayAdvancedValue = false;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                keySP = property.FindPropertyRelative(nameof(InteractPropertyData._key));
                keyValueTypeSP = keySP.FindPropertyRelative(nameof(StringPropertyValue._propertyValueType));
                keyValueValueSP = keySP.FindPropertyRelative(nameof(StringPropertyValue._value));

                // 空值时设定初值
                if (string.IsNullOrEmpty(keyValueValueSP.stringValue))
                {
                    keyValueValueSP.stringValue = PropertyKeyCache.propertyKeys.FirstOrDefault();
                }

                valueSP = property.FindPropertyRelative(nameof(InteractPropertyData._value));

                colorSP = property.FindPropertyRelative(nameof(InteractPropertyData._color));
                materailSP = property.FindPropertyRelative(nameof(InteractPropertyData._material));
                textureSP = property.FindPropertyRelative(nameof(InteractPropertyData._texture));
                audioClipSP = property.FindPropertyRelative(nameof(InteractPropertyData._audioClip));
                videoClipSP = property.FindPropertyRelative(nameof(InteractPropertyData._videoClip));
            }
        }

        /// <summary>
        /// 获取对象绘制高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);
            return (base.GetPropertyHeight(property, label) + 2) * (data.display ? (data.displayAdvancedValue ? 9 : 4) : 1);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        [LanguageTuple("Advanced", "高级")]
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);
            label = data.isArrayElement ? data.indexContent : label;

            // 标题
            var rect = new Rect(position.x, position.y, position.width, 18);
            GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
            data.display = GUI.Toggle(rect, data.display, label, EditorStyles.foldout);
            if (!data.display) return;

            // 匹配规则
            rect.xMin += 18;

            if (data.keyValueTypeSP.intValue == 0)// 值类型
            {
                var tmp = rect;
                tmp.width -= 100;
                tmp = PropertyDrawerHelper.DrawProperty(tmp, data.keySP);

                tmp.x += tmp.width;
                tmp.width = 100;
                data.keyValueValueSP.stringValue = UICommonFun.Popup(tmp, data.keyValueValueSP.stringValue, PropertyKeyCache.propertyKeys);

                rect.y += PropertyDrawerHelper.singleLineHeight;
            }
            else
            {
                rect = PropertyDrawerHelper.DrawProperty(rect, data.keySP, "");
            }
            rect = PropertyDrawerHelper.DrawProperty(rect, data.valueSP, "");

            rect.y += PropertyDrawerHelper.singleLineHeight;
            GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
            data.displayAdvancedValue = GUI.Toggle(rect, data.displayAdvancedValue, "Advanced".Tr(), EditorStyles.foldout);
            if (!data.displayAdvancedValue) return;

            // 匹配规则
            rect.xMin += 18;

            rect = PropertyDrawerHelper.DrawProperty(rect, data.colorSP, "");
            rect = PropertyDrawerHelper.DrawProperty(rect, data.materailSP, "");
            rect = PropertyDrawerHelper.DrawProperty(rect, data.textureSP, "");
            rect = PropertyDrawerHelper.DrawProperty(rect, data.audioClipSP, "");
            rect = PropertyDrawerHelper.DrawProperty(rect, data.videoClipSP, "");
        }
    }

    /// <summary>
    /// 交互提供者检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InteractProviderInspector<T> : BaseInteractProviderInspector<T> where T : InteractProvider
    {

    }
}

