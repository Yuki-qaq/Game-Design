using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.EditorSMS.Toolkit.PathWindowCore
{
    /// <summary>
    /// 变换数据
    /// </summary>
    [Name("变换数据")]
    public class TransformData
    {
        /// <summary>
        /// 关键点
        /// </summary>
        public virtual Vector3 keyPoint
        {
            get
            {
                return pathInfo.offsetValue + position;
            }
            set
            {
                position = value - pathInfo.offsetValue;
            }
        }

        /// <summary>
        /// 路径信息
        /// </summary>
        public PathInfo pathInfo = null;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 position = Vector3.zero;

        /// <summary>
        /// 旋转
        /// </summary>
        public Quaternion rotation = Quaternion.Euler(0, 0, 0);

        /// <summary>
        /// 缩放
        /// </summary>
        public Vector3 scale = Vector3.one;
        
        /// <summary>
        /// 记录位置
        /// </summary>
        public Vector3 recordPosition = Vector3.zero;

        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="transformData"></param>
        /// <returns></returns>
        public static TransformData Clone(TransformData transformData)
        {
            var newObj = new TransformData(transformData.pathInfo);
            newObj.pathInfo = transformData.pathInfo;
            newObj.position = transformData.position;
            newObj.rotation = transformData.rotation;
            newObj.scale = transformData.scale;
            return newObj;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="pathInfo"></param>
        public TransformData(PathInfo pathInfo)
        {
            this.pathInfo = pathInfo;
            if (pathInfo.firstTransform!=null)
            {
                this.recordPosition = pathInfo.firstTransform.position;
                this.rotation = pathInfo.firstTransform.localRotation;
                this.scale = pathInfo.firstTransform.localScale;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="pathInfo"></param>
        /// <param name="offset"></param>
        public TransformData(PathInfo pathInfo, Vector3 offset) : this(pathInfo)
        {
            this.position = offset;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public void Recover()
        {
            if (pathInfo.firstTransform != null)
            {
                pathInfo.firstTransform.position = recordPosition;
                pathInfo.firstTransform.localRotation = rotation;
                pathInfo.firstTransform.localScale = scale;
            }
        }
    }
}
