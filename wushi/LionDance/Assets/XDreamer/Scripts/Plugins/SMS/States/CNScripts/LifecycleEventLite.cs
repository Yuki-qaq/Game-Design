using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.Extension;
using XCSJ.PluginCNScripts;

namespace XCSJ.PluginSMS.States.CNScripts
{
    /// <summary>
    /// 生命周期事件简版:使用中文脚本编写控制逻辑
    /// </summary>
    [Serializable]
    [Name(Title, nameof(LifecycleEventLite))]
    [Tip("使用中文脚本编写控制逻辑", "Use Chinese script to write control logic")]
    [XCSJ.Attributes.Icon(EIcon.CNScript)]
    [ComponentMenu(CNScriptCategory.TitleDirectory + Title, typeof(ScriptManager))]
    public class LifecycleEventLite : StateScriptComponent<LifecycleEventLite, ELifecycleEventLite, LifecycleEventLiteFunction, LifecycleEventLiteFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "生命周期事件简版";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(LifecycleEventLite))]
        [Tip("使用中文脚本编写控制逻辑", "Use Chinese script to write control logic")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(CommonCategory.CommonUse, typeof(ScriptManager))]
        [StateComponentMenu(CommonCategory.CommonUseDirectory + Title, typeof(ScriptManager))]
        [StateLib(CNScriptCategory.Title, typeof(ScriptManager))]
        [StateComponentMenu(CNScriptCategory.TitleDirectory + Title, typeof(ScriptManager))]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _funcCollection.EnsureFunctionExist(ELifecycleEventLite.OnEntry).Enable = true;
            _funcCollection.EnsureFunctionExist(ELifecycleEventLite.OnExit).Enable = true;
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            ExecuteScriptEvent(ELifecycleEventLite.OnEntry);
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);
            ExecuteScriptEvent(ELifecycleEventLite.OnUpdate);
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);
            ExecuteScriptEvent(ELifecycleEventLite.OnExit);
        }
    }
}
