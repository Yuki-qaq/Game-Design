
namespace XCSJ.Extension.Base.CodeCreaters
{
    /// <summary>
    /// 代码行
    /// </summary>
    public class CodeLine
    {
        /// <summary>
        /// 缩进量
        /// </summary>
        protected int _indent = 0;

        /// <summary>
        /// 缩进量
        /// </summary>
        public int indent
        {
            get => _indent;
            set => _indent = value < 0 ? 0 : value;
        }

        /// <summary>
        /// 文本
        /// </summary>
        public string text { get; set; } = "";

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString() => OutputText(indent);

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="prefixIndent">前缀缩进量</param>
        /// <returns></returns>
        public string Output(int prefixIndent = 0) => OutputText(indent + prefixIndent);

        /// <summary>
        /// 输出文本
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        public string OutputText(int indent = 0) => GetCodeText(indent, text);

        /// <summary>
        /// 隐式转为字符串
        /// </summary>
        /// <param name="codeLine"></param>
        public static implicit operator string(CodeLine codeLine) => codeLine?.ToString() ?? "";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static CodeLine Create(int indent, string text) => new CodeLine() { indent = indent, text = text };

        /// <summary>
        /// 创建格式化
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static CodeLine CreateFormat(int indent, string format, params object[] args) => Create(indent, string.Format(format, args));

        private static string IndentText(int indent) => new string(' ', indent * 4);

        /// <summary>
        /// 获取代码缩进文本
        /// </summary>
        /// <param name="indent">缩进量</param>
        /// <returns></returns>
        public static string GetCodeIndentText(int indent) => indent > 0 ? IndentText(indent) : "";

        /// <summary>
        /// 获取代码文本
        /// </summary>
        /// <param name="prefixIndent">前缀缩进量</param>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public static string GetCodeText(int prefixIndent, string text) => prefixIndent > 0 ? CodeLine.IndentText(prefixIndent) + text : text;
    }
}
