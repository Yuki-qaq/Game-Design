using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Tweens;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.GameObjects;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.PluginSMS.States.MultiMedia
{
    /// <summary>
    /// 路径动画:路径动画组件是控制多个游戏对象沿着某条路径运动的动画。在第一个对象移动一段时间后，会控制第二个对象，接着第一个对象之后沿着路径运动。可用于模拟箭头流动的动画。播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSCategory.MultiMediaDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(PathAnimation))]
    [Tip("路径动画组件是控制多个游戏对象沿着某条路径运动的动画。在第一个对象移动一段时间后，会控制第二个对象，接着第一个对象之后沿着路径运动。可用于模拟箭头流动的动画。播放完毕后，组件切换为完成态。", "The path animation component is an animation that controls the movement of multiple game objects along a certain path. After the first object moves for a period of time, the second object is controlled, and then the first object moves along the path. Arrows that can be used to simulate flow. After playing, the component switches to the finished state.")]
    [XCSJ.Attributes.Icon(EIcon.Path)]
    [RequireComponent(typeof(GameObjectSet))]
    public class PathAnimation : Path<PathAnimation>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "路径动画";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.MultiMedia, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.MultiMediaDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(PathAnimation))]
        [Tip("路径动画组件是控制多个游戏对象沿着某条路径运动的动画。在第一个对象移动一段时间后，会控制第二个对象，接着第一个对象之后沿着路径运动。可用于模拟箭头流动的动画。播放完毕后，组件切换为完成态。", "The path animation component is an animation that controls the movement of multiple game objects along a certain path. After the first object moves for a period of time, the second object is controlled, and then the first object moves along the path. Arrows that can be used to simulate flow. After playing, the component switches to the finished state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 游戏对象集
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        #region IPath

        /// <summary>
        /// 变换
        /// </summary>
        public override List<Transform> transforms => gameObjectSet.objects.ToList(go => go.transform);

        /// <summary>
        /// 添加变换
        /// </summary>
        /// <param name="transform"></param>
        public override void AddTransform(Transform transform)
        {
            if (transform) gameObjectSet.Add(transform.gameObject);
        }

        #endregion

        /// <summary>
        /// 间隔类型
        /// </summary>
        [Name("间隔类型")]
        public enum ESpaceType
        {
            /// <summary>
            /// 百分比
            /// </summary>
            [Name("百分比")]
            Perent = 0,

            /// <summary>
            /// 时间
            /// </summary>
            [Name("时间")]
            Time,

            /// <summary>
            /// 距离
            /// </summary>
            [Name("距离")]
            Distance,
        }

        /// <summary>
        /// 移动完美间距
        /// </summary>
        [Group("移动间距设置", textEN = "Move Spacing Settings", needBoundBox = true, defaultIsExpanded = false)]
        [Name("移动完美间距")]
        public bool movePrettySpace = true;

        /// <summary>
        /// 移动间隔类型
        /// </summary>
        [Name("移动间隔类型")]
        [HideInSuperInspector(nameof(movePrettySpace), EValidityCheckType.True)]
        [EnumPopup]
        public ESpaceType moveSpaceType = ESpaceType.Perent;

        /// <summary>
        /// 移动间距值
        /// </summary>
        [Name("移动间距值")]
        [HideInSuperInspector(nameof(movePrettySpace), EValidityCheckType.True)]
        public double moveSpaceValue = 0.1f;

        /// <summary>
        /// 视图完美间距
        /// </summary>
        [Group("视图间距设置", textEN = "View Spacing Settings", needBoundBox = true, defaultIsExpanded = false)]
        [Name("视图完美间距")]
        public bool viewPrettySpace = true;

        /// <summary>
        /// 视图间隔类型
        /// </summary>
        [Name("视图间隔类型")]
        [HideInSuperInspector(nameof(viewPrettySpace), EValidityCheckType.True)]
        [EnumPopup]
        public ESpaceType viewSpaceType = ESpaceType.Perent;

        /// <summary>
        /// 视图间距值
        /// </summary>
        [Name("视图间距值")]
        [HideInSuperInspector(nameof(viewPrettySpace), EValidityCheckType.True)]
        public double viewSpaceValue = 0.1f;

        /// <summary>
        /// 转场
        /// </summary>
        [Name("转场")]
        public enum ETransition
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 渐入
            /// </summary>
            [Name("渐入")]
            FadeIn,

            /// <summary>
            /// 循环
            /// </summary>
            [Name("循环")]
            Loop,

            /// <summary>
            /// 渐出
            /// </summary>
            [Name("渐出")]
            FadeOut,
        }

        /// <summary>
        /// 转场效果
        /// </summary>
        [EndGroup]
        [Name("转场效果")]
        [EnumPopup]
        public ETransition transition = ETransition.FadeIn;

        /// <summary>
        /// 获取完美间距值
        /// </summary>
        /// <param name="count"></param>
        /// <param name="spaceType"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public double GetPrettySpaceValue(int count, ESpaceType spaceType, Vector3[] path)
        {
            if (--count < 1) return 0;
            switch (spaceType)
            {
                case ESpaceType.Time:
                    {
                        return timeLength / count;
                    }
                case ESpaceType.Distance:
                    {
                        return MathU.PathLength(path) / count;
                    }
                case ESpaceType.Perent:
                default:
                    {
                        return 1f / count;
                    }
            }
        }

        /// <summary>
        /// 获取间距百分比
        /// </summary>
        /// <param name="count"></param>
        /// <param name="prettySpace"></param>
        /// <param name="spaceType"></param>
        /// <param name="spaceValue"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public double GetSpacePercent(int count, bool prettySpace, ESpaceType spaceType, double spaceValue, Vector3[] path)
        {
            if (count <= 1) return 0;
            if (prettySpace) return 1f / (count - 1);

            switch (spaceType)
            {
                case ESpaceType.Time:
                    {
                        return MathX.ApproximatelyZero(timeLength) ? 0 : spaceValue / timeLength;
                    }
                case ESpaceType.Distance:
                    {
                        var length = MathU.PathLength(path);
                        return MathX.ApproximatelyZero(length) ? 0 : spaceValue / length;
                    }
                case ESpaceType.Perent:
                default:
                    {
                        return spaceValue;
                    }
            }
        }

        /// <summary>
        /// 当更新位置
        /// </summary>
        /// <param name="recorder"></param>
        /// <param name="path"></param>
        /// <param name="percent"></param>
        /// <param name="pathPatchType"></param>
        protected override void OnUpdatePosition(Recorder recorder, Vector3[] path, Percent percent, ELineType pathPatchType)
        {
            try
            {
                switch (transition)
                {
                    case ETransition.FadeIn:
                        {
                            var count = recorder._records.Count;
                            var spacePercent = GetSpacePercent(count, movePrettySpace, moveSpaceType, moveSpaceValue, path);
                            for (int i = 0; i < count; i++)
                            {
                                var p = percent.percentOfWorkCurve - spacePercent * i;
                                recorder._records[i].transform.position = Move.Interp(pathPatchType, path, MathX.Clamp01(p));
                            }
                            break;
                        }
                    case ETransition.FadeOut:
                        {
                            var count = recorder._records.Count;
                            var spacePercent = GetSpacePercent(count, movePrettySpace, moveSpaceType, moveSpaceValue, path);

                            for (int i = 0, j = count - 1; i < count; i++, j--)
                            {
                                var p = percent.percentOfWorkCurve + spacePercent * i;
                                recorder._records[j].transform.position = Move.Interp(pathPatchType, path, MathX.Clamp01(p));
                            }
                            break;
                        }
                    case ETransition.Loop:
                        {
                            var count = recorder._records.Count;
                            var spacePercent = GetSpacePercent(count, movePrettySpace, moveSpaceType, moveSpaceValue, path);

                            for (int i = 0, j = count - 1; i < count; i++, j--)
                            {
                                var p = percent.percentOfWorkCurve + spacePercent * i;
                                recorder._records[j].transform.position = Move.Interp(pathPatchType, path, Percent.Loop01(p));
                            }
                            break;
                        }
                    default:
                        {
                            base.OnUpdatePosition(recorder, path, percent, pathPatchType);
                            break;
                        }
                }

            }
            catch { }
        }

        /// <summary>
        /// 当更新视图
        /// </summary>
        /// <param name="recorder"></param>
        /// <param name="path"></param>
        /// <param name="percent"></param>
        /// <param name="pathPatchType"></param>
        protected override void OnUpdateView(Recorder recorder, Vector3[] path, Percent percent, ELineType pathPatchType)
        {
            try
            {
                switch (transition)
                {
                    case ETransition.FadeIn:
                        {
                            var willEnd = percent.percent > 1;
                            var percentage = percent.percentOfWorkCurve < viewForwardCoefficient ? percent.percentOfWorkCurve + (willEnd ? 1 : 0) : percent.percentOfWorkCurve;
                            var count = recorder._records.Count;
                            var spacePercent = GetSpacePercent(count, viewPrettySpace, viewSpaceType, viewSpaceValue, path);

                            for (int i = 0; i < count; i++)
                            {
                                var p = percentage - spacePercent * i;
                                if (p < viewForwardCoefficient) p = viewForwardCoefficient;
                                else if (p > 1) p = 1 + viewForwardCoefficient;

                                recorder._records[i].transform.LookAt(Move.Interp(pathPatchType, path, p));
                            }
                            break;
                        }
                    case ETransition.FadeOut:
                        {
                            var willEnd = percent.percent >= 1;
                            var percentage = percent.percentOfWorkCurve < viewForwardCoefficient ? percent.percentOfWorkCurve + (willEnd ? 1 : 0) : percent.percentOfWorkCurve;
                            var count = recorder._records.Count;
                            var spacePercent = GetSpacePercent(count, viewPrettySpace, viewSpaceType, viewSpaceValue, path);

                            for (int i = 0, j = count - 1; i < recorder._records.Count; i++, j--)
                            {
                                var p = percentage + spacePercent * i;
                                if (p < viewForwardCoefficient) p = viewForwardCoefficient;
                                else if (p > 1) p = 1 + viewForwardCoefficient;
                                recorder._records[j].transform.LookAt(Move.Interp(pathPatchType, path, p));
                            }
                            break;
                        }
                    case ETransition.Loop:
                        {
                            var count = recorder._records.Count;
                            var spacePercent = GetSpacePercent(count, viewPrettySpace, viewSpaceType, viewSpaceValue, path);

                            for (int i = 0, j = count - 1; i < recorder._records.Count; i++, j--)
                            {
                                var p = MathX.DecimalPart(percent.percent01OfWorkCurve + spacePercent * i);
                                if (p >= viewForwardCoefficient)
                                {
                                    recorder._records[j].transform.LookAt(Move.Interp(pathPatchType, path, p));
                                }
                            }
                            break;
                        }
                    default:
                        {
                            base.OnUpdateView(recorder, path, percent, pathPatchType);
                            break;
                        }
                }
            }
            catch { }
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(transition);
        }
    }
}
