using System;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.NodeKit
{
    /// <summary>
    /// 缩放区域
    /// </summary>
    public class ZoomArea
    {
        /// <summary>
        /// 原点：坐标原点
        /// </summary>
        public Vector2 origin { get; private set; } = Vector2.zero;

        /// <summary>
        /// 缩放：缩放系数
        /// </summary>
        public float zoom
        {
            get => _zoom;
            set
            {
                _zoom = Mathf.Clamp(value, range.x, range.y);
            }
        }

        private float _zoom = 1.0f;

        /// <summary>
        /// 范围：缩放范围
        /// </summary>
        public Vector2 range
        {
            get => _range;
            set
            {
                _range = value;
                zoom = _zoom;
            }
        }

        private Vector2 _range = new Vector2(0.1f, 2.2f);

        /// <summary>
        /// 速度：缩放速度
        /// </summary>
        public float speed { get; set; } = 0.025f;

        /// <summary>
        /// 前矩阵
        /// </summary>
        private Matrix4x4 _prevGuiMatrix;

        private float _offsetY = 22;

        /// <summary>
        /// 平移矩阵
        /// </summary>
        public Matrix4x4 translationMatrix { get; private set; }

        /// <summary>
        /// 缩放矩阵
        /// </summary>
        public Matrix4x4 scaleMatrix { get; private set; }

        /// <summary>
        /// 重置：重置缩放系数
        /// </summary>
        public void Reset()
        {
            zoom = 1;
            origin = Vector2.zero;
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="zoomRect"></param>
        /// <param name="tabHeight"></param>
        /// <param name="drawFun"></param>
        public void Draw(Rect zoomRect, float tabHeight, Action drawFun)
        {
            if (drawFun == null) return;

            Begin(zoomRect, tabHeight);
            drawFun.Invoke();
            End();
        }

        /// <summary>
        /// 缩放矩形
        /// </summary>
        public Rect zoomRect { get; private set; }

        /// <summary>
        /// 裁剪区域
        /// </summary>
        public Rect clippedArea => MathU.ScaleSizeBy(zoomRect, 1.0f / zoom, MathU.TopLeft(zoomRect));

        /// <summary>
        /// 开始：开启缩放区
        /// </summary>
        /// <param name="zoomRect"></param>
        /// <param name="offsetY"></param>
        public void Begin(Rect zoomRect, float offsetY = 0)
        {
            this.zoomRect = zoomRect;

            // 结束Unity编辑器窗口自动布局组，为了在窗口外绘制
            GUI.EndGroup();

            // 计算正确的裁剪区域
            var clippedArea = this.clippedArea;
            _offsetY = offsetY;
            clippedArea.y += _offsetY;
            GUI.BeginGroup(clippedArea);
            //GUI.Box(new Rect(0,0, zoomRect.width, zoomRect.height),"");
            _prevGuiMatrix = GUI.matrix;
            translationMatrix = Matrix4x4.TRS(MathU.TopLeft(clippedArea), Quaternion.identity, Vector3.one);
            scaleMatrix = Matrix4x4.Scale(new Vector3(zoom, zoom, 1.0f));
            GUI.matrix = translationMatrix * scaleMatrix * translationMatrix.inverse * _prevGuiMatrix;
        }

        /// <summary>
        /// 结束:结束缩放区
        /// </summary>
        public void End()
        {
            GUI.matrix = _prevGuiMatrix;
            GUI.EndGroup();

            // 重新开启Unity组
            //GUI.BeginGroup(zoomRect);
            GUI.BeginGroup(new Rect(0.0f, _offsetY, Screen.width, Screen.height));
        }

        /// <summary>
        /// 缩放：处理鼠标缩放区域后的偏移量；因为缩放与平移都会影响缩放区域的原点，函数必须放在缩放区域外部执行
        /// </summary>
        /// <param name="zoomRect">缩放区域</param>
        /// <param name="mousePosition">鼠标位置</param>
        public void Zoom(Rect zoomRect, Vector2 mousePosition)
        {
            float oldZoom = zoom;
            zoom -= Event.current.delta.y * speed;
            onZoomChanged?.Invoke((mousePosition - MathU.TopLeft(zoomRect)) / oldZoom * (1 - oldZoom / zoom));
        }

        /// <summary>
        /// 当缩放已变更
        /// </summary>
        public Action<Vector2> onZoomChanged { get; set; }

        /// <summary>
        /// 平移到缩放
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        public Vector2 TranslateToZoom(Vector2 screenPosition) => screenPosition - origin;

        /// <summary>
        /// 屏幕到缩放
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        public Vector2 ScreenToZoom(Vector2 screenPosition) => screenPosition / zoom;

        /// <summary>
        /// 缩放到屏幕
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        public Vector2 ZoomToScreen(Vector2 screenPosition) => screenPosition * zoom;
    }
}
