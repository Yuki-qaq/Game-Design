using XCSJ.Attributes;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginRepairman.Tools;
using XCSJ.PluginTools.SelectionUtils;
using Part = XCSJ.PluginRepairman.States.Part;

namespace XCSJ.PluginRepairman.UI
{
    /// <summary>
    /// 零件按钮界面
    /// </summary>
    [Name("零件按钮界面")]
    public class GUIPartButton : GUIItemButton
    {
        /// <summary>
        /// 零件
        /// </summary>
        protected Part part => item as Part;

        /// <summary>
        /// 更新
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (part && part.assembleState == EAssembleState.Assembled)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 当按钮点击
        /// </summary>
        protected override void OnButtonClick()
        {
            if (part && part.go)
            {
                LimitedSelection.SetSelected(part.go);
            }
        }
    }
}
