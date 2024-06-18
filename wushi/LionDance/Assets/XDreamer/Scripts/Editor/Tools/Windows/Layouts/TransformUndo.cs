using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;

namespace XCSJ.EditorTools.Windows.Layouts
{
    /// <summary>
    /// 变换撤销
    /// </summary>
    public class TransformUndo : XUndo<TransformUndo.Cmd>
    {
        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="transforms"></param>
        /// <param name="betweenOldAndNewWhenRecord"></param>
        public void Record(List<Transform> transforms, Action betweenOldAndNewWhenRecord)
        {
            if (transforms == null || transforms.Count == 0 || betweenOldAndNewWhenRecord == null) return;
            var cmd = new Cmd();
            Record(transforms, cmd.oldInfos);
            betweenOldAndNewWhenRecord?.Invoke();
            Record(transforms, cmd.newInfos);
            Record(cmd);
        }

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="transforms"></param>
        /// <param name="betweenOldAndNewWhenRecord"></param>
        public void Record(List<Transform> transforms, Func<bool> betweenOldAndNewWhenRecord)
        {
            var canRecord = transforms != null && transforms.Count > 0 && betweenOldAndNewWhenRecord != null;

            var old = canRecord ? TmpRecord(transforms) : null;
            if (betweenOldAndNewWhenRecord() && canRecord)
            {
                var cmd = new Cmd();
                cmd.oldInfos.AddRangeWithDistinct(old);
                Record(transforms, cmd.newInfos);
                Record(cmd);
            }
        }

        private Dictionary<Transform, Info> TmpRecord(List<Transform> transforms)
        {
            Dictionary<Transform, Info> kvs = new Dictionary<Transform, Info>();
            foreach (var t in transforms)
            {
                if (!t) continue;
                kvs[t] = new Info(t);
            }
            return kvs;
        }

        private void Record(List<Transform> transforms, Dictionary<Transform, Info> kvs)
        {
            foreach (var t in transforms)
            {
                if (!t) continue;
                kvs[t] = new Info(t);
            }
        }

        /// <summary>
        /// 信息
        /// </summary>
        public class Info
        {
            /// <summary>
            /// 位置
            /// </summary>
            public Vector3 position;

            /// <summary>
            /// 本地缩放
            /// </summary>
            public Vector3 localScale;

            /// <summary>
            /// 欧拉角
            /// </summary>
            public Vector3 eulerAngles;

            /// <summary>
            /// 信息
            /// </summary>
            /// <param name="transform"></param>
            public Info(Transform transform)
            {
                position = transform.position;
                localScale = transform.localScale;
                eulerAngles = transform.eulerAngles;
            }
        }

        /// <summary>
        /// 命令
        /// </summary>
        public class Cmd : BaseCommand<Transform, Info>
        {
            /// <summary>
            /// 做
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public override void Do(Transform key, Info value)
            {
                key.position = value.position;
                key.localScale = value.localScale;
                key.eulerAngles = value.eulerAngles;
            }

            /// <summary>
            /// 撤销
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public override void Undo(Transform key, Info value) => Do(key, value);
        }
    }
}
