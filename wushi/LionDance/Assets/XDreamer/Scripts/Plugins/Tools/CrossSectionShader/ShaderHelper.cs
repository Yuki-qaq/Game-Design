using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.PluginTools.CrossSectionShader
{
    /// <summary>
    /// 着色器组手
    /// </summary>
    public class ShaderHelper
    {
        /// <summary>
        /// 通用三平面BSP
        /// </summary>
        public class GenericThreePlanesBSP
        {
            /// <summary>
            /// 默认着色器名称
            /// </summary>
            public const string DefaultShaderName = Product.Name + "/CrossSection/GenericThreePlanesBSP";

            /// <summary>
            /// 面1法向量
            /// </summary>
            public const string _Plane1Normal = nameof(_Plane1Normal);

            /// <summary>
            /// 面1位置
            /// </summary>
            public const string _Plane1Position = nameof(_Plane1Position);

            /// <summary>
            /// 面2法向量
            /// </summary>
            public const string _Plane2Normal = nameof(_Plane2Normal);

            /// <summary>
            /// 面2位置
            /// </summary>
            public const string _Plane2Position = nameof(_Plane2Position);

            /// <summary>
            /// 面3法向量
            /// </summary>
            public const string _Plane3Normal = nameof(_Plane3Normal);

            /// <summary>
            /// 面3位置
            /// </summary>
            public const string _Plane3Position = nameof(_Plane3Position);

            /// <summary>
            /// 有效：检查材质是否有对应的Shader属性
            /// </summary>
            /// <param name="material"></param>
            /// <returns></returns>
            public static bool Valid(Material material)
            {
                if (!material) return false;

                return material.HasProperty(_Plane1Normal) && material.HasProperty(_Plane1Position)
                    && material.HasProperty(_Plane2Normal) && material.HasProperty(_Plane2Position)
                    && material.HasProperty(_Plane3Normal) && material.HasProperty(_Plane3Position);
            }
        }
    }
}
