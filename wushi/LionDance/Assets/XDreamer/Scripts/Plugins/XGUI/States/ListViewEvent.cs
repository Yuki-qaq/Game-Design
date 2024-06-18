using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Widgets;
using XCSJ.PluginXGUI.Windows.ListViews;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.States
{
    /// <summary>
    /// 列表视图事件
    /// </summary>
    [ComponentMenu(XGUICategory.XGUIDirectory + Title, typeof(XGUIManager))]
    [Name(Title, nameof(ListViewEvent))]
    [Tip("用于监听列表视图特有的各种交互事件的触发器", "Triggers for listening to various interactive events unique to the list view")]
    [XCSJ.Attributes.Icon(EIcon.UI)]
    [Owner(typeof(XGUIManager))]
    public class ListViewEvent : Trigger<ListViewEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "列表视图事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(XGUICategory.XGUI, typeof(XGUIManager))]
        [StateComponentMenu(XGUICategory.XGUIDirectory + Title, typeof(XGUIManager))]
        [Name(Title, nameof(ListViewEvent))]
        [XCSJ.Attributes.Icon(EIcon.UI)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 列表视图规则
        /// </summary>
        public enum EListViewRule
        {
            /// <summary>
            /// 任意
            /// </summary>
            [Name("任意")]
            Any = -1,

            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 指定的
            /// </summary>
            [Name("指定的")]
            Designated,

            /// <summary>
            /// 列表中
            /// </summary>
            [Name("列表中")]
            InList,

            /// <summary>
            /// 非列表中
            /// </summary>
            [Name("非列表中")]
            NotInList,
        }

        /// <summary>
        /// 列表视图规则
        /// </summary>
        [Name("列表视图规则")]
        [EnumPopup]
        public EListViewRule _listViewRule = EListViewRule.Designated;

        /// <summary>
        /// 列表视图
        /// </summary>
        [Name("列表视图")]
        [HideInSuperInspector(nameof(_listViewRule), EValidityCheckType.NotEqual, EListViewRule.Designated)]
        [ComponentPopup]
        public ListView _listView;

        /// <summary>
        /// 列表视图
        /// </summary>
        public ListView listView => this.XGetComponentInGlobal(ref _listView);

        /// <summary>
        /// 列表视图列表
        /// </summary>
        [Name("列表视图列表")]
        [HideInSuperInspector(nameof(_listViewRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EListViewRule.InList, nameof(_listViewRule), EValidityCheckType.NotEqual, EListViewRule.NotInList)]
        [ComponentPopup(typeof(ListView))]
        public List<ListView> _listViews = new List<ListView>();

        /// <summary>
        /// 列表视图事件
        /// </summary>
        [Name("列表视图事件")]
        [EnumPopup]
        public EListViewEvent _listViewEvent = EListViewEvent.Click;

        /// <summary>
        /// 列表视图变量字符串
        /// </summary>
        [Name("列表视图变量字符串")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _listViewVarString = "";

        /// <summary>
        /// 项索引变量字符串
        /// </summary>
        [Name("项索引变量字符串")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _itemIndexVarString = "";

        /// <summary>
        /// 子项索引变量字符串
        /// </summary>
        [Name("子项索引变量字符串")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _subitemIndexVarString = "";

        /// <summary>
        /// 子项键变量字符串
        /// </summary>
        [Name("子项键变量字符串")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _subitemKeyVarString = "";

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            ListView.onChildInteractEvent += OnChildInteractEvent;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            ListView.onChildInteractEvent -= OnChildInteractEvent;
        }

        private void OnChildInteractEvent(ListView listView, ViewInteractData viewInteractData)
        {
            if (finished) return;
            switch (_listViewRule)
            {
                case EListViewRule.Any: break;
                case EListViewRule.Designated:
                    {
                        if (listView == this.listView) break;
                        return;
                    }
                case EListViewRule.InList:
                    {
                        if (_listViews.Contains(listView)) break;
                        return;
                    }
                case EListViewRule.NotInList:
                    {
                        if (!_listViews.Contains(listView)) break;
                        return;
                    }
                default: return;
            }
            switch (_listViewEvent)
            {
                case EListViewEvent.Click:
                    {
                        var view = viewInteractData.view;
                        while (view != null)
                        {
                            if (view is ListViewSubitem subitem)
                            {
                                var item = subitem.listViewItem;
                                //Debug.Log("列表点击列表视图子项：" + name + "，行：" + item.index + ",列：" + subitem.index + ",列键：" + subitem.key);
                                _listViewVarString.TrySetOrAddSetHierarchyVarValue(listView.GameObjectComponentToString());
                                _itemIndexVarString.TrySetOrAddSetHierarchyVarValue(item.index + 1);
                                _subitemIndexVarString.TrySetOrAddSetHierarchyVarValue(subitem.index + 1);
                                _subitemKeyVarString.TrySetOrAddSetHierarchyVarValue(subitem.key);

                                finished = true;
                                break;
                            }
                            else if (view is ListViewItem item)
                            {
                                //Debug.Log("列表点击列表视图项：" + name + "，行：" + item.index);
                                _listViewVarString.TrySetOrAddSetHierarchyVarValue(listView.GameObjectComponentToString());
                                _itemIndexVarString.TrySetOrAddSetHierarchyVarValue(item.index + 1);
                                _subitemIndexVarString.TrySetOrAddSetHierarchyVarValue(0);
                                _subitemKeyVarString.TrySetOrAddSetHierarchyVarValue("");

                                finished = true;
                                break;
                            }
                            view = view.parentView;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            switch (_listViewRule)
            {
                case EListViewRule.Designated: return _listView;
                case EListViewRule.InList:
                case EListViewRule.NotInList: return _listViews.Count > 0;
            }
            return base.DataValidity();
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_listViewRule)
            {
                case EListViewRule.Any:
                case EListViewRule.InList:
                case EListViewRule.NotInList: return _listViewEvent.Tr();
                case EListViewRule.Designated:
                    {
                        if (_listView)
                        {
                            return _listView.name + "." + _listViewEvent.Tr();
                        }
                        break;
                    }
            }

            return base.ToFriendlyString();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (listView) { }
        }
    }

    /// <summary>
    /// 列表视图事件
    /// </summary>
    [Name("列表视图事件")]
    public enum EListViewEvent
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 点击
        /// </summary>
        [Name("点击")]
        Click,
    }
}
