using System;
using UnityEngine;

namespace XCSJ.EditorExtension.Base.XAssets.Libs
{
    /// <summary>
    /// 动画图像
    /// </summary>
    public class AnimatedImage : Image
    {
        /// <summary>
        /// 精灵
        /// </summary>
        public Texture2D[] sprites = null;

        /// <summary>
        /// 数量帧
        /// </summary>
        public int numFrames = 0;

        /// <summary>
        /// 每秒帧数
        /// </summary>
        public int framesPerSecond = 0;

        /// <summary>
        /// 每行帧数
        /// </summary>
        public int framesPerRow = 0;

        /// <summary>
        /// 帧宽
        /// </summary>
        public int frameWidth = 0;

        /// <summary>
        /// 帧高
        /// </summary>
        public int frameHeight = 0;

        /// <summary>
        /// 当前帧索引
        /// </summary>
        public int currFrameIndex = 0;

        /// <summary>
        /// 间隔时间
        /// </summary>
        public static float deltime = 0f;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="imageID"></param>
        /// <param name="label"></param>
        /// <param name="isAssetImage"></param>
        /// <param name="numframes"></param>
        /// <param name="fps"></param>
        /// <param name="framesperrow"></param>
        /// <param name="framewidth"></param>
        /// <param name="frameheight"></param>
        public AnimatedImage(string imageID = "", string label = "", bool isAssetImage = false, int numframes = 0, int fps = 0, int framesperrow = 0, int framewidth = 0, int frameheight = 0) : base(imageID, label, isAssetImage, 100, 100)
        {
            this.numFrames = numframes;
            this.framesPerSecond = fps;
            this.framesPerRow = framesperrow;
            this.frameWidth = framewidth;
            this.frameHeight = frameheight;
        }

        /// <summary>
        /// 拆分精灵表
        /// </summary>
        public void SplitSpriteSheet()
        {
            this.sprites = new Texture2D[this.numFrames];
            for (int i = 0; i < this.numFrames; i++)
            {
                int num = (int)(Math.Ceiling((double)this.numFrames / (double)this.framesPerRow) - 1.0) * this.frameHeight;
                int num2 = i % this.framesPerRow;
                int num3 = i / this.framesPerRow;
                this.sprites[i] = new Texture2D(this.frameWidth, this.frameHeight, TextureFormat.ARGB32, false, true);
                this.sprites[i].SetPixels(this.image.GetPixels(num2 * this.frameWidth, num - num3 * this.frameHeight, this.frameWidth, this.frameHeight));
                this.sprites[i].Apply();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            AnimatedImage.deltime += Time.fixedDeltaTime;
            bool flag = AnimatedImage.deltime > 1f / (float)this.framesPerSecond;
            if (flag)
            {
                this.currFrameIndex++;
                AnimatedImage.deltime = 0f;
            }
            bool flag2 = this.currFrameIndex >= this.numFrames;
            if (flag2)
            {
                this.currFrameIndex = 0;
            }
        }

        /// <summary>
        /// 加载后过程
        /// </summary>
        public override void PostLoadProcess()
        {
            base.PostLoadProcess();
            this.SplitSpriteSheet();
        }
    }
}
