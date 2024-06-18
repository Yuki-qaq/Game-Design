using System.Collections.Generic;
using System.Text;

namespace XCSJ.Extension.Base.CodeCreaters
{
    /// <summary>
    /// 代码写入器
    /// </summary>
    public class CodeWirter : ICodeWirter
    {
        /// <summary>
        /// 代码行列表
        /// </summary>
        private List<CodeLine> _codeLines = new List<CodeLine>();

        /// <summary>
        /// 代码行列表
        /// </summary>
        public List<CodeLine> codeLines => _codeLines;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _indent = 0;
            _codeLines.Clear();
        }

        #region 缩进量

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
        /// 重置缩进
        /// </summary>
        public void ResetIndent() => indent = 0;

        /// <summary>
        /// 增加缩进
        /// </summary>
        /// <param name="increaseValue"></param>
        public void IncreaseIndent(int increaseValue = 1) => indent += increaseValue;

        /// <summary>
        /// 减少缩进
        /// </summary>
        /// <param name="decreaseValue"></param>
        public void DecreaseIndent(int decreaseValue = 1) => indent -= decreaseValue;

        /// <summary>
        /// 获取缩进
        /// </summary>
        /// <returns></returns>
        public int GetIndent() => indent;

        /// <summary>
        /// 获取缩进量
        /// </summary>
        /// <returns></returns>
        public string GetIndentText() => GetIndentText(indent);

        /// <summary>
        /// 获取缩进字符串
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        public string GetIndentText(int indent) => new string(' ', indent > 0 ? indent * 4 : 0);

        #endregion

        #region 写入

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text = "") => Write(indent, text);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="text"></param>
        public void Write(int indent, string text) => _codeLines.Add(CodeLine.Create(indent, text));

        /// <summary>
        /// 写入格式化
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteFormat(string format, params object[] args) => WriteFormat(indent, format, args);

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteFormat(int indent, string format, params object[] args) => _codeLines.Add(CodeLine.CreateFormat(indent, format, args));

        #endregion

        #region 输出

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="prefixIndent">前缀缩进量</param>
        /// <returns></returns>
        public StringBuilder Output(StringBuilder stringBuilder, int prefixIndent = 0)
        {
            if (stringBuilder == null) return default;
            foreach (var line in _codeLines)
            {
                stringBuilder.AppendLine(line.Output(prefixIndent));
            }
            return stringBuilder;
        }

        private StringBuilder stringBuilder = new StringBuilder();

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="prefixIndent">前缀缩进量</param>
        /// <returns></returns>
        public string Output(int prefixIndent = 0)
        {
            stringBuilder.Clear();
            return Output(stringBuilder, prefixIndent).ToString();
        }

        #endregion
    }

    /// <summary>
    /// 代码写入器接口
    /// </summary>
    public interface ICodeWirter
    {
        /// <summary>
        /// 代码行列表
        /// </summary>
        List<CodeLine> codeLines { get; }

        /// <summary>
        /// 重置
        /// </summary>
        void Reset();

        #region 缩进量

        /// <summary>
        /// 获取缩进量
        /// </summary>
        /// <returns></returns>
        int GetIndent();

        /// <summary>
        /// 增加缩进
        /// </summary>
        void IncreaseIndent(int increaseValue = 1);

        /// <summary>
        /// 减少缩进
        /// </summary>
        void DecreaseIndent(int decreaseValue = 1);

        #endregion

        #region 写入

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="text"></param>
        void Write(string text = "");

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="text"></param>
        void Write(int indent, string text = "");

        /// <summary>
        /// 写入格式化
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void WriteFormat(string format, params object[] args);

        /// <summary>
        /// 写入格式化
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void WriteFormat(int indent, string format, params object[] args);

        #endregion

        #region 输出

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="prefixIndent"></param>
        /// <returns></returns>
        StringBuilder Output(StringBuilder stringBuilder, int prefixIndent = 0);

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="prefixIndent">前缀缩进量</param>
        /// <returns></returns>
        string Output(int prefixIndent = 0);

        #endregion
    }

    /// <summary>
    /// 代码写入器辅助类
    /// </summary>
    public static class CodeWirterHelper
    {
        /// <summary>
        /// 写入摘要，即代码注释
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ICodeWirter WriteSummary(this ICodeWirter codeWirter, string text = "")
        {
            if (codeWirter == null) return default;
            codeWirter.Write("/// <summary>");
            codeWirter.Write("/// " + text);
            codeWirter.Write("/// </summary>");
            return codeWirter;
        }

        /// <summary>
        /// 写入摘要，即代码注释
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ICodeWirter WriteSummaryFormat(this ICodeWirter codeWirter, string format, params object[] args)
        {
            if (codeWirter == null) return default;
            codeWirter.Write("/// <summary>");
            codeWirter.WriteFormat("/// " + format, args);
            codeWirter.Write("/// </summary>");
            return codeWirter;
        }

        /// <summary>
        /// 写入摘要参数
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ICodeWirter WriteSummaryParam(this ICodeWirter codeWirter, string name, string text = "")
        {
            if (codeWirter == null) return default;
            codeWirter.WriteFormat("/// <param name=\"{0}\">{1}</param>", name, text);
            return codeWirter;
        }

        /// <summary>
        /// 写入摘要返回值
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ICodeWirter WriteSummaryReturns(this ICodeWirter codeWirter, string text = "")
        {
            if (codeWirter == null) return default;
            codeWirter.WriteFormat("/// <returns>{0}</returns>", text);
            return codeWirter;
        }

        /// <summary>
        /// 写入宏#define
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <param name="macro"></param>
        /// <returns></returns>
        public static ICodeWirter WriteMacro_define(this ICodeWirter codeWirter, string macro)
        {
            if (codeWirter == null || string.IsNullOrEmpty(macro)) return codeWirter;
            codeWirter.Write(0, "#define " + macro);
            return codeWirter;
        }

        /// <summary>
        /// 写入宏#if
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <param name="macro"></param>
        /// <returns></returns>
        public static ICodeWirter WriteMacro_if(this ICodeWirter codeWirter, string macro)
        {
            if (codeWirter == null) return default;
            codeWirter.Write(0, "#if " + macro);
            return codeWirter;
        }

        /// <summary>
        /// 写入宏#elif
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <param name="macro"></param>
        /// <returns></returns>
        public static ICodeWirter WriteMacro_elif(this ICodeWirter codeWirter, string macro)
        {
            if (codeWirter == null) return default;
            codeWirter.Write(0, "#elif " + macro);
            return codeWirter;
        }

        /// <summary>
        /// 写入宏#else
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <returns></returns>
        public static ICodeWirter WriteMacro_else(this ICodeWirter codeWirter)
        {
            if (codeWirter == null) return default;
            codeWirter.Write(0, "#else");
            return codeWirter;
        }

        /// <summary>
        /// 写入宏#endif
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <returns></returns>
        public static ICodeWirter WriteMacro_endif(this ICodeWirter codeWirter)
        {
            if (codeWirter == null) return default;
            codeWirter.Write(0, "#endif");
            return codeWirter;
        }

        /// <summary>
        /// 写入#region
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ICodeWirter WriteRegion(this ICodeWirter codeWirter, string text)
        {
            if (codeWirter == null) return default;
            codeWirter.Write("#region " + text);
            return codeWirter;
        }

        /// <summary>
        /// 写入#region
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <returns></returns>
        public static ICodeWirter WriteEndRegion(this ICodeWirter codeWirter)
        {
            if (codeWirter == null) return default;
            codeWirter.Write("#endregion");
            return codeWirter;
        }
    }
}
