using UnityEngine;
using System.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginEasyAR;
using UnityEditor;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
#if XDREAMER_EASYAR_4_1_0
using easyar;
#endif

namespace XCSJ.EditorEasyAR
{
    /// <summary>
    /// 图片目标检查器: 本类对应的检查器必须使用EasyAR默认的检查器进行渲染，因为有些私有的成员需要通过该界面进行信息填充；
    /// </summary>
    [Name("图片目标检查器")]
    [CustomEditor(typeof(ImageTargetMB))]
#if XDREAMER_EASYAR_4_1_0 
    public class ImageTargetMBInspector : ImageTargetControllerEditor
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        public ImageTargetMB targetObject { get { return target as ImageTargetMB; } }

        /// <summary>
        /// 图片目标脚本事件
        /// </summary>
        public ImageTargetScriptEvent imageTargetScriptEvent;

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!EditorGUILayout.Toggle(CommonFun.NameTooltip(typeof(ImageTargetScriptEvent)), (imageTargetScriptEvent && imageTargetScriptEvent.enabled)))
            {
                if (imageTargetScriptEvent) imageTargetScriptEvent.enabled = false;
            }
            else
            {
                if (!imageTargetScriptEvent)
                {
                    imageTargetScriptEvent = targetObject.gameObject.AddComponent<ImageTargetScriptEvent>();
                }
                else
                {
                    imageTargetScriptEvent.enabled = true;
                }
            }
            if (Application.isPlaying) return;
            if(targetObject.marker == null)
            {
                targetObject.ImageFileSource.PathType = PathType.Absolute;
                targetObject.ImageFileSource.Path = "";
                targetObject.ImageFileSource.Name = "";
            }
            else
            {
                targetObject.ImageFileSource.PathType = PathType.Absolute;
                targetObject.ImageFileSource.Path = AssetDatabase.GetAssetPath(targetObject.marker);
                targetObject.ImageFileSource.Name = targetObject.marker.name;
            }
        }

        /// <summary>
        /// 当启用
        /// </summary>
        public new void OnEnable()
        {
            base.OnEnable();
            if (!imageTargetScriptEvent)
            {
                imageTargetScriptEvent = targetObject.GetComponent<ImageTargetScriptEvent>();
            }
        }
    }

#else 
    public class ImageTargetMBInspector : MBInspector<ImageTargetMB>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("test"))
            {
                mb.XGetOrAddComponent<ImageTargetScriptEvent>();
            }
        }
    }
#endif
}
