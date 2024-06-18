using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXGUI.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.Views.Toggles
{
    /// <summary>
    /// 切换交互执行器
    /// </summary>
    [Name("切换交互执行器")]
    [XCSJ.Attributes.Icon(EIcon.Toggle)]
    [Tip("通过切换（Toggle）触发执行交互", "Trigger execution interaction by toggling")]
    [Tool(XGUICategory.Component, nameof(XGUIManager))]
    public class ToggleInteractExecuter : View
    {
        /// <summary>
        /// 切换
        /// </summary>
        [Name("切换")]
        [Tip("如当前参数无效，会从当前组件所在游戏对象上查找当前参数类型的组件", "If the current parameter is invalid, it will find the component of the current parameter type from the game object where the current component is located")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Toggle _toggle = null;

        /// <summary>
        /// 切换触发器
        /// </summary>
        public class ToggleTrigger
        {
            /// <summary>
            /// toggle触发类型
            /// </summary>
            [Name("toggle触发类型")]
            [EnumPopup]
            public EToggleTriggerType _toggleTriggerType = EToggleTriggerType.SwitchOn;

            /// <summary>
            /// 是否触发
            /// </summary>
            /// <param name="isSwitch"></param>
            /// <param name="isToggleOn"></param>
            /// <returns></returns>
            public bool IsTrigger(bool isSwitch, bool isToggleOn)
            {
                switch (_toggleTriggerType)
                {
                    case EToggleTriggerType.On: return isToggleOn;
                    case EToggleTriggerType.Off: return !isToggleOn;
                    case EToggleTriggerType.Switch: return isSwitch;
                    case EToggleTriggerType.SwitchOn: return isSwitch && isToggleOn;
                    case EToggleTriggerType.SwitchOff: return isSwitch && !isToggleOn;
                }
                return false;
            }
        }

        #region 游戏对象激活

        /// <summary>
        /// 切换激活游戏对象
        /// </summary>
        [Serializable]
        public class ToggleActiveGameObject : ToggleTrigger
        {
            /// <summary>
            /// 激活操作
            /// </summary>
            [Name("激活操作")]
            [EnumPopup]
            public EBool _activeOperation = EBool.None;

            /// <summary>
            /// 游戏对象列表
            /// </summary>
            [Name("游戏对象列表")]
            [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
            public List<GameObject> _gameObjects = new List<GameObject>();

            /// <summary>
            /// 构造函数
            /// </summary>
            public ToggleActiveGameObject() { }

            /// <summary>
            /// 激活游戏对象
            /// </summary>
            public void ActiveGameObject()
            {
                foreach (var go in _gameObjects)
                {
                    go.XSetActive(_activeOperation);
                }
            }
        }

        /// <summary>
        /// 触发激活游戏对象列表
        /// </summary>
        [Name("触发激活游戏对象列表")]
        public List<ToggleActiveGameObject> _toggleActiveGameObjects = new List<ToggleActiveGameObject>();

        #endregion

        #region 交互执行

        /// <summary>
        /// 执行交互规则
        /// </summary>
        public enum EExecuteInteractRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 当启用
            /// </summary>
            [Name("当启用")]
            OnEnable,

            /// <summary>
            /// 当触发
            /// </summary>
            [Name("当触发")]
            OnTrigger,

            /// <summary>
            /// 当启用或触发
            /// </summary>
            [Name("当启用或触发")]
            OnEnableOrTrigger,
        }

        /// <summary>
        /// 执行交互规则
        /// </summary>
        [Name("执行交互规则")]
        [EnumPopup]
        public EExecuteInteractRule _executeInteractRule = EExecuteInteractRule.OnTrigger;

        /// <summary>
        /// 切换执行交互
        /// </summary>
        [Serializable]
        public class ToggleExecuteInteract : ToggleTrigger
        {
            /// <summary>
            /// 交互信息
            /// </summary>
            [Name("交互信息")]
            public ExecuteInteractInfo _executeInteractInfo = new ExecuteInteractInfo();

            /// <summary>
            /// 执行交互
            /// </summary>
            public void ExecuteInteract(object cmdParam = null) => _executeInteractInfo.TryInteract(cmdParam);
        }

        /// <summary>
        /// 交互执行列表
        /// </summary>
        [Name("交互执行列表")]
        public List<ToggleExecuteInteract> _executeInteracts = new List<ToggleExecuteInteract>();

        #endregion

        private Toggle toggleOflistened = null;


        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _toggle = GetComponent<Toggle>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_toggle)
            {
                toggleOflistened = _toggle;

                toggleOflistened.onValueChanged.AddListener(OnValueChanged);

                switch (_executeInteractRule)
                {
                    case EExecuteInteractRule.OnEnable:
                    case EExecuteInteractRule.OnEnableOrTrigger:
                        {
                            foreach (var item in _executeInteracts)
                            {
                                item.ExecuteInteract(toggleOflistened.isOn);
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (toggleOflistened)
            {
                toggleOflistened.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (!toggleOflistened) return;

            ActiveGameObject(false, toggleOflistened.isOn);

            switch (_executeInteractRule)
            {
                case EExecuteInteractRule.OnTrigger:
                case EExecuteInteractRule.OnEnableOrTrigger:
                    ExecuteInteract(false, toggleOflistened.isOn);
                    break;
            }
        }

        /// <summary>
        /// 切换值变更事件
        /// </summary>
        /// <param name="value"></param>
        protected void OnValueChanged(bool value)
        {
            ActiveGameObject(true, value);

            switch (_executeInteractRule)
            {
                case EExecuteInteractRule.OnTrigger:
                case EExecuteInteractRule.OnEnableOrTrigger:
                    ExecuteInteract(true, value);
                    break;
            }
        }

        private void ActiveGameObject(bool isSwitch, bool isToggleOn)
        {
            foreach (var item in _toggleActiveGameObjects)
            {
                if (item.IsTrigger(isSwitch, isToggleOn))
                {
                    item.ActiveGameObject();
                }
            }
        }

        private void ExecuteInteract(bool isSwitch, bool isToggleOn)
        {
            var scriptManager = ScriptManager.instance;

            // 执行交互
            foreach (var item in _executeInteracts)
            {
                if (item.IsTrigger(isSwitch, isToggleOn))
                {
                    // 引用变量赋值
                    if (scriptManager)
                    {
                        scriptManager.SetSystemVariableValues(this.gameObject.GameObjectToString());
                        scriptManager.TrySetOrAddSetHierarchyVarValue(this.referenceObjectVarString, this);
                    }

                    item.ExecuteInteract(isToggleOn);
                }
            }
        }
    }
}
