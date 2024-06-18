using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base;
using XCSJ.Products;
using XCSJ.Tools;

namespace XCSJ.EditorExtension.Base.XAssets.Items
{
    /// <summary>
    /// 资产管理器
    /// </summary>
    public class AssetsManager
    {
        static AssetsManager()
        {
            AssetItem.onItemChanged += OnItemChanged;
            Preferences.onOptionModified += OnOptionModified;
        }

        private static void OnItemChanged(AssetItem assetItem)
        {
            if (assetItem.itemState == EItemState.DownloadDone)
            {
                Save();
            }
        }

        private static void OnOptionModified(Option option)
        {
            if (option is AssetsManagerOption)
            {
                _assetsFolderPath = default;
                Reset();
            }
        }

        /// <summary>
        /// 当资产已加载
        /// </summary>
        public static event Action onAssetsLoaded;

        /// <summary>
        /// 资产项列表
        /// </summary>
        private static List<AssetItem> _assetItems;

        /// <summary>
        /// 资产项列表
        /// </summary>
        public static List<AssetItem> assetItems
        {
            get
            {
                if (_assetItems == null)
                {
                    LoadConfigFile(true);
                    onAssetsLoaded?.Invoke();
                }
                else
                {
                    var path = configFileFullPath;
                    if (FileHelper.Exists(path))
                    {
                        var newLastWriteTime = File.GetLastWriteTime(path);
                        if (newLastWriteTime != lastWriteTime)
                        {
                            lastWriteTime = newLastWriteTime;
                            LoadConfigFile(false);
                        }
                    }
                }
                return _assetItems;
            }
        }

        private static void LoadConfigFile(bool updateLastWriteTime)
        {
            _assetItems?.Clear();
            var path = configFileFullPath;
            if (FileHelper.Exists(path))
            {
                if (updateLastWriteTime) lastWriteTime = File.GetLastWriteTime(path);
                var jsonText = FileHelper.InputFile(path);
                if (!string.IsNullOrEmpty(jsonText))
                {
                    _assetItems = JsonHelper.ToObject<List<AssetItem>>(jsonText);
                }
            }
            if (_assetItems == null) _assetItems = new List<AssetItem>();
        }

        private static DateTime lastWriteTime = default;

        private static void UpdateLastWriteTime()
        {
            var path = configFileFullPath;
            if (FileHelper.Exists(path))
            {
                lastWriteTime = File.GetLastWriteTime(path);
            }
        }

        /// <summary>
        /// 重置：重新从磁盘文件加载信息
        /// </summary>
        public static void Reset()
        {
            LoadConfigFile(true);
            onAssetsLoaded?.Invoke();
        }

        /// <summary>
        /// 扩展名列表
        /// </summary>
        public static string[] Extensions = new string[] { ".unitypackage" };

        /// <summary>
        /// 配置文件名
        /// </summary>
        public static string configFileName = "assets.json";

        /// <summary>
        /// 配置文件全路径
        /// </summary>
        public static string configFileFullPath => assetsFolderPath + "/" + configFileName;

        /// <summary>
        /// 保存
        /// </summary>
        private static void Save()
        {
            FileHelper.OutputFile(configFileFullPath, JsonHelper.ToJson(assetItems, true));
            UpdateLastWriteTime();
        }

        /// <summary>
        /// 有效资产URL
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <returns></returns>
        public static bool ValidAssetUrl(string assetUrl)
        {
            if (string.IsNullOrEmpty(assetUrl)) return false;

            try
            {
                Uri uri = new Uri(assetUrl);
                if (!uri.IsAbsoluteUri) return false;

                if (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) return false;

                var ext = Path.GetExtension(assetUrl);
                if (Extensions.Any(e => string.Compare(e, ext, true) == 0)) return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试获取资产项
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <param name="assetItem"></param>
        /// <returns></returns>
        public static bool TryGetAssetItem(string assetUrl, out AssetItem assetItem)
        {
            assetItem = assetItems.FirstOrDefault(item => item.assetUrl == assetUrl);
            return assetItem != null;
        }

        /// <summary>
        /// 当添加下载
        /// </summary>
        public static event Action<AssetItem> onAddDownload;

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <returns></returns>
        public static AssetItem Download(string assetUrl)
        {
            if (!ValidAssetUrl(assetUrl)) return default;

            if (!TryGetAssetItem(assetUrl, out var item))
            {
                item = new AssetItem();
                item.assetUrl = assetUrl;
                assetItems.Add(item);

                onAddDownload?.Invoke(item);
            }
            item.Download();
            return item;
        }

        /// <summary>
        /// 当移除下载
        /// </summary>
        public static event Action<AssetItem> onRemoveDownload;

        /// <summary>
        /// 移除：只能移除下载失败的或是未知的，对于正在现在或下载完成的无法处理；
        /// </summary>
        /// <param name="assetUrl"></param>
        public static void Remove(string assetUrl)
        {
            if (TryGetAssetItem(assetUrl, out var item))
            {
                switch (item.itemState)
                {
                    case EItemState.Unknow:
                    case EItemState.DownloadFail:
                        {
                            _assetItems.Remove(item);

                            try
                            {
                                var dirPath = assetsFolderPath + "/" + item.dirFolderName;
                                if (Directory.Exists(dirPath)
                                    && Directory.GetFiles(dirPath).Length == 0)
                                {
                                    Directory.Delete(dirPath);
                                }
                            }
                            catch { }
                            Save();
                            onRemoveDownload?.Invoke(item);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 获取资产文件夹路径
        /// </summary>
        private static string _assetsFolderPath = null;

        /// <summary>
        /// 获取资产文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string assetsFolderPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_assetsFolderPath)) return _assetsFolderPath;

                if (AssetsManagerOption.weakInstance is AssetsManagerOption assetsManagerOption && assetsManagerOption.useCustomAssetsDirectory)
                {
                    if (!string.IsNullOrEmpty(assetsManagerOption.customAssetsDirectory) && Directory.Exists(assetsManagerOption.customAssetsDirectory))
                    {
                        _assetsFolderPath = assetsManagerOption.customAssetsDirectory;
                        return _assetsFolderPath;
                    }
                }

                _assetsFolderPath = DefaultAssetsFolderPath;
                return _assetsFolderPath;
            }
        }

        /// <summary>
        /// 默认资产文件夹路径
        /// </summary>
        public static string DefaultAssetsFolderPath
        {
            get
            {
                //在Unity编辑器内时，从软件官方的公司产品配置中获取
                var companyName = PlayerSettings.companyName;
                var productName = PlayerSettings.productName;
                try
                {
                    PlayerSettings.companyName = nameof(XCSJ);
                    PlayerSettings.productName = ProductServer.Name;

                    return Application.persistentDataPath + "/assets";
                }
                finally
                {
                    PlayerSettings.companyName = companyName;
                    PlayerSettings.productName = productName;
                }
            }
        }

        /// <summary>
        /// 主分类
        /// </summary>
        public const string MainCategory = "app";

        /// <summary>
        /// 下载并导入
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        public static AssetItem DownloadAndImport(string fileName, string category, string subCategory) => DownloadAndImport(CreateAssetUrl(fileName, category, subCategory));

        /// <summary>
        /// 下载并导入
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <returns></returns>
        public static AssetItem DownloadAndImport(string assetUrl)
        {
            var assetItem = Download(assetUrl);
            assetItem?.DownloadAndImport();
            return assetItem;
        }

        /// <summary>
        /// 尝试获取资产项
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="assetItem"></param>
        /// <returns></returns>
        public static bool TryGetAssetItem(string fileName, string category, string subCategory, out AssetItem assetItem)
        {
            return TryGetAssetItem(CreateAssetUrl(fileName, category, subCategory), out assetItem);
        }

        class UrlCache : TICache<UrlCache, string, string, string, string>
        {
            protected override KeyValuePair<bool, string> CreateValue(string key1, string key2, string key3)
            {
                return new KeyValuePair<bool, string>(true, CreateAssetUrlNoCache(key1, key2, key3));
            }
        }

        /// <summary>
        /// 创建资产URL
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        public static string CreateAssetUrl(string fileName, string category, string subCategory)
        {
            return UrlCache.GetCacheValue(fileName, category, subCategory);
        }

        /// <summary>
        /// 创建资产URL无缓存
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        public static string CreateAssetUrlNoCache(string fileName, string category, string subCategory)
        {
            return string.Format("https://{0}/{1}/{2}/{3}/{4}/{5}"
                , Product.WebSite
                , MainCategory
                , Product.PubliclyLastedVersion
                , category
                , subCategory
                , fileName
                );
        }

        class ParseUrlCache : TICache<ParseUrlCache, string, (bool, string, string, string, string)>
        {
            protected override KeyValuePair<bool, (bool, string, string, string, string)> CreateValue(string key1)
            {
                var r = TryParseAssetUrlNoCache(key1, out string version, out string category, out string subCategory, out string fileName);
                return new KeyValuePair<bool, (bool, string, string, string, string)>(true, (true, version, category, subCategory, fileName));
            }
        }

        /// <summary>
        /// 尝试分析资产URL
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <param name="version"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool TryParseAssetUrl(string assetUrl, out string version, out string category, out string subCategory, out string fileName)
        {
            var value = ParseUrlCache.GetCacheValue(assetUrl);
            version = value.Item2;
            category = value.Item3;
            subCategory = value.Item4;
            fileName = value.Item5;
            return value.Item1;
        }

        /// <summary>
        /// 尝试分析资产URL
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <param name="version"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool TryParseAssetUrlNoCache(string assetUrl, out string version, out string category, out string subCategory, out string fileName)
        {
            try
            {
                var uri = new Uri(assetUrl);
                if (uri.Scheme == Uri.UriSchemeHttps && Product.WebSite == uri.Host)
                {
                    fileName = Path.GetFileName(assetUrl);

                    var p0 = Path.GetDirectoryName(assetUrl);
                    subCategory = Path.GetFileName(p0);

                    var p1 = Path.GetDirectoryName(p0);
                    category = Path.GetFileName(p1);

                    var p2 = Path.GetDirectoryName(p1);
                    version = Path.GetFileName(p2);

                    var p3 = Path.GetDirectoryName(p2);
                    if (Path.GetFileName(p3) == MainCategory) return true;
                }
            }
            catch { }
            version = "";
            category = "";
            subCategory = "";
            fileName = "";
            return false;
        }
    }

    #region 资产管理器选项

    /// <summary>
    /// 资产管理器选项
    /// </summary>
    [XDreamerPreferences(index = IDRange.Begin + 61)]
    [Name(Product.Name + "-资产管理器")]
    [Import]
    public class AssetsManagerOption : XDreamerOption<AssetsManagerOption>
    {
        /// <summary>
        /// 使用自定义资产目录
        /// </summary>
        [Name("使用自定义资产目录")]
        public bool useCustomAssetsDirectory = false;

        /// <summary>
        /// 自定义资产目录
        /// </summary>
        [Name("自定义资产目录")]
        [Tip("用户需确保此目录路径支持文件的读写操作，否则会导致资产无法正常下载或导入；", "Users need to ensure that this directory path supports file read and write operations, otherwise it may cause assets to be unable to download or import normally;")]
        public string customAssetsDirectory = "";

        /// <summary>
        /// 当初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();
            if (string.IsNullOrEmpty(customAssetsDirectory) || !Directory.Exists(customAssetsDirectory))
            {
                customAssetsDirectory = AssetsManager.DefaultAssetsFolderPath;
            }
        }
    }

    #endregion

    #region 资产管理器选项编辑器

    /// <summary>
    /// 资产管理器选项编辑器
    /// </summary>
    [CommonEditor(typeof(AssetsManagerOption))]
    public class AssetsManagerOptionEditor : XDreamerOptionEditor<AssetsManagerOption>
    {
        /// <summary>
        /// 当绘制GUI
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public override bool OnGUI(object obj, FieldInfo fieldInfo)
        {
            var preference = this.preference;
            switch (fieldInfo.Name)
            {
                case nameof(preference.customAssetsDirectory):
                    {
                        preference.customAssetsDirectory = UICommonFun.DrawOpenFolder(fieldInfo.TrLabel(), EIcon.Open.TrLabel(ENameTip.Image), preference.customAssetsDirectory, UICommonOption.Width80, UICommonOption.Height18);
                        return true;
                    }
            }
            return base.OnGUI(obj, fieldInfo);
        }
    }

    #endregion
}
