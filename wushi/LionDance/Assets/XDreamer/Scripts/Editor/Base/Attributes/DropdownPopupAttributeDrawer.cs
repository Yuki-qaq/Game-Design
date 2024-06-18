using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;


namespace XCSJ.EditorExtension.Base.Attributes
{
    /// <summary>
    /// 下拉式弹出菜单特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(DropdownPopupAttribute), true)]
    public class DropdownPopupAttributeDrawer : PropertyDrawer<DropdownPopupAttribute>
    {
        /// <summary>
        /// 绘制GUI时调用
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var popup = property.ConvertValueTo<IDropdownPopupAttribute>();
            if (popup != null)
            {
                var attr = propertyAttribute;

                var purpose = attr.purpose;
                var propertyPath = property.propertyPath;

                var popupWidth = attr.width;
                var rect = new Rect(position.x, position.y, position.width - popupWidth - PropertyDrawerHelper.SpaceWidth, position.height);
                base.OnGUI(rect, property, label);

                var oldPropertyValue = property.GetSerializedPropertyValue();
                if (!popup.TryGetOption(purpose, propertyPath, oldPropertyValue, out string option))
                {
                    option = oldPropertyValue.ObjectToString();
                }

                rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
                rect.width = popupWidth;
                if (GUI.Button(rect,CommonFun.TempContent(option, option), EditorObjectHelper.MiniPopup))
                {
                    if (popup.TryGetOptions(purpose, propertyPath, out string[] options))
                    {
                        //Debug.Log(options.Length);
                        var propertyCopy = property.Copy();
                        EditorHelper.DrawMenu(option, options, newSelectText =>
                        {
                            if (popup.TryGetPropertyValue(purpose, propertyPath, newSelectText, out object propertyValue))
                            {
                                propertyCopy.SetSerializedPropertyValue(propertyValue);
                                propertyCopy.serializedObject.ApplyModifiedProperties();
                            }
                        });
                    }                        
                }
                return;                
            }
            else
            {
                var type = property.serializedObject.targetObject.GetType();
                Debug.LogErrorFormat("类[{0}]({1})未实现接口[{2}],属性[{3}]的修饰特性[{4}]无法生效!",
                    CommonFun.Name(type),
                    type.FullName,
                    nameof(IDropdownPopupAttribute),
                    property.propertyPath,
                    attribute.GetType()
                    );
            }
            base.OnGUI(position, property, label);
        }
    }
}
