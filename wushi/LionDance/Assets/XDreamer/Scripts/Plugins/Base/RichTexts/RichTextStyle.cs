using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.Tools;

namespace XCSJ.Extension.Base.RichTexts
{
    /// <summary>
    /// 富文本样式
    /// </summary>
    [Name("富文本样式")]
    [Import]
    [Serializable]
    public class RichTextStyle : Option, IToFriendlyString
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name = "";

        /// <summary>
        /// 提示
        /// </summary>
        public string tip = "";

        /// <summary>
        /// 颜色标记
        /// </summary>
        public bool colorFlag = false;

        /// <summary>
        /// 颜色
        /// </summary>
        [Json(exportString = true)]
        public Color color = UnityEngine.Color.black;

        /// <summary>
        /// 尺寸标记
        /// </summary>
        public bool sizeFlag = false;

        /// <summary>
        /// 尺寸
        /// </summary>
        public int size = 11;

        /// <summary>
        /// 粗体
        /// </summary>
        public bool bold = false;

        /// <summary>
        /// 斜体
        /// </summary>
        public bool italic = false;

        /// <summary>
        /// 有标记
        /// </summary>
        [Json(false)]
        public bool hasFlag => colorFlag || sizeFlag || bold || italic;

        /// <summary>
        /// 构造
        /// </summary>
        public RichTextStyle()
        {
            expand = true;
            active = true;
            enable = true;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        public RichTextStyle(int size, bool bold) : this()
        {
            this.size = size;
            this.sizeFlag = true;
            this.bold = bold;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        public RichTextStyle(Color color, int size, bool bold) : this(size, bold)
        {
            this.color = color;
            this.colorFlag = true;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        /// <param name="italic"></param>
        public RichTextStyle(Color color, int size, bool bold, bool italic) : this(color, size, bold)
        {
            this.italic = italic;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="text"></param>
        public RichTextStyle(string text) : this()
        {
            Set(text);
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="style"></param>
        public RichTextStyle(RichTextStyle style)
        {
            Set(style);
        }

        /// <summary>
        /// 是相同样式
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public bool IsSameStyle(RichTextStyle style)
        {
            if (style == null) return false;
            if (style == this) return true;
            if (colorFlag != style.colorFlag || sizeFlag != style.sizeFlag || bold != style.bold || italic != style.italic) return false;
            if (colorFlag && color != style.color) return false;
            if (sizeFlag && size != style.size) return false;
            return true;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="style"></param>
        public void Set(RichTextStyle style)
        {
            this.expand = style.expand;
            this.enable = style.enable;
            this.active = style.active;

            this.name = style.name;
            this.tip = style.tip;

            this.colorFlag = style.colorFlag;
            this.color = style.color;
            this.sizeFlag = style.sizeFlag;
            this.size = style.size;
            this.bold = style.bold;
            this.italic = style.italic;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="text"></param>
        public void Set(string text)
        {
            Set(RichText.GetOption(text));
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="flag"></param>
        public void Set(bool flag)
        {
            colorFlag = flag;
            sizeFlag = flag;
            bold = flag;
            italic = flag;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public string Get(string text, bool replace = true) => RichText.Add(text, this, replace);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Add(string text)
        {
            Set(true);
            return Get(text);
        }

        /// <summary>
        /// 清理
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Clear(string text)
        {
            Set(false);
            return Get(text);
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="text"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public string Format(string text, bool flag)
        {
            return flag ? Add(text) : Clear(text);
        }

        /// <summary>
        /// 颜色
        /// </summary>
        /// <param name="text"></param>
        /// <param name="colorFlag"></param>
        /// <returns></returns>
        public string Color(string text, bool colorFlag)
        {
            return colorFlag ? AddColorIfNeed(text) : ClearColor(text);
        }

        /// <summary>
        /// 颜色
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public string Color(string text, Color color)
        {
            return ReplaceColor(text, color, true);
        }

        /// <summary>
        /// 颜色
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Color(string text)
        {
            return colorFlag ? ReplaceColor(text) : ClearColor(text);
        }

        /// <summary>
        /// 添加颜色
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string AddColor(string text)
        {
            colorFlag = true;
            return RichText.AddColor(text, color);
        }

        /// <summary>
        /// 如果需要清理颜色
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string AddColorIfNeed(string text)
        {
            colorFlag = true;
            return RichText.AddColorIfNeed(text, color);
        }

        /// <summary>
        /// 清理颜色
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string ClearColor(string text)
        {
            colorFlag = false;
            return RichText.ClearColor(text);
        }

        /// <summary>
        /// 替换颜色
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="colorFlag"></param>
        /// <returns></returns>
        public string ReplaceColor(string text, Color color, bool colorFlag = true)
        {
            this.color = color;
            this.colorFlag = colorFlag;
            return Color(text);
        }

        /// <summary>
        /// 替换颜色
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string ReplaceColor(string text) => AddColor(ClearColor(text));

        /// <summary>
        /// 尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sizeFlag"></param>
        /// <returns></returns>
        public string Size(string text, bool sizeFlag)
        {
            return sizeFlag ? AddSizeIfNeed(text) : ClearSize(text);
        }

        /// <summary>
        /// 尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public string Size(string text, int size)
        {
            return ReplaceSize(text, size, true);
        }

        /// <summary>
        /// 尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Size(string text)
        {
            return sizeFlag ? ReplaceSize(text) : ClearSize(text);
        }

        /// <summary>
        /// 添加尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string AddSize(string text)
        {
            sizeFlag = true;
            return RichText.AddSize(text, size);
        }

        /// <summary>
        /// 如果需要添加尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string AddSizeIfNeed(string text)
        {
            sizeFlag = true;
            return RichText.AddSizeIfNeed(text, size);
        }

        /// <summary>
        /// 清理尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string ClearSize(string text)
        {
            sizeFlag = false;
            return RichText.ClearSize(text);
        }

        /// <summary>
        /// 替换尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <param name="sizeFlag"></param>
        /// <returns></returns>
        public string ReplaceSize(string text, int size, bool sizeFlag = true)
        {
            this.size = size;
            this.sizeFlag = sizeFlag;
            return Size(text);
        }

        /// <summary>
        /// 替换尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string ReplaceSize(string text) => AddSize(ClearSize(text));

        /// <summary>
        /// 粗体
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bold"></param>
        /// <returns></returns>
        public string Bold(string text, bool bold)
        {
            return bold ? AddBoldIfNeed(text) : ClearBold(text);
        }

        /// <summary>
        /// 粗体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Bold(string text)
        {
            return Bold(text, bold);
        }

        /// <summary>
        /// 添加粗体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string AddBold(string text)
        {
            bold = true;
            return RichText.AddBold(text);
        }

        /// <summary>
        /// 如果需要添加粗体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string AddBoldIfNeed(string text)
        {
            bold = true;
            return RichText.AddBoldIfNeed(text);
        }

        /// <summary>
        /// 清理粗体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string ClearBold(string text)
        {
            bold = false;
            return RichText.ClearBold(text);
        }

        /// <summary>
        /// 斜体
        /// </summary>
        /// <param name="text"></param>
        /// <param name="italic"></param>
        /// <returns></returns>
        public string Italic(string text, bool italic)
        {
            return italic ? AddItalicIfNeed(text) : ClearItalic(text);
        }

        /// <summary>
        /// 斜体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Italic(string text)
        {
            return Italic(text, italic);
        }

        /// <summary>
        /// 添加斜体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string AddItalic(string text)
        {
            italic = true;
            return RichText.AddItalic(text);
        }

        /// <summary>
        /// 添加斜体如果需要
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string AddItalicIfNeed(string text)
        {
            italic = true;
            return RichText.AddItalicIfNeed(text);
        }

        /// <summary>
        /// 清理斜体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string ClearItalic(string text)
        {
            italic = false;
            return RichText.ClearItalic(text);
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString()
        {
            return string.Format("{0}: {1}\n{2},{3},{4},{5}", name, tip, colorFlag ? ColorUtility.ToHtmlStringRGBA(color) : "默认颜色", sizeFlag ? size.ToString() : "默认字号", bold ? "加粗" : "正常", italic ? "斜体" : "正常");
        }
    }
}
