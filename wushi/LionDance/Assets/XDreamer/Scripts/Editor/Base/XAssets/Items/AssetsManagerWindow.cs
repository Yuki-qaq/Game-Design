using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.EditorWindows;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.XAssets.Items
{
    /// <summary>
    /// XDreamer资产管理器:可用于管理从XDreamer官网文档手册中根据需要动态下载的各种资产，使用此管理器窗口可支持这些资产在本机不同Unity工程间执行显示、下载、导入等管理操作；
    /// </summary>
    [Name(Title)]
    [Tip("可用于管理从XDreamer官网文档手册中根据需要动态下载的各种资产，使用此管理器窗口可支持这些资产在本机不同Unity工程间执行显示、下载、导入等管理操作；", "It can be used to manage various assets dynamically downloaded from the XDreamer official website document manual as needed. Using this manager window, these assets can be displayed, downloaded, imported, and other management operations can be performed between different Unity projects on this machine;")]
    [XCSJ.Attributes.Icon(EIcon.Download)]
    [XDreamerEditorWindow(nameof(TrHelper.Common), index = 3)]
    public class AssetsManagerWindow : XEditorWindowWithScrollView<AssetsManagerWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = Product.Name + "资产管理器";

        /// <summary>
        /// 初始化
        /// </summary>
        [MenuItem(XDreamerMenu.NamePath + Title, priority = 8001)]
        public static void Init() => OpenAndFocus();

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateDisplayItems();

            AssetItem.onItemChanged += OnItemChanged;

            AssetsManager.onAssetsLoaded += OnAssetsLoaded;
            AssetsManager.onAddDownload += OnAddDownload;
            AssetsManager.onRemoveDownload += OnRemoveDownload;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            AssetItem.onItemChanged -= OnItemChanged;

            AssetsManager.onAssetsLoaded -= OnAssetsLoaded;
            AssetsManager.onAddDownload -= OnAddDownload;
            AssetsManager.onRemoveDownload -= OnRemoveDownload;
        }

        /// <summary>
        /// 当项已变更
        /// </summary>
        /// <param name="assetItem"></param>
        private void OnItemChanged(AssetItem assetItem) => Repaint();

        private void OnAssetsLoaded()
        {
            UpdateDisplayItems();
        }

        private void OnAddDownload(AssetItem assetItem)
        {
            UpdateDisplayItems();
        }

        private void OnRemoveDownload(AssetItem assetItem)
        {
            UpdateDisplayItems();
        }

        private string tip = "";
        private MessageType tipType = MessageType.Info;

        private void SetTip(string tip = "", MessageType tipType = MessageType.Info, float delayClearTime = -1)
        {
            this.tip = tip;
            this.tipType = tipType;

            if (delayClearTime > 0)
            {
                ClearError(delayClearTime);
            }
        }

        private void ClearError(float delayTime) => UICommonFun.DelayCall(() => { tip = ""; Repaint(); }, delayTime);

        /// <summary>
        /// 资产URL
        /// </summary>
        [Name("资产URL")]
        public string _assetUrl = "";

        private string _searchText = "";

        /// <summary>
        /// 搜索文本
        /// </summary>
        [Name("搜索文本")]
        public string searchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    UpdateDisplayItems();
                }
            }
        }

        /// <summary>
        /// 显示资产列表
        /// </summary>
        [Name("显示资产列表")]
        public bool _displayAssets = true;

        /// <summary>
        /// 显示项列表
        /// </summary>
        public List<AssetItem> displayItems = new List<AssetItem>();

        /// <summary>
        /// 更新显示项列表
        /// </summary>
        private void UpdateDisplayItems()
        {
            displayItems.Clear();
            var vs = new HashSet<string>();
            var cs = new HashSet<string>();
            var assetItems = AssetsManager.assetItems;
            foreach (var item in assetItems)
            {
                if (string.IsNullOrEmpty(searchText) || item.assetUrl.SearchMatch(searchText))
                {
                    displayItems.Add(item);
                }
                if (!string.IsNullOrEmpty(item.version))
                {
                    vs.Add(item.version);
                }
                if (!string.IsNullOrEmpty(item.category))
                {
                    cs.Add(item.category);
                }
            }
            applicableVersions = vs.ToArray();
            categorys = cs.ToArray();

            countInfo = displayItems.Count + "/" + assetItems.Count;

            Repaint();
        }

        /// <summary>
        /// 数量信息
        /// </summary>
        [Name("数量信息")]
        public string countInfo = "0/0";

        /// <summary>
        /// 适用版本
        /// </summary>
        [Name("适用版本")]
        [Tip("已适配的XDreamer版本", "Adapted XDreamer version")]
        public string[] applicableVersions = Empty<string>.Array;

        /// <summary>
        /// 分类
        /// </summary>
        [Name("分类")]
        [Tip("与XDreamer插件名称关联的分类名", "Category name associated with XDreamer plugin name")]
        public string[] categorys = Empty<string>.Array;

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sortTitle"></param>
        /// <param name="asc"></param>
        private void Sort(string sortTitle, bool asc)
        {
            var value = asc ? 1 : -1;
            displayItems.Sort((x, y) =>
            {
                switch (sortTitle)
                {
                    case nameof(AssetItem.fileName):
                        {
                            return value * UICommonFun.NaturalCompare(x.fileName, y.fileName);
                        }
                    case nameof(AssetItem.version):
                        {
                            return value * UICommonFun.NaturalCompare(x.version, y.version);
                        }
                    case nameof(AssetItem.category):
                        {
                            return value * UICommonFun.NaturalCompare(x.category, y.category);
                        }
                    case nameof(AssetItem.subCategory):
                        {
                            return value * UICommonFun.NaturalCompare(x.subCategory, y.subCategory);
                        }
                    case nameof(AssetItem.itemState):
                        {
                            return value * string.Compare(x.itemState.Tr(), y.itemState.Tr());
                        }
                    case nameof(AssetItem.beginTime):
                        {
                            return value * DateTime.Compare(x.beginTime, y.beginTime);
                        }
                    case nameof(AssetItem.progress):
                        {
                            if (Mathf.Approximately(x.progress, y.progress)) return 0;
                            return (x.progress < y.progress ? -1 : 1) * (asc ? 1 : -1);
                        }
                    case nameof(AssetItem.endTime):
                        {
                            if (x.itemState == EItemState.Downloading && x.itemState == EItemState.Downloading)
                            {
                                return value * TimeSpan.Compare(x.timeSpan, y.timeSpan);
                            }
                            else if (x.itemState == EItemState.Downloading)
                            {
                                return asc ? -1 : 1;
                            }
                            else if (y.itemState == EItemState.Downloading)
                            {
                                return asc ? 1 : -1;
                            }
                            else
                            {
                                return value * DateTime.Compare(x.endTime, y.endTime);
                            }
                        }
                }
                return 0;
            });
        }

        private bool asc = true;

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            EditorGUILayout.Separator();
            _assetUrl = EditorGUILayout.TextField(TrLabel(nameof(_assetUrl)), _assetUrl);
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Tr("Download And Import"), EditorStyles.miniButtonLeft))
            {
                if (AssetsManager.ValidAssetUrl(_assetUrl))
                {
                    AssetsManager.DownloadAndImport(_assetUrl);
                }
                else
                {
                    SetTip(Tr("Asset URL Invalid"), MessageType.Error, 3);
                }
            }
            if (GUILayout.Button(Tr("Download"), EditorStyles.miniButtonMid))
            {
                if (AssetsManager.ValidAssetUrl(_assetUrl))
                {
                    AssetsManager.Download(_assetUrl);
                }
                else
                {
                    SetTip(Tr("Asset URL Invalid"), MessageType.Error, 3);
                }
            }
            if (GUILayout.Button(Tr("Download By Clipboard URL"), EditorStyles.miniButtonMid))
            {
                var url = GUIUtility.systemCopyBuffer;
                if (AssetsManager.ValidAssetUrl(url))
                {
                    _assetUrl = url;
                    AssetsManager.Download(_assetUrl);
                }
                else
                {
                    SetTip(Tr("Clipboard URL Invalid") + " [" + url + "]", MessageType.Error, 3);
                }
            }
            if (GUILayout.Button(Tr("Location"), EditorStyles.miniButtonRight))
            {
                EditorUtility.RevealInFinder(AssetsManager.assetsFolderPath);
            }

            EditorGUILayout.EndHorizontal();

            UICommonFun.RichHelpBox(tip, tipType);

            EditorGUILayout.Separator();

            EditorGUILayout.Separator();

            _displayAssets = UICommonFun.Foldout(_displayAssets, TrLabel(nameof(_displayAssets)), () =>
            {
                if (GUILayout.Button(UICommonOption.Reset, EditorStyles.miniButtonRight, UICommonOption.WH20x16))
                {
                    AssetsManager.Reset();
                }
            });
            if (!_displayAssets) return;

            CommonFun.BeginLayout();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(TrLabel(nameof(searchText)));
            searchText = UICommonFun.SearchTextField(searchText);
            EditorGUILayout.LabelField(countInfo, UICommonOption.Width60);
            EditorGUILayout.EndHorizontal();

            // 标题
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.Label("NO.", UICommonOption.Width32);
                if (GUILayout.Button(Tr("Name"), UICommonOption.Width200))
                {
                    Sort(nameof(AssetItem.fileName), asc = !asc);
                }
                if (GUILayout.Button(TrLabel(nameof(applicableVersions)), UICommonOption.Width60))
                {
                    Sort(nameof(AssetItem.version), asc = !asc);
                }
                if (GUILayout.Button("", EditorObjectHelper.MiniPopup, UICommonOption.Width20))
                {
                    CommonFun.FocusControl();
                    EditorHelper.DrawMenu(searchText, applicableVersions, text =>
                    {
                        if (!searchText.SearchMatch(text))
                        {
                            if (searchText.EndsWith("/")) searchText += text + "/";
                            else searchText += "/" + text + "/";
                        }
                    });
                }
                if (GUILayout.Button(TrLabel(nameof(categorys)), UICommonOption.Width80))
                {
                    Sort(nameof(AssetItem.category), asc = !asc);
                }
                if (GUILayout.Button("", EditorObjectHelper.MiniPopup, UICommonOption.Width20))
                {
                    CommonFun.FocusControl();
                    EditorHelper.DrawMenu(searchText, categorys, text =>
                    {
                        if (!searchText.SearchMatch(text))
                        {
                            if (searchText.EndsWith("/")) searchText += text + "/";
                            else searchText += "/" + text + "/";
                        }
                    });
                }
                if (GUILayout.Button(Tr("Sub Category"), UICommonOption.Width100))
                {
                    Sort(nameof(AssetItem.subCategory), asc = !asc);
                }
                if (GUILayout.Button(Tr("State"), UICommonOption.Width64))
                {
                    Sort(nameof(AssetItem.itemState), asc = !asc);
                }
                if (GUILayout.Button(Tr("Begin Time"), UICommonOption.Width128))
                {
                    Sort(nameof(AssetItem.beginTime), asc = !asc);
                }
                if (GUILayout.Button(Tr("Progress")))
                {
                    Sort(nameof(AssetItem.progress), asc = !asc);
                }
                if (GUILayout.Button(Tr("End Time/Duration"), UICommonOption.Width128))
                {
                    Sort(nameof(AssetItem.endTime), asc = !asc);
                }
                GUILayout.Label(Tr("Operation"), UICommonOption.Width100);
            }
            EditorGUILayout.EndHorizontal();
            base.OnGUI();
        }

        /// <summary>
        /// 绘制带滚动视图的GUI
        /// </summary>
        [LanguageTuple("Download", "下载")]
        [LanguageTuple("Download And Import", "下载并导入")]
        [LanguageTuple("Download By Clipboard URL", "通过剪贴板URL下载")]
        [LanguageTuple("Clipboard URL Invalid", "剪贴板URL无效")]
        [LanguageTuple("Asset URL Invalid", "资产URL无效")]
        [LanguageTuple("Location", "定位")]
        [LanguageTuple("Name", "名称")]
        [LanguageTuple("Sub Category", "子分类")]
        [LanguageTuple("State", "状态")]
        [LanguageTuple("Begin Time", "开始时间")]
        [LanguageTuple("End Time/Duration", "结束时间/耗时")]
        [LanguageTuple("Progress", "进度")]
        [LanguageTuple("Operation", "操作")]
        public override void OnGUIWithScrollView()
        {
            for (int i = 0; i < displayItems.Count; i++)
            {
                var item = displayItems[i];
                UICommonFun.BeginHorizontal(i);
                EditorGUILayout.LabelField("", GUILayout.Width(4));
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                EditorGUILayout.SelectableLabel(item.fileName, UICommonOption.Height18, UICommonOption.Width200);

                EditorGUILayout.SelectableLabel(item.version, UICommonOption.Height18, UICommonOption.Width80);
                EditorGUILayout.SelectableLabel(item.category, UICommonOption.Height18, UICommonOption.Width100);
                EditorGUILayout.SelectableLabel(item.subCategory, UICommonOption.Height18, UICommonOption.Width100);

                var state = item.itemState;
                EditorGUILayout.LabelField(state.TrLabel(), GetGUIStyle(state), UICommonOption.Width64);

                EditorGUILayout.LabelField(item.beginTime.ToDefaultFormat(), UICommonOption.Width128);
                EditorGUILayout.Slider(item.progress, 0, 1);
                EditorGUILayout.LabelField(state != EItemState.Downloading ? item.endTime.ToDefaultFormat() : item.timeSpan.ToString(), UICommonOption.Width128);

                if (GUILayout.Button(UICommonOption.Run, EditorStyles.miniButtonMid, UICommonOption.Width20, UICommonOption.Height20))
                {
                    item.DownloadOrImport();
                }

                var valid = state == EItemState.DownloadDone;

                EditorGUI.BeginDisabledGroup(valid);
                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonMid, UICommonOption.Width20, UICommonOption.Height20))
                {
                    AssetsManager.Remove(item.assetUrl);
                }

                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(!valid);
                if (GUILayout.Button(Tr("Location"), EditorStyles.miniButtonLeft, UICommonOption.Width60))
                {
                    EditorUtility.RevealInFinder(item.localFullPath);
                }
                EditorGUI.EndDisabledGroup();
                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        private GUIStyle GetGUIStyle(EItemState itemState)
        {
            switch (itemState)
            {
                case EItemState.Unknow: return UICommonOption.labelBlueBG;
                case EItemState.Downloading: return UICommonOption.labelGreenBG;
                case EItemState.DownloadFail: return UICommonOption.labelRedBG;
                default: return GUI.skin.label;
            }
        }
    }
}
