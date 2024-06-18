using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.Base.Dataflows.Base
{
    /// <summary>
    /// 基础属性值绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(BasePropertyValue), true)]
    public class PropertyValueDrawer : PropertyDrawer
    {
        internal const float PropertyValueTypeWidth = 80;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));
            var propertyValueType = (EPropertyValueType)propertyValueTypeSP.intValue;

            //绘制标签
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //绘制属性值类型选择
            var position1 = new Rect(position.x, position.y, PropertyValueTypeWidth, position.height);
            var newPropertyValueType = (EPropertyValueType)UICommonFun.EnumPopup(position1, propertyValueType);
            if (propertyValueType != newPropertyValueType)
            {
                propertyValueTypeSP.intValue = (int)newPropertyValueType;
            }

            //绘制属性值
            var position2 = new Rect(position.x + PropertyValueTypeWidth, position.y, position.width - PropertyValueTypeWidth, position.height);
            switch (newPropertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        var valueSP = property.FindPropertyRelative(PropertyValueFieldNameAttribute.GetFieldName(fieldInfo.FieldType));
                        EditorGUI.PropertyField(position2, valueSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.Variable:
                    {
                        var variableNameSP = property.FindPropertyRelative(nameof(BasePropertyValue._variableName));
                        EditorGUI.PropertyField(position2, variableNameSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.VarString:
                    {
                        var varStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._varString));
                        EditorGUI.PropertyField(position2, varStringSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.ExpressionString:
                    {
                        var expressionStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._expressionString));
                        EditorGUI.PropertyField(position2, expressionStringSP, GUIContent.none);
                        break;
                    }
            }
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// 字符串属性值绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(StringPropertyValue_TextArea), true)]
    public class StringPropertyValue_TextAreaDrawer : PropertyValueDrawer
    {
        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));
            switch ((EPropertyValueType)propertyValueTypeSP.intValue)
            {
                case EPropertyValueType.Value:
                    {
                        return base.GetPropertyHeight(property, label) * 3;
                    }
            }
            return base.GetPropertyHeight(property, label);
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var position1 = new Rect(position.x, position.y, PropertyValueTypeWidth, position.height);
            var propertyValueType = (EPropertyValueType)propertyValueTypeSP.intValue;
            var newPropertyValueType = (EPropertyValueType)UICommonFun.EnumPopup(position1, propertyValueType);
            if (propertyValueType != newPropertyValueType)
            {
                propertyValueTypeSP.intValue = (int)newPropertyValueType;
            }

            var position2 = new Rect(position.x + PropertyValueTypeWidth, position.y, position.width - PropertyValueTypeWidth, position.height);
            switch (newPropertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        var valueSP = property.FindPropertyRelative(PropertyValueFieldNameAttribute.GetFieldName(fieldInfo.FieldType));
                        valueSP.stringValue = EditorGUI.TextArea(position2, valueSP.stringValue);
                        break;
                    }
                case EPropertyValueType.Variable:
                    {
                        var variableNameSP = property.FindPropertyRelative(nameof(BasePropertyValue._variableName));
                        EditorGUI.PropertyField(position2, variableNameSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.VarString:
                    {
                        var varStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._varString));
                        EditorGUI.PropertyField(position2, varStringSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.ExpressionString:
                    {
                        var expressionStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._expressionString));
                        EditorGUI.PropertyField(position2, expressionStringSP, GUIContent.none);
                        break;
                    }
            }
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// 自定义函数属性值绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomFunctionPropertyValue), true)]
    public class CustomFunctionPropertyValueDrawer : PropertyValueDrawer
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));

            // Draw label
            var position0 = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var position1 = new Rect(position0.x, position0.y, PropertyValueTypeWidth, position.height);
            var propertyValueType = (EPropertyValueType)propertyValueTypeSP.intValue;
            var newPropertyValueType = (EPropertyValueType)UICommonFun.EnumPopup(position1, propertyValueType);
            if (propertyValueType != newPropertyValueType)
            {
                propertyValueTypeSP.intValue = (int)newPropertyValueType;
            }

            var position2 = new Rect(position0.x + PropertyValueTypeWidth, position0.y, position0.width - PropertyValueTypeWidth, position.height);
            switch (newPropertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        var valueSP = property.FindPropertyRelative(PropertyValueFieldNameAttribute.GetFieldName(fieldInfo.FieldType));
                        EditorGUILayout.PropertyField(valueSP, GUIContent.none, true);
                        break;
                    }
                case EPropertyValueType.Variable:
                    {
                        var variableNameSP = property.FindPropertyRelative(nameof(BasePropertyValue._variableName));
                        EditorGUI.PropertyField(position2, variableNameSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.VarString:
                    {
                        var varStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._varString));
                        EditorGUI.PropertyField(position2, varStringSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.ExpressionString:
                    {
                        var expressionStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._expressionString));
                        EditorGUI.PropertyField(position2, expressionStringSP, GUIContent.none);
                        break;
                    }
            }
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// 属性路径列表绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(PropertyPathList), true)]
    [LanguageFileOutput]
    public class PropertyPathListDrawer : PropertyDrawer
    {
        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => totalHeight;

        float totalHeight = 0;

        Dictionary<int, (GUIContent, Type)> dictionary = new Dictionary<int, (GUIContent, Type)>();

        private (GUIContent, Type) GetDisplayType(IPropertyPathList propertyPathList, int index)
        {
            if (dictionary.TryGetValue(index, out var displayType)) return displayType;

            displayType = propertyPathList != null && propertyPathList.TryGetPropertyPathValueType(index, out var type) ? (type.TrLabel(), type) : (new GUIContent(), default);
            dictionary[index] = displayType;
            return displayType;
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        [LanguageTuple("Member Name", "成员名称")]
        [LanguageTuple("Value Type", "值类型")]
        [LanguageTuple("Operation", "操作")]
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                EditorGUI.BeginChangeCheck();
                var h = EditorGUIUtility.singleLineHeight;
                var rect = new Rect(position.x, position.y, position.width, h + 4);
                totalHeight = rect.height;

                var content = EditorGUI.BeginProperty(position, label, property);
                var propertyPathsSP = property.FindPropertyRelative(nameof(PropertyPathList._propertyPaths));

                var isExpandedNew = UICommonFun.Foldout(rect, propertyPathsSP.isExpanded, content, () => 72, r =>
                {
                    var tmpRect = new Rect(r.x, r.y, 24, r.height);
                    if (GUI.Button(tmpRect, UICommonOption.Run, EditorStyles.miniButtonMid))
                    {
                        CommonFun.FocusControl();
                        var propertyPathList0 = property.ConvertValueTo<IPropertyPathList>();
                        if (propertyPathList0.TryGetPropertyValue(out var value))
                        {
                            propertyPathList0.TryGetPropertyValueType(out var type);
                            Debug.LogFormat("属性路径列表的实例值为[{0}],类型[{1}],定义类型[{2}]",
                               value.ToScriptParamString(),
                               value?.GetType()?.FullName ?? "",
                               type?.FullName ?? "");
                        }
                        else
                        {
                            Debug.LogFormat("属性路径列表的实例值计算失败");
                        }
                    }

                    tmpRect.x = tmpRect.xMax;
                    if (GUI.Button(tmpRect, UICommonOption.Insert, EditorStyles.miniButtonMid))
                    {
                        CommonFun.FocusControl();
                        propertyPathsSP.arraySize++;
                    }
                    tmpRect.x = tmpRect.xMax;
                    if (GUI.Button(tmpRect, UICommonOption.Delete, EditorStyles.miniButtonRight))
                    {
                        CommonFun.FocusControl();
                        if (propertyPathsSP.arraySize > 0) propertyPathsSP.arraySize--;
                    }
                });

                propertyPathsSP.isExpanded = isExpandedNew;
                if (isExpandedNew)
                {
                    var propertyPathList = property.ConvertValueTo<IPropertyPathList>();

                    rect.x = position.x + 24;
                    rect.y = position.y + h + 6;

                    totalHeight += h + 6;
                    GUI.Box(new Rect(rect.x, rect.y, position.width - 24, h + 4), GUIContent.none, XGUIStyleLib.Get(EGUIStyle.Box));

                    rect.y += 2;
                    rect.height = h;

                    rect.width = 32;
                    EditorGUI.LabelField(rect, "NO.");

                    rect.x = rect.xMax;
                    rect.width = 120;
                    EditorGUI.LabelField(rect, "Type".Tr());

                    rect.x = rect.xMax;
                    rect.width = position.width - 24 - 32 - 120 * 2 - 72 - 2;
                    EditorGUI.LabelField(rect, "Member Name".Tr(typeof(PropertyPathListDrawer)));

                    rect.x = rect.xMax;
                    rect.width = 120;
                    EditorGUI.LabelField(rect, "Value Type".Tr(typeof(PropertyPathListDrawer)));

                    rect.x = rect.xMax;
                    rect.width = 72;
                    EditorGUI.LabelField(rect, "Operation".Tr(typeof(PropertyPathListDrawer)));

                    dictionary.Clear();
                    var bg = GUI.backgroundColor;

                    for (int i = 0; i < propertyPathsSP.arraySize; i++)
                    {
                        totalHeight += h + 2;
                        rect.x = position.x + 24;
                        rect.y = position.y + i * (h + 2) + 2 * h + 12;

                        var typeMemberSP = propertyPathsSP.GetArrayElementAtIndex(i);
                        var memberNameSP = typeMemberSP.FindPropertyRelative(nameof(TypeMember._memberName));

                        var mainType = GetDisplayType(propertyPathList, i - 1);
                        var mainTypeLabel = mainType.Item1;
                        var valueType = GetDisplayType(propertyPathList, i);
                        var valueTypeLabel = valueType.Item1;

                        GUI.backgroundColor = (string.IsNullOrEmpty(mainTypeLabel.text) || string.IsNullOrEmpty(valueTypeLabel.text)) ? Color.red : bg;

                        GUI.Box(new Rect(rect.x, rect.y, position.width - 24, rect.height), GUIContent.none, CommonFun.GetGUIStyle(i, EditorGUIUtility.isProSkin));

                        rect.height = h;
                        rect.width = 32;
                        EditorGUI.LabelField(rect, (i + 1).ToString());

                        rect.x = rect.xMax;
                        rect.width = 120;
                        EditorGUI.LabelField(rect, mainTypeLabel);

                        rect.x = rect.xMax;
                        rect.width = position.width - 24 - 32 - 120 * 2 - 72 - 2;
                        EditorGUI.PropertyField(rect, memberNameSP, GUIContent.none);

                        rect.x = rect.xMax;
                        rect.width = 120;
                        EditorGUI.LabelField(rect, valueTypeLabel);

                        rect.x = rect.xMax;
                        rect.width = 24;
                        if (GUI.Button(rect, UICommonOption.Run, EditorStyles.miniButtonMid))
                        {
                            CommonFun.FocusControl();
                            if (propertyPathList.TryGetPropertyPathValue(i, out var value))
                            {
                                Debug.LogFormat("类型[{0}]中成员[{1}]（类型: {2}）值为[{3}]",
                                    mainTypeLabel.text,
                                    memberNameSP.stringValue,
                                    valueTypeLabel.text,
                                    value.ToScriptParamString());
                            }
                            else
                            {
                                var error = "异常错误！";
                                if (mainType.Item2 == typeof(void))
                                {
                                    error = "空返回值类型无法计算成员信息！";
                                }
                                Debug.LogWarningFormat("类型[{0}]中成员[{1}]（类型: {2}）值计算失败: {3}",
                                    mainTypeLabel.text,
                                    memberNameSP.stringValue,
                                    valueTypeLabel.text,
                                    error);
                            }
                        }

                        rect.x = rect.xMax;
                        rect.width = 24;
                        if (GUI.Button(rect, UICommonOption.Insert, EditorStyles.miniButtonMid))
                        {
                            CommonFun.FocusControl();
                            typeMemberSP.InsertArrayElementAtThisIndex();
                        }

                        rect.x = rect.xMax;
                        rect.width = 24;
                        if (GUI.Button(rect, UICommonOption.Delete, EditorStyles.miniButtonRight))
                        {
                            CommonFun.FocusControl();
                            typeMemberSP.DeleteArrayElementCommand();
                        }
                    }
                    GUI.backgroundColor = bg;
                }

                EditorGUI.EndProperty();
            }
            catch(Exception ex)
            {
                ex.HandleException(nameof(PropertyPathListDrawer));
            }
            finally
            {
                if (EditorGUI.EndChangeCheck())
                {
                    UICommonFun.RepaintInspector();
                }
            }
        }
    }
}
