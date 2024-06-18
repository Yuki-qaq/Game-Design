using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.DataViews;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.EditorXGUI.DataViews.Base
{
    /// <summary>
    /// 基础数据视图检查器
    /// </summary>
    [Name("基础数据视图检查器")]
    [CustomEditor(typeof(BaseModelView), true)]
    [CanEditMultipleObjects]
    public class BaseModelViewInspector : BaseViewControllerInspector<BaseModelView>
    {
        private CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            FindCategoryList();
        }

        /// <summary>
        /// 成员绘制
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            base.OnDrawMember(serializedProperty, propertyData);
            switch (serializedProperty.name)
            {
                case nameof(BaseModelView._modelToViewConverter):
                    {
                        var view = targetObject;
                        if (view._modelToViewConverter && !view.TryGetValidModelToViewConverter(out _))
                        {
                            EditorGUILayout.HelpBox("无效的模型到视图转换器组件对象", MessageType.Error);
                        }
                        break;
                    }
                case nameof(BaseModelView._viewToModelConverter):
                    {
                        var view = targetObject;
                        if (view._viewToModelConverter && !view.TryGetValidViewToModelConverter(out _))
                        {
                            EditorGUILayout.HelpBox("无效的视图到模型转换器组件对象", MessageType.Error);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 重新查找分类列表
        /// </summary>
        /// <returns></returns>
        protected virtual CategoryList RefindCategoryList()
        {
            var view = targetObject;
            if (!view) return default;

            var modelValueType = view.modelValueType ?? typeof(object);
            var viewValueType = view.viewValueType ?? typeof(object);

            return EditorToolsHelper.GetWithPurposes((c, b) =>
            {
                if (b is ComponentToolItem componentToolItem)
                {
                    return DataConverterCache.Get(modelValueType, viewValueType, componentToolItem.type).CanI2OOrO2I;
                }
                return false;
            }, nameof(BaseDataConverter));
        }

        private void FindCategoryList()
        {
            if (!targetObject) return;

            categoryList = RefindCategoryList();
            Repaint();
        }

        /// <summary>
        /// 当检查器绘制
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())// View的参数修改，也会导致匹配类型发生变化
            {
                UICommonFun.DelayCall(FindCategoryList);
            }
            categoryList?.DrawVertical();
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        protected override bool displayHelpInfo => true;//base.displayHelpInfo;

        /// <summary>
        /// 获取帮助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var stringBuilder = base.GetHelpInfo();
            var view = targetObject as BaseModelView;
            var modelDataType = view.modelValueType;
            var viewDataType = view.viewValueType;
            stringBuilder.AppendFormat("模型主对象:\t{0}\n", CommonFun.ObjectToString(view.modelMainObject));
            stringBuilder.AppendFormat("模型数据类型:\t{0}\n", modelDataType?.FullName);
            stringBuilder.AppendFormat("视图数据类型:\t{0}", viewDataType?.FullName);
            return stringBuilder;
        }
    }
}
