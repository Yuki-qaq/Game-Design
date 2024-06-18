using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor.Web
{
    /// <summary>
    /// JS代理管理器
    /// </summary>
    [LinkType("UnityEditor.Web.JSProxyMgr")]
    public class JSProxyMgr : LinkType<JSProxyMgr>
    {
        /// <summary>
        /// 够着
        /// </summary>
        /// <param name="obj"></param>
        public JSProxyMgr(object obj) : base(obj) { }

        #region GetInstance

        /// <summary>
        /// 获取实例 方法信息
        /// </summary>
        public static XMethodInfo GetInstance_MethodInfo { get; } = GetXMethodInfo(nameof(GetInstance), TypeHelper.StaticPublic);

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static JSProxyMgr GetInstance()
        {
            return new JSProxyMgr(GetInstance_MethodInfo.Invoke(null, null));
        }

        #endregion

        #region AddGlobalObject

        /// <summary>
        /// 添加全局对象 方法信息
        /// </summary>
        public static XMethodInfo AddGlobalObject_MethodInfo { get; } = GetXMethodInfo(nameof(AddGlobalObject), TypeHelper.InstancePublic);

        /// <summary>
        /// 添加全局对象
        /// </summary>
        /// <param name="referenceName"></param>
        /// <param name="obj"></param>
        public void AddGlobalObject(string referenceName, object obj)
        {
            AddGlobalObject_MethodInfo.Invoke(this.obj, new object[] { referenceName, obj });
        }

        #endregion

        #region RemoveGlobalObject

        /// <summary>
        /// 移除全局对象 方法信息
        /// </summary>
        public static XMethodInfo RemoveGlobalObject_MethodInfo { get; } = GetXMethodInfo(nameof(RemoveGlobalObject), TypeHelper.InstancePublic);

        /// <summary>
        /// 移除全局对象
        /// </summary>
        /// <param name="referenceName"></param>
        public void RemoveGlobalObject(string referenceName)
        {
            RemoveGlobalObject_MethodInfo.Invoke(this.obj, new object[] { referenceName });
        }

        #endregion
    }
}
