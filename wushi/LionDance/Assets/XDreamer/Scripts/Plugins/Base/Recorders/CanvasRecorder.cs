using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 画布记录器
    /// </summary>
    public class CanvasRecorder : Recorder<Canvas, CanvasRecorder.Info>
    {
        /// <summary>
        /// 记录信息类
        /// </summary>
        public class Info : ISingleRecord<Canvas>
        {
            private Canvas canvas;

            private RenderMode renderMode;
            private Camera worldCamera;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="canvas"></param>
            public void Record(Canvas canvas)
            {
                this.canvas = canvas;

                renderMode = canvas.renderMode;
                worldCamera = canvas.worldCamera;
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                canvas.renderMode = renderMode;
                worldCamera = canvas.worldCamera;
            }
        }
    }
}
