using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// 相机关联类型
    /// </summary>
    [LinkType(typeof(Camera))]
    public class Camera_LinkType : Behaviour_LinkType<Camera_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Camera_LinkType(Camera obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public Camera_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected Camera_LinkType() { }

        #region Camera.ProjectionMatrixMode

        /// <summary>
        /// 投影矩阵模式
        /// </summary>
        public enum ProjectionMatrixMode
        {
            /// <summary>
            /// 明确的
            /// </summary>
            Explicit,

            /// <summary>
            /// 隐式的
            /// </summary>
            Implicit,

            /// <summary>
            /// 基于物理属性
            /// </summary>
            PhysicalPropertiesBased
        }

        #endregion

        #region projectionMatrixMode

        /// <summary>
        /// 投影矩阵模式的属性信息
        /// </summary>
        public static XPropertyInfo projectionMatrixMode_XPropertyInfo { get; } = new XPropertyInfo(Type, nameof(projectionMatrixMode), TypeHelper.InstanceNotPublicHierarchy);

        /// <summary>
        /// 投影矩阵模式
        /// </summary>
        public ProjectionMatrixMode projectionMatrixMode => (ProjectionMatrixMode)(int)projectionMatrixMode_XPropertyInfo.GetValue(obj);

        #endregion

        #region GetLocalSpaceAim

        /// <summary>
        /// 获取局部空间目标的方法信息
        /// </summary>
        public static XMethodInfo GetLocalSpaceAim_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetLocalSpaceAim), TypeHelper.InstanceNotPublicHierarchy);

        /// <summary>
        /// 获取局部空间目标
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLocalSpaceAim() => (Vector3)GetLocalSpaceAim_MethodInfo.Invoke(obj, null);

        #endregion

        #region GetFrustumPlaneSizeAt

        /// <summary>
        /// 获取水果平面尺寸在指定距离的方法信息
        /// </summary>
        public static XMethodInfo GetFrustumPlaneSizeAt_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetFrustumPlaneSizeAt), TypeHelper.InstanceNotPublicHierarchy);

        /// <summary>
        /// 获取水果平面尺寸在指定距离
        /// </summary>
        /// <returns></returns>
        public Vector2 GetFrustumPlaneSizeAt(float distance) => (Vector2)GetFrustumPlaneSizeAt_MethodInfo.Invoke(obj, new object[] { distance });

        #endregion


    }
}
