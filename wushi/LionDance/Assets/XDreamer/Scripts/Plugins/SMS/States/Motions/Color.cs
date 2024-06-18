using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 颜色：颜色组件是游戏对象的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。
    /// </summary>
    [ComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(Color))]
    [Tip("颜色组件是游戏对象的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。", "The color component is the color gradient animation of the game object. The game object executes the change action of material color within the set time interval. After playing, the component switches to the completed state. If the game object has no material, the execution fails.")]
    [XCSJ.Attributes.Icon(EIcon.Color)]
    [RequireComponent(typeof(GameObjectSet))]
    public class Color : Motion<Color, Color.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "颜色";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.Action, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(Color))]
        [Tip("颜色组件是游戏对象的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。", "The color component is the color gradient animation of the game object. The game object executes the change action of material color within the set time interval. After playing, the component switches to the completed state. If the game object has no material, the execution fails.")]
        [XCSJ.Attributes.Icon(EIcon.Color)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 游戏对象集
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        /// <summary>
        /// 操作类型
        /// </summary>
        public enum EOperationType
        {
            /// <summary>
            /// 颜色
            /// </summary>
            [Name("颜色")]
            Color,

            /// <summary>
            /// 渐变色
            /// </summary>
            [Name("渐变色")]
            Gradient,
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        [Name("操作类型")]
        [EnumPopup]
        public EOperationType _operationType = EOperationType.Color;

        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        [HideInSuperInspector(nameof(_operationType), EValidityCheckType.NotEqual, EOperationType.Color)]
        public UnityEngine.Color _color = UnityEngine.Color.green;

        /// <summary>
        /// 渐变色
        /// </summary>
        [Name("渐变色")]
        [HideInSuperInspector(nameof(_operationType), EValidityCheckType.NotEqual, EOperationType.Gradient)]
        public Gradient _gradient = new Gradient();

        /// <summary>
        /// 包含成员
        /// </summary>
        [Name("包含成员")]
        public bool _includeChildren = true;

        /// <summary>
        /// 记录器
        /// </summary>
        public class Recorder : RendererRecorder, IPercentRecorder<Color>
        {
            private Color color;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="color"></param>
            public void Record(Color color)
            {
                this.color = color;
                if (!color.gameObjectSet) return;
                _records.Clear();
                foreach (var go in color.gameObjectSet.objects)
                {
                    if (go)
                    {
                        if (color._includeChildren)
                        {
                            Record(go.GetComponentsInChildren<Renderer>());
                        }
                        else
                        {
                            Record(go);
                        }
                    }
                }
            }

            /// <summary>
            /// 设置百分比
            /// </summary>
            /// <param name="percent"></param>
            public void SetPercent(Percent percent)
            {
                switch (color._operationType)
                {
                    case EOperationType.Color:
                        {
                            foreach (var i in _records)
                            {
                                i.SetPercent(percent, color._color);
                            }
                            break;
                        }
                    case EOperationType.Gradient:
                        {
                            var color = this.color._gradient.Evaluate((float)percent.percent01OfWorkCurve);
                            foreach (var i in _records)
                            {
                                i.SetColor(color);
                            }
                            break;
                        }
                }

            }
        }
    }
}
