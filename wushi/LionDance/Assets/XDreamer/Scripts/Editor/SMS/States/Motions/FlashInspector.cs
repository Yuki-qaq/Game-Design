using System.Text;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 闪烁检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FlashInspector<T> : MotionInspector<T> where T : Motion<T>, IFlash
    {
        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            info.AppendFormat("\n闪烁次数:\t{0}", stateComponent.flashCount);
            return info;
        }
    }
}
