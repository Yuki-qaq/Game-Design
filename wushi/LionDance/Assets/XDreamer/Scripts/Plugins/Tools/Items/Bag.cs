using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Languages;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 背包
    /// </summary>
    [Name("背包")]
    public abstract class Bag : BagItem, IBag
    {
        /// <summary>
        /// 容量
        /// </summary>
        [Name("容量")]
        [Tip("背包容量", "Backpack Capacity")]
        public int _capacity = 1;

        /// <summary>
        /// 剩余容量
        /// </summary>
        public int remainingCapacity => _capacity - usedCapacity;

        /// <summary>
        /// 已使用容量
        /// </summary>
        public int usedCapacity => _bagItemDatas.Count(data => data._templateBagItem);

        /// <summary>
        /// 当数量清空时处理规则
        /// </summary>
        [Name("当数量清空时处理规则")]
        [EnumPopup]
        public EHandleRuleOnCountClear handleRuleOnCountClear = EHandleRuleOnCountClear.Remove;

        /// <summary>
        /// 当数量清空时处理规则
        /// </summary>
        [Name("当数量清空时处理规则")]
        public enum EHandleRuleOnCountClear
        {
            /// <summary>
            /// 移除:将背包中对应物品信息移除，会将所在背包的剩余容量增加
            /// </summary>
            [Name("移除")]
            [Tip("将背包中对应物品信息移除，会将所在背包的剩余容量增加", "Removing the corresponding item information from the backpack will increase the remaining capacity of the backpack")]
            Remove = 0,

            /// <summary>
            /// 保持:背包中对应物品信息保留，会一直占用所在背包
            /// </summary>
            [Name("保持")]
            [Tip("背包中对应物品信息保留，会一直占用所在背包", "The information of the corresponding items in the backpack is retained and will always occupy the backpack")]
            Keep,
        }

        /// <summary>
        /// 背包物品数据列表
        /// </summary>
        [Name("背包物品数据列表")]
        public List<BagItemData> _bagItemDatas = new List<BagItemData>();

        /// <summary>
        /// 获取默认分类用途
        /// </summary>
        /// <returns></returns>
        public abstract string GetDefaultCategoryUsage();

        /// <summary>
        /// 获取分类用途
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetCategoryUsages();

        /// <summary>
        /// 尝试获取背包项
        /// </summary>
        /// <param name="usage"></param>
        /// <param name="bagItems"></param>
        /// <returns></returns>
        public abstract bool TryGetBagItems(string usage, out List<IBagItem> bagItems);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="bagItem"></param>
        public void Add(IBagItem bagItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="bagItem"></param>
        public void Remove(IBagItem bagItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 可添加
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool CanAdd(IBagItem bagItem, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 尝试添加
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool TryAdd(IBagItem bagItem, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 可移除
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool CanRemove(IBagItem bagItem, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 尝试移除
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool TryRemove(IBagItem bagItem, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 尝试处理背包项交互
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="itemContext"></param>
        /// <param name="itemInteractor"></param>
        /// <returns></returns>
        public bool TryHandleBagItemInteractable(IBagItem bagItem, IItemContext itemContext, IItemInteractor itemInteractor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取背包项交互用途
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        public List<string> GetBagItemInteractableUsages(IBagItem bagItem, IItemContext itemContext)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 背包物品数据
    /// </summary>
    [Serializable]
    [LanguageFileOutput]
    [Name("背包物品数据")]
    public class BagItemData
    {
        /// <summary>
        /// 背包
        /// </summary>
        [Name("背包")]
        public Bag _bag;

        /// <summary>
        /// 模版背包物品
        /// </summary>
        [Name("模版背包物品")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BagItem _templateBagItem;

        /// <summary>
        /// 数量：物品的当前堆叠数量
        /// </summary>
        [Name("数量")]
        [Tip("物品的当前堆叠数量", "Current stack quantity of items")]
        [ValidityCheck(EValidityCheckType.NotZero)]
        public int _count = 0;
    }

    /// <summary>
    /// 基础背包物品数据
    /// </summary>
    public interface IBagItemData : IBagItem
    {
        /// <summary>
        /// 总数量:可使用的总数量；可理解为背包中物品叠加上限；
        /// </summary>
        int totalCount { get; }

        /// <summary>
        /// 数量:可使用的数量；可理解为背包中物品当前叠加数；
        /// </summary>
        int count { get; }
    }

    /// <summary>
    /// 基础背包项数据
    /// </summary>
    public abstract class BaseBagItemData : IBagItemData
    {
        /// <summary>
        /// 原型物品
        /// </summary>
        public abstract IItem prototypeItem { get; set; }

        /// <summary>
        /// 总数量:可使用的总数量；可理解为背包中物品叠加上限；
        /// </summary>
        public abstract int totalCount { get; }

        /// <summary>
        /// 数量:可使用的数量；可理解为背包中物品当前叠加数；
        /// </summary>
        public abstract int count { get; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get => prototypeItem?.name; set => prototypeItem.name = value; }

        /// <summary>
        /// 能否选择
        /// </summary>
        public bool canSelect
        {
            get
            {
                var prototypeItem = this.prototypeItem;
                return prototypeItem != null ? prototypeItem.canSelect : false;
            }
        }

        /// <summary>
        /// 能否激活
        /// </summary>
        public bool canActivated
        {
            get
            {
                var prototypeItem = this.prototypeItem;
                return prototypeItem != null ? prototypeItem.canActivated : false;
            }
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool isSelected { get; set; }

        /// <summary>
        /// 输入命令列表
        /// </summary>
        public List<string> inCmdList => new List<string>();

        /// <summary>
        /// 输出命令列表
        /// </summary>
        public List<string> outCmdList => new List<string>();

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool CanInteractable(InteractData interactData)
        {
            var prototypeItem = this.prototypeItem;
            if (prototypeItem == null)
            {
                return false;
            }
            return prototypeItem.CanInteractAsInteractable(interactData);
        }

        /// <summary>
        /// 尝试处理交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public bool TryHandleInteractable(InteractData interactData, out EInteractResult interactResult)
        {
            var prototypeItem = this.prototypeItem;
            if (prototypeItem == null)
            {
                interactResult = EInteractResult.Fail;
                return false;
            }
            return prototypeItem.TryInteractAsInteractable(interactData, out interactResult);
        }

        /// <summary>
        /// 获取默认项可交互用户
        /// </summary>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        public string GetDefaultItemInteractableUsage(IItemContext itemContext) => prototypeItem?.GetDefaultItemInteractableUsage(itemContext);

        /// <summary>
        /// 获取项可交互用户
        /// </summary>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        public List<string> GetItemInteractableUsages(IItemContext itemContext) => prototypeItem?.GetItemInteractableUsages(itemContext);

        /// <summary>
        /// 尝试处理项可交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool TryHandleItemInteractable(InteractData interactData)
        {
            var prototypeItem = this.prototypeItem;
            if (prototypeItem == null)
            {
                return false;
            }
            return prototypeItem.TryInteractAsInteractable(interactData, out _);
        }

        /// <summary>
        /// 当克隆之后
        /// </summary>
        /// <param name="itemContext"></param>
        public void OnAfterClone(IItemContext itemContext) { }

        /// <summary>
        /// 尝试处理项可交互
        /// </summary>
        /// <param name="itemContext"></param>
        /// <param name="itemInteractor"></param>
        /// <returns></returns>
        public bool TryHandleItemInteractable(IItemContext itemContext, IItemInteractor itemInteractor) => default;

        /// <summary>
        /// 获取工作入命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public List<string> GetWorkInCmds(InteractData interactData) => new List<string>();

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public bool TryInteract(InteractData interactData, out EInteractResult interactResult)
        {
            interactResult = default;
            return default;
        }

        /// <summary>
        /// 可交互作为可交互对象
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool CanInteractAsInteractable(InteractData interactData) => default;

        /// <summary>
        /// 尝试交互作为可交互对象
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public bool TryInteractAsInteractable(InteractData interactData, out EInteractResult interactResult)
        {
            interactResult = default;
            return default;
        }
    }

    /// <summary>
    /// 基础背包项数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseBagItemData<T> : BaseBagItemData where T : class, IBagItem
    {
        /// <summary>
        /// 原型物品
        /// </summary>
        [Name("原型物品")]
        [Tip("具有原始标准基础信息的物品", "Items with original standard basic information")]
        [Json(false)]
        public T _prototypeItem;

        /// <summary>
        /// 原型物品名称
        /// </summary>
        public string prototypeItemName => _prototypeItem!=null ? _prototypeItem.name : "";

        /// <summary>
        /// 原型物品
        /// </summary>
        [Json(false)]
        public override IItem prototypeItem { get => _prototypeItem; set => _prototypeItem = value as T; }

        /// <summary>
        /// 总数量:可使用的总数量；可理解为背包中物品叠加上限；
        /// </summary>
        [Name("总数量")]
        [Tip("可使用的总数量；可理解为背包中物品叠加上限；", "Total usable quantity; It can be understood as the upper limit of items in the backpack;")]
        [Json(false)]
        public int _totalCount = 1;

        /// <summary>
        /// 总数量
        /// </summary>
        public override int totalCount => _totalCount;

        /// <summary>
        /// 数量:可使用的数量；可理解为背包中物品当前叠加数；
        /// </summary>
        [Name("数量")]
        [Tip("可使用的数量；可理解为背包中物品当前叠加数；", "Quantity available; It can be understood as the current superposition number of items in the backpack;")]
        [Json(false)]
        public int _count = 0;

        /// <summary>
        /// 数量
        /// </summary>
        public override int count => _count;

        /// <summary>
        /// 能否添加数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool CanAddCount(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "添加数量需大于0");
            return (_count + count) <= _totalCount;
        }

        /// <summary>
        /// 添加数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool AddCount(int count)
        {
            if (CanAddCount(count))
            {
                _count += count;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 能否移除数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool CanRemoveCount(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "移除数量需大于0");
            return (_count - count) >= 0;
        }

        /// <summary>
        /// 移除数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool RemoveCount(int count)
        {
            if (CanRemoveCount(count))
            {
                _count -= count;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="usage"></param>
        /// <param name="bag"></param>
        public void Handle(string usage, IBag bag)
        { }
    }

    /// <summary>
    /// 背包数据
    /// </summary>
    public interface IBagData : IBag { }
}
