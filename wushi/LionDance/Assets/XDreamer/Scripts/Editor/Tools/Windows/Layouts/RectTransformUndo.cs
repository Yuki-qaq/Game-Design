using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;

namespace XCSJ.EditorTools.Windows.Layouts
{
    /// <summary>
    /// 矩形变换撤销
    /// </summary>
    public class RectTransformUndo : XUndo<RectTransformUndo.Cmd>
    {
        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="rectTransforms"></param>
        /// <param name="betweenOldAndNewWhenRecord"></param>
        public void Record(List<RectTransform> rectTransforms, Action betweenOldAndNewWhenRecord)
        {
            if (rectTransforms == null || rectTransforms.Count == 0 || betweenOldAndNewWhenRecord == null) return;
            var cmd = new Cmd();
            Record(rectTransforms, cmd.oldInfos);
            betweenOldAndNewWhenRecord?.Invoke();
            Record(rectTransforms, cmd.newInfos);
            Record(cmd);
        }

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="rectTransforms"></param>
        /// <param name="betweenOldAndNewWhenRecord"></param>
        public void Record(List<RectTransform> rectTransforms, Func<bool> betweenOldAndNewWhenRecord)
        {
            var canRecord = rectTransforms != null && rectTransforms.Count > 0 && betweenOldAndNewWhenRecord != null;

            var old = canRecord ? TmpRecord(rectTransforms) : null;
            if (betweenOldAndNewWhenRecord() && canRecord)
            {
                var cmd = new Cmd();
                cmd.oldInfos.AddRangeWithDistinct(old);
                Record(rectTransforms, cmd.newInfos);
                Record(cmd);
            }
        }

        private Dictionary<RectTransform, Info> TmpRecord(List<RectTransform> rectTransforms)
        {
            Dictionary<RectTransform, Info> kvs = new Dictionary<RectTransform, Info>();
            foreach (var t in rectTransforms)
            {
                if (!t) continue;
                kvs[t] = new Info(t);
            }
            return kvs;
        }

        private void Record(List<RectTransform> rectTransforms, Dictionary<RectTransform, Info> kvs)
        {
            foreach (var t in rectTransforms)
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
            /// 尺寸
            /// </summary>
            public Vector2 size;

            /// <summary>
            /// 位置
            /// </summary>
            public Vector3 position;

            /// <summary>
            /// 右
            /// </summary>
            public Vector3 right;

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="rectTransform"></param>
            public Info(RectTransform rectTransform)
            {
                size = rectTransform.rect.size;
                position = rectTransform.position;
                right = rectTransform.right;
            }
        }

        /// <summary>
        /// 命令
        /// </summary>
        public class Cmd : BaseCommand<RectTransform, Info>
        {
            /// <summary>
            /// 做
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public override void Do(RectTransform key, Info value)
            {
                key.sizeDelta = value.size;
                key.position = value.position;
                key.right = value.right;
            }

            /// <summary>
            /// 撤销
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public override void Undo(RectTransform key, Info value) => Do(key, value);
        }
    }
}
