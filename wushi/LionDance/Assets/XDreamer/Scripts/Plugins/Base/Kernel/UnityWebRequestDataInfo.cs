using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using XCSJ.Extension.Base.Audios;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.Extension.Base.Kernel
{
    /// <summary>
    /// Unity Web请求数据信息
    /// </summary>
    public class UnityWebRequestDataInfo : IDataInfo
    {
        /// <summary>
        /// 数据请求
        /// </summary>
        public DataRequest dataRequest { get; private set; } = null;

        /// <summary>
        /// Unity Web请求
        /// </summary>
        public UnityWebRequest unityWebRequest { get; private set; } = null;

        /// <summary>
        /// 纹理
        /// </summary>
        public Texture2D texture
        {
            get
            {
                try
                {
                    return DownloadHandlerTexture.GetContent(unityWebRequest);
                }
                catch { }

                try
                {
                    if (unityWebRequest.downloadProgress == 1)
                    {
                        var tex = new Texture2D(2, 2);
                        tex.name = Path.GetFileNameWithoutExtension(unityWebRequest.url);
                        tex.LoadImage(unityWebRequest.downloadHandler.data, false);
                        return tex;
                    }
                }
                catch { }

                return null;
            }
        }

        /// <summary>
        /// 音频剪辑
        /// </summary>
        public AudioClip audioClip
        {
            get
            {
                try
                {
#if CSHARP_7_3_OR_NEWER
                    if (DownloadHandlerAudioClip.GetContent(unityWebRequest) is AudioClip audio && audio)
                    {
                        return audio;
                    }
#else
                    AudioClip audio = DownloadHandlerAudioClip.GetContent(unityWebRequest) as AudioClip;
                    if (audio)
                    {
                        return audio;
                    }   

#endif
                }
                catch { }

                try
                {
                    if (unityWebRequest.downloadProgress == 1)
                    {
                        return AudioClipHelper.LoadFromMemory(unityWebRequest.downloadHandler.data, Path.GetFileNameWithoutExtension(unityWebRequest.url));
                    }
                }
                catch { }

                return null;
            }
        }

        /// <summary>
        /// 资产包
        /// </summary>
        public AssetBundle assetBundle
        {
            get
            {
                try
                {
                    return DownloadHandlerAssetBundle.GetContent(unityWebRequest);
                }
                catch { }

                try
                {
                    if (unityWebRequest.downloadProgress == 1)
                    {
                        return AssetBundle.LoadFromMemory(unityWebRequest.downloadHandler.data);
                    }
                }
                catch { }

                return null;
            }
        }

        /// <summary>
        /// 全路径
        /// </summary>
        public string fullPath => unityWebRequest.url; //unityWebRequest.uri.AbsolutePath;

        /// <summary>
        /// URL
        /// </summary>
        public string url => unityWebRequest.url;

        /// <summary>
        /// 文本
        /// </summary>
        public string text => unityWebRequest.downloadHandler.text;

        /// <summary>
        /// 字节
        /// </summary>
        public byte[] bytes => unityWebRequest.downloadHandler.data;

        /// <summary>
        /// 是错误
        /// </summary>
#if UNITY_2020_2_OR_NEWER
        public bool isError => unityWebRequest.result == UnityWebRequest.Result.ConnectionError;
#else
        public bool isError => unityWebRequest.isNetworkError;
#endif

        /// <summary>
        /// 错误
        /// </summary>
        public string error => unityWebRequest.error;

        /// <summary>
        /// 精度
        /// </summary>
        public float progress => unityWebRequest.downloadProgress;

        /// <summary>
        /// 是完成
        /// </summary>
        public bool isDone => unityWebRequest.isDone;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="unityWebRequest"></param>
        /// <param name="dataRequest"></param>
        public UnityWebRequestDataInfo(UnityWebRequest unityWebRequest, DataRequest dataRequest)
        {
#if CSHARP_7_3_OR_NEWER
            this.unityWebRequest = unityWebRequest ?? throw new ArgumentNullException(nameof(unityWebRequest));
            this.dataRequest = dataRequest ?? throw new ArgumentNullException(nameof(dataRequest));
#else
            if(unityWebRequest == null) throw new ArgumentNullException(nameof(unityWebRequest));
            if (dataRequest == null) throw new ArgumentNullException(nameof(dataRequest));
            this.unityWebRequest = unityWebRequest;
            this.dataRequest = dataRequest;
#endif
        }
    }
}
