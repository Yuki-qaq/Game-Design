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
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Windows.ListViews;

namespace XCSJ.EditorXGUI.Windows.ListViews
{
    /// <summary>
    /// 列表视图检查器
    /// </summary>
    [Name("列表视图检查器")]
    [CustomEditor(typeof(ListView))]
    public class ListViewInspector : ObjectMemberViewInspector
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(ListViewModelProvider)));

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            InitGridLayoutGroup();

            XDreamerEvents.onSceneAnyAssetsChanged += OnSceneAnyAssetsChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            XDreamerEvents.onSceneAnyAssetsChanged -= OnSceneAnyAssetsChanged;
        }

        private void OnSceneAnyAssetsChanged() => InitGridLayoutGroup();

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawGridConstraint();

            CategoryListExtension.DrawVertical(categoryList);
        }

        #region 网格布局组

        /// <summary>
        /// 网格约束约束枚举
        /// </summary>
        public enum EGridConstraint
        {
            /// <summary>
            /// 弹性
            /// </summary>
            [Name("弹性")]
            Flexible,

            /// <summary>
            /// 固定列数
            /// </summary>
            [Name("固定列数")]
            FixedColumnCount,

            /// <summary>
            /// 固定行数
            /// </summary>
            [Name("固定行数")]
            FixedRowCount
        }

        /// <summary>
        /// 网格约束
        /// </summary>
        [Name("网格约束")]
        public EGridConstraint gridConstraint = EGridConstraint.Flexible;

        /// <summary>
        /// 约束数量
        /// </summary>
        [Name("约束数量")]
        public int constraintCount = 1;

        private GridLayoutGroup gridLayoutGroup;

        private void InitGridLayoutGroup()
        {
            if (!targetObject) return;
            gridLayoutGroup = targetObject.GetComponentInChildren<GridLayoutGroup>();
            if (gridLayoutGroup)
            {
                gridConstraint = ToGridConstraint(gridLayoutGroup.constraint);
                constraintCount = gridLayoutGroup.constraintCount;
            }
        }

        private void DrawGridConstraint()
        {
            if (!gridLayoutGroup) return;

            // 绘制约束规则
            EditorGUI.BeginChangeCheck();
            gridConstraint = (EGridConstraint)UICommonFun.EnumPopup(CommonFun.NameTip(this.GetType(), nameof(gridConstraint)), gridConstraint);
            if (EditorGUI.EndChangeCheck())
            {
                var template = (target as ListView)._template;
                // 在模版父级上添加布局
                if (!gridLayoutGroup && template)
                {
                    var parent = template.transform.parent;
                    if (parent)
                    {
                        gridLayoutGroup = parent.XAddComponent<GridLayoutGroup>();
                    }
                }

                if (gridLayoutGroup)
                {
                    gridLayoutGroup.XModifyProperty(() =>
                    {
                        gridLayoutGroup.enabled = true;
                        gridLayoutGroup.constraint = ToGridLayoutGroupConstraint(gridConstraint);
                    });
                }
            }

            // 绘制约束数量
            if (gridConstraint == EGridConstraint.FixedRowCount || gridConstraint == EGridConstraint.FixedColumnCount)
            {
                EditorGUI.BeginChangeCheck();
                constraintCount = EditorGUILayout.IntField(CommonFun.NameTip(this.GetType(), nameof(constraintCount)), constraintCount);
                if (EditorGUI.EndChangeCheck())
                {
                    if (gridLayoutGroup)
                    {
                        gridLayoutGroup.XModifyProperty(() => gridLayoutGroup.constraintCount = constraintCount);
                    }
                }
            }
        }

        private EGridConstraint ToGridConstraint(GridLayoutGroup.Constraint constraint) => (EGridConstraint)constraint;

        private GridLayoutGroup.Constraint ToGridLayoutGroupConstraint(EGridConstraint gridConstraint) => (GridLayoutGroup.Constraint)gridConstraint;

        #endregion
    }
}
