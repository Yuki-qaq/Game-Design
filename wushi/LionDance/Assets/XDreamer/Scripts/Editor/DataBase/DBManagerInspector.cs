using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.EditorTools;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginDataBase;

namespace XCSJ.EditorDataBase
{
    /// <summary>
    /// 数据库管理器检查器
    /// </summary>
    [Name("数据库管理器检查器")]
    [CustomEditor(typeof(DBManager))]
    public class DBManagerInspector : BaseManagerInspector<DBManager>
    {
        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(DBManager));
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Separator();
            categoryList.DrawVertical();
        }
    }
}
