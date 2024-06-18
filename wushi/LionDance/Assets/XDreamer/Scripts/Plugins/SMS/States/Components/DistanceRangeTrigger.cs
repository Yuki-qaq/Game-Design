using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 距离区间触发器:距离区间触发器组件是游戏对象与相机（或游戏对象）之间符合某种空间位置关系的触发器。当游戏对象位置与当前相机之间的距离，符合设定空间关系，组件切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu(SMSCategory.ComponentDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(DistanceRangeTrigger))]
    [Tip("距离区间触发器组件是游戏对象与相机（或游戏对象）之间符合某种空间位置关系的触发器。当游戏对象位置与当前相机之间的距离，符合设定空间关系，组件切换为完成态。", "The distance interval trigger component is a trigger that conforms to a certain spatial position relationship between the game object and the camera (or game object). When the distance between the game object position and the current camera conforms to the set spatial relationship, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Distance)]
    public class DistanceRangeTrigger : Trigger<DistanceRangeTrigger>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "距离区间触发器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.Component, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.ComponentDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(DistanceRangeTrigger))]
        [Tip("距离区间触发器组件是游戏对象与相机（或游戏对象）之间符合某种空间位置关系的触发器。当游戏对象位置与当前相机之间的距离，符合设定空间关系，组件切换为完成态。", "The distance interval trigger component is a trigger that conforms to a certain spatial position relationship between the game object and the camera (or game object). When the distance between the game object position and the current camera conforms to the set spatial relationship, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EIcon.Distance)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 触发器游戏对象
        /// </summary>
        [Name("触发器游戏对象")]
        [Tip("待检测的触发器游戏对象;一般为静止不动的游戏对象;", "Trigger game object to be detected; It is generally a stationary game object;")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject triggerGameObject;

        /// <summary>
        /// 距离区间
        /// </summary>
        [Name("距离区间")]
        [LimitRange(0, 10000)]
        public Vector2 distanceRange = new Vector2(1, 100);

        /// <summary>
        /// 触发类型
        /// </summary>
        [Name("触发类型")]
        public enum ETriggerType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            [Tip("无触发类型，即一直不满足触发条件；", "There is no trigger type, that is, the trigger conditions are not met all the time;")]
            None = 0,

            /// <summary>
            /// 左
            /// </summary>
            [Name("左")]
            Left = 1 << 0,

            /// <summary>
            /// 中
            /// </summary>
            [Name("中")]
            Middle = 1 << 1,

            /// <summary>
            /// 右
            /// </summary>
            [Name("右")]
            Right = 1 << 2,

            /// <summary>
            /// 左到中
            /// </summary>
            [Name("左到中")]
            LToM = 1 << 3,

            /// <summary>
            /// 中到右
            /// </summary>
            [Name("中到右")]
            MToR = 1 << 4,

            /// <summary>
            /// 左到右
            /// </summary>
            [Name("左到右")]
            LToR = 1 << 5,

            /// <summary>
            /// 右到中
            /// </summary>
            [Name("右到中")]
            RToM = 1 << 6,

            /// <summary>
            /// 中到左
            /// </summary>
            [Name("中到左")]
            MToL = 1 << 7,

            /// <summary>
            /// 右到左
            /// </summary>
            [Name("右到左")]
            RToL = 1 << 8,

            /// <summary>
            /// 左到中右
            /// </summary>
            [Name("左到中右")]
            LToMR = LToM | LToR,

            /// <summary>
            /// 中右到左
            /// </summary>
            [Name("中右到左")]
            MRToL = MToL | RToL,

            /// <summary>
            /// 中到左右
            /// </summary>
            [Name("中到左右")]
            MToLR = MToL | MToR,

            /// <summary>
            /// 左右到中
            /// </summary>
            [Name("左右到中")]
            LRToM = LToM | RToM,

            /// <summary>
            /// 右到左中
            /// </summary>
            [Name("右到左中")]
            RToLM = RToL | RToM,

            /// <summary>
            /// 左中到右
            /// </summary>
            [Name("左中到右")]
            LMToR = LToR | MToR,

            /// <summary>
            /// 左右到右左
            /// </summary>
            [Name("左右到右左")]
            LRToRL = LToR | RToL,

            /// <summary>
            /// 左中右到左中右
            /// </summary>
            [Name("左中右到左中右")]
            LMRToLMR = LRToM| MToLR| LRToRL,

            /// <summary>
            /// 左中
            /// </summary>
            [Name("左中")]
            LM = Left | Middle,

            /// <summary>
            /// 中右
            /// </summary>
            [Name("中右")]
            MR = Middle | Right,

            /// <summary>
            /// 左中右
            /// </summary>
            [Name("左中右")]
            LMR = Left | Middle | Right,

            /// <summary>
            /// 任意
            /// </summary>
            [Name("任意")]
            [Tip("任意触发类型，即一直满足触发条件；", "Any trigger type, that is, the trigger conditions are always met;")]
            Any = LMRToLMR | LMR,
        }

        /// <summary>
        /// 触发类型
        /// </summary>
        [Name("触发类型")]
        [EnumPopup]
        public ETriggerType triggerType = ETriggerType.RToLM;

        /// <summary>
        /// 当前主相机
        /// </summary>
        [Name("当前主相机")]
        public bool currentMainCamera = true;

        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [Tip("实时检测的游戏对象；一般为可发生移动的游戏对象;", "Game objects detected in real time; Generally, it is a movable game object;")]
        [HideInSuperInspector(nameof(currentMainCamera), EValidityCheckType.True)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject gameObject;

        private Transform triggerTransform;

        private Transform transform;

        private ETriggerType lastTriggerType = ETriggerType.None;

        private ETriggerType GetTriggerType(float distance)
        {
            if (distance < distanceRange.x) return ETriggerType.Left;
            if (distance > distanceRange.y) return ETriggerType.Right;
            return ETriggerType.Middle;
        }

        private ETriggerType GetTriggerType(Vector3 a, Vector3 b)
        {
            return GetTriggerType((a - b).magnitude);
        }

        private ETriggerType GetTriggerType(ETriggerType last, ETriggerType current)
        {
            if (last == current) return current;
            switch(last)
            {
                case ETriggerType.Left:
                    {
                        switch (current)
                        {
                            case ETriggerType.Middle: return ETriggerType.LToM;
                            case ETriggerType.Right:return ETriggerType.LToR;
                        }
                        break;
                    }
                case ETriggerType.Middle:
                    {
                        switch (current)
                        {
                            case ETriggerType.Left: return ETriggerType.MToL;
                            case ETriggerType.Right: return ETriggerType.MToR;
                        }
                        break;
                    }
                case ETriggerType.Right:
                    {
                        switch (current)
                        {
                            case ETriggerType.Left: return ETriggerType.RToL;
                            case ETriggerType.Middle: return ETriggerType.RToM;
                        }
                        break;
                    }
            }
            return ETriggerType.None;
        }

        private void CheckTransform()
        {
            triggerTransform = triggerGameObject ? triggerGameObject.transform : null;
            transform = currentMainCamera ? (Camera.main ? Camera.main.transform : null) : (gameObject ? gameObject.transform : null);

            if (triggerTransform && transform)
            {
                lastTriggerType = GetTriggerType(triggerTransform.position, transform.position);
            }
            else
            {
                lastTriggerType = ETriggerType.None;
            }
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            CheckTransform();            
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);
            if (finished) return;
            switch (triggerType)
            {
                case ETriggerType.None: break;
                case ETriggerType.LMR:
                case ETriggerType.Any:
                    {
                        finished = true;
                        break;
                    }
                default:
                    {
                        if (triggerTransform && transform)
                        {
                            var current = GetTriggerType(triggerTransform.position, transform.position);
                            
                            if ((GetTriggerType(lastTriggerType, current) & triggerType) != ETriggerType.None) finished = true;

                            lastTriggerType = current;
                        }
                        else
                        {
                            CheckTransform();
                        }
                        break;
                    }
            }            
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return triggerGameObject && (currentMainCamera || gameObject);
        }
    }
}
