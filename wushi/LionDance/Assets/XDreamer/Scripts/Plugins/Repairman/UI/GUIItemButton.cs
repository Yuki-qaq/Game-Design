using UnityEngine;
using UnityEngine.UI;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginRepairman.UI
{
    /// <summary>
    /// GUI项按钮
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class GUIItemButton : GUIItem
    {
        private Button button;

        private bool selected;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            button = GetComponent<Button>();
            if (button)
            {
                button.onClick.AddListener(OnButtonClick);
            }

            selected = false;
        }

        /// <summary>
        /// 当销毁
        /// </summary>
        protected void OnDestroy()
        {
            if (button)
            {
                button.onClick.RemoveListener(OnButtonClick);
            }
        }

        /// <summary>
        /// 当按钮点击
        /// </summary>
        protected abstract void OnButtonClick();

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (item.selected != selected)
            {
                selected = item.selected;
                button.SetColor(selected ? guiItemList.selectedColor : guiItemList.unselectedColor);
            }
        }

        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="texture2D"></param>
        protected override void SetIcon(Texture2D texture2D)
        {
            if (texture2D)
            {
                button.GetComponent<Image>().sprite = XGUIHelper.ToSprite(item.texture2D);
            }
        }
    }
}
