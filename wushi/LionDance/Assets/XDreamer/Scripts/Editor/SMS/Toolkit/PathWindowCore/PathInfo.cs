using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.Extension.Base.Tweens;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Motions;
using XCSJ.PluginSMS.States.Operations;

namespace XCSJ.EditorSMS.Toolkit.PathWindowCore
{
    /// <summary>
    /// 路径接口类的封装类，用于实现路径的编辑操作
    /// </summary>
    public class PathInfo
    {
        /// <summary>
        /// 路径接口对象
        /// </summary>
        public IPath pathObj = null;

        /// <summary>
        /// 状态
        /// </summary>
        public State state => (pathObj as IVirtual)?.parent as State;

        /// <summary>
        /// 状态名
        /// </summary>
        public string stateName
        {
            get
            {
                var state = this.state;
                return state ? state.name : "";
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string name
        {
            get
            {
#if CSHARP_7_3_OR_NEWER
                if (pathObj is IVirtual vir && vir.parent != null && vir.parent is IName n)
#else
                var vir = pathObj as IVirtual;
                var n = (vir == null) ? null : vir.parent as IName;
                if (n != null)
#endif
                {
                    return n?.name;
                }
                else
                {
                    return pathObj?.name;
                }
            }
            set
            {

            }
        }

        /// <summary>
        /// 第一个变换
        /// </summary>
        public Transform firstTransform
        {
            get
            {
                return pathObj?.transforms.FirstOrDefault();
            }
        }

        /// <summary>
        /// 变换数据
        /// </summary>
        public List<TransformData> transformDatas = new List<TransformData>();

        /// <summary>
        /// 生效
        /// </summary>
        public bool effective = true;

        /// <summary>
        /// 显示路径
        /// </summary>
        public bool displayPath = true;

        /// <summary>
        /// 记录中
        /// </summary>
        protected bool recording = false;

        /// <summary>
        /// 偏移值
        /// </summary>
        public Vector3 offsetValue => Vector3.zero;

        /// <summary>
        /// 参考虚拟点
        /// </summary>
        public Vector3 virtualPointPosition
        {
            get
            {
                return firstTransform ? firstTransform.position : _virtualPoint;
            }
            set
            {
                if (firstTransform)
                {
                    var offset = value - firstTransform.position;

                    firstTransform.position = value;
                    MoveTransforms(offset, 1);
                }
                _virtualPoint = value;
            }
        }

        private Vector3 _virtualPoint = Vector3.zero;

        private Vector3 _offsetToPath = Vector3.zero;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="pathObj"></param>
        public PathInfo(IPath pathObj)
        {
            this.pathObj = pathObj;

            ImportData();
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public void ImportData()
        {
            int i = 0;
            for (; i < pathObj.keyPoints.Count; ++i)
            {
                var point = pathObj.keyPoints[i];
                if (i >= transformDatas.Count)
                {
                    transformDatas.Add(new TransformData(this, point));
                }
                else
                {
                    transformDatas[i].position = point;
                }
            }

            if (i < transformDatas.Count)
            {
                transformDatas.RemoveRange(i, transformDatas.Count - i);
            }
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        public void ExportData()
        {
            pathObj.keyPoints = GetKeyPoints();
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            if (pathObj is IVirtual vir && vir.parent != null)
            {
                if (vir.parent.Delete(true))
                {
                    pathObj = null;
                }
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            transformDatas.Clear();
        }

        /// <summary>
        /// 获取关键点
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetKeyPoints() => transformDatas.ConvertAll(t => t.position);

        /// <summary>
        /// 当已选择
        /// </summary>
        public void OnSelected()
        {
            if (pathObj == null) return;

            if (pathObj.transforms.Count > 0)
            {
                Selection.objects = pathObj.transforms.ConvertAll(t => t.gameObject).ToArray();
            }
            else
            {
                Selection.activeObject = null;
            }
        }

        /// <summary>
        /// 当记录中是路径的变换出
        /// </summary>
        /// <returns></returns>
        public bool IsTransformOutOfPathWhenRecording()
        {
            return transformDatas.Exists(td => !MathX.ApproximatelyZero((virtualPointPosition - lastRecordPosition).sqrMagnitude));
        }

        #region 记录

        private TransformData beginTransformData = null;

        private Vector3 lastRecordPosition = Vector3.zero;

        /// <summary>
        /// 当开始记录
        /// </summary>
        public void OnBeginRecord()
        {
            recording = true;

            // 先插入一个点
            if (transformDatas.Count == 0)
            {
                transformDatas.Add(new TransformData(this, virtualPointPosition));
            }

            // 自动移动到开始位置
            MoveToBegin();

            // 记录开始录制点
            beginTransformData = new TransformData(this);
        }

        /// <summary>
        /// 记录
        /// </summary>
        public void Record()
        {
            if (!effective) return;
            if (beginTransformData==null)
            {
                beginTransformData = new TransformData(this);
            }

            var moveOffset = virtualPointPosition - lastRecordPosition;

            if (MathX.ApproximatelyZero(moveOffset.sqrMagnitude)) return;

            // 偏移同路径对象
            for (int i = 1; i < pathObj.transforms.Count; ++i)
            {
                pathObj.transforms[i].position += moveOffset;
            }

            // 记录关键点
            var position = virtualPointPosition;

            transformDatas.Add(new TransformData(this, position));

            lastRecordPosition = virtualPointPosition;
        }

        /// <summary>
        /// 当结束记录
        /// </summary>
        public void OnEndRecord()
        {
            recording = false;

            if (beginTransformData != null)
            {
                if (transformDatas.Count == 1
                    && MathX.ApproximatelyZero((beginTransformData.recordPosition - lastRecordPosition).sqrMagnitude))
                {
                    transformDatas.Clear();
                }

                _offsetToPath = Vector3.zero;
                beginTransformData.Recover();
            }
        }

        #endregion

        #region 路径操作

        /// <summary>
        /// 逆转动作
        /// </summary>
        /// <param name="ignoreEffective"></param>
        public void ReverseMotion(bool ignoreEffective = false)
        {
            if (!effective && !ignoreEffective) return;

            MoveToEnd();
            transformDatas.Reverse();
            transformDatas.ForEach(td => td.position += _offsetToPath);
            _offsetToPath = Vector3.zero;
        }

        /// <summary>
        /// 对手方向路径
        /// </summary>
        /// <param name="ignoreEffective"></param>
        public void OppositeDirectionPath(bool ignoreEffective = false)
        {
            if (!effective && !ignoreEffective) return;

            if (transformDatas.Count <= 1) return;

            var beginPosition = transformDatas[0].position;
            for (int i = 1; i < transformDatas.Count; ++i)
            {
                transformDatas[i].position = -(transformDatas[i].position - beginPosition) + beginPosition;
            }
        }

        /// <summary>
        /// 移动到开始
        /// </summary>
        /// <param name="ignoreEffective"></param>
        public void MoveToBegin(bool ignoreEffective = false) => MoveToPercent(0, ignoreEffective);

        /// <summary>
        /// 移动到结束
        /// </summary>
        /// <param name="ignoreEffective"></param>
        public void MoveToEnd(bool ignoreEffective = false) => MoveToPercent(1, ignoreEffective);

        private void MoveToPoint(int index)
        {
            MoveTransforms(Vector3.zero);
        }

        /// <summary>
        /// 移动变换
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="fromIndex"></param>
        protected void MoveTransforms(Vector3 offset, int fromIndex = 0)
        {
            if (pathObj == null || MathX.ApproximatelyZero(offset.sqrMagnitude)) return;

            for (int i = fromIndex; i < pathObj.transforms.Count; ++i)
            {
                pathObj.transforms[i].position += offset;
            }
        }

        /// <summary>
        /// 移动到百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="ignoreEffective"></param>
        public void MoveToPercent(double percent, bool ignoreEffective = false)
        {
            if (!effective && !ignoreEffective) return;

            if (transformDatas.Count > 0)
            {
                virtualPointPosition = MathU.LinerInterp(transformDatas.Cast(d => d.keyPoint).ToArray(), percent);
            }

            lastRecordPosition = virtualPointPosition;
        }

        #endregion
    }
}
