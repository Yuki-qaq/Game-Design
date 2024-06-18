using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 动作检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //[CustomEditor(typeof(Motion<>), true)]
    public class MotionInspector<T> : WorkClipInspector<T> where T : Motion<T>
    {

    }
}
