using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络变量
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    [DisallowMultipleComponent]
    [Name("网络变量")]
    [Tool(MMOHelper.CategoryName, nameof(NetIdentity), rootType = typeof(MMOManager))]
    public class NetVariable : NetMB
    {
        /// <summary>
        /// 变量字符串
        /// </summary>
        [Name("变量字符串")]
        [VarString]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _varString = "";

        /// <summary>
        /// 变量值
        /// </summary>
        [SyncVar]
        [Readonly]
        [Name("变量值")]
        public string _variableValue = "";

        /// <summary>
        /// 上一次变量值
        /// </summary>
        [Readonly]
        [Name("上一次变量值")]
        public string _lastVariableValue;

        /// <summary>
        /// 原始变量值
        /// </summary>
        [Readonly]
        [Name("原始变量值")]
        public string _originalVariableValue;

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            base.OnMMOEnterRoomCompleted(result);

            if (result == EACode.Success)
            {
                _originalVariableValue = _lastVariableValue = UpdateVariableValue();

                Variable.onValueChanged += OnVariableValueChanged;
            }
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            base.OnMMOExitRoomCompleted();

            Variable.onValueChanged -= OnVariableValueChanged;

            _varString.TrySetOrAddSetHierarchyVarValue(_originalVariableValue);
        }

        /// <summary>
        /// 当定时检查修改
        /// </summary>
        /// <returns></returns>
        protected override bool OnTimedCheckChange()
        {
            return _variableValue != _lastVariableValue;
        }

        bool isSyncModify = false;

        /// <summary>
        /// 当同步变量已修改
        /// </summary>
        protected override void OnSyncVarChanged()
        {
            base.OnSyncVarChanged();

            try
            {
                isSyncModify = true;
                _varString.TrySetOrAddSetHierarchyVarValue(_variableValue);
                _lastVariableValue = _variableValue;
            }
            finally
            {
                isSyncModify = false;
            }
        }
        
        /// <summary>
        /// 当变量值已修改
        /// </summary>
        /// <param name="variable"></param>
        private void OnVariableValueChanged(Variable variable)
        {
            if (!isSyncModify && VarStringAnalysisResult.TryParse(_varString, out var result) && result.rootVarString == variable.GetHierarchyVar().varString)
            {
                UpdateVariableValue();
            }
        }

        /// <summary>
        /// 更新变量值
        /// </summary>
        /// <returns></returns>
        private string UpdateVariableValue()
        {
            _varString.TryGetHierarchyVarValue(out var varValue);
            return _variableValue = varValue.ToScriptParamString();
        }
    }
}
