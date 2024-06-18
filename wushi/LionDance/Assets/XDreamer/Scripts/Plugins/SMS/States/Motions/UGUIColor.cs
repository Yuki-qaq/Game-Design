using UnityEngine;
using UnityEngine.UI;
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
    /// UGUI颜色:UGUI颜色组件是UGUI的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。
    /// </summary>
    [ComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(UGUIColor))]
    [Tip("UGUI颜色组件是UGUI的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。", "Ugui color component is the color gradient animation of ugui. The game object executes the change action of material color within the set time interval. After playing, the component switches to the completed state. If the game object has no material, the execution fails.")]
    [XCSJ.Attributes.Icon(EIcon.Color)]
    [RequireComponent(typeof(GameObjectSet))]
    public class UGUIColor : Motion<UGUIColor, UGUIColor.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "UGUI颜色";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.Action, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.ActionDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(UGUIColor))]
        [Tip("UGUI颜色组件是UGUI的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。", "Ugui color component is the color gradient animation of ugui. The game object executes the change action of material color within the set time interval. After playing, the component switches to the completed state. If the game object has no material, the execution fails.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
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
        public class Recorder : GraphicRecorder, IPercentRecorder<UGUIColor>
        {
            private UGUIColor uguiColor;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="uguiColor"></param>
            public void Record(UGUIColor uguiColor)
            {
                this.uguiColor = uguiColor;
                if (!uguiColor.gameObjectSet) return;
                _records.Clear();
                foreach (var go in uguiColor.gameObjectSet.objects)
                {
                    if (go)
                    {
                        if (uguiColor._includeChildren)
                        {
                            Record(go.GetComponentsInChildren<Graphic>());
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
                switch (uguiColor._operationType)
                {
                    case EOperationType.Color:
                        {
                            foreach (var i in _records)
                            {
                                i.SetPercent(percent, uguiColor._color);
                            }
                            break;
                        }
                    case EOperationType.Gradient:
                        {
                            var color = this.uguiColor._gradient.Evaluate((float)percent.percent01OfWorkCurve);
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
