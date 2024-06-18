using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XCSJ.EditorExtension.Base.XAssets.Libs
{
    /// <summary>
    /// 图像
    /// </summary>
    public class Image
    {
        /// <summary>
        /// 图像已加载
        /// </summary>
        public bool imageLoaded = false;

        /// <summary>
        /// 图像开始加载中
        /// </summary>

        public bool imageStartedLoading = false;

        /// <summary>
        /// 图像
        /// </summary>
        public Texture2D image = null;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="imageID"></param>
        /// <param name="label"></param>
        /// <param name="isAssetImage"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public Image(string imageID = "", string label = "", bool isAssetImage = false, int height = 100, int width = 100)
        {
            bool flag = imageID == "";
            if (!flag)
            {
                this.imageStartedLoading = true;
                ImageDownloaderJob job;
                job = new ImageDownloaderJob(imageID);

                ImageDownloader.Instance.AddJob(job, delegate (Texture2D image)
                {
                    this.image = image;
                    this.imageLoaded = true;
                    this.imageStartedLoading = false;
                    if (this.image != null)
                    {
                        this.PostLoadProcess();
                    }
                });
            }
        }

        /// <summary>
        /// 加载后过程
        /// </summary>
        public virtual void PostLoadProcess()
        {
        }
    }
}
