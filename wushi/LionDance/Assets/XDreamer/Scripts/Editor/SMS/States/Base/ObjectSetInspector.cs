using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorSMS.States.Base
{
    /// <summary>
    /// 对象集检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectSetInspector<T> : StateComponentInspector<T> where T : StateComponent, IObjectSet
    {
        internal const string ObjectsString = "_" + nameof(IObjectSet.objects);

        /// <summary>
        /// 当绘制辅助信息
        /// </summary>
        public override void OnDrawHelpInfo()
        {
            //base.OnDrawHelpInfo();
        }
    }
}
