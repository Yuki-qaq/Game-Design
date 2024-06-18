using System.Collections.Generic;
using UnityEngine;
using XCSJ.LitJson;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 变换记录器:用于记录变换的PRS信息；
    /// </summary>
    public class TransformRecorder : Recorder<Transform, TransformRecorder.Info>, ITransformRecorder
    {
        /// <summary>
        /// 变换记录列表
        /// </summary>
        [Json(false)]
        public ITransformRecord[] transformRecords => _records.ToArray();

        /// <summary>
        /// 记录游戏对象的变换
        /// </summary>
        /// <param name="gameObject"></param>
        public void Record(GameObject gameObject)
        {
            if (gameObject) Record(gameObject.transform);
        }

        /// <summary>
        /// 批量记录游戏对象的变换
        /// </summary>
        /// <param name="gameObjects"></param>
        public void Record(IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null) return;
            foreach (var go in gameObjects)
            {
                Record(go);
            }
        }

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="space"></param>
        public virtual void Recover(Space space)
        {
            foreach (var i in _records)
            {
                try
                {
                    i.Recover(space);
                }
                catch { }
            }
        }

        /// <summary>
        /// 信息
        /// </summary>
        public class Info : ITransformRecord
        {
            /// <summary>
            /// 原父级
            /// </summary>
            public Transform orgParent { get; private set; }

            #region 变换记录接口

            /// <summary>
            /// 变换
            /// </summary>
            [Json(false)]
            public Transform transform { get; set; }

            /// <summary>
            /// 组件
            /// </summary>
            [Json(exportString = true)]
            public Component component { get => transform; set => transform = value as Transform; }

            /// <summary>
            /// 本地位置
            /// </summary>
            [Json(exportString = true)]
            public Vector3 localPosition { get; set; }

            /// <summary>
            /// 本地旋转
            /// </summary>
            [Json(exportString = true)]
            public Quaternion localRotation { get; set; }

            /// <summary>
            /// 本地缩放
            /// </summary>
            [Json(exportString = true)]
            public Vector3 localScale { get; set; }

            /// <summary>
            /// 世界位置
            /// </summary>
            [Json(exportString = true)]
            public Vector3 worldPosition { get; set; }

            /// <summary>
            /// 世界旋转
            /// </summary>
            [Json(exportString = true)]
            public Quaternion worldRotation { get; set; }

            #endregion

            #region 矩形变换记录

            /// <summary>
            /// 偏移最大
            /// </summary>
            public Vector2 offsetMax { get; set; }

            /// <summary>
            /// 偏移最小
            /// </summary>
            public Vector2 offsetMin { get; set; }

            /// <summary>
            /// 锚点位置
            /// </summary>
            public Vector2 anchoredPosition { get; set; }

            /// <summary>
            /// 锚点位置3D
            /// </summary>
            public Vector3 anchoredPosition3D { get; set; }

            /// <summary>
            /// 轴心
            /// </summary>
            public Vector2 pivot { get; set; }

            /// <summary>
            /// 尺寸偏移
            /// </summary>
            public Vector2 sizeDelta { get; set; }

            /// <summary>
            /// 锚点最小
            /// </summary>
            public Vector2 anchorMin { get; set; }

            /// <summary>
            /// 锚点最大
            /// </summary>
            public Vector2 anchorMax { get; set; }

            #endregion

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="transform"></param>
            public virtual void Record(Transform transform)
            {
                this.transform = transform;
                if (!transform) return;

                orgParent = transform.parent;
                if (this.transform is RectTransform rectTransform && rectTransform)
                {
                    offsetMax = rectTransform.offsetMax;
                    offsetMin = rectTransform.offsetMin;
                    anchoredPosition3D = rectTransform.anchoredPosition3D;

                    pivot = rectTransform.pivot;
                    sizeDelta = rectTransform.sizeDelta;

                    anchorMax = rectTransform.anchorMax;
                    anchoredPosition = rectTransform.anchoredPosition;
                    anchorMin = rectTransform.anchorMin;
                }
                else
                {
                    localPosition = transform.localPosition;
                    localRotation = transform.localRotation;
                    localScale = transform.localScale;

                    worldPosition = transform.position;
                    worldRotation = transform.rotation;
                }
            }

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="arg1"></param>
            public void Record(GameObject arg1) => Record(arg1.transform);

            /// <summary>
            /// 恢复：基于本地信息执行恢复
            /// </summary>
            public virtual void Recover()
            {
                RecoverTo(transform);
            }

            /// <summary>
            /// 恢复
            /// </summary>
            /// <param name="space"></param>
            public void Recover(Space space) => RecoverTo(transform, space);

            /// <summary>
            /// 恢复到
            /// </summary>
            /// <param name="targetTransform"></param>
            /// <param name="space">本地或时间</param>
            public void RecoverTo(Transform targetTransform, Space space = Space.Self)
            {
                if (!targetTransform) return;

                targetTransform.SetParent(orgParent);
                if (targetTransform is RectTransform rt && rt)
                {
                    rt.offsetMax = offsetMax;
                    rt.offsetMin = offsetMin;
                    rt.anchoredPosition3D = anchoredPosition3D;

                    rt.pivot = pivot;
                    rt.sizeDelta = sizeDelta;

                    rt.anchorMax = anchorMax;
                    rt.anchoredPosition = anchoredPosition;
                    rt.anchorMin = anchorMin;
                }
                else
                {
                    switch (space)
                    {
                        case Space.Self:
                            {
                                targetTransform.localPosition = localPosition;
                                targetTransform.localRotation = localRotation;
                                targetTransform.localScale = localScale;
                                break;
                            }
                        case Space.World:
                            {
                                targetTransform.position = worldPosition;
                                targetTransform.rotation = worldRotation;
                                break;
                            }
                    }
                }
                targetTransform.hasChanged = true;
            }
        }
    }

    /// <summary>
    /// 层级变换记录器:用于记录变换的父级与PRS信息；
    /// </summary>
    public class HierarchyTransformRecorder : Recorder<Transform, HierarchyTransformRecorder.Info>
    {
        /// <summary>
        /// 信息
        /// </summary>
        public class Info : TransformRecorder.Info
        {
            /// <summary>
            /// 父级
            /// </summary>
            [Json(exportString = true)]
            public Transform parent { get; set; }

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="transform"></param>
            public override void Record(Transform transform)
            {
                parent = transform.parent;
                base.Record(transform);
            }

            /// <summary>
            /// 恢复：基于本地信息执行恢复
            /// </summary>
            public override void Recover()
            {
                transform.parent = parent;
                base.Recover();
            }
        }
    }

    /// <summary>
    /// 变换记录接口
    /// </summary>
    public interface ITransformRecord : ISingleRecord<Transform>, ISingleRecord<GameObject>
    {
        /// <summary>
        /// 变换
        /// </summary>
        Transform transform { get; set; }

        /// <summary>
        /// 本地位置
        /// </summary>
        Vector3 localPosition { get; set; }

        /// <summary>
        /// 本地旋转
        /// </summary>
        Quaternion localRotation { get; set; }

        /// <summary>
        /// 本地缩放
        /// </summary>
        Vector3 localScale { get; set; }

        /// <summary>
        /// 世界位置
        /// </summary>
        Vector3 worldPosition { get; set; }

        /// <summary>
        /// 世界旋转
        /// </summary>
        Quaternion worldRotation { get; set; }
    }

    /// <summary>
    /// 变换记录器接口
    /// </summary>
    public interface ITransformRecorder : IBatchRecorder<Transform>, IBatchRecorder<GameObject>
    {
        /// <summary>
        /// 记录列表
        /// </summary>
        ITransformRecord[] transformRecords { get; }
    }
}
