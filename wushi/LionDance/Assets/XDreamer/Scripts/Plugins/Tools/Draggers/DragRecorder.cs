using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 拖拽记录器
    /// </summary>
    [Name("拖拽记录器")]
    [XCSJ.Attributes.Icon(EIcon.Record)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    [Tool(ToolsCategory.SelectionSet, nameof(InteractableVirtual), rootType = typeof(ToolsManager))]
    public class DragRecorder : Interactor
    {
        /// <summary>
        /// 当输入交互
        /// </summary>
        /// <param name="interactor"></param>
        /// <param name="interactData"></param>
        protected override void OnInputInteract(InteractObject interactor, InteractData interactData)
        {
            base.OnInputInteract(interactor, interactData);

            if (interactData.interactState != EInteractState.Finished) return;

            if (interactor is Dragger dragger && dragger && _dragger.Contains(dragger))
            {
                switch (interactData.cmd)
                {
                    case nameof(Dragger.Grab):
                        {
                            RecordGrabbableTransfrom(interactData.interactable);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 变换记录字典
        /// </summary>
        private TransformRecorder transformRecorder = new TransformRecorder();

        /// <summary>
        /// 变换记录数量
        /// </summary>
        public int recordTransformCount => transformRecorder.recordCount;

        /// <summary>
        /// 记录可抓对象变换
        /// </summary>
        [InteractCmd]
        [Name("记录可抓对象变换")]
        public void RecordGrabbableTransfrom() => TryInteract(nameof(RecordGrabbableTransfrom));

        /// <summary>
        /// 记录可抓对象变换
        /// </summary>
        /// <param name="interactObject"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(RecordGrabbableTransfrom))]
        public EInteractResult RecordGrabbableTransfrom(InteractObject interactObject)
        {
            if (interactObject)
            {
                var grabbable = interactObject.GetComponent<Grabbable>();
                if (grabbable)
                {
                    transformRecorder.Record(grabbable.transform);
                    return EInteractResult.Success;
                }
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 还原上一次记录
        /// </summary>
        [InteractCmd]
        [Name("还原上一次记录")]
        public void RecoverLastRecord() => TryInteract(nameof(RecoverLastRecord));

        /// <summary>
        /// 还原上一次记录
        /// </summary>
        [InteractCmdFun(nameof(RecoverLastRecord))]
        public EInteractResult RecoverLastRecord(InteractData interactData)
        {
            if (recordTransformCount > 0)
            {
                transformRecorder.RecoverAndRemoveLast();
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 还原所有记录
        /// </summary>
        [InteractCmd]
        [Name("还原所有记录")]
        public void RecoverAllAndClearRecord() => TryInteract(nameof(RecoverAllAndClearRecord));

        /// <summary>
        /// 还原所有记录
        /// </summary>
        [InteractCmdFun(nameof(RecoverAllAndClearRecord))]
        public EInteractResult RecoverAllAndClearRecord(InteractData interactData)
        {
            if (recordTransformCount > 0)
            {
                transformRecorder.ReverseRecoverAndClear();
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 清除记录
        /// </summary>
        [InteractCmd]
        [Name("清除记录")]
        public void ClearRecord() => TryInteract(nameof(ClearRecord));

        /// <summary>
        /// 清除记录
        /// </summary>
        [InteractCmdFun(nameof(ClearRecord))]
        public EInteractResult ClearRecord(InteractData interactData)
        {
            if (recordTransformCount > 0)
            {
                transformRecorder.Clear();
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 记录处理规则
        /// </summary>
        public enum ERecordHandleRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 清除
            /// </summary>
            [Name("禁用时清除记录")]
            ClearOnDisable,

            /// <summary>
            /// 禁用时全部恢复并清除
            /// </summary>
            [Name("禁用时恢复全部并清除记录")]
            RecoverAllAndClearOnDisable,
        }

        /// <summary>
        /// 记录处理规则
        /// </summary>
        [Name("记录处理规则")]
        [EnumPopup]
        public ERecordHandleRule _recordHandleRule = ERecordHandleRule.ClearOnDisable;

        /// <summary>
        /// 记录拖拽器列表
        /// </summary>
        [Name("记录拖拽器列表")]
        public List<Dragger> _dragger = new List<Dragger>();

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            switch (_recordHandleRule)
            {
                case ERecordHandleRule.ClearOnDisable:
                    {
                        ClearRecord();
                        break;
                    }
                case ERecordHandleRule.RecoverAllAndClearOnDisable:
                    {
                        RecoverAllAndClearRecord();
                        break;
                    }
            }

            base.OnDisable();
        }

    }
}
