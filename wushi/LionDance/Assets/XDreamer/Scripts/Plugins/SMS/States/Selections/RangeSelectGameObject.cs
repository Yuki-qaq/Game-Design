using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.GameObjects;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Selections
{
    /// <summary>
    /// 区间选择游戏对象:区间选择游戏对象组件是在给定的时间内设置游戏对象是否选中的动画。播放完成后随即切换为完成态。
    /// </summary>
    [ComponentMenu(SMSCategory.SelectionSetDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(RangeSelectGameObject))]
    [Tip("区间选择游戏对象组件是在给定的时间内设置游戏对象是否选中的动画。播放完成后随即切换为完成态。", "The interval selection game object component is to set the animation of whether the game object is selected within a given time. After the playback is completed, it will be switched to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Select)]
    public class RangeSelectGameObject : WorkClip<RangeSelectGameObject>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "区间选择游戏对象";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.SelectionSet, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.SelectionSetDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(RangeSelectGameObject))]
        [Tip("区间选择游戏对象组件是在给定的时间内设置游戏对象是否选中的动画。播放完成后随即切换为完成态。", "The interval selection game object component is to set the animation of whether the game object is selected within a given time. After the playback is completed, it will be switched to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 使用游戏对象集
        /// </summary>
        [Group("游戏对象设置")]
        [Name("使用游戏对象集")]
        [Tip("游戏对象集组件存在时，使用游戏对象集", "When the GameObject set component exists, the GameObject set is used")]
        public bool useGameObjectSet = true;

        /// <summary>
        /// 包含子对象
        /// </summary>
        [Name("包含子对象")]
        public bool includeChildren = true;

        /// <summary>
        /// 自定义游戏对象
        /// </summary>
        [Name("自定义游戏对象")]
        public List<GameObject> customGameObjects = new List<GameObject>();

        /// <summary>
        /// 进入时
        /// </summary>
        [Group("选择设置")]
        [Name("进入时")]
        [EnumPopup]
        public EBool onEntry = EBool.No;

        /// <summary>
        /// 区间左
        /// </summary>
        [Name("区间左")]
        [EnumPopup]
        public EBool leftRange = EBool.No;

        /// <summary>
        /// 区间内
        /// </summary>
        [Name("区间内")]
        [EnumPopup]
        public EBool inRange = EBool.Yes;

        /// <summary>
        /// 区间右
        /// </summary>
        [Name("区间右")]
        [EnumPopup]
        public EBool rightRange = EBool.No;

        /// <summary>
        /// 退出时
        /// </summary>
        [Name("退出时")]
        [EnumPopup]
        public EBool onExit = EBool.No;

        private List<GameObject> operateGameObjects = new List<GameObject>();

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            InitGameObject();

            Select(onEntry);
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            Select(onExit);

            base.OnExit(data);
        }

        /// <summary>
        /// 当越界发生时回调；
        /// </summary>
        /// <param name="player">工作剪辑播放器对象</param>
        /// <param name="outOfBoundsMode">越界模式</param>
        /// <param name="percent">当前百分比</param>
        /// <param name="stateData">状态数据对象</param>
        /// <param name="lastPercent">上一次的百分比</param>
        /// <param name="stateWorkClip">状态工作剪辑对象</param>
        public override void OnOutOfBounds(IWorkClipPlayer player, EOutOfBoundsMode outOfBoundsMode, double percent, StateData stateData, double lastPercent, IStateWorkClip stateWorkClip)
        {
            switch (outOfBoundsMode)
            {
                case EOutOfBoundsMode.Left:
                    {
                        if (setPercentOnEntry)
                        {
                            SetPercent(percentOnEntry, stateData);
                        }
                        Select(onEntry);
                        break;
                    }
                case EOutOfBoundsMode.Right:
                    {
                        if (setPercentOnExit)
                        {
                            SetPercent(percentOnExit, stateData);
                        }
                        Select(onExit);
                        break;
                    }
            }
        }

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            if(percent.percent < 0)
            {
                Select(leftRange);
            }
            else if (percent.percent > 1)
            {
                Select(rightRange);
            }
            else
            {
                Select(inRange);
            }
        }

        private void InitGameObject()
        {
            operateGameObjects.Clear();

            // 将游戏对象集和自定义游戏对象列表， 整合到一个列表中
            var gameObjectList = new List<GameObject>();
            var gameObjectSet = GetComponent<GameObjectSet>();
            if (gameObjectSet)
            {
                gameObjectList.AddRange(gameObjectSet.objects);
            }
            gameObjectList.AddRange(customGameObjects);

            // 遍历对象列表和子对象
            gameObjectList.ForEach(go =>
            {
                if(go)
                {
                    operateGameObjects.Add(go);

                    if (includeChildren)
                    {
                        operateGameObjects.AddRange(CommonFun.GetChildGameObjects(go));
                    }
                }
            });
        }

        private void Select(EBool flag)
        {
            if (flag == EBool.None) return;
            
            operateGameObjects.ForEach(go=>
            {
                bool selected = CommonFun.BoolChange(Selection.Contains(go), flag);
                if (selected)
                {
                    Selection.AddWithDistinct(go);
                }
                else
                {
                    Selection.Remove(go);
                }
            });
        }
    }
}
