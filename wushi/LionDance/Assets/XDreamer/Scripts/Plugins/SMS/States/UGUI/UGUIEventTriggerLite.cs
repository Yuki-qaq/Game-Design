using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXGUI;

namespace XCSJ.PluginSMS.States.UGUI
{
    /// <summary>
    /// UGUI事件触发器简版:UGUI事件触发器是UGUI对象发生指定事件的触发器。事件包括UGUI元素上移入、移出和点击等，事件发生后，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSCategory.UGUIDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(UGUIEventTriggerLite))]
    [XCSJ.Attributes.Icon(EIcon.UIEvent, index = 33611)]
    [Tip("UGUI事件触发器是UGUI对象发生指定事件的触发器。事件包括UGUI元素上移入、移出和点击等，事件发生后，组件切换为完成态。", "Ugui event trigger is the trigger of the specified event of ugui object. Events include move in, move out and click on ugui elements. After the event occurs, the component switches to the completed state.")]
    public class UGUIEventTriggerLite : Trigger<UGUIEventTriggerLite>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "UGUI事件触发器简版";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.UGUI, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.UGUIDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(UGUIEventTriggerLite))]
        [Tip("UGUI事件触发器是UGUI对象发生指定事件的触发器。事件包括UGUI元素上移入、移出和点击等，事件发生后，组件切换为完成态。", "Ugui event trigger is the trigger of the specified event of ugui object. Events include move in, move out and click on ugui elements. After the event occurs, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateUGUIEventTriggerLite(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// UGUI对象
        /// </summary>
        [Name("UGUI对象")]
        [ComponentPopup()]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public RectTransform rectTransform;

        /// <summary>
        /// 触发事件类型(简版)
        /// </summary>
        [Name("触发事件类型(简版)")]
        [EnumPopup]
        [Readonly(EEditorMode.Runtime)]
        public EEventTriggerTypeLite eventTriggerTypeLite;

        /// <summary>
        /// 事件触发器
        /// </summary>
        protected EventTrigger eventTrigger;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            if (rectTransform)
            {
                eventTrigger = rectTransform.gameObject.AddComponent<EventTrigger>();

                XGUIHelper.AddEventTrigger(eventTrigger, eventTriggerTypeLite.ToEventTriggerType(), OnEventTrigger);
            }

            return base.Init(data);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="deleteObject"></param>
        /// <returns></returns>
        public override bool Delete(bool deleteObject)
        {
            if (eventTrigger) GameObject.Destroy(eventTrigger);

            return base.Delete(deleteObject);
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            if (eventTrigger) eventTrigger.enabled = true;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            if (eventTrigger) eventTrigger.enabled = false;
            base.OnExit(data);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return rectTransform;
        }

        /// <summary>
        /// 当事件触发器
        /// </summary>
        /// <param name="eventData"></param>
        protected virtual void OnEventTrigger(BaseEventData eventData)
        {
            finished = true;
        }
    }
}
