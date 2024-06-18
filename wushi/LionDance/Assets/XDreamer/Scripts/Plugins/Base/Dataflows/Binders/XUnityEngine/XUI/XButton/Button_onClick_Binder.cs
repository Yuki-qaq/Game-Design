using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;

namespace XCSJ.Extension.Base.Dataflows.Binders.XUnityEngine.XUI.XButton
{
    /// <summary>
    /// 按钮点击绑定器：在当前绑定模式下，源属性为命令，点击按钮会向源对象发送命令
    /// </summary>
    [Name("按钮点击")]
    [DataBinder(typeof(Button), nameof(Button.onClick))]
    public class Button_onClick_Binder : TypeMemberDataBinder<Button>
    {
    }
}
