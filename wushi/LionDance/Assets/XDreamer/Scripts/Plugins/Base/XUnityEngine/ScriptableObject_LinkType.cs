using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// 可脚本化对象关联类型
    /// </summary>
    public interface IScriptableObject_LinkType : IUnityEngine_Object
    {
        /// <summary>
        /// 可脚本化对象
        /// </summary>
        ScriptableObject scriptableObject { get; }
    }

    /// <summary>
    /// 可脚本化对象关联类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScriptableObject_LinkType<T> : UnityEngine_Object<T>, IScriptableObject_LinkType
    where T : ScriptableObject_LinkType<T>
    {
        /// <summary>
        /// 可脚本化对象
        /// </summary>
        public ScriptableObject scriptableObject => obj as ScriptableObject;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public ScriptableObject_LinkType(ScriptableObject obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public ScriptableObject_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        public ScriptableObject_LinkType() { }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        public static T CreateInstance()
        {
            var obj = TypeHelper.CreateInstance<T>();
            if (((object)obj) == null) return default;
            obj.obj = ScriptableObject.CreateInstance(Type);
            return obj;
        }
    }

    /// <summary>
    /// 可脚本化对象关联类型
    /// </summary>
    [LinkType(typeof(ScriptableObject))]
    public class ScriptableObject_LinkType : ScriptableObject_LinkType<ScriptableObject_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public ScriptableObject_LinkType(ScriptableObject obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public ScriptableObject_LinkType(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        public ScriptableObject_LinkType() { }
    }
}
