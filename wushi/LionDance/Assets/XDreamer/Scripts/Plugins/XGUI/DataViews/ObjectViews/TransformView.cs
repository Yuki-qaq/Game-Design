using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.SelectionUtils;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.ObjectViews
{
    /// <summary>
    /// 变换视图
    /// </summary>
    [Name("变换视图")]
    [XCSJ.Attributes.Icon(EIcon.GameObject)]
    public class TransformView : ComponentView
    {
        /// <summary>
        /// 变换源
        /// </summary>
        public enum ETransformSource
        {
            /// <summary>
            /// 自定义
            /// </summary>
            [Name("自定义")]
            Custom,

            /// <summary>
            /// 选择集
            /// </summary>
            [Name("选择集")]
            Selection,

            /// <summary>
            /// 选择修改器
            /// </summary>
            [Name("选择修改器")]
            SelectionModify,
        }

        /// <summary>
        /// 变换源
        /// </summary>
        [Name("变换源")]
        [EnumPopup]
        public ETransformSource _transformSource = ETransformSource.Selection;

        /// <summary>
        /// 自定义变换
        /// </summary>
        [Name("自定义变换")]
        [ComponentPopup]
        [HideInSuperInspector(nameof(_transformSource), EValidityCheckType.NotEqual, ETransformSource.Custom)]
        public Transform _customTransfrom;

        /// <summary>
        /// 选择修改器
        /// </summary>
        [Name("选择修改器")]
        [ComponentPopup]
        [HideInSuperInspector(nameof(_transformSource), EValidityCheckType.NotEqual, ETransformSource.SelectionModify)]
        public SelectionModify _selectionModify;

        /// <summary>
        /// 选择修改器
        /// </summary>
        public SelectionModify selectionModify => this.XGetComponentInParentOrGlobal(ref _selectionModify);

        /// <summary>
        /// 目标变换
        /// </summary>
        public Transform targetTransfrom
        {
            get
            {
                switch (_transformSource)
                {
                    case ETransformSource.Custom: return _customTransfrom;
                    case ETransformSource.Selection: return selectTransform;
                        case ETransformSource.SelectionModify: return _selectionModify ? _selectionModify.currentSelectionTransform : default;
                }
                return default;
            }
        }

        /// <summary>
        /// Unity对象
        /// </summary>
        public override UnityEngine.Object unityObject => targetTransfrom;

        private Transform selectTransform;

        /// <summary>
        /// 不可拖拽
        /// </summary>
        public override bool canDrag { get => false ; set { } }

        #region Unity消息

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (selectionModify) { }
        }

        /// <summary>
        /// 启用：绑定选择事件
        /// </summary>
        protected override void OnEnable()
        {
            // 启用是检查选择集对象
            var go = Selection.selection;
            if (go)
            {
                selectTransform = go.transform;
            }
            base.OnEnable();

            Selection.selectionChanged += OnSelectionChanged;

            TransformListener.onTransformHasChanged += OnTransformHasChanged;
            TransformListener.Add(targetTransfrom);
        }

        /// <summary>
        /// 启用：解除选择事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            Selection.selectionChanged -= OnSelectionChanged;

            TransformListener.onTransformHasChanged -= OnTransformHasChanged;
            TransformListener.Remove(targetTransfrom);
        }

        private void OnSelectionChanged(GameObject[] oldSelections, bool isUndoOrRedo)
        {
            if (_transformSource != ETransformSource.Selection) return;

            var go = Selection.selection;
            if (go)
            {

                TransformListener.Remove(targetTransfrom);
                selectTransform = go.transform;
                TransformListener.Add(targetTransfrom);
                objectMemberView.SetModelMainObject(selectTransform, this, "");
            }
        }

        private void OnTransformHasChanged(Transform transform)
        {
            if (transform == targetTransfrom)
            {
                objectMemberView.ModelToView();
            }
        }

        #endregion
    }
}
