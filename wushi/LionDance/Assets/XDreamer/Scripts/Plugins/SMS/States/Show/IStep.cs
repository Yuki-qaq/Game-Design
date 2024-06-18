using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 步骤
    /// </summary>
    [Name("步骤")]
    [XCSJ.Attributes.Icon(EIcon.Step)]
    public interface IStep : ITreeNodeGraphExtension
    {
        /// <summary>
        /// 父级
        /// </summary>
        new IStep parent { get; }

        /// <summary>
        /// 子级
        /// </summary>
        new IStep[] children { get; }

        /// <summary>
        /// 步骤状态
        /// </summary>
        EStepState stepState { get; }
    }

    /// <summary>
    /// 步骤状态
    /// </summary>
    [Name("步骤状态")]
    public enum EStepState
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 未完成
        /// </summary>
        [Name("未完成")]
        Unfinished,

        /// <summary>
        /// 活跃
        /// </summary>
        [Name("活跃")]
        Active,

        /// <summary>
        /// 已完成
        /// </summary>
        [Name("已完成")]
        Finished
    }
}
