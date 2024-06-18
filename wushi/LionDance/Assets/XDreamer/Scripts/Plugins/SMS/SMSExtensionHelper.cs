using XCSJ.Extension;

namespace XCSJ.PluginSMS
{
    /// <summary>
    /// 状态机系统分类
    /// </summary>
    public class SMSCategory
    {
        /// <summary>
        /// 状态机系统
        /// </summary>
        public const string SMS = "SMS";

        /// <summary>
        /// 状态机系统前缀
        /// </summary>
        public const string SMSPrefix = SMS + CommonCategory.HorizontalLine;

        #region UGUI

        /// <summary>
        /// UGUI
        /// </summary>
        public const string UGUI = SMSPrefix + CommonCategory.UGUI;

        /// <summary>
        /// UGUI目录
        /// </summary>
        public const string UGUIDirectory = UGUI + CommonCategory.PathSplitLine;

        #endregion

        #region 动作

        /// <summary>
        /// 动作
        /// </summary>
        public const string Action = SMSPrefix + "动作";

        /// <summary>
        /// 动作目录
        /// </summary>
        public const string ActionDirectory = Action + CommonCategory.PathSplitLine;

        #endregion

        #region 多媒体

        /// <summary>
        /// 多媒体
        /// </summary>
        public const string MultiMedia = SMSPrefix + CommonCategory.MultiMedia;

        /// <summary>
        /// 多媒体目录
        /// </summary>
        public const string MultiMediaDirectory = MultiMedia + CommonCategory.PathSplitLine;

        #endregion

        #region 展示

        /// <summary>
        /// 展示
        /// </summary>
        public const string Show = SMSPrefix + "展示";

        /// <summary>
        /// 展示目录
        /// </summary>
        public const string ShowDirectory = Show + CommonCategory.PathSplitLine;

        #endregion

        #region 数据流

        /// <summary>
        /// 数据流
        /// </summary>
        public const string DataFlow = SMSPrefix + "数据流";

        /// <summary>
        /// 数据流目录
        /// </summary>
        public const string DataFlowDirectory = DataFlow + CommonCategory.PathSplitLine;

        /// <summary>
        /// 数据流前缀
        /// </summary>
        public const string DataFlowPrefix = DataFlow + CommonCategory.HorizontalLine;

        /// <summary>
        /// 数据流-事件
        /// </summary>
        public const string DataFlowEvent = DataFlowPrefix + "事件";

        /// <summary>
        /// 数据流-事件目录
        /// </summary>
        public const string DataFlowEventDirectory = DataFlowEvent + CommonCategory.PathSplitLine;

        /// <summary>
        /// 数据流-属性绑定
        /// </summary>
        public const string DataFlowPropertyBind = DataFlowPrefix + "属性绑定";

        /// <summary>
        /// 数据流-属性绑定目录
        /// </summary>
        public const string DataFlowPropertyBindDirectory = DataFlowPropertyBind + CommonCategory.PathSplitLine;

        /// <summary>
        /// 数据流-数据模型
        /// </summary>
        public const string DataFlowDataModel = DataFlowPrefix + "数据模型";

        /// <summary>
        /// 数据流-数据模型目录
        /// </summary>
        public const string DataFlowDataModelDirectory = DataFlowDataModel + CommonCategory.PathSplitLine;

        #endregion

        #region 游戏对象

        /// <summary>
        /// 游戏对象
        /// </summary>
        public const string GameObject = SMSPrefix + CommonCategory.GameObject;

        /// <summary>
        /// 游戏对象目录
        /// </summary>
        public const string GameObjectDirectory = GameObject + CommonCategory.PathSplitLine;

        #endregion

        #region 状态操作

        /// <summary>
        /// 状态操作
        /// </summary>
        public const string StateOperation = SMSPrefix + "状态操作";

        /// <summary>
        /// 状态操作目录
        /// </summary>
        public const string StateOperationDirectory = StateOperation + CommonCategory.PathSplitLine;

        #endregion

        #region 组件操作

        /// <summary>
        /// 组件操作
        /// </summary>
        public const string Component = SMSPrefix + "组件操作";

        /// <summary>
        /// 组件操作目录
        /// </summary>
        public const string ComponentDirectory = Component + CommonCategory.PathSplitLine;

        #endregion

        #region 输入

        /// <summary>
        /// 输入
        /// </summary>
        public const string Input = SMSPrefix + "输入";

        /// <summary>
        /// 输入目录
        /// </summary>
        public const string InputDirectory = Input + CommonCategory.PathSplitLine;

        #endregion

        #region 选择集

        /// <summary>
        /// 选择集
        /// </summary>
        public const string SelectionSet = SMSPrefix + CommonCategory.SelectionSet;

        /// <summary>
        /// 选择集目录
        /// </summary>
        public const string SelectionSetDirectory = SelectionSet + CommonCategory.PathSplitLine;

        #endregion

        #region 其它

        /// <summary>
        /// 其它
        /// </summary>
        public const string Other = SMSPrefix + CommonCategory.Other;

        /// <summary>
        /// 其它目录
        /// </summary>
        public const string OtherDirectory = Other + CommonCategory.PathSplitLine; 

        #endregion
    }
}
