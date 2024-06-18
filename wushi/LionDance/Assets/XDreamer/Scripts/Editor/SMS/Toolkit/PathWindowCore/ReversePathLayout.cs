using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.Toolkit.PathWindowCore
{
    /// <summary>
    /// 正弦曲线布局
    /// </summary>
    public class ReversePathLayout //: IPathLayout
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; } = "正弦曲线";

        /// <summary>
        /// 布局
        /// </summary>
        /// <param name="path"></param>
        public void Layout(PathInfo path)
        {
            //path.SetToLastPoint();
            //path.offsetValues.Reverse();
        }
    }
}
