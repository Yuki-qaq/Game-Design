using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.EditorXGUI.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.DataViews;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.EditorXGUI.DataViews.Base
{
    /// <summary>
    /// 基础对象成员视图模型提供者检查器
    /// </summary>
    [Name("基础对象成员视图模型提供者检查器")]
    [CustomEditor(typeof(BaseObjectMemberViewModelProvider), true)]
    public class BaseObjectMemberViewModelProviderInspector : BaseObjectMemberViewModelProviderInspector<BaseObjectMemberViewModelProvider> { }

    /// <summary>
    /// 基础对象成员视图模型提供者检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseObjectMemberViewModelProviderInspector<T>: DraggableViewInspector<T> where T : BaseObjectMemberViewModelProvider
    {
    }
}
