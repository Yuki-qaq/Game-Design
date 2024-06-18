using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginCamera;
using XCSJ.Attributes;
using static XCSJ.PluginCameras.Tools.Controllers.CameraControllerEvent;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.PluginCameras.Tools.Base;
using UnityEngine;

namespace XCSJ.EditorCameras
{
    /// <summary>
    /// 相机管理器检查器
    /// </summary>
    [CustomEditor(typeof(CameraManager))]
    [Name("相机管理器检查器")]
    public class CameraManagerInspector : BaseManagerInspector<CameraManager>
    {
        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(CameraManager));
            var targetObject = this.targetObject;
            if (targetObject)
            {
                if (!targetObject.cameraManagerProvider) { }
            }
        }

        /// <summary>
        /// 当绘制脚本
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawScript(SerializedProperty serializedProperty)
        {
            base.OnDrawScript(serializedProperty);
            categoryList.DrawVertical();
        }
    }

    /// <summary>
    /// 相机控制器回调事件绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(CameraControllerCallbackEvent))]
    public class CameraControllerCallbackEventDrawer : EnumUnityEventDrawer<ECameraControllerEvent>
    {
        /// <summary>
        /// 当绘制枚举
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected override void OnDrawEnum(Rect position, SerializedProperty property, GUIContent label)
        {
            var esp = property.FindPropertyRelative(nameof(CameraControllerCallbackEvent._cameraControllerEvent));
            EditorGUI.BeginChangeCheck();            
            var eValue = UICommonFun.EnumPopup(position, PropertyData.GetPropertyData(esp).trLabel, (ECameraControllerEvent)esp.intValue);
            if (EditorGUI.EndChangeCheck())
            {
                esp.intValue = (int)(ECameraControllerEvent)eValue;
            }
        }
    }
}
