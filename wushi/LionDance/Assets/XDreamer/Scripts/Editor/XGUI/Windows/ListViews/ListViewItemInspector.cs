using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorTools;
using XCSJ.EditorXGUI.DataViews.Base;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Windows.ListViews;

namespace XCSJ.EditorXGUI.Windows.ListViews
{
    /// <summary>
    /// 列表视图项检查器
    /// </summary>
    [Name("列表视图项检查器")]
    [CustomEditor(typeof(ListViewItem))]
    public class ListViewItemInspector : BaseObjectMemberViewModelProviderInspector<ListViewItem>
    {
        /// <summary>
        /// 显示辅助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        /// <summary>
        /// 显示运行时辅助信息
        /// </summary>
        protected override bool displayRuntimeHelpInfo => true;

        /// <summary>
        /// 获取运行时辅助信息
        /// </summary>
        /// <returns></returns>
        [LanguageTuple("Index:\t{0}", "索引：\t{0}")]
        [LanguageTuple("\nHierarchy Index:\t{0}", "层级索引：\t{0}")]
        [LanguageTuple("The model is invalid or not in use!", "模型无效或未使用!")]
        public override StringBuilder GetRuntimeHelpInfo()
        {
            var sb = base.GetRuntimeHelpInfo();
            var model = mb.model;
            if (model != null)
            {
                sb.AppendFormat(Tr("Index:\t{0}"), model.index);
                sb.AppendFormat(Tr("\nHierarchy Index:\t{0}"), model.hierarchyIndex);
            }
            else
            {
                sb.Append("<color=red>" + Tr("The model is invalid or not in use!") + "</color>");
            }
            return sb;
        }
    }
}
