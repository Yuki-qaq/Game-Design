using System;
using System.Reflection;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 渲染器透明度区间检查器
    /// </summary>
    [Name("渲染器透明度区间检查器")]
    [CustomEditor(typeof(RendererRangeMotion))]
    public class RendererRangeMotionInspector : RendererRangeHandleInspector<RendererRangeMotion>
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
                case nameof(stateComponent._onEntryColorValue):
                case nameof(stateComponent._leftColorValue):
                case nameof(stateComponent._inColorValue):
                case nameof(stateComponent._rightColorValue):
                case nameof(stateComponent._onExitColorValue):
                    {
                        if (stateComponent._operationType != RendererRangeMotion.EOperationType.Color)
                        {
                            return;
                        }
                        break;
                    }
                case nameof(stateComponent._onEntryAlphaValue):
                case nameof(stateComponent._leftAlphaValue):
                case nameof(stateComponent._inAlphaValue):
                case nameof(stateComponent._rightAlphaValue):
                case nameof(stateComponent._onExitAlphaValue):
                    {
                        if(stateComponent._operationType == RendererRangeMotion.EOperationType.Alpha)
                        {
                            serializedProperty.floatValue = EditorGUILayout.Slider(propertyData.trLabel, serializedProperty.floatValue, 0, 1);
                        }
                        return;
                    }
                case nameof(stateComponent._setMaterialRule):
                case nameof(stateComponent._onEntryMaterialValue):
                case nameof(stateComponent._leftMaterialValue):
                case nameof(stateComponent._inMaterialValue):
                case nameof(stateComponent._rightMaterialValue):
                case nameof(stateComponent._onExitMaterialValue):
                    {
                        if (stateComponent._operationType != RendererRangeMotion.EOperationType.Material)
                        {
                            return;
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
