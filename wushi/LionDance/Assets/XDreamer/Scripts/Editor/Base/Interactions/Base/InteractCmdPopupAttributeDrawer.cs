using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;

namespace XCSJ.EditorExtension.Base.Interactions.Base
{
    /// <summary>
    /// 交互命令弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(InteractCmdPopupAttribute))]
    public class InteractCmdPopupAttributeDrawer : PropertyDrawer<InteractCmdPopupAttribute>
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String && property.serializedObject.targetObject is IInteractCmdHost interactCmdHost)
            {
                label = EditorGUI.BeginProperty(rect, label, property);
                EditorGUI.BeginChangeCheck();

                interactCmdHost.TryGetFriendlyCmdByCmd(property.stringValue, out var friendlyCmd);

                friendlyCmd = UICommonFun.Popup(rect, label, friendlyCmd, interactCmdHost.GetFriendlyCmds().ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    if (interactCmdHost.TryGetCmdByFriendlyCmd(friendlyCmd, out var cmd))
                    {
                        property.stringValue = cmd;
                    }
                }
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(rect, property, label);
            }
        }

    }
}

