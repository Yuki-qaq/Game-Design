using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Interfaces;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// Unity对象
    /// </summary>
    public interface IUnityEngine_Object : ILinkType_Name
    {
        /// <summary>
        /// Unity对象
        /// </summary>
        Object unityEngineObject { get; }

        /// <summary>
        /// 隐藏标志
        /// </summary>
        HideFlags hideFlags { get; set; }
    }

    /// <summary>
    /// Unity对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnityEngine_Object<T> : LinkType_Name<T>, IUnityEngine_Object
        where T : UnityEngine_Object<T>
    {
        /// <summary>
        /// Unity对象
        /// </summary>
        public Object unityEngineObject => obj as Object;

        /// <summary>
        /// 名称
        /// </summary>
        public override string name
        {
            get => unityEngineObject ? unityEngineObject.name : "";
            set
            {
                if (unityEngineObject)
                {
                    unityEngineObject.name = value;
                }
            }
        }

        /// <summary>
        /// 隐藏标志
        /// </summary>
        public HideFlags hideFlags
        {
            get => unityEngineObject ? unityEngineObject.hideFlags : HideFlags.None;
            set
            {
                if(unityEngineObject)
                {
                    unityEngineObject.hideFlags = value;
                }
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEngine_Object(Object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEngine_Object(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected UnityEngine_Object() { }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return unityEngineObject ? unityEngineObject.GetHashCode() : base.GetHashCode();
        }

        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            var _this = other as UnityEngine_Object<T>;
            if (_this == null && other != null && !(other is UnityEngine_Object<T>))
            {
                return false;
            }
            return Compare(this, _this);
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool Compare(UnityEngine_Object<T> x, UnityEngine_Object<T> y)
        {
            bool lflag = (object)x == null;
            bool rflag = (object)y == null;
            if (lflag && rflag) return true;
            if (lflag || rflag) return false;
            return x.unityEngineObject == y.unityEngineObject;
        }

        /// <summary>
        /// 隐式转布尔
        /// </summary>
        /// <param name="exists"></param>
        public static implicit operator bool(UnityEngine_Object<T> exists)
        {
            return !Compare(exists, null);
        }

        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(UnityEngine_Object<T> x, UnityEngine_Object<T> y)
        {
            return Compare(x, y);
        }

        /// <summary>
        /// 不等
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(UnityEngine_Object<T> x, UnityEngine_Object<T> y)
        {
            return !Compare(x, y);
        }
    }

    /// <summary>
    /// Unity对象
    /// </summary>
    [LinkType(typeof(Object))]
    public class UnityEngine_Object : UnityEngine_Object<UnityEngine_Object>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEngine_Object(Object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public UnityEngine_Object(object obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        protected UnityEngine_Object() { }
    }
}
