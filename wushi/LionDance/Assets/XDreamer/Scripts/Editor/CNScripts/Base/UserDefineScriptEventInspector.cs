using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCNScripts.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.EditorCNScripts.Base
{
    /// <summary>
    /// 用户自定义脚本事件检查器
    /// </summary>
    [CustomEditor(typeof(UserDefineScriptEvent))]
    [Name("用户自定义脚本事件检查器")]
    public class UserDefineScriptEventInspector : BaseScriptEventInspector<UserDefineScriptEvent, EUserDefineScriptEventType, UserDefineScriptEventFunction, UserDefineScriptEventFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(XDreamerMenu.ScriptEvent + UserDefineScriptEvent.Title, false)]
        public static void CreateScriptEvent() => CreateComponentInternal();

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(XDreamerMenu.ScriptEvent + UserDefineScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent() => ValidateCreateComponentInternal();
    }

    /// <summary>
    /// 用户自定义脚本事件集合属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(UserDefineScriptEventFunctionCollection), true)]
    public class UserDefineScriptEventCollectionPropertyDrawer : EnumFuncCollectionPropertyDrawer
    {
        /// <summary>
        /// 当绘制函数列表之前
        /// </summary>
        /// <param name="funcCollectionSP"></param>
        /// <param name="functionsSP"></param>
        protected override void OnBeforeDrawFunctions(SerializedProperty funcCollectionSP, SerializedProperty functionsSP)
        {
            //base.OnBeforeDrawFunctions(functionsSP);
            EditorGUILayout.SelectableLabel("目前最多支持 " + EnumCache<EUserDefineScriptEventType>.Array.Length.ToString() + " 个自定义事件", UICommonOption.labelGreenBG);
            if (enumType == null)
            {
                var enumValueSP = funcCollectionSP.FindPropertyRelative(nameof(UserDefineScriptEventFunctionCollection._enumValue));
                var enumValueData = PropertyData.GetPropertyData(enumValueSP);
                enumType = enumValueData.fieldInfo.FieldType;
            }
            DrawFunctionNameAndAdd(funcCollectionSP, functionsSP, nameof(UserDefineScriptEventFunctionCollection._functionName), functionNameNew => AddFunction(functionsSP, functionNameNew, out _));
        }

        /// <summary>
        /// 添加函数
        /// </summary>
        /// <param name="functionsSP"></param>
        /// <param name="functionName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected SerializedProperty AddFunction(SerializedProperty functionsSP, string functionName, out int index)
        {
            //已经存在同名函数
            var functionSP = functionsSP.GetArrayElement(out index, sp => sp.FindPropertyRelative(nameof(Function.name)).stringValue == functionName);
            if (functionSP != null)
            {
                functionSP.FindPropertyRelative(nameof(Function.Enable)).boolValue = true;
                return functionSP;
            }

            //查找第一个禁用的
            functionSP = functionsSP.GetArrayElement(out index, sp => !sp.FindPropertyRelative(nameof(Function.Enable)).boolValue);
            if (functionSP != null)
            {
                functionSP.FindPropertyRelative(nameof(Function.Enable)).boolValue = true;
                functionSP.FindPropertyRelative(nameof(Function.name)).stringValue = functionName;
                return functionSP;
            }

            //创建
            foreach(var e in EnumCache<EUserDefineScriptEventType>.Array)
            {
                var functionSPTmp = functionsSP.GetArrayElement(out index, sp => sp.FindPropertyRelative(nameof(UserDefineScriptEventFunction.type)).intValue == (int)e);
                if (functionSPTmp == null)
                {
                    functionSP = functionsSP.AddArrayElement();

                    functionSP.FindPropertyRelative(nameof(Function.Active)).boolValue = true;
                    functionSP.FindPropertyRelative(nameof(Function.Enable)).boolValue = true;
                    functionSP.FindPropertyRelative(nameof(Function.name)).stringValue = functionName;
                    functionSP.FindPropertyRelative(nameof(UserDefineScriptEventFunction.type)).intValue = (int)e;

                    break;
                }
            }

            return functionSP;
        }

        /// <summary>
        /// 获取函数名称内容
        /// </summary>
        /// <param name="functionSP"></param>
        /// <returns></returns>
        public override GUIContent GetFunctionNameContent(SerializedProperty functionSP) => CommonFun.TempContent(functionSP.FindPropertyRelative(nameof(UserDefineScriptEventFunction.name)).stringValue);
    }
}
