using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.ObjectViews
{
    /// <summary>
    /// Unity对象视图
    /// </summary>
    public abstract class UnityObjectView : BaseObjectMemberViewModelProvider
    {
        /// <summary>
        /// Unity对象
        /// </summary>
        public abstract UnityEngine.Object unityObject { get; }

        /// <summary>
        /// 启用：绑定选择事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            objectMemberView.AddListener(this);
            objectMemberView.SetModelMainObject(unityObject, this, "");
        }

        /// <summary>
        /// 禁用：解除选择事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            objectMemberView.RemoveListener(this);
        }
    }
}
