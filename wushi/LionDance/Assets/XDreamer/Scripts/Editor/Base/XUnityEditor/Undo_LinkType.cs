using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 撤销关联类型
    /// </summary>
    [LinkType(typeof(Undo))]
    public class Undo_LinkType : LinkType<Undo_LinkType>
    {
        #region GetRecords

        /// <summary>
        /// 获取记录 方法信息
        /// </summary>
        public static XMethodInfo GetRecords_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetRecords), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 获取记录
        /// </summary>
        /// <param name="undoRecords"></param>
        /// <param name="redoRecords"></param>
        public static void GetRecords(List<string> undoRecords, List<string> redoRecords)
        {
            GetRecords_MethodInfo.Invoke(null, new object[] { undoRecords, redoRecords });
        }

        #endregion

        #region DestroyObjectUndoable

        /// <summary>
        /// 可撤销销毁对象 方法信息
        /// </summary>
        public static XMethodInfo DestroyObjectUndoable_MethodInfo { get; } = new XMethodInfo(Type, nameof(DestroyObjectUndoable), TypeHelper.StaticNotPublic);

        /// <summary>
        /// 可撤销销毁对象
        /// </summary>
        /// <param name="objectToUndo"></param>
        /// <param name="name"></param>
        public static void DestroyObjectUndoable(UnityEngine.Object objectToUndo, string name)
        {
            DestroyObjectUndoable_MethodInfo.Invoke(null, new object[] { objectToUndo, name });
        }

        #endregion
    }
}
