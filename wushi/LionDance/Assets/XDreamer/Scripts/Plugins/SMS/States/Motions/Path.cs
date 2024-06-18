using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Base.Tweens;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginTools.ReferencedObjects;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 路径
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Path<T> : Motion<T, Path<T>.Recorder>, IPath where T : Path<T>
    {
        #region IPath

        /// <summary>
        /// 关键点
        /// </summary>
        public virtual List<Vector3> keyPoints
        {
            get => movePath.Where(t => t).ToList(t => t.position);
            set => ReferencedObjectHelper.SynchronousPosition(this, _movePath, value, parent.name);
        }

        /// <summary>
        /// 变换
        /// </summary>
        public abstract List<Transform> transforms { get; }

        /// <summary>
        /// 添加变换
        /// </summary>
        /// <param name="transform"></param>
        public abstract void AddTransform(Transform transform);

        EViewRule IPath.viewRule { get => viewRule; set => viewRule = value; }

        #endregion

        #region 移动-位置

        /// <summary>
        /// 移动路径补间类型
        /// </summary>
        [Group("移动路径设置", textEN = "Move Path Settings", needBoundBox = true, defaultIsExpanded = true)]
        [Name("移动路径补间类型")]
        [EnumPopup]
        public ELineType movePathType = ELineType.Liner;

        /// <summary>
        /// 移动路径变换类型
        /// </summary>
        [Name("移动路径变换类型")]
        [EnumPopup]
        public EPathTransformType _movePathTransformType = EPathTransformType.Transform;

        /// <summary>
        /// 移动变换排序规则
        /// </summary>
        [Name("移动变换排序规则")]
        [HideInSuperInspector(nameof(_movePathTransformType), EValidityCheckType.NotEqual, EPathTransformType.TransformChildren)]
        [EnumPopup]
        public EPathTransformSortRule _moveTransformSortRule = EPathTransformSortRule.NameAsc;

        /// <summary>
        /// 移动路径
        /// </summary>
        [Name("移动路径")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [FormerlySerializedAs(nameof(movePath))]
        [Array]
        public List<Transform> _movePath = new List<Transform>();

        /// <summary>
        /// 有效移动路径
        /// </summary>
        public bool validMovePath => _movePath.Count >= 1;

        /// <summary>
        /// 移动路径
        /// </summary>
        public List<Transform> movePath => CacheMovePath();


        private List<Transform> movePathCache = new List<Transform>();

        private List<Transform> CacheMovePath()
        {
            movePathCache.Clear();
            switch (_movePathTransformType)
            {
                case EPathTransformType.Transform:
                    {
                        movePathCache.AddRange(_movePath);
                        break;
                    }
                case EPathTransformType.TransformChildren:
                    {
                        foreach (var t in _movePath)
                        {
                            foreach (Transform child in t)
                            {
                                movePathCache.Add(child);
                            }
                        }
                        PathTransformSortRuleHelper.Sort(movePathCache, _moveTransformSortRule);
                        break;
                    }
            }
            return movePathCache;
        }

        #endregion

        #region 视图-方向

        /// <summary>
        /// 视图规则
        /// </summary>
        [Group("视图路径设置", textEN = "View Path Settings", needBoundBox = true, defaultIsExpanded = false)]
        [Name("视图规则")]
        [Tip("对象在移动路径上发生移动时，其Z轴目标方向的更新处理规则；", "When the object moves on the moving path, the update processing rules of its z-axis target direction;")]
        [EnumPopup]
        public EViewRule viewRule = EViewRule.MovePath;

        /// <summary>
        /// 视图路径补间类型
        /// </summary>
        [Name("视图路径补间类型")]
        [HideInSuperInspector(nameof(viewRule), EValidityCheckType.NotEqual, EViewRule.ViewPath)]
        [EnumPopup]
        public ELineType viewPathType = ELineType.Liner;

        /// <summary>
        /// 视图路径变换类型
        /// </summary>
        [Name("视图路径变换类型")]
        [HideInSuperInspector(nameof(viewRule), EValidityCheckType.NotEqual, EViewRule.ViewPath)]
        [EnumPopup]
        public EPathTransformType _viewPathTransformType = EPathTransformType.Transform;

        /// <summary>
        /// 视图变换排序规则
        /// </summary>
        [Name("视图变换排序规则")]
        [HideInSuperInspector(nameof(_viewPathTransformType), EValidityCheckType.NotEqual | EValidityCheckType.Or, EPathTransformType.TransformChildren, nameof(viewRule), EValidityCheckType.NotEqual, EViewRule.ViewPath)]
        [EnumPopup]
        public EPathTransformSortRule _viewTransformSortRule = EPathTransformSortRule.NameAsc;

        /// <summary>
        /// 视图路径
        /// </summary>
        [Name("视图路径")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [HideInSuperInspector(nameof(viewRule), EValidityCheckType.NotEqual, EViewRule.ViewPath)]
        [FormerlySerializedAs(nameof(viewPath))]
        [Array]
        public List<Transform> _viewPath = new List<Transform>();

        /// <summary>
        /// 有效视图路径
        /// </summary>
        public bool validViewPath => _viewPath.Count >= 1;

        /// <summary>
        /// 视图路径
        /// </summary>
        public List<Transform> viewPath => CacheViewPath();


        private List<Transform> viewPathCache = new List<Transform>();

        private List<Transform> CacheViewPath()
        {
            viewPathCache.Clear();
            switch (_viewPathTransformType)
            {
                case EPathTransformType.Transform:
                    {
                        viewPathCache.AddRange(_viewPath);
                        break;
                    }
                case EPathTransformType.TransformChildren:
                    {
                        foreach (var t in _viewPath)
                        {
                            foreach (Transform child in t)
                            {
                                viewPathCache.Add(child);
                            }
                        }
                        PathTransformSortRuleHelper.Sort(viewPathCache, _viewTransformSortRule);
                        break;
                    }
            }
            return viewPathCache;
        }

        /// <summary>
        /// 视图前移系数
        /// </summary>
        [Name("视图前移系数")]
        [Range(0.0001f, 0.1f)]
        [EndGroup(true)]
#if XDREAMER_EDITION_DEVELOPER
        [HideInSuperInspector(nameof(viewRule), EValidityCheckType.NotEqual, EViewRule.MovePath)]
#else
        [HideInSuperInspector]
#endif
        public float viewForwardCoefficient = 0.01f;

        #endregion

        #region 检查器窗口

        /// <summary>
        /// 批量处理对象
        /// </summary>
        [Name("批量处理对象")]
        [HideInSuperInspector]
        public Transform batchGameObject;

        /// <summary>
        /// 包含:为True时，将 批处理游戏对象<see cref="batchGameObject"/> 添加到对象集数组(包括移动路径<see cref="_movePath"/>、视图路径<see cref="_viewPath"/>)中；无则添加，有则不变；
        /// </summary>
        [Name("包含")]
        [Tip("为True时，将 批处理游戏对象 添加到对象集数组(包括移动路径、视图路径)中；无则添加，有则不变；", "When true, add the batch game object to the object set array (including moving path and view path); If there is none, it will be added, and if there is one, it will remain unchanged;")]
        [HideInSuperInspector]
        public bool include = true;

        /// <summary>
        /// 成员:为True时，将 批处理游戏对象<see cref="batchGameObject"/> 的子级成员全部添加到对象集数组(包括移动路径<see cref="_movePath"/>、视图路径<see cref="_viewPath"/>)中；无则添加，缺则补漏，有则不变；
        /// </summary>
        [Name("成员")]
        [Tip("为True时，将 批处理游戏对象 的子级成员全部添加到对象集数组(包括移动路径、视图路径)中；无则添加，缺则补漏，有则不变；", "When true, all child members of batch game objects are added to the object set array (including moving path and view path); If there is no, add, if there is a lack, make up the leak, and if there is, it will remain unchanged;")]
        [HideInSuperInspector]
        public bool chileren = false;

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            _movePath.RemoveAll(t => !t);
            _viewPath.RemoveAll(t => !t);
            viewPercent.Init(this);
            return base.Init(data);
        }

        internal static Vector3[] GetFullPath(Vector3[] path, ELineType pathPatchType)
        {
            return XGizmos.PathControlPointGenerator(pathPatchType, path);
        }

        /// <summary>
        /// 获取全移动路径
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetFullMovePath() => GetFullPath(GetMovePath(), movePathType);

        /// <summary>
        /// 获取移动路径
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetMovePath() => GetPath(movePath);

        /// <summary>
        /// 获取全视图路径
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetFullViewPath() => GetFullPath(GetViewPath(), viewPathType);

        /// <summary>
        /// 获取视图路径
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetViewPath() => GetPath(viewPath);

        /// <summary>
        /// 获取路径
        /// </summary>
        /// <param name="transforms"></param>
        /// <returns></returns>
        protected Vector3[] GetPath(IList<Transform> transforms)
        {
            List<Vector3> list = new List<Vector3>();
            try
            {
                foreach (var t in transforms)
                {
                    if (t) list.Add(t.position);
                }

                switch (list.Count)
                {
                    case 0: return Empty<Vector3>.Array;
                    case 1:
                        {
                            list.Add(list[0]);
                            break;
                        }
                }
            }
            catch(Exception ex)
            {
                ex.HandleException(nameof(GetPath));
            }
            return list.ToArray();
        }

        /// <summary>
        /// 视图百分比
        /// </summary>
        protected Percent viewPercent = new Percent();

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="recorder"></param>
        /// <param name="percent"></param>
        protected virtual void OnSetPercent(Recorder recorder, Percent percent)
        {
            switch (viewRule)
            {
                case EViewRule.None:
                    {
                        if (validMovePath) OnUpdatePosition(recorder, GetFullMovePath(), percent, movePathType);
                        break;
                    }
                case EViewRule.MovePath:
                    {
                        if (!validMovePath) break;
                        var movePath01 = GetFullMovePath();
                        OnUpdatePosition(recorder, movePath01, percent, movePathType);

                        OnUpdateView(recorder, movePath01, viewPercent.Update(percent.percentOfState + viewForwardCoefficient), movePathType);
                        break;
                    }
                case EViewRule.ViewPath:
                    {
                        if (validMovePath) OnUpdatePosition(recorder, GetFullMovePath(), percent, movePathType);
                        if (validViewPath) OnUpdateView(recorder, GetFullViewPath(), percent, viewPathType);
                        break;
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
        protected virtual void OnUpdatePosition(Recorder recorder, Vector3[] path, Percent percent, ELineType pathPatchType)
        {
            try
            {
                var movePoint = Move.Interp(pathPatchType, path, percent.percent01OfWorkCurve);
                foreach (var info in recorder._records)
                {
                    info.transform.position = movePoint;
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
        protected virtual void OnUpdateView(Recorder recorder, Vector3[] path, Percent percent, ELineType pathPatchType)
        {
            try
            {
                var lookAtPoint = Move.Interp(pathPatchType, path, percent.percent01OfWorkCurve);
                foreach (var info in recorder._records)
                {
                    info.transform.LookAt(lookAtPoint);
                }
            }
            catch { }
        }

        /// <summary>
        /// 记录器
        /// </summary>
        public class Recorder : TransformRecorder, IPercentRecorder<T>
        {
            /// <summary>
            /// 路径
            /// </summary>
            public T path;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="path"></param>
            public void Record(T path)
            {
                this.path = path;
                Record(path.transforms);
            }

            /// <summary>
            /// 设置百分比
            /// </summary>
            /// <param name="percent"></param>
            public void SetPercent(Percent percent)
            {
                if (path) path.OnSetPercent(this, percent);
            }
        }

        /// <summary>
        /// 当绘制Gizmos
        /// </summary>
        public static event Action<T> onDrawGizmos;

        /// <summary>
        /// 当绘制Gizmos
        /// </summary>
        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            onDrawGizmos?.Invoke((T)this);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return validMovePath;
        }
    }

    /// <summary>
    /// 路径变换类型
    /// </summary>
    [Name("路径变换类型")]
    public enum EPathTransformType
    {
        /// <summary>
        /// 变换
        /// </summary>
        [Name("变换")]
        Transform = 0,

        /// <summary>
        /// 变换成员
        /// </summary>
        [Name("变换成员")]
        TransformChildren,
    }

    /// <summary>
    /// 路径变换排序规则
    /// </summary>
    [Name("路径变换排序规则")]
    public enum EPathTransformSortRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 名称升序
        /// </summary>
        [Name("名称升序")]
        [Tip("名称自然比较后升序排序", "Sort names in ascending order after natural comparison")]
        NameAsc,

        /// <summary>
        /// 名称降序
        /// </summary>
        [Name("名称降序")]
        [Tip("名称自然比较后降序排序", "Sort names in descending order after natural comparison")]
        NameDesc,

        /// <summary>
        /// 名称字母升序
        /// </summary>
        [Name("名称字母升序")]
        [Tip("名称字母比较后升序排序", "Sort names in ascending order after letter comparison")]
        NameAlphabeticAsc,

        /// <summary>
        /// 名称字母降序
        /// </summary>
        [Name("名称字母降序")]
        [Tip("名称字母比较后降序排序", "Sort names in descending order after letter comparison")]
        NameAlphabeticDesc,
    }

    /// <summary>
    /// 路径变换排序规则组手
    /// </summary>
    public class PathTransformSortRuleHelper
    {
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="transforms"></param>
        /// <param name="pathTransformSortRule"></param>
        public static void Sort(List<Transform> transforms, EPathTransformSortRule pathTransformSortRule)
        {
            switch (pathTransformSortRule)
            {
                case EPathTransformSortRule.NameAsc:
                    {
                        transforms.Sort((x, y) => StringHelper.NaturalCompare(x.name, y.name));
                        break;
                    }
                case EPathTransformSortRule.NameDesc:
                    {
                        transforms.Sort((x, y) => StringHelper.NaturalCompare(y.name, x.name));
                        break;
                    }
                case EPathTransformSortRule.NameAlphabeticAsc:
                    {
                        transforms.Sort((x, y) => string.Compare(x.name, y.name));
                        break;
                    }
                case EPathTransformSortRule.NameAlphabeticDesc:
                    {
                        transforms.Sort((x, y) => string.Compare(y.name, x.name));
                        break;
                    }

            }
        }
    }
}
