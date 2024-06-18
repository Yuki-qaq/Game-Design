using System.Collections.Generic;
using UnityEngine;
using XCSJ.Interfaces;

namespace XCSJ.EditorTools.Windows.Layouts
{
    /// <summary>
    /// 布局窗口
    /// </summary>
    public interface ILayoutWindow : IExpanded
    {

    }

    /// <summary>
    /// 布局窗口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILayoutWindow<T> : ILayoutWindow
    {
        /// <summary>
        /// 当绘制GUI
        /// </summary>
        /// <param name="list"></param>
        /// <param name="standards"></param>
        /// <returns></returns>
        bool OnGUI(List<T> list, params T[] standards);
    }

    /// <summary>
    /// 变换布局窗口
    /// </summary>
    public interface ITransformLayoutWindow : ILayoutWindow<Transform>
    {
    }

    /// <summary>
    /// 矩形变换布局窗口
    /// </summary>
    public interface IRectTransformLayoutWindow : ILayoutWindow<RectTransform>
    {

    }
}
