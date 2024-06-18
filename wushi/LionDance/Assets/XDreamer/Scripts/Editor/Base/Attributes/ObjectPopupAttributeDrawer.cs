using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.Base.Attributes
{
    /// <summary>
    /// 组件集弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ObjectPopupAttribute))]
    public class ObjectPopupAttributeDrawer : PropertyDrawer<ObjectPopupAttribute>
    {
        /// <summary>
        /// 当绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                base.OnGUI(position, property, label);
                return;
            }
            var attr = propertyAttribute;

            var componentCollectionWidth = attr.componentCollectionWidth;
            var componentWidth = attr.componentWidth;
            var rect = new Rect(position.x, position.y, position.width - componentCollectionWidth - componentWidth - PropertyDrawerHelper.SpaceWidth * 2, EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = componentCollectionWidth;
            EditorObjectHelper.ComponentCollectionPopup(rect, property);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = componentWidth;
            EditorObjectHelper.ObjectComponentPopup(rect, property);
        }
    }

    /// <summary>
    /// 日期时间特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(DateTimeAttribute))]
    [LanguageTuple("Format", "格式化")]
    [LanguageFileOutput]
    public class DateTimeAttributeDrawer : PropertyDrawer<DateTimeAttribute>
    {
        /// <summary>
        /// 当绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }

            var buttonWidth = propertyAttribute.buttonWidth;

            var rect = new Rect(position.x, position.y, position.width - buttonWidth - PropertyDrawerHelper.SpaceWidth, EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = buttonWidth;
            if (GUI.Button(rect, "Format".Tr(typeof(DateTimeAttributeDrawer))))
            {
                if (DateTime.TryParse(property.stringValue, out var dateTime))
                {
                    property.stringValue = dateTime.ToString(propertyAttribute.format);
                }
                else
                {
                    property.stringValue = DateTime.Now.ToString(propertyAttribute.format);
                }
                CommonFun.FocusControl();
            }
        }
    }

    /// <summary>
    /// 组件弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ComponentPopupAttribute))]
    public class ComponentPopupAttributeDrawer : PropertyDrawer<ComponentPopupAttribute>
    {
        private Type _componentType;

        /// <summary>
        /// 组件类型
        /// </summary>
        public Type componentType
        {
            get
            {
                if (_componentType == null)
                {
                    _componentType = propertyAttribute.componentType;
                    if (_componentType == null)
                    {
                        _componentType = TypeHelper.TryGetElementType(fieldInfo.FieldType, out var elementType) ? elementType : fieldInfo.FieldType;
                    }
                    if (_componentType != null && !_componentType.IsInterface && !typeof(Component).IsAssignableFrom(_componentType))
                    {
                        _componentType = null;
                    }
                    if (_componentType == null)
                    {
                        _componentType = typeof(MB);
                    }

                    disallowAddComponent = componentType.IsAbstract || componentType.IsInterface || componentType.IsGenericType;
                }
                return _componentType;
            }
        }

        /// <summary>
        /// 不允许添加组件
        /// </summary>
        public bool disallowAddComponent { get; private set; } = false;

        private const float AddButtonWidth = 24;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    {
                        var propertyAttribute = this.propertyAttribute;
                        if (Application.isPlaying && !propertyAttribute.displayOnRuntime) break;
                        var componentType = this.componentType;
                        if (propertyAttribute.overrideLabel)
                        {
                            label = fieldInfo.TrLabel();// CommonFun.NameTip(fieldInfo);
                        }

                        var component = property.objectReferenceValue as Component;
                        //if (component)
                        //{
                        //    label.tooltip += string.Format("\n名称路径:" + CommonFun.GameObjectComponentToString(component));
                        //}
                        var popupWidth = propertyAttribute.width;
                        var components = component ? component.GetComponents(componentType) : Empty<Component>.Array;
                        Rect rect = Rect.zero;
                        if (components.Length > 1)
                        {
                            rect = new Rect(position.x, position.y, position.width - 2 * popupWidth - AddButtonWidth - 3 * SpaceWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.ObjectField(rect, property, label);

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;

                            EditorObjectHelper.GameObjectComponentPopup(rect, componentType, propertyAttribute.searchFlags, property);

                            //组件选择
                            rect.x = rect.x + rect.width + SpaceWidth;
                            //rect.width = popupWidth;

                            var componentNew = property.objectReferenceValue as Component;
                            if (componentNew != component) components = componentNew ? componentNew.GetComponents(componentType) : Empty<Component>.Array;
                            var index = components.IndexOf(componentNew);
                            var names = components.Cast((i, c) => (i + 1).ToString() + "." + c.GetType().Name).ToArray();
                            EditorGUI.BeginChangeCheck();
                            var newIndex = EditorGUI.Popup(rect, index, names);
                            if (EditorGUI.EndChangeCheck())
                            {
                                property.objectReferenceValue = newIndex >= 0 ? components[newIndex] : default;
                            }
                        }
                        else
                        {
                            rect = new Rect(position.x, position.y, position.width - popupWidth - AddButtonWidth - 2 * SpaceWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.ObjectField(rect, property, label);

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;

                            EditorObjectHelper.GameObjectComponentPopup(rect, componentType, propertyAttribute.searchFlags, property);
                        }

                        rect.x += rect.width;
                        rect.width = AddButtonWidth;

                        // 给宿主游戏对象加组件，并设置当前属性
                        EditorGUI.BeginDisabledGroup(disallowAddComponent || property.objectReferenceValue);
                        if (GUI.Button(rect, new GUIContent("", EditorIconHelper.GetIconInLib(EIcon.Add))))
                        {
                            var go = CommonFun.GetHostGameObject(property.serializedObject.targetObject);
                            if (go)
                            {
                                var component1 = go.XAddComponent(componentType);
                                if (component1) 
                                {
                                    property.objectReferenceValue = component1;
                                }
                            }
                        }
                        EditorGUI.EndDisabledGroup();

                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }

    /// <summary>
    /// 游戏对象弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(GameObjectPopupAttribute))]
    public class GameObjectPopupAttributeDrawer : PropertyDrawer<GameObjectPopupAttribute>
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    {
                        //var gameObject = property.objectReferenceValue as GameObject;
                        //if (gameObject)
                        //{
                        //    label.tooltip += string.Format("\n名称路径:" + CommonFun.GameObjectToString(gameObject));
                        //}

                        var popupWidth = propertyAttribute.width;
                        var rect = new Rect(position.x, position.y, position.width - popupWidth - SpaceWidth, EditorGUIUtility.singleLineHeight);
                        EditorGUI.ObjectField(rect, property, label);
                        //gameObject = property.objectReferenceValue as GameObject;

                        rect.x = rect.x + rect.width + SpaceWidth;
                        rect.width = popupWidth;
                        EditorObjectHelper.GameObjectPopup(rect, propertyAttribute.componentType, propertyAttribute.includeInactive, property);
                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }

    /// <summary>
    /// 组件类型弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ComponentTypePopupAttribute))]
    public class ComponentTypePopupAttributeDrawer : PropertyDrawer<ComponentTypePopupAttribute>
    {
        /// <summary>
        /// 当绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }
            var attr = propertyAttribute;

            var buttonWidth = attr.buttonWidth;
            var rect = new Rect(position.x, position.y, position.width - buttonWidth - PropertyDrawerHelper.SpaceWidth, EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = buttonWidth;
            EditorObjectHelper.GameObjectComponentTypePopup(rect, property);
        }
    }

    /// <summary>
    /// 数组元素数据
    /// </summary>
    public class ArrayElementData
    {
        /// <summary>
        /// 属性数据
        /// </summary>
        public PropertyData propertyData;

        /// <summary>
        /// 是数组元素
        /// </summary>
        public bool isArrayElement = false;

        /// <summary>
        /// 以0开始的程序索引
        /// </summary>
        public int index = -1;

        /// <summary>
        /// 以1开始的索引字符串，比<see cref="index"/>显示值大1；
        /// </summary>
        public string indexString => indexContent.text;

        /// <summary>
        /// 索引内容：索引字符串内容
        /// </summary>
        public GUIContent indexContent = new GUIContent();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="property"></param>
        public virtual void Init(SerializedProperty property)
        {
            propertyData = PropertyData.GetPropertyData(property);
            index = propertyData.arrayElementIndex;
            if (index >= 0)
            {
                indexContent.text = (index + 1).ToString();
            }

            isArrayElement = propertyData.isArrayElement;
        }

        /// <summary>
        /// 翻译标签
        /// </summary>
        public GUIContent trLabel = new GUIContent();

        /// <summary>
        /// 更新翻译标签
        /// </summary>
        public virtual void UpdateTrLabel()
        {
            trLabel.text = propertyData.trLabel.text;
            trLabel.tooltip = propertyData.trLabel.tooltip;
        }
    }

    /// <summary>
    /// 数组元素数据缓存
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ArrayElementDataCache<TData> where TData : ArrayElementData, new()
    {
        Dictionary<SerializedObject, Dictionary<string, TData>> cache = new Dictionary<SerializedObject, Dictionary<string, TData>>();

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public TData GetData(SerializedProperty property)
        {
            if (!cache.TryGetValue(property.serializedObject, out var dic))
            {
                //移除字典中的无效项
                var list = new List<SerializedObject>();
                foreach (var kv in cache)
                {
                    var so = kv.Key;
                    try
                    {
                        if (!so.targetObject)
                        {
                            list.Add(so);
                        }
                    }
                    catch//(Exception ex)
                    {
                        list.Add(so);
                        //ex.HandleException(typeof(TData).Name + "." + nameof(GetData));
                    }
                }
                foreach(var so in list)
                {
                    cache.Remove(so);
                }
                list.Clear();

                dic = new Dictionary<string, TData>();
                cache[property.serializedObject] = dic;
            }

            if (!dic.TryGetValue(property.propertyPath, out var data))
            {
                dic[property.propertyPath] = data = new TData();
                data.Init(property);
            }

            data.UpdateTrLabel();
            return data;
        }
    }

    /// <summary>
    /// 作为数组元素的属性绘制器
    /// </summary>
    /// <typeparam name="TArrayElementData"></typeparam>
    public abstract class PropertyDrawerAsArrayElement<TArrayElementData> : PropertyDrawer where TArrayElementData : ArrayElementData, new()
    {
        /// <summary>
        /// 缓存
        /// </summary>
        public static ArrayElementDataCache<TArrayElementData> cache { get; } = new ArrayElementDataCache<TArrayElementData>();
    }

    /// <summary>
    /// 启用组件信息绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(BaseRuntimePlatformDetailInfo), true)]
    public class BaseRuntimePlatformDetailInfoDrawer : PropertyDrawerAsArrayElement<BaseRuntimePlatformDetailInfoDrawer.Data>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public class Data : ArrayElementData
        {
            /// <summary>
            /// 运行时平台序列化属性
            /// </summary>
            public SerializedProperty runtimePlatformSP;

            /// <summary>
            /// 值序列化属性
            /// </summary>
            public SerializedProperty valueSP;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);
                runtimePlatformSP = property.FindPropertyRelative(nameof(BaseRuntimePlatformDetailInfo._runtimePlatform));
                valueSP = property.FindPropertyRelative("_value");
            }
        }

        private const float LabelWidth = 48;
        private const float RPWidth = 72 + 90;
        private const float ValueX = LabelWidth + RPWidth;

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);
            label = data.isArrayElement ? data.indexContent : label;

            var labelRect = new Rect(position.x, position.y, LabelWidth, position.height);
            label = EditorGUI.BeginProperty(labelRect, label, property);
            EditorGUI.PrefixLabel(labelRect, label);

            EditorGUI.PropertyField(new Rect(position.x + LabelWidth, position.y, RPWidth, position.height), data.runtimePlatformSP, GUIContent.none);
            EditorGUI.PropertyField(new Rect(position.x + ValueX, position.y, position.width - ValueX, position.height), data.valueSP, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }


    /// <summary>
    /// Guid生成器特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(GuidCreaterAttribute))]
    public class GuidCreaterAttributeDrawer : PropertyDrawer<GuidCreaterAttribute>
    {
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }

            var buttonWidth = propertyAttribute.buttonWidth;

            var rect = new Rect(position.x, position.y, position.width - 2 * buttonWidth - PropertyDrawerHelper.SpaceWidth, EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = buttonWidth;
            if (GUI.Button(rect, UICommonOption.Copy, EditorStyles.miniButtonLeft))
            {
                CommonFun.CopyTextToClipboardForPC(property.stringValue);
            }
            rect.x = rect.x + rect.width;
            //rect.width = buttonWidth;
            if (GUI.Button(rect, UICommonOption.Reset, EditorStyles.miniButtonRight))
            {
                property.stringValue = GuidHelper.GetNewGuid();
            }
        }
    }

    /// <summary>
    /// 枚举型Unity事件绘制器
    /// </summary>
    public abstract class EnumUnityEventDrawer<TEnum> : UnityEventDrawer
    {
        /// <summary>
        /// 当绘制枚举
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected abstract void OnDrawEnum(Rect position, SerializedProperty property, GUIContent label);

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rect = new Rect(position.x, position.y + 2f, position.width, EditorGUIUtility.singleLineHeight);
            OnDrawEnum(rect, property, label);
            position.yMin += EditorGUIUtility.singleLineHeight + 4f;
            base.OnGUI(position, property, label);
        }

        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight + 4f;
        }
    }
}
