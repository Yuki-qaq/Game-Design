using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.Interactions.Base
{
    /// <summary>
    /// 属性关键字使用者特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(PropertyKeyUserAttribute))]
    public class PropertyKeyUserAttributeDrawer : PropertyDrawer<PropertyKeyUserAttribute>
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(rect, label, property);
            {
                // 特性修饰的字段是String类型
                if (property.propertyType == SerializedPropertyType.String)
                {
                    var tmp = rect;
                    tmp.width -= 100;
                    EditorGUI.PropertyField(tmp, property, PropertyData.GetPropertyData(property).trLabel);

                    tmp.x += tmp.width;
                    tmp.width = 100;

                    //property.stringValue = UICommonFun.Popup(tmp, property.stringValue, PropertyKeyCache.propertyKeys);
                    EditorGUI.BeginChangeCheck();
                    var keyPath = UICommonFun.Popup(tmp, property.stringValue, PropertyKeyCache.instance.GetKeys());
                    if (EditorGUI.EndChangeCheck())
                    {
                        var array = keyPath.Split('/');
                        var arrayLength = array.Length;
                        property.stringValue = arrayLength > 0 ? array[arrayLength - 1] : keyPath;
                    }
                }
                //else if(property.propertyType == SerializedPropertyType.ManagedReference && property.FindPropertyRelative(nameof(StringPropertyValue._propertyValueType)) is SerializedProperty valueTypeSP && property.FindPropertyRelative(nameof(StringPropertyValue._value)) is SerializedProperty valueValueSP)
                //{

                //    if (valueTypeSP.intValue == 0)// 值类型
                //    {
                //        var tmp = rect;
                //        tmp.width -= 100;
                //        tmp = PropertyDrawerHelper.DrawProperty(tmp, property, true);

                //        tmp.x += tmp.width;
                //        tmp.width = 100;
                //        valueValueSP.stringValue = UICommonFun.Popup(tmp, valueValueSP.stringValue, PropertyKeyCache.instance.GetKeys());

                //        rect.y += PropertyDrawerHelper.singleLineHeight;
                //    }
                //    else
                //    {
                //        rect = PropertyDrawerHelper.DrawProperty(rect, property, PropertyData.GetPropertyData(property).trLabel);
                //    }
                //}
                else
                {
                    EditorGUI.PropertyField(rect, property, PropertyData.GetPropertyData(property).trLabel);
                }
            }
            EditorGUI.EndProperty();
        }
    }
}

