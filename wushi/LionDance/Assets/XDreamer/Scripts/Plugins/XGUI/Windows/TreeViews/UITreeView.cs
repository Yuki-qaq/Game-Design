using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.TreeViews
{
    /// <summary>
    /// 树视图
    /// </summary>
    [Name("树视图")]
    [DisallowMultipleComponent]
    public class UITreeView : View
    {
        /// <summary>
        /// 节点模版
        /// </summary>
        [Group("节点配置")]
        [Name("节点模版")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public UITreeNode templateNode;

        /// <summary>
        /// 节点展开图标
        /// </summary>
        [Name("节点展开图标")]
        public Sprite _expandIcon;

        /// <summary>
        /// 节点折叠图标
        /// </summary>
        [Name("节点折叠图标")]
        public Sprite _unexpandIcon;

        /// <summary>
        /// 自动滚动
        /// </summary>
        [Name("自动滚动")]
        [Tip("为了使选中项总是可见，需自动设定树型控件垂直滚动条！", "In order to make the selected item always visible, you need to automatically set the tree control vertical scroll bar!")]
        public bool autoScrollToShowSelectedNode = true;

        /// <summary>
        /// 垂直滚动条
        /// </summary>
        [Name("垂直滚动条")]
        [HideInSuperInspector(nameof(autoScrollToShowSelectedNode), EValidityCheckType.Equal, false)]
        [ComponentPopup]
        public Scrollbar scrollbar;

        /// <summary>
        /// 使用编号
        /// </summary>
        [Group("样式")]
        [Name("使用编号")]
        public bool useIndex = true;

        /// <summary>
        /// 编号分隔符
        /// </summary>
        [Name("编号分隔符")]
        [HideInSuperInspector(nameof(useIndex), EValidityCheckType.False)]
        public string separatorString = ".";

        /// <summary>
        /// 缩进
        /// </summary>
        [Name("缩进")]
        [Range(0,1000)]
        public float indent = 20;

        /// <summary>
        /// 选中颜色
        /// </summary>
        [Name("选中颜色")]
        public Color selectedColor = new Color(1, 0.6f, 0, 1);

        /// <summary>
        /// 未选中颜色
        /// </summary>
        [Name("未选中颜色")]
        public Color unselectedColor = Color.white;

        /// <summary>
        /// 数据
        /// </summary>
        public ITreeNodeGraph data;

        private WorkObjectPool<UITreeNode> _uiTreeNodePool;

        /// <summary>
        /// UI树视图节点池
        /// </summary>
        protected WorkObjectPool<UITreeNode> uiTreeNodePool
        {
            get
            {
                if (_uiTreeNodePool == null)
                {
                    _uiTreeNodePool = new WorkObjectPool<UITreeNode>();
                    uiTreeNodePool.Init(
                        () => {
                            if (templateNode)
                            {
                                var newGO = templateNode.gameObject.XCloneObject();
                                newGO.XSetParent(templateNode.transform.parent);
                                newGO.transform.localScale = templateNode.transform.localScale;
                                return newGO.GetComponent<UITreeNode>();
                            }
                            return default;
                        },
                        uiTreeNode => uiTreeNode.gameObject.SetActive(true),
                        uiTreeNode => uiTreeNode.ResetData(),
                        uiTreeNode => uiTreeNode
                        );
                }
                return _uiTreeNodePool;
            }
        }

        /// <summary>
        /// 网格布局组
        /// </summary>
        protected GridLayoutGroup gridLayoutGroup = null;

        /// <summary>
        /// 显示子级数量
        /// </summary>
        protected int showChildrenCount = 0 ;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            if (templateNode) templateNode.gameObject.SetActive(false);

            gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        public void Create()
        {
            if (data == null)
            {
                Debug.LogWarning("树形控件没有数据源!");
                return;
            }

            Create(data.children);

            if (gridLayoutGroup)
            {
                showChildrenCount = 0;
                for (int i = 0; i < gridLayoutGroup.transform.childCount; ++i)
                {
                    if (gridLayoutGroup.transform.GetChild(i).gameObject.activeSelf)
                    {
                        ++showChildrenCount;
                    }
                }
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="treeNodes"></param>
        /// <param name="parent"></param>
        protected void Create(ITreeNodeGraph[] treeNodes, UITreeNode parent = null)
        {
            for (int i = 0; i < treeNodes.Length; ++i)
            {
                Create(treeNodes[i], parent, i + 1);
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        protected void Create(ITreeNodeGraph node, UITreeNode parent, int index)
        {
            // 绘制根
            var uiTreeNode = uiTreeNodePool.Alloc();
            uiTreeNode.InitData(this, node, parent, index);
            uiTreeNode.transform.SetAsLastSibling();

            // 绘制子节点
            for (int i = 0; i < node.children.Length; ++i)
            {
                Create(node.children[i], uiTreeNode, i + 1);
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear() => uiTreeNodePool.Clear();

        /// <summary>
        /// 自动移动滚动条
        /// </summary>
        /// <param name="index"></param>
        public void AutoMoveScrollBar(int index)
        {
            if (autoScrollToShowSelectedNode && gridLayoutGroup && scrollbar)
            {
                // 为了显示步骤滚动条的值， 当滚动条比值大，才滚动， 滚动条值从1到0变化
                //XGUIHelper.GetScrollBarRange(index, showChildrenCount, rectTransform.sizeDelta.y, 
                //    gridLayoutGroup.cellSize.y, gridLayoutGroup.spacing.y, out float maxValue, out float minValue);

                //float value = Mathf.Clamp(scrollbar.value, minValue, maxValue);
                var value = 1.0f * (index - 1) / (showChildrenCount-1);

                if (scrollbar.direction == Scrollbar.Direction.BottomToTop)
                {
                    scrollbar.value = 1 - value;
                }
                else
                {
                    scrollbar.value = value;
                }
            }
        }
    }
}