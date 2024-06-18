using UnityEngine;
using System;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.Tools;
#if XDREAMER_EASYAR_4_1_0
using easyar;
#endif

namespace XCSJ.PluginEasyAR
{
    /// <summary>
    /// 图片目标脚本事件
    /// </summary>
    [Name("图片目标脚本事件")]
    [Tip("用于捕获对指定图片(识别图、Marker)识别情况的回调；", "It is used to capture the callback of the recognition of the specified picture (identification map, marker);")]
    [Serializable]
    [Tool(EasyARHelper.Title)]
    [RequireManager(typeof(EasyARManager))]
    [Owner(typeof(EasyARManager))]
#if XDREAMER_EASYAR_4_1_0
    [RequireComponent(typeof(ImageTargetMB))]
#endif
    public class ImageTargetScriptEvent : BaseScriptEvent<EImageTargetScriptEventType, ImageTargetScriptEventFunction, ImageTargetScriptEventFunctionCollection>
    {
#if XDREAMER_EASYAR_4_1_0
        private ImageTargetMB imageTargetController;
#endif

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
#if XDREAMER_EASYAR_4_1_0
            imageTargetController = GetComponent<ImageTargetMB>();
            if (imageTargetController)
            {
                imageTargetController.TargetLoad += OnTargetLoad;
                imageTargetController.TargetUnload += OnTargetUnload;
                imageTargetController.TargetFound += OnTargetFound;
                imageTargetController.TargetLost += OnTargetLost;
            }
            else
            {
                Log.ErrorFormat("游戏对象:[{0}] 不包含{1}组件", CommonFun.GameObjectToString(gameObject), typeof(ImageTargetController));
            }
#endif
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
#if XDREAMER_EASYAR_4_1_0
            if (imageTargetController)
            {
                imageTargetController.TargetLoad -= OnTargetLoad;
                imageTargetController.TargetUnload -= OnTargetUnload;
                imageTargetController.TargetFound -= OnTargetFound;
                imageTargetController.TargetLost -= OnTargetLost;
            }
#endif
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EImageTargetScriptEventType.Start);
        }

#if XDREAMER_EASYAR_4_1_0

        /// <summary>
        /// 当目标识别
        /// </summary>
        protected void OnTargetFound()
        {
            ExecuteScriptEvent(EImageTargetScriptEventType.TargetFound);
        }

        /// <summary>
        /// 当目标丢失
        /// </summary>
        protected void OnTargetLost()
        {
            ExecuteScriptEvent(EImageTargetScriptEventType.TargetLost);
        }

        /// <summary>
        /// 当目标丢失
        /// </summary>
        /// <param name="target"></param>
        /// <param name="status"></param>
        protected void OnTargetLoad(Target target, bool status)
        {
            ExecuteScriptEvent(EImageTargetScriptEventType.TargetLoad);
        }

        /// <summary>
        /// 当目标卸载
        /// </summary>
        /// <param name="target"></param>
        /// <param name="status"></param>
        protected void OnTargetUnload(Target target, bool status)
        {
            ExecuteScriptEvent(EImageTargetScriptEventType.TargetUnload);
        }
#endif
    }

    /// <summary>
    /// 图片目标脚本事件类型
    /// </summary>
    [Name("图片目标脚本事件类型")]
    public enum EImageTargetScriptEventType
    {
        /// <summary>
        /// 启动
        /// </summary>
        [Name("启动")]
        Start = 0,

        /// <summary>
        /// 目标识别
        /// </summary>
        [Name("目标识别")]
        TargetFound,

        /// <summary>
        /// 目标丢失
        /// </summary>
        [Name("目标丢失")]
        TargetLost,

        /// <summary>
        /// 目标加载
        /// </summary>
        [Name("目标加载")]
        TargetLoad,

        /// <summary>
        /// 目标卸载
        /// </summary>
        [Name("目标卸载")]
        TargetUnload,
    }

    /// <summary>
    /// 图片目标脚本事件函数
    /// </summary>
    [Name("图片目标脚本事件函数")]
    [Serializable]
    public class ImageTargetScriptEventFunction : EnumFunction<EImageTargetScriptEventType> { }

    /// <summary>
    /// 图片目标脚本事件函数集合
    /// </summary>
    [Name("图片目标脚本事件函数集合")]
    [Serializable]
    public class ImageTargetScriptEventFunctionCollection : EnumFunctionCollection<EImageTargetScriptEventType, ImageTargetScriptEventFunction> { }
}
