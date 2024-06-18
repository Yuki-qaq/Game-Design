using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.LitJson;

namespace XCSJ.EditorExtension.Base.XAssets.Items
{
    /// <summary>
    /// 资产项
    /// </summary>
    [Import]
    [LanguageFileOutput]
    public class AssetItem
    {
        /// <summary>
        /// 资产URL
        /// </summary>
        private string _assetUrl = null;

        /// <summary>
        /// 资产URL
        /// </summary>
        public string assetUrl
        {
            get => _assetUrl ?? "";
            set
            {
                if (string.IsNullOrEmpty(_assetUrl))
                {
                    _assetUrl = value ?? "";
                    CheckLocal();
                }
            }
        }

        private void ParseUrl() => AssetsManager.TryParseAssetUrl(assetUrl, out _version, out _category, out _subCategory, out _fileName);

        private string _version = null;

        /// <summary>
        /// 版本
        /// </summary>
        [Json(false)]
        public string version
        {
            get
            {
                if (_version == null) ParseUrl();
                return _version;
            }
        }

        private string _category = null;

        /// <summary>
        /// 分类
        /// </summary>
        [Json(false)]
        public string category
        {
            get
            {
                if (_category == null) ParseUrl();
                return _category;
            }
        }

        private string _subCategory = null;

        /// <summary>
        /// 子分类
        /// </summary>
        [Json(false)]
        public string subCategory
        {
            get
            {
                if (_subCategory == null) ParseUrl();
                return _subCategory;
            }
        }

        private string _fileName = null;

        /// <summary>
        /// 文件名
        /// </summary>
        [Json(false)]
        public string fileName
        {
            get
            {
                if (_fileName == null) ParseUrl();
                return _fileName;
            }
        }

        private string _dirFolderName = null;

        /// <summary>
        /// 目录文件夹名
        /// </summary>
        [Json(false)]
        public string dirFolderName => _dirFolderName ?? (_dirFolderName = string.IsNullOrEmpty(version) ? MD5Helper.Get32(assetUrl) : (version + "__" + category + "__" + subCategory));

        private string _localFullPath = null;

        /// <summary>
        /// 本地全路径
        /// </summary>
        [Json(false)]
        public string localFullPath => _localFullPath ?? (_localFullPath = AssetsManager.assetsFolderPath + "/" + dirFolderName + "/" + fileName);

        /// <summary>
        /// 是本地：即本地全路径对应的文件存在
        /// </summary>
        [Json(false)]
        public bool isLocal => FileHelper.Exists(localFullPath) && itemState != EItemState.Downloading;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime beginTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime endTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 时间跨度：下载耗时
        /// </summary>
        [Json(false)]
        public TimeSpan timeSpan => DateTime.Now - beginTime;

        /// <summary>
        /// 下载项状态
        /// </summary>
        [Json(false)]
        public EItemState itemState { get; private set; } = EItemState.Unknow;

        /// <summary>
        /// 当项已变更
        /// </summary>
        [Json(false)]
        public static Action<AssetItem> onItemChanged;

        /// <summary>
        /// 调用修改
        /// </summary>
        private void CallChanged() => onItemChanged?.Invoke(this);

        /// <summary>
        /// 进度
        /// </summary>
        [Json(false)]
        public float progress { get; set; } = 0;

        UnityWebRequest unityWebRequest;

        /// <summary>
        /// 下载
        /// </summary>
        public void Download()
        {
            switch (itemState)
            {
                case EItemState.Unknow:
                case EItemState.DownloadFail:
                    {
                        itemState = EItemState.WaitDownload;
                        RemoveUpdate();
                        EditorApplication.update += Update;
                        break;
                    }
            }
        }

        private void RemoveUpdate() => EditorApplication.update -= Update;

        [LanguageTuple("Invalid UnityWebRequest Object !", "无效UnityWebRequest对象！")]
        [LanguageTuple("Download file failed from Asset URL [", "从资产URL下载文件失败[")]
        private void Update()
        {
            switch (itemState)
            {
                case EItemState.WaitDownload:
                    {
                        progress = 0;
                        itemState = EItemState.Downloading;
                        beginTime = DateTime.Now;

                        unityWebRequest = new UnityWebRequest(assetUrl, UnityWebRequest.kHttpVerbGET, new DownloadHandlerFile(localFullPath) { removeFileOnAbort = true }, null);
                        unityWebRequest.SendWebRequest();

                        CallChanged();
                        break;
                    }
                case EItemState.Downloading:
                    {
                        if (unityWebRequest == null)
                        {
                            itemState = EItemState.DownloadFail;
                            Debug.LogError("Invalid UnityWebRequest Object !".Tr(GetType()));
                            break;
                        }
                        if (unityWebRequest.isDone)
                        {
                            itemState = EItemState.DownloadDone;
                            break;
                        }
#if UNITY_2020_2_OR_NEWER

                        if (unityWebRequest.result == UnityWebRequest.Result.ProtocolError || unityWebRequest.result == UnityWebRequest.Result.ConnectionError || !string.IsNullOrEmpty(unityWebRequest.error))
                        {
                            itemState = EItemState.DownloadFail;
                            Debug.LogError(unityWebRequest.error);
                            break;
                        }

#else

                        if (unityWebRequest.isHttpError || unityWebRequest.isNetworkError || !string.IsNullOrEmpty(unityWebRequest.error))
                        {
                            itemState = EItemState.DownloadFail;
                            Debug.LogError(unityWebRequest.error);
                            break;
                        }
#endif
                        progress = unityWebRequest.downloadProgress;

                        CallChanged();
                        break;
                    }
                case EItemState.DownloadDone:
                    {
                        unityWebRequest = default;
                        RemoveUpdate();
                        progress = 1;
                        endTime = DateTime.Now;

                        if (!CheckLocal())//下载失败
                        {
                            progress = 0;
                            itemState = EItemState.DownloadFail;
                            Debug.LogError("Download file failed from Asset URL [".Tr(GetType()) + assetUrl + "]");
                        }

                        CallChanged();
                        break;
                    }
                case EItemState.DownloadFail:
                    {
                        unityWebRequest = default;
                        RemoveUpdate();
                        progress = 0;
                        endTime = DateTime.Now;

                        CallChanged();
                        break;
                    }
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        public void Import()
        {
            if (isLocal)
            {
                AssetDatabase.ImportPackage(localFullPath, true);
            }
        }

        /// <summary>
        /// 检查本地
        /// </summary>
        /// <returns></returns>
        public bool CheckLocal()
        {
            if (isLocal)
            {
                progress = 1;
                itemState = EItemState.DownloadDone;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查下载
        /// </summary>
        public void CheckDownload()
        {
            if (!CheckLocal() && itemState != EItemState.Downloading)
            {
                progress = 0;
                itemState = EItemState.Unknow;
            }
            Download();
        }

        /// <summary>
        /// 下载或导入：无则下载，有则导入；
        /// </summary>
        public void DownloadOrImport()
        {
            CheckDownload();
            if (isLocal) Import();
        }

        /// <summary>
        /// 下载并导入：有则导入，无则下载并等待下载完成后导入；
        /// </summary>
        public void DownloadAndImport()
        {
            CheckDownload();
            if (isLocal) Import();
            else if (!inDelayImport)
            {
                inDelayImport = true;
                onItemChanged += DelayImport;
            }
        }

        private bool inDelayImport = false;

        private void DelayImport(AssetItem assetItem)
        {
            if (assetItem == this)
            {
                switch (itemState)
                {
                    case EItemState.DownloadDone:
                        {
                            inDelayImport = false;
                            onItemChanged -= DelayImport;
                            UICommonFun.DelayCall(Import);
                            break;
                        }
                    case EItemState.DownloadFail:
                        {
                            inDelayImport = false;
                            onItemChanged -= DelayImport;
                            break;
                        }
                }
            }
        }
    }

    /// <summary>
    /// 项状态
    /// </summary>
    public enum EItemState
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Name("未知")]
        Unknow,

        /// <summary>
        /// 等待下载
        /// </summary>
        [Name("等待下载")]
        WaitDownload,

        /// <summary>
        /// 下载中
        /// </summary>
        [Name("下载中")]
        Downloading,

        /// <summary>
        /// 下载完成
        /// </summary>
        [Name("下载完成")]
        DownloadDone,

        /// <summary>
        /// 下载失败
        /// </summary>
        [Name("下载失败")]
        DownloadFail,
    }
}
