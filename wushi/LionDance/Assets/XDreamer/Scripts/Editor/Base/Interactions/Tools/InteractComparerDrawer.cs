using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.EditorExtension.Base.Interactions.Tools
{
    #region 比较数据绘制器

    /// <summary>
    /// 比较数据绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(CompareData), true)]
    public class CompareDataDrawer : PropertyDrawerAsArrayElement<CompareDataDrawer.CompareDataData>
    {
        private static List<string> emptyCmds = new List<string>();

        /// <summary>
        /// 绘制命令名称
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="interactor"></param>
        /// <param name="useOutCmdName"></param>
        /// <returns></returns>
        public static Rect DrawCmdName(Rect rect, SerializedProperty property, InteractObject interactor, bool useOutCmdName = true)
        {
            rect.y += PropertyDrawerHelper.singleLineHeight;
            DrawStringPropertyWithCmdNameList(rect, property, interactor ? (useOutCmdName ? interactor.outCmdNameList : interactor.inCmdNameList) : emptyCmds);
            return rect;
        }

        private const int CmdDropdownWith = 80;

        /// <summary>
        /// 使用交互器上有的交互命令来设置命令名称
        /// </summary>
        public static void DrawStringPropertyWithCmdNameList(Rect rect, SerializedProperty property, List<string> cmds)
        {
            var strPropertyValue = (EPropertyValueType)property.FindPropertyRelative(nameof(StringPropertyValue._propertyValueType)).intValue;

            // 字符串属性等于值时，才启用下拉列表
            if (strPropertyValue == EPropertyValueType.Value)
            {
                // 绘制原有属性
                rect.width -= CmdDropdownWith;
                EditorGUI.PropertyField(rect, property, PropertyData.GetPropertyData(property).trLabel);

                // 绘制右侧下拉列表
                var valueSP = property.FindPropertyRelative(nameof(StringPropertyValue._value));
                var index = cmds.IndexOf(valueSP.stringValue);

                rect.x += rect.width;
                rect.width = CmdDropdownWith;

                var newIndex = EditorGUI.Popup(rect, index, cmds.ToArray());
                if (newIndex != index)
                {
                    valueSP.stringValue = cmds[newIndex];
                }
            }
            else
            {
                EditorGUI.PropertyField(rect, property, PropertyData.GetPropertyData(property).trLabel);
            }
        }

        /// <summary>
        /// 比较数据数据
        /// </summary>
        public class CompareDataData : ArrayElementData
        {
            private SerializedProperty currentSP;
            private SerializedProperty compareObjectTypeSP;
            private SerializedProperty compereConditionSP;

            private SerializedProperty cmdNameSP;
            private SerializedProperty tagvalueSP;
            private SerializedProperty cmdParamStringSP;
            private SerializedProperty cmdParamExpressionStringKeySP;
            private SerializedProperty cmdParamExpressionStringValueSP;
            private SerializedProperty cmdParamUnityObjectSP;
            private SerializedProperty interactorSP;
            private SerializedProperty interactableSP;

            private static InteractObject staticInteractor = null;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                currentSP = property;

                compereConditionSP = property.FindPropertyRelative(nameof(BaseCompareData._compereCondition));
                compareObjectTypeSP = property.FindPropertyRelative(nameof(BaseCompareData._compareObjectType));

                cmdNameSP = property.FindPropertyRelative(nameof(BaseCompareData._outCmdName));
                tagvalueSP = property.FindPropertyRelative(nameof(BaseCompareData._tagvalue));
                cmdParamStringSP = property.FindPropertyRelative(nameof(BaseCompareData._cmdParamString));
                cmdParamExpressionStringKeySP = property.FindPropertyRelative(nameof(BaseCompareData._cmdParamExpressionStringKey));
                cmdParamExpressionStringValueSP = property.FindPropertyRelative(nameof(BaseCompareData._cmdParamExpressionStringValue));
                cmdParamUnityObjectSP = property.FindPropertyRelative(nameof(BaseCompareData._cmdParamUnityObject));
                interactorSP = property.FindPropertyRelative(nameof(CompareData._interactor));
                interactableSP = property.FindPropertyRelative(nameof(CompareData._interactable));
            }

            /// <summary>
            /// 获取绘制的行数:包含标题
            /// </summary>
            /// <returns></returns>
            public virtual int GetRowCount(bool display)
            {
                if (display)
                {
                    return (ECompareObjectType)compareObjectTypeSP.intValue == ECompareObjectType.CmdParamExpressionString ? 5 : 4;
                }
                return 1;
            }

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

                display = GUI.Toggle(rect, display, isArrayElement ? indexContent : label, EditorStyles.foldout);
                if (display)
                {
                    rect.xMin += 18;

                    label = EditorGUI.BeginProperty(rect, label, currentSP);
                    {
                        rect = PropertyDrawerHelper.DrawProperty(rect, compareObjectTypeSP);
                        rect = PropertyDrawerHelper.DrawProperty(rect, compereConditionSP);

                        switch ((ECompareObjectType)compareObjectTypeSP.intValue)
                        {
                            case ECompareObjectType.Interactor:
                                {
                                    rect = PropertyDrawerHelper.DrawProperty(rect, interactorSP);
                                    var interactor = interactorSP.objectReferenceValue as InteractObject;
                                    if (interactor)
                                    {
                                        staticInteractor = interactor;
                                    }
                                    break;
                                }
                            case ECompareObjectType.CmdName:
                                {
                                    // 同层级中的交互器对象
                                    InteractObject interactor = null;

                                    var compereDatas = currentSP.serializedObject.FindProperty(nameof(InteractComparer._compereDatas));
                                    if (compereDatas == null)
                                    {
                                        compereDatas = currentSP.serializedObject.FindProperty("_interactComparer")?.FindPropertyRelative(nameof(InteractComparer._compereDatas));
                                    }

                                    if (compereDatas != null)
                                    {
                                        for (int i = 0; i < compereDatas.arraySize; i++)
                                        {
                                            var item = compereDatas.GetArrayElementAtIndex(i);
                                            var compareType = (ECompareObjectType)item.FindPropertyRelative(nameof(BaseCompareData._compareObjectType)).intValue;
                                            if (compareType == ECompareObjectType.Interactor)
                                            {
                                                interactor = item.FindPropertyRelative(nameof(CompareData._interactor)).objectReferenceValue as InteractObject;
                                            }
                                        }
                                    }

                                    // 使用同层级列表中查找比较对象为交互器的数据，并获取交互器输出命令作为当前对象的绘制
                                    rect = DrawCmdName(rect, cmdNameSP, interactor ? interactor : staticInteractor);
                                    break;
                                }
                            case ECompareObjectType.Interactable:
                                {
                                    rect = PropertyDrawerHelper.DrawProperty(rect, interactableSP);
                                    break;
                                }
                            case ECompareObjectType.TagValue:
                                {
                                    rect = PropertyDrawerHelper.DrawProperty(rect, tagvalueSP);
                                    break;
                                }
                            case ECompareObjectType.CmdParamString:
                                {
                                    rect = PropertyDrawerHelper.DrawProperty(rect, cmdParamStringSP);
                                    break;
                                }
                            case ECompareObjectType.CmdParamExpressionString:
                                {
                                    rect = PropertyDrawerHelper.DrawProperty(rect, cmdParamExpressionStringKeySP);
                                    rect = PropertyDrawerHelper.DrawProperty(rect, cmdParamExpressionStringValueSP);
                                    break;
                                }
                            case ECompareObjectType.CmdParamExpressionStringKey:
                                {
                                    rect = PropertyDrawerHelper.DrawProperty(rect, cmdParamExpressionStringKeySP);
                                    break;
                                }
                            case ECompareObjectType.CmdParamExpressionStringValue:
                                {
                                    rect = PropertyDrawerHelper.DrawProperty(rect, cmdParamExpressionStringValueSP);
                                    break;
                                }
                            case ECompareObjectType.CmdParamUnityObject:
                                {
                                    rect = PropertyDrawerHelper.DrawProperty(rect, cmdParamUnityObjectSP);
                                    break;
                                }
                        }

                    }
                    EditorGUI.EndProperty();
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

    #endregion

    #region 交互比较数据

    /// <summary>
    /// 交互比较数据
    /// </summary>
    public class InteractComparerData : ArrayElementData
    {
        private SerializedProperty interactStateSP;
        private SerializedProperty compareDataListRuleSP;
        private SerializedProperty compereDatasSP;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="property"></param>
        public override void Init(SerializedProperty property)
        {
            base.Init(property);

            interactStateSP = property.FindPropertyRelative(nameof(InteractComparer._interactState));
            compareDataListRuleSP = property.FindPropertyRelative(nameof(InteractComparer._compareDataListRule));
            compereDatasSP = property.FindPropertyRelative(nameof(InteractComparer._compereDatas));
        }

        /// <summary>
        /// 获取绘制的行数
        /// </summary>
        /// <returns></returns>
        public virtual int GetRowCount(bool display)
        {
            int rowCount = 1;// 包含标题

            if (display)
            {
                rowCount += 3;
                // 计算比较数据列表所占行数
                if (compereDatasSP.isExpanded)
                {
                    var arraySize = compereDatasSP.arraySize;

                    if (arraySize == 0)
                    {
#if UNITY_2020_3_OR_NEWER
                        rowCount += 2;
#endif
                    }
                    else
                    {
                        for (int i = 0; i < arraySize; i++)
                        {
                            rowCount += (compereDatasSP.GetArrayElementAtIndex(i).isExpanded ? 4 : 1);
                        }
                    }
                    rowCount += 2;
                }
            }

            return rowCount;
        }

        /// <summary>
        /// 绘制回调
        /// </summary>
        /// <param name="parentSP"></param>
        /// <param name="inRect"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual Rect OnGUI(SerializedProperty parentSP, Rect inRect, GUIContent label)
        {
            // 标题
            var rect = new Rect(inRect.x, inRect.y, inRect.width, 18);

            GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
            parentSP.isExpanded = GUI.Toggle(rect, parentSP.isExpanded, isArrayElement ? indexContent : label, EditorStyles.foldout);
            if (parentSP.isExpanded)
            {
                rect.xMin += 18;

                rect = PropertyDrawerHelper.DrawProperty(rect, interactStateSP);
                rect = PropertyDrawerHelper.DrawProperty(rect, compareDataListRuleSP);
                rect = PropertyDrawerHelper.DrawProperty(rect, compereDatasSP, true);

                // 计算列表展开项
                if (compereDatasSP.isExpanded)
                {
                    var arraySize = compereDatasSP.arraySize;

                    if (arraySize == 0)
                    {
#if UNITY_2020_3_OR_NEWER
                        rect.y += PropertyDrawerHelper.singleLineHeight * 2;
#endif
                    }
                    else
                    {
                        for (int i = 0; i < arraySize; i++)
                        {
                            var item = compereDatasSP.GetArrayElementAtIndex(i);
                            rect.y += PropertyDrawerHelper.singleLineHeight * (item.isExpanded ? 4 : 1);
                        }
                    }

                    rect.y += PropertyDrawerHelper.singleLineHeight * 2;
                }
            }
            return rect;
        }
    }

    #endregion

    #region 交互信息绘制器

    /// <summary>
    /// 交互信息绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ExecuteInteractInfo))]
    public class ExecuteInteractInfoDrawer : PropertyDrawerAsArrayElement<ExecuteInteractInfoDrawer.Data>
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public class Data : ArrayElementData
        {
            internal SerializedProperty interactorSP;
            internal SerializedProperty inCmdNameSP;
            internal SerializedProperty interactableEntitySP;
            internal SerializedProperty cmdParamRuleSP;
            internal SerializedProperty cmdParamStringSP;
            internal SerializedProperty cmdParamExpressionStringKeySP;
            internal SerializedProperty cmdParamExpressionStringValueSP;
            internal SerializedProperty unityObjectSP;

            /// <summary>
            /// 显示
            /// </summary>
            public bool display = true;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                interactorSP = property.FindPropertyRelative(nameof(ExecuteInteractInfo._interactor));
                inCmdNameSP = property.FindPropertyRelative(nameof(ExecuteInteractInfo._inCmdName));
                interactableEntitySP = property.FindPropertyRelative(nameof(ExecuteInteractInfo._interactableEntity));
                cmdParamRuleSP = property.FindPropertyRelative(nameof(ExecuteInteractInfo._cmdParamRule));
                cmdParamStringSP = property.FindPropertyRelative(nameof(ExecuteInteractInfo._cmdParamString));
                cmdParamExpressionStringKeySP = property.FindPropertyRelative(nameof(ExecuteInteractInfo._cmdParamExpressionStringKey));
                cmdParamExpressionStringValueSP = property.FindPropertyRelative(nameof(ExecuteInteractInfo._cmdParamExpressionStringValue));
                unityObjectSP = property.FindPropertyRelative(nameof(ExecuteInteractInfo._unityObject));
            }

            /// <summary>
            /// 绘制回调
            /// </summary>
            /// <param name="inRect"></param>
            /// <param name="label"></param>
            /// <returns></returns>
            public virtual Rect OnGUI(Rect inRect, GUIContent label)
            {
                // 标题
                var rect = new Rect(inRect.x, inRect.y, inRect.width, 18);

                GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
                display = GUI.Toggle(rect, display, isArrayElement ? indexContent : label, EditorStyles.foldout);
                if (!display) return rect;

                // 匹配规则
                rect.xMin += 18;
                rect = PropertyDrawerHelper.DrawProperty(rect, interactorSP);
                rect = CompareDataDrawer.DrawCmdName(rect, inCmdNameSP, interactorSP.objectReferenceValue as InteractObject, false);
                rect = PropertyDrawerHelper.DrawProperty(rect, interactableEntitySP);
                rect = PropertyDrawerHelper.DrawProperty(rect, cmdParamRuleSP);
                switch ((ECmdParamRule)cmdParamRuleSP.intValue)
                {
                    case ECmdParamRule.String:
                        {
                            rect = PropertyDrawerHelper.DrawProperty(rect, cmdParamStringSP);
                            break;
                        }
                    case ECmdParamRule.ExpressionString:
                        {
                            rect = PropertyDrawerHelper.DrawProperty(rect, cmdParamExpressionStringKeySP);
                            rect = PropertyDrawerHelper.DrawProperty(rect, cmdParamExpressionStringValueSP);
                            break;
                        }
                    case ECmdParamRule.ExpressionValueUseInputSource:
                        {
                            rect = PropertyDrawerHelper.DrawProperty(rect, cmdParamExpressionStringKeySP);
                            break;
                        }
                    case ECmdParamRule.UnityObject:
                        {
                            rect = PropertyDrawerHelper.DrawProperty(rect, unityObjectSP);
                            break;
                        }
                }
                return rect;
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
            int rowCount = 1;
            var data = cache.GetData(property) as ExecuteInteractInfoDrawer.Data;
            if (data.display)
            {
                rowCount += 4;
            }

            switch ((ECmdParamRule)data.cmdParamRuleSP.intValue)
            {
                case ECmdParamRule.String:
                case ECmdParamRule.ExpressionValueUseInputSource:
                case ECmdParamRule.UnityObject:
                    {
                        ++rowCount;
                        break;
                    }
                case ECmdParamRule.ExpressionString:
                    {
                        rowCount += 2;
                        break;
                    }
            }

            return (base.GetPropertyHeight(property, label) + 2) * rowCount;
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            cache.GetData(property).OnGUI(rect, label);
        }
    }

    #endregion
}
