using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using static XCSJ.PluginTools.Items.ColliderTrigger;
using static XCSJ.PluginTools.Items.GameObjectComparer;

namespace XCSJ.EditorTools.Items
{
    /// <summary>
    /// 游戏对象比较信息绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(GameObjectCompareInfo), true)]
    public class GameObjectCompareInfoDrawer : PropertyDrawerAsArrayElement<GameObjectCompareInfoDrawer.Data>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public class Data : ArrayElementData
        {
            private SerializedProperty collisionCompareRuleSP;
            private SerializedProperty compereConditionSP;

            private SerializedProperty collidersSP;
            private SerializedProperty gameObjectTagsSP;
            private SerializedProperty gameObjectLayerSP;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                collisionCompareRuleSP = property.FindPropertyRelative(nameof(GameObjectCompareInfo._collisionCompareRule));
                compereConditionSP = property.FindPropertyRelative(nameof(GameObjectCompareInfo._compereCondition));

                collidersSP = property.FindPropertyRelative(nameof(GameObjectCompareInfo._gameObjects));
                gameObjectTagsSP = property.FindPropertyRelative(nameof(GameObjectCompareInfo._gameObjectTags));
                gameObjectLayerSP = property.FindPropertyRelative(nameof(GameObjectCompareInfo._gameObjectLayer));
            }

            /// <summary>
            /// 获取绘制的行数:包含标题
            /// </summary>
            /// <returns></returns>
            public virtual int GetRowCount(bool display)
            {
                int count = 1;
                if (display)
                {
                    count += 2;

                    switch ((EGameObjectCompareRule)collisionCompareRuleSP.intValue)
                    {
                        case EGameObjectCompareRule.GameObjectList:
                            {
                                if (collidersSP.isExpanded)
                                {
                                    count += 3 + collidersSP.arraySize;
                                }
                                else
                                {
                                    count += 1;
                                }
                                break;
                            }
                        case EGameObjectCompareRule.GameObjectTags:
                            {
                                if (gameObjectTagsSP.isExpanded)
                                {
                                    count += 3 + gameObjectTagsSP.arraySize;
                                }
                                else
                                {
                                    count += 1;
                                }
                                break;
                            }

                    }
                }
                return count;
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
                    // 匹配规则
                    rect.xMin += 18;

                    rect = PropertyDrawerHelper.DrawProperty(rect, collisionCompareRuleSP);
                    rect = PropertyDrawerHelper.DrawProperty(rect, compereConditionSP);

                    switch ((EGameObjectCompareRule)collisionCompareRuleSP.intValue)
                    {
                        case EGameObjectCompareRule.GameObjectList:
                            {
                                rect = PropertyDrawerHelper.DrawProperty(rect, collidersSP, true);
                                break;
                            }
                        case EGameObjectCompareRule.GameObjectTags:
                            {
                                rect = PropertyDrawerHelper.DrawProperty(rect, gameObjectTagsSP, true);
                                break;
                            }
                        case EGameObjectCompareRule.GameObjectLayer:
                            {
                                rect = PropertyDrawerHelper.DrawProperty(rect, gameObjectLayerSP, true);
                                break;
                            }
                    }
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
            return (base.GetPropertyHeight(property, label) + 3) * cache.GetData(property).GetRowCount(property.isExpanded);
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
