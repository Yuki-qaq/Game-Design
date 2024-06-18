using UnityEditor;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorXGUI;
using XCSJ.EditorXGUI.Tools;
using XCSJ.PluginSMS.States.UGUI;

namespace XCSJ.EditorSMS.States.UGUI
{
    /// <summary>
    /// 输入框编辑检查器
    /// </summary>
    [Name("输入框编辑检查器")]
    [CustomEditor(typeof(InputFieldEdit))]
    public class InputFieldEditInspector : StateComponentInspector<InputFieldEdit>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(stateComponent.inputField):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorXGUIHelper.DrawCreateButton(stateComponent.inputField, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var inputField = ToolsMenu.CreateUIWithStyle<InputField>();
                                serializedProperty.objectReferenceValue = inputField;
                                return inputField.gameObject;
                            });
                        });
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
