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
    /// Toggle切换检查器
    /// </summary>
    [Name("Toggle切换检查器")]
    [CustomEditor(typeof(ToggleSwitch))]
    public class ToggleSwitchInspectorr : StateComponentInspector<ToggleSwitch>
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
                case nameof(stateComponent.toggle):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorXGUIHelper.DrawCreateButton(stateComponent.toggle, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var component = ToolsMenu.CreateUIWithStyle<Toggle>();
                                serializedProperty.objectReferenceValue = component;
                                return component.gameObject;
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
