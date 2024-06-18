using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.GameObjects;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 碰撞体点击批量:碰撞体点击批量组件是多个Unity碰撞体之中任意一个发生点击事件的触发器。当鼠标点击若干游戏对象中任意一个时，组件切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu(SMSCategory.ComponentDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(ColliderClickBatch))]
    [Tip("碰撞体点击批量组件是多个Unity碰撞体之中任意一个发生点击事件的触发器。当鼠标点击若干游戏对象中任意一个时，组件切换为完成态。", "Colliding body click batch component is the trigger of click event in any one of multiple unity colliding bodies. When the mouse clicks any one of several game objects, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(nameof(ColliderClick))]
    [RequireComponent(typeof(GameObjectSet))]
    public class ColliderClickBatch : Trigger<ColliderClickBatch>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "碰撞体点击批量";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.Component, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.ComponentDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(ColliderClickBatch))]
        [Tip("碰撞体点击批量组件是多个Unity碰撞体之中任意一个发生点击事件的触发器。当鼠标点击若干游戏对象中任意一个时，组件切换为完成态。", "Colliding body click batch component is the trigger of click event in any one of multiple unity colliding bodies. When the mouse clicks any one of several game objects, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateColliderClickBatch(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 点击配置
        /// </summary>
        [Name("点击配置")]
        [OnlyMemberElements]
        public ColliderClickHandle colliderClickHandle = new ColliderClickHandle();

        /// <summary>
        /// 变量
        /// </summary>
        [Name("变量")]
        [Tip("被点击碰撞体对象的保存变量", "Save variable of the clicked collider object")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string variable;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref variable);
        }

        #endregion

        /// <summary>
        /// 游戏对象集
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            colliderClickHandle.Init(gameObjectSet.objects);
            return base.Init(data);
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            colliderClickHandle.OnEntry();
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            if (finished) return;

            finished = colliderClickHandle.IsTrigger();
            if (finished && colliderClickHandle.lastClickGameObject)
            {
                variable.TrySetOrAddSetHierarchyVarValue(CommonFun.GameObjectToString(colliderClickHandle.lastClickGameObject));
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => gameObjectSet.objects.Count>0;

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return gameObjectSet.objects.Count.ToString();
        }
    }
}
