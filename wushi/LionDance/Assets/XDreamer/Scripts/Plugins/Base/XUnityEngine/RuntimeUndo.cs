using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// 运行时撤销
    /// </summary>
    [LinkType(PlguinsHelper.UnityEngine_Prefix + nameof(RuntimeUndo))]
    public class RuntimeUndo : LinkType<RuntimeUndo>
    {
        #region SetTransformParent

        /// <summary>
        /// 设置变换父级
        /// </summary>
        public static XMethodInfo SetTransformParent_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetTransformParent), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 设置变换父级
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="newParent"></param>
        /// <param name="name"></param>
        public static void SetTransformParent(Transform transform, Transform newParent, string name)
        {
            SetTransformParent_MethodInfo?.Invoke(null, new object[] { transform, newParent, name });
        }

        #endregion

        #region RecordObject

        /// <summary>
        /// 记录对象 方法信息
        /// </summary>
        public static XMethodInfo RecordObject_MethodInfo { get; } = new XMethodInfo(Type, nameof(RecordObject), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 记录对象
        /// </summary>
        /// <param name="objectToUndo"></param>
        /// <param name="name"></param>
        public static void RecordObject(UnityEngine.Object objectToUndo, string name)
        {
            RecordObject_MethodInfo?.Invoke(null, new object[] { objectToUndo, name });
        }

        #endregion

        #region RecordObjects

        /// <summary>
        /// 记录对象 方法信息
        /// </summary>
        public static XMethodInfo RecordObjects_MethodInfo { get; } = new XMethodInfo(Type, nameof(RecordObjects), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 记录对象
        /// </summary>
        /// <param name="objectsToUndo"></param>
        /// <param name="name"></param>
        public static void RecordObjects(UnityEngine.Object[] objectsToUndo, string name)
        {
            RecordObjects_MethodInfo?.Invoke(null, new object[] { objectsToUndo, name });
        }

        #endregion
    }
}
