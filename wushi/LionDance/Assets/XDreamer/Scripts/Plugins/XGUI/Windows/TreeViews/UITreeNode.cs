using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXAR.Foundation.Images.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Windows.ListViews;

namespace XCSJ.PluginXGUI.Windows.TreeViews
{
    /// <summary>
    /// 树节点
    /// </summary>
    [Name("树节点")]
    [DisallowMultipleComponent]
    public class UITreeNode : View, ITreeNodeGraph
    {
        /// <summary>
        /// 折叠切换
        /// </summary>
        [Name("折叠切换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Toggle expandedToggle;

        /// <summary>
        /// 显示文本信息
        /// </summary>
        [Name("显示文本信息")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text dsplayInfoText;

        /// <summary>
        /// 背景点击按钮
        /// </summary>
        [Name("背景点击按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Button backgroundButton;

        /// <summary>
        /// 图像
        /// </summary>
        [Name("图像")]
        public Image _image;

        #region 树相关数据

        private UITreeView treeView;

        private ITreeNodeGraph data;

        private UITreeNode parentNode;

        private List<UITreeNode> childrenNodes = new List<UITreeNode>();

        /// <summary>
        /// 索引
        /// </summary>
        public int index { get; set; } = 0;

        #endregion

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            if (parentNode)
            {
                parentNode.childrenNodes.Add(this);
            }

            //if (dsplayInfoText)
            //{
            //    dsplayInfoText.text = displayName;
            //}

            if (backgroundButton)
            {
                backgroundButton.onClick.AddListener(OnClick);
            }

            if (expandedToggle)
            {
                expandedToggle.onValueChanged.AddListener(OnExpanded);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (data == null || treeView==null ) return;

            if (dsplayInfoText)
            {
                dsplayInfoText.text = displayName;
            }

            // 树项选择状态不一致
            if (backgroundButton && selected != data.selected)
            {
                selected = data.selected;
                if (selected)
                {
                    XGUIHelper.SetColor(backgroundButton, treeView.selectedColor);

                    // 只响应叶子节点滚动
                    if (childrenNodes.Count == 0)
                    {
                        treeView.AutoMoveScrollBar(transform.GetSiblingIndex());
                    }                    
                }
                else
                {
                    XGUIHelper.SetColor(backgroundButton, treeView.unselectedColor);
                }
            }

            // 检查接口数据和toggle状态，让它们保持一致
            if (expandedToggle && this.expanded != expandedToggle.isOn)
            {
                expandedToggle.isOn = this.expanded;
            }

            // 设置精灵
            if (_image && !_image.sprite && data is ITexture2D texture2D && texture2D.texture2D)
            {
                _image.sprite = texture2D.texture2D.ToSprite();
            }
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public void ResetData()
        {
            XGUIHelper.SetColor(backgroundButton, treeView.unselectedColor);
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="data"></param>
        /// <param name="parentNode"></param>
        /// <param name="index"></param>
        public void InitData(UITreeView treeView, ITreeNodeGraph data, UITreeNode parentNode, int index)
        {
            this.treeView = treeView;
            this.data = data;
            this.parentNode = parentNode;
            this.index = index;

            if (expandedToggle)
            {
                MoveRight(expandedToggle.transform);
                if (data.children.Length==0)
                {
                    expandedToggle.gameObject.SetActive(false);
                }
            }

            if (dsplayInfoText)
            {
                MoveRight(dsplayInfoText.transform);
            }

            OnExpanded(true);
            ResetData();
        }     
        
        /// <summary>
        /// 向右缩进
        /// </summary>
        private void MoveRight(Transform moveTransform)
        {
            float indent = treeView ? treeView.indent : 0;
            moveTransform.Translate(indent * (depth - 1), 0, 0);
        }

        /// <summary>
        /// 获取索引
        /// </summary>
        /// <returns></returns>
        public string GetIndex()
        {
            return (parentNode ? parentNode.GetIndex() + "." : "") + index;
        }

        /// <summary>
        /// 当展开
        /// </summary>
        /// <param name="expanded"></param>
        public void OnExpanded(bool expanded)
        {
            this.expanded = expanded;
            if (expandedToggle && expandedToggle.image && treeView)
            {
                expandedToggle.image.sprite = this.expanded ? treeView._expandIcon : treeView._unexpandIcon;
            }
            SetChildExpanded(this.expanded);
        }

        /// <summary>
        /// 设置子级展开
        /// </summary>
        /// <param name="expanded"></param>
        public void SetChildExpanded(bool expanded)
        {
            childrenNodes.ForEach(n =>
            {
                // 父展开，子随着自己属性控制孙节点
                if (expanded)
                {
                    n.SetChildExpanded(n.expanded);
                }
                // 父折叠，子孙必须折叠
                else
                {
                    n.SetChildExpanded(expanded);
                }
                n.gameObject.SetActive(expanded);
            });
        }

        #region 树节点接口

        /// <summary>
        /// 显示
        /// </summary>
        public GUIContent display => null;

        /// <summary>
        /// 启用
        /// </summary>
        public bool enable { get; set; } = true;

        /// <summary>
        /// 可视
        /// </summary>
        public bool visible { get; set; } = true;

        /// <summary>
        /// 深度
        /// </summary>
        public int depth => data.depth;

        /// <summary>
        /// 展开
        /// </summary>
        public bool expanded { get { return data.expanded;} set { data.expanded = value; } }

        /// <summary>
        /// 显示名
        /// </summary>
        public string displayName => (treeView.useIndex) ? (GetIndex()+ treeView.separatorString + data?.displayName) : data?.displayName;

        ITreeNodeGraph ITreeNodeGraph.parent => parentNode;

        ITreeNodeGraph[] ITreeNodeGraph.children => childrenNodes.ToArray();

        ITreeNode ITreeNode.parent => parentNode;

        ITreeNode[] ITreeNode.children => childrenNodes.ToArray();

        /// <summary>
        /// 选择
        /// </summary>
        public bool selected { get; set; } = false;

        /// <summary>
        /// 当点击
        /// </summary>
        public void OnClick()
        {
            data?.OnClick();
        }

        #endregion
    }

    /// <summary>
    /// 贴图接口
    /// </summary>
    public interface ITexture2D
    {
        /// <summary>
        /// 贴图
        /// </summary>
        Texture2D texture2D { get; set; }
    }
}

