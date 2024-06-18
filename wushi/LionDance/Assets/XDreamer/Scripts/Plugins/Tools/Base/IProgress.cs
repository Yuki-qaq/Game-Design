namespace XCSJ.PluginTools.Base
{
    /// <summary>
    /// 进度
    /// </summary>
    public interface IProgress
    {
        /// <summary>
        /// 进度标题
        /// </summary>
        string progressTitle { get; }

        /// <summary>
        /// 进度值
        /// </summary>
        float progressValue { get; }
    }
}
