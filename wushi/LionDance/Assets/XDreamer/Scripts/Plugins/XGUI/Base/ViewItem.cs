using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 视图项
    /// </summary>
    [Name("视图项")]
    public sealed class ViewItem : DraggableView, IViewItem
    {
        /// <summary>
        /// 查找并设置父级视图
        /// </summary>
        /// <returns></returns>
        protected override View FindAndSetParentView()
        {
            var parentView = this._parentView;
            if (!parentView || !(parentView is IViewItem))
            {
                var parentViewItem = this.GetComponentsInParent<IViewItem>().FirstOrDefault(c => c is Component component && component.gameObject != gameObject);
                if (parentViewItem is View view)
                {
                    this.parentView = view;
                }
            }
            return base.FindAndSetParentView();
        }
    }

    /// <summary>
    /// 视图项接口
    /// </summary>
    public interface IViewItem { }
}
