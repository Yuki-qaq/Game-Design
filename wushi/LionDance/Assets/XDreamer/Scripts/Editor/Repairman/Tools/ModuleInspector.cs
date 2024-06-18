using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman;
using XCSJ.PluginRepairman.Tools;

namespace XCSJ.EditorRepairman.Tools
{
    /// <summary>
    /// 模块检查器
    /// </summary>
    [Name("模块检查器")]
    [CustomEditor(typeof(Module), true)]
    public class ModuleInspector : PartInspector<Module>
    {
        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(Module._partAssemblyNodes):
                    {
                        DrawCreatePartData(targetObject);
                        break;
                    }
                case nameof(Module._partAssemblyConstraints):
                    {
                        DrawCreateAssemblyConstraints(targetObject);
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 绘制创建零件数据按钮
        /// </summary>
        /// <param name="module"></param>
        public static void DrawCreatePartData(Module module)
        {
            if (GUILayout.Button(new GUIContent(CommonFun.Name(typeof(Module), nameof(Module._partAssemblyNodes)), EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                module.ClearPartAssemblyNodes();
                module.CreatePartAssemblyNodes();
            }
        }

        /// <summary>
        /// 绘制创建约束数据按钮
        /// </summary>
        /// <param name="module"></param>
        public static void DrawCreateAssemblyConstraints(Module module)
        {
            if (GUILayout.Button(new GUIContent(CommonFun.Name(typeof(Module), nameof(Module._partAssemblyConstraints)), EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                module.ClearAssemblyConstraints();
                module.CreateAssemblyConstraints();
            }
        }
    }
}
