using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCameras.Characters;
using XCSJ.EditorCommonUtils;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorCameras.Characters
{
    /// <summary>
    /// 基础地面检测检查器
    /// </summary>
    [Name("基础地面检测检查器")]
    [CustomEditor(typeof(BaseGroundDetection), true)]
    public class BaseGroundDetectionInspector : BaseGroundDetectionInspector<BaseGroundDetection> { }

    /// <summary>
    /// 基础地面检测检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseGroundDetectionInspector<T> : MBInspector<T> where T : BaseGroundDetection
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
                case nameof(BaseGroundDetection._stepOffset):
                case nameof(BaseGroundDetection._castDistance):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (targetObject.capsuleCollider && !MathX.Approximately(serializedProperty.floatValue, targetObject.capsuleCollider.radius))
                        {
                            if (GUILayout.Button(UICommonOption.Reset, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                            {
                                serializedProperty.floatValue = targetObject.capsuleCollider.radius;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
