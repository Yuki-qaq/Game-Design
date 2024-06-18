using UnityEngine;

namespace XCSJ.EditorExtension.Base.XAssets.Libs
{
    /// <summary>
    /// 加载页面
    /// </summary>
    public class LoadingPanel : BasePanel
    {
        private float loadingTime = 0;

        /// <summary>
        /// 更新加载动画
        /// </summary>
        public void UpdateLoadingAnimation()
        {
            AssetLibWindow.instance.spinnerImage.Update();
            loadingTime += Time.deltaTime;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            loadingTime = 0;
        }

        /// <summary>
        /// 渲染
        /// </summary>
        public override void Render()
        {
            Rect position = AssetLibWindow.instance.position;
            if (AssetLibWindow.instance.spinnerImage.sprites != null)
            {
                Rect rectLoading = new Rect(position.width / 2f - 100f, position.height / 2f - 100f, 200f, 200f);
                GUI.DrawTexture(rectLoading, AssetLibWindow.instance.spinnerImage.sprites[AssetLibWindow.instance.spinnerImage.currFrameIndex], ScaleMode.ScaleToFit);
                   
            }
            Rect rectText = new Rect(position.width / 2f - 20f, position.height / 2f + 120f, 100f, 50f);
            GUI.Label(rectText, "加载中…");
            if (Event.current.type == EventType.Repaint)
            {
                if (loadingTime > 5)
                    AssetLibWindow.instance.EnterLoad();
            }
        }
    }
}
