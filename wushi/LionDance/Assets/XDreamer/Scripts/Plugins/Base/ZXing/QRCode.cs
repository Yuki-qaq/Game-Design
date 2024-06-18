using UnityEngine;
using XCSJ.PluginCommonUtils;
#if XDREAMER_ZXING || UNITY_EDITOR
using ZXing;
using ZXing.QrCode;
#endif

namespace XCSJ.Extension.Base.ZXing
{
    /// <summary>
    /// 二维码
    /// </summary>
    public class QRCode
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Color32[] Encode(string text, int width, int height)
        {
            if (string.IsNullOrEmpty(text) || width < 0 || height < 0)
            {
                return null;
            }

#if XDREAMER_ZXING || UNITY_EDITOR
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    CharacterSet = "utf-8",
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(text);
#else
            return Texture2DHelper.GetTexture2D(Color.white, width, height).GetPixels32();
#endif
        }

        /// <summary>
        /// 编码：将文本信息转为二维码点阵信息
        /// </summary>
        /// <param name="text">文本信息</param>
        /// <param name="width">点阵的宽</param>
        /// <param name="height">点阵的搞</param>
        /// <param name="pixels">像素点输出数组</param>
        /// <returns>如果文本信息为空或无效，点阵的宽或高小于0返回false</returns>
        public static bool Encode(string text, int width, int height, out Color32[] pixels)
        {
            pixels = Encode(text, width, height);
            return (pixels != null);
        }

        /// <summary>
        /// 编码为2D纹理
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool EncodeToTexture2D(Texture2D texture, string text)
        {
            Color32[] pixels;
            if (texture != null && Encode(text, texture.width, texture.height, out pixels))
            {
                texture.SetPixels32(pixels);
                texture.Apply();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 编码为2D纹理
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Texture2D EncodeToTexture2D(string text, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            if (EncodeToTexture2D(texture, text)) return texture;
            UnityEngine.Object.DestroyImmediate(texture);
            return null;
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string Decode(Color32[] pixels, int width, int height)
        {
#if XDREAMER_ZXING || UNITY_EDITOR
            BarcodeReader reader = new BarcodeReader();
            Result r = reader.Decode(pixels, width, height);
            if (r != null) return r.Text;
#endif
            return "";
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool Decode(Color32[] pixels, int width, int height, out string text)
        {
            text = Decode(pixels, width, height);
            return !string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool Decode(Texture2D texture, out string text)
        {
            if (texture != null && Decode(texture.GetPixels32(), texture.width, texture.height, out text)) return true;
            text = "";
            return false;
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool Decode(WebCamTexture texture, out string text)
        {
            if (texture != null && texture.isPlaying && Decode(texture.GetPixels32(), texture.width, texture.height, out text)) return true;
            text = "";
            return false;
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static string Decode(Texture2D texture)
        {
            string text = "";
            if (texture != null && Decode(texture.GetPixels32(), texture.width, texture.height, out text)) return text;
            return "";
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static string Decode(WebCamTexture texture)
        {
            string text = "";
            if (texture != null && texture.isPlaying && Decode(texture.GetPixels32(), texture.width, texture.height, out text)) return text;
            return "";
        }
    }
}
