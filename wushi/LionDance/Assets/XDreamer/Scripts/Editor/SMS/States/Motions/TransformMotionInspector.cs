using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 变换动作检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //[CustomEditor(typeof(TransformMotion<>), true)]
    public class TransformMotionInspector<T> : MotionInspector<T> where T : TransformMotion<T>
    {
    }
}
