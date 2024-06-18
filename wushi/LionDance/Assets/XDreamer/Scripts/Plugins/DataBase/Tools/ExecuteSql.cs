using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Net;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Interfaces;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginDataBase.Base;
using XCSJ.PluginTools.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginDataBase.Tools
{
    /// <summary>
    /// 对象关联信息
    /// </summary>
    [Name("对象关联信息")]
    [Tip("对象关联信息", "Object association information")]
    [RequireManager(typeof(DBManager))]
    [Tool(DBCategory.FuncCompoents, nameof(DBManager))]
    [XCSJ.Attributes.Icon(EIcon.Link)]
    [Owner(typeof(DBManager))]
    public class ExecuteSql :Interactor, IToFriendlyString
    {
        /// <summary>
        /// 数据库
        /// </summary>
        [Name("数据库")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DBMB _dbMB;

        /// <summary>
        /// 数据库
        /// </summary>
        public DBMB dbMB => this.XGetComponentInParentOrGlobal(ref _dbMB);

        /// <summary>
        /// 修改结果集缓存
        /// </summary>
        [Name("修改结果集缓存")]
        [Tip("修改数据库组件中结果集缓存", "Modify the result set cache in the database component")]
        public bool _modifyResultSetCache = true;

        /// <summary>
        /// Sql执行模式
        /// </summary>
        [Name("Sql执行模式")]
        [EnumPopup]
        public ESqlExecuteMode _sqlExecuteMode = ESqlExecuteMode.ConditionQuery;

        /// <summary>
        /// 非查询SQL:非查询SQL语句
        /// </summary>
        [Name("非查询SQL")]
        [Tip("非查询SQL语句", "Non query SQL statement")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.NonQuery)]
        [OnlyMemberElements]
        public StringPropertyValue _nonQuerySql = new StringPropertyValue();

        /// <summary>
        /// 查询SQL:查询SQL语句
        /// </summary>
        [Name("查询SQL")]
        [Tip("查询SQL语句", "Query SQL Statement")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.Query)]
        [OnlyMemberElements]
        public StringPropertyValue _querySql = new StringPropertyValue();

        /// <summary>
        /// 条件查询:条件查询
        /// </summary>
        [Name("条件查询")]
        [Tip("条件查询", "Condition query")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.ConditionQuery)]
        [OnlyMemberElements]
        public ConditionQueryData _conditionQueryData = new ConditionQueryData();

        /// <summary>
        /// 覆盖条件值
        /// </summary>
        [Name("覆盖条件值")]
        public enum EOverrideConditionValue
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 名称
            /// </summary>
            [Name("名称")]
            Name,

            /// <summary>
            /// 类型名称
            /// </summary>
            [Name("类型名称")]
            TypeName,

            /// <summary>
            /// 类型全名称
            /// </summary>
            [Name("类型全名称")]
            TypeFullName,
        }

        /// <summary>
        /// 覆盖条件值
        /// </summary>
        [Name("覆盖条件值")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.ConditionQuery)]
        public EOverrideConditionValue _overrideConditionValue = EOverrideConditionValue.None;

        /// <summary>
        /// 覆盖条件Unity对象
        /// </summary>
        [Name("覆盖条件Unity对象")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.ConditionQuery)]
        [ObjectPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public UnityEngine.Object _overrideConditionUnityObject;

        /// <summary>
        /// 覆盖条件Unity对象
        /// </summary>
        public UnityEngine.Object overrideConditionUnityObject
        {
            get
            {
                if (!_overrideConditionUnityObject)
                {
                    this.XModifyProperty(ref _overrideConditionUnityObject, gameObject);
                }
                return _overrideConditionUnityObject;
            }
        }

        /// <summary>
        /// 非查询键:键值非查询使用的键名
        /// </summary>
        [Name("非查询键")]
        [Tip("键值非查询使用的键名", "Key name used in key value non query")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.KVNonQuery)]
        public StringPropertyValue _nonQueryKey = new StringPropertyValue();

        /// <summary>
        /// 非查询值参数列表:键值非查询使用的值参数列表
        /// </summary>
        [Name("非查询值参数列表")]
        [Tip("键值非查询使用的值参数列表", "List of value parameters used in key value non query")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.KVNonQuery)]
        public List<StringPropertyValue> _nonQueryValueParams = new List<StringPropertyValue>();

        /// <summary>
        /// 查询键:键值查询使用的键名
        /// </summary>
        [Name("查询键")]
        [Tip("键值查询使用的键名", "Key name used in key value query")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.KVQuery)]
        public StringPropertyValue _queryKey = new StringPropertyValue();

        /// <summary>
        /// 查询值参数列表:键值查询使用的值参数列表
        /// </summary>
        [Name("查询值参数列表")]
        [Tip("键值查询使用的值参数列表", "List of value parameters used in key value query")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.KVQuery)]
        public List<StringPropertyValue> _queryValueParams = new List<StringPropertyValue>();

        /// <summary>
        /// 获取条件值
        /// </summary>
        /// <returns></returns>
        public string GetConditionValue()
        {
            switch (_overrideConditionValue)
            {
                case EOverrideConditionValue.Name:
                    {
                        var target = overrideConditionUnityObject;
                        if (target) return target.name;
                        break;
                    }
                case EOverrideConditionValue.TypeName:
                    {
                        var target = overrideConditionUnityObject;
                        if (target) return target.GetType().Name;
                        break;
                    }
                case EOverrideConditionValue.TypeFullName:
                    {
                        var target = overrideConditionUnityObject;
                        if (target) return target.GetType().FullName;
                        break;
                    }
            }
            return null;
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString()
        {
            switch (_sqlExecuteMode)
            {
                case ESqlExecuteMode.NonQuery: return _nonQuerySql.ToFriendlyString();
                case ESqlExecuteMode.Query: return _querySql.ToFriendlyString();
                case ESqlExecuteMode.ConditionQuery: return _conditionQueryData.ToFriendlyString(GetConditionValue());
                case ESqlExecuteMode.KVNonQuery: return _nonQueryKey.ToFriendlyString();
                case ESqlExecuteMode.KVQuery: return _queryKey.ToFriendlyString();
            }
            return "";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public string GetSql()
        {
            switch (_sqlExecuteMode)
            {
                case ESqlExecuteMode.NonQuery: return _nonQuerySql.GetValue();
                case ESqlExecuteMode.Query: return _querySql.GetValue();
                case ESqlExecuteMode.ConditionQuery: return _conditionQueryData.GetSql(GetConditionValue());
                case ESqlExecuteMode.KVNonQuery: return _nonQueryKey.GetValue();
                case ESqlExecuteMode.KVQuery: return _queryKey.GetValue();
            }
            return "";
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [Name("执行")]
        [InteractCmd]
        [InteractCmdFun(nameof(Execute))]
        public EInteractResult Execute(InteractData interactData)
        {
            var dbMB = this.dbMB;
            if (!dbMB) return EInteractResult.Fail;

            //Debug.Log("Execute: " + name);
            switch (_sqlExecuteMode)
            {
                case ESqlExecuteMode.NonQuery:
                    {
                        if (!dbMB.TryExecuteNonQuery(_nonQuerySql.GetValue(), OnExecuteFinished))
                        {
                            OnExecuteFinished(InvokeResult.Fail, default(Result));
                        }
                        break;
                    }
                case ESqlExecuteMode.Query:
                    {
                        if (!dbMB.TryExecuteQuery(_querySql.GetValue(), OnExecuteFinished, _modifyResultSetCache))
                        {
                            OnExecuteFinished(InvokeResult.Fail, default(ResultSet));
                        }
                        break;
                    }
                case ESqlExecuteMode.ConditionQuery:
                    {
                        if (!dbMB.TryExecuteQuery(_conditionQueryData.GetSql(GetConditionValue()), OnExecuteFinished, _modifyResultSetCache))
                        {
                            OnExecuteFinished(InvokeResult.Fail, default(ResultSet));
                        }
                        break;
                    }
                case ESqlExecuteMode.KVNonQuery:
                    {
                        if (!dbMB.TryExecuteKVNonQuery(_nonQueryKey.GetValue(), _nonQueryValueParams.ToList(pv => pv.GetValue()), OnExecuteFinished))
                        {
                            OnExecuteFinished(InvokeResult.Fail, default(Result));
                        }
                        break;
                    }
                case ESqlExecuteMode.KVQuery:
                    {
                        if (!dbMB.TryExecuteKVQuery(_queryKey.GetValue(), _queryValueParams.ToList(pv => pv.GetValue()), OnExecuteFinished, _modifyResultSetCache))
                        {
                            OnExecuteFinished(InvokeResult.Fail, default(ResultSet));
                        }
                        break;
                    }
            }
            return EInteractResult.Success;
        }

        private void OnExecuteFinished(InvokeResult invokeResult, Result result)
        {
            //Debug.Log("OnExecuteFinished 0: " + name);
            if (invokeResult)
            {
                CallFinished(nameof(Execute));
            }
            else
            {
                CallAborted(nameof(Execute));
            }
        }

        private void OnExecuteFinished(InvokeResult invokeResult, ResultSet resultSet)
        {
            //Debug.Log("OnExecuteFinished 1: " + name);
            if (invokeResult)
            {
                CallFinished(nameof(Execute));
            }
            else
            {
                CallAborted(nameof(Execute));
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (overrideConditionUnityObject) { }
            if (dbMB) { }
        }
    }

    /// <summary>
    /// Sql执行模式
    /// </summary>
    [Name("Sql执行模式")]
    public enum ESqlExecuteMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 非查询
        /// </summary>
        [Name("非查询")]
        NonQuery,

        /// <summary>
        /// 查询
        /// </summary>
        [Name("查询")]
        Query,

        /// <summary>
        /// 条件查询
        /// </summary>
        [Name("条件查询")]
        ConditionQuery,

        /// <summary>
        /// 键值非查询
        /// </summary>
        [Name("键值非查询")]
        KVNonQuery,

        /// <summary>
        /// 键值查询
        /// </summary>
        [Name("键值查询")]
        KVQuery,
    }
}


