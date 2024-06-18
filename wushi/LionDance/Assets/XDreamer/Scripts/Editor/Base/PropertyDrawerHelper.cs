using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// 属性绘制器辅助类
    /// </summary>
    public static class PropertyDrawerHelper
    {
        /// <summary>
        /// 间隔宽度
        /// </summary>
        public const int SpaceWidth = ComponentPopupAttributeDrawer.SpaceWidth;

        /// <summary>
        /// 单行高
        /// </summary>
        public static float singleLineHeight = EditorGUIUtility.singleLineHeight + 2;

        /// <summary>
        /// 绘制字符串弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <param name="values"></param>
        /// <param name="hasText"></param>
        /// <param name="popupWidth"></param>
        /// <returns>成功弹出菜单返回True，否则返回False</returns>
        public static bool DrawStringPopup(Rect position, SerializedProperty property, GUIContent label, string[] values, bool hasText, float popupWidth)
        {
            string[] stringArray = values;
            int index = stringArray.IndexOf(property.stringValue);

            if (stringArray != null && index < stringArray.Length)
            {
                if (hasText)
                {
                    var rect = new Rect(position.x, position.y, position.width - popupWidth - 2, position.height);
                    label = EditorGUI.BeginProperty(rect, label, property);
                    property.stringValue = EditorGUI.TextField(rect, label, property.stringValue);
                    EditorGUI.EndProperty();

                    rect.x = rect.x + rect.width + 2;
                    rect.width = popupWidth;
                    var newIndex = EditorGUI.Popup(rect, index, stringArray);
                    if (newIndex != index) property.stringValue = stringArray[newIndex];
                }
                else
                {
                    label = EditorGUI.BeginProperty(position, label, property);
                    var newIndex = EditorGUI.Popup(position, label, index, CommonFun.TempContent(stringArray));
                    if (newIndex != index) property.stringValue = stringArray[newIndex];
                    EditorGUI.EndProperty();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 绘制属性
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="includeChildren"></param>
        /// <returns></returns>
        public static Rect DrawProperty(Rect rect, SerializedProperty property, bool includeChildren = false) => DrawProperty(rect, property, "", includeChildren);

        /// <summary>
        /// 绘制属性
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="customName"></param>
        /// <param name="includeChildren"></param>
        /// <returns></returns>
        public static Rect DrawProperty(Rect rect, SerializedProperty property, string customName, bool includeChildren = false)
        {
            return DrawProperty(rect, property, string.IsNullOrEmpty(customName) ? PropertyData.GetPropertyData(property).trLabel : new GUIContent(customName), includeChildren);
        }

        /// <summary>
        /// 绘制属性
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <param name="includeChildren"></param>
        /// <returns></returns>
        public static Rect DrawProperty(Rect rect, SerializedProperty property, GUIContent label, bool includeChildren = false)
        {
            rect.y += singleLineHeight;
            EditorGUI.PropertyField(rect, property, label, includeChildren);
            return rect;
        }
    }
}
