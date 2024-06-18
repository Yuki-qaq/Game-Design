using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTimelines.UI;
using static XCSJ.Extension.Base.Interactions.Tools.InteractorInput;

namespace XCSJ.EditorExtension.Base.Interactions.Tools
{

    /// <summary>
    /// 交互器检查器
    /// </summary>
    [CustomEditor(typeof(Interactor), true)]
    public class InteractorInspector : InteractorInspector<Interactor>
    {
    }


    /// <summary>
    /// 交互器检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InteractorInspector<T> : ExtensionalInteractObjectInspector<T> where T : Interactor
    {
    }

    /// <summary>
    /// 交互器输入绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(InteractorInput))]
    public class InteractorInputDrawer : PropertyDrawerAsArrayElement<InteractorInputDrawer.Data>
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public class Data : InteractComparerData
        {
            /// <summary>
            /// 匹配处理规则
            /// </summary>
            public SerializedProperty matchHandleRuleSP;

            /// <summary>
            /// 替换命令
            /// </summary>
            public SerializedProperty replaceCmdSP;

            /// <summary>
            /// 替换命令
            /// </summary>
            public SerializedProperty replaceCmdParamSP;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                matchHandleRuleSP = property.FindPropertyRelative(nameof(InteractorInput._matchHandleRule));
                replaceCmdSP = property.FindPropertyRelative(nameof(InteractorInput._repalceCmdName));
                replaceCmdParamSP = property.FindPropertyRelative(nameof(InteractorInput._repalceCmdParam));
            }

            /// <summary>
            /// 获取行数
            /// </summary>
            /// <param name="display"></param>
            /// <returns></returns>
            public override int GetRowCount(bool display)
            {
                var count = base.GetRowCount(display);
                if (display)
                {
                    ++count;
                    switch ((EMatchHandleRule)matchHandleRuleSP.intValue)
                    {
                        case EMatchHandleRule.ReplaceCmd:
                        case EMatchHandleRule.ReplaceCmdParam:
                            {
                                ++count;
                                break;
                            }
                        case EMatchHandleRule.ReplaceCmdAndCmdParam:
                            {
                                count += 2;
                                break;
                            }
                    }
                }
                return count;
            }

            /// <summary>
            /// 当绘制GUI
            /// </summary>
            /// <param name="parentProperty"></param>
            /// <param name="inRect"></param>
            /// <param name="label"></param>
            /// <returns></returns>
            public override Rect OnGUI(SerializedProperty parentProperty, Rect inRect, GUIContent label)
            {
                var rect = base.OnGUI(parentProperty, inRect, label);

                if (parentProperty.isExpanded)
                {
                    rect = PropertyDrawerHelper.DrawProperty(rect, matchHandleRuleSP);

                    // 替换命令
                    switch ((EMatchHandleRule)matchHandleRuleSP.intValue)
                    {
                        case EMatchHandleRule.ReplaceCmd:
                            {
                                CompareDataDrawer.DrawCmdName(rect, replaceCmdSP, replaceCmdSP.serializedObject.targetObject as InteractObject, false);
                                break;
                            }
                        case EMatchHandleRule.ReplaceCmdParam:
                            {
                                PropertyDrawerHelper.DrawProperty(rect, replaceCmdParamSP);
                                break;
                            }
                        case EMatchHandleRule.ReplaceCmdAndCmdParam:
                            {
                                rect = CompareDataDrawer.DrawCmdName(rect, replaceCmdSP, replaceCmdSP.serializedObject.targetObject as InteractObject, false);

                                PropertyDrawerHelper.DrawProperty(rect, replaceCmdParamSP);
                                break;
                            }
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
            var data = cache.GetData(property);
            if (data == null) return 6;
            return (base.GetPropertyHeight(property, label) + 2) * data.GetRowCount(property.isExpanded);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            cache.GetData(property).OnGUI(property, position, label);
        }
    }
}
