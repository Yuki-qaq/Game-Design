using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Machines;

namespace XCSJ.PluginRepairman.UI
{
    /// <summary>
    /// GUI项
    /// </summary>

    public abstract class GUIItem : BaseRepairmanGUI
    {
        /// <summary>
        /// 项
        /// </summary>
        protected IItem item = null;

        /// <summary>
        /// GUI项列表
        /// </summary>
        protected GUIItemList guiItemList = null;

        /// <summary>
        /// 名称
        /// </summary>
        [Name("名称")]
        public Text itemName = null;

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset() => FindComponents();

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake() => FindComponents();

        /// <summary>
        /// 查找组件
        /// </summary>
        protected virtual void FindComponents()
        {
            if (itemName == null)
            {
                itemName = GetComponentInChildren<Text>();
            }            
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="guiItemList"></param>
        public virtual void SetData(IItem item, GUIItemList guiItemList)
        {
            this.item = item;

            this.guiItemList = guiItemList;

            if (itemName)
            {
                itemName.text = this.item.showName;
            }

            SetIcon(item.texture2D);
        }

        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="texture2D"></param>
        protected abstract void SetIcon(Texture2D texture2D);
    }

    /// <summary>
    /// 基础拆装界面
    /// </summary>
    [RequireManager(typeof(RepairmanManager))]
    public abstract class BaseRepairmanGUI : InteractProvider { }
}
