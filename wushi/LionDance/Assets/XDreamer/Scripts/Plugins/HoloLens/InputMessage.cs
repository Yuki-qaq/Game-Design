using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;

namespace XCSJ.PluginHoloLens
{
    /// <summary>
    /// 聚焦
    /// </summary>
    [Name("聚焦")]
    public enum EFocus
    {
        /// <summary>
        /// 进入
        /// </summary>
        [Name("进入")]
        Enter,

        /// <summary>
        /// 退出
        /// </summary>
        [Name("退出")]
        Exit,
    }

    /// <summary>
    /// 点击
    /// </summary>
    [Name("点击")]
    public enum EClick
    {
        /// <summary>
        /// 按下
        /// </summary>
        [Name("按下")]
        Down,

        /// <summary>
        /// 弹起
        /// </summary>
        [Name("弹起")]
        Up,

        /// <summary>
        /// 点击
        /// </summary>
        [Name("点击")]
        Click,
    }
}
