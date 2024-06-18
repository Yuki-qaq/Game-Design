using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Items;
using static XCSJ.PluginTools.GameObjects.GrabbableModel;

namespace XCSJ.EditorTools.GameObjects
{
    /// <summary>
    /// 游戏对象视图列表检查器
    /// </summary>
    [Name("游戏对象视图列表检查器")]
    [CustomEditor(typeof(GrabbableList), true)]
    public class GrabbableListInspector : InteractorInspector<GrabbableList>
    {
        /// <summary>
        /// 游戏对象列表操作
        /// </summary>
        [Name("游戏对象列表操作")]
        public bool gameObjectListOperation;

        /// <summary>
        /// 添加选中游戏对象到列表中
        /// </summary>
        [Name("添加选中游戏对象到列表中")]
        public string addSelectedGameObjectsToList;

        private static int gameObjectCount = 3;

        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [Languages.LanguageTuple("Count","数量")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(GrabbableList._grabbableModels):
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(TrLabel(nameof(gameObjectListOperation)));
                        if (GUILayout.Button(TrLabel(nameof(addSelectedGameObjectsToList))))
                        {
                            targetObject.XModifyProperty(() =>
                            {
                                foreach (var go in Selection.gameObjects)
                                {
                                    var grabbable = go.XGetOrAddComponent<Grabbable>();
                                    if (grabbable)
                                    {
                                        targetObject._grabbableModels.Add(new SerializableGrabbableModel(grabbable, gameObjectCount));
                                    }
                                }
                            });
                        }
                        GUILayout.Label(Tr("Count"));
                        gameObjectCount = EditorGUILayout.IntField(gameObjectCount, GUILayout.Width(32));
                        if (gameObjectCount<1)
                        {
                            gameObjectCount = 1;
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                default:
                    break;
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        /// <summary>
        /// 获取帮助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            var dragger = UnityObjectExtension.XGetComponentInGlobal<Dragger>();
            if (!dragger)
            {
                info.AppendLine("<color=red>场景中缺少【一键拖拽工具】，本组件需要依赖该工具才能正常工作！</color>");
            }
            return info;
        }
    }

    /// <summary>
    /// 序列化的可抓模型绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableGrabbableModel), true)]
    public class SerializableGrabbableModelDrawer : PropertyDrawerAsArrayElement<SerializableGrabbableModelDrawer.Data>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public class Data : ArrayElementData
        {
            private SerializedProperty unityObjectSP;
            private SerializedProperty titleSP;
            private SerializedProperty texture2DSP;

            private SerializedProperty dataSourceSP;
            private SerializedProperty allowCloneMaxCountSP;
            private SerializedProperty enableMakeGroupSP;
            private SerializedProperty handleRuleSP;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                unityObjectSP = property.FindPropertyRelative(nameof(SerializableGrabbableModel._unityObject));
                titleSP = property.FindPropertyRelative(nameof(SerializableGrabbableModel._title));
                texture2DSP = property.FindPropertyRelative(nameof(SerializableGrabbableModel._texture2D));

                dataSourceSP = property.FindPropertyRelative(nameof(SerializableGrabbableModel._dataSource));
                allowCloneMaxCountSP = property.FindPropertyRelative(nameof(SerializableGrabbableModel._allowCloneMaxCount));

                enableMakeGroupSP = property.FindPropertyRelative(nameof(SerializableGrabbableModel._enableMakeGroup));
                handleRuleSP = property.FindPropertyRelative(nameof(SerializableGrabbableModel._handleRule));
            }

            /// <summary>
            /// 获取绘制的行数:包含标题
            /// </summary>
            /// <returns></returns>
            public virtual int GetRowCount(bool display) => display ? 7 : 1;

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

                    rect = PropertyDrawerHelper.DrawProperty(rect, unityObjectSP);
                    rect = PropertyDrawerHelper.DrawProperty(rect, titleSP);
                    rect = PropertyDrawerHelper.DrawProperty(rect, texture2DSP);
                    rect = PropertyDrawerHelper.DrawProperty(rect, dataSourceSP);
                    var dataSource = (EDataSource)dataSourceSP.intValue;
                    if (dataSource == EDataSource.UseClone)
                    {
                        rect = PropertyDrawerHelper.DrawProperty(rect, allowCloneMaxCountSP);
                    }
                    else if(dataSource == EDataSource.UsePrototype)
                    {
                        rect = PropertyDrawerHelper.DrawProperty(rect, enableMakeGroupSP);
                    }
                    rect = PropertyDrawerHelper.DrawProperty(rect, handleRuleSP);
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
