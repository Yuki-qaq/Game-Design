using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Rendering;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 处理工具
    /// </summary>
    [LinkType(typeof(HandleUtility))]
    public class HandleUtility_LinkType : LinkType<HandleUtility_LinkType>
    {
        #region ApplyWireMaterial

        /// <summary>
        /// 应用线条材质 方法信息
        /// </summary>
        public static XMethodInfo ApplyWireMaterial_MethodInfo { get; } = new XMethodInfo(Type, nameof(ApplyWireMaterial), new Type[] { }, TypeHelper.StaticNotPublic);

        /// <summary>
        /// 应用线条材质
        /// </summary>
        public static void ApplyWireMaterial()
        {
            ApplyWireMaterial_MethodInfo.Invoke(null, null);
        }

        #endregion
    }
}
