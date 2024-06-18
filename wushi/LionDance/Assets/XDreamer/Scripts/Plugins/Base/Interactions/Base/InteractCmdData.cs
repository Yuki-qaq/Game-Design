using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.PluginCNScripts.UGUI;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.Extension.Base.Interactions.Base
{
    #region 交互命令特性

    /// <summary>
    /// 命令类型
    /// </summary>
    public enum ECmdType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 输入
        /// </summary>
        [Name("输入")]
        In,

        /// <summary>
        /// 输出
        /// </summary>
        [Name("输出")]
        Out,

        /// <summary>
        /// 输入输出
        /// </summary>
        [Name("输入输出")]
        Both,
    }

    /// <summary>
    /// 交互命令特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InteractCmdAttribute : Attribute
    {
        /// <summary>
        /// 命令类型
        /// </summary>
        public ECmdType cmdType { get; private set; } = ECmdType.Both;

        /// <summary>
        /// 构造函数
        /// </summary>
        public InteractCmdAttribute() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdType"></param>
        public InteractCmdAttribute(ECmdType cmdType)
        {
            this.cmdType = cmdType;
        }

        /// <summary>
        /// 命令类型是否匹配
        /// </summary>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        public bool IsMatchCmdType(ECmdType cmdType)
        {
            switch (cmdType)
            {
                case ECmdType.In: return this.cmdType == ECmdType.In || this.cmdType == ECmdType.Both;
                case ECmdType.Out: return this.cmdType == ECmdType.Out || this.cmdType == ECmdType.Both;
                case ECmdType.Both: return this.cmdType == ECmdType.Both;
            }
            return false;
        }
    }

    #endregion

    #region 交互命令弹出特性

    /// <summary>
    /// 交互命令弹出特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InteractCmdPopupAttribute : PropertyAttribute { }

    #endregion

    #region 交互命令数据

    /// <summary>
    /// 方法参数
    /// </summary>
    public enum EMethodParams
    {
        /// <summary>
        /// 无
        /// </summary>
        None,

        /// <summary>
        /// 一个
        /// </summary>
        One,
    }

    /// <summary>
    /// 交互命令数据
    /// </summary>
    public class InteractCmdData
    {
        /// <summary>
        /// 方法参数
        /// </summary>
        public EMethodParams methodParams { get; private set; }

        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo methodInfo { get; private set; }

        /// <summary>
        /// 方法信息
        /// </summary>
        public InteractCmdAttribute interactCmdAttribtue { get; private set; }

        /// <summary>
        /// 命令
        /// </summary>
        public string cmd => methodInfo.Name;

        /// <summary>
        /// 友好命令
        /// </summary>
        public string friendlyCmd => CommonFun.Name(methodInfo);

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="interactCmdAttribtue"></param>
        private InteractCmdData(MethodInfo methodInfo, InteractCmdAttribute interactCmdAttribtue)
        {
            this.methodInfo = methodInfo;
            this.interactCmdAttribtue = interactCmdAttribtue;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static InteractCmdData Create(MethodInfo methodInfo)
        {
            if (!methodInfo.IsGenericMethod && !methodInfo.IsGenericMethodDefinition
                && AttributeCache<InteractCmdAttribute>.Get(methodInfo) is InteractCmdAttribute interactCmdAttribtue)
            {
                EMethodParams mode;
                var ps = methodInfo.GetParameters();
                switch (ps.Length)
                {
                    case 0:
                        {
                            mode = EMethodParams.None;
                            break;
                        }
                    case 1:
                        {
                            mode = EMethodParams.One;
                            break;
                        }
                    default: return default;
                }

                var data = new InteractCmdData(methodInfo, interactCmdAttribtue);
                data.methodParams = mode;
                return data;
            }
            return default;
        }

        /// <summary>
        /// 转布尔函数
        /// </summary>
        Func<object, bool> toBoolFunc;

        /// <summary>
        /// 转布尔
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ToBool(object result)
        {
            if (toBoolFunc != null) return toBoolFunc(result);

            //返回值类型
            var returnType = methodInfo.ReturnType;
            if (returnType == typeof(bool))
            {
                toBoolFunc = o => (bool)o;
                return (bool)result;
            }

            //缺省处理
            toBoolFunc = o => true;
            return true;
        }

        /// <summary>
        /// 调用函数
        /// </summary>
        Func<object, object, bool> invokeFunc;

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="param0"></param>
        /// <returns></returns>
        public bool TryInteract(object obj, object param0)
        {
            try
            {
                if (invokeFunc != null) return invokeFunc(obj, param0);

                switch (methodParams)
                {
                    case EMethodParams.None:
                        {
                            invokeFunc = (o, p0) => ToBool(methodInfo.Invoke(o, Empty<object>.Array));
                            break;
                        }
                    case EMethodParams.One:
                        {
                            var p0Type = methodInfo.GetParameters()[0].ParameterType;
                            invokeFunc = (o, p0) => Converter.instance.TryConvertTo(p0, p0Type, out var op0) && ToBool(methodInfo.Invoke(o, new object[] { op0 }));
                            break;
                        }
                    default:
                        {
                            invokeFunc = (o, p0) => false;
                            return false;
                        }
                }
                return invokeFunc(obj, param0);
            }
            catch (Exception ex)
            {
                ex.HandleException();
            }
            return false;
        }
    }

    #endregion

    #region 交互命令数据列表

    /// <summary>
    /// 交互命令数据列表
    /// </summary>
    public class InteractCmdDatas
    {
        /// <summary>
        /// 空交互命令数据列表
        /// </summary>
        public static InteractCmdDatas Empty { get; } = new InteractCmdDatas();

        /// <summary>
        /// 类型
        /// </summary>
        public Type type { get; private set; }

        /// <summary>
        /// 交互命令数据列表
        /// </summary>
        private List<InteractCmdData> _interactCmdDatas = new List<InteractCmdData>();

        /// <summary>
        /// 交互命令数据列表
        /// </summary>
        public List<InteractCmdData> interactCmdDatas => _interactCmdDatas;

        #region 命令

        /// <summary>
        /// 命令列表
        /// </summary>
        private List<string> _cmds;

        /// <summary>
        /// 命令列表
        /// </summary>
        public List<string> cmds => _cmds ?? (_cmds = _interactCmdDatas.ToList(d => d.cmd));

        /// <summary>
        /// 命令数组
        /// </summary>
        private string[] _cmdArray;

        /// <summary>
        /// 命令数组
        /// </summary>
        public string[] cmdArray => _cmdArray ?? (_cmdArray = _interactCmdDatas.Cast(d => d.cmd).ToArray());

        #endregion

        #region 友好命令

        /// <summary>
        /// 友好命令列表
        /// </summary>
        private List<string> _friendlyCmds;

        /// <summary>
        /// 友好命令列表
        /// </summary>
        public List<string> friendlyCmds => _friendlyCmds ?? (_friendlyCmds = _interactCmdDatas.ToList(d => d.friendlyCmd));

        /// <summary>
        /// 友好命令数组
        /// </summary>
        private string[] _friendlyCmdArray;

        /// <summary>
        /// 友好命令数组
        /// </summary>
        public string[] friendlyCmdArray => _friendlyCmdArray ?? (_friendlyCmdArray = _interactCmdDatas.Cast(d => d.friendlyCmd).ToArray());

        #endregion

        /// <summary>
        /// 命令字典
        /// </summary>
        private Dictionary<string, InteractCmdData> cmdDictionary = new Dictionary<string, InteractCmdData>();

        /// <summary>
        /// 友好命令字典
        /// </summary>
        private Dictionary<string, InteractCmdData> friendlyCmdDictionary = new Dictionary<string, InteractCmdData>();

        /// <summary>
        /// 尝试获取交互命令数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="interactCmdData"></param>
        /// <returns></returns>
        public bool TryGetInteractCmdData(string cmd, out InteractCmdData interactCmdData)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                interactCmdData = default;
                return false;
            }
            if (cmdDictionary.TryGetValue(cmd, out interactCmdData)) return true;

            interactCmdData = _interactCmdDatas.FirstOrDefault(d => d.cmd == cmd);
            cmdDictionary[cmd] = interactCmdData;

            return interactCmdData != null;
        }

        /// <summary>
        /// 通过友好命令尝试获取交互命令数据
        /// </summary>
        /// <param name="friendlyCmd"></param>
        /// <param name="interactCmdData"></param>
        /// <returns></returns>
        public bool TryGetInteractCmdDataByFriendlyCmd(string friendlyCmd, out InteractCmdData interactCmdData)
        {
            if (string.IsNullOrEmpty(friendlyCmd))
            {
                interactCmdData = default;
                return false;
            }
            if (friendlyCmdDictionary.TryGetValue(friendlyCmd, out interactCmdData)) return interactCmdData != null;

            interactCmdData = _interactCmdDatas.FirstOrDefault(d => d.friendlyCmd == friendlyCmd);
            friendlyCmdDictionary[friendlyCmd] = interactCmdData;

            return interactCmdData != null;
        }

        /// <summary>
        /// 构造
        /// </summary>
        private InteractCmdDatas() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        public InteractCmdDatas(Type type)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            foreach (var mi in type.GetMethods(TypeHelper.InstancePublicHierarchy))
            {
                if (InteractCmdData.Create(mi) is InteractCmdData interactCmdData)
                {
                    _interactCmdDatas.Add(interactCmdData);
                }
            }
        }

        /// <summary>
        /// 尝试获取友好命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="friendlyCmd"></param>
        /// <returns></returns>
        public bool TryGetFriendlyCmd(string cmd, out string friendlyCmd)
        {
            var index = cmds.IndexOf(cmd);
            if (index >= 0)
            {
                friendlyCmd = friendlyCmdArray[index];
                return true;
            }
            friendlyCmd = default;
            return false;
        }

        /// <summary>
        /// 尝试获取命令
        /// </summary>
        /// <param name="friendlyCmd"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool TryGetCmd(string friendlyCmd, out string cmd)
        {
            var index = friendlyCmds.IndexOf(friendlyCmd);
            if (index >= 0)
            {
                cmd = cmds[index];
                return true;
            }
            cmd = default;
            return false;
        }
    }

    #endregion

    #region 交互命令助手

    /// <summary>
    /// 交互命令助手
    /// </summary>
    public static class InteractCmdHelper
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private static Dictionary<Type, InteractCmdDatas> cache = new Dictionary<Type, InteractCmdDatas>();

        /// <summary>
        /// 获取交互命令数据列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static InteractCmdDatas InternalGetInteractCmdDatas(this Type type)
        {
            if (cache.TryGetValue(type, out var interactCmdDatas)) return interactCmdDatas;

            cache[type] = interactCmdDatas = new InteractCmdDatas(type);
            return interactCmdDatas;
        }

        /// <summary>
        /// 获取交互命令数据列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static InteractCmdDatas GetInteractCmdDatas(this Type type) => type != null ? InternalGetInteractCmdDatas(type) : InteractCmdDatas.Empty;

        /// <summary>
        /// 获取交互命令数据列表
        /// </summary>
        /// <param name="interactObject"></param>
        /// <returns></returns>
        public static InteractCmdDatas GetInteractCmdDatas(this InteractObject interactObject) => interactObject ? interactObject.GetType().InternalGetInteractCmdDatas() : InteractCmdDatas.Empty;
    }

    #endregion

    #region 交互命令方法特性

    /// <summary>
    /// 交互命令方法特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InteractCmdFunAttribute : Attribute
    {
        /// <summary>
        /// 命令
        /// </summary>
        public string cmd { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmd"></param>
        public InteractCmdFunAttribute(string cmd)
        {
            this.cmd = cmd;
        }
    }

    #endregion

    #region 交互命令方法数据

    /// <summary>
    /// 交互命令方法数据
    /// </summary>
    public class InteractCmdFunData
    {
        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo methodInfo { get; private set; }

        /// <summary>
        /// 交互命令方法特性
        /// </summary>
        public InteractCmdFunAttribute interactCmdFunAttribtue { get; private set; }

        /// <summary>
        /// 命令
        /// </summary>
        public string cmd => interactCmdFunAttribtue.cmd;

        private enum EReturnType
        {
            Void,
            Bool,
            EInteractResult
        }

        private EReturnType returnType = EReturnType.Void;

        private EMethodParams methodParams = EMethodParams.None;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="interactCmdFunAttribtue"></param>
        private InteractCmdFunData(MethodInfo methodInfo, InteractCmdFunAttribute interactCmdFunAttribtue)
        {
            this.methodInfo = methodInfo;
            this.interactCmdFunAttribtue = interactCmdFunAttribtue;
        }

        /// <summary>
        /// 创建: 当参数为空或<see cref="InteractData"/>且返回值为空或<see cref="EInteractResult"/>时为有效的交互命令函数
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static InteractCmdFunData Create(MethodInfo methodInfo)
        {
            if (!methodInfo.IsGenericMethod
                && !methodInfo.IsGenericMethodDefinition
                && AttributeCache<InteractCmdFunAttribute>.Get(methodInfo) is InteractCmdFunAttribute interactCmdFunAttribtue
                && !string.IsNullOrEmpty(interactCmdFunAttribtue.cmd))
            {
                var returnType = EReturnType.Void;
                if (methodInfo.ReturnType == typeof(void))
                {
                    returnType = EReturnType.Void;
                }
                else if (methodInfo.ReturnType == typeof(bool))
                {
                    returnType = EReturnType.Bool;
                }
                else if (methodInfo.ReturnType == typeof(EInteractResult))
                {
                    returnType = EReturnType.EInteractResult;
                }
                else
                {
                    return default;
                }

                var parameterInfos = methodInfo.GetParameters();
                var mode = EMethodParams.None;
                switch (parameterInfos.Length)
                {
                    case 0:
                        {
                            mode = EMethodParams.None;
                            break;
                        }
                    case 1:
                        {
                            if (typeof(InteractData).IsAssignableFrom(parameterInfos[0].ParameterType))
                            {
                                mode = EMethodParams.One;
                                break;
                            }
                            return default;
                        }
                    default: return default;
                }

                var data = new InteractCmdFunData(methodInfo, interactCmdFunAttribtue);
                data.returnType = returnType;
                data.methodParams = mode;
                return data;
            }
            return default;
        }

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public EInteractResult TryInteract(object obj, InteractData interactData)
        {
            try
            {
                if (obj != null)
                {
                    switch (returnType)
                    {
                        case EReturnType.Void:
                            {
                                switch (methodParams)
                                {
                                    case EMethodParams.None:
                                        {
                                            methodInfo.Invoke(obj, Empty<object>.Array);
                                            return EInteractResult.Success;
                                        }
                                    case EMethodParams.One:
                                        {
                                            methodInfo.Invoke(obj, new object[] { interactData });
                                            return EInteractResult.Success;
                                        }
                                }
                                break;
                            }
                        case EReturnType.Bool:
                            {
                                switch (methodParams)
                                {
                                    case EMethodParams.None: return (bool)methodInfo.Invoke(obj, Empty<object>.Array) ? EInteractResult.Success : EInteractResult.Fail;
                                    case EMethodParams.One: return (bool)methodInfo.Invoke(obj, new object[] { interactData }) ? EInteractResult.Success : EInteractResult.Fail;
                                }
                                break;
                            }
                        case EReturnType.EInteractResult:
                            {
                                switch (methodParams)
                                {
                                    case EMethodParams.None: return (EInteractResult)methodInfo.Invoke(obj, Empty<object>.Array);
                                    case EMethodParams.One: return (EInteractResult)methodInfo.Invoke(obj, new object[] { interactData });
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.HandleException();
            }
            return EInteractResult.Fail;
        }
    }

    #endregion

    #region 空交互命令方法数据列表

    /// <summary>
    /// 交互命令方法数据列表
    /// </summary>
    public class InteractCmdFunDatas
    {
        /// <summary>
        /// 空交互命令方法数据列表
        /// </summary>
        public static InteractCmdFunDatas Empty { get; } = new InteractCmdFunDatas();

        /// <summary>
        /// 类型
        /// </summary>
        public Type type { get; private set; }

        /// <summary>
        /// 交互命令方法数据列表
        /// </summary>
        private List<InteractCmdFunData> _interactCmdFunDatas = new List<InteractCmdFunData>();

        /// <summary>
        /// 交互命令方法数据列表
        /// </summary>
        public List<InteractCmdFunData> interactCmdDatas => _interactCmdFunDatas;

        /// <summary>
        /// 命令列表
        /// </summary>
        private List<string> _cmds;

        /// <summary>
        /// 命令列表
        /// </summary>
        public List<string> cmds => _cmds ?? (_cmds = _interactCmdFunDatas.ToList(d => d.cmd));

        /// <summary>
        /// 命令数组
        /// </summary>
        private string[] _cmdArray;

        /// <summary>
        /// 命令数组
        /// </summary>
        public string[] cmdArray => _cmdArray ?? (_cmdArray = _interactCmdFunDatas.Cast(d => d.cmd).ToArray());

        /// <summary>
        /// 命令字典
        /// </summary>
        private Dictionary<string, InteractCmdFunData> cmdDictionary = new Dictionary<string, InteractCmdFunData>();

        /// <summary>
        /// 尝试获取交互命令方法数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="interactCmdFunData"></param>
        /// <returns></returns>
        public bool TryGetInteractCmdFunData(string cmd, out InteractCmdFunData interactCmdFunData)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                interactCmdFunData = default;
                return false;
            }
            if (!cmdDictionary.TryGetValue(cmd, out interactCmdFunData))
            {
                interactCmdFunData = _interactCmdFunDatas.FirstOrDefault(d => d.cmd == cmd);
                cmdDictionary[cmd] = interactCmdFunData;
            }

            return interactCmdFunData != null;
        }

        /// <summary>
        /// 构造
        /// </summary>
        private InteractCmdFunDatas() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        public InteractCmdFunDatas(Type type)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            foreach (var mi in type.GetMethods(TypeHelper.InstancePublicHierarchy))
            {
                if (InteractCmdFunData.Create(mi) is InteractCmdFunData interactCmdFunData)
                {
                    _interactCmdFunDatas.Add(interactCmdFunData);
                }
            }
        }
    }

    #endregion

    #region 交互命令方法助手

    /// <summary>
    /// 交互命令方法助手
    /// </summary>
    public static class InteractCmdFunHelper
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private static Dictionary<Type, InteractCmdFunDatas> cache = new Dictionary<Type, InteractCmdFunDatas>();

        /// <summary>
        /// 获取交互命令方法数据列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static InteractCmdFunDatas InternalGetInteractCmdFunDatas(this Type type)
        {
            if (cache.TryGetValue(type, out var interactCmdFunDatas)) return interactCmdFunDatas;

            cache[type] = interactCmdFunDatas = new InteractCmdFunDatas(type);
            return interactCmdFunDatas;
        }

        /// <summary>
        /// 获取交互命令方法数据列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static InteractCmdFunDatas GetInteractCmdFunDatas(this Type type) => type != null ? InternalGetInteractCmdFunDatas(type) : InteractCmdFunDatas.Empty;

        /// <summary>
        /// 获取交互命令方法数据列表
        /// </summary>
        /// <param name="interactObject"></param>
        /// <returns></returns>
        public static InteractCmdFunDatas GetInteractCmdFunDatas(this InteractObject interactObject) => interactObject ? interactObject.GetType().InternalGetInteractCmdFunDatas() : InteractCmdFunDatas.Empty;
    }

    #endregion
}
