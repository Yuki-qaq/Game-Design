using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.ViewControllers;

namespace XCSJ.PluginXGUI.Views.ScrollViews
{
    /// <summary>
    /// UI容器类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UIContainer<T> : BaseViewController where T : Component
    {
        /// <summary>
        /// 模版
        /// </summary>
        [Name("模版")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public T _template;

        /// <summary>
        /// 项缓存
        /// </summary>
        public WorkObjectPool<T> pool
        {
            get
            {
                if (_pool == null)
                {
                    _pool = new WorkObjectPool<T>();
                    _template.gameObject.SetActive(false);

                    _pool.Init(() =>
                    {
                        var newGO = _template.gameObject.XCloneAndSetParent(_template.transform.parent);
                        newGO.transform.SetAsLastSibling();
                        var tmp = newGO.GetComponent<T>();
                        OnNewItem(tmp);
                        return tmp;
                    },
                        item =>
                        {
                            item.gameObject.SetActive(true);
                            OnAllocItem(item);
                        },
                        item =>
                        {
                            item.gameObject.SetActive(false);
                            OnFreeItem(item);
                            },
                        item => item);
                }
                return _pool;
            }
        }

        /// <summary>
        /// 池
        /// </summary>
        protected WorkObjectPool<T> _pool = null;

        /// <summary>
        /// 项数量
        /// </summary>
        public int itemCount => pool.workObjects.Count;

        /// <summary>
        /// 当新建项
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnNewItem(T item) { }

        /// <summary>
        /// 当分配项
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnAllocItem(T item) { }

        /// <summary>
        /// 当销毁项
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnFreeItem(T item) { }

    }
}
