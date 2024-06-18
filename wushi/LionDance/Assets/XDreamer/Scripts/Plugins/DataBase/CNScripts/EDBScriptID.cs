using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginDataBase.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginDataBase.CNScripts
{
    /// <summary>
    /// 数据库脚本ID
    /// </summary>
    [Name("数据库脚本ID")]
    [ScriptEnum(typeof(DBManager))]
    public enum EDBScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region 数据库-目录
        /// <summary>
        /// 数据库
        /// </summary>
        [ScriptName("数据库", nameof(DB), EGrammarType.Category)]
        #endregion
        DB,

        /// <summary>
        /// 执行非查询SQL
        /// </summary>
        [ScriptName("执行非查询SQL", nameof(ExecuteNonQuerySQL))]
        [ScriptReturn("成功返回  #True，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.String, "Create/Drop/Insert/Delete/Update语句:")]
        [ScriptParams(10, EParamType.UserDefineFun, "执行后回调函数(携带参数为影响记录的数目):")]
        ExecuteNonQuerySQL,

        /// <summary>
        /// 执行查询SQL
        /// </summary>
        [ScriptName("执行查询SQL", nameof(ExecuteQuerySQL))]
        [ScriptDescription("成功执行的结果存放在结果集中，具体信息可使用结果集的相关脚本进行信息获；")]
        [ScriptReturn("成功返回  #True，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.String, "Select语句:")]
        [ScriptParams(10, EParamType.UserDefineFun, "执行后回调函数(携带参数为结果集记录数目):")]
        ExecuteQuerySQL,

        /// <summary>
        /// 执行条件查询
        /// </summary>
        [ScriptName("执行条件查询")]
        [ScriptDescription("成功执行的结果存放在结果集中，具体信息可使用结果集的相关脚本进行信息获；")]
        [ScriptReturn("成功返回 #True，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.String, "数据库表名:")]
        [ScriptParams(2, EParamType.String, "字段名(如不填写使用*替换，多个字段使用<c>即,分割):")]
        [ScriptParams(3, EParamType.String, "条件字段:")]
        [ScriptParams(4, EParamType.Combo, "匹配条件:", "=", "<>", ">", ">=", "<", "<=", "like", "between", "in", "not in", " ")]
        [ScriptParams(5, EParamType.String, "条件值:")]
        [ScriptParams(6, EParamType.Bool2, "条件值是文本值(如果是，会对条件值添加单引号):")]
        [ScriptParams(10, EParamType.UserDefineFun, "执行后回调函数(携带参数为结果集记录数目):")]
        ExecuteConditionQuery,        

        /// <summary>
        /// 获取结果集信息
        /// </summary>
        [ScriptName("获取结果集信息")]
        [ScriptReturn("成功返回 具体的请求信息，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.Combo, "信息类型:", "记录数目", "字段数目", "SQL语句", "结果值", "错误信息", "是否有效")]
        GetResultSetInfo,

        /// <summary>
        /// 获取结果集字段名
        /// </summary>
        [ScriptName("获取结果集字段名")]
        [ScriptReturn("成功返回 具体的请求信息，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.IntSlider, "字段索引:", 1, 32)]
        GetResultSetFieldName,

        /// <summary>
        /// 获取结果集字段值
        /// </summary>
        [ScriptName("获取结果集字段值")]
        [ScriptReturn("成功返回 具体的请求信息，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.IntSlider, "记录索引:", 1, 999)]
        [ScriptParams(2, EParamType.IntSlider, "字段索引:", 1, 32)]
        GetResultSetFieldValue,

        /// <summary>
        /// 获取结果集字段值(按字段名)
        /// </summary>
        [ScriptName("获取结果集字段值(按字段名)")]
        [ScriptReturn("成功返回 具体的请求信息，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.IntSlider, "记录索引:", 1, 999)]
        [ScriptParams(2, EParamType.String, "字段名:")]
        GetResultSetFieldValueByFieldName,

        /// <summary>
        /// 结果集转字典
        /// </summary>
        [ScriptName("结果集转字典")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.Combo, "转换方式:", "完整结果集", "仅记录", "仅字段")]
        [ScriptParams(2, EParamType.Dictionary, "字典名:")]
        ResultSetToDictionary,

        /// <summary>
        /// 结果集转字符串
        /// </summary>
        [ScriptName("结果集转字符串")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.Combo, "转换方式:", "完整结果集", "仅记录", "仅字段")]
        ResultSetToString,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent,

        /// <summary>
        /// 借宿
        /// </summary>
        _End = IDRange.End,
    }
}

