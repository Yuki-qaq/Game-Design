using UnityEngine;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.EditorSMS.NodeKit.Nodes
{
    /// <summary>
    /// 状态组节点视图
    /// </summary>
    [NodeView(typeof(StateGroup))]
    public class StateGroupNodeView : ModelNodeView
    {
        /// <summary>
        /// 状态组
        /// </summary>
        public StateGroup stateGroup { get; private set; }

        /// <summary>
        /// 背景样式
        /// </summary>
        protected override GUIStyle backgroundStyle => customStyle.groupNodeStyle;

        /// <summary>
        /// 标题样式
        /// </summary>
        protected override GUIStyle titleStyle => customStyle.groupTitleBackgroundStyle;

        ///// <summary>
        ///// 当删除
        ///// </summary>
        ///// <returns></returns>
        //internal override bool OnDelete() => parentSubStateMachine.RemoveGroup(nodeModel as StateGroup);

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            if (nodeModel is StateGroup sg && sg)
            {
                stateGroup = sg;
            }
            else
            {
                Debug.LogErrorFormat("[{0}]模型数据必须是[{1}]类型！", typeof(StateGroupNodeView).FullName, typeof(StateGroup).FullName);
            }
        }

        /// <summary>
        /// 绘制内容背景
        /// </summary>
        protected override void DrawContentBackground()
        {
            //base.DrawContentBackground();
        }

        /// <summary>
        /// 当绘制小地图
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scaleValue"></param>
        public override void OnGUIMiniMap(Rect rect, float scaleValue)
        {
            // 不展开时绘制小地图
            if (!stateGroup.expand)
            {
                base.OnGUIMiniMap(rect, scaleValue);
            }
        }
    }
}
