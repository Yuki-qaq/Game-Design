using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 预制体工具关联类型
    /// </summary>
    [LinkType(typeof(PrefabUtility))]
    public class PrefabUtility_LinkType : LinkType<PrefabUtility_LinkType>
    {
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static PrefabUtility_LinkType()
        {
            AddDelegate();
        }

        #region 保存预制体

        private static Delegate onSaveingPrefabDelegate;
        private static bool isAddDelegate = false;

        /// <summary>
        /// 添加委托
        /// </summary>
        public static void AddDelegate()
        {
            if (isAddDelegate) return;

            var eventInfo = savingPrefab_XEventInfo.memberInfo;
            if (eventInfo == null) return;

            var methodInfo = typeof(PrefabUtility_LinkType).GetMethod(nameof(OnSavingPrefab));
            if (methodInfo == null) return;

            onSaveingPrefabDelegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, methodInfo);
            if (onSaveingPrefabDelegate == null) return;

            eventInfo.GetAddMethod(true)?.Invoke(null, new object[] { onSaveingPrefabDelegate });
            isAddDelegate = true;
        }

        /// <summary>
        /// 移除委托
        /// </summary>
        private static void RemoveDelegate()
        {
            if (!isAddDelegate) return;
            isAddDelegate = false;

            var eventInfo = savingPrefab_XEventInfo.memberInfo;
            if (eventInfo != null && onSaveingPrefabDelegate != null)
            {
                eventInfo.GetRemoveMethod(true)?.Invoke(null, new object[] { onSaveingPrefabDelegate });
            }
        }

        /// <summary>
        /// 保存预制体 事件信息
        /// </summary>
        public static XEventInfo savingPrefab_XEventInfo { get; } = GetXEventInfo("savingPrefab");

        /// <summary>
        /// 当保存预制体
        /// </summary>
        public static event Action<GameObject, string> onSaveingPrefab;

        /// <summary>
        /// 当保存预制体
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="path"></param>
        public static void OnSavingPrefab(GameObject gameObject, string path) => onSaveingPrefab?.Invoke(gameObject, path);

        #endregion
    }
}
