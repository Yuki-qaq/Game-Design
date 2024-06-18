using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Extension.Base.Interactions.Tools;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 物品集合
    /// </summary>
    public class ItemCollection : InteractProvider
    {
        /// <summary>
        /// 最大容量
        /// </summary>
        public int _maxCapacity = 10;

        /// <summary>
        /// 当前数量
        /// </summary>
        public int currentCount => _items.Count;

        /// <summary>
        /// 物品
        /// </summary>
        public List<Item> _items = new List<Item>();

        /// <summary>
        /// 物品
        /// </summary>
        public List<Item> items => _items;

        /// <summary>
        /// 当添加物品
        /// </summary>
        public static event Action<ItemCollection, Item> onAddItem;

        /// <summary>
        /// 当移除物品
        /// </summary>
        public static event Action<ItemCollection, Item> onRemoveItem;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            _items.ForEach(obj => obj.container = this);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Add(Item item)
        {
            if (item && currentCount < _maxCapacity)
            {
                item.Remove();
                
                item.container = this;
                _items.Add(item);
                onAddItem?.Invoke(this, item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Remove(Item item)
        {
            var rs = _items.Remove(item);
            if (rs)
            {
                onRemoveItem?.Invoke(this, item);
                item.container = null;
            }
            return rs;
        }

        /// <summary>
        /// 有物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool HasItems(Item item, int amount)
        {
            //return amount == _items.Count(obj => obj._id == item._id);
            return true;
        }

        /// <summary>
        /// 有物品
        /// </summary>
        /// <param name="itemInfos"></param>
        /// <param name="lostList"></param>
        /// <returns></returns>
        public bool HasItems(List<ItemInfo> itemInfos, out List<ItemInfo> lostList)
        {
            lostList = new List<ItemInfo>();

            foreach (ItemInfo info in itemInfos)
            {
                var needAmount = info._amount;
                if (needAmount > 0)
                {
                    var hasCount = _items.Count(obj => obj.itemName == info._name);
                    if (hasCount < needAmount)
                    {
                        lostList.Add(new ItemInfo(info._name, needAmount-hasCount, info._pose));
                    }
                }
            }

            return lostList.Count==0;
        }

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        public List<Item> GetItems(ItemInfo itemInfo)
        {
            var result = new List<Item>(); 
            var needAmount = itemInfo._amount;
            if (needAmount <= 0) return result;

            foreach (var item in _items)
            {
                if (item.itemName == itemInfo._name && result.Count < needAmount)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 提供
        /// </summary>
        /// <param name="itemInfos"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public bool Provide(List<ItemInfo> itemInfos, ItemCollection receiver)
        {
            if (!HasItems(itemInfos, out _)) return false;

            foreach (var info in itemInfos)
            {
                var result = GetItems(info);
                foreach (var item in result)
                {
                    info.SetPose(item.transform);
                    receiver.Add(item);
                    Remove(item);
                }
            }
            return true;
        }
    }

}
