using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine.XEvents
{
    /// <summary>
    /// Unity事件基础
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnityEventBase_LinkType<T> : LinkType<T>
        where T : UnityEventBase_LinkType<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEventBase_LinkType(UnityEventBase obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEventBase_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected UnityEventBase_LinkType() { }

        #region AddCall

        /// <summary>
        /// 添加调用 方法信息
        /// </summary>
        public static XMethodInfo AddCall_MethodInfo { get; } = GetXMethodInfo(nameof(AddCall));

        /// <summary>
        /// 添加调用
        /// </summary>
        /// <param name="call"></param>
        public void AddCall(BaseInvokableCall call)
        {
            AddCall_MethodInfo?.Invoke(obj, new object[] { call.obj });
        }

        #endregion

        #region RemoveListener

        /// <summary>
        /// 移除监听器 方法信息
        /// </summary>
        public static XMethodInfo RemoveListener_MethodInfo { get; } = GetXMethodInfo(nameof(RemoveListener), new Type[] { typeof(object), typeof(MethodInfo) });

        /// <summary>
        /// 移除监听器
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="method"></param>
        public void RemoveListener(object targetObj, MethodInfo method)
        {
            RemoveListener_MethodInfo?.Invoke(obj, new object[] { targetObj, method });
        }

        #endregion

        #region AddPersistentListener

        /// <summary>
        /// 添加持久侦听器 方法信息
        /// </summary>
        public static XMethodInfo AddPersistentListener_MethodInfo { get; } = GetXMethodInfo(nameof(AddPersistentListener));

        /// <summary>
        /// 添加持久侦听器
        /// </summary>
        public void AddPersistentListener()
        {
            AddPersistentListener_MethodInfo?.Invoke(obj, null);
        }

        #endregion

        #region AddVoidPersistentListener

        /// <summary>
        /// 添加空持久侦听器 方法信息
        /// </summary>
        public static XMethodInfo AddVoidPersistentListener_MethodInfo { get; } = GetXMethodInfo(nameof(AddVoidPersistentListener), new Type[] { typeof(UnityAction) });

        /// <summary>
        /// 添加空持久侦听器
        /// </summary>
        /// <param name="call"></param>
        public void AddVoidPersistentListener(UnityAction call)
        {
            AddVoidPersistentListener_MethodInfo?.Invoke(obj, new object[] { call });
        }

        #endregion

        #region AddIntPersistentListener

        /// <summary>
        /// 添加整型持久侦听器 方法信息
        /// </summary>
        public static XMethodInfo AddIntPersistentListener_MethodInfo { get; } = GetXMethodInfo(nameof(AddIntPersistentListener), new Type[] { typeof(UnityAction<int>), typeof(int) });

        /// <summary>
        /// 添加整型持久侦听器
        /// </summary>
        /// <param name="call"></param>
        /// <param name="argument"></param>
        public void AddIntPersistentListener(UnityAction<int> call, int argument)
        {
            AddIntPersistentListener_MethodInfo?.Invoke(obj, new object[] { call, argument });
        }

        #endregion

        #region AddFloatPersistentListener

        /// <summary>
        /// 添加浮点数持久侦听器 方法信息
        /// </summary>
        public static XMethodInfo AddFloatPersistentListener_MethodInfo { get; } = GetXMethodInfo(nameof(AddFloatPersistentListener), new Type[] { typeof(UnityAction<float>), typeof(float) });

        /// <summary>
        /// 添加浮点数持久侦听器
        /// </summary>
        /// <param name="call"></param>
        /// <param name="argument"></param>
        public void AddFloatPersistentListener(UnityAction<float> call, float argument)
        {
            AddFloatPersistentListener_MethodInfo?.Invoke(obj, new object[] { call, argument });
        }

        #endregion

        #region AddBoolPersistentListener

        /// <summary>
        /// 添加布尔持久侦听器 方法信息
        /// </summary>
        public static XMethodInfo AddBoolPersistentListener_MethodInfo { get; } = GetXMethodInfo(nameof(AddBoolPersistentListener), new Type[] { typeof(UnityAction<bool>), typeof(bool) });

        /// <summary>
        /// 添加布尔持久侦听器
        /// </summary>
        /// <param name="call"></param>
        /// <param name="argument"></param>
        public void AddBoolPersistentListener(UnityAction<bool> call, bool argument)
        {
            AddBoolPersistentListener_MethodInfo?.Invoke(obj, new object[] { call, argument });
        }

        #endregion

        #region AddStringPersistentListener

        /// <summary>
        /// 添加字符串持久侦听器 方法信息
        /// </summary>
        public static XMethodInfo AddStringPersistentListener_MethodInfo { get; } = GetXMethodInfo(nameof(AddStringPersistentListener), new Type[] { typeof(UnityAction<string>), typeof(string) });

        /// <summary>
        /// 添加字符串持久侦听器
        /// </summary>
        /// <param name="call"></param>
        /// <param name="argument"></param>
        public void AddStringPersistentListener(UnityAction<string> call, string argument)
        {
            AddStringPersistentListener_MethodInfo?.Invoke(obj, new object[] { call, argument });
        }

        #endregion
    }

    /// <summary>
    /// Unity事件基础关联类型
    /// </summary>
    [LinkType(typeof(UnityEventBase))]
    public class UnityEventBase_LinkType : UnityEventBase_LinkType<UnityEventBase_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEventBase_LinkType(UnityEventBase obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEventBase_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected UnityEventBase_LinkType() { }
    }

    /// <summary>
    /// Unity事件基础扩展
    /// </summary>
    public static class UnityEventBaseExtension
    {
        /// <summary>
        /// 添加调用
        /// </summary>
        /// <param name="unityEventBase"></param>
        /// <param name="call"></param>
        public static void AddCall(this UnityEventBase unityEventBase, UnityAction call)
        {
            if (unityEventBase == null || call == null) return;
            AddCall(unityEventBase, UnityEvent_LinkType.GetDelegate(call));
        }

        /// <summary>
        /// 添加调用
        /// </summary>
        /// <param name="unityEventBase"></param>
        /// <param name="call"></param>
        public static void AddCall(this UnityEventBase unityEventBase, BaseInvokableCall call)
        {
            if (unityEventBase == null || call == null) return;
            new UnityEventBase_LinkType(unityEventBase).AddCall(call);
        }

        /// <summary>
        /// 移除调用
        /// </summary>
        /// <param name="unityEventBase"></param>
        /// <param name="call"></param>
        public static void RemoveCall(this UnityEventBase unityEventBase, UnityAction call)
        {
            if (unityEventBase == null || call == null) return;
            RemoveCall(unityEventBase, call.Target, call.Method);
        }

        /// <summary>
        /// 移除调用
        /// </summary>
        /// <param name="unityEventBase"></param>
        /// <param name="targetObj"></param>
        /// <param name="method"></param>
        public static void RemoveCall(this UnityEventBase unityEventBase, object targetObj, MethodInfo method)
        {
            new UnityEventBase_LinkType(unityEventBase).RemoveListener(targetObj, method);
        }
    }
}
