using UnityEngine;
using System.Collections;
using System.Reflection;
using System.IO;
using System;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils.Tools;
#if XDREAMER_EASYAR_4_1_0
using easyar;
#endif

namespace XCSJ.PluginEasyAR
{
    /// <summary>
    /// 图片目标:用于图片标识的识别
    /// </summary>
    [Name("图片目标")]
    [Serializable]
    [Tool(EasyARHelper.Title)]
#if XDREAMER_EASYAR_4_1_0
    public class ImageTargetMB : ImageTargetController
#else
    public class ImageTargetMB : BaseEasyARMB
#endif
    {
        /// <summary>
        /// 识别图
        /// </summary>
        [Name("识别图")]
        [Tip("AR相机识别使用的Marker（识别图）;如果本参数有效会使用本参数的信息覆盖path、name、size等信息；", "Marker used for AR camera identification; If this parameter is valid, the information of this parameter will be used to overwrite path, name, size and other information;")]
        public Texture2D marker;

#if XDREAMER_EASYAR_4_1_0
        void Awake()
        {
            if (marker)
            {
                ImageFileSource.PathType = PathType.Absolute;
                if (string.IsNullOrEmpty(ImageFileSource.Name)) ImageFileSource.Name = marker.name;

                //将图片输出磁盘并获取绝对路径
                ImageFileSource.Path = string.Format("{0}/{1}.jpg", Application.persistentDataPath, marker.name);
                //Debug.Log(this.Path);
                File.WriteAllBytes(ImageFileSource.Path, marker.EncodeToJPG());
            }

            if (Tracker == null) Tracker = FindObjectOfType<ImageTrackerFrameFilter>();
        }
#endif
    }

    /// <summary>
    /// 基础EasyAR MB组件
    /// </summary>
    [RequireManager(typeof(EasyARManager))]
    [Owner(typeof(EasyARManager))]
    public abstract class BaseEasyARMB : InteractProvider { }
}
