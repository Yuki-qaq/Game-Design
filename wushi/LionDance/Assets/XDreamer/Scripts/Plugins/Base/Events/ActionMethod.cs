using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.Events
{
    /// <summary>
    /// Action方法辅助类
    /// </summary>
    public static class ActionMethodHelper
    {
        /// <summary>
        /// 是编辑器类型：仅在编辑器内生效的类型；通过识别类型所在程序集文件的名称中是否有Editor字样进行判断；
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEditorType(Type type)
        {
            if (type == null) return false;
            var name = Path.GetFileNameWithoutExtension(type.Assembly?.Location ?? "");
            return name.Contains("Editor");
        }

        /// <summary>
        /// 是运行时类型：即在运行时生效的类型；
        /// </summary>
        /// <returns></returns>
        public static bool IsRuntimeType(Type type) => type != null && !IsEditorType(type);

        /// <summary>
        /// 判断是否是Action方法类型的字段信息
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static bool IsActionMethodType(FieldInfo fieldInfo)
        {
            if (fieldInfo == null || ObsoleteAttributeCache.Exist(fieldInfo)) return false;
            try
            {
                return Cache.GetCacheValue(fieldInfo).valid;
            }
            catch (Exception ex)
            {
                ex.HandleException(nameof(IsActionMethodType));
                return false;
            }
        }

        class Cache : TIVCache<Cache, FieldInfo, CacheValue> { }

        class CacheValue : TIVCacheValue<CacheValue, FieldInfo>
        {
            public bool valid { get; private set; } = false;

            public Type actionType => key1.FieldType;

            public MethodInfo invokeMethodInfo { get; private set; }

            public Type[] typeArguments { get; private set; } = Empty<Type>.Array;

            public Type actionMethodGenericType { get; private set; }

            public Type actionMethodType { get; private set; }

            public override bool Init()
            {
                try
                {
                    if (IsEditorType(key1.ReflectedType ?? key1.DeclaringType)) return true;

                    var actionType = this.actionType;
                    if (typeof(Delegate).IsAssignableFrom(actionType))
                    {
                        invokeMethodInfo = GetActionInvokeMethodInfo(actionType);
                        if (invokeMethodInfo != null && invokeMethodInfo.ReturnType == typeof(void))
                        {
                            typeArguments = invokeMethodInfo.GetParameters().Cast(pi => pi.ParameterType).ToArray() ?? Empty<Type>.Array;
                            switch (typeArguments.Length)
                            {
                                case 0:
                                    {
                                        actionMethodType = actionMethodGenericType = GetActionMethodGenericType(0);
                                        valid = true;
                                        break;
                                    }
                                default:
                                    {
                                        if (ValidTypeArguments(typeArguments))
                                        {
                                            actionMethodGenericType = GetActionMethodGenericType(typeArguments.Length);
                                            actionMethodType = actionMethodGenericType.GetMakedGenericType(typeArguments) ?? actionMethodGenericType.MakeGenericType(typeArguments);
                                            //actionMethodType = GetOrMakeGenericType(key1, actionMethodGenericType, typeArguments) ?? actionMethodGenericType.MakeGenericType(typeArguments);

                                            if (actionMethodType != null)
                                            {
                                                valid = Delegate.CreateDelegate(actionType, TypeHelper.CreateInstance(actionMethodType), actionMethodType.GetMethod(nameof(ActionMethod.Invoked))) != null;
                                            }
                                        }
                                        break;
                                    }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.HandleException(key1.ToString());
                }
                return true;
            }
        }

        /// <summary>
        /// 有效的类型参数数组
        /// </summary>
        /// <param name="typeArguments"></param>
        /// <returns></returns>
        public static bool ValidTypeArguments(Type[] typeArguments)
        {
            if (typeArguments == null) return false;
            if (typeArguments.Length == 0) return true;
            if (typeArguments.Any(t =>
            {
                if (t.IsArrayOrList() || t.Name.Contains("`") || t.IsNotPublic) return true;
                return false;
            })) return false;
            return true;
        }

        /// <summary>
        /// 获取动作调用方法信息：获取与输入类型匹配的Action方法信息，即Action事件对应执行方法（Invoke）的方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetActionInvokeMethodInfo(Type type) => type?.GetMethod(nameof(Action.Invoke));

        /// <summary>
        /// 获取动作方法类型：获取与输入类型匹配的ActionMethodBase子类类型
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static Type GetActionMethodType(FieldInfo fieldInfo)
        {
            if (GetActionInvokeMethodInfo(fieldInfo?.FieldType) is MethodInfo methodInfo && methodInfo.ReturnType == typeof(void))
            {
                var types = methodInfo.GetParameters().Cast(pi => pi.ParameterType).ToArray();
                var length = types.Length;
                if (length == 0) return typeof(ActionMethod);
                if (GetActionMethodGenericType(length) is Type gType) return GetOrMakeGenericType(fieldInfo, gType, types);
            }
            return null;
        }

        /// <summary>
        /// 获取动作方法泛型类型
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Type GetActionMethodGenericType(int length)
        {
            switch (length)
            {
                case 0: return typeof(ActionMethod);
                case 1: return typeof(ActionMethod<>);
                case 2: return typeof(ActionMethod<,>);
                case 3: return typeof(ActionMethod<,,>);
                case 4: return typeof(ActionMethod<,,,>);
                case 5: return typeof(ActionMethod<,,,,>);
                case 6: return typeof(ActionMethod<,,,,,>);
                case 7: return typeof(ActionMethod<,,,,,,>);
                case 8: return typeof(ActionMethod<,,,,,,,>);
                default: return null;
            }
        }

        /// <summary>
        /// 获取动作方法委托：获取与输入类型匹配的ActionMethodBase子类中对应函数构建的委托；如果输入类型不符合规则时，报出异常；
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="actionMethod"></param>
        /// <returns></returns>
        public static Delegate GetActionMethodDelegate(FieldInfo fieldInfo, out ActionMethodBase actionMethod)
        {
            ;
            var t = GetActionMethodType(fieldInfo);
            if (t == null)
            {
                actionMethod = default;
                return null;
            }
            actionMethod = TypeHelper.CreateInstance(t) as ActionMethodBase;
            if (actionMethod == null)
            {
                return null;
            }
            return Delegate.CreateDelegate(fieldInfo.FieldType, actionMethod, t.GetMethod(nameof(ActionMethod.Invoked)));
        }

        /// <summary>
        /// 获取生成的泛型类型
        /// </summary>
        /// <param name="originGenericType"></param>
        /// <param name="typeArguments"></param>
        /// <returns></returns>
        public static Type GetMakedGenericType(this Type originGenericType, params Type[] typeArguments) => MakedGenericTypeAttribute.GetMakedGenericType(originGenericType, typeArguments);

        /// <summary>
        /// 获取或生成泛型类型
        /// </summary>>
        /// <param name="originFieldInfo"></param>
        /// <param name="originGenericType"></param>
        /// <param name="typeArguments"></param>
        /// <returns></returns>
        public static Type GetOrMakeGenericType(FieldInfo originFieldInfo, Type originGenericType, params Type[] typeArguments)
        {
            if (originGenericType == null) return default;
            if (GetMakedGenericType(originGenericType, typeArguments) is Type type) return type;
            return PlguinsHelper.MakeGenericType(originFieldInfo, originGenericType, typeArguments);
        }
    }

    #region 生成的泛型类型

    /// <summary>
    /// 生成的泛型类型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MakedGenericTypeAttribute : Attribute
    {
        /// <summary>
        /// 原始泛型类型
        /// </summary>
        public Type originGenericType { get; private set; }

        /// <summary>
        /// 类型参数数组
        /// </summary>
        public Type[] typeArguments { get; private set; } = Empty<Type>.Array;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="originGenericType"></param>
        /// <param name="typeArguments"></param>
        public MakedGenericTypeAttribute(Type originGenericType, params Type[] typeArguments)
        {
            this.originGenericType = originGenericType ?? throw new ArgumentNullException(nameof(originGenericType));
            this.typeArguments = typeArguments ?? Empty<Type>.Array;
        }

        class MakedType
        {
            public Type makedType;

            Type originGenericType;

            Type[] typeArguments;

            public MakedType(Type originGenericType, params Type[] typeArguments)
            {
                this.originGenericType = originGenericType;
                this.typeArguments = typeArguments;

                TypeHelper.Foreach(type =>
                {
                    if (AttributeCache<MakedGenericTypeAttribute>.Get(type) is MakedGenericTypeAttribute makedGenericTypeAttribute)
                    {
                        if (makedGenericTypeAttribute.originGenericType == originGenericType && ArrayMatch(makedGenericTypeAttribute.typeArguments, typeArguments))
                        {
                            makedType = type;
                            return false;
                        }
                    }
                    return true;
                });
            }

            static bool ArrayMatch(Type[] aTypes, Type[] bTypes)
            {
                if (aTypes.Length != bTypes.Length) return false;
                for (int i = 0; i < aTypes.Length; i++)
                {
                    if (aTypes[i] != bTypes[i]) return false;
                }
                return true;
            }

            public bool Match(Type[] typeArguments) => ArrayMatch(this.typeArguments, typeArguments);
        }

        class MakedTypes
        {
            public Type originGenericType;

            List<MakedType> makedTypes = new List<MakedType>();

            public Type GetMakedType(params Type[] typeArguments)
            {
                return makedTypes.FirstOrNew(makedType => makedType.Match(typeArguments), () =>
                {
                    var makedType = new MakedType(originGenericType, typeArguments);
                    makedTypes.Add(makedType);
                    return makedType;
                })?.makedType;
            }
        }

        static Dictionary<Type, MakedTypes> makedTypesCache = new Dictionary<Type, MakedTypes>();

        /// <summary>
        /// 获取生成的泛型类型
        /// </summary>
        /// <param name="originGenericType"></param>
        /// <param name="typeArguments"></param>
        /// <returns></returns>
        public static Type GetMakedGenericType(Type originGenericType, params Type[] typeArguments)
        {
            if (originGenericType == null) return default;

            if (!makedTypesCache.TryGetValue(originGenericType, out var makedTypes))
            {
                makedTypesCache[originGenericType] = makedTypes = new MakedTypes() { originGenericType = originGenericType };
            }
            return makedTypes.GetMakedType(typeArguments);
        }
    }

    #endregion

    #region 动作方法

    /// <summary>
    /// Action方法基础类
    /// </summary>
    public abstract class ActionMethodBase
    {
        /// <summary>
        /// 关联的Action事件回调时，本事件也执行回调
        /// </summary>
        public static event Action<ActionMethodBase, ITupleData> onEventInvoked;

        /// <summary>
        /// 内部调用
        /// </summary>
        /// <param name="tuple"></param>
        protected void InvokedInternal(ITupleData tuple)
        {
            onEventInvoked?.Invoke(this, tuple);
        }
    }

    /// <summary>
    /// Action方法类
    /// </summary>
    public class ActionMethod : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked() => InvokedInternal(EmptyTupleData.Default);
    }

    /// <summary>
    /// Action方法1泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class ActionMethod<T1> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        /// <param name="arg1"></param>
        public void Invoked(T1 arg1) => InvokedInternal(TupleData.Create(arg1));
    }

    /// <summary>
    /// Action方法2泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class ActionMethod<T1, T2> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2) => InvokedInternal(TupleData.Create(arg1, arg2));
    }

    /// <summary>
    /// Action方法3泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class ActionMethod<T1, T2, T3> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3) => InvokedInternal(TupleData.Create(arg1, arg2, arg3));
    }

    /// <summary>
    /// Action方法4泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public class ActionMethod<T1, T2, T3, T4> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => InvokedInternal(TupleData.Create(arg1, arg2, arg3, arg4));
    }

    /// <summary>
    /// Action方法5泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public class ActionMethod<T1, T2, T3, T4, T5> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => InvokedInternal(TupleData.Create(arg1, arg2, arg3, arg4, arg5));
    }

    /// <summary>
    /// Action方法6泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    public class ActionMethod<T1, T2, T3, T4, T5, T6> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => InvokedInternal(TupleData.Create(arg1, arg2, arg3, arg4, arg5, arg6));
    }

    /// <summary>
    /// Action方法7泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    public class ActionMethod<T1, T2, T3, T4, T5, T6, T7> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => InvokedInternal(TupleData.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7));
    }

    /// <summary>
    /// Action方法8泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    public class ActionMethod<T1, T2, T3, T4, T5, T6, T7, T8> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => InvokedInternal(TupleData.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));
    }

    #endregion
}
