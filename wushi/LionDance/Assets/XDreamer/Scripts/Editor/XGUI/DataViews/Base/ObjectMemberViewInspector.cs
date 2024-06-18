using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.EditorXGUI.Base;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.DataViews;
using XCSJ.PluginXGUI.DataViews.Base;
using static XCSJ.PluginXGUI.DataViews.DataViewHelper;

namespace XCSJ.EditorXGUI.DataViews.Base
{
    /// <summary>
    /// 对象成员视图检查器
    /// </summary>
    [Name("对象成员视图检查器")]
    [CustomEditor(typeof(ObjectMemberView), true)]
    [CanEditMultipleObjects]
    public class ObjectMemberViewInspector : BaseModelViewInspector
    {
        /// <summary>
        /// 成员绘制
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [LanguageTuple("Collect All Children Model View Components", "收集所有子级模型视图组件")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(ObjectMemberView._modelViews):
                    {
                        if (GUILayout.Button(Tr("Collect All Children Model View Components")))
                        {
                            var gameObject = targetObject.gameObject;
                            var modelViews = gameObject.GetComponentsInChildren<BaseModelView>();
                            foreach (var mv in modelViews)
                            {
                                if (mv.gameObject != gameObject)
                                {
                                    serializedProperty.AddArrayElement(mv);
                                }
                            }
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 重新查找分类列表
        /// </summary>
        /// <returns></returns>
        protected override CategoryList RefindCategoryList()
        {
            var view = targetObject;
            if (!view) return default;

            var modelValueType = view.modelValueType ?? typeof(object);
            var viewValueType = view.viewValueType ?? typeof(object);

            return EditorToolsHelper.GetWithPurposes((c, b) =>
            {
                if (b is ComponentToolItem componentToolItem)
                {
                    if (typeof(BaseObjectMemberViewModelProvider).IsAssignableFrom(componentToolItem.type)) return true;
                    return DataConverterCache.Get(modelValueType, viewValueType, componentToolItem.type).CanI2OOrO2I;
                }
                return false;
            }, nameof(BaseDataConverter), nameof(ObjectMemberView));
        }
    }
}
