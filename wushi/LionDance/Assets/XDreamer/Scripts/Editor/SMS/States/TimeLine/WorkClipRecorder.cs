using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.Maths;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.EditorSMS.States.TimeLine
{
    /// <summary>
    /// 工作剪辑记录器
    /// </summary>
    public class WorkClipRecorder
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public double beginTime = 0;

        /// <summary>
        /// 结束时间
        /// </summary>
        public double endTime = 0;

        /// <summary>
        /// 时长
        /// </summary>
        public double timeLength = 0;

        /// <summary>
        /// 开始百分比
        /// </summary>
        public double beginPercent = 0;

        /// <summary>
        /// 结束百分比
        /// </summary>
        public double endPercent = 0;

        /// <summary>
        /// 百分比长
        /// </summary>
        public double percentLength => endPercent - beginPercent;

        /// <summary>
        /// 总时长
        /// </summary>
        public double totalTimeLength { get; private set; }

        /// <summary>
        /// 对象
        /// </summary>
        public UnityEngine.Object obj { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public WorkClipRecorder(UnityEngine.Object obj) { this.obj = obj; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="workClip"></param>
        /// <param name="totalTimeLength"></param>
        public WorkClipRecorder(UnityEngine.Object obj, IWorkClip workClip, double totalTimeLength)
        {
            this.obj = obj;
            Record(workClip, totalTimeLength);
        }

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="workClip"></param>
        /// <param name="totalTimeLength"></param>
        public void Record(IWorkClip workClip, double totalTimeLength)
        {
            beginTime = workClip.beginTime;
            endTime = workClip.endTime;
            timeLength = workClip.timeLength;

            beginPercent = workClip.beginPercent;
            endPercent = workClip.endPercent;
            //percentLength = workClip.percentLength;

            this.totalTimeLength = totalTimeLength;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="workClip"></param>
        public void Recover(IWorkClip workClip)
        {
            UndoHelper.RegisterCompleteObjectUndo(obj);

            workClip.endPercent = MathX.Clamp(endPercent, 0, 1);
            workClip.beginPercent = MathX.Clamp(beginPercent, 0, endPercent);
            //workClip.percentLength = workClip.endPercent - workClip.beginPercent;

            workClip.beginTime = beginTime;
            workClip.endTime = endTime;
            //workClip.timeLength = workClip.endTime - workClip.beginTime;
        }

        /// <summary>
        /// 保持百分比
        /// </summary>
        public void KeepPercent()
        {
            beginTime = totalTimeLength * beginPercent;
            endTime = totalTimeLength * endPercent;
        }

        /// <summary>
        /// 保持开始时间
        /// </summary>
        public void KeepBeginTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginPercent = beginTime / totalTimeLength;
            endTime = totalTimeLength * endPercent;
        }

        /// <summary>
        /// 保持结束时间
        /// </summary>
        public void KeepEndTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endPercent = endTime / totalTimeLength;
            beginTime = totalTimeLength * beginPercent;            
        }

        /// <summary>
        /// 保持时长与开始百分比
        /// </summary>
        public void KeepTimeLengthAndBeginPercent()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = totalTimeLength * beginPercent;
            endTime = beginTime + timeLength;
            endPercent = endTime / totalTimeLength;
        }

        /// <summary>
        /// 保持时长与结束百分比
        /// </summary>
        public void KeepTimeLengthAndEndPercent()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = totalTimeLength * endPercent;
            beginTime = endTime - timeLength;
            beginPercent = beginTime / totalTimeLength;
        }

        /// <summary>
        /// 保持时间
        /// </summary>
        public void KeepTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginPercent = beginTime / totalTimeLength;
            endPercent = endTime / totalTimeLength;
        }

        /// <summary>
        /// 设置开始时间
        /// </summary>
        /// <param name="newBeginTime"></param>
        public void SetBeginTime(double newBeginTime)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = newBeginTime;
            beginPercent = beginTime / totalTimeLength;
        }

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="newBeginPercent"></param>
        public void SetBeginPercent(double newBeginPercent)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginPercent = newBeginPercent;
            beginTime = beginPercent * totalTimeLength;
        }

        /// <summary>
        /// 当开始时间保持时长
        /// </summary>
        public void KeepTimeLengthOnBeginTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = beginTime + timeLength;
            endPercent = endTime / totalTimeLength;
        }

        /// <summary>
        /// 设置结束白恩比
        /// </summary>
        /// <param name="newEndnPercent"></param>
        public void SetEndPercent(double newEndnPercent)
        {
            endPercent = newEndnPercent;
            endTime = endPercent * totalTimeLength;
        }

        /// <summary>
        /// 当结束时间保持时长
        /// </summary>
        public void KeepTimeLengthOnEndTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = endTime - timeLength;
            beginPercent = beginTime / totalTimeLength;
        }

        /// <summary>
        /// 设置结束时间
        /// </summary>
        /// <param name="newEndTime"></param>
        public void SetEndTime(double newEndTime)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = newEndTime;
            endPercent = endTime / totalTimeLength;
        }

        /// <summary>
        /// 设置时长
        /// </summary>
        /// <param name="newTimeLength"></param>
        public void SetTimeLength(double newTimeLength)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = beginTime + newTimeLength;

            if (endTime > totalTimeLength)
            {
                double offset = endTime - totalTimeLength;
                endTime = totalTimeLength;
                beginTime = MathX.Clamp(beginTime - offset, 0, endTime);
            }

            beginPercent = beginTime / totalTimeLength;
            endPercent = endTime / totalTimeLength;
        }

        /// <summary>
        /// 当时长变更固定开始时间
        /// </summary>
        /// <param name="newTimeLength"></param>
        public void OnTimeLengthChangeFixBeginTime(double newTimeLength)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = beginTime + newTimeLength;
            endTime = MathX.Clamp(endTime, beginTime, totalTimeLength);

            endPercent = endTime / totalTimeLength;
        }

        /// <summary>
        /// 当时长变更固定结束时间
        /// </summary>
        /// <param name="newTimeLength"></param>
        public void OnTimeLengthChangeFixEndTime(double newTimeLength)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = endTime - newTimeLength;
            beginTime = MathX.Clamp(beginTime, 0, endTime);

            beginPercent = beginTime / totalTimeLength;
        }
    }
}
