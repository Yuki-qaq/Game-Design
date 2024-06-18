using System.Text.RegularExpressions;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.RichTexts
{
    /// <summary>
    /// 富文本
    /// </summary>
    public class RichText
    {
        /// <summary>
        /// 富文本
        /// </summary>
        public static XGUIStyle richText => CommonGUIStyle.richText;

        /// <summary>
        /// 富文本左下
        /// </summary>
        public static XGUIStyle richTextLowerLeft { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
        {
            s.alignment = TextAnchor.LowerLeft;
            s.richText = true;
            s.wordWrap = true;
        });

        /// <summary>
        /// 富文本按钮
        /// </summary>
        public static XGUIStyle buttonRichText { get; } = new XGUIStyle(nameof(GUI.skin.button), s =>
        {
            s.richText = true;
        });

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        /// <param name="italic"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string Add(string text, Color color, int size, bool bold = true, bool italic = true, bool replace = true)
        {
            if (replace) text = Clear(text);
            if (italic) text = AddItalic(text);
            if (bold) text = AddBold(text);
            text = AddSize(text, size);
            text = AddColor(text, color);
            return text;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="text"></param>
        /// <param name="option"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string Add(string text, RichTextStyle option, bool replace = true)
        {
            if (replace) text = Clear(text);
            return AddDirect(text, option);
        }

        /// <summary>
        /// 直接添加
        /// </summary>
        /// <param name="text"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string AddDirect(string text, RichTextStyle option)
        {
            if (option.italic) text = AddItalic(text);
            if (option.bold) text = AddBold(text);
            if (option.sizeFlag) text = AddSize(text, option.size);
            if (option.colorFlag) text = AddColor(text, option.color);
            return text;
        }

        /// <summary>
        /// 清理
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Clear(string text)
        {
            text = ClearColor(text);
            text = ClearSize(text);
            text = ClearBold(text);
            text = ClearItalic(text);
            return text;
        }

        /// <summary>
        /// 开始颜色模式左
        /// </summary>
        public const string BeginColorPatternLeft = "<color=#";

        /// <summary>
        /// 开始颜色模式中
        /// </summary>
        public const string BeginColorPatternMid = @"[0123456789abcdefABCDEF]+";

        /// <summary>
        /// 开始颜色模式右
        /// </summary>
        public const string BeginColorPatternRight = ">";

        /// <summary>
        /// 开始颜色模式
        /// </summary>
        public const string BeginColorPattern = BeginColorPatternLeft + BeginColorPatternMid + BeginColorPatternRight;

        /// <summary>
        /// 结束颜色模式
        /// </summary>
        public const string EndColorPattern = "</color>";

        /// <summary>
        /// 添加颜色
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string AddColor(string text, Color color)
        {
            return string.Format("{0}{1}{2}{3}{4}", BeginColorPatternLeft, ColorUtility.ToHtmlStringRGBA(color), BeginColorPatternRight, text, EndColorPattern);
        }

        /// <summary>
        /// 如果需要添加颜色
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string AddColorIfNeed(string text, Color color)
        {
            return HasColor(text) ? text : AddColor(text, color);
        }

        /// <summary>
        /// 清理颜色
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ClearColor(string text)
        {
            text = Regex.Replace(text, BeginColorPattern, "");
            text = text.Replace(EndColorPattern, "");
            return text;
        }

        /// <summary>
        /// 替换颜色
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ReplaceColor(string text, Color color) => AddColor(ClearColor(text), color);

        /// <summary>
        /// 有颜色
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool HasColor(string text)
        {
            return Regex.IsMatch(text, BeginColorPattern) && text.Contains(EndColorPattern);
        }

        /// <summary>
        /// 尝试获取颜色
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool TryGetColor(string text, out Color color)
        {
            var match = Regex.Match(text, BeginColorPattern);
            if (match.Success)
            {
                return ColorUtility.TryParseHtmlString("#" + match.Value.Replace(BeginColorPatternLeft, "").Replace(BeginColorPatternRight, ""), out color);
            }
            color = Color.black;
            return false;
        }

        /// <summary>
        /// 开始尺寸模式左
        /// </summary>
        public const string BeginSizePatternLeft = "<size=";

        /// <summary>
        /// 开始尺寸模式中
        /// </summary>
        public const string BeginSizePatternMid = @"\d+";

        /// <summary>
        /// 开始尺寸模式右
        /// </summary>
        public const string BeginSizePatternRight = ">";

        /// <summary>
        /// 开始尺寸模式
        /// </summary>
        public const string BeginSizePattern = BeginSizePatternLeft + BeginSizePatternMid + BeginSizePatternRight;

        /// <summary>
        /// 结束尺寸模式
        /// </summary>
        public const string EndSizePattern = "</size>";

        /// <summary>
        /// 添加尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string AddSize(string text, int size)
        {
            if (size < 0) return text;
            return string.Format("{0}{1}{2}{3}{4}", BeginSizePatternLeft, size, BeginSizePatternRight, text, EndSizePattern);
        }

        /// <summary>
        /// 如果需要添加尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string AddSizeIfNeed(string text, int size)
        {
            return HasSize(text) ? text : AddSize(text, size);
        }

        /// <summary>
        /// 清理尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ClearSize(string text)
        {
            text = Regex.Replace(text, BeginSizePattern, "");
            text = text.Replace(EndSizePattern, "");
            return text;
        }

        /// <summary>
        /// 替换尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ReplaceSize(string text, int size) => AddSize(ClearSize(text), size);

        /// <summary>
        /// 有尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool HasSize(string text)
        {
            return Regex.IsMatch(text, BeginSizePattern) && text.Contains(EndSizePattern);
        }

        /// <summary>
        /// 尝试获取尺寸
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool TryGetSize(string text, out int size)
        {
            var match = Regex.Match(text, BeginSizePattern);
            if (match.Success)
            {
                return int.TryParse(match.Value.Replace(BeginSizePatternLeft, "").Replace(BeginSizePatternRight, ""), out size);
            }
            size = 0;
            return false;
        }

        /// <summary>
        /// 开始粗体模式
        /// </summary>
        public const string BeginBoldPattern = "<b>";

        /// <summary>
        /// 结束粗体模式
        /// </summary>
        public const string EndBoldPattern = "</b>";

        /// <summary>
        /// 粗体
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bold"></param>
        /// <returns></returns>
        public static string Bold(string text, bool bold)
        {
            return bold ? AddBoldIfNeed(text) : ClearBold(text);
        }

        /// <summary>
        /// 添加粗体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AddBold(string text)
        {
            return string.Format("{0}{1}{2}", BeginBoldPattern, text, EndBoldPattern);
        }

        /// <summary>
        /// 如果需要添加粗体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AddBoldIfNeed(string text)
        {
            return HasBold(text) ? text : AddBold(text);
        }

        /// <summary>
        /// 清理粗体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ClearBold(string text)
        {
            text = text.Replace(BeginBoldPattern, "");
            text = text.Replace(EndBoldPattern, "");
            return text;
        }

        /// <summary>
        /// 有粗体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool HasBold(string text) => text.Contains(BeginBoldPattern) && text.Contains(EndBoldPattern);

        /// <summary>
        /// 开始斜体模式
        /// </summary>
        public const string BeginItalicPattern = "<i>";

        /// <summary>
        /// 结束斜体模式
        /// </summary>
        public const string EndItalicPattern = "</i>";

        /// <summary>
        /// 斜体
        /// </summary>
        /// <param name="text"></param>
        /// <param name="italic"></param>
        /// <returns></returns>
        public static string Italic(string text, bool italic)
        {
            return italic ? AddItalicIfNeed(text) : ClearItalic(text);
        }

        /// <summary>
        /// 添加斜体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AddItalic(string text)
        {
            return string.Format("{0}{1}{2}", BeginItalicPattern, text, EndItalicPattern);
        }

        /// <summary>
        /// 如果需要添加斜体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AddItalicIfNeed(string text)
        {
            return HasItalic(text) ? text : AddItalic(text);
        }

        /// <summary>
        /// 清理斜体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ClearItalic(string text)
        {
            text = text.Replace(BeginItalicPattern, "");
            text = text.Replace(EndItalicPattern, "");
            return text;
        }

        /// <summary>
        /// 有斜体
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool HasItalic(string text) => text.Contains(BeginItalicPattern) && text.Contains(EndItalicPattern);

        /// <summary>
        /// 获取选项
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static RichTextStyle GetOption(string text)
        {
            var option = new RichTextStyle();
            if (TryGetColor(text, out Color color))
            {
                option.colorFlag = true;
                option.color = color;
            }
            if (TryGetSize(text, out int size))
            {
                option.sizeFlag = true;
                option.size = size;
            }
            option.bold = HasBold(text);
            option.italic = HasItalic(text);
            return option;
        }
    }
}
