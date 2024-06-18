using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.CodeCreaters;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.Extension.Base.CodeCreaters
{
    /// <summary>
    /// 抽象代码生成器
    /// </summary>
    public abstract class AbstractCodeCreater
    {
        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            ClearUsedTypes();
            nameSpace = "";
            codeWirter?.Reset();
            targetObjectType = null;
        }

        #region 已知引用的类型

        /// <summary>
        /// 引用类型列表：代码文件中所有引用到的已知类型
        /// </summary>
        public HashSet<Type> usedTypes { get; } = new HashSet<Type>();

        /// <summary>
        /// 添加引用类型
        /// </summary>
        /// <param name="types"></param>
        public void AddUsedType(params Type[] types)
        {
            if (types != null)
            {
                types.Foreach(t =>
                {
                    if (t != null)
                    {
                        usedTypes.Add(t);
                    }
                });
            }
        }

        /// <summary>
        /// 清理引用类型列表
        /// </summary>
        public void ClearUsedTypes() => usedTypes.Clear();

        #endregion

        #region 引用的命名空间

        /// <summary>
        /// 引用的命名空间列表
        /// </summary>
        public List<string> usedNameSpaces
        {
            get
            {
                HashSet<string> names = new HashSet<string>();
                foreach (var t in usedTypes)
                {
                    if (t.IsNested && t.DeclaringType != null)
                    {
                        names.Add("static " + t.DeclaringType.FullName);
                    }
                    names.Add(t.Namespace);
                }
                var list = names.Distinct().ToList();
                list.Sort();
                return list;
            }
        }

        /// <summary>
        /// 当开始引用的命名空间
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnBeginUsedNameSpaces(ICodeWirter codeWirter)
        {
            var usedNameSpaces = this.usedNameSpaces;
            if (usedNameSpaces.Count > 0)
            {
                foreach (var ns in usedNameSpaces)
                {
                    codeWirter.WriteFormat("using {0};", ns);
                }
                codeWirter.Write();
            }
        }

        /// <summary>
        /// 当结束引用的命名空间
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnEndUsedNameSpaces(ICodeWirter codeWirter) { }

        #endregion

        #region 命名空间

        /// <summary>
        /// 命名空间
        /// </summary>
        public virtual string nameSpace { get; set; } = "";

        /// <summary>
        /// 当开始命名空间
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnBeginNameSpaces(ICodeWirter codeWirter)
        {
            if (string.IsNullOrEmpty(nameSpace)) return;

            codeWirter.WriteFormat("namespace {0}", nameSpace);
            codeWirter.Write("{");
            codeWirter.IncreaseIndent();
        }

        /// <summary>
        /// 当结束命名空间
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnEndNameSpaces(ICodeWirter codeWirter)
        {
            if (string.IsNullOrEmpty(nameSpace)) return;

            codeWirter.DecreaseIndent();
            codeWirter.Write("}");
        }

        /// <summary>
        /// 重置命名空间
        /// </summary>
        public virtual void ResetNameSpace()
        {
            nameSpace = "";
            if (targetObjectType != null && !string.IsNullOrEmpty(targetObjectType.Namespace))
            {
                nameSpace = targetObjectType.Namespace;
            }
        }

        #endregion

        #region 类型定义

        #region 类型权限

        /// <summary>
        /// 类型权限
        /// </summary>
        public virtual ETypeAccess typeAccess { get; set; } = ETypeAccess.Public;

        /// <summary>
        /// 类型权限字符串
        /// </summary>
        public string typeAccessString => AbbreviationAttribute.GetAbbreviation(typeAccess, "");

        /// <summary>
        /// 获取类型权限字符串格式化
        /// </summary>
        /// <returns></returns>
        public string GetTypeAccessStringFormat()
        {
            var accessString = this.typeAccessString;
            accessString = string.IsNullOrEmpty(accessString) ? "" : (accessString + " ");
            return accessString;
        }

        #endregion

        #region 类型种类

        /// <summary>
        /// 类型种类
        /// </summary>
        public virtual ETypeKind typeKind { get; } = ETypeKind.Class;

        /// <summary>
        /// 类型种类字符串
        /// </summary>
        public string typeKindString => AbbreviationAttribute.GetAbbreviation(typeKind, "");

        /// <summary>
        /// 获取类型种类字符串格式化
        /// </summary>
        /// <returns></returns>
        public string GetKindStringFormat() => typeKindString + " ";

        #endregion

        #region 名称

        /// <summary>
        /// 默认名称
        /// </summary>
        public virtual string defaultName { get; } = "";

        /// <summary>
        /// 名称
        /// </summary>
        public string _name = null;

        /// <summary>
        /// 名称，代码文件名、代码中类型名称
        /// </summary>
        public string name
        {
            get => string.IsNullOrEmpty(_name) ? defaultName : _name;
            set => _name = value;
        }

        /// <summary>
        /// 获取名称字符串格式化
        /// </summary>
        /// <returns></returns>
        public virtual string GetNameStringFormat() => name + " ";

        /// <summary>
        /// 获取创建类型；仅在已经创建类型并输出到文件编译之后菜有可能获取到有效创建类型；但是找到的不一定就是真实创建的类型；会返回第一个类型名与<see cref="name"/>相同的；
        /// </summary>
        /// <returns></returns>
        public virtual Type GetCreateType() => TypeHelper.GetType(name, false);

        /// <summary>
        /// 获取创建类型列表；仅在已经创建类型并输出到文件编译之后菜有可能获取到有效创建类型；会返回所有类型名与<see cref="name"/>相同的；
        /// </summary>
        /// <returns></returns>
        public virtual List<Type> GetCreateTypes() => TypeHelper.GetTypes(t => t.Name == name);

        /// <summary>
        /// 通过创建类型初始化本类
        /// </summary>
        /// <param name="createType"></param>
        public virtual void InitByCreateType(Type createType)
        {
            if (createType == null) return;
            nameSpace = createType.Namespace;
        }

        #endregion

        #region 基类

        /// <summary>
        /// 基础类型定义字符串:包括集成的基类、接口等
        /// </summary>
        /// <returns></returns>
        protected virtual string baseTypeDefineString { get; } = "";

        /// <summary>
        /// 获取基础类型定义字符串格式化
        /// </summary>
        /// <returns></returns>
        protected virtual string GetBaseTypeDefineStringFormat()
        {
            var baseTypeDefineString = this.baseTypeDefineString;
            var tmp = baseTypeDefineString.Trim();
            if (!string.IsNullOrEmpty(tmp))
            {
                if (tmp.StartsWith(":"))
                {
                    baseTypeDefineString = tmp;
                }
                else
                {
                    baseTypeDefineString = ": " + tmp;
                }
            }
            else
            {
                baseTypeDefineString = "";
            }
            return baseTypeDefineString;
        }


        #endregion

        /// <summary>
        /// 当开始类型定义
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnBeginTypeDefine(ICodeWirter codeWirter)
        {
            codeWirter.WriteFormat("{0}{1}{2}{3}", GetTypeAccessStringFormat(), GetKindStringFormat(), GetNameStringFormat(), GetBaseTypeDefineStringFormat());
            codeWirter.Write("{");
            codeWirter.IncreaseIndent();
        }

        /// <summary>
        /// 当结束类型定义
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnEndTypeDefine(ICodeWirter codeWirter)
        {
            codeWirter.DecreaseIndent();
            codeWirter.Write("}");
        }

        /// <summary>
        /// 当生成类型内容：当生成新类型的内部逻辑代码时调用
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnCreateTypeContent(ICodeWirter codeWirter) { }

        #endregion

        #region 代码生成与输出

        /// <summary>
        /// 输出引用的命名空间
        /// </summary>
        public bool outputUsedNameSpaces { get; set; } = true;

        /// <summary>
        /// 输出命名空间
        /// </summary>
        public bool outputNameSpace { get; set; } = true;

        /// <summary>
        /// 代码写入器
        /// </summary>
        public ICodeWirter codeWirter { get; set; } = new CodeWirter();

        /// <summary>
        /// 输出代码
        /// </summary>
        /// <param name="prefixIndent">前缀缩进量</param>
        /// <returns></returns>
        public string OutputCode(int prefixIndent = 0) => CreateCode()?.Output(prefixIndent) ?? "";

        /// <summary>
        /// 输出代码
        /// </summary>
        /// <param name="codeWirter">代码写入器</param>
        /// <param name="prefixIndent">前缀缩进量</param>
        /// <returns></returns>
        public string OutputCode(ICodeWirter codeWirter, int prefixIndent = 0) => CreateCode(codeWirter)?.Output(prefixIndent) ?? "";

        /// <summary>
        /// 生成代码：将代码生成到代码写入器
        /// </summary>
        /// <param name="reuseCodeWirter">重用代码写入器</param>
        /// <returns></returns>
        public ICodeWirter CreateCode(bool reuseCodeWirter = true)
        {
            if (reuseCodeWirter)
            {
                if (codeWirter == null) codeWirter = new CodeWirter();
                codeWirter.Reset();
                return CreateCode(codeWirter);
            }
            return CreateCode(new CodeWirter());
        }

        /// <summary>
        /// 生成代码：将代码生成到代码写入器
        /// </summary>
        /// <param name="codeWirter"></param>
        public ICodeWirter CreateCode(ICodeWirter codeWirter)
        {
            if (codeWirter == null) return default;

            OnBeforeBeginCreateCode(codeWirter);
            OnBeginCreateCode(codeWirter);
            {
                if (outputUsedNameSpaces)
                {
                    OnBeginUsedNameSpaces(codeWirter);
                    OnEndUsedNameSpaces(codeWirter);
                }

                if (outputNameSpace) OnBeginNameSpaces(codeWirter);
                {
                    OnBeginTypeDefine(codeWirter);

                    OnCreateTypeContent(codeWirter);

                    OnEndTypeDefine(codeWirter);
                }
                if (outputNameSpace) OnEndNameSpaces(codeWirter);
            }
            OnEndCreateCode(codeWirter);
            OnAfterEndCreateCode(codeWirter);

            return codeWirter;
        }

        /// <summary>
        /// 当开始生成代码之前调用
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnBeforeBeginCreateCode(ICodeWirter codeWirter) => ClearUsedTypes();

        /// <summary>
        /// 当开始生成代码
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnBeginCreateCode(ICodeWirter codeWirter) => AddUsedType(targetObjectType, GetTargetObjectTypePopupAttribute());

        /// <summary>
        /// 当结束生成代码
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnEndCreateCode(ICodeWirter codeWirter) { }

        /// <summary>
        /// 当结束生成代码之后调用
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnAfterEndCreateCode(ICodeWirter codeWirter) { }

        #endregion

        #region 目标对象类型

        /// <summary>
        /// 目标对象类型:代码生成器生成代码时依赖的核心类型；此类型可为无效；
        /// </summary>
        public virtual Type targetObjectType { get; set; }

        /// <summary>
        /// 重置目标对象类型信息
        /// </summary>
        /// <param name="targetObjectType"></param>
        public virtual void ResetTargetObjectTypeInfo(Type targetObjectType)
        {
            Reset();
            this.targetObjectType = targetObjectType;
            ResetNameSpace();
        }

        /// <summary>
        /// 重置目标对象类型信息
        /// </summary>
        public void ResetTargetObjectTypeInfo() => ResetTargetObjectTypeInfo(targetObjectType);

        /// <summary>
        /// 有效目标对象类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool ValidTargetObjectType(Type type) => type != null;

        /// <summary>
        /// 获取目标对象类型的弹出菜单特性
        /// </summary>
        /// <returns></returns>
        public virtual Type GetTargetObjectTypePopupAttribute()
        {
            var targetObjectType = this.targetObjectType;
            if (targetObjectType == null) return null;
            if (typeof(Component).IsAssignableFrom(targetObjectType))
            {
                return typeof(ComponentPopupAttribute);
            }
            else if (typeof(StateComponent).IsAssignableFrom(targetObjectType))
            {
                return typeof(StateComponentPopupAttribute);
            }
            else if (typeof(TransitionComponent).IsAssignableFrom(targetObjectType))
            {
                return typeof(TransitionComponentPopupAttribute);
            }
            else if (typeof(StateGroupComponent).IsAssignableFrom(targetObjectType))
            {
                return typeof(StateGroupComponentPopupAttribute);
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(targetObjectType))
            {
                return typeof(ObjectPopupAttribute);
            }
            return null;
        }

        #endregion
    }

    /// <summary>
    /// 类型权限
    /// </summary>
    [Name("类型权限")]
    public enum ETypeAccess
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [Abbreviation("")]
        None,

        /// <summary>
        /// 公有
        /// </summary>
        [Name("公有")]
        [Abbreviation("public")]
        Public,

        /// <summary>
        /// 保护
        /// </summary>
        [Name("保护")]
        [Abbreviation("protected")]
        Protected,

        /// <summary>
        /// 私有
        /// </summary>
        [Name("私有")]
        [Abbreviation("private")]
        Private,

        /// <summary>
        /// 内部
        /// </summary>
        [Name("内部")]
        [Abbreviation("internal")]
        Internal,

        /// <summary>
        /// 内部保护
        /// </summary>
        [Name("内部保护")]
        [Abbreviation("internal protected")]
        InternalProtected,

        /// <summary>
        /// 内部保护抽象
        /// </summary>
        [Name("内部保护抽象")]
        [Abbreviation("internal protected abstract")]
        InternalProtectedAbstract,

        /// <summary>
        /// 抽象
        /// </summary>
        [Name("抽象")]
        [Abbreviation("abstract")]
        Abstract,

        /// <summary>
        /// 公有抽象
        /// </summary>
        [Name("公有抽象")]
        [Abbreviation("public abstract")]
        PublicAbstract,
    }

    /// <summary>
    /// 类型种类
    /// </summary>
    [Name("类型种类")]
    public enum ETypeKind
    {
        /// <summary>
        /// 类
        /// </summary>
        [Name("类")]
        [Abbreviation("class")]
        Class,

        /// <summary>
        /// 枚举
        /// </summary>
        [Name("枚举")]
        [Abbreviation("enum")]
        Enum,

        /// <summary>
        /// 结构体
        /// </summary>
        [Name("结构体")]
        [Abbreviation("struct")]
        Struct,
    }
}
