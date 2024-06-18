using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Input;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.States.Nodes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginRepairman.States.RepairTask;

namespace XCSJ.EditorRepairman.Inspectors
{
    /// <summary>
    /// 拆装任务零件视图检查器
    /// </summary>
    [Name("拆装任务零件视图检查器")]
    [CustomEditor(typeof(RepairTaskWorkPartView), true)]
    public class RepairTaskWorkPartViewInspector : StateComponentInspector<RepairTaskWorkPartView>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // 根据选择零件动态添加步骤
            if (GUILayout.Button(new GUIContent(string.Format("添加选中的零件为[{0}]", RepairStepByMatchPosition.Title), EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                foreach (var node in NodeSelection.selections)
                {
                    if (node is StateNode sn)
                    {
                        var s = sn.state;
                        if (s)
                        {
                            var part = s.GetComponent<Part>();
                            if (part)
                            {
                                var stepState = RepairStepByMatchPosition.Create(stateComponent.parent);
                                var step = stepState.GetComponent<RepairStep>();
                                step.XModifyProperty(() => step.selectedParts.Add(part));
                            }
                        }
                    }
                }
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
                case nameof(RepairTaskWorkPartView._repairAssistant):
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            base.OnDrawMember(serializedProperty, propertyData);
                            EditorGUI.BeginDisabledGroup(stateComponent._repairAssistant);
                            {
                                if (GUILayout.Button(new GUIContent(EditorIconHelper.GetIconInLib(EIcon.Add), ""), UICommonOption.WH32x16))
                                {
                                    var dev = UnityObjectExtension.GetComponentInGlobal<XCSJ.PluginRepairman.Tools.Device>();
                                    if (dev)
                                    {
                                        stateComponent.XModifyProperty(() => stateComponent._repairAssistant = dev.XGetOrAddComponent<RepairAssistant>());
                                    }
                                }
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
