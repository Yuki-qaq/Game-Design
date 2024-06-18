using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// GUI关联类型
    /// </summary>
    [LinkType(typeof(GUI))]
    public class GUI_LinkType : LinkType<GUI_LinkType>
    {
        #region tooltipRect

        /// <summary>
        /// 工具提示矩形 属性信息
        /// </summary>
        public static XPropertyInfo tooltipRect_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(tooltipRect), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 工具提示矩形
        /// </summary>
        public static Rect tooltipRect
        {
            get => (Rect)tooltipRect_PropertyInfo.GetValue(null);
            set => tooltipRect_PropertyInfo.SetValue(null, value);
        }

        #endregion
    }
}
