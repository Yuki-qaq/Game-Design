using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.DataBase;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginDataBase.CNScripts;
using XCSJ.PluginDataBase.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginDataBase.Tools.Kernel
{
    /// <summary>
    /// 默认DB处理器
    /// </summary>
    public class DefaultDBHandler : InstanceClass<DefaultDBHandler>, IDBHandler
    {
        private static bool initialized = false;

        /// <summary>
        /// 初始化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            DBHandler.handler = instance;
            if (initialized) return;
            initialized = true;
#if UNITY_WEBGL
            TypeHelper.RegisterConstructorFunc<RecordSet>();
            TypeHelper.RegisterConstructorFunc<FieldSet>();
            TypeHelper.RegisterConstructorFunc<Field>();
            TypeHelper.RegisterConstructorFunc<BaseA>();
            TypeHelper.RegisterConstructorFunc<NonQueryA>();
            TypeHelper.RegisterConstructorFunc<QueryA>();
            TypeHelper.RegisterConstructorFunc<TableStructureA>();
            TypeHelper.RegisterConstructorFunc<CreateTableA>();
            TypeHelper.RegisterConstructorFunc<TablesA>();
            TypeHelper.RegisterConstructorFunc<CloseA>();
            TypeHelper.RegisterConstructorFunc<UserA>();
#endif
        }

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public List<Script> GetScripts(DBManager manager) => Script.GetScriptsOfEnum<EDBScriptID>(manager);

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public ReturnValue ExecuteScript(DBManager manager, int id, ScriptParamList param) => ExecuteScript(manager, (EDBScriptID)id, param);

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private ReturnValue ExecuteScript(DBManager manager, EDBScriptID id, ScriptParamList param)
        {
            switch (id)
            {                
                case EDBScriptID.ExecuteNonQuerySQL:
                    {
                        var db = manager.GetDBMonoBehaviour((int)param[0] - 1);
                        if (db == null) break;
                        var funcName = param[10] as string;

                        var ret = db.TryExecuteNonQuery(param[1] as string, (ir, r) =>
                        {
                            ScriptManager.CallUDF(funcName, r.result.ToString());
                        });

                        return ReturnValue.Create(ret);
                    }
                case EDBScriptID.ExecuteQuerySQL:
                    {
                        var db = manager.GetDBMonoBehaviour((int)param[0] - 1);
                        if (db == null) break;
                        var funcName = param[10] as string;

                        var ret = db.TryExecuteQuery(param[1] as string, (ir, r) =>
                        {
                            ScriptManager.CallUDF(funcName, r.recordSet.count.ToString());
                        });

                        return ReturnValue.Create(ret);
                    }
                case EDBScriptID.ExecuteConditionQuery:
                    {
                        var db = manager.GetDBMonoBehaviour((int)param[0] - 1);
                        if (db == null) break;
                        var funcName = param[10] as string;

                        var selectFields = param[2] as string;
                        if (string.IsNullOrEmpty(selectFields)) selectFields = "*";

                        var condition = param[5] as string;
                        if (EBool2.Yes == (EBool2)param[6]) condition = string.Format("'{0}'", condition);

                        var sql = string.Format("select {0} from {1} where {2} {3} {4}", selectFields, param[1], param[3], param[4], condition);

                        var ret = db.TryExecuteQuery(sql, (ir, r) =>
                        {
                            ScriptManager.CallUDF(funcName, r.result.ToString());
                        });

                        return ReturnValue.Create(ret);
                    }
                case EDBScriptID.GetResultSetInfo:
                    {
                        var resultSet = manager.GetResultSet((int)param[0] - 1);
                        if (resultSet == null) break;
                        switch (param[1] as string)
                        {
                            case "记录数目": return ReturnValue.True(resultSet.recordSet.records.Count);
                            case "字段数目": return ReturnValue.True(resultSet.fieldSet.fields.Count);
                            case "SQL语句": return ReturnValue.True(resultSet.sql.sql);
                            case "结果值": return ReturnValue.True(resultSet.result);
                            case "错误信息": return ReturnValue.True(resultSet.error);
                            case "是否有效": return ReturnValue.Create(resultSet.isValid);
                        }
                        break;
                    }
                case EDBScriptID.GetResultSetFieldName:
                    {
                        var resultSet = manager.GetResultSet((int)param[0] - 1);
                        if (resultSet == null) break;
                        var fieldIndex = (int)param[1] - 1;
                        if (fieldIndex < 0 || fieldIndex >= resultSet.fieldSet.count) break;

                        return ReturnValue.True(resultSet.fieldSet[fieldIndex].name);
                    }
                case EDBScriptID.GetResultSetFieldValue:
                    {
                        var resultSet = manager.GetResultSet((int)param[0] - 1);
                        if (resultSet == null) break;
                        var recordIndex = (int)param[1] - 1;
                        var fieldIndex = (int)param[2] - 1;
                        FieldValue fieldValue;
                        if (resultSet.TryGetFieldValue(recordIndex, fieldIndex, out fieldValue))
                        {
                            return ReturnValue.True(fieldValue.Value<string>());
                        }
                        //Debug.Log("GetResultSetFieldValue: " + recordIndex + ", " + fieldIndex);
                        break;
                    }
                case EDBScriptID.GetResultSetFieldValueByFieldName:
                    {
                        var resultSet = manager.GetResultSet((int)param[0] - 1);
                        if (resultSet == null) break;
                        var recordIndex = (int)param[1] - 1;
                        var fieldName = param[2] as string;
                        FieldValue fieldValue;
                        if (resultSet.TryGetFieldValue(recordIndex, fieldName, out fieldValue))
                        {
                            return ReturnValue.True(fieldValue.Value<string>());
                        }
                        //Debug.Log("GetResultSetFieldValueByFieldName: " + recordIndex + ", " + fieldName);
                        break;
                    }
                case EDBScriptID.ResultSetToDictionary:
                    {
                        var resultSet = manager.GetResultSet((int)param[0] - 1);
                        if (resultSet == null) break;
                        try
                        {
                            switch (param[1] as string)
                            {
                                case "完整结果集":
                                    {
                                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, resultSet.ToJson(), EVarType.Dictionary);
                                        break;
                                    }
                                case "仅记录":
                                    {
                                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, resultSet.recordSet.ToJsonLite(), EVarType.Dictionary);
                                        break;
                                    }
                                case "仅字段":
                                    {
                                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, resultSet.fieldSet.ToJsonLite(), EVarType.Dictionary);
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(nameof(EDBScriptID.ResultSetToDictionary) + "执行异常：" + ex);
                        }
                        break;
                    }
                case EDBScriptID.ResultSetToString:
                    {
                        var resultSet = manager.GetResultSet((int)param[0] - 1);
                        if (resultSet == null) break;

                        try
                        {
                            switch (param[1] as string)
                            {
                                case "完整结果集":
                                    {
                                        return ReturnValue.True(resultSet.ToJson());
                                    }
                                case "仅记录":
                                    {
                                        return ReturnValue.True(resultSet.recordSet.ToJsonLite());
                                    }
                                case "仅字段":
                                    {
                                        return ReturnValue.True(resultSet.fieldSet.ToJsonLite());
                                    }
                            }
                        }
                        catch(Exception ex)
                        {
                            Log.Exception(nameof(EDBScriptID.ResultSetToString) + "执行异常：" + ex);
                        }
                        break;
                    }
            }
            return ReturnValue.No;
        }
    }
}
