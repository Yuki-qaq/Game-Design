using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.DataViews;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.EditorXGUI.DataViews.Base
{
    /// <summary>
    /// 模型键特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ModelKeyAttribute))]
    public class ModelKeyAttributeDrawer : PropertyDrawer<ModelKeyAttribute>
    {
        /// <summary>
        /// 当绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String && property.serializedObject.targetObject is IModelKeyValueHost host)
            {
                var keys = host?.modelKeyValue?.keys?.ToArray() ?? Empty<string>.Array;
                property.stringValue = UICommonFun.Popup(position, label, property.stringValue, keys, true, propertyAttribute.buttonWidth);
            }
            else
            {
                base.OnGUI(position, property, label);
            }
        }
    }
}
