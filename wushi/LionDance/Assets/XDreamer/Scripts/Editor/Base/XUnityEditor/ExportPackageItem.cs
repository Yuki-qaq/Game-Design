using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 导出包项
    /// </summary>
    [LinkType("UnityEditor.ExportPackageItem")]
    public class ExportPackageItem : LinkType<ExportPackageItem>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public ExportPackageItem(object obj) : base(obj) { }

        #region assetPath

        /// <summary>
        /// 资产路径 字段信息
        /// </summary>
        public static XFieldInfo assetPath_FieldInfo { get; } = GetXFieldInfo(nameof(assetPath));

        /// <summary>
        /// 资产路径
        /// </summary>
        public string assetPath
        {
            get => assetPath_FieldInfo.GetValue(obj) as string;
            set => assetPath_FieldInfo.SetValue(obj, value);
        }

        #endregion

        #region guid

        /// <summary>
        /// 唯一编号 字段信息
        /// </summary>
        public static XFieldInfo guid_FieldInfo { get; } = GetXFieldInfo(nameof(guid));

        /// <summary>
        /// 唯一编号
        /// </summary>
        public string guid
        {
            get => guid_FieldInfo.GetValue(obj) as string;
            set => guid_FieldInfo.SetValue(obj, value);
        }

        #endregion

        #region isFolder

        /// <summary>
        /// 是文件夹字段信息
        /// </summary>
        public static XFieldInfo isFolder_FieldInfo { get; } = GetXFieldInfo(nameof(isFolder));

        /// <summary>
        /// 是文件夹
        /// </summary>
        public bool isFolder
        {
            get => (bool)isFolder_FieldInfo.GetValue(obj);
            set => isFolder_FieldInfo.SetValue(obj, value);
        }

        #endregion

        #region enabledStatus

        /// <summary>
        /// 启用状态 字段信息
        /// </summary>
        public static XFieldInfo enabledStatus_FieldInfo { get; } = GetXFieldInfo(nameof(enabledStatus));

        /// <summary>
        /// 启用状态
        /// </summary>
        public int enabledStatus
        {
            get => (int)enabledStatus_FieldInfo.GetValue(obj);
            set => enabledStatus_FieldInfo.SetValue(obj, value);
        }

        #endregion
    }
}
