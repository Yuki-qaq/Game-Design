using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.DataBase;
using XCSJ.Extension;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginDataBase.Base;
using XCSJ.PluginDataBase.Tools;
using XCSJ.PluginXGUI.Windows.ListViews;
using XCSJ.Products;

namespace XCSJ.PluginDataBase.UI
{
    /// <summary>
    /// 结果集列表
    /// </summary>
    [Name("结果集列表")]
    [RequireManager(typeof(DBManager))]
    [Owner(typeof(DBManager))]
    public class ResultSetList : ListViewModelProvider
    {
        /// <summary>
        /// 结果集源
        /// </summary>
        [Name("结果集源")]
        [EnumPopup]
        public EResultSetSource _resultSetSource = EResultSetSource.SqlExecuteResult;

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
        /// SQL执行数据
        /// </summary>
        [Name("SQL执行数据")]
        public SqlExecuteData _sqlExecuteData = new SqlExecuteData();

        private List<ListViewItemModel> listViewItemModels = new List<ListViewItemModel>();

        private void SetResultSet(ResultSet resultSet)
        {
            ClearList();
            if (resultSet == null) return;
            foreach (var record in resultSet.recordSet.records)
            {
                listViewItemModels.Add(new RecordModel(record));
            }
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (dbMB) { }
            DisplayResultSet();
            DBMB.onResultSetChanged += OnResultSetChanged;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            DBMB.onResultSetChanged -= OnResultSetChanged;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (dbMB) { }
        }

        private void ClearList()
        {
            listView.RemoveModel(listViewItemModels);
            listViewItemModels.Clear();
        }

        private void RefreshList()
        {
            listView.AddModel(listViewItemModels);
            listView.Refresh();
        }

        void OnResultSetChanged(DBMB db)
        {
            switch(_resultSetSource)
            {
                case EResultSetSource.LatestDBResultSet:
                    {
                        if (db == _dbMB) SetDBResultSet();
                        break;
                    }
                case EResultSetSource.AnyLatestDBResultSet:
                    {
                        SetDBResultSet(db);
                        break;
                    }
            }
        }

        private void SetDBResultSet(DBMB db)
        {
            ClearList();
            if (db)
            {
                SetResultSet(db.resultSet);
            }
            RefreshList();
        }

        private void SetDBResultSet() => SetDBResultSet(_dbMB);

        /// <summary>
        /// 显示结果集
        /// </summary>
        [InteractCmd]
        [Name("显示结果集")]
        public void DisplayResultSet()
        {
            switch (_resultSetSource)
            {
                case EResultSetSource.DBResultSet:
                case EResultSetSource.LatestDBResultSet:
                case EResultSetSource.AnyLatestDBResultSet:
                    {
                        SetDBResultSet();
                        break;
                    }
                case EResultSetSource.SqlExecuteResult:
                    {
                        ClearList();
                        RefreshList();
                        ExecuteSql();
                        break;
                    }
                default:
                    {
                        ClearList();
                        RefreshList();
                        break;
                    }
            }
        }

        private void ExecuteSql()
        {
            _sqlExecuteData.Execute((d, ir, r) =>
            {
                SetResultSet(_sqlExecuteData.resultSet);
                RefreshList();
            });
        }
    }

    /// <summary>
    /// 结果集源
    /// </summary>
    [Name("结果集源")]
    public enum EResultSetSource
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// Sql执行结果
        /// </summary>
        [Name("Sql执行结果")]
        [Tip("使用Sql执行结果的结果集作为显示源；", "Use the result set of Sql execution results as the display source;")]
        SqlExecuteResult,

        /// <summary>
        /// 数据库结果集
        /// </summary>
        [Name("数据库结果集")]
        [Tip("使用数据库结果集作为显示源；", "Use the database result set as the display source;")]
        DBResultSet,

        /// <summary>
        /// 最新的数据库结果集
        /// </summary>
        [Name("最新的数据库结果集")]
        [Tip("使用数据库结果集作为显示源；每当数据库结果集发生变化时，会再次尝试刷新关联的界面；", "Use the database result set as the display source; Whenever the database result set changes, it attempts to refresh the associated interface again;")]
        LatestDBResultSet,

        /// <summary>
        /// 任意最新的数据库结果集
        /// </summary>
        [Name("任意最新的数据库结果集")]
        [Tip("使用任意数据库结果集作为显示源；每当数据库结果集发生变化时，会再次尝试刷新关联的界面；", "Use the database result set as the display source; Whenever the database result set changes, it attempts to refresh the associated interface again;")]
        AnyLatestDBResultSet,
    }

    /// <summary>
    /// SQL执行数据
    /// </summary>
    [Serializable]
    [Name("SQL执行数据")]
    public class SqlExecuteData
    {
        private ResultSet _resultSet;

        /// <summary>
        /// 结果集
        /// </summary>
        public ResultSet resultSet => _resultSet;

        /// <summary>
        /// 数据库
        /// </summary>
        [Name("数据库")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DBMB _dbMB;

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
        public ESqlExecuteMode _sqlExecuteMode = ESqlExecuteMode.Query;

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
        /// 执行
        /// </summary>
        /// <param name="onInvoked"></param>
        public void Execute(Action<SqlExecuteData, InvokeResult, Result> onInvoked)
        {
            var dbMB = this._dbMB;
            if (!dbMB) return;
            switch (_sqlExecuteMode)
            {
                case ESqlExecuteMode.NonQuery:
                    {
                        dbMB.TryExecuteNonQuery(_nonQuerySql.GetValue(), (ir, r) =>
                        {
                            onInvoked?.Invoke(this, ir, r);
                        });
                        break;
                    }
                case ESqlExecuteMode.Query:
                    {
                        dbMB.TryExecuteQuery(_querySql.GetValue(), (ir, rs) =>
                        {
                            SetRS(rs);
                            onInvoked?.Invoke(this, ir, rs);
                        }, _modifyResultSetCache);
                        break;
                    }
                case ESqlExecuteMode.ConditionQuery:
                    {
                        dbMB.TryExecuteQuery(_conditionQueryData.GetSql(), (ir, rs) =>
                        {
                            SetRS(rs);
                            onInvoked?.Invoke(this, ir, rs);
                        }, _modifyResultSetCache);
                        break;
                    }
                case ESqlExecuteMode.KVNonQuery:
                    {
                        dbMB.TryExecuteKVNonQuery(_nonQueryKey.GetValue(), _nonQueryValueParams.ToList(pv => pv.GetValue()), (ir, r) =>
                        {
                            onInvoked?.Invoke(this, ir, r);
                        });
                        break;
                    }
                case ESqlExecuteMode.KVQuery:
                    {
                        dbMB.TryExecuteKVQuery(_queryKey.GetValue(), _queryValueParams.ToList(pv => pv.GetValue()), (ir, rs) =>
                        {
                            SetRS(rs);
                            onInvoked?.Invoke(this, ir, rs);
                        }, _modifyResultSetCache);
                        break;
                    }
                default:
                    {
                        SetRS(dbMB.resultSet);
                        break;
                    }
            }
        }

        private void SetRS(ResultSet resultSet)
        {
            this._resultSet = resultSet;
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
                case ESqlExecuteMode.ConditionQuery: return _conditionQueryData.ToFriendlyString();
                case ESqlExecuteMode.KVNonQuery: return _nonQueryKey.ToFriendlyString();
                case ESqlExecuteMode.KVQuery: return _queryKey.ToFriendlyString();
            }
            return "";
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public bool DataValidity()
        {
            if (!_dbMB) return false;
            switch (_sqlExecuteMode)
            {
                case ESqlExecuteMode.NonQuery: return _nonQuerySql.DataValidity();
                case ESqlExecuteMode.Query: return _querySql.DataValidity();
                case ESqlExecuteMode.ConditionQuery: return _conditionQueryData.DataValidity();
                case ESqlExecuteMode.KVNonQuery: return _nonQueryKey.DataValidity();
                case ESqlExecuteMode.KVQuery: return _queryKey.DataValidity();
            }
            return true;
        }
    }

    /// <summary>
    /// 记录模型
    /// </summary>
    public class RecordModel : ListViewItemModel
    {
        /// <summary>
        /// 记录
        /// </summary>
        public Record record { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="record"></param>
        public RecordModel(Record record) : base(record)
        {
            this.record = record;
        }

        /// <summary>
        /// 键列表
        /// </summary>
        public override IEnumerable<string> keys => record.fieldSet.fields.Cast(field => field.name);

        /// <summary>
        /// 尝试获取模型键值类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool TryGetModelKeyValueType(string key, out Type type)
        {
            type = record.fieldSet[key]?.type;
            return type != null;
        }

        /// <summary>
        /// 尝试获取模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetModelKeyValue(string key, out object value)
        {
            if(record.TryGetFieldValue(key,out var fieldValue))
            {
                value = fieldValue.objectValue;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试设置取模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetModelKeyValue(string key, object value)
        {
            return false;
        }
    }
}
