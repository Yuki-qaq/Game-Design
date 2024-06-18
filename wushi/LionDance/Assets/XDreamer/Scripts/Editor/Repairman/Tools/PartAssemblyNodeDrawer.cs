using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginRepairman.Tools;

namespace XCSJ.EditorRepairman.Tools
{
    /// <summary>
    /// 可序列化零件装配节点绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializablePartAssemblyNode), true)]
    public class PartAssemblyNodeDrawer : PropertyDrawerAsArrayElement<PartAssemblyNodeDrawer.Data>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public class Data : ArrayElementData
        {
            private SerializedProperty currentProperty;
            private SerializedProperty partPrototypeSP;
            private SerializedProperty directParentSP;
            private SerializedProperty snapDistanceSP;
            private SerializedProperty assembledPartSP;
            private SerializedProperty moduleSP;

            /// <summary>
            /// 初始化
            /// </summary>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                currentProperty = property;
                partPrototypeSP = property.FindPropertyRelative(nameof(SerializablePartAssemblyNode._partPrototype));

                if (partPrototypeSP.objectReferenceValue is Part part && part)
                {
                    indexContent.text += "-" + part.firstReplacePartTagValue;
                }
                directParentSP = property.FindPropertyRelative(nameof(SerializablePartAssemblyNode._directParent));
                snapDistanceSP = property.FindPropertyRelative(nameof(SerializablePartAssemblyNode._snapDistance));
                assembledPartSP = property.FindPropertyRelative(nameof(SerializablePartAssemblyNode._assembledPart));
                moduleSP = property.FindPropertyRelative(nameof(SerializablePartAssemblyNode._module));
            }

            /// <summary>
            /// 获取行数：包含标题
            /// </summary>
            /// <returns></returns>
            public int GetRowCount(bool display) => display ? 6 : 1;

            /// <summary>
            /// 绘制回调
            /// </summary>
            /// <param name="display"></param>
            /// <param name="inRect"></param>
            /// <param name="label"></param>
            /// <returns></returns>
            public virtual bool OnGUI(bool display, Rect inRect, GUIContent label)
            {
                // 标题
                var rect = new Rect(inRect.x, inRect.y, inRect.width, 18);

                GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
                label = isArrayElement ? indexContent : label;
                display = GUI.Toggle(rect, display, label, EditorStyles.foldout);
                if (display)
                {
                    // 匹配规则
                    rect.xMin += 18;

                    rect = PropertyDrawerHelper.DrawProperty(rect, partPrototypeSP);
                    rect = PropertyDrawerHelper.DrawProperty(rect, directParentSP);
                    rect = PropertyDrawerHelper.DrawProperty(rect, snapDistanceSP);
                    rect = PropertyDrawerHelper.DrawProperty(rect, assembledPartSP);
                    rect = PropertyDrawerHelper.DrawProperty(rect, moduleSP);
                }

                return display;
            }
        }

        /// <summary>
        /// 获取对象绘制高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (base.GetPropertyHeight(property, label) + 2) * cache.GetData(property).GetRowCount(property.isExpanded);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(rect, label, property);
            property.isExpanded = cache.GetData(property).OnGUI(property.isExpanded, rect, label);
            EditorGUI.EndProperty();
        }
    }


    /// <summary>
    /// 零件装配约束绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(PartAssemblyConstraint), true)]
    public class PartAssemblyConstraintDrawer : PropertyDrawerAsArrayElement<PartAssemblyConstraintDrawer.Data>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public class Data : ArrayElementData
        {
            private SerializedProperty fromPartSP;
            private SerializedProperty toPartSP;

            private static string fromPartName = CommonFun.Name(typeof(PartAssemblyConstraint), nameof(PartAssemblyConstraint._fromPart));
            private static string toPartName = CommonFun.Name(typeof(PartAssemblyConstraint), nameof(PartAssemblyConstraint._toPart));

            /// <summary>
            /// 初始化
            /// </summary>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                fromPartSP = property.FindPropertyRelative(nameof(PartAssemblyConstraint._fromPart));
                toPartSP = property.FindPropertyRelative(nameof(PartAssemblyConstraint._toPart));

                if (fromPartSP.objectReferenceValue is Part fromPart && fromPart && toPartSP.objectReferenceValue is Part toPart && toPart)
                {
                    indexContent.text = string.Format("{0}-{1}[{2}]{3}[{4}]", indexContent.text, fromPartName, fromPart.firstReplacePartTagValue, toPartName, toPart.firstReplacePartTagValue);
                }
            }

            /// <summary>
            /// 获取行数：包含标题
            /// </summary>
            /// <returns></returns>
            public int GetRowCount(bool display) => display ? 3 : 1;

            /// <summary>
            /// 当绘制GUI
            /// </summary>
            /// <param name="display"></param>
            /// <param name="inRect"></param>
            /// <param name="label"></param>
            /// <returns></returns>
            public virtual bool OnGUI(bool display, Rect inRect, GUIContent label)
            {
                // 标题
                var rect = new Rect(inRect.x, inRect.y, inRect.width, 18);

                GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
                label = isArrayElement ? indexContent : label;
                display = GUI.Toggle(rect, display, label, EditorStyles.foldout);
                if (display)
                {
                    // 匹配规则
                    rect.xMin += 18;

                    rect = PropertyDrawerHelper.DrawProperty(rect, fromPartSP);
                    rect = PropertyDrawerHelper.DrawProperty(rect, toPartSP);
                }

                return display;
            }
        }

        /// <summary>
        /// 获取对象绘制高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (base.GetPropertyHeight(property, label) + 2) * cache.GetData(property).GetRowCount(property.isExpanded);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(rect, label, property);
            property.isExpanded = cache.GetData(property).OnGUI(property.isExpanded, rect, label);
            EditorGUI.EndProperty();
        }
    }
}