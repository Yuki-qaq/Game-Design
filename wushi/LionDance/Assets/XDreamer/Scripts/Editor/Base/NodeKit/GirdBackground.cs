using UnityEngine;

namespace XCSJ.EditorExtension.Base.NodeKit
{
    /// <summary>
    /// 网格背景
    /// </summary>
    public class GirdBackground
    {
        /// <summary>
        /// 网格平移量
        /// </summary>
        public Vector2 panOffset = Vector2.zero;

        private Texture2D _gridTexture2D = null;

        private Color _backgroundColor = new Color(178.0f/255, 178.0f / 255, 178.0f / 255, 1f);

        private Color _lineColor = new Color(107.0f / 255, 107.0f / 255, 107.0f / 255, 1f);

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="backgroundColor"></param>
        /// <param name="lineColor"></param>
        public void SetColor(Color backgroundColor, Color lineColor)
        {
            _backgroundColor = backgroundColor;
            _lineColor = lineColor;
            _gridTexture2D = GenerateTexture(_lineColor, _backgroundColor);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="zoom"></param>
        public void Draw(Rect rect, float zoom)
        {
            if (_gridTexture2D == null)
            {
                _gridTexture2D = GenerateTexture(_lineColor, _backgroundColor);
            }

            // 单元偏移
            var xOffset = panOffset.x / _gridTexture2D.width;
            var yOffset = (-rect.size.y / zoom - panOffset.y) / _gridTexture2D.height;

            // 单元数量
            var amountX = Mathf.Round(rect.size.x / zoom) / _gridTexture2D.width;
            var amountY = Mathf.Round(rect.size.y / zoom) / _gridTexture2D.height;

            GUI.DrawTextureWithTexCoords(rect, _gridTexture2D, new Rect(xOffset, yOffset, amountX, amountY));
        }

        /// <summary>
        /// 产生贴图
        /// </summary>
        /// <param name="lineColor"></param>
        /// <param name="backgroundColor"></param>
        /// <returns></returns>
        public static Texture2D GenerateTexture(Color lineColor, Color backgroundColor)
        {
            var tex = new Texture2D(64, 64);
            var colorArray = new Color[64 * 64];
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    var col = backgroundColor;
                    if (y % 16 == 0 || x % 16 == 0) col = Color.Lerp(lineColor, backgroundColor, 0.8f);
                    if (y == 63 || x == 63) col = Color.Lerp(lineColor, backgroundColor, 0.2f);
                    colorArray[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(colorArray);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Trilinear;
            tex.Apply();
            return tex;
        }
    }
}

