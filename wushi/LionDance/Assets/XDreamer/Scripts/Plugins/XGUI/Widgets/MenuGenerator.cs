using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.PropertyDatas;
using XCSJ.PluginXGUI.Base;
using static XCSJ.PluginTools.Inputs.MouseInput;

namespace XCSJ.PluginXGUI.Widgets
{

    #region 菜单生成器

    /// <summary>
    /// 菜单接口
    /// </summary>
    public interface IMenu
    {
        /// <summary>
        /// 菜单标题
        /// </summary>
        string menuTitle { get; }
    }

    /// <summary>
    /// 菜单生成器
    /// </summary>
    [Name("菜单生成器")]
    [Tool(XGUICategory.Component, rootType = typeof(XGUIManager))]
    [RequireManager(typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    [XCSJ.Attributes.Icon(EIcon.List)]
    public class MenuGenerator : View, IMenu, IPropertyKeyProvider
    {
        #region 菜单项标签

        /// <summary>
        /// 菜单项标签:菜单生成器所在游戏对象和当前正在交互的可交互实体所在游戏对象上的【交互属性】组件上与本关键字匹配的文本值作为菜单项
        /// </summary>
        [PropertyKey]
        public const string MenuItemTag = "菜单项标签";

        /// <summary>
        /// 菜单项标签关键字
        /// </summary>
        [Name("菜单项标签关键字")]
        public List<string> _menuItemTagKeys = new List<string>();

        /// <summary>
        /// 属性关键字信息
        /// </summary>
        public List<PropertyKeyInfo> propertyKeyInfos
        {
            get
            {
                var className = CommonFun.Name(typeof(MenuGenerator));
                var propertyKeyName = CommonFun.Name(typeof(MenuGenerator), nameof(MenuGenerator._menuItemTagKeys));

                var list = new List<PropertyKeyInfo>();
                foreach (var item in _menuItemTagKeys)
                {
                    list.Add(new PropertyKeyInfo(className, propertyKeyName, item));
                }
                return list;
            }
        }

        #endregion

        #region 交互命令

        /// <summary>
        /// 显示
        /// </summary>
        [InteractCmd]
        [Name("显示")]
        public void Show() => TryInteract(nameof(Show));

        /// <summary>
        /// 显示
        /// </summary>
        [InteractCmdFun(nameof(Show))]
        public EInteractResult Show(InteractData interactData)
        {
            ShowMenu(interactData);
            return EInteractResult.Success;
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        [InteractCmd]
        [Name("隐藏")]
        public void Hide() => TryInteract(nameof(Hide));

        /// <summary>
        /// 隐藏
        /// </summary>
        [InteractCmdFun(nameof(Hide))]
        public EInteractResult Hide(InteractData interactData)
        {
            HideMenu();
            return EInteractResult.Success;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 菜单标题
        /// </summary>
        public string menuTitle => string.IsNullOrEmpty(_menuTitle) ? name : _menuTitle;

        /// <summary>
        /// 菜单标题
        /// </summary>
        [Name("菜单标题")]
        public string _menuTitle;

        /// <summary>
        /// 菜单项生成规则
        /// </summary>
        [Name("菜单项生成规则")]
        [EnumPopup]
        public EMenuItemGenerateRule _menuItemGenerateRule = EMenuItemGenerateRule.InteractorCmd | EMenuItemGenerateRule.InteractableEntity | EMenuItemGenerateRule.InteractProperty;

        /// <summary>
        /// 交互器列表
        /// </summary>
        [Name("交互器列表")]
        [Tip("列表中的交互器自身的命令生成菜单命令项", "The command generation menu command item of the interactive device itself in the list")]
        [HideInSuperInspector(nameof(_menuItemGenerateRule), EValidityCheckType.NotHasFlag, EMenuItemGenerateRule.InteractorCmd)]
        public List<InteractObject> _interactors = new List<InteractObject>();

        #endregion

        #region Unity 消息

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _menuItemTagKeys.Clear();
            _menuItemTagKeys.Add(MenuItemTag);
            _tagProperty.AddTagWithDistinct(MenuItemTag);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            // 初始化隐藏菜单
            HideMenu();
        }

        #endregion

        #region 交互处理

        private HashSet<string> menuNames = new HashSet<string>();

        private void ShowMenu(InteractData interactData)
        {
            var menuDatas = new List<InteractData>();

            // 从交互器列表中生成菜单项
            if ((_menuItemGenerateRule & EMenuItemGenerateRule.InteractorCmd) == EMenuItemGenerateRule.InteractorCmd)
            {
                foreach (var interactor in _interactors)
                {
                    menuDatas.AddRange(CreateMenuData(CreateInteractDatas(interactData, interactor.GetWorkInCmds(interactData)), EMenuItemGenerateRule.InteractorCmd, null));
                }
            }

            // 从交互属性上获取菜单项数据
            if ((_menuItemGenerateRule & EMenuItemGenerateRule.InteractProperty) == EMenuItemGenerateRule.InteractProperty)
            {
                // 从自身上查找属性值组件
                {
                    menuDatas.AddRange(CreateMenuData(CreateInteractDatasWithPropertyValue(interactData, gameObject, out var interactProperty), EMenuItemGenerateRule.InteractProperty, interactProperty));
                }

                // 从可交互对象上查找属性值组件
                var component = interactData.interactable;
                if (component)
                {
                    menuDatas.AddRange(CreateMenuData(CreateInteractDatasWithPropertyValue(interactData, component.gameObject, out var interactProperty), EMenuItemGenerateRule.InteractProperty, interactProperty));
                }
            }

            // 从可交互对象实体上获取菜单项数据
            if ((_menuItemGenerateRule & EMenuItemGenerateRule.InteractableEntity) == EMenuItemGenerateRule.InteractableEntity)
            {
                menuDatas.AddRange(CreateMenuData(CreateInteractDatas(interactData, GetInteractableCmds(interactData)), EMenuItemGenerateRule.InteractableEntity, null));
            }

            if (menuDatas.Count > 0)
            {
                XGUIHelper.ShowMenu(this, menuTitle, menuDatas.ToArray());
            }
        }

        /// <summary>
        /// 隐藏菜单
        /// </summary>
        private void HideMenu()
        {
            menuNames.Clear();
            XGUIHelper.HideMenu(this);
        }

        private List<InteractData> CreateInteractDatasWithPropertyValue(InteractData interactData, GameObject go, out InteractProperty interactProperty)
        {
            var values = GetInteractPropertyTextValues(go, out interactProperty);
            foreach (var item in values)
            {
                menuNames.Add(item);
            }
            return CreateInteractDatas(interactData, values);
        }

        private string[] GetInteractPropertyTextValues(GameObject gameObject, out InteractProperty interactProperty)
        {
            interactProperty = gameObject.GetComponentInChildren<InteractProperty>();
            return interactProperty ? interactProperty._dataList.GetValues(_tagProperty.firstKey) : Empty<string>.Array;
        }

        private List<string> GetInteractableCmds(InteractData interactData)
        {
            var cmds = new List<string>();
            foreach (var item in interactData.interactables)
            {
                cmds.AddRange(item.GetWorkInCmds(interactData));
            }
            return cmds;
        }

        private List<MenuData> CreateMenuData(List<InteractData> interactDatas, EMenuItemGenerateRule menuItemGenerateRule, InteractProperty interactProperty)
        {
            var menuDatas = new List<MenuData>();
            foreach (var data in interactDatas)
            {
                if (data is MouseRayInteractData mouseRayInteractData)
                {
                    menuDatas.Add(new MenuData(menuItemGenerateRule, this, interactProperty, mouseRayInteractData));
                }
                else
                {
                    menuDatas.Add(new MenuData(menuItemGenerateRule, this, interactProperty, data));
                }
            }
            return menuDatas;
        }



        /// <summary>
        /// 1、遍历当前交互器的可工作命令列表
        /// 2、通过【工作命令】获取所有可与当前【交互器】进行交互的【可交互对象】
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public virtual List<InteractData> CreateWorkInteractDatas(InteractData interactData) => CreateInteractDatas(interactData, GetWorkInCmds(interactData));

        /// <summary>
        /// 1、遍历输入命令列表
        /// 2、通过【命令】获取所有可与当前【交互器】进行交互的【可交互对象】
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="inCmds"></param>
        /// <returns></returns>
        public virtual List<InteractData> CreateInteractDatas(InteractData interactData, IEnumerable<string> inCmds)
        {
            var result = new List<InteractData>();
            var data = default(InteractData);
            foreach (var cmd in inCmds)
            {
                if (data == null)
                {
                    data = CloneInteractData(interactData);
                }

                data.SetCmdName(cmd, this);

                if (CanInteract(data))
                {
                    result.Add(data);
                    data = null;
                }
            }

            return result;
        }
        #endregion
    }

    /// <summary>
    /// 弹出菜单资产
    /// </summary>
    [Serializable]
    public class PopupMenuAsset : XGUIAsset<MenuGenerator> { }

    #endregion

    #region 菜单项生成规则枚举

    /// <summary>
    /// 菜单项生成规则
    /// </summary>
    [Flags]
    public enum EMenuItemGenerateRule
    {
        /// <summary>
        /// 交互属性
        /// </summary>
        [Name("交互属性")]
        InteractProperty = 1 << 0,

        /// <summary>
        /// 交互器命令
        /// </summary>
        [Name("交互器命令")]
        InteractorCmd = 1 << 1,

        /// <summary>
        /// 可交互实体
        /// </summary>
        [Name("可交互实体")]
        InteractableEntity = 1 << 2,
    }

    #endregion

    #region 菜单数据

    /// <summary>
    /// 菜单数据
    /// </summary>
    public class MenuData : MouseRayInteractData
    {
        /// <summary>
        /// 菜单生成规则
        /// </summary>
        public EMenuItemGenerateRule menuItemGenerateRule { get; private set; } = 0;

        /// <summary>
        /// 菜单生成器
        /// </summary>
        public MenuGenerator menuGenerator { get; private set; }

        /// <summary>
        /// 交互属性：菜单从该对象提取文本值作为菜单项名称
        /// </summary>
        public InteractProperty interactProperty { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MenuData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="menuItemGenerateRule"></param>
        /// <param name="menuGenerator"></param>
        /// <param name="interactProperty"></param>
        /// <param name="interactData"></param>
        public MenuData(EMenuItemGenerateRule menuItemGenerateRule, MenuGenerator menuGenerator, InteractProperty interactProperty, InteractData interactData) : base(interactData.cmdName, interactData, interactData.interactor, interactData.interactables)
        {
            this.menuItemGenerateRule = menuItemGenerateRule;
            this.menuGenerator = menuGenerator;
            this.interactProperty = interactProperty;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="menuItemGenerateRule"></param>
        /// <param name="menuGenerator"></param>
        /// <param name="interactProperty"></param>
        /// <param name="mouseRayInteractData"></param>
        public MenuData(EMenuItemGenerateRule menuItemGenerateRule, MenuGenerator menuGenerator, InteractProperty interactProperty, MouseRayInteractData mouseRayInteractData) : base(mouseRayInteractData.mouseButton, mouseRayInteractData.mousePosition, mouseRayInteractData.sender, mouseRayInteractData.ray, mouseRayInteractData.raycastHit, mouseRayInteractData.rayMaxDistance, mouseRayInteractData.layerMask, mouseRayInteractData.cmdName, null, mouseRayInteractData.interactor, mouseRayInteractData.interactables)
        {
            this.menuItemGenerateRule = menuItemGenerateRule;
            this.menuGenerator = menuGenerator;
            this.interactProperty = interactProperty;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected override InteractData CreateInstance() => new MenuData();

        /// <summary>
        /// 拷贝数据
        /// </summary>
        /// <param name="interactData"></param>
        public override void CopyTo(InteractData interactData)
        {
            base.CopyTo(interactData);

            if (interactData is MenuData menuData)
            {
                menuData.menuItemGenerateRule = menuItemGenerateRule;
                menuData.menuGenerator = menuGenerator;
                menuData.interactProperty = interactProperty;
            }
        }
    }

    #endregion
}
