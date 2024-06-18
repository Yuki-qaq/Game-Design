using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;

#if UNITY_2021_3_OR_NEWER
using UnityEditor.Search;
#endif

namespace XCSJ.EditorExtension.Base.Searches
{
    /// <summary>
    /// 搜索提供者组手
    /// </summary>
    [LanguageFileOutput]
    public static class SearchProviderHelper
    {
#if UNITY_2021_3_OR_NEWER

        /// <summary>
        /// 编号
        /// </summary>
        internal const string id = nameof(XDreamer);

        /// <summary>
        /// 名称
        /// </summary>
        internal const string name = nameof(XDreamer);

        /// <summary>
        /// 创建提供者
        /// </summary>
        /// <returns></returns>
        [SearchItemProvider]
        internal static SearchProvider CreateProvider()
        {
            return new SearchProvider(id, name)
            {
                active = false,
                filterId = "xd:",
                priority = 99999,
                fetchItems = OnFetchItems,
                toObject = (item, type) => GetTypeInfo(item)?.monoScript,
                showDetailsOptions = ShowDetailsOptions.Inspector | ShowDetailsOptions.Actions,
                trackSelection = (item, context) =>
                {
                    EditorGUIUtility.PingObject(GetTypeInfo(item)?.monoScript);
                },
                startDrag = (item, context) =>
                {
                    //DragAndDrop.PrepareStartDrag();
                    //DragAndDrop.objectReferences = new Object[] { obj };
                    //DragAndDrop.StartDrag(item.label);
                },
            };
        }

        /// <summary>
        /// 动作处理程序
        /// </summary>
        /// <returns></returns>
        [SearchActionsProvider]
        [LanguageTuple("Select", "选择")]
        [LanguageTuple("Select the asset file corresponding to the search item", "选择搜索项对应的资产文件")]
        [LanguageTuple("Document Manual", "文档手册")]
        [LanguageTuple("Click to jump to the document manual", "点击跳转文档手册")]
        internal static IEnumerable<SearchAction> ActionHandlers()
        {
            return new[]
            {
                new SearchAction(id, "Select".Tr(typeof(SearchProviderHelper)), null, "Select the asset file corresponding to the search item".Tr(typeof(SearchProviderHelper)))
                {
                    handler = (item) =>
                    {
                        Selection.activeObject =GetTypeInfo(item)?.monoScript;
                    }
                },
                new SearchAction(id, "Document Manual".Tr(typeof(SearchProviderHelper)), null, "Click to jump to the document manual".Tr(typeof(SearchProviderHelper)))
                {
                    handler = (item) =>
                    {
                        CommonFun.OpenManual(GetTypeInfo(item)?.type??typeof(XDreamer));
                    }
                },
            };
        }

        class TypeInfo : SearchItem
        {
            public Type type;
            public MonoScript monoScript;

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="type"></param>
            public TypeInfo(Type type) : base(type.FullName)
            {
                this.type = type;
                monoScript = EditorHelper.GetMonoScript(type);
                label = type.Tr();
                description = type.TrTip();
                thumbnail = preview = AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(monoScript)) as Texture2D;
            }

            /// <summary>
            /// 匹配
            /// </summary>
            /// <param name="searchContext"></param>
            /// <returns></returns>
            public bool Match(SearchContext searchContext)
            {
                return searchContext.searchWords.Any(w => type.FullName.Contains(w, StringComparison.OrdinalIgnoreCase)
                || label.Contains(w, StringComparison.OrdinalIgnoreCase)
                || description.Contains(w, StringComparison.OrdinalIgnoreCase)
                );
            }
        }

        static Dictionary<string, TypeInfo> _types = null;

        static Dictionary<string, TypeInfo> types
        {
            get
            {
                if (_types == null)
                {
                    _types = new Dictionary<string, TypeInfo>();
                    foreach (var t in TypeHelper.GetTypes(EditorHelper.ValidApiMember))
                    {
                        _types[t.FullName] = new TypeInfo(t);
                    }
                }
                return _types;
            }
        }

        static TypeInfo GetTypeInfo(SearchItem searchItem)
        {
            if (searchItem is TypeInfo typeInfo0) return typeInfo0;
            if (types.TryGetValue(searchItem.id, out var typeInfo1)) return typeInfo1;
            return default;
        }

        private static object OnFetchItems(SearchContext searchContext, List<SearchItem> searchItems, SearchProvider searchProvider)
        {
            return OnSyncFetchItems(searchContext, searchItems, searchProvider);
        }

        private static IEnumerable<SearchItem> OnSyncFetchItems(SearchContext searchContext, List<SearchItem> searchItems, SearchProvider searchProvider)
        {
            if (string.IsNullOrEmpty(searchContext.searchQuery)) return null;

            foreach (var kv in types)
            {
                var item = kv.Value;
                if (item.Match(searchContext))
                {
                    item.context = searchContext;
                    item.provider = searchProvider;
                    searchItems.Add(item);
                }
            }
            return null;
        }

#endif
    }
}
