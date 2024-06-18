using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.DataBase;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginDataBase;
using XCSJ.PluginDataBase.Tools;

namespace XCSJ.EditorDataBase.Tools
{
    /// <summary>
    /// 数据库组件检查器
    /// </summary>
    [CustomEditor(typeof(DBMB), true)]
    [Name("数据库组件检查器")]
    public class DBMBInspector : DBMBInspector<DBMB> { }

    /// <summary>
    /// 数据库组件检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DBMBInspector<T> : MBInspector<T> where T : DBMB
    {
        private CategoryList categoryList;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            categoryList = EditorToolsHelper.GetWithPurposes(typeof(T).Name, nameof(DBMB));
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

        void OnResultSetChanged(DBMB dBMB)
        {
            Repaint();
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawTables();
            DrawResultSet();
            EditorGUILayout.Separator();
            CategoryListExtension.DrawVertical(categoryList);
        }

        /// <summary>
        /// 显示数据库信息
        /// </summary>
        [Name("显示数据库信息")]
        public bool displayDBInfo = true;

        /// <summary>
        /// 数据库连接状态
        /// </summary>
        [Name("数据库连接状态")]
        [Readonly]
        public bool dbConnnectState = false;

        /// <summary>
        /// 表名
        /// </summary>
        [Name("表名")]
        public string tableName;

        [LanguageTuple("Operation","操作")]
        [LanguageTuple("Query", "查询")]
        private void DrawTables()
        {
            var db = mb.db;
            displayDBInfo = UICommonFun.Foldout(displayDBInfo, TrLabel(nameof(displayDBInfo)), () =>
            {
                if (GUILayout.Button(UICommonOption.Update, EditorStyles.miniButtonRight, UICommonOption.WH24x16) && db.IsConnected())
                {
                    db.tables.Clear();
                    db.TryGetTablesWithTableStructure(ir =>
                    {
                        UICommonFun.DelayCall(Repaint);
                    });
                }
            });
            if (!displayDBInfo) return;

            CommonFun.BeginLayout();
            {
                EditorGUILayout.Toggle(TrLabel(nameof(dbConnnectState)), db.IsConnected());

                // 标题
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    GUILayout.Label("NO.", UICommonOption.Width32);
                    GUILayout.Label(TrLabel(nameof(tableName)));
                    GUILayout.Label(Tr("Operation"), UICommonOption.Width80);
                }
                EditorGUILayout.EndHorizontal();

                // 表
                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));
                if (db?.tables?.weakItems is IList<Table> tables)
                {
                    for (int i = 0; i < tables.Count; i++)
                    {
                        var table = tables[i];
                        var tableName = table.name;

                        UICommonFun.BeginHorizontal(i);

                        //编号
                        EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                        //表名
                        EditorGUILayout.TextField(tableName);

                        //查询表数据
                        if (GUILayout.Button(Tr("Query"), UICommonOption.Width80))
                        {
                            ExecuteSQL(string.Format("select * from {0}", tableName));
                        }

                        UICommonFun.EndHorizontal();                       
                    }
                }
                EditorGUILayout.EndVertical();
            }
            CommonFun.EndLayout();
        }

        /// <summary>
        /// 显示结果集
        /// </summary>
        [Name("显示结果集")]
        public bool displayResultSet = true;

        /// <summary>
        /// SQL
        /// </summary>
        [Name("SQL")]
        public string sql = "";

        /// <summary>
        /// 结果集SQL
        /// </summary>
        [Name("结果集SQL")]
        public string resultSetSql = "";

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql"></param>
        private void ExecuteSQL(string sql)
        {
            if (mb.db.IsConnected() && !string.IsNullOrEmpty(sql) && !mb.TryExecuteQuery(sql, (ir, rs) =>
            {
                UICommonFun.DelayCall(Repaint);
            }))
            {
                //
            }
        }

        /// <summary>
        /// 绘制结果集
        /// </summary>
        [LanguageTuple("Execute SQL", "执行SQL")]
        private void DrawResultSet()
        {
            displayResultSet = UICommonFun.Foldout(displayResultSet, TrLabel(nameof(displayResultSet)));
            if (!displayResultSet) return;
            CommonFun.BeginLayout();
            {
                EditorGUILayout.BeginHorizontal();
                sql = EditorGUILayout.TextField(TrLabel(nameof(sql)), sql);
                if (GUILayout.Button(Tr("Execute SQL"), UICommonOption.Width80))
                {
                    ExecuteSQL(sql);
                }
                EditorGUILayout.EndHorizontal();

                var resultSet = mb.resultSet;
                EditorGUILayout.TextField(TrLabel(nameof(resultSetSql)), resultSet?.sql ?? "");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(TrLabel(nameof(displayMode)));
                displayMode = UICommonFun.Toolbar(displayMode, ENameTip.Image, GUILayout.Height(24));
                EditorGUILayout.EndHorizontal();
                
                switch (displayMode)
                {
                    case EDisplayMode.KeyValue:
                        {
                            DrawResultSetKeyValue(resultSet);
                            break;
                        }
                    default:
                        {
                            DrawResultSetList(resultSet);
                            break;
                        }
                }
            }
            CommonFun.EndLayout();
        }

        /// <summary>
        /// 显示模式
        /// </summary>
        //[Name("显示模式")]
        public EDisplayMode displayMode = EDisplayMode.List;

        /// <summary>
        /// 显示模式
        /// </summary>
        //[Name("显示模式")]
        public enum EDisplayMode
        {
            /// <summary>
            /// 列表
            /// </summary>
            [Name("列表")]
            [Attributes.Icon(EIcon.List)]
            List,

            /// <summary>
            /// 键值
            /// </summary>
            [Name("键值")]
            [Attributes.Icon(EIcon.LeftAlign)]
            KeyValue,
        }

        /// <summary>
        /// 绘制结果集列表
        /// </summary>
        /// <param name="resultSet"></param>
        private void DrawResultSetList(ResultSet resultSet)
        {
            if (resultSet == null) return;

            //字段名
            GUILayout.BeginHorizontal(GUI.skin.box);
            for (int i = 0; i < resultSet.fieldSet.fields.Count; i++)
            {
                var field = resultSet.fieldSet.fields[i];
                var fieldName = field.name;

                EditorGUILayout.TextField(fieldName);
            }
            GUILayout.EndHorizontal();

            //记录
            for (int i = 0; i < resultSet.recordSet.records.Count; i++)
            {
                var record = resultSet.recordSet.records[i];
                UICommonFun.BeginHorizontal(i);
                for (int j = 0; j < record.fieldValues.Count; j++)
                {
                    EditorGUILayout.TextField(record[j].Value<string>());
                }
                UICommonFun.EndHorizontal();
            }
        }

        /// <summary>
        /// 绘制结果集键值
        /// </summary>
        /// <param name="resultSet"></param>
        private void DrawResultSetKeyValue(ResultSet resultSet)
        {
            if (resultSet == null) return;

            for (int i = 0; i < resultSet.recordSet.records.Count; i++)
            {
                try
                {
                    EditorGUILayout.LabelField((i + 1).ToString());

                    CommonFun.BeginLayout();

                    var record = resultSet.recordSet.records[i];

                    for (int j = 0; j < record.fieldValues.Count; j++)
                    {
                        var field = record.fieldSet.fields[j];

                        UICommonFun.BeginHorizontal(j);

                        EditorGUILayout.PrefixLabel(field.name);
                        EditorGUILayout.TextField(record[j].Value<string>(), GUILayout.ExpandWidth(true));

                        UICommonFun.EndHorizontal();
                    }
                }
                finally
                {
                    CommonFun.EndLayout();
                }
            }
        }
    }
}
