using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginTools.CrossSectionShader;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.EditorXGUI;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;

namespace XCSJ.EditorTools.CrossSectionShader
{
    /// <summary>
    /// 剖面控制器检查器
    /// </summary>
    [Name("剖面控制器检查器")]
    [CustomEditor(typeof(PlaneCuttingController))]
    public class PlaneCuttingControllerInspector : MBInspector<PlaneCuttingController>
    {
        /// <summary>
        /// 创建剖面控制器
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("剖面控制器")]
        [XCSJ.Attributes.Icon(EIcon.CrossSection)]
        [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
        [Tool(ToolsExtensionCategory.Model, rootType = typeof(ToolsExtensionManager), groupRule = EToolGroupRule.None)]
        public static void CreatePlaneCuttingController(ToolContext toolContext)
        {
            EditorXGUIHelper.CreateEventSystem();

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("剖面控制器.prefab"));
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!targetObject.cuttingMaterail)
            {
                targetObject.cuttingMaterail = GetCuttingMaterialPath();
            }

            if (!targetObject.unionCuttingMaterail)
            {
                targetObject.unionCuttingMaterail = GetUnionCuttingMaterialPath();
            }
        }

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(PlaneCuttingController.cuttingMaterail):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorGUI.BeginDisabledGroup(targetObject.cuttingMaterail);
                        if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            serializedProperty.objectReferenceValue = GetCuttingMaterialPath();
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                case nameof(PlaneCuttingController.unionCuttingMaterail):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorGUI.BeginDisabledGroup(targetObject.unionCuttingMaterail);
                        if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            serializedProperty.objectReferenceValue = GetUnionCuttingMaterialPath();
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                case nameof(PlaneCuttingController.includeChildren):
                    {
                        if (GUILayout.Button(new GUIContent("移动到=>包围盒中心", "将剖面控制器移动到剖切对象包围盒中心处")))
                        {
                            if (CommonFun.GetBounds(out Bounds bounds, targetObject.cuttedObjects, targetObject.includeChildren, targetObject.includeInactiveGameObject, targetObject.includeDisableRenderer))
                            {
                                targetObject.transform.position = bounds.center;
                            }
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private Material GetCuttingMaterialPath()
        {
            return UICommonFun.LoadFromAssets<Material>("Assets/XDreamer-Assets/基础/Materials/CrossSection/GenericThreePlanesBSP.mat");
        }

        private Material GetUnionCuttingMaterialPath()
        {
            return UICommonFun.LoadFromAssets<Material>("Assets/XDreamer-Assets/基础/Materials/CrossSection/UnionGenericThreePlanesBSP.mat");
        }
    }
}
