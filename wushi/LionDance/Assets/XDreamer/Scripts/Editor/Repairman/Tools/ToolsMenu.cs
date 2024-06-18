using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.EditorXGUI;
using XCSJ.EditorXGUI.Windows;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginRepairman;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginRepairman.States.Exam;
using XCSJ.PluginRepairman.States.RepairTask;
using XCSJ.PluginRepairman.UI;
using XCSJ.PluginRepairman.UI.Exam;
using XCSJ.PluginRepairman.UI.Study;
using XCSJ.PluginSMS.States.Show;
using XCSJ.PluginSMS.States.Show.UI;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Windows;

namespace XCSJ.EditorRepairman.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        #region 拆装步骤树

        /// <summary>
        /// 拆装步骤树名称
        /// </summary>
        public const string RepairStepTreeViewName = "拆装步骤列表";

        /// <summary>
        /// 创建拆装步骤树
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(RepairmanCategory.Step, nameof(RepairmanManager), nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(RepairStepTreeViewName)]
        [XCSJ.Attributes.Icon(EIcon.Task)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(RepairStepGroup))]
        public static void CreateUITaskWorkTreeView(ToolContext toolContext) => CreateUI(RepairStepTreeViewName);

        /// <summary>
        /// 创建计划步骤树
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("计划步骤树")]
        [XCSJ.Attributes.Icon(EIcon.Task)]
        [RequireManager(typeof(RepairmanManager))]
        //[Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true)]
        [Manual(typeof(Plan))]
        public static void CreateUIPlanTreeView(ToolContext toolContext)
        {
            var go = CreateUITreeView("计划步骤树界面");
            if (go)
            {
                go.XAddComponent<UITreeViewPlanData>();
            }
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        private static GameObject CreateUITreeView(string name)
        {
            var go = UITreeViewEditor.CreateUITreeView(EditorXGUIHelper.defaultResources);
            go.XSetName(name);
            return go;
        }

        #endregion

        #region 零件列表

        /// <summary>
        /// 零件列表名称
        /// </summary>
        public const string PartListName = "零件列表";

        /// <summary>
        /// 创建零件列表
        /// </summary>
        /// <param name="toolContext"></param>
        //[ToolAttribute(RepairmanCategory.Model, nameof(RepairmanManager), nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(PartListName)]
        [XCSJ.Attributes.Icon(nameof(Part))]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(RepairTaskWorkPartView))]
        public static void CreatePartList(ToolContext toolContext)
        {
            var go = CreatePartScrollView(EditorXGUIHelper.defaultResources);
            go.XSetName(PartListName);

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        private static GameObject CreatePartScrollView(DefaultControls.Resources resources)
        {
            try
            {
                var repairmanOption = RepairmanExtentionsOption.instance;
                var partList = EditorXGUIHelper.CreateScrollView<GUIPartList>(resources,
                    repairmanOption.partListSize, repairmanOption.partItemSize, repairmanOption.CellSpaceSize);

                //创建零件单元
                partList.itemButtonPrefab = CreatePartCell(resources);
                GameObjectUtility.SetParentAndAlign(partList.itemButtonPrefab,
                    partList.GetComponentInChildren<ContentSizeFitter>().gameObject);
                return partList.gameObject;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private static GameObject CreatePartCell(DefaultControls.Resources resources)
        {
            try
            {
                return DefaultControls.CreateButton(resources).AddComponent<GUIPartButton>().gameObject;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        #endregion

        #region 工具包

        /// <summary>
        /// 工具包名称
        /// </summary>
        public const string ToolBagName = "工具包";

        /// <summary>
        /// 创建工具包
        /// </summary>
        /// <param name="toolContext"></param>
        [ToolAttribute(RepairmanCategory.Model, nameof(RepairmanManager), nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(ToolBagName)]
        [XCSJ.Attributes.Icon(EIcon.Bag)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(XCSJ.PluginRepairman.UI.GUIToolList))]
        public static void CreateToolBag(ToolContext toolContext)
        {
            var go = CreateToolBagScrollView(EditorXGUIHelper.defaultResources);
            go.XSetName(ToolBagName);

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        private static GameObject CreateToolBagScrollView(DefaultControls.Resources resources)
        {
            try
            {
                var repairmanOption = RepairmanExtentionsOption.instance;
                var toolList = EditorXGUIHelper.CreateScrollView<GUIToolList>(resources,
                    repairmanOption.toolBagSize, repairmanOption.toolItemSize, repairmanOption.CellSpaceSize);

                //创建工具单元
                toolList.itemButtonPrefab = CreateToolCell(resources);
                GameObjectUtility.SetParentAndAlign(toolList.itemButtonPrefab,
                    toolList.GetComponentInChildren<ContentSizeFitter>().gameObject);
                return toolList.gameObject;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private static GameObject CreateToolCell(DefaultControls.Resources resources)
        {
            try
            {
                var toggleGO = DefaultControls.CreateToggle(resources);
                toggleGO.AddComponent<GUIItemToggle>();

                // 设置图标向四周扩展
                var bgRectTransform = toggleGO.transform.Find("Background").transform as RectTransform;
                bgRectTransform.XStretchHV();

                // 设置选中框向四周扩展
                var selectedRectTransform = toggleGO.transform.Find("Background/Checkmark").transform as RectTransform;
                selectedRectTransform.XStretchHV();

                // 设置文字属性
                var textRectTransform = toggleGO.GetComponentInChildren<Text>().transform as RectTransform;
                textRectTransform.offsetMin = new Vector2(0f, -20f);
                textRectTransform.offsetMax = new Vector2(0f, -60f);

                return toggleGO;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        #endregion

        #region 学习提示信息

        /// <summary>
        /// 学习提示信息名称
        /// </summary>
        public const string StudyTipInfoName = "学习提示信息";

        /// <summary>
        /// 创建学习提示信息
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(RepairmanCategory.Step, nameof(RepairmanManager), nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(StudyTipInfoName)]
        [XCSJ.Attributes.Icon(EIcon.Study)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(UIStudyTipInfo))]
        public static void CreateStudyTipInfo(ToolContext toolContext)
        {
            var go = DefaultControls.CreateText(EditorXGUIHelper.defaultResources);
            go.XAddComponent<UIStudyTipInfo>();
            go.XSetName(StudyTipInfoName);

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        #endregion

        #region 步骤索引

        /// <summary>
        /// 步骤索引名称
        /// </summary>
        public const string StepIndexName = "步骤索引";

        /// <summary>
        /// 创建步骤索引
        /// </summary>
        /// <param name="toolContext"></param>
        [Name(StepIndexName)]
        [XCSJ.Attributes.Icon(EIcon.List)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(GUICurrentStepIndex))]
        [ToolAttribute(RepairmanCategory.Step, nameof(RepairmanManager), nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        public static void CreateStepIndexUI(ToolContext toolContext) => CreateUI(StepIndexName);

        #endregion

        #region 答题表格

        /// <summary>
        /// 答题表格名称
        /// </summary>
        public const string QuestionTableName = "答题表格";

        /// <summary>
        /// 创建答题表格
        /// </summary>
        /// <param name="toolContext"></param>
        [ToolAttribute(RepairmanCategory.Step, nameof(RepairmanManager), nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(QuestionTableName)]
        [XCSJ.Attributes.Icon(EIcon.AnswerQuestion)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(UIQuestionTable))]
        public static void CreateExamUIQuestionTable(ToolContext toolContext) => CreateUI(QuestionTableName);

        #endregion

        #region 考试提示信息

        /// <summary>
        /// 考试提示信息名称
        /// </summary>
        public const string ExamTipInfoName = "考试提示信息";

        /// <summary>
        /// 创建考试提示
        /// </summary>
        /// <param name="toolContext"></param>
        [ToolAttribute(RepairmanCategory.Step, nameof(RepairmanManager), nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(ExamTipInfoName)]
        [XCSJ.Attributes.Icon(EIcon.Exam)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(UIExamInfo))]
        public static void CreateExamTipInfo(ToolContext toolContext)
        {
            var go = DefaultControls.CreateText(EditorXGUIHelper.defaultResources);
            go.XAddComponent<UIExamInfo>();
            go.XSetName(ExamTipInfoName);

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        #endregion

        #region 考试结果

        /// <summary>
        /// 考试结果名称
        /// </summary>
        public const string ExamResultName = "考试结果";

        /// <summary>
        /// 创建考试结果
        /// </summary>
        /// <param name="toolContext"></param>
        [ToolAttribute(RepairmanCategory.Step, nameof(RepairmanManager), nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name(ExamResultName)]
        [XCSJ.Attributes.Icon(EIcon.Exam)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(RepairExam))]
        public static void CreateExamResult(ToolContext toolContext) => CreateUI(ExamResultName);

        #endregion

        #region 零件列表

        /// <summary>
        /// 零件列表
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>       
        [Name("零件列表")]
        [XCSJ.Attributes.Icon(EIcon.List)]
        [Tool(RepairmanCategory.Model, nameof(RepairmanManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(RepairmanManager))]
        [Manual(typeof(Device))]
        public static void CreateLabelWindowTemplate(ToolContext toolContext)
        {
            MenuHelper.DrawMenu("零件列表", m =>
            {
                m.AddMenuItem("可拆卸零件列表", () => CreateUI("可拆卸零件列表"));
                m.AddMenuItem("可装配零件列表", () => CreateUI("可装配零件列表"));
                m.AddMenuItem("已拆卸零件列表", () => CreateUI("已拆卸零件列表"));
                m.AddMenuItem("已装配零件列表", () => CreateUI("已装配零件列表"));
            });
        }

        private static void CreateUI(string prefabName)
        {
            EditorXGUI.Tools.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(string.Format(@"{0}/{1}.prefab", RepairmanCategory.Title, prefabName)));
        }

        #endregion
    }
}

