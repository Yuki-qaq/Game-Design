using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Interfaces;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.Toolkit.PathWindowCore
{
    /// <summary>
    /// 路径布局接口
    /// </summary>
    public interface IPathLayout : IName
    {
        /// <summary>
        /// 当绘制GUI
        /// </summary>
        /// <param name="path"></param>
        void OnGUI(PathInfo path);

        /// <summary>
        /// 布局
        /// </summary>
        /// <param name="path"></param>
        void Layout(PathInfo path);
    }
}

